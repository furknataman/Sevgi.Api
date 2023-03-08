using System;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Sevgi.Data.Database;
using Sevgi.Model;
using Sevgi.Model.Utilities;

namespace Sevgi.Data.Services
{
    public interface IAdminService
    {

        public Task<IEnumerable<User>> GetAll();
        public Task<IEnumerable<Sale>> GetAllSell();
        public Task<IdentityResult> UpdateUser(string id, string firstName, string LastName, string phone, bool status);
    }
    public class AdminService : IAdminService
    {

        private DapperContext _context;
        private readonly UserManager<User> _userManager;


        public AdminService(DapperContext dapperContext, UserManager<User> userManager)
        {
            //get db here
            _context = dapperContext;
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAll()
        {

            var query = "SELECT * from Users";

            using var connection = _context.CreateConnection();
            var allStores = await connection.QueryAsync<User>(query);
            return allStores;
        }


        public async Task<IEnumerable<Sale>> GetAllSell()
        {

            var query = "SELECT * from SaleReceipts";

            using var connection = _context.CreateConnection();
            var allSale = await connection.QueryAsync<Sale>(query);
            return allSale;
        }




        public async Task<IdentityResult> UpdateUser(string id, string firstName, string LastName, string phone, bool status)
        {
            var userToCheck = await _userManager.FindByIdAsync(id) ?? throw new UserException("User not found ");
            
            userToCheck.FirstName = firstName;
            userToCheck.LastName = LastName;
            userToCheck.PhoneNumber = phone;
            userToCheck.IsActive = status;

            var result = await _userManager.UpdateAsync(userToCheck);
            return result;

        }
    }
}

