using FitForge.Core.Entities;
using FitForge.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitForge.Infrastructure.Data.Configurations;

public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.ToTable("workout_plans");

        builder.HasKey(wp => wp.Id);
        builder.Property(wp => wp.Id).HasColumnName("id");

        builder.Property(wp => wp.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(wp => wp.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(wp => wp.Description)
            .HasColumnName("description");

        builder.Property(wp => wp.PlanType)
            .HasColumnName("plan_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(wp => wp.DaysPerWeek)
            .HasColumnName("days_per_week")
            .IsRequired();

        builder.Property(wp => wp.TotalWeeks)
            .HasColumnName("total_weeks")
            .IsRequired();

        builder.Property(wp => wp.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(wp => wp.GeneratedBy)
            .HasColumnName("generated_by")
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(wp => wp.AiMetadata)
            .HasColumnName("ai_metadata")
            .HasColumnType("jsonb");

        builder.Property(wp => wp.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(wp => wp.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(wp => wp.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(wp => wp.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasIndex(wp => wp.UserId);
        builder.HasIndex(wp => wp.Status);

        builder.HasOne(wp => wp.User)
            .WithMany()
            .HasForeignKey(wp => wp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(wp => wp.Sessions);
    }
}
