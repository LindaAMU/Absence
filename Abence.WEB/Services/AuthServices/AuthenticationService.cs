using Abence.WEB.Models;
using Abence.WEB.Services.HttpServices;
using Abence.WEB.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Abence.WEB.Services.AuthServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly NavigationManager _nav;
        private readonly AuthStateProvider _authStateProvider;

        public AuthenticationService(IConfiguration configuration, IHttpService httpService, NavigationManager nav, AuthenticationStateProvider authProvider)
        {
            _configuration = configuration;
            _httpService = httpService;
            _nav = nav;
            _authStateProvider = (AuthStateProvider)authProvider;
        }

        public async Task Login(LoginFormModel model)
        {
            try
            {
                UserModel response = await _httpService.Post<UserModel>(_configuration.GetSection("API:Auth").Value, model);
                _nav.NavigateTo("/Inicio");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Logout()
        {
            _authStateProvider.MarkUserAsLoggedOut();
        }
    }
}