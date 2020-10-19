/// <reference path="en-US-Js.js" />
/// <reference path="../../../js/jquery.min.js" />
/// <reference path="mainPage.js" />
/// <reference path="extension.js" />
controllerName = "index";
var months = ["يناير", "فبراير", "مارس", "إبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"];
var days = ["اﻷحد", "اﻷثنين", "الثلاثاء", "اﻷربعاء", "الخميس", "الجمعة", "السبت"];
$(function () {
    $('.nav > li').click(function () {
        $(this).addClass('active').siblings().removeClass('active');
    });
    currentLangauge = $('body').attr('lang'); 
    startImageCarousel();
    handleLocations();
    handleDocCardDesign();
    //////////////////////////   
    $('.form-group').has('input[type="password"]').prepend('<i class="showPassword fa fa-eye fa-lg"></i>')
                    .css("position", "relative").end()
                    .find('.showPassword')
                    .click(function () { $(this).parents('.form-group').find('[name="password"]').togglePassowrdShow(); });
    /*advices page*/
    $('ul.selectDoctor li:not(.noResult)').mouseenter(function () { $(this).parent().prev().removeAttr('onblur'); });
    $('ul.selectDoctor li:not(.noResult)').mouseleave(function () { $(this).parent().prev().attr('onblur', '$(this).next().addClass("hidden")'); });
    /*advices page*/  
    $('.complainMessage').find('i.fa-close').click(function () { $(this).parent().fadeOut(); });
    /*register section*/
    $('.register-section .login-section .login-box .login form .form-group label').next('input').addClass("form-control");
    /*register section*/
    /*header*/
    $('.reserving-search form .row .form-group .dropdown>button').click(function () {
        if ($(window).width()>767)
        {
            makeScrollTop(0);
        }
    });
    /*header*/
    /*start scroll botton*/
    var scrollDown = $('.reserving .scroll-btn i');
    scrollDown.click(function () {
        var pageHeight = $('body').height();
        $('body,html').animate({ scrollTop: pageHeight }, 2000);
    });

    var scrollUp = $('.scroll-btn i');
    scrollUp.click(function () {
        var scrollUp = $(window).scrollTop();
        if (scrollUp > 500)
            $('html,body').animate({ scrollTop: 0 }, 2000);
    });

    /*end scroll button*/
    /*start speciality section*/
    var leaved = false;
    $('.specialities .speciality').bind('mouseenter mouseleave', function (ev) {
        leaved = false;
        if (ev.type == 'mouseenter') {
            $(this).parents('.specialities').find('.speciality .overlay').stop().css('height','0%').end().find('a.btn').hide();
            var specialityOverlay = $(this).find('.overlay');
            var specialityButton = $(this).find('a.btn');
            $(specialityOverlay).animate({ 'height': '96%' }, 1000, function () {
                $(specialityButton).fadeIn();
            });
        }
        else if (ev.type == "mouseleave") {
            var specialityOverlay = $(this).find('.overlay');
            var specialityButton = $(this).find('a.btn');
            specialityOverlay.animate({ 'height': '0%' }, 1000, function () {
                specialityButton.fadeOut();
            });
        }
    });
    /*end speciality section*/
});/*end jquery function*/
/*general functions*/
function getStart(controller) {
    controllerName = controller;
}
function callAjax(ajUrl, ajData, ajType, ajContenttype, ajDataType, ajSuccessFunc, ajErrorFunc) {
    $.ajax({
        url: ajUrl,
        data: ajData,
        type: ajType,
        contentType: ajContenttype,
        dataType: ajDataType,
        success: function (result) {
            if (ajSuccessFunc == undefined) return;
            ajSuccessFunc(result);
        },
        error: function (errormessage) {
            if (ajErrorFunc == undefined) return;
            ajErrorFunc(errormessage);
        }
    });
}
function isArabic(strInput) {
    var arregex = /[\u0600-\u06FF]/;
    return (arregex.test(strInput))
}
function convertToJsonString(str)
{
    return str.replace(/&quot;/gi, "\"");
}
/*auto called functions at starts*/
function startImageCarousel() {
    var changeImage = function () {
        var nextImg;
        var currentImg = $('.reserving .overlay>img.shown');
        currentImg.removeClass('shown').fadeOut();
        if (currentImg.next().is('img'))
            nextImg = currentImg.next().addClass('shown');
        else {
            nextImg = $('.reserving .overlay>img:first-of-type').addClass('shown');
        }
        nextImg.fadeIn();
    }
    setInterval(changeImage, 6000)
}
/*scroll functions*/
function makeScrollTop(scrollVal) {
    $('html').scrollTop(scrollVal);
    $('body').scrollTop(scrollVal);
}
function makeScrollTopAnimation(target) {
    var scroll = $('.top-nav').height() + $('.header').height();
    location.href = target;
    var scrollValue = ($('body').scrollTop() > $('html').scrollTop()) ? $('body').scrollTop() : $('html').scrollTop();
    $('html').animate({ scrollTop: scrollValue - scroll }, 1000);
    $('body').animate({ scrollTop: scrollValue - scroll }, 1000);
}
/*paginations functions*/
function togglePaginationEvent(type) {//add click event to pagination links
    if (type == 'bind') $('#pagination li').bind('click', function () { onPaginationClicked(this); });
    else if (type == 'unbind') $('#pagination li').unbind('click');
}
function onPaginationClicked(context) {
    var clickedLi = $(context);
    var paginationDiv = clickedLi.parents('#pagination');
    var searchObject = paginationDiv.data('searchObject');
    var currentPageNumber = searchObject.pageNumber; 
    var requestedPageLiContext;
    if (clickedLi.hasClass('edge')) {//this is edge element
        if (clickedLi.index() == 0) {//this is first edge[go to previous]
            if (currentPageNumber == 1) return;
            var activePageLi = clickedLi.siblings().eq(currentPageNumber - 1);
            activePageLi.removeClass('w3-red').prev().addClass('w3-red');
            searchObject.pageNumber=currentPageNumber - 1;
            requestedPageLiContext = activePageLi.prev();
        }
        else {//this is last edge[go to next]
            if (currentPageNumber == paginationDiv.find('li').length - 2) return;
            var activePageLi = clickedLi.siblings().eq(currentPageNumber);
            activePageLi.removeClass('w3-red').next().addClass('w3-red');
            searchObject.pageNumber = currentPageNumber + 1;
            requestedPageLiContext = activePageLi.next();
        }
    }
    else {//this is page element
        var activePageLi = paginationDiv.find('li').eq(currentPageNumber-1); 
        activePageLi.removeClass('w3-red'); 
        clickedLi.addClass('w3-red');
        searchObject.pageNumber=clickedLi.data('pageNumber');
        requestedPageLiContext = clickedLi;
    }
    paginationDiv.data('searchObject',searchObject);//set modified search object to pagination
    getPageOfSearchData(paginationDiv, requestedPageLiContext);
}
function getPageOfSearchData(pagination, requestPageLiContext) {
    pagination = $(pagination);
    requestPageLiContext = $(requestPageLiContext);
    var records = requestPageLiContext.data('records');
    if(records == undefined) {//get the records from database
        var dataSearchType = pagination.data('dataType');
        togglePaginationEvent('unbind');//timply disable click on pagination
        requestPageLiContext.find('a').toggleTextWithLoadingIcon();//show loading icon
        var getDta = function (result) {
            //hide loading icon
            requestPageLiContext.find('a').toggleTextWithLoadingIcon();
            requestPageLiContext.data('records', result.data);
            mapDoctorsDataToHtmlCards(result.data);
            togglePaginationEvent('bind');//enable click on pagination
        }
        var searchError = function (error) {
            //show loading icon
            $('#searchButton').toggleIconWithLoadingImg();
            alert(language.cannotLoadData);
            requestPageLiContext.find('a').toggleTextWithLoadingIcon();//show loading icon
        }
        getSearchDataByAjax(getDta, searchError, dataSearchType, pagination.data('searchObject'));
    }
    else {//records alraedy founded
        mapDoctorsDataToHtmlCards(records);
    }
}
function getSearchDataByAjax(fndata, fnError, dataType, searchDataObject,method) {
    var url;
    if (method == undefined) method = "POST";
    if (dataType == 'doctorsData') {
        url = defaultPathSearch + "getSearchResult";
    }
    callAjax(//get search data
            url,
            JSON.stringify(searchDataObject),
            method, "application/json;charset=utf-8",
            "json",
            fndata,
            fnError
            );
}
function settingPagination(result, searchObject, dataType) {
    var count = result.count;
    var dataRecords = result.data;
    var paginationDiv = $('#pagination');
    /*pagination div will contain such info
    1-data type searched data[doctor search|advice search|....]
    2-search object that contain search data patterns
    */
    paginationDiv.data('dataType', dataType);
    paginationDiv.data('searchObject',searchObject);
    //number of pages of matched search data
    var pages = Math.ceil(count / maxOfCardNumbersForSearchPage);
    var ul = paginationDiv.find('ul');
    if (pages < 2) ul.empty();//empty pagination
    else {//pgaes more than 2       
        var new_li = function (classes, text) {
            return $('<li class="' + classes + '"><a href="#">' + text + '</a></li>');
        }
        $(ul).empty();
        if (pages > numberOfPaginationLinks) {
            var li = new_li('first edge', '«').data('pageNumber', 0);
            $(ul).append(li);
        }
        if (count > maxOfCardNumbersForSearchPage) {
            for (var i = 1; i <= pages; i++) {
                if (i ==1) {
                    var li = new_li('w3-red', i).data('pageNumber', i).data('records', dataRecords);
                    $(ul).append(li);
                }
                else {
                    var li = new_li('', i).data('pageNumber', i);
                    $(ul).append(li);
                }
            }
        }
        if (pages > numberOfPaginationLinks) {
            var li = new_li('last edge', '»').data('pageNumber', -1);
            $(ul).append(li);
        }
    }
}
//complain form of main page
function validateComplainForm(button) {
    var isvalidate = true;
    button = $(button);
    var form = button.parents('form');
    var inputName = form.find('input[name="name"]');
    var inputMail = form.find('input[name="mail"]');
    var inputComment = form.find('textarea[name="message"]');
    var name = inputName.val();
    var mail = inputMail.val();
    var comment = inputComment.val();
    if (mail.match(/^[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}/) == null) {
        inputMail.css("border", '2px solid #f00');
        inputMail.clearText();
        inputMail.attr('placeholder',language.emailNotVlaid);
        isvalidate = false;
    }
    if (name == "" || name.trim().length < 3) {
        inputName.css("border", '2px solid #f00');
        inputName.clearText();
        inputName.attr('placeholder', language.nameNotValid);
        isvalidate = false;
    }
    else if (name.trim().length >25) {
        inputName.css("border", '2px solid #f00');
        inputName.clearText()
        inputName.attr('placeholder', language.nameIsTooMany);
        isvalidate = false;
    }
    if (comment == "" || comment.trim().length < 15) {
        inputComment.css("border", '2px solid #f00');
        inputComment.clearText();
        inputComment.attr('placeholder',language.validateFielLength(15)).css("border", '2px solid #f00').val('');
        isvalidate = false;
    }
    form.find('textarea,input').keydown(function () {
        $(this).css('border', '1px solid lightgray');
        $(this).unbind('keydown');
    });
    if (isvalidate) {
        button.find('i').toggleLoadingIcon('fa-send-o');
        $.post("/getComplains", { name: name, mail: mail, message: comment,general:false}, function (data, status) {
            if (data == true) {
                button.find('i').toggleLoadingIcon('fa-send-o');
                $('.complainMessage').fadeIn();
            }
        });
    }
}
/*speciality page*/
function getSpecialityDescrip(context, spName) {
    context = $(context);
    var imgSrc = context.parents('.speciality').find('img').attr('src');
    var spname = context.parents('.speciality').find('p').text();
    var spDiv = $('.speciality-type');
    var fillData = function (data) {
        makeScrollTop(0);
        $(spDiv).fadeIn(1500);
        $(spDiv).find('img').attr('src', imgSrc);
        $(spDiv).find('>h4').text(language.specialityname + spname);
        $(spDiv).find('.discription p').text(data);
        $(spDiv).find('div>a').text(language.specializedDoc + spname);
        var spDocsHref = $(spDiv).find('div>a').attr('href');
        $(spDiv).find('div>a').attr('href', spDocsHref + spname);
    }
    if (context.data('loaded')) {
        fillData(context.data('description'));
        return;
    }
    else {
        //show loading icon
        context.find('i').removeClass('hidden');
    }
    $.get(defaultPathSpecialitiesGetData + 'getSpDescription', { name: spname }, function (data, status) {
        context.find('i').addClass('hidden');
        if (data == false || data == "") {
            alert(language.serverProblem);
        }
        else {
            fillData(data);
            //store data to button so that not loaded each time
            context.data('loaded', true).data('description', data);
        }
    });
}
/*search page*/
function getDestrictsOfCity(cityID) {
    var destrictsIds = [];
    $('.areaSelect').find('option').each(function (i, el) {
        if (parseInt($(el).attr('data-value')) == cityID) {
            destrictsIds.push(i);
        }
    });
    return destrictsIds;
}
function onCitySelected(context) {
    var id = parseInt($(context).find('option:selected').val());//selected city id
    var areaItemsLi = $('.areaSelect .dropdown-menu li');//menu of areas/destricts
    //all destricts except first one(----)
    if (id == 0) {//if no city is selected            
        areaItemsLi.not(':first-of-type').addClass('hidden');//hide all destrcits
        $('.areaSelect .dropdown-toggle .filter-option').text(language.allDestricts);
        return;
    }
    areaItemsLi.not(':first-of-type').addClass('hidden');//hide all destricts except default one
    var areaSelectsIDs = [];//all destricts of selected city
    //set default destrict ----- to be selected
    $('.areaSelect').find('option:selected').removeAttr('selected').end().find('option').eq(0).attr('selected', 'selected');
    //get areas id of selected city
    areaSelectsIDs = getDestrictsOfCity(id);
    areaItemsLi.each(function (i, li) {
        if (areaSelectsIDs.indexOf(i) != -1) $(li).removeClass('hidden');
    });
    areaItemsLi.removeClass('selected').eq(0).addClass('selected');
    //set default destrict to be selected  
    $('.areaSelect .dropdown-toggle .filter-option').text(language.allDestricts);
}
function hideDestrictsOfUnSelectedCity(selectedCityID) {
    var cityContext = $('select.citySelect').get(0);
    var areaItemsLi = $('.areaSelect .dropdown-menu li');//menu of areas/destricts
    var areaSelectsIDs = getDestrictsOfCity(selectedCityID);//all destricts of selected city
    areaItemsLi.each(function (i, li) {
        if (areaSelectsIDs.indexOf(i) != -1) $(li).removeClass('hidden');
        else { $(li).addClass('hidden') }
    });//map select item to menu items
}
function getFormDataSearchObject(form)
{
    var searchFormValues = $(form).serializeArray(); 
    var formData = {};
    formData.type =new Array();
    formData.education = new Array();
    formData.price = new Array();
    formData.specialityIDs = new Array();
    formData.pageNumber = 1; 
    formData.generalSearchPattern =null;//default page size is 9
    $(searchFormValues).each(function (i, obj) {
        var name = obj.name; 
        var value = obj.value;
        if (name == "type") {
            formData.type.push(parseInt(value));
        }
        else if (name == "education") {
            formData.education.push(parseInt(value));
        }
        else if (name == "price") {
            formData.price.push(parseInt(value));
        }
        else if (name == "specialityIDs") {
            formData.specialityIDs.push(parseInt(value));
        }
        else if (name == "destrictID") {
            formData.destrictID = value;
        }
        else if (name == "cityID") {
            formData.cityID = value;
        }
        else if(name=="docName")
        {
            formData.docName = value;
        }
    });
    return formData; 
}
function submitSearch(context, type) {
    var searchForm = $('#searchForm');
    //show loading icon
    $(context).toggleIconWithLoadingImg();
    //object of search data
    var searchDataObject = getFormDataSearchObject(searchForm);
    searchDataObject.docName = null;    
    var getDta=function(result)
    {
        //hide loading icon
        $(context).toggleIconWithLoadingImg();
        settingPagination(result, searchDataObject, 'doctorsData');
        mapDoctorsDataToHtmlCards(result.data);
        togglePaginationEvent('bind');//enable click on pagination
    }
    var searchError=function(error)
    {
        //show loading icon
        $(context).toggleIconWithLoadingImg();
        alert(language.cannotLoadData);
    }
    togglePaginationEvent('unbind');//timply disable click on pagination
    getSearchDataByAjax(getDta, searchError, 'doctorsData', searchDataObject);
}
function mapDoctorsDataToHtmlCards(dataRecords) {
    /*dataRecords:doctor data that will maped to html cards*/
    //this is section that will be container of html cards
    var serachSectionRow = $('#searchResult-section .row');
    //this is empty html card that will be cloned by javascript to be loped based on data records
    var card = $('#clonedSection .outerCard').clone().removeClass('hidden');
    $(serachSectionRow).empty();//empty search section container
    if (dataRecords.length == 0) {
        $(serachSectionRow).empty().append("<div class='alert alert-danger'>" + language.noMatchedItemsForSearch + "</div>");
        //this div show only when there are search result
        $('.fast-service').addClass('hidden');
    }
    else {
        //this div show only when there are search result
        $('.fast-service').removeClass('hidden');
        $(dataRecords).each(function (i, doctor) {
            var cardNode = card.clone();//copy new card node
            $(serachSectionRow).append(mapDataRecordToHtmlDoctorCard(cardNode, doctor));
        });
    }//handle given data
    makeScrollTopAnimation("#searchResult-section");
}
function mapDataRecordToHtmlDoctorCard(htmlCard,dataRecord)
{
    newcard = $(htmlCard).find('.doc-card');//find doc-card part
    $(newcard).find('img').attr({ 'src': doctorPersonalImgPath + dataRecord.image, 'onclick':'location.href="'+defaultPathForDoctorPage + dataRecord.id+'"' });
    $(newcard).find('.base ul li').eq(0).find('p>span').text(dataRecord.fname + " " + dataRecord.lname);
    $(newcard).find('.base ul li').eq(1).find('p>span').text(dataRecord.education + " " + dataRecord.spName);
    $(newcard).find('.base ul li').eq(2).find('p>span').text(dataRecord.clinicAddress);
    $(newcard).find('.base ul li').eq(3).find('p>span>span').text(dataRecord.price);
    $(newcard).find('.base ul li').eq(4).find('p>span').text(dataRecord.timing);
    $(newcard).find('.base ul li').eq(5).find('p>span').text(dataRecord.phone);
    $(newcard).find('.base p.viewers span').text(dataRecord.viewers);
    $(newcard).find('.base p.rate i').each(function (i, icon) {
        if (dataRecord.rate >= i + 1) {
            $(icon).removeClass('fa-star-o').addClass('fa-star');
        }
    });
    $(newcard).find('.base button').attr('onclick', 'location.href="' + defaultPathDoctorReservingPage +dataRecord.fname+'/' + dataRecord.id + '"');
    return htmlCard;
}
function handleDocCardDesign() {
    $('body').on('click', '.doc-card .base ul>li >p>span', function () {
        var span = $(this);
        $(span).parent().css({
            'overflow': 'visible',
            'whiteSpace': 'inherit'
        });
        var text = $(span).text();
        $(span).parent().append('<div>' + text + '</div>');
        var newDiv = $(span).parent().find('div');
        $(newDiv).mouseleave(function () {
            $(newDiv).parent().css({
                'overflow': 'hidden',
                'whiteSpace': 'nowrap'
            });
            $(newDiv).parent().append('<span>' + text + '</span>');
            $(newDiv).remove();
        });
        $(span).remove();
    });
}
function validateFastSearch(form, value) {
    if (value.trim() == '') return false;
    var inp = $(form).find('input');
    value = value.replace(/[,/'-_/"@#$%*!(//)?[]/g, '').replace(/ or /g, '');
    inp.val(value);
    if (value.trim() == "") return false;
    var searchDataObject = getFormDataSearchObject($(form));
    searchDataObject.generalSearchPattern = value;
    $(form).find('button:submit i').toggleLoadingIcon('fa-search');
    localStorage.setItem('searchDataObject', JSON.stringify(searchDataObject));
    localStorage.generalSearchValue = value;
    return searchDataObject;
}
/*search bar*/
function setSelectedSearchValue(liContext)
{
    var li = $(liContext);
    var idValue =li.data('value');
    var nameValue = li.data('name'); 
    $(li).parent().next().val(idValue); 
    $(li).parent().siblings('.dropdown-toggle').find('.selectedValue').text(nameValue);
}
function updateDestrictsMenu(destrictsID) {
    var destrictsMenu = $('#destrictsMenu');
    destrictsMenu.prev().find('.selectedValue').text(destrictsMenu.find('li').eq(0).data('name'));
    if (destrictsID == undefined)
    {
        destrictsMenu.find('li').removeClass('hidden');
        return;
    }
    destrictsMenu.find('li').not(':first').each(function (i, li) {
        var destrictID = parseInt($(li).data('value'));
        if (destrictsID.indexOf(destrictID)==-1) {
            $(li).addClass('hidden');
        }
        else {
            $(li).removeClass('hidden');
        }
    });
}
function filterDoctors(context) {
    var value = $(context).val();
    var foundedResult = false;
    $(context).parents('.doctors').find('li.noResult').remove();
    var resetNamesToDefaults = function (lang) {        
        var names=$(context).parents('.doctors')
            .find('li.noResult').remove().end()
            .find('li')
            .removeClass('hidden');
        names.each(function (i, li) {
            if (lang == "en") $(li).find('a').text($(li).data('nameeng'));
            else $(li).find('a').text($(li).data('namear'));
        });
    }
    var namesLi = $(context).parents('.doctors').find('li').not(':first');
    $(namesLi).each(function (i, li) {
        var nameAr = $(li).data('namear');
        var nameEng = $(li).data('nameeng');
        if (nameAr.indexOf(value) >= 0) {
            $(li).removeClass('hidden').find('a').text(nameAr);
            foundedResult = true;
        }
        else if (nameEng.indexOf(value) >= 0) {
            $(li).removeClass('hidden').find('a').text(nameEng);
            foundedResult = true;
        }
        else {
            $(li).addClass('hidden');
        }
    });
    if (value == "" || value.trim() == "") {
        resetNamesToDefaults(currentLangauge);
        foundedResult = true;
    }
    if (!foundedResult) {
            $(context).parents('.doctors').append("<li class='noResult'><a href='#'>" +language.noMathchedResult + "</a></li>");
    }
}
function validateFastSearchForm(buttonContext,type)
{
    $(buttonContext).toggleIconWithLoadingImg();
    if (type == "submit") 
    {
        var form = $('#searchForm');
        localStorage.setItem('searchDataObject', JSON.stringify(getFormDataSearchObject(form)));
        form.submit();
    }
    else {//ajax operation
        var searchDataObject = getFormDataSearchObject($('#searchForm'));  
        var getDta = function (result) {            
            //hide loading icon
            $(buttonContext).toggleIconWithLoadingImg();
            settingPagination(result, searchDataObject, 'doctorsData');
            mapDoctorsDataToHtmlCards(result.data);
            togglePaginationEvent('bind');//enable click on pagination
        }
        var searchError = function (error) {
            //show loading icon
            $(buttonContext).toggleIconWithLoadingImg();
            alert(language.cannotLoadData);
        }
        togglePaginationEvent('unbind');//timply disable click on pagination
        getSearchDataByAjax(getDta, searchError, 'doctorsData', searchDataObject);
    }
}
function settingPaginationOfQuickSearch(totalCards, cards) {
    //cached search object that contain search items
    var searchDataObject = JSON.parse(localStorage.getItem('searchDataObject'));
    //set searched values for forms
    setSearchedValuesPatternsToForms(searchDataObject);
    if ((totalCards / maxOfCardNumbersForSearchPage) <= 1)
    {
        makeScrollTopAnimation("#searchResult-section");
        return;
    } 
    cards = JSON.parse(convertToJsonString(cards));
    var resultObject = { count: parseInt(totalCards), data: cards };
    settingPagination(resultObject, searchDataObject, "doctorsData");
    togglePaginationEvent('bind');//timply disable click on pagination
    makeScrollTopAnimation("#searchResult-section");   
}
function setSearchedValuesPatternsToForms(searchDataObject) {
    var generalSearchPatternValue = searchDataObject.generalSearchPattern;
    if (generalSearchPatternValue != null && generalSearchPatternValue != "") {
        $('#fast-search input[name="generalSearchPattern"]').val(generalSearchPatternValue);
    }
    else {
    }
}
/*doctor page at users area*/
function mapDoctorLinks(urlsString, links) {
    /*this function get all social media links of current doctor from url string
    and each link is found in urls string is shown as icon on html page
    */
    var urlsString = urlsString.replace(/&quot;/gi, "\"");
    var urlsArray = JSON.parse(urlsString);//convert to array of urls
    $(links).each(function (i, a) {
        //social marks are[y|i|l|.....](i=>instagram),(l=>linkedin),(.....)
        var socialMark = $(a).attr('data-value').trim();
        var url = urlsArray.find(function (url) { if (url.s == socialMark) return url; })
        //url object lik{s:"",u:""},s=>social mark,u=>url of social media
        if (url == undefined) {//doctor has not this link for this social media type
            $(a).addClass('disabledLink');
        }
        else {
            $(a).attr("href", url.u);
        }
    });

}
function mapDoctorServices() {
    $.post('/' + defaultPathForDoctorArea + '/getDocServices', {}, function (data, status) {
        var servicesDiv = $('.doc-services .services');
        var services = data.services;
        if (services != '' && services != null) {
            $('.doc-services').removeClass('hidden');
            services = services.split(',');
            $(servicesDiv).empty();
            for (var i = 0; i < services.length; i++) {
                $(servicesDiv).append('<div>' + services[i] + '</div>');
            }
        }
    });
}
/*doctor advices at users area*/
/*other general functions may used at future*/
function compareDate(dashedDate, intDate) {
    if (intDate == null) return false;
    var dashedDates = dashedDate.trim().split('-');
    var dY = parseInt(dashedDates[0]);
    var dM = parseInt(dashedDates[1]);
    var dD = parseInt(dashedDates[2]);
    var intDates = new Date(Number(intDate.match(/\d+/)[0]));
    var intY = intDates.getFullYear();
    var intM = (intDates.getMonth() + 1);
    var intD = intDates.getDate();// console.log(dY + " " + dM + " " + dD + " " + intDate + " " + intY + " " + intM + " " + intD);
    if (dY == intY && dM == intM && dD == intD) return true;
    return false;
}
function getUserStat(result) {
    if (result != undefined) {
        if (result == true || result == false)
            return result;
        else return false;
    }
    callAjax(
      "/users/index/getUserStatus",
       {},
      "Get", "application/json;charset=utf-8",
      "json",
      getUserStat,
      getUserStat
      );
}
function handleLocations() {
   // makeScrollTopAnimation();
   // makeScrollTop();
}

function searchForDoctorName(context) {
    var textName = $(context).val();
    var ulName = $(context).next();
    var isFoundDoctorName = false;
    var spID = parseInt($(ulName).attr('data-value'));
    if (textName == null || textName.trim() == '') {
        $(ulName).find('li.noResult').remove();
        $(ulName).find('li').each(function (i, li) {          
            var docSpID = parseInt($(li).attr('data-value').split('-')[1]);
            if ((spID == docSpID || spID == 0) &&$(li).hasClass(lang)) {
                $(li).removeClass('hidden');
                isFoundDoctorName = true; 
            }
            else {
                $(li).addClass('hidden'); 
            }
        });
    }
    else {
        $(ulName).find('li.noResult').remove();
        $(ulName).find('li').each(function (i, li) {
            var docName = $(li).text();
            var docSpID = parseInt($(li).attr('data-value').split('-')[1]);
            if (docName.search(textName) != -1 && (spID == docSpID || spID == 0)) {
                isFoundDoctorName = true;
                $(li).removeClass('hidden');
            }
            else {
                $(li).addClass('hidden');
            }
        });
    }
    if (!isFoundDoctorName) {
        $(ulName).append("<li class='noResult'>لا يوجد نتائج</li>").find('li.noResult').css('cursor', 'default').hover(
            function () {
                $(this).css({ 'background': '#fff', 'color': '#00f' });
            });
    }

}
function filterDoctorBasedOnSp(context) {
    var spID = parseInt($(context).find('option:selected').val());
    var ulName = $('.selectDoctor');
    var noResult = (currentLangauge== "ar") ? "لا يوجد أى طبيب" : "No Doctor Founded";
    $(ulName).prev().val('');
    $(ulName).prev().attr('data-value', '');
    $(ulName).attr('data-value', spID);
    var isFoundDoctorName = false;
    $(ulName).find('li.noResult').remove();
    $(ulName).find('li').each(function (i, li) {
        var docSpID = parseInt($(li).attr('data-value').split('-')[1]);
        if (spID == 0 &&$(li).hasClass(lang)) {
            isFoundDoctorName = true;
            $(li).removeClass('hidden');
        }
        else if (spID != docSpID ||!$(li).hasClass(lang)) {
            $(li).addClass('hidden');
        }
        else if (spID == 0 &&!$(li).hasClass(lang)) {
            $(li).addClass('hidden');
        }
        else {
            isFoundDoctorName = true;
            $(li).removeClass('hidden');
        }
    });
    if (!isFoundDoctorName) {
        $(ulName).append("<li class='noResult'>"+noResult+"</li>").find('li.noResult').css('cursor', 'default').hover(function () {
            $(this).css({ 'background': '#fff', 'color': '#00f' });
        });
    }
}
function setInputVal(context) {
    $(context).parent().prev().val($(context).text());
    $(context).parent().prev().attr('data-value', $(context).attr('data-value').split('-')[0]);
    $(context).parent().addClass('hidden');
}
function mapDoctorEdu(elments, lang) {
    var arr = [];
    arr['professor'] = "أستاذ";
    arr['lecturer'] = "دكتور";
    arr['specialist'] = "متخصص";
    arr['Assistant Lecturer'] = "دكتور مساعد";
    arr['Assistant Professor'] = "استاذ مساعد";
    arr['Consaltant'] = "استشارى";
    arr['FellowShip'] = "مرافق";
    $(elments).each(function (i, el) {
        var name = $(el).text().split(' ')[0];
        var result = "";
        if (currentLangauge== "ar") {
            result = arr[name.toLocaleLowerCase()] + ' فى';
            if (result == undefined) result = 'دكتور فى';
        }
        else {
            result = name + ' in';
            if (result == undefined) result = 'lecturer in';
        }
        $(el).text($(el).text().replace(name, result));
    });
}
function mapAdvicesUrls(context) {
    var urls = $(context).attr('data-value');
    if (urls == null || urls == '') { urls = ""; }
    var socialDiv = $(context);
    $(socialDiv).removeClass('hidden');
    var links = $(socialDiv).find('a');
    $(links).each(function (i, a) {
        var socialType = $(a).attr('data-value');
        if (urls.search('social-' + socialType + '-') != -1) {
            var startIndex = urls.indexOf('social-' + socialType + '-');
            var endIndex = urls.indexOf(',end,', startIndex + 1);
            var linkevalue = urls.substring(startIndex, endIndex);
            linkevalue = linkevalue.replace('social-' + socialType + '-', '');
            $(a).attr('href', linkevalue);
        }
        else {
            $(a).css({ 'opacity': '.5', 'cursor': 'not-allowed' }).click(function () { return false; });
        }

    });
}
function FillAdvice(result) {
    var arr = [];
    if (currentLangauge== "ar")
    {
        arr['professor'] = "أستاذ";
        arr['Lecturer'] = "دكتور";
        arr['Specialist'] = "أخصائى";
        arr['Assistant Lecturer'] = "دكتور مساعد فى";
        arr['Assistant Professor'] = "استاذ مساعد فى";
        arr['Consaltant'] = "استشارى";
        arr['FellowShip'] = "مرافق";
    }
    else {
        arr['professor'] = "professor of";
        arr['Lecturer'] = "lecturer of";
        arr['Specialist'] = "Specialist of";
        arr['Assistant Lecturer'] = "Assistant Lecturer of";
        arr['Assistant Professor'] = "Assistant Professor of";
        arr['Consaltant'] = "Consaltant of";
        arr['FellowShip'] = "FellowShip of";
    }
    var doctorName = (currentLangauge== "ar") ? "د." : "Dr.";
    var docAdviceCloned = $('.clonedAdvice').first();
    $(docAdviceCloned).find('.card .socials>a').css({ 'cursor': 'pointer', 'opacity': '1' });
    var advicesDiv = $('.all-advices .advices');
    $(advicesDiv).empty();
    for (var i = 0; i < result.length; i++) {
        var docAdvice = result[i];
        var newAdvicehtml = $(docAdviceCloned).clone();
        newAdvicehtml = $(newAdvicehtml).removeClass('hidden clonedAdvice')
        $(newAdvicehtml).find('img').attr('src', '/Areas/doctors/doctorImages/' + docAdvice.docImage);
        $(newAdvicehtml).find('.card .record:first-of-type').find('.name h5').text(doctorName+ docAdvice.docName);
        $(newAdvicehtml).find('.card .record:nth-of-type(2)').find('.name h5').text(arr[docAdvice.docEdu]+" "+ docAdvice.spName)
        $(newAdvicehtml).find('.card .socials').attr('data-value', docAdvice.docUrls);
        mapAdvicesUrls($(newAdvicehtml).find('.card .socials'));
        $(newAdvicehtml).find('.content').text(docAdvice.adviceContent);
        $(newAdvicehtml).find('.notifications>div').eq(0).find('>.number').text(docAdvice.count);
        $(newAdvicehtml).find('.notifications>div').eq(1).find('>.number').text(docAdvice.shares);
        $(newAdvicehtml).find('.notifications>div').eq(2).find('>.number').text(docAdvice.seen);
        $(newAdvicehtml).find('.notifications>div').eq(3).find('>.number').text(docAdvice.likes);
        $(newAdvicehtml).find('.hidden-div button[type="button"]').attr('data-value', docAdvice.id);
        $(newAdvicehtml).find('.hidden-div button.oldComments').attr('onclick', 'getOldCommentsOnAdvices(this,' + docAdvice.id + ',' + docAdvice.count + ')')
        $(newAdvicehtml).find('.notifications>div>i.fa-comments-o').attr('onclick', 'showCommentsOnAdvice(this,' + docAdvice.id + ',' + docAdvice.count + ');')
        $(newAdvicehtml).find('.notifications>div:last-of-type>i').attr('onclick', 'incrementNotifByOne(this,' + docAdvice.id + ',"like");')
        $(newAdvicehtml).find('.notifications>div:nth-of-type(3)>i').attr('onclick', 'incrementNotifByOne(this,' + docAdvice.id + ',"seen");')
        $(newAdvicehtml).find('.notifications>div:nth-of-type(2)>i').attr('onclick', 'incrementNotifByOne(this,' + docAdvice.id + ',"share");')
        if (docAdvice.likeStat) {
            var likeIcon = $(newAdvicehtml).find('.notifications>div:last-of-type>i');
            $(likeIcon).removeClass('fa-heart-o').addClass('fa-heart');
            $(likeIcon).attr('title', 'unlike');
        }
        else {
            var likeIcon = $(newAdvicehtml).find('.notifications>div:last-of-type>i');
            $(likeIcon).addClass('fa-heart-o').removeClass('fa-heart');
            $(likeIcon).attr('title', 'like');
        }
        $(advicesDiv).append(newAdvicehtml);
    }
}
function getAdvices(pageNumber) {
    $('.all-advices h3').find('i').removeClass('fa-info').addClass('fa-circle-o-notch fa-spin');
    $.post('/users/advices/advices', { pageNumber: pageNumber }, function (result, status) {
        $('.all-advices h3').find('i').addClass('fa-info').removeClass('fa-circle-o-notch fa-spin');
        if (result.length > 0 && result != false) {
            FillAdvice(result);
        }
    });
}
function getAdvicesOfDoctorOrSpeciality(context, pageNumber) {
    var docID = parseInt($(context).parents('form').find('input').attr('data-value'));
    var spID = parseInt($(context).parents('form').find('ul.selectDoctor').attr('data-value'));
    if (spID == 0 && isNaN(docID)) return;
    if (spID == 0 && docID == 0) return;
    if (isNaN(docID)) docID = 0;
    if (document.cookie.search('currentAdvicesPageNumber') == -1) {
        document.cookie = "currentAdvicesPageNumber=" + pageNumber + ";expires=new Date(new Date().setDate(new Date(Date.now()).getDate()+1));path=/";
    }
    $(context).find('i').removeClass('fa-search').addClass('fa-circle-o-notch fa-spin');
    $.post('/users/advices/doctorOrSpecialityAdvices', { pageNumber: pageNumber, spID: spID, docID: docID }, function (data, status) {
        var result = data.result;
        $(context).find('i').addClass('fa-search').removeClass('fa-circle-o-notch fa-spin');
        if (result.length > 0 && result != false) {
            FillAdvice(result);
            settingPaginationDiv(data.count, pageNumber);
        }
        else {
            var advicesDiv = $('.all-advices .advices');
            $(advicesDiv).empty();
            $(advicesDiv).append('<div class="alert alert-danger">لا يوجد اى نتائج متعلقة بالبحث</div>');
        }
    });

}
function settingPaginationDiv(count, pageNumber) {
    var pagination = $('.pagination');
    var end = parseInt(Math.ceil(count / 10.0));
    var ul = $(pagination).find('ul');
    $(ul).empty();
    if (end > 2) {
        $(ul).append('<li class="first edge"><a href="#">«</a></li>');
    }
    if (count > 10) {
        $(pagination).find('ul').addClass('getParticularAdvices');
        for (var i = 1; i <= end; i++) {
            if (i == pageNumber) {
                $(ul).append('<li><a class="w3-red" href="#">' + i + '</a></li>');
            }
            else {
                $(ul).append('<li><a href="#">' + i + '</a></li>');
            }
        }
    }
    if (end > 2) {
        $(ul).append('<li class="last edge"><a href="#">»</a></li>');
    }
}
function showCommentsOnAdvice(context, adviceID, commentsNumber) {
    $('.loadingComment').removeClass('hidden');
    var commentDiv = $(context).parents('.advice').find('.hidden-div');
    commentDiv.css({ 'display': 'block' });
    $(commentDiv).find('.comments .alert').remove();
    if (commentsNumber == 0) {
        $('button.oldComments ').addClass('hidden');
        $('.loadingComment').addClass('hidden');
        $(commentDiv).find('.comments').append('<div class="alert alert-danger">لا يوجد اى تعليقات من قبل المرضى</div>');
        return false;
    }
    var blockNumber = parseInt($(context).attr('data-value'));
    $.ajax({
        url: "/users/advices/getAdviceComments",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        data: JSON.stringify({ blockNumber: blockNumber, adviceID: parseInt(adviceID) }),
        success: function (result) {
            if (result.data.length > 0) $(commentDiv).find('.comments').empty();
            $('button.oldComments ').removeClass('hidden');
            var morecomments = "";
            var totalComments = parseInt($(commentDiv).find('button.oldComments').first().attr('data-value'));
            var remainComments = totalComments - (blockNumber + 1) * 10;
            if (remainComments == 1) morecomments = "تعليق واحد اخر";
            if (remainComments == 2) morecomments = "تعليقين اخرين";
            if (remainComments > 2 && remainComments <= 10) morecomments = "يوجد " + remainComments + " تعليقات اخرى";
            if (remainComments > 10) morecomments = "يوجد " + remainComments + " تعليق اخر";
            $(commentDiv).find('button.oldComments').text(morecomments);
            $('.loadingComment').addClass('hidden');
            for (var i = 0; i < result.data.length; i++) {
                var html = "";
                html += '<div class="row comment">';
                html += '<div class="col-sm-3 col-xs-12 pull-left name">' + result.data[i].name + '</div>';
                html += '<div class="col-sm-9 col-xs-12 pull-right comment-content">' + result.data[i].comment + '</div>';
                html += '</div>';
                $(commentDiv).find('.comments').prepend(html);
            }
            if (result.isLastBlock || result.length == 0) {
                $('button.oldComments ').addClass('hidden');
            }
        },
        error: function (errormessage) {
            alert("جدث خطأ اثناء الاتصال بالسيرفر");
        }
    });

}
function addCommentOnAdvice(context, comment) {
    if (comment == null) {
        return false;
    }
    if (comment.trim().length == 0) return false;
    var id = parseInt($(context).attr('data-value'));
    $(context).find('i').removeClass('fa-search').addClass('fa-circle-o-notch fa-spin');
    $.post('/users/advices/addCommentOnAdvice', { adviceID: id, comment: comment }, function (data, status) {
        $(context).find('i').addClass('fa-search').removeClass('fa-circle-o-notch fa-spin');
        if (data == false) {
            alert("جدث خطأ اثناء الاتصال بالسيرفر");
        }
        else if (data == 'notLogged') {
            if (confirm('من فضلك قم بتسجيل الدخول اولا,هل تريد ان تسجل الدخول الان')) {
                gotoLoginPage();
            }
        }
        else {
            var commentDiv = $(context).parents('.advice').find('.hidden-div');
            var html = "";
            html += '<div class="row comment" style="display:none">';
            html += '<div class="col-sm-3 col-xs-12 pull-left name">' + data.name + '</div>';
            html += '<div class="col-sm-9 col-xs-12 pull-right comment-content">' + comment + '</div>';
            html += '</div>';
            $(commentDiv).find('.comments').prepend(html).find('.comment').fadeIn();
            $(context).parents('form').find('input').val('');
            var commentsNum = parseInt($(context).parents('.advice').find('.notifications>div').eq(0).find('>.number').text());
            $(context).parents('.advice').find('.notifications>div').eq(0).find('>.number').text((commentsNum + 1));
        }
    });
}
function getOldCommentsOnAdvices(context, adviceID, totalComments) {
    var commentIcon = $(context).parents('.advice').find('.notifications i.fa-comments-o');
    var blockNumber = parseInt($(commentIcon).attr('data-value'));
    if (totalComments > (blockNumber + 1) * 10)
        blockNumber += 1;
    $(commentIcon).attr('data-value', blockNumber);
    showCommentsOnAdvice(commentIcon, adviceID);
}
function incrementNotifByOne(context, adviceID, name) {
    $.post('/users/advices/incrementAdviceNotificationByOne', { adviceID: adviceID, type: name }, function (data, st) {
        if (data != true) {
            alert("جدث خطأ اثناء الاتصال بالسيرفر");
        }
        else {
            var num = parseInt($(context).next().text());
            var title = $(context).attr('title');
            if ($(context).hasClass('fa-heart') || $(context).hasClass('fa-heart-o')) {
                if (title == "like") {
                    $(context).attr('title', 'unlike');
                    $(context).removeClass('fa-heart-o').addClass('fa-heart');
                    $(context).next().text((num + 1));
                }
                else {
                    $(context).attr('title', 'like');
                    $(context).addClass('fa-heart-o').removeClass('fa-heart');
                    $(context).next().text((num - 1));
                }
            }
        }
    });
}
function getPatientAppointements(context, type) {
    $(context).find('i').removeClass('hidden');
    var appoint = $('').clone();
    $.post('/users/patient/getAppointements', { type: type }, function (data, status) {
        $(context).find('i').addClass('hidden');
        if (data.length > 0 && data != false) {
            $('.allAppoints').empty();
            for (var i = 0; i < data.length; i++) {

            }
        }
    })
}
function validateLange(form, currLang) {
    var currentLangauge= $(form).find('input:checked').val();
    currentLangauge= (currentLangauge== "false") ? "ar" : "en";
    if (currcurrentLangauge== lang) return false;
    return true;
}


 