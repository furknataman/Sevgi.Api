using Dapper;
using ProjectBase.Data.Database;

namespace ProjectBase.Data.Services
{
    public interface IBaseService
    {
        public Task<IEnumerable<int>> GetNumber();
    }
    public class BaseService : IBaseService
    {
        private DapperContext _context;
        public BaseService(DapperContext context)
        {
            //get db here
            _context = context;
        }

        public async Task<IEnumerable<int>> GetNumber()
        {
            var query = @"
                SELECT 1
            ";

            using var connection = _context.CreateConnection();
            var tests = await connection.QueryAsync<int>(query);
            return tests;
        }
    }
}
