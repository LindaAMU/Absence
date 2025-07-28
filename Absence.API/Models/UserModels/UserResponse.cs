namespace Absence.API.Models.UserModels
{
    public class UserResponse : StandardResponse
    {
        public List<UserRequest> Users { get; set; }
    }
}
