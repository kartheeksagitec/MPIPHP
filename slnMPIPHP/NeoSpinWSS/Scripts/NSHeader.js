var lblnCallScript_Init = true;
/// <reference path="/scripts/jquery-1.4.2.min.js" />

function OnChanged(sender, args) {
}


var oControl;
var iaccTabIndex = 0;
//NEO_CERTIFY_CHANGES
var blnLoading = false;

function isPageLoading() {
    return blnLoading;
}

function ieAjaxBeginRequest(sender, args) {
    //To Avoid DoubleClick on button
    blnLoading = true;
    $("#pnlPopup").dialog('open');
    lblnCallScript_Init = true;
    oControl = args.get_postBackElement();
    oControl.disabled = true;
    var luppAccordian = $("#uppAccordian");
    if (luppAccordian.length > 0) {
        var laccChildren = luppAccordian[0].children;
        for (i = 0; i < laccChildren.length; i++) {
            if (laccChildren[i].clientHeight > 50) {
                iaccTabIndex = i - 1;
                iaccTabIndex = iaccTabIndex / 2;
                break;
            }
        }
    }
    window.status = "Please wait...";
    document.body.style.cursor = "wait";
}

function ieAjaxPageLoaded(sender, args) {
    window.status = "Done";
    document.body.style.cursor = "default";
    if (lblnCallScript_Init) {
        BuildPanelsToUpdate(args);
        Script_Init();
    }
}

function ieAjaxEndRequest(sender, args) {
    if (args.get_error()) {
        document.getElementById("lblMessage").innerText = args.get_error().description;
        args.set_errorHandled(true);
    }
    $("#pnlPopup").dialog('close');
    window.status = "Done";
    document.body.style.cursor = "default";
    __defaultFired = false;
    oControl.disabled = false;
    myLayout.resizeAll();

    PositionCursor();
    blnLoading = false;

    //FM upgrade: 6.0.0.28 changes
    if (iblnCallFrameworkGetUserPreference === true) {
        GetUserPreferencesDetails();
    }
}


function MPIPHP_PageLoad(sender, args) {
    if (oControl == undefined) {
        PositionCursor();
    }
}

