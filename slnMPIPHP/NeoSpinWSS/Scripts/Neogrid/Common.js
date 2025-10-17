/// <reference path="typings/extended.d.ts" />
/// <reference path="typings/cryptojs.d.ts" />
"use strict";
var MVVMGlobal;
(function (MVVMGlobal) {
    MVVMGlobal.idictSelectedControls = undefined;
})(MVVMGlobal || (MVVMGlobal = {}));
var nsCommon;
(function (nsCommon) {
    function GetFormNameFromDivID(astrDiveID) {
        if (astrDiveID == "" || astrDiveID == undefined) {
            return "";
        }
        var no = astrDiveID.replace(nsConstants.REGX_NUMBER, '');
        var lstrFormID = astrDiveID.replace(no, "");
        return lstrFormID.replace("_CorrDiv", "");
    }
    nsCommon.GetFormNameFromDivID = GetFormNameFromDivID;
    function GetActiveDivId(lbtnSelf, e) {
        var ActiveDivID = "";
        if ($(lbtnSelf).closest('div[id^="wfm"]').length > 0) {
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }
        else if (ns.viewModel.currentModel != undefined) {
            ActiveDivID = ns.viewModel.currentModel;
        }
        return ActiveDivID;
    }
    nsCommon.GetActiveDivId = GetActiveDivId;
    function DispalyMessage(astrMessage, ActiveDivID) {
        //hook to change msg at solution side.
        var fn = nsUserFunctions["ChangeDispalyMessage"];
        if (typeof fn === 'function') {
            var Context = {
                activeDivID: ActiveDivID,
                Message: astrMessage
            };
            var e = {};
            e.context = Context;
            var lstrMsg = fn(e);
            if (lstrMsg !== undefined) {
                astrMessage = lstrMsg;
            }
        }
        var MesageDiv = $([nsConstants.HASH, ActiveDivID].join('')).find([nsConstants.HASH, nsConstants.GLOBAL_MESSAGE_DIV].join('')).first();
        if (MesageDiv.length === 0) {
            $([nsConstants.HASH, ActiveDivID].join('')).first().prepend(["<div id='", nsConstants.GLOBAL_MESSAGE_DIV, "'></div>"].join(''));
            MesageDiv = $([nsConstants.HASH, ActiveDivID].join('')).find([nsConstants.HASH, nsConstants.GLOBAL_MESSAGE_DIV].join('')).first();
        }
        if (MesageDiv.hasClass(nsConstants.GLOBAL_ERROR)) {
            MesageDiv.removeClass(nsConstants.GLOBAL_ERROR);
        }
        MesageDiv.addClass(nsConstants.GLOBAL_MESSAGE);
        $([nsConstants.HASH, ActiveDivID, nsConstants.ERROR_DIV].join('')).html("").hide();
        MesageDiv.html("");
        if (astrMessage == "" || astrMessage == undefined) {
            MesageDiv.hide();
        }
        else {
            MesageDiv.html(astrMessage).show();
        }
    }
    nsCommon.DispalyMessage = DispalyMessage;
    function DestroyAllWidgetsOnForm(astrFormID) {
        //Destroy all widget controls on form
        var lobjWidgetControls = nsCommon.GetWidgetControlsByDivID(astrFormID);
        if (lobjWidgetControls != undefined) {
            for (var lstrControlId in lobjWidgetControls) {
                var lobjWidgetControl = lobjWidgetControls[lstrControlId];
                if (lobjWidgetControl != undefined && lobjWidgetControl.jsObject != undefined) {
                    lobjWidgetControl.destroy();
                    lobjWidgetControl.jsObject = null;
                    lobjWidgetControl = null;
                }
            }
        }
    }
    nsCommon.DestroyAllWidgetsOnForm = DestroyAllWidgetsOnForm;
    /*
    *Method to get the wideget controls from view model by active div id
    */
    function RemoveWidgetByActiveDivIdAndControlId(astrActiveDivID, astrControlID) {
        astrControlID = astrControlID.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
        if (astrActiveDivID.lastIndexOf(nsConstants.LOOKUP) > 0 || astrActiveDivID.indexOf("rpt") === 0 || astrActiveDivID.indexOf("cor") === 0 || astrActiveDivID.indexOf(nsConstants.CORRESPONDENCE_CLIENT_MVVM) === 0 || astrActiveDivID.indexOf(nsConstants.REPORT_CLIENT_MVVM) === 0) {
            if (ns.Templates[astrActiveDivID] != undefined && ns.Templates[astrActiveDivID].WidgetControls != undefined && ns.Templates[astrActiveDivID].WidgetControls[astrControlID] != undefined) {
                ns.Templates[astrActiveDivID].WidgetControls[astrControlID] = null;
                delete ns.Templates[astrActiveDivID].WidgetControls[astrControlID];
            }
        }
        else {
            if (ns.viewModel[astrActiveDivID] != undefined && ns.viewModel[astrActiveDivID].WidgetControls != undefined && ns.viewModel[astrActiveDivID].WidgetControls[astrControlID] != undefined) {
                ns.viewModel[astrActiveDivID].WidgetControls[astrControlID] = null;
                delete ns.viewModel[astrActiveDivID].WidgetControls[astrControlID];
            }
        }
    }
    nsCommon.RemoveWidgetByActiveDivIdAndControlId = RemoveWidgetByActiveDivIdAndControlId;
    /*
    *Method to get the wideget control from view model by control
    */
    function GetWidgetByActiveDivIdAndControlId(astrActiveDivID, astrControlID) {
        var lobjWidgetControls = nsCommon.GetWidgetControlsByDivID(astrActiveDivID);
        if (lobjWidgetControls != undefined) {
            astrControlID = astrControlID.replace(nsConstants.GRID_TABLE_UNDERSCORE, "");
            return lobjWidgetControls[astrControlID];
        }
        return undefined;
    }
    nsCommon.GetWidgetByActiveDivIdAndControlId = GetWidgetByActiveDivIdAndControlId;
    /*
    *Method to get the wideget control from view model by control
    */
    function GetWidgetControl(aobjControl) {
        if (aobjControl.length > 0)
            return GetWidgetByActiveDivIdAndControlId(nsCommon.GetActiveDivId(aobjControl), aobjControl[0].id);
        else
            return undefined;
    }
    nsCommon.GetWidgetControl = GetWidgetControl;
    /*
    *Method to get the wideget controls from view model by active div id
    */
    function GetWidgetControlsByDivID(astrActiveDivID) {
        var lobjWidgetControls;
        if (astrActiveDivID.lastIndexOf(nsConstants.LOOKUP) > 0 || astrActiveDivID.indexOf("rpt") === 0 || astrActiveDivID.indexOf("cor") === 0 || astrActiveDivID.indexOf(nsConstants.CORRESPONDENCE_CLIENT_MVVM) === 0 || astrActiveDivID.indexOf(nsConstants.REPORT_CLIENT_MVVM) === 0) {
            lobjWidgetControls = (ns.Templates[astrActiveDivID] != undefined && ns.Templates[astrActiveDivID].WidgetControls != undefined) ? ns.Templates[astrActiveDivID].WidgetControls : {};
        }
        else {
            lobjWidgetControls = (ns.viewModel[astrActiveDivID] != undefined && ns.viewModel[astrActiveDivID].WidgetControls != undefined) ? ns.viewModel[astrActiveDivID].WidgetControls : {};
        }
        return lobjWidgetControls;
    }
    nsCommon.GetWidgetControlsByDivID = GetWidgetControlsByDivID;
    /*
    *Method to set the wideget control in view model by active div-id
    */
    function SetWidgetControlByDivID(astrControlId, aControl, astrActiveDivID, astrControlType) {
        if (astrActiveDivID.lastIndexOf(nsConstants.LOOKUP) > 0 || astrActiveDivID.indexOf("rpt") === 0 || astrActiveDivID.indexOf("cor") === 0 || astrActiveDivID.indexOf(nsConstants.CORRESPONDENCE_CLIENT_MVVM) === 0 || astrActiveDivID.indexOf(nsConstants.REPORT_CLIENT_MVVM) === 0) {
            ns.Templates[astrActiveDivID].WidgetControls[astrControlId] = aControl;
            if (astrControlType === "sfwTabContainer") {
                var key;
                for (key in aControl.idictTabs) {
                    ns.Templates[astrActiveDivID].WidgetControls[key] = aControl.idictTabs[key];
                }
            }
        }
        else {
            ns.viewModel[astrActiveDivID].WidgetControls[astrControlId] = aControl;
            if (astrControlType === "sfwTabContainer") {
                var key;
                for (key in aControl.idictTabs) {
                    ns.viewModel[astrActiveDivID].WidgetControls[key] = aControl.idictTabs[key];
                }
            }
        }
    }
    nsCommon.SetWidgetControlByDivID = SetWidgetControlByDivID;
    function Eval(astrExpression) {
        return new Function(["return ", astrExpression].join(''))();
    }
    nsCommon.Eval = Eval;
})(nsCommon || (nsCommon = {}));

