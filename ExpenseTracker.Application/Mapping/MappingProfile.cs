using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>().ReverseMap();

        // Category
        CreateMap<Category, CategoryDto>().ReverseMap();

        // Transaction
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ReverseMap()
            .ForMember(dest => dest.Category, opt => opt.Ignore()); // Avoid circular mapping
    }
}