using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WPMAsignmentHandling.Models;

namespace WPMAsignmentHandling.Additional
{

    public static class Extensions
    {
        

        public static bool CaseInsensitiveContains(this string text, string value)
        {
            //string text = value;
            bool returnValue = false;
            if(text.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
            
        }

        public static bool NotificationUndercutMeldebestand (Artikel artikel){

            string Messename = "";
            if (artikel.MesseartikelAllgemein)
            {
                Messename = "Allgemein";
            }else{
                Messename = artikel.Messe.Name;
            }

            string subject = "Meldebestand unterschritten: " + artikel.Artikelnummer + " | " + Messename + " | " + artikel.artikelart.Art + " | " + artikel.Name + " | " + artikel.Sprache.Sprache;
            string mailbody ="Unterschreitung des Meldebestands für folgenden Artikel,\n\n"
                                    + artikel.Artikelnummer + " | " + Messename + " | " + artikel.artikelart.Art + " | " + artikel.Name + " | " + artikel.Sprache.Sprache + "\n\n"
                                    + "Bestand: \t\t" + artikel.Bestand + "\n"
                                    + "Meldebstand: \t\t" + artikel.Meldebestand + "\n"
                                    + "Sicherheitsbestand: \t" + artikel.Sicherheitsbestand + "\n\n\n\n";
            


            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de")); //replace with valid value
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Werbemittelapplikation");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }

            return true;
        }

        public static bool NotificationUndercutSicherheitsbestand(Artikel artikel)
        {
            string Messename = "";
            if (artikel.MesseartikelAllgemein)
            {
                Messename = "Allgemein";
            }
            else
            {
                Messename = artikel.Messe.Name;
            }

            string subject = "Sicherheitsbestand unterschritten: " + artikel.Artikelnummer + " | " + Messename + " | " + artikel.artikelart.Art + " | " + artikel.Name + " | " + artikel.Sprache.Sprache;
            string Artikel_Inforamtion ="Unterschreitung des Sicherheitsbestands für folgenden Artikel,\n\n"
                                    + artikel.Artikelnummer + " | " + Messename + " | " + artikel.artikelart.Art + " | " + artikel.Name + " | " + artikel.Sprache.Sprache + "\n\n"
                                    + "Bestand: \t\t" + artikel.Bestand + "\n"
                                    + "Meldebstand: \t\t" + artikel.Meldebestand + "\n"
                                    + "Sicherheitsbestand: \t" + artikel.Sicherheitsbestand + "\n\n\n\n";
            //string ContactPersons_Messe = "Projektleiter\n" 
            //                        + 



            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de")); 
            message.Subject = subject;
            message.Body = string.Format(Artikel_Inforamtion, "Werbemittelapplikation");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }

            return true;
        }

