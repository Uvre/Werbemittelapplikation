using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using WPMAsignmentHandling.Models;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using System.Data.Entity.Validation;

namespace WPMAsignmentHandling.Controllers
{

    [Authorize(Roles = "Administrator, Winkhardt-MA")]
    public class HomeController : Controller
    {
        DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();
        private string unUsedVariable = "";

        private bool checkXMLElementIsOncePresent(XmlDocument xmlDoc, String ElementName)
        {
            XmlNodeList Elements = xmlDoc.GetElementsByTagName(ElementName);
            if (Elements.Count == 1)
            {
                Debug.WriteLine("Element '" + ElementName + "' wurde gefunden: " + Elements[0].InnerText);
                return true;
            }
            else
            {
                Debug.WriteLine("___Error___ Element '" + ElementName + "' wurde nicht gefunden oder es gab mehr als eines.");
                return false;
            }
        }

        private bool ReadOrderInformation(string OrderDirectoryPath, string Filename)
        {

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
                xmlDoc.Load(@OrderDirectoryPath + Filename);
                if (xmlDoc.DocumentElement.Name == "tdSalesOrder")
                {
                    Debug.WriteLine("'tdSalesOder' wurde gefunden. Starte Auftrag einlesen");

                    //Suche nach Messe
                    if (checkXMLElementIsOncePresent(xmlDoc, "Messe_MesseDesc"))
                    {
                        string Messename = xmlDoc.GetElementsByTagName("Messe_MesseDesc")[0].InnerText;
                        messe = dms.Messen.SingleOrDefault(r => r.Name == Messename);
                        if (messe != null)
                        {
                            WMA.messe = messe;
                        }
                        else
                        {
                            Debug.WriteLine("___Error___ Messe '" + Messename + "' wurde nicht gefunden ");
                            return false;
                        }
                    }

                    //Artikel der Bestellung auslesen
                    XmlNodeList tdSalesOrderPositions = xmlDoc.GetElementsByTagName("tdSalesOrderPos");
                    if (tdSalesOrderPositions.Count > 1)
                    {
                        for (int index = 0; index < xmlDoc.GetElementsByTagName("tdSalesOrderPos").Count - 1; index++)
                        {
                            string Artikelnummer = xmlDoc.GetElementsByTagName("ProductID")[index].InnerText;
                            string ArtikelmengeXML = xmlDoc.GetElementsByTagName("BaseQuantity")[index].InnerText.Replace(".", "").Replace(",", ".");
                            int Artikelmenge = 0;
                            if (!String.IsNullOrEmpty(Artikelnummer) && !String.IsNullOrEmpty(ArtikelmengeXML))
                            {
                                float Penismenge = float.Parse(ArtikelmengeXML);
                                Artikelmenge = (int)Penismenge;
                                Debug.WriteLine("Artikelnummer wurde gefunden: " + Artikelnummer);
                                Debug.WriteLine("Artikelmenge wurde gefunden: " + Artikelmenge);
                            }
                            else
                            {
                                Debug.WriteLine("___Error___    Artikelnummer und oder Artikelmenge wurde(n) nicht gefunden!");
                                return false;
                            }


                            Artikel artikel = dms.Artikell.SingleOrDefault(r => r.Artikelnummer == Artikelnummer);
                            if (artikel != null)
                            {
                                WMA.Auftragsmengen.Add(new Auftragsmenge { menge = Artikelmenge, artikel = artikel });

                            }
                            else
                            {
                                Debug.WriteLine("___Error___    Artikel wurde nicht gefunden: '" + Artikelnummer + "'");
                                return false;
                            }
                        }

                    }
                    else
                    {
                        Debug.WriteLine("In der Bestellung sind keine Artikel enthalten!");
                        return false;
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
                            Auftraggeberkontakt.Land = dms.Laender.SingleOrDefault(r => r.CountryCode == "DE");
                            Debug.WriteLine("___Error___    Ländercode konnte nicht zugeordnet werden: " + CountryCode + " Es wurde DE ausgewählt");

                        }
                        WMA.Auftraggeberadresse = Auftraggeberkontakt;
                    }
                    else
                    {
                        Debug.WriteLine("___Error___    Auftraggeberkontaktdaten konnten nicht gefunden werden: ");
                        return false;
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
                            Debug.WriteLine("___Error___    Ländercode konnte nicht zugeordnet werden: " + CountryCode);
                            return false;
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
                        Debug.WriteLine("___Error___    Lieferadressdaten konnten nicht gefunden werden: ");
                        return false;
                    }




                    //Auftrags Kennzeichnung
                    if (checkXMLElementIsOncePresent(xmlDoc, "MainReferenceNumber"))
                    {
                        string WMAKenzeichen = xmlDoc.GetElementsByTagName("MainReferenceNumber")[0].InnerText;
                        WMA.Kennzeichnung = WMAKenzeichen;
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
                    if (checkXMLElementIsOncePresent(xmlDoc, "MemoText"))
                    {
                        string Bemerkung = xmlDoc.GetElementsByTagName("MemoText")[0].InnerText;
                        WMA.Bemerkung = Bemerkung;
                    }

                    //XML Daten miteinfügen
                    WMA.Auftragsmailtext = xmlDoc.InnerText;

                    try
                    {
                        dms.Werbemittelauftraege.Add(WMA);
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
                    //dms.SaveChanges();
                    return true;
                }
                else
                {
                    Debug.WriteLine("Die Datei '" + Filename + "'konnte nicht eingelesen werden weil das Element 'tdSalesOder' nicht gefunden werden konnte.");
                    return false;
                }
            }
            catch (XmlException exc)
            {
                Debug.WriteLine("Not an XML Doc: " + exc.Message);
                return false;
            }
        }

