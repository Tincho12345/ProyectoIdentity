using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class RegistroVM
    {
        [Required(ErrorMessage ="El Correo es Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="El Password es Obligatorio")]
        [StringLength(50,ErrorMessage ="El {0} debe tener mínimo {2} caracteres de Longitud", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La Confirmación es Obligatoria")]
        [Compare("Password",ErrorMessage ="La contraseña y confirmación No Coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El Campo Nombre es Obligatorio")]
        public string Nombre { get; set; }

        public string Url { get; set; }
        public int CodigoPais { get; set; }
        public string Telefono { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }
        public bool Estado { get; set; }

        [DataType(DataType.ImageUrl)] public string ImageUrl { get; set; }

        //Creamos un Constructor auxiliar Campos Autocompletado
        public RegistroVM()
        {
            Estado = true;
            Pais = "Argentina";
        }
    }
}
