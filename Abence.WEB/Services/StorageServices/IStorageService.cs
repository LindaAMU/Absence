using static Abence.WEB.Services.StorageServices.StorageService;

namespace Abence.WEB.Services.StorageServices
{
    public interface IStorageService
    {
        public void InitializeForClientSide();
        public Task<T> GetItem<T>(string key, StorageType type);
        public Task SetItem<T>(string key, T value, StorageType type);
        public Task RemoveItem(string key, StorageType type);
    }
}
