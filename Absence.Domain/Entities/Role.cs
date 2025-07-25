namespace Absence.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
