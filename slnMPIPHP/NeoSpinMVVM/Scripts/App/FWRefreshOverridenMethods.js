//Application level refresh flag
ns.getData = function () {
    var lstrFirstID = nsCommon.sessionGet([ns.viewModel.currentModel, "_Params"].join(''));
    ns.blnDataFromServer = true;
    var lActivityCopyParams = nsCommon.sessionGet([ns.viewModel.currentModel, "_ActivityCopyParams"].join(''));
    nsCommon.sessionRemove([ns.viewModel.currentModel, "_ActivityCopyParams"].join(''));
    if (lActivityCopyParams == undefined) {
        lActivityCopyParams = {};
    }
    var lActivityInstanceDetails = nsCommon.sessionGet([ns.viewModel.currentModel, nsConstants.UNDERSCORE_ACTIVITY_INSTANCE_DETAILS].join(''));
    var lobjTreeNode = nsCommon.GetDataItemFromDivID(ns.viewModel.currentModel);
    var navParams = undefined;
    var lblnIsOpenInDialog = undefined;
    var lblnViewOnlyForm = false;
    var lblnParentViewOnly = false;
    var parentNode;
    if (lobjTreeNode != undefined) {
        if (lobjTreeNode.SenderID != undefined) {
            ns.SenderID = lobjTreeNode.SenderID;
        }
        if (ns.SenderID == "") {
            ns.SenderID = "FromMenu";
        }
        if (lobjTreeNode.SenderForm != undefined) {
            ns.SenderForm = lobjTreeNode.SenderForm;
        }
        if (lobjTreeNode.SenderKey != undefined) {
            ns.SenderKey = lobjTreeNode.SenderKey;
        }
        navParams = lobjTreeNode.navParams;
        lblnIsOpenInDialog = lobjTreeNode.IsOpenInDialog === true;
        lblnViewOnlyForm = lobjTreeNode.IsViewOnly === true;
        parentNode = lobjTreeNode.parentNode();
        if (parentNode != undefined) {
            lblnParentViewOnly = parentNode.IsViewOnly === true;
        }
    }
    else {
        navParams = nsCommon.sessionGet([nsCommon.GetProperFormName(ns.viewModel.currentForm), lstrFirstID, "_navParams"].join(''));
        ns.SenderID = "FromMenu";
    }
    var ldomSrcElement = ns.viewModel.srcElement;
    var ActiveDivID;
    if (ldomSrcElement != null && $(ldomSrcElement).length > 0) {
        ActiveDivID = nsCommon.GetActiveDivId(ldomSrcElement);
    }
    if (lobjTreeNode == undefined && parentNode == undefined && ldomSrcElement != null && $(ldomSrcElement).length > 0) {
        var lstrMethoName = MVVMGlobal.GetControlAttribute($(ldomSrcElement), nsConstants.SFW_METHOD_NAME, ActiveDivID);
        if (lstrMethoName === "btnOpenLookup_Click" || lstrMethoName === "btnOpen_Click") {
            parentNode = nsCommon.GetDataItemFromDivID(ActiveDivID);
            if (parentNode != undefined) {
                lblnParentViewOnly = parentNode.IsViewOnly === true;
            }
            if (navParams == null && ns.viewModel[ActiveDivID] != undefined && ns.viewModel[ActiveDivID].HeaderData != undefined && ns.viewModel[ActiveDivID].HeaderData.ButtonNavParams != undefined) {
                var lobjButtonNavDetails = ns.viewModel[ActiveDivID].HeaderData.ButtonNavParams[$(ldomSrcElement)[0].id];
                if (lobjButtonNavDetails != undefined) {
                    if (lobjButtonNavDetails[nsConstants.istrAccessDenied] != undefined || lobjButtonNavDetails[nsConstants.istrError] != undefined) {
                        return false;
                    }
                    else {
                        navParams = lobjButtonNavDetails[nsConstants.istrNavParams];
                    }
                }
            }
        }
    }
    if (lblnViewOnlyForm === true || lblnParentViewOnly === true) {
        lActivityCopyParams = {};
        lActivityInstanceDetails = null;
    }
    var dm = {
        ActivityCopyParams: lActivityCopyParams,
        NavigationParams: lstrFirstID,
        ActivityInstanceDetails: lActivityInstanceDetails,
        NavParams: navParams,
        IsOpenInDialog: lblnIsOpenInDialog,
        IsViewOnlyForm: lblnViewOnlyForm,
        IsParentViewOnly: lblnParentViewOnly
    };
    if (dm.NavParams == undefined)
        delete dm["NavParams"];
    var larrCodes = ns.GetCodesValuesData(ns.viewModel.currentModel, ns.viewModel.currentForm);
    if (larrCodes != undefined && larrCodes.length > 0) {
        dm["LoadSourceCodeValues"] = larrCodes;
    }
    if (ns.SenderID === "FromMenu" || nsCommon.sessionGet("ChangedDetailsDataByRefresh") === "true") {
        dm["ChangedDetailsDataByRefresh"] = true;
    }
    var lstrFormId = nsCommon.GetProperFormName(ns.viewModel.currentForm);
    var lobjKnowtionData;
    if (ns.iblnHasKnowtionSearch === true) {
        lobjKnowtionData = nsCommon.GetKnowtionData(lstrFormId);
        if (lobjKnowtionData == null) {
            dm["GetKnowtionSearchData"] = true;
            if (ns.iblnKnowtionCalled === true) {
                dm["KnowtionSearchCalled"] = true;
                ns.iblnKnowtionCalled = false;
            }
        }
    }
    nsCommon.sessionRemove("ChangedDetailsDataByRefresh");
    if (nsCommon.sessionGet("FormMenuNavParams") != null) {
        dm["MenuNavParams"] = nsCommon.sessionGet("FormMenuNavParams");
        nsCommon.sessionRemove("FormMenuNavParams");
    }
    var lobjAjaxData = { action: ["GetFormForOpen?astrFormID=", lstrFormId].join(''), "param": dm, PrevActiveForm: ActiveDivID, ActiveForm: ns.viewModel.currentModel, SrcElement: ns.viewModel.srcElement };
    var lobjDeferred = nsCommon.GetAjaxRequest(lobjAjaxData);
    if (lobjKnowtionData != undefined) {
        nsCommon.BindKnowtionForm(lstrFormId);
    }
    return lobjDeferred;
}


