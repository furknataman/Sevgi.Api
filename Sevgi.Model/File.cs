using System;
namespace Sevgi.Model
{
	public class UploadableFile
	{
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}

