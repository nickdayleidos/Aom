using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;

namespace MyApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationsController : ControllerBase
    {
        private readonly IDbContextFactory<AomDbContext> _contextFactory;
        // Hardcoded path as requested
        private const string BasePath = @"\\nmcitod\Web\TODData\EmployeeCertificates\";

        public CertificationsController(IDbContextFactory<AomDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewCertification(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var cert = await ctx.Certifications.FindAsync(id);

            if (cert == null || string.IsNullOrEmpty(cert.FileName))
            {
                return NotFound("Certification record not found.");
            }

            var filePath = Path.Combine(BasePath, cert.FileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File not found on server: {cert.FileName}");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf");
        }
    }
}