        //[Authorize]
        public ViewResult Index()
        {
            //List<Artikel> Artikell = dms.Artikell.ToList();
            //foreach (Artikel artikel in Artikell){
            //    artikel.ArtikelHerstellungsart = dms.Artikel_Herstellungsarten.Find(1);
            //}
            //dms.SaveChanges();
            //List<Messe> Messen = dms.Messen.ToList();

            //foreach (var Messe in Messen)
            //{
            //    if (Messe.Position_1 == null)
            //    {
            //        Messe.Position_1 = "kA";
            //    }

            //    if (Messe.ProjektleiterName == null)
            //    {
            //        Messe.ProjektleiterName = "kA";
            //    }

            //    if (Messe.ProjektleiterTelefon == null)
            //    {
            //        Messe.ProjektleiterTelefon = "kA";
            //    }

            //    if (Messe.ProjektleiterEmail == null)
            //    {
            //        Messe.ProjektleiterEmail = "kA";
            //    }

            //    if (Messe.Position_2 == null)
            //    {
            //        Messe.Position_2 = "kA";
            //    }

            //    if (Messe.ProjektleiterVizeName == null)
            //    {
            //        Messe.ProjektleiterVizeName = "kA";
            //    }

            //    if (Messe.ProjektleiterVizeTelefon == null)
            //    {
            //        Messe.ProjektleiterVizeTelefon = "kA";
            //    }

            //    if (Messe.ProjektleiterVizeEmail == null)
            //    {
            //        Messe.ProjektleiterVizeEmail = "kA";
            //    }

            //    if (Messe.Position_3 == null)
            //    {
            //        Messe.Position_3 = "kA";
            //    }

            //    if (Messe.KommunikationsleiterName == null)
            //    {
            //        Messe.KommunikationsleiterName = "kA";
            //    }

            //    if (Messe.KommunikationsleiterTelefon == null)
            //    {
            //        Messe.KommunikationsleiterTelefon = "kA";
            //    }

            //    if (Messe.KommunikationsleiterEmail == null)
            //    {
            //        Messe.KommunikationsleiterEmail = "kA";
            //    }

            //    if (Messe.Position_4 == null)
            //    {
            //        Messe.Position_4 = "kA";
            //    }

            //    if (Messe.KommunikationsleiterVizeName == null)
            //    {
            //        Messe.KommunikationsleiterVizeName = "kA";
            //    }

            //    if (Messe.KommunikationsleiterVizeTelefon == null)
            //    {
            //        Messe.KommunikationsleiterVizeTelefon = "kA";
            //    }

            //    if (Messe.KommunikationsleiterVizeEmail == null)
            //    {
            //        Messe.KommunikationsleiterVizeEmail = "kA";
            //    }
            //}

            //List<Artikel> artikels = dms.Artikell.ToList();

            //foreach (var artikel in artikels)
            //{
            //    artikel.Artikelnummer = "ka";
            //}
            //dms.SaveChanges();

            //var paketpreise = new List<Paketpreis>
            //{
            //    new Paketpreis{ Preis=1.45F, Versandart="Brief klein"},
            //    new Paketpreis{ Preis=2.40F, Versandart="Brief groß"},
            //    new Paketpreis{ Preis=3.45F, Versandart="Brief int klein"},
            //    new Paketpreis{ Preis=7F, Versandart="Brief int mittel"},
            //    new Paketpreis{ Preis=7F, Versandart="Brief int groß"}
            //};
            //paketpreise.ForEach(s => dms.Paketpreise.Add(s));
            //dms.SaveChanges();

            //Werbemittelauftrag WMA = dms.Werbemittelauftraege.Find(3419);
            //var Pakete = dms.Pakete.Where(r => r.auftrag.WerbemittelauftragID == 3419);
            //List<int> Paketids = new List<int>();

            //foreach (var item in Pakete)
            //{
            //    Paketids.Add(item.PaketID);
            //}
            //foreach (var item in Paketids)
            //{
            //    var AmengenPakete = dms.Auftragsmengen.Where(r => r.paket.PaketID == item);
            //    foreach (var Amengen in AmengenPakete)
            //    {
            //        dms.Auftragsmengen.Remove(Amengen);
            //    }
            //}
            //dms.SaveChanges();
            //foreach (var paket in Pakete)
            //{

            //    dms.Pakete.Remove(paket);
            //}
            //dms.SaveChanges();


            //var AMengen = dms.Auftragsmengen.Where(r => r.auftrag.WerbemittelauftragID == WMA.WerbemittelauftragID);

            // foreach (var item in AMengen)
            // {
            //     dms.Auftragsmengen.Remove(item);
            // }
            // if (WMA.Auftraggeberadresse != null)
            // {
            //     dms.Kontaktdatenn.Remove(WMA.Auftraggeberadresse);
            // }
            // dms.SaveChanges();

            // if (WMA.Lieferadresse != null)
            // {
            //     dms.Kontaktdatenn.Remove(WMA.Lieferadresse);
            // }
            // if (WMA.Rechnungsadresse != null)
            // {
            //     dms.Kontaktdatenn.Remove(WMA.Rechnungsadresse);
            // }
            // if (WMA.Austelleradresse != null)
            // {
            //     dms.Kontaktdatenn.Remove(WMA.Austelleradresse);
            // }
            // var BAE = dms.BAenderungen.Where(r => r.WMA.WerbemittelauftragID == WMA.WerbemittelauftragID);
            // foreach (var bae in BAE)
            // {
            //     dms.BAenderungen.Remove(bae);
            // }
            // dms.Werbemittelauftraege.Remove(WMA);
            // dms.SaveChanges();

            //foreach (var messe in dms.Messen)
            //{
            //    messe.Active = true;
            //}
            //dms.SaveChanges();

            //foreach (var amenge in amengen)
            //{
            //    int menge = amenge.gelieferteMenge - amenge.menge;
            //    amenge.artikel.Bestand += menge;
            //    //amenge.gelieferteMenge = amenge.menge;
            //    //dms.SaveChanges();
            //}

            //Paket pak = dms.Pakete.Find(237);
            //int AuftragsId = pak.auftrag.WerbemittelauftragID;
            //Werbemittelauftrag wma = dms.Werbemittelauftraege.Find(pak.auftrag.WerbemittelauftragID);

            //foreach (var artikelmenge in pak.artikelmenge)
            //{
            //    Artikel artikel = dms.Artikell.Find(artikelmenge.artikel.ArtikelID);
            //    //Auftragsmenge wmaAuftrag = wma.Auftragsmengen.Single(r => r.artikel.ArtikelID == artikel.ArtikelID);
            //    //wmaAuftrag.gelieferteMenge -= artikelmenge.menge;
            //    dms.BAenderungen.Add(new Bestandsaenderung{ Artikel=artikel, Menge=artikelmenge.menge, Datum=DateTime.Now, Grund="Paket wurde gelöscht"});
            //    artikel.Bestand += artikelmenge.menge;
            //}
            //dms.Pakete.Remove(pak);
            //dms.SaveChanges();
            //wma = dms.Werbemittelauftraege.Find(AuftragsId);
            //System.Diagnostics.Debug.WriteLine("Afutrg " + wma.Pakete.Count);
            //if (wma.Pakete.Count > 0)
            //{
            //    List<Paket> pakete = wma.Pakete.OrderByDescending(r => r.Versanddatum).ToList();
            //    wma.Versanddatum = pakete.First().Versanddatum;
            //    wma.Stat = dms.Stati.Find(2);
            //}
            //else
            //{
            //    wma.Versanddatum = wma.Erstellungsdatum;
            //    wma.Stat = dms.Stati.Find(1);
            //}
            //dms.SaveChanges();
            //return View("Werbemittelauftragdetails", wma); 

            //List<Werbemittelauftrag> wmas = dms.Werbemittelauftraege.ToList();
            //foreach (var wma in wmas)
            //{
            //    wma.Bestelldatum = wma.Erstellungsdatum;
            //    wma.Auftragsmengen = dms.Werbemittelauftraege.Find(wma.WerbemittelauftragID).Auftragsmengen;
            //    wma.Pakete = dms.Werbemittelauftraege.Find(wma.WerbemittelauftragID).Pakete;
            //    try
            //    {
            //        List<Paket> Pakete = dms.Pakete.Where(r => r.auftrag.WerbemittelauftragID == wma.WerbemittelauftragID).ToList();

            //        if (Pakete.Count > 0)
            //        {
            //            //wma.Versanddatum = DateTime.Now;
            //            wma.Versanddatum = Pakete.Last().Versanddatum;
            //        }
            //        else
            //        {
            //            wma.Versanddatum = wma.Erstellungsdatum;
            //        }
            //        dms.SaveChanges();
            //    }
            //    catch (DbEntityValidationException dbEx)
            //    {
            //        foreach (var validationErrors in dbEx.EntityValidationErrors)
            //        {
            //            foreach (var validationError in validationErrors.ValidationErrors)
            //            {
            //                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
            //                System.Diagnostics.Debug.Write("val probelem" + validationError.ErrorMessage);
            //            }
            //        }
            //    }
            //}
            //Werbemittelauftrag wma_1 = dms.Werbemittelauftraege.Single(r => r.Kennzeichnung == "WM23200");
            //Werbemittelauftrag wma_2 = dms.Werbemittelauftraege.Single(r => r.Kennzeichnung == "WM23139");
            //System.Diagnostics.Debug.WriteLine(wma_1.HalleUStand + wma_1.kunde.Name);
            //System.Diagnostics.Debug.WriteLine(wma_2.HalleUStand + wma_2.kunde.Name);
            return View();
        }

