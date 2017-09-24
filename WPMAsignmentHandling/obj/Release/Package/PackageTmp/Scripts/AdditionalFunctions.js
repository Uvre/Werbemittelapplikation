function getTop(e) {
        var offset = e.offsetTop;
        if (e.offsetParent != null) offset += getTop(e.offsetParent);
        return offset;
    }
function getLeft(e) {
    var offset = e.offsetLeft;
    if (e.offsetParent != null) offset += getLeft(e.offsetParent);
    return offset;
}
function hideDivImageDisplay() {
    $('#PicPreview').hide();
}

function showDivImageDisplay(img) {
    var leftPos = getLeft(img) + 80;
    var topPos = getTop(img) + 20;
    $('#PicPreview').html("<img src='" + img.src + "'height='400px'/>");
    
    //$('#PicPreview').offset({ top: topPos, left: leftPos });
    $('#PicPreview').css({
        'position': 'absolute',
        'left': '50%',
        'top': '50%',
        'margin-left': -$('#PicPreview').outerWidth() / 2,
        'margin-top': -$('#PicPreview').outerHeight() / 2
    });
    $('#PicPreview').show();
    
}



function checkInputNumber(e) {
    var key = e.which || e.keyCode;
    if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
        // numbers   
        key >= 48 && key <= 57 ||
        // Numeric keypad
        key >= 96 && key <= 105 ||
        // comma, period and minus, . on keypad
        //key == 190 ||
        //key == 188 ||
        //key == 109 ||
        //key == 110 ||
        // Backspace and Tab and Enter
       key == 8 || key == 9 || //key == 13 ||
        // Home and End
       key == 35 || key == 36 ||
        // left and right arrows
       key == 37 || key == 39 ||
        // Del and Ins
       key == 46 || key == 45) {
        return;
    } else {
        e.preventDefault();
    }
}

function checkInputNumberComma(e) {
    var key = e.which || e.keyCode;
    if ((key == 110 || key == 188) && $('#' + e.target.id).val().indexOf(',') != -1) {
        e.preventDefault();
    }
    if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
        // numbers   
        key >= 48 && key <= 57 ||
        // Numeric keypad
        key >= 96 && key <= 105 ||
        // comma, period and minus, . on keypad
        //key == 190 || //period
        key == 188 || //period
        //key == 109 || //minus
        key == 110 ||
        // Backspace and Tab and Enter
       key == 8 || key == 9 || //key == 13 ||
        // Home and End
       key == 35 || key == 36 ||
        // left and right arrows
       key == 37 || key == 39 ||
        // Del and Ins
       key == 46 || key == 45) {
        return;
    } else {
        e.preventDefault();
    }
}


function CheckDecimalDigits(number) {
    var DDigits = number.split(",")
    if (DDigits[1].length != 2) {
        return false;
    }
    return true;
}

function BestelldatumEinlesen(dialog) {
    var startIndex = dialog.indexOf("Werbemittel Online-Bestellung vom ");
    if (startIndex == -1) {
        return false;
    }
    var endIndex = dialog.indexOf("\n");
    var Bestelldatum = dialog.substring(startIndex+34, startIndex+44);
    return Bestelldatum;
};

function KennzeichnungEinlesen(dialog) {
    var startIndex = dialog.indexOf("BESTELLNR:");
    if (startIndex == -1) {
        return "false";
    }
    var endIndex = dialog.length;
    var Linefull = dialog.substring(startIndex, endIndex);
    endIndex = Linefull.indexOf("\n");
    var Line = Linefull.substring(0, endIndex);
    var Line = Line.replace(" ", "");
    var Kennzeichnung = Line.replace("BESTELLNR:", "");
    $.ajax({
        url: "/Werbemittelverwaltung/KennzeichenCheck",
        type: 'Post',
        async: false,
        cache: false,
        timeout: 30000,
        data: { Kennzeichen: Kennzeichnung },
        error: function () {
        },
        success: function (msg) {
            if (!msg) {
            } else {
                Kennzeichnung= "exists";
            }
        }
    });
    return Kennzeichnung;
}


