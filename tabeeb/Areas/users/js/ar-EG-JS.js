var language = {}; 
language.nameNotValid = "الاسم غير صحيح";
language.userNameIsAlreadyExists = "اسم المستخدم محجوز بالفعل";
language.phoneNumberIsAlreadyExists = "ررقم المحمول محجوز بالفعل من قبل";
language.emailNotVlaid = "البريد الالكترونى غير صحيح";
language.commentNotValid = "من فضلك اكتب رساله بشكل صحيح";
language.nameIsTooMany = "الاسم طويل جدا";
language.enterValidPassword = "ادخل كلمة سر صحيحة";
language.twoPasswordNotEqual = "كلمتى السر غير متطابقتين";
language.validateFielLength = function (min, max) {
    if (min != undefined && max != undefined)
        return "هذا الحقل لابد ان يحتوى على الاقل " + min + " من الحروف وعلى الاكثر " + max + " من الحروف";
    if (max == undefined)
        return "هذا الحقل لابد ان يحتوى على الاقل " + min + " من الحروف";
    if (min == undefined && max != undefined)
        return "هذا الحقل لابد ان يحتوى على الاكثر  " + max + " من الحروف";
}
language.notConnectedToInternet = 'انت غير متصل بالانترنت';
language.slowInternet = "قد تكون سرعة الانترنت بطيئة لديك ,من  فظلك حاول مرة اخرى";
language.emailProblem = "حدثت مشكلة اثناء العثور على البريد الالكترونى";
language.loginPoblemAtServer = "حدثت مشكلة اثناء تسجيل الدخول من فضلك حاول مرة اخرى وادخل بيانات صيحية";
language.login = "دخول";
language.formAlert = "من فضلك ادخل بعض البانات المطلوبة من موقع طبيبك للانضمام الينا";
language.changeProfileNameAlert = "تغيير اسم المستخدم سوف يؤدى بك الى الخروج من الموقع ,والدخول مرة اخرى  بأسم المستخدم الجديد وكلمة السر.هل موافق على الخروج";
language.loginFaild = "لقد فشلت عملية الدخول من فضلك حاول مرة اخرى";
language.notConnectedFacebook = "من فضلك انت غير مسجل دخول بفيسبوك,من فضلك سجل دخولك لفيسبوك اولا ثم قم بستجيل الدخول على موقع طبيبك";
language.facebookAlert = "للاسف لابد من الموافقة على اعطاء الايميل والاسم بالكامل عن طريق فيسبوك للدخول لموقع طبيبك بشكل سهل .من فضلك حاول الدخول مرة اخرى";
language.cancelAppointementAlert = "هل انت متأكد من الغاء هذا الحجز عند الطبيب";
language.notifyPatientWithCancelAppointement = "لقد تم الغاء الموعد,سوف نبلغ الطبيب بالغاء حجزك";
language.notSuccessOperation = "لقد حدثت مشكلة فى السيرفر,لم تتم العملية بنجاح";
language.allDestricts = "كل المناطق";
language.cannotLoadData = "لا يمكن تحميل اى بيانات الان من الخادم,ربما ذلك يكون بسبب سرعة الانترنت البطيئة,أو حدوث مشكلة عند الخادم,من فضلك حاول مرة اخرى";
language.serverProblem = "لقد حدثت مشكلة اثناء تنفيذالعملية,من فضلك حاول مرة اخرى";
language.professions =[];//not used
language.professions[0] = "استاذ فى";
language.professions[1] = "محاضر فى";
language.professions[2] = "استشارى";
language.professions[3] = "اخصائى";
language.professions[4] = "مدرس مساعد فى";
language.professions[5] = "استاذ مساعد فى";
language.noMatchedItemsForSearch = "لا يوجد نتائج متعلقة بعناصر البحث";
language.specialityname = "تخصص ";
language.specializedDoc = "الاطباء المتخصصة فى ";
language.noMathchedResult = "لا يوجد نتائج متطابقة";
language.noWrittenAdvices = "ليس لديك اى نصائح مدونة  من قبل";
language.aboutNumber = "حوالى";
language.otherAdvices = "نصيحة اخرى";
language.ratingWithoutComment = "تقييم بدون تعليق";
language.noPatientViewsForDoctor = "لا يوجد اى اراء للمرضى حتى الان";
language.changeDoctorView = "تغيير رأيك فى الدكتور";
language.evaluateProblem = "لقد حدثت مشكلة اثناء عملية التقييم,يمكنك اعادة تحديث الصفحة ثم المحاولة مرة اخرة,هل تريد تحديث الصفحة";
language.loginFirst = "من فضلك قم بتسجيل الدخول اولا,هل تريد ان تسجل الدخول الان";
language.youHavNoReservation = "انت ليس لديك اى زيارة للدكتور من قبل ,لذلك لا يمكنك عمل تقييم اوتعليق الان";
language.haveNoAccount = "انت لست لديك حساب على موقع طبيبك من قبل,من فضلك سجل حساب مع طبيبك وانضم الينا الان";
language.fromTime = "من";
language.toTime = "حتى";
language.noAvailbalAppointements = "لا يوجد مواعيد متاحة";
language.pleaseSelectAppointement = "من فضلك اختر موعد الحجز من جدول الحجوزات";
language.waitingTime = function (minutes) {
    if (minutes >= 3 && minutes < 11) return minutes + " دقائق انتظار قبل الدخول للدكتور";
    if (minutes >= 11) return minutes + " دقيقة انتظار قبل الدخول للدكتور";
    return "لا يوجد انتظار قبل الدخول للدكتور";
}
language.months = ["يناير", "فبراير", "مارس", "إبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"];
language.days = ["اﻷحد", "اﻷثنين", "الثلاثاء", "اﻷربعاء", "الخميس", "الجمعة", "السبت"];

