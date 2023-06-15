using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class ConfirmacionAccesoExternoVM
    {
        [Required(ErrorMessage = "El Email es Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Nombre es Obligatorio")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
    }
}