function MesseEinlesen(e) {
    var WMABestellung = e;
    var startIndex = WMABestellung.indexOf("MESSE:");
    var endIndex = WMABestellung.length;
    var MessenameLinefull = WMABestellung.substring(startIndex, endIndex);
    endIndex = MessenameLinefull.indexOf("\n");
    var MessenameLine = MessenameLinefull.substring(0, endIndex);
    var Messename = MessenameLine.replace("MESSE: ", "");
    $.ajax({
        url: "/Werbemittelverwaltung/MesseLookUP",
        type: 'Post',
        async: false,
        cache: false,
        timeout: 30000,
        data: { Messename: Messename },
        error: function () {
        },
        success: function (msg) {
            if (!msg) {
                Messename = false;
            }
        }
    });
    return Messename;
}



function KundeEinlesen(e) {
    var WMABestellung = e;
    var startIndex = -1;
    var endIndex = -1;
    var LineFull = "";
    var Line = "";
    var Halle = "ka";
    var Stand = "ka";
    var TelLine = "ka";
    var Tel = "ka";
    var PLZLine = "";
    var PLZ = -1;
    var ORTuLand = "ka";
    var Ort = "ka";
    var CountryCode = "ka";
    var Strasse = "ka";
    var Lines = 0;
    var Firma = "ka";
    var Ansprechpartner = "ka";
    var Zusatz = "ka";
    var Email = "ka";
    var land = "";


    function AdressDatenEinlesen(Text) {
        startIndex = -1;
        endIndex = -1;
        LineFull = "";
        Line = "";
        Halle = "";
        Stand = "";
        TelLine = "";
        Tel = "";
        PLZLine = "";
        PLZ = -1;
        ORTuLand = "";
        Ort = "";
        CountryCode = "";
        Strasse = "";
        Lines = 0;
        Firma = "";
        Ansprechpartner = "";
        Zusatz = "";
        Email = "";
        land = "";

        Line = Text;
        //E-Mail Adresse ermitteln
        if (Line.indexOf("@") > -1) {
            Line = Line.replace("\n\n", "")
            startIndex = Line.lastIndexOf("\n");
            endIndex = Line.length;
            Email = Line.substring(startIndex, endIndex);
            Line = Line.replace(Email, "");
        }

        // Halle/Stand Ermitteln
        if (Line.lastIndexOf("Halle/Stand:") > -1) {
            startIndex = Line.lastIndexOf("Halle/Stand:");
            endIndex = Line.length;
            var HalleUStand = Line.substring(startIndex, endIndex).replace(" ", "");
            Line = Line.replace(Line.substring(startIndex, endIndex), "");
            startIndex = HalleUStand.indexOf(":") + 1;
            endIndex = HalleUStand.length;
            Stand = HalleUStand.substring(startIndex, endIndex);
            var end = Stand.replace(/\D/, "#").indexOf("#");
            Halle = Stand.substring(0, end);
            Line = Line.replace(HalleUStand, "");
        }

        //Tel ermitteln
        if (Line.indexOf("Tel") > -1) {
            startIndex = Line.indexOf("Tel");
            endIndex = Line.length;
            TelLine = Line.substring(startIndex, endIndex);
            Tel = TelLine.replace("Tel.: ", "");
            Line = Line.replace(TelLine, "");
            Line = Line.substring(0, Line.length - 1)
        }

        //ORT Land PLZ ermitteln
        startIndex = Line.lastIndexOf("\n");
        endIndex = Line.length;
        PLZLine = Line.substring(startIndex, endIndex);
        Line = Line.replace(PLZLine, "");
        //PLZ
        PLZ = PLZLine.replace(/\D/g, "");
        //Ort und Land
        ORTuLand = PLZLine.replace(/\d/g, "");
        ORTuLand = ORTuLand.replace(/\n/g, "");
        ORTuLand = ORTuLand.replace(/\s+/g, "");
        ORTuLand = ORTuLand.replace("-", "");
        //startIndex = ORTuLand.indexOf(" ") + 1;
        Ort = ORTuLand.substring(2, ORTuLand.length)
        CountryCode = ORTuLand.substring(0, 2);
        $.ajax({
            url: "/Werbemittelverwaltung/LandLookUP",
            type: 'Post',
            async: false,
            cache: false,
            timeout: 30000,
            data: { cc: CountryCode },
            error: function () {
            },
            success: function (Land) {
                land = Land;
            }
        });

        //Strasse und Namen einlesen
        startIndex = Line.lastIndexOf("\n");
        endIndex = Line.length;
        Strasse = Line.substring(startIndex, endIndex);
        Line = Line.replace(Strasse, "");
        Strasse = Strasse.replace(/\n/g, "");
        Lines = Line.length - Line.replace(/\n/g, "").length + 1;
        if (Lines > 3) {
            alert("Zuviele Zeilen in der Adressangabe. Adresse muss manuell angelegt werden")
        } else {
            if (Lines == 1) {
                Firma = Line.substring(0, endIndex);
            }
            if (Lines == 2) {
                endIndex = Line.indexOf("\n") + 1;
                Firma = Line.substring(0, endIndex);
                Line = Line.replace(Firma, "");
                Firma = Firma.replace(/\n/g, "");
                endIndex = Line.length;
                Ansprechpartner = Line.substring(0, endIndex);
                Line = Line.replace(Zusatz, "");
            }
            if (Lines == 3) {
                
                endIndex = Line.indexOf("\n") + 1;
                Firma = Line.substring(0, endIndex);
                Line = Line.replace(Firma, "");
                Firma = Firma.replace(/\n/g, "");
                endIndex = Line.indexOf("\n") + 1;
                Ansprechpartner = Line.substring(0, endIndex);
                Line = Line.replace(Ansprechpartner, "");
                Zusatz = Line.substring(0, Line.length);
                Zusatz.replace(/\n/g, "");
                Line = Line.replace(Zusatz, "");
            }
        }
    }

    //Auftraggeber Auslesen
    startIndex = WMABestellung.indexOf("BESTELLT VON:");
    endIndex = WMABestellung.length;
    Linefull = WMABestellung.substring(startIndex, endIndex - 1);
    endIndex = Linefull.indexOf("\n\n");
    Line = Linefull.substring(0, endIndex);
    startIndex = Line.indexOf("\n");
    endIndex = Line.length;
    Line = Line.substring(startIndex + 1, endIndex);
    if (Line.indexOf("Ausstelleradresse") != -1) {
        AustellerEinlesen();
        AuftraggeberEinlesen();
        $('#AuButtons').hide();
        $('#AuWie').show();
        $('#AuStatus').val( 'Auftraggeber');
    } else {
        AdressDatenEinlesen(Line);
        AuftraggeberEinlesen();
        AustellerEinlesen(); 
            $('#AuButtons').hide();
            $('#AustelleradresseAendern').show();
            $('#AuStatus').val( 'true');
            $('#Austelleradresse_Name').val(Firma);
            $('#Austelleradresse_Name2').val(Ansprechpartner);
            $('#Austelleradresse_Name3').val(Zusatz);
            $('#Austelleradresse_Strasse').val(Strasse);
            $('#Austelleradresse_PLZ').val(PLZ);
            $('#Austelleradresse_Ort').val(Ort);
            $('#AustellerLand').val(land);
            $('#Austelleradresse_Telefon').val(Tel);
            $('#Austelleradresse_EMail').val(Email);
    }
    
    HalleUStandEintragen(Halle, Stand);
    LieferadresseEinlesen();
    RechnungsadresseEinlesen();
    BeiRueckfragenEinlesen();


    function AuftraggeberEinlesen() {
        $.ajax({
            url: "/Werbemittelverwaltung/KundeLookUP",
            type: 'Post',
            async: false,
            cache: false,
            timeout: 30000,
            data: { kundenname: Firma, Name2: Ansprechpartner, Name3:Zusatz },
            error: function () {
            },
            success: function (msg) {
                if (msg == 0) {
                } else {
                    $('#Kunde_KundeID').val(msg);
                    $('#Kunde_Erstellungsdatum').val(Date.Now);
                }
                $('#SBKunde').val('');
                $('#message').hide();
                $('#Kunde_Name').val(Firma);
                $('#Auftraggeberadresse_Name').val(Firma);
                $('#Auftraggeberadresse_Name2').val(Ansprechpartner);
                $('#Auftraggeberadresse_Name3').val(Zusatz);
                $('#Auftraggeberadresse_Strasse').val(Strasse);
                $('#Auftraggeberadresse_PLZ').val(PLZ);
                $('#Auftraggeberadresse_Ort').val(Ort);
                $('#AuftraggeberLand').val(land);
                $('#Auftraggeberadresse_Telefon').val(Tel);
                $('#Auftraggeberadresse_EMail').val(Email);
                $('#Auftraggeberadresse_Name').show();
                $('#Auftraggeberadresse_Name2').show();
                $('#Auftraggeberadresse_Name3').show();
                $('#Auftraggeberadresse_Strasse').show();
                $('#Auftraggeberadresse_PLZ').show();
                $('#Auftraggeberadresse_Ort').show();
                $('#AuftraggeberLand').show();
                $('#Auftraggeberadresse_Telefon').show();
                $('#Auftraggeberadresse_EMail').show();
            }
        });
    };

    function AustellerEinlesen() {
        startIndex = WMABestellung.indexOf("AUSSTELLER:");
        endIndex = WMABestellung.length;
        Linefull = WMABestellung.substring(startIndex, endIndex - 1);
        endIndex = Linefull.indexOf("\n\n");
        Line = Linefull.substring(0, endIndex);
        startIndex = Line.indexOf("\n");
        endIndex = Line.length;
        Line = Line.substring(startIndex + 1, endIndex);
        AdressDatenEinlesen(Line);
        
    };

    function LieferadresseEinlesen() {
        startIndex = WMABestellung.indexOf("LIEFERADRESSE:");
        endIndex = WMABestellung.length;
        Linefull = WMABestellung.substring(startIndex, endIndex - 1);
        endIndex = Linefull.indexOf("\n\n");
        Line = Linefull.substring(0, endIndex);
        startIndex = Line.indexOf("\n");
        endIndex = Line.length;
        Line = Line.substring(startIndex + 1, endIndex);
        if (Line.indexOf("Ausstelleradresse") != -1) {
            $('#LiButtons').hide();
            $('#LiWie').show();
            $('#LiStatus').val( 'Auftraggeber');
        } else {
            AdressDatenEinlesen(Line);
            $('#LiButtons').hide();
            $('#LieferadresseAendern').show();
            $('#LiStatus').val( 'true');
            $('#Lieferadresse_Name').val(Firma);
            $('#Lieferadresse_Name2').val(Zusatz);
            $('#Lieferadresse_Name3').val(Ansprechpartner);
            $('#Lieferadresse_Strasse').val(Strasse);
            $('#Lieferadresse_PLZ').val(PLZ);
            $('#Lieferadresse_Ort').val(Ort);
            $('#LieferLand').val(land);
            $('#Lieferadresse_Telefon').val(Tel);
            $('#Lieferadresse_EMail').val(Email);
        }
    };


    function RechnungsadresseEinlesen() {
        startIndex = WMABestellung.indexOf("AUSSTELLER-RECHNUNGSADRESSE:");
        endIndex = WMABestellung.length;
        Linefull = WMABestellung.substring(startIndex, endIndex - 1);
        endIndex = Linefull.indexOf("\n\n");
        Line = Linefull.substring(0, endIndex);
        startIndex = Line.indexOf("\n");
        endIndex = Line.length;
        Line = Line.substring(startIndex + 1, endIndex);
        if (Line.indexOf("Ausstelleradresse") != -1) {
            $('#ReButtons').hide();
            $('#ReWie').show();
            $('#ReStatus').val( 'Auftraggeber');
        } else {
            AdressDatenEinlesen(Line);
            $('#ReButtons').hide();
            $('#RechnungsadresseAendern').show();
            $('#ReStatus').val( 'Eingegeben');
            $('#Rechnungsadresse_Name').val(Firma);
            $('#Rechnungsadresse_Name2').val(Ansprechpartner);
            $('#Rechnungsadresse_Name3').val(Zusatz);
            $('#Rechnungsadresse_Strasse').val(Strasse);
            $('#Rechnungsadresse_PLZ').val(PLZ);
            $('#Rechnungsadresse_Ort').val(Ort);
            $('#RechnungLand').val(land);
            $('#Rechnungsadresse_Telefon').val(Tel);
            $('#Rechnungsadresse_EMail').val(Email);
        }
    };

    function BeiRueckfragenEinlesen() {
        startIndex = WMABestellung.indexOf("CKFRAGEN:");
        endIndex = WMABestellung.length;
        Linefull = WMABestellung.substring(startIndex, endIndex - 1);
        endIndex = Linefull.indexOf("\n\n");
        Line = Linefull.substring(0, endIndex);
        startIndex = Line.indexOf("\n");
        endIndex = Line.length;
        Line = Line.substring(startIndex + 1, endIndex);
        AdressDatenEinlesen(Line);
        if (Tel.length > 0) {
            $('#Auftraggeberadresse_Telefon').val(Tel);
        }
        if (Email.length > 0) {
            Email = Email.replace("E-Mail: ", "");
            $('#Auftraggeberadresse_EMail').val(Email);
        }
    }


    

    return "";
}

