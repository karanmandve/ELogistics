using core.Interface;
using core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            return services;
        }
    }
}
