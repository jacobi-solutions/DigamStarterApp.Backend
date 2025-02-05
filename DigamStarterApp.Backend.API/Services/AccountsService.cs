using DigamStarterApp.Backend.API.Models;
using DigamStarterApp.Backend.API.Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _accountsRepo.GetAllAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            return await _accountsRepo.GetByIdAsync(id);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            return await _accountsRepo.CreateAsync(account);
        }

        public async Task<bool> UpdateAccountAsync(string id, Account updatedAccount)
        {
            return await _accountsRepo.UpdateAsync(id, updatedAccount);
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            return await _accountsRepo.DeleteAsync(id);
        }
    }
}
