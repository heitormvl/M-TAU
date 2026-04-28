using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using M_TAU.Application.Mappers;
using M_TAU.Application.Validators.Identity;

namespace M_TAU.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(IdentityMappingProfile).Assembly));

        services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

        return services;
    }
}
