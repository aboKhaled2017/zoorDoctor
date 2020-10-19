/// <reference path="../../../js/jquery.min.js" />
var currentLangauge;
var appFacebookIDLocal = '1327277757397665';
var appFacebookSecretLocal = '1267b51548a791c9fe1552d1ce3824ba';
var facebookVersion = 'v2.10';
var googleAppClientID = '1088445861484-296utpnbqsbv6aipghdko6p28eu3nkqr.apps.googleusercontent.com';
googleAppClientID = '807047009507-86thjjkclif9blpeq7asit8k1b44jigg.apps.googleusercontent.com';
var defaultPathForDoctorArea = "/doctor-site/";
var defaultPathAdvices = "/users_advices/";
var defaultPathSpecialities = "/All-specialities/";
var defaultPathSearch = "/Advanced-search/";
var defaultPathSpecialitiesGetData = "/All-specialities/specialities/";
var defaultPathPatients = "/user/";
var defaultPathForDoctorPage = "/doctor-page/";
var defaultPathUsers = "/";
var doctorPersonalImgPath = "/Areas/doctors/doctorImages/";
var defaultPathDoctorReservingPage = "/reserving/doctor/";
var defaultPathReservation = "/reserving";
var defaultPathAdvices = "/advices/";
var maxOfCardNumbersForSearchPage =9;//maximum number of cards that will be visible for one page
var numberOfPaginationLinks = 10;//number of links that will be show for pagination
var defaultPageSizeForComments = 5;
var defaultPageSizeForAdvices = 5;
var defaultPageSizeForPatientViews = 5;
var IsMainJsLoaded = false;
var onMainJsLoaded;
var gotoLoginPage =function () {
        window.open(defaultPathPatients + 'login', '_blank', '', '');
    }
$(function () {
    currentLangauge = $('body').attr('lang');
    $.ajaxSetup({
        cache: true
    });
    /*after loading secNav.js then load mainJs then load lnaguage js then load patient js*/
    var scriptLangName = (currentLangauge == "en") ? "en-US-Js.js" : "ar-EG-JS.js";
    var load = $.Deferred();
    load.done(loadLocalScript('/Areas/users/js/secNav.js'))
    .then(loadLocalScript('/Areas/users/js/' + scriptLangName))
    .then(loadLocalScript('/Areas/users/js/mainJS.js', function (isLoaded) {
            if (isLoaded) IsMainJsLoaded = true;
            onMainJsLoaded(IsMainJsLoaded);
        }))    
    .then(loadLocalScript('/Areas/users/js/patient.js'))
    //all required scripts has been loaded
});
function loadLocalScript(path,callBack)
{
    $.getScript(location.protocol + '//' + location.host + path, function () { callBack(true);})
    .fail(function (e) { callBack(false); });
}

