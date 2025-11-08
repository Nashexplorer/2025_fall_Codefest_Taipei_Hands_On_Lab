using GongCanApi.Data;
using GongCanApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GongCanApi.Controllers
{
    [ApiController]
    [Route("api/support-points")]
    [Tags("Support Points API")]
    public class SupportPointController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SupportPointController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 取得愛心補給站與老人共食點位列表（可搜尋、可分頁）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSupportPoints(
            [FromQuery] string? keyword = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.SupportPoints.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    (p.OrgName != null && p.OrgName.Contains(keyword)) ||
                    (p.OrgGroupName != null && p.OrgGroupName.Contains(keyword)) ||
                    (p.Address != null && p.Address.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.OrgName)
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
        /// 根據 ID 取得愛心補給站與老人共食點位詳情
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSupportPointById(int id)
        {
            var supportPoint = await _db.SupportPoints
                .FirstOrDefaultAsync(p => p.Id == id);

            if (supportPoint == null)
            {
                return NotFound(new { Message = $"找不到 ID 為 {id} 的愛心補給站或老人共食點位" });
            }

            return Ok(supportPoint);
        }
    }
}
