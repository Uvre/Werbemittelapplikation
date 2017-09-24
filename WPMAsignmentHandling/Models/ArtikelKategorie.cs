using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class ArtikelKategorie
    {
        public int ArtikelKategorieID { get; set; }
        public string Bezeichnung { get; set; }
        public string Bildpfad { get; set; }
        public virtual ICollection<Artikel> Artikel { get; set; }
    }
}