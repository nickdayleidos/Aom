using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr
{
    public sealed class AcrService : IAcrService
    {
        private readonly AomDbContext _db;

        // Status constants
        private const int Submitted = 1;
        private const int ManagerApproved = 2;
        private const int WfmApproved = 3;
        private const int Cancelled = 4;
        private const int Rejected = 5;

        // Type names – must match seed data precisely
        private const string Type_OrganizationChange = "Organization Change";
        private const string Type_ScheduleChange = "Schedule change";
        private const string Type_NewHire = "New Hire";
        private const string Type_Separation = "Seperation"; // spelling per your seed/UI
        private const string Type_OrganizationAndSchedule = "Organization and Schedule change";

        private async Task<int?> ResolveManagerIdAsync(int? managerEmployeeId, CancellationToken ct)
        {
            if (managerEmployeeId is not int empId) return null;
            return await _db.Managers
                .Where(m => m.EmployeeId == empId && m.IsActive)
                .Select(m => (int?)m.Id)
                .FirstOrDefaultAsync(ct);
        }

        private async Task<int?> ResolveSupervisorIdAsync(int? supervisorEmployeeId, CancellationToken ct)
        {
            if (supervisorEmployeeId is not int empId) return null;
            return await _db.Supervisors
                .Where(s => s.EmployeeId == empId && s.IsActive)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync(ct);
        }


        public AcrService(AomDbContext db) => _db = db;

        // -------------------- CREATE: Organization Change --------------------
        public async Task<int> CreateOrganizationChangeAsync(OrganizationChangeDto dto, CancellationToken ct = default)
        {
            GuardEmployee(dto.EmployeeId);

            var mgrId = await ResolveManagerIdAsync(dto.ManagerId, ct);
            var supId = await ResolveSupervisorIdAsync(dto.SupervisorId, ct);
            var typeId = await GetTypeIdAsync(Type_OrganizationChange, ct);
            var acr = NewAcr(dto.EmployeeId, typeId, dto.EffectiveDate, dto.SubmitterComment);

            var org = new AcrOrganization
            {
                AcrRequest = acr,
                ManagerId = mgrId,         // <— Manager.Id
                SupervisorId = supId,         // <— Supervisor.Id
                OrganizationId = dto.OrganizationId,
                SubOrganizationId = dto.SubOrganizationId,
                EmployerId = dto.EmployerId,
                SiteId = dto.SiteId, // moved to AcrOrganization
                IsActive = dto.IsActive,
                IsLoa = dto.IsLoa,
                IsIntLoa = dto.IsIntLoa,
                IsRemote = dto.IsRemote
            };

            _db.Add(acr);
            _db.Add(org);
            await _db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Schedule Change --------------------
        public async Task<int> CreateScheduleChangeAsync(ScheduleChangeDto dto, CancellationToken ct = default)
        {
            GuardEmployee(dto.EmployeeId);

            var typeId = await GetTypeIdAsync(Type_ScheduleChange, ct);
            var acr = NewAcr(dto.EmployeeId, typeId, dto.EffectiveDate, dto.SubmitterComment);

            var shift1 = new AcrSchedule
            {
                AcrRequest = acr,
                IsSplitSchedule = dto.IsSplitSchedule,
                ShiftNumber = 1,
                MondayStart = dto.MondayStart,
                MondayEnd = dto.MondayEnd,
                TuesdayStart = dto.TuesdayStart,
                TuesdayEnd = dto.TuesdayEnd,
                WednesdayStart = dto.WednesdayStart,
                WednesdayEnd = dto.WednesdayEnd,
                ThursdayStart = dto.ThursdayStart,
                ThursdayEnd = dto.ThursdayEnd,
                FridayStart = dto.FridayStart,
                FridayEnd = dto.FridayEnd,
                SaturdayStart = dto.SaturdayStart,
                SaturdayEnd = dto.SaturdayEnd,
                SundayStart = dto.SundayStart,
                SundayEnd = dto.SundayEnd,
                BreakTime = dto.BreakTime,
                Breaks = dto.Breaks,
                LunchTime = dto.LunchTime
            };

            _db.Add(acr);
            _db.Add(shift1);

            if (dto.IsSplitSchedule)
            {
                var shift2 = new AcrSchedule
                {
                    AcrRequest = acr,
                    IsSplitSchedule = true,
                    ShiftNumber = 2,
                    MondayStart = dto.MondayStart2,
                    MondayEnd = dto.MondayEnd2,
                    TuesdayStart = dto.TuesdayStart2,
                    TuesdayEnd = dto.TuesdayEnd2,
                    WednesdayStart = dto.WednesdayStart2,
                    WednesdayEnd = dto.WednesdayEnd2,
                    ThursdayStart = dto.ThursdayStart2,
                    ThursdayEnd = dto.ThursdayEnd2,
                    FridayStart = dto.FridayStart2,
                    FridayEnd = dto.FridayEnd2,
                    SaturdayStart = dto.SaturdayStart2,
                    SaturdayEnd = dto.SaturdayEnd2,
                    SundayStart = dto.SundayStart2,
                    SundayEnd = dto.SundayEnd2,
                    BreakTime = dto.BreakTime,
                    Breaks = dto.Breaks,
                    LunchTime = dto.LunchTime
                };
                _db.Add(shift2);
            }

            await _db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Org + Schedule Change (combined) --------------------
        public async Task<int> CreateOrgScheduleAsync(OrganizationChangeDto orgDto, ScheduleChangeDto schDto, CancellationToken ct = default)
        {
            GuardEmployee(orgDto.EmployeeId);
            if (schDto.EmployeeId != orgDto.EmployeeId)
                throw new ArgumentException("Org and Schedule DTO EmployeeId must match for combined ACR.");

            

            var typeId = await GetTypeIdAsync(Type_OrganizationAndSchedule, ct);
            var effective = orgDto.EffectiveDate;
            var comment = !string.IsNullOrWhiteSpace(orgDto.SubmitterComment) ? orgDto.SubmitterComment : schDto.SubmitterComment;

            var acr = NewAcr(orgDto.EmployeeId, typeId, effective, comment);

            var org = new AcrOrganization
            {
                AcrRequest = acr,
                ManagerId = orgDto.ManagerId,
                SupervisorId = orgDto.SupervisorId,
                OrganizationId = orgDto.OrganizationId,
                SubOrganizationId = orgDto.SubOrganizationId,
                EmployerId = orgDto.EmployerId,
                SiteId = orgDto.SiteId,
                IsActive = orgDto.IsActive,
                IsLoa = orgDto.IsLoa,
                IsIntLoa = orgDto.IsIntLoa,
                IsRemote = orgDto.IsRemote
            };

            var shift1 = new AcrSchedule
            {
                AcrRequest = acr,
                IsSplitSchedule = schDto.IsSplitSchedule,
                ShiftNumber = 1,
                MondayStart = schDto.MondayStart,
                MondayEnd = schDto.MondayEnd,
                TuesdayStart = schDto.TuesdayStart,
                TuesdayEnd = schDto.TuesdayEnd,
                WednesdayStart = schDto.WednesdayStart,
                WednesdayEnd = schDto.WednesdayEnd,
                ThursdayStart = schDto.ThursdayStart,
                ThursdayEnd = schDto.ThursdayEnd,
                FridayStart = schDto.FridayStart,
                FridayEnd = schDto.FridayEnd,
                SaturdayStart = schDto.SaturdayStart,
                SaturdayEnd = schDto.SaturdayEnd,
                SundayStart = schDto.SundayStart,
                SundayEnd = schDto.SundayEnd,
                BreakTime = schDto.BreakTime,
                Breaks = schDto.Breaks,
                LunchTime = schDto.LunchTime
            };

            _db.Add(acr);
            _db.Add(org);
            _db.Add(shift1);

            if (schDto.IsSplitSchedule)
            {
                var shift2 = new AcrSchedule
                {
                    AcrRequest = acr,
                    IsSplitSchedule = true,
                    ShiftNumber = 2,
                    MondayStart = schDto.MondayStart2,
                    MondayEnd = schDto.MondayEnd2,
                    TuesdayStart = schDto.TuesdayStart2,
                    TuesdayEnd = schDto.TuesdayEnd2,
                    WednesdayStart = schDto.WednesdayStart2,
                    WednesdayEnd = schDto.WednesdayEnd2,
                    ThursdayStart = schDto.ThursdayStart2,
                    ThursdayEnd = schDto.ThursdayEnd2,
                    FridayStart = schDto.FridayStart2,
                    FridayEnd = schDto.FridayEnd2,
                    SaturdayStart = schDto.SaturdayStart2,
                    SaturdayEnd = schDto.SaturdayEnd2,
                    SundayStart = schDto.SundayStart2,
                    SundayEnd = schDto.SundayEnd2,
                    BreakTime = schDto.BreakTime,
                    Breaks = schDto.Breaks,
                    LunchTime = schDto.LunchTime
                };
                _db.Add(shift2);
            }

            await _db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: New Hire --------------------
        public async Task<int> CreateNewHireAsync(NewHireDto dto, CancellationToken ct = default)
        {
            var first = (dto.FirstName ?? "").Trim();
            var last = (dto.LastName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(first)) throw new ArgumentException("First name is required.", nameof(dto.FirstName));
            if (string.IsNullOrWhiteSpace(last)) throw new ArgumentException("Last name is required.", nameof(dto.LastName));

            var emp = new Employees
            {
                FirstName = first,
                LastName = last,
                MiddleInitial = string.IsNullOrWhiteSpace(dto.MiddleInitial) ? null : dto.MiddleInitial!.Trim(),
                NmciEmail = string.IsNullOrWhiteSpace(dto.NmciEmail) ? null : dto.NmciEmail!.Trim(),
                UsnOperatorId = string.IsNullOrWhiteSpace(dto.UsnOperatorId) ? null : dto.UsnOperatorId!.Trim(),
                UsnAdminId = string.IsNullOrWhiteSpace(dto.UsnAdminId) ? null : dto.UsnAdminId!.Trim(),
                FlankspeedEmail = string.IsNullOrWhiteSpace(dto.FlankspeedEmail) ? null : dto.FlankspeedEmail!.Trim(),
                CorporateEmail = string.IsNullOrWhiteSpace(dto.CorporateEmail) ? null : dto.CorporateEmail!.Trim(),
                CorporateId = string.IsNullOrWhiteSpace(dto.CorporateId) ? null : dto.CorporateId!.Trim(),
                DomainLoginName = string.IsNullOrWhiteSpace(dto.DomainLoginName) ? null : dto.DomainLoginName!.Trim(),
                IsActive = true
            };
            _db.Employees.Add(emp);
            await _db.SaveChangesAsync(ct); // emp.Id available

            var newHireTypeId = await GetTypeIdAsync(Type_NewHire, ct);
            var acr = NewAcr(emp.Id, newHireTypeId, dto.EffectiveDate, dto.SubmitterComment);

            _db.AcrRequests.Add(acr);
            await _db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Separation --------------------
        public async Task<int> CreateSeparationAsync(SeparationDto dto, CancellationToken ct = default)
        {
            GuardEmployee(dto.EmployeeId);

            var typeId = await GetTypeIdAsync(Type_Separation, ct);
            var acr = NewAcr(dto.EmployeeId, typeId, dto.EffectiveDate, dto.SubmitterComment);

            // If you want an org row to mark inactive on separation:
            var org = new AcrOrganization { AcrRequest = acr, IsActive = false };

            _db.Add(acr);
            _db.Add(org);
            await _db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- READ --------------------
        public async Task<AcrRequest?> GetAsync(int id, CancellationToken ct = default)
            => await _db.Set<AcrRequest>()
                        .Include(r => r.Employee)
                        .Include(r => r.AcrStatus)
                        .Include(r => r.AcrType)
                        .FirstOrDefaultAsync(r => r.Id == id, ct);

        // -------------------- UPDATE STATUS --------------------
        public async Task UpdateStatusAsync(int id, int newStatusId, CancellationToken ct = default)
        {
            var acr = await _db.Set<AcrRequest>().FirstOrDefaultAsync(r => r.Id == id, ct)
                      ?? throw new InvalidOperationException($"ACR {id} not found.");

            if (!IsValidTransition(acr.AcrStatusId, newStatusId))
                throw new InvalidOperationException($"Invalid status transition {acr.AcrStatusId} -> {newStatusId}");

            acr.AcrStatusId = newStatusId;
            acr.LastUpdateTime = Et.Now;   // <-- Eastern
            await _db.SaveChangesAsync(ct);
        }

        // -------------------- helpers --------------------
        private static void GuardEmployee(int employeeId)
        {
            if (employeeId <= 0) throw new ArgumentException("Employee is required.");
        }

        private AcrRequest NewAcr(int employeeId, int typeId, DateOnly effective, string? comment) => new()
        {
            EmployeeId = employeeId,
            AcrTypeId = typeId,
            AcrStatusId = Submitted,
            EffectiveDate = effective,
            SubmitterComment = comment,
            SubmitTime = Et.Now,        // <-- Eastern
            LastUpdateTime = Et.Now     // <-- Eastern
        };


        private async Task<int> GetTypeIdAsync(string typeName, CancellationToken ct)
            => await _db.Set<AcrType>()
                        .Where(t => t.Name == typeName)
                        .Select(t => t.Id)
                        .FirstAsync(ct);

        private static bool IsValidTransition(int from, int to)
        {
            if (from == to) return false;
            if (from is Cancelled or Rejected) return false;     // terminal
            if (to is Cancelled or Rejected) return true;      // allow cancel/reject from active
            if (from == Submitted && to == ManagerApproved) return true;
            if (from == ManagerApproved && to == WfmApproved) return true;
            return false;
        }
    }
}
