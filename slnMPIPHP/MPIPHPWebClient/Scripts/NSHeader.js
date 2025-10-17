var lblnCallScript_Init = true;
var chkcount;
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

    //$("input[type='image'][imagebutton='true'][src*='sfwApplicationName']").each(function (item) {
    //    var lImageSource = $(this).attr("src");
    //    $(this).attr("src", lImageSource.replace("sfwApplicationName/", ""));
    //});

    if ($$("lblFormName") != undefined && $$("lblFormName") != "" && $$("lblFormName").text() == "wfmPayeeAccountStatusMaintenance") {
        if ($("#cphCenterMiddle_lblStatusEffectiveDate").text() != '') {
            var StatusEffectiveDate = $("#cphCenterMiddle_lblStatusEffectiveDate").text();
            nsCommon.sessionSet("StatusEffectiveDate", StatusEffectiveDate);
        }
        else {
            var StatusEffectiveDate = nsCommon.sessionGet("StatusEffectiveDate");
            $("#cphCenterMiddle_lblStatusEffectiveDate").text(StatusEffectiveDate);
        }
    }
    else {
        nsCommon.sessionRemove("StatusEffectiveDate");
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
    
    if ($$("lblFormName") != undefined && $$("lblFormName") != "" && $$("lblFormName").text() == "wfmPayeeAccountTaxwithholdingMaintenance") {
        if ($('#cphCenterMiddle_ddlTaxIdentifierValue > option').length <= 2)
        {
            $("#cphCenterMiddle_ddlTaxIdentifierValue").prepend("<option value=''></option>");
        }

        var benefitDistributionType = $('#cphCenterMiddle_ddlBenefitDistributionTypeValue1 option:selected').val();
        if (benefitDistributionType != undefined) // && benefitDistributionType != '')
        {
            $$('ddlBenefitDistributionTypeValue1').trigger('change');
        }
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
    //    alert("Error in NSHeader.js InitializeWatermark function, error : " + e.message);
    //}
    try {
        Extend_Date();
    }
    catch (e) {
        alert("Error in NSHeader.js Extend_Date function, error : " + e.message);
    }

    //try {
    //    Extend_DateTime();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_DateTime function, error : " + e.message);
    //}
    //try {
    //    Extend_Time();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Time function, error : " + e.message);
    //}
    //try {
    //    Extend_MonthYear();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_MonthYear function, error : " + e.message);
    //}
    try {
        OverRideExtend_SSN();
        $("input[type='text'].classFormatSSN").mask('999-99-9999')
.blur(function (e) {
});
    }
    catch (e) {
        alert("Error in NSHeader.js Extend_SSN function, error : " + e.message);
    }
    //try {
    //    Extend_Phone();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Phone function, error : " + e.message);
    //}
    //try {
    //    Extend_Currency();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Currency function, error : " + e.message);
    //}
    //try {
    //    Extend_Custom();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js Extend_Custom function, error : " + e.message);
    //}

    //try {
    //    InitializeComboBox();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeComboBox function, error : " + e.message);
    //}
    //try {
    //    InitializeCascadingDropDown();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeCascadingDropDown function, error : " + e.message);
    //}

    //try {
    //    SetClientVisibility();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js SetClientVisibility function, error : " + e.message);
    //}

    //try {
    //    InitializeListControls();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeListControls function, error : " + e.message);
    //}
    //try {
    //    InitializeRetrievalTextBox();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeRetrievalTextBox function, error : " + e.message);
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
    catch (e) {
    }
    //EmergencyOneTimePayment - 03/17/2020
    try {
        if ($$("lblFormName") != undefined && $$("lblFormName") != "" && $$("lblFormName").text() == "wfmWithdrawalApplicationMaintenance") {
            var lblCovidRequestedAmount = $$("capCOVIDWithdrawalAmount");
            var lblCovidFedTaxPerc = $$("capCOVIDFedTaxPerc");
            var lblCovidStateTaxPerc = $$("capCOVIDStateTaxPerc");
            var txtCovidRequestedAmount = $$("txtIdecCOVIDWithdrawalAmount");
            var txtCovidFedTaxPerc = $$("txtCOVIDFederalPerc");
            var txtCovidStateTaxPerc = $$("txtCOVIDStatePerc");
            var check = $$("chkEmergencyOneTimePayment");

            if (check[0].checked == false) {
                lblCovidRequestedAmount.hide();
                lblCovidFedTaxPerc.hide();
                lblCovidStateTaxPerc.hide();
                txtCovidRequestedAmount.hide();
                txtCovidFedTaxPerc.hide();
                txtCovidStateTaxPerc.hide();
            }

            var lddlWithdrawalType = $$("ddlWithdrawalType");
            var lWithdrawalType = lddlWithdrawalType[0].value;
            if (lWithdrawalType != "") {
                lblCovidRequestedAmount.show();
                lblCovidFedTaxPerc.show();
                lblCovidStateTaxPerc.show();
                txtCovidRequestedAmount.show();
                txtCovidFedTaxPerc.show();
                txtCovidStateTaxPerc.show();
            }

        }
    }
    catch (e) {

    }

    //try {
    //    OpenSmartLink();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js OpenSmartLink function, error : " + e.message);
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
    //    alert("Error in NSHeader.js HideEmptyButtonCells function, error : " + e.message);
    //}
    //try {
    //    SelectAllRows();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js SelectAllRows function, error : " + e.message);
    //}
    try {
        SelectAllGridWrapRows();
    }
    catch (e) {
        alert("Error in NSHeader.js SelectAllRows function, error : " + e.message);
    }
    //try {
    //    InitializeCollapsiblePanels();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeCollapsiblePanels function, error : " + e.message);
    //}
    //try {
    //    InitializeTabContainer();
    //}
    //catch (e) {
    //    alert("Error in NSHeader.js InitializeTabContainer function, error : " + e.message);
    //}

    try {
        o_InitializePrintButton();
    }
    catch (e) {
        alert("Error in NSHeader.js InitializePrintButton function, error : " + e.message);
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
    try {
        CheckChange('chkMinDistributionFlag', 'lblMinDistributionDate', 'txtRetirementDate', 'lblRetirementDate', 'lblintIsManager', 'FromScript')
    }
    catch (e) {
        // alert("Error in NSHeader.js InitializePopupDialogs function, error : " + e.message);
    }
    try {
        //PIR Scout Effort Hours
        $$('txtEffortHours').keypress(function (event) {
            return IsNumber(event, this)
        });
    }
    catch (e) {
    }
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
            if (i == 'click') { clickAssigned = true }
        });
    }

    if (clickAssigned == false) {
        $("#btnMPPrint").off("click").on("click",function(event) {
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

    chkcount = 0;

    //    try {
    //        InitializePrintButton();
    //    }
    //    catch (e) {
    //        alert("Error in NSHeader.js InitializePrintButton function, error : " + e.message);
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
    //CheckForSupportedBrowser();
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

// FW Upgrade #PIR 28466 - Resize panel is moving in popup screen (Through Out Application)
// FW Upgrade #PIR 28470 - LOB >> In Every Popup there is alignment Issue (UI Issue)
function SetChildPageLayoutStyle(ablnChild) {
    var lstrWindowFieldName = "hflWindowName";
    if (ablnChild === true || ($$(lstrWindowFieldName).length > 0 && $$(lstrWindowFieldName).val().indexOf("ChildWindow") === 0)) {
        if ((window.name == undefined || window.name == "" || window.name == "Child") && $$(lstrWindowFieldName).length > 0 && $$(lstrWindowFieldName).val().indexOf("ChildWindow") === 0) {
            window.name = $$("hflWindowName").val().replace("ChildWindow", "");
        }
        var fnSetLeftTop = function () {
            $("#divLayoutCenter").css("width", "");
            $(".ui-layout-center").css({ "left": "0px", "top": "0px", "width": "100%", "height": "100%" });
            $(".ui-layout-north, .ui-layout-west, .ui-layout-resizer-west, .ui-layout-resizer-west, .ui-layout-resizer-west-open, .ui-draggable-handle, .ui-layout-resizer-open, .ui-layout-south").hide();
            $('.modalBackground').css("left", "35%");
        };
        var lstrForm = nsCommon.GetCurrentForm();
        if (lstrForm != undefined && lstrForm.indexOf("Lookup") > 0) {
            var larrKeys = $$("hflWindowName").closest("form").find("input[type='button'][sfwmethodname],input[type='submit'][sfwmethodname],button[sfwmethodname],input[type='image'][sfwmethodname]");
            for (var i = 0, len = larrKeys.length; i < len; i++) {
                var key = larrKeys[i].getAttribute("sfwmethodname");
                if (key != undefined && (nsConstants.METHODS_TO_REMOVE_FROM_RETRIEVAL.indexOf(key) >= 0)) {
                    $(larrKeys[i]).hide();
                }
            }
        }
        //Fw upgrade: PIR ID : 28582: LOB > through out application > observe UI issue in popup while retrieving the popup screen
        //setTimeout(fnSetLeftTop, 0);
        fnSetLeftTop();
    }
}

//Fw upgrade: PIR ID : 28582: LOB > through out application > observe UI issue in popup while retrieving the popup screen
function SetPageLayout(aintWestSize, aintNorthSize, aintWesSpacingOpen, aintEastSize) {
    //alert("SetPageLayout");
    var spnWestSize = readCookie("NeoSpin__WestSize");
    if (spnWestSize == null)
        spnWestSize = aintWestSize;
    myLayout = $('body').layout({
        west__size: spnWestSize
        , north__size: aintNorthSize
        , east__size: aintEastSize
        , north__spacing_open: 0
        , west__spacing_open: aintWesSpacingOpen
        , enableCursorHotkey: false
        , south__spacing_open: 0
        // RESIZE Accordion widget when panes resize
        , west__onresize: function (x, ui) {
            var date = new Date();
            date.setTime(date.getTime() + (365 * 24 * 60 * 60 * 1000));
            document.cookie = "NeoSpin__WestSize=" + myLayout.state.west.size + "; expires=" + date.toGMTString() + "; path=/";
            var $P = ui.jquery ? ui : $(ui.panel);
            // find all VISIBLE accordions inside this pane and resize them
            $P.find(".ui-accordion:visible").each(function () {
                var $E = $(this);
                if ($E.data("uiAccordion"))
                    $E.accordion("refresh");
            });
            if ($("#hflWindowName").val().indexOf("ChildWindow") >= 0) {
                SetChildPageLayoutStyle(true);
            }
        }
        , west__fxName: "none"
    });
    myLayout.allowOverflow('north');
    var luppAccordian = $("#uppAccordian");
    if (luppAccordian.length > 0) {
        luppAccordian.accordion({
            fillSpace: true
        });
    }
    if ($("#hflWindowName").val().indexOf("ChildWindow") >= 0) {
        SetChildPageLayout();
    }
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

function Extend_Date() {

    $("input[type='text'][sfwExtendDate='true']").mask('99/99/9999')
        .datepick({ dateFormat: 'mm/dd/yy', yearRange: '1901:2100', showOn: 'button', buttonImageOnly: true, buttonImage: 'Image/Calendar_scheduleHS.png' })
        .blur(function (e) {
            var value = e.currentTarget.value;
            //F/W Upgrade: PIR: 28555: Invalid date popup is displaying on each click.            
            if (value.length > 0 && value !== "__/__/____") {
                if (value.indexOf("_") >= 8) {
                    varlue = value.replace("_", "").replace("_", "");
                }
                if (!IsValidDate(value, 'mm/dd/yy')) {
                    var ldomControl = $(this);
                    nsCommon.Alert("Invalid date", ldomControl);
                    return false;
                }
                var year = value.split("/")[2];
                if (year.length == 2) {
                    e.currentTarget.value = value.substr(0, 6) + "20" + year;
                }
            }
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

function CheckChange(obj, lbl, text, lblRetirementDate, txtIsMngr, flag) {
    if ($$("lblFormName") != undefined && $$("lblFormName") != "" && $$("lblFormName").text() == "wfmRetirementApplicationMaintenance") {
        if (chkcount != undefined && chkcount == 0 && flag == 'FromScript') {
            chkcount = chkcount + 1;
            if ($$(obj)[0].checked) {
                if ($$(txtIsMngr).text() == 1) {
                    $$(text).prop("disabled", false);
                }
                else {
                    $$(text).prop("disabled", true);
                }
            }
            else {
                $$(text).prop("disabled", false);
            }
        }
        else {
            if ($$(obj)[0].checked) {
                if (chkcount != undefined && chkcount != 0 && flag == 'FromChkChanged') {
                    if ($$(text)[0].value != $$(lbl).text()) {
                        $$(text)[0].value = $$(lbl).text();
                    }
                    if ($$(txtIsMngr).text() == 1) {
                        $$(text).prop("disabled", false);
                    }
                    else {
                        $$(text).prop("disabled", true);
                    }
                }
                else {
                    if ($$(txtIsMngr).text() == 1) {
                        $$(text).prop("disabled", false);
                    }
                    else {
                        $$(text).prop("disabled", true);
                    }
                }
            }
            else {
                $$(text).prop("disabled", false);
            }
        }
    }
}

//LOB-Neogrid- Delete Buttons should be displayed as image when Neogrid settings are enabled on User Preferences Maintenance screen
if (nsConstants.ARR_TOOLBAR_BUTTONS != undefined && nsConstants.ARR_TOOLBAR_BUTTONS != null && nsConstants.ARR_TOOLBAR_BUTTONS.length > 0) {
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnExecuteBusinessMethodSelectRows_Click");
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnNewPopupDialog_Click");
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnGridViewAdd_Click");
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnOpenDoc_Click");
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnGridViewUpdate_Click");
    nsConstants.ARR_TOOLBAR_BUTTONS.push("btnCancel_Click");
}

//PIR Scout Effort Hours - THE SCRIPT THAT CHECKS IF THE KEY PRESSED IS A NUMERIC OR DECIMAL VALUE.
function IsNumber(evt, element) {
    if ($$("lblFormName") != undefined && $$("lblFormName") != "" && $$("lblFormName").text() == "wfmPirMaintenance") {
        var charCode = (evt.which) ? evt.which : event.keyCode
        //(charCode != 45 || $(element).val().indexOf('-') != -1) &&      // "-" CHECK MINUS, AND ONLY ONE.
        if ((charCode != 46 || $(element).val().indexOf('.') != -1) &&      // "." CHECK DOT, AND ONLY ONE.
            (charCode < 48 || charCode > 57)) {
            $(element).val('');
            return false;
        }
        return true;
    }
}

//NEO_CERTIFY_CHANGES - END

// Old window.showModalDialog coding

//Commented code to use Jquery Pop Up Window
//$(function () {
//    setTimeout(function () {
//        window.showModalDialog = RootshowModalDialog;
//    }, 200);
//})



function LaunchLookup(WindowName, ControlID, ReturnIndex, NavigationParameters) {
    //show modal dialog box and collect its return value
    if (ReturnIndex == "" || ReturnIndex == null) {
        ReturnIndex = "1";
    }
    if (NavigationParameters != "" && NavigationParameters !== undefined) {
        var lstrRequestParams = "";
        var larrParam = NavigationParameters.split(";");
        for (var i in larrParam) {
            var lstrParam = larrParam[i];
            var lintPos = lstrParam.indexOf("=");
            if (lintPos < 0) {
            }
            else {
                var lstrToField = lstrParam.substring(0, lintPos);
                var lstrFromField = lstrParam.substring(lintPos + 1);
                if (lstrFromField.startsWith("#")) {
                    lstrRequestParams = lstrRequestParams + "&" + lstrToField + "=" + lstrFromField.substring(1);
                }
                else if (lstrToField == "MessageID") {
                    lstrRequestParams = lstrRequestParams + "&" + lstrToField + "=" + lstrFromField;
                }
                else {
                    lstrRequestParams = lstrRequestParams + "&" + lstrToField + "=" + document.getElementById(lstrFromField).value;
                }
            }
        }
    }
    var LookupForm = "wfmDefault.aspx?FormID=" + WindowName + "&ReturnValue=Yes&ReturnIndex=" + ReturnIndex + "&FWN=" + window.name;
    if (lstrRequestParams != "" && lstrRequestParams !== undefined) {
        LookupForm = LookupForm + lstrRequestParams;
    }

    SetProcessingLookupFlag('true');
    if (iblnTestingMode) {
        window.open(LookupForm, window, "dialogWidth=1000px;dialogHeight=800px;center=yes; help: no; resizable: yes; status: no; scrollbar=yes;");
    }
    else {
        var retval = window.showModalDialog(LookupForm, window,
          "dialogWidth=1000px;dialogHeight=800px;center=yes; help: no; resizable: yes; status: no; scrollbar=yes;",ControlID);

        // Check if user closed the dialog without selecting any value
        if (retval != "" && retval != null) {
            // Fill the TextBox with selected value and fire change event
            var ctrl = $$(ControlID);
            ctrl.val(retval);
            //FM upgrade: 6.0.1.1 changes - Upgrade Framework with latest jQuery version 3.2.1
            //ctrl.change();
            ctrl.trigger('change');

            // Execute Server Side Refresh
            //FM upgrade: 6.0.1.1 changes - Upgrade Framework with latest jQuery version 3.2.1
            var btnServerSideRefresh = $$("btnServerSideRefresh");
            if (btnServerSideRefresh.length == 1) {
                //btnServerSideRefresh.click();
                btnServerSideRefresh.trigger('click');
            }

            // Reset the Processing Lookup flag
            var retctrl = $(ctrl).attr('sfwRetrievalControls');
            if (retctrl == null)
                SetProcessingLookupFlag('false');
        }
        else {
            SetProcessingLookupFlag('false');
        }
    }

    return false;
}
// End Old window.showModalDialog coding

// edittor tool https changes
//function EditCorrOnLocalTool() {
//    var aParams = {
//        aParams: {
//            DefaultPrinter: "",
//            ShowPrintDialog: true
//        }
//    }
//    var methodparams = JSON.stringify(aParams);
//    var resultString = "";
//    $.ajax({
//        type: "POST",
//        url: istrWebServiceName + "/EditCorrOnLocalTool2",
//        beforeSend: function (request) {
//            request.setRequestHeader("FWN", window.name);
//        },
//        data: methodparams,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        async: false,
//        success: function (result) {
//            resultString = (result.d);
//            if ($("#lblMessage").length > 0) {
//                $("#lblMessage").html(resultString);
//            } else {
//                alert(resultString);
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            if ($("#lblMessage").length > 0) {
//                $("#lblMessage").html("<div style='color:red'>" + XMLHttpRequest.responseText.replace("{\"d\":\"", "").replace("\"}", "") + "</div>");
//            } else {
//                alert(XMLHttpRequest.responseText.replace("{\"d\":\"", "").replace("\"}", ""));
//            }
//            return false;
//        }
//    });
//}

//Fix for Cor Edit tool. To open the correspondence in local tool.
function fwkOpenWindow(astrWindowType, astrUrl, astrFeatures) {
    if (astrUrl.indexOf("wfmCorrespondenceClientEdit.aspx") == 0) {
        EditCorrOnLocalTool();
        return;
    }

    if (lblnCallScript_Init)
        Script_Init();
    astrUrl = buildFWKUrl(astrUrl);
    if (iblnTestingMode)
        window.open(astrUrl, window, astrFeatures);
    else
        window.showModalDialog(astrUrl, window, astrFeatures);
}

// edittor tool https changes end

////////////////////////////////////////////new tool changes//////////////////////////////////////////////////
nsCorr = {
    FileData: undefined
}

function SignalRCallToEditCorr(data) {
    if ($.CorrHubConnection == undefined) {
        InitializeSignalrForCorrTool();
    }
    nsCorr.FileData = "";
    if ($.CorrHubConnection) {
        RegisterClientFunctions();
        $.CorrHubConnection.start().done(function () {
            $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
            });
            var CorrData = data;
            var Base64Chunks = CorrData.Base64String;
            delete CorrData.Base64String;
            $.CorrHubConnection.proxies.corrsignalrhub.server.createCorrInstance(JSON.stringify(CorrData)).done(function () {
                SendChunkByIndex(0, Base64Chunks);
            });
        }).fail(function () {
            alert(DefaultMessages.CorrEditorServiceNotRunning);
            console.log('Could not Connect!');
        });
    }
}

function SendChunkByIndex(i, Base64Chunks) {
    if (i < Base64Chunks.length) {
        var EOF = false;
        if (i == Base64Chunks.length - 1) {
            EOF = true;
        }
        $.CorrHubConnection.proxies.corrsignalrhub.server.createCorrInstance(Base64Chunks[i], EOF, window.name).done(function () {
            sessionStorage.setItem("ConnectedToCorrTool", true);
            SendChunkByIndex(i + 1, Base64Chunks);
        });
    }
}

function RegisterClientFunctions() {
    $.CorrHubConnection.proxies.corrsignalrhub.client.invokeResponseMessage = function (OtherData, astrFileData, ablnEOF) {
        if (astrFileData != undefined) {
            nsCorr.FileData += astrFileData;
        }
        if (ablnEOF) {
            if (nsCorr.FileData != "" && nsCorr.FileData != undefined)
                OtherData.CorrFileData = nsCorr.FileData;
            var ApiAction = OtherData.ApiAction;
            if (ApiAction != undefined && ApiAction != null) {
                ApiAction = [ApiAction, "_Override"].join('')
            }
            nsCorr.FileData = "";
            console.dir(OtherData);

            function UpdateCorrespondenceStatusCallback(jqXHR, textStatus, errorThrown, nsDeferred) {
                if (textStatus == "success") {
                    $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR == null ? textStatus : jqXHR.responseText, false, false).done(function () {
                    });
                }
                else {
                    if (jqXHR.status == 400) {
                        $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR.responseText, false, true).done(function () {
                        });
                    }
                    else {
                        $.CorrHubConnection.proxies.corrsignalrhub.server.statusUpdateSuccess(jqXHR.responseText, true, false).done(function () {
                        });
                    }
                }
            }

            var testr = OtherAjaxRequest("CorrEditor/" + ApiAction, { param: OtherData }, null, false, "POST", UpdateCorrespondenceStatusCallback);

            if (testr != undefined) {
                UpdateCorrespondenceStatusCallback(null, "success", "null", null);
                console.dir(testr);
            }
        }
    };
    $.CorrHubConnection.proxies.corrsignalrhub.client.invokeResponseMessageNew = function (newMessage) {
        alert(newMessage);
    };
}
function GetPrefixforAjaxCall() {
    var lstrSiteName = location.pathname.split("/")[1];
    var Prefix = [(lstrSiteName == "") ? "" : "/", lstrSiteName, "/"].join('');
    if (Prefix == "///") {
        return "/";
    }
    else {
        return Prefix;
    }
}

function OtherAjaxRequest(actionName, reqObject, nsDeferred, ablnASync, astrType, aErrorCallback) {

    if (reqObject === void 0) { reqObject = {}; }
    if (ablnASync === void 0) { ablnASync = true; }
    if (astrType === void 0) { astrType = "POST"; }
    if (aErrorCallback === void 0) { aErrorCallback = null; }
    var Prefix = GetPrefixforAjaxCall();
    var ContentType = "application/json; charset=utf-8";
    if (reqObject.IsFormData) {
        ContentType = false;
    }
    var ajaxData;
    var dataToSend = reqObject.IsFormData ? reqObject.param || {} : JSON.stringify(reqObject.param || {});
    $.ajax({
        url: [Prefix, "api/", actionName].join(''),
        async: ablnASync,
        data: dataToSend,
        dataType: "json",
        type: reqObject.Type || astrType || "POST",
        beforeSend: function (request) {
            request.setRequestHeader("FWN", window.name);
        },
        cache: false,
        processData: false,
        contentType: ContentType,
        success: function (data) {
            if (actionName != "" && data != undefined && typeof data === 'object' && !Array.isArray(data)) {
                data["LastExecutedAction"] = actionName;
            }
            if (nsDeferred != undefined) {
                nsDeferred.resolve(data);
            }
            if (ablnASync === false) {
                ajaxData = data;
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (aErrorCallback != null) {
                aErrorCallback(jqXHR, textStatus, errorThrown, nsDeferred);
            }
            else {
                nsRequest.HandleAjaxError(jqXHR, textStatus, errorThrown, nsDeferred);
            }
        }
    });
    if (ablnASync === false) {
        return ajaxData;
    }
}


function InitializeSignalrForCorrTool() {
    (function ($, window, undefined) {
        "use strict";
        if (typeof ($.signalR) !== "function") {
            console.log("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
            return;
        }
        function makeProxyCallback(hub, callback) {
            return function () {
                callback.apply(hub, $.makeArray(arguments));
            };
        }
        function registerHubProxies(instance, shouldSubscribe) {
            var key, hub, memberKey, memberValue, subscriptionMethod;
            for (key in instance) {
                if (instance.hasOwnProperty(key)) {
                    hub = instance[key];
                    if (!(hub.hubName)) {
                        continue;
                    }
                    if (shouldSubscribe) {
                        subscriptionMethod = hub.on;
                    }
                    else {
                        subscriptionMethod = hub.off;
                    }
                    for (memberKey in hub.client) {
                        if (hub.client.hasOwnProperty(memberKey)) {
                            memberValue = hub.client[memberKey];
                            if (!$.isFunction(memberValue)) {
                                continue;
                            }
                            subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                        }
                    }
                }
            }
        }
        $.hubConnection.prototype.createHubProxies = function () {
            var proxies = $.connection.hub.proxies || {};
            this.starting(function () {
                registerHubProxies(proxies, true);
                this._registerSubscribedHubs();
            }).disconnected(function () {
                registerHubProxies(proxies, false);
            });
            proxies['CorrSignalRHub'] = this.createHubProxy('CorrSignalRHub');
            proxies['CorrSignalRHub'].client = {};
            proxies['CorrSignalRHub'].server = {
                createCorrInstance: function (message) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["CreateCorrInstance"], $.makeArray(arguments)));
                },
                statusUpdateSuccess: function (astrMessage, ablnError) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["StatusUpdateSuccess"], $.makeArray(arguments)));
                },
                setWindowName: function (astrWindowName) {
                    return proxies['CorrSignalRHub'].invoke.apply(proxies['CorrSignalRHub'], $.merge(["SetWindowName"], $.makeArray(arguments)));
                }
            };
            return proxies;
        };
        if (document.location.protocol.indexOf("https") == 0) {
            $.CorrHubConnection = $.hubConnection("https://localhost:8082/signalr", { useDefaultPath: false });
        }
        else {
            $.CorrHubConnection = $.hubConnection("http://localhost:8081/signalr", { useDefaultPath: false });
        }
        $.CorrHub = $.CorrHubConnection.createHubProxies("CorrSignalRHub");
    }(window.jQuery, window));
    if (sessionStorage.getItem("ConnectedToCorrTool") == true) {
        if ($.CorrHubConnection) {
            RegisterClientFunctions();
            $.CorrHubConnection.start().done(function () {
                $.CorrHubConnection.proxies.corrsignalrhub.server.setWindowName(window.name).done(function () {
                });
            }).fail(function () {
                console.log('Could not Connect!');
            });
        }
    }
}


