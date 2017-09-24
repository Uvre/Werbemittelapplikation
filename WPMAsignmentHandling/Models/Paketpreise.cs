using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Paketpreis
    {
        public int PaketpreisID { get;set; }
        public float Preis { get; set; }
        public string Versandart { get; set; }
    }
}