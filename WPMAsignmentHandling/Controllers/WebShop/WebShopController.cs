using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WPMAsignmentHandling.Models;
using iTextSharp;
using WPMAsignmentHandling.Additional;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net.Mail;
using RazorEngine.Templating;
using System.IO;
using RazorEngine;



namespace WPMAsignmentHandling.Controllers.WebShop
{
    //[RequireHttpsAttribute]
    [Authorize(Roles = "Administrator, Winkhardt-MA, Messe-MA")]
    public class WebShopController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();

        public ActionResult Index()
        {
            ViewData["Artikel"] = dms.Artikell.Where(r => r.Landesmesseartikel && r.Active).OrderBy(r => r.Name).ToList();
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewData["User"] = user;
            return View();
        }

        public ActionResult AutoSearchArtikel(string term)
        {
            var Artikel = dms.Artikell.Where(r => r.Name.Contains(term) && r.Active && r.Landesmesseartikel).Take(20).Select(r => new { label = r.Name + " (AN: " + r.Artikelnummer.ToString() + ")" });
            //var Artikelnummern = dms.Artikell.Where(r => r.Name.ToLower().Contains(term.ToLower()) && r.Active && r.Landesmesseartikel).Take(20).Select(r => r.Artikelnummer.ToString());
            //var AutosearchList = Artikel.Concat(Artikelnummern).ToList();
            return Json(Artikel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AuftragAnlegen(FormCollection fc)
        {

            string Mailtext = "Bestellbestätigung\n\n";
            
            Kontakdaten Auftraggeber = new Kontakdaten {

                Name2 = fc["Name"].ToString(),
                Name = fc["Name2"].ToString(),
                Name3 = fc["Name3"].ToString(),
                Strasse = fc["Strasse"].ToString(),
                Ort = fc["Ort"].ToString(),
                PLZ = fc["Plz"].ToString(),
                EMail = fc["EMail"].ToString(),
                Land = dms.Laender.Single(r => r.land == "Deutschland")
            };

            Kontakdaten Lieferadresse = null;
            if (fc["TbName"].ToString() != fc["Name"].ToString())
            {
                Lieferadresse = new Kontakdaten
                {
                    Name2 = fc["TbName"].ToString(),
                    Name = fc["Name2"].ToString(),
                    Name3 = fc["Name3"].ToString(),
                    Strasse = fc["Strasse"].ToString(),
                    Ort = fc["Ort"].ToString(),
                    PLZ = fc["Plz"].ToString(),
                    EMail = fc["EMail"].ToString(),
                    Land = dms.Laender.Single(r => r.land == "Deutschland")
                };
            }



            //Kunde kunde = new Kunde{ Name = Auftraggeber.Name, Erstellungsdatum = DateTime.Now};
            ////kunde.Hauptadresse = new Kontakdaten { Name = Auftraggeber.Name, Name2 = Auftraggeber.Name2, Name3 = Auftraggeber.Name3, Strasse = Auftraggeber.Strasse, PLZ = Auftraggeber.PLZ, Ort = Auftraggeber.Ort, Land = Auftraggeber.Land, Telefon = Auftraggeber.Telefon, EMail = Auftraggeber.EMail};

            //Kunde KundeDB = dms.Kunden.SingleOrDefault(r => r.Name.Equals(fc["Name"].ToString()) && r.Hauptadresse.Name2.Equals(fc["Name2"].ToString()));
            Kunde KundeDB = dms.Kunden.Find(4409);

            //if (KundeDB != null)
            //{
            //    kunde = KundeDB;
            //}

            ICollection<Auftragsmenge> Auftragsmengen = new List<Auftragsmenge>();
            foreach(var item in fc){
                if (item.ToString().Length > 13 && item.ToString().Substring(0, 13) == "ArtikelMenge_")
                {
                    int length = item.ToString().Length -13;
                    int ArtikelID = int.Parse(item.ToString().Substring(13, length));
                    Artikel artikel = dms.Artikell.Find(ArtikelID);
                    int Menge = int.Parse(fc[item.ToString()])*artikel.Verpackungseinheit;
                    Auftragsmengen.Add(new Auftragsmenge { artikel = dms.Artikell.Find(ArtikelID), menge=Menge });
                    Mailtext = Mailtext + " - " + Menge + " " + artikel.Name + "\n";
                }
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(fc["EMail"].ToString());

            string kennzeichen = "";
            if (dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMMA")).Count() > 0)
            {
                Werbemittelauftrag wmala = dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMMA")).ToList().Last();
                string laKennzeichnung = wmala.Kennzeichnung;
                if (wmala.Erstellungsdatum.Year < DateTime.Now.Year)
                {
                    kennzeichen = "WMMA" + DateTime.Now.Year.ToString() + "_000001";
                }
                else
                {
                    int number = int.Parse(laKennzeichnung.Substring(9, 6));
                    number ++;
                    kennzeichen = "WMMA" + DateTime.Now.Year.ToString() + "_" + number.ToString("000000");
                }
            }
            else
            {
                kennzeichen = "WMMA" + DateTime.Now.Year.ToString() + "_000001";
            }
            
                Werbemittelauftrag WebShopAuftrag = new Werbemittelauftrag
                {
                    Erstellungsdatum = DateTime.Now,
                    Versanddatum = DateTime.Now,
                    ZeitAuftragPacken = 0,
                    Auftraggeberadresse = Auftraggeber,
                    Kennzeichnung = kennzeichen,
                    messe = dms.Messen.Single(r => r.isLandesmesse),
                    isLandesmesseauftrag = true,
                    ZeitAuftragAnlegen = 0,
                    Bestelldatum = DateTime.Now,
                    Auftragsmengen = Auftragsmengen,
                    Stat = dms.Stati.Find(1),
                    Pakete = new List<Paket>(),
                    PSPNummer = fc["PSPNummer"].ToString(),
                    //Kostenstelle = fc["Kostenstelle"].ToString(),
                    Lieferadresse = Lieferadresse,
                    kunde = KundeDB,
                    Verschickungskosten = 0,
                    Bemerkung = fc["Bemerkung"].ToString()
                };
                dms.Werbemittelauftraege.Add(WebShopAuftrag);


                var model = new OrderConfirmationModel() { UserMail = fc["EMail"].ToString(), UserName = user.Vorname + " " + user.Nachname, Auftragsmengen = Auftragsmengen, WMA = WebShopAuftrag };
                

            try
            {
                dms.SaveChanges();
                Extensions.SendOrderConfirmation(model, user);
                //try
                //{
                //    string mailBody = "";
                //    if (Engine.Razor.IsTemplateCached("Bestellbestaetigung", null))
                //    {
                //        mailBody = Engine.Razor.Run("Bestellbestaetigung", null, model);
                //    }
                //    else
                //    {
                //        var templateFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Views/WebShop/Bestellbestaetigung.cshtml");
                //        var templateAsString = System.IO.File.ReadAllText(templateFilePath);
                //        //var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebShop");
                //        mailBody = Engine.Razor.RunCompile(templateAsString, "Bestellbestaetigung", null, model);
                //    }

                //    if (!String.IsNullOrEmpty(mailBody))
                //    {
                //        userManager.SendEmail(user.Id, "Bestellbestätigung", mailBody);
                //        var message = new MailMessage();
                //        message.To.Add(new MailAddress("werbemittel@briefdruck.de")); 
                //        message.Subject = "Test";
                //        message.Body = string.Format(mailBody, "Werbemittelapplikation");
                //        message.IsBodyHtml = true;
                //        using (var smtp = new SmtpClient())
                //        {
                //            smtp.Send(message);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debug.WriteLine("Mail: " + ex.Message);
                //}
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        System.Diagnostics.Debug.Write("val probelem" + validationError.ErrorMessage);
                    }
                }
            }
            
            ViewData["Artikel"] = dms.Artikell.Where(r => r.Landesmesseartikel).OrderBy(r => r.Name).ToList();
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var ShopUser = UserManager.FindById(User.Identity.GetUserId());
            ViewData["User"] = ShopUser;
            ViewData["AuftragAngelegt"] = "true";
            return View("Index");
            //return PartialView("Bestellbestaetigung", model);
        }

    }


}