var nsUserFunctions;
(function (nsUserFunctions) {
    var tempVar;
})(nsUserFunctions || (nsUserFunctions = {}));
var MVVMGlobal;
(function (MVVMGlobal) {
    var iarrPopulatedCascadingList = new Array();
    var istrWebServiceName = "SagitecWebServices.asmx";
    var CanDeleteForm = true;
    function GetHeaders() {
        var WindowName = window.name;
        if (WindowName == "" || WindowName == undefined) {
            var lsrtTempName = sessionStorage.getItem(nsConstants.LOGIN_WINDOW_NAME);
            if (lsrtTempName != null) {
                WindowName = lsrtTempName;
                window.name = WindowName;
            }
        }
        var RequestVerificationToken = $("#antiForgeryToken").val();
        var Header = {
            'RequestVerificationToken': RequestVerificationToken,
            'WindowName': WindowName
        };
        return Header;
    }
    MVVMGlobal.GetHeaders = GetHeaders;
    function GetControlAttribute(astrControl, astrAttribute, astrActiveDivID) {
        if (typeof astrActiveDivID === "undefined") {
            astrActiveDivID = "";
        }
        var returnValue;
        if (astrControl.length === 0 || astrControl.length === undefined)
            astrControl = $(astrControl);
        if (astrControl.length === 0)
            return null;
        if (astrControl[0].id.indexOf(nsConstants.GRID_TABLE_UNDERSCORE) == 0) {
            astrControl = $([nsConstants.HASH, astrActiveDivID, nsConstants.SPACE_HASH, astrControl[0].id.replace(nsConstants.GRID_TABLE_UNDERSCORE, "")].join(''));
        }
        if (typeof astrActiveDivID === "undefined") {
            astrActiveDivID = "";
        }
        var returnValue = astrControl[0].getAttribute(astrAttribute);
        if (astrControl.length > 1) {
            returnValue = astrControl[0].getAttribute(astrAttribute);
        }
        if (returnValue !== null && returnValue !== undefined)
            return returnValue;
        var controlID = $(astrControl)[0].id;
        if ($(astrControl).closest('div[PopupDialog="true"]').length > 0) {
            astrActiveDivID = nsCommon.GetActiveDivId(astrControl); //$(astrControl).closest('div[id^="wfm"]')[0].id;
        }
        if (astrActiveDivID === "") {
            astrActiveDivID = nsCommon.GetActiveDivId(astrControl); //$(astrControl).closest('div[id^="wfm"]')[0].id;
        }
        var no = astrActiveDivID.replace(nsConstants.REGX_NUMBER, '');
        astrActiveDivID = astrActiveDivID.replace(no, "");
        try {
            if (controlID.indexOf(nsConstants.LISTVIEW_CONTAINER_UNDERSCORE) == 0) {
                controlID = controlID.replace(nsConstants.LISTVIEW_CONTAINER_UNDERSCORE, "");
                controlID = [controlID, "_ListViewElement_", controlID].join('');
            }
            else if ($(astrControl).closest(nsConstants.LISTVIEW_CONTAINER_STARTWITH_SELECTOR).length > 0) {
                var listViewId = $(astrControl).closest(nsConstants.LISTVIEW_CONTAINER_STARTWITH_SELECTOR)[0].id.replace(nsConstants.LISTVIEW_CONTAINER_UNDERSCORE, "");
                var controlName = $(astrControl).attr("name");
                controlID = [$.trim(controlName), "_ListViewElement_", listViewId].join('');
            }
            if (ns.Templates[astrActiveDivID].ControlAttribites[controlID] == undefined) {
                if (ns.Templates[astrActiveDivID].QuestionnaireControlAttributes != undefined && ns.Templates[astrActiveDivID].QuestionnaireControlAttributes[controlID] != undefined) {
                    returnValue = ns.Templates[astrActiveDivID].QuestionnaireControlAttributes[controlID][astrAttribute];
                    return returnValue == undefined ? null : returnValue;
                }
                return null;
            }
            returnValue = ns.Templates[astrActiveDivID].ControlAttribites[controlID][astrAttribute];
            return returnValue === undefined ? null : returnValue;
        }
        catch (ex) {
            console.log(["error in getting control attribute ", astrAttribute, " in form ", astrActiveDivID].join(''));
            return null;
        }
    }
    MVVMGlobal.GetControlAttribute = GetControlAttribute;
    function StartsWith(aStrValue, aStrSearchValue) {
        var length = aStrSearchValue.length;
        return aStrValue.substr(0, length) == aStrSearchValue;
    }
    MVVMGlobal.StartsWith = StartsWith;
    function RegisterEvents() {
        MVVMGlobal.CheckNSetStorageMethods();
        $(document).on('click', "a[focusControl]", function (e) {
            var FocusControl = $(this).attr("focusControl");
            var ActiveDivID = nsCommon.GetActiveDivId(this).replace(nsConstants.ERROR_DIV, ""); //$(this).closest('div[id^="wfm"]')[0].id.replace(nsConstants.ERROR_DIV, "");
            var control = $([nsConstants.HASH, ActiveDivID, nsConstants.SPACE_HASH, FocusControl].join(''));
            var lobjHtmlEditorWidget = nsCommon.GetWidgetControl(control);
            if (control.length > 0 && lobjHtmlEditorWidget != undefined && lobjHtmlEditorWidget.jsObject != undefined) {
                lobjHtmlEditorWidget.focus(ns.blnHighlightErrorControlsOnClick);
            }
            else if (control.length > 0 && (control[0].tagName === nsConstants.INPUT_TAG && (control[0].type === nsConstants.CHECKBOX || control[0].type === nsConstants.RADIO))) {
                control.focus();
                if (ns.blnHighlightErrorControlsOnClick) {
                    ns.RemoveHighlightingFromControls(ActiveDivID);
                    control.addClass("HighlightError");
                    control.parent().addClass("HighlightError");
                }
            }
            else if (control.length > 0 && (control[0].tagName === nsConstants.INPUT_TAG && control[0].type === "file")) {
                control.focus();
                if (ns.blnHighlightErrorControlsOnClick) {
                    ns.RemoveHighlightingFromControls(ActiveDivID);
                    control.addClass("HighlightError");
                    control.closest(".k-upload-button").addClass("HighlightError");
                }
            }
            else if (control.length > 0) {
                control.focus();
                if (ns.blnHighlightErrorControlsOnClick) {
                    ns.RemoveHighlightingFromControls(ActiveDivID);
                    control.addClass("HighlightError");
                }
            }
            else if (control.wrapper !== undefined) {
                if (ns.blnHighlightErrorControlsOnClick) {
                    ns.RemoveHighlightingFromControls(ActiveDivID);
                    control.wrapper.find(".k-input").addClass("HighlightError");
                }
            }
        });
        $(document).on('click', "input[ReadOnlyCheckBox]", function (e) {
            e.preventDefault();
            return false;
        });
        $("#CenterSplitter").scroll(function () {
            var ActiveDivID = "";
            if (ns.viewModel.currentForm.indexOf(nsConstants.LOOKUP) > 0)
                ActiveDivID = ns.viewModel.currentForm;
            else
                ActiveDivID = ns.viewModel.currentModel;
            ns.SessionStorePageState(ActiveDivID, "scroll", "scroll", $("#CenterSplitter").scrollTop());
        });
        Date.prototype.toString = function () {
            //return this.getFullYear() + "-" + (this.getMonth() + 1) + "-" + this.getDate() + " " + this.getHours() + ":" + this.getMinutes() + ":" + this.getSeconds();
            return MVVM.ServiceLoad.ToString(this, "yyyy-MM-ddTHH:mm:ss");
        };
        Date.prototype.toJSON = function () {
            //return this.getFullYear() + "-" + (this.getMonth() + 1) + "-" + this.getDate() + " " + this.getHours() + ":" + this.getMinutes() + ":" + this.getSeconds();
            return MVVM.ServiceLoad.ToString(this, "yyyy-MM-ddTHH:mm:ss");
        };
        //for centerleft menu
        $(document).on('click', "input[Base_Click='true']", function (e) {
            nsEvents.btnBase_Click(e);
            $(this).trigger('mouseout');
        });
        $(document).on('click', "a[linkbutton='true']", function (e) {
            nsEvents.btnBase_Click(e);
            $(this).trigger('mouseout');
        });
        $(document).on('click', '.check_row', function (e) {
            var $cb = $(this);
            var ActiveDivID = nsCommon.GetActiveDivId(this); //$cb.closest('div[id^="wfm"]')[0].id;
            var lobjGridWidget;
            var lstrRelatedControlId;
            if ($cb.attr("ListViewId") != undefined && $cb.closest(nsConstants.LISTVIEW_CONTAINER_STARTWITH_SELECTOR).length > 0) {
                lstrRelatedControlId = $cb.closest(nsConstants.LISTVIEW_CONTAINER_STARTWITH_SELECTOR)[0].id;
            }
            else {
                lstrRelatedControlId = $cb.attr("GridID");
            }
            lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lstrRelatedControlId);
            if (lobjGridWidget == undefined && lobjGridWidget.jsObject == undefined) {
                return false;
            }
            var index = $cb.attr("rowindex");
            index = parseInt(index);
            var lchkAll = $(["#checkAll_", lstrRelatedControlId].join(''));
            if (($cb[0]).type === nsConstants.RADIO) {
                setTimeout(function () {
                    lobjGridWidget.checkLastSelectedIndex(index);
                    $cb.prop('checked', true);
                }, 100);
            }
            if (lchkAll.length > 0)
                lchkAll.attr("CanCheckAll", nsConstants.TRUE);
            var lblnChecked = $cb.is(':checked');
            //Fix for chrome, sometimes radio button gives on/off for checked/unchecked
            if (($cb[0]).type === nsConstants.RADIO) {
                lblnChecked = lblnChecked === "on" ? true : (lblnChecked === "off" ? false : lblnChecked);
            }
            lobjGridWidget.selectRowByIndex(index, lblnChecked);
        });
      
        $(document).on('click', '.checkAllPages', function (e) {
            MVVM.ServiceLoad.CheckAllPages_Click(e, this);
        });
        $(document).on('click', "#clickExcel", function () {
            MVVMGlobal.ExportToExcel();
        });
        //Register User Defined Events
        var fn = nsUserFunctions["InitilizeUserDefinedEvents"];
        if (typeof fn === 'function') {
            fn();
        }
    }
    MVVMGlobal.RegisterEvents = RegisterEvents;
    function ExportToExcel() {
        var lstrActiveDivID = nsCommon.GetActiveDivId(ns.viewModel.srcElement);
        var lstrGridID = MVVMGlobal.GetControlAttribute(ns.viewModel.srcElement, nsConstants.SFW_RELATED_CONTROL);
        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lstrActiveDivID, lstrGridID);
        if (lobjGridWidget != undefined && lobjGridWidget.jsObject != undefined) {
            var lblnResult = lobjGridWidget.exportToExcel();
            if (!lblnResult)
                return;
        }
        //var ExportWindow = $('#DivExportWindow');
        if (ns.arrDialog['DivExportWindow'] != undefined) {
            ns.arrDialog['DivExportWindow'].close();
        }
        // log the export activity in user Activity ..
        nsRequest.AjaxRequest({
            action: "GetLogExcelUserActivity"
        });
    }
    MVVMGlobal.ExportToExcel = ExportToExcel;
    function Generateguid() {
        return [MVVMGlobal.GuidPartCreator(), MVVMGlobal.GuidPartCreator(), '-', MVVMGlobal.GuidPartCreator(), '-', MVVMGlobal.GuidPartCreator(), '-',
            MVVMGlobal.GuidPartCreator(), '-', MVVMGlobal.GuidPartCreator(), MVVMGlobal.GuidPartCreator(), MVVMGlobal.GuidPartCreator()].join('');
    }
    MVVMGlobal.Generateguid = Generateguid;
    function GuidPartCreator() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    MVVMGlobal.GuidPartCreator = GuidPartCreator;
})(MVVMGlobal || (MVVMGlobal = {}));

