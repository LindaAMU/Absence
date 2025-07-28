namespace Abence.WEB.Models.UserModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? Message { get; set; }

        public UserModel() { }
        public UserModel(int id, string email)
        {
            Id = id;
            Email = email;
        }
    }
}
