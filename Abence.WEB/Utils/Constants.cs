namespace Abence.WEB.Utils
{
    public class Constants
    {
        public static Dictionary<int, string> ROLES = new()
        {
            { 1, "Admin" },
            { 2, "User" }
        };

        public const string API_USER_GETALL = "API:User:GetAll";
        public const string API_USER_CREATE = "API:User:Create";
        public const string API_USER_UPDATE = "API:User:Update";
        public const string API_USER_DELETE = "API:User:Delete";
        public const string API_ABSENCE_CREATE = "API:Absence:Create";
        public const string API_ABSENCE_GETALL = "API:Absence:GetAll";
        public const string API_ABSENCE_UPDATESTATUS = "API:Absence:UpdateStatus";

        public enum AbsenceType { Vacation = 1, SkicLeave = 2, PersonalDay = 3 }
        public enum RequestStatus { Pending = 0, Approve = 1, Rejected = 2 }
    }
}
