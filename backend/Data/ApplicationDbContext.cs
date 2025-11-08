using Microsoft.EntityFrameworkCore;
using GongCanApi.Models;

namespace GongCanApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MealEvent> MealEvents { get; set; }
    public DbSet<MealEventParticipant> MealEventParticipants { get; set; }
    public DbSet<SupportPoint> SupportPoints { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // 配置 MealEventParticipant 的唯一約束（同一使用者不能重複預約同一活動）
        modelBuilder.Entity<MealEventParticipant>()
            .HasIndex(p => new { p.MealEventId, p.UserId })
            .IsUnique();

        // 添加索引以優化查詢（用於 SumAsync 查詢參與人數）
        modelBuilder.Entity<MealEventParticipant>()
            .HasIndex(p => new { p.MealEventId, p.Status })
            .HasDatabaseName("IX_MealEventParticipants_MealEventId_Status");
    }
}

