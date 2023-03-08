using System.Collections.Generic;
using Dapper;
using Sevgi.Data.Database;
using Sevgi.Model;

namespace Sevgi.Data.Services
{
    public interface IUtilService
    {
        public Task<int> uploadFile(UploadableFile newFile);
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
    }
}