var MVVM;
(function (MVVM) {
    var JQueryControls;
    (function (JQueryControls) {
        /**
       *Dialog  Control Widget Wrapper.
       */
        var Dialog = (function () {
            /**
           *Constructor for Display window.
           *@param {JQuery} element Jquery element Object.
           *@param {String} astrActiveDivID Jquery Object.
           *@param {Object} aobjDialogOptions Dialog options Object
           *@Return DialogControl Object
           */
            function Dialog(element, astrActiveDivID, aobjDialogOptions) {
                this.element = element;
                this.jsObject = undefined;
                this.istrActiveDivID = astrActiveDivID;
                this.SetDialogOptions(aobjDialogOptions);
                this.init();
            }
            /**
           *Display popup window.
           *@param {any} aobjDialogOptions Dialog Options with which the Dialog Panel needs to be created.
           */
            Dialog.prototype.SetDialogOptions = function (aobjDialogOptions) {
                var _this = this;
                if (aobjDialogOptions == undefined)
                    aobjDialogOptions = {};
                aobjDialogOptions.height = (aobjDialogOptions.height == undefined) ? "auto" : aobjDialogOptions.height.toLowerCase().replace('px', '');
                aobjDialogOptions.width = (aobjDialogOptions.width == undefined) ? "auto" : aobjDialogOptions.width.toLowerCase().replace('px', '');
                if (aobjDialogOptions.height.indexOf('%') > 0) {
                    aobjDialogOptions.height = ($('body').height() * aobjDialogOptions.height.split('%')[0]) / 100;
                }
                aobjDialogOptions.draggable = true;
                aobjDialogOptions.position = undefined;
                switch (aobjDialogOptions.dialogName) {
                    case "correspondence":
                        aobjDialogOptions.draggable = false;
                        aobjDialogOptions.position = { at: "center top" };
                        aobjDialogOptions.dialogClass = "fixtop";
                        break;
                }
                var lcloseFun;
                // set the close function for Dialog
                if (typeof aobjDialogOptions.close !== 'function') {
                    switch (aobjDialogOptions.close) {
                        case "ColumnsToExport":
                            aobjDialogOptions.close = function () {
                                $("#DivExportCols")[0].innerHTML = "";
                            };
                            break;
                        case "Correspondence":
                            aobjDialogOptions.draggable = false;
                            aobjDialogOptions.position = { at: "center top" };
                            aobjDialogOptions.dialogClass = "fixtop";
                            aobjDialogOptions.close = function () {
                                var obj;
                                obj = document.getElementById("ControlWordExcelObj");
                                try {
                                    if (obj) {
                                        obj.Close("W");
                                    }
                                }
                                catch (ex) { }
                            };
                            break;
                        case "Retrieve":
                            aobjDialogOptions.draggable = false;
                            aobjDialogOptions.position = { at: "center top" };
                            aobjDialogOptions.dialogClass = "fixtop";
                            aobjDialogOptions.close = function (e) {
                                if (_this.jsObject != undefined)
                                    _this.jsObject.destroy();
                                ns.blnFromDeleteTreeNode = true;
                                ns.destroyAll(aobjDialogOptions.extraDivId);
                                ns.blnFromDeleteTreeNode = false;
                                var lobjRetrival = ns.ParentFormsForRetrival[aobjDialogOptions.extraDivId];
                                var lstrParentFormID = lobjRetrival.ParentForm;
                                ns.viewModel.currentModel = lstrParentFormID;
                                ns.viewModel.currentForm = nsCommon.GetFormNameFromDivID(lstrParentFormID);
                                //check for autocomplete lists, and if exists, remove it from dom. because it is being created again & again when we open retrieval
                                $("ul.ui-autocomplete[relatedtextboxid]").remove();
                                if (aobjDialogOptions.arrObjCollection != undefined && aobjDialogOptions.arrObjCollection.arrCollection != undefined)
                                    delete aobjDialogOptions.arrObjCollection.arrCollection[aobjDialogOptions.arrObjCollection.divID];
                            };
                            break;
                        case "DisplayChart":
                            aobjDialogOptions.close = function (e) {
                                if (ns.GridGroupChart != undefined) {
                                    ns.ClearGridChart();
                                }
                            };
                            break;
                        case "empty":
                            aobjDialogOptions.close = function (e) { };
                            break;
                        case "OpenPrototype":
                            aobjDialogOptions.draggable = false;
                            aobjDialogOptions.position = { at: "center top" };
                            aobjDialogOptions.dialogClass = "fixtop";
                            aobjDialogOptions.close = function (e) {
                                ns.viewModel.srcElement = undefined;
                                ns.viewModel.currentForm = ns.viewModel.previousForm;
                                ns.viewModel.currentModel = ns.viewModel.previousDiv;
                                $(_this.element).parent().remove();
                            };
                        default:
                            aobjDialogOptions.close = function (e) {
                                if (_this.jsObject != undefined)
                                    _this.jsObject.destroy();
                                if (aobjDialogOptions.arrObjCollection != undefined && aobjDialogOptions.arrObjCollection.arrCollection != undefined)
                                    delete aobjDialogOptions.arrObjCollection.arrCollection[aobjDialogOptions.arrObjCollection.divID];
                            };
                    }
                }
                if (typeof aobjDialogOptions.deactivate !== 'function') {
                    // set the close function for Dialog 
                    switch (aobjDialogOptions.deactivate) {
                        case "OpenPrototype":
                            aobjDialogOptions.draggable = false;
                            aobjDialogOptions.position = { at: "center top" };
                            aobjDialogOptions.dialogClass = "fixtop";
                            aobjDialogOptions.deactivate = function (e) {
                                if (_this.jsObject != undefined) {
                                    _this.jsObject.destroy();
                                    $(_this.element).remove();
                                    if (aobjDialogOptions.arrObjCollection != undefined && aobjDialogOptions.arrObjCollection.arrCollection != undefined)
                                        delete aobjDialogOptions.arrObjCollection.arrCollection[aobjDialogOptions.arrObjCollection.divID];
                                }
                            };
                            break;
                        case "empty":
                            aobjDialogOptions.deactivate = function (e) { };
                            break;
                        default:
                            aobjDialogOptions.deactivate = function (e) {
                                if (_this.jsObject != undefined) {
                                    //this.jsObject.close();
                                    _this.jsObject.destroy();
                                    $(_this.element).remove();
                                    if (aobjDialogOptions.arrObjCollection != undefined && aobjDialogOptions.arrObjCollection.arrCollection != undefined)
                                        delete aobjDialogOptions.arrObjCollection.arrCollection[aobjDialogOptions.arrObjCollection.divID];
                                }
                            };
                    }
                }
                if (aobjDialogOptions.dialogName == "session") {
                    aobjDialogOptions.closeOnEscape = false;
                    aobjDialogOptions.open = function (event, ui) {
                        $(_this.element).parent().find(".ui-dialog-titlebar-close").hide();
                    };
                }
                else {
                    aobjDialogOptions.open = function (e) { $("html, body").css("overflow", "auto"); };
                    aobjDialogOptions.closeOnEscape = true;
                }
                var dialogObj = {
                    draggable: aobjDialogOptions.draggable,
                    title: aobjDialogOptions.title,
                    height: aobjDialogOptions.height,
                    width: aobjDialogOptions.width,
                    dialogClass: aobjDialogOptions.dialogClass,
                    position: aobjDialogOptions.position,
                    modal: true,
                    open: aobjDialogOptions.open,
                    closeOnEscape: aobjDialogOptions.closeOnEscape,
                    close: function () {
                        aobjDialogOptions.close();
                        aobjDialogOptions.deactivate();
                    }
                };
                this.iobjDialogOptions = dialogObj;
            };
            /**
           *Init method to intialize Display window.
           */
            Dialog.prototype.init = function () {
                this.jsObject = $(this.element).dialog(this.iobjDialogOptions).data('ui-dialog');
                /* if (this.iobjDialogOptions.blnAlignCenter === undefined) {
                     this.show();
                     this.center();
                 }*/
            };
            /**
           *Close method to closes the Display window.
           */
            Dialog.prototype.close = function (isClose) {
                if (this.jsObject != undefined) {
                    if (isClose) {
                        this.jsObject.destroy();
                        $(this.element).remove();
                    }
                    else
                        this.jsObject.close();
                }
            };
            /**
           *Center method opens the Display window.
           */
            Dialog.prototype.center = function () {
                if (this.jsObject != undefined) {
                    this.jsObject.dialog("option", "position", "center");
                }
            };
            /**
           *Open method opens the Display window.
           */
            Dialog.prototype.open = function () {
                if (this.jsObject != undefined) {
                    this.jsObject.option('position', 'center');
                    this.jsObject.open();
                }
            };
            /**
           *Destroy method destroys the Display window object.
           */
            Dialog.prototype.destroy = function () {
                if (this.jsObject != undefined) {
                    this.jsObject.destroy();
                }
            };
            return Dialog;
        }());
        JQueryControls.Dialog = Dialog;
    })(JQueryControls = MVVM.JQueryControls || (MVVM.JQueryControls = {}));
})(MVVM || (MVVM = {}));