function GetDefaultPrinter() {
    return "";
}
function ShowPrintDialog() {
    return true;
}

function EditCorrOnLocalTool() {
    var aParams = {
        aParams: {
            DefaultPrinter: GetDefaultPrinter(),
            ShowPrintDialog: ShowPrintDialog()
        }
    }
    var methodparams = JSON.stringify(aParams);
    var resultString = "";
    SetChildWindowName();
    $.ajax({
        type: "POST",
        url: istrWebServiceName + "/EditCorrOnLocalTool",
        beforeSend: function (request) {
            request.setRequestHeader("FWN", window.name);
        },
        data: methodparams,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            data = (result.d);

            if ($("#lblMessage").length > 0) {
                $("#lblMessage").html(data);
            } else {
                alert(data);
            }

            if ($$("uppMessage") != null && $$("uppMessage").length > 0
                && $$("uppMessage").find(".msg-info").length > 0) {
                if (data.Message == "File is not generated.") {
                    nsCommon.DisplayError(DefaultMessages.FileNotGenerated);
                    return false;
                }
                else if (data.Message == "File does not exist.") {
                    nsCommon.DisplayError(DefaultMessages.FileNotFound);
                    return false;
                }
                else if (data.Message == "Correspondence file does not exist or not generated.") {
                    nsCommon.DisplayError(DefaultMessages.FileNotFoundOrGenerated);
                    return false;
                }                  
                else {
                    nsCommon.DisplayError(data.Message);
                }
            } else {
                if (data.Message != "Correspondence is opened in local tool.")
                    alert(data.Message);
            }
            SignalRCallToEditCorr(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if ($("#lblMessage").length > 0) {
                $("#lblMessage").html("<div style='color:red'>" + XMLHttpRequest.responseText.replace("{\"d\":\"", "").replace("\"}", "") + "</div>");
            } else {
                alert(XMLHttpRequest.responseText.replace("{\"d\":\"", "").replace("\"}", ""));
            }
            return false;
        }
    });
}

