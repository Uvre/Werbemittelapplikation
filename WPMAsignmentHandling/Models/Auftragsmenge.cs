using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Auftragsmenge
    {
        public int AuftragsmengeID { get; set; }
        public virtual Artikel artikel { get; set; }
        public int menge { get; set; }
        public int gelieferteMenge { get; set; }
        public virtual Werbemittelauftrag auftrag { get; set; }
        public virtual Paket paket { get; set; }
    }
}