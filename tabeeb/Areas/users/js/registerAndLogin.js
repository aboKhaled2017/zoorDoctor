﻿/// <reference path="../../../Scripts/jquery-1.12.4.js" />
/// <reference path="mainPage.js" />
/// <reference path="extension.js" />
/// <reference path="en-US-Js.js" />
/// <reference path="../../../Scripts/jquery.validate.unobtrusive.js" />
/// <reference path="../../../Scripts/jquery.validate.js" />
var sdkISLoaded = false;
var startSdkLoad=null;
function checkPatientProperty(input, property) {
    var inputValue = $(input).val();
    //if you want unique phone ,delete the following comment
    if (property == "phone") return;//not check phone [temporarly]will be edit fater this
    inputValue = (inputValue.length > 0) ? inputValue.trim() : "";
    var formGroup = $(input).parent();
    if (
        (property == "username" && (inputValue == "" || inputValue == undefined || inputValue.length < 3))
        ||
        (property == "phone" && (inputValue.match('[0-9]{11}') == null || inputValue.length > 11))
        ) {
        formGroup.find('label .glyphicon-ok-sign').addClass('hidden');
        formGroup.find('label .fa-close').addClass('hidden');
        return;
    }
    formGroup.find('label .fa-spin').removeClass('hidden'); 
    var errorMessage = (property == "username") ? language.userNameIsAlreadyExists:language.phoneNumberIsAlreadyExists;
    $.post(defaultPathPatients + 'IsUserNameOrPhoneExists', { value: inputValue, propertyType: property },
        function (isExistsProperty, status) {
            formGroup.find('label .fa-spin').addClass('hidden');
            if (isExistsProperty) {
                formGroup.find('label .glyphicon-ok-sign').addClass('hidden');
                formGroup.find('label .fa-close').removeClass('hidden');
                var inputValidator = $(input).removeClass('valid').addClass('invalid').next().addClass('field-validation-error').removeClass('field-validation-valid');
                if (inputValidator.find('#' + $(input).attr('id') + '-error').text()== "") {
                    inputValidator.append('<span id="' + $(input).attr('id') + '-error" class="">' + errorMessage + '</span>');
                    $(input).keyup(function () {
                        $('#' + $(input).attr('id') + '-error').remove();
                        formGroup.find('label .fa-close').addClass('hidden');
                        $(this).removeClass('invalid');
                        $(this).unbind('keyup');
                    });
                }
            }
            else {
                formGroup.find('label .glyphicon-ok-sign').removeClass('hidden');
                formGroup.find('label .fa-close').addClass('hidden');              
            }
        });
}
function validatePatientForm(form) {
    var isValidated = true;
    if ($(form).find('input').hasClass('invalid'))
        isValidated = false;
    var checkInp = $(form).find('input[type="checkbox"]');
    if (!$(checkInp).is(':checked')) {
        $(checkInp).parents('.form-group').css('border', '1px solid #f00');
        $(checkInp).change(function () {
            $(this).parents('.form-group').css('border', '0');
        });
        isValidated = false;
    }
    return isValidated;
}
$(function () {
    loadLocalScript("/validationLinks", function (isLoaded) { });
    loadLocalScript("/Areas/users/js/sdk.js", function (isLoaded) { clearInterval(startSdkLoad); });
    /*var loadSdk=function()
    {//if we want to load main js first
        if(IsMainJsLoaded)
            loadLocalScript("/Areas/users/js/sdk.js", function (isLoaded) { clearInterval(startSdkLoad); });
    }
    startSdkLoad = setInterval(loadSdk,4);*/
    
});
function validateExternalForm(context)
{
    var isValidPhone = true, isValidGender = true, IsValidBirthDate = true;
    var externalForm = $('#externalForm');
    var patientObject = externalForm.data('patient');
    if (patientObject.type == null)
    {//type must be inputed
        if (!externalForm.validate().element(externalForm.find('#type'))) {
            isValidGender = false;
        }
        else {
            patientObject.type = externalForm.serializeArray()[2].value;
        }
    }
    if (patientObject.birthDate == null)
    {//birthdate must be inputed
        if (!externalForm.validate().element(externalForm.find('#birthDate'))) {
            IsValidBirthDate = false;
        }
        else {
            patientObject.birthDate = externalForm.find('#birthDate').val();
        }
    }
    if(!externalForm.validate().element(externalForm.find('#phone')))
    {//phone must be inputed
        isValidPhone = false;
    }
    else {
        patientObject.phone = externalForm.find('#phone').val();
    }
    if (isValidGender&&isValidPhone&&IsValidBirthDate)
    {
        submitExternalLoginForm(patientObjectData, context);
    }
    else {
        return false;
    }
    
}
function submitExternalLoginForm(patientObjectData,context)
{
    patientObjectData.type = (patientObjectData.type == "True" || patientObjectData.type==true) ? true : false;
    $('#externalForm').data('patient', patientObjectData);
    $(context).text('').append("<i class='fa fa-circle-o-notch fa-spin'></i>");
    $.ajax({
        url: defaultPathPatients + "externalRegister",
        data: JSON.stringify(patientObjectData),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if(patientObjectData.providerName=="facebook")
            $('.facebook button').find('i').toggleLoadingIcon('fa-facebook');
            else $('.gmail button').find('i').toggleLoadingIcon('fa-google-plus');
            $(context).text(language.login).find('i.fa-spin').remove();
            if (result) {
                $('#externalLogin').modal('hide');
                location.href = defaultPathUsers;
            }
            else {
                    $(context).text(language.login).find('i.fa-spin').remove();
                    $('#externalLogin form p.form-alert').text(language.loginFaild).css('color','red').removeClass('hidden');
                }            
        },
        error: function (errormessage) {
            if (patientObjectData.providerName == "facebook")
                $('.facebook button').find('i').toggleLoadingIcon('fa-facebook');
            else $('.gmail button').find('i').toggleLoadingIcon('fa-google-plus');
            $(context).text(language.login).find('i.fa-spin').remove();
            $('#externalLogin').modal('hide');
            alert(language.loginPoblemAtServer);
        }
    });
}