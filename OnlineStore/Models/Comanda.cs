
using System.ComponentModel.DataAnnotations;

namespace COM {

    public class Comanda
    {
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
        ErrorMessage = "Characters and numbers are not allowed !")]
        public string Nume { get; set; }
        [Required]
        public string Prenume { get; set; }
        [Required]
        public string AdresaLivrare { get; set; }
        [Required]
        public long NumarTelefon { get; set; }
        [Required]
        public string Email { get; set; }

    }


}