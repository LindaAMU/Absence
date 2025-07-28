using Abence.WEB.Utils;

namespace Abence.WEB.Models.AbsenceModels
{
    public class AbsenceModel
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public Constants.AbsenceType AbsenceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Constants.RequestStatus RequestStatus { get; set; }
    }
}
