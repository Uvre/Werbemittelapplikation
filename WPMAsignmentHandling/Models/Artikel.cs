using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WPMAsignmentHandling.Additional;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Web.Mvc;

namespace WPMAsignmentHandling.Models 
{
    public class Artikel
    {
        public int ArtikelID { get; set; }



        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Erstellungsdatum { get; set; }
        public bool Active { get; set; }

        //[DisplayName("Herstellungsart")]
        //[Required(ErrorMessage = "Keine Herstellungsart ausgewählt!")]
        //public virtual Artikel_Herstellungsart ArtikelHerstellungsart { get; set; }

        [DisplayName("Artikelname")]
        [Required(ErrorMessage = "Kein Artikelname eingetragen!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Kein Format eingetragen!")]
        public string Format { get; set; }
        [Required(ErrorMessage = "Kein Gewicht eingetragen!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.0}")]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [Range(0, 10000, ErrorMessage = "Es kann kein Gewicht größer als 10kg eingetragen werden!")]
        public float Gewicht { get; set; }
        public string Bemerkung { get; set; }
        [Required(ErrorMessage = "Kein Bestand eingetragen!")]
        public int Bestand { get; set; }
        public bool MesseartikelAllgemein { get; set; }
        public string Lagerplatz { get; set; }
        public virtual ICollection<Werbemittelauftrag> auftraege { get; set; }
        public virtual ICollection<Bestandsaenderung> BAE { get; set; }
        public virtual ICollection<Auftragsmenge> Auftragsmengen { get; set; }
        public virtual Messe Messe { get; set; }
        public virtual Kunde Kunde { get; set; }
        [DisplayName("Artikelart")]
        public virtual Artikelart artikelart { get; set; }
        public virtual Artikelsprache Sprache { get; set; }


        [Required(ErrorMessage = "Keine Artikelnummer eingetragen!")]
        public string Artikelnummer { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [DisplayName("Inhalt pro Verpackungseinheit")]
        [Required(ErrorMessage = "Kein Inhalt pro Verpackungseinheit eingetragen!")]
        public int Verpackungseinheit { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [Required(ErrorMessage = "Kein Kartoninhalt eingetragen!")]
        public int Kartoninhalt { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [Required(ErrorMessage = "Kein Sicherheitsbestand eingetragen!")]
        public int Sicherheitsbestand { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [GreaterThan("Sicherheitsbestand")]
        [Required(ErrorMessage = "Kein Meldebestand eingetragen!")]
        public int Meldebestand { get; set; }

        public bool Landesmesseartikel { get; set; }

        public string Bildpfad { get; set; }
        [DisplayName("Auflage/Charge/MHD")]
        public string AuflageCharge { get; set; }
        public virtual ArtikelKategorie ArtikelKategorie { get; set; }

        [Required(ErrorMessage = "Kein Preis eingetragen!")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        [Range(0, 1000, ErrorMessage = "Es kann kein Preis größer als 1000€ eingetragen werden!")]
        [DisplayName("Preis pro Verpackungseinheit")]
        public float PreisProVE { get; set; }

        [Required(ErrorMessage = "Keine Breite eingetragen!")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        //[Range(0.1, 10000, ErrorMessage = "Breite kann nicht kleiner als 0,1mm und größer als 10000mm sein!")]
        public float Breite { get; set; }

        [Required(ErrorMessage = "Keine Höhe eingetragen!")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        //[Range(0.1, 10000, ErrorMessage = "Breite kann nicht kleiner als 0,1mm und größer als 10000mm sein!")]
        [DisplayName("Höhe")]
        public float Hoehe { get; set; }

        [Required(ErrorMessage = "Keine Tiefe eingetragen!")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^[,0-9]*$", ErrorMessage = "Es können nur Zahlen eingetragen werden")]
        //[Range(0.1, 10000, ErrorMessage = "Breite kann nicht kleiner als 0,1mm und größer als 10000mm sein!")]
        [DisplayName("Tiefe")]
        public float Laenge { get; set; }
        
    }

    public class GreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        public GreaterThanAttribute(string otherProperty)
            : base("{0} muss größer oder gleich {1} sein")
        {
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult
            IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) < 1 && !firstComparable.Equals(secondComparable))
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }

        public IEnumerable<ModelClientValidationRule>
        GetClientValidationRules(ModelMetadata metadata,
                                 ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("other", OtherProperty);
            rule.ValidationType = "greaterthan";
            yield return rule;
        }
    }

    //public class CorrectPriceValue : ValidationAttribute, IClientValidatable
    //{
    //    public CorrectPriceValue(string otherProperty)
    //        : base("Der Preis muss im Korrekten Format eingegeben werden '123,45'")
    //    {
    //        OtherProperty = otherProperty;
    //    }

    //    public string OtherProperty { get; set; }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, OtherProperty);
    //    }

    //    protected override ValidationResult
    //        IsValid(object firstValue, ValidationContext validationContext)
    //    {
    //        var firstComparable = firstValue as IComparable;

    //        if (firstComparable != null && secondComparable != null)
    //        {
    //            if (firstComparable.CompareTo(secondComparable) < 1 && !firstComparable.Equals(secondComparable))
    //            {
    //                return new ValidationResult(
    //                    FormatErrorMessage(validationContext.DisplayName));
    //            }
    //        }

    //        return ValidationResult.Success;
    //    }


    //    public IEnumerable<ModelClientValidationRule>
    //    GetClientValidationRules(ModelMetadata metadata,
    //                             ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule();
    //        rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
    //        rule.ValidationParameters.Add("other", OtherProperty);
    //        rule.ValidationType = "greaterthan";
    //        yield return rule;
    //    }
    //}


    //public class LessThan : ValidationAttribute, IClientValidatable
    //{
    //    public LessThan(string otherProperty)
    //        : base("{0} muss über {1} liegen")
    //    {
    //        OtherProperty = otherProperty;
    //    }

    //    public string OtherProperty { get; set; }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, OtherProperty);
    //    }

    //    protected override ValidationResult
    //        IsValid(object firstValue, ValidationContext validationContext)
    //    {
    //        var firstComparable = firstValue as IComparable;
    //        var secondComparable = GetSecondComparable(validationContext);

    //        if (firstComparable != null && secondComparable != null)
    //        {
    //            if (firstComparable.CompareTo(secondComparable) > 1)
    //            {
    //                return new ValidationResult(
    //                    FormatErrorMessage(validationContext.DisplayName));
    //            }
    //        }

    //        return ValidationResult.Success;
    //    }

    //    protected IComparable GetSecondComparable(
    //        ValidationContext validationContext)
    //    {
    //        var propertyInfo = validationContext
    //                              .ObjectType
    //                              .GetProperty(OtherProperty);
    //        if (propertyInfo != null)
    //        {
    //            var secondValue = propertyInfo.GetValue(
    //                validationContext.ObjectInstance, null);
    //            return secondValue as IComparable;
    //        }
    //        return null;
    //    }

    //    public IEnumerable<ModelClientValidationRule>
    //    GetClientValidationRules(ModelMetadata metadata,
    //                             ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule();
    //        rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
    //        rule.ValidationParameters.Add("other", OtherProperty);
    //        rule.ValidationType = "lessthan";
    //        yield return rule;
    //    }
    //}

}