/// <reference path="../../../Scripts/jquery-1.10.2.js" />
var allData = [];
var filteredData = [];
var pageRecords = 0;
var pageNum = 1;
var nextDiff = 0;
var prevDiff = 0;
var end = 0;
var start = 0;
var nextEnded = false;
var prevEnded = false;
var nextClicked = false;
var prevClicked = false;
var controllerName = "";
var tableClass = "";
var docCities = [];
var docDestricts = [];

//Load Data in Table when documents is ready
function getStart(controller) {
    controllerName = controller;
    tableClass = '.'.concat(controllerName, "tbody");
    updatedesign();
    if (controllerName == "" || controllerName == "index" ||controllerName=="adv") {
        return;
    }
    loadData();
    if (controllerName == "doctor") {
        getCitiesAndDestrictsAndSpecialities();
    }
}
//Load Data function
function loadData(pageNumber) {
    if (pageNumber == undefined) pageNumber = 1;
    var destinedController = controllerName;
    if (controllerName == 'patientComplains' || controllerName == 'doctorComplains' || controllerName == 'generalComplains')
        destinedController='complains';
    $('.loading').removeClass('hidden');
    $.ajax({
        url: "/admin/" + destinedController + "/List",
        type: "GET",
        data:{pageNumber:pageNumber},
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            allData = result.data;            
            filteredData = result.data;
            if (result.length == 0) {
                $('table tbody tr.hidden').removeClass('hidden');
                return;
            }
            $('table tbody .hidden').addClass('hidden');
            getRecords($('#pagingBtn'));
            if(!result.isLastPage)
            {
                loadTheRestData(2);
            }
            else
            {
                $('.loading').addClass('hidden');
            }
        },
        error: function (errormessage) {
            alert("from loaddata function" + errormessage.responseText);
        }
    });
}
function loadTheRestData(pageNumber)
{
    var destinedController = controllerName;
    if (controllerName == 'patientComplains' || controllerName == 'doctorComplains' || controllerName == 'generalComplains')
        destinedController = 'complains';
    $('.loading').removeClass('hidden');
    $.ajax({
        url: "/admin/" + destinedController + "/List",
        type: "GET",
        data: { pageNumber: pageNumber },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            for (var i = 0; i < result.data.length; i++)
            {
                allData.push(result.data[i]);
                filteredData.push(result.data[i]);
            }
            if (!result.isLastPage) {
                loadTheRestData(pageNumber+1);
            }
            else
            {
                $('.loading').addClass('hidden');
            }
        },
        error: function (errormessage) {
            alert("from loaddata function" + errormessage.responseText);
        }
    });
}
function getRecords(context, isFiltered) {
    var appliedData = allData;
    if (isFiltered) appliedData = filteredData;
    if (appliedData.length == 0) {
        $(tableClass).empty();
        alertNoData(tableClass);
        return;
    }
    $('table tbody tr.hidden').addClass('hidden');
    $(tableClass).empty();
    var num = parseInt($(context).val());
    pageNum = 1;
    if (num.toString() == NaN.toString()) num = 10;
    start = 0;
    end = 0;
    nextDiff = 0;
    pageRecords = num;
    prevEnded = false;
    nextEnded = false;
    nextClicked = false;
    prevClicked = false;
    if (controllerName == "doctor")
        $(tableClass).html(TableData(getDoctorTableHtml,appliedData, num));
    else if (controllerName == "patient")
        $(tableClass).html(TableData(getPatientTableHtml, appliedData, num));
    else if (controllerName == "admin")
        $(tableClass).html(TableData(getAdminTableHtml, appliedData, num));
    else if (controllerName == "reserving")
        $(tableClass).html(TableData(getReservingTableHtml, appliedData, num));
    else if (controllerName == "adv")
        $(tableClass).html(TableData(getAdvTableHtml, appliedData, num));
    else if (controllerName == "doctorComplains")
        $(tableClass).html(TableData(getDoctorComplainsTableHtml, appliedData, num));
    else if (controllerName == "patientComplains" || controllerName == "generalComplains")
        $(tableClass).html(TableData(getPatientComplainsTableHtml, appliedData, num));
}
function TableData(funcName, appliedData, num) {
    var html = '';
    for (var i = 0; i < appliedData.length; i++) {
        var item = appliedData[i];
        if (num == i) {
            $(tableClass).html(html); break;
        }
        html += funcName(item,i);
    };
    return html;
}
function getDoctorTableHtml(item) {
    var html = ''; 
    var dat = (item.dateOfJoin).replace(/[a-zA-Z\)\(\/]/g, '');
    var type = (item.type == true) ? 'ذكر' : 'انثى';
    var activeEl = "";
    if (item.stat == false)
    {
        activeEl = '<a href="#" class="btn btn-success" onclick="return activation(this,'+'\''+ item.id+'\''+ ')"><text>تفعيل</text> <i class="glyphicon glyphicon-saved  fa-lg"></i></a>';
    }
    else {
        activeEl = '<a href="#" class="btn btn-danger"  onclick="return activation(this,'+ '\''+ item.id +'\''+ ',' + false + ')"><text>تعطيل</text> <i class="fa fa-remove  fa-lg"></i></a>';
    }
    var doctorSpecialities = "";
    var specialities= item.specialities;
    for (var i = 0; i < specialities.length; i++)
    {
        var sp=specialities[i];
        doctorSpecialities += sp.name;
        if (i < specialities.length - 1) doctorSpecialities += ",";
    }
    html += '<tr>';
    html += '<td>' + item.name+'</td>';
    html += '<td>' + new Date(parseInt(dat)).toDateString() + '</td>';
    html += '<td>' + item.phone + '</td>';
    html += '<td>' + item.mail + '</td>';
    html += '<td>' + item.education + '</td>';
    html += '<td>' + doctorSpecialities + '</td>';
    html += '<td>' + item.examinFees + '</td>';
    html += '<td>' + activeEl + '<a href="#" class="btn btn-danger" onclick="Delete('+'\'' + item.id+ '\''+',this)">حذف <i class="fa fa-trash  fa-lg"></i></a><a  class="btn btn-info" data-toggle="modal" data-target="#viewModal" onclick=fillModal("' + item.id + '")> عرض<i class="fa fa-book fa-lg"></i></a></td>';
    html += '</tr>';
    return html;
}
function getAdvTableHtml(item) {
    var html = ''; 
    html+='<tr><td>'+item.description+'</td>';
    html+='<td>'+item.content+'</td>';
    html += '<td>' + new Date(parseInt(item.startDate.substr(6))).toLocaleDateString() +" "+ new Date(parseInt(item.startDate.substr(6))).toLocaleTimeString()+ '</td>';
    html+='<td>'+item.amount+'<bdi>يوم</bdi></td>';
    html+='<td>';
    html+='<a href="#" class="btn btn-info" onclick="return getbyID('+item.id+')">تعديل <i class="fa fa-edit  fa-lg"></i></a>';
    html+='<a href="#" class="btn btn-danger" onclick="Delete('+item.id+',this)">حذف <i class="fa fa-trash  fa-lg"></i></a>';
    if (!item.stat) {
              html+='<a href="#" class="btn btn-success" onclick="return activation(this,'+item.id+','+true+')">';
              html+='<text>تفعيل</text> <i class="glyphicon glyphicon-saved  fa-lg"></i></a>';
              }
    else {
              html+='<a href="#" class="btn btn-danger" onclick="return activation(this,'+item.id+','+false+')">';
              html+='<text>تعطيل</text> <i class="fa fa-remove  fa-lg"></i> </a>'; 
             }
          html += '</td></tr>';
    return html;
}
function getPatientTableHtml(item, index) {
    var html = '';
    html += '<tr>';
    html += '<td>' + (index + 1) + '</td>';
    html += '<td>' + item.name + '</td>';
    html += '<td>' + new Date(parseInt((item.dateOfJoin.substr(6)))).toDateString() + '</td>';
    html += '<td>' + item.phone + '</td>';
    html += '<td>' + item.mail + '</td>';
    html += '<td><a href="#" class="btn btn-danger" onclick="Delete(' + item.id + ',this)">أحذف <i class="fa fa-trash  fa-lg"></i></a></td>';
    html += '</tr>';
    return html;
}
function getAdminTableHtml(item, index) {
    var html = '';
    var type = (item.type == 'male') ? 'ذكر' : 'انثى';
    html += '<tr>';
    html += '<td>' + (index + 1) + '</td>';
    html += '<td>' + item.name + '</td>';
    html += '<td>' + new Date(parseInt((item.dateOfJoin.substr(6)))).toDateString() + '</td>';
    html += '<td>' + item.phone + '</td>';
    html += '<td>' + item.mail + '</td>';
    html += '<td>' + type + '</td>';
    html += '<td>' + item.periority + '</td>';
    html += '<td><a  class="btn btn-danger" href="#" onclick="Delete(' + item.periority + ',this)">أحذف <i class="fa fa-trash  fa-lg"></i></a><a  class="btn btn-info" data-toggle="modal" data-target="#viewModal" onclick=fillModal(' + item.periority + ')> عرض<i class="fa fa-book fa-lg"></i></a></td>';
    html += '</tr>';
    return html;
}
function getReservingTableHtml(item, index) {
    var html = '';
    html += '<tr>';
    html += '<td>' + (index + 1) + '</td>';
    html += '<td>' + item.name + '</td>';
    html += '<td>' + item.ptName + '</td>';
    html += '<td>' + new Date(parseInt(item.reservingDate.substr(6))).toDateString() + '</td>';
    html += '<td>' + item.spName + '</td>';
    html += '</tr>';
    return html;
}
function getDoctorComplainsTableHtml(item,index) {
    var html = '';
    html += '<tr>';
    html += '<td>' + (index+1) + '</td>';
    html += '<td>' + item.name + '</td>';
    html += '<td>' + item.mail + '</td>';
    html += '<td>' + new Date(parseInt((item.dateOfJoin.substr(6)))).toDateString() + '</td>';
    html += '<td>' + item.phone + '</td>';
    html += '<td>' + item.message + '</td>';
    html += '<td><a href="#" class="btn btn-danger" onclick="Delete(' + item.id + ',this)">أحذف <i class="fa fa-trash  fa-lg"></i></a></td>';
    html += '</tr>';
    return html;
}
function getPatientComplainsTableHtml(item,index) {
    var html = '';
    html += '<tr>';
    html += '<td>' + (index+1) + '</td>';
    html += '<td>' + item.name + '</td>';
    html += '<td>' + item.mail + '</td>';
    html += '<td>' + new Date(parseInt((item.dateOfJoin.substr(6)))).toDateString() + '</td>';
    html += '<td>' + item.message + '</td>';
    html += '<td><a href="#" class="btn btn-danger" onclick="Delete(' + item.id + ',this)">أحذف <i class="fa fa-trash  fa-lg"></i></a></td>';
    html += '</tr>';
    return html;
}
function fillModal(PK) {
    $('#viewModal table tr td').remove();
    var html = '';
    var item = null;
    if (controllerName == "admin")
    {
        for (var i = 0; i < allData.length; i++) {
            if (allData[i].periority == PK)
            { item = allData[i]; break; }
        }
        var type = (item.type == 'male') ? 'ذكر' : 'انثى';
        $('#viewModal table tr').eq(0).append('<td>' + item.name + '</td>');
        $('#viewModal table tr').eq(1).append('<td>' + new Date(parseInt(item.dateOfJoin.substr(6))).toDateString() + '</td>');
        $('#viewModal table tr').eq(2).append('<td>' + item.phone + '</td>');
        $('#viewModal table tr').eq(3).append('<td>' + item.mail + '</td>');
        $('#viewModal table tr').eq(4).append('<td>' + type + '</td>');
        $('#viewModal table tr').eq(5).append('<td>' + item.periority + '</td>');
        $('#viewModal img').attr('src', item.image);
    }
    else if (controllerName == "doctor") {
        for (var i = 0; i < allData.length; i++) {
            if (allData[i].id == PK)
            { item = allData[i]; break; }
        }
        var type = (item.type == true) ? 'ذكر' : 'انثى';
        $('#viewModal table tr').eq(0).append('<td>' + item.name + '</td>');
        $('#viewModal table tr').eq(1).append('<td>' + new Date(parseInt(item.dateOfJoin.substr(6))).toDateString() + '</td>');
        $('#viewModal table tr').eq(2).append('<td>' + item.phone + '</td>');
        $('#viewModal table tr').eq(3).append('<td>' + item.mail + '</td>');
        $('#viewModal table tr').eq(4).append('<td>' + type + '</td>');
        $('#viewModal table tr').eq(5).append('<td>' + item.examinFees + '</td>');
        $('#viewModal table tr').eq(6).append('<td>' + item.spName + '</td>');
        $('#viewModal table tr').eq(7).append('<td>' + item.education + '</td>');
        $('#viewModal table tr').eq(8).append('<td>' + item.city + '</td>');
        $('#viewModal table tr').eq(9).append('<td>' + item.destName + '</td>');
        $('#viewModal table tr').eq(10).append('<td>' + item.allReservings + '</td>');
        $('#viewModal table tr').eq(11).append('<td>' + item.dayReservings + '</td>');
        $('#viewModal img').attr('src', '/Areas/doctors/profImages/' + item.proImage);
    }
}
function getDataByNameSearch(value) {
    filteredData = allData;
    if (value.trim() == '') {
        getRecords($('#pagingBtn'), true);
        return;
    }
    var matchedData = [];
    if(controllerName=="reserving")
    {
        var personFilter = $('.specifiedFilterName input').attr('data-value').trim();
        for (var i = 0; i < allData.length; i++) {
            var item = allData[i];
            if (personFilter == "doctor") {
                if (item.docName.indexOf(value) >= 0) matchedData.push(item);
            }
            else if (personFilter == "patient") {
                if (item.ptName.indexOf(value) >= 0) matchedData.push(item);
            }
           
        }
    }
    else {
        for (var i = 0; i < allData.length; i++) {
            var item = allData[i];
            if (item.name.indexOf(value) >= 0) matchedData.push(item);
        }
    }
    
    filteredData = matchedData;
    getRecords($('#pagingBtn'),true);
}
function getDataBasedOn(context, matchedValue) {
    filteredData = allData;
    var columnValue = $(context).val().toString();
    var day = new Date(columnValue).getDate();
    var month = new Date(columnValue).getMonth();
    var year = new Date(columnValue).getFullYear();
    var matchedData = [];
    if (matchedValue == 'dateOfJoin' || matchedValue == 'reservingDate')
         {//filtering based on date
            for (var i = 0; i < allData.length; i++) {
                var item = allData[i];
                var getDate = (matchedValue == "reservingDate") ? item.reservingDate : item.dateOfJoin;
                var currentDate = getDate.replace(/[a-zA-Z\)\(\/]/g, '');
                var itemDay = new Date(parseInt(currentDate)).getDate();
                var itemMonth = new Date(parseInt(currentDate)).getMonth();
                var itemYear = new Date(parseInt(currentDate)).getFullYear();
                if (day == itemDay && month == itemMonth && year == itemYear) matchedData.push(item);
            }
        }
        else 
            {//filtering based on speciality
                for (var i = 0; i < allData.length; i++) {
                    var item = allData[i];                  
                    if (item.docSpId==matchedValue) matchedData.push(item);
                }
            }
 
   filteredData = matchedData;
   getRecords($('#pagingBtn'),true);
}
function getSortedData(context, col) {
    if (context != null) {
        if (!$(context).hasClass('sorted'))
        {
        $(context).addClass('sorted');
        $(context).text("Order by date Desc");
        $(context).parent().hide();
        }
        else {
            allData.reverse();
            getRecords($('#pagingBtn'));
            $(context).text("Order by date Asc");
            $(context).parent().hide();
            return;
        }
    }
    filteredData = SortDataByName(filteredData, col); 
    getRecords($('#pagingBtn'));
}
function nextPage() {
    if (pageRecords >= filteredData.length || pageRecords == 0) return;
    if (prevEnded) { pageNum + 1; prevEnded = false; }
    if (nextDiff != 0 || end == filteredData.length) { if (!nextEnded) { nextEnded = true; } return; }
    nextClicked = true;
    if (prevClicked) { prevClicked = false; pageNum += 1; }
    $(tableClass).empty();
    var html = '';
    end = (pageNum * pageRecords + pageRecords);
    start = pageNum * pageRecords;
    nextDiff = (filteredData.length >= end) ? 0 : end - filteredData.length;
    for (var i = start; i < end - nextDiff; i++) {
        var item = filteredData[i];
        html += getHtml(item,i);
    };
    pageNum += 1;
    $(tableClass).html(html);

}
function prevPage() {
    if (pageRecords >= filteredData.length || pageRecords == 0) return;
    if (pageNum == 0) { if (!prevEnded) { prevEnded = true; start = 0; end = 0; } return };
    if (nextEnded) { pageNum -= 0; nextEnded = false; }
    $(tableClass).empty();
    prevClicked = true;
    if (nextClicked) { nextClicked = false; pageNum -= 1; }
    var html = '';
    end = pageNum * pageRecords;
    start = end - pageRecords;
    for (var i = start ; i < end ; i++) {
        var item = filteredData[i];
        html += getHtml(item,i);
    };
    pageNum -= 1;
    nextDiff = 0;
    $(tableClass).html(html);
}
function getHtml(item,i)
{
    var html = '';
    if (controllerName == "doctor") {
        html += getDoctorTableHtml(item);
    }
    else if (controllerName == "patient") {
        html += getPatientTableHtml(item, i);
    }
    else if (controllerName == "reserving") {
        html += getReservingTableHtml(item, i);
    }
    else if (controllerName == "patientComplains") {
        html += getPatientComplainsTableHtml(item, i);
    }
    else if (controllerName == "doctorComplains") {
        html += getDoctorComplainsTableHtml(item, i);
    }
    return html;
}
//Add Data Function
function AddRecord() {
    var res = validate();
    if (res == false) {
        return false;
    }
    $('#myModal .modal-body i.updatingNow').removeClass('hidden');
    var Obj;
    if (controllerName == "doctor") {
        Obj = {
            id: $('#id').val(),
            name: $('#name').val(),
            type: $('#type').attr('data-value'),
            mail: $('#email').val(),
            phone: $('#phone').val(),
            password: $('#password').val(),
            price: $('#price').val(),
            docSpId: $('#speciality option:selected').val(),
            education: $('#education option:selected').val(),
            destrictID: $('#destrict option:selected').val(),
            image: '/userImages/default.png',
            stat:1
        };
    }
    else if (controllerName == "adv") {
        // var selectPageIds = new Array();
        var selectPageIds = [];
        $('#page option:selected').each(function(index,el){
            // selectPageIds.push(parseInt($(el).val()));
            selectPageIds.push(parseInt($(el).val()));
        });
        Obj = {
            id: 1,
            description: $('#description').val(),
            content: $('#content').val(),
            isViewAgain: $('#isViewAgain').attr('data-value'),
            startDate: $('#startDate').val(),
            waitingTime: parseInt($('#waitingTime').val()),
            waitingAfterClosed: parseInt($('#waitingAfterClosed').val()),
            pagesID: selectPageIds,
            amount: parseInt($('#amount').val()),
            image: 'default.png',
            stat: 0
        };
    }
    else if (controllerName == "admin") {
        Obj = {
            name: $('#name').val(),
            type: $('#type').attr('data-value'),
            mail: $('#email').val(),
            phone: $('#phone').val(),
            password: $('#password').val()
        };
    }
     
    $.ajax({
        url: "/admin/" + controllerName + "/Add",
        data: JSON.stringify(Obj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            $('#myModal .modal-body i.updatingNow').addClass('hidden');
            if (result.result == false ||result==false)
            {
                $('.errorMessageValidation').css("display", "block");
                $('#email,#startDate').focus(function () {
                    $('.errorMessageValidation').fadeOut(2000);
                });
            }
            else {
                $('#myModal').modal('hide');               
                alert("record added successfully");
                loadData();
                emptyForm($('.modal-body form'));
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function emptyForm(form)
{
    $(form).find('input').val('');
    $(form).find('textarea').val('');
    $(form).find('input[type="radio"]').removeAttr('checked');
}
//Function for getting the Data Based upon Employee ID
function getbyID(ID) {
    $('modal-body i.updatingNow').removeClass('hidden');
    $('#myModal').modal('show');
    $('#btnUpdate').show();
    $('#btnAdd').hide();
    if (controllerName == "adv")
    {
        $('#description').css('border-color', 'lightgrey');
        $('#content').css('border-color', 'lightgrey');
        $('#startDate').css('border-color', 'lightgrey');
        $('#isViewAgain').css('border-color', 'lightgrey');
        $('#waitingTime').css('border-color', 'lightgrey');
        $('#amount').css('border-color', 'lightgrey');
        $('#isViewAgain').css('border-color', 'lightgrey');
        $('#waitingAfterClosed').css('border-color', 'lightgrey');
        $('#page').css('border-color', 'lightgrey');
        var advData = $('.advData ul:nth-of-type(' + ID + ') li');
        var id = parseInt($(advData).eq(0).text());
        var description = $(advData).eq(1).text();
        var content = $(advData).eq(2).text();
        var startDate = Date.parse($(advData).eq(3).text());
        var amount = parseInt($(advData).eq(4).text());
        var waitingTime = parseInt($(advData).eq(6).text());
        var waitingAfterClosed = parseInt($(advData).eq(7).text());
        var isViewAgain =($(advData).eq(8).text()==true)?true:false;
        var stat = ($(advData).eq(9).text() == "True") ? true : false;
        var dateTime = new Date(startDate).getTime() - new Date(startDate).getTimezoneOffset() * 60000;
        var pagesID = $(advData).eq(10).text().trim().split(' ');
        $('#id').val(id);
        $('#stat').val(stat);
        $('#description').val(description);
        $('#content').val(content);
        $('#isViewAgain').attr('data-value', isViewAgain);
        var isoStr = new Date(dateTime).toISOString();
        $('#startDate').val(isoStr.substring(0, isoStr.length - 1));
        $('#waitingTime').val(waitingTime);
        $('#amount').val(amount);
        $('#waitingAfterClosed').val(waitingAfterClosed);
        if (isViewAgain == true ||isViewAgain=="true") {
            $('#isViewAgain').find('input:first-of-type').attr("checked", "");
            $('div.waitingAfterClosed').removeClass('hidden');
        }
        else {
            $('#isViewAgain').find('input:nth-of-type(2)').attr("checked", "");
            $('div.waitingAfterClosed').addClass('hidden');
        }
        $('#page').find('option').removeAttr("selected").css('background', '#e51400');
        $('#page').find('option').on("click focus", function () {
            $('#page').find('option').css('background', '#e51400');
        });
        $('#page option').each(function (index, op) {
            for (var i = 0; i < pagesID.length; i++) {
                if (pagesID[i] == $(op).val()) {
                    $(op).attr("selected", "");
                    $(op).css('background', '#337ab7');
                }
            }
        });
        return;
    }
    else if (controllerName == "doctor")
    {
        $('#name').css('border-color', 'lightgrey');
        $('#email').css('border-color', 'lightgrey');
        $('#birthdate').css('border-color', 'lightgrey');
        $('#price').css('border-color', 'lightgrey');
        $('#type').css('border-color', 'lightgrey');
        $('#speciality').css('border-color', 'lightgrey');
        $('#education').css('border-color', 'lightgrey');
    }
    $.ajax({
        url: "/admin/"+controllerName+"/getbyID/" + ID,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('modal-body i.updatingNow').addClass('hidden');
            if (controllerName == "doctor")
            {
                $('#id').val(result.id);
                $('#name').val(result.name);
                $('#birthdate').val(new Date(parseInt(result.birthdate.substr(6))));
                $('#email').val(result.mail);
                $('#phone').val(result.phone);
                $('#password').val(result.password);
                $('#price').val(result.price);
                $('#speciality option').each(function (i, el) {
                    if ($(el).val() == result.docSpId) $(el).attr("selected", "");
                });
                $('#education option').each(function (i, el) {
                    if ($(el).val() == result.education) $(el).attr("selected", "");
                });
                if (result.type == "female") {
                    $('#type').attr('data-value', 'female');
                    $('#type>input:first-of-type').attr("checked", "");
                }
                else {
                    $('#type').attr('data-value', 'male');
                    $('#type>input:nth-of-type(2)').attr("checked", "");
                }
            }/*doctor update*/
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}
//function for updating employee's record
function UpdateRecord(context) {
    var res = validate();
    if (res == false) {
        return false;
    }
    $('#myModal .modal-body i').removeClass('hidden');
    var obj;
     if (controllerName == "adv") {
        // var selectPageIds = new Array();
        var selectPageIds = [];
        $('#page option:selected').each(function (index, el) {
            selectPageIds.push(parseInt($(el).val()));  
        }); 
        obj = {
            id: $('#id').val(),
            description: $('#description').val(),
            content: $('#content').val(),
            isViewAgain: $('#isViewAgain').attr('data-value'),
            startDate: $('#startDate').val(),
            waitingTime: parseInt($('#waitingTime').val()),
            waitingAfterClosed: parseInt($('#waitingAfterClosed').val()),
            pagesID: selectPageIds,
            amount: parseInt($('#amount').val()),
            image: '/advImages/default.png',
            stat: $('#stat').val()
        };  
    }
    else if (controllerName == "admin") {
        $(context).find('i').removeClass('fa-edit').addClass('fa-spinner fa-spin');
        obj = {
            name: $('#name').val(),
            type: $('#type').attr('data-value'),
            mail: $('#email').val(),
            phone: $('#phone').val(),
            password: $('#password').val(),
            periority: $('#id').val(),
            image: $('.adminImage img').attr("src")
        };
    }
    $.ajax({
        url: "/admin/" + controllerName + "/Update",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            $('#myModal .modal-body i').addClass('hidden');
            loadData();
            if (controllerName == 'admin') {
                $(context).find('i').removeClass('fa-spinner fa-spin').addClass('fa-edit');
                if (result.success == true||result!=false)
                    $('.nav > li:last-of-type>a.dropdown-toggle').text($('#name').val()).append('<span class="caret"></span>');
                alert(result.mess);
            }
            else {                
                $('#myModal .modal-body i').addClass('hidden');
                $('#myModal').modal('hide');
                var ID = parseInt($('#id').val()); 
                var advData = $('.advData ul:nth-of-type(' + ID + ') li');
                $(advData).eq(1).text(obj.description);
                $(advData).eq(2).text(obj.content);
                $(advData).eq(3).text(obj.startDate);
                $(advData).eq(4).text(obj.amount);
                $(advData).eq(6).text(obj.waitingTime);
                $(advData).eq(7).text(obj.waitingAfterClosed);
                var isViewAgain = (obj.isViewAgain == true) ? 'True' : 'False';
                var stat = (obj.stat == 'true') ? 'True' : 'False';
                $(advData).eq(8).text(isViewAgain);
                $(advData).eq(9).text(stat);               
                $(advData).eq(10).text(obj.pagesID.toString().replace(/,/g,' '));
            }            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function getCitiesAndDestrictsAndSpecialities()
{
    $.ajax({
        url: "/admin/doctor/getCitiesAndDestrictsAndSpecialities",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            docCities = result.cities;
            docDestricts = result.destricts;
            docSpecialities = result.specialities;
            for (var i = 0; i < docSpecialities.length; i++) {
                var item = docSpecialities[i];
                $('#speciality').append('<option value="' + item.id + '">' + item.name + '</option>');
            }
            for(var i=0;i<docCities.length;i++)
            {
                var item=docCities[i];
                $('#city').append('<option value="'+item.id+'">'+item.name+'</option>');
            }
            var firstCityID = docCities[0].id;
            for (var i = 0; i < docDestricts.length; i++) {
                var item = docDestricts[i];
                if (firstCityID != item.cityID) continue;
                $('#destrict').append('<option value="' + item.id + '">' + item.destName + '</option>');
            }
            $('#city').change(function () {
                var selectedCityID = $('#city option:selected').val();
                $('#destrict').empty();
                for (var i = 0; i < docDestricts.length; i++) {
                    var item = docDestricts[i];
                    if (selectedCityID != item.cityID) continue;
                    $('#destrict').append('<option value="' + item.id + '">' + item.destName + '</option>');
                }
            });
            
        },
        error: function (errormessage) {
            alert("erro in system in getCitiesAndDestrictsAndSpecialities function" + errormessage.responseText);
        }
    });
}
//function for deleting employee's record
function Delete(PK, context) {
    var destinedController = controllerName;
    if (controllerName == 'patientComplains' || controllerName == 'doctorComplains'||controllerName=="generalComplains")
        destinedController = 'complains';
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $(context).find('i.fa-trash').removeClass('fa-trash').addClass('fa-circle-o-notch fa-spin');
        $('#myModal .modal-body i.updatingNow').removeClass('hidden');
        $.ajax({
            url: "/admin/" + destinedController + "/Delete/" + PK,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if(controllerName=='admin'&&result.result==false)
                {
                    if (result.confirm == "confirm")
                    {
                        if (confirm(result.mess + " هل تريد الغاء حسابك")) {
                            $.post("/admin/" + destinedController + "/confirmDelete", { ID: PK }, function (result,status) {
                                if (result == true) {
                                    location.href = "/admin";
                                }
                                else {
                                    alert('حدثت مشكلة اثناء الاتصال بالسيرفر');
                                }
                            });
                        }
                    }
                    else
                    {
                        $(context).find('i.fa-spin').addClass('fa-trash').removeClass('fa-circle-o-notch fa-spin');
                        alert(result.mess);
                    }
                }
               else  if(result==true)
               {                  
                    reArrangeControlsID(destinedController, context, PK);
                    $(context).find('i.fa-spin').addClass('fa-trash').removeClass('fa-circle-o-notch fa-spin');
                    $(context).parents('tr').fadeOut();
                }
                else if(result==false)
                {
                    alert('حدثت مشكلة فى السيرفر اثنار تنفيذ العملية');
                }
            },
            error: function (errormessage) {
                alert("there are error" + errormessage.responseText);
            }
        });
    }
}
function reArrangeControlsID(destinedController, context,PK)
{
    var undeletedRecords = $(context).parents('tr').siblings().not('.alert');
    if (destinedController == "patient" || destinedController == "complains")
    {
        $(undeletedRecords).each(function (index, tr) {
            var a = $(tr).find('td:last-of-type>a:nth-of-type(1)')
            var id = $(a).attr('onclick');
            id = parseInt(id.substring(7, id.indexOf(',')));
            if (id > PK) id = id - 1;
            $(a).attr('onclick', 'Delete(' + id + ',this)');
        });
    }
    else if (destinedController == "doctor") {
        $(undeletedRecords).each(function (index, tr) {
            var activateLink = $(tr).find('td:last-of-type>a:nth-of-type(1)');
            var deleteLink = $(tr).find('td:last-of-type>a:nth-of-type(2)');
            var showLink = $(tr).find('td:last-of-type>a:nth-of-type(3)');
            var id = $(deleteLink).attr('onclick');
            id = parseInt(id.substring(7, id.indexOf(',')));
            if (id > PK) {
                id = id - 1;
            }
            $(activateLink).attr('onclick', 'return activation(this,' + id + ')');
            $(deleteLink).attr('onclick', 'Delete(' + id + ',this)');
            $(showLink).attr('onclick', 'fillModal(' + id + ')');
        });
    }
    else if (destinedController == "admin") {
        $(undeletedRecords).each(function (index, tr) {
            var deleteLink = $(tr).find('td:last-of-type>a:nth-of-type(1)');
            var showLink = $(tr).find('td:last-of-type>a:nth-of-type(2)');
            var id = $(deleteLink).attr('onclick');
            id = parseInt(id.substring(7, id.indexOf(',')));
            if (id > PK) {
                id = id - 1;
            }
            $(deleteLink).attr('onclick', 'Delete(' + id + ',this)');
            $(showLink).attr('onclick', 'fillModal(' + id + ')');
        });
    }
    else if (destinedController == "adv") {
        console.log(undeletedRecords);
        $(undeletedRecords).each(function (index, tr) {
            var editLink = $(tr).find('td:last-of-type>a:nth-of-type(1)');
            var deleteLink = $(tr).find('td:last-of-type>a:nth-of-type(2)');
            var activateLink = $(tr).find('td:last-of-type>a:nth-of-type(3)');
            var id = $(deleteLink).attr('onclick'); 
            id = parseInt(id.substring(7, id.indexOf(','))); 
            if (id > PK) {
                id = id - 1;
            }
            $(editLink).attr('onclick', 'return getbyID(' + id + ')');
            $(deleteLink).attr('onclick', 'Delete(' + id + ',this)');
            $(activateLink).attr('onclick', 'return activation(this,' + id + ')');
        });
    }
}
function validateEmailProfile(context)
{
    var value = $(context).val().trim();
    if (value == '') return;
    $.ajax({
        url: "/admin/admin/validateEmailProfile",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        data: { email: value },
        success: function (result) {
            if (result.result == true) {
                $(context).val("this email is already exist");
                $(context).css('border-color', 'red');
            }
            else {

            }
        },
        error: function (errormessage) {
            alert("there are error" + errormessage.responseText);
        }
    });
}
function validateUserName(context,type,operation)
{
    var value = $(context).val().trim();
    $(context).val(value);
    if (operation == "update")
    {
        var oldValue = $(context).attr('data-value');
        if (value == oldValue) return;
    }
    if (value == '') return; 
    $.ajax({
        url: "/admin/admin/validateUserName",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        data: { userName: value, type: type, operation: operation },
        success: function (result) {
            if(result==true)
            {
                if (type == 'name')
                {
                    $(context).val("this User Name is already exist");
                }
                else if (type = 'email')
                {
                    $(context).val("this Email is already exist");
                }
                $(context).css('border-color', 'red');
                $(context).keydown(function () {
                    $(this).css('border-color', 'lightgray');
                });
            }
        },
        error: function (errormessage) {
            alert("there are error" + errormessage.responseText);
        }
    });
}
//Function for clearing the textboxes
function clearTextBox() {
    if (controllerName == "adv")
    {
        $('#description').val("");
        $('#content').val("");
        $('#startDate').val("");
        $('#amount').val("");
        $('#waitingTime').val("");
        $('#waitingAfterClosed').val("");

    }
    if(controllerName=="admin")return;
    $('.errorMessageValidation').css("display", "none");
    $('.evaluatePassword').text('');
    $('#id').val("");
    $('#name').val("");
    $('#email').val("");
    $('#phone').val("");
    $('#password').val("");
    $('#type').val("");
    $('#price').val("");
    $('#birthdate').val("");
    $('#speciality >option:first-of-type').attr("selected", "");
    $('#education >option:first-of-type').attr("selected", "");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#name').css('border-color', 'lightgrey');
    $('#bithdate').css('border-color', 'lightgrey');
    $('#email').css('border-color', 'lightgrey');
    $('#password').css('border-color', 'lightgrey');
    $('#phone').css('border-color', 'lightgrey');
    $('#type').css('border-color', 'lightgrey');
    $('#price').css('border-color', 'lightgrey');
    $('#speciality').css('border-color', 'lightgrey');
    $('#education').css('border-color', 'lightgrey');
}
//Valdidation using jquery
function validate() {
    var isValid = true;
    /*validate adverisement*/
    if (controllerName == 'adv') {
        var description = $('#description').val().trim();
        var content = $('#content').val().trim();
        if (description == "" || description.length < 10) {
            $('#description').css('border-color', 'Red');
            $('#description').val('');
            $('#description').attr('placeholder', 'Please enter valid description');
            $('#description').next().text('At least 10 characters');
            isValid = false;
        }
        else {
            $('#description').css('border-color', 'lightgrey');
        }/*end of description validation*/
        if (content == "" || content.length <10) {
            $('#content').css('border-color', 'Red');
            $('#content').val('');
            $('#content').attr('placeholder', 'Please enter valid content');
            $('#content').next().text('At least 10 characters');
            isValid = false;
        }
        else {
            $('#content').css('border-color', 'lightgrey');
        }/*end of content validation*/
        if ($('#startDate').val().trim() == "") {
            $('#startDate').css('border-color', 'Red');
            isValid = false;
        }
        else {
            $('#startDate').css('border-color', 'lightgrey');
        }/*end of startDate validation*/
        if ($('#amount').val().trim() == "") {
            $('#amount').css('border-color', 'Red');
            $('#amount').val('');
            $('#amount').attr('placeholder', 'Please enter valid amount');
            isValid = false;
        }
        else {
            $('#amount').css('border-color', 'lightgrey');
        }/*end of amount validation*/
        if ($('#isViewAgain').attr('data-value').trim() == "") {
            $('#isViewAgain').css('border', '1px solid Red');
            isValid = false;
        }
        else {
            $('#isViewAgain').css('border-color', 'lightgrey');
        }/*end of isViewAgain validation*/

        if ($('#waitingTime').val().trim() == "") {
            $('#waitingTime').css('border-color', 'Red');
            $('#waitingTime').val('');
            $('#waitingTime').attr('placeholder', 'Please enter valid waitingTime');
            isValid = false;
        }
        else {
            $('#waitingTime').css('border-color', 'lightgrey');
        }/*end of waitingTime validation*/
        if ($('#isViewAgain').attr('data-value').trim() == true || $('#isViewAgain').attr('data-value').trim()=='true')
        {
            if ($('#waitingAfterClosed').val().trim() == "") {
                $('#waitingAfterClosed').css('border-color', 'Red');
                $('#waitingAfterClosed').val('');
                $('#waitingAfterClosed').attr('placeholder', 'Please enter valid waitingAfterClosed');
                isValid = false;
            }
            else {
                $('#waitingAfterClosed').css('border-color', 'lightgrey');
            }/*end of waitingAfterClosed validation*/
        }
        return isValid;
    }/*end of advertisement validation*********************************************/
    /*end of advertisement validation*/
    var email = $('#email').val().trim();
    var name = $('#name').val().trim();
    if (name == "" || name.length < 3 || name.match(/[a-zA-Z ]{3,}/) == null ||name.length>15) {
        $('#name').css('border-color', 'Red');
        $('#name').val('');
        $('#name').attr('placeholder', 'Please enter valid name');
        $('#name').next().text('at least 3 characters and at most 15 characters');
        $('#name').keyup(function () { $(this).next().text(''); });
        isValid = false;
    }
    else {
        $('#name').css('border-color', 'lightgrey');
    }/*end of name validation*/

    if (email.match(/[a-zA-Z0-9]{2,}@[a-zA-z]{2,}\.[a-zA-Z]{2,}/) != null) {
        $('#email').css('border-color', 'lightgrey');
    }
    else {
        $('#email').css('border-color', 'Red');
        $('#email').val('');
        $('#email').attr('placeholder', 'Please enter valid email');
        isValid = false;
    }/*end of mail validation*/

    if ($('#phone').val().trim().match(/[0-9]{11}/) == null) {
        $('#phone').css('border-color', 'Red');
        $('#phone').val('');
        $('#phone').attr('placeholder', 'Please enter valid phoneNumber');
        isValid = false;
    }
    else {
        $('#phone').css('border-color', 'lightgrey');
    }/*end of phone validation*/

    if ($('#type').attr('data-value').trim() == "") {
        $('#type').css('border', '1px solid Red');
        isValid = false;
    }
    else {
        $('#type').css('border-color', 'lightgrey');
    }/*end of type validation*/
    if ($('#password').val().trim().length < 6 ||
        $('#password').val().trim().length>25||
        $('#password').val().trim().match('(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}')==null) {
        $('#password').css('border-color', 'Red');
        $('#password').val('');
        $('#password').attr('placeholder', 'please enter valid password');
        $('#password').next().text('at least one number, one lowercase and one uppercase letter at least 8 characters and at most 25 characters');
        $('#password').siblings('i.showPassword').css('top','33%');
        $('#password').keyup(function () { $(this).next().text('').end().siblings('i.showPassword').css('top','53%');;});
        isValid = false;
    }
    else {
        $('#password').css('border-color', 'lightgrey');
    }/*end of password validation*/
    /***********these above properties is shared between doctor and admin*********/
    if (controllerName == 'admin') {
        return isValid;
    }
    if ($('#price').val().trim() == "") {
        $('#price').css('border-color', 'Red');
        $('#price').val('');
        $('#price').attr('placeholder', 'Please enter valid price');
        isValid = false;
    }
    else {
        $('#price').css('border-color', 'lightgrey');
    }/*end of price validation*/
    if ($('#birthdate').val().trim() == "") {
        $('#birthdate').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#birthdate').css('border-color', 'lightgrey');
    }/*end of birthdate validation*/
    return isValid;
}
function getType(type) {
    $('#type').attr('data-value', type);
}
function evaluatePassword(context) {
    $('.evaluatePassword').css('margin', '0 10px');
    $(context).blur(function () {
        $('.evaluatePassword').text('');
    });
    var value = $(context).val();
    if (value.length < 5) $('.evaluatePassword').text("vary weak password").css('color', 'red');
    else if (value.length >= 5 && value.length < 8) $('.evaluatePassword').text("weak password").css('color', 'yellow');
    else if (value.length >= 8) $('.evaluatePassword').text("strong password").css('color', 'green');
}
/*This function activate doctor profile to be accepted to be seen for patients*/
function activation(context,ID)
{
    if (!confirm("هل انت متأكد من اجراء التفعيل او التعطيل لحساب هذا الطبيب"))
    {
        return false;
    }
    $(context).find('i').removeClass('fa-remove glyphicon glyphicon-saved').addClass('fa fa-circle-o-notch fa-spin');
    $.ajax({
        url: "/admin/" + controllerName + "/activation",
        type: "GET",
        data: {id:ID },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if ($(context).hasClass('btn-danger'))
            {
                $(context).removeClass('btn-danger').addClass('btn-success');
                $(context).find('i').addClass('glyphicon glyphicon-saved').removeClass('fa fa-circle-o-notch fa-spin');
                $(context).find('text').text('تفعيل');
                $('.advData ul:nth-of-type(' + ID + ') li').eq(9).text('False');
            }
            else {
                $(context).removeClass('btn-success').addClass('btn-danger');
                $(context).find('i').addClass('fa fa-remove').removeClass('fa-circle-o-notch fa-spin');
                $(context).find('text').text('تعطيل');
                $('.advData ul:nth-of-type(' + ID + ') li').eq(9).text('True');
            }
            if (result.result == false)
            {
                alert('erro in system');
            }
            
        },
        error: function (errormessage) {
            alert("error is system" + errormessage.responseText);
        }
    });
}
function updateLink(context, controller,func) {
    $(context).parent().prev().attr('href', '/admin/' + controller + '/' + controller + '?' + func + '=' + $(context).val());
}
/*sorting function*/
function customSort(obj)
{

    var min = 0;
    var minObj = {};
    for (var i = 0; i < obj.length; i++)
    {
        min = obj[i].id;
        for (var j = i + 1; j < obj.length; j++)
        {
            if (min > obj[j].id)
            {
                min = obj[j].id;
                minObj=obj[j];
                obj[j] = obj[i];
                obj[i] = minObj;
            }
        }
    }
    return obj;
}
function SortDataByName(arr, col)
{
    var min = 0;
    var minObj = {};
    if (col == "dateOfJoin")
    {
        for (var i = 0; i < arr.length; i++) {
            min = new Date(parseInt((arr[i].dateOfJoin).substr(6)));
            for (var j = i + 1; j < arr.length; j++) {
                if (min > new Date(parseInt((arr[j].dateOfJoin).substr(6)))) {
                    min = new Date(parseInt((arr[j].dateOfJoin).substr(6)));
                    minObj = arr[j];
                    arr[j] = arr[i];
                    arr[i] = minObj;
                }
            }
        }
    }
    else if (col == "reservingDate")
    {
        for (var i = 0; i < arr.length; i++) {
            min = new Date(parseInt((arr[i].reservingDate).substr(6)));
            for (var j = i + 1; j < arr.length; j++) {
                if (min > new Date(parseInt((arr[j].reservingDate).substr(6)))) {
                    min = new Date(parseInt((arr[j].reservingDate).substr(6)));
                    minObj = arr[j];
                    arr[j] = arr[i];
                    arr[i] = minObj;
                }
            }
        }
    }
    else if (col == "price") {
        for (var i = 0; i < arr.length; i++) {
            min = arr[i].examinFees;
            for (var j = i + 1; j < arr.length; j++) {
                if (min >arr[j].examinFees) {
                    min = arr[j].examinFees;
                    minObj = arr[j];
                    arr[j] = arr[i];
                    arr[i] = minObj;
                }
            }
        }
    }
   
    return arr;
}
/*function update design after page loaded date*/
function updatedesign()
{
    if (controllerName == "doctor") {
        $('.navbar-inverse .navbar-nav > li').eq(1).siblings('.active').removeClass('active');
        $('.navbar-inverse .navbar-nav > li').eq(1).parent().siblings('ul').find('.active').removeClass('active');
        $('.navbar-inverse .navbar-nav > li').eq(1).addClass('active');
    }
   else  if (controllerName == "patient") {
        $('.navbar-inverse .navbar-nav > li').eq(2).siblings('.active').removeClass('active');
        $('.navbar-inverse .navbar-nav > li').eq(2).parent().siblings('ul').find('.active').removeClass('active');
        $('.navbar-inverse .navbar-nav > li').eq(2).addClass('active');
   }
   else if (controllerName == "reserving") {
       $('.navbar-inverse .navbar-nav > li').eq(3).siblings('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(3).parent().siblings('ul').find('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(3).addClass('active');
   }
   else if (controllerName == "admin") {
       $('.navbar-inverse .navbar-nav > li').eq(4).siblings('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(4).parent().siblings('ul').find('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(4).addClass('active');
   }
   else if (controllerName == "adv") {
       $('.navbar-inverse .navbar-nav > li').eq(5).siblings('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(5).parent().siblings('ul').find('.active').removeClass('active');
       $('.navbar-inverse .navbar-nav > li').eq(5).addClass('active');
   }
   else if (controllerName == "index") {
       $('.navbar-inverse .navbar-nav > li').removeClass('active');
       $('.navbar-inverse .navbar-nav > li.main').addClass('active');
   }

}
function alertNoData(context) {
    $('table tbody').append('<tr class="alert alert-danger"><td colspan="20">لا يوجد بيانات متعلقة بالبحث</td></tr');
}
/*jquery function on page loaded*/
$(function () {
    /*modal*/
    $('.modal-body form').find('input,textarea').keyup(function () {
        $(this).css('border-color', 'lightgray');
        $(this).next().text('');
    });
    /*modal*/
    $('i.showPassword').mouseenter(function () {
        $(this).siblings('#password').attr('type', 'text');
    });
    $('i.showPassword').mouseleave(function () {
        $(this).siblings('#password').attr('type', 'password');
    });
    $('table tbody').append('<tr class="alert alert-danger hidden"><td colspan="20">لا يوجد بيانات متعلقة بالبحث</td></tr');
    var reservingSpSelect = $('.specifiedFilterSpeciality select');
    //fill the speciality select with data
    $('.filterSp').click(function () {
        if ($(reservingSpSelect).hasClass('filled')) return;
        var specialities = [];
        for (var i = 0; i < allData.length; i++) {
            var obj = { name: allData[i].spName.trim(), id: allData[i].docSpId };
            specialities.push(obj);
        };
        var destinctSpecialities = [];
        specialityObj = customSort(specialities);
        for (var i = 0; i < specialityObj.length-1; i++) {
            if (specialityObj[i].id != specialityObj[i + 1].id) destinctSpecialities.push(specialityObj[i]);
            if (i == specialityObj.length -2) destinctSpecialities.push(specialityObj[i+1]);
        }
        $(reservingSpSelect).append('<option value="0">......</option>');
        for (var i = 0; i < destinctSpecialities.length; i++) {
            var sp = destinctSpecialities[i];
            $(reservingSpSelect).append('<option value="'+sp.id +'">'+ sp.name + '</option>');
        }
        $(reservingSpSelect).addClass('filled');
    });
    $('input#email').focus(function () {
        $(this).css('border-color', 'lightgrey');
    });
    $('table td>a').addClass('btn btn-primary');
    $('.specifiedFilter>i').click(function () {
        $(this).parent().fadeOut();
    });
    /*table controls*/
    $('.paging i.fa-star').click(function () {
        filteredData = allData;
        getRecords($('#pagingBtn'));
    });
    $('th.filter').append("<i class='fa  fa-caret-up'></i>");
    $('.filterDate,.filterName,.filterSp').find('i').removeClass('fa-caret-up').addClass('fa-filter');
    $('th.filter>i').click(function () {
        if (!$(this).parent().hasClass('filterDate') && !$(this).parent().hasClass('filterName')&& !$(this).parent().hasClass('filterSp')) {/*sorting*/
            var field = $(this).parent();
            var col = $(field).attr('data-value').trim();
            if ($(field).hasClass('sorted')) {
                allData.reverse();
                getRecords($('#pagingBtn'));
                $(this).removeClass('fa-caret-down').addClass('fa-caret-up');
            }
            else {
                $(field).addClass('sorted');
                getSortedData(null, col);
                $(this).removeClass('fa-caret-up').addClass('fa-caret-down');
            }
        }
        else if ($(this).parent().hasClass('filterDate')) {/*filtering by  date*/;
            $('.specifiedFilterDate').toggle();
        }
        else if ($(this).parent().hasClass('filterSp')) {/*filtering by  date*/;
            $('.specifiedFilterSpeciality').toggle();
        }
        else if ($(this).parent().hasClass('filterName')) {/*filtering by  name*/;
            $('.specifiedFilterName').toggle();
            $('.specifiedFilterName input').attr('data-value', $(this).parent().attr('data-value'));
        }

    });
/*end of table controls*/
    /*upload image*/
    $('#img').on('change', function (e) {
        $('.profile .progress').removeClass('hidden');
        var files = e.target.files;
        var filename = $(this).val().split('\\').pop();
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                data.append("file", files[0]);
                $.ajax({
                    xhr: function () {
                        var xhr = new window.XMLHttpRequest();
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = evt.loaded / evt.total;
                                percentComplete = parseInt(percentComplete * 100);
                                $('.progress').attr("value", percentComplete);

                                if (percentComplete === 100) {
                                    $('.profile .progress').addClass('hidden');
                                }

                            }
                        }, false);

                        return xhr;
                    },
                    type: "POST",
                    url: '/admin/admin/saveImage',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        if(result)
                        {
                            $('.profile .adminImage img').attr('src', "/userImages/" + filename);
                            $('.dropdown .profileImage').attr('src', "/userImages/" + filename);
                        }
                        else {
                            alert("sorry,uploading error,try in another time");
                        }
                    },

                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            } else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });
    /*uploading image*/
});
/*jquery function on page loaded*/
function fillScreenImage(img, context) {
    if ($(img).hasClass('fullScreenImage'))
    {
        $(img).css('transform', 'scale3d(1,1,1)');
        $(img).removeClass('fullScreenImage');
        $(context).parent().removeClass('fullScreenButton');
        return;
    }
    $(img).addClass('fullScreenImage');
    $(img).css('transform','scale3d('+($("body").width()/$(img).width()*.75)+','+$("body").height()/$(img).height()*.75+',1)');
    $(context).parent().addClass('fullScreenButton'); 
}
function checkAdminIfExists(inp)
{
    var newName = $(inp).val().trim();
    var oldName = $(inp).attr('data-value');
    if (newName == oldName) return;
    else {
        $.post('/users/admin/checkIfNameIsExists', { name: newName }, function (result,status) {
            if (result == false) return;
            else {

            }
        });
    }
}