using MapeamentoUrbano.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.Data.Seed
{
    public class SeedRoles
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("75bad1fa-ea44-418e-a6a8-4eeca6167ec9"), RoleName = "Master" },
                new Role { Id = Guid.Parse("13124c76-5425-4494-8557-eef5db803bc8"), RoleName = "Administrador" },
                new Role { Id = Guid.Parse("591b47b6-c0e8-43fd-8a38-986a2f897f61"), RoleName = "User" }
            );
        }
    }
}