function Script_Init() {
    //FM upgrade: 6.0.0.28 changes - Remove all initialization Framework code within Scrip_Init() method
    FrameworkInitialize();
    //sfw.Initialize();
    lblnCallScript_Init = false;
    iblnRoundedTabs = false;
    //try {
    //    InitializeWatermark();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeWatermark function, error : " + e.Message);
    //}
    try {
        InitializeMSSMenu();
    }
    catch (e) {
        alert("Error in NSHeader.js InitializeMSSMenu function, error : " + e.Message);
    }
    //try {
    //    Extend_Date();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Date function, error : " + e.Message);
    //}

    //try {
    //    Extend_DateTime();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_DateTime function, error : " + e.Message);
    //}
    //try {
    //    Extend_Time();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Time function, error : " + e.Message);
    //}
    //try {
    //    Extend_MonthYear();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_MonthYear function, error : " + e.Message);
    //}
    try {
        OverRideExtend_SSN();
        $("input[type='text'].classFormatSSN").mask('999-99-9999')
.blur(function (e) {
});
    }
    catch (e) {
        alert("Error in NSHeader.js Extend_SSN function, error : " + e.Message);
    }
    //try {
    //    Extend_Phone();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Phone function, error : " + e.Message);
    //}
    //try {
    //    Extend_Currency();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Currency function, error : " + e.Message);
    //}
    //try {
    //    Extend_Custom();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Custom function, error : " + e.Message);
    //}

    //try {
    //    InitializeComboBox();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeComboBox function, error : " + e.Message);
    //}
    //try {
    //    InitializeCascadingDropDown();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeCascadingDropDown function, error : " + e.Message);
    //}

    //try {
    //    SetClientVisibility();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js SetClientVisibility function, error : " + e.Message);
    //}

    //try {
    //    InitializeListControls();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeListControls function, error : " + e.Message);
    //}
    //try {
    //    InitializeRetrievalTextBox();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeRetrievalTextBox function, error : " + e.Message);
    //}
    try {
        $$("ddlContactTypeValue").change(OnContactTypeChange)
        var lddlConatct = $$("ddlContactTypeValue");
        if (lddlConatct[0].length > 0 && lddlConatct[0].length > 0) {
            var contact_type = lddlConatct[0].value;
            var pnlAddr = $$("pnlAddress");
            var pnlSameAsPart = $$("pnlAddSamsAsPerson");
            var chkCorFlag = $$("chkCorrespondenceAddrFlag");
            var chkCorLabel = $$("capCorrespondenceAddrFlag");
            if (contact_type != null) {
                if (contact_type[0].length > 0) {
                    var check = $$("chkAddrSameAsPerson");
                    if (check[0].checked) {
                        pnlSameAsPart.show();
                        pnlAddr.hide();
                        chkCorLabel.hide();
                        chkCorFlag.hide();

                    }
                    else {
                        var lControlList = GetControlsToChangeState(lddlConatct[0].getAttribute('sfwClientVisibility'), lddlConatct[0].value);
                        if (lControlList != null) {
                            if (lControlList.length == 2) {
                                ChangeVisibility(lControlList[0], true);
                                ChangeVisibility(lControlList[1], false);
                                pnlSameAsPart.hide();
                                pnlAddr.show();
                            }
                        }
                    }
                }
            }
        }
    }
    catch (e) {
    }

    try {
        $$("chkAddrSameAsPerson").click(OnContactTypeChange)
        var lddlConatct = $$("ddlContactTypeValue");
        if (lddlConatct[0].length > 0 && lddlConatct[0].length > 0) {
            var contact_type = lddlConatct[0].value;
            var pnlAddr = $$("pnlAddress");
            var pnlSameAsPart = $$("pnlAddSamsAsPerson");
            var chkCorFlag = $$("chkCorrespondenceAddrFlag");
            var chkCorLabel = $$("capCorrespondenceAddrFlag");
            if (contact_type != null) {
                if (contact_type[0].length > 0) {
                    var check = $$("chkAddrSameAsPerson");
                    if (check[0].checked) {
                       

                        pnlSameAsPart.show();
                        pnlAddr.hide();
                        chkCorLabel.hide();
                        chkCorFlag.hide();

                    }
                    else {
                        var lControlList = GetControlsToChangeState(lddlConatct[0].getAttribute('sfwClientVisibility'), lddlConatct[0].value);
                        if (lControlList != null) {
                            if (lControlList.length == 2) {
                                ChangeVisibility(lControlList[0], true);
                                ChangeVisibility(lControlList[1], false);
                                pnlSameAsPart.hide();
                                pnlAddr.show();
                            }
                        }
                    }
                }
            }
        }
        }
        catch(e){
        }
    //try {
    //    OpenSmartLink();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js OpenSmartLink function, error : " + e.Message);
    //}

    $("[sfwBalloonTooltip]").mouseover(function (e) {
        var txt = e.currentTarget.sfwBalloonTooltip;
        Tip(txt, BALLOON, true, ABOVE, true, STICKY, 1, CLOSEBTN, false, CLICKCLOSE, true, DURATION, 10000, OFFSETX, -12);
    }).mouseout(function () {
        UnTip();
    });

    var luppAccordian = $("#uppAccordian");
    if (luppAccordian.length > 0) {
        if (luppAccordian[0].children.length < (iaccTabIndex * 2 + 1)) {
            iaccTabIndex = 0;
        }
        luppAccordian.accordion({
            fillSpace: true, active: iaccTabIndex
        });

        luppAccordian.each(function () {
            $.removeData(this);
        });
    }

    //try {
    //    HideEmptyButtonCells();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js HideEmptyButtonCells function, error : " + e.Message);
    //}
    //try {
    //    SelectAllRows();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js SelectAllRows function, error : " + e.Message);
    //}
    try {
        SelectAllGridWrapRows();
    }
    catch (e) {
        alert("Error in NSHeader.js SelectAllRows function, error : " + e.Message);
    }
    //try {
    //    InitializeCollapsiblePanels();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeCollapsiblePanels function, error : " + e.Message);
    //}
    //try {
    //    InitializeTabContainer();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeTabContainer function, error : " + e.Message);
    //}

    try {
        o_InitializePrintButton();
    }
    catch (e) {
        alert("Error in NSHeader.js InitializePrintButton function, error : " + e.Message);
    }

    //try {
    //    InitializeComboBox();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeComboBox function, error : " + e.message);
    //}

    //try {
    //    InitializeAutoComplete();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeAutoComplete function, error : " + e.message);
    //}

    //try {
    //    InitializePopupDialogs();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializePopupDialogs function, error : " + e.message);
    //}

    $("[sfwReadOnlyCheckBox]").click(function (event) {
        event.preventDefault();
        return false;
    });

    InitializeTreeView();
    //SetCascadingDropDownValues();

    //FM upgrade: 6.0.0.24 changes
    //RenderNeoGrid();
}

