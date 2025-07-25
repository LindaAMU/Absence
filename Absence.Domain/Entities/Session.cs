namespace Absence.Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }        
        public Guid SessionCode { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
