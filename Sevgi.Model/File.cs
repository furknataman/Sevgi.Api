using System;
namespace Sevgi.Model
{
	public class Store
	{
		public int Id { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public int FileId { get; set; }
        public string ExternalId { get; set; }
        public bool IsDeleted { get; set; }

    }
}

