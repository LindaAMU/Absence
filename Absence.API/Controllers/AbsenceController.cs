using Absence.API.Models;
using Absence.API.Models.AbsenceModels;
using Absence.API.Utils;
using Absence.Domain.Entities;
using Absence.Domain.Enums;
using Absence.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Absence.API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AbsenceController : GenericController
    {
        public AbsenceController(IAbsenceUnitOfWork absenceUnitOfWork, IConfiguration configuration) : base(absenceUnitOfWork, configuration)
        {
        }

        [HttpPost("[action]")]
        [EnableCors("PolicyCors")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Create([FromBody] AbsenceRequestModel request)
        {
            StandardResponse response = new();
            try
            {
                /* Validar Requester */
                if (!ValidateRequester(User, out var user))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }
                /* Validar Modelo */
                if (!Enum.IsDefined(typeof(AbsenceType), request.Type))
                {
                    response.Success = false;
                    response.Message = "Invalid type of request.";
                    return BadRequest(response);
                }

                /* Validar Fechas */

                if (request.StartDate >= request.EndDate)
                {
                    response.Success = false;
                    response.Message = "Invalid application date.";
                    return BadRequest(response);
                }

                var existingRequests = _absenceUnitOfWork.AbsenceRepository.Get().Where(r => r.UserId == user.Id);

                bool overlaps = existingRequests.Any(r =>
                    request.StartDate < r.EndDate &&
                    request.EndDate > r.StartDate
                );

                if (overlaps)
                {
                    response.Success = false;
                    response.Message = "There is already a request that overlaps with the selected dates.";
                    return Conflict(response);
                }

                /* Insertamos el AbsenceRequest */
                AbsenceRequest newRequest = new()
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                    UserId = user.Id,
                };
                _absenceUnitOfWork.AbsenceRepository.Insert(newRequest);
                _absenceUnitOfWork.Save();

                response.Success = true;
                response.Message = "The request has been created.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("[action]")]
        [EnableCors("PolicyCors")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetAll()
        {
            AbsenceResponse response = new();
            try
            {
                /* Validar Requester */
                if (!ValidateRequester(User, out var user))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Resultados Rol: Admin */
                List<AbsenceRequest> results = new();
                var userRole = _absenceUnitOfWork.RoleRepository.Get(r => r.UserId == user.Id).FirstOrDefault();
                if (userRole != null && userRole.Name.Equals("Admin"))
                {
                    results = _absenceUnitOfWork.AbsenceRepository.Get().ToList();
                }
                /* Resultados Rol: User*/
                else
                {
                    results = _absenceUnitOfWork.AbsenceRepository.Get(a => a.UserId == user.Id).ToList();
                }

                var userList = _absenceUnitOfWork.UserRepository.Get().Select(u => new {key = u.Id, value = u.Email}).ToDictionary(u => u.key, u => u.value);

                List<AbsenceListModel> list = results.Select(r => new AbsenceListModel()
                {
                    Id = r.Id,
                    Email = userList[r.UserId],
                    AbsenceType = r.Type,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    RequestStatus = r.Status
                }).ToList();

                response.Success = true;
                response.Message = $"{results.Count} results were found";
                response.Results = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("[action]")]
        [EnableCors("PolicyCors")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdateStatus(int absenceId, RequestStatus status)
        {
            StandardResponse response = new();
            try
            {
                /* Validar Requester */
                if (!ValidateRequester(User, out var user))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                var userRole = _absenceUnitOfWork.RoleRepository.Get(r => r.UserId == user.Id).FirstOrDefault();
                if (userRole != null && !userRole.Name.Equals("Admin"))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Validamos Existencia */
                var ar = _absenceUnitOfWork.AbsenceRepository.Get(a => a.Id == absenceId).FirstOrDefault();
                if (ar == null)
                {
                    response.Success = false;
                    response.Message = "Request not found";
                    return NotFound(response);
                }

                if (ar.Status != RequestStatus.Pending || status == RequestStatus.Pending)
                {
                    response.Success = false;
                    response.Message = "Unable to update status";
                    return BadRequest(response);
                }

                ar.Status = status;
                _absenceUnitOfWork.Save();

                response.Success = true;
                response.Message = "Status updated";
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

            }
            return Ok(response);
        }
    }
}