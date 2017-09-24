using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Paket
    {
        public int PaketID { get; set; }

        [Required(ErrorMessage="Keine Paketnummer eingetragen")]
        public string Paketnummer{get;set;}

        [Required(ErrorMessage = "Keine Gewicht eingetragen")]
        public float Gewicht { get; set; }

        [Required(ErrorMessage = "Keine Preis eingetragen")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float Preis { get; set; }
        
        public bool versendet { get; set; }
        public string Versandart{get;set;}
        
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Versanddatum { get; set; }
        
        public virtual ICollection<Auftragsmenge> artikelmenge { get; set; }
        public virtual Werbemittelauftrag auftrag {get; set;}


        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Ruecksendedatum { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float Zusatzkosten { get; set; }
        public bool Ruecklaeufer { get; set; }
        public bool RuecklaeuferAbgerechnet { get; set; }
        public string Bemerkung { get; set; }

    }
}