        public ViewResult DownloadFTP()
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://transfer-ftp.de/");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("messe_xml", "2017Messe!");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            //File.


            //using (MemoryStream stream = new MemoryStream())
            //{
            //    //Download the File.
            //    response.GetResponseStream().CopyTo(stream);
            //    Response.AddHeader("content-disposition", "attachment;filename=" + "myfile.zip");
            //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    Response.BinaryWrite(stream.ToArray());
            //    Response.End();

            //}

            Debug.WriteLine(reader.ReadToEnd());

            Debug.WriteLine("Download Complete, status {0}", response.StatusDescription);

            reader.Close();
            response.Close();
            return View();
        }

        public ViewResult readOrders()
        {
            string OrderDirectoryPath = @"C:\Users\NotJohn\Desktop\XML Dateien\";
            //string[] xmlFiles = Directory.GetFiles(OrderDirectoryPath, "*.xml").Select(Path.GetFileName).ToArray();
            string[] xmlFiles = { "AB221700141.xml" };
            foreach (var Filename in xmlFiles)
            {
                ReadOrderInformation(OrderDirectoryPath, Filename);
            }
            return View();
        }
    }
}

//                 try
//                {
//                
//                }
//                catch (DbEntityValidationException dbEx)
//                {
//                    foreach (var validationErrors in dbEx.EntityValidationErrors)
//                    {
//                        foreach (var validationError in validationErrors.ValidationErrors)
//                        {
//                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
//                            System.Diagnostics.Debug.Write("val probelem" + validationError.ErrorMessage);
//                        }
//                    }
//                }



