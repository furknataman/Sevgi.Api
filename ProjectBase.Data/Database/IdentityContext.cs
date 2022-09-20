using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Model;

namespace ProjectBase.Data.Database
{
    public class BaseIdentityContext : IdentityDbContext<User>
    {
        public BaseIdentityContext(DbContextOptions<BaseIdentityContext> options) : base(options)
        {
            //this is the dbcontext which creates and connects to your database for identity operations.
            //use options if you will have startup settings
        }
    }
}