function GridRowCheckEditMode(container, options) {
    MVVMGlobal.GridRowCheckEditMode(container, options);
}
function GridCheckBoxEditor(container, options) {
    MVVMGlobal.GridCheckBoxEditor(container, options);
}
function GridRowRadioEditMode(container, options) {
    MVVMGlobal.GridRowRadioEditMode(container, options);
}
function GridTextAreaEditor(container, options) {
    MVVMGlobal.GridTextAreaEditor(container, options);
}
function GridRadioButtonListEditor(container, options) {
    MVVMGlobal.GridRadioButtonListEditor(container, options);
}
function GridDropDownEditor(container, options) {
    MVVMGlobal.GridDropDownEditor(container, options);
}
function checkBoxListener(cnrtl) {
    nsEvents.checkBoxListener(cnrtl);
}
function GridLinkButtonEditor(container, options) {
    MVVMGlobal.GridLinkButtonEditor(container, options);
}

function btnColumnsToExport_Click(sfwrelatedcontrol) {
    var lstrActiveDivID = nsCommon.GetActiveDivId(); //$(ns.viewModel.srcElement).closest('div[id^="wfm"]')[0].id;
    var lstrGridID = sfwrelatedcontrol;
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lstrActiveDivID, lstrGridID);
    if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
        return false;
    }
    if (lobjGridWidget.iintRecordLength == 0) {
        //nsCommon.DispalyError(DefaultMessages.NoRecordPresentToExport);
        return false;
    }
    var divExportCols = $("#DivExportCols");
    var lsrtHtml = lobjGridWidget.getColumnTemplateForExportToExcel();
    divExportCols.append($(lsrtHtml));
    var ExportWindow = $('#DivExportWindow');
    $("#clickExcel").attr("gridid", lstrGridID);
    NeoWidgetsOnForm['DivExportWindow'] = new MVVM.JQueryControls.Dialog(ExportWindow, "", { title: "Export To Excel", width: "500px", close: "ColumnsToExport", deactivate: "empty", arrObjCollection: { arrCollection: ns.arrDialog, divID: 'DivExportWindow' } });
    NeoWidgetsOnForm['DivExportWindow'].open();
    //nsCommon.DispalyMessage(DefaultMessages.ExportToExcel, lstrActiveDivID);
}

