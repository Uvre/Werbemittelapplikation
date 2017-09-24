using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class Artikelsprache
    {
        public int ArtikelspracheID { get; set; }
        public string Sprache { get; set; }
        public virtual ICollection<Artikel> artikel { get; set; }
    }
}