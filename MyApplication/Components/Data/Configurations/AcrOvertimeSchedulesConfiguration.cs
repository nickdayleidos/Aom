using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Data.Configurations
{
    public class AcrOvertimeSchedulesConfiguration : IEntityTypeConfiguration<AcrOvertimeSchedules>
    {
        public void Configure(EntityTypeBuilder<AcrOvertimeSchedules> builder)
        {
            builder.ToTable("AcrOvertimeSchedules", "Employee").HasKey(x => x.Id);

            builder.HasOne(x => x.AcrRequest)
                 .WithMany()
                 .HasForeignKey(x => x.AcrRequestId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.AcrRequestId).IsUnique();

            builder.HasOne(x => x.MondayType).WithMany().HasForeignKey(x => x.MondayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_MondayType_OvertimeTypes");
            builder.HasOne(x => x.TuesdayType).WithMany().HasForeignKey(x => x.TuesdayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_TuesdayType_OvertimeTypes");
            builder.HasOne(x => x.WednesdayType).WithMany().HasForeignKey(x => x.WednesdayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_WednesdayType_OvertimeTypes");
            builder.HasOne(x => x.ThursdayType).WithMany().HasForeignKey(x => x.ThursdayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_ThursdayType_OvertimeTypes");
            builder.HasOne(x => x.FridayType).WithMany().HasForeignKey(x => x.FridayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_FridayType_OvertimeTypes");
            builder.HasOne(x => x.SaturdayType).WithMany().HasForeignKey(x => x.SaturdayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_SaturdayType_OvertimeTypes");
            builder.HasOne(x => x.SundayType).WithMany().HasForeignKey(x => x.SundayTypeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_AcrOvertimeSchedules_SundayType_OvertimeTypes");
        }
    }
}