$(document).on("click", "[ExportToExcelNeoGrid]", function (e) {
    var sfwrelatedcontrol = $(this).attr("sfwrelatedcontrol");
    if ($("#GridTable_" + sfwrelatedcontrol).data("neoGrid") == undefined) {
        //execute normal web from export to exlce
        return true;
    }
    else {
        //execute neoGrid client side export to excel
        btnColumnsToExport_Click(sfwrelatedcontrol);
        return false;
    }
});

$(document).on('click', "#clickExcel", function () {
    MVVMGlobal.ExportToExcel();
});

function ExportToExcel() {
    var lstrGridID = $("#clickExcel").attr("gridid");
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId("", lstrGridID);
    if (lobjGridWidget != undefined && lobjGridWidget.jsObject != undefined) {
        var lblnResult = lobjGridWidget.exportToExcel();
        if (!lblnResult)
            return;
    }
    //var ExportWindow = $('#DivExportWindow');
    if (NeoWidgetsOnForm['DivExportWindow'] != undefined) {
        NeoWidgetsOnForm['DivExportWindow'].close();
    }

    // log the export activity in user Activity ..
    //nsRequest.AjaxRequest({
    //    action: "GetLogExcelUserActivity"
    //});
}
MVVMGlobal.ExportToExcel = ExportToExcel;

