var nsCorrespondenceUDF = {};

nsCorrespondenceUDF = {

    OpenReprintHeader: function (e) {

        var btnId = $(e)[0].target.id;
        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
            var dataRows = lobjGridOrListView.getSelectedRows();
            if (dataRows.length > 0) {
                if (dataRows[0][btnId] != null && (dataRows[0][btnId]['P4'] == "" || dataRows[0][btnId]['P4'] == null || dataRows[0][btnId]['P4'] == "0")) {
                    nsCommon.DispalyError("Re-Print Header does not exist. Please select a valid record.", e.context.activeDivID);
                    return false;
                }
            }
            else {
                nsCommon.DispalyError("No record selected. Please select record(s) and try again.", e.context.activeDivID);
                return false;
            }
        }
        return true;
    },

    // Close Correspondence Editor
    CloseCorrWindow: function (e) {
        $("#EditCorrDiv_wnd_title").parent().find('a.k-window-action').trigger("click");
    },

    // Finish Button on Correspondence Editor
    CompleteCorrespondence: function (e) {
        try {
            if (nsCorr.CurrentCorr.SecurityLevel > 1) {
                try {
                    var BookmarkParams = {};
                    BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
                    BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
                    BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
                    BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
                    BookmarkParams["Status"] = "SEND";
                    var obj;
                    var data = nsCommon.SyncPost("GetGeneratedCommunicationInfo", BookmarkParams);
                    if (data != undefined) {
                        if (data.IsUserEditable == "Y") {
                            if (obj == null) {
                                obj = document.getElementById("ControlWordExcelObj");
                            }
                            if (obj) {
                                if (!obj.wDocument.Saved) {
                                    var btnCompleteCorrespondence = $("#btnCompleteCorrespondence")[0];
                                    alert("There are unsaved changes in the communication,  Please save changes before finishing the communication.");
                                    return;
                                }
                            }
                        }
                    }
                } catch (ex) {
                }
            }
            var chkSaveForLater = $('#chkSaveForLater').is(":checked");
            var chkDoNotSendToCommEngine = $('#chkDoNotSendToCommEngine').is(":checked");

            nsCorr.CloseCorrespondence();
            var BookmarkParams = {};
            BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
            BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
            BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
            BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
            BookmarkParams["SaveForLater"] = chkSaveForLater;
            BookmarkParams["DoNotSendToCommEngine"] = chkDoNotSendToCommEngine;
            BookmarkParams["Status"] = "PEND";
            data = nsCommon.SyncPost("SendCorrespondence", BookmarkParams);
            if (data != undefined) {
                alert(data);
            }
        }
        catch (ex) {
        }
    },

    FinishNonEditableCorrespondence: function (e) {
        try {
            var chkDoNotSendToCommunicationEngine = $('#chkDoNotSendToCommunicationEngine').is(":checked");

            nsCorr.CloseCorrespondence();
            var BookmarkParams = {};
            BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
            BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
            BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
            BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
            BookmarkParams["DoNotSendToCommEngine"] = chkDoNotSendToCommunicationEngine;
            BookmarkParams["Status"] = "PEND";
            BookmarkParams["Action"] = "FINISH";
            Result = nsCommon.SyncPost("FinishNonEditableCorrespondence", BookmarkParams);
            if (Result != undefined) {
                $('#chkDoNotSendToCommunicationEngine').attr("disabled", true);
                $('#btnFinishNonEditableCorrespondence').prop("disabled", true);
                $('#btnVOIDNonEditableCorrespondence').prop("disabled", true);
                $(":input[id^='correspondence']").prop("disabled", true);

                if (Result["IndexEcm"] == true)
                    $('#btnImageCorrespondence').prop("disabled", false);

                alert(Result["Message"]);
            }
        }
        catch (ex) {
        }
    },

    VOIDNonEditableCorrespondence: function (e) {
        try {
            var chkDoNotSendToCommunicationEngine = $('#chkDoNotSendToCommunicationEngine').is(":checked");

            nsCorr.CloseCorrespondence();
            var BookmarkParams = {};
            BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
            BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
            BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
            BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
            BookmarkParams["DoNotSendToCommEngine"] = chkDoNotSendToCommunicationEngine;
            BookmarkParams["Status"] = "PEND";
            BookmarkParams["Action"] = "VOID";
            data = nsCommon.SyncPost("FinishNonEditableCorrespondence", BookmarkParams);
            if (data != undefined) {
                $('#chkDoNotSendToCommunicationEngine').attr("disabled", true);
                $('#btnFinishNonEditableCorrespondence').prop("disabled", true);
                $('#btnVOIDNonEditableCorrespondence').prop("disabled", true);
                // Date: 24 September 2020
                // Iteration: Iteration-7
                // Developer: Tanaji Biradar.
                // Comment : [PIR-2179] - When a communication is voided system shows invalid message on success popup
                alert(data["Message"]);
            }
        }
        catch (ex) {
        }
    },

    UploadCorrespondenceToEcm: function (e) {
        try {
            nsCorr.CloseCorrespondence();
            var BookmarkParams = {};
            BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
            BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
            BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
            BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
            BookmarkParams["Action"] = "ECM";

            Result = nsCommon.SyncPost("FinishNonEditableCorrespondence", BookmarkParams);
            if (Result != undefined) {

                if (Result["IndexEcm"] === true)
                    $('#btnImageCorrespondence').prop("disabled", true);

                alert(Result["Message"]);
            }
        }
        catch (ex) {
        }
    },

    // Added JS Method to Handle Delete Confirmation on Resend Communication.
    DeleteConfirmation: function (e) {
        ns.viewModel.srcElement = e.target;
        var lobjBtnInfo = nsCommon.GetEventInfo(e.target);
        var ActiveDivID = e.context.activeDivID;
        var lbtnSelf = lobjBtnInfo.lbtnSelf;

        var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
        if (RelatedGridID === null) {
            alert(DefaultMessages.GridNotFound);
            return false;
        }

        var lobjRelatedControl = nsCommon.CheckGridOrListView(ActiveDivID, RelatedGridID);
        if (lobjRelatedControl.NotFound) {
            alert(DefaultMessages.GridNotFound);
            return false;
        }

        var lblnIsListView = lobjRelatedControl.blnIsListView;
        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lobjRelatedControl.RelatedControlId);
        if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
            alert(DefaultMessages.GridNotFound);
            return false;
        }

        var arrSelectedRows = [];
        var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
        if (lintSelectedIndex > -1) {
            arrSelectedRows.push(Number(lintSelectedIndex));
        }
        else {
            arrSelectedRows = lobjGridWidget.getSelectedIndexes();
        }

        if (arrSelectedRows.length === 0) {
            var sfwMessageNoRowSelected = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageNoRowSelected", ActiveDivID);
            if (sfwMessageNoRowSelected == null)
                sfwMessageNoRowSelected = DefaultMessages.NoRowSelectedforGridViewDelete;

            nsCommon.DispalyError(sfwMessageNoRowSelected, ActiveDivID);
            return false;
        }

        var lblnIsLookUp = ActiveDivID.indexOf(nsConstants.LOOKUP) > 0;
        var result = true;

        if (arrSelectedRows.length > 0) {

            if (!lblnIsLookUp && ns.DirtyData[ActiveDivID] != undefined) {
                result = confirm(DefaultMessages.DeleteConfirmationIfUnsaved);
            } else {
                var sfwMessageActionConfirmation = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageActionConfirmation", ActiveDivID);
                if (sfwMessageActionConfirmation == null)
                    sfwMessageActionConfirmation = DefaultMessages.DeleteConfirmation;
                result = confirm(sfwMessageActionConfirmation);
            }
            if (!result)
                return false;
        }

        return result;
    },

    btnEditCorrespo: function (e) {

        if (e !== undefined && e !== null) {
            if (e.currentTarget !== undefined && e.currentTarget != null) {
                var formName = e.currentTarget.getAttribute('sfwparentformid');
                var BookmarkParams = {};
                var Bk = {};

                if (formName != undefined && formName != null) {
                    if (formName === 'wfmCorrespondenceClientMVVM') {
                        var fileName = e.currentTarget.name;
                        var SecurityValue = (document.getElementById('ddlCorrespondenceList').value).split(';')[0];

                        Bk["FileName"] = fileName;
                        Bk["SecurityLevel"] = SecurityValue;

                        BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
                        BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
                    }

                    if (formName == 'wfmCorTrackingMaintenance') {

                        var lintTrackingId = (document.getElementById('lblTrackingId')).innerText;
                        Bk["TrackingId"] = lintTrackingId;

                        BookmarkParams["FormID"] = formName;
                        BookmarkParams["KeyField"] = lintTrackingId;
                    }
                }
            }
        }

        if (nsCorr.UseLocalTool) {
            var Result = nsCommon.SyncPost("GetCorrEnclosureInfo", Bk);

            if (Result["[Error]"] !== undefined && Result["[Error]"] !== null) {
                alert(Result["[Error]"]);
                return false;
            }

            if (formName == 'wfmCorTrackingMaintenance') {
                if (nsCorr != undefined && nsCorr.CurrentCorr != undefined) {
                    nsCorr.CurrentCorr.CorrTemplate = Result["TemplateName"];
                    nsCorr.CurrentCorr.CallingForm = formName;
                    nsCorr.CurrentCorr.ParentCorrForm = formName;
                    nsCorr.CurrentCorr.KeyField = lintTrackingId;
                    nsCorr.CurrentCorr.CorrForm = Result["CorrForm"];
                    nsCorr.CurrentCorr.CorrFilePath = Result["CorrFilePath"];
                    nsCorr.CurrentCorr.SecurityLevel = Result["SecurityLevel"];
                    //nsCorr.CurrentCorr.CorrFileData = '';
                }
            }

            if (Result != undefined) {
                BookmarkParams["TemplateName"] = Result["TemplateName"];
                BookmarkParams["LastGeneratedCorr"] = Result["CorrFilePath"];
                BookmarkParams["LastCorrSecurityLevel"] = Result["SecurityLevel"];
            }

            BookmarkParams["ShowPrintDialog"] = nsCorr.ShowPrintDialog();
            BookmarkParams["DefaultPrinter"] = nsCorr.GetDefaultPrinter();

            var lobjAjaxData = { action: "EditCorrOnLocalTool", param: BookmarkParams, PrevActiveForm: nsCorr.CurrentCorr.CorrDivID, ActiveForm: nsCorr.CurrentCorr.CorrDivID, SrcElement: ns.viewModel.srcElement };
            return nsCommon.GetAjaxRequest(lobjAjaxData);
        }
        else {
            var lstrEditCorrDiv = "EditCorrDiv";
            var EditCorrDiv = $([nsConstants.HASH, lstrEditCorrDiv].join(''));
            var lobjCorrWidgetControls = nsCommon.GetWidgetControl(EditCorrDiv);
            if (lobjCorrWidgetControls != undefined) {
                lobjCorrWidgetControls.open();
            }
            else {
                var lobjCorrWidgetControls = MVVM.Controls.Dialog.CreateInstance(EditCorrDiv, lstrEditCorrDiv, {
                    title: Sagitec.DefaultText.WINDOW_TITLE_ERROR_MESSAGE,
                    width: "600px",
                    close: function () { },
                    deactivate: "empty",
                    position: '{"top": 100}'
                });
                nsCommon.SetWidgetControlByDivID(lstrEditCorrDiv, lobjCorrWidgetControls, nsCorr.CurrentCorr.CorrDivID);
                lobjCorrWidgetControls.open();
            }
            nsCorr.OpenCorrespondence();
        }
    },
};

