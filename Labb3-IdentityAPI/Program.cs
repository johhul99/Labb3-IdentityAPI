
using Labb3_IdentityAPI.Data;
using Labb3_IdentityAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labb3_IdentityAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<LoginContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<LoginContext>()
                .AddDefaultTokenProviders();

            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? builder.Configuration["Jwt:Key"];

            var key = Encoding.UTF8.GetBytes(jwtKey);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Add services to the container.
            builder.Services.AddAuthorization();

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

            app.UseAuthentication();

            app.UseAuthorization();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapPost("/login", async (UserManager<IdentityUser> userManager, IConfiguration config, [FromBody] LoginModel model) =>
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Results.Unauthorized();
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
            });
                

            app.MapPost("/register", async (UserManager<IdentityUser> userManager, [FromBody] RegisterModel model) =>
            {
                var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                return result.Succeeded ? Results.Ok("User created!") : Results.BadRequest(result.Errors);

            });

            app.MapGet("/protected", (HttpContext context) =>
            {
                return Results.Ok($"{context.User.Identity?.Name}, has access to the protected page!");
            }).RequireAuthorization();


            app.Run();
        }
    }
}
