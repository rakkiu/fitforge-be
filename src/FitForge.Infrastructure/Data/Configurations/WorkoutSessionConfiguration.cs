using FitForge.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitForge.Infrastructure.Data.Configurations;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.ToTable("workout_sessions");

        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id).HasColumnName("id");

        builder.Property(ws => ws.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        builder.Property(ws => ws.DayOfWeek)
            .HasColumnName("day_of_week")
            .IsRequired();

        builder.Property(ws => ws.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(ws => ws.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ws => ws.Description)
            .HasColumnName("description");

        builder.Property(ws => ws.OrderIndex)
            .HasColumnName("order_index")
            .IsRequired();

        builder.Property(ws => ws.DurationMinutes)
            .HasColumnName("duration_minutes");

        builder.Property(ws => ws.CaloriesBurned)
            .HasColumnName("calories_burned");

        builder.Property(ws => ws.Completed)
            .HasColumnName("completed")
            .HasDefaultValue(false);

        builder.Property(ws => ws.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(ws => ws.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(ws => ws.PlanId);
        builder.HasIndex(ws => ws.OrderIndex);

        builder.HasOne(ws => ws.Plan)
            .WithMany(wp => wp.Sessions)
            .HasForeignKey(ws => ws.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ws => ws.ExerciseSets);
    }
}
