using Abence.WEB.Models;
using Abence.WEB.Models.UserModels;

namespace Abence.WEB.Services.UserServices
{
    public interface IUserServices
    {
        public Task<List<UserLightModel>> GetAll();
        public Task<StandardResponse> Create(LoginFormModel user);
        public Task<StandardResponse> Update(UserModel user);
        public Task<StandardResponse> Delete(UserModel user);
    }
}
