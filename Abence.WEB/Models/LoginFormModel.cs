using System.ComponentModel.DataAnnotations;

namespace Abence.WEB.Models
{
    public class LoginFormModel
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El correo debe tener un formato valido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.!#$*]*){0,}$", ErrorMessage = "La clave contiene caracteres no aceptados")]
        public string Password { get; set; }
    }
}