function HalleUStandEintragen(Halle, Stand) {
    var halle = Halle;
    var stand = Stand;
    $('#Halle').val(halle);
    $('#Stand').val(stand);
    if (halle == '1') {
        $('#Hallehidden').val('L-Bank Forum (Halle 1) ');
    }
    if (halle == '6') {
        $('#Hallehidden').val('Oskar Lapp Halle (Halle 6) ');
    }
    if (halle == '9') {
        $('#Hallehidden').val('Alfred Kärcher Halle (Halle 9) ');
    }
    if (halle != '1' && halle != '6' && halle != '9') {
        if (Halle.length == 0) {
            $('#Hallehidden').val('');
        } else {
            $('#Hallehidden').val('Halle ' + halle + ' ');
        }
    }

    $('#Standhidden').val('Stand ' + stand);
    $('#Eindruck').text($('#Hallehidden').val() + $('#Standhidden').val());
    $('#HalleUStand').val($('#Hallehidden').val() + $('#Standhidden').val());
}

function DataMatrixKundeEinlesen(dialog) {
    HalleUStandEintragen("-", "-");
    var DataText = dialog;
    endindex = DataText.indexOf(";");
    var KundenName = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var Name2 = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var Name3 = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var Strasse = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var PLZ = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var Ort = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    endindex = DataText.indexOf(";");
    var Land = DataText.substring(0, endindex);
    DataText = DataText.substring(endindex + 1, DataText.length);
    if (Land.length == 0) {
        Land = "Deutschland";
    }

    $.ajax({
        url: "/Werbemittelverwaltung/LandLookUPName",
        type: 'Post',
        async: false,
        cache: false,
        timeout: 30000,
        data: { Name: Land },
        error: function () {
        },
        success: function (land) {
            if (land.length < 1) {
                $('#AuLandErr').show();
            }
            $('#AuftraggeberLand').val(land);
        }
    });

    $('#message').hide();
    $('#Kunde_Name').val(KundenName);
    $('#Auftraggeberadresse_Name').val(KundenName);
    $('#Auftraggeberadresse_Name2').val(Name2);
    $('#Auftraggeberadresse_Name3').val(Name3);
    $('#Auftraggeberadresse_Strasse').val(Strasse);
    $('#Auftraggeberadresse_PLZ').val(PLZ);
    $('#Auftraggeberadresse_Ort').val(Ort);
    $('#Auftraggeberadresse_Name').show();
    $('#Auftraggeberadresse_Name2').show();
    $('#Auftraggeberadresse_Name3').show();
    $('#Auftraggeberadresse_Strasse').show();
    $('#Auftraggeberadresse_PLZ').show();
    $('#Auftraggeberadresse_Ort').show();
    $('#AuftraggeberLand').show();
    $('#Auftraggeberadresse_Telefon').show();
    $('#Auftraggeberadresse_EMail').show();

    $('#LiButtons').hide();
    $('#LiWie').show();
    $('#LiStatus').val( 'Auftraggeber');

    $('#ReButtons').hide();
    $('#ReWie').show();
    $('#ReStatus').val( 'Auftraggeber');

    $('#AuButtons').hide();
    $('#AuWie').show();
    $('#AuStatus').val( 'Auftraggeber');
};





