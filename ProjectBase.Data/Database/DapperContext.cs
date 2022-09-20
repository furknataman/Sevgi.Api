using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ProjectBase.Data.Database
{
    public class DapperContext
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SolvyDb");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
