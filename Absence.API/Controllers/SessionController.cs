using Absence.API.Models;
using Absence.API.Models.AbsenceModels;
using Absence.API.Models.SessionModels;
using Absence.Domain.Entities;
using Absence.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Absence.API.Controllers
{
    [ApiController, Route("[Controller]")]
    public class SessionController : GenericController
    {
        public SessionController(IAbsenceUnitOfWork absenceUnitOfWork, IConfiguration configuration) : base(absenceUnitOfWork, configuration)
        {
        }

        [HttpPost("[action]")]
        [EnableCors("PolicyCors")]
        public IActionResult Login([FromBody] SessionRequest request)
        {
            SessionResponse response = new();
            try
            {
                /* Validar Requester */
                var user = _absenceUnitOfWork.UserRepository.Get(u => u.Email.Equals(request.Email)).FirstOrDefault();
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User Not Found";
                    return NotFound(response);
                }

                /* Validar si la contraseña es correcta */
                if (!HashVerify(request.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Unauthorized user.";
                    return Unauthorized(response);
                }

                /* Se crea la sesión */
                var sessionCode = CreateOrRefresh(user.Id, TimeSpan.FromHours(72));

                /* Se genera token */
                var userRole = _absenceUnitOfWork.RoleRepository.Get(r => r.UserId == user.Id).FirstOrDefault();
                var (jwt, exp) = JwtCreate(user.Id, sessionCode, userRole.Id);

                response.Success = true;
                response.Message = jwt;
                response.ExpirationTime = exp;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        private (string jwt, DateTime expires) JwtCreate(int userId, string sessionCode, int roleId)
        {
            var secret = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
            var key = new SymmetricSecurityKey(secret);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(72);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("sc", sessionCode),
                new Claim("role", roleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:ValidIssue"]),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:ValidAudience"])
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssue"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private string CreateOrRefresh(int userId, TimeSpan lifeTime)
        {
            var code = Guid.NewGuid();
            var expires = DateTime.UtcNow.Add(lifeTime);
            var userSession = _absenceUnitOfWork.SessionRepository.Get(s => s.UserId == userId).FirstOrDefault();

            /* Si no existe la sesión, se crea */
            if (userSession == null)
            {
                userSession = new()
                {
                    UserId = userId
                };
                _absenceUnitOfWork.SessionRepository.Insert(userSession);
            }

            /* Se actializan los datos de la sesión */
            userSession.SessionCode = code;
            userSession.ExpiresAt = expires;

            _absenceUnitOfWork.Save();
            return code.ToString();
        }

        private bool HashVerify(string pass, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] hashFromStorage = Convert.FromBase64String(parts[2]);

            byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                hashFromStorage.Length);

            return CryptographicOperations.FixedTimeEquals(hashFromStorage, hashToCompare);
        }
    }
}