function SecurityQuestion(Text) {
    $('<div></div>').appendTo('body').html('<div><h3 style="margin-top:50PX;text-align:center">'+Text+'</h2></div>')
            .dialog({
                autoOpen: true,
                width: 700,
                height: 250,
                resizable: true,
                title: 'Sicherheitsabfrage',
                buttons: {
                    'Ja': function (e) {
                        $(this).dialog('close');
                        return "true";
                    },
                    'Nein': function (e) {
                        $(this).dialog('close');
                        return false;
                    }
                }
     });
}

function setTableHeaderSize() {
    var lengthCombined = 0;
    var longestColumnID = 0;
    var longestWidth = 0;
    for (i = 0; i < ($(".AuflistungTabelle th").length) ; i++) {
        var HeadWidth = $(".AuflistungTabelle tr:eq(0) th:eq(" + i + ")").outerWidth();
        var BottomWidth = $(".AuflistungTabelle tr:eq(1) td:eq(" + i + ")").outerWidth();
        if (HeadWidth > BottomWidth) {
            lengthCombined += HeadWidth
            if (HeadWidth > longestWidth) {
                longestColumnID = i;
                longestWidth = HeadWidth;
            }
        }else{
            lengthCombined += BottomWidth
            if (BottomWidth > longestWidth) {
                longestColumnID = i;
                longestWidth = BottomWidth;
            }
        }
    }

    if (lengthCombined > ($(".AuflistungTabelle").width() - 5)) {
        $(".AuflistungTabelle tr:eq(1) td:eq(" + longestColumnID + ")").width($(".AuflistungTabelle tr:eq(1) td:eq(" + longestColumnID + ")").width() + $(".AuflistungTabelle").outerWidth() - lengthCombined - 200);
    }

    for (i = 0; i < $(".AuflistungTabelle th").length; i++) {
        var HeadWidth = $(".AuflistungTabelle tr:eq(0) th:eq(" + i + ")").outerWidth();
        var BottomWidth = $(".AuflistungTabelle tr:eq(1) td:eq(" + i + ")").outerWidth();
        if (HeadWidth > BottomWidth) {
            $(".AuflistungTabelle tr:eq(1) td:eq(" + i + ")").outerWidth(HeadWidth);
        } else {
            $(".AuflistungTabelle tr:eq(0) th:eq(" + i + ")").outerWidth(BottomWidth);
        }
    }
}

function hideLoading() {
    $('#DivLoading').hide();
}

function showLoading() {
    $('#DivLoading').show();
}
