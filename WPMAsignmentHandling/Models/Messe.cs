
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Messe
    {
        
        public int MesseID { get; set; }

        public bool Active { get; set; }

        public bool isLandesmesse { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Erstellungsdatum { get; set; }

        [Required(ErrorMessage = "Kein Messename eingetragen")]
        public string Name { get; set; }

        public string Position_1 { get; set; }
        public string ProjektleiterName { get; set; }
        public string ProjektleiterTelefon { get; set; }
        public string ProjektleiterEmail { get; set; }


        public string Position_2 { get; set; }
        public string ProjektleiterVizeName { get; set; }
        public string ProjektleiterVizeTelefon { get; set; }
        public string ProjektleiterVizeEmail { get; set; }

        public string Position_3 { get; set; }
        public string KommunikationsleiterName { get; set; }
        public string KommunikationsleiterTelefon { get; set; }
        public string KommunikationsleiterEmail { get; set; }

        public string Position_4 { get; set; }
        public string KommunikationsleiterVizeName { get; set; }
        public string KommunikationsleiterVizeTelefon { get; set; }
        public string KommunikationsleiterVizeEmail { get; set; }

        public string Bemerkung { get; set; }

        public string Bannerkontakt { get; set; }

        public bool abgegrechnet { get; set; }
        public bool RestmengenlisteVerschickt { get; set; }

        public string Webadresse { get; set; }

        public virtual ICollection<Werbemittelauftrag> auftraege { get; set; }

        [Required(ErrorMessage = "Kein Stardatum eingetragen")]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Startdatum { get; set; }

        [Required(ErrorMessage = "Kein Endatum eingetragen")]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Enddatum { get; set; }


        [Required(ErrorMessage = "Kein Preis eingetragen!")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [Range(0, 100, ErrorMessage = "Es kann kein Preis größer als 1000€ eingetragen werden!")]
        [DisplayName("Paketpreis")]
        public float AbrechungsPreisStandardpaket { get; set; }

        public virtual ICollection<Artikel> artikel{get;set;}
    }
}