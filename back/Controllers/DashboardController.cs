using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            var usersCount = await _db.Users.CountAsync();
            // add more metrics (ex: new signups last 7 days) using DB queries
            var metrics = new {
                usersCount,
                now = DateTime.UtcNow
            };
            return Ok(metrics);
        }
    }
}