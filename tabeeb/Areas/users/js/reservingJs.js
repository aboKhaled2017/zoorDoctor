/// <reference path="mainPage.js" />
/// <reference path="../../../Scripts/jquery-1.12.4.min.js" />
/// <reference path="en-US-Js.js" />
/// <reference path="mainJS.js" />
var translateData = {};
var currentMediaValue = mediaCase();  
intialTranslateDataObject();
function mediaCase()
{
    var max360Media = window.matchMedia("only screen and (max-width: 360px)").matches;
    var max499Media = window.matchMedia("only screen and (min-width:361px )and (max-width:499px)").matches;
    var max767Media = window.matchMedia("only screen and  (min-width:500px )and (max-width:767px)").matches;
    if (max360Media) return 1;
    if (max499Media) return 2;
    if (max767Media) return 3;
    return 0;
}
function handleReservingTableDesgin() {
    var tablesDiv = $('.appointementReserving .reservingTables');
    translateData.maxNavigator = tablesDiv.data('maxnavigator');
    var tableMargin = $('.appointementReserving .reservingTables table').css('marginTop').replace('px', '');
    if (currentMediaValue == 0)
        tablesDiv.css('height', ((2 * $('.tables table').height()) + (2 * tablesDiv.get(0).clientTop) + (4 * tableMargin)));
    else
        tablesDiv.css('height', ((1 * $('.tables table').height()) + (2 * tablesDiv.get(0).clientTop) + (2 * tableMargin)));
    $('body').on({
        'click': function () {//on appointement selected
            var selectedInterval = $(this); 
            $(selectedInterval).toggleClass('selectedAppointement').parent().siblings().find('td').removeClass('selectedAppointement');
        },
        'mouseenter': function () { $(this).addClass('hoveredAppointement'); },
        'mouseleave': function () {$(this).removeClass('hoveredAppointement'); }
    }, '.reservingTables table tbody td');
}
function intialTranslateDataObject() {
    var navigatorValue = 0;
    var table = $('.appointementReserving .reservingTables table');
    var tableHeight = table.height() + (2 * parseInt(table.css('marginTop').replace('px', '')));
    translateData.navigator = navigatorValue;
    translateData.tableHeight = tableHeight;
    translateData.translatedValue = 0;    
    var totalTables = $('.appointementReserving .reservingTables .tables').data('totaltables');
    var rowOfTablesCount = 3;
    if (currentMediaValue == 1 || currentMediaValue == 2) rowOfTablesCount = 1;
    if (currentMediaValue == 3) rowOfTablesCount = 2;
    translateData.maxNavigator = parseInt(Math.ceil(totalTables / rowOfTablesCount));
}
function translateUp(upBtn)
{
    var diffValue = (currentMediaValue == 0) ? 3 :2; 
    if (translateData.navigator > translateData.maxNavigator - diffValue) return;
    var translatedDiv = $('.appointementReserving .reservingTables .tables');
    translateData.translatedValue -= translateData.tableHeight;
    translatedDiv.css('transform', 'translateY(' + translateData.translatedValue + 'px)');
    translateData.navigator += 1;    
}
function translateDown(downBtn) {
    if (translateData.navigator<1) return;
    var translatedDiv = $('.appointementReserving .reservingTables .tables');
    translateData.translatedValue += translateData.tableHeight;
    translatedDiv.css('transform', 'translateY(' + translateData.translatedValue + 'px)');
    translateData.navigator -= 1;    
}
function activeDoctorImageAsLink()
{
    var id = sessionStorage.getItem('doctorID');
    $('.doc-card img')
        .css('cursor', 'pointer')
        .click(function(){location.href=defaultPathForDoctorPage +id;});
}
function mapAppointementsToTables()
{
    var tables = $('.appointementReserving .reservingTables table');
    tables.each(function (i, tb) {
        tb=$(tb);
        var tbody = tb.find('tbody');
        var appointements = tbody.data('value');  
        for (var i = 0; i < appointements.length; i++) {
            var newAppointement = $('<tr></tr>');
            var td = $("<td></td>");
            td.data('interval', appointements[i]).text(getAppointementString(appointements[i]));           
            newAppointement.append(td);
            tbody.append(newAppointement);
        }
        if(appointements.length==0)
        {
            var div = $("<div class='noAppointeFounded'></div>");
            var innerText = $("<div class='innerText'>"+language.noAvailbalAppointements+"</div>");
            div.append(innerText);
            div.height(tbody.height());
            tbody.append(div);
            tb.find('tfoot .bookNow').addClass('disabledBooking').data('active', false);
        }
    }); 
    $('table .bookNow').click(function () { onBooking(this);});
    $('#loadAppointements').addClass('hidden');
}
function onBooking(contextButton)
{
    contextButton = $(contextButton);
    if (contextButton.data('active') != undefined) return;
    var selectedAppointement = contextButton.parents('table').find('.selectedAppointement').eq(0);
    if (selectedAppointement.length != 1) {
        alert(language.pleaseSelectAppointement);
        return;
    }
    var appointementID = selectedAppointement.parents('tbody').data('id');
    var interval = selectedAppointement.data('interval');
    var doctorID = sessionStorage.getItem('doctorID');
    var appointementDate = contextButton.parents('table').data('appointementdate'); 
    if (doctorID == undefined || interval == undefined || appointementID == undefined||appointementDate==undefined) return;
    var isAuthenticated = localStorage.getItem('isAuthenticated');
    if(isAuthenticated=="false")
    {
        if (confirm(language.loginFirst))
            gotoLoginPage();
        return;
    } 
    var bookingForm = $('#bookingForm');
    bookingForm.find('input#docID').val(doctorID);
    bookingForm.find('input#appointementDate').val(appointementDate);
    bookingForm.find('input#interval').val(JSON.stringify(interval));
    bookingForm.modal('show');
}
function getAppointementString(appointement)
{
    var convertArTime=function(time)
    {
        if(currentLangauge=="en")return time;
        return time.replace(/am/ig,"صباحا").replace(/pm/ig,"مساءا");
    }
    if(appointement.from==undefined)
    {
        return convertArTime(appointement);
    }
    return language.fromTime+" "+convertArTime(appointement.from)+" "+language.toTime+" "+convertArTime(appointement.to);
}

