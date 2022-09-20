using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectBase.Model;
using ProjectBase.Model.Utilities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Data.Services
{
    public interface IAuthenticationService
    {
        Task<string> Register(string email, string password);
        Task<string> Login(string email, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


        public async Task<string> Register(string email, string password)
        {
            var userToCheck = await _userManager.FindByEmailAsync(email);
            if (userToCheck is not null) throw new UserExistsException($"There already is a user registered with email: {userToCheck.Email}.");
            
            var userToRegister = new User()
            {
                UserName = email,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(userToRegister, password);

            if (!result.Succeeded) throw new UserException($"User with email: {email} cannot be registered. Errors: {GetErrorsText(result.Errors)}");

            return await Login(email, password);
        }

        public async Task<string> Login(string email, string password)
        {
            var userToCheck = await _userManager.FindByEmailAsync(email);
            if (userToCheck is null) throw new UserNotFoundException($"There is no such user with email: {email}");

            if (!await _userManager.CheckPasswordAsync(userToCheck, password)) throw new InvalidPasswordException($"Unable to authenticate user {email}");

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GetToken(authClaims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
        private string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }
}
