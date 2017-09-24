using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WPMAsignmentHandling.Additional;
using WPMAsignmentHandling.Models;
using System.IO;


namespace WPMAsignmentHandling.Controllers.ProcessAM
{
    [Authorize(Roles = "Administrator, Winkhardt-MA")]
    public class ArtikelverwaltungController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();

        public ActionResult Index()
        {

            string suchtext = "";
            int Filter = 1;
            if (Session["SuchtextArtikel"] != null)
            {
                suchtext = Session["SuchtextArtikel"].ToString();
            }

            if (Session["FilterArtikel"] != null)
            {
                Filter = int.Parse(Session["FilterArtikel"].ToString());
            }

                ICollection<Artikel> artikel = new List<Artikel>();
                List<Artikel> Artikels = null;
                if (String.IsNullOrEmpty(suchtext))
                {
                    if (Filter == 1)
                    {
                        Artikels = dms.Artikell.Where(r => r.Active == true).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 2)
                    {
                        Artikels = dms.Artikell.Where(r => r.Active == false).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 3)
                    {
                        Artikels = dms.Artikell.OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                }
                else
                {
                    if (Filter == 1)
                    {
                        Artikels = dms.Artikell.Where(r => r.Active == true && (r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Name.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower()))).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 2)
                    {
                        Artikels = dms.Artikell.Where(r => r.Active == false && (r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Name.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower()))).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 3)
                    {
                        Artikels = dms.Artikell.Where(r => r.Name.ToLower().Contains(suchtext.ToLower()) || r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower())).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                }


