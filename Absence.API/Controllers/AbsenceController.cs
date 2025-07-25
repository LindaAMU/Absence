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

                /* Validar Fechas */
                DateTime dateTime = DateTime.UtcNow;

                if (request.StartDate >= request.EndDate)
                {
                    response.Success = false;
                    response.Message = "Invalid application date.";
                    return BadRequest(response);
                }

                /* Insertamos el AbsenceRequest */
                AbsenceRequest newRequest = new()
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                    UserId = request.UserId,
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
                if (user.Roles.Any(r => r.Name.Equals(Constants.S_ROLE_ADMIN)))
                {
                    results = _absenceUnitOfWork.AbsenceRepository.Get().ToList();
                }
                /* Resultados Rol: User*/
                else
                {
                    results = _absenceUnitOfWork.AbsenceRepository.Get(a => a.UserId == user.Id).ToList();
                }

                response.Success = true;
                response.Message = $"{results.Count} results were found";
                return Ok(results);
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
        public IActionResult UpdateStatus([FromBody] int absenceId, RequestStatus status)
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

                /* Validamos Rol */
                if (!user.Roles.Any(r => r.Name.Equals(Constants.S_ROLE_ADMIN)))
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

                if (status != RequestStatus.Pending)
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