        public static bool NotificationUserRegistration(ApplicationUser user)
        {
            string subject = "Benutzerregiestrierung";
            string mailbody = "Der Benutzer, \n\n"
                                    + "Frau/Herr " + user.Vorname + " " + user.Nachname + "\n\n" 
                                    + "Tel.: " +user.Telefonnummer + "\n"
                                    + "Email: " + user.Email+ "\n\n"
                                    + "Hat sich für den Marketingartikel Webshop registriert.";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de")); 
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Marketingartikel-Webshop");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool NotificationUserEmailComfirmation(ApplicationUser user)
        {
            string subject = "Bestätigung der Email-Adresse";
            string mailbody = "Der Benutzer, \n\n"
                                    + "Frau/Herr " + user.Vorname + " " + user.Nachname + "\n\n"
                                    + "Tel.: " + user.Telefonnummer + "\n"
                                    + "Email: " + user.Email + "\n\n"
                                    + "Hat sich seine Email-Adresse für die Anmeldung für den Marketingartikel-Webshop bestätigt.";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Marketingartikel-Webshop");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool NotificationResendEmailConfirmationLink(ApplicationUser user)
        {
            string subject = "Link zur Bestätigung der Email-Adresse";
            string mailbody = "Der Benutzer, \n\n"
                                    + "Frau/Herr " + user.Vorname + " " + user.Nachname + "\n\n"
                                    + "Tel.: " + user.Telefonnummer + "\n"
                                    + "Email: " + user.Email + "\n\n"
                                    + "Hat einen Link zur Bestätigung seiner Emailadresse angefordert";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Marketingartikel-Webshop");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool NotificationUserResetPassword(ApplicationUser user)
        {
            string subject = "Passwort zurücksetzen";
            string mailbody = "Der Benutzer, \n\n"
                                    + "Frau/Herr " + user.Vorname + " " + user.Nachname + "\n\n"
                                    + "Tel.: " + user.Telefonnummer + "\n"
                                    + "Email: " + user.Email + "\n\n"
                                    + "hat einen Link zur Vergabe eines neuen Passworts erhalten.";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Marketingartikel-Webshop");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool NotificationUserChangePassword(ApplicationUser user)
        {
            string subject = "Änderung des Passworts";
            string mailbody = "Der Benutzer, \n\n"
                                    + "Frau/Herr " + user.Vorname + " " + user.Nachname + "\n\n"
                                    + "Tel.: " + user.Telefonnummer + "\n"
                                    + "Email: " + user.Email + "\n\n"
                                    + "hat ein neues Passwort für seinen Account vergeben.";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Marketingartikel-Webshop");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

        public static bool SendOrderConfirmation(OrderConfirmationModel model, ApplicationUser user)
        {

            Kontakdaten Kontakt = null; 
              if (model.WMA.Lieferadresse != null)
              {
                  Kontakt = model.WMA.Lieferadresse;
              }
              else
              {
                  Kontakt = model.WMA.Auftraggeberadresse;
              }
              string Auftragsartikel = "";
              float Gesamtpreis = 0;
              foreach (var artikel in model.Auftragsmengen)
              {
                  var VEs = artikel.menge / artikel.artikel.Verpackungseinheit;
                  var GesamtpreisArtikel = artikel.artikel.PreisProVE * artikel.menge / artikel.artikel.Verpackungseinheit;
                  Gesamtpreis += artikel.artikel.PreisProVE * artikel.menge / artikel.artikel.Verpackungseinheit;
                  Auftragsartikel += "\t" + VEs + "(" + artikel.artikel.Verpackungseinheit + ")" + "\t" + artikel.menge + "\t" + artikel.artikel.PreisProVE.ToString("0.00") + " €" + "\t\t" + GesamtpreisArtikel.ToString("0.00") + " €" + "\t\t" + artikel.artikel.Name + "\n";
              }


              string mailBody = "Sehr geehrte(r) Frau/Herr " + model.UserName + ",\n\n"
                  + "Ihre Bestellung ist bei uns eingegangen. Wir werden diese sorgfältig bearbeiten und mit der nächsten Lieferung versenden.\n\n"
                  + "Voraussichtliches Lieferdatum: " + DateTime.Now.AddDays(1).ToString("dd.MM.yyy") + "\n\n"
                  + "Auftragsnummer: \t" + model.WMA.Kennzeichnung + "\n"
                  + "Bestelldatum: \t\t" + model.WMA.Bestelldatum.ToString("dd.MM.yyy") + "\n"
                  + "Lieferanschrift: \t" + Kontakt.Name + "\n"
                  + "\t\t\t" + Kontakt.Name2 + "\n"
                  + "\t\t\t" + Kontakt.Name3 + "\n"
                  + "\t\t\t" + Kontakt.Strasse + "\n"
                  + "\t\t\t" + Kontakt.PLZ + " " + Kontakt.Ort + "\n\n\n"
                  + "\tVEs\tAnzahl\tPreis VE\tGesamtpreis\tArtikel\n"
                  + Auftragsartikel + "\n\n"
                  + "\t\t\t\t\tGesamtpreis: " + Gesamtpreis.ToString("0.00") + " €\n\n\n" 
                  + "Wenn Sie weitere Fragen zu Ihrer Bestellung haben, helfen wir Ihnen gerne weiter. Sie erreichen uns unter der Rufnummer 0711-781988-30 oder werbemittel@briefdruck.de\n\n"
                  + "Unser Serviceteam ist montags bis freitags von 09:00 Uhr bis 17:00 Uhr erreichbar.\n\n\n"
                  + "Ihr Winkhardt-Team"
                  ;
            
            
            if (!String.IsNullOrEmpty(mailBody))
            {
                var message = new MailMessage();
                //message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
                message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
                message.To.Add(new MailAddress(model.UserMail));
                message.Subject = "Auftragsbestätigung: " + model.WMA.Kennzeichnung;
                message.Body = string.Format(mailBody, "webshop@briefdruck.de");
                message.IsBodyHtml = false;
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(message);
                }
            }
            return true;
        }

        public static bool CheckDateAndSendSpecificMailReminder()
        {
            DMS_Winkhardt_DB dms = new DMS_Winkhardt_DB();
            
            DateTime date = DateTime.Now;
            string aktuellesJahr = date.Year.ToString();
            //if(date.Date)

            if(date.Day == 1 )
            {
                SendMailReminder(1, "es ist wieder an der Zeit die Rückläufer in Augenschein zu nehmen.");
            }

            if (date.ToString("dd.MM.yyyy") == "31.03." + aktuellesJahr || date.ToString("dd.MM.yyyy") == "30.09." + aktuellesJahr || 
                date.ToString("dd.MM.yyyy") == "30.06." + aktuellesJahr || date.ToString("dd.MM.yyyy") == "31.12." + aktuellesJahr || date.ToString("dd.MM.yyyy") == "25.05." + aktuellesJahr)
            {
                SendMailReminder(4, "viele Marketingartikel sind raus. Jetzt muss Kohle rein!");
            }

            List<Messe> messen = dms.Messen.Where(r => !(r.abgegrechnet && r.RestmengenlisteVerschickt)).OrderBy(r => r.Startdatum).ToList();
            //SendCheckMail(messen);
            foreach(var messe in messen)
            {
                DateTime startDatum = messe.Startdatum;
                
                int daysBetweenStartAndToday = (startDatum - date).Days;
                string textMesseRestmengenlieferung = "die Messe \"" + messe.Name + " (Stardatum: " + messe.Startdatum.ToString("dd.MM.yyy") + ") \" möchtet ihre Restmengen zurück.";
                string textMesseAbrechnung = "die Messe \"" + messe.Name + " (Enddatum: " + messe.Enddatum.ToString("dd.MM.yyy") + ") \" möchte abgerechnet werden.";

                if ((date-messe.Enddatum).Days == 1)
                {
                    SendMailReminder(3, textMesseAbrechnung);
                }
                
                if (date.DayOfWeek == DayOfWeek.Monday)
                {
                    if (daysBetweenStartAndToday > 5 && daysBetweenStartAndToday < 8)
                    {
                        SendMailReminder(2, textMesseRestmengenlieferung);
                    }
                }
                if (date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    if (daysBetweenStartAndToday == 7)
                    {
                        SendMailReminder(2, textMesseRestmengenlieferung);
                    }
                }
                if (date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    if (daysBetweenStartAndToday == 7)
                    {
                        SendMailReminder(2, textMesseRestmengenlieferung);
                    }
                }
                if (date.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (daysBetweenStartAndToday == 7)
                    {
                        SendMailReminder(2, textMesseRestmengenlieferung);
                    }
                }
                                
                if(date.DayOfWeek == DayOfWeek.Friday) {
                    if (daysBetweenStartAndToday > 6 && daysBetweenStartAndToday < 10)
                    {
                        SendMailReminder(2, textMesseRestmengenlieferung);
                    }
                }
            }
            return true;
        }

        public static bool SendMailReminder(int Taetigkeit, String Taetigkeitsbeschreibung)
        {

            string subject = "";
            if (Taetigkeit == 1)
            {
                subject = "Rückläufer abrechnen";
            }
            if (Taetigkeit == 2)
            {
                subject = "Restmengen verschicken";
            }
            if (Taetigkeit == 3)
            {
                subject = "Messe abrechnen";
            }
            if (Taetigkeit == 4)
            {
                subject = "Marketingartikel abrechnen";
            }

            string mailbody = "Sehr geehrter Herr Jeltsch, \n\n"
                                    + Taetigkeitsbeschreibung + "\n\n"
                                    + "MfG, deine Werbemittel-Äppi";
            var message = new MailMessage();
            message.To.Add(new MailAddress("werbemittel@briefdruck.de"));
            message.To.Add(new MailAddress("uvre@gmx.de"));
            message.Subject = subject;
            message.Body = string.Format(mailbody, "Werbemittel-Applikation");
            message.IsBodyHtml = false;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return true;
        }

    }
}
