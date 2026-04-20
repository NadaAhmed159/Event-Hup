using EventHub.BLL.Configuration;
using EventHub.BLL.Services.Implementations;
using EventHub.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<AttachmentStorageOptions>(configuration.GetSection(AttachmentStorageOptions.SectionName));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAttachmentService, AttachmentService>();

            return services;
        }
    }
}
