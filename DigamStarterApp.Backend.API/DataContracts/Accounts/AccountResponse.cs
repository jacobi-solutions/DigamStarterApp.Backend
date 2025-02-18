using DigamStarterApp.Backend.API.Models;

namespace DigamStarterApp.Backend.API.DataContracts
{
    public class AccountResponse: Response
    {
      public Account Account { get; set; }
    }

}
