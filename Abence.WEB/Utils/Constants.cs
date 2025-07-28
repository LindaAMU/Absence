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
    }
}
