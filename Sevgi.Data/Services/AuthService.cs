using Dapper;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sevgi.Data.Database;
using Sevgi.Data.Utilities;
using Sevgi.Model;
using Sevgi.Model.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly DapperContext _dapperContext;


        public AuthService(UserManager<User> userManager, IConfiguration configuration, DapperContext context)
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
            if (!userToCheck.IsActive) return new("User is not allowed.", !userToCheck.IsActive);
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

                    #region REMOVE_WHEN_POSSIBLE
                    //register if not
                    if (userFromFirebase is null)
                    {
                        //check for internal user first
                        var checkUserQuery = "SELECT * FROM Users U WHERE U.PhoneNumber = '@PhoneNumber'";
                        using var connection = _dapperContext.CreateConnection();
                        var checkedUser = await connection.QueryFirstOrDefaultAsync<User>(checkUserQuery, firebaseUser.PhoneNumber);
                        //if exists login
                        if (checkedUser is not null)
                        {
                            //run internal login
                            response = await SignIn(userToRegister.Email, checkedUser.PhoneNumber!.GeneratePassword());
                        }
                        else
                        {
                            //if not, continue with firebase signup process
                            response = await SignUp(userToRegister, firebaseUser.Uid.GeneratePassword());
                        }
                    }
                    else
                    {
                        response = await SignIn(userToRegister.Email, firebaseUser.Uid.GeneratePassword());
                    } 
                    #endregion

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
