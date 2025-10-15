namespace MyEmailWebApi.DTO.UserDto
{
    public class UserProfileResponse
    {
        public string? Email { get; set; }
        public IEnumerable<string> Departments { get; set; } = [];
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
