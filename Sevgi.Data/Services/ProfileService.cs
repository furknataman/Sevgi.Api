using Dapper;
using Microsoft.AspNetCore.Identity;
using Sevgi.Data.Database;
using Sevgi.Model;

namespace Sevgi.Data.Services
{
    public interface IProfileService
    {
        public Task Update(User userToUpdate, ProfileInformation newInfo);
        public Task ClaimCard(string userId);
        public Task <UserInfo> GetUserInfo(String id);
        
        public Task<IEnumerable<UserSale>> GetUserSale(string id);
    }
    public class ProfileService : IProfileService
    {
        private DapperContext _context;
        private readonly UserManager<User> _userManager;
        public ProfileService(DapperContext context, UserManager<User> userManager)
        {
            //get db here
            _context = context;
            _userManager = userManager;
        }

        public async Task Update(User userToUpdate, ProfileInformation newInfo)
        {
            userToUpdate.FirstName = newInfo.FirstName;
            userToUpdate.LastName = newInfo.LastName;
            userToUpdate.Gender = newInfo.Gender;
            userToUpdate.BirthDate = newInfo.BirthDate;
            userToUpdate.IsReady = true;

            await _userManager.UpdateAsync(userToUpdate);
        }
        public async Task ClaimCard(string userId)
        {
            var query = @"
                
                        SET @new_user_id = @userId;
                        SET @inactive_bonus_id = (
                        SELECT id
                        FROM Bonus
                        WHERE IsActive = 0
                        LIMIT 1
                        );
                        INSERT INTO UserBonus (UserId, BonusId)
                        VALUES (@new_user_id, @inactive_bonus_id);
                        UPDATE Bonus
                        SET IsActive = 1
                        WHERE id = @inactive_bonus_id;

            ";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, new { UserId = userId });
        }
        public async Task<UserInfo> GetUserInfo(String Id)
        {
            
            var query = @"SELECT U.*, B.*, UB.*
                        FROM Users U
                        JOIN UserBonus UB ON U.id = UB.UserId
                        JOIN Bonus B ON UB.BonusId = B.Id
                        WHERE U.Id = @UserId";

            using var connection = _context.CreateConnection();
            var userInfo = await connection.QuerySingleAsync<UserInfo>(query, new {UserId =Id});
            return userInfo;
         
        }
        public async Task<IEnumerable<UserSale>> GetUserSale(string id)
        {
            var query = @"SELECT S.*, St.fileid, St.Name
                          
                          FROM SaleReceipts S
                          JOIN Stores St ON S.BranchCode = St.ExternalId
                          WHERE S.CardNo = @CardNo";

            using var connection = _context.CreateConnection();
            var allSale = await connection.QueryAsync<UserSale>(query, new { CardNo = id });
            return allSale;
        }


    }
}
