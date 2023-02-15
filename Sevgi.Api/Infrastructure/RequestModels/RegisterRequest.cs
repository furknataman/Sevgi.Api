using System.ComponentModel.DataAnnotations;

namespace Sevgi.Api.Infrastructure.RequestModels
{
    public record RegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
