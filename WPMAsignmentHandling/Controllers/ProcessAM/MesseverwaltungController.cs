using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using WPMAsignmentHandling.Models;
using OfficeOpenXml.Drawing;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data;
using OfficeOpenXml;


namespace WPMAsignmentHandling.Controllers.ProcessAM
{
    [Authorize(Roles = "Administrator, Winkhardt-MA")]
    public class MesseverwaltungController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();


        public ActionResult Index()
        {
            Messe Marketingmesse = dms.Messen.Single(r => r.isLandesmesse);
            ViewData["Marketingmesse"] = Marketingmesse;   
            if (Session["SuchtextMesse"] != null)
            {
                
                string ersterTerm = Session["SuchtextMesse"].ToString();
                if (String.IsNullOrEmpty(ersterTerm))
                {
                    List<Messe> messen = dms.Messen.OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
                    return View(messen);
                }
                else
                {
                    List<Messe> messen = dms.Messen.Where(r => r.Name.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
                    return View(messen);
                }
                
            }
            else
            {
                ICollection<Messe> messen = dms.Messen.OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
                ICollection<Messe> messenAktuell = new List<Messe>();
                return View(messen);
            }

            
        }

        public ActionResult setMesseActivity(int MesseID, bool active)
        {
            Messe messe = dms.Messen.Find(MesseID);
            messe.Active = active;
            foreach (var artikel in messe.artikel)
            {
                artikel.Active = active;
            }
            dms.SaveChanges();
            return Json("true");
        }

        public ActionResult Suchen(FormCollection fc)
        {
            Messe Marketingmesse = dms.Messen.Single(r => r.isLandesmesse);
            ViewData["Marketingmesse"] = Marketingmesse; 
            Session["SuchtextMesse"] = fc["FilterEins"];
            string ersterTerm = fc["FilterEins"];
            List<Messe> messen = null;
            if (String.IsNullOrEmpty(ersterTerm))
            {
                messen = dms.Messen.OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
            }
            else
            {
                messen = dms.Messen.Where(r => r.Name.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
            }
            return PartialView("Messeliste", messen);
        }

        public ActionResult AutocompleteMesse(string term)
        {
            var Messen = dms.Messen.Where(r => r.Name.ToLower().Contains(term.ToLower())).Take(50).Select(r => new { label = r.Name });
            return Json(Messen, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MesseBearbeiten(FormCollection fc, int MesseID=0)
        {

            if (fc["Anlegen"] != null)
            {
                return RedirectToAction("Anlegen");
            }
            if (fc["DatenAendern"] != null && MesseID != 0)
            {

                return RedirectToAction("Aendern", new { MesseID = MesseID });
            }
            if (fc["MesseDetails"] != null && MesseID != 0)
            {

                return RedirectToAction("Messedetails", new { MesseID = MesseID });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Anlegen()
        {
            ViewBag.StandardpreisPaket = dms.Abrechnungspreise.Find(1).Preis;
            return View("Messedateneingabe");
        }

        public ActionResult MesseSpeichern(Messe mes)
        {
            if (ModelState.IsValid)
            {
                dms.Dispose();
                dms = new DMS_Winkhardt_DB();
                Messe messe = mes;
                
                if (mes.Enddatum < mes.Startdatum)
                {
                    ViewData["EndvorStart"] = "Enddatum liegt vor Startdatum!";
                    return View("Messedateneingabe", mes);
                }
                dms.Messen.Add(messe);
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
                return View("Messedateneingabe", mes);
            }     
        }

        public ActionResult Aendern(int MesseID=0)
        {
            Messe messe = dms.Messen.Find(MesseID);
            return View("Messedateneingabe",messe);
        }

        public ActionResult MesseAenderungSpeichern(Messe mes)
        {
            if (ModelState.IsValid)
            {
                if (mes.Enddatum < mes.Startdatum)
                {
                    ViewData["EndvorStart"] = "Enddatum liegt vor Startdatum!";
                    return View("Messedateneingabe", mes);
                }
                dms.Entry(mes).State = System.Data.Entity.EntityState.Modified;
                dms.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Messedateneingabe", mes);
            }
        }

        public ActionResult Messedetails(int MesseID=0)
        {
            Messe messe = dms.Messen.Find(MesseID);
            
            return View("Details", messe);
        }

        public ActionResult ExcelArtikelBestaendeExtern(DateTime Abrechnungsbegin, DateTime Abrechnungsende, int MesseID = 0)
        {
            Messe messe = dms.Messen.Find(MesseID);
            string HeadMesseKostenstelle = "Messe";
            if(messe.isLandesmesse){
                HeadMesseKostenstelle = "PSP-Nummer/ \r\n Kostenstelle";
            }

            List<Artikel> messeArtikel = messe.artikel.ToList();
            List<Artikel> ArtikellAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active == true).ToList();
            List<Artikel> Artikelliste = messeArtikel;
            if (!messe.isLandesmesse)
            {
                Artikelliste = messeArtikel.Concat(ArtikellAllgemein).OrderBy(r => r.MesseartikelAllgemein).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ToList();
            }

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Bestellungen");
            ws.View.ShowGridLines = true;
            ws.Cells[1, 1].Value = "Werbemittelbestellungen \"" + messe.Name + "\"";


            //Überschriften Austelleradresse und Lieferadresse
            ws.Cells[3, 5].Value = "Austelleradresse";
            ws.Cells[3, 12].Value = "Abweichende Lieferadresse";


            //Tabellen Überschriften Artikel und Adressangaben
            int i = 21;
            foreach (var artikel in Artikelliste)
            {
                i++;
                float Artikelpreis = artikel.PreisProVE / artikel.Verpackungseinheit;
                ws.Cells[4, i].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache + "\r\n" + Artikelpreis.ToString("#,##0.00") + " €";
                ws.Cells[4, i].AutoFitColumns();
                ws.Cells[4, i].Style.WrapText = true;
            }

             ws.Cells[4, 1].Value = "Nr.";  ws.Cells[4, 2].Value = HeadMesseKostenstelle; ws.Cells[4, 3].Value = "Auftragsnummer"; ws.Cells[4, 4].Value = "Bemerkung"; 
            ws.Cells[4, 5].Value = "NAME1"; ws.Cells[4, 6].Value = "NAME2"; ws.Cells[4, 7].Value = "NAME3"; ws.Cells[4, 8].Value = "Strasse"; ws.Cells[4, 9].Value = "PLZ";
            ws.Cells[4, 10].Value = "Stadt"; ws.Cells[4, 11].Value = "Land"; ws.Cells[4, 12].Value = "NAME1"; ws.Cells[4, 13].Value = "NAME2"; ws.Cells[4, 14].Value = "NAME3";
            ws.Cells[4, 15].Value = "Strasse"; ws.Cells[4, 16].Value = "PLZ"; ws.Cells[4, 17].Value = "Stadt"; ws.Cells[4, 18].Value = "Land";
            ws.Cells[4, 19].Value = "Halle und Stand"; ws.Cells[4, 20].Value = "Bestelldatum"; ws.Cells[4, 21].Value = "Versanddatum";
           

            //Layout-Hauptüberschriften
            ExcelRange mainHeading = ws.Cells[1, 1, 1, i];
            mainHeading.Merge = true;

            mainHeading.Style.Font.Bold = true;
            mainHeading.Style.Font.Size = 13;
            //mainHeading.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            //mainHeading.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
            ExcelRange headingAusteller = ws.Cells[3, 5, 3, 11];
            headingAusteller.Merge = true;
            headingAusteller.Style.Font.Bold = true;
            headingAusteller.Style.Font.Size = 9;
            headingAusteller.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAusteller.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAusteller.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAusteller.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAusteller.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingLiefer = ws.Cells[3, 12, 3, 18];
            headingLiefer.Merge = true;
            headingLiefer.Style.Font.Bold = true;
            headingLiefer.Style.Font.Size = 9;
            headingLiefer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingLiefer.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingLiefer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingLiefer.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingLiefer.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingAngaben = ws.Cells[4, 1, 4, i];
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Font.Size = 9;
            headingAngaben.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAngaben.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAngaben.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAngaben.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



            List<Werbemittelauftrag> auftraege = null;

            if (!messe.isLandesmesse)
            {
                auftraege = messe.auftraege.OrderBy(r => r.Bestelldatum).ToList();
            }
            else
            {
                auftraege = messe.auftraege.Where(r => r.Bestelldatum > Abrechnungsbegin && r.Bestelldatum < Abrechnungsende).OrderBy(r => r.Bestelldatum).ToList();
            }

            i = 4;
            int auftragsnummer = 0;
            int paketeCount = 0;



            if (auftraege != null && auftraege.Count > 0)
            {
                foreach (var auftrag in auftraege)
                {
                    auftragsnummer++;
                    int pc = auftrag.Pakete.Count;
                    Kontakdaten kontakt = auftrag.Auftraggeberadresse;

                    int j = 22;
                    foreach (var artikel in Artikelliste)
                    {
                        int pn = 0;
                        foreach (var Paket in auftrag.Pakete)
                        {
                            pn++;
                            paketeCount++;
                            if (Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID) != null && Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID).menge > 0)
                            {
                                ws.Cells[i + pn, j].Value = Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID).menge;
                                ws.Cells[i + pn, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                                if (pn == 1)
                                {
                                    ws.Cells[i + pn, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                            }
                            else
                            {
                                ws.Cells[i + pn, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                                if (pn == 1)
                                {
                                    ws.Cells[i + pn, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }

                            }
                        }
                        if (pn == 0)
                        {
                            foreach (var artikelmenge in auftrag.Auftragsmengen)
                            {
                                if (artikel.ArtikelID == artikelmenge.artikel.ArtikelID)
                                {
                                    ws.Cells[i + 1, j].Value = artikelmenge.menge;
                                    ws.Cells[i + 1, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    ws.Cells[i + 1, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                                else
                                {
                                    ws.Cells[i + 1, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    ws.Cells[i + 1, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                            }
                        }
                        j++;

                    }
                    int pn2 = 0;
                    foreach (var Paket in auftrag.Pakete)
                    {
                        pn2++;
                        ws.Cells[i + pn2, 21].Value = Paket.Versanddatum.ToString("dd.MM.yyyy");
                        ws.Cells[i + pn2, 21].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                        if (pn2 == 1)
                        {
                            ws.Cells[i + pn2, 21].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    i += pc;
                    if (pc == 0)
                    {
                        i++;
                        ws.Cells[i, 21].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[i, 21].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    string MesseOderKostenstelle = auftrag.messe.Name;
                    if (messe.isLandesmesse)
                    {
                        MesseOderKostenstelle = auftrag.PSPNummer;
                    }

                    List<string> Angaben = new List<string>() { auftragsnummer.ToString(), MesseOderKostenstelle, auftrag.Kennzeichnung, auftrag.Bemerkung, kontakt.Name, kontakt.Name2, kontakt.Name3, kontakt.Strasse, kontakt.PLZ, kontakt.Ort, kontakt.Land.land };
                    //List<string> Angaben_2 = new List<string>() { };
                    if (auftrag.Lieferadresse != null)
                    {
                        Angaben.Add(auftrag.Lieferadresse.Name); Angaben.Add(auftrag.Lieferadresse.Name2); Angaben.Add(auftrag.Lieferadresse.Name3); Angaben.Add(auftrag.Lieferadresse.Strasse);
                        Angaben.Add(auftrag.Lieferadresse.PLZ); Angaben.Add(auftrag.Lieferadresse.Ort); Angaben.Add(auftrag.Lieferadresse.Land.land);
                    }
                    else
                    {
                        for (int r = 0; r < 7; r++)
                        {
                            Angaben.Add(" ");
                        }
                    }
                    Angaben.Add(auftrag.HalleUStand);
                    Angaben.Add(auftrag.Bestelldatum.ToString("dd.MM.yyyy"));
                    if (pc > 1)
                    {

                        for (int m = 1; m <= 20; m++)
                        {
                            ExcelRange auftragsAngaben = ws.Cells[i - pc + 1, m, i, m];
                            auftragsAngaben.Merge = true;
                            auftragsAngaben.Value = Angaben[m - 1];
                        }
                    }
                    else
                    {
                        for (int m = 1; m <= 20; m++)
                        {
                            ws.Cells[i, m].Value = Angaben[m - 1];
                        }
                    }
                    //}
                }
                i++;
                int ac = 21;
                foreach (var artikel in Artikelliste)
                {
                    ac++;
                    ws.Cells[i, ac].Formula = string.Format("SUM({0}:{1})", ExcelCellBase.GetAddress(5, ac), ExcelCellBase.GetAddress(i - 1, ac));
                    ws.Cells[i + 1, ac].Value = artikel.Bestand;
                }
                ws.Cells[i + 1, 21].Value = "Restbestände";

                //Layout- Angaben
                //System.Diagnostics.Debug.WriteLine("Wert für i: " + i);
                //if (i == 5)
                //{

                //}
                ws.Cells[5, 1, i - 1, ac].Style.Font.Size = 9;
                ws.Cells[5, 1, i - 1, ac].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, ac].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, 20].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, 20].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, ac + 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[5, 1, i - 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, 2, i - 1, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                ws.Cells[5, 8, i - 1, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, 18, i - 1, ac].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                //Layout-Letzte Zeile Summe Bestellungen 
                ExcelRange headingBottom = ws.Cells[i, 1, i, ac];
                headingBottom.Style.Font.Bold = true;
                headingBottom.Style.Font.Size = 10;
                headingBottom.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headingBottom.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                headingBottom.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                headingBottom.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                headingBottom.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                headingBottom.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                //Zeile Aktuelle Bestände der Artikel
                ExcelRange artikelBestaende = ws.Cells[i + 1, 21, i + 1, ac];
                artikelBestaende.Style.Font.Bold = true;
                artikelBestaende.Style.Font.Size = 10;
                artikelBestaende.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                artikelBestaende.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                artikelBestaende.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                artikelBestaende.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            else
            {
                ws.Cells[5, 1].Value = "Keine Aufträge vorhanden";
            }

            //Ausgabe als ExcelDatei anstoßen
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Werbemittelübersicht-Messe-Stuttgart_" + messe.Name.Replace(" ", "-") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();
            return RedirectToAction("Messedetails");
        }
       
        public ActionResult ExcelArtikelBestaendeIntern(DateTime Abrechnungsbegin, DateTime Abrechnungsende, int MesseID = 0, bool ListeExtern=false)
        {
            Messe messe = dms.Messen.Find(MesseID);
            string HeadMesseKostenstelle = "Messe";
            if (messe.isLandesmesse)
            {
                HeadMesseKostenstelle = "PSP-Nummer/ \r\n Kostenstelle";
            }

            List<Artikel> messeArtikel = messe.artikel.ToList();
            List<Artikel> ArtikellAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active == true).ToList();
            List<Artikel> Artikelliste = messeArtikel;
            if (!messe.isLandesmesse)
            {
                Artikelliste = messeArtikel.Concat(ArtikellAllgemein).OrderBy(r => r.MesseartikelAllgemein).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ToList();
            }

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Bestellungen");
            ws.View.ShowGridLines = true;
            ws.Cells[1, 1].Value = "Werbemittelbestellungen \"" + messe.Name + "\"";
            

            //Überschriften Austelleradresse und Lieferadresse
            ws.Cells[3, 5].Value = "Austelleradresse";
            ws.Cells[3, 12].Value = "Abweichende Lieferadresse";


            //Tabellen Überschriften Artikel und Adressangaben
            int i = 21;
            foreach (var artikel in Artikelliste)
            {
                float Artikelpreis = artikel.PreisProVE / artikel.Verpackungseinheit;
                i++;
                ws.Cells[4, i].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache +"\r\n" + Artikelpreis.ToString("#,##0.00") + " €";
                //ws.Cells[4, 1 + i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
                ws.Cells[4, i].AutoFitColumns();
                ws.Cells[4, i].Style.WrapText = true;
            }

            ws.Cells[4, 1].Value = "Nr.";  ws.Cells[4, 2].Value = HeadMesseKostenstelle; ws.Cells[4, 3].Value = "Auftragsnummer"; ws.Cells[4, 4].Value = "Bemerkung"; 
            ws.Cells[4, 5].Value = "NAME1"; ws.Cells[4, 6].Value = "NAME2"; ws.Cells[4, 7].Value = "NAME3"; ws.Cells[4, 8].Value = "Strasse"; ws.Cells[4, 9].Value = "PLZ";
            ws.Cells[4, 10].Value = "Stadt"; ws.Cells[4, 11].Value = "Land"; ws.Cells[4, 12].Value = "NAME1"; ws.Cells[4, 13].Value = "NAME2"; ws.Cells[4, 14].Value = "NAME3";
            ws.Cells[4, 15].Value = "Strasse"; ws.Cells[4, 16].Value = "PLZ"; ws.Cells[4, 17].Value = "Stadt"; ws.Cells[4, 18].Value = "Land";
            ws.Cells[4, 19].Value = "Halle und Stand"; ws.Cells[4, 20].Value = "Bestelldatum"; ws.Cells[4, 21].Value = "Versanddatum";
            ws.Cells[4, i + 1].Value = "Gewicht"; ws.Cells[4, i + 2].Value = "Versandart";
            ws.Cells[4, i + 3].Value = "Versandkosten"; ws.Cells[4, i + 4].Value = "Paketnummer"; ws.Cells[4, i + 5].Value = "Erfassungszeit"; ws.Cells[4, i + 6].Value = "Packzeit";

            //Layout--Hauptüberschriften
            ExcelRange mainHeading = ws.Cells[1, 1, 1, i+6];
            mainHeading.Merge = true;
            
            mainHeading.Style.Font.Bold = true;
            mainHeading.Style.Font.Size = 13;
            ExcelRange headingAusteller = ws.Cells[3, 5, 3, 11];
            headingAusteller.Merge = true;
            headingAusteller.Style.Font.Bold = true;
            headingAusteller.Style.Font.Size = 9;
            headingAusteller.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAusteller.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAusteller.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAusteller.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAusteller.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingLiefer = ws.Cells[3, 12, 3, 18];
            headingLiefer.Merge = true;
            headingLiefer.Style.Font.Bold = true;
            headingLiefer.Style.Font.Size = 9;
            headingLiefer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingLiefer.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingLiefer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingLiefer.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingLiefer.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingAngaben = ws.Cells[4, 1, 4, i+6];
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Font.Size = 9;
            headingAngaben.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAngaben.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAngaben.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAngaben.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //ExcelRange toptablehead;
            //tableHead.AutoFitColumns();

            
            List<Werbemittelauftrag> auftraege =null;

            if (!messe.isLandesmesse)
            {
                auftraege = messe.auftraege.OrderBy(r => r.Bestelldatum).ToList();
            }
            else
            {
                auftraege = messe.auftraege.Where(r => r.Bestelldatum > Abrechnungsbegin && r.Bestelldatum < Abrechnungsende).OrderBy(r => r.Bestelldatum).ToList();
            }

            i = 4;
            int auftragsnummer = 0;
            int paketeCount = 0;
            if (auftraege != null && auftraege.Count > 0)
            {
                foreach (var auftrag in auftraege)
                {
                    auftragsnummer++;
                    int pc = auftrag.Pakete.Count;
                    //if (auftrag.Auftraggeberadresse.Name != "Landesmesse Stuttgart GmbH")
                    //{
                    Kontakdaten kontakt = auftrag.Auftraggeberadresse;

                    int j = 22;
                    foreach (var artikel in Artikelliste)
                    {
                        int pn = 0;
                        foreach (var Paket in auftrag.Pakete)
                        {
                            pn++;
                            paketeCount++;
                            if (Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID) != null && Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID).menge > 0)
                            {
                                ws.Cells[i + pn, j].Value = Paket.artikelmenge.SingleOrDefault(r => r.artikel.ArtikelID == artikel.ArtikelID).menge;
                                ws.Cells[i + pn, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                                if (pn == 1)
                                {
                                    ws.Cells[i + pn, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                            }
                            else
                            {
                                ws.Cells[i + pn, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                                if (pn == 1)
                                {
                                    ws.Cells[i + pn, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }

                            }
                        }
                        if (pn == 0)
                        {
                            foreach (var artikelmenge in auftrag.Auftragsmengen)
                            {
                                if (artikel.ArtikelID == artikelmenge.artikel.ArtikelID)
                                {
                                    ws.Cells[i + 1, j].Value = artikelmenge.menge;
                                    ws.Cells[i + 1, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    ws.Cells[i + 1, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                                else
                                {
                                    ws.Cells[i + 1, j].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    ws.Cells[i + 1, j].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                }
                            }
                        }
                        j++;

                    }
                    int pn2 = 0;
                    foreach (var Paket in auftrag.Pakete)
                    {
                        pn2++;

                        ws.Cells[i + pn2, j].Value = Paket.Gewicht;
                        ws.Cells[i + pn2, j + 1].Value = Paket.Versandart;
                        ws.Cells[i + pn2, j + 2].Value = Paket.Preis;
                        ws.Cells[i + pn2, j + 3].Value = Paket.Paketnummer;
                        ws.Cells[i + pn2, j, i + pn2, j + 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Dotted;
                        if (pn2 == 1)
                        {
                            ws.Cells[i + pn2, j, i + pn2, j + 3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    i += pc;
                    if (pc == 0)
                    {
                        i++;
                        ws.Cells[i, 21].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[i, 21].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[i, j, i, j + 3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    string MesseOderKostenstelle = auftrag.messe.Name;
                    if (messe.isLandesmesse)
                    {
                        MesseOderKostenstelle = auftrag.PSPNummer;
                    }

                    List<string> Angaben = new List<string>() { auftragsnummer.ToString(), MesseOderKostenstelle, auftrag.Kennzeichnung, auftrag.Bemerkung, kontakt.Name, kontakt.Name2, kontakt.Name3, kontakt.Strasse, kontakt.PLZ, kontakt.Ort, kontakt.Land.land };
                    //List<string> Angaben_2 = new List<string>() { };
                    if (auftrag.Lieferadresse != null)
                    {
                        Angaben.Add(auftrag.Lieferadresse.Name); Angaben.Add(auftrag.Lieferadresse.Name2); Angaben.Add(auftrag.Lieferadresse.Name3); Angaben.Add(auftrag.Lieferadresse.Strasse);
                        Angaben.Add(auftrag.Lieferadresse.PLZ); Angaben.Add(auftrag.Lieferadresse.Ort); Angaben.Add(auftrag.Lieferadresse.Land.land);
                    }
                    else
                    {
                        for (int r = 0; r < 7; r++)
                        {
                            Angaben.Add("");
                        }
                    }
                    Angaben.Add(auftrag.HalleUStand);
                    Angaben.Add(auftrag.Bestelldatum.ToString("dd.MM.yyyy"));
                    Angaben.Add(auftrag.Versanddatum.ToString("dd.MM.yyyy"));
                    var tsAnlegen = TimeSpan.FromSeconds(auftrag.ZeitAuftragAnlegen);
                    Angaben.Add(string.Format("{0}h {1}min {2}s", tsAnlegen.Hours, tsAnlegen.Minutes, tsAnlegen.Seconds));
                    var tsPacken = TimeSpan.FromSeconds(auftrag.ZeitAuftragPacken);
                    Angaben.Add(string.Format("{0}h {1}min {2}s", tsPacken.Hours, tsPacken.Minutes, tsPacken.Seconds));
                    if (pc > 1)
                    {

                        for (int m = 1; m <= 21; m++)
                        {
                            ExcelRange auftragsAngaben = ws.Cells[i - pc + 1, m, i, m];
                            auftragsAngaben.Merge = true;
                            auftragsAngaben.Value = Angaben[m - 1];
                            
                        }
                        for (int m = 1; m <= 2; m++)
                        {
                            ExcelRange auftragsAngaben = ws.Cells[i - pc + 1, j + 3+m, i, j + 3+m];
                            auftragsAngaben.Merge = true;
                            auftragsAngaben.Value = Angaben[m +21 - 1];
                            auftragsAngaben.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            auftragsAngaben.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    else
                    {
                        for (int m = 1; m <= 21; m++)
                        {
                            ws.Cells[i, m].Value = Angaben[m - 1];
                        }
                        for (int m = 1; m <= 2; m++)
                        {
                            ws.Cells[i, j+m+3].Value = Angaben[m+21 - 1];
                            ws.Cells[i, j + m + 3].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            ws.Cells[i, j+m+3].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    //}
                }
                i++;
                int ac = 21;

                foreach (var artikel in Artikelliste)
                {
                    ac++;
                    ws.Cells[i, ac].Formula = string.Format("SUM({0}:{1})", ExcelCellBase.GetAddress(5, ac), ExcelCellBase.GetAddress(i - 1, ac));
                    ws.Cells[i + 1, ac].Value = artikel.Bestand;
                }



                ws.Cells[i + 1, 20].Value = "Restbestände";
                ws.Cells[i, ac + 1].Formula = string.Format("SUM({0}:{1})", ExcelCellBase.GetAddress(5, ac + 1), ExcelCellBase.GetAddress(i - 1, ac + 1));
                ws.Cells[i, ac + 3].Formula = string.Format("SUM({0}:{1})", ExcelCellBase.GetAddress(5, ac + 3), ExcelCellBase.GetAddress(i - 1, ac + 3));
                var SummeErfassenSec = 0;
                foreach (var Auftrag in auftraege)
                {
                    SummeErfassenSec += Auftrag.ZeitAuftragAnlegen;
                }
                var SummeAnlegen = TimeSpan.FromSeconds(SummeErfassenSec);
                ws.Cells[i, ac + 5].Value = string.Format("{0}h {1}min {2}s", SummeAnlegen.Hours, SummeAnlegen.Minutes, SummeAnlegen.Seconds);
                var SummePackenSec = 0;
                foreach (var Auftrag in auftraege)
                {
                    SummePackenSec += Auftrag.ZeitAuftragPacken;
                }
                var SummePacken = TimeSpan.FromSeconds(SummePackenSec);
                
                ws.Cells[i, ac + 6].Value = string.Format("{0}h {1}min {2}s", SummePacken.Hours, SummePacken.Minutes, SummePacken.Seconds);

                //Layout- Angaben 
                ws.Cells[5, 1, i - 1, ac + 6].Style.Font.Size = 9;
                ws.Cells[5, 1, i - 1, ac + 6].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, ac + 6].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, 21].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i - 1, 21].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                ws.Cells[5, 1, i - 1, ac + 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[5, 1, i - 1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, 2, i - 1, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                ws.Cells[5, 8, i - 1, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, 18, i - 1, ac + 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[5, ac + 3, i, ac + 3].Style.Numberformat.Format = @"#,##0.00 €";
                ws.Cells[5, ac + 1, i, ac + 1].Style.Numberformat.Format = @"#,##0.00";

                //Layout-Letzte Zeile
                ExcelRange headingBottom = ws.Cells[i, 1, i, ac + 6];
                headingBottom.Style.Font.Bold = true;
                headingBottom.Style.Font.Size = 10;
                headingBottom.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headingBottom.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                headingBottom.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                headingBottom.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headingBottom.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                headingBottom.Style.Font.Bold = true;
                headingBottom.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                headingBottom.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //Zeile Aktuelle Bestände der Artikel
                ExcelRange artikelBestaende = ws.Cells[i + 1, 21, i + 1, ac];
                artikelBestaende.Style.Font.Bold = true;
                artikelBestaende.Style.Font.Size = 10;
                artikelBestaende.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                artikelBestaende.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                artikelBestaende.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                artikelBestaende.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            else
            {
                ws.Cells[5, 1].Value = "Keine Aufträge vorhanden";
            }

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Werbemittelübersicht-Intern_" + messe.Name.Replace(" ", "-") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();
            return RedirectToAction("Messedetails");
        }

        public ActionResult RestmengenListe(int MesseID = 0, string AuswahlStatusArtikel = "3")
        {
            Messe messe = dms.Messen.Find(MesseID);
            int ArtikelStatus = int.Parse(AuswahlStatusArtikel);
            string status = "";
            List<Artikel> messeArtikel = messe.artikel.ToList();
            List<Artikel> ArtikellAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active == true).ToList();
            List<Artikel> Artikelliste = messeArtikel;
            if (ArtikelStatus == 1)
            {
                Artikelliste = messe.artikel.Where(r => r.Active == true).ToList();
                status = "aktive";
            }
            if (ArtikelStatus == 2)
            {
                Artikelliste = messe.artikel.Where(r => r.Active == false).ToList();
                status = "inaktive";
            }
            if (!messe.isLandesmesse)
            {
                Artikelliste = messeArtikel.Concat(ArtikellAllgemein).OrderBy(r => r.MesseartikelAllgemein).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ToList();
            }
            ExcelPackage pck = new ExcelPackage();
            var wsRestmengen = pck.Workbook.Worksheets.Add("Restmengenliste");
            wsRestmengen.View.ShowGridLines = true;
            wsRestmengen.Column(1).Width = 40;
            wsRestmengen.Column(2).Width = 28;
            wsRestmengen.Row(1).Height = 30;
           
            
            int i = 2;
            if (Artikelliste.Count > 0)
            {
                foreach (var artikel in Artikelliste)
                {
                    i++;
                    int spalte = 1;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache;
                    //wsRestmengen.Cells[i, 1].AutoFitColumns();
                    wsRestmengen.Cells[i, spalte].Style.WrapText = true;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
                    wsRestmengen.Row(i).Height = 40;

                    spalte++;
                    status = "aktiv";
                    if (!artikel.Active)
                    {
                        status = "inkativ";
                    }
                    wsRestmengen.Cells[i, spalte].Value = status;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    
                    spalte++;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Lagerplatz;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Verpackungseinheit;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    spalte++;
                    int AnzahlAnVE = artikel.Bestand;
                    if(artikel.Verpackungseinheit > 0)
                    {
                        AnzahlAnVE = artikel.Bestand / artikel.Verpackungseinheit;
                    }
                    wsRestmengen.Cells[i, spalte].Value = AnzahlAnVE;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Bestand;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Meldebestand;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Value = artikel.Sicherheitsbestand;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    spalte++;
                    wsRestmengen.Cells[i, spalte].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                    wsRestmengen.Cells[i, spalte].Style.Font.Size = 9;
                    wsRestmengen.Cells[i, spalte].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    wsRestmengen.Cells[i, spalte].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                    wsRestmengen.Cells[i, spalte].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    wsRestmengen.Cells[i, spalte].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
            }
            else
            {
                wsRestmengen.Cells[3, 1].Value = "Keine Artikel vorhanden";
            }

            wsRestmengen.Cells[1, 1, 1, 10].Merge = true;
            wsRestmengen.Cells[1, 1, 1, 10].Value = messe.Name + " Restmengenliste " + status + " Artikel (" + DateTime.Now.ToString("dd.MM.yyy") + ")";
            wsRestmengen.Cells[1, 1, 1, 10].Style.Font.Size = 14;
            wsRestmengen.Cells[1, 1, 1, 10].Style.Font.Bold = true;
            wsRestmengen.Cells[1, 1, 1, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            wsRestmengen.Cells[1, 1, 1, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            wsRestmengen.Cells[1, 1, 1, 10].Style.Font.Color.SetColor(System.Drawing.Color.White);
            wsRestmengen.Cells[1, 1, 1, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            wsRestmengen.Cells[1, 1, 1, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            wsRestmengen.Cells[2, 1].Value = "Artikel";
            wsRestmengen.Cells[2, 2].Value = "Status";
            wsRestmengen.Cells[2, 3].Value = "Lagerplatz";
            wsRestmengen.Cells[2, 4].Value = "Inhalt pro Verpackungseinheit";
            wsRestmengen.Cells[2, 5].Value = "Anzahl Verpackungseinheiten";
            wsRestmengen.Cells[2, 6].Value = "Bestand";
            wsRestmengen.Cells[2, 7].Value = "Meldebestand";
            wsRestmengen.Cells[2, 8].Value = "Sicherheitsbestand";
            wsRestmengen.Cells[2, 9].Value = "Zentrallager";
            wsRestmengen.Cells[2, 10].Value = "Entsorgen";

            wsRestmengen.Cells[2, 1, 2, 10].Style.Font.Bold = true;
            wsRestmengen.Cells[2, 1, 2, 10].Style.Font.Size = 13;
            wsRestmengen.Cells[2, 1, 2, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            wsRestmengen.Cells[2, 1, 2, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            wsRestmengen.Cells[2, 1, 2, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            wsRestmengen.Cells[2, 1, 2, 10].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            wsRestmengen.Cells[2, 1, 2, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            wsRestmengen.Cells[2, 1, 2, 10].Style.Font.Color.SetColor(System.Drawing.Color.White);
            wsRestmengen.Cells[2, 1, 2, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            wsRestmengen.Cells[2, 1, 2, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Restmengeliste_" + messe.Name.Replace(" ", "-") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();

            return RedirectToAction("Messedetails");
        }

        public ActionResult Artikelbewegungen(DateTime Abrechnungsbegin, DateTime Abrechnungsende, int MesseID = 0)
        {
            Messe messe = dms.Messen.Find(MesseID);

            List<Artikel> messeArtikel = messe.artikel.ToList();
            List<Artikel> ArtikellAllgemein = dms.Artikell.Where(r => r.MesseartikelAllgemein == true && r.Active == true).ToList();
            List<Artikel> Artikelliste = messeArtikel;
            if (!messe.isLandesmesse)
            {
                Artikelliste = messeArtikel.Concat(ArtikellAllgemein).OrderBy(r => r.MesseartikelAllgemein).ThenBy(r => r.artikelart.Art).ThenBy(r => r.Name).ToList();
            }
            

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Restmengenliste");
            ws.View.ShowGridLines = true;
            ws.Column(1).Width = 30;
            ws.Row(1).Height = 30;

            //Layout Überschrift
            ws.Cells[1, 1, 1, 3].Merge = true;
            ws.Cells[1, 1, 1, 3].Value = messe.Name + " - Artikelbewegungen (" + DateTime.Now.ToString("dd.MM.yyy") + ")";
            ws.Cells[1, 1, 1, 3].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, 3].Style.Font.Size = 13;
           
            ws.Cells[1, 1, 1, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            ws.Cells[1, 1, 1, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            int i = 1;
            if (Artikelliste.Count > 0)
            {
                foreach (var artikel in Artikelliste)
                {
                    List<Bestandsaenderung> BAES = null;
                    if (messe.isLandesmesse)
                    {
                        BAES = artikel.BAE.Where(r => r.Datum > Abrechnungsbegin && r.Datum < Abrechnungsende).ToList();
                    }
                    else
                    {
                        BAES = artikel.BAE.ToList();
                    }

                    BAES = artikel.BAE.Where(r => r.Datum > Abrechnungsbegin && r.Datum < Abrechnungsende).ToList();


                    i += 2;
                    if (BAES.Count < 1)
                    {
                        ws.Cells[i, 1].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache;
                        ws.Cells[i, 1].Style.WrapText = true;
                        ws.Cells[i, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells[i, 1].Style.Font.Size = 9;
                        ws.Cells[i, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells[i, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[i, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
                        //ws.Cells[i, 1, i, 3].Style.Font.Color.SetColor(System.Drawing.Color.White);
                        ws.Row(i).Height = 36;

                        ws.Cells[i, 2].Value = 0;
                        ws.Cells[i, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                        ws.Cells[i, 2].Style.Font.Size = 9;
                        ws.Cells[i, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i, 21].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells[i, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[i, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);

                    }
                    else
                    {
                        int dateCount = 2;
                        int day = 0;
                        int year = 0;
                        int j = 0;
                        int jmax = 0;
                        foreach (var Bae in BAES)
                        {
                            if (!Bae.Artikel.MesseartikelAllgemein)
                            {
                                if (day == 0)
                                {
                                    day = Bae.Datum.DayOfYear;
                                    year = Bae.Datum.Year;
                                    ws.Cells[i + j, dateCount].Value = Bae.Datum.ToString("dd.MM.yyy");
                                    ws.Cells[i + j, dateCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                                    ws.Cells[i + j, dateCount].AutoFitColumns();
                                    ws.Cells[i + j, dateCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    ws.Cells[i + j, dateCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
                                    ws.Cells[i + j, dateCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    ws.Cells[i + j, dateCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    ws.Row(i + j).Height = 20;

                                }

                                if (day == Bae.Datum.DayOfYear && Bae.Datum.Year == year)
                                {
                                    j++;
                                    ws.Cells[i + j, dateCount].Value = Bae.Menge + " " + Bae.Grund + "\r\n - \"" + Bae.Bemerkung + "\"";
                                    ws.Row(i + j).Height = 20;
                                    ws.Cells[i + j, dateCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                                    ws.Cells[i + j, dateCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    ws.Cells[i + j, dateCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                                    ws.Cells[i + j, dateCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    ws.Cells[i + j, dateCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }
                                else
                                {
                                    j = 0;
                                    dateCount++;
                                    ws.Cells[i + j, dateCount].Value = Bae.Datum.ToString("dd.MM.yyy");
                                    ws.Cells[i + j, dateCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                                    ws.Cells[i + j, dateCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    ws.Cells[i + j, dateCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
                                    ws.Cells[i + j, dateCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    ws.Cells[i + j, dateCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    j++;
                                    day = Bae.Datum.DayOfYear;
                                    year = Bae.Datum.Year;
                                    ws.Cells[i + j, dateCount].Value = Bae.Menge + " " + Bae.Grund + "\r\n - \"" + Bae.Bemerkung +"\"";
                                    ws.Cells[i + j, dateCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
                                    ws.Cells[i + j, dateCount].AutoFitColumns();
                                    ws.Cells[i + j, dateCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    ws.Cells[i + j, dateCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);
                                    ws.Cells[i + j, dateCount].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    ws.Cells[i + j, dateCount].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }
                                if (j > jmax)
                                {
                                    jmax = j;
                                }
                            }
                        }

                        ws.Cells[i, 1, i + jmax, 1].Merge = true;
                        ws.Cells[i, 1, i + jmax, 1].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache;
                        ws.Cells[i, 1, i + jmax, 1].Style.WrapText = true;
                        ws.Cells[i, 1, i + jmax, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
                        ws.Cells[i, 1, i + jmax, 1].Style.Font.Size = 9;
                        ws.Cells[i, 1, i + jmax, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i, 1, i + jmax, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells[i, 1, i + jmax, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[i, 1, i + jmax, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
                        ws.Cells[i, 1, i + jmax, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                        ws.Cells[i, 2, i + jmax, dateCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
                        i += jmax;
                    }
                }
            }
            else
            {
                ws.Cells[3,1].Value = "Keine Artikel vorhanden";
            }
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Artikelbewegungen_"+messe.Name.Replace(" ","-")+"_"+DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();
            return RedirectToAction("Messedetails");
        }

        public ActionResult ExcelAbbrechnung(int MesseID = 0)
        {
            Messe messe = dms.Messen.Find(MesseID);

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Artikelbewegungen");
            ws.View.ShowGridLines = true;
            ws.Cells[1, 1].Value = "Werbemittelbestellungen " + messe.Name;
            ws.Cells[3, 2].Value = "Werbemittel";


            //Überschrift
            ExcelRange heading = ws.Cells[1, 1, 1, 5];
            heading.Merge = true;
            heading.Style.Font.Bold = true;
            heading.Style.Font.Size = 15;
            ws.Row(1).Height = 30;

            //Bestellungen 
            ws.Cells[3, 1].Value = "Bestellungen";
            ws.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            ws.Cells[3, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            ws.Cells[3, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            ws.Cells[3, 2].Value = messe.auftraege.Count();
            ws.Cells[3, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange bestell = ws.Cells[3, 1, 3, 2];
            bestell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            bestell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            bestell.Style.Font.Bold = true;
            bestell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            ws.Row(3).Height = 20;

            //Druck Head
            ws.Cells[5, 1].Value = "Druck"; ws.Cells[5, 2].Value = "Anzahl"; ws.Cells[5, 3].Value = "Stückzahl";
            ExcelRange topDruck = ws.Cells[5, 1, 5, 3];
            topDruck.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            topDruck.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            topDruck.Style.Font.Color.SetColor(System.Drawing.Color.White);
            topDruck.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            topDruck.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            topDruck.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            topDruck.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            topDruck.Style.Font.Bold = true;
            topDruck.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            topDruck.AutoFitColumns();

            //Druck values first column
            ExcelRange DruckvaluesLeft = ws.Cells[6, 1, 13, 1];
            DruckvaluesLeft.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            DruckvaluesLeft.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
            DruckvaluesLeft.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            DruckvaluesLeft.AutoFitColumns();
            //Druck values
            ExcelRange Druckvalues = ws.Cells[6, 2, 13, 3];
            Druckvalues.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            Druckvalues.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            Druckvalues.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            Druckvalues.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            Druckvalues.AutoFitColumns();
            ExcelRange FirstColumnDruck = ws.Cells[6, 1, 13, 1];
            FirstColumnDruck.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            FirstColumnDruck.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            FirstColumnDruck.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            FirstColumnDruck.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            FirstColumnDruck.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);

            ExcelRange BottomColumnDruck = ws.Cells[13, 1];
            BottomColumnDruck.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

            int BMKLmsOrder = 0; int BMKLmsAnzahl = 0; int BKMWerbemittelOrder = 0; int BKMWerbemittelAnzahl = 0; int BKMIndiOrder = 0; int BKMIndiAnzahl = 0;
            int BKMIndiOrderFree = 0; int BKMIndiAnzahlFree = 0; int CouponAnOrder = 0; int CouponAnAnzahl = 0; int CouponAnEinOrder = 0; int CouponAnEinAnzahl = 0;
            int CouponDigiOrder = 0; int CouponDigiAnzahl = 0; int CouponDigiEinOrder = 0; int CouponDigiEinAnzahl = 0;
            int BKM500 = 0; int BKM1000 = 0; int BKM2000 = 0; int BKM3000 = 0; int BKM5000 = 0; int BKM7000 = 0; int BKM10000 = 0;
            int BKMFree500 = 0; int BKMFree1000 = 0; int BKMFree2000 = 0; int BKMFree3000 = 0; int BKMFree5000 = 0; int BKMFree7000 = 0; int BKMFree10000 = 0;
            foreach (var item in messe.auftraege)
            {
                foreach (var artikel in item.Auftragsmengen)
                {
                    if (artikel.artikel.artikelart.ArtikelartID == 2 && artikel.auftrag.kunde.Name == "Landesmesse Stuttgart GmbH")
                    {
                        BMKLmsOrder++;
                        BMKLmsAnzahl += artikel.gelieferteMenge;
                    }
                    if (artikel.artikel.artikelart.ArtikelartID == 2  && artikel.auftrag.kunde.Name != "Landesmesse Stuttgart GmbH")
                    {
                        BKMWerbemittelOrder++;
                        BKMWerbemittelAnzahl += artikel.gelieferteMenge;
                    }
                    if ((artikel.artikel.artikelart.ArtikelartID == 3 ))
                    {
                        BKMIndiOrder++;
                        BKMIndiAnzahl += artikel.gelieferteMenge;
                        if (artikel.gelieferteMenge <= 500)
                        {
                            BKM500++;
                        }
                        if (artikel.gelieferteMenge > 500 && artikel.gelieferteMenge <= 1000 )
                        {
                            BKM1000++;
                        }
                        if (artikel.gelieferteMenge > 1000 && artikel.gelieferteMenge <= 2000)
                        {
                            BKM2000++;
                        }
                        if (artikel.gelieferteMenge > 2000 && artikel.gelieferteMenge <= 3000)
                        {
                            BKM3000++;
                        }
                        if (artikel.gelieferteMenge > 3000 && artikel.gelieferteMenge <= 5000)
                        {
                            BKM5000++;
                        }
                        if (artikel.gelieferteMenge > 5000 && artikel.gelieferteMenge <= 7000)
                        {
                            BKM7000++;
                        }
                        if (artikel.gelieferteMenge > 7000 && artikel.gelieferteMenge <= 10000)
                        {
                            BKM10000++;
                        }
                    }
                    if ((artikel.artikel.artikelart.ArtikelartID == 43))
                    {
                        BKMIndiOrderFree++;
                        BKMIndiAnzahlFree += artikel.gelieferteMenge;
                        if (artikel.gelieferteMenge <= 500)
                        {
                            BKMFree500++;
                        }
                        if (artikel.gelieferteMenge > 500 && artikel.gelieferteMenge <= 1000)
                        {
                            BKMFree1000++;
                        }
                        if (artikel.gelieferteMenge > 1000 && artikel.gelieferteMenge <= 2000)
                        {
                            BKMFree2000++;
                        }
                        if (artikel.gelieferteMenge > 2000 && artikel.gelieferteMenge <= 3000)
                        {
                            BKMFree3000++;
                        }
                        if (artikel.gelieferteMenge > 3000 && artikel.gelieferteMenge <= 5000)
                        {
                            BKMFree5000++;
                        }
                        if (artikel.gelieferteMenge > 5000 && artikel.gelieferteMenge <= 7000)
                        {
                            BKMFree7000++;
                        }
                        if (artikel.gelieferteMenge > 7000 && artikel.gelieferteMenge <= 10000)
                        {
                            BKMFree10000++;
                        }
                    }
                    if (artikel.artikel.artikelart.ArtikelartID == 4)
                    {
                        CouponAnOrder++;
                        CouponAnAnzahl += artikel.gelieferteMenge;
                    }
                    if (artikel.artikel.artikelart.ArtikelartID == 5)
                    {
                        CouponAnEinOrder++;
                        CouponAnEinAnzahl += artikel.gelieferteMenge;
                    }
                    if (artikel.artikel.artikelart.ArtikelartID == 6)
                    {
                        CouponDigiOrder++;
                        CouponDigiAnzahl += artikel.gelieferteMenge;
                    }
                    if (artikel.artikel.artikelart.ArtikelartID == 7)
                    {
                        CouponDigiEinOrder++;
                        CouponDigiEinAnzahl += artikel.gelieferteMenge;
                    }
                    
                }
            }

            ws.Cells[6, 1].Value = "BKM für LMS"; ws.Cells[6, 2].Value = BMKLmsOrder; ws.Cells[6, 3].Value = BMKLmsAnzahl;
            ws.Cells[7, 1].Value = "BKM Werbemittel"; ws.Cells[7, 2].Value = BKMWerbemittelOrder; ws.Cells[7, 3].Value = BKMWerbemittelAnzahl; 
            ws.Cells[8, 1].Value = "BKM individualisiert (kostenpflichtig)"; ws.Cells[8, 2].Value = BKMIndiOrder; ws.Cells[8, 3].Value = BKMIndiAnzahl;
            ws.Cells[9, 1].Value = "BKM individualisiert (kostenfrei)"; ws.Cells[9, 2].Value = BKMIndiOrderFree; ws.Cells[9, 3].Value = BKMIndiAnzahlFree;
            ws.Cells[10, 1].Value = "Vorteilscoupon (angeliefert)"; ws.Cells[10, 2].Value = CouponAnOrder; ws.Cells[10, 3].Value = CouponAnAnzahl;
            ws.Cells[11, 1].Value = "Vorteilscoupon (angeliefert mit Eindruck)"; ws.Cells[11, 2].Value = CouponAnEinOrder; ws.Cells[11, 3].Value = CouponAnEinAnzahl; 
            ws.Cells[12, 1].Value = "Vorteilscoupon (Digitaldruck)"; ws.Cells[12, 2].Value = CouponDigiOrder; ws.Cells[12, 3].Value = CouponDigiAnzahl;
            ws.Cells[13, 1].Value = "Vorteilscoupon (Digitaldruck mit Eindruck)"; ws.Cells[13, 2].Value = CouponDigiEinOrder; ws.Cells[13, 3].Value = CouponDigiEinAnzahl; 

            //Versand
            ws.Cells[15, 1].Value = "Versand"; ws.Cells[15, 2].Value = "Anzahl"; ws.Cells[15, 3].Value = "EK"; ws.Cells[15, 4].Value = "Faktor"; ws.Cells[15, 5].Value = "VK";
            ExcelRange topVersand = ws.Cells[15, 1, 15, 5];
            topVersand.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            topVersand.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            topVersand.Style.Font.Color.SetColor(System.Drawing.Color.White);
            topVersand.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            topVersand.Style.Font.Bold = true;
            topVersand.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            topVersand.AutoFitColumns();

            int AnzahlBriefe = 0; float KostenBriefe = 0;
            int AnzahlBriefeInt = 0; float KostenBriefeInt = 0;
            int AnzahlDHL = 0; float KostenDHL = 0;
            int AnzahlDHLEX = 0; float KostenDHLEX = 0;
            int AnzahlDHLInt = 0; float KostenDHLInt = 0;
            int AnzahlDHLIntEx = 0; float KostenDHLIntEx = 0;
            int AnzahlExpressCar = 0; float KostenExpressCar = 0;
            int AnzahlKurier = 0; float KostenKurier = 0;
            int AnzahlAbholung = 0; float KostenAbholung = 0;
            int AnzahlGesamt = 0; float KostenGesamt = 0;

            foreach (var auftrag in messe.auftraege)
            {
                foreach (var paket in auftrag.Pakete)
                {
                    if (paket.Versandart == "Brief")
                    {
                        AnzahlBriefe += 1;
                        KostenBriefe += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "Brief International")
                    {
                        AnzahlBriefeInt += 1;
                        KostenBriefeInt += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "DHL" && paket.artikelmenge.Where(r => r.artikel.artikelart.ArtikelartID == 3 || r.artikel.artikelart.ArtikelartID == 37).Count() == 0)
                    {
                        AnzahlDHL += 1;
                        KostenDHL += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                        System.Diagnostics.Debug.WriteLine("Werbemittelauftrag: " + paket.auftrag.Auftraggeberadresse.Name);
                        
                    }
                    if (paket.Versandart == "DHL Express" && paket.artikelmenge.Where(r => r.artikel.artikelart.ArtikelartID == 3 || r.artikel.artikelart.ArtikelartID == 37).Count() == 0)
                    {
                        AnzahlDHLEX += 1;
                        KostenDHLEX += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "DHL International" && paket.artikelmenge.Where(r => r.artikel.artikelart.ArtikelartID == 3 || r.artikel.artikelart.ArtikelartID == 37).Count() == 0)
                    {
                        AnzahlDHLInt += 1;
                        KostenDHLInt += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "DHL Internat. Express" && paket.artikelmenge.Where(r => r.artikel.artikelart.ArtikelartID == 3 || r.artikel.artikelart.ArtikelartID == 37).Count() == 0)
                    {
                        AnzahlDHLIntEx += 1;
                        KostenDHLIntEx += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "Messecar")
                    {
                        AnzahlExpressCar += 1;
                        KostenExpressCar += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "Kurier")
                    {
                        AnzahlKurier += 1;
                        KostenKurier += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                    if (paket.Versandart == "Abholung")
                    {
                        AnzahlAbholung += 1;
                        KostenAbholung += paket.Preis;
                        AnzahlGesamt += 1;
                        KostenGesamt += paket.Preis;
                    }
                }
            }

            //values Versand
            ws.Cells[16, 1].Value = "Briefe"; ws.Cells[16, 2].Value = AnzahlBriefe; ws.Cells[16, 3].Value = KostenBriefe; ws.Cells[16, 4].Value = "x 1,15"; ws.Cells[16, 5].Value = KostenBriefe * 1.15;
            ws.Cells[17, 1].Value = "Briefe Ausland"; ws.Cells[17, 2].Value = AnzahlBriefeInt; ws.Cells[17, 3].Value = KostenBriefeInt; ws.Cells[17, 4].Value = "x 1,15"; ws.Cells[17, 5].Value = KostenBriefeInt * 1.15;
            ws.Cells[18, 1].Value = "Paket Innland"; ws.Cells[18, 2].Value = AnzahlDHL; ws.Cells[18, 3].Value = KostenDHL; ws.Cells[18, 4].Value = messe.AbrechungsPreisStandardpaket; ws.Cells[18, 5].Value = AnzahlDHL * 4.5F;
            ws.Cells[19, 1].Value = "Paket Innland Express"; ws.Cells[19, 2].Value = AnzahlDHLEX; ws.Cells[19, 3].Value = KostenDHLEX; ws.Cells[19, 4].Value = "x 1,15"; ws.Cells[19, 5].Value = KostenDHLEX * 1.15;
            ws.Cells[20, 1].Value = "Paket Ausland"; ws.Cells[20, 2].Value = AnzahlDHLInt; ws.Cells[20, 3].Value = KostenDHLInt; ws.Cells[20, 4].Value = "x 1,15"; ws.Cells[20, 5].Value = KostenDHLInt * 1.15;
            ws.Cells[21, 1].Value = "Paket Ausland Express"; ws.Cells[21, 2].Value = AnzahlDHLIntEx; ws.Cells[21, 3].Value = KostenDHLIntEx; ws.Cells[21, 4].Value = "x 1,15"; ws.Cells[21, 5].Value = KostenDHLIntEx * 1.15;
            ws.Cells[22, 1].Value = "ExpressCar"; ws.Cells[22, 2].Value = AnzahlExpressCar; ws.Cells[22, 3].Value = KostenExpressCar; ws.Cells[22, 4].Value = ""; ws.Cells[22, 5].Value = KostenExpressCar * 1.15;
            ws.Cells[23, 1].Value = "Kurier"; ws.Cells[23, 2].Value = AnzahlKurier; ws.Cells[23, 3].Value = KostenKurier; ws.Cells[23, 4].Value = "x 1,15";  ws.Cells[23, 5].Value = KostenKurier * 1.15;
            ws.Cells[24, 1].Value = "Abholung"; ws.Cells[24, 2].Value = AnzahlAbholung; ws.Cells[24, 3].Value = KostenAbholung; ws.Cells[24, 4].Value = ""; ws.Cells[24, 5].Value = KostenAbholung * 1.15;

            ExcelRange firstColumnVersand = ws.Cells[15, 1, 15, 5];
            firstColumnVersand.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            firstColumnVersand.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            firstColumnVersand.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            
            ExcelRange valueVersand = ws.Cells[16, 1, 24, 5];
            valueVersand.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            valueVersand.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            valueVersand.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            valueVersand.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            valueVersand.AutoFitColumns();
            ExcelRange valueVersandNumbers = ws.Cells[16, 3, 25, 3];
            valueVersandNumbers.Style.Numberformat.Format = @"#,##0.00 €";
            valueVersandNumbers.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            valueVersandNumbers.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            ExcelRange valueVersandNumbers2 = ws.Cells[16, 5, 25, 5];
            valueVersandNumbers2.Style.Numberformat.Format = @"#,##0.00 €";
            valueVersandNumbers2.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            valueVersandNumbers2.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            ExcelRange AbrechnungsPreisPaket = ws.Cells[18, 4];
            AbrechnungsPreisPaket.Style.Numberformat.Format = @"#,##0.00 €";
            AbrechnungsPreisPaket.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            AbrechnungsPreisPaket.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            ExcelRange FirstColumnVersand = ws.Cells[16, 1, 24, 1];
            FirstColumnVersand.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            FirstColumnVersand.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            FirstColumnVersand.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            FirstColumnVersand.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            FirstColumnVersand.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);

            ExcelRange summeVK = ws.Cells[25, 5];
            ws.Cells[25, 1].Value = "Insgesamt"; ws.Cells[25, 2].Value = AnzahlGesamt; ws.Cells[25, 3].Value = KostenGesamt; ws.Cells[25, 4].Value = ""; summeVK.Formula = "=sum(E16:E24)";
            ExcelRange gesamtVersand = ws.Cells[25, 1, 25, 5];
            gesamtVersand.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            gesamtVersand.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            gesamtVersand.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            gesamtVersand.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            gesamtVersand.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            gesamtVersand.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            gesamtVersand.Style.Font.Color.SetColor(System.Drawing.Color.White);
            gesamtVersand.Style.Font.Bold = true;


            //Auflistung nach Bestellmenge für individualisierte Briefklebememarken
            ws.Cells[27, 1].Value = "BKM Indi kostenpflichtig nach Bestellmengen";
            ws.Cells[28, 1].Value = "500"; ws.Cells[28, 2].Value = BKM500;
            ws.Cells[29, 1].Value = "1000"; ws.Cells[29, 2].Value = BKM1000;
            ws.Cells[30, 1].Value = "2000"; ws.Cells[30, 2].Value = BKM2000;
            ws.Cells[31, 1].Value = "3000"; ws.Cells[31, 2].Value = BKM3000;
            ws.Cells[32, 1].Value = "5000"; ws.Cells[32, 2].Value = BKM5000;
            ws.Cells[33, 1].Value = "7000"; ws.Cells[33, 2].Value = BKM7000;
            ws.Cells[34, 1].Value = "10000"; ws.Cells[34, 2].Value = BKM10000;

            ws.Cells[37, 1].Value = "BKM Indi kostenlos nach Bestellmengen";
            ws.Cells[38, 1].Value = "500"; ws.Cells[38, 2].Value = BKMFree500;
            ws.Cells[39, 1].Value = "1000"; ws.Cells[39, 2].Value = BKMFree1000;
            ws.Cells[40, 1].Value = "2000"; ws.Cells[40, 2].Value = BKMFree2000;
            ws.Cells[41, 1].Value = "3000"; ws.Cells[41, 2].Value = BKMFree3000;
            ws.Cells[42, 1].Value = "5000"; ws.Cells[42, 2].Value = BKMFree5000;
            ws.Cells[43, 1].Value = "7000"; ws.Cells[43, 2].Value = BKMFree7000;
            ws.Cells[44, 1].Value = "10000"; ws.Cells[44, 2].Value = BKMFree10000;

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Abrechnung_" + messe.Name.Replace(" ", "-") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();

            return RedirectToAction("Messedetails");
        }

        public ActionResult AbrechnungErstellt(int MesseID = 0)
        {
            dms.Messen.Find(MesseID).abgegrechnet = true;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult ArtikellisteVerschickt(int MesseID = 0)
        {
            dms.Messen.Find(MesseID).RestmengenlisteVerschickt = true;
            dms.SaveChanges();
            return Json(true);
        }

        public ActionResult MesseLoeschen(int MesseID = 0)
        {
            System.Diagnostics.Debug.WriteLine("MesseId: " + MesseID);
            if (MesseID != 0)
            {
                Messe messe = dms.Messen.Single(r => r.MesseID == MesseID);
                foreach (var artikel in messe.artikel)
                {
                    var BAEs = dms.BAenderungen.Where(r => r.ArtikelID == artikel.ArtikelID);
                    foreach (var BAE in BAEs)
                    {
                        System.Diagnostics.Debug.WriteLine("BAEArt:" + BAE.BestandsaenderungID);
                    }

                    foreach (var BAE in BAEs)
                    {
                        dms.BAenderungen.Remove(BAE);
                    }
                    dms.SaveChanges();
                }

                foreach (var artikel in messe.artikel)
                {
                    foreach (var auftrag in messe.auftraege)
                    {
                        var BAEs = dms.BAenderungen.Where(r => r.WMA.WerbemittelauftragID == auftrag.WerbemittelauftragID);
                        foreach (var BAE in BAEs)
                        {
                            System.Diagnostics.Debug.WriteLine("BAEArt:" + BAE.BestandsaenderungID);
                        }

                        foreach (var BAE in BAEs)
                        {
                            dms.BAenderungen.Remove(BAE);
                        }
                        dms.SaveChanges();
                        foreach (var BAE in auftrag.BAE)
                        {
                            System.Diagnostics.Debug.WriteLine("BAEAuf:" + BAE.BestandsaenderungID);
                        }

                        var Amengen = dms.Auftragsmengen.Where(r => r.auftrag.WerbemittelauftragID == auftrag.WerbemittelauftragID);
                        foreach (var Amenge in Amengen)
                        {
                            dms.Auftragsmengen.Remove(Amenge);
                        }
                        dms.SaveChanges();
                        foreach (var Amenge in Amengen)
                        {
                            System.Diagnostics.Debug.WriteLine("Amenge:" + Amenge.AuftragsmengeID);
                        }

                        foreach (var Paket in auftrag.Pakete)
                        {
                            var AmengenPaket = dms.Auftragsmengen.Where(r => r.paket.PaketID == Paket.PaketID);
                            foreach (var Amenge in AmengenPaket)
                            {
                                dms.Auftragsmengen.Remove(Amenge);
                            }
                            dms.SaveChanges();
                            //dms.Pakete.Remove(Paket);
                        }

                        var Pakete = dms.Pakete.Where(r => r.auftrag.WerbemittelauftragID == auftrag.WerbemittelauftragID);
                        foreach (var Paket in Pakete)
                        {
                            dms.Pakete.Remove(Paket);
                        }
                        dms.SaveChanges();
                        foreach (var Paket in Pakete)
                        {
                            System.Diagnostics.Debug.WriteLine("Paket:" + Paket.PaketID);
                        }

                        if (auftrag.Rechnungsadresse != null)
                        {

                            Kontakdaten kontakt = dms.Kontaktdatenn.Find(auftrag.Rechnungsadresse.KontakdatenID);
                            dms.Kontaktdatenn.Remove(kontakt);
                        }
                        if (auftrag.Lieferadresse != null)
                        {
                            Kontakdaten kontakt = dms.Kontaktdatenn.Find(auftrag.Lieferadresse.KontakdatenID);
                            dms.Kontaktdatenn.Remove(kontakt);
                        }
                        if (auftrag.Austelleradresse != null)
                        {
                            Kontakdaten kontakt = dms.Kontaktdatenn.Find(auftrag.Austelleradresse.KontakdatenID);
                            dms.Kontaktdatenn.Remove(kontakt);
                        }
                        if (auftrag.Auftraggeberadresse != null)
                        {
                            Kontakdaten kontakt = dms.Kontaktdatenn.Find(auftrag.Auftraggeberadresse.KontakdatenID);
                            dms.Kontaktdatenn.Remove(kontakt);
                        }
                        dms.SaveChanges();
                    }
                }

                var Auftraege = dms.Werbemittelauftraege.Where(r => r.messe.MesseID == messe.MesseID);
                foreach (var afutrag in Auftraege)
                {
                    System.Diagnostics.Debug.WriteLine("Auftrag:" + afutrag.WerbemittelauftragID);
                    dms.Werbemittelauftraege.Remove(afutrag);
                }

                var Artikel = dms.Artikell.Where(r => r.Messe.MesseID == messe.MesseID);
                foreach (var artikels in Artikel)
                {
                    System.Diagnostics.Debug.WriteLine("Artikel:" + artikels.Name);
                }
                foreach (var artikel in Artikel)
                {
                    dms.Artikell.Remove(artikel);
                }
                dms.SaveChanges();

                var kontakte = dms.Kontaktdatenn.Where(r => r.messe.MesseID == messe.MesseID);
                foreach (var kon in kontakte)
                {
                    System.Diagnostics.Debug.WriteLine("kontakt" + kon.Name);
                }
                foreach (var kon in kontakte)
                {
                    dms.Kontaktdatenn.Remove(kon);
                }
                dms.Messen.Remove(messe);
                dms.SaveChanges();
            }
            if (Session["SuchtextMesse"] != null)
            {
                string ersterTerm = Session["SuchtextMesse"].ToString();
                List<Messe> messen = dms.Messen.Where(r => r.Name.ToLower().Contains(ersterTerm.ToLower())).OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();

                return View("Index", messen);
            }
            else
            {
                ICollection<Messe> messen = dms.Messen.OrderByDescending(r => r.Active).ThenBy(r => r.Startdatum).Take(1000).ToList();
                ICollection<Messe> messenAktuell = new List<Messe>();
                return View("Index", messen);
            }
        }

        public ActionResult Ruecklaeufer()
        {
            ICollection<Paket> ruecklaeufer = dms.Pakete.Where(r => r.Ruecklaeufer && !r.RuecklaeuferAbgerechnet).OrderBy(r => r.Ruecksendedatum).ToList();
            return View(ruecklaeufer);
        }

        public ActionResult Ruecklaeufersuche(FormCollection fc)
        {
            ICollection<Paket> Pakete = new List<Paket>();
            string suchtext = fc["FilterEins"];
            int Filter = 1;
            Filter = int.Parse(fc["SuchFilterRadio"]);
            if (String.IsNullOrEmpty(suchtext))
            {
                if (Filter == 1)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer && !r.RuecklaeuferAbgerechnet).OrderByDescending(r => r.Ruecksendedatum).ToList();
                }
                if (Filter == 2)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer && r.RuecklaeuferAbgerechnet).OrderByDescending(r => r.Ruecksendedatum).Take(250).ToList();
                }
                if (Filter == 3)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer ).OrderByDescending(r => r.Ruecksendedatum).Take(250).ToList();
                }
            }
            else
            {
                if (Filter == 1)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer && !r.RuecklaeuferAbgerechnet && r.auftrag.messe.Name.ToLower().Contains(suchtext.ToLower())).OrderByDescending(r => r.Ruecksendedatum).ToList();
                }
                if (Filter == 2)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer && r.RuecklaeuferAbgerechnet && r.auftrag.messe.Name.ToLower().Contains(suchtext.ToLower())).OrderByDescending(r => r.Ruecksendedatum).Take(250).ToList();
                }
                if (Filter == 3)
                {
                    Pakete = dms.Pakete.Where(r => r.Ruecklaeufer && r.auftrag.messe.Name.ToLower().Contains(suchtext.ToLower())).OrderByDescending(r => r.Ruecksendedatum).Take(250).ToList();
                }
            }


            
            return PartialView("Ruecklaeuferliste", Pakete);
        }

        public ActionResult setRueckActivity(int PaketID, bool active)
        {
            //Abrechnungsdatum einfügen 
            Paket paket = dms.Pakete.Find(PaketID);
            paket.RuecklaeuferAbgerechnet = active;
            dms.SaveChanges();
            return Json("true");
        }

        public ActionResult CloseAllOpenRuecklaeufer()
        {
            var pakete = dms.Pakete.Where(r => r.RuecklaeuferAbgerechnet == false).ToList();
            foreach (var paket in pakete)
            {
                paket.RuecklaeuferAbgerechnet = true;
            }
            dms.SaveChanges();
            return Json("true");
        }

        public ActionResult GenerateRucklaeuferExcel()
        {

            List<Paket> ruecklaeufer = dms.Pakete.Where(r => r.Ruecklaeufer).OrderBy(r => r.RuecklaeuferAbgerechnet).ThenBy(r => r.Ruecksendedatum).ToList();


            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Rückläufer");
            ws.View.ShowGridLines = true;
            ws.Cells[1, 1].Value = "Rückläufer";

            //Überschrift
            ExcelRange heading = ws.Cells[1, 1, 1, 5];
            heading.Merge = true;
            heading.Style.Font.Bold = true;
            heading.Style.Font.Size = 15;
            ws.Row(1).Height = 30;

            //Überschriften Austelleradresse und Lieferadresse
            ws.Cells[3, 4].Value = "Austelleradresse";
            ws.Cells[3, 11].Value = "Abweichende Lieferadresse";

            //int i = 20;
            //foreach (var artikel in Artikelliste)
            //{
            //    i++;
            //    ws.Cells[4, i].Value = artikel.Artikelnummer + "\r\n" + artikel.artikelart.Art + "\r\n" + artikel.Name + "\r\n" + artikel.Sprache.Sprache;
            //    //ws.Cells[4, 1 + i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium, Color.Black);
            //    ws.Cells[4, i].AutoFitColumns();
            //    ws.Cells[4, i].Style.WrapText = true;
            //}

            ws.Cells[4, 1].Value = "Nr."; ws.Cells[4, 2].Value = "Messename"; ws.Cells[4, 3].Value = "Bemerkung";
            ws.Cells[4, 4].Value = "NAME1"; ws.Cells[4, 5].Value = "NAME2"; ws.Cells[4, 6].Value = "NAME3"; ws.Cells[4, 7].Value = "Strasse"; ws.Cells[4, 8].Value = "PLZ"; ws.Cells[4, 9].Value = "Stadt"; ws.Cells[4, 10].Value = "Land";
            ws.Cells[4, 11].Value = "NAME1"; ws.Cells[4, 12].Value = "NAME2"; ws.Cells[4, 13].Value = "NAME3"; ws.Cells[4, 14].Value = "Strasse"; ws.Cells[4, 15].Value = "PLZ"; ws.Cells[4, 16].Value = "Stadt"; ws.Cells[4, 17].Value = "Land";
            ws.Cells[4, 18].Value = "Bestelldatum"; ws.Cells[4, 19].Value = "Versanddatum"; ws.Cells[4, 20].Value = "Gewicht"; ws.Cells[4, 21].Value = "Versandart"; ws.Cells[4, 22].Value = "Paketnummer";
            ws.Cells[4, 23].Value = "Versandkosten"; ws.Cells[4, 24].Value = "Rücksendedatum"; ws.Cells[4, 25].Value = "Zusatzkosten"; ws.Cells[4, 26].Value = "Abgerechnet";

            //Layout--Hauptüberschriften
            ExcelRange headingAusteller = ws.Cells[3, 4, 3, 10];
            headingAusteller.Merge = true;
            headingAusteller.Style.Font.Bold = true;
            headingAusteller.Style.Font.Size = 9;
            headingAusteller.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAusteller.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAusteller.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAusteller.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAusteller.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingLiefer = ws.Cells[3, 11, 3, 17];
            headingLiefer.Merge = true;
            headingLiefer.Style.Font.Bold = true;
            headingLiefer.Style.Font.Size = 9;
            headingLiefer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingLiefer.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingLiefer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingLiefer.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingLiefer.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ExcelRange headingAngaben = ws.Cells[4, 1, 4, 26];
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Font.Size = 9;
            headingAngaben.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headingAngaben.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headingAngaben.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);
            headingAngaben.Style.Font.Bold = true;
            headingAngaben.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            headingAngaben.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            headingAngaben.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //ExcelRange toptablehead;
            //tableHead.AutoFitColumns();

            int i=4;
            int auftragsnummer = 0;
            if (ruecklaeufer != null && ruecklaeufer.Count > 0)
            {
                foreach (var paket in ruecklaeufer)
                {
                    i++;
                    auftragsnummer++;
                    Kontakdaten kontakt = paket.auftrag.Auftraggeberadresse;
                    List<string> Lieferadresse = new List<string>();
                    if (paket.auftrag.Lieferadresse != null)
                    {
                        Lieferadresse.Add(paket.auftrag.Lieferadresse.Name); Lieferadresse.Add(paket.auftrag.Lieferadresse.Name2); Lieferadresse.Add(paket.auftrag.Lieferadresse.Name3); Lieferadresse.Add(paket.auftrag.Lieferadresse.Strasse);
                        Lieferadresse.Add(paket.auftrag.Lieferadresse.PLZ); Lieferadresse.Add(paket.auftrag.Lieferadresse.Ort); Lieferadresse.Add(paket.auftrag.Lieferadresse.Land.land);
                    }
                    else
                    {
                        for (int r = 0; r < 7; r++)
                        {
                            Lieferadresse.Add("");
                        }
                    }
                    string abgerechnet = "offen";
                    if (paket.RuecklaeuferAbgerechnet)
                    {
                        abgerechnet = "abgerechnet";
                    }
                    ws.Cells[i, 1].Value = auftragsnummer; ws.Cells[i, 2].Value = paket.auftrag.messe.Name; ws.Cells[i, 3].Value = paket.Bemerkung; ws.Cells[i, 4].Value = kontakt.Name; ws.Cells[i, 5].Value = kontakt.Name2;
                    ws.Cells[i, 6].Value = kontakt.Name3; ws.Cells[i, 7].Value = kontakt.Strasse; ws.Cells[i, 8].Value = kontakt.PLZ; ws.Cells[i, 9].Value = kontakt.Ort; ws.Cells[i, 10].Value = kontakt.Land.land;
                    ws.Cells[i, 11].Value = Lieferadresse[0]; ws.Cells[i, 12].Value = Lieferadresse[1]; ws.Cells[i, 13].Value = Lieferadresse[2]; ws.Cells[i, 14].Value = Lieferadresse[3];
                    ws.Cells[i, 15].Value = Lieferadresse[4]; ws.Cells[i, 16].Value = Lieferadresse[5]; ws.Cells[i, 17].Value = Lieferadresse[6]; ws.Cells[i, 18].Value = paket.auftrag.Bestelldatum;
                    ws.Cells[i, 19].Value = paket.Versanddatum; ws.Cells[i, 20].Value = paket.Gewicht; ws.Cells[i, 21].Value = paket.Versandart; ws.Cells[i, 22].Value = paket.Paketnummer;
                    ws.Cells[i, 23].Value = paket.Preis; ws.Cells[i, 24].Value = paket.Ruecksendedatum; ws.Cells[i, 25].Value = paket.Zusatzkosten; ws.Cells[i, 26].Value = abgerechnet;

                }


                //Layout- Angaben 
                ws.Cells[5, 1, i, 26].Style.Font.Size = 9;
                ws.Cells[5, 1, i, 26].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i, 26].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i, 26].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, 1, i, 26].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                ws.Cells[5, 23, i, 23].Style.Numberformat.Format = @"#,##0.00 €";
                ws.Cells[5, 25, i, 25].Style.Numberformat.Format = @"#,##0.00 €";
                ws.Cells[5, 20, i, 20].Style.Numberformat.Format = @"#,##0.000";
                ws.Cells[5, 18, i, 18].Style.Numberformat.Format = @"dd.MM.yyyy";
                ws.Cells[5, 19, i, 19].Style.Numberformat.Format = @"dd.MM.yyyy";
                ws.Cells[5, 24, i, 24].Style.Numberformat.Format = @"dd.MM.yyyy";

            }
            else
            {
                ws.Cells[5, 1].Value = "Keine Rückläufer vorhanden";
            }

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", "attachment;  filename=Rückläufer"  + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();

            return RedirectToAction("Ruecklaeufer");
        }

        //public ActionResult ExcelOffeneRuecklaeufer()
        //{
        //    ICollection<Paket> Ruecklaeufer = dms.Pakete.Where(r => r.Ruecklaeufer && !r.RuecklaeuferAbgerechnet).ToList();

        //    ExcelPackage pck = new ExcelPackage();
        //    var ws = pck.Workbook.Worksheets.Add("Rückläufer");
        //    ws.View.ShowGridLines = true;
        //    ws.Cells[1, 1].Value = "Offene Rückläufer";



        //    //tabellen überschriften artikel und adressangaben
        //    int i = 20;
        //    foreach (var artikel in Artikelliste)
        //    {
        //        i++;
        //        ws.cells[4, i].value = artikel.artikelnummer + "\r\n" + artikel.artikelart.art + "\r\n" + artikel.name + "\r\n" + artikel.sprache.sprache;
        //        ws.cells[4, i].autofitcolumns();
        //        ws.cells[4, i].style.wraptext = true;
        //    }

        //    ws.cells[4, 1].value = "nr."; ws.cells[4, 2].value = "messe"; ws.cells[4, 3].value = "bemerkung";
        //    ws.cells[4, 4].value = "name1"; ws.cells[4, 5].value = "name2"; ws.cells[4, 6].value = "name3"; ws.cells[4, 7].value = "strasse"; ws.cells[4, 8].value = "plz"; ws.cells[4, 9].value = "stadt"; ws.cells[4, 10].value = "land";
        //    ws.cells[4, 11].value = "name1"; ws.cells[4, 12].value = "name2"; ws.cells[4, 13].value = "name3"; ws.cells[4, 14].value = "strasse"; ws.cells[4, 15].value = "plz"; ws.cells[4, 16].value = "stadt"; ws.cells[4, 17].value = "land";
        //    ws.cells[4, 18].value = "halle und stand"; ws.cells[4, 19].value = "bestelldatum"; ws.cells[4, 20].value = "versanddatum";

        //    //layout-hauptüberschriften
        //    excelrange mainheading = ws.cells[1, 1, 1, i];
        //    mainheading.merge = true;

        //    mainheading.style.font.bold = true;
        //    mainheading.style.font.size = 13;
        //    //mainheading.style.fill.patterntype = officeopenxml.style.excelfillstyle.solid;
        //    //mainheading.style.fill.backgroundcolor.setcolor(system.drawing.color.lightsteelblue);
        //    excelrange headingausteller = ws.cells[3, 4, 3, 10];
        //    headingausteller.merge = true;
        //    headingausteller.style.font.bold = true;
        //    headingausteller.style.font.size = 9;
        //    headingausteller.style.fill.patterntype = officeopenxml.style.excelfillstyle.solid;
        //    headingausteller.style.fill.backgroundcolor.setcolor(system.drawing.color.lightgray);
        //    headingausteller.style.border.borderaround(officeopenxml.style.excelborderstyle.thin, color.black);
        //    headingausteller.style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //    headingausteller.style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //    excelrange headingliefer = ws.cells[3, 11, 3, 17];
        //    headingliefer.merge = true;
        //    headingliefer.style.font.bold = true;
        //    headingliefer.style.font.size = 9;
        //    headingliefer.style.fill.patterntype = officeopenxml.style.excelfillstyle.solid;
        //    headingliefer.style.fill.backgroundcolor.setcolor(system.drawing.color.lightgray);
        //    headingliefer.style.border.borderaround(officeopenxml.style.excelborderstyle.thin, color.black);
        //    headingliefer.style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //    headingliefer.style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //    excelrange headingangaben = ws.cells[4, 1, 4, i];
        //    headingangaben.style.font.bold = true;
        //    headingangaben.style.font.size = 9;
        //    headingangaben.style.fill.patterntype = officeopenxml.style.excelfillstyle.solid;
        //    headingangaben.style.fill.backgroundcolor.setcolor(system.drawing.color.lightgray);
        //    headingangaben.style.border.borderaround(officeopenxml.style.excelborderstyle.thin, color.black);
        //    headingangaben.style.font.bold = true;
        //    headingangaben.style.border.left.style = officeopenxml.style.excelborderstyle.thin;
        //    headingangaben.style.border.right.style = officeopenxml.style.excelborderstyle.thin;
        //    headingangaben.style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //    headingangaben.style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;

        //    list<werbemittelauftrag> auftraege = messe.auftraege.orderby(r => r.bestelldatum).tolist();
        //    i = 4;
        //    int auftragsnummer = 0;
        //    int paketecount = 0;
        //    if (auftraege != null && auftraege.count > 0)
        //    {
        //        foreach (var auftrag in auftraege)
        //        {
        //            auftragsnummer++;
        //            int pc = auftrag.pakete.count;
        //            kontakdaten kontakt = auftrag.auftraggeberadresse;

        //            int j = 21;
        //            foreach (var artikel in artikelliste)
        //            {
        //                int pn = 0;
        //                foreach (var paket in auftrag.pakete)
        //                {
        //                    pn++;
        //                    paketecount++;
        //                    if (paket.artikelmenge.singleordefault(r => r.artikel.artikelid == artikel.artikelid) != null && paket.artikelmenge.singleordefault(r => r.artikel.artikelid == artikel.artikelid).menge > 0)
        //                    {
        //                        ws.cells[i + pn, j].value = paket.artikelmenge.singleordefault(r => r.artikel.artikelid == artikel.artikelid).menge;
        //                        ws.cells[i + pn, j].style.border.bottom.style = officeopenxml.style.excelborderstyle.dotted;
        //                        if (pn == 1)
        //                        {
        //                            ws.cells[i + pn, j].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ws.cells[i + pn, j].style.border.bottom.style = officeopenxml.style.excelborderstyle.dotted;
        //                        if (pn == 1)
        //                        {
        //                            ws.cells[i + pn, j].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //                        }

        //                    }
        //                }
        //                if (pn == 0)
        //                {
        //                    foreach (var artikelmenge in auftrag.auftragsmengen)
        //                    {
        //                        if (artikel.artikelid == artikelmenge.artikel.artikelid)
        //                        {
        //                            ws.cells[i + 1, j].value = artikelmenge.menge;
        //                            ws.cells[i + 1, j].style.border.bottom.style = officeopenxml.style.excelborderstyle.thin;
        //                            ws.cells[i + 1, j].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //                        }
        //                        else
        //                        {
        //                            ws.cells[i + 1, j].style.border.bottom.style = officeopenxml.style.excelborderstyle.thin;
        //                            ws.cells[i + 1, j].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //                        }
        //                    }
        //                }
        //                j++;

        //            }
        //            int pn2 = 0;
        //            foreach (var paket in auftrag.pakete)
        //            {
        //                pn2++;
        //                ws.cells[i + pn2, 20].value = paket.versanddatum.tostring("dd.mm.yyyy");
        //                ws.cells[i + pn2, 20].style.border.bottom.style = officeopenxml.style.excelborderstyle.dotted;
        //                if (pn2 == 1)
        //                {
        //                    ws.cells[i + pn2, 20].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //                }
        //            }
        //            i += pc;
        //            if (pc == 0)
        //            {
        //                i++;
        //                ws.cells[i, 20].style.border.bottom.style = officeopenxml.style.excelborderstyle.thin;
        //                ws.cells[i, 20].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //            }
        //            list<string> angaben = new list<string>() { auftragsnummer.tostring(), auftrag.messe.name, auftrag.bemerkung, kontakt.name, kontakt.name2, kontakt.name3, kontakt.strasse, kontakt.plz, kontakt.ort, kontakt.land.land };
        //            //list<string> angaben_2 = new list<string>() { };
        //            if (auftrag.lieferadresse != null)
        //            {
        //                angaben.add(auftrag.lieferadresse.name); angaben.add(auftrag.lieferadresse.name2); angaben.add(auftrag.lieferadresse.name3); angaben.add(auftrag.lieferadresse.strasse);
        //                angaben.add(auftrag.lieferadresse.plz); angaben.add(auftrag.lieferadresse.ort); angaben.add(auftrag.lieferadresse.land.land);
        //            }
        //            else
        //            {
        //                for (int r = 0; r < 7; r++)
        //                {
        //                    angaben.add(" ");
        //                }
        //            }
        //            angaben.add(auftrag.halleustand);
        //            angaben.add(auftrag.bestelldatum.tostring("dd.mm.yyyy"));
        //            if (pc > 1)
        //            {

        //                for (int m = 1; m <= 19; m++)
        //                {
        //                    excelrange auftragsangaben = ws.cells[i - pc + 1, m, i, m];
        //                    auftragsangaben.merge = true;
        //                    auftragsangaben.value = angaben[m - 1];
        //                }
        //            }
        //            else
        //            {
        //                for (int m = 1; m <= 19; m++)
        //                {
        //                    ws.cells[i, m].value = angaben[m - 1];
        //                }
        //            }
        //            //}
        //        }
        //        i++;
        //        int ac = 20;
        //        foreach (var artikel in artikelliste)
        //        {
        //            ac++;
        //            ws.cells[i, ac].formula = string.format("sum({0}:{1})", excelcellbase.getaddress(5, ac), excelcellbase.getaddress(i - 1, ac));
        //            ws.cells[i + 1, ac].value = artikel.bestand;
        //        }
        //        ws.cells[i + 1, 20].value = "restbestände";

        //        //layout- angaben
        //        //system.diagnostics.debug.writeline("wert für i: " + i);
        //        //if (i == 5)
        //        //{

        //        //}
        //        ws.cells[5, 1, i - 1, ac].style.font.size = 9;
        //        ws.cells[5, 1, i - 1, ac].style.border.left.style = officeopenxml.style.excelborderstyle.thin;
        //        ws.cells[5, 1, i - 1, ac].style.border.right.style = officeopenxml.style.excelborderstyle.thin;
        //        ws.cells[5, 1, i - 1, 19].style.border.top.style = officeopenxml.style.excelborderstyle.thin;
        //        ws.cells[5, 1, i - 1, 19].style.border.bottom.style = officeopenxml.style.excelborderstyle.thin;
        //        ws.cells[5, 1, i - 1, ac + 5].style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //        ws.cells[5, 1, i - 1, 1].style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //        ws.cells[5, 2, i - 1, 17].style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.left;
        //        ws.cells[5, 8, i - 1, 8].style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //        ws.cells[5, 18, i - 1, ac].style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;


        //        //layout-letzte zeile summe bestellungen 
        //        excelrange headingbottom = ws.cells[i, 1, i, ac];
        //        headingbottom.style.font.bold = true;
        //        headingbottom.style.font.size = 10;
        //        headingbottom.style.fill.patterntype = officeopenxml.style.excelfillstyle.solid;
        //        headingbottom.style.fill.backgroundcolor.setcolor(system.drawing.color.lightgray);
        //        headingbottom.style.border.borderaround(officeopenxml.style.excelborderstyle.thin, color.black);
        //        headingbottom.style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //        headingbottom.style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //        headingbottom.style.border.right.style = officeopenxml.style.excelborderstyle.thin;

        //        //zeile aktuelle bestände der artikel
        //        excelrange artikelbestaende = ws.cells[i + 1, 20, i + 1, ac];
        //        artikelbestaende.style.font.bold = true;
        //        artikelbestaende.style.font.size = 10;
        //        artikelbestaende.style.border.borderaround(officeopenxml.style.excelborderstyle.thin, color.black);
        //        artikelbestaende.style.verticalalignment = officeopenxml.style.excelverticalalignment.center;
        //        artikelbestaende.style.horizontalalignment = officeopenxml.style.excelhorizontalalignment.center;
        //        artikelbestaende.style.border.right.style = officeopenxml.style.excelborderstyle.thin;
        //    }
        //    else
        //    {
        //        ws.cells[5, 1].value = "keine aufträge vorhanden";
        //    }

        //    //ausgabe als exceldatei anstoßen
        //    system.web.httpresponse response = system.web.httpcontext.current.response;
        //    response.clearcontent();
        //    response.clear();
        //    response.contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    response.appendheader("content-disposition", "attachment;  filename=werbemittelübersicht-messe-stuttgart_" + messe.name.replace(" ", "-") + "_" + datetime.now.tostring("yyyymmdd") + ".xlsx");
        //    response.binarywrite(pck.getasbytearray());
        //    response.end();
        //    return view("details", messe);
        //}

    }
}


//---------------------- Messe Löschen -----------------------------------------


