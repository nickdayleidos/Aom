using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Data;

public partial class AomDbContext
{
    partial void ConfigureToolsSchema(ModelBuilder b)
    {
        // ---- Table mappings ----
        b.Entity<EmailTemplates>().ToTable(nameof(EmailTemplates), "Tools").HasKey(x => x.Id);
        b.Entity<IntervalSummary>().ToTable(nameof(IntervalSummary), "Tools").HasKey(x => x.Id);
        b.Entity<OiCategory>().ToTable(nameof(OiCategory), "Tools").HasKey(x => x.Id);
        b.Entity<OiSeverity>().ToTable(nameof(OiSeverity), "Tools").HasKey(x => x.Id);
        b.Entity<OiEvent>().ToTable(nameof(OiEvent), "Tools").HasKey(x => x.Id);
        b.Entity<OiEventUpdate>().ToTable(nameof(OiEventUpdate), "Tools").HasKey(x => x.Id);
        b.Entity<OiStatus>().ToTable(nameof(OiStatus), "Tools").HasKey(x => x.Id);
        b.Entity<ProactiveAnnouncement>().ToTable(nameof(ProactiveAnnouncement), "Tools").HasKey(x => x.Id);
        b.Entity<OstPassdown>().ToTable(nameof(OstPassdown), "Tools").HasKey(x => x.Id);

        // ---- Relationships ----
        b.Entity<OstPassdown>(e =>
        {
            e.HasOne(x => x.NewEdl)
            .WithMany()
            .HasForeignKey(x => x.NewEdlId)
            .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.PrevEdl)
                .WithMany()
                .HasForeignKey(x => x.PrevEdlId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ReskillBy)
                .WithMany()
                .HasForeignKey(x => x.ReskillById)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ProactiveBy)
                .WithMany()
                .HasForeignKey(x => x.ProactiveById)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.HomeportBy)
                .WithMany()
                .HasForeignKey(x => x.HomeportById)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SharepointBy)
                .WithMany()
                .HasForeignKey(x => x.SharepointById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
