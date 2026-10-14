using MapeamentoUrbano.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MapeamentoUrbano.Data.Seed
{
    public class SeedUsers
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = Guid.Parse("25B358C8-EDCF-437D-81A0-08DBF7295CA5"), UserName = "Master", UserEmail = "Master", FirstAccess = false, RoleId = Guid.Parse("75BAD1FA-EA44-418E-A6A8-4EECA6167EC9"), UserNameNormalized = "MASTER", UserPassword = "AITD5hKgy9KAb92qx88Wzid/4YxLHQJhkGDrwVt2rNlwjfyNc56vDg5NAUmuKeryRg==" }
            );
        }
    }
}
