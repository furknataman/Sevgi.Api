using System.Collections.Generic;
using Dapper;
using Sevgi.Data.Database;
using Sevgi.Model;

namespace Sevgi.Data.Services
{
    public interface IUtilService
    {
        public Task<int> uploadFile(UploadableFile newFile);
        public Task<UploadableFile> DownloadFile(int id);
    }
    public class UtilService : IUtilService
    {
        private DapperContext _context;
        public UtilService(DapperContext context)
        {
            //get db here
            _context = context;
        }

        public async Task<int> uploadFile(UploadableFile newFile)
        {
            var query = @"
                        INSERT INTO Files(Data, Name,Type ) VALUES(@Data, @Name,@Type);
                        SELECT LAST_INSERT_ID();
                        ";

            using var connection = _context.CreateConnection();
            var uploadFile = await connection.QuerySingleAsync<int>(query, new {Data=newFile.Data, Name=newFile.Name,Type=newFile.Type });

            return uploadFile;
        }

        public async Task<UploadableFile> DownloadFile(int id)
        {
            var query = @"
                        SELECT * FROM Files WHERE Id=@Id
                        ";

            using var connection = _context.CreateConnection();
            var uploadFile = await connection.QuerySingleAsync<UploadableFile>(query, new {Id=id});

            return uploadFile;
        }
    }
}
