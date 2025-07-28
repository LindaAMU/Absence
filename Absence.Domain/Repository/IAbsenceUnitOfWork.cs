using Absence.Domain.Entities;

namespace Absence.Domain.Repository
{
    public interface IAbsenceUnitOfWork
    {
        public AbsenceRepository<User> UserRepository { get; }        
        public AbsenceRepository<AbsenceRequest> AbsenceRepository { get; }
        public AbsenceRepository<Role> RoleRepository { get; }
        public AbsenceRepository<Session> SessionRepository { get; }
        public void Save();
        public void Dispose();

    }
}
