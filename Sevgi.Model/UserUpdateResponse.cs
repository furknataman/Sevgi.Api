using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public bool IsRegistered { get; set; }
    }
}
