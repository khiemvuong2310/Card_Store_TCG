using CardStore.Data.Repositories;
using CardStore.Services;
using CardStore.Mappings;
using CardStore.Validators;
using FluentValidation;

namespace CardStore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCardStoreServices(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));
        
        // Add FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateCardDtoValidator>();
        
        // Add repositories
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        
        // Add services
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}