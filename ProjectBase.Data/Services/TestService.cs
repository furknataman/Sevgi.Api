using Dapper;
using ProjectBase.Data.Database;
using Solvy.Data.Database;
using Solvy.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvy.Data.Services
{
    public interface ITestService
    {
        public Task<IEnumerable<int>> GetAll();
    }
    public class TestService : ITestService
    {
        private DapperContext _context;
        public TestService(DapperContext context)
        {
            //get db here
            _context = context;
        }

        public async Task<IEnumerable<int>> GetAll()
        {
            var query = @"
SELECT *
FROM Tests
ORDER BY 1 DESC
            ";

            using var connection = _context.CreateConnection();
            var tests = await connection.QueryAsync<Test>(query);
            return tests;

        }



        //get tests
        //get one test
        //check test purchase
        //return test
    }
}
