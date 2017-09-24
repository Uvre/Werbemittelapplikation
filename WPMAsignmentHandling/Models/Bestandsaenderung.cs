using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Bestandsaenderung
    {
        
        public int BestandsaenderungID { get; set; }
        
        [Required(ErrorMessage="Kein Grund angegeben")]
        public string Grund { get; set; }



        public string Bemerkung { get; set; }



        [Required(ErrorMessage = "Kein Menge angegeben")]
        [RegularExpression(@"^[-0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        public int Menge { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Datum { get; set; }
        public int ArtikelID { get; set; }
        public virtual Artikel Artikel { get; set; }
        public virtual Werbemittelauftrag WMA { get; set; }
        public virtual Bestandsaenderungsart BAEArt { get; set; }

    }
}