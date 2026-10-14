using MapeamentoUrbano.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.Data.Seed
{
    public class SeedMedals
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medals>().HasData(
                new Medals { Id = Guid.Parse("3c55e49f-fd3c-43ba-88d3-b990db7e5d01"), Name = "Ouro" },
                new Medals { Id = Guid.Parse("59c32bce-38ca-4d5d-a73f-76848fdf9f4d"), Name = "Prata" },
                new Medals { Id = Guid.Parse("043187e9-e471-43d8-8978-7fe73c83995d"), Name = "Bronze" }
            );
        }
    }
}
