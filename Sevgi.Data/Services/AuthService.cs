using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
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
        Task<AuthResponse> SignUp(string email, string password);
        Task<AuthResponse> SignIn(string email, string password);
        Task<AuthResponse> ExternalAuth(AuthRequest request);
        Task<string> SignOut(string email);
        Task ClearUsers();
        Task AddUserInfo(ProfileInformation request);
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

        public async Task<AuthResponse> SignUp(string email, string password)
        {
            var userToCheck = await _userManager.FindByEmailAsync(email);
            if (userToCheck is not null) return new("The user is already registered.");

            var userToRegister = new User()
            {
                UserName = email,
                Email = email,
               
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(userToRegister, password);

            if (!result.Succeeded)return new("Signup is not successful.");

            var signinResponse = await SignIn(email, password);
            
            return signinResponse;
        }
        public async Task<AuthResponse> SignUp(User user, string password)
        {
            user.CreatedAt = DateTime.Now;

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded) return new("Signup is not successful.");

            var signinResponse = await SignIn(user.Email!, password);

            return signinResponse;
        }
        public async Task<AuthResponse> SignIn(string email, string password)
        {
            var userToCheck = await _userManager.FindByEmailAsync(email);
            if (userToCheck is null) return new("User not found.");
            if (!userToCheck.IsActive) return new("User is not allowed.", IsBanned = true);
            if (!await _userManager.CheckPasswordAsync(userToCheck, password)) return new("One or more of the credentials are not valid.");

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GetToken(authClaims);

            return new()
            {
                IsSuccessful = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

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
        public async Task<AuthResponse> ExternalAuth(AuthRequest request)
        {
            var response = new AuthResponse();
            switch (request.Provider)
            {
                case AuthProviders.GOOGLE:
                    //verify google
                    var payload = await VerifyGoogleToken(request.IdToken);
                    if (payload == null) return new("Invalid Token.");

                    //check if registered
                    var googleLoginInfo = new UserLoginInfo(request.Provider.ToString(), payload.Subject, "Google");
                    var userFromGoogle = await _userManager.FindByEmailAsync(payload.Email);

                    //register if not
                    if (userFromGoogle is null) response = await SignUp(payload.Email, payload.Email.GeneratePassword());
                    else response = await SignIn(payload.Email, payload.Email.GeneratePassword());

                    userFromGoogle = userFromGoogle is null ? await _userManager.FindByEmailAsync(payload.Email) : userFromGoogle;

                    //add login
                    await _userManager.AddLoginAsync(userFromGoogle!, googleLoginInfo);

                    //check if registration complete
                    response.IsUserReady = userFromGoogle!.IsReady;

                    return response;

                case AuthProviders.FIREBASE:

                    //verify firebase token
                    var firebaseToken = await FirebaseAuth.DefaultInstance!.VerifyIdTokenAsync(request.IdToken);
                    if (firebaseToken is null) return new("Invalid Token.");

                    var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken.Uid);
                    if (firebaseUser is null) return new("Invalid Token.");

                    //check if registered
                    var firebaseLoginInfo = new UserLoginInfo(request.Provider.ToString(), firebaseUser.Uid, "Firebase");
                    var userFromFirebase = await _userManager.FindByLoginAsync(firebaseLoginInfo.LoginProvider, firebaseLoginInfo.ProviderKey);
                    var userToRegister = new User()
                    {
                        UserName = firebaseUser.Uid.GenerateUsernameForFirebase(firebaseUser.PhoneNumber),
                        Email = firebaseUser.Uid.GenerateEmailForFirebase(),
                        PhoneNumber = firebaseUser.PhoneNumber,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    //register if not
                    if (userFromFirebase is null) response = await SignUp(userToRegister, firebaseUser.Uid.GeneratePassword());
                    else response = await SignIn(userToRegister.Email, firebaseUser.Uid.GeneratePassword());

                    userFromFirebase = userFromFirebase is null ? await _userManager.FindByEmailAsync(userToRegister.Email) : userFromFirebase;

                    //add login
                    await _userManager.AddLoginAsync(userFromFirebase!, firebaseLoginInfo);

                    //check if registration complete
                    response.IsUserReady = userFromFirebase!.IsReady;
                    
                    return response;

                case AuthProviders.INTERNAL:
                case AuthProviders.APPLE:
                default:
                    return new("This provider is not supported.");
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

        public Task AddUserInfo(ProfileInformation request)
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
