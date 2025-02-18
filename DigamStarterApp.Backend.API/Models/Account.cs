using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DigamStarterApp.Backend.API.Models
{
  public class Account : Model
  {
    public string Name { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }


    [JsonIgnore]
    public bool IsAdmin { get; set; }
    
    [JsonIgnore]
    public string GoogleAuthUserId { get; set; }
  }
  
}
