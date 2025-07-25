using Abence.WEB.Models;
using Abence.WEB.Services.StorageServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Abence.WEB.Services.AuthServices
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IStorageService _storageService;
        private ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthStateProvider(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var accountObj = await _storageService.GetItem<UserModel>("_ut", StorageService.StorageType.LocalStorage);
                if (accountObj != null && !string.IsNullOrWhiteSpace(accountObj.Token))
                {
                    var trace = accountObj.Token;
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(trace), "jwt");
                    user = new ClaimsPrincipal(identity);
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                }
                else
                {
                    user = new ClaimsPrincipal(new ClaimsIdentity());
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                }
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                Console.WriteLine($"Error en GetAuthenticationStateAsync: {ex.Message}");
                throw; // Propagar la excepción
            }

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string trace)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(trace), "jwt");
            user = new ClaimsPrincipal(identity);

            // Notifica al sistema de cambios en el estado de autenticación
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            user = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(jwt);
            return jsonToken.Claims;
        }
    }
}
