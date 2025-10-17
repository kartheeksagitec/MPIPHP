//nsConstants.MY_TASK_SELECTOR = "#divMyTaskBasketContainer"
$(function () {
    nsConstants.MY_TASK_SELECTOR = "#divMyTaskBasketContainer";
    nsConstants.BPM_WORKFLOW_CENTERLEFT_MAINTENANCE = "wfmBPMWorkflowCenterLeftMaintenance";
    //Framework 6.0.10.0  Support for Shortcut Keys for Forms 
    ns.iblnIsShortCutRequired = true;
    //Framework 6.0.10.0 Reset value of the date control if Entered Date Value is Invalid Date.This functionality is made configurable at the Application Level.
    ns.iblnClearValueForInvalidDate = true;
    //Framework 6.0.10.0 Expecting to suppress the "Invalid Date" alert on the basis of application configuration.This functionality is made configurable at the Application Level.
    ns.iblnShowAlertForInvalidDate = true;
    //FM 6.0.4.2 - Execution of constraints on focus out can be enabled by setting flag.
    ns.iblnErrorOnFocusOut = false;
    ns.iblnMobileGrids = true;
    //FM 6.0.3.0 - Enhanced the UI functionality by freezing toolbar and breadcrumb from scrolling vertically.
    ns.iblnFreezeBreadCrumToolBar = true;
    ns.iblnHideCrumOnNonMaintenance = true;
    ns.iblnNavigateInNewMode = true; //FM 6.0.0.32 to override default behaviour of not allowing to navigate in new mode.
    nsCenterLeftRefresh.iblnShowMyBasketInCenterLeft = true;
    ns.iblnAddCustomButtonsToGridToolbar = true;
    //ns.iblnShowHardErrorAssociatedToControl = true;
    //ns.iblnShowHardErrorAssociatedToGridControl = true;
    ns.iblnHighlightAllErrorControls = true;
    ns.iblnDisplayConstraintsAsSummary = true;
    nsRpt.iblnAddEmptyReportParams = true;
   // ns.iblnNonCollapsiblePanels = true;
    if (ns.iblnADATesting) { return false; }
    ns.blnLoading = true;
    ns.blnUseSlideoutForLookup = false;   
    //ns.Language = "en-US";
    ns.lstrDateRange = '1901:2100';
    Sagitec.nsFormatting.DateTimeFormatter.MaximumSupportedYear = 9999;
    Sagitec.nsFormatting.DateTimeFormatter.MinimumSupportedYear = 0000;
    Sagitec.nsFormatting.DateTimeFormatter.iobjCultureDateFormat.AbbreviatedMonthNames = Sagitec.nsFormatting.DateTimeFormatter.iobjCultureDateFormat.MonthNames;

    MVVMGlobal.RegisterEvents();
    ns.iblnTabNavigatorOnMainteance = false;
    ns.ReportPagePath = window.location.href.replace(window.location.hash, "") + "/";
    nsCorr.UseLocalTool = true;
    ns.iblnHidePagesFromGridPaging = true;
    ns.iblnImagesForPaging = true;

    //Application level refresh flag
    ns.iblnOpenRefreshedForm = true;
    DefaultMessages.NoRecordSelected = "No rows were selected. Please select the row(s) to be opened.";
    DefaultMessages.NoRowSelectedforGridViewDelete = "No rows were selected.Please select the row(s) to be deleted."

    // 6.0.2.2 framework Changes Added support for displaying right aligned currency fields in the textbox
    //ns.iblnCurrencyRightAligned = true;
    ns.iblnExecuteSearchOnBpmLaunch = true;
    // 6.0.3.0 Enhanced the UI functionality by freezing tool bar and breadcrumb from scrolling vertically
    ns.iblnFreezeBreadCrumToolBar = true;
    ns.intFormsToOpenLimit = 50;
    // 6.0.3.0 To hide the freezed crum div container on the Non maintenance pages
    ns.iblnHideCrumOnNonMaintenance = true; 
    nsCenterLeftRefresh.iblnShowMyBasketInCenterLeft = true;
    //ns.iobjCenterLeftContainers[nsConstants.KNOWTION_CENTERLEFT_MAINTENANCE] = "KnowtionQuickSearch"; 
    ns.iblnHideStoreSearchOnRetrive = false;
    nsVisi.iblnHideParentOnVisibility = false;
    nsCorr.WindowWidth = "960px";

    ns.CenterLeftPanelBar = $("#CenterLeft" + " ul[controlType='accordian']").kendoPanelBar({
        expandMode: "single"
    }).data("kendoPanelBar");

    $(".task-panel-wrapper" + " ul[controlType='panelbar']").kendoPanelBar({
        expandMode: "single"
    }).data("kendoPanelBar");

    $("#CenterSplitter").scroll(function () {
        $('#ui-datepicker-div').css('display', 'none');
    });

    $("#CenterLeft").scroll(function () {
        $('#ui-datepicker-div').css('display', 'none');
    });

    if (ns.blnUseSlideoutForLookup) {

        $('#SlideOutLookup').slidePanel({
            triggerName: '#SearchTriger',
            position: 'fixed',
            triggerTopPos: '28px',
            panelTopPos: '87px',
            panelOpacity: 1,
            clickOutsideToClose: true,
            speed: "slow"
        });
    }
    else {
        $("#SearchTriger").hide();
        $("#crumDiv").after($("#LookupName"));
    }

    $('#SlideOutTree').slidePanel({
        triggerName: '#navTreeTriger',
        position: 'fixed',
        triggerTopPos: '22px',
        panelTopPos: '100px',
        panelOpacity: 1,
        clickOutsideToClose: true,
        speed: "slow"
    });    
    ns.LandingPage = ($("#LandingPage").length > 0 && $.trim($("#LandingPage").val()) != "" ? $("#LandingPage").val() : "wfmPersonLookup");
    ns.viewModel.currentForm = ns.LandingPage;
    ns.viewModel.currentModel = ns.LandingPage;
    ns.iblnSkipWhiteSpacesFromSearch = true; 
    MVVMGlobal.FrameworkInitilize();
    var MenuData = { MenuTemplate: ns.istrMenuTemplate };
    bindMenu(MenuData);
    ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");   

    kendo.bind($("#NotificationBar"), ns.NotificationModel.DirtyForms);

    //This property sets the Correcpondence Dialogue title.

    nsCorr.WindowTitle = "Communication";
    
});

