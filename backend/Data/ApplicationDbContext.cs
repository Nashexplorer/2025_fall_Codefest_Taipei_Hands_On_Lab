using Microsoft.EntityFrameworkCore;
using GongCanApi.Models;

namespace GongCanApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaipeiParkingStatus> TaipeiParkingStatuses { get; set; }
    public DbSet<MealEvent> MealEvents { get; set; }
    public DbSet<MealEventParticipant> MealEventParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置複合主鍵
        modelBuilder.Entity<TaipeiParkingStatus>()
            .HasKey(p => new { p.Id, p.UpdateTime });

        // 配置 MealEventParticipant 的唯一約束（同一使用者不能重複預約同一活動）
        modelBuilder.Entity<MealEventParticipant>()
            .HasIndex(p => new { p.MealEventId, p.UserId })
            .IsUnique();
    }
}

