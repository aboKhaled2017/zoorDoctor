/// <reference path="ar-EG-JS.js" />
/// <reference path="mainJS.js" />
/// <reference path="mainPage.js" />
/*this file is for all patient pages
profile page
appointement page,and so on
*/
function onChangeProfileName(nameContext) {
    nameContext = $(nameContext);
    var oldUserName = nameContext.attr('dataname').trim();
    var newuserName = nameContext.val().trim()
    if (oldUserName != newuserName) {
        if (!confirm(language.changeProfileNameAlert)) {
            nameContext.val(oldUserName);
            $('.top-nav .log .loged p .name').text(oldUserName);
        }
        else {
            $('.top-nav .log .loged p .name').text(newuserName);
        }
    }
}
$(function () {
    $('.top-nav .loged p i').click(function () {
        $('.top-nav .loged .profile').toggle();
    });
    $('i.showPassword').on({
        mouseenter: function () {
            $(this).parent().find('input[type="password"]').attr('type', 'text');
        },
        mouseleave: function () {
            $(this).parent().find('input[type="text"]').attr('type', 'password');
        }
    });
    $('#rememberPassword #verificationWay :radio').change(function () { changeResetPasswordForm() });
    /*patient appointement*/
    $('.patientAppointement .tabs li').click(function () {
        $(this).addClass('active').siblings().removeClass('active');
        if ($(this).index() == 0) {
            location.href = defaultPathPatients+"appointements";
        }
        else if ($(this).index() == 1) {
            location.href = defaultPathPatients+"appointements?type=" + false;
        }
    });
    $('.cancelAppointement').click(function () {
        var appointID = $(this).data('value');
        cancelPatientAppointement(this,appointID);
    });
    /*patient appointement*/
});
function cancelPatientAppointement(context, appointementID) {
    if (!confirm(language.cancelAppointementAlert)) return;
    $(context).toggleLoadingIcon('fa-close');
    $.post(defaultPathPatients + "cancelPatientAppointement", { id: appointementID }, function (data, status) {
        if (data) {            
            alert(language.notifyPatientWithCancelAppointement);
            $(context).parents('.appointement').slideUp(1000);
        }
        else {
            alert(language.notSuccessOperation);
        }
    }).complete(function () {
        $(context).toggleLoadingIcon('fa-close');
    });
}
//reset password and change password pages
function changeResetPasswordForm() {
    var emailOrPhoneFormGroup = $('#rememberPassword').find('#byEmail,#byPhone').toggleClass('hidden');
    $('#sendVerify').toggleText();
    emailOrPhoneFormGroup.find('input').each(function (i, inp) { $(inp).toggleAttribute('required'); });
}
function validateresetpassword(context) {
    context = $(context);
    if ($(context).val().match("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,}") == null) {
        $(context).val('');
        $(context).attr('placeholder', language.enterValidPassword);
    }
}
function confirmPassword(context) {
    var form = $(context).parents('form');
    if (form.find('input[type="password"]').val().trim() !== $(context).val().trim()) {
        form.find('.confirmPassValidationMessage').text(language.twoPasswordNotEqual);
    }
    else {
        form.find('.confirmPassValidationMessage').text("");
    }
}