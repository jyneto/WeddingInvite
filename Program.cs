using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data.SqlTypes;
using System.Security.Claims;
using System.Text;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Implementations;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Implemetations;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Logging.AddConsole();

            builder.Services.AddDbContext<WeddingDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });






            //Repository
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IGuestRepository, GuestRepository>();
            builder.Services.AddScoped<IBookingRespository, BookingRepository>();
            builder.Services.AddScoped<ITableRespiratory, TableRepository>();
            builder.Services.AddScoped<IMenuRepository, MenuRepository>();

            //Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGuestService, GuestService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ITableService, TableService>();
            builder.Services.AddScoped<IMenuService, MenuService>();


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token like this: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


            builder.Configuration.AddUserSecrets<Program>();


            // Sanity checks (logga ej nyckeln, bara l�ngden)
            Console.WriteLine("JWT Issuer   = " + builder.Configuration["Jwt:Issuer"]);
            Console.WriteLine("JWT Audience = " + builder.Configuration["Jwt:Audience"]);
            Console.WriteLine("JWT Key len  = " + (builder.Configuration["Jwt:Key"]?.Length ?? 0));

            // Fail fast om saknas
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Missing Jwt:Key (User Secrets?)");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role
                    };
                    
                });

            builder.Services.AddAuthorization();



            var app = builder.Build();

            // L�gg till en admin-anv�ndare om ingen finns
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WeddingDbContext>();
                db.Database.Migrate(); // skapar DB/tabeller om saknas

                if (!db.Admins.Any())
                {
                    db.Admins.Add(new Admin
                    {
                        UserName = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123")
                    });
                    db.SaveChanges();
                }
            }
 

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

