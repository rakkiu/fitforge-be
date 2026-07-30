using FitForge.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitForge.Infrastructure.Data.Configurations;

public class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
{
    public void Configure(EntityTypeBuilder<ExerciseSet> builder)
    {
        builder.ToTable("exercise_sets");

        builder.HasKey(es => es.Id);
        builder.Property(es => es.Id).HasColumnName("id");

        builder.Property(es => es.WorkoutId)
            .HasColumnName("workout_id")
            .IsRequired();

        builder.Property(es => es.ExerciseId)
            .HasColumnName("exercise_id")
            .IsRequired();

        builder.Property(es => es.SetNumber)
            .HasColumnName("set_number")
            .IsRequired();

        builder.Property(es => es.Reps)
            .HasColumnName("reps");

        builder.Property(es => es.WeightKg)
            .HasColumnName("weight_kg")
            .HasColumnType("decimal(10,2)");

        builder.Property(es => es.Completed)
            .HasColumnName("completed")
            .HasDefaultValue(false);

        builder.Property(es => es.Notes)
            .HasColumnName("notes");

        builder.Property(es => es.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(es => es.WorkoutId);
        builder.HasIndex(es => es.ExerciseId);

        builder.HasOne(es => es.Workout)
            .WithMany(ws => ws.ExerciseSets)
            .HasForeignKey(es => es.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(es => es.Exercise)
            .WithMany()
            .HasForeignKey(es => es.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
