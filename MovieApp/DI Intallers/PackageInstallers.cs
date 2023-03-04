using GOSBackend.Configurations;
using GOSBackend.Data;
using GOSBackend.SqlTables;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.Options;
using GOSLibraries.URI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Support.SDK;
using System.Text;

namespace GOSBackend.DI_Intallers
{
    public class PackageInstallers : IInstallers
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);

            services.AddSingleton(jwtSettings);
            var tokenValidatorParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
            };

            services.AddSingleton(tokenValidatorParameters);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidatorParameters;
            });
            services.AddDbContext<DataBaseContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<Users>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataBaseContext>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IBaseURIs>(configuration.GetSection("BaseURIs").Get<BaseURIs>());
            services.AddSingleton<IEmailConfiguration>(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());


           

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            services.AddDetection();

        }
    }
}
