using Absence.Domain.Enums;

namespace Absence.Domain.Entities
{
    public class AbsenceRequest
    {
        public int Id { get; set; }
        public AbsenceType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public int UserId { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
