using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyApplication.Common;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr
{
    public sealed class AcrService : IAcrService
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;
        private readonly IHttpContextAccessor _http;

        public AcrService(IDbContextFactory<AomDbContext> dbFactory, IHttpContextAccessor http)
        {
            _dbFactory = dbFactory;
            _http = http;
        }

        // Status constants
        private const int Submitted = 1;
        private const int ManagerApproved = 2;
        private const int WfmApproved = 3;
        private const int Cancelled = 5;
        private const int Rejected = 6;

        // Type names – must match seed data precisely
        private const string Type_OrganizationChange = "Organization Change";
        private const string Type_ScheduleChange = "Schedule change";
        private const string Type_NewHire = "New Hire";
        private const string Type_Separation = "Seperation"; // spelling per your seed/UI
        private const string Type_OrganizationAndSchedule = "Organization and Schedule change";

        // ---------- helpers ----------
        private static async Task<int?> ResolveManagerIdAsync(AomDbContext db, int? managerEmployeeId, CancellationToken ct)
        {
            if (managerEmployeeId is not int empId) return null;
            return await db.Managers
                .Where(m => m.EmployeeId == empId && m.IsActive)
                .Select(m => (int?)m.Id)
                .FirstOrDefaultAsync(ct);
        }

        private static async Task<int?> ResolveSupervisorIdAsync(AomDbContext db, int? supervisorEmployeeId, CancellationToken ct)
        {
            if (supervisorEmployeeId is not int empId) return null;
            return await db.Supervisors
                .Where(s => s.EmployeeId == empId && s.IsActive)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync(ct);
        }

        private static void GuardEmployee(int employeeId)
        {
            if (employeeId <= 0) throw new ArgumentException("Employee is required.");
        }

        private string? GetActor() =>
            IdentityHelpers.GetSamAccount(_http.HttpContext?.User) is { Length: > 0 } sam ? sam : null;

        private AcrRequest NewAcr(int employeeId, int typeId, DateOnly effective, string? comment) => new()
        {
            EmployeeId = employeeId,
            AcrTypeId = typeId,
            AcrStatusId = Submitted,
            EffectiveDate = effective,
            SubmitterComment = comment,
            SubmitTime = Et.Now,
            LastUpdateTime = Et.Now,
            SubmittedBy = GetActor()
        };

        private static async Task<int> GetTypeIdAsync(AomDbContext db, string typeName, CancellationToken ct)
            => await db.Set<AcrType>()
                       .Where(t => t.Name == typeName)
                       .Select(t => t.Id)
                       .FirstAsync(ct);

        private static bool IsValidTransition(int from, int to)
        {
            if (from == to) return false;
            if (from is Cancelled or Rejected) return false; // terminal
            if (to is Cancelled or Rejected) return true;    // allow cancel/reject from active
            if (from == Submitted && to == ManagerApproved) return true;
            if (from == ManagerApproved && to == WfmApproved) return true;
            return false;
        }

        // -------------------- CREATE: Organization Change --------------------
        public async Task<int> CreateOrganizationChangeAsync(OrganizationChangeDto dto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            GuardEmployee(dto.EmployeeId);

            var mgrId = await ResolveManagerIdAsync(db, dto.ManagerId, ct);
            var supId = await ResolveSupervisorIdAsync(db, dto.SupervisorId, ct);
            var typeId = await GetTypeIdAsync(db, Type_OrganizationChange, ct);
            var acr = NewAcr(dto.EmployeeId, typeId, dto.EffectiveDate, dto.SubmitterComment);

            var org = new AcrOrganization
            {
                AcrRequest = acr,
                ManagerId = mgrId,                // Manager.Id
                SupervisorId = supId,             // Supervisor.Id
                OrganizationId = dto.OrganizationId,
                SubOrganizationId = dto.SubOrganizationId,
                EmployerId = dto.EmployerId,
                SiteId = dto.SiteId,
                IsActive = dto.IsActive,
                IsLoa = dto.IsLoa,
                IsIntLoa = dto.IsIntLoa,
                IsRemote = dto.IsRemote
            };

            db.Add(acr);
            db.Add(org);
            await db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Schedule Change --------------------
        public async Task<int> CreateScheduleChangeAsync(ScheduleChangeDto dto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            GuardEmployee(dto.EmployeeId);

            var typeId = await GetTypeIdAsync(db, Type_ScheduleChange, ct);
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

            db.Add(acr);
            db.Add(shift1);

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
                db.Add(shift2);
            }

            await db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Org + Schedule Change --------------------
        public async Task<int> CreateOrgScheduleAsync(OrganizationChangeDto orgDto, ScheduleChangeDto schDto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            GuardEmployee(orgDto.EmployeeId);
            if (schDto.EmployeeId != orgDto.EmployeeId)
                throw new ArgumentException("Org and Schedule DTO EmployeeId must match for combined ACR.");

            var typeId = await GetTypeIdAsync(db, Type_OrganizationAndSchedule, ct);
            var effective = orgDto.EffectiveDate;
            var comment = !string.IsNullOrWhiteSpace(orgDto.SubmitterComment) ? orgDto.SubmitterComment : schDto.SubmitterComment;

            var acr = NewAcr(orgDto.EmployeeId, typeId, effective, comment);

            var org = new AcrOrganization
            {
                AcrRequest = acr,
                ManagerId = await ResolveManagerIdAsync(db, orgDto.ManagerId, ct),
                SupervisorId = await ResolveSupervisorIdAsync(db, orgDto.SupervisorId, ct),
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

            db.Add(acr);
            db.Add(org);
            db.Add(shift1);

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
                db.Add(shift2);
            }

            await db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: New Hire --------------------
        public async Task<int> CreateNewHireAsync(NewHireDto dto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

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
                DomainLoginName = string.IsNullOrWhiteSpace(dto.DomainLoginName) ? null : dto.DomainLoginName!.Trim()
                // Do NOT set Employees.IsActive here
            };
            db.Employees.Add(emp);
            await db.SaveChangesAsync(ct); // emp.Id

            var typeId = await GetTypeIdAsync(db, Type_NewHire, ct);
            var acr = NewAcr(emp.Id, typeId, dto.EffectiveDate, dto.SubmitterComment);

            // Drive active status via AcrOrganization on this ACR
            var org = new AcrOrganization
            {
                AcrRequest = acr,
                IsActive = true
                // other org fields optional for new hire
            };

            db.AcrRequests.Add(acr);
            db.Add(org);
            await db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Separation --------------------
        public async Task<int> CreateSeparationAsync(SeparationDto dto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            GuardEmployee(dto.EmployeeId);

            var typeId = await GetTypeIdAsync(db, Type_Separation, ct);
            var acr = NewAcr(dto.EmployeeId, typeId, dto.EffectiveDate, dto.SubmitterComment);

            // Status via AcrOrganization
            var org = new AcrOrganization { AcrRequest = acr, IsActive = false };

            db.Add(acr);
            db.Add(org);
            await db.SaveChangesAsync(ct);
            return acr.Id;
        }

        // -------------------- CREATE: Rehire --------------------
        public async Task<int> CreateRehireAsync(RehireDto dto)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var empExists = await db.Set<Employees>().AnyAsync(e => e.Id == dto.EmployeeId);
            if (!empExists) throw new InvalidOperationException($"Employee {dto.EmployeeId} not found.");

            // Resolve type "Rehire" (use your seeded value/name)
            var rehireTypeId = await db.Set<AcrType>()
                .Where(t => t.Name != null && EF.Functions.Like(t.Name, "%rehire%"))
                .Select(t => (int?)t.Id)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("ACR Type 'Rehire' not found.");

            var acr = NewAcr(dto.EmployeeId, rehireTypeId, dto.EffectiveDate, dto.SubmitterComment);

            // Flip status here, not in Employees table
            var org = new AcrOrganization
            {
                AcrRequest = acr,
                IsActive = true
            };

            db.Add(acr);
            db.Add(org);
            await db.SaveChangesAsync();
            return acr.Id;
        }

        // -------------------- READ --------------------
        public async Task<AcrRequest?> GetAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Set<AcrRequest>()
                           .Include(r => r.Employee)
                           .Include(r => r.AcrStatus)
                           .Include(r => r.AcrType)
                           .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        // -------------------- UPDATE STATUS --------------------
        public async Task UpdateStatusAsync(int id, int newStatusId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var acr = await db.Set<AcrRequest>().FirstOrDefaultAsync(r => r.Id == id, ct)
                      ?? throw new InvalidOperationException($"ACR {id} not found.");

            if (!IsValidTransition(acr.AcrStatusId, newStatusId))
                throw new InvalidOperationException($"Invalid status transition {acr.AcrStatusId} -> {newStatusId}");

            // Stamp actor field for this transition; reuse LastUpdateTime as the event time
            var actor = GetActor();
            switch (newStatusId)
            {
                case Cancelled:
                    acr.CancelledBy = actor;
                    break;
                case Rejected:
                    acr.RejectedBy = actor;
                    break;
                case ManagerApproved:
                    acr.ManagerApprovedBy = actor;
                    break;
                case WfmApproved:
                    acr.WfmApprovedBy = actor;
                    break;
            }

            acr.AcrStatusId = newStatusId;
            acr.LastUpdateTime = Et.Now;

            await db.SaveChangesAsync(ct);
        }
    }
}
