using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WPMAsignmentHandling.Models;

namespace WPMAsignmentHandling.Controllers.ProcessAM
{

    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();

        public ActionResult Index()
        {
            ViewData["Artikelarten"] = dms.Artikelarten.OrderBy(r => r.Art).ToList();
            ViewData["Artikelsprachen"] = dms.Artikelsprachen.OrderBy(r => r.Sprache).ToList();
            ViewData["Laender"] = dms.Laender.OrderBy(r => r.CountryCode).ToList();
            ViewData["BestandsArten"] = dms.BAenderungsarten.OrderBy(r => r.Grund).ToList();
            ViewData["Paketdaten"] = dms.Paketpreise.ToList();
            ViewData["Abrechnungspreise"] = dms.Abrechnungspreise.ToList();
            return View();
        }

        public ActionResult ArtikelartAnlegen(string Artikelart="", string Bemerkung=""){
            Artikelart Art = new Artikelart { Art = Artikelart, Bemerkung= Bemerkung };
            dms.Artikelarten.Add(Art);
            dms.SaveChanges();
            return Json(Art.ArtikelartID);
        }

        public ActionResult ArtikelartAendern(int ArtikelartID, string Artikelart="", string Bemerkung="")
        {
            Artikelart Art = dms.Artikelarten.Find(ArtikelartID);
            Art.Art = Artikelart;
            Art.Bemerkung = Bemerkung;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult ArtikelspracheAnlegen(string Sprache = "")
        {
            Artikelsprache sprache = new Artikelsprache { Sprache = Sprache };
            dms.Artikelsprachen.Add(sprache);
            dms.SaveChanges();
            return Json(sprache.ArtikelspracheID);
        }

        public ActionResult ArtikelspracheAendern(int ArtikelspracheID, string Sprache = "")
        {
            Artikelsprache sprache = dms.Artikelsprachen.Find(ArtikelspracheID);
            sprache.Sprache = Sprache;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult BestandaenderungsartAnlegen(string Grund = "", bool Art=true)
        {
            Bestandsaenderungsart BAEArt = new Bestandsaenderungsart { Grund = Grund, Art = Art };
            dms.BAenderungsarten.Add(BAEArt);
            dms.SaveChanges();
            return Json(BAEArt.BestandsaenderungsartID);
        }

        public ActionResult BestandaenderungsartAender(int BAEArtId, string Grund = "", bool Art = true)
        {
            Bestandsaenderungsart BAEArt = dms.BAenderungsarten.Find(BAEArtId);
            BAEArt.Grund = Grund;
            BAEArt.Art = Art;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult LandAnlegen(string CC = "", string Land = "")
        {
            Land land = new Land {CountryCode=CC, land=Land};
            dms.Laender.Add(land);
            dms.SaveChanges();
            return Json(land.LandID);
        }

        public ActionResult LandAendern(int LandID, string CC = "", string Land = "")
        {
            Land land = dms.Laender.Find(LandID);
            land.land = Land;
            land.CountryCode = CC;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult Paketpreisaendern(int PaketpreisID, float preis =  0, string versandart="")
        {
            Paketpreis Ptpreis = dms.Paketpreise.Find(PaketpreisID);
            Ptpreis.Preis = preis;
            Ptpreis.Versandart = versandart;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult AbrechnungsPreisAendern(int AbrechnungsPreisID, float Preis = 0, string Name = "")
        {
            Abrechnungspreis abrechnungsPreis = dms.Abrechnungspreise.Find(AbrechnungsPreisID);
            abrechnungsPreis.Preis = Preis;
            abrechnungsPreis.Name = Name;
            dms.SaveChanges();
            return Json(true);
        }

    }
}
