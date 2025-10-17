var nsEcmUDF = {};

nsEcmUDF = {
    UploadDocument: function (e) {
        //Pushawart: Need to make hard error show up through javascript with the help of Amol
        //var FileType = $("#" + e.context.activeDivID).find("#ddlFileType").val();
        //if (FileType === undefined || FileType === "") {
        //    nsCommon.DispalyError("File type must be selected.", e.context.activeDivID);
        //    return false;
        //}
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName, KeysData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        ns.DirtyData[e.context.activeDivID].KeysData = ns.viewModel[e.context.activeDivID].KeysData;
        switch (lstrFormName) {
            case "wfmECMUploadDocumentMaintenance":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["DocGroup"] = $("select#ddlIstrGroupValue option:selected").val();
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["DocType"] = $("select#ddlDocumentType option:selected").val();
                }
                break;
        }
        //Pushawart: Not deleting the code to call the button click to refresh the screen after uploading the document to show all uploaded files in the grid.
        //var executeButton = $("#" + e.context.activeDivID).find("#btnExecuteRefreshData");
        //setTimeout(function (e) {
        //    executeButton.trigger("click");
        //}, 3000);

        return true;
    },
    DownloadDocument: function (e) {
        if (ns.DirtyData[e.context.activeDivID] === undefined) {
            var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
            ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName };
            ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
            ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        }
        else {
            var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
            ns.DirtyData[e.context.activeDivID].HeaderData.istrFormName = lstrFormName;
        }
        return true;
    },

    //Metod to open the Document.
    PrintECMDocumentClick: function (e) {

        var lstrGuid = "";
        var FormContainerID = "";
        var ActiveDivID = "";
        var RelatedGridID = "";
        var lbtnSelf;
        var event = undefined;
        if (e !== undefined && e.tagName === "A") {
            event = e;
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = nsCommon.GetActiveDivId(e); //$(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
        } else {
            lbtnSelf = e.target;
            ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf); //$(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }

        RelatedGridID = GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
        if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
            return false;
        }

        //getting navigation params
        var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, event);

        if (ldictParams.larrRows.length == 0 && RelatedGridID != undefined) {
            nsCommon.DispalyError("No record selected. Please select record(s) and try again.", ActiveDivID);
            return;
        }

        lstrGuid = '';
        for (i = 0; i < ldictParams.larrRows.length; i++) {
            lstrGuid += ldictParams.larrRows[i]["astrGuID"];
            if (i != ldictParams.larrRows.length - 1)
                lstrGuid += ',';
        }

        var fileDownloadUrl = nsCommon.SyncPost("PrintECMDocument?GuId=" + lstrGuid);


        return false;

    },

    //Metod to open the Document.
    OpenECMDocumentClick: function (e) {
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);

        if (lstrFormName == "wfmECMImagesLookup_retrieve") {
            return true;
        }

        var lstrDocClass = "";
        var lstrGuid = "";
        var FormContainerID = "";
        var ActiveDivID = "";
        var RelatedGridID = "";
        var lbtnSelf;
        var event = undefined;
        if (e != undefined && e.tagName === "A") {
            event = e;
            lbtnSelf = $(e)[0];
            var FormContainerID = "#" + $(e).closest('div[role="group"]')[0].id;
            var ActiveDivID = nsCommon.GetActiveDivId(e); //$(e).closest('div[id^="wfm"]')[0].id;
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
        } else {
            lbtnSelf = e.target;
            ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf); //$(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }

        RelatedGridID = GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
        var larrRows = null;
        if (RelatedGridID === null) {
            if (ns.viewModel[ns.viewModel.currentModel].HeaderData.ButtonNavParams["btnView"] !== undefined)
                var btnnavparams = ns.viewModel[ns.viewModel.currentModel].HeaderData.ButtonNavParams["btnView"].P1;
            DataToSend = {
                "SenderForm": ns.viewModel.currentForm,
                "SenderKey": ns.viewModel[ns.viewModel.currentModel].OtherData.SenderKey,
                "ButtonNavParams": btnnavparams,
                "PrimaryKey": ns.viewModel[ns.viewModel.currentModel].KeysData.PrimaryKey
            };
            var data = nsCommon.SyncPost("GetNavParamsData", DataToSend, null, "POST");
            if (data == undefined)
                return false;
            larrRows = [];
            larrRows.push(data);
            lstrGuid = data["astrGuID"];
            lstrDocClass = data["astrDocClass"];
        }
        else {

            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
            if (lobjGridWidget === undefined || lobjGridWidget.jsObject === undefined) {
                return false;
            }

            //getting navigation params
            var ldictParams = nsCommon.GetNavigationParams(lbtnSelf, event);

            if (ldictParams.larrRows.length === 0 && RelatedGridID != undefined) {
                nsCommon.DispalyError("No record selected. Please select record(s) and try again.", ActiveDivID);
                return false;
            }

            larrRows = ldictParams.larrRows;
            lstrGuid = ldictParams.larrRows[0]["astrGuID"];
            lstrDocClass = ldictParams.larrRows[0]["astrDocClass"];
        }

        if (lstrGuid !== "" && lstrDocClass !== "") {
            var larrECMDocumentUrl = '';
            if (lbtnSelf.id === 'btnAnnotate')
                larrECMDocumentUrl = nsCommon.SyncPost("AnnotateECMDocument?GuId=" + lstrGuid + "&DocValue=" + lstrDocClass);//nsCommon.SyncPost("GetECMDocument", larrRows, undefined, "POST");
            else if (lbtnSelf.id === 'btnView' || lbtnSelf.id == 'btnView_UserControlUdc') {
                var larrECMDocumentUrl = nsCommon.SyncPost("GetECMDocumentUrls", larrRows, undefined, "POST");

                //var lstGuid = '';
                //for (i = 0; i < ldictParams.larrRows.length; i++) {
                //    lstGuid += ldictParams.larrRows[i]["astrGuID"];
                //    if (i != ldictParams.larrRows.length - 1)
                //        lstGuid += ',';}
                //larrECMDocumentUrl = nsCommon.SyncPost("ViewECMDocument?GuId=" + lstGuid + "&DocValue=" + lstrDocClass);
            }
            else if (lbtnSelf.id == 'btnAnnotateView')
                larrECMDocumentUrl = nsCommon.SyncPost("AnnotateViewECMDocument?GuId=" + lstrGuid + "&DocValue=" + lstrDocClass);


            if (larrECMDocumentUrl !== undefined) {
                $.each(larrECMDocumentUrl, function (index, value) {
                    var urlValue = value;
                    window.open(urlValue, '_blank');
                    PopupCenterDual(urlValue, index, 1000, 1000);
                    var dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : screen.left;
                    var dualScreenTop = window.screenTop !== undefined ? window.screenTop : screen.top;
                    width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                    height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                    var left = ((width / 2) - (1000 / 2)) + dualScreenLeft;
                    var top = ((height / 2) - (1000 / 2)) + dualScreenTop;
                    var newWindow = window.open(urlValue, "ECM_" + index, 'width=' + 1000 + 'height=' + 1000 + ', top=' + top + ', left=' + left + ',resizable=yes,scrollbars=yes,titlebar=yes,toolbar=no,menubar=no,location=no,directories=no, status=yes');
                });
            }
            else {
                nsCommon.DispalyError("The user is not authenticated.", ActiveDivID);
            }



            //var win = window.open(larrECMDocumentUrl, '_blank');
            //win.focus();
            //PopupCenterDual(larrECMDocumentUrl, 0, 1000, 1000);
            //if (larrECMDocumentUrl !== undefined) {
            //    $.each(larrECMDocumentUrl, function (index, value) {
            //        var urlValue = value;
            //        alert(urlValue);
            //        PopupCenterDual(urlValue, index, 1000, 1000);
            //    });
            //}
            //else {
            //    nsCommon.DispalyError("The user is not authenticated.", ActiveDivID);
            //}
        }

        return false;

    },

    PopupCenterDual: function (url, title, w, h) {
        // Fixes dual-screen position Most browsers Firefox
        var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
        var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;
        width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        var left = ((width / 2) - (w / 2)) + dualScreenLeft;
        var top = ((height / 2) - (h / 2)) + dualScreenTop;
        //var newWindow = window.open(url, title, 'scrollbars=yes, resizable = yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        var newWindow = window.open(url, "ECM_" + title, 'width=' + w + 'height=' + h + ', top=' + top + ', left=' + left + ',resizable=yes,scrollbars=yes,titlebar=yes,toolbar=no,menubar=no,location=no,directories=no, status=yes');
    },

    GetECMDocumentNew: function (e) {
        var DocId = $("#lblIstrGuId").text();
        var DocValue = $('#lblIstrDocumentClass').text();
        var DocPath = nsCommon.SyncPost("GetECMDocument?DocId=" + DocId + "&DocValue=" + DocValue);
        window.open(DocPath);
        return false;
    },
};

nsUserFunctions = Object.assign(nsUserFunctions, nsEcmUDF);
nsEcmUDF = undefined;
