using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GongCanApi.Data;
using GongCanApi.Models;

namespace GongCanApi.Controllers;

[ApiController]
[Route("api/parking")]
[Tags("Parking API")]
public class ParkingController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ParkingController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 取得所有停車場狀態（分頁）
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllParkingStatuses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var totalCount = await _db.TaipeiParkingStatuses.CountAsync();
        var items = await _db.TaipeiParkingStatuses
            .OrderByDescending(p => p.UpdateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = items
        });
    }

    /// <summary>
    /// 根據 ID 取得最新的停車場狀態
    /// </summary>
    [HttpGet("status/{id}")]
    [ProducesResponseType(typeof(TaipeiParkingStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParkingStatusById(string id)
    {
        var status = await _db.TaipeiParkingStatuses
            .Where(p => p.Id == id)
            .OrderByDescending(p => p.UpdateTime)
            .FirstOrDefaultAsync();

        return status is not null 
            ? Ok(status) 
            : NotFound(new { Message = $"找不到 ID 為 {id} 的停車場" });
    }

    /// <summary>
    /// 根據 ID 和更新時間取得特定停車場狀態
    /// </summary>
    [HttpGet("status/{id}/{updateTime}")]
    [ProducesResponseType(typeof(TaipeiParkingStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParkingStatusByIdAndTime(string id, DateTime updateTime)
    {
        var status = await _db.TaipeiParkingStatuses
            .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

        return status is not null 
            ? Ok(status) 
            : NotFound(new { Message = "找不到指定的停車場狀態記錄" });
    }

    /// <summary>
    /// 取得特定停車場的歷史記錄
    /// </summary>
    [HttpGet("status/{id}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParkingStatusHistory(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var totalCount = await _db.TaipeiParkingStatuses.Where(p => p.Id == id).CountAsync();
        var items = await _db.TaipeiParkingStatuses
            .Where(p => p.Id == id)
            .OrderByDescending(p => p.UpdateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            TotalCount = totalCount,
            ParkingId = id,
            Page = page,
            PageSize = pageSize,
            Data = items
        });
    }

    /// <summary>
    /// 新增停車場狀態
    /// </summary>
    [HttpPost("status")]
    [ProducesResponseType(typeof(TaipeiParkingStatus), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateParkingStatus([FromBody] TaipeiParkingStatus status)
    {
        // 檢查是否已存在相同的記錄
        var exists = await _db.TaipeiParkingStatuses
            .AnyAsync(p => p.Id == status.Id && p.UpdateTime == status.UpdateTime);

        if (exists)
        {
            return Conflict(new { Message = "該停車場狀態記錄已存在" });
        }

        _db.TaipeiParkingStatuses.Add(status);
        await _db.SaveChangesAsync();

        return Created($"/api/parking/status/{status.Id}/{status.UpdateTime:O}", status);
    }

    /// <summary>
    /// 更新停車場狀態
    /// </summary>
    [HttpPut("status/{id}/{updateTime}")]
    [ProducesResponseType(typeof(TaipeiParkingStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateParkingStatus(string id, DateTime updateTime, [FromBody] TaipeiParkingStatus updatedStatus)
    {
        var status = await _db.TaipeiParkingStatuses
            .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

        if (status is null)
        {
            return NotFound(new { Message = "找不到指定的停車場狀態記錄" });
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

        await _db.SaveChangesAsync();

        return Ok(status);
    }

    /// <summary>
    /// 刪除停車場狀態
    /// </summary>
    [HttpDelete("status/{id}/{updateTime}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteParkingStatus(string id, DateTime updateTime)
    {
        var status = await _db.TaipeiParkingStatuses
            .FirstOrDefaultAsync(p => p.Id == id && p.UpdateTime == updateTime);

        if (status is null)
        {
            return NotFound(new { Message = "找不到指定的停車場狀態記錄" });
        }

        _db.TaipeiParkingStatuses.Remove(status);
        await _db.SaveChangesAsync();

        return Ok(new { Message = "刪除成功" });
    }

    /// <summary>
    /// 取得所有不重複的停車場 ID
    /// </summary>
    [HttpGet("ids")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllParkingIds()
    {
        var ids = await _db.TaipeiParkingStatuses
            .Select(p => p.Id)
            .Distinct()
            .OrderBy(id => id)
            .ToListAsync();

        return Ok(ids);
    }
}

