using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public class UpdateUserRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
