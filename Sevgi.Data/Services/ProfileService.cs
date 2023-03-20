using Dapper;
using Microsoft.AspNetCore.Identity;
using Sevgi.Data.Database;
using Sevgi.Model;

namespace Sevgi.Data.Services
{
    public interface IProfileService
    {
        public Task Update(User userToUpdate, ProfileInformation newInfo);
        public Task<UserView> GetInfo(User UserInfo);
        public Task<IEnumerable<Sale>> GetUserSale(String id);
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
        public async Task<UserView> GetInfo(User user)
        {
            var viewModel = new UserView
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
            return viewModel;
        }
        public async Task<IEnumerable<Sale>> GetUserSale(String id)
        {

            var query = "SELECT * from SaleReceipts Where CardNo=@CardNo";

            using var connection = _context.CreateConnection();
            var allSale = await connection.QueryAsync<Sale>(query, new { CardNo = id });
            return allSale;
        }
    }
}
