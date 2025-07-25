namespace Absence.API.Models.UserModels
{
    public class UserRequest
    {
        public int? Id { get; set; } 
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        
    }
}
