namespace MapeamentoUrbano.Domain.DTOs.Role
{
    public class RoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public RoleDTO()
        {
        }

        public RoleDTO(Entities.Role role)
        {
            Id = role.Id;
            Name = role.RoleName;
        }
    }
}
