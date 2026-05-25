using FluentValidation;
using M_TAU.Application.Mappers;
using M_TAU.Application.Services;
using M_TAU.Application.Validators.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace M_TAU.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(IdentityMappingProfile).Assembly));

        services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IChatSessionService, ChatSessionService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IMetricsService, MetricsService>();

        return services;
    }
}
