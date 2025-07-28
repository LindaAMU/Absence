using Absence.Domain.Entities;

namespace Absence.API.Models.AbsenceModels
{
    public class AbsenceResponse : StandardResponse
    {
        public List<AbsenceListModel> Results { get; set; }
    }
}