MVVMGlobal.OpenFormOnLeft = function (dataItem) {
    ns.activityStart();
    ns.isRightSideForm = false;
    var lblnFormExist = MVVMGlobal.isFormAlreadyExistinDom(dataItem.divID);
    if (dataItem.divID.indexOf(nsConstants.LOOKUP) > 0)
        lblnFormExist = true;
    var lblnIsNewForm = false;
    if ((ns.viewModel[dataItem.divID] != undefined && ns.viewModel[dataItem.divID].ExtraInfoFields != undefined)) {
        lblnIsNewForm = ns.viewModel[dataItem.divID].ExtraInfoFields["IsNewForm"] == nsConstants.TRUE;
    }
    ns.iblnOpenRefreshedForm = false;

    if (ns.DirtyData[dataItem.divID] || nsUserFunctions.BindingBpmScreen) {
        ns.iblnOpenRefreshedForm = false;
    }
    else {
        ns.iblnOpenRefreshedForm = true;
    }

    if (ns.iblnOpenRefreshedForm && !lblnIsNewForm && !ns.blnDontUpdateUrl)
        lblnFormExist = false;
    if (ns.FormOpenedOnLeft !== undefined) {
        dataItem.previousForm = ns.FormOpenedOnLeft.divID;
        MVVMGlobal.hideDiv([nsConstants.HASH, ns.FormOpenedOnLeft.divID].join(''));
        if (ns.FormOpenedOnLeft.divID.indexOf(nsConstants.MAINTENANCE) > 0 && !lblnFormExist
            && ns.FormOpenedOnLeft.divID != dataItem.divID) {
            ns.destroyAll(ns.FormOpenedOnLeft.divID);
        }
    }
    if (dataItem.formID.indexOf(nsConstants.LOOKUP) > 0) {
        if ($([nsConstants.CONTENT_SPLITTER_SELECTOR, nsConstants.SPACE_HASH, dataItem.divID].join('')).length === 0)
            $(nsConstants.CONTENT_SPLITTER_SELECTOR).append($([nsConstants.HASH, dataItem.divID].join('')));
        dataItem.side = "left";
        ns.FormOpenedOnLeft = dataItem;
        MVVMGlobal.showDiv([nsConstants.HASH, dataItem.divID].join(''));
        ns.activeDivID = dataItem.divID;
        ns.viewModel.currentForm = dataItem.formID;
        ns.viewModel.currentModel = dataItem.divID;
        if (ns.iblnHasKnowtionSearch) {
            if (nsCenterLeftRefresh.istrCenterMiddleCurrentForm != undefined && nsCenterLeftRefresh.istrCenterMiddleCurrentForm != "") {
                nsCenterLeftRefresh.istrCenterMiddleCurrentForm = ns.viewModel.currentForm;
            }
            if (nsCenterLeftRefresh.istrCenterMiddleCurrentModel != undefined && nsCenterLeftRefresh.istrCenterMiddleCurrentModel != "") {
                nsCenterLeftRefresh.istrCenterMiddleCurrentModel = ns.viewModel.currentModel;
            }
            var lstrFormId = nsCommon.GetProperFormName(dataItem.formID);
            if (nsCommon.sessionGet("FMknowtionSearchFormId") !== lstrFormId) {
                nsCommon.BindKnowtionForm(lstrFormId);
            }
            ns.viewModel.currentForm = dataItem.formID;
            ns.viewModel.currentModel = dataItem.divID;
        }
        MVVMGlobal.UpdateUrl(dataItem.formID, 0);
        ns.activityComplete();
    }
    else {
        var data = nsCommon.sessionGet(dataItem.modelID);
        ns.viewModel.currentModel = dataItem.modelID;
        dataItem.side = "left";
        ns.FormOpenedOnLeft = dataItem;
        ns.activeDivID = dataItem.divID;
        ns.viewModel.currentForm = dataItem.formID;
        MVVMGlobal.UpdateUrl(dataItem.formID, dataItem.PrimaryKey);
        if (lblnFormExist) {
            if (ns.iblnHasKnowtionSearch) {
                if (nsCenterLeftRefresh.istrCenterMiddleCurrentForm != undefined && nsCenterLeftRefresh.istrCenterMiddleCurrentForm != "") {
                    nsCenterLeftRefresh.istrCenterMiddleCurrentForm = ns.viewModel.currentForm;
                }
                if (nsCenterLeftRefresh.istrCenterMiddleCurrentModel != undefined && nsCenterLeftRefresh.istrCenterMiddleCurrentModel != "") {
                    nsCenterLeftRefresh.istrCenterMiddleCurrentModel = ns.viewModel.currentModel;
                }
                var lstrFormId = nsCommon.GetProperFormName(dataItem.formID);
                if (nsCommon.sessionGet("FMknowtionSearchFormId") !== lstrFormId) {
                    nsCommon.BindKnowtionForm(lstrFormId);
                }
                ns.viewModel.currentForm = dataItem.formID;
                ns.viewModel.currentModel = dataItem.divID;
            }
            if ($([nsConstants.CONTENT_SPLITTER_SELECTOR, nsConstants.SPACE_HASH, dataItem.divID].join('')).length === 0) {
                $(nsConstants.CONTENT_SPLITTER_SELECTOR).append($([nsConstants.HASH, dataItem.divID].join('')));
            }
            MVVMGlobal.showDiv([nsConstants.CONTENT_SPLITTER_SELECTOR, nsConstants.SPACE_HASH, dataItem.divID].join(''));
        }
        else {
            ns.viewModel.currentModel = dataItem.modelID;
            ns.viewModel.currentForm = dataItem.formID;
            if (nsCommon.NeedToRefresh(dataItem.modelID) || ns.iblnOpenRefreshedForm) {
                data = null;
            }
            if (data === null) {
                ns.viewModel.currentModel = dataItem.modelID;
                nsCommon.sessionSet([dataItem.modelID, "_Params"].join(''), dataItem.PrimaryKey);
                nsEvents.raiseEvent(ns.getData);
            }
            else {
                ns.displayActivity(true);
                ns.blnLoading = true;
                ns.bindFormData(data);
                ns.blnLoading = false;
                ns.displayActivity(false);
                ns.activityComplete();
            }
        }
    }
    if (dataItem != undefined && dataItem.IsOpenInDialog !== true)
        MVVMGlobal.LoadBreadCrums(dataItem.divID);
}



