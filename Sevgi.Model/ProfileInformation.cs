using System.ComponentModel.DataAnnotations;

namespace Sevgi.Model
{
    public class ProfileInformation
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Genders Gender { get; set; }
    }
}
