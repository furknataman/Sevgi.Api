using System;
namespace Sevgi.Model
{
	public class Spending
	{
        public class SpendingInformation
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public DateTime BirthDate { get; set; }
            public Genders Gender { get; set; }
        }
    }
}

