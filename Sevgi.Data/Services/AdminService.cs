using System;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Sevgi.Data.Database;
using Sevgi.Data.Utilities;
using Sevgi.Model;
using Sevgi.Model.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Sevgi.Data.Services
{
    public interface IAdminService
    {

        public Task<IEnumerable<UserView>> GetAll();
        public Task<IEnumerable<Sale>> GetAllSell();
        public Task<IdentityResult> UpdateUser(UpdateUserRequest updateRequest);
        public Task CreateUser(CreateUserRequest newUser);
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

        public async Task<IEnumerable<UserView>> GetAll()
        {
    
            var query = @"
                        SELECT U.Id,
                        U.FirstName,
                        U.LastName,
                        U.Gender,
                        U.PhoneNumber,
                        U.BirthDate,
                        U.CreatedAt,
                        U.IsActive,
                        U.FileId,
                        B.Bonus,
                        B.Total as TotalAmount
                        FROM Users U
                        JOIN UserBonus UB ON U.Id = UB.UserId
                        JOIN Bonus B ON UB.BonusId = B.Id";

            using var connection = _context.CreateConnection();
            var allUsers = await connection.QueryAsync<UserView>(query);
            return allUsers;
        }
        public async Task<IEnumerable<Sale>> GetAllSell()
        {

            var query = "SELECT * from SaleReceipts";

            using var connection = _context.CreateConnection();
            var allSale = await connection.QueryAsync<Sale>(query);
            return allSale;
        }
        public async Task<IdentityResult> UpdateUser(UpdateUserRequest updateRequest)
        {
            var userToCheck = await _userManager.FindByIdAsync(updateRequest.Id) ?? throw new UserException("User not found ");
            
            userToCheck.FirstName = updateRequest.FirstName;
            userToCheck.LastName = updateRequest.LastName;
            userToCheck.PhoneNumber = updateRequest.PhoneNumber;
            userToCheck.IsActive = updateRequest.Status;

            var result = await _userManager.UpdateAsync(userToCheck);
            return result;
        }
        public async Task CreateUser(CreateUserRequest newUser)
        {
            var checkUserQuery = @"
                SELECT * FROM
                Users U
                WHERE U.PhoneNumber = '@PhoneNumber'
            ";

            using var connection = _context.CreateConnection();
            var checkedUser = await connection.QueryFirstOrDefaultAsync<User>(checkUserQuery, new { newUser.PhoneNumber });

            if (checkedUser is not null) throw new UserExistsException();

            var userToCreate = new User()
            {
                BirthDate = newUser.BirthDate,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.PhoneNumber.GenerateEmailForInternal(),
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Gender = newUser.Gender,
                FileId = newUser.FileId,
            };

            await _userManager.CreateAsync(userToCreate, userToCreate.PhoneNumber.GeneratePassword());
        }
    }
}

