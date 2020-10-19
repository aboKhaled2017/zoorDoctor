/// <reference path="ar-EG-JS.js" />
/// <reference path="mainPage.js" />
/// <reference path="../../../scripts/jquery-1.12.4.js" />
/// <reference path="extension.js" />
/*these are function of doctor page--------------------------*/
function getDocorAdvices() {
    var advicesDiv = $('.doc-prev-advices');
    var adviceRecord = $('#clonedItems').find('.adviceRecord')[0];
    var getOldAdvicesButton = $('button.oldAdvices');
    if (getOldAdvicesButton.data('pageNumber') == undefined) getOldAdvicesButton.data('pageNumber', 1);
    var currentPageNumber = getOldAdvicesButton.data('pageNumber');
    //show loading icon         
    $(advicesDiv).find('.loading-circle').removeClass('hidden');
    $.get( defaultPathAdvices + '/getAdvices/',
        { pageNumber: currentPageNumber, pageSize: defaultPageSizeForAdvices },
        function (data, status) {
            //hide loading icon         
            $(advicesDiv).find('.loading-circle').addClass('hidden');
            var totalRecords = data.total;//total of doctor advices at server
            if (totalRecords <= (currentPageNumber * defaultPageSizeForAdvices)) {//all advices are fetched from server
                getOldAdvicesButton.addClass('hidden');
            }
            else {//there are more advice on server not fetched until 
                getOldAdvicesButton.removeClass('hidden');
                getOldAdvicesButton.data('pageNumber', (currentPageNumber + 1));
                $(getOldAdvicesButton).find('.hint').text("[" + language.aboutNumber + (totalRecords - currentPageNumber * defaultPageSizeForAdvices) + " " + language.otherAdvices + "]");
            }
            if (data.status != false) {
                var records = data.data;
                if (records.length == 0) {//there are no any advices 
                    $(advicesDiv).find('div.alert-info').removeClass('hidden').text(language.noWrittenAdvices);
                }
                else {//there are at least one record of advice
                    $(records).each(function (i, record) {
                        addAdviceRecordAsHtml("append", record.id, record.content, record.comments, record.likes, record.shares, record.seens);
                    });
                }
            }
            else {//problem occured at server
                $(advicesDiv).find('div.alert-info').removeClass('hidden').text(problemMessageAtServer);
            }
        });
}
function showCommentsOnAdvice(context, direction) {
    if (direction == undefined) direction = 0;
    /*
    directon[undefined|1|-1]
    undefined=>fetch data fro first time
    1=>>fetch next page of data
    -1=>fetch previous page of data
    */
    var adviceID = $(context).data('adviceID');
    var totalComments = $(context).data('commentsCount');
    if (parseInt(totalComments) == 0) {
        return false;
    }
    /*this is the div that contains commnets
    this div is by default is hidden
    and be visible when clicked on comments icon
    */
    context = $(context);//convert javascript object to jquery 
    //this is model that will show and contains comments of selected advice
    var showCommentModal = $('#commentsOnAdvice');
    var commentDiv = $(showCommentModal).find('.all-comments');
    var getOldCommentsButton = $(showCommentModal).find('button.oldComments');//navigate to next comments
    var getNewCommentsButton = $(showCommentModal).find('button.newComments');//navigat to previous comments
    //this function control 
    //when to hide or show both[getOldComments button,and getNewCommnets button]
    var buttonVisiblityControl = function (totalComments, pageSize, currentNavifator) {
        var maxNavigatorVlaue = Math.ceil(totalComments / pageSize);
        if (currentNavifator < maxNavigatorVlaue || (currentNavifator == 0 && maxNavigatorVlaue == 0))
            getOldCommentsButton.removeClass('hidden');//show get old comments buttons
        else {
            getOldCommentsButton.addClass('hidden');//hide get old comments buttons
        }
        if (currentNavifator > 1) {
            getNewCommentsButton.removeClass('hidden');//show get new comments buttons
        }
        else {
            getNewCommentsButton.addClass('hidden');//hide get new comments buttons
        }
    }
    /*the current context will hold dictionary data as follow
    adviceID=>this is advice id whose comments will be fetched,
    commentsCount=>total number of comments for this advice,
    pageNumber=>this is next page of number that will fetched in next time,
    pageSize=>this is number of comments that will be displayed at model each time
    and these are the number of items that will fetched from server each request,
    records=>this will contains all recorded that has been loaded from server,
    navigator->this is indicator for cuurent displayed page number.
    */
    var pageSize;
    /*record key is added to current button ,this key will hold all data 
    that will be loaded and all old data that already loaded befor
    so that ,if all data has been loaded from server ,we will needn't to load it agin from server
    */
    if (context.data('records') == undefined) {//the comments will be fetched for first time
        //initialization for data
        context.data('records', []);
        context.data('pageNumber', 1);
        context.data('pageSize', defaultPageSizeForComments);
        context.data('navigator', 0);
        pageSize = context.data('pageSize');
        buttonVisiblityControl(0, context.data('pageSize'), 0);
    }
    else {//get old fetched comments or get new comments from server[based on direction]
        /*
        direction
        case 1:fetched data may be from old data or from server
        case -1:no data will be fetched from server
        case undefined:the navigator of page will be displayed
        */
        var oldCommentsRecords = context.data('records');
        var nextPageNumber = context.data('pageNumber');
        var pageSize = context.data('pageSize');
        var totalComments = context.data('commentsCount');
        var currentNavigator = context.data('navigator');
        buttonVisiblityControl(totalComments, pageSize, currentNavigator + direction);
        if (direction == 1) {//next page will be displayed from server or from old data
            var lastValueForNavigator = (oldCommentsRecords.length / pageSize);
            if (currentNavigator < lastValueForNavigator) {//next page will not fetched from server
                context.data('navigator', currentNavigator + 1);
                $(commentDiv).find('.comments').empty();
                FillCommentsModalWithCommentsRecords(oldCommentsRecords, $(commentDiv), currentNavigator + 1, pageSize);
                $(showCommentModal).modal('show');
                return;
            }
        }
        else if (direction == -1) {//previous navigator page will be displayed
            //you can add previous page comments to currents page comments by disabling next line
            context.data('navigator', currentNavigator - 1);
            $(commentDiv).find('.comments').empty();
            FillCommentsModalWithCommentsRecords(oldCommentsRecords, $(commentDiv), currentNavigator - 1, pageSize);
            $(showCommentModal).modal('show');
            return;
        }
        else {//current page navigator will be displayed
            //you can add previous page comments to currents page comments by disabling next line
            $(commentDiv).find('.comments').empty();
            FillCommentsModalWithCommentsRecords(oldCommentsRecords, $(commentDiv), currentNavigator, pageSize);
            $(showCommentModal).modal('show');
            return;
        }
    }
    var currentPageNumber = context.data('pageNumber');
    $(showCommentModal).modal('show');
    //show loading icon
    $(showCommentModal).find('.loadingComment').removeClass('hidden');
    //get json data from server
    $.get(defaultPathAdvices + "getAdviceComments",
        { pageNumber: currentPageNumber, pageSize: pageSize, adviceID: adviceID },
        function (result, status) {
            //hide loading icon
            $(showCommentModal).find('.loadingComment').addClass('hidden');
            var totalComments = result.total;
            var data = result.data;
            if (totalComments <= (currentPageNumber * pageSize)) {//all comments of advice are fetched from server
                getOldCommentsButton.addClass('hidden');//hide get comments buttons
            }
            else {//there are more comments of this advice on server not fetched until  
                //this is next pagenumber that will be fetched next time from server
                context.data('pageNumber', (currentPageNumber + 1));
                getOldCommentsButton.removeClass('hidden');//show get comments buttons
                getOldCommentsButton.find('.hint').text("["+language.aboutNumber + (totalComments - currentPageNumber * pageSize) + " "+language.otherAdvices+"]");
            }
            if (data.length > 0) {//add the newlly fetched data to previous data in records
                var currentRecords = context.data('records');
                var currentnavigator = context.data('navigator');
                context.data('records', currentRecords.concat(data));
                context.data('navigator', currentnavigator + 1);
            }
            //empty previous page comments and add current page comments
            //you can add previous page comments to currents page comments
            $(commentDiv).find('.comments').empty();
            FillCommentsModalWithCommentsRecords(data, $(commentDiv));
        }
        ).fail(function () { alert(problemMessageAtServer); })
         .error(function () {
             alert(problemMessageAtServer);
         }).complete(function () {
             /*here you can load the rest of comments from server
             after the current page number of comments has been loaded*/
         });
}
function getPatientViewsOnDoctor(direction,docID) {
    var patientViewsDiv = $('#patient-view');
    var moreViewsButton = $('#moreViews');
    var prevViewsButton = $('#prevViews');
    var allPatientsDiv = $(patientViewsDiv).find('.patients .body');
    //this is object that will store patientViews data
    var PVData = patientViewsDiv.data();
    /*
    this function control 
    which time to hide or show both[more advices button,and previous advices button]
    */
    var buttonVisiblityControl = function () {
        var maxNavigatorVlaue = Math.ceil(PVData.totalRecordsAtServer / defaultPageSizeForPatientViews);
        if (PVData.navigator < maxNavigatorVlaue || (PVData.navigator == 0 && maxNavigatorVlaue == 0))
            moreViewsButton.removeClass('hidden');
        else {
            moreViewsButton.addClass('hidden');
        }
        if (PVData.navigator > 1) {
            prevViewsButton.removeClass('hidden');
        }
        else {
            prevViewsButton.addClass('hidden');
        }
    }
    //this function is called on error
    var error = function (message) {
        $(patientViewDiv).find('.alert').removeClass('hidden').text(message);
    }
    //check if any patient views fetched befor 
    if (PVData.loadedRecords == undefined) {//intailize data
        PVData.loadedRecords = [];
        PVData.nextpageNumber = 1;
        PVData.navigator = 0;
        PVData.totalRecordsAtServer = 0;
    }
    switch (direction) {
        case 1: {//next page will be displayed from server or from old data
            var lastValueForNavigator = (PVData.loadedRecords.length / defaultPageSizeForPatientViews);
            if (PVData.navigator < lastValueForNavigator) {//next page will not fetched from server
                ++PVData.navigator;
                allPatientsDiv.empty();//empty old views
                showPtientVeiwRecordsAsHtml(PVData.loadedRecords, allPatientsDiv, PVData.navigator, defaultPageSizeForPatientViews);
                buttonVisiblityControl();
                return;
            }
        }; break;
        case -1: {
            --PVData.navigator;
            allPatientsDiv.empty();
            showPtientVeiwRecordsAsHtml(PVData.loadedRecords, allPatientsDiv, PVData.navigator, defaultPageSizeForPatientViews);
            buttonVisiblityControl();
            return;
        }; break;
        default: break;
    }
    //show loading icon
    patientViewsDiv.find('.loadingComments').removeClass('hidden'); 
    $.post(defaultPathForDoctorArea + 'doctorPatientViews',
        { pageNumber: PVData.nextpageNumber, pageSize: defaultPageSizeForPatientViews, doctorID: docID },
        function (data, status) {           
            //show loading icon            
            patientViewsDiv.find('.loadingComments').addClass('hidden');
            if (data.status == false) {
                error(problemMessageAtServer);
            }
            else {
                if (data.data.length > 0) {
                    PVData.totalRecordsAtServer = data.total;
                    PVData.loadedRecords = (PVData.loadedRecords.concat(data.data));
                    PVData.navigator = PVData.navigator + 1;
                    PVData.nextpageNumber = PVData.nextpageNumber + 1;
                    //add newlly loadded data to view model
                    allPatientsDiv.empty();
                    showPtientVeiwRecordsAsHtml(PVData.loadedRecords, allPatientsDiv, PVData.navigator, defaultPageSizeForPatientViews);
                    buttonVisiblityControl();
                }
                else {
                    patientViewsDiv.find('.alert').removeClass('hidden').text(language.noPatientViewsForDoctor);
                }
            }
        })
    .error(function () {
        //show loading icon            
        patientViewsDiv.find('.loadingComments').addClass('hidden');
        patientViewsDiv.find('.alert').removeClass('hidden').text(language.noPatientViewsForDoctor);
    })
    .fail(function () {
        //show loading icon            
        patientViewsDiv.find('.loadingComments').addClass('hidden');
        patientViewsDiv.find('.alert').removeClass('hidden').text(language.noPatientViewsForDoctor);
    });
}
function getCurrentPatientView(doctorID) {   
    var patientViewDiv = $('#patient-view');
    $('#addBtnView').data('doctorID',doctorID);
    var ratingDiv = $(patientViewDiv).find('.addView .stars');
    $.get(defaultPathPatients + '/getCurrentPatientView/', { docID: doctorID }, function (data, status) {
        if(!data)
        {//patient is not authenticated
            ratingDiv.data('status',false);
        }
        else
        {
            if (data.isAuthenticated) ratingDiv.data('isAuthenticated', true);
            else  ratingDiv.data('isAuthenticated', false);
            if (data.canRate) ratingDiv.data('canRate', true);
            else ratingDiv.data('canRate', false);
            ratingDiv.data('status', true);
            //alert(JSON.stringify(data));
            $(ratingDiv).data('value', data.currentPtRate);
            $(patientViewDiv).find('form textarea').val(data.currentPtComment);
            if (data.currentPtComment != null && data.currentPtComment != "") {
                if (data.currentPatinetID != "" && data.currentPatinetID != 0)
                    $(patientViewDiv).find('form button').text('').append('<i class="fa fa-edit"></i> '+language.changeDoctorView);
            }
            $(ratingDiv).find('i').unbind('click');
            handleOnRateChanged(ratingDiv, doctorID);
        }
    });
}
function handleOnRateChanged(rateDiv, doctorID) {
    rateDiv = $(rateDiv);
    var setRating=function(rateValue)
    {
        rateDiv.find('i').each(function (index, icon) {
            if (index < rateValue) $(icon).rateMark(true);
            else {
                $(icon).rateMark(false);
            }
        });
    }
    var onRateClicked = function () {

    }
    var currentRateValue = parseInt(rateDiv.data('value'));
    //set patient rate
    setRating(currentRateValue);
    var onMouswEnter = function (context) {
        $(context).css("cursor", "pointer");
        $(context).rateMark(true)
            .prevAll().rateMark(true);
        $(context).nextAll().rateMark(false);

    }
    var onMouseLeave = function (context) {
        setRating(currentRateValue);
    }
    rateDiv.find('i')   
    .mouseenter(function () { onMouswEnter(this);})
    .mouseleave(function () { onMouseLeave(this);})
    .bind('touchstart click',function () {
        var clickedStare = $(this);
        var ratedValue = clickedStare.index() + 1
        rateDiv.find('i').unbind('mouseleave mouseenter click');//ban click event
        clickedStare.rateMark(true).prevAll().rateMark(true);
        if (ratedValue == parseInt(rateDiv.data('value'))) return;//rating is not changes
        rateDiv.data('value', ratedValue);
        if (!rateDiv.data('status')) {//if there are problem at serevr
            if (confirm(language.evaluateProblem))
                location.reload(); return;
        }
        if (!rateDiv.data('isAuthenticated'))
        {//if user not registered 
            if (confirm(language.loginFirst)) {
                gotoLoginPage();
            }
            setRating(currentRateValue); 
            return;
        }
        if (!rateDiv.data('canRate'))
        {//if user has no visits to this doctor
            alert(language.youHavNoReservation);
            setRating(currentRateValue);
            return;
        }
        //show laoding icon
        clickedStare.toggleLoadingIcon('fa-star');
        $.post(defaultPathPatients + 'addPatientRate', { rate: ratedValue, doctorID: doctorID }, function (data, status) {
             if (data == false) {
                alert(language.serverProblem);
                setRating(currentRateValue);
                 //hide laoding icon
                clickedStare.toggleLoadingIcon('fa-star');
             }
             else{
                 //hide laoding icon
                 clickedStare.toggleLoadingIcon('fa-star');
                 setRating(ratedValue);
             }
            /*else {
                var rateDivInComments = $('.patient-view .patients .patient:first-of-type .view p.rate>i');
                if ($(rateDivInComments).parents('.rate').prev().text() == $('.addView textarea').val()) {
                    $(rateDivInComments).each(function (index, icon) {
                        if (index < ratedValue) $(icon).removeClass('fa-star-o').addClass('fa-star');
                        else {
                            $(icon).addClass('fa-star-o').removeClass('fa-star');
                        }
                    });
                }
            }*/
        }).complete(function () { rateDiv.find('i').bind('click','mouseleave mouseenter'); });//end of post
    });//end of click rate event
} 
function addPatientComment(btnContext, comment,patientName) {
    btnContext = $(btnContext);
    var doctorID = btnContext.data('doctorID');
    if (comment.length == 0 || comment.trim().length<3)
    {
        $(btnContext).prev().val('');
        $(btnContext).prev().attr('placeholder', language.validateFielLength(3));
        return false;
    }
    var rateDiv = $('#patient-view').find('.addView .stars');
    if (!rateDiv.data('status')) {//if there are problem at serevr
        if (confirm(language.serverProblem))
            location.reload(); return;
    }
    if (!rateDiv.data('isAuthenticated')) {//if user not registered 
        if (confirm(language.loginFirst)) {
            gotoLoginPage();
        }
        return;
    }
    var isAddBtn = ($(btnContext).find('i').hasClass('fa-plus')) ? true : false;
    var toggleLoadingIcon=function()
    {
        if (isAddBtn) $(btnContext).find('i').toggleLoadingIcon('fa-plus');
        else $(btnContext).find('i').toggleLoadingIcon('fa-edit');
    }
    toggleLoadingIcon();//show loading icon
    $.post(defaultPathPatients + 'addPatientComment/', { doctorID: doctorID, comment: comment }, function (data, status) {
        toggleLoadingIcon();//hide loading icon
        if (data) {
            var rate = $('#patient-view').find('.addView .stars').data('value');
            if(isAddBtn)
            {//add new comment
                addNewPatientCommentAsHtml(comment, patientName, rate);
            }
            else
            {//update comment
                //patient name to identify updated record
                updatePatientCommentAsHtml(comment,patientName);
            }
        }
        else {
            alert(language.serverProblem);
        }
    });
}
function addNewPatientCommentAsHtml(comment, name, rate) {
    var patientViewDiv = $('#patient-view');
    $(patientViewDiv).find('form button').text('').append('<i class="fa fa-edit"></i> '+language.changeDoctorView);
    var allPatientsDiv = $(patientViewDiv).find('.patients .body');
    var patientDiv = document.createElement('div');
    patientDiv.className = 'patient';
    $(patientDiv).css('display', 'none');
    var ptInfoDiv = document.createElement('div');
    ptInfoDiv.className = 'patient-info';
    $(ptInfoDiv).append('<span class="name"> <i class="fa fa-envelope-o fa-lg"></i> ' + name + ' </span>');
    $(patientDiv).append(ptInfoDiv);
    var ptViewDiv = document.createElement('div');
    ptViewDiv.className = 'view';
    $(ptViewDiv).data('patientName',name);
    $(ptViewDiv).append('<p>' + comment + '</p>');
    var doctorRate = document.createElement('p');
    doctorRate.className = 'rate';
    for (var k = 0; k < rate ; k++) {
        $(doctorRate).append('<i class="fa fa-star fa-lg"></i>');
    }
    for (var k = rate; k < 5 ; k++) {
        $(doctorRate).append('<i class="fa fa-star-o fa-lg"></i>');
    }
    $(ptViewDiv).append(doctorRate);
    $(patientDiv).append(ptViewDiv);
    $(allPatientsDiv).prepend('<hr class="center-block">');
    $(allPatientsDiv).prepend(patientDiv);
    $(patientDiv).fadeIn(1000);
    var textArea = $(allPatientsDiv).parents('.patients').siblings('.addView').find('textarea');
    $(textArea).keyup(function () {
        var text = $(this).val();
        $(patientDiv).find('.view p:first-of-type').text(text);
    });
}
function updatePatientCommentAsHtml(newComment,patientName)
{
    var allViews = $('#patient-view').find('.patients .body .view');
    allViews.each(function (i, view) {
        view = $(view);
        if(view.data('patientName')==patientName)
        {
            view.find('.ptComment').text(newComment);
        }
    });
}
/*
this function take array of records and take page from these records 
starting from navigator index  based on page size 
and design each record as 
html record to target div element
*/
function FillCommentsModalWithCommentsRecords(records, targetElement, currentNavigator, pageSize) {
    if (currentNavigator != undefined) {//only part of given records will be displayed based on navigator
        var startIndex = ((currentNavigator - 1) * pageSize) - 1;
        records = $(records).map(function (i, record) {
            if (startIndex < i && i < startIndex + pageSize + 1) return record;
        });
    }
    for (var i = 0; i < records.length; i++) {
        var html = "";
        html += '<div class="row comment">';
        html += '<div class="col-sm-3 col-xs-12 name">' + records[i].patientName + '</div>';
        html += '<div class="col-sm-9 col-xs-12 comment-content">' + records[i].comment + '</div>';
        html += '</div>';
        $(targetElement).find('.comments').append(html);
    }
}
/*
addingType[append|prpend]
commentsNum:number of comments on this advice
likesNum:number of likes
sharesNum:number of shres
seenNum:number of viewers
*/
function addAdviceRecordAsHtml(addingtype, adviceID, adviceContent, commentsNum, likesNum, sharesNum, seenNum) {
    //adding type[append/prepend]
    var adviceRecord = $('#clonedItems').find('.adviceRecord')[0];
    adviceRecord = $(adviceRecord).clone();//
    var adviceContentDiv = $(adviceRecord).find('.advice');
    var adviceControls = $(adviceRecord).find('.controls >i');
    var adviceNotificationsButtons = $(adviceRecord).find('.notifications>i');
    var adviceNotificationsStaticts = $(adviceRecord).find('.notf-number>div');
    var commentsModal = $('#commentsOnAdvice');
    $(adviceContentDiv).text(adviceContent);
    $(adviceControls).eq(0).attr('onclick', 'updateDoctorAdvice("' + adviceID + '",this)');
    $(adviceControls).eq(1).attr('onclick', 'deleteDoctorAdvice("' + adviceID + '",this)');
    //this is comment icon,on click it,it will show model that contains comments
    var showCommentIconButton = $(adviceNotificationsButtons).eq(0);
    //when comment icon is clicked to get comments
    $(showCommentIconButton).click(function () {
        //this is button that found on modal to get old comments
        //the modal is shared between all comment icons
        //so we must change the target of this button each time the modal is called to get comments
        var showOldCommentsButton = $(commentsModal).find('button.oldComments');
        var showNewCommentsButton = $(commentsModal).find('button.newComments');
        $(showOldCommentsButton).unbind('click');
        $(showNewCommentsButton).unbind('click');
        //on clicking show old comments at modal
        showOldCommentsButton.click(function () {
            //get next page comments
            showCommentsOnAdvice(showCommentIconButton, 1);
        });
        //on clicking show new comments at modal
        showNewCommentsButton.click(function () {
            //get previous page comments
            showCommentsOnAdvice(showCommentIconButton, -1);
        });
        showCommentsOnAdvice(this);
    });
    showCommentIconButton.data('adviceID', adviceID);
    showCommentIconButton.data('commentsCount', commentsNum);
    $(adviceNotificationsStaticts).eq(0).text(commentsNum);
    $(adviceNotificationsStaticts).eq(1).text(sharesNum);
    $(adviceNotificationsStaticts).eq(2).text(seenNum);
    $(adviceNotificationsStaticts).eq(3).text(likesNum);
    var advicesDiv = $('.doc-prev-advices');//this is advices div that contains group of div record
    $(advicesDiv).find('div.alert-info').addClass('hidden');
    if (addingtype == "append") {
        $(advicesDiv).find('.advices').append(adviceRecord);
    }
    else {
        $(advicesDiv).find('.advices').prepend(adviceRecord);
    }
    return adviceRecord;
}
/*
records->array of patients views that will be displayed
target element->this is container of patient views
*/
function showPtientVeiwRecordsAsHtml(records, targetElement, currentNavigator, pageSize) {
    if (currentNavigator != 0) {//only part of given records will be displayed based on navigator
        var startIndex = ((currentNavigator - 1) * pageSize) - 1;
        records = $(records).map(function (i, record) {
            if (startIndex < i && i < startIndex + pageSize + 1) return record;
        });
    }
    for (var i = 0; i < records.length; i++) {
        var record = records[i];
        if (i > 0) $(targetElement).append('<hr class="center-block"/>');
        var patientDiv = document.createElement('div');
        patientDiv.className = 'patient';
        var ptInfoDiv = document.createElement('div');
        ptInfoDiv.className = 'patient-info';
        $(ptInfoDiv).append('<span class="name"> <i class="fa fa-envelope-o fa-lg"></i> ' + record.patientName + ' </span>');
        $(patientDiv).append(ptInfoDiv);
        var ptViewDiv = document.createElement('div');
        ptViewDiv.className = 'view';
        $(ptViewDiv).data('patientName', record.patientName);
        if (record.comment == "" || record.comment.length < 1) {
            record.comment = language.ratingWithoutComment;
            $(ptViewDiv).append('<p class="ptComment" style="color:#f00">' + record.comment + '</p>');
        }
        else {
            $(ptViewDiv).append('<p class="ptComment">' + record.comment + '</p>');
        }
        var doctorRate = document.createElement('p');
        doctorRate.className = 'rate';
        var rate = record.rate;
        for (var k = 0; k < rate ; k++) {
            $(doctorRate).append('<i class="fa fa-star fa-lg"></i>');
        }
        for (var k = rate; k < 5 ; k++) {
            $(doctorRate).append('<i class="fa fa-star-o fa-lg"></i>');
        }
        $(ptViewDiv).append(doctorRate);
        $(patientDiv).append(ptViewDiv);
        $(targetElement).append(patientDiv);
    }
}
