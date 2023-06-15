using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models
{
    public class AppUsuario : IdentityUser
    {
        [Required(ErrorMessage ="El Campo Nombre es Obligatorio")]
        public string Nombre { get; set; }
    
        public int CodigoPais { get; set; }
        public string Telefono { get; set;}
        public string Pais { get; set;}
        public string Ciudad { get; set;}
        public string Direccion { get; set;}
        public DateTime FechaNacimiento { get; set;}
        public bool Estado { get; set; }

        [DataType(DataType.ImageUrl)] public string ImageUrl { get; set;}

        //Creamos un Constructor auxiliar Campos Autocompletado
        public AppUsuario()
        {
            Estado = true;
            Pais = "Argentina";
        }
    }
}
