using backend_wishlist.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_wishlist.Data;

public class WishlistContext : DbContext
{
    public WishlistContext(DbContextOptions<WishlistContext> options) : base(options) { }

    public DbSet<WishList> WishLists { get; set; }
}