var BaseGetNavigationParams = nsCommon.GetNavigationParams;

nsCommon.GetNavigationParams = function (button, e) {
    if (button != undefined && button.id === "btnViewCorrespondence" && button.value === "View") {

        var BookmarkParams = {};
        if (button.name === "btnViewCorrespondence") {
            BookmarkParams["FileName"] = nsCorr.CurrentCorr.CorrFilePath;
        }
        else {
            BookmarkParams["FileId"] = button.name;
        }

        var aintTrackingID = nsCommon.SyncPost("GetDecryptedFilePath", BookmarkParams, null, "POST");
        var params = {
            "larrRows": [
                {
                    "aintTrackingId": aintTrackingID
                }
            ],
            "lstrActiveForm": null,
            "lstrFirstID": aintTrackingID,
            "larrNodeInfo": [
                {
                    "ActiveForm": null,
                    "PrimaryKey": aintTrackingID,
                    "Title": aintTrackingID,
                    "ToolTip": aintTrackingID
                }
            ]
        }
        return params;
    }
    else {
        var params = BaseGetNavigationParams(button, e);
        return params;
    }
}

// Iteration : 9
// Developer : Jagruti Bachhav
// Comment   : Function to update properties of PieCharts of Communication Dashboard.
// Date      : 14 May 2021
var BasePieChartBeforeRenderChart = MVVM.JQueryControls.Piechart.prototype.BeforeRenderChart;
MVVM.JQueryControls.Piechart.prototype.BeforeRenderChart = function () {
    this.BaseBeforePieChartRenderChart = BasePieChartBeforeRenderChart;
    this.BaseBeforePieChartRenderChart();
    if (ns.viewModel.currentForm == "wfmCommDashboardMaintenance" || ns.viewModel.currentForm == "wfmPrintDashboardMaintenance") {
        if (this.iobjLayout == null)
            this.iobjLayout = {};
        this.iobjLayout.margin = { "t": 0, "b": 0, "l": 0, "r": 0 };
        if (this.iobjSeriesData && this.iobjSeriesData.length > 0) {
            this.iobjSeriesData[0].textinfo = "none";
            this.iobjSeriesData[0].sort = false;
        }
    }
}

function LoadCommunicationDashboard() {
    // Date: 23rd june 2021
    // Iteration: Iteration 9
    // Developer: Jagruti Bachhav
    // Comment : Communication Dashboard changes.
    $(document).on("change", "#chkReconcileFlag", function (e) {
        if ($('#chkReconcileFlag').is(':checked')) {
            $('#txtReconFlag').text('Y');
            $('#txtReconFlag').trigger('change');
            $('#btnCancel').click();
            return true;
        }
        $('#btnCancel').click();
    });
}

nsUserFunctions = Object.assign(nsUserFunctions, nsCorrespondenceUDF);
nsCorrespondenceUDF = undefined;