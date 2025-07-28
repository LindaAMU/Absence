using Abence.WEB.Models.UserModels;
using Abence.WEB.Services.AuthServices;
using Abence.WEB.Services.StorageServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Abence.WEB.Utils
{
    public class AuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
    {
        private readonly IStorageService _storageService;
        private ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthStateProvider(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal principal;

            try
            {
                var accountObj = await _storageService.GetItem<UserModel>("_um", StorageService.StorageType.LocalStorage);
                if (accountObj != null && !string.IsNullOrWhiteSpace(accountObj.Message))
                {
                    var trace = accountObj.Message;
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(trace), "jwt");
                    principal = new ClaimsPrincipal(identity);
                }
                else
                {
                    principal = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAuthenticationStateAsync: {ex.Message}");
                principal = new ClaimsPrincipal(new ClaimsIdentity());
            }

            user = principal;
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string trace)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(trace), "jwt");
            user = new ClaimsPrincipal(identity);

            
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async void MarkUserAsLoggedOut()
        {
            user = new ClaimsPrincipal(new ClaimsIdentity());
            await _storageService.RemoveItem("_um", StorageService.StorageType.LocalStorage);
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(jwt);
            return jsonToken.Claims;
        }
    }
}