using Absence.API.Models;
using Absence.API.Models.UserModels;
using Absence.API.Utils;
using Absence.Domain.Entities;
using Absence.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Absence.API.Controllers
{
    [ApiController, Route("api/[Controller]")]
    [EnableCors("PolicyCors")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : GenericController
    {
        public UserController(IAbsenceUnitOfWork absenceUnitOfWork, IConfiguration configuration) : base(absenceUnitOfWork, configuration)
        {

        }

        [HttpPost("[action]")]
        [EnableCors("PolicyCors")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Create([FromBody] UserRequest request)
        {
            StandardResponse response = new();
            try
            {
                /* Validar Requester y Rol */
                if (!ValidateRequester(User, out var user) && !user.Roles.Any(r => r.Name.Equals(Constants.S_ROLE_ADMIN)))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Validamos existencia del usuario a crear */
                var userToCreate = _absenceUnitOfWork.UserRepository.Get(u => u.Email.Equals(request)).FirstOrDefault();
                if (userToCreate != null)
                {
                    response.Success = false;
                    response.Message = "The user already exists.";
                    return BadRequest(response);
                }

                /* Insertar usuario en la tabla */
                User newUser = new()
                {
                    Email = request.Email,
                    PasswordHash = Hash(request.Password)
                };

                _absenceUnitOfWork.UserRepository.Insert(newUser);
                _absenceUnitOfWork.Save();

                response.Success = true;
                response.Message = "User Created";
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
        public IActionResult Update([FromBody] UserRequest request)
        {
            StandardResponse response = new();
            try
            {
                /* Validar Requester y Rol */
                if (!ValidateRequester(User, out var user) && !user.Roles.Any(r => r.Name.Equals(Constants.S_ROLE_ADMIN)))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Validar modelo */
                if (request.Id == null)
                {
                    response.Success = false;
                    response.Message = "Model invalid.";
                    return BadRequest(response);
                }

                /* Validamos existencia de usuario a modificar */
                var userToUpdate = _absenceUnitOfWork.UserRepository.Get(u => u.Id == request.Id).FirstOrDefault();
                if (userToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "User Not Found.";
                    return NotFound(response);
                }

                userToUpdate.Email = request.Email;
                userToUpdate.PasswordHash = Hash(request.Password);

                _absenceUnitOfWork.UserRepository.Update(userToUpdate);
                _absenceUnitOfWork.Save();

                response.Success = true;
                response.Message = "Authorized User";
                return Ok();
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
        public IActionResult Delete([FromBody] int id)
        {
            StandardResponse response = new();
            try
            {
                /* Validar Requester y Rol */
                if (!ValidateRequester(User, out var user) && !user.Roles.Any(r => r.Name.Equals(Constants.S_ROLE_ADMIN)))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Validamos Existencia de Usuario*/
                var userToDelete = _absenceUnitOfWork.UserRepository.Get(u => u.Id == id).FirstOrDefault();
                if(userToDelete == null)
                {
                    response.Success = false;
                    response.Message = "User Not Found.";
                    return NotFound(response);
                }

                _absenceUnitOfWork.UserRepository.Delete(userToDelete);
                _absenceUnitOfWork.Save();

                response.Success = true;
                response.Message = "User Deleted Successfully";
                return Ok();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        private string Hash(string pass)
        {
            int SaltSize = 16;
            int HashSize = 32;
            int Iterations = 100000;

            Span<byte> salt = stackalloc byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }
    }
}
