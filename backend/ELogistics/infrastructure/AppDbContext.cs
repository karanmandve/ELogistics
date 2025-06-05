using domain.Model.States;
using domain.Model.Users;
using Microsoft.EntityFrameworkCore;
using core.Interface;
using domain.Model.Otp;
using domain.Model.Products;
using domain.Model.Cart;
using domain.Model.Sales;

namespace infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartMaster> CartMasters { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SalesDetail> SalesDetails { get; set; }

    }   
}
