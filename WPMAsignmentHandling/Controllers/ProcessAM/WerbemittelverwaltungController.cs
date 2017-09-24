using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml;
using WPMAsignmentHandling.Additional;
using WPMAsignmentHandling.Models;


namespace WPMAsignmentHandling.Controllers.ProcessAM
{

    [Authorize(Roles = "Administrator, Winkhardt-MA")] 
    public class WerbemittelverwaltungController : Controller
    {

        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();
        private string PathToXMLAuftragsdateien = @"C:\inetpub\wwwroot\webapp\XMLAuftragsdateien\";
        //private string PathToXMLAuftragsdateien = @"C:\Users\NotJohn\Desktop\XML Dateien\";

        public ActionResult Index()
        {
            if (Session["SuchtextWerbemittel"] != null)
            {
                string suchtext = "";
                int Filter = 1;
                if (Session["SuchtextWerbemittel"] != null)
                {
                    suchtext = Session["SuchtextWerbemittel"].ToString();
                }
                
                if (Session["FilterWerbemittel"] != null)
                {
                    Filter = int.Parse(Session["FilterWerbemittel"].ToString());
                }

                string ersterTerm = suchtext.ToLower();
                List<Werbemittelauftrag> bla;

                switch (Filter)
                {
                    case 1: if ( suchtext == "")
                        {
                            bla = dms.Werbemittelauftraege.OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                        }
                        else
                        {
                            bla = dms.Werbemittelauftraege.Where(r => r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                                || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                        } break;
                    case 2: if (suchtext == "")
                        {
                            bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3).OrderByDescending(r => r.Erstellungsdatum).ToList();
                        }
                        else
                        {
                            bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3 && (r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                                || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower()))).OrderByDescending(r => r.Erstellungsdatum).ToList();
                        } break;
                    case 3: if (suchtext == "")
                        {
                            bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID == 3 || r.Stat.ID == 4).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                        }
                        else
                        {
                            bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID == 3 || r.Stat.ID == 4 && (r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                                || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                                || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower()))).OrderByDescending(r => r.Versanddatum).Take(200).ToList();
                        } break;
                    default: bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList(); break;
                }
                return View(bla);
            }
            else
            {
                ICollection<Werbemittelauftrag> auftraege = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3).OrderByDescending(r => r.Erstellungsdatum).ToList();
                return View(auftraege);
            }
        }      

        public ActionResult AuftragBearbeiten(FormCollection fc, int AuftragID=0)
        {
            if (fc["AuftragAnlegen"] != null)
            {
                return RedirectToAction("Anlegen");
            }
            if (fc["DatenAendern"] != null && AuftragID != 0)
            {
                return RedirectToAction("Aendern", new { AuftragID = AuftragID });
            }
            if (fc["AuftragPacken"] != null && AuftragID != 0)
            {
                return RedirectToAction("WerbemittelPacken", new { AuftragID = AuftragID });
            }
            if (fc["Auftragsdetails"] != null && AuftragID != 0)
            {
                return RedirectToAction("Werbemittelauftragdetails", new { AuftragID = AuftragID });
            }
            if (fc["Packliste"] != null )
            {
                return RedirectToAction("Packliste");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Anlegen()
        {
            ViewBag.ArtikelAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active).ToList();
            return View("WerbemittelauftragsdatenEingabe");
        }

        public ActionResult Aendern(int AuftragID = 0)
        {
            ViewBag.ArtikelAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active).ToList();
            Werbemittelauftrag WMA = dms.Werbemittelauftraege.Find(AuftragID);
            ViewData["WerbemittelauftragID"] = WMA.WerbemittelauftragID;
            return View("WerbemittelauftragsdatenEingabe", WMA);
        }

        public ActionResult WerbemittelPacken(int AuftragID = 0)
        {
            Werbemittelauftrag WMA = dms.Werbemittelauftraege.Find(AuftragID);
            ICollection<Paketpreis> paketpreise = dms.Paketpreise.ToList();
            ViewData["Paketpreise"] = paketpreise;
            return View(WMA);
        }

        public ActionResult Werbemittelauftragdetails(int AuftragID = 0)
        {
            Werbemittelauftrag WMA = dms.Werbemittelauftraege.Find(AuftragID);
            return View(WMA);
        }

        public ActionResult Packliste()
        {
            ICollection<Werbemittelauftrag> auftraege = dms.Werbemittelauftraege.Where(r => r.Stat.ID > 0 && r.Stat.ID < 3).OrderBy(r => r.Erstellungsdatum).ToList();
            return View(auftraege);
        }

        public ActionResult WMABearbeitungSpeichern(FormCollection fc, int WerbemittelauftragID = 0, int HighPID=0, string startPacken="" )
        {
            
            Werbemittelauftrag wma = dms.Werbemittelauftraege.Find(WerbemittelauftragID);
            for (int i = 1; i <= HighPID; i++)
            {
                if (fc["Tb_Ge_" + i.ToString()] != null)
                {
                    Paket paket = new Paket { artikelmenge = new List<Auftragsmenge>() };
                    if (fc["Hd_Va_" + i] == "Brief" || fc["Hd_Va_" + i] == "Brief International" || fc["Hd_Va_" + i] == "Abholung" || fc["Hd_Va_" + i] == "Messecar")
                    {
                        paket.Paketnummer = "-";
                    }
                    else
                    {
                        paket.Paketnummer = fc["Tb_Pn_" + i];
                    }
                    paket.Versandart = fc["Hd_Va_" + i];
                    paket.Gewicht = float.Parse(fc["Tb_Ge_" + i]);
                    paket.Preis = float.Parse(fc["Tb_Pr_" + i]);
                    wma.Auftragsmengen = dms.Werbemittelauftraege.Find(wma.WerbemittelauftragID).Auftragsmengen;
                    wma.Verschickungskosten += float.Parse(fc["Tb_Pr_" + i]);
                    paket.Ruecksendedatum = DateTime.Now.AddHours(-1);
                    paket.Versanddatum = DateTime.Now;
                    paket.versendet = true;
                    wma.Pakete.Add(paket);
                    dms.SaveChanges();

                    var Lieferadresse = wma.Auftraggeberadresse;
                    if (wma.Lieferadresse != null)
                    {
                        Lieferadresse = wma.Lieferadresse;
                    }

                    foreach(var item in wma.Auftragsmengen.ToList())
                    {
                        if (item.menge >= item.gelieferteMenge)
                        {
                            if (fc["Tmenge_PID" + i.ToString() + "_AID(" + item.AuftragsmengeID.ToString() + ")"] != null && fc["Tmenge_PID" + i + "_AID(" + item.AuftragsmengeID + ")"].Length > 0)
                            {
                                int Menge = int.Parse(fc["Tmenge_PID" + i + "_AID(" + item.AuftragsmengeID + ")"]);
                                item.gelieferteMenge += Menge;
                                paket.artikelmenge.Add(new Auftragsmenge{artikel=item.artikel, menge=Menge, paket=paket});
                                item.artikel.Bestand -= Menge;
                                item.artikel.BAE.Add(new Bestandsaenderung { Menge = Menge, Datum = DateTime.Now, Grund = "Versendet an " + Lieferadresse.Name, WMA = wma });
                                if (item.artikel.Meldebestand >= item.artikel.Bestand && item.artikel.Active)
                                {
                                    Extensions.NotificationUndercutMeldebestand(item.artikel);
                                }
                                if (item.artikel.Sicherheitsbestand >= item.artikel.Bestand && item.artikel.Active)
                                {
                                    Extensions.NotificationUndercutSicherheitsbestand(item.artikel);
                                }
                                dms.SaveChanges();
                            }
                            if (fc["CBMenge_PID" + i.ToString() + "_AID(" + item.AuftragsmengeID + ")"] != null && fc["CBMenge_PID" + i.ToString() + "_AID(" + item.AuftragsmengeID.ToString() + ")"] == "on")
                            {
                                int Menge = item.menge-item.gelieferteMenge;
                                item.gelieferteMenge += Menge;
                                paket.artikelmenge.Add(new Auftragsmenge { artikel = item.artikel, menge = Menge, paket = paket });
                                item.artikel.Bestand -= Menge;
                                item.artikel.BAE.Add(new Bestandsaenderung { Menge = Menge, Datum = DateTime.Now, Grund = "Versand an " + Lieferadresse.Name, WMA = wma });
                                if (item.artikel.Meldebestand >= item.artikel.Bestand && item.artikel.Active)
                                {
                                    Extensions.NotificationUndercutMeldebestand(item.artikel);
                                }
                                if (item.artikel.Sicherheitsbestand >= item.artikel.Bestand && item.artikel.Active)
                                {
                                    Extensions.NotificationUndercutSicherheitsbestand(item.artikel);
                                }
                                dms.SaveChanges();
                            }
                        }
                    }
                }
            }
            bool allSend = true;
            foreach (var intem in wma.Auftragsmengen)
            {
                if (intem.gelieferteMenge < intem.menge)
                {
                    allSend = false;
                    wma.Stat = dms.Stati.Find(2);
                    break;
                }
            }
            if (allSend == true)
            {
                wma.Stat = dms.Stati.Find(3);
            }
            if (!String.IsNullOrEmpty(startPacken))
            {
                int packseconds = DateTime.Now.Subtract(DateTime.Parse(fc["startPacken"])).Seconds;
                wma.ZeitAuftragPacken = wma.ZeitAuftragPacken + packseconds;
            }

            wma.Versanddatum = DateTime.Now;
            dms.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AutoSearchMesse(string term)
        {
            var messen = dms.Messen.Where(r => r.Name.ToLower().Contains(term.ToLower()) && r.Active).Take(20).Select(r => new { label = r.Name });
            return Json(messen, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoSearchKunde(string term)
        {
            var kunden = dms.Kunden.Where(r => r.Name.ToLower().Contains(term.ToLower())).Take(20).ToList();
            List<string> kundenadressen = new List<string>();
            
            foreach (var item in kunden)
            {
                string adresse = item.Name;
                if (item.Hauptadresse != null && item.Hauptadresse.Name2 != null)
                {
                    adresse += " -- " + item.Hauptadresse.Name2;
                }
                if (item.Hauptadresse != null && item.Hauptadresse.Name3 != null)
                {
                    adresse += " -- " + item.Hauptadresse.Name3;
                }

                kundenadressen.Add(adresse);
            }
            return Json(kundenadressen, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutocompleteLand(string term)
        {
            var Laender = dms.Laender.Where(r => r.land.StartsWith(term)).Take(20).Select(r => new { label = r.land });
            return Json(Laender, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MesseWaehlen(string Messename = "ka")
        {
            Messe messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
            if (messe.isLandesmesse)
            {
                ViewData["isLandesmesse"] = true;
            }
            ViewData["MesseChange"] = "true";
            return PartialView("Messedetails", messe);
        }

        public ActionResult MesseAuftraegeWaehlen(string Messename = "ka")
        {
            Messe messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
            if (messe != null)
            {
                return PartialView("PacklisteAuftraege", messe.auftraege);
            }
            else
            {
                return PartialView("PacklisteAuftraege");
            }
            
        }

        public ActionResult KundeWaehlen(string kundenname = "ka", string Name2="ka", string Name3 = "ka")
        {
            Kunde kunde = null;
            if (kundenname != "ka")
            {
                if (Name2 != "ka")
                {
                    if (Name3 != "ka")
                    {
                        kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname) && r.Hauptadresse.Name2.Equals(Name2) && r.Hauptadresse.Name3.Equals(Name3));
                    }
                    else
                    {
                        kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname) && r.Hauptadresse.Name2.Equals(Name2));
                        if (kunde == null)
                        {
                            kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname) && r.Hauptadresse.Name3.Equals(Name2));
                        }
                    }
                }
                else
                {
                    kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname));
                }
            }
            else
            {
                ViewData["empty"] = "true";
            }
            ViewBag.Kunde = kunde;
            return PartialView("Adressangaben");
        }

        public ActionResult KundeLookUp(string kundenname = "", string Name2="", string Name3="")
        {
            System.Diagnostics.Debug.WriteLine("Namen: " + kundenname + " -- " + Name2 + " -- " + Name3);
            Kunde kunde = null;
            if (kundenname != "ka")
            {
                if (Name2 != "ka")
                {
                    if (Name3 != "ka")
                    {
                        kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname) && r.Hauptadresse.Name2.Equals(Name2) && r.Hauptadresse.Name3.Equals(Name3));
                    }
                    else
                    {
                        kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname) && r.Hauptadresse.Name2.Equals(Name2));
                    }
                }
                else
                {
                    kunde = dms.Kunden.SingleOrDefault(r => r.Name.Equals(kundenname));
                }
            }
            
            int KundenId = 0;
            if (kunde != null)
            {
                KundenId = kunde.KundeID;
            }
            return Json(KundenId);
        }

        public ActionResult MesseLookUP(string Messename = "ka")
        {
            if (Messename.Length == 0)
            {
                Messename = "ka";
            }
            Messe messe = dms.Messen.SingleOrDefault(r => r.Name.ToLower().Contains(Messename.ToLower()));
            bool messeExistiert = false;
            if (messe != null)
            {
                messeExistiert = true;
            }
            return Json(messeExistiert);
        }

        public ActionResult LandLookUP(string cc = "")
        {
            string landname="";
            Land land = dms.Laender.SingleOrDefault(r => r.CountryCode == cc);
            if (land != null)
            {
                landname = land.land;
            }
            return Json(landname);
        }

        public ActionResult getAuftragsartikelLageberbestand(int AuftragID)
        {
            Werbemittelauftrag WMA =  dms.Werbemittelauftraege.Find(AuftragID);
            return PartialView("SchnellInfoAuftragsartikelLagerbestandstatus", WMA);
        }

        public ActionResult LandLookUPName(string Name = "")
        {
            string landname = "";
            Land land = dms.Laender.SingleOrDefault(r => r.land == Name);
            if (land != null)
            {
                landname = land.land;
            }
            return Json(landname);
        }

        public ActionResult KennzeichenCheck(string Kennzeichen = "")
        {
            bool AuftragVorhanden = false;
            if(dms.Werbemittelauftraege.SingleOrDefault(r => r.Kennzeichnung == Kennzeichen) != null){
                AuftragVorhanden = true;
            }
            return Json(AuftragVorhanden);
        }

        public ActionResult GetDataMatrixKennzeichen()
        {
            var kennzeichen = "";
            if (dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMDM")).Count() > 0)
            {
                Werbemittelauftrag wmala = dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMDM")).ToList().Last();
                string laKennzeichnung = wmala.Kennzeichnung;
                if (wmala.Erstellungsdatum.Year < DateTime.Now.Year)
                {
                    kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_000001";
                }
                else
                {
                    int number = int.Parse(laKennzeichnung.Substring(9, 6));
                    number += 1;
                    if (number < 10)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_00000" + number.ToString();
                    }
                    if (number > 9 && number < 100)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_0000" + number.ToString();
                    }
                    if (number > 99 && number < 1000)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_000" + number.ToString();
                    }
                    if (number > 999 && number < 10000)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_00" + number.ToString();
                    }
                    if (number > 9999 && number < 100000)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_0" + number.ToString();
                    }
                    if (number > 99999 && number < 1000000)
                    {
                        kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_" + number.ToString();
                    }
                }
            }
            else
            {
                kennzeichen = "WMDM" + DateTime.Now.Year.ToString() + "_000001";
            }
            return Json(kennzeichen);
        }

        public ActionResult AuftragsdatenSpeichern(FormCollection fc, Kunde Kunde, 
            Werbemittelauftrag WMA, string startAnlegen = "", string XMLAuftragDateinamen = "", string Kennzeichnung = 
            "ka", string HalleUStanden = "", string Bemerkung = "", string Auftragsmailtext = "", 
            Kontakdaten Auftraggeberadresse = null, Kontakdaten Lieferadresse = null, 
            Kontakdaten Rechnungsadresse = null, Kontakdaten Austelleradresse = null)
        {
            ViewBag.ArtikelAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true).ToList();
            Werbemittelauftrag wa = dms.Werbemittelauftraege.SingleOrDefault(r => r.WerbemittelauftragID == WMA.WerbemittelauftragID);
            Kunde kun = dms.Kunden.SingleOrDefault(r => r.KundeID == Kunde.KundeID);
            string AufLand = fc["AuftraggeberLand"].ToString();
            string RecLand = fc["RechnungLand"].ToString();
            string LieLand = fc["LieferLand"].ToString();
            string AusLand = fc["AustellerLand"].ToString();
            Land AuftLand = dms.Laender.SingleOrDefault(r => r.land == AufLand);
            Land RechLand = dms.Laender.SingleOrDefault(r => r.land == RecLand);
            Land LiefLand = dms.Laender.SingleOrDefault(r => r.land == LieLand);
            Land AustLand = dms.Laender.SingleOrDefault(r => r.land == AusLand);
            Kontakdaten AufAdresse = dms.Kontaktdatenn.SingleOrDefault(r => r.KontakdatenID == Auftraggeberadresse.KontakdatenID);
            Kontakdaten LieAdresse = dms.Kontaktdatenn.SingleOrDefault(r => r.KontakdatenID == Lieferadresse.KontakdatenID);
            Kontakdaten RecAdresse = dms.Kontaktdatenn.SingleOrDefault(r => r.KontakdatenID == Rechnungsadresse.KontakdatenID);
            Kontakdaten AusAdresse = dms.Kontaktdatenn.SingleOrDefault(r => r.KontakdatenID == Austelleradresse.KontakdatenID);
            bool land = true;
            if (AuftLand != null)
            {
                Auftraggeberadresse.Land = AuftLand;
            }
            else
            {
                if (!String.IsNullOrEmpty(Auftraggeberadresse.Name))
                {
                    land = false;
                    ViewBag.ErrLandAuftraggeber = true;
                }
            }
            
            if (LiefLand != null)
            {
                Lieferadresse.Land = LiefLand;
            }
            else
            {
                if (!String.IsNullOrEmpty(Lieferadresse.Name))
                {
                    land = false;
                    ViewBag.ErrLandLiefer = true;
                }
            }
            if (RechLand != null)
            {
                Rechnungsadresse.Land = RechLand;
            }
            else
            {
                if (!String.IsNullOrEmpty(Rechnungsadresse.Name))
                {
                    land = false;
                    ViewBag.ErrLandRechnung = true;
                }
            }
            if (AustLand != null)
            {
                Austelleradresse.Land = AustLand;
            }
            else
            {
                if (!String.IsNullOrEmpty(Austelleradresse.Name))
                {
                    land = false;
                    ViewBag.ErrLandAusteller = true;
                }
            }
            if (kun == null)
            {
                ICollection<Kunde> Kunden = null;
                
                if (!String.IsNullOrEmpty(Auftraggeberadresse.Name2))
                {
                    if (!String.IsNullOrEmpty(Auftraggeberadresse.Name3))
                    {
                        Kunden = dms.Kunden.Where(r => r.Name.Equals(Kunde.Name) && r.Hauptadresse.Name2.Equals(Auftraggeberadresse.Name2) && r.Hauptadresse.Name3.Equals(Auftraggeberadresse.Name3)).ToList();
                    }
                    else
                    {
                        Kunden = dms.Kunden.Where(r => r.Name.Equals(Kunde.Name) && r.Hauptadresse.Name2.Equals(Auftraggeberadresse.Name2)).ToList();
                    }
                }
                else
                {
                    Kunden = dms.Kunden.Where(r => r.Name.Equals(Kunde.Name)).ToList();
                }

                if (Kunden.Count() == 0)
                {
                    dms.Kunden.Add(Kunde);
                    dms.SaveChanges();
                    Kunde.Hauptadresse = new Kontakdaten { Name = Auftraggeberadresse.Name, Name2 = Auftraggeberadresse.Name2, Name3 = Auftraggeberadresse.Name3, Strasse = Auftraggeberadresse.Strasse, PLZ = Auftraggeberadresse.PLZ, Ort = Auftraggeberadresse.Ort, Land = Auftraggeberadresse.Land, Telefon = Auftraggeberadresse.Telefon, EMail = Auftraggeberadresse.EMail, Kunde = Kunde };
                    kun = Kunde;
                }
                if (Kunden.Count() == 1)
                {
                    Kunde kunName = dms.Kunden.Find(Kunden.First().KundeID);
                    kunName.Hauptadresse.Name2 = Auftraggeberadresse.Name2; kunName.Hauptadresse.Name3 = Auftraggeberadresse.Name3; kunName.Hauptadresse.Strasse = Auftraggeberadresse.Strasse; kunName.Hauptadresse.PLZ = Auftraggeberadresse.PLZ; kunName.Hauptadresse.Ort = Auftraggeberadresse.Ort; kunName.Hauptadresse.Land = Auftraggeberadresse.Land; kunName.Hauptadresse.Telefon = Auftraggeberadresse.Telefon; kunName.Hauptadresse.EMail = Auftraggeberadresse.EMail;
                    dms.SaveChanges();
                    kun = kunName;
                }
                if (Kunden.Count() > 1)
                {
                    Kunde kunName = dms.Kunden.Find(Kunden.First().KundeID);
                    kunName.Hauptadresse.Name2 = Auftraggeberadresse.Name2; kunName.Hauptadresse.Name3 = Auftraggeberadresse.Name3; kunName.Hauptadresse.Strasse = Auftraggeberadresse.Strasse; kunName.Hauptadresse.PLZ = Auftraggeberadresse.PLZ; kunName.Hauptadresse.Ort = Auftraggeberadresse.Ort; kunName.Hauptadresse.Land = Auftraggeberadresse.Land; kunName.Hauptadresse.Telefon = Auftraggeberadresse.Telefon; kunName.Hauptadresse.EMail = Auftraggeberadresse.EMail;
                    dms.SaveChanges();
                    kun = kunName;
                }
            }
            
            Messe Messe = null;
            if (fc["MesseID"] != null)
            {
                Messe = dms.Messen.Find(int.Parse(fc["MesseID"]));
            }
            string kennzeichen = "";
            if (Kennzeichnung.Length > 3)
            {
                kennzeichen = Kennzeichnung;
            }
            else
            {
                if (dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMM") && !r.Kennzeichnung.Contains("WMMA")).Count() > 0)
                {
                    Werbemittelauftrag wmala = dms.Werbemittelauftraege.Where(r => r.Kennzeichnung.Contains("WMM") && !r.Kennzeichnung.Contains("WMMA")).ToList().Last();
                    string laKennzeichnung = wmala.Kennzeichnung;
                    if (wmala.Erstellungsdatum.Year < DateTime.Now.Year)
                    {
                        kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_000001";
                    }
                    else
                    {
                        int number = int.Parse(laKennzeichnung.Substring(8, 6));
                        number += 1;
                        if (number < 10)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_00000" + number.ToString();
                        }
                        if (number > 9 && number < 100)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_0000" + number.ToString();
                        }
                        if (number > 99 && number < 1000)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_000" + number.ToString();
                        }
                        if (number > 999 && number < 10000)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_00" + number.ToString();
                        }
                        if (number > 9999 && number < 100000)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_0" + number.ToString();
                        }
                        if (number > 99999 && number < 1000000)
                        {
                            kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_" + number.ToString();
                        }
                    }
                }
                else
                {
                    kennzeichen = "WMM" + DateTime.Now.Year.ToString() + "_000001";
                }
            }

            DateTime Erstellungsdatum = DateTime.Now;

            if (Messe != null && kun != null )
            {
                if (wa == null)
                {
                    wa = new Werbemittelauftrag { Stat = dms.Stati.Find(1), Kennzeichnung = kennzeichen, messe = Messe, Erstellungsdatum = Erstellungsdatum,
                        Versanddatum = Erstellungsdatum, Bestelldatum=WMA.Bestelldatum, Auftragsmengen = new List<Auftragsmenge>(), Verschickungskosten = 0,
                        kunde = kun, Pakete = new List<Paket>(), HalleUStand = HalleUStanden, Bemerkung = Bemerkung, Auftragsmailtext = Auftragsmailtext, isLandesmesseauftrag=WMA.isLandesmesseauftrag };
                    dms.Werbemittelauftraege.Add(wa);
                }
                else
                {
                    wa.Pakete = dms.Werbemittelauftraege.Find(wa.WerbemittelauftragID).Pakete;
                    wa.Auftragsmengen = dms.Werbemittelauftraege.Find(wa.WerbemittelauftragID).Auftragsmengen;
                    wa.Bemerkung = Bemerkung.ToString();
                    wa.HalleUStand = HalleUStanden;
                    wa.Auftragsmailtext = Auftragsmailtext;
                }
                try
                {
                    dms.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                
                if (kun.KundeID != wa.kunde.KundeID)
                {
                    dms.Kunden.Find(wa.kunde.KundeID).auftraege.Remove(wa);
                    wa.kunde = kun;
                    dms.SaveChanges();
                }

                //Adressangaben dem Auftrag zuordnen und aktualisieren
                if (AufAdresse == null)
                {
                    if (String.IsNullOrEmpty(Auftraggeberadresse.Name))
                    {
                        if (wa.Auftraggeberadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Auftraggeberadresse);
                        }
                    }
                    else
                    {
                        if (wa.Auftraggeberadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Auftraggeberadresse);
                            wa.Auftraggeberadresse = Auftraggeberadresse;
                        }
                        else
                        {
                            wa.Auftraggeberadresse = Auftraggeberadresse;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Auftraggeberadresse.Name))
                    {
                        dms.Kontaktdatenn.Remove(wa.Auftraggeberadresse);
                    }
                    else
                    {
                        if (wa.Auftraggeberadresse.KontakdatenID == AufAdresse.KontakdatenID)
                        {
                            dms.Entry(AufAdresse).CurrentValues.SetValues(Auftraggeberadresse);
                        }
                        else
                        {
                            dms.Kontaktdatenn.Remove(wa.Auftraggeberadresse);
                            wa.Auftraggeberadresse = AufAdresse;
                        }
                        AufAdresse.Land = AuftLand;
                    }
                }
                dms.SaveChanges();

                if (LieAdresse == null)
                {
                    if (String.IsNullOrEmpty(Lieferadresse.Name))
                    {
                        if (wa.Lieferadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Lieferadresse);
                        }
                    }
                    else
                    {
                        if (wa.Lieferadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Lieferadresse);
                            wa.Lieferadresse = Lieferadresse;
                        }
                        else
                        {
                            wa.Lieferadresse = Lieferadresse;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Lieferadresse.Name))
                    {
                        dms.Kontaktdatenn.Remove(wa.Lieferadresse);
                    }
                    else
                    {
                        if (wa.Lieferadresse.KontakdatenID == LieAdresse.KontakdatenID)
                        {
                            dms.Entry(LieAdresse).CurrentValues.SetValues(Lieferadresse);
                        }
                        else
                        {
                            dms.Kontaktdatenn.Remove(wa.Lieferadresse);
                            wa.Lieferadresse = LieAdresse;
                        }
                        LieAdresse.Land = LiefLand;
                    }
                }
                try
                {
                    dms.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                    {
                        foreach (var item in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine("error: " + item.ErrorMessage);
                        }
                    }
                }


                if (RecAdresse == null)
                {
                    if (String.IsNullOrEmpty(Rechnungsadresse.Name))
                    {
                        if (wa.Rechnungsadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Rechnungsadresse);
                        }
                    }
                    else
                    {
                        if (wa.Rechnungsadresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Rechnungsadresse);
                            wa.Rechnungsadresse = Rechnungsadresse;
                        }
                        else
                        {
                            wa.Rechnungsadresse = Rechnungsadresse;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Rechnungsadresse.Name))
                    {
                        dms.Kontaktdatenn.Remove(wa.Rechnungsadresse);
                    }
                    else
                    {
                        if (wa.Rechnungsadresse.KontakdatenID == RecAdresse.KontakdatenID)
                        {
                            dms.Entry(RecAdresse).CurrentValues.SetValues(Rechnungsadresse);
                        }
                        else
                        {
                            dms.Kontaktdatenn.Remove(wa.Rechnungsadresse);
                            wa.Rechnungsadresse = RecAdresse;
                        }
                        RecAdresse.Land = RechLand;
                    }
                }
                try{
                    dms.SaveChanges();
                }
                catch
                {
                    foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                    {
                        foreach (var item in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine("error: " + item.ErrorMessage);
                        }
                    }
                }
                
                if (AusAdresse == null)
                {
                    if (String.IsNullOrEmpty(Austelleradresse.Name))
                    {
                        if (wa.Austelleradresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Austelleradresse);
                        }
                    }
                    else
                    {
                        if (wa.Austelleradresse != null)
                        {
                            dms.Kontaktdatenn.Remove(wa.Austelleradresse);
                            wa.Austelleradresse = Austelleradresse;
                        }
                        else
                        {
                            wa.Austelleradresse = Austelleradresse;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Austelleradresse.Name))
                    {
                        dms.Kontaktdatenn.Remove(wa.Austelleradresse);
                    }
                    else
                    {
                        if (wa.Austelleradresse.KontakdatenID == AusAdresse.KontakdatenID)
                        {
                            dms.Entry(AusAdresse).CurrentValues.SetValues(Austelleradresse);
                        }
                        else
                        {
                            dms.Kontaktdatenn.Remove(wa.Austelleradresse);
                            wa.Austelleradresse = AusAdresse;
                        }
                        AusAdresse.Land = AustLand;
                    }
                }
                dms.SaveChanges();

                //Artikel dem Auftrag zuordnen
                for (int i = 1; fc["AA_" + i.ToString()] != null; i++)
                {
                    Artikel artikel = dms.Artikell.Find(int.Parse(fc["AID_" + i.ToString()]));
                    if (int.Parse(fc["AA_" + i.ToString()]) > 0)
                    {
                        if (wa.Auftragsmengen.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID) != null)
                        {
                            Auftragsmenge am = wa.Auftragsmengen.Single(r => r.artikel.ArtikelID == artikel.ArtikelID);
                            am.menge = int.Parse(fc["AA_" + i.ToString()]);

                        }
                        else
                        {
                            wa.Auftragsmengen.Add(new Auftragsmenge { artikel = artikel, auftrag = wa, menge = int.Parse(fc["AA_" + i.ToString()]) });
                        }
                    }
                    else
                    {
                        if (wa.Auftragsmengen.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID) != null)
                        {
                            Auftragsmenge am = wa.Auftragsmengen.Single(r => r.artikel.ArtikelID == artikel.ArtikelID);
                            wa.Auftragsmengen.Remove(am);
                            dms.Auftragsmengen.Remove(am);
                        }
                    }  
                }
                bool allesVerpackt = true;
                foreach (var aMenge in wa.Auftragsmengen)
                {
                    if (aMenge.gelieferteMenge < aMenge.menge)
                    {
                        allesVerpackt = false;
                    }
                }
                if (allesVerpackt)
                {
                    wa.Stat = dms.Stati.Find(3);
                }


                dms.SaveChanges();
            }

            if (!String.IsNullOrEmpty(startAnlegen))
            {
                int zeitAnlegen = DateTime.Now.Subtract(DateTime.Parse(startAnlegen)).Seconds;
                wa.ZeitAuftragAnlegen = zeitAnlegen + wa.ZeitAuftragAnlegen;
                dms.SaveChanges(); 
            }

            if (!String.IsNullOrEmpty(XMLAuftragDateinamen))
            {
                string Einlesedatum = "_"+DateTime.Now.Year.ToString() +"-"+ DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                string DateinameMitDatum = XMLAuftragDateinamen.Replace(".xml", "") + Einlesedatum + ".xml";
                System.IO.File.Move(PathToXMLAuftragsdateien + XMLAuftragDateinamen, PathToXMLAuftragsdateien 
                    + "AngelegtWerbemittelapplikation/" + DateinameMitDatum);
            }


            if (Messe != null && kun != null && land != false)
            {

                return RedirectToAction("Index");
            }
            else
            {
                return View("WerbemittelauftragsdatenEingabe", wa);
            }
        }

        public ActionResult AuftragLoeschen(int WerbemittelauftragsID = 0)
        {
            Werbemittelauftrag WMA = dms.Werbemittelauftraege.Find(WerbemittelauftragsID);
            if (WMA.Pakete.Count < 1)
            {
                var AMengen = dms.Auftragsmengen.Where(r => r.auftrag.WerbemittelauftragID== WMA.WerbemittelauftragID);
                foreach (var item in AMengen)
                {
                    dms.Auftragsmengen.Remove(item);
                }
                if (WMA.Auftraggeberadresse != null)
                {
                    dms.Kontaktdatenn.Remove(WMA.Auftraggeberadresse);
                }
                dms.SaveChanges();
                if (WMA.Lieferadresse != null)
                {
                    dms.Kontaktdatenn.Remove(WMA.Lieferadresse);
                }
                if (WMA.Rechnungsadresse != null)
                {
                    dms.Kontaktdatenn.Remove(WMA.Rechnungsadresse);
                }
                if (WMA.Austelleradresse != null)
                {
                    dms.Kontaktdatenn.Remove(WMA.Austelleradresse);
                }
                var BAE = dms.BAenderungen.Where(r => r.WMA.WerbemittelauftragID == WMA.WerbemittelauftragID);
                foreach (var bae in BAE)
                {
                    dms.BAenderungen.Remove(bae);
                }
                dms.Werbemittelauftraege.Remove(WMA);
                dms.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Werbemittelauftragdetails");
            }
        }

        public ActionResult Auftragsartikel(int WerbemittelauftragID = 0, string Messename = "ka")
        {
            ViewBag.ArtikelAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true).ToList();
            Messe messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
            Werbemittelauftrag WMA = dms.Werbemittelauftraege.SingleOrDefault(r => r.WerbemittelauftragID == WerbemittelauftragID);
            if (messe != null)
            {
                if (WMA != null && messe.MesseID == WMA.messe.MesseID)
                {
                    return PartialView(WMA);
                }
                else
                {
                    ViewData["Messe"] = messe;
                    return PartialView(WMA);
                }
            }
            else
            {
                return PartialView();
            }
        }
        
        public ActionResult Auftragsuche(FormCollection fc=null)
        {

            string suchtext = "";
            int Filter = 0;
            suchtext = fc["FilterEins"];
            Session["SuchtextWerbemittel"] = suchtext;
            Filter = int.Parse(fc["SuchFilterRadio"]);
            Session["FilterWerbemittel"] = Filter;
            string ersterTerm = suchtext;
            List<Werbemittelauftrag> bla;
            switch (Filter)
            {
                case 1: if (suchtext == "")
                    {
                        bla = dms.Werbemittelauftraege.OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                    }
                    else
                    {
                        bla = dms.Werbemittelauftraege.Where(r => r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                            || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                    } break;
                case 2: if (suchtext == "")
                    {
                        bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                    }
                    else
                    {
                        bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3 && (r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                            || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower()))).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                    } break;
                case 3: if (suchtext == "")
                    {
                        bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID == 3 || r.Stat.ID == 4).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList();
                    }
                    else
                    {
                        bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID == 3 || r.Stat.ID == 4 && (r.messe.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Auftraggeberadresse.Name.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name2.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name2.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Name3.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Name3.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.Ort.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.Ort.ToLower().Contains(ersterTerm.ToLower())
                            || r.Lieferadresse.PLZ.ToLower().Contains(ersterTerm.ToLower()) || r.Auftraggeberadresse.PLZ.ToLower().Contains(ersterTerm.ToLower())
                            || r.Kennzeichnung.ToLower().Contains(ersterTerm.ToLower()))).OrderByDescending(r => r.Versanddatum).Take(500).ToList();
                    } break;
                default: bla = dms.Werbemittelauftraege.Where(r => r.Stat.ID < 3).OrderByDescending(r => r.Erstellungsdatum).Take(200).ToList(); break;
            }

            return PartialView("Auftragliste", bla);
        }

        public ActionResult PacklisteErstellen(FormCollection fc, int IdMesse=0)
        {
            ICollection<Werbemittelauftrag> auftraegeAll = dms.Werbemittelauftraege.Where(r => r.Stat.ID > 0 && r.Stat.ID < 3).OrderBy(r => r.Erstellungsdatum).ToList();
            List<Werbemittelauftrag> auftraege = new List<Werbemittelauftrag>();
            foreach (var item in auftraegeAll)
            {
                if (fc["Auftrag_" + item.WerbemittelauftragID.ToString()] != null)
                {
                    auftraege.Add(item);
                }
            }
            if (auftraege.Count() > 0)
            {
                return PartialView("Packlistenartikel", auftraege);
            }
            else
            {
                return PartialView("Packlistenartikel");
            }
        }

        public ActionResult PollingDatei(int WMAid = 0, string Gewicht = "ka")
        {
            string[] files = System.IO.Directory.GetFiles(@"\inetpub\webtrans\WMAPolling\Paketdaten", "*.xls");
            if (files.Count() >= 1)
            {
                foreach (var datei in files)
                {
                    System.IO.File.Delete(datei);
                }
            }
            Werbemittelauftrag WMA;
            string Pollingtext;
            if (WMAid == 0 || Gewicht == "ka")
            {
                WMA = null;
            }
            else
            {
                WMA = dms.Werbemittelauftraege.Find(WMAid);
                if (WMA.Lieferadresse != null)
                {
                    if (WMA.Lieferadresse.Land.land != "Deutschland")
                    {
                        Pollingtext = WMA.Kennzeichnung + "|53|02|5301|" + WMA.Lieferadresse.Name + "|" + WMA.Lieferadresse.Name2 + "|" + WMA.Lieferadresse.Name3 + "|"
                            + WMA.Lieferadresse.Strasse + "|" + WMA.Lieferadresse.PLZ + "|" + WMA.Lieferadresse.Ort + "|" + WMA.Lieferadresse.Land.land + "|" + Gewicht + "|7|Goods not for sale;Germany; ;" + Gewicht + ";10;|";
                    }
                    else
                    {
                        Pollingtext = WMA.Kennzeichnung + "|1|03|101|" + WMA.Lieferadresse.Name + "|" + WMA.Lieferadresse.Name2 + "|" + WMA.Lieferadresse.Name3 + "|"
                            + WMA.Lieferadresse.Strasse + "|" + WMA.Lieferadresse.PLZ + "|" + WMA.Lieferadresse.Ort + "|" + WMA.Lieferadresse.Land.land + "|" + Gewicht + "|7|Goods not for sale;Germany; ;" + Gewicht + ";10;|";
                    }
                }
                else
                {
                    if (WMA.Auftraggeberadresse.Land.land != "Deutschland")
                    {
                        Pollingtext = WMA.Kennzeichnung + "|53|02|5301|" + WMA.Auftraggeberadresse.Name + "|" + WMA.Auftraggeberadresse.Name2 + "|" + WMA.Auftraggeberadresse.Name3 + "|"
                            + WMA.Auftraggeberadresse.Strasse + "|" + WMA.Auftraggeberadresse.PLZ + "|" + WMA.Auftraggeberadresse.Ort + "|" + WMA.Auftraggeberadresse.Land.land + "|" + Gewicht + "|7|Goods not for sale;Germany; ;" + Gewicht + ";10;|";
                    }
                    else
                    {
                        Pollingtext = WMA.Kennzeichnung + "|1|03|101|" + WMA.Auftraggeberadresse.Name + "|" + WMA.Auftraggeberadresse.Name2 + "|" + WMA.Auftraggeberadresse.Name3 + "|"
                           + WMA.Auftraggeberadresse.Strasse + "|" + WMA.Auftraggeberadresse.PLZ + "|" + WMA.Auftraggeberadresse.Ort + "|" + WMA.Auftraggeberadresse.Land.land + "|" + Gewicht + "|7|Goods not for sale;Germany; ;" + Gewicht + ";10;|";
                    }
                }
                try
                {
                    TextWriter writer = new StreamWriter(@"\inetpub\webtrans\WMAPolling\PollingDaten\Polling_werbemittel.txt", false, Encoding.Default);
                    writer.WriteLine(Pollingtext);
                    writer.Close();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("exeption" + exception);
                }
            }
            return Json(WMA.kunde.Name);
        }

        public ActionResult PreisUndPaketnummerAuslesen()
        {
            string[] files = System.IO.Directory.GetFiles(@"\inetpub\webtrans\WMAPolling\Paketdaten", "*.xls");
            string filename = "";
            if (files.Count() == 1)
            {
                filename = files.ElementAt(0);
            }
            else
            {
                return Json("false");
            }

            string Preis = "";
            string Paketnummer = "";
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfwb.GetSheetAt(0);
            for (int row = 0; row <= 50; row++)
            {
                string cellcontent = "";
                if (sheet.GetRow(0).GetCell(row) != null)
                {
                    cellcontent = sheet.GetRow(0).GetCell(row).ToString();
                    if (cellcontent == "SITEMS_ENTGELD")
                    {
                        if (sheet.GetRow(1).GetCell(row) != null)
                        {
                            Preis = sheet.GetRow(1).GetCell(row).ToString();
                        }
                    }
                    if (cellcontent == "SITEMS_IDENTCODE")
                    {
                        if (sheet.GetRow(1).GetCell(row) != null)
                        {
                            Paketnummer = sheet.GetRow(1).GetCell(row).ToString();
                        }
                    }
                }
            }
            string PreisUndPaketnummer = Preis + "&" + Paketnummer;
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            return Json(PreisUndPaketnummer);
        }

        public ActionResult PaketDatenÄndern(Paket paket, float preis, float Gewicht, string Versandart, string Paketnummer ="", string Versanddatum="", bool Ruecklaeufer=false, string Rueckdatum="", string Bemerkung="", float Rueckpreis=0)
        {
            Paket pak = dms.Pakete.Find(paket.PaketID);
            Werbemittelauftrag wma = pak.auftrag;
            wma.Auftragsmengen = dms.Werbemittelauftraege.Find(wma.WerbemittelauftragID).Auftragsmengen;
            pak.Preis = preis;
            pak.Gewicht = Gewicht;
            pak.Versandart = Versandart;
            pak.Bemerkung = Bemerkung;
            var versanddatum = DateTime.Parse(Versanddatum);
            if (versanddatum != null)
            {
                pak.Versanddatum = versanddatum;
            }
            
            
            if(!String.IsNullOrEmpty(Paketnummer)){
                pak.Paketnummer = Paketnummer;
            }
            float Verschickungskosten = 0;
            foreach (var paketle in pak.auftrag.Pakete)
            {
                Verschickungskosten += paketle.Preis;
            }
            pak.auftrag.Verschickungskosten = Verschickungskosten;
            if (Ruecklaeufer)
            {
                pak.Ruecklaeufer = true;
                if (!String.IsNullOrEmpty(Rueckdatum) && Rueckdatum != "-")
                {
                    pak.Ruecksendedatum = DateTime.Parse(Rueckdatum);
                }
                
                pak.Zusatzkosten = Rueckpreis;
            }
            else
            {
                pak.Ruecklaeufer = false;
                pak.Zusatzkosten = 0;
            }

            try
            {
                dms.SaveChanges();
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
            
            return Json("true");
        }

        [HttpGet]
        public ActionResult PaketLoeschen(Paket paket)
        {
            //Artikel wieder an den Auftrag zurückbuchen
            
            Paket pak = dms.Pakete.Find(paket.PaketID);
            int AuftragsId = pak.auftrag.WerbemittelauftragID;
            Werbemittelauftrag wma = dms.Werbemittelauftraege.Find(pak.auftrag.WerbemittelauftragID);
            wma.Verschickungskosten -= pak.Preis;


            foreach (var artikelmenge in pak.artikelmenge)
            {
                Artikel artikel = dms.Artikell.Find(artikelmenge.artikel.ArtikelID);
                Auftragsmenge wmaAuftrag = wma.Auftragsmengen.Single(r => r.artikel.ArtikelID == artikel.ArtikelID);
                wmaAuftrag.gelieferteMenge -= artikelmenge.menge;
                dms.BAenderungen.Add(new Bestandsaenderung{ Artikel=artikel, Menge=artikelmenge.menge, Datum=DateTime.Now, Grund="Paket wurde gelöscht"});
                artikel.Bestand += artikelmenge.menge;
                if (artikel.Meldebestand >= artikel.Bestand && artikel.Active)
                {
                    Extensions.NotificationUndercutMeldebestand(artikel);
                }
                if (artikel.Sicherheitsbestand >= artikel.Bestand && artikel.Active)
                {
                    Extensions.NotificationUndercutSicherheitsbestand(artikel);
                }
            }
            dms.Pakete.Remove(pak);
            dms.SaveChanges();
            wma = dms.Werbemittelauftraege.Find(AuftragsId);
            if (wma.Pakete.Count > 0)
            {
                List<Paket> pakete = wma.Pakete.OrderByDescending(r => r.Versanddatum).ToList();
                wma.Versanddatum = pakete.First().Versanddatum;
                wma.Stat = dms.Stati.Find(2);
            }
            else
            {
                wma.Versanddatum = wma.Erstellungsdatum;
                wma.Stat = dms.Stati.Find(1);
            }
            dms.SaveChanges();
            return View("Werbemittelauftragdetails", wma); 
        }

        public ActionResult AuftragAbschliesen(int WerbemittelauftragsID)
        {
            Werbemittelauftrag wma = dms.Werbemittelauftraege.Find(WerbemittelauftragsID);
            var AM = dms.Auftragsmengen.Where(r => r.auftrag.WerbemittelauftragID == WerbemittelauftragsID).ToList();
            foreach (var am in AM)
            {
                if (am.menge != am.gelieferteMenge)
                {
                    am.menge = am.gelieferteMenge;
                }

                if (am.menge == 0)
                {
                    dms.Auftragsmengen.Remove(am);
                }
            }

            wma.Stat = dms.Stati.Find(3);
            dms.SaveChanges();
            return View("Werbemittelauftragdetails", wma);
        }


        public ActionResult getXMLAuftragsDateinamen()
        {
            //string OrderDirectoryPath = Server.MapPath("~/XMLAuftragsdateien");
            //string OrderDirectoryPath = @"C:\Users\NotJohn\Desktop\XML Dateien\";
            List<string> xmlFiles = Directory.GetFiles(PathToXMLAuftragsdateien, "*.xml").Select(Path.GetFileName).ToList();
            return Json(xmlFiles);
        }

        public ActionResult getXMLDateiInhalt(string Filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(Server.MapPath("~/XMLAuftragsdateien") +@"\"+ Filename);
            xmlDoc.Load(PathToXMLAuftragsdateien + Filename);
            string DateiInhalt = xmlDoc.InnerXml;
            return Json(DateiInhalt);
        }

        private string XMLEinleseLog="";

        private List<string> EinleseLog = new List<string>();

        private bool checkXMLElementIsOncePresent(XmlDocument xmlDoc, String ElementName)
        {
            XmlNodeList Elements = xmlDoc.GetElementsByTagName(ElementName);
            if (Elements.Count == 1)
            {
                XMLEinleseLog += "Element '" + ElementName + "' wurde gefunden: " + Elements[0].InnerText +"\n";
                return true;
            }
            else
            {
                XMLEinleseLog += "___Error___ Element '" + ElementName + "' wurde nicht gefunden oder es gab mehr als eines." + "\n";
                return false;
            }
        }

        [HttpPost]
        public ActionResult AuslesenDerXMLAuftragsinformationen(string Filename)
        {
            bool AuftragsdatenVollständig = true;
            XmlDocument xmlDoc = new XmlDocument();
            Messe messe;
            Werbemittelauftrag WMA = new Werbemittelauftrag
            {
                Stat = dms.Stati.Find(1),
                Erstellungsdatum = DateTime.Now,
                Versanddatum = DateTime.Now,
                Auftragsmengen = new List<Auftragsmenge>(),
                Pakete = new List<Paket>(),
                Verschickungskosten = 0,
                isLandesmesseauftrag = false,

                //kunde = kun,  Bemerkung = Bemerkung,
                //Auftragsmailtext = Auftragsmailtext, 
            };

            try
            {
                xmlDoc.Load(PathToXMLAuftragsdateien + Filename);
                //xmlDoc.Load(Server.MapPath("~/XMLAuftragsdateien") +@"\"+ Filename);
                if (xmlDoc.DocumentElement.Name == "tdSalesOrder")
                {
                    XMLEinleseLog += "'tdSalesOder' wurde gefunden. Starte Auftrag einlesen" + "\n";
                    EinleseLog.Add("'tdSalesOder' wurde gefunden. Starte Auftrag einlesen" + "\n");

                    //Suche nach Messe
                    if (checkXMLElementIsOncePresent(xmlDoc, "Messe_MesseDesc"))
                    {
                        string Messename = xmlDoc.GetElementsByTagName("Messe_MesseDesc")[0].InnerText;
                        messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
                        if (messe != null)
                        {
                            if (messe.Active)
                            {
                                WMA.messe = messe;
                            }
                            else
                            {
                                XMLEinleseLog += "___Error___ Messe '" + Messename + "' ist auf Inaktiv gesetzt " + @"\n";
                                AuftragsdatenVollständig = false;
                            }
                        }
                        else
                        {
                            XMLEinleseLog += "___Error___ Messe '" + Messename + "' wurde nicht gefunden " + @"\n";
                            AuftragsdatenVollständig = false;
                        }
                    }

                    //Artikel der Bestellung auslesen
                    XmlNodeList tdSalesOrderPositions = xmlDoc.GetElementsByTagName("tdSalesOrderPos");
                    if (tdSalesOrderPositions.Count > 1)
                    {
                        for (int index = 0; index < xmlDoc.GetElementsByTagName("tdSalesOrderPos").Count - 1; index++)
                        {
                            string Artikelnummer = xmlDoc.GetElementsByTagName("ProductNumber")[index].InnerText;
                            string ArtikelmengeXML = xmlDoc.GetElementsByTagName("BaseQuantity")[index].InnerText.Replace(".", "");//.Replace(",", ".");
                            int Artikelmenge = 0;
                            if (!String.IsNullOrEmpty(Artikelnummer) && !String.IsNullOrEmpty(ArtikelmengeXML))
                            {
                                float Penismenge = float.Parse(ArtikelmengeXML);
                                Artikelmenge = (int)Penismenge;
                                XMLEinleseLog += "Artikelnummer wurde gefunden: " + Artikelnummer + "\n";
                                XMLEinleseLog += "Artikelmenge wurde gefunden: " + Artikelmenge + "\n";
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(Artikelnummer))
                                {
                                    XMLEinleseLog += "Artikelnummer konnte nicht gefunden werden!" + "\n";
                                    AuftragsdatenVollständig = false;
                                }
                                if (!String.IsNullOrEmpty(ArtikelmengeXML))
                                {
                                    XMLEinleseLog += "Artikelmenge konnte nicht gefunden werden!" + "\n";
                                    AuftragsdatenVollständig = false;
                                }
                            }
                            Artikel artikel= null;
                            if (dms.Artikell.Where(r => r.Artikelnummer == Artikelnummer).ToList().Count > 1)
                            {
                                XMLEinleseLog += "___Error___ Es wurden mehr als ein Artikel mit dieser Artikelnummer gefunden." + "\n";
                            }
                            else
                            {
                                artikel = dms.Artikell.SingleOrDefault(r => r.Artikelnummer == Artikelnummer);
                            }
                            
                            if (artikel != null)
                            {
                                if (artikel.Active)
                                {
                                    WMA.Auftragsmengen.Add(new Auftragsmenge { menge = Artikelmenge, artikel = artikel });
                                }
                                else
                                {
                                    XMLEinleseLog += "___Error___    Artikel ist auf Inaktiv gesetzt: '" + Artikelnummer + "'" + "\n";
                                    AuftragsdatenVollständig = false;
                                }
                            }
                            else
                            {
                                XMLEinleseLog += "___Error___    Artikel wurde nicht in der Datenbank gefunden: '" + Artikelnummer + "'" + "\n";
                                AuftragsdatenVollständig = false;
                            }
                        }

                    }
                    else
                    {
                        XMLEinleseLog += ("In der Bestellung sind keine Artikel enthalten!" + "\n");
                        AuftragsdatenVollständig = false;
                    }

                    //Auftraggeber Kontaktdaten
                    Kontakdaten Auftraggeberkontakt = new Kontakdaten();
                    if (checkXMLElementIsOncePresent(xmlDoc, "Contact_FirstName") && checkXMLElementIsOncePresent(xmlDoc, "Contact_LastName")
                         && checkXMLElementIsOncePresent(xmlDoc, "Contact_PhoneNr") && checkXMLElementIsOncePresent(xmlDoc, "Contact_EMail")
                         && checkXMLElementIsOncePresent(xmlDoc, "Contact_Country"))
                    {
                        Auftraggeberkontakt.Name = xmlDoc.GetElementsByTagName("Contact_FirstName")[0].InnerText + " " + xmlDoc.GetElementsByTagName("Contact_LastName")[0].InnerText;
                        Auftraggeberkontakt.Name2 = "Tel2: " + xmlDoc.GetElementsByTagName("Contact_PhoneNr2")[0].InnerText;
                        Auftraggeberkontakt.Name3 = "Mobile: " + xmlDoc.GetElementsByTagName("Contact_MobilePhone")[0].InnerText;
                        if (String.IsNullOrEmpty(xmlDoc.GetElementsByTagName("Contact_Street")[0].InnerText))
                        {
                            Auftraggeberkontakt.Strasse = "-";

                        }
                        if (String.IsNullOrEmpty(xmlDoc.GetElementsByTagName("Contact_PostalCode")[0].InnerText))
                        {
                            Auftraggeberkontakt.PLZ = "-";

                        }
                        if (String.IsNullOrEmpty(xmlDoc.GetElementsByTagName("Contact_City")[0].InnerText))
                        {
                            Auftraggeberkontakt.Ort = "-";

                        }
                        Auftraggeberkontakt.Telefon = xmlDoc.GetElementsByTagName("Contact_PhoneNr")[0].InnerText;
                        Auftraggeberkontakt.EMail = xmlDoc.GetElementsByTagName("Contact_EMail")[0].InnerText;
                        string CountryCode = xmlDoc.GetElementsByTagName("Contact_Country")[0].InnerText;
                        if (dms.Laender.SingleOrDefault(r => r.CountryCode == CountryCode) != null)
                        {
                            Land AuftraggeberkontaktLand = dms.Laender.SingleOrDefault(r => r.CountryCode == CountryCode);
                            Auftraggeberkontakt.Land = dms.Laender.SingleOrDefault(r => r.CountryCode == CountryCode);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(CountryCode))
                            {
                                Auftraggeberkontakt.Land = dms.Laender.SingleOrDefault(r => r.CountryCode == "DE");
                                XMLEinleseLog += "___Warnung___    Contact_Country war leer. Es wurde DE ausgewählt" + "\n";
                            }
                            else
                            {
                                XMLEinleseLog += "___Error___    Ländercode konnte nicht zugeordnet werden: " + CountryCode + "\n";
                            }                        
                        }
                        WMA.Auftraggeberadresse = Auftraggeberkontakt;
                    }
                    else
                    {
                        XMLEinleseLog += "___Error___    Auftraggeberkontaktdaten konnten nicht gefunden werden: " + "\n";
                        AuftragsdatenVollständig = false;
                    }

                    //Lieferadresse
                    Kontakdaten Lieferadresse = new Kontakdaten();
                    Land land = new Land();
                    if (checkXMLElementIsOncePresent(xmlDoc, "CustomerAddress_Name1") && checkXMLElementIsOncePresent(xmlDoc, "CustomerAddress_Street")
                        && checkXMLElementIsOncePresent(xmlDoc, "CustomerAddress_PostalCode") && checkXMLElementIsOncePresent(xmlDoc, "CustomerAddress_City")
                        && checkXMLElementIsOncePresent(xmlDoc, "CustomerAddress_Country"))
                    {
                        Lieferadresse.Name = xmlDoc.GetElementsByTagName("CustomerAddress_Name1")[0].InnerText;
                        Lieferadresse.Name2 = xmlDoc.GetElementsByTagName("CustomerAddress_Name2")[0].InnerText;
                        Lieferadresse.Strasse = xmlDoc.GetElementsByTagName("CustomerAddress_Street")[0].InnerText;
                        Lieferadresse.PLZ = xmlDoc.GetElementsByTagName("CustomerAddress_PostalCode")[0].InnerText;
                        Lieferadresse.Ort = xmlDoc.GetElementsByTagName("CustomerAddress_City")[0].InnerText;
                        string CountryCode = xmlDoc.GetElementsByTagName("CustomerAddress_Country")[0].InnerText;
                        if (dms.Laender.SingleOrDefault(r => r.CountryCode == CountryCode) != null)
                        {
                            land = dms.Laender.SingleOrDefault(r => r.CountryCode == CountryCode);
                            Lieferadresse.Land = land;
                            WMA.Lieferadresse = Lieferadresse;
                        }
                        else
                        {
                            XMLEinleseLog += "___Error___    Ländercode konnte nicht zugeordnet werden: " + CountryCode + "\n";
                        }

                        Kunde kunde = new Kunde
                        {
                            Name = Lieferadresse.Name,
                            Hauptadresse = Auftraggeberkontakt,
                            Erstellungsdatum = DateTime.Now,
                        };

                        WMA.kunde = kunde;
                    }
                    else
                    {
                        XMLEinleseLog +=  "___Error___    Lieferadressdaten konnten nicht gefunden werden: " + "\n";
                        AuftragsdatenVollständig = false;
                    }




                    //Auftrags Kennzeichnung
                    if (checkXMLElementIsOncePresent(xmlDoc, "MainReferenceNumber"))
                    {
                        string WMAKenzeichen = xmlDoc.GetElementsByTagName("MainReferenceNumber")[0].InnerText;
                        if(dms.Werbemittelauftraege.Where(r => r.Kennzeichnung == WMAKenzeichen).ToList().Count == 0)
                        {
                            WMA.Kennzeichnung = WMAKenzeichen;
                        }
                        else
                        {
                            XMLEinleseLog += "___Error___    Auftrag mit demeselben Auftragskennzeichen bereits vorhanden: " +WMAKenzeichen+ "\n";
                            AuftragsdatenVollständig = false;
                        }
                    }

                    //Bestelldatum eintragen
                    if (checkXMLElementIsOncePresent(xmlDoc, "CreationDate"))
                    {
                        string BookingDate = xmlDoc.GetElementsByTagName("CreationDate")[0].InnerText;
                        CultureInfo deCulture = new CultureInfo("de-DE");
                        DateTime Bestelldatum = DateTime.Parse(BookingDate, deCulture.DateTimeFormat);
                        WMA.Bestelldatum = Bestelldatum;
                    }

                    //Halle und Stand
                    if (checkXMLElementIsOncePresent(xmlDoc, "HALLESTAND"))
                    {
                        string HallUndStand = xmlDoc.GetElementsByTagName("HALLESTAND")[0].InnerText;
                        WMA.HalleUStand = HallUndStand;
                    }


                    //Bemerkung Auslesen
                    XmlNodeList Elements = xmlDoc.GetElementsByTagName("MemoText");
                    if (Elements.Count == 1)
                    {
                        XMLEinleseLog += "Element 'MemoText' wurde gefunden: " + Elements[0].InnerText + "\n";
                        string Bemerkung = xmlDoc.GetElementsByTagName("MemoText")[0].InnerText;
                        WMA.Bemerkung = Bemerkung;
                    }
                    else
                    {
                        XMLEinleseLog += "___Info___ Element 'MemoText' wurde nicht gefunden.";
                    }

                }
                else
                {
                    XMLEinleseLog = "Die Datei '" + Filename + "'konnte nicht eingelesen werden. Element 'tdSalesOder' nicht gefunden." + "\n";
                    AuftragsdatenVollständig = false;
                }
            }
            catch (XmlException exc)
            {
                XMLEinleseLog += "XML Datei Fehlerhaft: " + exc.Message + "\n";
            }

            if (AuftragsdatenVollständig)
            {
                var PartialViewContent = PartialView("Auftragsdaten", WMA);
                return PartialViewContent;
            }
            else
            {
                var PartialViewContent = PartialView("Auftragsdaten");
                var Response = new { Status = "False", Log=XMLEinleseLog, ViewContent=PartialViewContent};
                return Json(Response);
            }
            
        }

        public ActionResult CleanForm()
        {
            return PartialView("Auftragsdaten");
        }



        public void postXML()
        {
            var url = "http://localhost:1234/WebShop/receiveXML";
            string postData = "<?xml version='1.0' encoding='UTF-8'?><Bestellung><Auftrag id='Jihaa'><Auftraggeber> Ich, ganz allein </Auftraggeber><soab>test</soab></Auftrag></Bestellung>";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postData);
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "text/xml";
            req.Method = "POST";
            req.ContentLength = bytes.Length;
            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            string response = "";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    response = sr.ReadToEnd().Trim();
                    ViewData["response"] = response;
                }
            }
        }


        public void callXMLWebsite()
        {
            string url = "http://localhost:49849/Werbemittelverwaltung/postXML";
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
            webReq.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
        }

    }
}
