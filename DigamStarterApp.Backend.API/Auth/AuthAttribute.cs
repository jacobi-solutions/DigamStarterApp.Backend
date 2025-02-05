using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DigamStarterApp.Backend.API.Auth {
    public class AuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var idToken = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Verify the Firebase ID token
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                httpContext.Items["User"] = decodedToken; // Store the user info in the request context
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase token verification failed: {ex.Message}");
                context.Result = new UnauthorizedResult();
            }
        }
    }
}