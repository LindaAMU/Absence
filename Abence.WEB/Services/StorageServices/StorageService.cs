using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Abence.WEB.Services.StorageServices
{
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IDataProtector _dataProtector;
        private bool _isPredendering;

        public StorageService(IJSRuntime jSRuntime, IDataProtectionProvider dataProtector)
        {
            _jsRuntime = jSRuntime;
            _isPredendering = true;
            _dataProtector = dataProtector.CreateProtector("StorageService");
        }

        public void InitializeForClientSide()
        {
            _isPredendering = false;
        }

        public async Task<T> GetItem<T>(string key, StorageType type)
        {
            if (_isPredendering)
            {
                return default;
            }
            var storage = type == StorageType.LocalStorage ? "localStorage" : "sessionStorage";
            var encryptedJson = await _jsRuntime.InvokeAsync<string>($"{storage}.getItem", key);
            if (encryptedJson == null)
            {
                return default;
            }
            try
            {
                var json = _dataProtector.Unprotect(encryptedJson);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                return default;
            }

        }

        public async Task SetItem<T>(string key, T value, StorageType type)
        {
            var storage = type == StorageType.LocalStorage ? "localStorage" : "sessionStorage";
            var json = JsonSerializer.Serialize(value);
            var encryptedJson = _dataProtector.Protect(json);
            await _jsRuntime.InvokeVoidAsync($"{storage}.setItem", key, encryptedJson);
        }

        public async Task RemoveItem(string key, StorageType type)
        {
            var storage = type == StorageType.LocalStorage ? "localStorage" : "sessionStorage";
            await _jsRuntime.InvokeVoidAsync($"{storage}.removeItem", key);
        }

        public enum StorageType
        {
            LocalStorage = 1,
            SessionStorage = 2
        }
    }
}
