using Abence.WEB.Components.Pages.Users;
using Abence.WEB.Models;
using Abence.WEB.Models.AbsenceModels;
using Abence.WEB.Services.HttpServices;
using Abence.WEB.Utils;

namespace Abence.WEB.Services.AbsenceServices
{
    public class AbsenceService : IAbsenceService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;

        public AbsenceService(IConfiguration configuration, IHttpService httpService)
        {
            _configuration = configuration;
            _httpService = httpService;
        }

        public async Task<StandardResponse> Create(AbsenceLightModel lightModel)
        {
            try
            {
                StandardResponse response = await _httpService.Post<StandardResponse>(_configuration.GetSection(Constants.API_ABSENCE_CREATE).Value, lightModel);
                if (response != null || response.Success)
                {
                    return response;
                }
                    return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AbsenceModel>> GetAll()
        {
            try
            {
                AbsenceResponse response = await _httpService.Get<AbsenceResponse>(_configuration.GetSection(Constants.API_ABSENCE_GETALL).Value);
                if (response.Success)
                {
                    return response.Results;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StandardResponse> UpdateStatus(int absenceId, Constants.RequestStatus status)
        {
            try
            {
                StandardResponse response = await _httpService.Post<StandardResponse>(_configuration.GetSection(Constants.API_ABSENCE_UPDATESTATUS).Value + $"?absenceId={absenceId}&status={status}", null);
                if (response != null || response.Success)
                {
                    return response;
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
