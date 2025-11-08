using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GongCanApi.Data;
using GongCanApi.Models;

namespace GongCanApi.Controllers;

[ApiController]
[Route("api/gongcan")]
[Tags("GongCan API")]
public class GongCanController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public GongCanController(ApplicationDbContext db)
    {
        _db = db;
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

        // 轉換 tags JSON 字串為陣列
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
            m.Notes,
            m.Phone,
            m.Reserved1,
            m.Reserved2,
            m.Reserved3
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

        // 轉換 tags JSON 字串為陣列
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
            meal.Notes,
            meal.Reserved1,
            meal.Reserved2,
            meal.Reserved3,
            meal.Reserved4,
            meal.Reserved5
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
    // 檢查 ID 是否已存在
    var exists = await _db.MealEvents
        .AnyAsync(m => m.Id == mealEvent.Id);

    if (exists)
    {
        return Conflict(new { Message = $"ID {mealEvent.Id} 的共餐活動已存在" });
    }

    // 設定預設值
    mealEvent.CreatedAt ??= DateTime.UtcNow;
    mealEvent.UpdatedAt ??= DateTime.UtcNow;

    // status 不可超過 20 字元，預設 "open"
    if (string.IsNullOrEmpty(mealEvent.Status))
    {
        mealEvent.Status = "open";
    }
    else if (mealEvent.Status.Length > 20)
    {
        mealEvent.Status = mealEvent.Status.Substring(0, 20); // 截斷到 20 個字元
    }

    // tags 若為空或非合法 JSON，預設為空陣列 JSON
    if (string.IsNullOrEmpty(mealEvent.Tags))
    {
        mealEvent.Tags = "[]";
    }
    else
    {
        try
        {
            // 嘗試解析 JSON，若失敗則改成空陣列
            var parsed = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mealEvent.Tags);
            if (parsed == null)
            {
                mealEvent.Tags = "[]";
            }
        }
        catch
        {
            mealEvent.Tags = "[]";
        }
    }

    _db.MealEvents.Add(mealEvent);
    await _db.SaveChangesAsync();

    // 回傳結果（轉換 tags JSON 字串為陣列）
    var result = new
    {
        mealEvent.Id,
        mealEvent.Title,
        mealEvent.Description,
        mealEvent.ImageUrl,
        mealEvent.Location,
        mealEvent.Latitude,
        mealEvent.Longitude,
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
        Tags = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mealEvent.Tags)!,
        mealEvent.Status,
        mealEvent.Notes,
        mealEvent.Reserved1,
        mealEvent.Reserved2,
        mealEvent.Reserved3,
        mealEvent.Reserved4,
        mealEvent.Reserved5
    };

    return Created($"/api/gongcan/meals/{mealEvent.Id}", result);
}


    /// <summary>
    /// 商家更新共餐活動
    /// </summary>
    [HttpPut("meals/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMealEvent(string id, [FromBody] MealEvent updatedMealEvent)
    {
        var mealEvent = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mealEvent is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        // 更新可編輯的欄位（不允許更新 ID）
        mealEvent.Title = updatedMealEvent.Title;
        mealEvent.Description = updatedMealEvent.Description;
        mealEvent.ImageUrl = updatedMealEvent.ImageUrl;
        mealEvent.Location = updatedMealEvent.Location;
        mealEvent.Latitude = updatedMealEvent.Latitude;
        mealEvent.Longitude = updatedMealEvent.Longitude;
        mealEvent.Capacity = updatedMealEvent.Capacity;
        mealEvent.DietType = updatedMealEvent.DietType;
        mealEvent.IsDineIn = updatedMealEvent.IsDineIn;
        mealEvent.StartTime = updatedMealEvent.StartTime;
        mealEvent.EndTime = updatedMealEvent.EndTime;
        mealEvent.SignupDeadline = updatedMealEvent.SignupDeadline;
        mealEvent.Tags = updatedMealEvent.Tags;
        mealEvent.Status = updatedMealEvent.Status;
        mealEvent.Notes = updatedMealEvent.Notes;
        mealEvent.Reserved1 = updatedMealEvent.Reserved1;
        mealEvent.Reserved2 = updatedMealEvent.Reserved2;
        mealEvent.Reserved3 = updatedMealEvent.Reserved3;
        mealEvent.Reserved4 = updatedMealEvent.Reserved4;
        mealEvent.Reserved5 = updatedMealEvent.Reserved5;
        mealEvent.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // 轉換 tags JSON 字串為陣列
        var result = new
        {
            mealEvent.Id,
            mealEvent.Title,
            mealEvent.Description,
            mealEvent.ImageUrl,
            mealEvent.Location,
            mealEvent.Latitude,
            mealEvent.Longitude,
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
            Tags = string.IsNullOrEmpty(mealEvent.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(mealEvent.Tags) ?? new List<string>(),
            mealEvent.Status,
            mealEvent.Notes,
            mealEvent.Reserved1,
            mealEvent.Reserved2,
            mealEvent.Reserved3,
            mealEvent.Reserved4,
            mealEvent.Reserved5
        };

        return Ok(result);
    }

    /// <summary>
    /// 商家刪除共餐活動
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

        _db.MealEvents.Remove(mealEvent);
        await _db.SaveChangesAsync();

        return Ok(new { Message = "刪除成功" });
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

        // 檢查是否已額滿
        var confirmedCount = await _db.MealEventParticipants
            .CountAsync(p => p.MealEventId == id && p.Status == "confirmed");
        
        if (mealEvent.Capacity > 0 && confirmedCount >= mealEvent.Capacity)
        {
            return BadRequest(new { Message = "活動已額滿" });
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
                Status = "confirmed",
                CreatedAt = DateTime.UtcNow
            };
            _db.MealEventParticipants.Add(participant);
        }

        // 更新活動的參與人數（從參與者表計算）
        var newConfirmedCount = await _db.MealEventParticipants
            .CountAsync(p => p.MealEventId == id && p.Status == "confirmed");
        mealEvent.CurrentParticipants = newConfirmedCount;
        
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

        // 更新活動的參與人數（從參與者表計算）
        var confirmedCount = await _db.MealEventParticipants
            .CountAsync(p => p.MealEventId == id && p.Status == "confirmed");
        mealEvent.CurrentParticipants = confirmedCount;
        
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
                var participantCount = await _db.MealEventParticipants
                    .CountAsync(p => p.MealEventId == m.Id && p.Status == "confirmed");

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
                        m.Notes
                    },
                    ParticipantCount = participantCount
                });
            }
        }

        // 取得我預約的活動
        if (type == null || type == "participated")
        {
            var participated = await _db.MealEventParticipants
                .Where(p => p.UserId == userId && p.Status == "confirmed")
                .Include(p => p.MealEvent)
                .OrderByDescending(p => p.CreatedAt)
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
                        MealEvent = new
                        {
                            p.MealEvent.Id,
                            p.MealEvent.Title,
                            p.MealEvent.Description,
                            p.MealEvent.ImageUrl,
                            p.MealEvent.Location,
                            p.MealEvent.Latitude,
                            p.MealEvent.Longitude,
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
                            Tags = string.IsNullOrEmpty(p.MealEvent.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.MealEvent.Tags) ?? new List<string>(),
                            p.MealEvent.Status,
                            p.MealEvent.Notes
                        },
                        ParticipantCount = (int?)null
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
            Status = participant.Status,
            CreatedAt = participant.CreatedAt,
            UpdatedAt = participant.UpdatedAt,
            MealEvent = participant.MealEvent != null ? new
            {
                participant.MealEvent.Id,
                participant.MealEvent.Title,
                participant.MealEvent.Description,
                participant.MealEvent.ImageUrl,
                participant.MealEvent.Location,
                participant.MealEvent.Latitude,
                participant.MealEvent.Longitude,
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
                Tags = string.IsNullOrEmpty(participant.MealEvent.Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(participant.MealEvent.Tags) ?? new List<string>(),
                participant.MealEvent.Status,
                participant.MealEvent.Notes
            } : null
        };

        return Ok(result);
    }
}

/// <summary>
/// 參加共餐活動請求模型
/// </summary>
public record ParticipateRequest(string UserId);

