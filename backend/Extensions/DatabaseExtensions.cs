using Microsoft.EntityFrameworkCore;
using GongCanApi.Data;

namespace GongCanApi.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// 配置資料庫連接，支援 Cloud SQL Unix socket 和一般 MySQL 連接
    /// </summary>
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 檢查是否使用 Cloud SQL Unix socket（優先）
        var cloudSqlConnectionName = configuration["CloudSQL:ConnectionName"];
        var useUnixSocket = !string.IsNullOrEmpty(cloudSqlConnectionName);

        if (useUnixSocket)
        {
            // Cloud SQL 使用 Unix socket 連接
            var unixSocketPath = $"/cloudsql/{cloudSqlConnectionName}";
            connectionString = connectionString?.Replace("Server=34.81.245.32", $"Server={unixSocketPath}")
                .Replace(";Port=3306;", ";")
                ?? throw new InvalidOperationException("Connection string is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useUnixSocket)
            {
                // Cloud SQL 使用 Unix socket，不需要指定 ServerVersion
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)), mySqlOptions =>
                {
                    // 啟用重試邏輯
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
            }
            else
            {
                // 一般 MySQL 連接
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
                {
                    // 啟用重試邏輯
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
            }
        });

        return services;
    }
}

