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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置複合主鍵
        modelBuilder.Entity<TaipeiParkingStatus>()
            .HasKey(p => new { p.Id, p.UpdateTime });
    }
}

