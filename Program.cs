using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using GongCanApi.Data;
using GongCanApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 支援 Cloud Run 的 PORT 環境變數
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置 MySQL 資料庫連接
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 檢查是否使用 Cloud SQL Unix socket（優先）
var cloudSqlConnectionName = builder.Configuration["CloudSQL:ConnectionName"];
var useUnixSocket = !string.IsNullOrEmpty(cloudSqlConnectionName);

if (useUnixSocket)
{
    // Cloud SQL 使用 Unix socket 連接
    var unixSocketPath = $"/cloudsql/{cloudSqlConnectionName}";
    connectionString = connectionString?.Replace("Server=34.81.245.32", $"Server={unixSocketPath}")
        .Replace(";Port=3306;", ";")
        ?? throw new InvalidOperationException("Connection string is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // 生產環境也啟用 Swagger（可選）
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cloud Run 使用 HTTPS，不需要在應用層重定向
// app.UseHttpsRedirection();

// ============================================
// Parking API - 停車場相關 API
// ============================================
var parkingApi = app.MapGroup("/api/parking")
    .WithTags("Parking API")
    .WithOpenApi();

// 1. 取得所有停車場狀態（分頁）
parkingApi.MapGet("/status", async (ApplicationDbContext db, int page = 1, int pageSize = 10) =>
{
    var totalCount = await db.TaipeiParkingStatuses.CountAsync();
    var items = await db.TaipeiParkingStatuses
        .OrderByDescending(p => p.UpdateTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(new
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Data = items
    });
})
.WithName("GetAllParkingStatuses")
.WithOpenApi()
.Produces<object>(StatusCodes.Status200OK);

// 2. 根據 ID 取得最新的停車場狀態
parkingApi.MapGet("/status/{id}", async (ApplicationDbContext db, string id) =>
{
    var status = await db.TaipeiParkingStatuses
        .Where(p => p.Id == id)
        .OrderByDescending(p => p.UpdateTime)
        .FirstOrDefaultAsync();

    return status is not null ? Results.Ok(status) : Results.NotFound(new { Message = $"找不到 ID 為 {id} 的停車場" });
})
.WithName("GetParkingStatusById")
.WithOpenApi()
.Produces<TaipeiParkingStatus>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// 3. 根據 ID 和更新時間取得特定停車場狀態
parkingApi.MapGet("/status/{id}/{updateTime:datetime}", async (ApplicationDbContext db, string id, DateTime updateTime) =>
{
    var status = await db.TaipeiParkingStatuses
        .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

    return status is not null ? Results.Ok(status) : Results.NotFound(new { Message = "找不到指定的停車場狀態記錄" });
})
.WithName("GetParkingStatusByIdAndTime")
.WithOpenApi()
.Produces<TaipeiParkingStatus>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// 4. 取得特定停車場的歷史記錄
parkingApi.MapGet("/status/{id}/history", async (ApplicationDbContext db, string id, int page = 1, int pageSize = 20) =>
{
    var totalCount = await db.TaipeiParkingStatuses.Where(p => p.Id == id).CountAsync();
    var items = await db.TaipeiParkingStatuses
        .Where(p => p.Id == id)
        .OrderByDescending(p => p.UpdateTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(new
    {
        TotalCount = totalCount,
        ParkingId = id,
        Page = page,
        PageSize = pageSize,
        Data = items
    });
})
.WithName("GetParkingStatusHistory")
.WithOpenApi()
.Produces<object>(StatusCodes.Status200OK);

// 5. 新增停車場狀態
parkingApi.MapPost("/status", async (ApplicationDbContext db, TaipeiParkingStatus status) =>
{
    // 檢查是否已存在相同的記錄
    var exists = await db.TaipeiParkingStatuses
        .AnyAsync(p => p.Id == status.Id && p.UpdateTime == status.UpdateTime);

    if (exists)
    {
        return Results.Conflict(new { Message = "該停車場狀態記錄已存在" });
    }

    db.TaipeiParkingStatuses.Add(status);
    await db.SaveChangesAsync();

    return Results.Created($"/api/parking/status/{status.Id}/{status.UpdateTime:O}", status);
})
.WithName("CreateParkingStatus")
.WithOpenApi()
.Produces<TaipeiParkingStatus>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status409Conflict);

// 6. 更新停車場狀態
parkingApi.MapPut("/status/{id}/{updateTime:datetime}", async (ApplicationDbContext db, string id, DateTime updateTime, TaipeiParkingStatus updatedStatus) =>
{
    var status = await db.TaipeiParkingStatuses
        .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

    if (status is null)
    {
        return Results.NotFound(new { Message = "找不到指定的停車場狀態記錄" });
    }

    // 更新可編輯的欄位
    status.AvailableCar = updatedStatus.AvailableCar;
    status.AvailableMotor = updatedStatus.AvailableMotor;
    status.AvailableBus = updatedStatus.AvailableBus;
    status.AvailablePregnancy = updatedStatus.AvailablePregnancy;
    status.AvailableHandicap = updatedStatus.AvailableHandicap;
    status.AvailableHeavyMotor = updatedStatus.AvailableHeavyMotor;
    status.ChargeTotal = updatedStatus.ChargeTotal;
    status.ChargeBusy = updatedStatus.ChargeBusy;
    status.ChargeIdle = updatedStatus.ChargeIdle;
    status.Reserved1 = updatedStatus.Reserved1;
    status.Reserved2 = updatedStatus.Reserved2;
    status.Reserved3 = updatedStatus.Reserved3;
    status.Reserved4 = updatedStatus.Reserved4;
    status.Reserved5 = updatedStatus.Reserved5;
    status.Reserved6 = updatedStatus.Reserved6;
    status.Reserved7 = updatedStatus.Reserved7;
    status.Reserved8 = updatedStatus.Reserved8;
    status.Reserved9 = updatedStatus.Reserved9;
    status.Reserved10 = updatedStatus.Reserved10;

    await db.SaveChangesAsync();

    return Results.Ok(status);
})
.WithName("UpdateParkingStatus")
.WithOpenApi()
.Produces<TaipeiParkingStatus>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// 7. 刪除停車場狀態
parkingApi.MapDelete("/status/{id}/{updateTime:datetime}", async (ApplicationDbContext db, string id, DateTime updateTime) =>
{
    var status = await db.TaipeiParkingStatuses
        .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

    if (status is null)
    {
        return Results.NotFound(new { Message = "找不到指定的停車場狀態記錄" });
    }

    db.TaipeiParkingStatuses.Remove(status);
    await db.SaveChangesAsync();

    return Results.Ok(new { Message = "刪除成功" });
})
.WithName("DeleteParkingStatus")
.WithOpenApi()
.Produces<object>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// 8. 取得所有不重複的停車場 ID
parkingApi.MapGet("/ids", async (ApplicationDbContext db) =>
{
    var ids = await db.TaipeiParkingStatuses
        .Select(p => p.Id)
        .Distinct()
        .OrderBy(id => id)
        .ToListAsync();

    return Results.Ok(ids);
})
.WithName("GetAllParkingIds")
.WithOpenApi()
.Produces<List<string>>(StatusCodes.Status200OK);

// ============================================
// GongCan API - 共餐活動相關 API
// ============================================
var gongCanApi = app.MapGroup("/api/gongcan")
    .WithTags("GongCan API")
    .WithOpenApi();

// 1. 取得所有共餐活動（分頁）
gongCanApi.MapGet("/meals", async (ApplicationDbContext db, int page = 1, int pageSize = 10) =>
{
    var totalCount = await db.MealActivities.CountAsync();
    var items = await db.MealActivities
        .OrderByDescending(m => m.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // 轉換 tags 字串為陣列
    var result = items.Select(m => new
    {
        m.Id,
        m.Title,
        m.Description,
        m.ImageUrl,
        m.Location,
        m.Latitude,
        m.Longitude,
        m.HostUserId,
        m.Capacity,
        m.CurrentParticipants,
        m.DietType,
        m.IsDineIn,
        m.StartTime,
        m.EndTime,
        m.SignupDeadline,
        m.CreatedAt,
        m.UpdatedAt,
        Tags = string.IsNullOrEmpty(m.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(m.Tags) ?? new List<string>(),
        m.Status,
        m.Price,
        m.Notes
    }).ToList();

    return Results.Ok(new
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Data = result
    });
})
.WithName("GetAllMeals")
.WithOpenApi()
.Produces<object>(StatusCodes.Status200OK);

// 2. 根據 ID 取得共餐活動
gongCanApi.MapGet("/meals/{id}", async (ApplicationDbContext db, string id) =>
{
    var meal = await db.MealActivities
        .FirstOrDefaultAsync(m => m.Id == id);

    if (meal is null)
    {
        return Results.NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
    }

    // 轉換 tags 字串為陣列
    var result = new
    {
        meal.Id,
        meal.Title,
        meal.Description,
        meal.ImageUrl,
        meal.Location,
        meal.Latitude,
        meal.Longitude,
        meal.HostUserId,
        meal.Capacity,
        meal.CurrentParticipants,
        meal.DietType,
        meal.IsDineIn,
        meal.StartTime,
        meal.EndTime,
        meal.SignupDeadline,
        meal.CreatedAt,
        meal.UpdatedAt,
        Tags = string.IsNullOrEmpty(meal.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(meal.Tags) ?? new List<string>(),
        meal.Status,
        meal.Price,
        meal.Notes
    };

    return Results.Ok(result);
})
.WithName("GetMealById")
.WithOpenApi()
.Produces<object>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.Run();
