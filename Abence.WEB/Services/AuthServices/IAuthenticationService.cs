using Abence.WEB.Models;

namespace Abence.WEB.Services.AuthServices
{
    public interface IAuthenticationService
    {
        public Task Login(LoginFormModel model);
        public Task Logout();
    }
}
