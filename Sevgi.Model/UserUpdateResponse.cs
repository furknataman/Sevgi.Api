using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public record UpdateUserRequest : CreateUserRequest
    {
        public string Id { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public record CreateUserRequest
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Genders Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int FileId { get; set; }
    }
}
