using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sevgi.Data.Database;
using Sevgi.Data.Services;
using Sevgi.Model;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Firebase
//create firebase instance if there is not
if (FirebaseAuth.DefaultInstance is null) FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(@"Infrastructure/FirebaseToken/firebaseAdminToken.json")
}); 
#endregion

#region Identity

//adding identity context
//builder.Services.AddDbContext<BaseIdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var serverVersion = new MySqlServerVersion(new Version(5, 7, 33));
builder.Services.AddDbContext<SevgiIdentityContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion)
    .EnableDetailedErrors()
);

//adding indentity
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<SevgiIdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    //this is where you can configure anything about users and authorization
    options.Password.RequireNonAlphanumeric = false;
    //options.SignIn.RequireConfirmedEmail = false;
    //options.SignIn.RequireConfirmedAccount = false;
    //options.User.RequireUniqueEmail = false;
});
#endregion

#region JWT

//adding JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

#endregion


//adding db context as singleton
builder.Services.AddSingleton<DapperContext>();

//adding authservice
builder.Services.AddScoped<IAuthService, AuthService>();

//adding repository services
////adding test repo
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUtilService, UtilService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sevgi API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

//adding CORS policy for dev client
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
         builder => builder
             .WithOrigins("http://82.165.242.81:8080")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

//use authentication. this is for protecting controlllers with auth service and identity
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
