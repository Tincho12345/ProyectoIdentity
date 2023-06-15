using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class OlvidoPasswordVM
    {
        [Required(ErrorMessage ="El Correo es Obligatorio")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
