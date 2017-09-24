using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WPMAsignmentHandling.Models
{
    public class Werbemittelauftrag 
    {
        public int WerbemittelauftragID { get; set; }
        public bool isLandesmesseauftrag { get; set; }
        public string Kennzeichnung { get; set; }
        public virtual ST Stat { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float Verschickungskosten { get; set; }
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Erstellungsdatum { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Versanddatum { get; set; }
        [Required(ErrorMessage = "Kein Bestelldatum eingetragen")]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Bestelldatum { get; set; }
        public virtual Messe messe { get; set; }
        public virtual Kunde kunde { get; set; }
        public virtual Kontakdaten Auftraggeberadresse { get; set; }
        public virtual Kontakdaten Lieferadresse { get; set; }
        public virtual Kontakdaten Rechnungsadresse { get; set; }
        public virtual Kontakdaten Austelleradresse { get; set; }
        public string HalleUStand { get; set; }
        public string Bemerkung { get; set; }
        [Required]
        public virtual ICollection<Auftragsmenge> Auftragsmengen { get; set; }
        [Required]
        public virtual ICollection<Paket> Pakete { get; set; }
        public virtual ICollection<Bestandsaenderung> BAE { get; set; }
        public string Auftragsmailtext { get; set; }


        public int ZeitAuftragAnlegen { get; set; }
        public int ZeitAuftragPacken { get; set; }
        public string PSPNummer { get; set; }
        public string Kostenstelle { get; set; }
    }
}