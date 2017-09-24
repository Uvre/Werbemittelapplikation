using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WPMAsignmentHandling.Models;

namespace WPMAsignmentHandling.Additional
{
    public class DMSDBInitializer:DropCreateDatabaseAlways<DMS_Winkhardt_DB>
    {
        protected override void Seed(DMS_Winkhardt_DB context)
        {
            //var artikelarten = new List<Artikelart>
            //{
            //    new Artikelart{ Art="Briefklebemarken (1)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Briefklebemarken (2)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Briefklebemarken indi (3)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Vorteilscoupons (1)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Vorteilscoupons (2)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Vorteilscoupons (3)", Bemerkung="Rechnung Beilegen"},
            //    new Artikelart{ Art="Austellerprospekt"},
            //    new Artikelart{ Art="Besucherprospekt"},
            //    new Artikelart{ Art="Postkarte"},
            //    new Artikelart{ Art="Plakate"},
            //    new Artikelart{ Art="Flyer"},
            //    new Artikelart{ Art="Kuvert"},
            //};
            //artikelarten.ForEach(s => context.Artikelarten.Add(s));
            //context.SaveChanges();

            //var Sprachen = new List<Artikelsprache>
            //{
            //    new Artikelsprache{ Sprache="Deutsch"},
            //    new Artikelsprache{ Sprache="Englisch"},
            //    new Artikelsprache{ Sprache="Französisch"},
            //    new Artikelsprache{ Sprache="Spanisch"},
            //    new Artikelsprache{ Sprache="Italienisch"}
            //};
            //Sprachen.ForEach(s => context.Artikelsprachen.Add(s));
            //context.SaveChanges();

            //var Laender = new List<Land>
            //{
            //    new Land{  land="Afghanistan", CountryCode="AF"},new Land{  land="Ägypten", CountryCode="EG"}, new Land{  land="Albanien", CountryCode="AL"}, new Land{  land="Algerien", CountryCode="DZ"},
            //    new Land{  land="Amerikanische Jungferninseln", CountryCode="VI"},new Land{  land="Amerikanische Überseeinseln", CountryCode="UM"}, new Land{  land="Andorra", CountryCode="AD"}, new Land{  land="Angola", CountryCode="AO"},
            //    new Land{  land="Anguilla", CountryCode="AI"},new Land{  land="Antigua und Barbuda", CountryCode="AG"},new Land{  land="Äquatorial-Guinea", CountryCode="GQ"}, new Land{  land="Argentinien", CountryCode="AR"}, new Land{  land="Armenien", CountryCode="AM"},
            //    new Land{  land="Aruba", CountryCode="AW"},new Land{  land="Ascension", CountryCode="SH"}, new Land{  land="Aserbaidschan", CountryCode="AZ"}, new Land{  land="Äthiopien", CountryCode="ET"},
            //    new Land{  land="Australien", CountryCode="AU"},new Land{  land="Bahamas", CountryCode="BS"}, new Land{  land="Bahrain", CountryCode="BH"}, new Land{  land="Bangladesh", CountryCode="BD"},
            //    new Land{  land="Barbados", CountryCode="BB"},new Land{  land="Belarus", CountryCode="BY"}, new Land{  land="Belgien", CountryCode="BE"}, new Land{  land="Belize", CountryCode="BZ"},
            //    new Land{  land="Benin", CountryCode="BJ"},new Land{  land="Bermuda", CountryCode="BM"}, new Land{  land="Bhutan", CountryCode="BT"}, new Land{  land="Bolivien", CountryCode="BO"}, new Land{  land="Bosnien-Herzegowina", CountryCode="BA"}, 
            //    new Land{  land="Botswana", CountryCode="BW"},new Land{  land="Brasilien", CountryCode="BR"}, new Land{  land="Brunei Darussalam", CountryCode="BN"}, new Land{  land="Bulgarien", CountryCode="BG"}, new Land{  land="Burkina Faso", CountryCode="BF"}, 
            //    new Land{  land="Burundi", CountryCode="BI"},new Land{  land="Cayman", CountryCode="KY"}, new Land{  land="Chile", CountryCode="CL"}, new Land{  land="China, Taiwan", CountryCode="TW"}, new Land{  land="China, Volksrepublik", CountryCode="CN"}, 
            //    new Land{  land="Cookinseln", CountryCode="CK"}, new Land{  land="Costa Rica", CountryCode="CR"}, new Land{  land="Côte d\"Ivoire", CountryCode="CI"},  new Land{  land="Dänemark", CountryCode="DK"},new Land{  land="Deutschland", CountryCode="DE"},
            //    new Land{  land="Djibouti", CountryCode="DJ"}, new Land{  land="Dominica", CountryCode="DM"}, new Land{  land="Dominikanische Republik", CountryCode="DO"},new Land{  land="Ekuador", CountryCode="EC"}, new Land{  land="El Salvador", CountryCode="SV"},
            //    new Land{  land="Eritrea", CountryCode="ER"},new Land{  land="Estland", CountryCode="EE"}, new Land{  land="Falkland", CountryCode="FK"}, new Land{  land="Färöer", CountryCode="FO"}, new Land{  land="Fidschi", CountryCode="FJ"},
            //    new Land{  land="Finnland", CountryCode="FI"},new Land{  land="Frankreich", CountryCode="FR"}, new Land{  land="Französisch-Guayana", CountryCode="GF"}, new Land{  land="Französisch-Polynesien", CountryCode="PF"}, new Land{  land="Französische Südgebiete", CountryCode="TF"},
            //    new Land{  land="Gabun", CountryCode="GA"},new Land{  land="Gambia", CountryCode="GM"}, new Land{  land="Georgien", CountryCode="GE"}, new Land{  land="Ghana", CountryCode="GH"}, new Land{  land="Gibraltar", CountryCode="GI"},
            //    new Land{  land="Grenada", CountryCode="GD"},new Land{  land="Griechenland", CountryCode="GR"}, new Land{  land="Grönland", CountryCode="GL"}, new Land{  land="Grossbritannien", CountryCode="GB"}, new Land{  land="Guadeloupe", CountryCode="GP"},
            //    new Land{  land="Guam", CountryCode="GU"},new Land{  land="Guatemala", CountryCode="GT"}, new Land{  land="Guinea, Republik", CountryCode="GN"}, new Land{  land="Guinea-Bissau", CountryCode="GW"}, 
            //    new Land{  land="Guyana", CountryCode="GY"},new Land{  land="Haiti", CountryCode="HT"}, new Land{  land="Honduras", CountryCode="HN"}, new Land{  land="Hongkong", CountryCode="HK"}, new Land{  land="Indien", CountryCode="IN"},
            //    new Land{  land="Indonesien", CountryCode="ID"},new Land{  land="Irak", CountryCode="IQ"}, new Land{  land="Iran", CountryCode="IR"}, new Land{  land="Irland", CountryCode="IE"}, new Land{  land="Island", CountryCode="IS"},
            //    new Land{  land="Israel", CountryCode="IL"},new Land{  land="Italien", CountryCode="IT"}, new Land{  land="Jamaika", CountryCode="JM"}, new Land{  land="Japan", CountryCode="JP"}, new Land{  land="Jemen", CountryCode="YE"},
            //    new Land{  land="Jordanien", CountryCode="JO"}, new Land{  land="Kambodscha", CountryCode="KH"}, new Land{  land="Kamerun", CountryCode="CM"}, new Land{  land="Kanada", CountryCode="CA"},
            //    new Land{  land="Kapverdische Inseln", CountryCode="CV"},new Land{  land="Kasachstan", CountryCode="KZ"}, new Land{  land="Kenia", CountryCode="KE"}, new Land{  land="Kirgisistan", CountryCode="KG"}, new Land{  land="Kiribati", CountryCode="KI"},
            //    new Land{  land="Kokos-Inseln", CountryCode="CC"},new Land{  land="Kolumbien", CountryCode="CO"}, new Land{  land="Komoren", CountryCode="KM"}, new Land{  land="Kongo, Republik", CountryCode="CG"}, new Land{  land="Kongo, Demokratische Republik (ex-Zaire)", CountryCode="CD"},
            //    new Land{  land="Korea, Demo. Volksrepublik (Nordkorea)", CountryCode="KP"},new Land{  land="Korea, Republik (Südkorea)", CountryCode="KR"}, new Land{  land="Kosovo (Interim. Verw. der UNO)", CountryCode="XZ"}, new Land{  land="Kroatien", CountryCode="HR"}, new Land{  land="Kuba", CountryCode="CU"},
            //    new Land{  land="Kuwait", CountryCode="KW"},new Land{  land="Laos", CountryCode="LA"}, new Land{  land="Lesotho", CountryCode="LS"}, new Land{  land="Lettland", CountryCode="LV"}, new Land{  land="Libanon", CountryCode="LB"},
            //    new Land{  land="Liberia", CountryCode="LR"},new Land{  land="Libyen", CountryCode="LY"}, new Land{  land="Liechtenstein", CountryCode="LI"}, new Land{  land="Litauen", CountryCode="LT"}, new Land{  land="Luxemburg", CountryCode="LU"},
            //    new Land{  land="Macao", CountryCode="MO"},new Land{  land="Madagaskar", CountryCode="MG"}, new Land{  land="Malawi", CountryCode="MW"}, new Land{  land="Malaysia", CountryCode="MY"}, new Land{  land="Malediven", CountryCode="MV"},
            //    new Land{  land="Mali", CountryCode="ML"},new Land{  land="Malta", CountryCode="MT"}, new Land{  land="Marokko", CountryCode="MA"}, new Land{  land="Marianen, nördliche", CountryCode="MP"},
            //    new Land{  land="Marshallinseln", CountryCode="MH"},new Land{  land="Martinique", CountryCode="MQ"}, new Land{  land="Mauretanien", CountryCode="MR"}, new Land{  land="Mauritius", CountryCode="MU"}, new Land{  land="Mayotte", CountryCode="YT"},
            //    new Land{  land="Mazedonien ", CountryCode="MK"},new Land{  land="Mexiko", CountryCode="MX"}, new Land{  land="Moldova", CountryCode="MD"}, new Land{  land="Monaco", CountryCode="MC"}, new Land{  land="Mongolei", CountryCode="MN"},
            //    new Land{  land="Montenegro", CountryCode="ME"},new Land{  land="Montserrat", CountryCode="MS"}, new Land{  land="Mosambik", CountryCode="MZ"}, new Land{  land="Myanmar", CountryCode="MM"}, new Land{  land="Namibia", CountryCode="NA"},
            //    new Land{  land="Nauru", CountryCode="NR"},new Land{  land="Nepal", CountryCode="NP"}, new Land{  land="Neukaledonien", CountryCode="NC"}, new Land{  land="Neuseeland", CountryCode="NZ"}, new Land{  land="Nikaragua", CountryCode="NI"},
            //    new Land{  land="Niederlande", CountryCode="NL"},new Land{  land="Niederländische Antillen", CountryCode="AN"}, new Land{  land="Niger", CountryCode="NE"}, new Land{  land="Nigeria", CountryCode="NG"}, new Land{  land="Norfolk", CountryCode="NF"},
            //    new Land{  land="Norwegen", CountryCode="NO"},new Land{  land="Oman", CountryCode="OM"}, new Land{  land="Österreich", CountryCode="AT"}, new Land{  land="Pakistan", CountryCode="PK"}, new Land{  land="Palau", CountryCode="PW"},
            //    new Land{  land="Panama", CountryCode="PA"},new Land{  land="Papouasie-Nouvelle-Guinée", CountryCode="PG"}, new Land{  land="Paraguay", CountryCode="PY"}, new Land{  land="Pérou", CountryCode="PE"},
            //    new Land{  land="Philippines", CountryCode="PH"},new Land{  land="Pitcairn", CountryCode="PN"}, new Land{  land="Pologne", CountryCode="PL"}, new Land{  land="Polynésie française", CountryCode="PF"}, new Land{  land="Portugal", CountryCode="PT"},
            //    new Land{  land="Porto Rico", CountryCode="PR"},new Land{  land="Qatar", CountryCode="QA"}, new Land{  land="Réunion", CountryCode="RE"}, new Land{  land="Rwanda", CountryCode="RW"}, new Land{  land="Rumänien", CountryCode="RO"},
            //    new Land{  land="Russische Föderation", CountryCode="RU"},new Land{  land="Salomon-Inseln", CountryCode="SB"}, new Land{  land="Sambia", CountryCode="ZM"}, new Land{  land="Samoa ", CountryCode="AS"}, new Land{  land="San Marino", CountryCode="SM"},
            //    new Land{  land="Saudi-Arabien", CountryCode="SA"},new Land{  land="Schweden", CountryCode="SE"}, new Land{  land="Schweiz", CountryCode="CH"}, new Land{  land="Senegal", CountryCode="SN"}, new Land{  land="Serbien, Republik", CountryCode="RS"},
            //    new Land{  land="Seychellen", CountryCode="SC"},new Land{  land="Sierra Leone", CountryCode="SL"}, new Land{  land="Singapur", CountryCode="SG"}, new Land{  land="Slowakische Republik", CountryCode="SK"}, new Land{  land="Slowenien", CountryCode="SI"},
            //    new Land{  land="Somalia", CountryCode="SO"},new Land{  land="Spanien", CountryCode="ES"}, new Land{  land="Sri Lanka", CountryCode="LK"}, new Land{  land="St. Christoph und Nevis", CountryCode="KN"}, new Land{  land="St. Helena", CountryCode="SH"},
            //    new Land{  land="St. Lucia", CountryCode="LC"},new Land{  land="St. Pierre und Miquelon", CountryCode="PM"}, new Land{  land="St. Thomas und Principe", CountryCode="ST"}, new Land{  land="St. Vincent und die Grenadinen", CountryCode="VC"}, new Land{  land="Südafrika", CountryCode="ZA"},
            //    new Land{  land="Sudan", CountryCode="SD"},new Land{  land="Südgeorgien ", CountryCode="GS"}, new Land{  land="Suriname", CountryCode="SR"}, new Land{  land="Swasiland", CountryCode="SZ"}, new Land{  land="Syrien", CountryCode="SY"},
            //    new Land{  land="Tadschikistan", CountryCode="TJ"},new Land{  land="Tansania", CountryCode="TZ"}, new Land{  land="Thailand", CountryCode="TH"}, new Land{  land="Timor-Leste", CountryCode="TL"}, new Land{  land="Togo", CountryCode="TG"},
            //    new Land{  land="Tokelau", CountryCode="TK"},new Land{  land="Tonga", CountryCode="TO"}, new Land{  land="Trinidad und Tobago", CountryCode="TT"}, new Land{  land="Tristan da Cunha", CountryCode="SH"}, new Land{  land="Tschad", CountryCode="TD"},
            //    new Land{  land="Tschechische Republik", CountryCode="CZ"},new Land{  land="Tunesien", CountryCode="TN"}, new Land{  land="Türkei", CountryCode="TR"}, new Land{  land="Turkmenistan", CountryCode="TM"}, new Land{  land="Turks und Caicos", CountryCode="TC"},
            //    new Land{  land="Tuvalu", CountryCode="TV"},new Land{  land="Uganda", CountryCode="UG"}, new Land{  land="Ukraine", CountryCode="UA"}, new Land{  land="Ungarn", CountryCode="HU"}, new Land{  land="Uruguay", CountryCode="UY"},
            //    new Land{  land="Usbekistan", CountryCode="UZ"},new Land{  land="Vanuatu", CountryCode="VU"}, new Land{  land="Vatikan", CountryCode="VA"}, new Land{  land="Venezuela", CountryCode="VE"}, new Land{  land="Vereinigte Arabische Emirate", CountryCode="AE"},
            //    new Land{  land="Vereinigte Staaten von Amerika", CountryCode="US"},new Land{  land="Vietnam", CountryCode="VN"}, new Land{  land="Virginische Inseln", CountryCode="VG"}, new Land{  land="Wallis und Futuna", CountryCode="WF"}, new Land{  land="Weihnachtsinsel", CountryCode="CX"},
            //    new Land{  land="West Samoa", CountryCode="WS"},new Land{  land="Zentralafrika", CountryCode="CF"}, new Land{  land="Zypern", CountryCode="CY"}, new Land{  land="Zimbabwe", CountryCode="ZW"}
            //};
            //Laender.ForEach(s => context.Laender.Add(s));
            //context.SaveChanges();

            //var Stati = new List<ST>
            //{
            //    new ST{ wert="angelegt"},
            //    new ST{ wert="teilw. versendet"},
            //    new ST{ wert="versendet"},
            //    new ST{ wert="abgerechnet"}
            //};
            //Stati.ForEach(s => context.Stati.Add(s));
            //context.SaveChanges();

            //var Kontakte = new List<Kontakdaten>
            //{
            //    new Kontakdaten{ Name="Wie Auftraggeber", Name2="", Name3="", Strasse="-", PLZ="12345", Ort="nix leer", Land=context.Laender.Single(r => r.land == "Deutschland"),  EMail="", Telefon=""}
            //};
            //Kontakte.ForEach(s => context.Kontaktdatenn.Add(s));
            //context.SaveChanges();

            //var Kunden = new List<Kunde>
            //{
            //    new Kunde{ Name="Landesmesse Stuttgart GmbH", Erstellungsdatum = DateTime.Now, Hauptadresse= new Kontakdaten{ Name="Landesmesse Stuttgart GmbH", Name2="...Ausfüllen...", Name3="", Strasse="Messepiazza 1", PLZ="70629", Ort="Stuttgart", EMail="", Land= context.Laender.Single(r => r.land=="Deutschland") , Telefon=""}},
            //    new Kunde{ Name="Best Kunde AG", Erstellungsdatum = DateTime.Now, Hauptadresse = new Kontakdaten{ Name="Best Kunde AG", Name2="Herr Best", Strasse="best strasse 10", PLZ="55555", Ort="hintupfingen", EMail="best@best.de", Land=context.Laender.Single(r => r.land=="Schweiz"), Telefon="1234556778"}}
            //};
            //Kunden.ForEach(s => context.Kunden.Add(s));
            //context.SaveChanges();


            //DateTime time = new DateTime();
            //time = DateTime.Parse("2014-10-12");
            //DateTime end = new DateTime();
            //end = DateTime.Parse("2014-10-14");
            //var auftrags = new List<Werbemittelauftrag>
            //{
            //    new Werbemittelauftrag{ Stat=context.Stati.Find(1), Kennzeichnung="WMM2013_000001", messe = new Messe{Name="interband",  Startdatum= time, Enddatum=end, Webadresse="http://www.theuselessweb.com/",  Bannerkontakt="", Erstellungsdatum=DateTime.Now}, Erstellungsdatum=DateTime.Now, Auftragsmengen=new List<Auftragsmenge>(), Verschickungskosten=0, Pakete=new List<Paket>(), kunde=context.Kunden.Find(1), Lieferadresse=context.Kunden.Find(1).Hauptadresse, Rechnungsadresse= context.Kunden.Find(1).Hauptadresse, Austelleradresse=context.Kunden.Find(1).Hauptadresse, Auftraggeberadresse=context.Kunden.Find(1).Hauptadresse, Bestelldatum=DateTime.Now, Versanddatum=DateTime.Now}
            //};
            //auftrags.ForEach(s => context.Werbemittelauftraege.Add(s));
            //context.SaveChanges();


            //var artikels = new List<Artikel>
            //{
            //    new Artikel{Name="DasRingaDingaDingsbumsDing", artikelart= context.Artikelarten.Find(1), Format="A4", Gewicht=10, Bestand=500, Lagerplatz="kA", Erstellungsdatum=DateTime.Now, Messe=context.Messen.Find(1), Sprache=context.Artikelsprachen.Find(1) },
            //};
            //artikels.ForEach(s => context.Artikell.Add(s));
            //context.SaveChanges();

            //context.Werbemittelauftraege.Find(1).Auftragsmengen.Add(new Auftragsmenge { artikel = context.Artikell.Find(1), auftrag = context.Werbemittelauftraege.Find(1), menge = 200, gelieferteMenge=0 });
            //context.SaveChanges();

            
        }
    }
}