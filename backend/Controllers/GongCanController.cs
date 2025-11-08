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
            m.Reserved1,
            m.Reserved2,
            m.Reserved3,
            m.Reserved4,
            m.Reserved5
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
        if (mealEvent.CreatedAt == null)
        {
            mealEvent.CreatedAt = DateTime.UtcNow;
        }
        if (mealEvent.UpdatedAt == null)
        {
            mealEvent.UpdatedAt = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(mealEvent.Status))
        {
            mealEvent.Status = "open";
        }

        _db.MealEvents.Add(mealEvent);
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
    /// 使用者參加共餐活動
    /// </summary>
    [HttpPost("meals/{id}/participate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        if (mealEvent.Capacity > 0 && mealEvent.CurrentParticipants >= mealEvent.Capacity)
        {
            return BadRequest(new { Message = "活動已額滿" });
        }

        // TODO: 這裡應該要有一個參與者表（meal_event_participants）來記錄參與者
        // 目前先簡單地增加參與人數
        mealEvent.CurrentParticipants++;
        
        // 如果已額滿，更新狀態
        if (mealEvent.Capacity > 0 && mealEvent.CurrentParticipants >= mealEvent.Capacity)
        {
            mealEvent.Status = "full";
        }

        mealEvent.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new
        {
            Message = "參加成功",
            MealEventId = mealEvent.Id,
            UserId = request.UserId,
            CurrentParticipants = mealEvent.CurrentParticipants,
            Status = mealEvent.Status
        });
    }

    /// <summary>
    /// 使用者取消參加共餐活動
    /// </summary>
    [HttpDelete("meals/{id}/participate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelParticipation(string id, [FromQuery] string userId)
    {
        var mealEvent = await _db.MealEvents
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mealEvent is null)
        {
            return NotFound(new { Message = $"找不到 ID 為 {id} 的共餐活動" });
        }

        // TODO: 這裡應該要檢查參與者表中是否有該使用者
        // 目前先簡單地減少參與人數
        if (mealEvent.CurrentParticipants > 0)
        {
            mealEvent.CurrentParticipants--;
            
            // 如果原本是 full 狀態，改回 open
            if (mealEvent.Status == "full" && mealEvent.CurrentParticipants < mealEvent.Capacity)
            {
                mealEvent.Status = "open";
            }
        }

        mealEvent.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new
        {
            Message = "取消參加成功",
            MealEventId = mealEvent.Id,
            UserId = userId,
            CurrentParticipants = mealEvent.CurrentParticipants,
            Status = mealEvent.Status
        });
    }
}

/// <summary>
/// 參加共餐活動請求模型
/// </summary>
public record ParticipateRequest(string UserId);

