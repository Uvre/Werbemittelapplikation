using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Kontakdaten
    {
        public int KontakdatenID { get; set; }

        [Required(ErrorMessage = "Angabe fehlt!")]
        [StringLength(32, ErrorMessage = "max 32 Zeichen")]
        public string Name { get; set; }

        [StringLength(32, ErrorMessage = "max 32 Zeichen")]
        public string Name2 { get; set; }

        [StringLength(32, ErrorMessage = "max 32 Zeichen")]
        public string Name3 { get; set; }

        [Required(ErrorMessage = "Angabe fehlt!")]
        [StringLength(32, ErrorMessage = "max 32 Zeichen")]
        public string Strasse { get; set; }

        [Required(ErrorMessage = "Angabe fehlt!")]
        [StringLength(15, ErrorMessage = "max 15 Zeichen")]
        public string PLZ { get; set; }

        [Required(ErrorMessage = "Angabe fehlt!")]
        [StringLength(32, ErrorMessage = "max 32 Zeichen")]
        public string Ort { get; set; }

        public virtual Land Land { get; set; }
        public string Telefon { get; set; }
        public string EMail { get; set; }
        public string Bemerkung { get; set; }
        public virtual Kunde Kunde { get; set; }
        public virtual Messe messe { get; set; }
        public virtual ICollection<Werbemittelauftrag> WMA { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PLZ.Length < 4)
            {
                yield return new ValidationResult("Es müssen mindestens 4 Zeichen eingetragen werden", new string[] { "Name" });
            }
            if (PLZ.Length < 15)
            {
                yield return new ValidationResult("Es können nicht mehr als 15 Zeichen eingetragen werden", new string[] { "Name" });
            }
        }
    }
}