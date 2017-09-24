using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Lieferart
    {
        public int LieferartID { get; set; }
        public string Lieferungsart { get; set; }
        public virtual ICollection<Paket> Paket { get; set; }
    }
}