MVVMGlobal.showDiv = function (astrDivID, aobjDataItem, adomDiv, astrFormContainerID) {

    var tempArr = astrDivID.split(nsConstants.HASH);
    var DivTorefresh = tempArr[tempArr.length - 1];
    var CanHideLoader = true;
    var ldomDiv = adomDiv;
    if (adomDiv == undefined) {
        var ldomCorDiv = document.getElementById(DivTorefresh);
        ldomDiv = $(ldomCorDiv);
    }
    if (nsWizard.FinishClickedToNavigate === true) {
        nsWizard.FinishClickedToNavigate = false;
    }
    MVVMGlobal.setAuditInformation(DivTorefresh, ldomDiv);
    setTimeout(function () {
        nsCommon.PopulateTabNavigator(DivTorefresh, ldomDiv);
    }, 0);
    var fnBeforeShowDiv = nsUserFunctions[nsConstants.USER_FUNCTION_BEFORE_SHOW_DIV];
    if (typeof fnBeforeShowDiv === 'function') {
        var objBeforeContext = {
            activeDivID: DivTorefresh,
            astrDivID: astrDivID,
            idomActiveDiv: ldomDiv
        };
        var e = {};
        e.context = objBeforeContext;
        var lblnShowDiv = fnBeforeShowDiv(e);
        if (lblnShowDiv === true) {
            return;
        }
    }
    var lstrFormName = nsCommon.GetProperFormName(DivTorefresh);
    if (lstrFormName.indexOf("rpt") == 0 || lstrFormName.indexOf("cor") == 0 || lstrFormName == nsConstants.REPORT_CLIENT_MVVM_RPT_DIV)
        ns.setSenderData("", lstrFormName, (ns.Templates[DivTorefresh] != undefined ? ns.Templates[DivTorefresh].SenderKey : ""));
    else
        ns.setSenderData("", lstrFormName, (ns.viewModel[DivTorefresh] != undefined ? ns.viewModel[DivTorefresh].SenderKey : ""));
    if (nsCommon.NeedToRefresh(DivTorefresh) || (ns.iblnOpenRefreshedForm && DivTorefresh.indexOf(nsConstants.LOOKUP) > 0 && (!ns.viewModel.FromMenu))) {
        CanHideLoader = false;
        if (DivTorefresh.indexOf(nsConstants.LOOKUP) > 0) {
            if (ns.viewModel[DivTorefresh] != undefined && ns.viewModel[DivTorefresh].SenderKey != undefined) {
                var btn;
                var lstrSearchButton = ldomDiv[0].getAttribute("SearchButtonId");
                if (lstrSearchButton != undefined && lstrSearchButton != "" && ldomDiv[0].querySelector([nsConstants.HASH, lstrSearchButton].join("")) != null) {
                    btn = ldomDiv[0].querySelector([nsConstants.HASH, lstrSearchButton].join(""));
                }
                else {
                    btn = ldomDiv[0].querySelector("input[value=Search]");
                }
                ns.lblnCanSetLookupParams = true;
                ldomDiv[0].style.display = "block";
                ns.iblnTriggeredSearch = true;
                $(btn).trigger("click");
            }
        }
        else {
            if (ns.viewModel[DivTorefresh].ExtraInfoFields.IsNewForm == nsConstants.TRUE) {
                CanHideLoader = true;
                MVVMGlobal.GetIntoNewMode(true);
            }
            else {
                if (DivTorefresh.indexOf("wizard") > 0 || DivTorefresh.indexOf("Wizard") > 0) {
                    var lobjWizard = $([nsConstants.HASH, DivTorefresh, nsConstants.SPACE, nsConstants.DIV_SW_MAIN].join('')).data(nsConstants.SMART_WIZARD);
                    if (lobjWizard != null) {
                        var ldivCurrenStep = $(lobjWizard.elmStepContainer.find(".content")[lobjWizard.curStepIdx]);
                        if (ldivCurrenStep != null) {
                            var btn = ldivCurrenStep[0].querySelector("input[value=Reset]");
                            if (btn == null || MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) !== "btnCancel_Click")
                                btn = ldivCurrenStep[0].querySelector("input[value='Refresh']");
                            if (btn == null || MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) !== "btnCancel_Click")
                                btn = ldivCurrenStep[0].querySelector("input[value='Cancel']");
                            if (btn != null && MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) === "btnCancel_Click") {
                                ns.iblnIsRefreshClickedByCode = true;
                                $(btn).trigger("click");
                            }
                            else {
                                CanHideLoader = true;
                                console.log("No refresh button added on form");
                            }
                        }
                    }
                }
                else {
                    var btn = ldomDiv[0].querySelector("input[value=Reset]");
                    if (btn == null || MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) !== "btnCancel_Click")
                        btn = ldomDiv[0].querySelector("input[value='Refresh']");
                    if (btn == null || MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) !== "btnCancel_Click")
                        btn = ldomDiv[0].querySelector("input[value='Cancel']");
                    if (btn != null && MVVMGlobal.GetControlAttribute(btn, "sfwMethodName", DivTorefresh) === "btnCancel_Click") {
                        ns.iblnIsRefreshClickedByCode = true;
                        $(btn).trigger("click");
                    }
                    else {
                        CanHideLoader = true;
                        console.log("No refresh button added on form");
                    }
                }
            }
        }
        delete ns.arrNeedToRefresh[DivTorefresh];
        nsCommon.UpdateParentRefreshListInSession();
    }
    var FormContainerID = astrFormContainerID || "";
    var ldomCrumDivSelector = document.getElementById(nsConstants.CRUM_DIV_SELECTOR.replace("#", ""));
    var ldomClosestgroup = nsCommon.jQClosest(ldomDiv[0], function (el) {
        return el.tagName === "DIV" && (el.getAttribute("role") === "group" || (el.id != undefined && el.id.indexOf("_" + nsConstants.MAINTENANCE_FORM_HOLDER) !== -1));
    });
    if (ldomClosestgroup != null) {
        FormContainerID = [nsConstants.HASH, ldomClosestgroup.id].join('');
    }
    if (FormContainerID != undefined && FormContainerID != "") {
        if (FormContainerID !== nsConstants.RPT_HOLDER_SELECTOR) {
            var ldomFormContainerID = $(FormContainerID);
            ldomFormContainerID.find("div[id^='wfm']").hide();
            ldomFormContainerID.find(nsConstants.FORMCONTAINER_SELECTOR).hide();
        }
        else {
            $(nsConstants.CONTENT_SPLITTER_SELECTOR).find("div[id^='wfm']").hide();
            ldomCrumDivSelector.style.display = 'none';
        }
    }
    if (astrDivID.indexOf(nsConstants.LOOKUP) > 0) {
        $([astrDivID, "_parent"].join('')).show();
        $(nsConstants.LOOKUP_NAME_SELECTOR).show();
        var dataitem = aobjDataItem;
        if (dataitem == undefined) {
            dataitem = nsCommon.GetDataItemFromDivID(DivTorefresh);
        }
        if ((!ns.blnUseSlideoutForLookup || ns.iblnHideBreadCrumForSlideOutLookup) && dataitem.parentNode() == null) {
            ldomCrumDivSelector.style.display = 'none';
        }
        else {
            MVVMGlobal.LoadBreadCrums(DivTorefresh);
        }
    }
    else if (FormContainerID != undefined && FormContainerID.indexOf(nsConstants.MAINTENANCE_FORM_HOLDER) == -1) {
        if (!ns.blnUseSlideoutForLookup && FormContainerID !== nsConstants.RPT_HOLDER_SELECTOR) {
            var fnAddToLookUpNames = nsUserFunctions["AddToLookUpNames"];
            var lblnShowLookupNames = false;
            if (typeof fnAddToLookUpNames === 'function') {
                var Context = {
                    activeDivID: ldomDiv.length > 0 ? ldomDiv[0].id : DivTorefresh,
                    idomActiveDiv: ldomDiv
                };
                var e = {};
                e.context = Context;
                lblnShowLookupNames = fnAddToLookUpNames(e);
            }
            if (lblnShowLookupNames) {
                $(nsConstants.LOOKUP_NAME_SELECTOR).show();
            }
            else {
                $(nsConstants.LOOKUP_NAME_SELECTOR).hide();
            }
            var fnDisplayBreadCrums = nsUserFunctions["DisplayBreadCrums"];
            var lblnDisplayBreadCrums = true;
            if (typeof fnDisplayBreadCrums === 'function') {
                var Context = {
                    activeDivID: ldomDiv.length > 0 ? ldomDiv[0].id : DivTorefresh,
                    idomActiveDiv: ldomDiv
                };
                var e = {};
                e.context = Context;
                lblnDisplayBreadCrums = fnDisplayBreadCrums(e);
            }
            if (lblnDisplayBreadCrums) {
                ldomCrumDivSelector.style.display = 'block';
            }
            else {
                ldomCrumDivSelector.style.display = 'none';
            }
        }
    }
    if (ns.iblnShowViewEditForOpenButton === true && astrDivID.indexOf(nsConstants.UNDERSCORE_RETRIEVE) <= 0 && astrDivID.indexOf(nsConstants.USER_PREFERENCES_MAINTENANCE) < 0 && ldomDiv.length) {
        nsCommon.AddViewEditButtonAttribute(ldomDiv, ldomDiv[0].id);
    }
    else if (ns.iblnShowViewEditForOpenButton !== true && ldomDiv.length > 0) {
        nsCommon.RemoveViewEditButtonAttribute(ldomDiv, ldomDiv[0].id);
    }
    var lstrFormDivID = (ldomDiv.length > 0 ? ldomDiv[0].id : DivTorefresh);
    if (lstrFormDivID.indexOf(nsConstants.UNDERSCORE_RETRIEVE) <= 0 && !(lstrFormDivID.indexOf(nsConstants.LOOKUP) > 0 && ns.blnUseSlideoutForLookup)
        && !(FormContainerID != undefined && FormContainerID.indexOf(nsConstants.MAINTENANCE_FORM_HOLDER) > -1)) {
        nsCommon.ApplyFreezeCrumToolbar(ldomDiv, lstrFormDivID);
    }
    if (!ns.iblnRestoredScrollPostion && !nsCommon.checkForNonMaintenanceForm(lstrFormDivID)) {
        var SessionStoredInfo = ns.GetSessionStoredInfo(lstrFormDivID);
        if (SessionStoredInfo != null && SessionStoredInfo.scrollTop != undefined) {
            ns.iblnRestoredScrollPostion = true;
            var fnSetTimeout = function () {
                $(nsConstants.SCROLL_DIV).scrollTop(SessionStoredInfo.scrollTop);
            };
            setTimeout(fnSetTimeout, 200);
        }
    }
    var lobjWidgetControls = nsCommon.GetWidgetControlsByDivID(lstrFormDivID);
    if (lobjWidgetControls != undefined) {
        var lobjAllPanles = _.filter(lobjWidgetControls, function (wid) {
            return wid instanceof MVVM.Controls.Panel;
        });
        if (lobjAllPanles.length) {
            for (var i = 0; i < lobjAllPanles.length; i++) {
                var lobjPanel = lobjAllPanles[i];
                if (lobjPanel.iblnAutoRefresh && lobjPanel.iintSetIntervalID == -1) {
                    lobjPanel.registerRefreshEvent();
                }
            }
        }
    }
    ldomDiv.show();
    if (ns.iblnPreventDragDropForInputs === true) {
        NeoGrid.iblnPreventDragDropForInputs = ns.iblnPreventDragDropForInputs;
        ldomDiv.find("input, select, textarea").off('.neoDragEvents', neo.preventDragDrop)
            .on('dragenter.neoDragEvents', neo.preventDragDrop)
            .on('dragover.neoDragEvents', neo.preventDragDrop)
            .on('drop.neoDragEvents', neo.preventDragDrop);
    }
    ldomDiv.find("div[id^='wfm']").show();
    var dataitem = aobjDataItem;
    if (dataitem == undefined) {
        dataitem = nsCommon.GetDataItemFromDivID(DivTorefresh);
    }
    if (dataitem != undefined) {
        var lstrTitle = dataitem.title;
        nsCommon.SetTitle(lstrTitle);
        if (!ns.blnUseSlideoutForLookup && !ns.blnIsNewFormSaved && (FormContainerID == undefined || FormContainerID.indexOf(nsConstants.MAINTENANCE_FORM_HOLDER) == -1)) {
            ns.FormOpenedOnLeft = dataitem;
        }
    }
    if (MVVM.Controls.Chart.istrChartClass != "") {
        $(astrDivID).find([".", MVVM.Controls.Chart.istrChartClass].join("")).each(function (aintIndex, adomElement) {
        });
    }
    var fn = nsUserFunctions[nsConstants.USER_FUNCTION_AFTER_SHOW_DIV];
    if (typeof fn === 'function') {
        var objAfterContext = {
            activeDivID: DivTorefresh,
            astrDivID: astrDivID,
            idomActiveDiv: ldomDiv
        };
        e = {};
        e.context = objAfterContext;
        fn(e);
    }
    if (CanHideLoader && astrDivID.indexOf(nsConstants.LOOKUP) < 0) {
        ns.displayActivity(false);
    }
    //if ($('#lblCurrTime').length > 0) {
    //    $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
    //}
}
