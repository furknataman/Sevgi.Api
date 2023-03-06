using System;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Sevgi.Data.Database;
using Sevgi.Model;
using Sevgi.Model.Utilities;

namespace Sevgi.Data.Services
{
    public interface IAdminService
    {

        public Task<IEnumerable<User>> GetAll();
        public Task<IdentityResult> Update(string id, string name, string surname, string telephoneNuber, bool status);
    }
    public class AdminService : IAdminService
    {

        private readonly UserManager<User> _adminManager;
        private DapperContext _context;
        private readonly UserManager<User> _userManager;


        public AdminService(DapperContext dapperContext, UserManager<User> userManager)
        {
            //get db here
            _context = dapperContext;
            _adminManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAll()
        {

            var query = "SELECT * from Users";

            using var connection = _context.CreateConnection();
            var allStores = await connection.QueryAsync<User>(query);
            return allStores;
        }


        public async Task<IdentityResult> Update(string id, string name, string surname, string telephoneNuber, bool status)
        {
            var userToCheck = await _userManager.FindByIdAsync(id);
            if (userToCheck is null) throw new UserException("User not found ");

            userToCheck.FirstName = name;
            userToCheck.LastName = surname;
            userToCheck.LockoutEnabled = status;
            userToCheck.PhoneNumber = telephoneNuber;


            var result = await _userManager.UpdateAsync(userToCheck);
            return result;

        }

  

    
    }
}

