namespace Absence.API.Models.UserModels
{
    public class UserRequest
    {
        public int? Id { get; set; } 
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        public UserRequest(int id, string email) 
        { 
            Id = id;
            Email = email;
        }
        public UserRequest() { }
    }
}
