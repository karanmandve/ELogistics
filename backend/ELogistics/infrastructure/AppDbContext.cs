using core.Interface;
using domain.Model.Appointment;
using domain.Model.CountryState;
using domain.Model.Otp;
using domain.Model.SoapNotes;
using domain.Model.User;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Specialisation> Specialisations { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<SoapNote> SoapNotes { get; set; }


        public IDbConnection GetConnection()
        {
            return this.Database.GetDbConnection();
        }
    }
}
