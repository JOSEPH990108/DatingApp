using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace API.Extensions;

public static class ApplicationServiceExtensions 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // services.AddDbContext<ApplicationDbContext>(opt => {
        //     // opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        //     opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        // });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        // services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        // services.AddScoped<ILikesRepository, LikesRepository>();
        // services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
