using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Admin
{
    public class HistoryReprocessor
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        // Ensure this matches your DB ID for "WfmApproved"
        private const int Status_WfmApproved = 3;

        // Hardcoded Types for logic (Match your AcrTypeKey enum or DB IDs)
        private const int Type_NewHire = 1;
        private const int Type_Rehire = 2;
        private const int Type_Separation = 3;
        // ... add others if needed for specific logic

        public HistoryReprocessor(IDbContextFactory<AomDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<string> ReconcileAllAsync(CancellationToken ct = default)
        {
            using var db = await _factory.CreateDbContextAsync(ct);
            int added = 0, updated = 0, deleted = 0;

            // 1. Fetch Source of Truth (All Approved ACRs)
            var acrs = await db.AcrRequests
                .AsNoTracking()
                .Where(x => x.AcrStatusId == Status_WfmApproved)
                // UPDATED: Use Singular properties for 1-to-1 relations
                .Include(x => x.AcrOrganization)
                .Include(x => x.AcrOvertimeSchedule)
                // KEEP: Plural for Schedules (1-to-Many for split shifts)
                .Include(x => x.AcrSchedules)
                .OrderBy(x => x.EffectiveDate)
                .ThenBy(x => x.SubmitTime)
                .ThenBy(x => x.Id)
                .ToListAsync(ct);

            // 2. Fetch Existing History (To compare)
            // Loading all into memory. If >50k rows, consider doing this per-employee.
            var existingHistory = await db.EmployeeHistory.ToListAsync(ct);

            var acrGroups = acrs.GroupBy(x => x.EmployeeId);

            foreach (var group in acrGroups)
            {
                var empId = group.Key;

                // === A. Build Desired State ===
                var desiredSnapshots = new List<EmployeeHistory>();
                var state = new EmployeeState { IsActive = true };

                // Group by EffectiveDate to merge same-day changes
                var dayBatches = group.GroupBy(x => x.EffectiveDate).OrderBy(x => x.Key);

                foreach (var batch in dayBatches)
                {
                    // Replay all ACRs for this specific day
                    foreach (var acr in batch)
                    {
                        ApplyAcrToState(state, acr);
                    }

                    // Create the Snapshot for this day
                    desiredSnapshots.Add(new EmployeeHistory
                    {
                        EmployeeId = empId,
                        EffectiveDate = batch.Key.ToDateTime(TimeOnly.MinValue),
                        IsActive = state.IsActive,
                        IsLoa = state.IsLoa,
                        IsIntLoa = state.IsIntLoa,
                        IsRemote = state.IsRemote,
                        EmployerId = state.EmployerId,
                        SiteId = state.SiteId,
                        OrganizationId = state.OrganizationId,
                        SubOrganizationId = state.SubOrganizationId,
                        ManagerId = state.ManagerId,
                        SupervisorId = state.SupervisorId,
                        ScheduleRequestId = state.ScheduleRequestId,
                        OvertimeRequestId = state.OvertimeRequestId,
                        SourceAcrId = batch.Last().Id // Point to the last ACR of the day
                    });
                }

                // === B. Reconcile with DB ===
                var currentHistory = existingHistory
                    .Where(h => h.EmployeeId == empId)
                    .ToList();

                foreach (var desired in desiredSnapshots)
                {
                    // Find rows in DB that match this Date
                    var matches = currentHistory
                        .Where(h => h.EffectiveDate == desired.EffectiveDate)
                        .OrderByDescending(h => h.Id) // Keep the newest ID
                        .ToList();

                    if (matches.Count == 0)
                    {
                        // Missing? Add it.
                        db.EmployeeHistory.Add(desired);
                        added++;
                    }
                    else
                    {
                        // Found? Update the 'Keeper' (first one)
                        var keeper = matches.First();

                        bool changed = false;
                        if (keeper.IsActive != desired.IsActive) { keeper.IsActive = desired.IsActive; changed = true; }
                        if (keeper.IsLoa != desired.IsLoa) { keeper.IsLoa = desired.IsLoa; changed = true; }
                        if (keeper.IsIntLoa != desired.IsIntLoa) { keeper.IsIntLoa = desired.IsIntLoa; changed = true; }
                        if (keeper.IsRemote != desired.IsRemote) { keeper.IsRemote = desired.IsRemote; changed = true; }
                        if (keeper.EmployerId != desired.EmployerId) { keeper.EmployerId = desired.EmployerId; changed = true; }
                        if (keeper.OrganizationId != desired.OrganizationId) { keeper.OrganizationId = desired.OrganizationId; changed = true; }
                        if (keeper.SubOrganizationId != desired.SubOrganizationId) { keeper.SubOrganizationId = desired.SubOrganizationId; changed = true; }
                        if (keeper.SiteId != desired.SiteId) { keeper.SiteId = desired.SiteId; changed = true; }
                        if (keeper.ManagerId != desired.ManagerId) { keeper.ManagerId = desired.ManagerId; changed = true; }
                        if (keeper.SupervisorId != desired.SupervisorId) { keeper.SupervisorId = desired.SupervisorId; changed = true; }
                        if (keeper.ScheduleRequestId != desired.ScheduleRequestId) { keeper.ScheduleRequestId = desired.ScheduleRequestId; changed = true; }
                        if (keeper.OvertimeRequestId != desired.OvertimeRequestId) { keeper.OvertimeRequestId = desired.OvertimeRequestId; changed = true; }
                        if (keeper.SourceAcrId != desired.SourceAcrId) { keeper.SourceAcrId = desired.SourceAcrId; changed = true; }

                        if (changed) updated++;

                        // Handle Duplicates (Delete extras)
                        if (matches.Count > 1)
                        {
                            var duplicates = matches.Skip(1).ToList();
                            db.EmployeeHistory.RemoveRange(duplicates);
                            deleted += duplicates.Count;
                        }
                    }
                }
            }

            await db.SaveChangesAsync(ct);
            return $"Done. Added: {added}, Updated: {updated}, Deleted Duplicates: {deleted}";
        }

        private void ApplyAcrToState(EmployeeState state, AcrRequest acr)
        {
            // 1. Status Changes
            if (acr.AcrTypeId == Type_Separation) state.IsActive = false;
            else if (acr.AcrTypeId == Type_Rehire || acr.AcrTypeId == Type_NewHire) state.IsActive = true;

            // 2. Organization Changes (UPDATED: Check Singular)
            if (acr.AcrOrganization != null)
            {
                var o = acr.AcrOrganization;
                state.OrganizationId = o.OrganizationId;
                state.SubOrganizationId = o.SubOrganizationId;
                state.SiteId = o.SiteId;
                state.EmployerId = o.EmployerId;
                state.ManagerId = o.ManagerId;
                state.SupervisorId = o.SupervisorId;
                state.IsLoa = o.IsLoa;
                state.IsIntLoa = o.IsIntLoa;
                state.IsRemote = o.IsRemote;
            }

            // 3. Schedule Pointer (KEEP: Check Collection)
            if (acr.AcrSchedules.Any())
            {
                state.ScheduleRequestId = acr.Id;
            }

            // 4. Overtime Pointer (UPDATED: Check Singular)
            if (acr.AcrOvertimeSchedule != null)
            {
                state.OvertimeRequestId = acr.Id;
            }
        }

        private class EmployeeState
        {
            public bool IsActive { get; set; }
            public bool? IsLoa { get; set; }
            public bool? IsIntLoa { get; set; }
            public bool? IsRemote { get; set; }
            public int? EmployerId { get; set; }
            public int? SiteId { get; set; }
            public int? OrganizationId { get; set; }
            public int? SubOrganizationId { get; set; }
            public int? ManagerId { get; set; }
            public int? SupervisorId { get; set; }
            public int? ScheduleRequestId { get; set; }
            public int? OvertimeRequestId { get; set; }
        }
    }
}