function o_InitializePrintButton() {

    var clickAssigned = false;
    if ($("#btnMPPrint").data("events") != null) {
        $.each($("#btnMPPrint").data("events"), function (i, e) {
            if (i == 'click') { clickAssigned = true}
        });
    }

    if (clickAssigned == false) {
        $("#btnMPPrint").click(function (event) {
            $('#pnlCenterMiddle').jqprint(); // This is the div that has to be printed.
            event.preventDefault();
            return false;
        });
    }
}

function InitializeTreeView() {
    var uppUpdatePanelName = $$("uppCenterMiddle").attr("id");

    $("#trMPIPHPigation").find("A").click(function (event) {
        if ($(this).attr("swfNavigateURL") == undefined) {
            if (window.location.href.indexOf("wfmDefault.aspx") > -1) {
                if (this.href.indexOf("wfmDefault.aspx") > -1) {
                    $(this).attr("swfNavigateURL", this.href);
                }
            }
        }
        var navigationURl = $(this).attr("swfNavigateURL");
        if (navigationURl != undefined) {
            if (window.location.href.indexOf("wfmDefault.aspx") > -1) {
                __doPostBack(uppUpdatePanelName, navigationURl);
                event.preventDefault();
            }
        }
    });
}

//jquery layout script

$(document).ready(function () {
    $("#pnlPopup").dialog({ dialogClass: 'modalBackground', title: '      Please wait..', position: 'center', autoOpen: false, width: 130, height: 70, minHeight: 50, modal: true, resizable: false, closeText: '' });
    $("#pnlPopup").siblings(".ui-dialog-titlebar").hide();
    $("#pnlPopup").toggle(); /* added to show the panel */

    /* other code */
    InitializeMenu();

//    try {
//        InitializePrintButton();
//    }
//    catch (e) {
//        alert("Error in NSHeader.js InitializePrintButton function, error : " + e.Message);
//    }

    if (window.location.toString().indexOf("ReturnValue=") > 0) {
        SetChildPageLayout();
    }
    else {
        var lpnlCenterRight = $("#pnlCenterRight");
        var lintEastWidth = 0;
        if (lpnlCenterRight.length == 1) {
            if (lpnlCenterRight[0].children.length > 0) {
                lintEastWidth = 150;
            }
        }
        SetPageLayout(150, 86, 6, lintEastWidth);
    }

    //FM upgrade: 5.4.22.0, 6.0.0.23 changes
    CheckForSupportedBrowser();
    //FM upgrade: 6.0.0.28 changes
    SetPageStateNUserPreferences();
})

function LoadScripts() {
}

//NEO_CERTIFY_CHANGES - BEGIN
function GetClientID(ctrlid) {
    var ctrl = $$(ctrlid);
    if (ctrl.length == 1) {
        return ctrl[0].id;
    }
    else if (ctrl.length > 1) {
        for (i = 0; i < ctrl.length; i++) {
            if ((ctrl[i].id.length > 3) && ctrl[i].id.substring(0, 3) == "img") {
            }
            else {
                return ctrl[i].id;
            }
        }
    }
    return null;
}

function click_temp(astrExpression) {
    $$("hfldRowIndex").value = "";
    $$("btnPositionGrid")[0].value = astrExpression;
    //FM upgrade: 6.0.1.1 changes - Upgrade Framework with latest jQuery version 3.2.1
    //$$("btnPositionGrid").click();
    $$("btnPositionGrid").trigger('click');
}

function TriggerListItem(ctrlid) {
    var ctrl = $$(ctrlid);
    if (ctrl.length == 1) {
        $(ctrl).trigger("change");
    }
}


function OnContactTypeChange() {
    var lddlConatct = $$("ddlContactTypeValue");
    if (lddlConatct[0].length > 0 && lddlConatct[0].length > 0) {
        var contact_type = lddlConatct[0].value;
        var pnlAddr = $$("pnlAddress");
        var pnlSameAsPart = $$("pnlAddSamsAsPerson");
        var chkCorFlag = $$("chkCorrespondenceAddrFlag");
        var chkCorLabel = $$("capCorrespondenceAddrFlag");
        if (contact_type != null) {
            if (contact_type[0].length > 0) {
                var check = $$("chkAddrSameAsPerson");
                if (check[0].checked) {
                    

                    pnlSameAsPart.show();
                    pnlAddr.hide();
                    chkCorLabel.hide();
                    chkCorFlag.hide();

                }
                else {
                    var lControlList = GetControlsToChangeState(lddlConatct[0].getAttribute('sfwClientVisibility'), lddlConatct[0].value);
                    if (lControlList != null) {
                        if (lControlList.length == 2) {
                            ChangeVisibility(lControlList[0], true);
                            ChangeVisibility(lControlList[1], false);
                            pnlSameAsPart.hide();
                            pnlAddr.show();
                        }
                    }
                }
            }
        }
    }
}


