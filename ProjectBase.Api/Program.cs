using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectBase.Data.Database;
using ProjectBase.Model;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


#region Identity

//adding identity context
builder.Services.AddDbContext<BaseIdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BaseProjectDb")));

//adding indentity
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<BaseIdentityContext>()
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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
