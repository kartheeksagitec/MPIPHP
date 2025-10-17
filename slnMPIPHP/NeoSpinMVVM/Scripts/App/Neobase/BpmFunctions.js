var nsBpmUDF = {};

nsBpmUDF = {

    // Added JS Method to Handle Delete Confirmation on BPM wfmBpmActivityInstanceMaintenance screen
    SuspendAndTerminateConfirmation: function (e) {
        var data = $('#' + e.context.activeDivID);
        var result = true;

        if (e.currentTarget.id == "btnSuspendActivity") {
            var SuspensionReason = data.find("#ddlCMSuspensionReasonValue").val();
            var SuspensionDate = data.find("#txtCMSuspensionDate").val();
            var ResumeAction = data.find("#ddlCMResumeActionValue").val();
            var SuspensionNote = data.find("#txtCMComments").val();

            if (SuspensionReason !== "" && SuspensionDate !== "" && ResumeAction !== "" && SuspensionNote !== "") {
                result = confirm("Are you sure you want to Suspend the activity?");
            }
        }
        else if (e.currentTarget.id === "btnCancelActivity") {
            var TerminationReason = data.find("#txtTerminationReason").val();

            if (TerminationReason !== "") {
                result = confirm("Are you sure you want to terminate the workflow?");
            }
        }

        return result;
    },

    btnMap_Click: function (e) {
        var FormContainerID = "";
        var ActiveDivID = "";
        var RelatedGridID = "";

        var lbtnSelf;
        var lintSelectedIndex = -1;
        if (e != undefined && e.tagName === "A") {
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = $(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
        } else {
            lbtnSelf = ns.viewModel.srcElement;
            var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }

        RelatedGridID = GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
        if (RelatedGridID !== null) {
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
            if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
                return false;
            }
        }

        var PrimaryId = 0;
        if (ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
            PrimaryId = parseInt($("#" + e.context.activeDivID + " #lblCaseId").text());
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
            PrimaryId = parseInt($("#" + e.context.activeDivID + " #ddlProcessId").val());
        }
        else {
            var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, e);
        }

        if (ActiveDivID.lastIndexOf("wfmDesignSpecificationMaintenance", 0) === 0) {
            nsCommon.localStorageSet("design_specification_bpm_map_id", ldictParams.lstrFirstID);
            //window.open(ns.SiteName + "/BPMExecution/MapRender.html", "_blank");
            window.open(ns.SiteName + "/Home/MapRender", "_blank");
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMInitiationMaintenance", 0) === 0) {
            nsCommon.localStorageSet("ProcessId", PrimaryId);
            nsCommon.localStorageSet("CaseId", 0);
            window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
        }
        else if (ActiveDivID.lastIndexOf("wfmBPMCaseLookup", 0) === 0 || ActiveDivID.lastIndexOf("wfmBPMCaseMaintenance", 0) === 0) {
            nsCommon.localStorageSet("ProcessId", 0);
            nsCommon.localStorageSet("CaseId", PrimaryId > 0 ? PrimaryId : ldictParams.lstrFirstID);
            var w = window.open(ns.SiteName + "/Home/BPMNReadOnlyMap", "_blank");
            w.name = window.name;
        }
        else {
            nsCommon.localStorageSet("CaseInstanceID", ldictParams.lstrFirstID);
            //window.open(ns.SiteName + "/BPMExecution/BPMNMap.html", "_blank");
            window.open(ns.SiteName + "/Home/BPMNMap", "_blank");
        }
    },
    DisplayBreadCrums: function (e) {
        if ((e.context.activeDivID.indexOf("wfmBPMWorkflowCenterLeftMaintenance") == 0)) {
            //BreadCrumLoadNames(e);
            return false;
        }
        return true;
    },
};


nsUserFunctions = Object.assign(nsUserFunctions, nsBpmUDF);
nsBpmUDF = undefined;
