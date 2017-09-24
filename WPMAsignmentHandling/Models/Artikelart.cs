using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WPMAsignmentHandling.Models
{
    public class Artikelart
    {
        public int ArtikelartID { get; set; }
        [Display(Name = "Artikelart")]
        public string Art { get; set; }
        public string Bemerkung { get; set; }
        public virtual ICollection<Artikel> artikel{ get; set; }
    }
}