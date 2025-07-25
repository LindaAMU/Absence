namespace Abence.WEB.Services.HttpServices
{
    public interface IHttpService
    {
        public Task<T> Get<T>(string uri);
        public Task<T> Post<T>(string uri, object value);
    }
}
