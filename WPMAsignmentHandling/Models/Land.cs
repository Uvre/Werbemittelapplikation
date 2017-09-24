using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Land
    {
        public int LandID { get; set; }
        public String land { get; set; }
        public String CountryCode { get; set; }
        public virtual ICollection<Kontakdaten> laender { get; set; }
        public virtual ICollection<Kunde> Kunden { get; set; }
    }
}