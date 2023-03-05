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
        Task<string> UpdateUser();
        public Task<IEnumerable<User>> GetAll();
    }
    public class AdminService : IAdminService
    {

        private readonly UserManager<User> _adminManager;
        private DapperContext _context;


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

        public async Task UpdateUser(User userToUpdate)
        {
            //check user
            //set user
            //update
            var result = await _adminManager.UpdateAsync(userToUpdate);
            //check if successful

        }

        public Task<string> UpdateUser()
        {
            throw new NotImplementedException();
        }
    }
}

