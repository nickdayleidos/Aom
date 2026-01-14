using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Training.Certifications
{
    public class CertificationsRepository : ICertificationsRepository
    {
        private readonly IDbContextFactory<AomDbContext> _contextFactory;

        // Added BasePath for file operations (Delete)
        private const string BasePath = @"E:\Web\TODData\EmployeeCertificates\";

        public CertificationsRepository(IDbContextFactory<AomDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Certification>> GetAllCertificationsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Certifications
                .Include(c => c.Employee)
                .Include(c => c.CertificationType)
                .ThenInclude(ct => ct.Vendor)
                .OrderByDescending(c => c.UploadDate)
                .ToListAsync();
        }

        public async Task<Certification?> GetCertificationByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Certifications
                .Include(c => c.Employee)
                .Include(c => c.CertificationType)
                .ThenInclude(ct => ct.Vendor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CertificationType>> GetCertificationTypesAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.CertificationTypes
                .Include(t => t.Vendor)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Employees>> GetEmployeesAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<List<EmployeeCurrentDetails>> GetEmployeeCurrentDetailsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.EmployeeCurrentDetails.ToListAsync();
        }

        public async Task<List<Current8570ReportItem>> GetCurrent8570ReportAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();

            // 1. Get Active Employee details from the View
            // We need NewestHireDate from here
            var activeEmployeeDetails = await ctx.EmployeeCurrentDetails
                .Where(x => x.IsActive)
                .ToListAsync();

            var activeEmpIds = activeEmployeeDetails.Select(x => x.EmployeeId).ToList();

            if (!activeEmpIds.Any())
                return new List<Current8570ReportItem>();

            // 2. Fetch History to get SubOrganization info (for Requirements)
            var histories = await ctx.EmployeeHistory
                .Include(h => h.Employee)
                .Include(h => h.SubOrganization)
                .Where(h => activeEmpIds.Contains(h.EmployeeId))
                .ToListAsync();

            var employeeHistoryMap = histories
                .GroupBy(h => h.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(h => h.EffectiveDate).ThenByDescending(h => h.Id).First());

            // 3. Get All Certifications for these active employees
            var allCerts = await ctx.Certifications
                .Include(c => c.CertificationType)
                .Where(c => activeEmpIds.Contains(c.EmployeeId))
                .ToListAsync();

            // 4. Fetch necessary data to calculate Newest Hire Date (replicating SQL Function logic)
            // SQL Logic: 
            //   1. Max EffectiveDate from AcrRequest where Type IN (1, 6) AND IsActive = 1
            //   2. Fallback: Min EffectiveDate from EmployeeHistory

            var hireRequests = await ctx.AcrRequests
                .Where(r => activeEmpIds.Contains(r.EmployeeId) &&
                            (r.AcrTypeId == 1 || r.AcrTypeId == 6) &&
                            r.IsActive == true) // Fixed: Explicit comparison for nullable bool
                .Select(r => new { r.EmployeeId, r.EffectiveDate })
                .ToListAsync();

            var report = new List<Current8570ReportItem>();

            foreach (var detail in activeEmployeeDetails)
            {
                if (!employeeHistoryMap.TryGetValue(detail.EmployeeId, out var latestHistory))
                    continue;

                var subOrg = latestHistory.SubOrganization;

                string reqLevelString = "N/A";
                bool isIam = false;
                bool isIat = false;

                // Determine Requirement
                if (subOrg != null)
                {
                    if (subOrg.IamLevel.HasValue && subOrg.IamLevel.Value > 0)
                    {
                        isIam = true;
                        reqLevelString = $"IAM Level {subOrg.IamLevel.Value}";
                    }
                    else if (subOrg.IatLevel.HasValue && subOrg.IatLevel.Value > 0)
                    {
                        isIat = true;
                        reqLevelString = $"IAT Level {subOrg.IatLevel.Value}";
                    }
                }

                // Find Best Cert
                Certification? bestCert = null;
                var empCerts = allCerts.Where(c => c.EmployeeId == detail.EmployeeId);

                if (isIam)
                {
                    bestCert = empCerts
                        .Where(c => c.CertificationType != null && c.CertificationType.IamLevel.HasValue)
                        .OrderByDescending(c => c.CertificationType!.IamLevel)
                        .ThenByDescending(c => c.ExpirationDate)
                        .FirstOrDefault();
                }
                else if (isIat)
                {
                    bestCert = empCerts
                        .Where(c => c.CertificationType != null && c.CertificationType.IatLevel.HasValue)
                        .OrderByDescending(c => c.CertificationType!.IatLevel)
                        .ThenByDescending(c => c.ExpirationDate)
                        .FirstOrDefault();
                }

                // Determine Achieved Level String
                string achievedLevelString = "N/A";
                if (bestCert?.CertificationType != null)
                {
                    if (isIam && bestCert.CertificationType.IamLevel.HasValue)
                        achievedLevelString = $"IAM Level {bestCert.CertificationType.IamLevel.Value}";
                    else if (isIat && bestCert.CertificationType.IatLevel.HasValue)
                        achievedLevelString = $"IAT Level {bestCert.CertificationType.IatLevel.Value}";
                    else if (bestCert.CertificationType.IamLevel.HasValue)
                        achievedLevelString = $"IAM Level {bestCert.CertificationType.IamLevel.Value}";
                    else if (bestCert.CertificationType.IatLevel.HasValue)
                        achievedLevelString = $"IAT Level {bestCert.CertificationType.IatLevel.Value}";
                }

                // Calculate Hire Date manually
                // Use DateTime (non-nullable) initialized to MinValue to avoid null check issues with non-nullable sources
                DateTime calculatedHireDate = DateTime.MinValue;

                var empHireRequests = hireRequests
                    .Where(r => r.EmployeeId == detail.EmployeeId)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefault();

                if (empHireRequests != null)
                {
                    // FIX 1: Convert DateOnly (from EffectiveDate) to DateTime for assignment to calculatedHireDate
                    // Error 183 said Cannot convert DateOnly to DateTime.
                    // Assuming empHireRequests.EffectiveDate is DateOnly based on error.
                    calculatedHireDate = empHireRequests.EffectiveDate.ToDateTime(TimeOnly.MinValue);
                }
                else
                {
                    var empHistories = histories.Where(h => h.EmployeeId == detail.EmployeeId);
                    if (empHistories.Any())
                    {
                        // EffectiveDate is non-nullable DateTime, Min returns DateTime
                        calculatedHireDate = empHistories.Min(h => h.EffectiveDate);
                    }
                }

                // Fallback to View Property
                // If calculatedHireDate is still MinValue, it means logic 1 & 2 failed.
                // detail.NewestHireDate is DateTime? (nullable).
                if (calculatedHireDate == DateTime.MinValue && detail.NewestHireDate.HasValue)
                {
                    calculatedHireDate = detail.NewestHireDate.Value;
                }

                report.Add(new Current8570ReportItem
                {
                    EmployeeId = detail.EmployeeId,
                    CertificationId = bestCert?.Id,
                    EmployeeName = $"{detail.LastName}, {detail.FirstName} {detail.MiddleInitial}".Trim(),
                    SupervisorName = detail.SupervisorName ?? "N/A", // Added Supervisor Name
                    RequiredLevel = reqLevelString,
                    CertificationName = bestCert?.CertificationType?.ShortName ?? bestCert?.CertificationType?.Name ?? "N/A",
                    ExpirationDate = bestCert?.ExpirationDate,
                    SerialNumber = bestCert?.SerialNumber ?? "N/A",
                    AchievedLevel = achievedLevelString,
                    // FIX 2: Convert DateTime calculatedHireDate to DateOnly? for DTO HireDate
                    // Error 214 said Cannot convert DateTime to DateOnly?
                    // HireDate in DTO must be DateOnly?
                    HireDate = calculatedHireDate == DateTime.MinValue ? null : DateOnly.FromDateTime(calculatedHireDate)
                });
            }

            return report.OrderBy(r => r.EmployeeName).ToList();
        }

        // ... existing helper methods ...
        // ... (GetManagersAsync, GetSupervisorsAsync, etc. from user snippet)

        public async Task<List<Manager>> GetManagersAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Managers.Include(m => m.Employee).Where(m => m.IsActive == true).OrderBy(m => m.Employee.LastName).ThenBy(m => m.Employee.FirstName).ToListAsync();
        }
        public async Task<List<Supervisor>> GetSupervisorsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Supervisors.Include(s => s.Employee).Where(s => s.IsActive == true).OrderBy(s => s.Employee.LastName).ThenBy(s => s.Employee.FirstName).ToListAsync();
        }
        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Organizations.Where(o => o.IsActive == true).OrderBy(o => o.Name).ToListAsync();
        }
        public async Task<List<SubOrganization>> GetSubOrganizationsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.SubOrganizations.Where(s => s.IsActive == true).OrderBy(s => s.Name).ToListAsync();
        }
        public async Task AddCertificationAsync(Certification certification)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Certifications.Add(certification);
            await ctx.SaveChangesAsync();
        }
        public async Task UpdateCertificationAsync(Certification certification)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Certifications.Update(certification);
            await ctx.SaveChangesAsync();
        }

        // Updated Delete method with file deletion
        public async Task DeleteCertificationAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var cert = await ctx.Certifications.FindAsync(id);
            if (cert != null)
            {
                // Attempt to delete physical file
                if (!string.IsNullOrEmpty(cert.FileName))
                {
                    try
                    {
                        var filePath = Path.Combine(BasePath, cert.FileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    catch
                    {
                        // Ignore file deletion errors to ensure DB record is still deleted
                    }
                }

                ctx.Certifications.Remove(cert);
                await ctx.SaveChangesAsync();
            }
        }
    }
}