using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Abence.WEB.Services.AuthServices
{
    public interface IAuthStateProvider
    {
        public Task<AuthenticationState> GetAuthenticationStateAsync();
        public Task MarkUserAsAuthenticated(string token);
        public void MarkUserAsLoggedOut();
        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt);
    }
}
