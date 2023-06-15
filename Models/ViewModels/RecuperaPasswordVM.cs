using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class RecuperaPasswordVM
    {
        [Required(ErrorMessage = "El Correo es Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Password es Obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El Password es Obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}


