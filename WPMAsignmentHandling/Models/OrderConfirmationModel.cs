using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace WPMAsignmentHandling.Models
{
    public class OrderConfirmationModel 
    {
        public string UserMail { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Auftragsmenge> Auftragsmengen { get; set; }
        public virtual Werbemittelauftrag WMA { get; set; }
    }
}