function SelectAllGridWrapRows() {
    $("tbody > tr.gridheadWrap").each(function () {
        $("th:first > input:checkbox", this).click(function (event) {
            var lblnChecked = this.checked;
            var lobjRows = $("tr", this.parentNode.parentNode.parentNode);
            $("td:first input:checkbox", lobjRows).each(function () {
                this.checked = lblnChecked;
            });
        });
    });
}

function OverRideExtend_SSN() {
    $("input[type='text'].classFormatSSN").mask('999-99-9999')
.blur(function (e) {
});
}

function OverRideExtend_Phone() {
    $("input[type='text'].classFormatPhone").mask('(999) 999-9999')
.blur(function (e) {
});
}

// Chart related code
var tooltip_shown = false;
var timer_id = 1;

function DisplayTooltip(tooltip_text) {
    var ChartToolTip = $('#chrTooltipChart')[0];
    ChartToolTip.innerHTML = tooltip_text;
    tooltip_shown = (tooltip_text != "") ? true : false;
    if (tooltip_text != "") {
        // Get tooltip window height
        ToolTipFading(0);
    }
    else {
        clearTimeout(timer_id);
        ChartToolTip.style.visibility = "hidden";
    }
}

$(document).mousemove(function (e) {
    if (tooltip_shown) {
        // Depending on IE/Firefox, find out what object to use to find mouse position
        var ev;
        if (e)
            ev = e;
        else
            ev = event;

        var ChartToolTip = $('#chrTooltipChart')[0];
        ChartToolTip.style.visibility = "visible";

        var wnd_height = this.body.clientHeight;
        var wnd_width = this.body.clientWidth;

        var tooltip_width = (ChartToolTip.style.pixelWidth) ? ChartToolTip.style.pixelWidth : ChartToolTip.offsetWidth;
        var tooltip_height = (ChartToolTip.style.pixelHeight) ? ChartToolTip.style.pixelHeight : ChartToolTip.offsetHeight;

        offset_y = (ev.clientY + tooltip_height - document.body.scrollTop + 30 >= wnd_height) ? -15 - tooltip_height : 20;
        ChartToolTip.style.left = Math.min(wnd_width - tooltip_width - 10, Math.max(3, ev.clientX + 6)) + document.body.scrollLeft + 'px';
        ChartToolTip.style.top = ev.clientY + offset_y + document.body.scrollTop + 'px';
    }
});

function ToolTipFading(transparency) {
    if (transparency <= 100) {
        var ChartToolTip = $('#chrTooltipChart')[0];
        ChartToolTip.style.filter = "alpha(opacity=" + transparency + ")";
        ChartToolTip.style.opacity = transparency / 100;
        transparency += 5;
        timer_id = setTimeout('ToolTipFading(' + transparency + ')', 35);
    }
}

function InitializeMSSMenu() {
    var uppUpdatePanelName = $("#uppCenterMiddle").attr("id");
    $("#pnlCenterLeft").find("A[href!='#']").each(function (event) {
        if (this.href.indexOf("wfmDefault.aspx") > -1) {
            if (this.href.indexOf("__doPostBack") == -1) {
                $(this).attr("sfwNavigateURL", this.href);
                if (this.href.indexOf("SkipPostBack=True") == -1) {
                    this.href = "javascript:__doPostBack('" + uppUpdatePanelName + "','" + this.href + "')";
                }
            }
        }
    });

    $("#pnlCenterLeft").find("A[href!='#']").click(function (event) {
        if (window.location.href.indexOf("wfmDefault.aspx") == -1) {
            if (this.href.indexOf("wfmDefault.aspx") > -1) {
                this.href = $(this).attr("sfwNavigateURL");
            }
        }
        $(this).parents("[className='level2 submenu dynamic']").hide();
    });
}

//NEO_CERTIFY_CHANGES - END

//FM upgrade: 6.0.0.24 changes
//function RenderNeoGrid() {
//    NeoWidgetsOnForm = {};
//    var lstrFormName = $$("hflFrameworkCurrentForm").val().trim();
//    var lJsonGridResult = null;
//    if ($$("hfldGridResults").length > 0 && $$("hfldGridResults").val() != "") {
//        lJsonGridResult = JSON.parse($$("hfldGridResults").val());
//        if (lJsonGridResult.OtherData.SenderKey != undefined) {
//            $$("uppCenterMiddle").attr("SenderKey", lJsonGridResult.OtherData.SenderKey)
//        }
//    }

