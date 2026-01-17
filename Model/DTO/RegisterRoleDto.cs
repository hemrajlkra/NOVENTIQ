namespace NOVENTIQ.Model.DTO
{
    public class RegisterRoleDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}
