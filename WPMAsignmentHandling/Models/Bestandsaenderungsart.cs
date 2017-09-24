using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Bestandsaenderungsart
    {
        public int BestandsaenderungsartID { get; set; }

        public string Grund { get; set; }

        public bool Art { get; set; }

        public virtual ICollection<Bestandsaenderung> BAEs {get;set;}  
    }
}