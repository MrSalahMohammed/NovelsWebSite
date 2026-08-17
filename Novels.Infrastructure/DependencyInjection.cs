using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Novels.Core.Interfaces.Repositories;
using Novels.Infrastructure.Data;
using Novels.Infrastructure.Repositories;

namespace Novels.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var ConnectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));

            services.AddScoped<IReaderRepository, ReaderRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IFavouritRepository, FavouritRepository>();
            services.AddScoped<INovelRepository, NovelRepository>();
            services.AddScoped<IReadingProgressRepository, ReadingProgressRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            return services;
        }
    }
}
