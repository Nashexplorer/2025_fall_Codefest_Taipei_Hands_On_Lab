using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GongCanApi.Data;
using GongCanApi.Models;
using System.Text.Json;

namespace GongCanApi.Controllers;

[ApiController]
[Route("api/gongcan")]
[Tags("GongCan API")]
public class GongCanController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GongCanController(ApplicationDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// 生成唯一的共餐活動 ID
    /// 格式：Event{yyyyMMddHHmmss}{4位隨機數}
    /// 例如：Event202502151830001234
    /// </summary>
    private async Task<string> GenerateMealEventIdAsync()
    {
        var random = new Random();
        string newId;
        bool exists;

        do
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomSuffix = random.Next(1000, 9999).ToString(); // 4位隨機數
            newId = $"Event{timestamp}{randomSuffix}";

            exists = await _db.MealEvents.AnyAsync(m => m.Id == newId);
        } while (exists); // 如果已存在，重新生成

        return newId;
    }

    /// <summary>
    /// 組合完整地址
    /// </summary>
    private string BuildFullAddress(string? city, string? district, string? street, string? number)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(city))
            parts.Add(city);

        if (!string.IsNullOrWhiteSpace(district))
            parts.Add(district);

        if (!string.IsNullOrWhiteSpace(street))
            parts.Add(street);

        if (!string.IsNullOrWhiteSpace(number))
            parts.Add(number);

        return string.Join("", parts);
    }

    /// <summary>
    /// 根據地址計算經緯度（使用 Google Maps Geocoding API）
    /// </summary>
    private async Task<(decimal? latitude, decimal? longitude)> GeocodeAddressAsync(string address)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrWhiteSpace(address))
        {
            return (null, null);
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}&language=zh-TW&region=tw";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(json);

                if (result.TryGetProperty("status", out var status) && status.GetString() == "OK")
                {
                    if (result.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                    {
                        var firstResult = results[0];
                        if (firstResult.TryGetProperty("geometry", out var geometry))
                        {
                            if (geometry.TryGetProperty("location", out var location))
                            {
                                if (location.TryGetProperty("lat", out var lat) && location.TryGetProperty("lng", out var lng))
                                {
                                    return ((decimal)lat.GetDouble(), (decimal)lng.GetDouble());
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // 如果地理編碼失敗，返回 null
        }

        return (null, null);
    }

    // ============================================
    // 查詢相關 API
    // ============================================

    /// <summary>
    /// 取得所有共餐活動（分頁）
    /// </summary>
    [HttpGet("meals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMeals([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var totalCount = await _db.MealEvents.CountAsync();
        var items = await _db.MealEvents
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = items.Select(m => new
        {
            m.Id,
            m.Title,
            m.Description,
            m.ImageUrl,
            m.Latitude,
            m.Longitude,
            m.FullAddress,
            m.City,
            m.District,
            m.Street,
            m.Number,
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
            m.Status,
            m.Notes,
            m.Phone,
            m.Email
        }).ToList();

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = result
        });
    }

    /// <summary>
    /// 根據 ID 取得共餐活動
    /// </summary>
    [HttpGet("meals/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMealById(string id)
    {
        var meal = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meal is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        var result = new
        {
            meal.Id,
            meal.Title,
            meal.Description,
            meal.ImageUrl,
            meal.Latitude,
            meal.Longitude,
            meal.FullAddress,
            meal.City,
            meal.District,
            meal.Street,
            meal.Number,
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
            meal.Status,
            meal.Notes,
            meal.Phone,
            meal.Email
        };

        return Ok(result);
    }

    // ============================================
    // 商家相關 API
    // ============================================

    /// <summary>
    /// 商家上架共餐活動
    /// </summary>
    [HttpPost("meals")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMealEvent([FromBody] MealEvent mealEvent)
    {
        // 系統自動生成唯一 ID
        mealEvent.Id = await GenerateMealEventIdAsync();

        // 設定預設值
        mealEvent.CreatedAt ??= DateTime.UtcNow;
        mealEvent.UpdatedAt ??= DateTime.UtcNow;

        // status 不可超過 20 字元，預設 "open"
        mealEvent.Status = "open";

        // 後端自動組合 full_address
        mealEvent.FullAddress = BuildFullAddress(mealEvent.City, mealEvent.District, mealEvent.Street, mealEvent.Number);

        // 後端自動計算經緯度
        if (!string.IsNullOrWhiteSpace(mealEvent.FullAddress))
        {
            var (latitude, longitude) = await GeocodeAddressAsync(mealEvent.FullAddress);
            if (latitude.HasValue && longitude.HasValue)
            {
                mealEvent.Latitude = latitude.Value;
                mealEvent.Longitude = longitude.Value;
            }
        }

        _db.MealEvents.Add(mealEvent);
        await _db.SaveChangesAsync();

        // 回傳結果
        var result = new
        {
            mealEvent.Id,
            mealEvent.Title,
            mealEvent.Description,
            mealEvent.ImageUrl,
            mealEvent.Latitude,
            mealEvent.Longitude,
            mealEvent.FullAddress,
            mealEvent.City,
            mealEvent.District,
            mealEvent.Street,
            mealEvent.Number,
            mealEvent.HostUserId,
            mealEvent.Capacity,
            mealEvent.CurrentParticipants,
            mealEvent.DietType,
            mealEvent.IsDineIn,
            mealEvent.StartTime,
            mealEvent.EndTime,
            mealEvent.SignupDeadline,
            mealEvent.CreatedAt,
            mealEvent.UpdatedAt,
            mealEvent.Status,
            mealEvent.Notes,
            mealEvent.Phone,
            mealEvent.Email
        };

        return Created($"/api/gongcan/meals/{mealEvent.Id}", result);
    }

    /// <summary>
    /// 商家刪除共餐活動（將所有參與者狀態改為 cancelled，活動狀態改為 cancelled）
    /// </summary>
    [HttpDelete("meals/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMealEvent(string id)
    {
        var mealEvent = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mealEvent is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        // 將所有參與者狀態改為 cancelled
        var participants = await _db.MealEventParticipants
            .Where(p => p.MealEventId == id && p.Status == "confirmed")
            .ToListAsync();

        foreach (var participant in participants)
        {
            participant.Status = "cancelled";
            participant.UpdatedAt = DateTime.UtcNow;
        }

        // 將活動狀態改為 cancelled
        mealEvent.Status = "cancelled";
        mealEvent.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { Message = "活動已取消，所有參與者狀態已更新為 cancelled" });
    }

    // ============================================
    // 使用者相關 API
    // ============================================

    /// <summary>
    /// 申請參加共餐活動（預約）
    /// </summary>
    [HttpPost("meals/{id}/participate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ParticipateMealEvent(string id, [FromBody] ParticipateRequest request)
    {
        var mealEvent = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mealEvent is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        // 檢查活動狀態
        if (mealEvent.Status != "open")
        {
            return BadRequest(new { Message = $"活動狀態為 {mealEvent.Status}，無法參加" });
        }

        // 檢查報名截止時間
        if (mealEvent.SignupDeadline.HasValue && mealEvent.SignupDeadline.Value < DateTime.UtcNow)
        {
            return BadRequest(new { Message = "報名已截止" });
        }

        // 檢查參與人數是否有效
        var participantCount = request.ParticipantCount > 0 ? request.ParticipantCount : 1;

        // 計算目前已確認的總參與人數（加總所有參與者的 participantCount）
        var currentTotalParticipants = await _db.MealEventParticipants
            .Where(p => p.MealEventId == id && p.Status == "confirmed")
            .SumAsync(p => p.ParticipantCount);

        // 檢查是否已額滿（考慮新增的人數）
        if (mealEvent.Capacity > 0 && (currentTotalParticipants + participantCount) > mealEvent.Capacity)
        {
            return BadRequest(new { Message = $"活動剩餘名額不足，目前剩餘 {mealEvent.Capacity - currentTotalParticipants} 個名額" });
        }

        // 檢查使用者是否已經預約過（避免重複預約）
        var existingParticipant = await _db.MealEventParticipants
            .FirstOrDefaultAsync(p => p.MealEventId == id && p.UserId == request.UserId);

        if (existingParticipant != null)
        {
            if (existingParticipant.Status == "confirmed")
            {
                return Conflict(new { Message = "您已經預約過此活動" });
            }
            else if (existingParticipant.Status == "cancelled")
            {
                // 如果之前取消過，重新啟用預約
                existingParticipant.Status = "confirmed";
                existingParticipant.Phone = request.Phone ?? "0912345678";
                existingParticipant.Email = request.Email ?? "chishian.yang@gmail.com";
                existingParticipant.ParticipantCount = participantCount;
                existingParticipant.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // 創建新的參與者記錄
            var participant = new MealEventParticipant
            {
                Id = Guid.NewGuid().ToString(),
                MealEventId = id,
                UserId = request.UserId,
                Phone = request.Phone ?? "0912345678",
                Email = request.Email ?? "chishian.yang@gmail.com",
                ParticipantCount = participantCount,
                Status = "confirmed",
                CreatedAt = DateTime.UtcNow
            };
            _db.MealEventParticipants.Add(participant);
        }

        // 先保存變更，確保新參與者記錄已寫入資料庫
        mealEvent.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // 更新活動的參與人數（加總所有已確認參與者的 participantCount）
        var newTotalParticipants = await _db.MealEventParticipants
            .Where(p => p.MealEventId == id && p.Status == "confirmed")
            .SumAsync(p => p.ParticipantCount);
        mealEvent.CurrentParticipants = newTotalParticipants;

        // 如果已額滿，更新狀態
        if (mealEvent.Capacity > 0 && mealEvent.CurrentParticipants >= mealEvent.Capacity)
        {
            mealEvent.Status = "full";
        }

        mealEvent.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // 取得參與者記錄
        var participantRecord = await _db.MealEventParticipants
            .FirstOrDefaultAsync(p => p.MealEventId == id && p.UserId == request.UserId);

        return Created($"/api/gongcan/reservations/{participantRecord?.Id}", new
        {
            Message = "預約成功",
            ReservationId = participantRecord?.Id,
            MealEventId = mealEvent.Id,
            UserId = request.UserId,
            Phone = participantRecord?.Phone,
            Email = participantRecord?.Email,
            ParticipantCount = participantRecord?.ParticipantCount ?? 1,
            CurrentParticipants = mealEvent.CurrentParticipants,
            Status = mealEvent.Status
        });
    }

    /// <summary>
    /// 取消預約共餐活動
    /// </summary>
    [HttpDelete("meals/{id}/participate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelParticipation(string id, [FromQuery] string userId)
    {
        var mealEvent = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mealEvent is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        // 檢查使用者是否有預約記錄
        var participant = await _db.MealEventParticipants
            .FirstOrDefaultAsync(p => p.MealEventId == id && p.UserId == userId);

        if (participant is null)
        {
            return NotFound(new { Message = "您沒有預約此活動" });
        }

        if (participant.Status == "cancelled")
        {
            return BadRequest(new { Message = "此預約已經取消過了" });
        }

        // 更新參與者狀態為 cancelled
        participant.Status = "cancelled";
        participant.UpdatedAt = DateTime.UtcNow;

        // 先保存變更
        await _db.SaveChangesAsync();

        // 更新活動的參與人數（加總所有已確認參與者的 participantCount）
        var confirmedTotalParticipants = await _db.MealEventParticipants
            .Where(p => p.MealEventId == id && p.Status == "confirmed")
            .SumAsync(p => p.ParticipantCount);
        mealEvent.CurrentParticipants = confirmedTotalParticipants;

        // 如果原本是 full 狀態，改回 open
        if (mealEvent.Status == "full" && mealEvent.CurrentParticipants < mealEvent.Capacity)
        {
            mealEvent.Status = "open";
        }

        mealEvent.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new
        {
            Message = "取消預約成功",
            MealEventId = mealEvent.Id,
            UserId = userId,
            CurrentParticipants = mealEvent.CurrentParticipants,
            Status = mealEvent.Status
        });
    }

    /// <summary>
    /// 取得使用者的所有預約（包含刊登的活動和預約的活動）
    /// </summary>
    [HttpGet("users/{userId}/reservations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserReservations(
        string userId,
        [FromQuery] string? type = null) // "hosted" (我刊登的), "participated" (我預約的), null (全部)
    {
        var allResults = new List<object>();

        // 取得我刊登的活動
        if (type == null || type == "hosted")
        {
            var hosted = await _db.MealEvents
                .Where(m => m.HostUserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            foreach (var m in hosted)
            {
                // 計算總參與人數（加總所有已確認參與者的 participantCount）
                var participantCount = await _db.MealEventParticipants
                    .Where(p => p.MealEventId == m.Id && p.Status == "confirmed")
                    .SumAsync(p => p.ParticipantCount);

                allResults.Add(new
                {
                    Type = "hosted",
                    ReservationId = (string?)null,
                    ReservationStatus = (string?)null,
                    ReservationCreatedAt = (DateTime?)null,
                    MealEvent = new
                    {
                        m.Id,
                        m.Title,
                        m.Description,
                        m.ImageUrl,
                        m.Latitude,
                        m.Longitude,
                        m.FullAddress,
                        m.City,
                        m.District,
                        m.Street,
                        m.Number,
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
                        m.Status,
                        m.Notes,
                        m.Phone,
                        m.Email
                    },
                    ParticipantCount = participantCount
                });
            }
        }

        // 取得我預約的活動
        if (type == null || type == "participated")
        {
            var participated = await _db.MealEventParticipants
                .Where(p => p.UserId == userId)
                .Include(p => p.MealEvent)
                .OrderByDescending(p => p.Status == "confirmed") // confirmed 在最前面
                .ThenByDescending(p => p.CreatedAt)              // 再依時間新→舊
                .ToListAsync();

            foreach (var p in participated)
            {
                if (p.MealEvent != null)
                {
                    allResults.Add(new
                    {
                        Type = "participated",
                        ReservationId = p.Id,
                        ReservationStatus = p.Status,
                        ReservationCreatedAt = p.CreatedAt,
                        Phone = p.Phone,
                        Email = p.Email,
                        ParticipantCount = p.ParticipantCount,
                        MealEvent = new
                        {
                            p.MealEvent.Id,
                            p.MealEvent.Title,
                            p.MealEvent.Description,
                            p.MealEvent.ImageUrl,
                            p.MealEvent.Latitude,
                            p.MealEvent.Longitude,
                            p.MealEvent.FullAddress,
                            p.MealEvent.City,
                            p.MealEvent.District,
                            p.MealEvent.Street,
                            p.MealEvent.Number,
                            p.MealEvent.HostUserId,
                            p.MealEvent.Capacity,
                            p.MealEvent.CurrentParticipants,
                            p.MealEvent.DietType,
                            p.MealEvent.IsDineIn,
                            p.MealEvent.StartTime,
                            p.MealEvent.EndTime,
                            p.MealEvent.SignupDeadline,
                            p.MealEvent.CreatedAt,
                            p.MealEvent.UpdatedAt,
                            p.MealEvent.Status,
                            p.MealEvent.Notes,
                            p.MealEvent.Phone,
                            p.MealEvent.Email
                        }
                    });
                }
            }
        }

        // 排序：優先使用 ReservationCreatedAt，否則使用 MealEvent.CreatedAt
        var sortedResults = allResults.OrderByDescending(r =>
        {
            var reservationCreatedAt = r.GetType().GetProperty("ReservationCreatedAt")?.GetValue(r) as DateTime?;
            if (reservationCreatedAt.HasValue)
                return reservationCreatedAt.Value;

            var mealEvent = r.GetType().GetProperty("MealEvent")?.GetValue(r);
            if (mealEvent != null)
            {
                var createdAt = mealEvent.GetType().GetProperty("CreatedAt")?.GetValue(mealEvent) as DateTime?;
                return createdAt ?? DateTime.MinValue;
            }

            return DateTime.MinValue;
        }).ToList();

        return Ok(new
        {
            Type = type ?? "all",
            Data = sortedResults
        });
    }

    /// <summary>
    /// 取得特定預約詳情
    /// </summary>
    [HttpGet("reservations/{reservationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservationById(string reservationId)
    {
        var participant = await _db.MealEventParticipants
            .Include(p => p.MealEvent)
            .FirstOrDefaultAsync(p => p.Id == reservationId);

        if (participant is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {reservationId} 的預約記錄" });
        }

        var result = new
        {
            ReservationId = participant.Id,
            MealEventId = participant.MealEventId,
            UserId = participant.UserId,
            Phone = participant.Phone,
            Email = participant.Email,
            ParticipantCount = participant.ParticipantCount,
            Status = participant.Status,
            CreatedAt = participant.CreatedAt,
            UpdatedAt = participant.UpdatedAt,
            MealEvent = participant.MealEvent != null ? new
            {
                participant.MealEvent.Id,
                participant.MealEvent.Title,
                participant.MealEvent.Description,
                participant.MealEvent.ImageUrl,
                participant.MealEvent.Latitude,
                participant.MealEvent.Longitude,
                participant.MealEvent.FullAddress,
                participant.MealEvent.City,
                participant.MealEvent.District,
                participant.MealEvent.Street,
                participant.MealEvent.Number,
                participant.MealEvent.HostUserId,
                participant.MealEvent.Capacity,
                participant.MealEvent.CurrentParticipants,
                participant.MealEvent.DietType,
                participant.MealEvent.IsDineIn,
                participant.MealEvent.StartTime,
                participant.MealEvent.EndTime,
                participant.MealEvent.SignupDeadline,
                participant.MealEvent.CreatedAt,
                participant.MealEvent.UpdatedAt,
                participant.MealEvent.Status,
                participant.MealEvent.Notes,
                participant.MealEvent.Phone,
                participant.MealEvent.Email
            } : null
        };

        return Ok(result);
    }
}

/// <summary>
/// 參加共餐活動請求模型
/// </summary>
public record ParticipateRequest(string UserId, string? Phone = null, string? Email = null, int ParticipantCount = 1);

