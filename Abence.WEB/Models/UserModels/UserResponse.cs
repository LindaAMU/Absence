namespace Abence.WEB.Models.UserModels
{
    public class UserResponse : StandardResponse
    {
        public List<UserLightModel> Users { get; set; }
    }
}
