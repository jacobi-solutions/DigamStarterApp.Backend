using DigamStarterApp.Backend.API.Models;
using DigamStarterApp.Backend.API.Repos;
using DigamStarterApp.Backend.API.DataContracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using System;

namespace DigamStarterApp.Backend.API.Services
{
    public class AccountsService
    {
        private readonly AccountsRepo _accountsRepo;

        public AccountsService(AccountsRepo accountsRepo)
        {
            _accountsRepo = accountsRepo;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var response = new Response();
            try
            {
                return await _accountsRepo.GetAllAsync();
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
                return new List<Account>();
            }
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            var response = new Response();
            try
            {
                return await _accountsRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
                return null;
            }
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            var response = new Response();
            try
            {
                return await _accountsRepo.CreateAsync(account);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
                return null;
            }
        }

        public async Task<bool> UpdateAccountAsync(string id, Account updatedAccount)
        {
            var response = new Response();
            try
            {
                return await _accountsRepo.UpdateAsync(id, updatedAccount);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var response = new Response();
            try
            {
                return await _accountsRepo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
                return false;
            }
        }

        public async Task<AccountResponse> CreateAccountAsync(AccountRequest request, string googleAuthUserId)
        {
            var response = new AccountResponse();
            try
            {
                var existingAccount = await _accountsRepo.GetAccountByGoogleAuthUserIdAsync(googleAuthUserId);
                if (existingAccount != null)
                {
                    response.AddError("User already exists", "USER_ALREADY_EXISTS");
                    return response;
                }

                var account = new Account
                {
                    Name = request.Username,
                    GoogleAuthUserId = googleAuthUserId,
                    Email = request.Email
                };

                response.Account = await _accountsRepo.CreateAsync(account);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message, "EXCEPTION");
            }

            return response;
        }

        // public async Task<(bool IsSuccess, int StatusCode, string Message, object Data)> VerifyGoogleTokenAsync(string idToken)
        // {
        //     try
        //     {
        //         var payload = await GoogleJsonWebSignature.ValidateAsync(
        //             idToken,
        //             new GoogleJsonWebSignature.ValidationSettings
        //             {
        //                 Audience = new[] { "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com" }
        //             }
        //         );

        //         var userId = payload.Subject;
        //         var email = payload.Email;
        //         var name = payload.Name;

        //         var existingAccount = await GetAccountByIdAsync(userId);
        //         if (existingAccount == null)
        //         {
        //             var newAccount = new Account
        //             {
        //                 Id = userId,
        //                 Name = name,
        //                 Email = email,
        //                 GoogleAuthUserId = userId
        //             };

        //             await _accountsRepo.CreateAsync(newAccount);
        //         }

        //         return (true, 200, null, new { userId, email, name });
        //     }
        //     catch (InvalidJwtException ex)
        //     {
        //         return (false, 401, $"Invalid Google ID token: {ex.Message}", null);
        //     }
        // }
    }
}
