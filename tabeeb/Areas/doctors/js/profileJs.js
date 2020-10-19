/// <reference path="../../../scripts/jquery-1.12.4.js" />
/// <reference path="../../../Scripts/jquery.validate.unobtrusive.js" />
/// <reference path="doctorJS.js" />
/// <reference path="../../../Scripts/jquery.validate.js" />
var currentTab = 0; // Current tab is set to be the first tab (0)
var tabs = $(".tab");//all tabs
var validateElement = function (element) {
    return $("#profileForm").validate().element($(element));//validate image and show error
}
$(function () {
     showTab(currentTab); // Display the current tab
    //default is ignor:'hidden'
    $.validator.setDefaults({ ignore: "input[type='url']" });//to validate any fields except url input fields
    //on key up get trim value 
    $('#profileForm').find('input[type!="file"],textarea').keyup(function () {
        $(this).removeClass('invalid');
        $(this).css({ 'border-color': 'lightgray', 'color': '#000' }).attr('data-value', '');
    }).blur(function () {
        $(this).val($(this).val().trim());
    });
    //when user change his profession
    $('#education').change(function () {
        $("#profileForm").validate().element('#education');
    });
    //when user update his image
    $('#uploadProfileImage').change(function (e) {
        var _URL = window.URL || window.webkitURL;
        var imgLoaded = false;
        var imgInput = $(this);
        var files = e.target.files;
        ImageTools.resize(files[0], {
            width: 120, // maximum width
            height: 120 // maximum height
        }, function (blob, didItResize) {
            files[0] = blob;
            var fileSize = files[0].size;
            var extension = $(imgInput).val().split('.').pop().toUpperCase();
            var filename = $(imgInput).val().split('\\').pop();
            if (!(extension == "PNG" || extension == "JPG" || extension == "GIF" || extension == "JPEG")) {
                alert(language.imgExtension);
                $('#image').val("");
                validateElement('#image');
                return;//file extension not valid
            }
            if (files[0].type.indexOf("image") == -1) {
                alert(language.imgFileNotSupported);
                $('#image').val("");
                validateElement('#image');
                return;//this is not image
            }
            //$('#imageProgress').removeClass('hidden');
            if (fileSize > 0) {
                var img = new Image();
                img.src = _URL.createObjectURL(files[0]);
                img.onload = function () {
                    var width = img.naturalWidth,
                    height = img.naturalHeight;
                    if (width != height || (fileSize > maxPersonalImg)) {
                        alert(language.personalImgValidation);
                        $('#image').val("");
                        validateElement('#image');
                        return;
                    }
                    else {//image has been accepted
                        alert(language.imgLoadedSuccess);
                        $('#image').val(filename);
                        validateElement('#image');
                    }
                }
            }
        }); 
        
    });
    /* when user leave url fields
    this function validate links that user will input
    */
    $('.doctorSocial').find('input[type="url"]').blur(function () {
        //regular expression for url
        var notVliadMessgae = (lang == "en")
        ? 'this is not valid ' : 'هذا الرابط غير صحيح ل ';
        var linkWord = (lang == "en") ? "link" : "";
        var notValidLink=function(element,socialType)
        {
            $(element).css({ 'border-color': 'red', 'color': 'red' }).val(notVliadMessgae + socialType + linkWord);
        }
        var isValidUrl = true;
        var pattern = /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;
        var socialType = $(this).data('class').trim();
        var urlValue=$(this).val().trim();
        if (urlValue.trim().length > 0)
        {
            if (socialType == "f" && urlValue.search('www.facebook.com') == -1) {//check if url is valid facbook link
                notValidLink(this, "facebook");
                isValidUrl = false;
            }
            if (socialType == "f" && urlValue.search('www.facebook.com') == -1) {//check if url is valid facbook link
                notValidLink(this, "facebook");
                isValidUrl = false;
            }
            if (socialType == "l" && urlValue.search('www.linkedin.com') == -1) {//check if url is valid linkedin link
                notValidLink(this, "linkedin");
                isValidUrl = false;
            }
            if (socialType == "y" && urlValue.search('www.youtube.com') == -1) {//check if url is valid youtube link
                notValidLink(this, "youtube");
                isValidUrl = false;
            }
            if (socialType == "i" && urlValue.search('www.instagram.com') == -1) {//check if url is valid instagram link
                notValidLink(this, "instagram");
                isValidUrl = false;
            }
            if (socialType == "g" && urlValue.search('www.google.com') == -1) {//check if url is valid google link
                notValidLink(this, "google");
                isValidUrl = false;
            }
            if (socialType == "t" && urlValue.search('www.twitter.com') == -1) {//check if url is valid twitter link
                notValidLink(this, "twitter");
                isValidUrl = false;
            }
        }
        if(isValidUrl&&urlValue.trim().length>0)
        {
            var errorurl = (lang == "en")
            ? 'enter valid url'
            : 'ادخل رابط صحيح';
            if (!pattern.test(urlValue.trim())) {//url is not valid
                $(this).css({ 'border-color': 'red', 'color': 'red' }).val(errorurl);
            }
            else
            {//add this url to urls input
                setDoctorUrlsToUrlsField();
            }
        }
    }).keyup(function () {
        $(this).css({ 'border-color': 'lightgray', 'color': '#000' });
    });
    /*function of profile page*/
    $('.doctorProfileForm .form-control').keydown(function () {
        $('.doctorProfileForm .errorMessage').addClass("hidden");
    });
});
function showTab(n) {
   // This function will display the specified tab of the form ...
    $(tabs).eq(n).fadeIn();
    // ... and fix the Previous/Next buttons:
    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }
    if (n == (tabs.length - 1)) {       
        $("#nextBtn").html(""+language.submit+" <i class='fa fa-send'></i>");
    } else {
        $("#nextBtn").html(""+language.next+" <i class='fa fa-arrow-right'></i>");
    }
    // ... and run a function that displays the correct step indicator:
    fixStepIndicator(n)
}
function nextPrev(n) {
    // This function will figure out which tab to display
    //n==1[go t next tab],n==-1[go to previous tab]
    //submit the current tab to get error
    if (n == 1) {
        $("#profileForm").validate().form();
        //focus on invalid element
        $("#profileForm").validate().focusInvalid();
    }
    //check if there any error in the submitted tab
    if (n == 1 && !SubmittedtabIsValid(tabs.eq(currentTab))) return false;
    // Exit the function if any field in the current tab is invalid:
    if (n == 1 && !validateForm()) return false; 
    // Hide the current tab:
    $(tabs).eq(currentTab).fadeOut(); 
    // Increase or decrease the current tab by 1:
    currentTab = currentTab + n;
    //clear any error on next tab
    if (n == 1) clearTabError(tabs.eq(currentTab));
    // if you have reached the end of the form... :
    if (currentTab >= tabs.length && $("#profileForm").validate().numberOfInvalids() == 0)
    {
        //...the form gets submitted:
        $("#profileForm").submit();
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}
function SubmittedtabIsValid(submittedTab)
{
    var isValidTab = true;
    if ($(submittedTab).find('.input-validation-error,.invalid').length > 0)
        isValidTab = false;
    return isValidTab;
}
function clearTabError(nextTab)
{
    var invalidControl=$(nextTab).find('.input-validation-error');
    invalidControl.removeClass('input-validation-error').addClass('valid');
    $('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').empty();
}
function validateForm() {
    // This function deals with validation of the form fields
    var tab, valid = true;
    tab =$(".tab");
    if (currentTab == 2) valid = validateProfileTab3();
    // If the valid status is true, mark the step as finished and valid:
    if (valid) {
        document.getElementsByClassName("step")[currentTab].className += " finish";
    }
    return valid; // return the valid status
}
function fixStepIndicator(n) {
    // This function removes the "active" class of all steps...
    var steps =$(".step");
        steps.removeClass("active");
    //... and adds the "active" class to the current step:
        $(steps[n]).addClass("active");
}
function validateProfileTab3()
{
    var isValid = true;
    var imgValue = $('#image').val().trim();
    var imagePattern = /^[^\s]+\.[a-zA-Z]{3,4}$/i;
    if(!imagePattern.test(imgValue))
    {
        $("#profileForm").validate().element('#image');
        $('#image').val('');
        isValid = false;
    }
    var urls = $('.doctorSocial').find('input[type="url"]');
    urls.each(function (i, url) {
        var inp = $(url).get(0);
        if (!inp.checkValidity()) {
            $(url).css({ 'border-color': 'red', 'color': 'red' }).val(language.notValidUrl);
            isValid = false;
        }
    });
    return isValid;
}
function validateProfileForm(form)
{
    if ($("#profileForm").validate().numberOfInvalids() > 0) return false;
    if ($('#urls').val().trim() == "") $('#urls').val("[]");
    return true;
}
/*this function get all social media links from input[type=url]that doctor entered
    and concatinating these links in some format to one string 
    and set this composite string to urls input
    and at server ,this string is splited into multi sub-string 
    this sub-string simulate each url of social media links
    *(/^\[(\{"s":"(y|f|l|i|t|g)","u":"(.)+"\}(,){1})*\]$/)*/
function setDoctorUrlsToUrlsField() {   
    var urls = [];//object contains array of url object
    //url object format is like {s:socialType,u:url}
    //urlInputs simulate all inputs of type url
    $('.doctorProfileForm > .generalProfile .doctorSocial input[type="url"]').each(function (i, url) {
        //value for one of url inputs
        var urlValue = $(url).val().trim();
        //socialType is like[y|f|l|i] y means=>youtyoub,f=>facebook and so on...
        var socialType = $(url).attr('data-class').trim();
        if(urlValue!="")//if linke has value
            urls.push({ s: socialType, u: urlValue });
    });
    $('#urls').val(JSON.stringify(urls));
}
function fillUrlInputFieldsWithLinks() {
    /*this function get string from url hidden input
    this string contains all doctor links of his/her social media
    this string is formated with some way(concatinating all social media links in one url string)
    */
    var urlsString = $('#urls').val().trim();
    var urls = JSON.parse(urlsString);
    if (urlsString == "[]") return;
    $('.doctorProfileForm > .generalProfile .doctorSocial input[type="url"]').each(function (i, urlInput) {
        var socialType = $(urlInput).data('class');
        for(var i=0;i<urls.length;i++)
        {
            var url = urls[i];
            if(url.s==socialType)
            {
                $(urlInput).val(url.u);
                break;
            }
        }
    });
}