using System;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Sevgi.Data.Database;
using Sevgi.Model;
using Sevgi.Model.Utilities;

namespace Sevgi.Data.Services
{
    public interface ICardService
    {

        public Task<int> CardControl(String cardNo);

    }
    public class CardService : ICardService
    {

        private DapperContext _context;
        private readonly UserManager<User> _userManager;


        public CardService(DapperContext dapperContext, UserManager<User> userManager)
        {
            //get db here
            _context = dapperContext;
            _userManager = userManager;
        }
        public async Task<int> CardControl(String cardNo)
        {

            var query = @"
                       SELECT IF(COUNT(*) > 0,
                       IF(IsActive = 0 AND IsPhysically = 1, 1, 2),
                       0) AS Result
                       FROM Bonus
                       WHERE CardNo = @cardNo;
                       ";

            using var connection = _context.CreateConnection();
            var cardControl = await connection.QuerySingleAsync<int>(query, new { cardNo = cardNo});
            return cardControl;



        }



    }
}

