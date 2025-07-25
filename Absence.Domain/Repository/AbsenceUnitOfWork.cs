using Absence.Domain.Context;
using Absence.Domain.Entities;

namespace Absence.Domain.Repository
{
    public class AbsenceUnitOfWork : IDisposable, IAbsenceUnitOfWork
    {
        protected AbsenceContext context;
        private bool _disposed;

        private AbsenceRepository<User> userRepository;
        private AbsenceRepository<AbsenceRequest> absenceRepository;
        private AbsenceRepository<Role> roleRepository;
        private AbsenceRepository<Session> sessionRepository;

        public AbsenceUnitOfWork(AbsenceContext context)
        {
            this.context = context;
        }

        public AbsenceRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new AbsenceRepository<User>((AbsenceContext)this.context);
                }
                return userRepository;
            }
        }

        public AbsenceRepository<Role> RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                {
                    this.roleRepository = new AbsenceRepository<Role>((AbsenceContext)this.context);
                }
                return roleRepository;
            }
        }

        public AbsenceRepository<AbsenceRequest> AbsenceRepository
        {
            get
            {
                if (this.absenceRepository == null)
                {
                    this.absenceRepository = new AbsenceRepository<AbsenceRequest>((AbsenceContext)this.context);
                }
                return absenceRepository;
            }
        }

        public AbsenceRepository<Session> SessionRepository
        {
            get
            {
                if (this.sessionRepository == null)
                {
                    this.sessionRepository = new AbsenceRepository<Session>((AbsenceContext)this.context);
                }
                return sessionRepository;
            }
        }

        public void Save()
        {
            this.context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
