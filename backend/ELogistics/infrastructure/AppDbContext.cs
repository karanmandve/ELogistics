using domain.Model.States;
using domain.Model.Users;
using Microsoft.EntityFrameworkCore;
using core.Interface;
using domain.Model.Otp;
using domain.Model.Products;

namespace infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Product> Products { get; set; }

    }   
}
