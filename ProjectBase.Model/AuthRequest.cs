using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Model
{
    public class AuthRequest
    {
        [Required]
        public AuthProviders Provider { get; set; } = AuthProviders.INTERNAL;

        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
