using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Data.Configurations
{
    public class EmployeeCurrentDetailsConfiguration : IEntityTypeConfiguration<EmployeeCurrentDetails>
    {
        public void Configure(EntityTypeBuilder<EmployeeCurrentDetails> builder)
        {
            builder
                .ToView("vw_EmployeeCurrentDetails", "Employee")
                .HasKey(x => x.EmployeeId);
        }
    }
}