                List<Artikel> allgArtikel = null;
                if ("Allgemein".ToLower().Contains(suchtext.ToLower()))
                {
                    if (Filter == 1)
                    {
                        allgArtikel = dms.Artikell.Where(r => r.Active == true && r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 2)
                    {
                        allgArtikel = dms.Artikell.Where(r => r.Active == false && r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                    if (Filter == 3)
                    {
                        allgArtikel = dms.Artikell.Where(r => r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                    }
                }

                foreach (var item in Artikels)
                {
                    artikel.Add(item);
                }

                if (allgArtikel != null)
                {
                    foreach (var item in allgArtikel)
                    {
                        foreach (var artikellist in artikel)
                        {
                            if (artikellist.ArtikelID == item.ArtikelID)
                            {
                                artikel.Remove(item);
                                break;
                            }
                        }
                        artikel.Add(item);
                    }
                }
                return View(artikel);

        }

        public ActionResult setArtikelActivity(int artikelID, bool active)
        {
            Artikel artikel = dms.Artikell.Find(artikelID);
            artikel.Active = active;
            dms.SaveChanges();
            return Json("true");
        }

        public ActionResult Artikelsuche(FormCollection fc)
        {
            ICollection<Artikel> artikel = new List<Artikel>();
            string suchtext = fc["FilterEins"];
            Session["SuchtextArtikel"] = suchtext;
            int Filter = 1;
           
            Filter = int.Parse(fc["SuchFilterRadio"]);
            Session["FilterArtikel"] = Filter;
            List<Artikel> Artikels = null;

            if (String.IsNullOrEmpty(suchtext))
            {
                if (Filter == 1)
                {
                    Artikels = dms.Artikell.Where(r => r.Active == true ).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 2)
                {
                    Artikels = dms.Artikell.Where(r => r.Active == false).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 3)
                {
                    Artikels = dms.Artikell.OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
            }
            else
            {
                if (Filter == 1)
                {
                    Artikels = dms.Artikell.Where(r => r.Active == true && (r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Name.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower()))).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 2)
                {
                    Artikels = dms.Artikell.Where(r => r.Active == false && (r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Name.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower()))).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 3)
                {
                    Artikels = dms.Artikell.Where(r => r.Name.ToLower().Contains(suchtext.ToLower()) || r.Artikelnummer.ToLower().Contains(suchtext.ToLower()) || r.Messe.Name.ToLower().Contains(suchtext.ToLower())).OrderBy(r => r.Messe.Name).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
            }
            


            List<Artikel> allgArtikel= null;
            if ("Allgemein".ToLower().Contains(suchtext.ToLower()))
            {
                if (Filter == 1)
                {
                    allgArtikel = dms.Artikell.Where(r => r.Active == true && r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 2)
                {
                    allgArtikel = dms.Artikell.Where(r => r.Active == false && r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
                if (Filter == 3)
                {
                    allgArtikel = dms.Artikell.Where(r => r.MesseartikelAllgemein == true).OrderBy(r => r.artikelart.Art).ThenBy(r => r.Name).ThenBy(r => r.Sprache).ToList();
                }
            }
            foreach (var item in Artikels)
            {
                artikel.Add(item);
            }

            if (allgArtikel != null)
            {
                foreach (var item in allgArtikel)
                {
                    foreach (var artikellist in artikel)
                    {
                        if (artikellist.ArtikelID == item.ArtikelID)
                        {
                            artikel.Remove(item);
                            break;
                        }
                    }
                    artikel.Add(item);
                }
            }
            
            return PartialView("Artikelliste", artikel);
        }
        
        public ActionResult Anlegen()
        {
            ViewBag.artikelarten = new SelectList(dms.Artikelarten.OrderBy(R => R.Art), "ArtikelartID", "Art");
            ViewBag.artikelsprachen = new SelectList(dms.Artikelsprachen.OrderBy(R => R.Sprache), "ArtikelspracheID", "Sprache");
            ViewBag.Artikel_Herstellungsarten = new SelectList(dms.Artikel_Herstellungsarten.OrderBy(r => r.Bezeichnung), "Artikel_HerstellungsartID", "Bezeichnung");
            //ViewBag.Artikelkategorien = new SelectList(dms.ArtikelKategorien.OrderBy(R => R.Bezeichnung), "ArtikelKategorieID", "Bezeichnung");

            Artikel wma = null;

            if (dms.Artikell.Any(r => r.Artikelnummer.Contains("wma_")))
            {
                wma = dms.Artikell.Where(r => r.Artikelnummer.Contains("wma_")).ToList().Last();
            }

            if (wma == null)
            {
                ViewBag.NewWMA = "wma_" + DateTime.Now.Year.ToString() + "_" + "00001";
            }
            else
            {
                if (wma.Erstellungsdatum.Year < DateTime.Now.Year)
                {
                    ViewBag.NewWMA = "wma_" + DateTime.Now.Year.ToString() + "_" + "00001";
                }
                else
                {
                    int number = int.Parse(wma.Artikelnummer.Substring(9, 5));
                    number++;
                    ViewBag.NewWMA = "wma_" + DateTime.Now.Year.ToString() + "_" + number.ToString("00000");
                }
            }
            return View("ArtikeldatenEingabe");
        }

        public ActionResult ArtikelAendern(int ArtikelID = 0)
        {
            Artikel artikel = dms.Artikell.Find(ArtikelID);
            ViewBag.artikelarten = new SelectList(dms.Artikelarten.OrderBy(r => r.Art), "ArtikelartID", "Art", artikel.artikelart.ArtikelartID);
            ViewBag.artikelsprachen = new SelectList(dms.Artikelsprachen.OrderBy(r => r.Sprache), "ArtikelspracheID", "Sprache", artikel.Sprache.ArtikelspracheID);
            //ViewBag.Artikel_Herstellungsarten = new SelectList(dms.Artikel_Herstellungsarten.OrderBy(r => r.Bezeichnung), "Artikel_HerstellungsartID", "Bezeichnung", artikel.ArtikelHerstellungsart.Artikel_HerstellungsartID);
            //ViewBag.Artikelkategorien = new SelectList(dms.ArtikelKategorien.OrderBy(R => R.Bezeichnung), "ArtikelKategorieID", "Bezeichnung", artikel.ArtikelKategorie.ArtikelKategorieID);
            return View("ArtikeldatenEingabe", artikel);
        }

        public ActionResult AutoSearchMesseKunde(string term, string bla = "na")
        {
            var messen = dms.Messen.Where(r => r.Name.ToLower().Contains(term.ToLower()) && r.Active).Take(20).Select(r => new {label =r.Name});
            var kunden = dms.Kunden.Where(r => r.Name.ToLower().Contains(term.ToLower())).Take(20).Select(r => new { label = r.Name });
            return Json(messen, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BestandAendern(int ArtikelID = 0)
        {
            Artikel artikel = dms.Artikell.Find(ArtikelID);
            ViewData["artikel"] = artikel;
            ViewBag.Bestandarten = new SelectList(dms.BAenderungsarten.OrderBy(r => r.Grund), "BestandsaenderungsartID", "Grund");
            return View();
        }

        public ActionResult BestandAnpassen(Bestandsaenderung aend, int Grund, float Gewicht = 0)
        {
            Artikel artikel = dms.Artikell.Find(aend.ArtikelID);
            Bestandsaenderungsart Art = dms.BAenderungsarten.Find(Grund);
            ViewBag.Bestandarten = new SelectList(dms.BAenderungsarten, "BestandsaenderungsartID", "Grund");
            if (!Art.Art && aend.Menge > artikel.Bestand)
            {
                ViewBag.ErrMenge = true;
                ViewData["artikel"] = artikel;
                return View("BestandAendern", aend);
            }
                   try
                   {
                       if (ModelState.IsValid)
                       {
                           Bestandsaenderung aenderung = aend;
                           aenderung.Artikel = artikel;
                           artikel.Gewicht = Gewicht;
                           aenderung.BAEArt = Art;
                           aenderung.Grund = Art.Grund;
                           dms.BAenderungen.Add(aenderung);
                           if (Art.Art)
                           {
                               aenderung.Artikel.Bestand = aenderung.Artikel.Bestand + aenderung.Menge;
                           }
                           else
                           {
                               aenderung.Artikel.Bestand = aenderung.Artikel.Bestand - aenderung.Menge;
                           }
                           dms.SaveChanges();
                           if (artikel.Meldebestand >= artikel.Bestand && artikel.Active)
                           {
                               Extensions.NotificationUndercutMeldebestand(artikel);
                           }
                           if (artikel.Sicherheitsbestand >= artikel.Bestand && artikel.Active)
                           {
                               Extensions.NotificationUndercutSicherheitsbestand(artikel);
                           }
                           return RedirectToAction("Index");
                       }
                       else
                       {
                           foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                           {
                               foreach (var item in state.Errors)
                               {
                                   //System.Diagnostics.Debug.WriteLine("error: " + item.ErrorMessage);
                               }
                           }
                           ViewData["artikel"] = artikel;
                           return View("BestandAendern", aend);
                       }
                   }
                   catch (DbEntityValidationException dbEx)
                   {
                           foreach (var validationErrors in dbEx.EntityValidationErrors)
                           {
                               foreach (var validationError in validationErrors.ValidationErrors)
                               {
                                   Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                   //System.Diagnostics.Debug.Write("val probelem" + validationError.ErrorMessage);
                               }
                           }
                           ViewData["artikel"] = artikel;
                           return View("BestandAendern", aend);
                   }
        }

        public ActionResult ArtikelBearbeiten(FormCollection fc, int ArtikelID=0)
        {
            if (fc["ArtikelAnlegen"] != null)
            {
                return RedirectToAction("Anlegen");
            }
            if (fc["ArtikelAendern"] != null && ArtikelID != 0)
            {
                return RedirectToAction("ArtikelAendern", new { ArtikelID = ArtikelID });
            }
            if (fc["BestandAendern"] != null && ArtikelID != 0)
            {
                return RedirectToAction("BestandAendern", new { ArtikelID = ArtikelID });
            }
            if (fc["DetailsAnzeigen"] != null && ArtikelID != 0)
            {
                return RedirectToAction("DetailsAnzeigen", new { ArtikelID = ArtikelID });
            }
            return RedirectToAction("Index");
        }

        public ActionResult AenderungSpeichern(FormCollection fc, Artikel artikels, Artikelart art, HttpPostedFileBase file, Artikelsprache sprache, int MeldeOld, int SicherOld, ArtikelKategorie Kategorie = null, string Messename = "")
        {

            bool changeMeldeSicherheitsbestand = false;
            if (artikels.Meldebestand != MeldeOld || artikels.Sicherheitsbestand != SicherOld)
            {
                changeMeldeSicherheitsbestand = true;
            }
            if (file != null && !String.IsNullOrEmpty(artikels.Bildpfad))
            {
                var path = Path.Combine(Server.MapPath("~/Images/Artikelbilder"), artikels.ArtikelID + Path.GetExtension(file.FileName));
                if (System.IO.File.Exists(path)){
                    System.IO.File.Delete(path);
                }
                file.SaveAs(path);
                artikels.Bildpfad = artikels.ArtikelID + Path.GetExtension(file.FileName);
            }

            if (fc["AllgemeinerMesserartikel"].ToString() == "true")
            {
                artikels.MesseartikelAllgemein = true;
            }
            if (fc["Landesmesseartikel"].ToString() == "true")
            {
                artikels.Landesmesseartikel = true;
            }
            
            bool stateModel = false;

            if (ModelState.IsValid)
            {
                dms.Entry(artikels).State = EntityState.Modified;
                try
                {
                    dms.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Mist:" + ex.Message);
                }
                dms.SaveChanges();
                stateModel = true;
            }else
            {
                ViewBag.artikelsprachen = new SelectList(dms.Artikelsprachen.OrderBy(r => r.Sprache), "ArtikelspracheID", "Sprache");
                ViewBag.artikelarten = new SelectList(dms.Artikelarten.OrderBy(r => r.Art), "ArtikelartID", "Art");
            }

            dms.Dispose();
            dms = new DMS_Winkhardt_DB();
            Artikel artikel = dms.Artikell.Find(artikels.ArtikelID);
            Messe messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
            if (!(artikels.MesseartikelAllgemein||artikels.Landesmesseartikel))
            {
                if (messe == null)
                {
                    ViewData["KMerr"] = true;
                }
                else
                {
                    if (artikel.Messe.MesseID != messe.MesseID)
                    {
                        artikel.Messe = dms.Messen.Find(messe.MesseID);
                        dms.SaveChanges();
                    }
                }
            }
            else
            {
                if (artikel.Messe != null)
                {
                    if (artikel.Landesmesseartikel)
                    {
                        artikel.Messe = dms.Messen.Single(r => r.isLandesmesse);
                    }
                    else
                    {
                        dms.Messen.Find(artikel.Messe.MesseID).artikel.Remove(artikel);
                    }
                }
                
                dms.SaveChanges();
            }
            if (art.ArtikelartID == 0)
            {
                ViewData["Arterr"] = true;
            }
            else
            {
                dms.Artikelarten.Find(artikel.artikelart.ArtikelartID).artikel.Remove(artikel);
                artikel.artikelart = dms.Artikelarten.Find(art.ArtikelartID);
                dms.SaveChanges();
            }
            if (sprache.ArtikelspracheID == 0)
            {
                ViewData["Spracheerr"] = true;
            }
            else
            {
                dms.Artikelsprachen.Find(artikel.Sprache.ArtikelspracheID).artikel.Remove(artikel);
                artikel.Sprache = dms.Artikelsprachen.Find(sprache.ArtikelspracheID);
                dms.SaveChanges();
            }

            //if (Kategorie != null)
            //{
            //    artikel.ArtikelKategorie = dms.ArtikelKategorien.Find(Kategorie.ArtikelKategorieID);
            //}

            
            if ((messe != null||artikel.Landesmesseartikel||artikel.MesseartikelAllgemein) && art.ArtikelartID != 0 && sprache.ArtikelspracheID != 0 && stateModel)
            {
                if (changeMeldeSicherheitsbestand)
                {
                    if (artikel.Meldebestand >= artikel.Bestand && artikel.Active)
                    {
                        Extensions.NotificationUndercutMeldebestand(artikel);
                    }
                    if (artikel.Sicherheitsbestand >= artikel.Bestand && artikel.Active)
                    {
                        Extensions.NotificationUndercutSicherheitsbestand(artikel);
                    }
                    
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.artikelarten = new SelectList(dms.Artikelarten.OrderBy(r => r.Art), "ArtikelartID", "Art", artikel.artikelart.ArtikelartID);
                ViewBag.artikelsprachen = new SelectList(dms.Artikelsprachen.OrderBy(r => r.Sprache), "ArtikelspracheID", "Sprache", artikel.Sprache.ArtikelspracheID);
                return View("ArtikeldatenEingabe", artikels);
            }
            
        }

        public ActionResult ArtikelAnlegen(FormCollection fc, Artikel artikels, Artikelart art, Artikelsprache sprache, HttpPostedFileBase file, ArtikelKategorie Kategorie = null, string Messename = "")
        {

            
            if (fc["AllgemeinerMesserartikel"].ToString() == "true")
            {
                artikels.MesseartikelAllgemein = true;
            }
            if (fc["Landesmesseartikel"].ToString() == "true")
            {
                artikels.Landesmesseartikel = true;
            }
            Messe messe = null;
            if (!(artikels.MesseartikelAllgemein && artikels.Landesmesseartikel))
            {
                messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
                if (messe == null)
                {
                    ViewData["KMerr"] = true;
                }
                else
                {
                    artikels.Messe = messe;
                }
            }

            if (art.ArtikelartID == 0)
            {
                ViewData["Arterr"] = true;
            }
            if (sprache.ArtikelspracheID == 0)
            {
                ViewData["Spracheerr"] = true;
            }

            if (ModelState.IsValid && (messe != null||artikels.Landesmesseartikel||artikels.MesseartikelAllgemein) && art.ArtikelartID != 0 && sprache.ArtikelspracheID != 0)
            {
                Artikel artikel = artikels;
                try{
                    dms.Artikell.Add(artikel);
                    dms.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                   
                           foreach (var validationErrors in dbEx.EntityValidationErrors)
                           {
                               foreach (var validationError in validationErrors.ValidationErrors)
                               {
                                   Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                   //System.Diagnostics.Debug.Write("val probelem" + validationError.ErrorMessage);
                               }
                           }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Mail Mist:" + ex.Message);
                }
                
                artikel.BAE = new List<Bestandsaenderung>();
                if (!artikels.Landesmesseartikel && !artikels.MesseartikelAllgemein)
                {
                    artikel.Messe = messe;
                }
                if (artikel.Landesmesseartikel)
                {
                    artikel.Messe = dms.Messen.Single(r => r.isLandesmesse);
                }
                artikel.artikelart = dms.Artikelarten.Find(art.ArtikelartID);
                artikel.Sprache = dms.Artikelsprachen.Find(sprache.ArtikelspracheID);
                //if (Kategorie != null)
                //{
                //    artikel.ArtikelKategorie = dms.ArtikelKategorien.Find(Kategorie.ArtikelKategorieID);
                //}
                //if (artikel.Bestand > 0)
                //{
                //    artikel.BAE.Add(new Bestandsaenderung { Artikel = artikel, Datum = DateTime.Now, Menge = artikel.Bestand, Grund = "Anlieferung" });
                //}
                if (file != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Images/Artikelbilder"), artikels.ArtikelID + Path.GetExtension(file.FileName));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    file.SaveAs(path);
                    artikel.Bildpfad = artikel.ArtikelID + Path.GetExtension(file.FileName);
                }
                dms.SaveChanges();
                return Redirect("Index");
            }
            else
            {
                //System.Diagnostics.Debug.Write(error);
                foreach (ModelState modelState in ViewData.ModelState.Values) {
                    foreach (ModelError error in modelState.Errors) {
                        System.Diagnostics.Debug.Write(error);
                    }
                }
                ViewBag.artikelsprachen = new SelectList(dms.Artikelsprachen, "ArtikelspracheID", "Sprache");
                ViewBag.artikelarten = new SelectList(dms.Artikelarten, "ArtikelartID", "Art");
                return View("ArtikeldatenEingabe", artikels);
            }
        }

        public ActionResult ArtikelLoeschen(int ArtikelID = 0)
        {
            Artikel artikel = dms.Artikell.Find(ArtikelID);
            if (artikel.Auftragsmengen.Count() == 0)
            {
                dms.Artikell.Remove(artikel);
                dms.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Details", artikel);
            }
        }

        //public ActionResult BestandsaenderungLoeschen(int BAEid = 0)
        //{
            
        //    Bestandsaenderung BAE = dms.BAenderungen.Find(BAEid);
        //    Artikel artikel = dms.Artikell.Find(BAE.ArtikelID);
        //    int ArtikelID = BAE.ArtikelID;
        //    if (BAE.BAEArt != null)
        //    {
        //        if (BAE.BAEArt.Art)
        //        {
        //            artikel.Bestand -= BAE.Menge;
        //        }
        //        else
        //        {
        //            artikel.Bestand += BAE.Menge;
        //        }
        //    }
        //    else
        //    {
        //        artikel.Bestand -= BAE.Menge;
        //    }
        //    dms.BAenderungen.Remove(BAE);
        //    dms.SaveChanges();
        //    return Json(true);
        //}

        public ActionResult DetailsAnzeigen(int ArtikelID=0)
        {
            Artikel artikel = dms.Artikell.Find(ArtikelID);
            return View("Details", artikel);
        }

        public ActionResult BAEanzeigenArtikel(int ArtikelID = 0)
        {
            Artikel artikel = dms.Artikell.Find(ArtikelID);
            return PartialView(artikel);
        }
    }
}