MVVM.JQueryControls.GridView.prototype.getColumnTemplateForExportToExcel = function () {
    var lstrHTML = "";
    var Allcolumns = this.element.find('thead').find("th[data-field]");
    var larrColumns = this.jsObject.columns;
    lstrHTML = "<table id='tblExcelColumns'><tr>";
    var lstrTempHtml = "";
    var lintI = 0, lintJ = 0;
    for (var lintJ = 0; lintJ < Allcolumns.length; lintJ++) {
        var col = Allcolumns[lintJ];
        if (col.getAttribute('data-field') == "rowSelect" || col.getAttribute('data-field') == "rowIndex") {
            continue;
        }
        // added check to show columns for which sfwShowInExportToExcel attribute is true
        if ($(col).text() == undefined || $.trim($(col).text()) === 'Select' || ((larrColumns[lintJ].hidden == nsConstants.TRUE || larrColumns[lintJ].hidden == true) && !(larrColumns[lintJ].sfwShowInExportToExcel != undefined && (larrColumns[lintJ].sfwShowInExportToExcel == true || larrColumns[lintJ].sfwShowInExportToExcel.trim() == "True")))) {
            continue;
        }
        lintI++;
        lstrHTML = [lstrHTML, '<td><label for="ExportChk', lintJ, '"> <input type="checkbox" id="ExportChk', lintJ, '" checked="checked" value="', col.attributes["data-field"].value, '" />', $(col).text(), '</label></td>'].join('');
        if (lintI % 3 === 0) {
            lstrHTML = [lstrHTML, "</tr><tr>"].join('');
        }
    }
    lstrHTML = [lstrHTML, "</tr></table>"].join('');
    return lstrHTML;
};

/*
  *Method to get the wideget control from view model by control
  */
function GetWidgetByActiveDivIdAndControlId(astrActiveDivID, astrControlID) {
    return NeoWidgetsOnForm[astrControlID];
}
nsCommon.GetWidgetByActiveDivIdAndControlId = GetWidgetByActiveDivIdAndControlId;

function GetActiveDivId() {
    return "cphCenterMiddle_tblCenterMiddle";
}
nsCommon.GetActiveDivId = GetActiveDivId;