function bindMenu(data) {

    $("#cssmenu").html(data.MenuTemplate);    

    var ldomMyTaskBasket = document.getElementById(nsConstants.MY_TASK_DIV_CONTAINER);
    if (ldomMyTaskBasket != null) {
        ldomMyTaskBasket.style.display = "none";
        $(ldomMyTaskBasket).find(".my-task-panel[controltype='panelbar']").hide();
        $(ldomMyTaskBasket).find(".my-task-panel[controltype='panelbar']:first").show();
        ns.RenderPanelBar($(ldomMyTaskBasket), "body", nsConstants.MY_TASK_DIV_CONTAINER, nsConstants.MY_TASK_DIV_CONTAINER, nsConstants.MY_TASK_DIV_CONTAINER, false, false, {});
    }
    ns.idictSpitter["MainSplitter"] = MVVM.Controls.Splitter.CreateInstance($("#MainSplitter"), "MainSplitter", {
        lstrOrientation: "vertical",
        larrPane: [
            { size: "66px", resizable: false },
            { resizable: false, scrollable: false },
            { size: "25px", resizable: false }
        ]
    });


    ns.idictSpitter[nsConstants.MIDDLE_SPLITTER] = MVVM.Controls.Splitter.CreateInstance($("#MiddleSplitter"), "MiddleSplitter", {
        lstrOrientation: "horizontal",
        larrPane: [
            //size: "280px",
            { resizable: false, collapsible: true, collapsed: false, scrollable: false },
            { resizable: true, scrollable: false }
        ]
    });

    ns.idictSpitter["CenterMiddle"] = MVVM.Controls.Splitter.CreateInstance($("#CenterMiddle"), "CenterMiddle", {
        lstrOrientation: "horizontal",
        larrPane: [
            { resizable: false, collapsible: false, size: "15px", scrollable: false },
            { resizable: true, scrollable: true },
            { resizable: true, collapsible: true, collapsed: true, scrollable: true }
        ]
    });
   // $("[sfwCBPanel='true']").kendoPanelBar({});


    var temp = selectnav('MenuUl', {
        label: '### Table of content ### ',
        nested: true,
        indent: '--'
    });

    $("#selectnav1").on("change", function (e) {
        if ($(this).val() !== "")
            $("#cssmenu").find("li[formid='" + $(this).val() + "']").trigger("click");
    });
}

/* Added for framework version 6.0.0.18*****************************************************/
if (window['MVVMGlobal'] != undefined) {
    function RegisterEvents() {
        MVVMGlobal.RegisterEvents();
    }

    function FrameworkInitilize() {
        MVVMGlobal.FrameworkInitilize();

        window.onerror = function (msg, url, linenumber) {
            console.log(['Error message: ', msg, '\nURL: ', url, '\nLine Number: ', linenumber].join(''));
            ns.displayActivity(false);
            ns.blnLoading = false;
            ns.iblnBindingDialog = false;
            ns.istrDialogPanelID = "";
            ns.iblnBindingLeftForm = false;
            $("#custom-loader").hide();
            return true;
        };
        //MVVMGlobal.CheckForSupportedBrowser();
    }

    nsCommon.SyncPost = nsRequest.SyncPost;

    function GetControlAttribute(astrControl, astrAttribute, astrActiveDivID) {
        return MVVMGlobal.GetControlAttribute(astrControl, astrAttribute, astrActiveDivID);
    }

    ns.getTemplate = nsRequest.getTemplate;

    function setRequestingForm(btnSelf) {
        MVVMGlobal.setRequestingForm(btnSelf);
    }

    function Extend_Custom(DivToApplyUI) {
        MVVMGlobal.Extend_Custom(DivToApplyUI);
    }

    function showDiv(astrDivID) {
        MVVMGlobal.showDiv(astrDivID);
    }
    function OnDeleteNodeClick(e) {
        nsEvents.OnDeleteNodeClick(e)
    }
}


$(document).ready(function () {
    $('.toggle-menu-container .CssSliderLeftMenu.cssmenu li.active').click(function () {
        $('.toggle-menu-container .CssSliderLeftMenu.cssmenu li.active').removeClass("item-menu-selected");
        var sub_li = $(this).siblings("li");
        $.each(sub_li, function (index, value) {
            var sub_ul = $(value).find("ul.sub-menu");
            $(sub_ul).hide();
        })
        var firstelement = $(this).children("ul.sub-menu")[0];
        $(firstelement).toggle();
        $(this).addClass("item-menu-selected");
        return this.defaultSelected;
    });
    $('ul.sub-menu').click(function () {
        $(this).toggle();
        return this.defaultSelected;
    })
});
