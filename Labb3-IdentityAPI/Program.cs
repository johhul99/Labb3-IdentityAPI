
using Labb3_IdentityAPI.Data;
using Labb3_IdentityAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb3_IdentityAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<LoginContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapPost("/login", async (LoginContext db, Login login) =>
            {
                var user = await db.Logins.FirstOrDefaultAsync(u => u.Username == login.Username);

                if (user == null || user.Password != login.Password)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new { Message = "Login successful" });
            });


            app.MapPost("/register", async (LoginContext db, Login login) =>
            {
                if (await db.Logins.AnyAsync(u => u.Username == login.Username))
                {
                    return Results.BadRequest("Username already exists." );
                }

                db.Logins.Add(login);
                await db.SaveChangesAsync();
                return Results.Ok(new { Message = "User registered successfully!" });

            });


            app.Run();
        }
    }
}
