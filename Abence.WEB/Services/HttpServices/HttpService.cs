using Abence.WEB.Models;
using Abence.WEB.Services.AuthServices;
using Abence.WEB.Services.StorageServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Abence.WEB.Services.HttpServices
{

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _stateProvider;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;
        private readonly String _baseUrl;

        private HttpService (HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration, IStorageService storageService)
        {
            _httpClient = httpClient;
            _stateProvider = authenticationStateProvider;
            _configuration = configuration;
            _storageService = storageService;
            _baseUrl = _configuration.GetSection("BaseURL").Value;
        }

        public async Task<T> Get<T>(string uri)
        {
            HttpRequestMessage request = new(HttpMethod.Get, _baseUrl + uri);
            try
            {
                return await SendRequest<T>(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<T> Post<T>(string uri, object value)
        {
            HttpRequestMessage request = new(HttpMethod.Post, _baseUrl + uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            try
            {
                return await SendRequest<T>(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            try
            {
                /* Agregar Bearer token */
                UserModel storageUser = await _storageService.GetItem<UserModel>("_um", StorageService.StorageType.LocalStorage);
                if (storageUser != null && !String.IsNullOrEmpty(storageUser.Token))
                {
                    if(IsTokenExpired(storageUser.Token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storageUser.Token);
                    }
                }
                else
                {
                    /* Lo enviamos a iniciar sesión */
                }
                /* Enviar consulta */
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                /* Procesar errores */
                if (response != null)
                {
                    //TODO - Gestionar errores
                }

                if (typeof(UserModel).IsAssignableFrom(typeof(T)))
                {
                    UserModel user = await response.Content.ReadFromJsonAsync<UserModel>();
                    if (user.Token == null || user != null || user.Token == "")
                    {
                        //Todo - Gestionar error de inicio de sesión.

                    }
                    else
                    {
                        user = UserModelFromToken(user.Token);
                        JwtSecurityToken securityToken = new JwtSecurityTokenHandler().ReadJwtToken(user.Token);
                        int role = int.Parse(securityToken.Claims.First(c => c.Type == "role").Value);
                        string sc = securityToken.Claims.First(c => c.Type == "sc").Value;
                        int userId = int.Parse(securityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

                        ClaimsIdentity claims = new(new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, role.ToString()),
                            new Claim("sc", sc),
                            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                        }, "Auth");

                        ClaimsPrincipal claimsPrincipal = new(claims);
                        try
                        {
                            /* Generar Nuevo Token */
                            /* [Doc: v_def_d#, Est: v_def_s#]*/
                            string status = "";
                            await _storageService.SetItem("_um", user, StorageService.StorageType.LocalStorage);
                            await ((AuthStateProvider)_stateProvider).MarkUserAsAuthenticated(user.Token);

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        return default;
                    }
                }



                    /* Entregar respuesta al servicio(controller) */
                    return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private UserModel UserModelFromToken(string token)
        {
            JwtSecurityToken securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string email = securityToken.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            string role = securityToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            string userName = securityToken.Claims.First(c => c.Type == "UserName").Value;
            string userLastName = securityToken.Claims.First(c => c.Type == "UserLastName").Value;
            string nameIdentifier = securityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            // Retornar un UserModel con los datos extraídos
            return new UserModel
            {
                Id = int.Parse(nameIdentifier),
                Email = email,
                Token = token
            };
        }

        private bool IsTokenExpired(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(jwt);
            var expiryClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp");
            if (expiryClaim != null)
            {
                var expiryDateUnix = long.Parse(expiryClaim.Value);
                var expiryDateUtc = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;
                return expiryDateUtc < DateTime.UtcNow;
            }
            return true;
        }
    }
}
