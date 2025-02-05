using DigamStarterApp.Backend.API.Models;
using DigamStarterApp.Backend.API.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DigamStarterApp.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccountsService _accountsService;

        public AuthController(AccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [HttpPost("google")]
        public async Task<IActionResult> VerifyGoogleToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Bearer "))
                return BadRequest("Missing or invalid Authorization header");

            var idToken = authHeader.Substring("Bearer ".Length);

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com" }
                    }
                );

                var userId = payload.Subject;  
                var email = payload.Email;
                var name = payload.Name;

                // Check if the user already exists
                var existingAccount = await _accountsService.GetAccountByIdAsync(userId);
                if (existingAccount == null)
                {
                    var newAccount = new Account
                    {
                        Id = userId,
                        UserName = name
                    };

                    await _accountsService.CreateAccountAsync(newAccount);
                }

                return Ok(new
                {
                    userId = userId,
                    email = email,
                    name = name
                });
            }
            catch (InvalidJwtException ex)
            {
                return Unauthorized($"Invalid Google ID token: {ex.Message}");
            }
        }
    }
}