function SetChildWindowName() {
    if (window.name == undefined || window.name == "" || window.name == "Child") {
        var ldomWindow = $$("hflWindowName");
        if (ldomWindow.length > 0 && ldomWindow.val() != undefined && ldomWindow.val() != "") {
            window.name = ldomWindow.val().replace("ChildWindow", "");
        }
    }
}

$(document).on('change', '#cphPopupDialog_ddlJobScheduleID', function (e) {
    var scheduleName = $('#cphPopupDialog_ddlJobScheduleID option:selected').text();
    $('#cphPopupDialog_lblLabel7').val(scheduleName);
});
$(document).on('change', '#cphPopupDialog_ddlJobScheduleDetailID', function (e) {
    var scheduleName = $('#cphPopupDialog_ddlJobScheduleDetailID option:selected').text();
    var jobDetailId = $('#cphPopupDialog_ddlJobScheduleDetailID option:selected').val();
    $('#cphPopupDialog_lblLabel8').val(scheduleName);
    $('#cphPopupDialog_txtlblJobDetailsId').val(jobDetailId);
});
$(document).on("click", "#cphPopupDialog_btnFinishPopupDialogClick_JobStep", function () {
    $("#lblDialogErrors").html("");
    if ($("#cphPopupDialog_pnlActLogJobStep").find("#cphPopupDialog_lblLabel7").length > 0) {
        var ScheduleName = $("#cphPopupDialog_pnlActLogJobStep").find("#cphPopupDialog_lblLabel7").val();
        if (ScheduleName == null || ScheduleName == "") {
            $("#lblDialogErrors").html("Please Enter Schedule Name.").css("color", "red");
            return false;
        }
    }
    if ($("#cphPopupDialog_pnlActLogJobStep").find("#cphPopupDialog_lblLabel8").length > 0) {
        var StepName = $("#cphPopupDialog_pnlActLogJobStep").find("#cphPopupDialog_lblLabel8").val();
        if (StepName == null || StepName == "") {
            $("#lblDialogErrors").html("Please Enter Step Name.").css("color", "red");
            return false;
        }
    }
});

