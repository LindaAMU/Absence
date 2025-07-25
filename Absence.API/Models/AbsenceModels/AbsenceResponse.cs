using Absence.Domain.Entities;

namespace Absence.API.Models.AbsenceModels
{
    public class AbsenceResponse : StandardResponse
    {
        public List<AbsenceRequest> Results { get; set; }
    }
}
