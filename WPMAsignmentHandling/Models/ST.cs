using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class ST
    {
        public int ID { get; set; }
        public string wert { get; set; }
        public virtual ICollection<Werbemittelauftrag> auftraege { get; set; }
    }
}