NeoGrid.prototype.renderGrid = function () {
    this.iintStartTime = new Date().getTime();
    this.iblnRendering = true;
    this.totalRecords = this.RenderData.length;
    if (this.iblnPaging !== true || this.iblnNavigatable === true) {
        this.pageSize = this.totalRecords;
        this.iintOriginalPageSize = this.pageSize;
        this.currentPage = 1;
    }
    if (this.istrRowTemplate == undefined || this.istrRowTemplate == '') {
        this.createTemplate();
    }
    var ldomTBody = this.element.find("tbody");
    var TBodyFragment = document.createDocumentFragment();
    var lblnAddHeader = true;
    var ldomTable;
    if (this.element.data("neoGrid") == undefined || this.element.data("neoGrid") == "")
        this.element.data("neoGrid", this);
    if (ldomTBody.length > 0) {
        if (this.iblnTable) {
            ldomTable = this.element;
        }
        else {
            ldomTable = this.element.find("table.s-grid");
        }
        lblnAddHeader = false;
        ldomTBody.find('*').off().end().empty();
    }
    else {
        this.element.find('*').off().end().empty();
        //this.element.empty();
        this.renderToolBarPanel();
        if (this.iblnCommonFilterBox) {
            this.renderCommonFilterBox();
        }
        if (this.iblnPaging && this.totalRecords > 0) {
            this.renderPager();
        }
        if (this.iblnShowSettings) {
            this.renderSettings();
        }
        if (this.iblnGrouping) {
            var lstrGroupableTemplate = ["<div class='s-groups'> <ul class='s-groups-list'> <li class='placeholder s-groupds-drop-header'>", nsConstants.GROUPING_DRAGNDROP_PLACEHOLDER_TEXT, "</li> </ul></div>"].join("");
            if (this.iblnTable) {
                $(lstrGroupableTemplate).insertBefore(this.element);
            }
            else {
                this.element.append($(lstrGroupableTemplate));
            }
        }
        if (this.iblnTable) {
            ldomTable = this.element;
        }
        else {
            ldomTable = $(["<table  role='table' class='s-grid fluid-table' id='Table_", this.element[0].id, "'></table>"].join(''));
        }
        ldomTBody = $("<tbody role='tbody' class='s-tbody'></tbody>");
        ldomTable.append(this.istrHdrTemplate);
        if (this.iblnGrouping) {
            this.registerGroupableEvents(ldomTable);
        }
    }
    if (this.iblnColumnRerendering === true) {
        ldomTable.find("thead").find('*').off();
        ldomTable.find("thead").remove();
        ldomTable.prepend(this.istrHdrTemplate);
        if (this.sortFields != undefined && this.sortFields.length > 0) {
            var larrSortFields = _.cloneDeep(this.sortFields);
            this.setSort(larrSortFields);
        }
        if (this.iblnGrouping) {
            this.registerGroupableEvents(ldomTable);
            if (this.iblnGrouping && this.groupedColumns != undefined && this.groupedColumns.length > 0 && this.totalRecords > 0) {
                var larrGroupedColumns = _.cloneDeep(this.groupedColumns);
                this.setGroup(larrGroupedColumns);
            }
        }
    }
    if (this.totalRecords <= 0) {
        if (this.pager != undefined && this.pager.length > 0) {
            this.pager.hide();
        }
        if (this.iblnShowHeaderWhenEmpty === false) {
            ldomTable.find("thead").hide();
            if (this.iblnGrouping) {
                if (this.iblnTable) {
                    this.gridContainer.find(".s-groups").hide();
                }
                else {
                    this.element.find(".s-groups").hide();
                }
            }
        }
        var lstrEmptyRow = ['<tr class="s-grid-empty-row"><td colspan="', this.columns.length, '" style="text-align:center">', this.istrEmptyDataText, '</td></tr>'].join('');
        //ldomTBody.append(lstrEmptyRow);
        TBodyFragment.appendChild($(lstrEmptyRow)[0]);
    }
    else {
        if (this.pager != undefined && this.pager.length > 0) {
            this.pager.show();
            var lintPages = this.pager.pagination("getPagesCount");
            if (lintPages == 1) {
                this.pager.find('ul').hide();
                this.pager.find('.s-paging-msg').hide();
                if (this.pager.find('.s-paging-msg').length == 1 && this.iblnCommonFilterBox === true) {
                    this.pager.find('.s-paging-msg').show();
                }
            }
        }
        if (this.iblnShowHeaderWhenEmpty === false) {
            ldomTable.find("thead").show();
            if (this.iblnGrouping)
                this.element.find(".s-groups").show();
        }
        ldomTBody.find("*").off().end().empty();
        this.view = [];
        var currentIndex = (this.currentPage - 1) * this.pageSize;
        var aintPageSize = this.currentPage * this.pageSize;
        var lintTolatRecs = this.totalRecords;
        if (this.prevPage != this.currentPage && this.iblnPaging && this.pager != undefined && this.pager.length > 0) {
            var lstrDisplayingInfo = ["<span class='s-paging-msg'>", nsConstants.PAGER_DISPLAYING_TEXT, " ", (currentIndex + 1), " ", nsConstants.PAGER_TO_TEXT, " ", (aintPageSize > lintTolatRecs ? lintTolatRecs : aintPageSize), " ", nsConstants.OF_TEXT, " ", lintTolatRecs, "</span>"].join("");
            if (this.pager.find('.s-paging-msg').length > 0) {
                this.pager.find('.s-paging-msg').remove();
            }
            this.pager.append(lstrDisplayingInfo);
            if (this.pager != undefined && this.pager.length > 0) {
                var lintPages = this.pager.pagination("getPagesCount");
                if (lintPages == 1) {
                    this.pager.find('ul').hide();
                    this.pager.find('.s-paging-msg').hide();
                    if (this.pager.find('.s-paging-msg').length == 1 && this.iblnCommonFilterBox === true) {
                        this.pager.find('.s-paging-msg').show();
                    }
                }
            }
        }
        aintPageSize = (aintPageSize > lintTolatRecs ? lintTolatRecs : aintPageSize);
        this.prevPage = this.currentPage;
        ldomTable.find("thead tr").find(".s-empty-th").remove();
        if (this.groupedColumns.length > 0 && this.iblnGrouping) {
            this.renderGridByGroupedData(ldomTBody, currentIndex, aintPageSize, TBodyFragment);
            var ths = "";
            for (var i = 0, colLen = this.groupedColumns.length; i < colLen; i++) {
                ths = [ths, "<th class='s-empty-th'></th>"].join("");
            }
            ldomTable.find("thead tr").prepend(ths);
        }
        else {
            for (var i = currentIndex; i < aintPageSize; i++) {
                if (this.RenderData[i]['uid'] === undefined) {
                    this.RenderData[i].uid = this.RenderData[i].rowIndex;
                }
                var ldomRow = $(this.fnRowTemplate(this.RenderData[i]));
                if (i % 2 == 0) {
                    ldomRow.addClass("s-row");
                }
                else {
                    ldomRow.addClass("s-altrow");
                }
                this.view.push(this.RenderData[i]);
                if (this.iblnTable) {
                    this.renderRawTemplateRow(ldomRow, this.RenderData[i]);
                }
                else if (this.iblnEditable || (this.irrEditableColumns != undefined && this.irrEditableColumns.length > 0)) {
                    this.renderRow(ldomRow, this.RenderData[i]);
                }
                if ((this.selection === 'multiple' || this.selection === 'single')) {
                    this.renderSelectCell(ldomRow, this.RenderData[i]);
                }
                //ldomTBody.append(ldomRow);
                TBodyFragment.appendChild(ldomRow[0]);
                this.onRowRender({
                    row: ldomRow,
                    item: this.RenderData[i],
                    sender: this
                });
            }
            // Add aggregate row
            if (_.filter(this.columnFields, function (col) { return col["aggregate"] != undefined; }).length > 0) {
                var FooterRow = NeoGrid.getAggregatedRow(this, this.RenderData, "");
                TBodyFragment.appendChild(FooterRow[0]);
            }
        }
    }
    ldomTBody[0].appendChild(TBodyFragment);
    if (lblnAddHeader) {
        this.iblnRestoreState = false;
        ldomTable.append(ldomTBody);
        if (!this.iblnTable)
            this.element.append(ldomTable);
        if (this.sortFields != undefined && this.sortFields.length > 0) {
            var larrSortFields = _.cloneDeep(this.sortFields);
            this.setSort(larrSortFields);
        }
        if (this.iblnGrouping && this.groupedColumns != undefined && this.groupedColumns.length > 0 && this.totalRecords > 0) {
            var larrGroupedColumns = _.cloneDeep(this.groupedColumns);
            this.setGroup(larrGroupedColumns);
        }
    }
    jQuery.removeData(this.element[0], "neoGrid");
    this.element.data("neoGrid", this);
    this.onDataBind();
    this.iintEndTime = new Date().getTime();
    this.istrGridRenderingTime = (this.iintEndTime - this.iintStartTime) + " ms";
    //console.log(this.istrGridRenderingTime);
    this.iblnRendering = false;
    this.setGroupHeaderWidth();
    jQuery.removeData(this.element[0], "neoGrid");
    this.iblnRestoreState = true;
    this.iblnColumnRerendering = false;
    if (this.options.iblnDisableDragDropForInputs === true && this.totalRecords > 0) {
        this.registerDropEventsForInput();
    }
    this.element.data("neoGrid", this);
    $("input[type='image'][imagebutton='true'][src*='sfwApplicationName']").each(function (item) {
        var lImageSource = $(this).attr("src");
        $(this).attr("src", lImageSource.replace("sfwApplicationName/", ""));
    });
};

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

