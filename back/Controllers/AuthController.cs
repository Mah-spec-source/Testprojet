using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                var user = await _auth.Register(req.Name, req.Email, req.Password);
                return CreatedAtAction(nameof(Register), user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req)
        {
            var res = await _auth.Authenticate(req.Email, req.Password);
            if (res == null) return Unauthorized(new { error = "Invalid credentials" });
            return Ok(res);
        }
    }

    public record RegisterRequest(string Name, string Email, string Password);
}