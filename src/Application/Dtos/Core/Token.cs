namespace ContractorBackend.Application.Dtos.Core
{
    public class Token
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserVM UserData { get; set; } = null!;
    }
}
