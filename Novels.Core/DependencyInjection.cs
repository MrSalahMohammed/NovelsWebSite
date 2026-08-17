using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Novels.core.Security;
using Novels.Core.Interfaces.Services;
using Novels.Core.Services;
using System.Text;

namespace Novels.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped<IReaderService, ReaderService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ITokenService, JwtTokenService>();

            services.AddScoped<INovelService, NovelService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IReadingProgressService, ReadingProgressService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IAdminService, AdminService>();

            var jwtSection = configuration.GetSection("Jwt");
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // add this line
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSection["Key"]!)
                        ),
                    };
                });

            services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            );

            return services;
        }
    }
}
