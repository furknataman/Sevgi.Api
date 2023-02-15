using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sevgi.Data.Utilities;
using Sevgi.Model;
using Sevgi.Model.Utilities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sevgi.Data.Services
{
    public interface IAuthService
    {
        Task<string> SignUp(string email, string password);
        Task<string> SignIn(string email, string password);
        Task<string> ExternalAuth(AuthRequest request);
        Task<string> SignOut(string email);
        Task ClearUsers();

    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


        public async Task<string> SignUp(string email, string password)
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

            return await SignIn(email, password);
        }

        public async Task<string> SignIn(string email, string password)
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
        private static string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }


        public async Task<string> ExternalAuth(AuthRequest request)
        {
            switch (request.Provider)
            {
                case AuthProviders.GOOGLE:

                    //verify google
                    var payload = await VerifyGoogleToken(request.IdToken);
                    if (payload == null) throw new InvalidTokenException("Google token is invalid, cannot authorize.");

                    //check if registered
                    var loginInfo = new UserLoginInfo(request.Provider.ToString(), payload.Subject, "Google");
                    var userToCheck = await _userManager.FindByEmailAsync(payload.Email);

                    //register if not
                    string result;
                    if (userToCheck == null) result = await SignUp(payload.Email, payload.Email.GeneratePassword());
                    else result = await SignIn(payload.Email, payload.Email.GeneratePassword());

                    //add login
                    await _userManager.AddLoginAsync(await _userManager.FindByEmailAsync(payload.Email), loginInfo);
                    return result;

                case AuthProviders.INTERNAL:
                case AuthProviders.APPLE:
                default:
                    throw new Exception("The option you requested is not supported, please use available options.");
            }
        }

        public Task<string> SignOut(string email)
        {
            throw new NotImplementedException();
        }

        public Task ClearUsers()
        {
            throw new NotImplementedException();
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] } };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return payload;
        }
    }
}
