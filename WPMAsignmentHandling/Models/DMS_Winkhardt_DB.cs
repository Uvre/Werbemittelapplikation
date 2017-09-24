using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WPMAsignmentHandling.Models
{
    public class DMS_Winkhardt_DB:DbContext
    {
        public DbSet<Artikel> Artikell { get; set; }
        public DbSet<Artikelart> Artikelarten { get; set; }
        public DbSet<Werbemittelauftrag> Werbemittelauftraege { get; set; }
        public DbSet<Messe> Messen { get; set; }
        public DbSet<Kunde> Kunden { get; set; }
        public DbSet<Lieferart> Lieferarten { get; set; }
        public DbSet<Paket> Pakete { get; set; }
        public DbSet<ST> Stati { get; set; }
        public DbSet<Bestandsaenderung> BAenderungen { get; set; }
        public DbSet<Bestandsaenderungsart> BAenderungsarten { get; set; }
        public DbSet<Auftragsmenge> Auftragsmengen { get; set; }
        public DbSet<Kontakdaten> Kontaktdatenn { get; set; }
        public DbSet<Artikelsprache> Artikelsprachen { get; set; }
        public DbSet<Land> Laender { get; set; }
        public DbSet<Paketpreis> Paketpreise { get; set; }
        public DbSet<Abrechnungspreis> Abrechnungspreise { get; set; }
        public DbSet<Artikel_Herstellungsart> Artikel_Herstellungsarten { get; set; }
        //public DbSet<ArtikelKategorie> ArtikelKategorien { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kontakdaten>().HasOptional(r => r.Kunde);
            //modelBuilder.Entity<Kontakdaten>().HasOptional(r => r.messe);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }

}