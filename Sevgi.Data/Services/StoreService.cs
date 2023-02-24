using Dapper;
using Sevgi.Data.Database;
using Sevgi.Model;
namespace Sevgi.Data.Services
{
    public interface IStoreService
    {
        public Task<IEnumerable<Store>> GetAll();
    }
    public class StoreService : IStoreService
    {
        private DapperContext _context;
        public StoreService(DapperContext context)
        {
            //get db here
            _context = context;
        }

        public async Task<IEnumerable<Store>> GetAll()
        {
            string userId = "1' OR 1=1 GO DROP TABLE Users";
            var query = @"
                SELECT * from Stores
                where id = '"+userId+"'";

            using var connection = _context.CreateConnection();
            var allStores = await connection.QueryAsync<Store>(query);
            return allStores;
        }
    }
}
