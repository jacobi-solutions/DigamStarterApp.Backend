using DigamStarterApp.Backend.API.DataContracts;
using DigamStarterApp.Backend.API.Models;
using DigamStarterApp.Backend.API.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize]
        [HttpPost("register")]
        public async Task<ActionResult<AccountResponse>> RegisterUser([FromBody] AccountRequest request)
        {

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine("Received Authorization Header: " + authHeader); // Debugging


            var response = new AccountResponse();
            if (request == null || string.IsNullOrEmpty(request.Username))
            {
                response.AddError("Invalid user data", "INVALID_USER_DATA");
                return response;
            }

            // Get GoogleAuthUserId from HttpContext
            var googleAuthUserId = User.Claims.FirstOrDefault(x => x.Type.Equals("user_id"))?.Value;
            if (string.IsNullOrEmpty(googleAuthUserId))
            {
                response.AddError("Invalid Google Auth User ID", "INVALID_GOOGLE_AUTH_USER_ID");
                return response;
            }

            // Delegate the account creation to the service
            response = await _accountsService.CreateAccountAsync(request, googleAuthUserId);

            if (response.IsSuccess)
                return Ok("User registered successfully in backend");

            return response;   
        }


        // [HttpPost("google")]
        // public async Task<IActionResult> VerifyGoogleToken()
        // {
        //     var authHeader = Request.Headers["Authorization"].ToString();
        //     if (!authHeader.StartsWith("Bearer "))
        //         return BadRequest("Missing or invalid Authorization header");

        //     var idToken = authHeader.Substring("Bearer ".Length);

        //     // Delegate the Google token verification logic to the service
        //     var result = await _accountsService.VerifyGoogleTokenAsync(idToken);

        //     if (result.IsSuccess)
        //         return Ok(result.Data);

        //     return StatusCode(result.StatusCode, result.Message);
        // }
    }
}
