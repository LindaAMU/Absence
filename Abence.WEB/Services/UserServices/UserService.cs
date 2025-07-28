using Abence.WEB.Models;
using Abence.WEB.Models.UserModels;
using Abence.WEB.Services.HttpServices;
using Abence.WEB.Utils;

namespace Abence.WEB.Services.UserServices
{
    public class UserService : IUserServices
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;

        public UserService(IConfiguration configuration, IHttpService httpService)
        {
            _configuration = configuration;
            _httpService = httpService;
        }

        public async Task<List<UserLightModel>> GetAll()
        {
            try
            {
                UserResponse response = await _httpService.Get<UserResponse>(_configuration.GetSection(Constants.API_USER_GETALL).Value);
                if (response.Success)
                {
                    return response.Users;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StandardResponse> Create(LoginFormModel user)
        {
            try
            {
                StandardResponse response = await _httpService.Post<StandardResponse>(_configuration.GetSection(Constants.API_USER_CREATE).Value, user);
                if (response.Success)
                {
                    return response;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StandardResponse> Update(UserModel user)
        {
            try
            {
                StandardResponse response = await _httpService.Post<StandardResponse>(_configuration.GetSection(Constants.API_USER_UPDATE).Value, user);
                if (response.Success)
                {
                    return response;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StandardResponse> Delete(UserModel user)
        {
            try
            {
                StandardResponse response = await _httpService.Post<StandardResponse>(_configuration.GetSection(Constants.API_USER_DELETE).Value + $"?Id={user.Id}", null);
                if (response.Success)
                {
                    return response;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
