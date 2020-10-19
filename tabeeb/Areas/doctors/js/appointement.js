/// <reference path="doctorJS.js" />
/// <reference path="../../../scripts/jquery-1.12.4.js" />
/// <reference path="extensionFunctions.js" />
var currentAppointementType = true;
var currentAppointementParentDiv;
$(function () {
    /*functions of appolintement page*/
    $('#scedualedNumber').blur(function () {
        if (!this.checkValidity())
            $(this).val(5);
        if ($(this).val().trim() == "") $(this).val(5);
    });
    $('.AppointSetting .form-group .type input[type="radio"]').change(function () {
        if (!confirm(language.areYourSureToChangeBookingType)) return false;
        if ($(this).parent().is(':first-of-type')) {
            $('.AppointSetting .prescribed').fadeOut();
            $('.AppointSetting .priority').fadeIn(1000);
        }
        else {
            $('.AppointSetting .priority').fadeOut();
            $('.AppointSetting .prescribed').fadeIn(1000);
        }
        var formgroupParent = $(this).parents('.form-group');
        formgroupParent.find('.loading-circle').removeClass('hidden');
        var successMessage = (lang == "en")
        ? "your booking type has been changed successfully"
        : "لقد تم تغير نوع الحجز بنجاح";
        $.post('/' + defaultPathForDoctorArea + '/chnageBookingType', {}, function (result, status) {
            formgroupParent.find('.loading-circle').addClass('hidden');
            if (result.status) {
                setCurrentParentAppointement(getCurrentbookingType());
                alert(language.bookingTypeChangedSuccess);
            }
            else {
                alert(operationProblem);
            }
        });
    });
    $('body').on('click', '.appointementRecord .periods>.oneTimePeriod>i.fa-close', function () {
        //this is div that will be removed
        var removedPeriodHtml = $(this).parent();
        //remove parent div
        $(removedPeriodHtml).fadeOut(1000, function () {   //this button that will svae value to server
            var saveButton = $(this).parent().next();
            //periods of current save button that will be reduced by removed period
            var totalPeriods = saveButton.data('periods');
            //get removed period from htmlDiv of period
            var removePeriod = removedPeriodHtml.data('period'); 
            totalPeriods = $(totalPeriods).removePeriod(removePeriod);
            saveButton.data('periods', totalPeriods);
            var periodsButtons = (currentAppointementType == true)
            ? $('.priority.appointementRecord .periods>.oneTimePeriod')
            : $('.prescribed .appointementRecord .periods>.oneTimePeriod');
            //if the removed appointement was the default appointement
            if ($(removedPeriodHtml).parents('.appointementRecord').is(':first-child')) {
                periodsButtons.each(function (i, timePeriod) {
                    timePeriod = $(timePeriod);
                    var periodObject = timePeriod.data('period');
                    if (equalPeriod(removePeriod, periodObject)) {
                        //this is button foreach appointement that contain array of periods
                        var saveButton = timePeriod.parents('.periods').siblings('.save');
                        var totalReriodsOfCurrentAppointement = saveButton.data('periods');
                        //set new period records after delete to saveButton
                        saveButton.data('periods', $(totalReriodsOfCurrentAppointement).removePeriod(removePeriod));
                        //remove html dive of period
                        timePeriod.remove();
                    }
                });
            }
            else {
                $(removedPeriodHtml).remove();
            }


        })
    });
});
function setCurrentParentAppointement(type)
{
       currentAppointementParentDiv = (type)
       ?$('.priority')
       : $('.prescribed');
       currentAppointementType = type;

}
function getCurrentbookingType()
{
    return $("input[name='booking']").serializeArray()[0].value;
}
/*start appointement page functions*/
/*these are appointement page javascript function--------------------------*/
/*this function change number of appointements 
that will be scedualed by doctor*/
function changeAppointementScedualing(context) {
    //ponus one is the default one
    var scedualValue = parseInt($('#scedualedNumber').val());
    var oldValue = parseInt($('#scedualedNumber').data('prevvalue'));
    if (oldValue == scedualValue) return;
    if (!confirm(language.changeScedualedNumber(oldValue,scedualValue))) return;
    //show loading icon
    toggleLoadingIcon(context);
    $.post("/" + defaultPathForDoctorArea + "/changeScedualedDaysNumber", { daysNumber: parseInt(scedualValue) },
        function (result, status) {
            //hide loading icon
            toggleLoadingIcon(context);
            if (result.status) {
                if (oldValue > scedualValue) {
                    removeLastAppointementRecords(oldValue - scedualValue);
                }
                else {                   
                    var addedDates = result.data;
                    priorityParentAppointementsDiv = $('.priority');
                    prescribedParentAppointementsDiv = $('.prescribed');
                    for (var i = 0; i < addedDates.length; i++) {
                        addnewEmptyAppointementRecord(addedDates[i], priorityParentAppointementsDiv);
                        addnewEmptyAppointementRecord(addedDates[i], prescribedParentAppointementsDiv);
                    }
                }
                $('#scedualedNumber').data('prevvalue', scedualValue)
            }
            else {
                alertServerOerationProblem();
            }
        });
}
/*this function add new empty appointement record
to be added appointements record list
*/
function addnewEmptyAppointementRecord(newDate, targetParentAppointementDiv) {
    //this is appointement record that will be returned
    var newAppointementRecord;
    //this is target parent that will contain new added record
    targetParentAppointementDiv = $(targetParentAppointementDiv);
    var defaultAppointementrecord=targetParentAppointementDiv.find('.appointementRecord').eq(0);
    newAppointementRecord = defaultAppointementrecord.clone();
    //if default was opend then close the inheirted one
    newAppointementRecord.find('.appointementButton').next().removeClass('w3-show');
    //newAppointementRecord.find('.periods').empty();
    var buttondata = {};
    //intialize button data
    buttondata.periods = [];
    buttondata.appointementdate = newDate;
    newAppointementRecord.find('.save').data(buttondata);
    //to clone periods data from default one
    cloneDataOfPeriods(defaultAppointementrecord.find('.periods'), newAppointementRecord.find('.appointementButton'));
    var buttonAccourdian = newAppointementRecord.find('.appointementButton');
    buttonAccourdian.text(language.addAppointementOfDay+" "+ newDate);
    lastdayNumber = targetParentAppointementDiv.find('.appointementRecord').last().find('.appointementButton').data('daynumber');
    buttonAccourdian.data('daynumber', (parseInt(lastdayNumber) + 1));
    buttonAccourdian.attr('data-daynumber', (parseInt(lastdayNumber) + 1));
    targetParentAppointementDiv.find('.appointementRecords').append(newAppointementRecord);
}
function cloneDataOfPeriods(sourcePeriods,targetButton)
{
    sourcePeriods = $(sourcePeriods);
    targetButton = $(targetButton);
    targetButton.next().find('.periods').empty();
    var allPeriodsData = [];
    sourcePeriods.find('.oneTimePeriod').each(function (i, periodDiv) {
        allPeriodsData.push($(periodDiv).data('period'));
    }); 
    addNewAppointementPeriodOrtime(allPeriodsData, targetButton);
}
/*
this function remove the last num number of appointements records
*/
function removeLastAppointementRecords(num) {
    for (var i = 0; i < num; i++) {
        $('.priority').find('.appointementRecord').last().remove();
        $('.prescribed').find('.appointementRecord').last().remove();
    }
}
/*
this fynction is called after page is loaded
this function get string that repersent all appointements
periods and convert it to json object
in order to operate on it
[defaultAppointement:this is default appointement of doctor for all days]
[allAppointements:these are doctor appointements for different days]
[type(0|1):0=>this is perioriy based appoint,1=>this is prescriped time based appoint]
*/
function mapPeriodsIntervalsToFields(defaultAppointement, allAppointements, type) {

    var currentBookingType = getCurrentbookingType(); 
    currentBookingType = (currentBookingType == "false") ? false : true;
    //set the global variable of current type
    setCurrentParentAppointement(currentBookingType);
    //these are default appointements for all dayes.
    defaultAppointement = defaultAppointement.replace(/&quot;/gi, "\"");
    defaultAppointement = JSON.parse(defaultAppointement);
    //these are different appointements for particulars days
    allAppointements = allAppointements.replace(/&quot;/gi, "\"");
    allAppointements = JSON.parse(allAppointements); 
    //intialize all buttons data
    $('button.appointementButton').next().find('.save').each(function(i,saveButton){
        $(saveButton).data('periods', []);
    });
    $(currentAppointementParentDiv).find('button.appointementButton').each(function (i, button) {
        button = $(button);
        var dayNumber = parseInt(button.data('daynumber'));
        //there are appointements exists
        if (allAppointements[dayNumber] != undefined) {
            addNewAppointementPeriodOrtime(allAppointements[dayNumber], button);
        }
        else {
            //if there are default appointement exists
            if (defaultAppointement.length > 0) {
                //add default appointement for every buuton
                 addNewAppointementPeriodOrtime(defaultAppointement, button);
            }
        }
    });

}
/*
this function take array of time interval and push them as html divisions to target button
based on priority
*/
function addPeriodsToAppointementRecordAsHtml(periods, targetButton) {
    targetButton = $(targetButton);
    var saveButton = targetButton.next().find('.save');
    for (var i = 0; i < periods.length; i++) {
        var currentPeriodFrom = (lang == "en") ? periods[i].from : periods[i].from.replace(/AM/i, 'ص').replace(/PM/i, 'م');
        var currentPeriodTo = (lang == "en") ? periods[i].to : periods[i].to.replace(/AM/i, 'ص').replace(/PM/i, 'م');
        var divPeriod = $("<div class='oneTimePeriod'>" + language.from + " " + currentPeriodFrom + " " + language.to + " " + currentPeriodTo + " <i class='fa fa-close'></i></div>");
        divPeriod.data('period', periods[i]);
        targetButton.next().find('.periods').append(divPeriod);
        saveButton.data('periods').push(periods[i]);
    }
}
/*
this function take array of periods and push them as html divisions to target button
pased on prescriped time
*/
function addNewTimeAppointementRecordAsHtml(periods, targetButton) {
    var saveButton = targetButton.next().find('.save');
    for (var i = 0; i < periods.length; i++) {
        var divPeriod = $("<div class='oneTimePeriod'>" + periods[i] + " <i class='fa fa-close'></i></div>");
        divPeriod.data('period', periods[i]);
        targetButton.next().find('.periods').append(divPeriod);
        saveButton.data('periods').push(periods[i]);
    }
}
/*
this function switch between two functions to to called
on adding new appointement period interval or time 
based on current booking type
*/
function addNewAppointementPeriodOrtime(data,targetDiv)
{
    if (currentAppointementType)
        addPeriodsToAppointementRecordAsHtml(data, targetDiv);
    else
        addNewTimeAppointementRecordAsHtml(data, targetDiv);
}
/*this function show accordion content when clicking on accordian button*/
function showAccordianContent(context)//doctor
{
    $(context).next().toggleClass("w3-show").parents().siblings().find('.w3-show').removeClass('w3-show');;
    $(context).toggleClass("w3-red");
}
/*
this function take new period(from,to)and check if it is valid
*/
function validateIntervalPeriods(fromValue, toValue, oldAppointementsPeriods) {
    var isValid = true;
    if (toValue != null) {//this is first type that is period interval[from,to]
        if (fromValue > toValue) { alert(language.endPeriodMustExceedBeginPeriod); return false; }
        //check if current these values[from,to] confuse with old periods
        //that already has been selected
        $(oldAppointementsPeriods).each(function (i, period) {           
            if (
                   (fromValue > period.from && fromValue < period.to)
                || (fromValue < period.from && toValue > period.to)
                || (toValue > period.from && toValue < period.to)
                ) {
                alert(language.periodsAreIntersects);
                isValid = false;
                return;
            }
        });
    }
    else {//this is second type [prescriped time]
        $(oldAppointementsPeriods).each(function (i, period) {
            if (fromValue == period) {
                alert(language.periodIsAlreadyExists);
                isValid = false;
                return;
            }
        });
    }
    return isValid;
}
/*this function get time entered by doctor as period 
and add html period at div
*/
function addNewIntervalPeriod(context) {
    //this is button that will save value to server
    //this button will append the new period of time to it's data
    var saveButton = $(context).parents('.appointementRecord').find('button.save');
    //select the two input time
    var timeInputs = $(context).parents('.controls').find('.selectTime input[type="time"]');
    var from = timeInputs.eq(0);
    var to = timeInputs.eq(1);
    if (!validateIntervalPeriods(from.val(), to.val(), saveButton.data('periods'))) return false;
    if (from.val() > to.val()) { alert(language.endPeriodMustExceedBeginPeriod); return false; }
    fromValue = timeCoversion($(from).val());
    toValue = timeCoversion($(to).val()); 
    //validate time inputed
    if (parseInt(fromValue) == 0 || parseInt(toValue) == 0) { alert(language.enterValidInterval); return; }
    var newPeriodObject = { from: fromValue, to: toValue };
    var targetAppointementButton = $(context).parents('.appointementRecord').find('button.appointementButton');
    addPeriodsToAppointementRecordAsHtml(new Array(newPeriodObject), targetAppointementButton);
    $(from).val('');//empty input
    $(to).val('');
}
function addNewTime(context) {
    context = $(context);
    var time = $(context).parents('.controls').find('.selectTime input');
    time = timeCoversion($(time).val());
    if (time == 0) { alert(language.enterValidInterval); return; }
    var targetButtonAccordian = $(context).parents('.appointementRecord').find('button.appointementButton');
    //this is button that will save value to server
    //this button will append the new period of time to it's data
    var saveButton = targetButtonAccordian.next().find('button.save');
    if (!validateIntervalPeriods(time, null, saveButton.data('periods'))) return false;
    addNewTimeAppointementRecordAsHtml(new Array(time), targetButtonAccordian);
}
/*this function validate and convert time value to string*/
function timeCoversion(time) {

    if (time.trim() == "") return 0;
    var times = time.split(':');
    var hours = parseInt(times[0]);
    var minuts = parseInt(times[1]);
    var type ="AM";
    if (hours == 0) {
        type ="AM"; hours = 12;
    }
    else if (hours == 12) {
        type ="PM"; hours = 12;
    }
    else if (hours > 12)
    { type ="PM"; hours = hours - 12; }

    if (hours.toString().length == 1) hours = "0" + hours.toString();
    if (minuts.toString().length == 1) minuts = "0" + minuts.toString();
    return hours + ':' + minuts + ' ' + type;
}
function saveAppointement(context) {
    var errorMessage, successMessage;
    var hideOrShowLoadingIcon = function (property) {
        $('.w3-accordion-content div button.save i.fa-spin').addClass(property);
    }
    context = $(context);
    var appointementDate = context.data('appointementdate');
    var periods = context.data('periods'); 
    periods = sortPeriods(periods);
    context.data('periods', periods)
    periods = JSON.stringify(periods);
    if (periods == undefined || periods.length == 0) {
        alert(language.noChangesToSave);
        return;
    }
    $(context).find('i').removeClass('hidden');
    var bookingType = $('.AppointSetting .form-group .type div input:checked').val();
    var obj = { appointementInterval: periods, appointementDate: appointementDate, bookingType: bookingType };
    $.post("/" + defaultPathForDoctorArea + "/saveAppointement", obj,
        function (data, status) {
            hideOrShowLoadingIcon('hidden');
            if (data.status) {
                alert(language.changesAreDone);
            }
            else {
                hideOrShowLoadingIcon('hidden');
                alert(operationProblem);
            }
        })
    .error(function () {
        hideOrShowLoadingIcon('hidden');
        alert(operationProblem);
    })
    .fail(function () {
        hideOrShowLoadingIcon('hidden');
        alert(operationProblem);
    });
}
function setDefaultAppointement(buttonContext) {
    buttonContext = $(buttonContext);
    //target button container of these new periods
    var targetButtonContainer = buttonContext.parents('.appointementRecord').find('button.appointementButton');
    //empty cuurent periods of trageted appointement
    targetButtonContainer.next().find('.periods').empty();
    //clear old periods
    targetButtonContainer.next().find('.save').data('periods', []);
    //array of sefaulted periods
    var defaultAppointementPeriods = currentAppointementParentDiv.find('.appointementRecord')
    .eq(0).find('button.appointementButton').eq(0).next().find('button.save').data('periods'); 
    //add html periods to target button
    addNewAppointementPeriodOrtime(defaultAppointementPeriods, targetButtonContainer);
}
/*sort array of periods*/
function sortPeriods(arr) {
        if(arr.length==0)return new Array();
        var isPeriod = arr[0].from != undefined;
        return arr.sort(function (obj1, obj2) {
            if(isPeriod)
            {
                if (obj1.from > obj2.from) return 1; else return -1;
            }
            else
            {
                if (obj1 > obj2) return 1; else return -1;
            }
        });
    }

