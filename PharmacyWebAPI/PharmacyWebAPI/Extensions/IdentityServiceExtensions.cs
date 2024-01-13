using PharmacyWebAPI.DataAccess;

namespace PharmacyWebAPI.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()/*
                .AddDefaultTokenProviders()*/;

            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.SignIn.RequireConfirmedAccount = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}