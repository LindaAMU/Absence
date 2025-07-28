using static Abence.WEB.Utils.Constants;

namespace Abence.WEB.Models.AbsenceModels
{
    public class AbsenceLightModel
    {
        public AbsenceType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
