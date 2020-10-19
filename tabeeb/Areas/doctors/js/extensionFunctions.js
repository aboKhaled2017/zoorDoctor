﻿
$(function () {
    /*this function that operate with array ,
       and take removed element as parameter
       and return new arry with removed passed element from array*/
    $.prototype.removePeriod = function (element) {
        var arr = $(this);
        if (arr.length == 0) return new Array();
        var isPeriod = arr[0].from != undefined;
        arr = $.grep(arr, function (el) {
            if (isPeriod) {
                if (!(el.from == element.from && el.to == element.to)) return el;
            }
            else {
                if (el != element) return el;
            }
        });
        return arr;
    }
    /*this function check if array contains particular element*/
    $.prototype.isContains = function (element) {
        var arr = $(this);
        var isContains = false;
        $(arr).each(function (i, el) {
            if (el.from == element.from && el.to == element.to) { isContains = true; return; }
        });
        return isContains;
    }
    /*compare two objects*/
});
var equalPeriod = function (comparedObject1, comparedObject2) {
    var objectType = typeof (comparedObject1);
    if (comparedObject1.hasOwnProperty('from')) {
        return (comparedObject1.from == comparedObject2.from && comparedObject1.to == comparedObject2.to);
    }
    else {
        return comparedObject1 == comparedObject2;
    }
    return false;
}