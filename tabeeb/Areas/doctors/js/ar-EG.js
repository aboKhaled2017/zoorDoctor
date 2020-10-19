var language = {};
var lang = "ar";
language.serverProblem = "لقد حدثت مشكلة اثناء جلب البيانات من السيرفر";
language.serverOperationProblem = "لقد حدثت مشكلة اثناء تنفيذ العملية من السيرفر,حاول فى وقت لاحق";
language.imgExtension = "امتداد الصورة لابد ان من الامتدادات الاتية png ,jpg,gif,jpeg";
language.chhoseValidImg = "من فضلك اختار صورة بشكل صحيح";
language.personalImgValidation = "الصورة الشخصية لابد انت تكون لها طول وعرض متساويان وحجمها لا يزيد عن 1 ميجا بايت";
language.professionImgValidation = "الصورة لايزيد حجمها عن 2 ميجابايت ";
language.imgFileNotSupported = "هذا الملف غير مدعوم";
language.imgLoadedSuccess = "تم تحميل الصورة بنجاح";
language.mustAgreeOntermOfuser = "لابد من الموافقة على سياسة الاستخدام";
language.enterValidUsernameOrPassword = "ادخل كلمة سر او اسم مستخدم صحيحة";
language.showPassword = "اظهار كلمة السر";
language.hidePassword = "اخفاء كلمة السر";
language.logoutAfterChangeUsename = "تغيير اسم المستخدم سوف يؤدى بك الى الخروج من الموقع ,والدخول مرة اخرى  بأسم المستخدم الجديد وكلمة السر.هل موافق على الخروج";
language.submit = "حفظ البيانات";
language.next = "التالى";
language.notValidUrl = "هذا الرابط غير صحيح";
language.noReservingToday = "ليس لديك حجوزات اليوم";
language.noWrittenAdvices = "ليس لديك اى نصائح مدونة  من قبل";
language.aboutNumber = "حوالى";
language.otherAdvices = function (num) {
    if (num > 10) return num + " نصيحة اخرى";
    if (num == 2) return "نصيحتان اخرتان";
    if (num == 1) return "نصيحة اخرى";
    if (num <= 10) return num + " نصائح اخرى";
}
language.enterValidAdvice = "(من فضلك اكتب نصيحة بطريقه صحيحةالنصيحة على الاقل 10 حروف)";
language.adviceUpdatedSuccess = "لقد تم تعديل النصيحة بنجاح";
language.areYouSureToDeleteAdvice = "هل انت متأكد من حذف النصيحه؟";
language.noPatientViewTillNow = "لا يوجد اى اراء للمرضى حتى الان";
language.ratingWithoutComment = "[تقييم بدون تعليق]";
language.validateFielLength = function (min, max) {
    if (min != undefined && max != undefined)
        return "هذا الحقل لابد ان يحتوى على الاقل " + min + " من الحروف وعلى الاكثر " + max + " من الحروف";
    if (max == undefined)
        return "هذا الحقل لابد ان يحتوى على الاقل " + min + " من الحروف";
    if (min == undefined && max != undefined)
        return "هذا الحقل لابد ان يحتوى على الاكثر  " + max + " من الحروف";
}
language.messageSentSuccess = "لقد تم ارسال رسالتك بنجاح";
language.messageNotSent = "لقد حدث خطأ,لا يمكن ارسال رسالتك بعد";
language.enterValidPassword = "ادخل كلمة سر صحيحة";
language.twoPasswordsNotIdentical = "كلمتى السر غير متطابقتين";
language.changeScedualedNumber = function (from, to) {
    return "هل تريد تغيير عدد الايام المجدولة من " + from + " الى " + to;
}
language.addAppointementOfDay = "اضف مواعيد حجوزات ليوم";
language.from = "من";
language.to = "الى";
language.endPeriodMustExceedBeginPeriod = "قيمة نهاية الفترة لابد ان تكون اكبر من قيمة بدايتها";
language.periodsAreIntersects = "هذه الفترة تتعارض مع فترة اخرى تم اخيارها,من فضلك اختار المواعيد بدقة";
language.periodIsAlreadyExists = "هذه الفترة موجودة بالفعل";
language.enterValidInterval = "من فضلك اختار فترة بشكل صحيح";
language.am = "ص";
language.pm = "م";
language.noChangesToSave = "لايوجد تغيرات للحفظ";
language.changesAreDone = "لقد تمت التعديلات بشكل صيحيح";
language.areYourSureToChangeBookingType = "هل انت متأكد من تغيير نظام الحجز,فى هذه الحالة انت سوف تقوم بحذف كل المواعيد المتعلقة بنظام الحجز السابق";
language.bookingTypeChangedSuccess = "لقد تم تغير نوع الحجز بنجاح";
language.getGeneralSpecialityName = function (specialityName)
{
    return "تخصص "+specialityName+" بشكل عام";
}
