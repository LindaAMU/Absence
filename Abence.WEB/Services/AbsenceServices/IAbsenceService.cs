using Abence.WEB.Models;
using Abence.WEB.Models.AbsenceModels;
using Abence.WEB.Utils;

namespace Abence.WEB.Services.AbsenceServices
{
    public interface IAbsenceService
    {
        public Task<StandardResponse> Create(AbsenceLightModel lightModel);
        public Task<List<AbsenceModel>> GetAll();
        public Task<StandardResponse> UpdateStatus(int absenceId, Constants.RequestStatus status);
    }
}
