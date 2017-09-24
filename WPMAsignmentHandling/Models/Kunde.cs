using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Kunde
    {
        public int KundeID { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Erstellungsdatum { get; set; }
        [Required(ErrorMessage = "Angabe fehlt!")]
        //[StringLength(32, ErrorMessage = "nur 32 möglich")]
        public string Name { get; set; }
        public virtual Kontakdaten Hauptadresse { get; set; }
        public virtual ICollection<Kontakdaten> kontakdaten { get; set; }
        public virtual ICollection<Werbemittelauftrag> auftraege { get; set; }
        public virtual ICollection<Artikel> artikel { get; set; }
    }
}