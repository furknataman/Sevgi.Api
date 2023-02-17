using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public class AuthRequest
    {
        [Required]
        public AuthProviders Provider { get; set; } = AuthProviders.INTERNAL;

        [Required]
        public string IdToken { get; set; } = string.Empty;
        public VerificationMethods VerificationMethod { get; set; } = VerificationMethods.PHONE;
    }
}
