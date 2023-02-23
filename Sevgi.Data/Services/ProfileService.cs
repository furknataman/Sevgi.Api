using Dapper;
using Microsoft.AspNetCore.Identity;
using Sevgi.Data.Database;
using Sevgi.Model;

namespace Sevgi.Data.Services
{
    public interface IProfileService
    {
        public Task Update(User userToUpdate, ProfileInformation newInfo);
        public Task<String> GetInfo();
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
        public async Task<String> GetInfo()
        {
            return "SomeThing";
        }
    }
}
