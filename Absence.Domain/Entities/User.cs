using Absence.Domain.Enums;

namespace Absence.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Enable;
        public ICollection<AbsenceRequest> AbsenceRequests { get; set; } = [];
        public ICollection<Role> Roles { get; set; } = [];
        public Session? Session { get; set; }
    }
}
