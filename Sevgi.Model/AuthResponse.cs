using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public class AuthResponse
    {
        public AuthResponse()
        {
            
        }

        public AuthResponse(string message)
        {
            Message = message;
        }
        public AuthResponse(string message, bool isBanned)
        {
            Message = message;
            IsBanned = isBanned;
        }
        public string Token { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public bool IsUserReady { get; set; }
        public bool IsBanned { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
