using AutoMapper;
using backend_wishlist.DTO;
using backend_wishlist.Models;

namespace wishlist.Profiles;

public class WishlistProfile : Profile
{
    public WishlistProfile()
    {
        CreateMap<WishList, WishList>();
        CreateMap<WishlistCreate, WishList>();
    }
}
