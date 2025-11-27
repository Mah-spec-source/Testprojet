using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Select(u => new UserDto {
                Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role.ToString()
            }).ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role.ToString() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            u.Name = dto.Name;
            u.Role = Enum.Parse<Role>(dto.Role);
            await _db.SaveChangesAsync();
            return Ok(new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role.ToString() });
        }
    }
}