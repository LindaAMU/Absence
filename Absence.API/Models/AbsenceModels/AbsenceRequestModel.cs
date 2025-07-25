using Absence.Domain.Entities;
using Absence.Domain.Enums;

namespace Absence.API.Models.AbsenceModels
{
    public class AbsenceRequestModel
    {
        public AbsenceType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; } = default!;
    }
}
