/// <reference path="Plane.js" />
/// <reference path="DataContext.js" />

function raiseEvent(callFunction, lstrFormName) {
    if (typeof lstrFormName === "undefined") { lstrFormName = ""; }
    try {
        ns.displayActivity(true);
        $.when(callFunction(lstrFormName)).then(function (data) {
            if (data === undefined)
                return;
            var lstrAction = ns.LastAction;
            if (lstrAction.indexOf("GetData") == 0) {
                LoadMap(data);
            }
            else if (lstrAction.indexOf("GetCallActivityData") == 0) {
                OpenPopup(data, ns.IsRun, ns.ParentShape);
            }
            else if (lstrAction.indexOf("RenderBPM") == 0) {
                RenderMap(data);
            }
            else if (lstrAction.indexOf("RenderReadOnlyBPM") == 0) {
                RenderReadOnlyBPMMap(data);
            }

        }).done([
            function () {
                ns.displayActivity(false);
            }
        ]).fail([
            function () {
                ns.displayActivity(false);
            }
        ]).always([
            function () {
                ns.displayActivity(false);
            }
        ]);
    } catch (e) {
        alert(e.message);
        ns.displayActivity(false);
    }
}

var ns = ns || {};
ns = {
    LastAction: "",
    CurrentRequestObj: "",
    IsRun: false,
    mapData: {},
    timers: {},
    Parent: [],
    displayActivity: function (flag) {
        if (flag) {
            $("#page-loader").show();
        } else {
            $("#page-loader").hide();
        }
    }
};


var nsEvents = nsEvents || {};
nsEvents = {

    GetData: function () {
        ns.deferred = $.Deferred();
        var aintCaseInstanceID = localStorageGet("CaseInstanceID");
        if (aintCaseInstanceID == null)
            aintCaseInstanceID = 0;
        var reqObject = {
            Type: "GET",
            Action: "GetData?aintCaseInstanceID=" + aintCaseInstanceID
        };
        nsRequest.AjaxRequest(reqObject);
        return ns.deferred;
    },
    GetCallActivityData: function () {
        ns.deferred = $.Deferred();
        nsRequest.AjaxRequest(ns.CurrentRequestObj);
        ns.CurrentRequestObj = null;
        return ns.deferred;
    },
    RenderBPM: function () {
        ns.deferred = $.Deferred();
        var design_specification_bpm_map_id = localStorageGet("design_specification_bpm_map_id");
        if (design_specification_bpm_map_id == null)
            design_specification_bpm_map_id = 0;
        var reqObject = {
            Type: "GET",
            Action: "RenderBPM?design_specification_bpm_map_id=" + design_specification_bpm_map_id
        };
        nsRequest.AjaxRequest(reqObject);
        return ns.deferred;
    },
    RenderReadOnlyBPM: function () {
    ns.deferred = $.Deferred();
    var aintProcessId = localStorageGet("ProcessId");
    if (aintProcessId == null)
        aintProcessId = 0;
    var aintCaseId = localStorageGet("CaseId");
    if (aintCaseId == null)
        aintCaseId = 0;
    var reqObject = {
        Type: "GET",
        Action: "RenderReadOnlyBPM?aintProcessId=" + aintProcessId +"&aintCaseId=" + aintCaseId
    };
    nsRequest.AjaxRequest(reqObject);
    return ns.deferred;
}

}

function sessionGet(astrKey) {
    var sessionObject = sessionStorage.getItem(astrKey);
    if (sessionObject !== null) {
        sessionObject = JSON.parse(sessionObject);
    }
    return sessionObject;
}

 function localStorageGet(astrKey) {
    var localObject = localStorage.getItem(astrKey);
    if (localObject !== null) {
        localObject = jQuery.parseJSON(localObject);
    }
    return localObject;
}