using Absence.Domain.Entities;
using Absence.Domain.Enums;
using Absence.Domain.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Absence.API.Controllers
{
    public abstract class GenericController : Controller
    {
        protected IAbsenceUnitOfWork _absenceUnitOfWork;
        protected IConfiguration _configuration;

        public GenericController(IAbsenceUnitOfWork absenceUnitOfWork, IConfiguration configuration)
        {
            _absenceUnitOfWork = absenceUnitOfWork;
            _configuration = configuration;
        }

        protected bool ValidateRequester(ClaimsPrincipal claims, out User user)
        {
            var requesterClaims = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            User requester = null;
            if (requesterClaims != null && requesterClaims.Value != null)
            {
                requester = _absenceUnitOfWork.UserRepository.Get(u => u.Email.Equals(requesterClaims.Value)).FirstOrDefault();
                if (requester == null || requester.Status == UserStatus.Disable)
                {
                    user = null;
                    return false;
                }
            }
            user = requester;
            return true;
        }
    }
}
