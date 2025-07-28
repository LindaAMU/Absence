using Abence.WEB.Models.UserModels;
using Abence.WEB.Services.StorageServices;
using Abence.WEB.Utils;
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
        private readonly string _baseUrl;

        public HttpService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration, IStorageService storageService)
        {
            _configuration = configuration;
            _stateProvider = authenticationStateProvider;
            _storageService = storageService;
            _httpClient = httpClient;
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
                UserModel storedUser = await _storageService.GetItem<UserModel>("_um", StorageService.StorageType.LocalStorage);
                if (storedUser != null && !String.IsNullOrEmpty(storedUser.Message))
                {
                    if (!IsTokenExpired(storedUser.Message))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedUser.Message);
                    }
                }
                else
                {
                    /* Lo enviamos a iniciar sesión */
                }



                /* Enviar consulta */
                using HttpResponseMessage response = await _httpClient.SendAsync(request);
                /* Procesar errores */
                if (!response.IsSuccessStatusCode)
                {
                    // TODO - Gestionar Errores
                }

                if (typeof(UserModel).IsAssignableFrom(typeof(T)))
                {
                    UserModel user = await response.Content.ReadFromJsonAsync<UserModel>();
                    if (user == null || user.Message == null || user.Message == "")
                    {
                        // TODO - Gestionar Error de Inicio de Sesión
                    }
                    else
                    {
                        user = GetUserModelFromToken(user.Message);
                        try
                        {
                            await _storageService.SetItem("_um", user, StorageService.StorageType.LocalStorage);
                            await ((AuthStateProvider)_stateProvider).MarkUserAsAuthenticated(user.Message);
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

        private UserModel GetUserModelFromToken(string token)
        {
            JwtSecurityToken securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string role = securityToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            string sc = securityToken.Claims.First(c => c.Type == "sc").Value;
            string email = securityToken.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            string nameIdentifier = securityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return new UserModel
            {
                Id = int.Parse(nameIdentifier),
                Email = email,
                Role = role,
                Message = token,
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
