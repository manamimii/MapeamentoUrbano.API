using MapeamentoUrbano.Data.Seed;
using MapeamentoUrbano.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.Data.Context
{
    public class CoreContext : DbContext
    {
        public DbSet<Address> Address { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<LogBase> Log { get; set; }
        public DbSet<Role>? Role { get; set; }
        public DbSet<User>? User { get; set; }

        public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region [One to one]
            #endregion

            #region [Seeds]
            SeedRoles.Seed(modelBuilder);
            SeedUsers.Seed(modelBuilder);
            SeedMedals.Seed(modelBuilder);
            #endregion
        }
    }
}