//    if (lstrFormName.indexOf("Lookup") > 0 && lJsonGridResult == null) {
//        var data = { DomainModel: { KeysData: {}, DetailsData: {} }, ExtraInfoFields: {} }
//        var grid = $$("uppCenterMiddle").find("div[sfwgridview]").attr("sfwgridview");
//        data.DomainModel.DetailsData[grid] = { FieldsType: {}, Records: [] }
//        data.DomainModel.KeysData[["CollectionOf_", grid].join('')] = "";
//        lJsonGridResult = data.DomainModel;
//    }

//    if (lJsonGridResult != null) {
//        var ObjectData = { data: { DomainModel: lJsonGridResult } };
//        for (gridid in lJsonGridResult.DetailsData) {

//            var GridDiv = $("[sfwGridView='" + gridid + "']");
//            var sfwMVVMTemplate = GridDiv.attr("sfwMVVMTemplate");
//            if (sfwMVVMTemplate != undefined) {
//                GridDiv.removeAttr("sfwMVVMTemplate");
//                var ldomDivToAppend = $(sfwMVVMTemplate);
//                var OldGridParent = GridDiv.parent();
//                $$(gridid).remove();
//                OldGridParent.append(ldomDivToAppend);
//                $$("hfldGridResults").val("");
//                var GridToApply = $$(ldomDivToAppend[0].id);
//                if ($$("GridTable_" + ldomDivToAppend[0].id).length > 0) {
//                    GridToApply = $$("GridTable_" + ldomDivToAppend[0].id);
//                }
//                NeoWidgetsOnForm[gridid] = new MVVM.JQueryControls.GridView(GridToApply, "cphCenterMiddle_tblCenterMiddle", ObjectData);
//                NeoWidgetsOnForm[gridid].init();
//            }
//        }
//    }

//}

//$(document).on("click", "[sfwrelatedcontrol]", function () {
//    var sfwrelatedcontrol = $(this).attr("sfwrelatedcontrol");
//    if ($$(sfwrelatedcontrol)[0].tagName == "INPUT") {
//        $$(sfwrelatedcontrol).trigger("click");
//    }
//    if ($$("GridTable_" + sfwrelatedcontrol).data("neoGrid") != undefined) {
//        UpdateSelectedGridIndex(sfwrelatedcontrol);
//    }
//});

//function UpdateSelectedGridIndex(sfwrelatedcontrol) {
//    var neoGrid = $$("GridTable_" + sfwrelatedcontrol).data("neoGrid");
//    var selectedIndexes = _.map(_.filter(neoGrid.dataSource.data, function (d) { return d.rowSelect }), function (d) { return d.rowIndex });

//    var selectedIndexesObj = $$("hfldGridSelectedIndexes").val();

//    if (selectedIndexesObj == "") {
//        selectedIndexesObj = {};
//    } else {
//        selectedIndexesObj = JSON.parse(selectedIndexesObj);
//    }
//    var SenderKey = $$("uppCenterMiddle").attr("SenderKey")
//    if (SenderKey != null) {
//        selectedIndexesObj["SenderKey"] = SenderKey;
//    }
//    selectedIndexesObj[sfwrelatedcontrol] = selectedIndexes;
//    $$("hfldGridSelectedIndexes").val(JSON.stringify(selectedIndexesObj));
//}

//function clickListner(linkbutton) {

//    //$(linkbutton).closest("tr").find("td[data-container-for='rowSelect']").find("input").trigger("click");
//    var rowIndex = $(linkbutton).attr("rowIndex");
//    var sfwMethodName = $(linkbutton).attr("sfwMethodName");
//    var lgrvneogrid = $(linkbutton).closest("[data-role='neogrid']");
//    lgrvneogrid.data("neoGrid").dataSource.data[rowIndex].rowSelect = true

//    if (sfwMethodName != null) {
//        var sfwrelatedcontrol = lgrvneogrid[0].id.replace("GridTable_", "");
//        var sfwDataField = $(linkbutton).attr("data-field");
//        UpdateSelectedGridIndex(sfwrelatedcontrol);
//        var EventArgs = {
//            sfwrelatedcontrol: sfwrelatedcontrol,
//            sfwDataField: sfwDataField
//        }
//        __doPostBack('uppCenterMiddle', 'NeoGridRowCommand_' + JSON.stringify(EventArgs));
//    }
//}