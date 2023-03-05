using Dapper;
using Sevgi.Data.Database;
using Sevgi.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Sevgi.Data.Services
{
    public interface IStoreService
    {
        public Task<IEnumerable<Store>> GetAll();
        public Task<IEnumerable<Store>> Update(Store store);
        public Task<IEnumerable<Store>> Add(Store store);
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

            var query = " SELECT * from Stores";

            using var connection = _context.CreateConnection();
            var allStores = await connection.QueryAsync<Store>(query);
            return allStores;
        }

        public async Task<IEnumerable<Store>> Update(Store store)
        {

            Store update = store;
           

            var query = @"UPDATE Stores SET
                         Name = @Name,
                         Percentage = @Percentage,
                         FileId = @FileId,
                         ExternalId = @ExternalId,
                         IsDeleted = @IsDeleted
                         WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var updateStore = await connection.QueryAsync<Store>(query, new { Name = update.Name, FileId=update.FileId , ExternalId=update.ExternalId,Percentage = update.Percentage, Id=update.Id, IsDeleted=update.IsDeleted, });
            return updateStore;
        }
        public async Task<IEnumerable<Store>> Add(Store store)
        {
            Store newStore = store;
            var query = @" INSERT INTO Stores (Name, Percentage, FileId, ExternalId, IsDeleted) 
                VALUES (@Name, @Percentage, @FileId, @ExternalId, @IsDeleted);";

            using var connection = _context.CreateConnection();
            var addStore = await connection.QueryAsync<Store>(query, new{ Name = newStore.Name, FileId = newStore.FileId , ExternalId = newStore.ExternalId,Percentage = newStore.Percentage, Id = newStore.Id, IsDeleted = newStore.IsDeleted, });
            return addStore;
        }
    }
}
