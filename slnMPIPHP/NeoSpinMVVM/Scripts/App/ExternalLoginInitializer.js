/// <reference path="reference.js" />
$(function () {
    
    ns.blnLoading = true;
    ns.blnUseSlideoutForLookup = false;
    ns.blnShowConfirmMsgForChildNodeDelete = false;
    ns.Language = "en-US";
    ns.iblnSetLandingPageFromInit = true;
    RegisterEvents();
    ns.iblnAddCustomButtonsToGridToolbar = true;
    ns.ReportPagePath = window.location.href.replace(window.location.hash, "") + "/";
    //ns.LandingPage = "wfmEssUserLoginWizard";
    //ns.viewModel.currentForm = ns.LandingPage;
    //ns.viewModel.currentModel = ns.LandingPage;
    
    ns.LandingPage = $("#LandingPage").val();
    ns.viewModel.currentForm = ns.LandingPage;
    ns.viewModel.currentModel = ns.LandingPage;
    FrameworkInitilize();

    // var MenuData = { MenuTemplate: ns.istrMenuTemplate }; // nsCommon.SyncPost("getMenu", [], null, "GET");
    //bindMenu(MenuData);
    //ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");
    kendo.bind($("#NotificationBar"), ns.NotificationModel.DirtyForms);

});


/* Added for framework version 6.0.0.18*****************************************************/
if (window['MVVMGlobal'] != undefined) {
    function RegisterEvents() {
        MVVMGlobal.RegisterEvents();
    }

    function FrameworkInitilize() {
        MVVMGlobal.FrameworkInitilize();
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

    function Extend_Custom(DivToApplyUI, astrActiveDivId) {
        MVVMGlobal.Extend_Custom(DivToApplyUI, astrActiveDivId);
    }

    function showDiv(astrDivID) {
        MVVMGlobal.showDiv(astrDivID);
    }
    function OnDeleteNodeClick(e) {
        nsEvents.OnDeleteNodeClick(e)
    }
}
/* End - Added for framework version 6.0.0.18*****************************************************/
