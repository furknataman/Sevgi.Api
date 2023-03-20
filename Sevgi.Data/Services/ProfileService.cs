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

            await _userManager.UpdateAsync(userToUpdate);
        }
        public async Task ClaimCard(string userId)
        {
            var query = @"
                //select claimed and active cards
                //select cards not in that group
                //select 1
                //perform insert

                SELECT TOP 1 @CardId = C.Id
                FROM Cards C
                WHERE C.Id NOT IN (
                    SELECT UC.Id
                    FROM UserCards UC
                    WHERE IsActive = 1
                ) AND IsDeleted = 0
                
                INSERT INTO UserCards (UserId, CardId, IsActive) VALUES(@UserId, @CardId, 1)
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
            /*var viewModel = new UserView
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                TotalAmount = user.TotalAmount,
                BirthDate = user.BirthDate,
                FileId = user.FileId,
                //PhoneNumber=user.PhoneNumber,
                IsActive = user.IsActive,

            };
            return viewModel;*/
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
