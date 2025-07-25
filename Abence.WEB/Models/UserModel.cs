using System.Data;

namespace Abence.WEB.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string? Message { get; set; }
    }
}
