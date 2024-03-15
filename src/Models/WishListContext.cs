using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wish.Models;

public class WishListContext : IdentityDbContext<IdentityUser>
{
    public WishListContext(DbContextOptions<WishListContext> options)
        : base(options)
    {
    }

    public DbSet<WishListItem> WishListItems => Set<WishListItem>();
}