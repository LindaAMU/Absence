using Absence.Domain.Enums;

namespace Absence.API.Models.AbsenceModels
{
    public class AbsenceListModel
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public AbsenceType AbsenceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RequestStatus RequestStatus { get; set; }

        public AbsenceListModel() { }

        public AbsenceListModel(int? id, string? email, AbsenceType absenceType, DateTime startDate, DateTime endDate, RequestStatus requestStatus)
        {
            Id = id;
            Email = email;
            AbsenceType = absenceType;
            StartDate = startDate;
            EndDate = endDate;
            RequestStatus = requestStatus;
        }
    }
}
