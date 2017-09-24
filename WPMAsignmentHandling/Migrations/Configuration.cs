namespace WPMAsignmentHandling.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WPMAsignmentHandling.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WPMAsignmentHandling.Models.DMS_Winkhardt_DB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WPMAsignmentHandling.Models.DMS_Winkhardt_DB context)
        {
            if (!context.Messen.Any(r => r.Name == "Marketingartikel"))
            {
                Messe Landesmesse = new Messe { Startdatum = DateTime.Now, Enddatum = DateTime.Parse("2070-12-31"), Name = "Marketingartikel", MesseID = 1000, Active = true, Erstellungsdatum = DateTime.Now, isLandesmesse = true };
                context.Messen.Add(Landesmesse);
                context.SaveChanges();
            }
            //if (context.Paketpreise.Count() == 0)
            //{
            //    var paketpreise = new List<Paketpreis>
            //    {
            //        new Paketpreis{ Preis=1.45F, Versandart="Brief klein"},
            //        new Paketpreis{ Preis=2.40F, Versandart="Brief groß"},
            //        new Paketpreis{ Preis=3.45F, Versandart="Brief int klein"},
            //        new Paketpreis{ Preis=7F, Versandart="Brief int mittel"},
            //        new Paketpreis{ Preis=7F, Versandart="Brief int groß"}
            //    };
            //    paketpreise.ForEach(s => context.Paketpreise.Add(s));
            //    context.SaveChanges();
            //}

            var Artikelnummern = context.Artikell.ToList();
            foreach (var artikel in Artikelnummern)
            {
                if(String.IsNullOrEmpty(artikel.Artikelnummer)){
                    artikel.Artikelnummer = "ka";
                }
            }
            context.SaveChanges();
        }
    }
}
