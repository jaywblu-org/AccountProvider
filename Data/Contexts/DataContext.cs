using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<AddressEntity> Address { get; set; }
}
