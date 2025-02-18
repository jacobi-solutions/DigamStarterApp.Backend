namespace DigamStarterApp.Backend.API.DataContracts
{
    public class  Response
  {
    // public string Token { get; set; }
    public List<Error> Errors { get; set; } = new List<Error>();
    public bool IsSuccess { get; set; } = true;

    public void AddError(string errorMessage, string errorCode)
    {
        Errors.Add(new Error { ErrorMessage = errorMessage, ErrorCode = errorCode });
        IsSuccess = false;
    }
  }

}
