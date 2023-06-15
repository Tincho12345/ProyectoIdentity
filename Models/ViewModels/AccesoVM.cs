using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AccesoVM
    {
        [Required(ErrorMessage ="El Correo es Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="El Password es Obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name ="Recordarme?")]
        public bool RememberMe { get; set; }
    }
}
