using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DigamStarterApp.Backend.API.Auth {
    public class AuthService
    {
        private readonly FirebaseApp _firebaseApp;

        public AuthService(IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }
            else
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
            }
        }

        // Verify Firebase ID Token
        public async Task<FirebaseToken> VerifyIdToken(string idToken)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token verification failed: {ex.Message}");
                return null;
            }
        }

        // Verify Google Identity Token
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}