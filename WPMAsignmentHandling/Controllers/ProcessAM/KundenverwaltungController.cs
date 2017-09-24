using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WPMAsignmentHandling.Models;
using WPMAsignmentHandling.Additional;

namespace WPMAsignmentHandling.Controllers.ProcessAM
{
    [Authorize(Roles = "Administrator, Winkhardt-MA")]
    public class KundenverwaltungController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();

        public ActionResult Index()
        {
            ICollection<Kunde> kunden = dms.Kunden.OrderByDescending(r => r.Erstellungsdatum).Take(500).ToList();  
            return View(kunden);
        }

        public ActionResult Suchen(FormCollection fc)
        {
            ICollection<Kunde> kunden = new List<Kunde>();
            string ersterTerm = fc["FilterEins"];
            List<Kunde> bla = dms.Kunden.Where(r => r.Name.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Erstellungsdatum).Take(250).ToList();
            foreach (var item in bla)
            {
                kunden.Add(item);
            }

            return PartialView("Kundenliste", kunden);
        }

        public ActionResult AutocompleteLand(string term)
        {
            var Laender = dms.Laender.Where(r => r.land.StartsWith(term)).Take(20).Select(r => new { label = r.land });
            return Json(Laender, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KundeBearbeiten(FormCollection fc, int KundeID)
        {
            if (fc["KundeAnlegen"] != null)
            {
                return RedirectToAction("Anlegen");
            }
            if (fc["DatenAendern"] != null && KundeID != 0)
            {

                return RedirectToAction("Aendern", new { KundeID = KundeID });
            }
            if (fc["DetailsAnzeigen"] != null && KundeID != 0)
            {

                return RedirectToAction("Details", new { KundeID = KundeID });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int KundeID = 0)
        {
            Kunde kunde = dms.Kunden.Find(KundeID);
            return View(kunde);
        }

        public ActionResult Anlegen()
        {
            return View("KundendatenEingabe");
        }

        public ActionResult KundeSpeichern(Kunde kunde, Kontakdaten Hauptadresse, string Land="")
        {
            Land land = dms.Laender.SingleOrDefault(r => r.land == Land);
            Kunde kun = dms.Kunden.SingleOrDefault(r => r.KundeID == kunde.KundeID);
            if (kun == null)
            {
                if (land != null)
                {
                    Hauptadresse.Land = land;
                }
                else
                {
                    ViewBag.ErrLand = "true";
                }
                kunde.Hauptadresse = Hauptadresse;
                if (ModelState.IsValid && land != null)
                {
                    dms.Kunden.Add(kunde);
                    dms.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                    {
                        foreach (var item in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine("error: " + item.ErrorMessage);
                        }
                    }
                    return View("KundendatenEingabe", kunde);
                }
            }
            else
            {
                if (ModelState.IsValid && land != null)
                {
                    Kontakdaten HaupTAD = dms.Kontaktdatenn.Find(Hauptadresse.KontakdatenID);
                    dms.Entry(kun).CurrentValues.SetValues(kunde);
                    dms.Entry(HaupTAD).CurrentValues.SetValues(Hauptadresse);
                    if (land.LandID != HaupTAD.Land.LandID)
                    {
                        dms.Laender.Find(HaupTAD.Land.LandID).laender.Remove(HaupTAD);
                        HaupTAD.Land = land;
                    }
                    dms.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("KundendatenEingabe", kunde);
                }
            }
        }

        public ActionResult KundeLoeschen(int KundeID)
        {
            Kunde kunde = dms.Kunden.Find(KundeID);
            Kontakdaten Hauptadresse = dms.Kontaktdatenn.Find(kunde.Hauptadresse.KontakdatenID);
            if (kunde.auftraege.Count() == 0)
            {
                Hauptadresse.Kunde = null;
                dms.SaveChanges();
                dms.Kontaktdatenn.Remove(Hauptadresse);
                var Adressen = dms.Kontaktdatenn.Where(r => r.Kunde.KundeID == kunde.KundeID);
                foreach (var adresse in Adressen)
                {
                    dms.Kontaktdatenn.Remove(adresse);
                }
                dms.Kunden.Remove(kunde);
                dms.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Details");
            }
        }

        public ActionResult Aendern(int KundeID = 0)
        {
            Kunde kunde = dms.Kunden.Find(KundeID);
            return View("KundendatenEingabe",kunde);
        }
    }
}
