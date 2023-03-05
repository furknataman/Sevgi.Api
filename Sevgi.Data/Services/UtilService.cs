using System.Collections.Generic;
using Dapper;
using Sevgi.Data.Database;
using Sevgi.Model;
using File = Sevgi.Model.File;

namespace Sevgi.Data.Services
{
    public interface IUtilService
    {
        public Task<IEnumerable<File>> uploadFile();
    }
    public class UtilService : IUtilService
    {
        private DapperContext _context;
        public UtilService(DapperContext context)
        {
            //get db here
            _context = context;
        }

        public async Task<IEnumerable<File>> uploadFile()
        {
            var query = "INSERT INTO Files(Data, Name, Type) VALUES(@Data, @Name, @Type)";

            using var connection = _context.CreateConnection();
            var uploadFile = await connection.QueryAsync<File>(query);
            return uploadFile;
        }
    }
}
