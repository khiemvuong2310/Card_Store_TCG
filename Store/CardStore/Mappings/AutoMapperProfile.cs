using AutoMapper;
using CardStore.Models;
using CardStore.DTOs;
using CardStore.Services;

namespace CardStore.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Card mappings
        CreateMap<Card, CardDto>();
        CreateMap<CreateCardDto, Card>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Collections, opt => opt.Ignore());
        
        CreateMap<UpdateCardDto, Card>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Collections, opt => opt.Ignore());

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Collections, opt => opt.Ignore());

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Collections, opt => opt.Ignore());

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.Card.Name))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ShippedDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveredDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<CreateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Card, opt => opt.Ignore());
    }
}