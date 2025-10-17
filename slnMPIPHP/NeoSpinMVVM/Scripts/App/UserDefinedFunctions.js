var nsUserFunctions = nsUserFunctions || {};
var QrdoLoadFlag = true;
nsUserFunctions = {
    // To close maintenance as popup for Validate caller
    CloseCustomPopup: function (e) {
        var lstrMaintenanceFormHolder = [e.context.activeDivID, "_", nsConstants.MAINTENANCE_FORM_HOLDER].join("");
        var lstCallerValidated = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData.lblIsCallerValidated;
        if (lstCallerValidated != undefined && lstCallerValidated === "Y") {
            ns.arrDialog[lstrMaintenanceFormHolder].close();
        }
        return false;
    },
    // Framework version 6.0.0.30.0
    ApplyUserDefinedFormat: function (e) {        
        return null;
    },

    SetLanguage: function (e) {
        // add solution side logic to get language.  
        var lstrLanguage = 'en-US';
        return lstrLanguage;
    },
    // Framework version 6.0.0.28.0
    ClearReassignTreeNode: function (e) {
        //ns.tabsTreeView.remove("wfmBpmReassignWorkMaintenance0");
        var dataitem = ns.tabsTreeView.getNodeDataByDivID("wfmBpmReassignWorkMaintenance0");
        if (dataitem != undefined)
            MVVMGlobal.RemoveForm([], dataitem);
        return true;
    },

    // Framework version 6.0.2.1
    GetChartTitle: function (e) {
        e.data 		// DomainModel Object
        e.chartId 	// Id of the chart
        return "";
    },
    //Reissue Check Temporary fix...
    FixDirtyData: function (e) {

        try {

            if (ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlReissuePaymentTypeId"] != undefined &&

                ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlReissuePaymentTypeId"] == "PTOS" &&

                ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["txtIstrRMPIDAddPayee"] != undefined)

                delete ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["txtIstrRMPIDAddPayee"];

            return true;

        }

        catch (ex) {

            return true;

        }

    },
  

    //Fix to stop displaying knowtion unsaved form on every screen.
    SetUnSavedFormIcon: function (e) {
        if (

            (e.context.activeDivID.indexOf("wfmBPMWorkflowCenterLeftMaintenance") == 0 || e.context.activeDivID.indexOf("wfmMSSLoginInternalMaintenance") == 0)

        ) {
            return false;
        }
        return true;
    },
    // FS007 BPM Related JS

    DisplayBreadCrums: function (e) {
        if ((e.context.activeDivID.indexOf("wfmBPMWorkflowCenterLeftMaintenance") == 0)) {
            //BreadCrumLoadNames(e);
            return false;
        }
        return true;
    },

    btnChangeBenefitOption_Click: function (e) {
        setTimeout(function () {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ActiveDivID = "";
            var lbtnSelf = ns.viewModel.srcElement;
            ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf);
            var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);

            if (lobjGridWidget.jsObject && lobjGridWidget.jsObject.RenderData.length > 0) {
                var ddlIintPlanID = ldomDiv.find("#ddlIintPlanID option:selected").val();
                var ddlPlanBenfitId = ldomDiv.find("#ddlPlanBenfitId option:selected");
                var txtBenefitApplicationDetailId = ldomDiv.find("#txtBenefitApplicationDetailId").val(); var chkSpousalConsent = "N";
                var ddlJointAnnunantName = ldomDiv.find("#ddlJointAnnunantName option:selected").val(); var SpousalConsent = ldomDiv.find("#chkSpousalConsent").is(':checked');
                if (SpousalConsent == true) {
                    chkSpousalConsent = "Y";
                }
                if (ldomDiv.find("#ddlPlanBenfitId").is(":visible") == false) {
                    ddlPlanBenfitId = ldomDiv.find("#ddlCascadingDropDownList option:selected");
                }
                $.each(lobjGridWidget.jsObject.RenderData, function (index, item) {
                    var BenefitApplicationDetailID = item[Object.keys(item).filter(k => k.startsWith('dt_BenefitApplicationDetailID'))[0]];
                    var PlanBenefitDescription = item[Object.keys(item).filter(k => k.startsWith('dt_PlanBenefitDescription'))[0]];

                    if (BenefitApplicationDetailID == txtBenefitApplicationDetailId && ddlPlanBenfitId.text() != "" && ddlPlanBenfitId.text() != PlanBenefitDescription) {
                        var NavigationParam = {};
                        NavigationParam["intPlan_Id"] = ddlIintPlanID;
                        NavigationParam["istrBenefitOptionValue"] = ddlPlanBenfitId.val();
                        NavigationParam["iintJointAnnuaintID"] = ddlJointAnnunantName;
                        NavigationParam["spousal_consent_flag"] = chkSpousalConsent;
                        NavigationParam["benefit_application_detail_id"] = txtBenefitApplicationDetailId;
                        var Result = nsCommon.SyncPost("ChangeBenefitOption", NavigationParam);

                        if (Result != undefined && Result.length > 0) {
                            var errorMessages = '';
                            for (var i = 0; i < Result.length; i++) {
                                errorMessages += Result[i].istrErrorMessage + '<br/>';
                            }
                            nsCommon.DispalyError(errorMessages, e.context.activeDivID);
                        }
                        else {
                            $('#' + e.context.activeDivID).find("#btnCancel").trigger("click");
                        }
                    }
                    else {
                        nsCommon.DispalyMessage("Record displayed.", e.context.activeDivID);
                    }
                });
            }
            else {
                nsCommon.DispalyMessage("Record displayed.", e.context.activeDivID);
            }
        }, 50);
    },

    CallToBuildCenterLeftForms: function (e) {
        var lstrFormID = e.context.activeDivID || "";
        var $ldomForm = e.context.adomDiv;
        if (lstrFormID.indexOf("wfmBPMWorkflowCenterLeftMaintenance") === 0 && $ldomForm.length > 0) {
            ns.DestroyFormFromDOM($ldomForm[0].id, $ldomForm);
            ns.BuildLeftForm("wfmBPMWorkflowCenterLeftMaintenance");
        }        
    },
    // END FS007 BPM Related JS
    // FS005 ECM Related JS 

    UploadDocument: function (e) {
        //Pushawart: Need to make hard error show up through javascript with the help of Amol
        
        var lstrFormName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        ns.DirtyData[e.context.activeDivID] = { HeaderData: {}, istrFormName: lstrFormName, KeysData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData = { MaintenanceData: {} };
        ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData = ns.viewModel[e.context.activeDivID].HeaderData.MaintenanceData;
        ns.DirtyData[e.context.activeDivID].KeysData = ns.viewModel[e.context.activeDivID].KeysData;
        switch (lstrFormName) {
            case "wfmMSSMEHPInsuranceApplicationWizard":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["PersonId"] = $("#" + e.context.activeDivID).find("#lblApplicantPersonId").text();
                }
                break;
            case "wfmRefundApplicationWizard":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlRefundFileDesc"] = $("select#ddlRefundFileDesc option:selected").val();
                }
                break;
            case "wfmECMUploadDocumentMaintenance":
                {
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["DocGroup"] = $("select#ddlIstrGroupValue option:selected").val();
                    ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["DocType"] = $("select#ddlDocumentType option:selected").val();
                }
                break;
        }
        

        return true;
    },
    

    

    //END FS005 ECM Related JS 

    // START - FS001 Related JS
    // We have used to cleare the text box value on wfmUserChangePasswordMaintenance
    AfterApplyingUI: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmUserChangePasswordMaintenance") === 0) {
            if ($("#" + ns.viewModel.currentModel).find("#txtOldPassword").val() !== "" && $("#" + ns.viewModel.currentModel).find("#txtIstrNewPassword").val() == "" && $("#" + ns.viewModel.currentModel).find("#txtConfirmPassword").val() == "") {
                $("#" + ns.viewModel.currentModel).find("#txtOldPassword").val("").trigger("change");
            }
        }
        // if (ns.viewModel.currentModel.indexOf("wfmFileHdrLookup") >= 0) {
        //    var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        //    if (ldomDiv.find("#ddlFileId").length >= 1) {
        //        ldomDiv.find("#ddlFileId").prepend($('<option></option>').val('All').html('All').attr("selected", "selected"));
        //    }
        //}
        if (ns.viewModel.currentForm.indexOf("Lookup") > 0) {
            nsCommon.DispalyMessage(" Msg ID : 5 [ Please enter search criteria and press SEARCH. ]", e.context.activeDivID);

            $("#" + ns.viewModel.currentModel).find('.hasDatepicker').on('dragstart', function (event) {
                $(this).val("");
            });
        } 
      
    },
    showDivCallBack: function (e) {
      var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

        //Application level refresh flag
        if (ns.viewModel.FromMenu) {
            ns.viewModel.FromMenu = false;
        }
        if (ns.viewModel.currentForm.indexOf("wfmBPMMyBasketMaintenance") == 0 && ns.viewModel.previousForm.indexOf("wfmBPMMyBasketMaintenance") !== 0) {
            if (ns.viewModel.srcElement != undefined && ns.viewModel.srcElement["id"] != undefined && ns.viewModel.srcElement["id"] == "btnSearchActivities") {
                $("#custom-loader").hide();
            }
            var srcElement = "";
            if (ns.viewModel.srcElement != undefined && ns.viewModel.srcElement["id"] != undefined) {
                srcElement = ns.viewModel.srcElement["id"];
            }
            var srcElementMyBasket = $("#" + ns.viewModel.currentModel).find("#" + srcElement);

            //if (srcElementMyBasket != undefined && srcElementMyBasket.length == 0) {
            var btnSearchActivities = $("#" + ns.viewModel.currentModel).find("#btnSearchActivities");
            if (btnSearchActivities != undefined && btnSearchActivities.length > 0 && btnSearchActivities[0]["id"] != srcElement) {
                    $("#custom-loader").show();
                    $("#" + ns.viewModel.currentModel).find("#btnSearchActivities").trigger("click");
                }
            //}
        }
        //setTimeout(function () {
        //    if (ns.viewModel.currentModel.indexOf("wfmBPMMyBasketMaintenance") == 0) {
        //    $("#" + ns.viewModel.currentModel).find(".IsFlagRework").each(function (item) {
        //        if ($(this).text() == 'Y') {
        //            $(this).closest('tr').addClass('cell-red-color');
        //        }
        //    });
        //}
        //}, 100);

        if (e.context.astrDivID != undefined && e.context.astrDivID.indexOf("Lookup") > 0) {
            var ldivmasterspanBpmActivityInstanceDetailhtml = $('#spanBpmActivityInstanceDetails');
            if (ldivmasterspanBpmActivityInstanceDetailhtml && ldivmasterspanBpmActivityInstanceDetailhtml.length > 0) {

                setTimeout(function () {
                    var ldivToolbarContainer = $('.s-freezed-crumtoolbar-Container');
                    if (ldivToolbarContainer.hasClass('hideByFreeze')) {
                        ldivToolbarContainer.removeClass('hideByFreeze');
                    }
                }, 30, e.context.idomActiveDiv);
            }
        }
        else {
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
            ldomDiv.find('#spanBpmActivityInstanceDetails').remove();
            var data = ns.viewModel[ns.viewModel.currentModel];
            setTimeout(function (data, formDiv) {
                if (data && data != undefined && data.OtherData != undefined && data.OtherData["ShowActivityInstanceDetails"] != undefined && data.OtherData["ShowActivityInstanceDetails"]) {
                    if (data.OtherData["ActivityDetailsNavParams"] != undefined) {
                        var lstrdivBpmActivityInstanceDetails = '#divBpmActivityInstanceDetails';
                        var ldivBpmActivityInstanceDetails = formDiv.find(lstrdivBpmActivityInstanceDetails);
                        formDiv.find('#spanBpmActivityInstanceDetails').remove();
                        ldivBpmActivityInstanceDetails = $(ldivBpmActivityInstanceDetails);
                        ldivBpmActivityInstanceDetails.html(
                            [
                                '<span id="spanBpmActivityInstanceDetails" >',
                                '<strong>', 'Process : ', '</strong> ',
                                data.OtherData["ProcessName"],
                                ' <strong>' + ' Activity : ' + '</strong> ',
                                data.OtherData["ActivityName"],
                                '</a > </span>'].join(''));
                        ldivBpmActivityInstanceDetails.css("text-align", "right"); ldivBpmActivityInstanceDetails.css("text-align", "right");

                        if ((ns.viewModel.srcElement) && (ns.viewModel.srcElement.id == "btnCheckout" || ns.viewModel.srcElement.id == "btnInProcess2")) {
                            nsCommon.DispalyMessage("[ Record displayed. Please make changes and press SAVE.]", e.context.activeDivID);
                        }
                    }
                }
            }, 30, data, e.context.idomActiveDiv);
        }

        if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountACHDetailsWMaintenance") >= 0) {           
                var paymentType = ldomDiv.find("#ddlPaymentMethod").val();              
                if (paymentType == "CHK")
                {
                    ldomDiv.find("#lblIstrInformation").show();
                }
                else {
                    ldomDiv.find("#lblIstrInformation").hide();
                }
        }
        if (ns.viewModel.currentModel.indexOf("wfmRetirementApplicationMaintenance") >= 0) {
            CheckChange(false);
        }
        if (nsConstants.ARR_TOOLBAR_BUTTONS != undefined && nsConstants.ARR_TOOLBAR_BUTTONS != null && nsConstants.ARR_TOOLBAR_BUTTONS.length > 0) {
            nsConstants.ARR_TOOLBAR_BUTTONS.push("btnGridViewUpdate_Click");
        }

        //FW Upgrade #PIR 34315 - LOB- Total field section not fit the amount to the correct position, having alignment issue
        if (ns.viewModel.currentModel.indexOf("wfmAnnualBenefitSummaryOverviewMaintenance") >= 0) {
            objAgent = navigator.userAgent;
            if ( objAgent.indexOf('Edg') != -1 || objAgent.indexOf('Firefox') != -1 ) {
                ldomDiv.find(".tblPersWorkHistMPI").addClass('setEdgeFirefoxPersWorkHistMPI');
            }
        }

        setTimeout(function () {
            if (ns.viewModel.currentModel.indexOf("wfmPaymentReissueDetailMaintenance") >= 0 && ns.viewModel && ns.viewModel.srcElement && ns.viewModel.srcElement.id =="btnAdd") {
                var lrowGridRecord = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvPaymentReissueDetail").jsObject.RenderData;;
                var ldomDiv = $('#' + ns.viewModel.currentModel);
                $.each(lrowGridRecord, function (index, item) {
                    var result = item[Object.keys(item).filter(k => k.startsWith('dt_PaymentReissueDetailID_4_0'))[0]];
                    if (result == "") {
                        ldomDiv.find("#Table_GridTable_grvPaymentReissueDetail > tbody tr[rowindex=" + index + "] > td > input[id ^= 'btnDelete']").hide();
                    }
                })
            }
            if (ns.viewModel.currentModel.indexOf("wfmJobHeaderLookup" == 0) || ns.viewModel.currentModel.indexOf("wfmProcessLogLookup" == 0)
                || ns.viewModel.currentModel.indexOf("wfmUserActivityLogLookup" == 0) || ns.viewModel.currentModel.indexOf("wfmUserActivityLogMaintenance" == 0)) {
                $("#" + ns.viewModel.currentModel).find("input[type=text][class='s-textbox hasDatepicker timecontrol'][sfwextendtime]").each(function (e) {
                    $(this).removeAttr("disabled");
                    $(this).removeClass("hasDatepicker timecontrol");
                    $(this).siblings("img").css("display", "none");
                });
            }
            if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountDeductionMaintenance") >= 0 && ns.viewModel && ns.viewModel.srcElement && (ns.viewModel.srcElement.id == "btnAdd" || ns.viewModel.srcElement.id == "btnUpdate")) {
                var lrowGridRecord = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvBusPayeeAccountDeduction").jsObject.RenderData;;
                var ldomDiv = $('#' + ns.viewModel.currentModel);
                $.each(lrowGridRecord, function (index, item) {
                    var result = item[Object.keys(item).filter(k => k.startsWith('dt_PayeeAccountDeductionID_0_0'))[0]];
                    if (result == "") {
                        ldomDiv.find("#Table_GridTable_grvBusPayeeAccountDeduction > tbody tr[rowindex=" + index + "] > td > input[id ^= 'btnDeleteDeduction']").hide();
                    }
                })
            }
            if (ns.viewModel.currentModel.indexOf("wfmQDROApplicationMaintenance") >= 0) {
                QrdoLoadFlag = true;
                
                var ldomDiv = $('#' + ns.viewModel.currentModel);
                var ApplicationId = ldomDiv.find("#lblDroApplicationId");
                if (ApplicationId !=null && ApplicationId.text() =="") {
                    if (ldomDiv.find('#chkEligibleForContinuanceFlag').is(':checked') == true) {
                        ldomDiv.find("#" + "chkEligibleForContinuanceFlag").attr("checked", false).trigger("change");
                        ldomDiv.find("#" + "chkEligibleForContinuanceFlag").attr("checked", true).trigger("change");
                    }
                    else {
                        ldomDiv.find("#" + "chkEligibleForContinuanceFlag").attr("checked", true).trigger("change");
                        ldomDiv.find("#" + "chkEligibleForContinuanceFlag").attr("checked", false).trigger("change");
                    }
                }
            }
            if (ns.viewModel.currentModel.indexOf("wfmDeathNotificationMaintenance") >= 0) {
                var ldomDiv = $('#' + ns.viewModel.currentModel);
                if (ldomDiv.find("#txtDeathNotificationReceivedDate").is(":visible") == false) {
                    ldomDiv.find("#lblrdfor_txtDeathNotificationReceivedDate").removeClass("hasDatepicker");
                }
                if (ldomDiv.find("#txtDateOfDeath").is(":visible") == false) {
                    ldomDiv.find("#lblrdfor_txtDateOfDeath").removeClass("hasDatepicker");
                }
            }
        }, 50);
        
        if (ns.viewModel.currentModel.indexOf("wfmPersonAddressMaintenance") >= 0) {
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
            ldomDiv.find("#cblCheckBoxList_0").attr("disabled", true);
        }
        if (ns.viewModel.currentModel.indexOf("wfmSupervisorDashboardMaintenance") >= 0) {
            setTimeout(function (e) {
                ldomDiv.find('#ddlDropDownList').prop('selectedIndex', 1);
            }, 0);
        }
    },

    //logoutSesssion: function () {
    //    var lstrLogoutUrl = ["../", ns.SiteName, "/account/login"].join('');
    //    window.location.href = lstrLogoutUrl
    //},

    //logoutApp: function () {
    //    var lstrLogoutUrl = ["../", ns.SiteName, "/account/login"].join('');
    //    window.location.href = lstrLogoutUrl
    //},

    SaveFileHdrClick: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (ldomDiv.find("#idStatusValue").is(":checked") && ldomDiv.find("#idStatusValue1").is(":checked") == false) {
            ldomDiv.find("#idStatusValue").prop('checked', false).trigger("change");
        }
        else if (ldomDiv.find("#lblStatusDescription").text() == "Uploaded" && ldomDiv.find("#idStatusValue1").is(":checked") == true) {
            ldomDiv.find("#idStatusValue").prop('checked', false).trigger("change");
            ldomDiv.find("#btnRefreshData").trigger("click");
            return false;
        }
        else if (ldomDiv.find("#idStatusValue").is(":checked") == false && ldomDiv.find("#idStatusValue1").is(":checked") == false) {
            ldomDiv.find("#idStatusValue").prop('checked', false).trigger("change");
        }
        return true;
    },
    PrintCurrentPage: function (e) {
        $("#" + ns.viewModel.currentModel).jqprint();
    },
 

    OnSaveEndDateClick: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var ActiveDivID = e.context != undefined && e.context.activeDivID != undefined ? e.context.activeDivID : nsCommon.GetActiveDivId(ns.viewModel.srcElement);

        if (ns.DirtyData[ActiveDivID] === undefined) {
            ldomDiv.find("#txtEffectiveEndDate").hide();
            ldomDiv.find("img[class='ui-datepicker-trigger']").hide();
            nsCommon.DispalyMessage("No Changes To Save.", e.context.activeDivID);
            return false;
        }
        return true;
    },
    OnContinueClick: function (e) {
        if (confirm('Please confirm and click Ok to Create Application, Calculation and Payee Account...')) {
            return true;
        }
        else {
            e.preventDefault()
            return false;
        }
    },
    PayeeAccountAddClick: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountACHDetailsMaintenance") >= 0) {
            ldomDiv.find('#chkPreNoteFlag').prop('checked', true).trigger('change');          
            var txtAchStartDate = ldomDiv.find("#txtAchStartDate");
            if (txtAchStartDate != undefined && (txtAchStartDate.val() == "" )) {
                txtAchStartDate.val(GetCurrentDateTime(new Date())).trigger("change");
            }

            var RoutingNumber = ldomDiv.find("#txtIintRoutingNumber").val();
            if (RoutingNumber != undefined && RoutingNumber != null && RoutingNumber != "") {
                var Params = {
                    "astrRoutingNumber": RoutingNumber
                };
                var result = nsCommon.SyncPost("GetBankName", Params, "POST");
                if (result !== undefined || result !== null || result !== "") {
                    ldomDiv.find("#txtBankName").val(result).trigger('change');
                }
            }
        }
       else if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountWireDetailMaintenance") === 0) {
            if (ldomDiv.find("#txtWireStartDate").val() == "") {
                ldomDiv.find("#txtWireStartDate").val(GetCurrentDateTime(new Date())).trigger("change");
            }
            if (ldomDiv.find("#chkCallBackFlag").is(":checked")) {
                ldomDiv.find("#txtCallBackCompletionDate").val(GetCurrentDateTime(new Date())).trigger("change");
            }
        }
        else if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountMaintenance") === 0) {
            ldomDiv.find("#txtPaybackCheckPostedDate").val(GetCurrentDateTime(new Date())).trigger("change");
        }
        else if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountTaxwithholdingMaintenance") === 0) {
            var ddlTaxIdentifier = ldomDiv.find("#ddlTaxIdentifierValue option:selected").val();
            var ddlBenefitdistributiontypevalue = ldomDiv.find("#ddlBenefitDistributionTypeValue1 option:selected").val();
            var ddlTaxOptionValue11 = ldomDiv.find("#ddlTaxOptionValue11 option:selected").val();
            if (ddlTaxIdentifier == "FDRL" && ddlBenefitdistributiontypevalue == "MNBF") {
                ldomDiv.find("#txtAdditionalTaxAmount1,#txtTaxPercentage,#txtTaxAllowance").val("0").trigger("change");
            }
            if (ddlTaxIdentifier != "FDRL") {
                ldomDiv.find("#txtStep2B,#txtStep3Amount,#txtStep4a,#txtStep4b,#txtStep4c").val("0").trigger("change");
            }
            if (ddlTaxIdentifier == "VAST") {
                ldomDiv.find("#txtTaxAllowance").val("0").trigger("change");
                if (ddlTaxOptionValue11 == "STST" || ddlTaxOptionValue11 == "NSTX") {
                    ldomDiv.find("#txtVoluntary_Withholding,#txtAdditionalTaxAmount1").val("0").trigger("change");
                }
            }
        }
        return true;
    },
    BenefitApplicationDetailAddClick: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmDeathPreRetirementMaintenance") === 0) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var ddlIstrSurvivorTypeValue = ldomDiv.find("#ddlIstrSurvivorTypeValue").val();
            ldomDiv.find("#ddlIstrSurvivorTypeValue").val(ddlIstrSurvivorTypeValue).trigger("change");
            var ddlCascadingDropDownList = ldomDiv.find("#ddlCascadingDropDownList").val();
            var ddlPlanBenfitId = ldomDiv.find("#ddlPlanBenfitId").val();
            if ((ddlCascadingDropDownList != undefined && ddlCascadingDropDownList != null && ddlCascadingDropDownList != "")
                && (ddlPlanBenfitId != undefined && ddlPlanBenfitId != null && ddlPlanBenfitId == "")) {
                ldomDiv.find("#ddlPlanBenfitId").val(ddlCascadingDropDownList).trigger("change");
            }
        }
        return true;
    },
    fnChangeBenefitoptionvalue: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find("#ddlPlanBenfitId").is(":visible") == false && ns.DirtyData[e.context.activeDivID].HeaderData != undefined &&
            ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData != undefined)
            delete ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["ddlPlanBenfitId"];
        return true;
    },
    GetLaunchImageViewer: function (e) {
        var ssn;
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmMyBasketMaintenance") === 0) {
            var lobjBtnInfo = nsCommon.GetEventInfo(e);
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lobjBtnInfo.ActiveDivID, lobjBtnInfo.RelatedGrid);
            var data = lobjGridWidget.getRowByIndex(lobjBtnInfo.lintSelectedIndex);
            for (var dataField in data) {
                if (dataField.indexOf("SSN") > 0) {
                    ssn = data[dataField];
                }
            }
        }
        else if (e.srcElement.id == "btnCLOpen1") {
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvUserAssignedActivities");
            var data = lobjGridWidget.getRowByIndex(e.srcElement.attributes.rowindex.value);
            var LoggedInUserIsVIP = $("#lblLoggedInUserIsVIP").val();
            for (var dataField in data) {
                if (dataField.indexOf("SSN") > 0) {
                    ssn = data[dataField];
                }
                if (LoggedInUserIsVIP != "" && LoggedInUserIsVIP != "VIPAccessUser") {
                    if (dataField.indexOf("RelativeVipFlag") > 0) {
                        if (data[dataField] != "" && data[dataField] == "Y") {
                            $("#btnjQueryCancel").click(function (e) {
                                $("#overlay").hide();
                                $("#dialog").fadeOut(300);
                                e.preventDefault();
                            });
                            $("#overlay").show();
                            $("#dialog").fadeIn(300);
                            $("#overlay").off("click");
                            return false;
                        }
                    }
                }
            }
        }
        else if (e.srcElement.id == "btnCenterLeftCheckout" || ns.viewModel.currentModel.indexOf("wfmWorkflowMaintenance") === 0) {
            ssn = ldomDiv.find("#lblssn").text();
        }
        var ImageViewer = {};
        var Result = nsCommon.SyncPost("GetLaunchImageViewerUrl", ImageViewer);
        var LaunchImage = $("#wfmBPMWorkflowCenterLeftMaintenance").find("#lblDisplayURL").text("");
        LaunchImage.text(Result + ssn + "&" + "FWN=" + window.name);
        return true;
    },

    //END FS001 Related JS

    //FS006 (Communication) Related JS

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

    OnclickInitiateServiceRetirement: function (e) {
        var lobjGridOrListView;
        var dataRows;
        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
           if (relatedGrid != null) {
               lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
               dataRows = lobjGridOrListView.getSelectedRows();
               if (dataRows.length==0) {
                   nsCommon.DispalyError("Plan is required to initiate the BPM", e.context.activeDivID);
                   return false;
               }
               if (dataRows.length > 1) {
                   nsCommon.DispalyError("Cannot select more than one plan to initiate the BPM.", e.context.activeDivID);
                   return false;
               }
           }

           if (confirm('Please verify the eligibility of the participant for Service Retirement. Ok/Cancel')) {
                if (dataRows.length > 0) {

                    if (dataRows[0].dt_Vested_6_0 == true && dataRows[0].dt_PlanStatus_2_0== "Active") {
                        var ldomDiv = $("#" + ns.viewModel.currentModel);
                        var PersonId = ldomDiv.find("#txtPersonId").val();
                        var PersonAccountID = dataRows[0][$(e)[0].target.id]['P4'];
                        var ControlID = e.srcElement.id;

                        var Params = {
                            "PersonId": PersonId,
                            "PersonAccountId": PersonAccountID,
                            "ControlId": ControlID
                        };
                        var result = nsCommon.SyncPost("InitiateServiceRetirement", Params, "POST");

                         if (result != undefined && result != "") {
                            nsCommon.DispalyError(result, e.context.activeDivID);
                            return false;
                        }
                       return true;
                    }
                    else {
                        nsCommon.DispalyError("Not Eligible for Retirement.", e.context.activeDivID);
                        return false;
                    }
                }
            }
           else {
               return false;
           }
    },

    OnclickICancelServiceRetirement: function (e) {

        var relatedGrid = MVVMGlobal.GetControlAttribute($($(e)[0].target), 'sfwRelatedControl', e.context.activeDivID, false); // templateAttr
        if (relatedGrid != null) {
            var lobjGridOrListView = nsCommon.GetWidgetByActiveDivIdAndControlId(e.context.activeDivID, relatedGrid);
            var dataRows = lobjGridOrListView.getSelectedRows();
            if (dataRows.length > 0) {
                if (dataRows.length > 1) {
                    nsCommon.DispalyError("Cannot Cancel more than one Service Retirement.", e.context.activeDivID);
                    return false;
                }
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var PersonId = ldomDiv.find("#txtPersonId").val();
                var PersonAccountID = dataRows[0][$(e)[0].target.id]['P4'];
                var ControlID = e.srcElement.id;

                var Params = {
                    "PersonId": PersonId,
                    "PersonAccountId": PersonAccountID,
                    "ControlId": ControlID
                };
                var result = nsCommon.SyncPost("InitiateServiceRetirement", Params, "POST");

                if (result != undefined && result != "") {
                    if (confirm('A BPM process exists for this MPID. Terminate the existing BPM Ok/Cancel?')) {
                         nsCommon.SyncPost("CancelServiceRetirement", Params, "POST");
                        return true;
                    }
                    return false;
                }
                return true;
            }
            else {
                nsCommon.DispalyError("Plan is required to initiate the BPM.", e.context.activeDivID);
                return false;
            }
        }
        else {
            return false;
        }
    },

    OnClickCanelApplication: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find("#divBpmActivityInstanceDetails").length > 0) {
            if (confirm('A BPM process exists for this MPID. Terminate the existing BPM Ok.')) {
                return true;
            }
            return false;
        }
        return true;
    },


    OnclickBenefitCalculation: function (e) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var ActivityInstanceRefrenceID = ldomDiv.find("#lblActivityInstanceId").text();
        var Params = {
            "ActivityInstanceRefrenceId": ActivityInstanceRefrenceID,
        };
        nsCommon.SyncPost("SetActivityInstanceRefrenceId", Params, "POST");
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
    AddSocialSecurityDisabilityApplication: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        if (ldomDiv.find('#chkSent').is(':checked') == false) {
            ldomDiv.find("#chkSent").attr("checked", false).trigger("change");
        }
        if (ldomDiv.find('#chkReceived').is(':checked') == false) {
            ldomDiv.find("#chkReceived").attr("checked", false).trigger("change");
        }
        return true;
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

    //end Communication Related JS

    // FS006 (Reports) Related JS

    // end Report Related JS


    // Uncommented for ECM Upload
    FileUploadSuccess: function (e) {
        delete ns.DirtyData[e.context.activeDivID];
        MVVMGlobal.PopulateDirtyFormList();
        // F/W Upgrade : code conversion of btnUploadFile_Click method.
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ns.viewModel.currentModel.indexOf("wfmPirMaintenance") == 0) {
            ldomDiv.find("#btnCancel").trigger("click");
        }
    },
    
    //F/w Upgrade: Code conversion of btnGenerateReport_Click validations related code
    ValidateReportParams: function (e) {
        var lActiveRpt = nsRpt.CurrentRpt.RptForm;
        var ldomDiv = $("#" + e.context.activeDivID);

        if (lActiveRpt == "") {
            nsCommon.DispalyError("No report selected. Please select the report to be generated.", e.context.activeDivID);
            return false;
        }
        if (lActiveRpt == "rptRetireeListByDatesReport") {
            var PaymentDate = ldomDiv.find('#adtPaymentDate').val();
            if (PaymentDate == "") {
                nsCommon.DispalyError("Please enter payment date.", e.context.activeDivID);
                return false;
            }
        }
        //Reuired filed validation for reports .
        if (lActiveRpt == "rptBenefitProcessCountsReport" || lActiveRpt == "rptIAPCountsandAmountsbyMonthReport" || lActiveRpt == "rptDisabilityCountsandAmountsbyMonthReport" || lActiveRpt == "rptContinuingSurvivorBenefitsbyMonthReport"
            || lActiveRpt == "rptDeceasedRetireesOverpaymentReport" || lActiveRpt == "rptDemographicAuditHistory") {
            var FromDate = new Date(ldomDiv.find("#FROM").val());
            var ToDate = new Date(ldomDiv.find("#TO").val());
            var diff = Math.abs(FromDate.getTime() - ToDate.getTime());
            var result = ((diff / (1000 * 3600 * 24)) / 365);
            if (ldomDiv.find("#FROM").val() == "" && ldomDiv.find("#TO").val() == "") {
                nsCommon.DispalyError("From Date is Required. <br/> To Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#FROM").val() == "") {
                nsCommon.DispalyError("From Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#TO").val() == "") {
                nsCommon.DispalyError("To Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (FromDate > ToDate) {
                nsCommon.DispalyError("To Date cannot be less than From Date. ", e.context.activeDivID);
                return false;
            }

            if (lActiveRpt != "rptDemographicAuditHistory" && result > 3) {
                nsCommon.DispalyError("Msg ID : 6120 [ This report cannot be run for a duration of greater than 3 years. ] ", e.context.activeDivID);
                return false;
            }
        }
        else if (lActiveRpt == "rptPensionReportForDeath") {
            var Month = ldomDiv.find("#MONTH").val();
            var Year = ldomDiv.find("#YEAR").val()
            if (Month == "" && Year == "") {
                nsCommon.DispalyError("Month is Required <br/> Year is Required ", e.context.activeDivID);
                return false;
            }
            else if (Month == "") {
                nsCommon.DispalyError("Month is Required. ", e.context.activeDivID);
                return false;
            }
            else if (Year == "") {
                nsCommon.DispalyError("Year is Required ", e.context.activeDivID);
                return false;
            }
        }
        else if (lActiveRpt == "rptAgingReport" || lActiveRpt == "rptDistributionStatusTransitionReport" || lActiveRpt == "rptPaymentBalancingReport") {
            if (ldomDiv.find("#FromDate").val() == "" && ldomDiv.find("#ToDate").val() == "") {
                nsCommon.DispalyError("From Date is Required. <br/> To Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#FromDate").val() == "") {
                nsCommon.DispalyError("From Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#ToDate").val() == "") {
                nsCommon.DispalyError("To Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#FromDate").val() > ldomDiv.find("#ToDate").val()) {
                nsCommon.DispalyError("To Date cannot be less than From Date. ", e.context.activeDivID);
                return false;
            }
        }
        else if (lActiveRpt == "rpt1099ReconReport" || lActiveRpt == "rptReturnedMailReport") {
            if (lActiveRpt != "rptReturnedMailReport" && ldomDiv.find("#START_DATE").val() == "" && ldomDiv.find("#END_DATE").val() == "") {
                nsCommon.DispalyError("Start Date is Required. <br/> End Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#START_DATE").val() == "") {
                nsCommon.DispalyError("Start Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (lActiveRpt != "rptReturnedMailReport" && ldomDiv.find("#END_DATE").val() == "") {
                nsCommon.DispalyError("End Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#START_DATE").val() > ldomDiv.find("#END_DATE").val()) {
                nsCommon.DispalyError("End Date cannot be less than Start Date. ", e.context.activeDivID);
                return false;
            }
        }
        else if (lActiveRpt == "rptPayeeErrorReport") {

            if (ldomDiv.find("#aintPlanId").val() == "" && ldomDiv.find("#adtStatusEffectiveDateFrom").val() == "" && ldomDiv.find("#adtStatusEffectiveDateTo").val() == "") {
                nsCommon.DispalyError("Please select Plan <br/> Status Effective Date From is Required <br/> Status Effective Date To is Required ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#adtStatusEffectiveDateFrom").val() == "" && ldomDiv.find("#adtStatusEffectiveDateTo").val() == "") {
                nsCommon.DispalyError("Status Effective Date From is Required <br/> Status Effective Date To is Required ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#aintPlanId").val() == "") {
                nsCommon.DispalyError("Please select Plan ", e.context.activeDivID);
                return false;
            }

            else if (ldomDiv.find("#adtStatusEffectiveDateFrom").val() == "") {
                nsCommon.DispalyError("Status Effective Date From is Required ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#adtStatusEffectiveDateTo").val() == "") {
                nsCommon.DispalyError("Status Effective Date To is Required ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#adtStatusEffectiveDateFrom").val() > ldomDiv.find("#adtStatusEffectiveDateTo").val()) {
                nsCommon.DispalyError("Status Effective Date To cannot be less than Status Effective Date From ", e.context.activeDivID);
                return false;
            }
        }
        if (lActiveRpt == "rptStaleDatedReport") {
            if (ldomDiv.find("#FromPayDate").val() == "" && ldomDiv.find("#ToPayDate").val() == "") {
                nsCommon.DispalyError("From Date is Required. <br/> To Pay Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#FromPayDate").val() == "") {
                nsCommon.DispalyError("From Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#ToPayDate").val() == "") {
                nsCommon.DispalyError("To Pay Date is Required. ", e.context.activeDivID);
                return false;
            }
            else if (ldomDiv.find("#FromPayDate").val() > ldomDiv.find("#ToPayDate").val()) {
                nsCommon.DispalyError("To Pay Date cannot be less than From Date. ", e.context.activeDivID);
                return false;
            }
        }
        //If All condtions or validation passed then return true .
        return true;
    },
    AddFirstItemToDdl: function (addlElement) {
        if (addlElement != null && addlElement != undefined) {
            // Create a new option element
            var option = $("<option></option>");

            // Set the value and text of the option
            option.val("").text("All").attr("selected", "selected");;

            // Prepend the new option to the dropdown
            addlElement.prepend(option);
        }
    },
    AddFirstEmptyItemToDdl: function (addlElement) {
        if (addlElement != null && addlElement != undefined) {
            //if (addlElement[0].is(':empty'))
               
            // Create a new option element
            var option = $("<option></option>");

            // Set the value and text of the option
            option.val("NULL").text("").attr("selected", "selected");;

            // Prepend the new option to the dropdown
            addlElement.prepend(option);
        }
    },
    BuildLeftFormById: function (astrFormName, cssId, binddata) {
        var currentFormName = ns.viewModel.currentModel;
        ns.viewModel.currentModel = astrFormName;
        var data = ns.getTemplate(astrFormName, true);

        var lstrFormId = data.ExtraInfoFields["FormId"];
        ns.viewModel[data.ExtraInfoFields["FormId"]] = {
            HeaderData: kendo.observable(data.DomainModel.HeaderData),
            DetailsData: data.DomainModel.DetailsData
        };

        var sidebarForm = "<div id='" + astrFormName + "'><div id='" + astrFormName + "ErrorDiv' class='ErrorDiv'></div>" + data.Template + "</div>";

        var parentItem;
        if ($("#" + astrFormName).length > 0) {
            parentItem = $("#" + astrFormName).parent();
        }

        if (parentItem === undefined) {
            $(cssId).append(sidebarForm);
        } else {
            ns.blnFromDeleteTreeNode = true;
            ns.destroyAll(astrFormName);
            ns.blnFromDeleteTreeNode = false;
            $(cssId).append(sidebarForm);
        }
        if (currentFormName !== undefined || currentFormName !== null)
            ns.viewModel.currentModel = currentFormName;

        ns.applyKendoUI(cssId, astrFormName, astrFormName);
        ns.Templates[astrFormName].HeaderData = kendo.observable(ns.Templates[astrFormName].HeaderData);
        kendo.bind($(cssId + " #" + astrFormName)[0], ns.Templates[astrFormName].HeaderData);
    },

    AfterBindFormData: function (e) {
        var formName = nsCommon.GetFormNameFromDivID(e.context.activeDivID);
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (formName == "wfmPaymentReissueDetailMaintenance") {
            var txtIstrBenefitOptionValue = ldomDiv.find("#txtIstrBenefitOptionValue").val();
            if (txtIstrBenefitOptionValue != "LUMP") {
                ldomDiv.find("#txtIstrRecipientRollOverOrgMPID").hide();
                ldomDiv.find("#capIstrRecipientRollOverOrgMPID").hide();
                ldomDiv.find("#btnRetrieveRecipientRollOverOrgMPID").hide();
            }
            else {
                ldomDiv.find("#txtIstrRecipientRollOverOrgMPID").show();
                ldomDiv.find("#capIstrRecipientRollOverOrgMPID").show();
                ldomDiv.find("#btnRetrieveRecipientRollOverOrgMPID").show();
            }

        }

        else if (formName == "wfmPayeeAccountACHDetailsMaintenance") {
            
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvPayeeAccountAchDetail");
            if (lobjGridWidget.jsObject.totalRecords == 0) {
                ldomDiv.find("#chkPreNoteFlag").prop('checked', true);
            }
        }

        else if (formName == "wfmWithdrawalApplicationMaintenance") {

            var DROId = ldomDiv.find("#lblDROId").text();
            if (DROId != "")
                ldomDiv.find("#pnlMain .s-lblPanelbarTitle").text("Alternate Payee Details").trigger("change");

            if (ldomDiv.find("#chkEmergencyOneTimePayment").is(':checked') == false) {
                ldomDiv.find("#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc").hide();
            }
            if (ldomDiv.find("#ddlWithdrawalType option:selected").val() != "") {
                ldomDiv.find("#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc").show();
            }
            if (ldomDiv.find("#chkEmergencyOneTimePayment").is(':checked') == true) {
                ldomDiv.find("#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc").show();
            }
        }
        else if (formName == "wfmParticipantBeneficiaryMaintenance") {

            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvPersonAccountBeneficiary");
            if (lobjGridWidget.jsObject.totalRecords == 0) {
                
                ldomDiv.find("#txtStartDate").val((new Date()).toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' })).trigger("change");
            }
            if (ldomDiv.find("#ddlBeneficiaryFromValue1 option[value='']").val()=="") {
                ldomDiv.find("#ddlBeneficiaryFromValue1 option[value='']").remove();
            }
        }
        else if (formName == "wfmPayeeAccountTaxwithholdingMaintenance") {

            var BenefitDistributionType = ldomDiv.find("#lblBenefitDistributionType").text();
            var RetireeIncrFlag = ldomDiv.find("#lblRetireeIncrFlag").text();
            if (RetireeIncrFlag == "Y" && BenefitDistributionType == null)
                ldomDiv.find("#ddlTaxIdentifierValue").prop("disabled", true);
            else
                ldomDiv.find("#ddlTaxIdentifierValue").prop("disabled", false);
            if (ldomDiv.find("#ddlBenefitDistributionTypeValue1").hasClass('neo-PopulateCascadingDropdown'))
                ldomDiv.find("#ddlBenefitDistributionTypeValue1").removeClass('neo-PopulateCascadingDropdown');
        }
        else if (formName == "wfmPayeeAccountRolloverMaintenance") {
            if (ldomDiv.find("#chkSendToParticipant").is(':checked')) {
               
                fnGetParticipantAddress();
            }
        }
        
        else if (formName == "wfmRetirementMaintenance") {
            var PlanId = ldomDiv.find("#ddlIintPlanID").val();
            if (PlanId == null || PlanId == 0 || PlanId == "")
                RetirementWizardHideControls(ldomDiv);
        } 
        else if (formName == "wfmPayeeAccountFedralWthHoldingMaintenance") {
            var ddlWizardBenefitDistributionTypeValue1 = ldomDiv.find("#ddlWizardBenefitDistributionTypeValue1").val();
            if (ddlWizardBenefitDistributionTypeValue1 == null || ddlWizardBenefitDistributionTypeValue1 == "")
                RetirementWizardFedTaxHolding(ldomDiv);
        }
        setTimeout(function () {
            if (ns.viewModel.currentModel.indexOf("wfmDeathPreRetirementMaintenance") == 0) {
            if (ldomDiv.find('#ddlIstrSurvivorTypeValue:selected').val() == undefined) {
                ldomDiv.find("#ddlIstrSurvivorTypeValue").val(ldomDiv.find("#ddlIstrSurvivorTypeValue option:first").val());
                }
            }
            if (formName == "wfmRetirementApplicationMaintenance") {
                if ($("#" + ns.viewModel.currentModel).find('#capIAPWaitTimer').css('display') === 'none') {
                    $("#" + ns.viewModel.currentModel).find('#txtIAPWaitTimer, #txtIAPWaitTimer + img.ui-datepicker-trigger').hide();
                }
                else {
                    $("#" + ns.viewModel.currentModel).find('#txtIAPWaitTimer, #txtIAPWaitTimer + img.ui-datepicker-trigger').show();
                }
            }
            if (formName == "wfmBenefitCalculationWithdrawalMaintenance") {
                var txtAge = ldomDiv.find("#txtAge");
                var txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#txtBeneficiaryPersonDateOfBirth");
                var txtPaymentDate = ldomDiv.find("#txtPaymentDate");
                var txtRetirementDate = ldomDiv.find("#txtRetirementDate");
                
                if (txtAge != undefined && txtAge.hasClass('hideByReadonly')) {
                    var lblrdfor_txtAge = ldomDiv.find("#lblrdfor_txtAge");
                    if (lblrdfor_txtAge != undefined) {
                        lblrdfor_txtAge.text(lblrdfor_txtAge.text().replace(/\\/g, ''));                                            
                    }
                }
                if (txtBeneficiaryPersonDateOfBirth != undefined && txtBeneficiaryPersonDateOfBirth.hasClass('hideByReadonly')) {
                    var lblrdfor_txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#lblrdfor_txtBeneficiaryPersonDateOfBirth");
                    if (lblrdfor_txtBeneficiaryPersonDateOfBirth != undefined) {
                        lblrdfor_txtBeneficiaryPersonDateOfBirth.removeClass('hasDatepicker');
                    }
                }
                if (txtPaymentDate != undefined && txtPaymentDate.hasClass('hideByReadonly')) {                    
                    var lblrdfor_txtPaymentDate = ldomDiv.find("#lblrdfor_txtPaymentDate");
                    if (lblrdfor_txtPaymentDate != undefined) {
                        lblrdfor_txtPaymentDate.removeClass('hasDatepicker');
                    }
                }  
                if (txtRetirementDate != undefined && txtRetirementDate.hasClass('hideByReadonly')) {
                    var lblrdfor_txtRetirementDate = ldomDiv.find("#lblrdfor_txtRetirementDate");
                    if (lblrdfor_txtRetirementDate != undefined && lblrdfor_txtRetirementDate.hasClass('hasDatepicker')) {
                        lblrdfor_txtRetirementDate.removeClass('hasDatepicker');
                    }
                }
            }
            if (formName == "wfmBenefitCalculationPostRetirementDeathMaintenance") {
                var lblOvverideAmountreadonly = ldomDiv.find("#lblOvverideAmountreadonly");
                if (lblOvverideAmountreadonly != undefined) {
                    if (lblOvverideAmountreadonly.text() == "Y") {
                        ldomDiv.find("#lblOverriddenSurvivorAmount").hide();                 
                    }
                    else if (lblOvverideAmountreadonly.text() == "N") {
                        ldomDiv.find("#txtOverriddenSurvivorAmount").hide();
                    }
                }
            }
            if (formName == "wfmDisabiltyBenefitCalculationMaintenance") {
                var txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#txtBeneficiaryPersonDateOfBirth");
                if (txtBeneficiaryPersonDateOfBirth != undefined && txtBeneficiaryPersonDateOfBirth.hasClass('hideByReadonly')) {
                    var lblrdfor_txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#lblrdfor_txtBeneficiaryPersonDateOfBirth");
                    if (lblrdfor_txtBeneficiaryPersonDateOfBirth != undefined) {
                        lblrdfor_txtBeneficiaryPersonDateOfBirth.removeClass('hasDatepicker');
                        ldomDiv.find("#lblrdfor_txtRetirementDate,#lblrdfor_txtRetirementDateOption2,#lblrdfor_txtSsaDisabilityOnsetDate,#lblrdfor_txtSsaApplicationDate").removeClass('hasDatepicker');
                        ldomDiv.find("#lblrdfor_txtSsaApprovalDate,#lblrdfor_txtAwardedOnDate,#lblrdfor_txtPaymentDate").removeClass('hasDatepicker');
                    }
                }
            }
            if (formName == "wfmBenefitCalculationPreRetirementDeathMaintenance") {                
                var txtLabel10 = ldomDiv.find("#txtLabel10");
                var txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#txtBeneficiaryPersonDateOfBirth");
                if (txtLabel10 != undefined && txtLabel10.hasClass('hideByReadonly')) {
                    var lblrdfor_txtLabel10 = ldomDiv.find("#lblrdfor_txtLabel10");
                    if (lblrdfor_txtLabel10 != undefined && lblrdfor_txtLabel10.hasClass('hasDatepicker')) {
                        lblrdfor_txtLabel10.removeClass('hasDatepicker');
                    }
                }
                if (txtBeneficiaryPersonDateOfBirth != undefined && txtBeneficiaryPersonDateOfBirth.hasClass('hideByReadonly')) {
                    var lblrdfor_txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#lblrdfor_txtBeneficiaryPersonDateOfBirth");
                    if (lblrdfor_txtBeneficiaryPersonDateOfBirth != undefined && lblrdfor_txtBeneficiaryPersonDateOfBirth.hasClass('hasDatepicker')) {
                        lblrdfor_txtBeneficiaryPersonDateOfBirth.removeClass('hasDatepicker');
                    }
                }
            }
            if (formName == "wfmBenefitCalculationRetirementMaintenance") {
                var txtAge = ldomDiv.find("#txtAge");
                var txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#txtBeneficiaryPersonDateOfBirth");
                var txtRetirementDate = ldomDiv.find("#txtRetirementDate");
                if (txtAge != undefined && txtAge.hasClass('hideByReadonly')) {
                    var lblrdfor_txtAge = ldomDiv.find("#lblrdfor_txtAge");
                    if (lblrdfor_txtAge != undefined) {
                        lblrdfor_txtAge.text(lblrdfor_txtAge.text().replace(/\\/g, ''));
                    }
                }
                if (txtBeneficiaryPersonDateOfBirth != undefined && txtBeneficiaryPersonDateOfBirth.hasClass('hideByReadonly')) {
                    var lblrdfor_txtBeneficiaryPersonDateOfBirth = ldomDiv.find("#lblrdfor_txtBeneficiaryPersonDateOfBirth");
                    if (lblrdfor_txtBeneficiaryPersonDateOfBirth != undefined) {
                        lblrdfor_txtBeneficiaryPersonDateOfBirth.removeClass('hasDatepicker');
                    }
                }
                if (txtRetirementDate != undefined && txtRetirementDate.hasClass('hideByReadonly')) {
                    var lblrdfor_txtRetirementDate = ldomDiv.find("#lblrdfor_txtRetirementDate");
                    if (lblrdfor_txtRetirementDate != undefined && lblrdfor_txtRetirementDate.hasClass('hasDatepicker')) {
                        lblrdfor_txtRetirementDate.removeClass('hasDatepicker');
                    }
                }
            }
            if (ns.viewModel.currentModel.indexOf("wfmPersonContactMaintenance") >= 0) {
                OnContactTypeChangeFunction();
            }
            if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountACHDetailsWMaintenance") >= 0) {
                nsUserFunctions.SetVisibilityOnDdlPaymentMethod(e);
            }
            if (ns.viewModel.currentModel.indexOf("wfmParticipantBeneficiaryMaintenance") >= 0) {
                ldomDiv.find("#ddlDropDownList1").prop('disabled', true);
            }
            if (ns.viewModel.currentModel.indexOf("wfmJobScheduleMaintenance") == 0) {
                ldomDiv.find("#ddlFreqSubdayType").val("4");
            }
            if (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") >= 0) {
                GridColumnVisbility();
            }
            if (ns.viewModel.currentModel.indexOf("wfmRepaymentScheduleMaintenance") >= 0) {
                nsUserFunctions.OnPaymentOptionChange(e);
            }
        }, 50);
    },
    InitilizeUserDefinedEvents: function () {
        //Reports Code Starts Here.
        //Reports Code Starts Here.
        $(document).on("change", "#ddlReports", function (e) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            var selectedReportValue = ldomDiv.find('#ddlReports :selected').val();
            if (selectedReportValue != null && selectedReportValue !== undefined) {
                //nsUserFunctions.GetReportInfo(this, selectedReportValue);
                nsCommon.DispalyError("", ldomDiv[0].id);
            }
            if (selectedReportValue != null && selectedReportValue == "rptPensionReportForDeath") {
                var startYear = 1975;
                var currentYear = new Date().getFullYear();
                var years = Array.from({ length: currentYear - startYear + 1 }, (_, i) => startYear + i);
                years.unshift("");

                var $lYear = $("#YEAR");
                $.each(years, function (index, year) {
                    $lYear.append($("<option></option>").attr("value", year).text(year));
                });
            }
            if (selectedReportValue != null && (selectedReportValue == "rptDisabilityCountsandAmountsbyMonthReport"
                || selectedReportValue == "rptStaleDatedReport"
                || selectedReportValue == "rptAgingReport")) {
                var lddlPLAN_ID = ldomDiv.find("#PLAN_ID");
                nsUserFunctions.AddFirstItemToDdl(lddlPLAN_ID);
            }
            if (selectedReportValue != null && selectedReportValue == "rpt1099ReconReport")
            {
                var lddlStateCode = ldomDiv.find("#STATE_CODE");
                nsUserFunctions.AddFirstItemToDdl(lddlStateCode);
            }
            if (selectedReportValue != null && selectedReportValue == "rptWorkflowMetricsDetail") {
                var lddltypeId = ldomDiv.find("#typeId");
                nsUserFunctions.AddFirstItemToDdl(lddltypeId);
                var lddlqualifiedName = ldomDiv.find("#qualifiedName");
                nsUserFunctions.AddFirstItemToDdl(lddlqualifiedName);
                var lddluserID = ldomDiv.find("#userID");
                nsUserFunctions.AddFirstItemToDdl(lddluserID);
                var lddlstatus = ldomDiv.find("#status");
                nsUserFunctions.AddFirstItemToDdl(lddlstatus);
                var ltxtpersonID = ldomDiv.find("#personID");
                if (ltxtpersonID != null && ltxtpersonID != undefined) {
                    ltxtpersonID.text = "";
                }
                var ltxtorgID = ldomDiv.find("#orgID");
                if (ltxtorgID != null && ltxtorgID != undefined) {
                    ltxtorgID.text = "";
                }
            }
            if (selectedReportValue != null && selectedReportValue == "rptWorkflowMetricsSummaryReport") {
                var lddltypeId = ldomDiv.find("#typeId");
                nsUserFunctions.AddFirstItemToDdl(lddltypeId);
            }
            if (selectedReportValue != null && (selectedReportValue == "rptLocalDeathReport" || selectedReportValue == "rptLocalRetireeListReport")) {
            
                var lddlLcl_ID = ldomDiv.find("#Lcl_ID");
                nsUserFunctions.AddFirstItemToDdl(lddlLcl_ID);
            }
            if (selectedReportValue != null && (selectedReportValue == "rptPaymentDirectivesReconReport")) {
                ldomDiv.find("#PLAN_ID").prop("selectedIndex", 0);
                ldomDiv.find("#ADHOC").prop("selectedIndex", 0);
            }
        });
        //End Reports Code

        $(document).on("change", "#txtJoinderRecvDate", function (e) {
            if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined && ((ns.viewModel.currentModel.indexOf("wfmQDROApplicationMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationMedicalMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationFuneralMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationDDAPartialWithdrawalMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalDDAFullWithdrawalMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationUnemploymentMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationOthersMaintenance") == 0) || (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationHousingMaintenance") == 0))) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var txtJoinderRecvDate = ldomDiv.find("#txtJoinderRecvDate").val();
                if (txtJoinderRecvDate == "" || txtJoinderRecvDate == null || txtJoinderRecvDate.endsWith("_")) {
                    ldomDiv.find("#chkJoinderOnFile").prop('checked', false).trigger("change");
                }
                else {
                    ldomDiv.find("#chkJoinderOnFile").prop('checked', true).trigger("change");
                }
            }
        });

        $(document).on("change", "#ddlBeneficiaryTypeValue", function (e) {
            if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined && ((ns.viewModel.currentModel.indexOf("wfmParticipantBeneficiaryMaintenance") == 0)))
                var ldomDiv = $("#" + ns.viewModel.currentModel);
            var txtDistPercent = ldomDiv.find("#txtDistPercent").val();
            var BeneficiaryTypeValue = ldomDiv.find("#ddlBeneficiaryTypeValue").val();
            if ((txtDistPercent == "" || txtDistPercent == null) && (BeneficiaryTypeValue == "PRIM" || BeneficiaryTypeValue == "CONT")) {
                ldomDiv.find("#txtDistPercent").val(100).trigger('change');
            }
        });

        $(document).on("focus", "#txtScheduleName", function (e) {
            if (ns.viewModel.currentForm == "wfmJobScheduleMaintenance") {
                $(this).get(0).setSelectionRange(0, 0);
            }
        });  


        $(document).on("change", "#chkSendToParticipant", function (e) {
            if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined && ((ns.viewModel.currentModel.indexOf("wfmPayeeAccountRolloverMaintenance") == 0))) {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find("#chkSendToParticipant").is(':checked')) {
                
                fnGetParticipantAddress();
            }
        }
                
        });
        //End Reports Code

        $(document).on("click", "#btnRetrieve", function (e) {
            if (ns.viewModel.currentForm == 'wfmBeneficiaryLookup_retrieve' && e.currentTarget.getAttribute('sfwparentformid') == 'wfmPersonDependentMaintenance') {
                $('.k-window-title').text('Dependent Lookup');
            }
        });

        $(document).on("change", "#ddlIintPlanID", function (e) {
            if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined && ((ns.viewModel.currentModel.indexOf("wfmRetirementMaintenance") == 0))) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var PlanId = ldomDiv.find("#ddlIintPlanID option:selected").val();

                var eligiblePlan = ldomDiv.find("#ddlIintPlanID option:selected").text();
                var txtEligiblePlan = ldomDiv.find("#txtEligiblePlan");
                var txtEligibleIAPPlan = ldomDiv.find("#txtEligibleIAPPlan");
                txtEligiblePlan.val(eligiblePlan);
                txtEligibleIAPPlan.val("Individual Account Plan");


                if (PlanId == 2) {

                    ldomDiv.find("#capEligibleIAPPlan,#txtEligibleIAPPlan,#capIAPBenefitOption,#ddlIAPBenefitOpt,#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName").show();                   
                    ldomDiv.find("#capEligiblePlan,#txtEligiblePlan,#capBenefitOption,#ddlBenefitOpt,#capSpouseConsent,#chkSpouseConsent,#lblJointAnnunantName,#ddlJointAnnunantName").show();
                    ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName,#capIAPSpouseConsent,#chkIAPSpouseConsent").hide();                                                                           
                } else {

                    ldomDiv.find("#capEligiblePlan,#txtEligiblePlan,#capBenefitOption,#ddlBenefitOpt,#capSpouseConsent,#chkSpouseConsent").show();

                    ldomDiv.find("#capEligibleIAPPlan,#txtEligibleIAPPlan,#capIAPBenefitOption,#ddlIAPBenefitOpt,#capIAPSpouseConsent,#chkIAPSpouseConsent,#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName").hide();                  

                }
            }

        });

        $(document).on("change", "#ddlBenefitOpt", function (e) {
            if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined && ((ns.viewModel.currentModel.indexOf("wfmRetirementMaintenance") == 0))) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var benefitOptType1 = ldomDiv.find("#ddlBenefitOpt").val();

               
                if (benefitOptType1 == "J100" || benefitOptType1 == "JA66" || benefitOptType1 == "JP50" || benefitOptType1 == "JPOP" || benefitOptType1 == "JS66" ||
                    benefitOptType1 == "JS75" || benefitOptType1 == "JSAA") {

                    ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName,#chkSpousalConsent,#capSpouseConsent").show();

                } else if (benefitOptType1 == "QJ50") {

                        ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName").show();

                } else {
                         ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName,#capIAPSpouseConsent,#chkIAPSpouseConsent").hide();
                         ldomDiv.find("#chkSpousalConsent,#capSpouseConsent").show();
                    
                       }
            }

        });

        $(document).on("keyup", "#txtTaxAllowance", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountTaxwithholdingMaintenance") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var txtTaxAllowancevalue = ldomDiv.find("#txtTaxAllowance").val();
                if (txtTaxAllowancevalue.indexOf('-') !== -1) {
                    txtTaxAllowancevalue = txtTaxAllowancevalue.replace(/-/g, '');
                    ldomDiv.find("#txtTaxAllowance").val('-' + txtTaxAllowancevalue);
                }
            }
        });

        $(document).on("change", "#ddlPayTo", function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountDeductionMaintenance") === 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var ddlPayToValue = ldomDiv.find("#ddlPayTo option:selected").val();
                if (ddlPayToValue == "PRSN") {
                    ldomDiv.find("#txtPersonId").val("").trigger("change");
                }
            }
        });

        $(document).on("click", ".k-in, .jstree-anchor", function (e) {
            if ($("#SlideOutTree").is(":visible") == true)
                $("#navTreeTriger").trigger("click");
        });

        $(document).on('click', '#btnAddNotes', function (e) {
            if (ns.viewModel.currentModel.indexOf("wfmDeathPreRetirementMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDisabilityApplicationMaintenance") == 0 ||
                ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmRetirementApplicationMaintenance") == 0 ||
                ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0) {
                ns.PositionCursor(ns.viewModel.currentModel, $('#CenterSplitter').scrollTo(0));
            }
            return true;
        });
        $(document).on('click', '#btnReset,#btnStoreSearch,#btnOpen', function (e) {          
            if (ns.viewModel.currentModel.indexOf("wfmPendingRetirementStatusLookup") == 0) {
                var ldomDiv = $("#" + ns.viewModel.currentModel);
                var lblLabeldatefrom = ldomDiv.find("#lblLabeldatefrom");
                var lblLabeldateto = ldomDiv.find("#lblLabeldateto");
                if (lblLabeldatefrom != undefined && !(lblLabeldatefrom.hasClass('hideControl'))) {
                    lblLabeldatefrom.addClass('hideControl');
                }
                if (lblLabeldateto != undefined && !(lblLabeldateto.hasClass('hideControl'))) {
                    lblLabeldateto.addClass('hideControl');
                }
            }
        });
    },

    OnContactTypeChange: function (e) {
        OnContactTypeChangeFunction();
        return true;
    },

    OnPaymentOptionChange: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var paymentOptionValue = ldomDiv.find("#ddlPaymentOptionValue").val();

        if (paymentOptionValue == 'PBEN') {
            ldomDiv.find("#lblRepaymentType").show();
            ldomDiv.find("#btnAdd").hide();
        }
        else if (paymentOptionValue == 'PRCK') {
            ldomDiv.find("#lblRepaymentType").hide();
            ldomDiv.find("#btnAdd").show();
        }
        else {
            ldomDiv.find("#lblRepaymentType").hide();
            ldomDiv.find("#btnAdd").show();
        }
    },

    ChangeDispalyMessage: function (e) {
        var lstrFormName = nsCommon.GetProperFormName(e.context.activeDivID);
        var lstrPrevMSG = e.context.Message;
        if ((lstrFormName == "wfmReturnMailOrganizationWizard" || lstrFormName == "wfmReturnMailWizard" ) && lstrPrevMSG == " [ Record displayed.  Please make changes and press SAVE. ]") {
            return "[ Wizard Started ]";
        }
    },

    ChangeSearchCriteriaOnOpenLookupFromBPM: function (e) {
        var LaunchImage = $("#wfmBPMWorkflowCenterLeftMaintenance").find("#lblDisplayURL").text() + window.name;
        window.open(LaunchImage, null, 'width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes');
        return true;
    },

    SetFederalTaxFieldVisibleStatus: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var current_identifier = ldomDiv.find("#ddlTaxIdentifierValue option:selected").val();
        var current_BenefitDistributionType = ldomDiv.find("#ddlBenefitDistributionTypeValue1 option:selected").val();
        if (current_BenefitDistributionType != "" && current_BenefitDistributionType != undefined) {
            ldomDiv.find("#ddlBenefitDistributionTypeValue1").attr("disabled", true);
        }
        if ((current_identifier == "" || current_identifier == "STAT")) {
            ldomDiv.find("#txtStep2B,#txtStep3Amount,#txtStep4a,#txtStep4b,#txtStep4c,#lbltxtStep2B,#lbltxtStep3Amount,#lbltxtStep4a,#lbltxtStep4b,#lbltxtStep4c,#lblfilingstatus").hide();
            ldomDiv.find("#txtTaxPercentage,#txtTaxAllowance,#txtAdditionalTaxAmount1,#lbltxtTaxPercentage,#lbltxtTaxAllowance,#lbltxtAdditionalTaxAmount1,#lblmaritalstatus").show();
        }
        if ((current_identifier == "FDRL" && current_BenefitDistributionType == "LSDB")) {
            ldomDiv.find("#txtStep2B,#txtStep3Amount,#txtStep4a,#txtStep4b,#txtStep4c,#txtAdditionalTaxAmount1,#lbltxtStep2B,#lbltxtStep3Amount,#lbltxtStep4a,#lbltxtStep4b,#lbltxtStep4c,#lbltxtAdditionalTaxAmount1,#lblmaritalstatus").hide();
            ldomDiv.find("#txtTaxPercentage,#txtTaxAllowance,#lbltxtTaxPercentage,#lbltxtTaxAllowance,#lblfilingstatus").show();
        }
        if (current_identifier == "FDRL" && current_BenefitDistributionType == "MNBF") {
            ldomDiv.find("#txtTaxPercentage,#lbltxtTaxPercentage,#txtTaxAllowance,#lbltxtTaxAllowance,#txtAdditionalTaxAmount1,#lbltxtAdditionalTaxAmount1,#lblmaritalstatus").hide();
            ldomDiv.find("#txtStep2B,#lbltxtStep2B,#txtStep3Amount,#lbltxtStep3Amount,#txtStep4a,#lbltxtStep4a,#txtStep4b,#lbltxtStep4b,#txtStep4c,#lbltxtStep4c,#lblfilingstatus").show();
        }
    },

    SaveNoChangesMessage: function (e) {
        var ActiveDivID = e.context.activeDivID;
        if (ns.DirtyData[ActiveDivID] != undefined) {
            return true;
        }
        else {
            nsCommon.DispalyMessage("No Changes To Save.", e.context.activeDivID);
            return false;
        }
    },

    OnSelectedIndexChange: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var lddlPlanId = ldomDiv.find("#ddlIaintPlan1");
        var lddlBenType = ldomDiv.find("#ddlBeneficiaryTypeValue");
        var lblBeneficiaryId = ldomDiv.find("#lblPersonAccountBeneficiaryId").text();
        var Prikarykey = 0;
        if (!ns.blnInNewMode) {
            Prikarykey = lblBeneficiaryId;
        }
        if (lddlPlanId != null && lddlBenType != null) {
            var current_plan_id = lddlPlanId.val();
            var Ben_Type = lddlBenType.val();
            if (current_plan_id != null && current_plan_id != "" && current_plan_id != "0") {
                GetPlanExists(current_plan_id, Ben_Type, Prikarykey, ldomDiv);
            }
        }
    },

    SetDefaultValuesforQdro: function (e) {
        var lstrifNotExecuteThisMethod = null;
        lstrifNotExecuteThisMethod = sessionStorage.getItem("doNotExecuteChangeEventOfPlanId");
        if (lstrifNotExecuteThisMethod != null && lstrifNotExecuteThisMethod == "True") {
            sessionStorage.removeItem("doNotExecuteChangeEventOfPlanId");
            return;
        }

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var lddlPlanId = ldomDiv.find("#ddlPlanId");
        var lddlDroModelId = ldomDiv.find("#ddlDroModelId");
        if (lddlPlanId.length == 0 && QrdoLoadFlag == false) {
            ldomDiv.find("#chkAltPayeeIncrease").prop('checked', false);
            ldomDiv.find("#chkAltPayeeEarlyRet").prop('checked', false);
            ldomDiv.find("#txtBenefitPerc").val("");
        }
        if (ldomDiv.find("#txtNetInvestmentFromDate").is(":visible") == false) {
            ldomDiv.find("#lblrdfor_txtNetInvestmentFromDate").hide();
        }
        if (ldomDiv.find("#txtNetInvestmentToDate").is(":visible") == false) {
            ldomDiv.find("#lblrdfor_txtNetInvestmentToDate").hide();
        }
        if (lddlPlanId.length == 0 || lddlDroModelId.length == 0 || QrdoLoadFlag == true) {
            QrdoLoadFlag = false;
            return;
        }
        var current_plan_id = lddlPlanId.val();
        var current_dro_id = lddlDroModelId.val();

        if (current_plan_id == "2" && (current_dro_id == "STRF" || current_dro_id == "STAF" || current_dro_id == "SPDQ")) {
            ldomDiv.find("#chkAltPayeeIncrease").prop('checked', true).trigger("change");
        }
        else {
            ldomDiv.find("#chkAltPayeeIncrease").prop('checked', false);
        }

        if (current_plan_id == "2" && current_dro_id == "STAF") {
            ldomDiv.find("#chkAltPayeeEarlyRet").prop('checked', true).trigger("change");;
        }
        else {
            ldomDiv.find("#chkAltPayeeEarlyRet").prop('checked', false);
        }

        if (current_dro_id != "SSUP" && current_dro_id != "OTHR" && current_dro_id != "CSUP" && current_plan_id != "") {
            ldomDiv.find("#txtBenefitPerc").val("50").trigger("change");
        }
        else {
            ldomDiv.find("#txtBenefitPerc").val("");
        }
        if (current_plan_id == null || current_plan_id=="") {
            ldomDiv.find("#txtBenefitAmt").val("");
        }
    },
    
    SetDefaultPayeeMPIDValue: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var current_identifier = ldomDiv.find("#ddlReissuePaymentTypeId").val();
        var current_txtPayeeMPID = ldomDiv.find("#txtIstrPayeeMPID").val();
        var current_OrgId = ldomDiv.find("#txtIstrOrgMPID").val();

        if (current_identifier == "PYEE" || current_identifier == "ROTP") {
            ldomDiv.find("#txtIstrRMPIDAddPayee").val(current_txtPayeeMPID).trigger("change");
        }
        else {
            ldomDiv.find("#txtIstrRMPIDAddPayee").val(" ").trigger("change");
        }
        if (current_identifier == "TORG" && current_OrgId != "") {
            ldomDiv.find("#txtIstrRMPIDAddPayee").val(current_OrgId).trigger("change");
        }    
    },

    SetDefaultDisableNextDueAmount: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        
        var current_identifier = ldomDiv.find("#ddlRepaymentTypeValue").val();

        if (current_identifier == "PERC" || current_identifier == null) {
            ldomDiv.find("#txtNextAmountDue").attr("disabled", true);
        }
        else {
            ldomDiv.find("#txtNextAmountDue").removeAttr("disabled");
        }
        ldomDiv.find("#txtNextAmountDue").trigger("change");

        if (current_identifier == "PERC") {

            if (ldomDiv.find("#txtFlatPercentage").val() == 0 || ldomDiv.find("#txtFlatPercentage").val() == null) {

                ldomDiv.find("#txtFlatPercentage").val("25").trigger("change").blur();
            }
        }
        else {
            ldomDiv.find("#txtFlatPercentage").val("0").trigger("change");
        }
    },

    SetVisibilityOnDdlIAPBenefitOpt: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var benefitOptType = ldomDiv.find("#ddlIAPBenefitOpt").val();

        if (benefitOptType == "J100" || benefitOptType == "JA66" || benefitOptType == "JP50" || benefitOptType == "JPOP" || benefitOptType == "JS66" ||
            benefitOptType == "JS75" || benefitOptType == "JSAA") {

            ldomDiv.find("#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName,#capIAPSpouseConsent,#chkIAPSpouseConsent").show();

        } else if (benefitOptType == "QJ50") {

            ldomDiv.find("#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName").show();

        } else {

                 ldomDiv.find("#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName").hide();

                 var IntPlan = ldomDiv.find("#ddlIintPlanID").val();
                 if (IntPlan == 2 && benefitOptType != null) {

                    ldomDiv.find("#capIAPSpouseConsent,#chkIAPSpouseConsent").show();
                 }
               }
    },
  
    SetVisibilityOnDdlPaymentMethod: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var paymentType = ldomDiv.find("#ddlPaymentMethod").val();
        var start_date = ldomDiv.find("#txtAchStartDate").parent("td");
        var end_date = ldomDiv.find("#txtAchEndDate").parent("td");
        var routing_numer = ldomDiv.find("#txtIintRoutingNumber").parent("td");
        var account_number = ldomDiv.find("#capAccountNumber").parent("td");
        if (paymentType == "CHK") {           
            ldomDiv.find("#lblLabelRoutingNumber,#capIintRoutingNumber,#capIstrOrgName,#txtIstrOrgName").hide();
            ldomDiv.find("#txtAccountNumber,#lblLabelReq2,#capBankAccountTypeValue,#ddlBankAccountTypeValue,#lblLabelReq3,#capAchStartDate,#txtAchStartDate").hide();
            ldomDiv.find("#capAchEndDate,#txtAchEndDate,#capPreNoteFlag,#capPreNoteCompletionDate,#chkPreNoteFlag,#lblPreNoteCompletionDate,#capJointAccountOwner,#chkJointAccountOwner").hide();
            ldomDiv.find("#capPaymentMethod,#lblLabelPaymentMethod,#lblIstrInformation").show();
            account_number.hide();
            routing_numer.hide();
            start_date.hide();
            end_date.hide();
        }
        else {             
            ldomDiv.find("#lblLabelRoutingNumber,#capIintRoutingNumber,#capIstrOrgName,#txtIstrOrgName").show();
            ldomDiv.find("#txtAccountNumber,#lblLabelReq2,#capBankAccountTypeValue,#ddlBankAccountTypeValue,#lblLabelReq3,#capAchStartDate,#txtAchStartDate").show();
            ldomDiv.find("#capPaymentMethod,#lblLabelPaymentMethod,#capAchEndDate,#txtAchEndDate,#capPreNoteFlag,#capPreNoteCompletionDate,#chkPreNoteFlag,#lblPreNoteCompletionDate,#capJointAccountOwner,#chkJointAccountOwner").show();
            ldomDiv.find("#lblIstrInformation").hide();
            account_number.show();
            routing_numer.show();
            start_date.show();
            end_date.show();
        }
    },
    SetVisibilityOnDdlPlanBenfitId: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var benefitOptType2 = ldomDiv.find("#ddlPlanBenfitId").val();

        if (benefitOptType2 == "J100" || benefitOptType2 == "JA66" || benefitOptType2 == "JP50" || benefitOptType2 == "JPOP" || benefitOptType2 == "JS66" ||
            benefitOptType2 == "JS75" || benefitOptType2 == "JSAA" || benefitOptType2 == "QJSA") {

            ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName,#chkSpousalConsent,#capPlanBenfitId,#lblSpousalConsent").show();

        } else if (benefitOptType2 == "QJ50") {
            ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName,#capPlanBenfitId").show();
            ldomDiv.find("#chkSpousalConsent,#lblSpousalConsent").hide();

        } else {

            ldomDiv.find("#chkSpousalConsent,#capPlanBenfitId,#lblSpousalConsent").show();
            ldomDiv.find("#lblJointAnnunantName,#ddlJointAnnunantName").hide();
        }
    },
   
    SetVisibilityOnDdlWizardBenefitDistributionTypeValue1: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var WizardBenefitDistributionTypeValue1 = ldomDiv.find("#ddlWizardBenefitDistributionTypeValue1").val();
        if (WizardBenefitDistributionTypeValue1 == "LSDB") {
            ldomDiv.find("#lblwtxtTaxAllowance,#txtwTaxAllowance,#lblwtxtAdditionalTaxAmount1,#txtwAdditionalTaxAmount1,#lblwtxtTaxPercentage,#txtwTaxPercentage").show();
            ldomDiv.find("#lblWtxtStep2B,#txtWStep2B,#lbltxtWStep3Amount,#txtWStep3Amount,#lbltxtWStep4a,#txtWStep4a,#lblWtxtStep4b,#txtWStep4b,#lblWtxtStep4c,#txtWStep4c").hide();
        } else {

            RetirementWizardFedTaxHolding(ldomDiv);
        }
    },

    SetVisibilityOnContinueClick: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

        if (ldomDiv.find("#ddlWizardBenefitDistributionTypeValue1").val() == 'MNBF' && (ldomDiv.find("#txtwAdditionalTaxAmount1").val() != '0' || ldomDiv.find("#txtwAdditionalTaxAmount1").val() != undefined)) {
            ldomDiv.find("#lblwtxtTaxPercentage,#txtwTaxPercentage,#lblwtxtTaxAllowance,#txtwTaxAllowance,#lblwtxtAdditionalTaxAmount1,#txtwAdditionalTaxAmount1").show();
        }
        return true;
    },

    SetVisibilityOnDdlTaxIdentifierValue: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var TaxIdentifierVal = ldomDiv.find("#ddlTaxIdentifierValue").val();
        var ddlBenefitDistributionTypeValue1 = ldomDiv.find("#ddlBenefitDistributionTypeValue1")
        var BenDistType = ddlBenefitDistributionTypeValue1.val();
        if (BenDistType != null && BenDistType != "" && BenDistType != "NULL") {
            GetTaxWithHoldingScreenConfiguratorColumns(ldomDiv, TaxIdentifierVal, BenDistType);
        }
        var ltxtPayeeAccountTaxWithholdingId1 = ldomDiv.find("#txtPayeeAccountTaxWithholdingId1").val();
        if (ltxtPayeeAccountTaxWithholdingId1 == "") {
            ldomDiv.find("#txtPayeeAccountTaxWithholdingId1").val(0);
        }

        var lddlMaritalStatusValue11 = ldomDiv.find("#ddlMaritalStatusValue11").val();
        var lddlTaxOption = ldomDiv.find("#ddlTaxOptionValue11").val();

        if (lddlMaritalStatusValue11 == "" && (lddlTaxOption == null || lddlTaxOption == "")) {
            var dt = new Date($.now());
            var DateCreated = $.datepicker.formatDate('mm/dd/yy', new Date($.now()));

            ldomDiv.find("#txtStartDate1").val(DateCreated).trigger("change");
            ldomDiv.find("#txtEndDate1").val("").trigger("change");
            ldomDiv.find("#txtTaxAllowance").val(0).trigger("change");
            ldomDiv.find("#txtAdditionalTaxAmount1").val(0).trigger("change");
            ldomDiv.find("#txtTaxPercentage").val(0).trigger("change");
            ldomDiv.find("#txtStep2B").val(0).trigger("change");
            ldomDiv.find("#txtStep3Amount").val(0).trigger("change");
            ldomDiv.find("#txtStep4a").val(0).trigger("change");
            ldomDiv.find("#txtStep4b").val(0).trigger("change");
            ldomDiv.find("#txtStep4c").val(0).trigger("change");
            ldomDiv.find("#txtPersonalExemptions").val(0);
            ldomDiv.find("#txtAgeandBlindnessExemptions").val(0);
            ldomDiv.find("#txtVoluntary_Withholding").val(0).trigger("change");
        }

        if (TaxIdentifierVal == "VAST") {

            ldomDiv.find("#ddlMaritalStatusValue11").hide();

        } else {

            ldomDiv.find("#ddlMaritalStatusValue11").show();
        }
        if (BenDistType == null || BenDistType == "") {
            nsUserFunctions.AddFirstEmptyItemToDdl(ddlBenefitDistributionTypeValue1);
        }
        var attr = ddlBenefitDistributionTypeValue1.attr('disabled');
        if ((attr != undefined && attr != false) && (BenDistType == null || BenDistType == "" || BenDistType == "NULL")) {
            ddlBenefitDistributionTypeValue1.removeAttr("disabled");
        }
        else if (BenDistType != null && BenDistType != "" && BenDistType != "NULL" && BenDistType != undefined) {
            ddlBenefitDistributionTypeValue1.attr('disabled',"disabled");
        }
        if (TaxIdentifierVal == null || TaxIdentifierVal == "") {
            ldomDiv.find("#lblmaritalstatus").show();
            ldomDiv.find("#lblfilingstatus").hide();
        }
    },
    SetVisibilityOnDdlDropDownList12: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

        var lstrifNotExecuteThisMethod = null;
        lstrifNotExecuteThisMethod = sessionStorage.getItem("doNotExecuteChangeEventOfStateStatus");
        if (lstrifNotExecuteThisMethod != null && lstrifNotExecuteThisMethod == "True") {
            sessionStorage.removeItem("doNotExecuteChangeEventOfStateStatus");
            ldomDiv.find("#txtPersonalExemption").removeAttr("disabled");
            ldomDiv.find("#txtAgeandBlindnessExemptions").removeAttr("disabled");
            return;
        }

        var ddlDropDownList12 = ldomDiv.find("#ddlDropDownList12").val();
        if (ddlDropDownList12 == "VAST") {
            ldomDiv.find("#txtPersonalExemption").removeAttr("disabled");
            ldomDiv.find("#txtAgeandBlindnessExemptions").removeAttr("disabled");

        } else if (ddlDropDownList12 != "") {
            ldomDiv.find("#txtPersonalExemption").val(0).trigger("change");
            ldomDiv.find("#txtAgeandBlindnessExemptions").val(0).trigger("change");
            ldomDiv.find("#txtPersonalExemption").attr("disabled", true);
            ldomDiv.find("#txtAgeandBlindnessExemptions").attr("disabled", true);
        }
    },
    SetVisibilityOnDdlTaxOptionValue11: function (e) {
        var lstrifNotExecuteThisMethod = null;
        lstrifNotExecuteThisMethod = sessionStorage.getItem("doNotExecuteChangeEventOfTaxOption");
        if (lstrifNotExecuteThisMethod != null && lstrifNotExecuteThisMethod == "True") {
            sessionStorage.removeItem("doNotExecuteChangeEventOfTaxOption");
            return;
        }

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var TaxIdentifierVal = ldomDiv.find("#ddlTaxIdentifierValue").val();
        var ddlTaxOptionValue11 = ldomDiv.find("#ddlTaxOptionValue11").val();
        if (TaxIdentifierVal == "VAST") {

            if (ddlTaxOptionValue11 == "FLAD") {
                ldomDiv.find("#lbltxtVoluntary_Withholding,#txtVoluntary_Withholding").show();
                ldomDiv.find("#lbltxtPersonalExemptions,#txtPersonalExemptions,#lbltxtAgeandBlindnessExemptions,#txtAgeandBlindnessExemptions").hide();
            } else {
                ldomDiv.find("#lbltxtVoluntary_Withholding,#txtVoluntary_Withholding").hide();
                ldomDiv.find("#lbltxtPersonalExemptions,#txtPersonalExemptions,#lbltxtAgeandBlindnessExemptions,#txtAgeandBlindnessExemptions").show();
            }

            if (ddlTaxOptionValue11 == "STAT") {
                ldomDiv.find("#lbltxtAdditionalTaxAmount1,#txtAdditionalTaxAmount1").show();
            } else {
                ldomDiv.find("#lbltxtAdditionalTaxAmount1,#txtAdditionalTaxAmount1").hide();
            }
        }

        if (ddlTaxOptionValue11 == "FTIR" || ddlTaxOptionValue11 == "FTIA" || ddlTaxOptionValue11 == "STAT" || ddlTaxOptionValue11 == "STST" || ddlTaxOptionValue11 == "FLAP" || ddlTaxOptionValue11 == "FLAD" || ddlTaxOptionValue11 == "NSTX" || ddlTaxOptionValue11 == "NFTX") {
            if (ldomDiv.find("#txtTaxAllowance").val() != null && (ldomDiv.find("#txtTaxAllowance").val() != 0 || ldomDiv.find("#txtTaxAllowance").val() != "")) {
                ldomDiv.find("#txtTaxAllowance").val(0).trigger("change");
            }
        }
        //Ticket#73404
        //SetDefaultValuesforTaxWithHolding();
    },
    SetVisibilityOnDdlBenefitDistributionTypeValue1: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var lddlTaxIdentifier = ldomDiv.find("#ddlTaxIdentifierValue");
        TaxIdentifierVal = lddlTaxIdentifier.val();

        var lddlBenefitDistributionTypeValue1 = ldomDiv.find("#ddlBenefitDistributionTypeValue1");
        BenDistType = lddlBenefitDistributionTypeValue1.val();

        GetTaxWithHoldingScreenConfiguratorColumns(ldomDiv, TaxIdentifierVal, BenDistType);
        if (BenDistType != null && BenDistType != "" && BenDistType != "NULL") {
            ldomDiv.find("#ddlTaxIdentifierValue").removeAttr("disabled");
            ldomDiv.find("#ddlBenefitDistributionTypeValue1").attr("disabled", true);
        }  
    },
    SetVisibilityOnDdlWithdrawalTypeValue: function (e) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if (ldomDiv.find("#ddlWithdrawalType option:selected").val() != "") {
            ldomDiv.find('#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc').show();
        }
        else {
            ldomDiv.find('#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc').hide();
        }
    },
    ClearNotes: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        if (ns.viewModel.currentModel.indexOf("wfmOrganizationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmPersonOverviewMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmPersonMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDeathNotificationMaintenance") == 0) {
            ldomDiv.find("#txtNotes").val("");
        }
        if (ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmRetirementApplicationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDeathPreRetirementMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDisabilityApplicationMaintenance") == 0) {
            ldomDiv.find("#itxtNotes").val("");
        }
        if (ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0) {
            ldomDiv.find("#txtIstrNewNotes").val("");
        }
        if (ns.viewModel.currentModel.indexOf("wfmQDROApplicationMaintenance") == 0 ) {
            ldomDiv.find("#txtTextBox").val("");
        }
        if (ns.viewModel.currentModel.indexOf("wfmReturnToWorkRequestMaintenance") == 0) {
            ldomDiv.find("#txtNotes").val("").trigger("focus");
            return false;
        }

        var oFirstControl = ldomDiv.find(":not([gridid]):not([listviewid]):not(.filter):not(input.check_row):not(input.s-grid-check-all):not(input.ellipse-input-pageHolder):not(input.s-grid-common-filterbox):input[type !='button']:input[type !='submit']:input[type !='image']:input[sfwretrieval !='True']:input[sfwretrieval !='true']:visible:enabled:first");
        if ((oFirstControl !== undefined) && (oFirstControl.length > 0)) {
            oFirstControl.trigger("focus");
        }
        return false;
    },
    QdroCalClick: function (e) {
        return false;
    },

    ValidateRequiredField: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var Fromdate = ldomDiv.find("#txtCpctRetirementDateFrom").val();
        var Todate = ldomDiv.find("#txtCpctRetirementDateTo").val();
        if (e.context.activeDivID.indexOf("wfmPendingRetirementStatusLookup") == 0) {
            var IstrErrors = ""
            var lblLabeldatefrom = ldomDiv.find("#lblLabeldatefrom");
            var lblLabeldateto = ldomDiv.find("#lblLabeldateto");                      
            if (Fromdate == null || Fromdate == "") {
                IstrErrors = "Please select valid Retirement from Date";
                if (lblLabeldatefrom != undefined && lblLabeldatefrom.hasClass('hideControl')) {
                    lblLabeldatefrom.removeClass("hideControl");
                }
            }
            else {
                if (lblLabeldatefrom != undefined && !(lblLabeldatefrom.hasClass('hideControl'))) {
                    lblLabeldatefrom.addClass('hideControl');
                }
            }
            if (Todate == null || Todate == "") {
                IstrErrors = IstrErrors != "" ? IstrErrors + "</br>" + "Please select valid Retirement to Date" : "Please select valid Retirement to Date";
                if (lblLabeldateto != undefined && lblLabeldateto.hasClass('hideControl')) {
                    lblLabeldateto.removeClass("hideControl");
                }
            }
            else {
                if (lblLabeldateto != undefined && !(lblLabeldateto.hasClass('hideControl'))) {
                    lblLabeldateto.addClass('hideControl');
                }
            }
            if (IstrErrors) {
                nsCommon.DispalyError(IstrErrors, e.context.activeDivID);
                return false;
            }
        }
       
        if (e.context.activeDivID.indexOf("wfmTaxWithholdingCalculatorMaintenance") == 0) {
            IstrErrors = "";
            var StatusValue = ldomDiv.find("#ddlDropDownList2").val();
            if (StatusValue != undefined && StatusValue == "") {
                IstrErrors = "Select Status.";
            }
            if (IstrErrors) {
                nsCommon.DispalyError(IstrErrors, e.context.activeDivID);
                return false;
            }
            sessionStorage.setItem("doNotExecuteChangeEventOfStateStatus", "True");
        }

        return true;
    },
    AddParticipantDtailsClick: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvPersonAccountBeneficiary");
        if (lobjGridWidget.jsObject.totalRecords == 0 && ldomDiv.find("#txtStartDate").val() != "" && ldomDiv.find("#txtStartDate").val() == GetCurrentDateTime(new Date())) {
            ldomDiv.find("#txtStartDate").val("").trigger("change");
            ldomDiv.find("#txtStartDate").val(GetCurrentDateTime(new Date())).trigger("change");
        }
        return true;
    },
    AddPayeeAccountRolloverDetail: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        if (ldomDiv.find('#chkSendToParticipant').is(':checked') == false) {
            ldomDiv.find("#chkSendToParticipant").attr("checked", false).trigger("change");
        }
        if (ldomDiv.find('#chkParticipantPickup').is(':checked') == false) {
            ldomDiv.find("#chkParticipantPickup").attr("checked", false).trigger("change");
        }
        return true;
    },
    AddPayeeAccountDeduction: function (e) {
        if (ns.DirtyData[e.context.activeDivID] && ns.DirtyData[e.context.activeDivID].HeaderData != undefined &&
            ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData != undefined) {
            delete ns.DirtyData[e.context.activeDivID].HeaderData.MaintenanceData["txtPayeeAccountDeductionId"];
        }
        return true;
    },
    BenefitApplicationNewButtonClick: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        var ddlBenefitType = ldomDiv.find("#ddlBenefitType option:selected").val();
        if (ddlBenefitType == "" || ldomDiv.find("#txtParticipantmpid").val() == "") {
            ldomDiv.find("#btnBenefitApplicationNew").trigger("click");
            return false;
        }
        return true;
    },

    BenefitCalculationNewButtonClick: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        var ddlBenefitType = ldomDiv.find("#ddlBenefitTypeValue option:selected").val();
        if (ddlBenefitType == "" || ldomDiv.find("#txtMpiPersonId").val() == "" && ldomDiv.find("#txtBenefitApplicationId").val() == "") {
            ldomDiv.find("#btnBenefitCalculationNew").trigger("click");
            return false;
        }
        else if (ddlBenefitType == "" || ldomDiv.find("#txtMpiPersonId").val() == "") {
            ldomDiv.find("#btnBenefitCalculationNew").trigger("click");
            return false;
        }
        return true;
    },

    ShowWebExtenderURL: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var Params = {
        };
        var result = nsCommon.SyncPost("GetWebExFlagDB", Params, "POST");
        if (result != undefined) {
            var obj = jQuery.parseJSON(result);
            $(obj).each(function (i, val) {
                //collection keys are table column alias names - do not change the column name.
                $.each(val, function (key, val) {
                    if (val != "Y") {
                        //LaserFische Url
                        GetLaserFische(ldomDiv);
                    }
                    else {
                        //OpusWebExtendUrl
                        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
                        var mpi_person_id = ldomDiv.find("#lblSsnWeb").text();
                        window.open("http://webx/AppXtender/ISubmitQuery.aspx?Appname=PENSION_DOCS&DataSource=Imaging&QueryType=0&SSN=" + mpi_person_id, "OpenURL", "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes", null);
                        return false;
                    }
                });
            });
        }
    },
   
    // START - FS006 Communication Related JS
    EditCorrOnLocalTool_Override: function () {
        if (nsCorr.UseLocalTool) {
            var BookmarkParams = {};
            BookmarkParams["FormID"] = nsCorr.CurrentCorr.CallingForm;
            BookmarkParams["KeyField"] = nsCorr.CurrentCorr.KeyField;
            BookmarkParams["TemplateName"] = nsCorr.CurrentCorr.CorrTemplate;
            BookmarkParams["LastGeneratedCorr"] = nsCorr.CurrentCorr.CorrFilePath;
            BookmarkParams["LastCorrSecurityLevel"] = nsCorr.CurrentCorr.LastCorrSecurityLevel;
            BookmarkParams["ShowPrintDialog"] = nsCorr.ShowPrintDialog();
            BookmarkParams["DefaultPrinter"] = nsCorr.GetDefaultPrinter();
            var result = nsCommon.SyncPost("EditCorrOnLocalTool_Override", BookmarkParams, "POST");
            if (result != null && result.ResponseMessage != null && result.ResponseMessage.istrMessage != null && result.ResponseMessage.istrMessage != "Correspondence is opened in local tool.") {
                alert(result.ResponseMessage.istrMessage);
                return false;
            }
            return result;
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
                    position: '{"top": 100}',
                    uiClasses: nsConstants.Dialog.Standard.Correspondence
                });
                nsCommon.SetWidgetControlByDivID(lstrEditCorrDiv, lobjCorrWidgetControls, nsCorr.CurrentCorr.CorrDivID);
                lobjCorrWidgetControls.open();
            }
            nsCorr.OpenCorrespondence();
        }
    },

    NavigateToMSS: function (e) {

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

        var MPID = ldomDiv.find("#txtMpiPersonId").val();

        if (MPID == null || MPID == "") {

            nsCommon.DispalyMessage("Record displayed.", e.context.activeDivID);
            
        }
        else if (MPID != undefined && MPID != null && MPID != "") {
                var Params = {
                    "astrMPID": MPID,
                };
                var result = nsCommon.SyncPost("NavigateToMSS", Params, "POST");

                if (result.ResponseMessage != null && result.ResponseMessage != undefined &&
                    result.ResponseMessage.istrMessage != null && result.ResponseMessage.istrMessage != "") {

                    if (result.DomainModel.OtherData.Data != null && result.DomainModel.OtherData.Data != undefined
                        && result.DomainModel.OtherData.Data["NewURL"] != "" && result.ResponseMessage.istrMessage.indexOf("Url created successfully.") == 0) {
                        var url = ["../", result.DomainModel.OtherData.Data['NewURL']].join('');
                        window.open(url, "_blank");
                    } else if (result.ResponseMessage.istrMessage.indexOf("Show VIP Dialog") == 0) {
                        $("#btnjQueryCancel").click(function (e) {
                            $("#overlay").hide();
                            $("#dialog").fadeOut(300);
                            e.preventDefault();
                        });
                        $("#overlay").show();
                        $("#dialog").fadeIn(300);
                        $("#overlay").off("click");
                    }
                }   
            nsCommon.DispalyMessage("Record displayed.", e.context.activeDivID);
        }                  
        return false;
    },

    BeforeShowDiv: function (e) {       
        if (ns.viewModel.currentForm.indexOf("SSNMergeLookup") > 0) {
            nsCommon.DispalyMessage("Msg ID : 6215 [ Please enter search criteria for the MPID that you are merging TO and press SEARCH. ]", e.context.activeDivID);
        }
        if (ns.viewModel.currentModel.indexOf("wfmPacketTrackingLookup") == 0) {

            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if ((ldomDiv.find("#ddlDropDownList2").find('option').filter(function () { return $(this).text().trim() == "All"; })).length == 0) {
                ldomDiv.find("#ddlDropDownList2").prepend($('<option></option>').val('0').html('All').attr("selected", "selected"));
            }
            else {
                ldomDiv.find("#ddlDropDownList2").val("0").trigger("change");
            }
            if ((ldomDiv.find("#ddlDropDownList3").find('option').filter(function () { return $(this).text().trim() == "All"; })).length == 0) {
                ldomDiv.find("#ddlDropDownList3").prepend($('<option></option>').val('0').html('All').attr("selected", "selected"));
            }
            else {
                ldomDiv.find("#ddlDropDownList3").val("0").trigger("change");
            }
        }
    },


    // F/W Upgrade : Code conversion of btn_OpnPDF and btn_OpenPDF method.
    fnOpenPDF: function (e) {
        var lstrClickedButtonId = "";
        var lstrButtonInfo = "";
        var lintPerson = "";
        var aintPayeeAccountId = "";
        var astrSpecialInstructions = "";
        var adtAdhocPaymentDate = "";
        var adtPaymentCycleDate = "";
        var aintDeletedPaymentDirectiveId = "";
        var Params = {};

        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

        if (ns.viewModel.currentModel.indexOf("wfmPersonMaintenance") === 0 || ns.viewModel.currentModel.indexOf("wfmPersonOverviewMaintenance") === 0) {
            lstrClickedButtonId = e.target.id;
            lstrButtonInfo = ldomDiv.find("#" + lstrClickedButtonId).val();

            if (lstrButtonInfo == "Month Of Suspendible Service Report") {
                lintPerson = ldomDiv.find("#txtPersonId").val();
                if (!confirm('Hours will get populated only if participant is retired or has attended age 65.Please click OK to continue.') == true) {
                    return false;
                }
            }
            else if (lstrButtonInfo == "Retrieve Annual Statement") {
                lintPerson = ldomDiv.find("#txtPersonId").val();
            }
            else {
                lintPerson = ldomDiv.find("#lblPersonId1").text();
            }
            Params = {
                "aintPersonID": lintPerson,
                "lstrButtonInfo": lstrButtonInfo
            };
        }
        else if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountMaintenance") === 0) {
            lintPerson = ldomDiv.find("#txtPersonId").val();
            lstrClickedButtonId = e.target.id;
            lstrButtonInfo = ldomDiv.find("#" + lstrClickedButtonId).val();
            aintPayeeAccountId = ldomDiv.find("#lblPayeeAccountId").text();
            astrSpecialInstructions = ldomDiv.find("#txtSpecialInstructions").val();
            adtAdhocPaymentDate = ldomDiv.find("#txtAdhocPaymentDate").val();
            astrModifiedBy = ldomDiv.find("#lbModifiedBy").text();

            if (lstrClickedButtonId == "btnRetrieveDirectives") {
                if (ldomDiv.find('#ddlPaymentCycleDate').val() !== undefined && ldomDiv.find('#ddlPaymentCycleDate').val() !== "") {
                    adtPaymentCycleDate = ldomDiv.find("#ddlPaymentCycleDate").val();
                }
                else {
                    nsCommon.DispalyError("Please select a date from below dropdown.", e.context.activeDivID);
                    return false;
                }
            }
            if (lstrClickedButtonId == "btnRetrieveDeletedDirectives") {
                if (ldomDiv.find('#ddlDeletedDirectiveCreatedDate').val() == null) {
                    nsCommon.DispalyError("Please select a date from below dropdown.", e.context.activeDivID);
                    return false;
                }
                else
                {
                    aintDeletedPaymentDirectiveId = ldomDiv.find("#ddlDeletedDirectiveCreatedDate").val() !== "" ? ldomDiv.find("#ddlDeletedDirectiveCreatedDate").val() : "0";
                }
            }

            Params = {
                "aintPersonID": lintPerson,
                "lstrButtonInfo": lstrButtonInfo,
                "aintPayeeAccountId": aintPayeeAccountId,
                "astrSpecialInstructions": astrSpecialInstructions,
                "adtAdhocPaymentDate": adtAdhocPaymentDate,
                "astrModifiedBy": astrModifiedBy,
                "adtPaymentCycleDate": adtPaymentCycleDate,
                "aintDeletedPaymentDirectiveId": aintDeletedPaymentDirectiveId
            };
        }
        else if (ns.viewModel.currentModel.indexOf("wfmIAPAllocationDetailMaintenance") === 0) {
            lstrClickedButtonId = e.target.id;
            lstrButtonInfo = ldomDiv.find("#" + lstrClickedButtonId).val();

            var fileName = ldomDiv.find("#lblFileName_0").text() + ".pdf";
            Params = {
                "FileName": fileName,
                "lstrButtonInfo": lstrButtonInfo
            };
        }

        var winName = 'MyWindow';
        var windowoption = 'title:hello;dialogWidth=800px;dialogHeight=800px;center=yes; help: no; resizable: yes; status: no; scrollbar=yes;';
        var lstrAction = ["SenderID=", ns.SenderID, "&SenderForm=", ns.SenderForm, "&Action=", "OpenPDF", "&SenderKey=", ns.SenderKey].join('');
        var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
        var url = [Prefix, "Home/OpenPDF?", lstrAction].join('');
        url = [url, "&WindowName=", window.name].join('');
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", url);
        form.setAttribute("target", winName);

        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'aobjDownload';
        input.value = JSON.stringify(Params);
        form.appendChild(input);
        var input_antiforgery = document.createElement('input');
        input_antiforgery.type = 'hidden';
        input_antiforgery.name = 'antiForgeryToken';
        input_antiforgery.value = $("#antiForgeryToken").val();
        form.appendChild(input_antiforgery);
        document.body.appendChild(form);
        window.open('', winName, windowoption);
        form.target = winName;
        form.submit();
        document.body.removeChild(form);
        return false;
    },

    processmanually: function (e) {
        var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);
        var txtIstrPaymentMethod = ldomDiv.find("#txtIstrPaymentMethod").val();
        if (txtIstrPaymentMethod == "Wire" || txtIstrPaymentMethod == "WIRE") {
            confirm('Please contact Accounting Department/Bank to process manually');
            return true;
        }
        return true;
    },
    AdverseInterestConfirm: function (e) {
        var ldomDiv = $("#" + e.context.activeDivID);
        if (ldomDiv.find('#chkAdverseInterest').is(':checked') == true) {
        //if (true) {
            return confirm('Please verify any possible Adverse Interest before approving.');
        }
        return true;
    },
    OpenAllRecords: function (e) {
        return true;
    },
    IsSelectedRecord: function (e) {
        if (e.context.activeDivID.indexOf("wfmJobScheduleMaintenance") === 0) {
            var larrRows = [];
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvJobScheduleDetail");
            larrRows = lobjGridWidget.getSelectedIndexes();
            if (larrRows.length === 0) {
                nsCommon.DispalyError("No records selected. Please select a record.", e.context.activeDivID);
                return false;
            }
        }
        return true;
    },
    ProcessAjaxCallResult: function (e, data) {
        if (data != undefined && data.SrcElement != undefined && (((data.SrcElement['id'] == 'btnSaveWizard') && !data.ActiveForm.indexOf("wfmRetirementMaintenance") == 0)
            || ((data.SrcElement['id'] == 'btnSaveACHDetails' || data.SrcElement['id'] == 'btnCancelRetirementApplicationACHDetails') && !data.ActiveForm.indexOf("wfmPayeeAccountACHDetailsWMaintenance") == 0)
            || ((data.SrcElement['id'] == 'btnSaveFedralWthHolding' || data.SrcElement['id'] == 'btnCancelRetirementApplicationFedralWthHolding') && !data.ActiveForm.indexOf("wfmPayeeAccountFedralWthHoldingMaintenance") == 0)
            || ((data.SrcElement['id'] == 'btnSaveStateWthHolding' || data.SrcElement['id'] == 'btnCancelRetirementApplicationStateWthHolding') && !data.ActiveForm.indexOf("wfmPayeeAccountStateWthHoldingMaintenance") == 0))) {
            data.SrcElement = undefined;
            ns.viewModel.srcElement = undefined;
        }
        return true;
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
            lbtnSelf = $(FormContainerID + " #" + ActiveDivID + " #" + MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID))[0];
            lintSelectedIndex = e.getAttribute("rowIndex");
        } else {
            lbtnSelf = ns.viewModel.srcElement;
            var FormContainerID = "#" + $(lbtnSelf).closest('div[role="group"]')[0].id;
            ActiveDivID = $(lbtnSelf).closest('div[id^="wfm"]')[0].id;
        }

        RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwRelatedControl", ActiveDivID);
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

    fnOpenReportPDF: function (e) {
        if (ns.viewModel.currentModel.indexOf("wfmReturnToWorkRequestMaintenance") === 0) {
            var ldomDiv = e.context.idomActiveDiv != undefined ? e.context.idomActiveDiv : $("#" + e.context.activeDivID);

            var lintPerson = ldomDiv.find("#lblPersonId").text();
            //var lintNotificationId = ldomDiv.find("#lblReemploymentNotificationId").text();
            var lintTrackingId = 0;

            var lobjBtnInfo = nsCommon.GetEventInfo(e);
            var ActiveDivID = lobjBtnInfo.ActiveDivID;
            var lbtnSelf = lobjBtnInfo.lbtnSelf;
            var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
            var ldictControlAttr = MVVMGlobal.GetControlAttribute(lbtnSelf, "GetAllAttr", ActiveDivID, true);
            ldictControlAttr = ldictControlAttr != null ? ldictControlAttr : {};
            var RelatedGridID = "";
            RelatedGridID = $(lbtnSelf).attr(nsConstants.SFW_RELATED_CONTROL) || ldictControlAttr[nsConstants.SFW_RELATED_CONTROL];
            var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
            if (lobjGridWidget != undefined) {
                var data = lobjGridWidget.getRowByIndex(lintSelectedIndex);
                if (data != undefined) {
                    lintTrackingId = data['PrimaryKey'];
                }
            }

            lstrClickedButtonId = e.target.id;
            lstrButtonInfo = ldomDiv.find("#" + lstrClickedButtonId).val();
            var Params = {
                "aintPersonID": lintPerson,
                "lstrButtonInfo": lbtnSelf.value,
                "aintTrackingId": lintTrackingId
                //"aintNotificationId": lintNotificationId
            };

            var winName = 'MyWindow';
            var windowoption = 'title:hello;dialogWidth=800px;dialogHeight=800px;center=yes; help: no; resizable: yes; status: no; scrollbar=yes;';
            var lstrAction = ["SenderID=", ns.SenderID, "&SenderForm=", ns.SenderForm, "&Action=", "OpenPDF", "&SenderKey=", ns.SenderKey].join('');
            var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
            var url = [Prefix, "Home/OpenPDF?", lstrAction].join('');
            url = [url, "&WindowName=", window.name].join('');
            var form = document.createElement("form");
            form.setAttribute("method", "post");
            form.setAttribute("action", url);
            form.setAttribute("target", winName);

            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'aobjDownload';
            input.value = JSON.stringify(Params);
            form.appendChild(input);
            var input_antiforgery = document.createElement('input');
            input_antiforgery.type = 'hidden';
            input_antiforgery.name = 'antiForgeryToken';
            input_antiforgery.value = $("#antiForgeryToken").val();
            form.appendChild(input_antiforgery);
            document.body.appendChild(form);
            window.open('', winName, windowoption);
            form.target = winName;
            form.submit();
            document.body.removeChild(form);
            return false;
        }
    },

    ChangeForSuspensionEndDateForCenterMiddle: function (e) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
        var lddlSuspensionReason = ldomDiv.find("#ddlCMSuspensionReasonValue");

        if (lddlSuspensionReason.length == 0)
            return;

        //Get the suspension end date only if suspension reason is PEND.
        if (lddlSuspensionReason[0].value != 'SRPD') {
            return;
        }

        var lblCMSuspendEndDate = ldomDiv.find("#lblCMSuspendEndDate");
        var txtCMSuspensionDate = ldomDiv.find("#txtCMSuspensionDate");
        if (lblCMSuspendEndDate != undefined && txtCMSuspensionDate != undefined) {
            var nextDay = new Date();
            nextDay.setDate(nextDay.getDate() + 1);
            lblCMSuspendEndDate[0].innerText = formatDate(nextDay);
            txtCMSuspensionDate.val(formatDate(nextDay)).trigger("change");
        }

    },

    ChangeForSuspensionEndDateForCenterLeft: function (e) {
        ldomDiv = $("#" + ns.viewModel.currentModel);
        var lddlSuspensionReason = $("#ddlCLSuspensionReasonValue");

        if (lddlSuspensionReason.length == 0)
            return;

        //Get the suspension end date only if suspension reason is PEND.
        if (lddlSuspensionReason[0].value != 'SRPD') {
            return;
        }

        var lblCLSuspendEndDate = $("#lblCLSuspensionDate");
        var txtCLSuspensionDate = $("#txtCLSuspensionDate");
        if (lblCLSuspendEndDate != undefined && txtCLSuspensionDate != undefined) {
            var nextDay = new Date();
            nextDay.setDate(nextDay.getDate() + 1);
            lblCLSuspendEndDate[0].innerText = formatDate(nextDay)
            txtCLSuspensionDate.val(formatDate(nextDay)).trigger("change");
        }
    },
    HideResumeButtonOnCaseInstance: function (e) {
        $("#FreezedControl_btnExecuteRefreshFromObject1").hide();
        return false;
    }
};
// END (Communication) Related JS

// START Reports Related JS
nsRptOpenReportClient = nsRpt.OpenReportClient;
nsRpt.OpenReportClient = function (data) {
    //    // Making ReportFrame Body Empty before navigating to Page. -- PIR - 16752
    $("#ReportFrame").contents().find("body").html('');
    nsRptOpenReportClient(data);
    nsCommon.SetTitle("Reports");
}


// End FS006 Report Related JS

nsCommon.SetTitle = function (astrTitle) {
    $("#FormTitle").html(astrTitle);
    document.title = astrTitle;
    $("#LookupFormTitle").text(astrTitle);
    $('#lblForm').html(ns.viewModel.currentModel);
};

nsCommon.SetFirstItemTextBase = nsCommon.SetFirstItemText;
nsCommon.SetFirstItemText = function (sfwDropDown, DataToBind, astrActiveDivID, adictListControlAttr) {
    if (adictListControlAttr == undefined)
        adictListControlAttr = {};
    nsCommon.SetFirstItemTextBase(sfwDropDown, DataToBind, astrActiveDivID, adictListControlAttr);
}

nsEvents.btnRefreshServers_Click = function (e) {
    var lobjAjaxData = { action: "RefreshAllAppServers" };
    return nsCommon.GetAjaxRequest(lobjAjaxData);
}
function GetCurrentDateTime(dt) {
    var res = "";
    res += formatdigits(dt.getMonth() + 1);
    res += "/";
    res += formatdigits(dt.getDate());
    res += "/";
    res += formatdigits(dt.getFullYear());
    return res;
}
function formatdigits(val) {
    val = val.toString();
    return val.length == 1 ? "0" + val : val;
}

function GetPlanExists(Planid, BenType, Prikarykey, ldomDiv) {
    var Params = {
        "astrPlanId": Planid,
        "astrBenType": BenType,
        "aintPrimaryKey": Prikarykey,
    };
    var result = nsCommon.SyncPost("GetPlanExists", Params, "POST");
    if (result == "100") {
        ldomDiv.find("#txtDistPercent").val(100).trigger('change');
    }
    else if (result == "50") {
        ldomDiv.find("#txtDistPercent").val(50).trigger('change');
    }
}

function RetirementWizardHideControls(ldomDiv) {
    ldomDiv.find("#capEligibleIAPPlan,#txtEligibleIAPPlan,#capIAPBenefitOption,#ddlIAPBenefitOpt,#capIAPSpouseConsent,#chkIAPSpouseConsent,#lblIAPJointAnnunantName,#ddlIAPJointAnnunantName").hide();
    ldomDiv.find("#capEligiblePlan,#txtEligiblePlan,#capBenefitOption,#ddlBenefitOpt,#capSpouseConsent,#chkSpouseConsent,#lblJointAnnunantName,#ddlJointAnnunantName").hide();
}
function RetirementWizardFedTaxHolding(ldomDiv) {
    ldomDiv.find("#lblWtxtStep2B,#txtWStep2B,#lbltxtWStep3Amount,#txtWStep3Amount,#lbltxtWStep4a,#txtWStep4a,#lblWtxtStep4b,#txtWStep4b,#lblWtxtStep4c,#txtWStep4c").show();
    ldomDiv.find("#lblwtxtTaxAllowance,#txtwTaxAllowance,#lblwtxtAdditionalTaxAmount1,#txtwAdditionalTaxAmount1,#lblwtxtTaxPercentage,#txtwTaxPercentage").hide();
}
function GetTaxWithHoldingScreenConfiguratorColumns(ldomDiv, TaxIdentifierVal, BenDistType) {
    var lddlTaxOption = ldomDiv.find("#ddlTaxOptionValue11");
    var currrent_taxoption = lddlTaxOption.val();

    var Params = {
        "astrTaxIdentifierVal": TaxIdentifierVal,
        "astrBenType": BenDistType,
    };
    var result = nsCommon.SyncPost("GetTaxWithHoldingScreenConfiguratorColumns", Params, "POST");
    if (result != undefined) {
        var obj = jQuery.parseJSON(result);

        $(obj).each(function (i, val) {
            //collection keys are table column alias names - do not change the column name.
            $.each(val, function (key, val) {
                //alert(key, val);
                //alert(currrent_taxoption);
                if (ldomDiv.find("#"+key).length > 0) {
                    if (val != "Y") {

                        ldomDiv.find("#" + key).hide()
                        ldomDiv.find("#lbl" + key).hide()
                    } else {

                        ldomDiv.find("#lbl" + key).show();
                        ldomDiv.find("#" + key).show();
                    }

                }

                if (TaxIdentifierVal == "VAST") {
                    ldomDiv.find("#ddlMaritalStatusValue11").hide();
                    if (currrent_taxoption == "FLAD") {
                        ldomDiv.find("#lbltxtVoluntary_Withholding,#txtVoluntary_Withholding").show();
                        ldomDiv.find("#lbltxtPersonalExemptions,#txtPersonalExemptions,#lbltxtAgeandBlindnessExemptions,#txtAgeandBlindnessExemptions").hide();
                    } else {
                        ldomDiv.find("#lbltxtVoluntary_Withholding,#txtVoluntary_Withholding").hide();
                        ldomDiv.find("#lbltxtPersonalExemptions,#txtPersonalExemptions,#lbltxtAgeandBlindnessExemptions,#txtAgeandBlindnessExemptions").show();
                    }

                    if (currrent_taxoption == "STAT") {
                        ldomDiv.find("#lbltxtAdditionalTaxAmount1,#txtAdditionalTaxAmount1").show();
                    } else {
                        ldomDiv.find("#lbltxtAdditionalTaxAmount1,#txtAdditionalTaxAmount1").hide();
                    }
                }
            });
        });
    }
}

function GetLaserFische(ldomDiv) {
    if (ldomDiv == undefined) {
        ldomDiv = $('#' + ns.viewModel.currentModel);
    }
    var Params = {
    };
    var result = nsCommon.SyncPost("GetLaserFicheUrlfromDB", Params, "POST");
    if (result != undefined) {
        var obj = jQuery.parseJSON(result);
        $(obj).each(function (i, val) {
            //collection keys are table column alias names - do not change the column name.
            $.each(val, function (key, val) {
                window.open("" + val + "#view=search;search={[]:[Department] = \"Retirement Benefits\"} & {[]:[MPID] = \"" + ldomDiv.find("#btnMpiPersonId").text() + "\"}", "OpenURL", "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes", null);
            });
        });
    }
}
var FM_CorrDropDownChange = nsCorr.CorrDropDownChange;
nsCorr.CorrDropDownChange = function (e) {
    if (nsCorr.CurrentCorr.CallingForm != "" && nsCorr.CurrentCorr.CallingForm == "wfmPayeeAccountMaintenance") {
        FillCorrpondenceNames();
    }
    FM_CorrDropDownChange(e);
    if (nsCorr.CurrentCorr.CorrTemplate != undefined && nsCorr.CurrentCorr.CorrTemplate != "" && (nsCorr.CurrentCorr.CorrTemplate == "RETR-0003" || nsCorr.CurrentCorr.CorrTemplate == "RETR-0056")) {
        FillPlanValuesCorres();
    }
}

function FillCorrpondenceNames() {
    var ddlCorrespondence = $([nsConstants.HASH, nsCorr.CurrentCorr.CorrDivID].join("")).find([nsConstants.HASH, nsConstants.DDL_CORRESPONDENCE_LIST].join(''));
    if (ns.SenderID != "" && ns.SenderID == "btnConfirmationLetter") {
        var ldomDIv = $(nsCommon.GetActiveDivElement(ns.viewModel.srcElement));
        if (ldomDIv.find('#lbIintPlanId').text() == "1") {
            ddlCorrespondence.empty();
            if (ldomDIv.find('#lblIstrMoreInformation').text() && ldomDIv.find('#lblIstrMoreInformation').text().includes("Hardship")) {
                ddlCorrespondence.append($('<option></option>').val("PAYEE-0054").html("2025 IAP Hardship Payment Confirmation"));
            }
            else {
                ddlCorrespondence.append($('<option></option>').val("RETR-0042").html("Confirmation of Payment"));
            }
        }
        else {
            ddlCorrespondence.empty();
            ddlCorrespondence.append($('<option></option>').val("PAYEE-0005").html("Confirmation Letter"));
        }
    }
    if (ns.SenderID != "" && ns.SenderID == "btnVerificationLetter") {
        ddlCorrespondence.empty();
        ddlCorrespondence.append($('<option></option>').val("PAYEE-0027").html("Pension Income Verification"));
    }
}

function FillPlanValuesCorres() {
    var lrowGridRecord = nsCommon.GetWidgetByActiveDivIdAndControlId(nsCorr.CurrentCorr.ParentActiveDivId, "grvPersonAccount").jsObject.RenderData;
    var ddlPlanCode = $([nsConstants.QUERY_BOOKMARK_HOLDER, nsConstants.SPACE, nsConstants.QUERY_BOOKMARK_DIV].join('')).find('#PLAN');
    ddlPlanCode.append($("<option></option>").val("").html(""));
    $.each(lrowGridRecord, function (index, item) {
        var PlanCode = item[Object.keys(item).filter(k => k.startsWith('dt_PlanCode_0_0'))[0]];
        if (PlanCode == "Local600") {
            ddlPlanCode.append($("<option></option>").val("Local 600").html("Local 600"));
        }
        if (PlanCode == "Local666") {
            ddlPlanCode.append($("<option></option>").val("Local 666").html("Local 666"));
        }
        if (PlanCode == "Local700") {
            ddlPlanCode.append($("<option></option>").val("Local 700").html("Local 700"));
        }
        if (PlanCode == "Local52") {
            ddlPlanCode.append($("<option></option>").val("Local 52").html("Local 52"));
        }
        if (PlanCode == "Local161") {
            ddlPlanCode.append($("<option></option>").val("Local 161").html("Local 161"));
        }
    });
}

function fnGetParticipantAddress() {
    var lrowGridRecord = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, "grvParticipantAddress").jsObject.RenderData;;
    var ldomDiv = $('#' + ns.viewModel.currentModel);
    
    $.each(lrowGridRecord, function (index, item) {
        
        ldomDiv.find("#txtAddrLine1").val(item["dt_AddressLine1_0_0"]).trigger("change");
        ldomDiv.find("#txtAddrLine2").val(item["dt_AddressLine2_1_0"]).trigger("change");
        ldomDiv.find("#ddlCountryValue").val(item["dt_CountryValue_2_0"]).trigger("change");
        ldomDiv.find("#ddlStateValue").val(item["dt_StateValue_3_0"]).trigger("change");
        ldomDiv.find("#txtCity").val(item["dt_City_4_0"]).trigger("change");
        ldomDiv.find("#txtZipCode").val(item["dt_ZipCode_5_0"]).trigger("change");
        ldomDiv.find("#txtZip4Code").val(item["dt_Zip4code_6_0"]).trigger("change");
        ldomDiv.find("#txtContactName").val(item["dt_ContactName_7_0"]).trigger("change");
      
    });
}

var base_btnOpen_Click = nsEvents.btnOpen_Click;
nsEvents.btnOpen_Click = function (e, aobjAdditionalParams) {
    var LoggedInUserIsVIP = $("#lblLoggedInUserIsVIP").val();
    if (LoggedInUserIsVIP != "" && LoggedInUserIsVIP != "VIPAccessUser") {
        var lobjBtnInfo = nsCommon.GetEventInfo(e);
        var ActiveDivID = lobjBtnInfo.ActiveDivID;
        var lbtnSelf = lobjBtnInfo.lbtnSelf;
        var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
        var ldictControlAttr = MVVMGlobal.GetControlAttribute(lbtnSelf, "GetAllAttr", ActiveDivID, true);
        ldictControlAttr = ldictControlAttr != null ? ldictControlAttr : {};
        var RelatedGridID = "";
        RelatedGridID = $(lbtnSelf).attr(nsConstants.SFW_RELATED_CONTROL) || ldictControlAttr[nsConstants.SFW_RELATED_CONTROL];
        var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
        if (lobjGridWidget != undefined) {
            var data =  lobjGridWidget.getRowByIndex(lintSelectedIndex);
            if (data != undefined) {
                for (var dataField in data) {
                    if (dataField.indexOf("RelativeVipFlag") > 0) {
                        if (data[dataField] != "" && data[dataField] == "Y") {
                            $("#btnjQueryCancel").click(function (e) {
                                $("#overlay").hide();
                                $("#dialog").fadeOut(300);
                                e.preventDefault();
                            });
                            $("#overlay").show();
                            $("#dialog").fadeIn(300);
                            $("#overlay").off("click");
                            return false;
                        }
                    }
                }
            }
        }
    }
    base_btnOpen_Click(e, aobjAdditionalParams);
}

var FM_bindSummary = ns.bindSummary;
ns.bindSummary = function(data, divId, ablnFromFocusOut) {
    if (divId && divId.indexOf("wfmOrganizationBankMaintenance") >= 0 &&
        data.ValidationSummary.length != 0 && data.ValidationSummary != undefined && data.ValidationSummary != null &&
        data.ValidationSummary[0]["istrErrorMessage"].indexOf("Account No - Invalid value:") >= 0 && data.ValidationSummary[0]["istrErrorID"] == "120") {
        data.ValidationSummary[0]["istrErrorID"] = "113";
        data.ValidationSummary[0]["istrErrorMessage"] = " Account No cannot enter more than 20 characters";
    }
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    if ((ns.viewModel.currentModel.indexOf("wfmRetirementApplicationMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDisabilityApplicationMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmDeathPreRetirementMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmWithdrawalApplicationMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0 || ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0) &&
        (data.ValidationSummary.length != 0 && data.ValidationSummary != undefined && data.ValidationSummary[0]["istrErrorID"] == "4076") && ns.SenderID == "btnAddNotes") {
        var MessageDiv = ldomDiv.find("#GlobalMessageDiv").first();
        MessageDiv.html('Msg ID : 30 [ Errors found. Record not saved. ]');
    }
    if (ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationPreRetirementDeathMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmQDROCalculationMaintenance") == 0 ||
        ns.viewModel.currentModel.indexOf("wfmWithholdingInformationMaintenance") == 0) {
        var istrMessageAfterParentGridClick = sessionStorage.getItem('lstrMessageAfterParentGridClick');
        var islsExecutedStatus = sessionStorage.getItem('lsExecuted');
        if (islsExecutedStatus != "true") {
            if (istrMessageAfterParentGridClick != undefined && istrMessageAfterParentGridClick != null) {
                ablnFromFocusOut = true;
                sessionStorage.removeItem('lstrMessageAfterParentGridClick');
                sessionStorage.setItem('lsExecuted', "true");
            }
        }
        if (islsExecutedStatus == "true") {
            ablnFromFocusOut = true;
        }
    }
    FM_bindSummary(data, divId, ablnFromFocusOut);
}

var FM_DispalyMessage = nsCommon.DispalyMessage;
nsCommon.DispalyMessage = function (astrMessage, ActiveDivID, ablnScrollToTop) {

    if (astrMessage.contains("Records met the search criteria.") && (ns.viewModel.currentForm && (ns.viewModel.currentForm.indexOf("wfmPersonLookup") >= 0 || ns.viewModel.currentForm.indexOf("wfmBenefitApplicationLookup") >= 0) ||
        ns.viewModel.currentForm.indexOf("wfmBeneficiaryLookup") >= 0 || ns.viewModel.currentForm.indexOf("wfmDeathNotificationLookup") >= 0 ||
        ns.viewModel.currentForm.indexOf("wfmBenefitCalculationLookup") >= 0 || ns.viewModel.currentForm.indexOf("wfmSSNMergeLookup") >= 0 )) {

        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var lstrSSN = "";
        var lstrOldSsn = ldomDiv.find('#txtSsn');
        var lstrOldSSN = ldomDiv.find('#txtSSN');
        
        var lstrMPID = "";
        var lstrOldMPID = ldomDiv.find("#txtMpiPersonId").val();
        if (ns.viewModel.currentModel.indexOf("wfmPersonLookup") === 0)
            lstrOldMPID = ldomDiv.find("#txtMpiPersonId2").val();

        if (lstrOldSsn != null && lstrOldSsn.length > 0) {
            lstrSSN = lstrOldSsn.val();
        }
        else if (lstrOldSSN != null && lstrOldSSN.length > 0) {
            lstrSSN = lstrOldSSN.val();
        }
        if (lstrOldMPID != null)
            lstrMPID = lstrOldMPID;

        var lParam = {
            "astrSsn": lstrSSN,
            "astrMPID": lstrMPID
        }
        
        if (lstrSSN != '') {
            lvarResult = nsRequest.SyncPost("FindPersonFromSSNAndMPID", lParam, "POST");
            var lNewPersonMPID = lvarResult;
            if (lNewPersonMPID != '')
                astrMessage = ("[ This SSN is merged to MPID" + ": " + lNewPersonMPID + ". ]");
        }
        if (lstrMPID != '') {
            lvarResult = nsRequest.SyncPost("FindPersonFromSSNAndMPID", lParam, "POST");
            var lNewPersonMPID = lvarResult;
            if (lNewPersonMPID != '')
                astrMessage = ("[ This SSN is merged to MPID" + ": " + lNewPersonMPID + " ]");
        }
    }

    setTimeout(function () {
        if ((ns.viewModel.currentModel.indexOf("wfmBenefitCalculationLookup") == 0 || ns.viewModel.currentModel.indexOf("wfmDROCalculationLookup") == 0) && astrMessage.indexOf("[ No records met the search criteria.  Please change the criteria and search again. ]")) {
            if ($('ul[class="breadcrumb"]').length > 0) {
                $("#ContentSplitter").attr("style", "padding-top: 30px;");
            }
        }
    },1);

    if ((ns.viewModel.currentModel.indexOf("wfmAuditLogDetailMaintenance") >= 0 || ns.viewModel.currentModel.indexOf("wfmQDROOffsetDetailsMaintenance") >= 0 ) && astrMessage.indexOf("[Record displayed.Please make changes and press SAVE. ]")) {
        astrMessage = " Record displayed. ";
    }
    if (ns.viewModel.currentModel.indexOf("wfmPayeeAccountMaintenance") >= 0 && ns.viewModel.srcElement && ns.viewModel.srcElement.id =="btnButton3" && astrMessage.indexOf(" [ All changes successfully saved. ]") == 0) {
        astrMessage = "Msg ID : 7 [ Record displayed. Please make changes and press SAVE. ]";
    }

    if (ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") >= 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") >= 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationPreRetirementDeathMaintenance") >= 0 ||
        ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") >= 0 ||
        ns.viewModel.currentModel.indexOf("wfmWithholdingInformationMaintenance") >= 0) {
        var lstrMsg = sessionStorage.getItem('IstrPreservedMessage');
        if (lstrMsg != undefined && lstrMsg == astrMessage) {
        }
        else {
            sessionStorage.setItem('IstrPreservedMessage', astrMessage);
        }
    }
    
    if (ns.viewModel.currentModel.indexOf("wfmQDROCalculationMaintenance") >= 0) {
        var lsGridSelected = sessionStorage.getItem('lsGridSelected');
        var lstrMsg = sessionStorage.getItem('IstrPreservedMessage');

        if (astrMessage == " [ Existing item selected in the grid. ]" && lsGridSelected == "false") {
            astrMessage = lstrMsg;
            sessionStorage.removeItem('lsGridSelected');
            return;
        }
        if (lstrMsg != undefined && lstrMsg == astrMessage) {
        }
        else {
            sessionStorage.setItem('IstrPreservedMessage', astrMessage);
        }
    }
    
    FM_DispalyMessage(astrMessage, ActiveDivID, ablnScrollToTop);
}
var FM_DispalyError = nsCommon.DispalyError;
nsCommon.DispalyError = function (astrMessage, ActiveDivID, ablnScrollToTop) {
    FM_DispalyError(astrMessage, ActiveDivID, ablnScrollToTop);
    if (ActiveDivID != null && ActiveDivID != undefined && ((ActiveDivID.indexOf("wfmDashboardMaintenance") == 0 && ns.SenderID == "btnButton1") || (ActiveDivID.indexOf("wfmUserDashboardMaintenance") == 0 || ActiveDivID.indexOf("wfmSupervisorDashboardMaintenance") == 0) && ns.SenderID == "btnExecuteRefreshFromObject"))
    {
        if (astrMessage != null && astrMessage != undefined && astrMessage != "" && astrMessage.contains("Please enter all required fields.")) {

            $("#" + ActiveDivID).find("#GlobalMessageDiv").html('');
            $("#" + ActiveDivID).find("#GlobalMessageDiv").append("Date From is greater than Date To must be less than or equal to .<br>Date To should be Greater Than or Equal to Date From .").attr("style", "line-height:20px;");

        }
    }
}

function btnDelete_Click(e) {
    var lobjBtnInfo = nsCommon.GetEventInfo(e);
    var ActiveDivID = lobjBtnInfo.ActiveDivID;
    var lbtnSelf = lobjBtnInfo.lbtnSelf;
    var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
    var lblnIsLookUp = ActiveDivID.indexOf(nsConstants.LOOKUP) > 0;
    if (ActiveDivID.indexOf("_retrieve") > 0) {
        ns.displayActivity(false);
        ns.displayCenterleftActivity(false);
        return;
    }
    var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
    if (RelatedGridID == null) {
        alert(DefaultMessages.GridNotFound);
        return false;
    }
    var lobjRelatedControl = nsCommon.CheckGridOrListView(ActiveDivID, RelatedGridID);
    if (lobjRelatedControl.NotFound)
        return false;
    if (CheckForDisplayOnlyGrid(ActiveDivID, lobjRelatedControl.RelatedControlId)) {
        return;
    }
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, lobjRelatedControl.RelatedControlId);
    if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
        return false;
    }
    var sfwMessageId = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageId", ActiveDivID);
    if (sfwMessageId === undefined || sfwMessageId === null) {
        sfwMessageId = 0;
    }
    var larrRows = [], arrSelectedRows = [];
    if (lintSelectedIndex > -1) {
        var lblnIsLnkBtn = lbtnSelf.getAttribute("linkbutton");
        var lblnIsImgBtn = lbtnSelf.getAttribute("imagebutton");
        if ((lblnIsLnkBtn != undefined && lblnIsLnkBtn.trim().toLowerCase() == "true") || (lblnIsImgBtn != undefined && lblnIsImgBtn.trim().toLowerCase() == "true")) {
            var lblnMultiRow = lobjGridWidget.iSMultipleRow(lintSelectedIndex);
            if (lblnMultiRow) {
                alert(DefaultMessages.UnselectRows);
                return false;
            }
        }
        if (lblnIsLookUp) {
            larrRows.push(lobjGridWidget.getRowPropertyByIndex(lintSelectedIndex, nsConstants.istrObjectPrimaryKey));
        }
        else {
            larrRows.push(Number(lintSelectedIndex));
            arrSelectedRows.push(lobjGridWidget.getRowByIndex(lintSelectedIndex));
        }
        var larrSelRows = lobjGridWidget.getSelectedRows();
        for (var i = 0; i < larrSelRows.length; i++) {
            lobjGridWidget.checkRow(larrSelRows[i], false);
        }
        lobjGridWidget.setRowPropertyByIndex(lintSelectedIndex, "rowSelect", true);
    }
    else {
        if (lblnIsLookUp) {
            var larrSelRows = lobjGridWidget.getSelectedRows();
            for (var i = 0; i < larrSelRows.length; i++) {
                larrRows.push(larrSelRows[i][nsConstants.istrObjectPrimaryKey]);
            }
        }
        else {
            larrRows = lobjGridWidget.getSelectedIndexes();
            arrSelectedRows = lobjGridWidget.getSelectedRows();
        }
    }
    if (larrRows.length === 0) {
        var sfwMessageNoRowSelected = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageNoRowSelected", ActiveDivID);
        if (sfwMessageNoRowSelected == null)
            sfwMessageNoRowSelected = DefaultMessages.NoRowSelectedforGridViewDelete;
        nsCommon.DispalyError(sfwMessageNoRowSelected, ActiveDivID);
        return false;
    }
    if (larrRows.length > 0) {
        var result = true;
        if (!lblnIsLookUp && ns.DirtyData[ActiveDivID] != undefined) {
            result = confirm(DefaultMessages.DeleteConfirmationIfUnsaved);
        }
        else {
            var sfwMessageActionConfirmation = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageActionConfirmation", ActiveDivID);
            if (sfwMessageActionConfirmation == null)
                sfwMessageActionConfirmation = DefaultMessages.DeleteConfirmation;
            result = confirm(sfwMessageActionConfirmation);
        }
        if (!result) {
            lobjGridWidget.setRowPropertyByIndex(lintSelectedIndex, "rowSelect", false);
            return;
        }
    }
    var ldomElement = lobjGridWidget.idomGridElement;
    var ObjectID = lobjGridWidget.iobjAllAttrs["sfwObjectID"] || ldomElement.getAttribute("sfwObjectID");
    if (!lblnIsLookUp && arrSelectedRows != undefined && arrSelectedRows.length > 0) {
        var lobjRows = {};
        lobjRows[[ActiveDivID, "~", ldomElement.id].join("")] = arrSelectedRows;
        nsCommon.sessionSet("GridRowLatDelete", lobjRows);
    }
    var DeleteParams = {};
    DeleteParams["RelatedGridID"] = RelatedGridID;
    DeleteParams["SelectedRows"] = larrRows;
    DeleteParams["ObjectID"] = ObjectID;
    DeleteParams["CollectionOf"] = lobjGridWidget.istrCollectionOf || "";
    DeleteParams["ActiveDivID"] = ActiveDivID;
    DeleteParams["MessageID"] = sfwMessageId;
    if (!lblnIsLookUp) {
        var formDirtyData = { HeaderData: {}, DetailsData: {} };
        if (ns.DirtyData[ActiveDivID] !== undefined) {
            if (ns.DirtyData[ActiveDivID].HeaderData !== undefined) {
                formDirtyData.HeaderData = ns.DirtyData[ActiveDivID].HeaderData;
            }
            if (ns.DirtyData[ActiveDivID].DetailsData !== undefined) {
                formDirtyData.DetailsData = ns.DirtyData[ActiveDivID].DetailsData;
            }
        }
        DeleteParams["ResponseData"] = formDirtyData;
    }
    if (ns.viewModel[ActiveDivID] != undefined && ns.viewModel[ActiveDivID] != null && ns.viewModel[ActiveDivID].ExtraInfoFields != undefined && ns.viewModel[ActiveDivID].ExtraInfoFields != null) {
        DeleteParams["IsNewForm"] = ns.viewModel[ActiveDivID].ExtraInfoFields["IsNewForm"] === nsConstants.TRUE;
    }
    if (ActiveDivID.indexOf("Wizard") > 0) {
        var istrWizardStpID = $(lbtnSelf).closest(nsConstants.STEPDIV_CONTROL_TYPE_SELECTOR)[0].id;
        istrWizardStpID = istrWizardStpID.replace(nsConstants.VERTICAL_WIZARD_DIV_SUFFIX, '');
        DeleteParams["istrWizardStpID"] = istrWizardStpID;
    }
    var dataItem = nsCommon.GetDataItemFromDivID(ActiveDivID);
    if (ActiveDivID.indexOf(nsConstants.LOOKUP) == -1) {
        DeleteParams["PrimaryKey"] = dataItem.PrimaryKey;
    }
    var lobjAjaxData = { action: ["DeleteRecord?astrFormID=", lobjGridWidget.iobjApplyUIData.istrFormName].join(""), param: DeleteParams, PrevActiveForm: ActiveDivID, ActiveForm: ActiveDivID, SrcElement: lbtnSelf };
    var nsDeferred = nsCommon.GetAjaxRequest(lobjAjaxData);
    var lobjParentNode = dataItem.parentNode();
    if (dataItem !== undefined && lobjParentNode !== undefined) {
        ns.arrNeedToRefresh[lobjParentNode.divID] = true;
    }
    return nsDeferred;
}
nsEvents.btnDelete_Click = btnDelete_Click;

function CheckChange(FromChkChanged) {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var chkMinDistributionFlag = ldomDiv.find("#chkMinDistributionFlag").is(':checked');
    var lblMinDistributionDate = ldomDiv.find("#lblMinDistributionDate").text();
    var txtRetirementDate = ldomDiv.find("#txtRetirementDate"); var lblintIsManager = ldomDiv.find("#lblintIsManager").text();
    if (chkMinDistributionFlag == true && FromChkChanged) {
        if (txtRetirementDate.val() != lblMinDistributionDate) {
            ldomDiv.find("#txtRetirementDate").val(lblMinDistributionDate).trigger('change');
        }
        if (lblintIsManager == 1) {
            txtRetirementDate.prop("disabled", false);
        }
        else {
            txtRetirementDate.prop("disabled", true);
        }
    }
    else if (chkMinDistributionFlag == true && !FromChkChanged) {
        if (lblintIsManager == 1) {
            txtRetirementDate.prop("disabled", false);
        }
        else {
            txtRetirementDate.prop("disabled", true);
        }
    }
    else {
        txtRetirementDate.prop("disabled", false);
    }
}

function OnContactTypeChangeFunction() {
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var lddlConatct = ldomDiv.find('#ddlContactTypeValue');
    if (lddlConatct != undefined && lddlConatct[0] != undefined && lddlConatct[0].length > 0) {
        var contact_type = lddlConatct[0].value;
        if (contact_type != null && contact_type[0].length > 0) {

            if (ldomDiv.find("#chkAddrSameAsPerson").is(":checked")) {
                ldomDiv.find("#pnlAddSamsAsPerson").show();
                ldomDiv.find("#pnlAddress,#capCorrespondenceAddrFlag,#chkCorrespondenceAddrFlag").hide();
            }
            else {
                var lstrClientVisi = MVVMGlobal.GetControlAttribute(lddlConatct, "GetAllAttr", "", true)
                var lControlList = nsVisi.GetControlsToChangeState(lstrClientVisi['sfwClientVisibility'], lddlConatct[0].value);
                if (lControlList != null && lControlList.length == 2) {

                    for (var i = 0; i < lControlList[0].length; i++) {
                        ldomDiv.find("#" + lControlList[0][i]).show();
                    }

                    for (var i = 0; i < lControlList[1].length; i++) {
                        ldomDiv.find("#" + lControlList[1][i]).hide();
                    }

                    ldomDiv.find("#pnlAddSamsAsPerson").hide();
                    ldomDiv.find("#pnlAddress").show();

                }
            }
        }
    }
}

function VisiOnChildSupportFlag() {
    $("#" + ns.viewModel.currentModel).find('#lblSpecialAccounts,#ddlIstrSubPlan').hide();
}

function VisiOnEmergencyPaymentFlag() {
    
    var ldomDiv = $("#" + ns.viewModel.currentModel);
    var checked = ldomDiv.find("#chkEmergencyOneTimePayment").is(':checked');
    var ddlWithdrawalType = ldomDiv.find("#ddlWithdrawalType option:selected").val();
    if ((checked != undefined && checked == true) || (ddlWithdrawalType!=undefined && ddlWithdrawalType!="")) {
        ldomDiv.find('#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc').show();
    }
    else {
        ldomDiv.find('#capCOVIDWithdrawalAmount,#txtIdecCOVIDWithdrawalAmount,#capCOVIDFedTaxPerc,#txtCOVIDFederalPerc,#capCOVIDStateTaxPerc,#txtCOVIDStatePerc').hide();
    }
}
//ns.BindDetailViewFM = ns.BindDetailView;
//ns.BindDetailView = function (data) {
//    var lstrAction = (data != undefined && data.LastExecutedAction != undefined && data.LastExecutedAction != "") ? data.LastExecutedAction : ns.settings.data.action;
//    if (lstrAction.indexOf("GridItemSelect") >= 0) {
//        ns.blnSetValueWhileLoading = true;
//    }
//    ns.BindDetailViewFM(data);
//    ns.blnSetValueWhileLoading = false;
//}
function CheckForDisplayOnlyGrid(ActiveDivID, RelatedGridID) {
    var ldomFormContainer = $([nsConstants.HASH, ActiveDivID].join(''));
    var ldomRelatedGrid = ldomFormContainer.find([nsConstants.HASH, RelatedGridID].join(''));
    var lblnGridDisplayOnly = MVVMGlobal.GetControlAttribute(ldomRelatedGrid, "sfwDisplayOnly", ActiveDivID);
    if (lblnGridDisplayOnly != null && lblnGridDisplayOnly.toLowerCase() == 'true') {
        nsCommon.DispalyMessage("Cannot perform the operation, grid is marked as display only.", ActiveDivID);
        return true;
    }
    return false;
}
nsEvents.CheckForDisplayOnlyGrid = CheckForDisplayOnlyGrid;

function btnGridViewAddUpdate_Click(e) {
    var ActiveDivID = "";
    var lbtnSelf;
    lbtnSelf = ns.viewModel.srcElement;
    ActiveDivID = nsCommon.GetActiveDivId(lbtnSelf);
    var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
    if (RelatedGridID == null) {
        alert(DefaultMessages.GridNotFound);
        return;
    }
    if (CheckForDisplayOnlyGrid(ActiveDivID, RelatedGridID)) {
        return;
    }
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);
    if (lobjGridWidget == undefined || lobjGridWidget.jsObject == undefined) {
        return false;
    }
    var grid = $([nsConstants.HASH, ActiveDivID, nsConstants.SPACE_HASH, nsConstants.GRID_TABLE_UNDERSCORE, RelatedGridID].join(''));
    var lstrMethod = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMethodName", ActiveDivID);
    var lintSelectedIndex = grid.attr("SelectedIndex");
    if (lstrMethod == "btnGridViewUpdate_Click" && lintSelectedIndex == null) {
        var sfwMessageNoRowSelected = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageNoRowSelected", ActiveDivID);
        if (sfwMessageNoRowSelected == null)
            sfwMessageNoRowSelected = DefaultMessages.NoRowSelectedForUpdate;
        nsCommon.DispalyError(sfwMessageNoRowSelected, ActiveDivID);
        return false;
    }
    var sfwMessageId = MVVMGlobal.GetControlAttribute(lbtnSelf, "sfwMessageId", ActiveDivID);
    if (sfwMessageId === undefined || sfwMessageId === null) {
        sfwMessageId = 0;
    }
    var FormName = nsCommon.GetFormNameFromDivID(ActiveDivID);
    var GridControls = [];
    var Attributes = ns.Templates[FormName].ControlAttribites;
    for (var key in Attributes) {
        if (Attributes[key].sfwRelatedGrid != undefined && Attributes[key].sfwRelatedGrid == RelatedGridID) {
            GridControls.push(key);
        }
    }
    var DataToSend = { ResponseData: { HeaderData: { MaintenanceData: {} } }, RefreshObjParams: {}, NavigationParams: {}, PassSelectedRowsParams: {} };
    if (ns.DirtyData[ActiveDivID] !== undefined) {
        if (ns.DirtyData[ActiveDivID].HeaderData !== undefined) {
            if (ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData != undefined) {
                for (var keyM in ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData) {
                    if (keyM.indexOf("istrEV") == -1) {
                        if (ns.viewModel[ActiveDivID] != undefined && ns.viewModel[ActiveDivID].ListControlData != undefined && ns.viewModel[ActiveDivID].ListControlData[keyM] != undefined && ns.viewModel[ActiveDivID].ListControlData[keyM].istrEV != undefined) {
                            ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData[keyM + "_istrEV"] = ns.viewModel[ActiveDivID].ListControlData[keyM].istrEV;
                        }
                        var control = $(["#", ActiveDivID].join("")).find("#" + keyM);
                        if (control.data("istrEV") != undefined) {
                            ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData[keyM + "_istrEV"] = control.data("istrEV");
                        }
                    }
                }
            }
            DataToSend.ResponseData.HeaderData = ns.DirtyData[ActiveDivID].HeaderData;
        }
        if (ns.DirtyData[ActiveDivID].DetailsData !== undefined) {
            DataToSend.ResponseData.DetailsData = ns.DirtyData[ActiveDivID].DetailsData;
        }
    }
    var istrInitiator = MVVMGlobal.GetControlAttribute(lbtnSelf, "id", ActiveDivID);
    DataToSend.istrInitiator = istrInitiator;
    var ObjectID = MVVMGlobal.GetControlAttribute(grid, "sfwObjectID", ActiveDivID);
    var SelectParams = {};
    SelectParams["RelatedGridID"] = RelatedGridID;
    SelectParams["PrimaryKey"] = ns.viewModel[ActiveDivID].KeysData["PrimaryKey"];
    SelectParams["ObjectID"] = ObjectID;
    SelectParams["CollectionOf"] = lobjGridWidget.istrCollectionOf || "";
    SelectParams["SelectedIndex"] = lintSelectedIndex;
    SelectParams["MessageID"] = sfwMessageId;
    SelectParams["MethodType"] = lstrMethod;
    DataToSend.ResponseData.OtherData = SelectParams;
    var lobjAjaxData = { action: ["GridItemAddUpdate?astrFormID=", nsCommon.GetProperFormName(ActiveDivID)].join(''), param: DataToSend, PrevActiveForm: ActiveDivID, ActiveForm: ActiveDivID, SrcElement: lbtnSelf };
    return nsCommon.GetAjaxRequest(lobjAjaxData);
}
nsEvents.btnGridViewAddUpdate_Click = btnGridViewAddUpdate_Click;

var base_ns_PositionCursor = ns.PositionCursor;
ns.PositionCursor = function (astrFormID, adomDiv) {
    base_ns_PositionCursor(astrFormID, adomDiv);
    
    if (ns.viewModel && (ns.viewModel.currentForm == "wfmPayeeAccountRolloverMaintenance" || ns.viewModel.currentForm == "wfmOrganizationBankMaintenance")) {
        var oFirstControl = adomDiv.find(":not([gridid]):not([listviewid]):not(.filter):not(input.check_row):not(input.s-grid-check-all):not(input.ellipse-input-pageHolder):not(input.s-grid-common-filterbox):input[type !='button']:input[type !='submit']:input[type !='image']:visible:enabled:first");
        if ((oFirstControl !== undefined) && (oFirstControl.length > 0)) {
            oFirstControl.trigger("focus");
            oFirstControl.trigger("select");
        }
    }
}
function btnPrintPage_Click(e) {
    $([nsConstants.HASH, nsConstants.CENTER_SPLITTER].join("")).find("div[id^='wfm']:not([id$='" + nsConstants.ERROR_DIV + "']):visible").find("div:not([id$='" + nsConstants.ERROR_DIV + "']):first").trigger('mouseup');
    if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined
        && (ns.viewModel.currentModel != "" && $("#" + ns.viewModel.currentModel).length == 0) && (ns.viewModel.currentForm != "" && $("#" + ns.viewModel.currentForm).length == 0)) {
        ns.viewModel.currentModel = ns.viewModel.currentForm;
    }
    if (ns.viewModel != undefined && ns.viewModel.currentModel != undefined
        && (ns.viewModel.currentModel != "" && $("#" + ns.viewModel.currentModel).length > 0)) {
        if (parseInt($("#MainSplitter").css("opacity")) == 0) {
        }
        ns.iblnPrint = true;
        ns.istrPrintPage = ns.viewModel.currentModel;
        nsEvents.sfwActionPrintPage.PrintPage(e, ns.viewModel.currentModel);
    }
    return false;
}
nsEvents.btnPrintPage_Click = btnPrintPage_Click;
var sfwActionPrintPage = (function () {
    function sfwActionPrintPage() {
    }
    sfwActionPrintPage.LoadCssFiles = function (aarrAllStyles, afnCallBack, astrSelector) {
        if (astrSelector === void 0) { astrSelector = ":not([linkusercsstheme='true'])"; }
        if (aarrAllStyles == undefined) {
            aarrAllStyles = {};
        }
        var llstLinks = $("link[rel=stylesheet][href]" + astrSelector);
        var llintTotalLength = llstLinks.length;
        if ((Object.keys(aarrAllStyles).length == 0 || astrSelector === "[linkusercsstheme='true']") && llintTotalLength > 0) {
            var lintCount = 0;
            llstLinks.each(function () {
                var href = $(this).attr("href");
                if (href) {
                    if (aarrAllStyles[href] == undefined) {
                        $.get(href).done(function (astrData) {
                            lintCount++;
                            if (astrData != null) {
                                var lstrCss = nsCommon.ReplaceAll(astrData, "../image", "/image");
                                lstrCss = nsCommon.ReplaceAll(lstrCss, "../StaticResources", "/StaticResources");
                                aarrAllStyles[href] = lstrCss;
                            }
                            if (llintTotalLength == lintCount && afnCallBack) {
                                afnCallBack();
                            }
                        }).fail(function () {
                            lintCount++;
                            if (llintTotalLength == lintCount && afnCallBack) {
                                afnCallBack();
                            }
                        });
                    }
                    else {
                        lintCount++;
                        if (llintTotalLength == lintCount && afnCallBack) {
                            afnCallBack();
                        }
                    }
                }
            });
        }
        else if (afnCallBack) {
            afnCallBack();
        }
    };
    sfwActionPrintPage.PrintPage = function (e, astrActiveDivID) {
        ns.iintPrintMaxWidth = ns.iintPrintMaxWidth || 1248;
        if (astrActiveDivID == undefined || astrActiveDivID == ""
            || astrActiveDivID.startsWith("body")
            || ($("#" + astrActiveDivID).length > 0 && $("#" + astrActiveDivID)[0].tagName === "BODY")) {
            astrActiveDivID = ns.viewModel.currentModel;
        }
        var ldomCurrentModel = $([nsConstants.HASH, astrActiveDivID].join(""));
        if (ldomCurrentModel != undefined && ldomCurrentModel.length > 0) {
            var lobjDataItem = nsCommon.GetDataItemFromDivID(astrActiveDivID);
            var lstrTitle_1 = (lobjDataItem != null) ? lobjDataItem.title || document.title : document.title;
            ns.iblnPrint = true;
            ns.istrPrintPage = astrActiveDivID;
            $("#pnlLoading").css("display", "block");
            ns.displayActivity(true);
            var ldomDivToPrint = ldomCurrentModel.clone(true);
            ldomDivToPrint = $("<span/>").append(ldomDivToPrint);
            var fn = nsUserFunctions[nsConstants.USER_FUNCTION_BEFORE_PRINT_PAGE];
            if (typeof fn === 'function') {
                var Context = {
                    activeDivID: astrActiveDivID,
                    DivToPrint: ldomDivToPrint
                };
                var e = {};
                e.context = Context;
                fn(e);
            }
            ldomCurrentModel.attr(nsConstants.ATTR_ID, astrActiveDivID + "_jQueryPrint");
            nsEvents.sfwActionPrintPage.PrepareControlsToPrint(ldomDivToPrint, ldomCurrentModel);
            var ldomGrids = ldomCurrentModel.find(".s-gridparent:visible");
            ldomCurrentModel.hide();
            var lblnIsIEBrowser = false;
            lblnIsIEBrowser = nsCommon.detectIE();
            var lbnIE = lblnIsIEBrowser;
            var ldomGrp = ldomCurrentModel.closest('[role="group"]');
            if (ldomGrp != undefined)
                ldomGrp.prepend(ldomDivToPrint);
            ldomDivToPrint.show();
            if (ns.iblnPrintAllPagesOnLookup === true && astrActiveDivID.indexOf(nsConstants.LOOKUP) > 0) {
                ldomGrids.each(function (aintIndex, adomElement) {
                    var ldomGrid = $(adomElement);
                    var lstrActiveDivId = nsCommon.GetActiveDivId(ldomGrid);
                    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(lstrActiveDivId.replace("_jQueryPrint", ""), adomElement.id);
                    if (lobjGridWidget != undefined && lobjGridWidget.jsObject != undefined) {
                        var lobjGrid = ldomGrid.data("neoGrid");
                        var options = _.cloneDeep(lobjGrid.options);
                        var lobjGridStoredInfo = lobjGridWidget.getStoredObject();
                        options.pageable = false;
                        options.RestorableObject = lobjGridStoredInfo;
                        options.iblnPrintPage = true;
                        var ldomGridElement = ldomDivToPrint.find("#" + ldomGrid[0].id);
                        ldomGridElement.find('.s-pager').hide();
                        if (ns.iblnKeepToolBarForPrintForIEnFF !== true && ((lbnIE != undefined && lbnIE != false) || navigator.userAgent.search("Firefox") > -1) && options.iblnShowToolBar === true) {
                            ldomDivToPrint.find(".s-grid-toolbar-button-hide").removeClass("s-grid-toolbar-button-hide").removeAttr("toolbar-grid");
                            options.iblnShowToolBar = false;
                            options.iobjToolBarPanel = null;
                            ldomGridElement.empty();
                        }
                        if (lobjGrid.iblnTable && ldomGridElement.closest(".s-grid-helper").length > 0) {
                            var ldomHelper = ldomGridElement.closest(".s-grid-helper");
                            ldomGridElement.insertAfter(ldomHelper);
                            ldomGridElement.html(lobjGrid.istrTableInnerHTML);
                            ldomHelper.remove();
                        }
                        ldomGridElement.neoGrid(options);
                        lobjGridWidget.jsObject.options.iblnPrintPage = false;
                    }
                });
            }
            var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
            if (Prefix == "///") {
                Prefix = "/";
            }
            nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomDivToPrint);
            var lstrParentStyle = "";
            var ldomScrollDiv = $(nsConstants.SCROLL_DIV);
            if (ldomScrollDiv.length > 0 && ldomScrollDiv[0].getAttribute("style") != undefined && ldomScrollDiv[0].getAttribute("style") != "") {
                var DivStyle = ldomScrollDiv[0].getAttribute("style");
                lstrParentStyle = ["style='", DivStyle, "' "].join("");
            }
            if (ldomScrollDiv.length > 0 && $(nsConstants.SCROLL_DIV).attr("class") != undefined && ldomScrollDiv[0].getAttribute("class") != "") {
                lstrParentStyle = [lstrParentStyle, " class='", ldomScrollDiv[0].getAttribute("class"), "'"].join("");
            }
            var ldomPrintPageWithScroll = $("<div id='divPrintPageWithScroll' " + lstrParentStyle + "></div>");
            ldomScrollDiv.hide();
            if (!lblnIsIEBrowser) {

                ldomPrintPageWithScroll.insertAfter(ldomScrollDiv);
                ldomDivToPrint.appendTo(ldomPrintPageWithScroll);
                ldomDivToPrint.css({ "page-break-after": "always", "width": "100%", "overflow": "auto" });

            }
            else {
                ldomPrintPageWithScroll = ldomDivToPrint;
                ldomDivToPrint.css({ "page-break-after": "always", "width": "100%", "height": "100%", "overflow": "auto" });
            }
            ldomPrintPageWithScroll.show();
            var ldomPrintDiv = lblnIsIEBrowser ? ldomDivToPrint : $("#divPrintPageWithScroll");
            if (ldomPrintDiv.width() > ns.iintPrintMaxWidth) {
                ldomPrintDiv.width(ns.iintPrintMaxWidth);
            }
            ldomDivToPrint.width(ldomPrintDiv.width());
            var lintWidth = ldomDivToPrint.width();
            ldomPrintDiv.find(nsConstants.TAB_CONTAINER_SELECTOR).width(lintWidth);
            ldomPrintDiv.find(nsConstants.TAB_CONTAINER_SELECTOR).parent().width(lintWidth);
            ldomPrintDiv.find("ul.s-ulControlTabs").css("width", "100%");
            ldomPrintDiv.find(nsConstants.PANEL_CONTROL_TYPE_SELECTOR).width(lintWidth);
            ldomPrintDiv.find("li.s-liControlTabSheet").css("float", "left");
            ldomPrintDiv.find("li.s-liControlTabSheet").find("a").css("float", "left");
            var llstGrids = ldomPrintDiv.find(".s-gridparent");
            if (ns.iblnKeepToolBarForPrintForIEnFF !== true && ((lbnIE != undefined && lbnIE != false) || navigator.userAgent.search("Firefox") > -1)) {
                ldomDivToPrint.find(".s-grid-toolbar-button-hide").removeClass("s-grid-toolbar-button-hide").removeAttr("toolbar-grid");
                ldomDivToPrint.find(".s-grid-toolbar-button-container").remove();
            }
            llstGrids.each(function () {
                var lobjGrid = $(this);
                lobjGrid.parent().css("width", "100%");
                lobjGrid.css("width", "99%");
                var lintGrindWidth = lobjGrid.width();
                if (lintGrindWidth > lintWidth) {
                    lobjGrid.css("width", lintWidth + "px");
                    lobjGrid.find("td, th").css({ "max-width": "" + (ns.iintGridColumnPrintMaxWidth || 120) + "px", "white-space": "pre-wrap", "-ms-word-break": "break-word" }).addClass("s-PrintCellWrap");
                }
                else {
                    lobjGrid.find("td, th").css({ "white-space": "pre-wrap", "-ms-word-break": "break-word" }).addClass("s-PrintCellWrap");
                }
            });
            ldomPrintDiv.css("zoom", "80%");
            var beforePrint_1 = function () {
                nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomPrintPageWithScroll);
            };
            var fnAfterPrint_1 = function () {
                $("#divPrintPageWithScroll").remove();
                ldomPrintPageWithScroll = null;
                $(nsConstants.SCROLL_DIV).show();
                ldomDivToPrint.hide();
                ldomDivToPrint.remove();
                nsEvents.sfwActionPrintPage.MarkControlsAsChecked(ldomCurrentModel);
                ldomCurrentModel.attr(nsConstants.ATTR_ID, astrActiveDivID).show();
                ldomDivToPrint = null;
                ns.displayActivity(false);
                $("#pnlLoading").css("display", "none");
            };
            ldomPrintPageWithScroll = lblnIsIEBrowser ? ldomDivToPrint : $("#divPrintPageWithScroll");
            var fnCallThemePrint = function () {
                var ldomThemeLink = $("link[rel=stylesheet][href][linkusercsstheme='true']");
                if (ldomThemeLink.length > 0 && !sfwActionPrintPage.iarrThemeStyles[ldomThemeLink[0].getAttribute("href")]) {
                    var fnCallPrint = function () {
                        nsEvents.sfwActionPrintPage.PrintCurrentPage(ldomPrintPageWithScroll, lstrTitle_1, beforePrint_1, fnAfterPrint_1, ns.iintPrintDelay);
                    };
                    sfwActionPrintPage.LoadCssFiles(sfwActionPrintPage.iarrThemeStyles, fnCallPrint, "[linkusercsstheme='true']");
                }
                else {
                    nsEvents.sfwActionPrintPage.PrintCurrentPage(ldomPrintPageWithScroll, lstrTitle_1, beforePrint_1, fnAfterPrint_1, ns.iintPrintDelay);
                }
            };
            sfwActionPrintPage.LoadCssFiles(sfwActionPrintPage.iarrAllStyles, fnCallThemePrint);
        }
        return false;
    };
    sfwActionPrintPage.PrepareControlsToPrint = function (adomDivToPrint, adomOriginalDiv) {
        adomDivToPrint.find("[data-bind]").each(function (aintIndex, adomElement) {
            var ldomControl = $(this);
            var lstrValue;
            var lstrID = ldomControl[0].getAttribute(nsConstants.ATTR_ID);
            if (lstrID != undefined && lstrID != "") {
                if (adomOriginalDiv.find("#" + lstrID).length > 0) {
                    if (ldomControl[0].getAttribute(nsConstants.TYPE) === "text") {
                        var lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl[0].setAttribute("value", lstrValue);
                        }
                    }
                    else if (ldomControl[0].tagName === "SELECT") {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "" && ldomControl.find("option[value='" + lstrValue + "']").length > 0) {
                            ldomControl.find("option[value='" + lstrValue + "']").attr("selected", "selected");
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "radio" && ldomControl.closest("[islistcontrol]").length > 0) {
                        lstrValue = adomOriginalDiv.find("#" + ldomControl.closest("[islistcontrol]").attr(nsConstants.ATTR_ID)).find("input:checked");
                        if (lstrValue != undefined && lstrValue.length > 0) {
                            var lblnChecked = lstrValue.is(":checked");
                            if (neo.IsChrome && (lblnChecked === true || lblnChecked === "on"))
                                lblnChecked = "on";
                            else if (neo.IsChrome && (lblnChecked === false || lblnChecked === "off"))
                                lblnChecked = "off";
                            ldomControl.closest("[islistcontrol]").find("#" + lstrValue[0].id).attr("checked", lblnChecked);
                            ldomControl.closest("[islistcontrol]").find("#" + lstrValue[0].id)[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "radio" && ldomControl.closest("[islistcontrol]").length <= 0) {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].checked;
                        if (lstrValue === true) {
                            ldomControl[0].setAttribute("checked", lstrValue);
                            ldomControl[0].checked = lstrValue;
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "checkbox" && ldomControl.closest("[islistcontrol]").length > 0) {
                        lstrValue = adomOriginalDiv.find("#" + ldomControl.closest("[islistcontrol]").attr(nsConstants.ATTR_ID)).find("input:checked");
                        if (lstrValue != undefined && lstrValue.length > 0) {
                            lstrValue.each(function () {
                                ldomControl.closest("[islistcontrol]").find("#" + $(this).attr(nsConstants.ATTR_ID)).attr("checked", this.checked);
                                ldomControl.closest("[islistcontrol]").find("#" + $(this).attr(nsConstants.ATTR_ID))[0].checked = this.checked;
                            });
                        }
                    }
                    else if (ldomControl[0].getAttribute(nsConstants.TYPE) === "checkbox" && ldomControl.closest("islistcontrol").length <= 0) {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].checked;
                        if (lstrValue === true) {
                            ldomControl[0].setAttribute("checked", lstrValue);
                            ldomControl[0].checked = lstrValue;
                        }
                    }
                    else if (ldomControl[0].tagName === "TEXTAREA") {
                        lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl.text(lstrValue);
                        }
                    }
                    else {
                        var lstrValue = adomOriginalDiv.find("#" + lstrID)[0].value;
                        if (lstrValue != undefined && lstrValue != "") {
                            ldomControl[0].setAttribute("value", lstrValue);
                        }
                    }
                }
            }
        });
        adomDivToPrint.find("input[type='text'],input[type='radio'],input[type='checkbox'],select")
            .each(function (aintIndex, adomElement) {
                var $field = $(this);
                var lstrValue;
                var lstrID = $field[0].getAttribute(nsConstants.ATTR_ID);
                var ldomControl = adomOriginalDiv.find("#" + lstrID);
                var lblnChecked;
                if (lstrID != undefined && lstrID != "" && ldomControl.length > 0) {
                    if ($field.is("[type='radio']")) {
                        lblnChecked = ldomControl.is(":checked");
                        if (neo.IsChrome) {
                            if (lblnChecked === true || lblnChecked === "on") {
                                lblnChecked = "on";
                            }
                            else if (lblnChecked === false || lblnChecked === "off") {
                                lblnChecked = "off";
                            }
                        }
                        $field.attr("checked", lblnChecked);
                        $field[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                    }
                    else if ($field.is("[type='checkbox']")) {
                        lblnChecked = ldomControl.is(":checked");
                        $field.attr("checked", lblnChecked);
                        $field.prop("checked", lblnChecked);
                        $field[0].checked = ((lblnChecked === "on") ? true : (lblnChecked === "off" ? false : lblnChecked));
                    }
                    else if ($field[0].tagName === "SELECT") {
                        lstrValue = ldomControl[0].value;
                        if (lstrValue != undefined && lstrValue != "" && $field.find("option[value='" + lstrValue + "']").length > 0) {
                            $field.find("option[value='" + lstrValue + "']").attr("selected", "selected");
                        }
                    }
                    else {
                        $field.attr("value", ldomControl.val());
                    }
                }
            });
    };
    sfwActionPrintPage.MarkControlsAsChecked = function (adomDiv) {
        var lstrSelector = "input[type='radio'][checked='checked'],input[type='radio'][checked='true'],input[type='radio'][checked='on'],input[type='checkbox'][checked='checked'],input[type='checkbox'][checked='true'],input[type='checkbox'][checked='on']";
        adomDiv.find(lstrSelector)
            .each(function () {
                var $field = $(this);
                $field[0].checked = true;
            });
    };
    sfwActionPrintPage.PrintCurrentPage = function (adomElementToPrint, astrTiTle, beforePrintFunction, afterPrintFunction, aintPrintDelay) {
        if (aintPrintDelay === void 0) { aintPrintDelay = 333; }
        if (!astrTiTle || astrTiTle == "") {
            astrTiTle = document.title;
        }
        var lobjThemeCss = {};
        var ldomThemeLink = $("link[rel=stylesheet][href][linkusercsstheme='true']");
        if ((ldomThemeLink.length > 0)) {
            var lsrHref = ldomThemeLink[0].getAttribute("href");
            if (lsrHref && sfwActionPrintPage.iarrThemeStyles[lsrHref]) {
                lobjThemeCss[lsrHref] = sfwActionPrintPage.iarrThemeStyles[lsrHref];
            }
        }
        adomElementToPrint.printThis({
            debug: false,
            importCSS: false,
            importStyle: true,
            printContainer: true,
            loadCSS: "",
            pageTitle: astrTiTle || "",
            removeInline: false,
            removeInlineSelector: "*",
            printDelay: aintPrintDelay || 333,
            header: null,
            footer: null,
            base: false,
            formValues: true,
            canvas: true,
            doctypeString: '<!DOCTYPE html>',
            removeScripts: true,
            copyTagClasses: true,
            beforePrintEvent: null,
            beforePrint: beforePrintFunction,
            afterPrint: afterPrintFunction,
            iobjCssLinks: sfwActionPrintPage.iarrAllStyles,
            iobjCssThemeLinks: lobjThemeCss,
            iintBodyWidth: ns.iintPrintMaxWidth || 1248
        });
    };
    sfwActionPrintPage.iarrAllStyles = {};
    sfwActionPrintPage.iarrThemeStyles = {};
    return sfwActionPrintPage;
}());
nsEvents.sfwActionPrintPage = sfwActionPrintPage;

//FW Upgrade :: Handle the first item click of the parent grid for to load the child grid records
MVVM.JQueryControls.GridView.prototype.afterDataBound = function (e) {
    var lobjGridElement = e.sender.element;
    var lstrGridID = lobjGridElement[0].id;
    if ((ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0 && (lstrGridID === "GridTable_grvMPIBenefitCalculationDetail" || lstrGridID === "GridTable_grvLocalBenefitCalculationDetail")) ||
        (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") == 0 && (lstrGridID === "GridTable_grvBenefitCalculationDetailPension" || lstrGridID === "GridTable_grvBenefitCalculationDetail")) ||
        (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationPreRetirementDeathMaintenance") == 0 && (lstrGridID === "GridTable_grvBenefitCalculationDetailPension" || lstrGridID === "GridTable_grvBenefitCalculationDetail")) ||
        (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0 && lstrGridID === "GridTable_grvBenefitCalculationDetail") ||
        (ns.viewModel.currentModel.indexOf("wfmWithholdingInformationMaintenance") == 0 && lstrGridID === "GridTable_grvBusWithholdingInformation")) {
        setTimeout(function () {
            if (lobjGridElement) {
                $($(lobjGridElement).find('a[sfwmethodname="btnMasterDetailHeader_Click"]')[0]).trigger("click");
            }
        }, 200, lobjGridElement);
    }
    if (ns.viewModel.currentModel.indexOf("wfmQDROCalculationMaintenance") == 0 && lstrGridID === "GridTable_grvQdroCalculationDetail"){
        setTimeout(function () {
            if (lobjGridElement) {
                sessionStorage.setItem('lsGridSelected', "false");
                $($(lobjGridElement).find('a[sfwmethodname="btnGridViewSelect_Click"]')[0]).trigger("click");
            }
        }, 200, lobjGridElement);
    }
}

//FW Upgrade :: Handle child grid records loading on gridview select
var Base_btnGridViewSelect_Click = nsEvents.btnGridViewSelect_Click;
nsEvents.btnGridViewSelect_Click = function (e) {
    var lobjBtnInfo = nsCommon.GetEventInfo(e);
    var ActiveDivID = lobjBtnInfo.ActiveDivID;
    var lbtnSelf = lobjBtnInfo.lbtnSelf;
    var lintSelectedIndex = lobjBtnInfo.lintSelectedIndex;
    var RelatedGridID = MVVMGlobal.GetControlAttribute(lbtnSelf, nsConstants.SFW_RELATED_CONTROL, ActiveDivID);
    var lobjGridWidget = nsCommon.GetWidgetByActiveDivIdAndControlId(ActiveDivID, RelatedGridID);

    if ((ActiveDivID.indexOf("wfmWithholdingInformationMaintenance") >= 0 && RelatedGridID === "grvBusWithholdingInformation") ||
        (ActiveDivID.indexOf("wfmQDROCalculationMaintenance") >= 0 && RelatedGridID === "grvQdroCalculationDetail")) { 
        setTimeout(function () {
            $($(lobjGridWidget.jsObject.idomHtmlElement).find('table tbody tr[rowindex="' + lintSelectedIndex + '"]').find('a[sfwmethodname="btnMasterDetailHeader_Click"]')[0]).trigger('click');
        }, 200, lintSelectedIndex, RelatedGridID, lobjGridWidget);
    }
    return Base_btnGridViewSelect_Click(e);
}

//FW Upgrade #PIR 34299 - MVVM features - multiple issues (Filter caption added in neo-grid filter box)
var base_RenderCommonFilterBox = NeoGrid.prototype.renderCommonFilterBox;
NeoGrid.prototype.renderCommonFilterBox = function () {
    if (this.iblnCommonFilterBox) {
        var ldomFilrterContainer = neo.Clone(neo.elFilterContainer);
        var ldomFilrterBoxContainer = neo.Clone(neo.elFilterBoxContainer);
        ldomFilrterBoxContainer.setAttribute("id", ["NeoGridCommonFilterBox_", this.id].join(""));
        var ldomTextBox = ldomFilrterBoxContainer.querySelector("input[type='text'].s-grid-common-filterbox");
        NeoGrid.setAttributes(ldomTextBox, { "gridid": this.id, "title": Sagitec.DefaultText.FILTER_BOX_FILTER_TITLE_TEXT, "placeholder": Sagitec.DefaultText.FILTER_BOX_FILTERS_TEXT });
        var ldomButton = ldomFilrterBoxContainer.querySelector("input[type='button'].s-grid-common-filterbutton");
        NeoGrid.setAttributes(ldomButton, { "gridid": this.id, "title": Sagitec.DefaultText.FILTER_BOX_FILTER_TITLE_TEXT });
        if (this.itxtCommonFilterText != null && this.itxtCommonFilterText.trim() != "") {
            ldomTextBox.value = this.itxtCommonFilterText;
        }
        ldomFilrterContainer.appendChild(ldomFilrterBoxContainer);
        this.idomToolBarContainer[0].appendChild(ldomFilrterContainer);
        this.idomCommonFilterBoxContainer = this.idomToolBarContainer.find(".s-grid-common-filterbox-container");
        this.idomCommonFilterBoxContainer.data('GridElement', this.element);
    }
}

function AfterGetTemplate(astrFormID, ablnCenterLeft, data, astrPostFix) {
    astrFormID = [astrFormID, astrPostFix].join('');
    var NewFormid = nsCommon.GetProperFormId(astrFormID);
    var lobjPageStateData = null;
    if (ns.iblnShowGridStoreStateButtons === true && ablnCenterLeft !== true) {
        var lobjData = data;
        if (lobjData.DomainModel.OtherData != undefined && lobjData.DomainModel.OtherData["PageStateData"] != undefined) {
            try {
                lobjPageStateData = jQuery.parseJSON(HtmlWhitelistedSanitizer.sanitizeHTMLString(lobjData.DomainModel.OtherData["PageStateData"]));
            }
            catch (ex) {
                try {
                    lobjData.DomainModel.OtherData["PageStateData"] = lobjData.DomainModel.OtherData["PageStateData"].replaceAll('\\"', "'");
                    lobjPageStateData = jQuery.parseJSON(HtmlWhitelistedSanitizer.sanitizeHTMLString(lobjData.DomainModel.OtherData["PageStateData"]));
                }
                catch (ex1) {
                    lobjPageStateData = jQuery.parseJSON(lobjData.DomainModel.OtherData["PageStateData"]);
                }
            }
        }
    }
    var larrCodeValues = (data != undefined && data.DomainModel != undefined && data.DomainModel.OtherData != undefined) ? data.DomainModel.OtherData["FormLoadSourceValues"] : null;
    if (data.ExtraInfoFields["FormType"] == "Report" || data.ExtraInfoFields["FormType"] == nsConstants.LOOKUP || data.ExtraInfoFields["FormType"] == "FormLinkLookup") {
        var lblnIsReportForm = false;
        if (data.ExtraInfoFields["FormType"] == "Report") {
            lblnIsReportForm = true;
        }
        if (ns.iblnShowGridStoreStateButtons === true && lobjPageStateData != null) {
            nsCommon.SetPageStateData(lobjPageStateData, NewFormid);
        }
        if (lblnIsReportForm) {
            var lstrSenderForm = astrFormID;
        }
        else {
            var lstrSenderForm = nsCommon.GetProperFormName(astrFormID);
        }
        if (ns.SenderForm != lstrSenderForm) {
            ns.setSenderData("", lstrSenderForm, "");
        }
        if (nsConstants.UNDERSCORE_RETRIEVE === astrPostFix) {
            lstrSenderForm = [lstrSenderForm, astrPostFix].join('');
        }
        var lstrUserDefaults = nsCommon.sessionGet(nsConstants.USER_STORED_DEFAULTS_FOR_LOOKUP);
        if (lstrUserDefaults != undefined && lstrUserDefaults[lstrSenderForm] != undefined) {
            var headerData = lstrUserDefaults[lstrSenderForm];
            if (headerData != undefined) {
                for (var k in headerData) {
                    if (k != "ControlList") {
                        data.DomainModel.HeaderData[k] = headerData[k];
                    }
                }
            }
        }
        var lblnHasDefaultControls = false;
        if (data != undefined && data.ControlAttribites != undefined) {
            lblnHasDefaultControls = _.filter(data.ControlAttribites, function (item) {
                return (item["sfwDefaultValue"] != undefined && item["sfwDefaultValue"] != "") || (item["sfwDefaultType"] != undefined && item["sfwDefaultType"] != "");
            }).length > 0;
        }
        var lblnControlDefaultsCalled = false;
        var lobjControlList = nsCommon.ManageLookupControlList(astrFormID);
        if (lobjControlList == undefined) {
            var lobjSenderData = nsCommon.GetSenderData(lstrSenderForm, lstrSenderForm, lstrSenderForm, ablnCenterLeft ? "CenterLeft" : "");
            var lobjLookupControlList = nsRequest.SyncPost(["GetLookupControlList?astrFormID=", lstrSenderForm, "&ablnIsCenterLeft=", ablnCenterLeft, "&ablnGetDefaults=", lblnHasDefaultControls, "&ablnCallList=true"].join(''), null, null, "GET", lobjSenderData);
            lblnControlDefaultsCalled = true;
            if (lobjLookupControlList != null) {
                lobjControlList = lobjLookupControlList.ControlList;
                if (lobjControlList != undefined) {
                    nsCommon.ManageLookupControlList(astrFormID, lobjControlList);
                    if (!lblnIsReportForm) {
                        data.DomainModel.HeaderData.ControlList = lobjControlList;
                    }
                }
                if (lobjLookupControlList.DefaultControlValues != undefined) {
                    var larrDefaultKeys = Object.keys(lobjLookupControlList.DefaultControlValues), lstrControlkey;
                    for (var iC = 0, iClen = larrDefaultKeys.length; iC < iClen; iC++) {
                        lstrControlkey = larrDefaultKeys[iC];
                        if (data.DomainModel.HeaderData["tblCriteria"] != undefined && data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] === ""))) {
                            data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                        }
                        else if (!lblnIsReportForm && data.DomainModel.HeaderData["tblAdvCriteria"] != undefined && data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] === ""))) {
                            data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                        }
                    }
                }
            }
        }
        else if (data.DomainModel != undefined && data.DomainModel.HeaderData != undefined) {
            data.DomainModel.HeaderData.ControlList = lobjControlList;
        }
        if (!lblnIsReportForm) {
            if (lobjControlList != undefined && lobjControlList["SecurityMessage"] != undefined) {
                data.ExtraInfoFields.SecurityMessage = lobjControlList["SecurityMessage"];
            }
        }
        if (lblnControlDefaultsCalled !== true && lblnHasDefaultControls === true) {
            var lobjSenderData = nsCommon.GetSenderData(lstrSenderForm, lstrSenderForm, lstrSenderForm, ablnCenterLeft ? "CenterLeft" : "");
            var lobjLookupControlList = nsRequest.SyncPost(["GetLookupControlList?astrFormID=", lstrSenderForm, "&ablnIsCenterLeft=", ablnCenterLeft, "&ablnGetDefaults=", lblnHasDefaultControls, "&ablnCallList=false"].join(''), null, null, "GET", lobjSenderData);
            if (lobjLookupControlList != null && lobjLookupControlList.DefaultControlValues != undefined) {
                var larrDefaultKeys = Object.keys(lobjLookupControlList.DefaultControlValues), lstrControlkey;
                for (var iC = 0, iClen = larrDefaultKeys.length; iC < iClen; iC++) {
                    lstrControlkey = larrDefaultKeys[iC];
                    if (data.DomainModel.HeaderData["tblCriteria"] != undefined && data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] === ""))) {
                        data.DomainModel.HeaderData["tblCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                    }
                    else if (data.DomainModel.HeaderData["tblAdvCriteria"] != undefined && data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] != undefined && ((data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] === ""))) {
                        data.DomainModel.HeaderData["tblAdvCriteria"][lstrControlkey] = lobjLookupControlList.DefaultControlValues[lstrControlkey];
                    }
                }
            }
        }
        data.DomainModel.HeaderData.ClientVisibility = data.ClientVisibility;
        headerData = data.DomainModel.HeaderData;
        nsCommon.sessionSet(["UserDefaults_", NewFormid].join(''), headerData);
        ns.Templates[NewFormid] = {
            "FormType": nsConstants.LOOKUP,
            "Template": data.Template,
            "ExtraInfoFields": data.ExtraInfoFields,
            "InnerTemplates": data.InnerTemplates,
            "HeaderData": headerData,
            "ControlAttribites": data.ControlAttribites,
            "DetailsData": {},
            "ControlsHaveingVisibility": {},
            "WidgetControls": {},
            "PageStateData": lobjPageStateData,
            "LoadSourceValues": larrCodeValues
        };
    }
    else {
        ns.Templates[NewFormid] = {
            "FormType": nsConstants.MAINTENANCE,
            "Template": data.Template,
            "ExtraInfoFields": data.ExtraInfoFields,
            "InnerTemplates": data.InnerTemplates,
            "ControlAttribites": data.ControlAttribites,
            "ClientVisibility": data.ClientVisibility,
            "WidgetControls": {},
            "PageStateData": lobjPageStateData,
            "LoadSourceValues": larrCodeValues
        };
        if (NewFormid.indexOf("wfmwfp") === 0 || NewFormid.indexOf("wfp") === 0) {
            ns.Templates[NewFormid]["DomainModel"] = data.DomainModel;
        }
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ExtraInfoFields != undefined && data.ExtraInfoFields != undefined && data.ExtraInfoFields.SecurityMessage != undefined) {
        ns.Templates[NewFormid].ExtraInfoFields.SecurityMessage = data.ExtraInfoFields.SecurityMessage;
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ExtraInfoFields
        && ns.Templates[NewFormid].ExtraInfoFields.ShortCutKeys) {
        ns.Templates[NewFormid].ShortCutKeys = JSON.parse(ns.Templates[NewFormid].ExtraInfoFields.ShortCutKeys);
    }
    if (ns.Templates[NewFormid] != undefined && ns.Templates[NewFormid].ControlAttribites != null) {
        var lCodeValues, lstrCodeValue, lstrLoadType;
        if (larrCodeValues == undefined) {
            lCodeValues = [];
        }
        Object.freeze(ns.Templates[NewFormid].ControlAttribites);
        var larrKeys = Object.keys(ns.Templates[NewFormid].ControlAttribites), key;
        for (var i = 0, len = larrKeys.length; i < len; i++) {
            key = larrKeys[i];
            Object.freeze(ns.Templates[NewFormid].ControlAttribites[key]);
            if (lCodeValues != undefined) {
                lstrLoadType = ns.Templates[NewFormid].ControlAttribites[key].sfwLoadType;
                if (lstrLoadType === "CodeGroup") {
                    lstrCodeValue = ns.Templates[NewFormid].ControlAttribites[key].sfwLoadSource;
                    if (lstrCodeValue != undefined && lstrCodeValue.trim() != "" && lCodeValues.indexOf(lstrCodeValue.trim()) < 0) {
                        lCodeValues.push(lstrCodeValue.trim());
                    }
                }
            }
        }
        if (lCodeValues != undefined && larrCodeValues == undefined) {
            ns.Templates[NewFormid].LoadSourceValues = lCodeValues;
        }
    }
}
MVVMGlobal.AfterGetTemplate = AfterGetTemplate;

MVVM.KendoControls.Panel.prototype.restoreStateBase = MVVM.KendoControls.Panel.prototype.restoreState;
MVVM.KendoControls.Panel.prototype.restoreState = function () {

    this.restoreStateBase();
    var panelID = $(this)[0].id;
    if ($("#" + panelID).is(":visible")) {
        if ($("#" + panelID + " .s-liControlPanelbar").attr("aria-expanded") == "false")
            $("#" + panelID + " .s-spnControlPanelbar").trigger("click");
    }

}

var FM_MasterDetailHeader = nsEvents.btnMasterDetailHeader_Click;
nsEvents.btnMasterDetailHeader_Click = btnMasterDetailHeader_Click = function (e) {
    var lstrPreviousMessage = sessionStorage.getItem('IstrPreservedMessage');
    if (lstrPreviousMessage != undefined) {
        if (ns.viewModel.currentModel.indexOf("wfmDisabiltyBenefitCalculationMaintenance") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmBenefitCalculationPreRetirementDeathMaintenance") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmBenefitCalculationWithdrawalMaintenance") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmQDROCalculationMaintenance") == 0 ||
            ns.viewModel.currentModel.indexOf("wfmWithholdingInformationMaintenance") == 0) {
            setTimeout(function () {
                sessionStorage.setItem('lstrMessageAfterParentGridClick', lstrPreviousMessage);
                nsCommon.DispalyMessage(lstrPreviousMessage, ns.viewModel.currentModel);
                sessionStorage.removeItem('lsExecuted');
            }, 200, lstrPreviousMessage, ns.viewModel.currentModel);
        }
    }
    return FM_MasterDetailHeader(e);
}

var FM_Reset = nsEvents.btnReset_Click;
nsEvents.btnReset_Click = btnReset_Click = function (e) {
    FM_Reset(e);
    if (ns.viewModel.currentModel.indexOf("wfmPacketTrackingLookup") == 0) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        if ((ldomDiv.find("#ddlDropDownList2").find('option').filter(function () { return $(this).text().trim() == "All"; })).length == 0) {
            ldomDiv.find("#ddlDropDownList2").prepend($('<option></option>').val('0').html('All').attr("selected", "selected"));
        }
        else {
            ldomDiv.find("#ddlDropDownList2").val("0").trigger("change");
        }
        if ((ldomDiv.find("#ddlDropDownList3").find('option').filter(function () { return $(this).text().trim() == "All"; })).length == 0) {
            ldomDiv.find("#ddlDropDownList3").prepend($('<option></option>').val('0').html('All').attr("selected", "selected"));
        }
        else {
            ldomDiv.find("#ddlDropDownList3").val("0").trigger("change");
        }
    }
}


var FM_Cancel = nsEvents.btnCancel_Click;
nsEvents.btnCancel_Click = btnCancel_Click = function (e) {
    if (ns.viewModel.currentModel.indexOf("wfmPersonContactMaintenance") == 0) {
        var ldomDiv = $("#" + ns.viewModel.currentModel);
        var ActiveDivID = e.context != undefined && e.context.activeDivID != undefined ? e.context.activeDivID : nsCommon.GetActiveDivId(ns.viewModel.srcElement);
        if (ldomDiv.find("#txtEffectiveEndDate").is(":hidden")) {
            ns.RemoveReadOnlyAndEnableRules(ns.viewModel[ActiveDivID].HeaderData.ControlList, ActiveDivID, ldomDiv);
            ldomDiv.find("#txtEffectiveEndDate").show();
            ldomDiv.find("img[class='ui-datepicker-trigger']").show();
            ldomDiv.find("#txtPersonId").hide();

            if ($("#ddlContactTypeValue").val() !== "ATRN") {
                ldomDiv.find("#ddlAttorneyFor,#lblAttorneyFor,#lblAttrnForReq").hide();
            }
            var Country = ldomDiv.find("#ddlAddrCountryValue").val();
            if (Country == "0001") {
                ldomDiv.find("#txtForeignProvince, #txtForeignPostalCode").hide();
            }
        }

        return true;
    }
    return FM_Cancel(e);
}


function BindDetailView(data) {
    if (data.DomainModel == undefined || data.DomainModel.HeaderData == undefined)
        return;
    var HeaderData = data.DomainModel.HeaderData;
    var ActiveDivID = ns.viewModel.currentModel;
    var FormContainerID = "";
    if (ns.isRightSideForm === true) {
        FormContainerID = "#RightContentSplitter";
    }
    else {
        FormContainerID = nsConstants.CONTENT_SPLITTER_SELECTOR;
    }
    var ldomFormContainer = $([FormContainerID, nsConstants.SPACE_HASH, ActiveDivID].join(''));
    var SourceModel = HeaderData.MaintenanceData;
    var DestinationModel = ns.viewModel[ActiveDivID].HeaderData.MaintenanceData;
    var larrKeys = Object.keys(SourceModel);
    var key = "";

    var lstrAction = (data != undefined && data.LastExecutedAction != undefined && data.LastExecutedAction != "") ? data.LastExecutedAction : ns.settings.data.action;
    if (lstrAction.indexOf("GridItemSelect") >= 0) {
        ns.blnSetValueWhileLoading = true;
    }


    for (var i = 0, iLen = larrKeys.length; i < iLen; i++) {
        key = larrKeys[i];
        DestinationModel.set(key, SourceModel[key]);
    }
    var detailViewControl, keyField = "";
    for (var i = 0, iLen = larrKeys.length; i < iLen; i++) {
        keyField = larrKeys[i];
        detailViewControl = ldomFormContainer.find([nsConstants.HASH, keyField].join(''));
        if (ActiveDivID.indexOf("wfmQDROApplicationMaintenance") == 0) {
            if (keyField == "ddlPlanId")
                sessionStorage.setItem("doNotExecuteChangeEventOfPlanId", "True");
            if (keyField == "ddlPlanId" || keyField == "ddlIstrSubPlan" || keyField == "ddlIstrBenefitOptionValue")
                continue;
        }
      
        if (detailViewControl.length > 0 && detailViewControl.attr("IsCascadingDropDown") != undefined) {
            if (detailViewControl.attr("sfwparentcontrol") == undefined && MVVMGlobal.GetControlAttribute(detailViewControl, "sfwParameters", ActiveDivID) != null) {
                MVVMGlobal.PopulateDropDownList(detailViewControl[0], true, ActiveDivID, ldomFormContainer);
            }
            else {
                var llstOptions = detailViewControl[0].options;
                var lblnItemPresent = false;
                for (var counter1 = 0; counter1 < llstOptions.length; counter1++) {
                    if (llstOptions[counter1].value == detailViewControl.val())
                        lblnItemPresent = true;
                }
                if (lblnItemPresent) {
                    detailViewControl.trigger('change', [true]);
                }
                else {
                    delete ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData[keyField];
                    delete ns.DirtyData[ActiveDivID].HeaderData.MaintenanceData[keyField + "_istrEV"];
                    if (ActiveDivID.indexOf("wfmPayeeAccountTaxwithholdingMaintenance") == 0) {
                        if (keyField == "ddlTaxOptionValue11") {
                            sessionStorage.setItem("doNotExecuteChangeEventOfTaxOption", "True");
                            continue;
                        }
                    }
                }
            }
        }
        if (detailViewControl.attr('sfwDataFormat') != undefined) {
            detailViewControl.trigger('blur', [true]);
        }
    }
    if (HeaderData != null && HeaderData.ControlList != undefined) {
        nsCommon.ApplyVisiblityRules(HeaderData.ControlList, ActiveDivID, ldomFormContainer);
        ns.ApplyReadOnlyAndEnableRules(HeaderData.ControlList, ActiveDivID, ldomFormContainer);
    }

    ns.blnSetValueWhileLoading = false;
}
ns.BindDetailView = BindDetailView;

function BindDetailData(data, FormContainerID, astrModel, adomDiv, aobjApplyUIData, DetailsData, HiddenControls, ablnRebindData, ablnPush, ablnIsLazyLoad, adomTabDiv) {
    var larrDetailKeys = Object.keys(DetailsData), grid = "", ldomCntrl, lobjGrid;
    if (aobjApplyUIData != undefined) {
        var ldataItem = nsCommon.GetDataItemFromDivID(astrModel);
        if (ldataItem != undefined && ldataItem.IsViewOnly === true)
            aobjApplyUIData.iblnViewOnly = ldataItem.IsViewOnly === true;
    }
    if (HiddenControls == undefined)
        HiddenControls = {};
    if (ablnPush === true)
        ns.viewModel[astrModel].grids = [];
    for (var i = 0, iLen = larrDetailKeys.length; i < iLen; i++) {
        grid = larrDetailKeys[i];
        ldomCntrl = adomDiv[0].querySelector(["#", grid].join(""));
        if (ldomCntrl != null) {
            if (ablnPush === true)
                ns.viewModel[astrModel].grids.push(grid);
            var gridObj = DetailsData[grid];
            var ldomGrid = adomDiv[0].querySelector(nsConstants.HASH + nsConstants.GRID_TABLE_UNDERSCORE + grid);
            if (gridObj.istrControlType == "Chart") {
                ns.BindChartData(data, grid, astrModel, aobjApplyUIData, adomDiv, ablnIsLazyLoad);
            }
            else if (gridObj.istrControlType == "sfwRuleViewer") {
                ns.BindRuleViewerData(data, astrModel, adomDiv, grid);
            }
            else if (grid.indexOf(nsConstants.LISTVIEW_CONTAINER_UNDERSCORE) === 0) {
                var listView = grid.replace(nsConstants.LISTVIEW_CONTAINER_UNDERSCORE, "");
                if (HiddenControls[listView] === undefined) {
                    MVVMGlobal.BindListViewFromData(data, grid, FormContainerID, astrModel, ablnRebindData, aobjApplyUIData, adomDiv);
                }
            }
            else if (grid != "HeaderTemplate") {
                if (HiddenControls[grid] === undefined)
                    ns.BindGridFromData(data, grid, FormContainerID, astrModel, ablnRebindData, adomDiv, aobjApplyUIData);
            }
            if (ablnIsLazyLoad === true) {
                ldomCntrl.setAttribute("IsDataLoaded", "true");
                lobjGrid = nsCommon.GetWidgetByActiveDivIdAndControlId(astrModel, grid);
                if (lobjGrid != undefined && lobjGrid.jsObject == undefined) {
                    lobjGrid.init();
                }
            }
            if (ldomGrid != null && $(ldomGrid).attr('SelectedIndex') != undefined)
                $(ldomGrid).removeAttr('SelectedIndex');
        }
    }
    var lviewModel = ns.viewModel[astrModel];
    if (lviewModel != undefined && lviewModel.DetailsData != undefined) {
        var dataToSend = {
            DomainModel: lviewModel
        };
        var larrGridsToBound = adomDiv[0].querySelectorAll(".sfwgrid");
        for (var i = 0, iLen = larrGridsToBound.length; i < iLen; i++) {
            var lhtmlGrid = larrGridsToBound[i];
            if (lviewModel.WidgetControls != undefined && (lviewModel.WidgetControls[lhtmlGrid.id] == undefined || (ns.iblnIsMobileMedia !== lviewModel.WidgetControls[lhtmlGrid.id].iblnIsMobileMedia)) && lviewModel.DetailsData[lhtmlGrid.id] != undefined) {
                ns.BindGridFromData(dataToSend, lhtmlGrid.id, FormContainerID, astrModel, ablnRebindData, adomDiv, aobjApplyUIData);
            }
            else if (lviewModel.WidgetControls != undefined && (lviewModel.WidgetControls[lhtmlGrid.id] != undefined && lviewModel.WidgetControls[lhtmlGrid.id].jsObject && lviewModel.WidgetControls[lhtmlGrid.id].jsObject.totalRecords <= 0)) {
                lviewModel.WidgetControls[lhtmlGrid.id].jsObject.changeRowSelection();
            }
        }
        if (adomDiv[0].querySelectorAll(nsConstants.TAB_CONTAINER_SELECTOR) != null) {
            var larrTabIds = (adomTabDiv != null) ? [adomTabDiv.id] : undefined;
            MVVM.Controls.TabSheet.updateTabCaptionWithRecordCount(astrModel, adomDiv, larrTabIds);
        }
    }
}
nsCommon.BindDetailData = BindDetailData;
function GridColumnVisbility() {
    //Implementing IsPopUpToLifeConversion Visible Rule.

    var ldomDiv = $("#" + ns.viewModel.currentModel);

    var grid_name = '';
    if (ldomDiv.find("#GridTable_grvBenefitCalculationOptionsPension").is(":visible") == true) {
        grid_name = "GridTable_grvBenefitCalculationOptionsPension";
    }
    else if (ldomDiv.find("#GridTable_grvBenefitCalculationOptions").is(":visible") == true) {
        grid_name = "GridTable_grvBenefitCalculationOptions";
    }

    //var aGridData = nsCommon.GetWidgetByActiveDivIdAndControlId(ns.viewModel.currentModel, grid_name);    
    
    grid_id = "Table_" + grid_name;
    rows_data = $("#Table_" + grid_name + " tr");
    var lbliblnPopuplife = ldomDiv.find("#lbliblnPopuplife");
    if (lbliblnPopuplife != undefined && (lbliblnPopuplife.text() == 'false')) {
        ldomDiv.find("table[id='" + grid_id + "']>thead>tr>th[data-field='dt_PopupOptionFactor_8_0']").hide();
        ldomDiv.find("table[id='" + grid_id + "']>tbody>tr>td[data-container-for='dt_PopupOptionFactor_8_0']").hide();
        ldomDiv.find("table[id='" + grid_id + "']>thead>tr>th[data-field='dt_PopupBenefitAmount_9_0']").hide();
        ldomDiv.find("table[id='" + grid_id + "']>tbody>tr>td[data-container-for='dt_PopupBenefitAmount_9_0']").hide();
    }
}
var FM_BindMasterDetail = ns.BindMasterDetail;
ns.BindMasterDetail = function (e) {
    FM_BindMasterDetail(e);
    if (ns.viewModel.currentModel.indexOf("wfmBenefitCalculationRetirementMaintenance") >= 0) {
        GridColumnVisbility();
    }
};

function ChangeVisibility(astrDivID, aAryControlList, visible, restoreValue, VisibilityChangedFromCode, adomDiv, adomControl) {
    var VisibilityList;
    var View;
    if (astrDivID.indexOf(nsConstants.LOOKUP) > 0 || nsCommon.IsCorrForm(astrDivID)) {
        View = ns.Templates[astrDivID];
    }
    else {
        View = ns.viewModel[astrDivID];
    }
    var ldomDiv = adomDiv;
    if (adomDiv == undefined) {
        ldomDiv = $([nsConstants.HASH, astrDivID].join(''));
    }
    VisibilityList = View.HeaderData.ClientVisibility;
    for (var i = 0, iLen = aAryControlList.length; i < iLen; i++) {
        var lstrControlID = aAryControlList[i];
        if (lstrControlID === "") {
            continue;
        }
        if (nsCommon.IsHiddenControl(null, lstrControlID, View)) {
            if (VisibilityList[lstrControlID] != undefined) {
                VisibilityList[lstrControlID].set(nsConstants.ATTRIBUTE_VISIBLE, false);
            }
            continue;
        }
        var ldomCntrl = ldomDiv[0].querySelector([nsConstants.HASH, lstrControlID].join(''));
        if (!ns.iblnKeepReadonlyControls) {
            if (ldomCntrl != null) {
                if (ldomCntrl.tagName === nsConstants.SELECT_TAG && ldomCntrl.getAttribute("multiple") === "multiple") {
                    ldomCntrl = ldomDiv[0].querySelector("[id^='MultiSelectWidget_'][originalid='MultiSelectWidget_" + lstrControlID + "']");
                    if (ldomcontrol == null) {
                        ldomCntrl = ldomDiv[0].querySelector([nsConstants.HASH, "MultiSelectWidget_", lstrControlID].join(''));
                    }
                }
                if (ldomCntrl != null) {
                    MVVM.Controls.Panel.ShowHideNavigatorItem(ldomCntrl, astrDivID, visible, "li.s-panel-navigator-li[panelid='" + ldomCntrl.id + "']");
                    var ldomControl = $(ldomCntrl);
                    if (ldomControl.length == 1 && ldomCntrl.classList.contains(nsConstants.Hide_BY_READONLY_CSSCLASS)) {
                        if (VisibilityList[lstrControlID] != undefined) {
                            VisibilityList[lstrControlID].set(nsConstants.ATTRIBUTE_VISIBLE, false);
                        }
                        var ldomlblnRdControl = ldomDiv[0].querySelector([nsConstants.HASH, "lblrdfor_", ldomCntrl.id].join(''));
                        if (ldomlblnRdControl != null) {
                            if (visible == true) {
                                ldomlblnRdControl.style.display = "block";
                            }
                            else {
                                ldomlblnRdControl.style.display = "none";
                            }
                        }
                        continue;
                    }
                }
            }
        }
        if (VisibilityChangedFromCode === undefined)
            ns.VisibilityChangedFromCode = false;
        else
            ns.VisibilityChangedFromCode = VisibilityChangedFromCode;
        if (VisibilityList[lstrControlID] !== undefined) {
            if (ldomCntrl == null) {
                continue;
            }
            if (visible == true) {
                VisibilityList[lstrControlID].set(nsConstants.ATTRIBUTE_VISIBLE, true);
                if (restoreValue && VisibilityList[lstrControlID]["OldValue"] != undefined) {
                    MVVMGlobal.SetFieldValueIntoModel(astrDivID, lstrControlID, VisibilityList[lstrControlID]["OldValue"], ldomDiv);
                    var lblniscascadingdropdown = ldomCntrl.getAttribute("iscascadingdropdown") === "true" && ldomCntrl.getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) === "cascading";
                    if (lblniscascadingdropdown) {
                        MVVMGlobal.SetFieldValueIntoDirtyData(astrDivID, lstrControlID, VisibilityList[lstrControlID]["OldValue"], ldomDiv);
                    }
                }
                else if ($(ldomCntrl)[0].nodeName.toLowerCase() == "select" && $(ldomCntrl).val() != undefined && $(ldomCntrl).val() != "") {
                    $(ldomCntrl).trigger('change', [true]);
                }
                nsVisi.ChangeVisibilityForWizardButtons(astrDivID, lstrControlID, true, ldomDiv);
                nsVisi.SetParentVisibilityByWithGroup($(ldomCntrl), ldomCntrl.id, visible, ldomDiv, 1, true);
            }
            else {
                var alreadyHidden = VisibilityList[lstrControlID].get(nsConstants.ATTRIBUTE_VISIBLE) == false;
                VisibilityList[lstrControlID].set(nsConstants.ATTRIBUTE_VISIBLE, false);
                if (restoreValue) {
                    var oldValue = MVVMGlobal.GetFieldValueFromModel(astrDivID, lstrControlID, ldomDiv);
                    if (!alreadyHidden) {
                        VisibilityList[lstrControlID]["OldValue"] = oldValue;
                        var lblniscascadingdropdown = ldomCntrl.getAttribute("iscascadingdropdown") === "true" && ldomCntrl.getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) === "cascading";
                        if (lblniscascadingdropdown && ldomCntrl.getAttribute("sfwparentcontrol") == undefined) {
                            ShowHideCascadingDropdown(ldomCntrl, astrDivID, VisibilityList, ldomDiv);
                        }
                    }
                }
                nsVisi.ChangeVisibilityForWizardButtons(astrDivID, lstrControlID, false, ldomDiv);
                nsVisi.SetParentVisibilityByWithGroup($(ldomCntrl), ldomCntrl.id, visible, ldomDiv, 1, true);
                var lblnDialogPanel = false;
                var lstrModel = astrDivID;
                if (View != undefined && View.ExtraInfoFields != undefined && View.ExtraInfoFields["DialogPanel"] == astrDivID) {
                    lblnDialogPanel = true;
                    lstrModel = ns.viewModel.currentModel;
                }
                var ldomToSetValue = ldomDiv[0].querySelector([nsConstants.HASH, lstrControlID].join(''));
                if (lblnDialogPanel && $([nsConstants.HASH, lstrModel, nsConstants.SPACE_HASH, astrDivID].join('')).length > 0) {
                    ldomToSetValue = $([nsConstants.HASH, lstrModel, nsConstants.SPACE_HASH, astrDivID].join(''))[0].querySelector([nsConstants.SPACE_HASH, lstrControlID].join(''));
                }
                var lcontrolToSetValue;
                var valueToSet = '';
                if (ldomToSetValue != null) {
                    lcontrolToSetValue = $(ldomToSetValue);
                    if (ldomToSetValue.getAttribute("iscascadingdropdown") === "true" && ldomToSetValue.getAttribute("disabled") === "disabled"
                        && ldomToSetValue.getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) === "cascading" && ldomToSetValue.getAttribute("sfwparentcontrol") != undefined
                        && ldomToSetValue.getElementsByTagName("option").length == 0) {
                        continue;
                    }
                    var sfwAttributes = MVVMGlobal.GetControlAttribute(lcontrolToSetValue, "GetAllAttr", lstrModel, true);
                    var defaultValue = sfwAttributes["sfwDefaultValue"];
                    var lblnIsBoundByItems = (sfwAttributes["sfwLoadType"] == undefined || sfwAttributes["sfwLoadType"] == "Items");
                    if (ldomToSetValue.tagName === nsConstants.SELECT_TAG && ldomToSetValue.getAttribute("multiple") !== "multiple" && lblnIsBoundByItems) {
                        if (ldomToSetValue.querySelector("option[value=''],option[value='0'],option[value='0.00']") != null) {
                            valueToSet = ldomToSetValue.querySelector("option[value=''],option[value='0'],option[value='0.00']").getAttribute('value');
                        }
                        else if (defaultValue != undefined) {
                            valueToSet = defaultValue;
                        }
                        else if (ldomToSetValue.querySelector("option[value]") != null) {
                            valueToSet = ldomToSetValue.querySelector("option[value]").getAttribute('value');
                        }
                    }
                    else if (ldomToSetValue.getAttribute(nsConstants.CONTROL_TYPE) != undefined && ldomToSetValue.getAttribute(nsConstants.CONTROL_TYPE).toLowerCase() == nsConstants.SFW_RADIO_BUTTON_LIST_LOWER && defaultValue != undefined && lblnIsBoundByItems) {
                        valueToSet = defaultValue;
                    }
                    else if ((ldomToSetValue.getAttribute(nsConstants.CONTROL_TYPE) != undefined && ldomToSetValue.getAttribute(nsConstants.CONTROL_TYPE).toLowerCase() == nsConstants.SFW_CHECKBOX_LIST_LOWER) || (ldomToSetValue.tagName === nsConstants.SELECT_TAG && ldomToSetValue.getAttribute("multiple") === "multiple")) {
                        valueToSet = [];
                    }
                    else {
                        valueToSet = '';
                    }
                    if (ldomToSetValue.getAttribute("sfwRenderAsRadioButtonList") != undefined &&
                        ldomToSetValue.getAttribute("sfwRenderAsRadioButtonList").toLowerCase().trim() == "true") {
                        lcontrolToSetValue.next(".drplistforcasddl").find("input").removeAttr('checked');
                    }
                    MVVMGlobal.SetFieldValueIntoModel(astrDivID, lstrControlID, valueToSet, ldomDiv);
                    MVVMGlobal.SetFieldValueIntoDirtyData(astrDivID, lstrControlID, valueToSet, ldomDiv);
                    Validator.removeErrorForControl(ldomDiv, lstrControlID);
                }
            }
        }
        var ldomcontrol = ldomDiv[0].querySelector([nsConstants.HASH, lstrControlID, "[hasClientVisibility]"].join(''));
        if (ldomcontrol != null && ldomcontrol != adomControl) {
            nsVisi.ApplyClientVisibilityToControl(astrDivID, $(ldomcontrol), true, ldomDiv);
            ns.VisibilityChangedFromCode = false;
        }
    }
}
nsVisi.ChangeVisibility = ChangeVisibility;

function ShowTagListForSearchCriteria(SearchCriteria, lstrFormID, ActiveDivID) {
    if (lstrFormID.toLowerCase().indexOf("centerleft") > 0) {
        return;
    }
    var taglistitems = [];
    var ActiveDiv = $("#" + ActiveDivID);
    for (var key in SearchCriteria) {
        if (key == "FormID" || key == "IsRetrivalForm" || key.indexOf("~Soundex") > 0) {
            continue;
        }
        var value = SearchCriteria[key];
        if (value == null || value == "" || (value.trim != undefined && value.trim() == "")) {
            continue;
        }
        var caption = key;
        var element = ActiveDiv.find("#" + key);
        if (element == undefined || element.length == 0) {
            continue;
        }
        var lHiddenElement = element;
        if (element[0].tagName == nsConstants.SELECT_TAG && element[0].getAttribute("multiple") === "multiple") {
            lHiddenElement = element.next();
        }
        if (lHiddenElement.css("display") == "none") {
            if (!(element[0].tagName == "SELECT" && element[0].getAttribute("sfwRenderAsRadioButtonList") != undefined &&
                element[0].getAttribute("sfwRenderAsRadioButtonList").toLowerCase().trim() == "true")) {
                continue;
            }
        }
        var parentControl = "";
        var capLabel = ActiveDiv.find("[for='" + key + "']");
        if (capLabel.length > 0) {
            caption = capLabel.text();
        }
        else {
            var previousTdText = element.closest("td").prev("td").text();
            if (previousTdText != "") {
                caption = previousTdText;
            }
        }
        caption = caption.replace(":", "");
        var control = key;
        if (element[0].tagName == "SELECT" && element[0].getAttribute("multiple") !== "multiple") {
            value = element.find("option:selected").text();
            var sfwParentControl = MVVMGlobal.GetControlAttribute(element, "sfwParentControl");
            if (sfwParentControl != null) {
                parentControl = sfwParentControl;
            }
        }
        else if (element[0].tagName == nsConstants.SELECT_TAG && element[0].getAttribute("multiple") === "multiple") {
            var selectedvalues = value.split(',');
            if (selectedvalues.length > 0) {
                for (var i in selectedvalues) {
                    var optionValue = selectedvalues[i];
                    if (optionValue == "") {
                        continue;
                    }
                    var valueText = element.find("option[value='" + optionValue + "']").text();
                    taglistitems.push({
                        caption: caption,
                        control: control,
                        value: optionValue,
                        valueText: valueText,
                        parentControl: parentControl
                    });
                }
                continue;
            }
        }
        if (element[0].tagName == "INPUT" && element[0].getAttribute("type") == "checkbox") {
            if (!element.is(":checked")) {
                continue;
            }
        }
        else if (element[0].getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) == nsConstants.SFW_CHECKBOX_LIST) {
            var selectedvalues = value.split(',');
            if (selectedvalues.length > 0) {
                for (var i in selectedvalues) {
                    var checkvalue = selectedvalues[i];
                    if (checkvalue == "" || checkvalue == "FW_CHECKALL") {
                        continue;
                    }
                    var valueText = element.find("[value='" + checkvalue + "']").parent().text();
                    if (ActiveDivID.indexOf("wfmPacketTrackingLookup") == 0) {
                        taglistitems.push({
                            caption: valueText,
                            control: control,
                            value: checkvalue,
                            valueText: checkvalue,
                            parentControl: parentControl
                        });
                    } 
                    else {
                        taglistitems.push({
                            caption: caption,
                            control: control,
                            value: checkvalue,
                            valueText: valueText,
                            parentControl: parentControl
                        });
                    }
                    
                }
                continue;
            }
        }
        else if ((element[0].tagName == "INPUT" && element[0].getAttribute("type") == "radio") || (element[0].getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) == nsConstants.SFW_RADIO_BUTTON_LIST)) {
            var valueText = element.find("[value='" + value + "']").parent().text();
            taglistitems.push({
                caption: caption,
                control: control,
                value: value,
                valueText: valueText,
                parentControl: parentControl
            });
            continue;
        }
        taglistitems.push({
            caption: caption,
            control: control,
            value: value,
            valueText: value,
            parentControl: parentControl
        });
    }
    var currentPanel = $(ns.viewModel.srcElement).closest(nsConstants.PANEL_CONTROL_TYPE_SELECTOR);
    if (currentPanel.length == 0) {
        return;
    }
    var searchtagsDiv = currentPanel.next(".searchtags");
    if (searchtagsDiv.length == 0) {
        currentPanel.after("<div class='searchtags'></div>");
        searchtagsDiv = currentPanel.next(".searchtags");
        searchtagsDiv.show();
    }
    var lobjNeoTags = lobjNeoTags = searchtagsDiv.data(nsConstants.NeoTags);
    if (taglistitems.length > 0) {
        if (lobjNeoTags == null) {
            searchtagsDiv.neoTags();
            lobjNeoTags = searchtagsDiv.data(nsConstants.NeoTags);
            lobjNeoTags.setClickCallback(function (control) {
                if (control.childNodes[0].className == "filters") { }
                else if (control.childNodes[0].className == "modifysearch") {
                    var currentSearchtag = $(control.parentElement.parentElement);
                    currentSearchtag.parent().find(".s-spnControlPanelbar").trigger("click");
                }
                else {
                    var lblnHitSearch = true;
                    var controlid = $(control).attr('controlid');
                    var element = $("#" + ActiveDivID + " #" + controlid);
                    if (element.length > 0) {
                        if (element[0].getAttribute(nsConstants.DATA_SFW_CONTROL_TYPE) == nsConstants.SFW_CHECKBOX_LIST) {
                            var checkvalue = $(control).attr("value");
                            element.find("[value='" + checkvalue + "']").attr('checked', "false").trigger("change");
                        }
                        else if ((element[0].tagName === nsConstants.SELECT_TAG && element[0].getAttribute("multiple") === "multiple")) {
                            var optionValue = $(control)[0].getAttribute("value");
                            var larrValues = element.val();
                            if (larrValues != undefined && larrValues.length > 0 && larrValues.indexOf(optionValue) >= 0) {
                                larrValues.splice(larrValues.indexOf(optionValue), 1);
                            }
                            element.val(larrValues).trigger("change");
                            MVVM.JQueryControls.MultiSelect.ReloadValues(element);
                        }
                        else if ((element[0].tagName.toLowerCase() === nsConstants.INPUT && element[0].getAttribute("type") == "checkbox")) {
                            element[0].checked = false;
                            element.trigger("change");
                        }
                        else {
                            element.val("").trigger("change");
                        }
                    }
                    if ($(control).parent().find("li[parentControl='" + controlid + "']").length > 0) {
                        nsEvents.RemoveParentControls(control, controlid);
                    }
                    if ($(control).parent().find("li").length == 1) {
                        ns.Templates[ActiveDivID].WidgetControls[currentPanel[0].id].expand();
                        lblnHitSearch = false;
                    }
                    $(control).fadeOut(500, function () {
                        $(control).remove();
                    });
                    if (lblnHitSearch) {
                        var searchbuttonid = ActiveDiv.attr("searchbuttonid");
                        if (searchbuttonid != null) {
                            var ClickFunction = function (controlelement) {
                                ActiveDiv.find("#" + searchbuttonid).trigger("click");
                                var currentSearchtag = controlelement;
                                currentSearchtag.parent().find(".s-spnControlPanelbar").trigger("click");
                            };
                            setTimeout(function () { return ClickFunction($(control.parentElement.parentElement)); }, 200);
                        }
                    }
                    else {
                        searchtagsDiv.hide();
                        var resetButton = ActiveDiv.find("[value='Reset']");
                        if (resetButton.length == 0) {
                            resetButton = ActiveDiv.find("[value='Refresh']");
                        }
                        resetButton.trigger("click");
                    }
                }
            });
        }
    }
    if (lobjNeoTags != null) {
        lobjNeoTags.setItems({ list: taglistitems });
        lobjNeoTags._refresh();
        searchtagsDiv.hide();
    }
}
nsEvents.ShowTagListForSearchCriteria = ShowTagListForSearchCriteria;

function SetLookupParamValues(ParamsToSet, DataToSet, ActiveDivID, FormContainerID, adomDiv, ablnFromRetrieval) {
    if (ParamsToSet !== undefined) {
        var lobjDataToJson = ablnFromRetrieval === true ? ParamsToSet : (DataToSet.toJSON ? DataToSet.toJSON() : DataToSet);
        for (var field in lobjDataToJson) {
            if (field === "FormID") {
                continue;
            }
            var lblnIsDataField = ablnFromRetrieval !== true || (DataToSet[field] !== undefined);
            var ldomDiv = adomDiv ? adomDiv : $([FormContainerID, nsConstants.SPACE_HASH, ActiveDivID].join(''));
            var Control = ldomDiv.find([nsConstants.HASH, field].join(''));
            if (Control.length > 0 && ParamsToSet[field] != undefined && lblnIsDataField) {
                if ((Control[0].getAttribute(nsConstants.CONTROL_TYPE) === nsConstants.SFW_CHECKBOX_LIST)
                    || (Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple")) {
                    if (typeof ParamsToSet[field] === "string") {
                        ParamsToSet[field] = ParamsToSet[field].split(",");
                    }
                    if (typeof DataToSet[field] === typeof ParamsToSet[field]) {
                        if (DataToSet.set) {
                            DataToSet.set(field, ParamsToSet[field]);
                        }
                        else {
                            DataToSet[field] = ParamsToSet[field];
                        }
                        if ((Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple")) {
                            MVVM.JQueryControls.MultiSelect.ReloadValues(Control);
                        }
                        continue;
                    }
                }
                else {
                    if (DataToSet.set) {
                        DataToSet.set(field, ParamsToSet[field]);
                    }
                    else {
                        DataToSet[field] = ParamsToSet[field];
                    }
                    continue;
                }
            }
            if (ablnFromRetrieval !== true) {
                var datafield = MVVMGlobal.GetControlAttribute(Control, "sfwDataField", ActiveDivID);
                if (datafield == undefined || ParamsToSet[datafield] === undefined) {
                    if (Control.length > 0 && ((Control[0].getAttribute(nsConstants.CONTROL_TYPE) === nsConstants.SFW_CHECKBOX_LIST)
                        || (Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple"))) {
                        DataToSet.set(field, []);
                        if ((Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple")) {
                            MVVM.JQueryControls.MultiSelect.ReloadValues(Control);
                        }
                    }
                    else if (Control[0].tagName !== nsConstants.SELECT_TAG || ParamsToSet[datafield] === undefined) {
                        DataToSet.set(field, "");
                    }
                }
                else if (ParamsToSet[datafield] != undefined) {
                    if (Control.length > 0 && ((Control[0].getAttribute(nsConstants.CONTROL_TYPE) === nsConstants.SFW_CHECKBOX_LIST)
                        || (Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple"))) {
                        var paramvalues = "";
                        if (typeof ParamsToSet[datafield] === "string") {
                            paramvalues = ParamsToSet[datafield].split(",").join(",");
                            ParamsToSet[datafield] = ParamsToSet[datafield].split(",");
                        }
                        if (!Array.isArray(ParamsToSet[datafield]) && typeof ParamsToSet[datafield] !== "string") {
                            paramvalues = String(ParamsToSet[datafield]).split(",").join(",");
                            ParamsToSet[datafield] = String(ParamsToSet[datafield]).split(",");
                        }
                        if (typeof DataToSet[field] === "string") {
                            ParamsToSet[datafield] = paramvalues;
                        }
                        if (typeof DataToSet[field] === typeof ParamsToSet[datafield]) {
                            if (DataToSet.set) {
                                DataToSet.set(field, ParamsToSet[datafield]);
                            }
                            else {
                                DataToSet[field] = ParamsToSet[datafield];
                            }
                            if ((Control[0].tagName === nsConstants.SELECT_TAG && Control[0].getAttribute("multiple") === "multiple")) {
                                MVVM.JQueryControls.MultiSelect.ReloadValues(Control);
                            }
                        }
                    }
                    else {
                        if (DataToSet.set) {
                            DataToSet.set(field, ParamsToSet[datafield]);
                        }
                        else {
                            DataToSet[field] = ParamsToSet[datafield];
                        }
                        if (Control.length > 0 && Control[0].getAttribute("IsCascadingDropDown") == 'true') {
                            Control.trigger('change', [true]);
                        }
                    }
                }
            }
        }
    }
}
nsCommon.SetLookupParamValues = SetLookupParamValues;

const originalCreateElement = document.createElement;
document.createElement = function (tagName) {
    const element = originalCreateElement.call(document, tagName);
    setTimeout(function () {
        if (tagName == "div") {
            var ldomDiv = $("#" + ns.viewModel.currentModel);
            if (ldomDiv.find('#GlobalMessageDiv').length > 1) {
                if (ns.viewModel.currentModel.indexOf("wfmQDROCalculationMaintenance") == 0) {
                    var allElements = ldomDiv.find('#GlobalMessageDiv');
                    for (i = 0; i < allElements.length; i++) {
                        if (i == 0)
                            continue;

                        allElements[i].remove();
                    }
                }
            }
        }
    }, 1);
    return element;
};
SessionEvents.InitTimer = function (anumSessionTimeout) {
    window.removeEventListener('storage', SessionEvents.storageListner);
    window.addEventListener('storage', SessionEvents.storageListner);
    SessionEvents.TimerStartTime = new Date();
    nsCommon.localStorageSet(SessionEvents.GetWindowName() + "_TimerStartTime", SessionEvents.TimerStartTime);
    var SessionTimeout = anumSessionTimeout;
    if (SessionTimeout == undefined) {
        SessionTimeout = 60;
        SessionEvents.iintSessionTimeout = 60;
    }
    else {
        SessionEvents.iintSessionTimeout = SessionTimeout;
    }
    var total = ((SessionTimeout * 60) - 31) * 1000;
    if ($.idleTimer == undefined)
        return;
    $.idleTimer(total);
    $(document).off("idle.idleTimer");
    $(document).on("idle.idleTimer", function (event, elem, obj) {
        if (ns.iblnFileUploadInProgress === true || ns.blnLoading === true) {
            ns.refreshSession();
            nsUserFunctions.idteLastRequestCompletedTime = new Date();
            if ($('#lblCurrTime').length > 0) {
                $('#lblCurrTime').text(GetCurrentDateTime(new Date()));
            }
        }
        else {
            SessionEvents.TimerReset = false;
            SessionEvents.ShowTimeoutWarning(SessionEvents.iintSessionRemainingTimer, true);
            SessionEvents.countdown(SessionEvents.iintSessionRemainingTimer, ns.logoutSesssion);
        }
    });
}
SessionEvents.ResetTimer = function (aintSessionTimeout, ablnResetStartTime) {
    if (!SessionEvents.TimerStartTime) {
        SessionEvents.TimerStartTime = new Date();
    }
    if (ablnResetStartTime == undefined || ablnResetStartTime) {
        var localTimeStart = new Date();
        SessionEvents.TimerStartTime = localTimeStart;
        nsCommon.localStorageSet(SessionEvents.GetWindowName() + "_TimerStartTime", SessionEvents.TimerStartTime);
        SessionEvents.TimerStartTime = localTimeStart;
    }
    if (SessionEvents.iintSessionTimeout === -1) {
        return;
    }
    if ($(document).data("idleTimerObj") == undefined) {
        SessionEvents.InitTimer(SessionEvents.iintSessionTimeout);
    }
    $(document).idleTimer("reset");
    if (SessionEvents.timer) {
        SessionEvents.timer.reset();
        SessionEvents.timer = null;
    }
    SessionEvents.TimerReset = true;
    SessionEvents.ShowTimeoutWarning(1, false);
    nsUserFunctions.idteLastRequestCompletedTime = new Date();
}
SessionEvents.storageListner = function (event) {
    if (event.key === SessionEvents.GetWindowName() + "_TimerStartTime") {
        SessionEvents.TimerStartTime = new Date(nsCommon.localStorageGet(event.key));
        SessionEvents.ResetTimer(undefined, false);
    }
    else if (event.key === SessionEvents.GetWindowName() + "_LO") {
        ns.logoutSesssion("SessionTimeout");
    }
    else if (event.key === SessionEvents.GetWindowName() + "_CID") {
        window.location = window.location.origin + window.location.pathname;
    }
}
ns.logoutSesssion = function(astrReason) {
    nsEvents.onWindowUnload = function () { };
    var fn = nsUserFunctions["logoutSesssion"];
    if (typeof fn === 'function') {
        var result = fn(astrReason);
        if (!result) {
            return;
        }
    }
    if (window["nsCenterLeftRefresh"] && typeof window["nsCenterLeftRefresh"]["stop"] === "function") {
        nsCenterLeftRefresh.stop();
    }
    var Prefix = MVVMGlobal.GetPrefixforAjaxCall();
    if (location.pathname === "/") {
        Prefix = "";
    }
    else if (location.pathname === "/".concat(ns.SiteName, "/")) {
        Prefix = "";
    }
    else if (location.pathname === "/".concat(ns.SiteName)) {
        Prefix = "/".concat(ns.SiteName, "/");
    }
    else if (Prefix == "") {
        Prefix = "/";
    }
    nsCommon.localStorageSet(SessionEvents.GetWindowName() + "_LO", true);
    window.location.href = HtmlWhitelistedSanitizer.sanitizeHTMLString([Prefix, "account/logout"].join(''));
}
nsUserFunctions.BindingBpmScreen = false;
fmBindFormData = ns.bindFormData;
ns.bindFormData = function (aobjData, ablnRebindData) {
    if (aobjData.SrcElement != undefined && (aobjData.SrcElement == "FromMenu" && aobjData.DomainModel != undefined && aobjData.DomainModel.OtherData != undefined && aobjData.DomainModel.OtherData["ActivityInstanceId"] != undefined)) {
        nsUserFunctions.BindingBpmScreen = true;
        if (aobjData.ExtraInfoFields != undefined && aobjData.ExtraInfoFields["LaunchingFormId"] != undefined && aobjData.ExtraInfoFields["LaunchingFormId"].contains("wfmBPMMyBasketMaintenance")) {
            
            ns.isWorkflowBasket = false;
        }
        else {           
            ns.isWorkflowBasket = true;            
        }
    }
    fmBindFormData(aobjData, ablnRebindData);
    if (nsUserFunctions.BindingBpmScreen) {
        nsUserFunctions.BindingBpmScreen = false;
        ns.isWorkflowBasket = false;
        $("#custom-loader").hide();
    }
}

FM_OpenFormOnLeft = MVVMGlobal.OpenFormOnLeft;
MVVMGlobal.OpenFormOnLeft = function(dataItem) {
    ns.activityStart();
    ns.isRightSideForm = false;
    var lblnFormExist = MVVMGlobal.isFormAlreadyExistinDom(dataItem.divID);
    if (dataItem.divID.indexOf(nsConstants.LOOKUP) > 0)
        lblnFormExist = true;
    var lblnIsNewForm = false;
    if ((ns.viewModel[dataItem.divID] != undefined && ns.viewModel[dataItem.divID].ExtraInfoFields != undefined)) {
        lblnIsNewForm = ns.viewModel[dataItem.divID].ExtraInfoFields["IsNewForm"] == nsConstants.TRUE;
    }
    var lblnIsCenterLeftSorce = false;
    if ((ns.viewModel != undefined && ns.viewModel.srcElement != undefined)
        && (ns.viewModel.srcElement.id == "btnOpenActivityCenterLeft" || ns.viewModel.srcElement.id == "btnCheckoutActivity")) {
        lblnIsCenterLeftSorce = true;
    }
    if (ns.iblnOpenRefreshedForm && !lblnIsNewForm && !ns.blnDontUpdateUrl && !lblnIsCenterLeftSorce)
        lblnFormExist = false;
    var lblnIsIntraAppForm = dataItem[nsConstants.IntraAppCommunication.ATTR_IS_INTRA_APP_FORM] || false;
    if (ns.FormOpenedOnLeft !== undefined) {
        dataItem.previousForm = ns.FormOpenedOnLeft.divID;
        if (!lblnIsIntraAppForm) {
            MVVMGlobal.hideDiv([nsConstants.HASH, ns.FormOpenedOnLeft.divID].join(''));
            if (ns.FormOpenedOnLeft.divID.indexOf(nsConstants.MAINTENANCE) > 0 && !lblnFormExist
                && ns.FormOpenedOnLeft.divID != dataItem.divID) {
                ns.destroyAll(ns.FormOpenedOnLeft.divID);
            }
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
        ns.iblnIsIntraAppForm = lblnIsIntraAppForm;
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
                if (dataItem != undefined && dataItem.formID != "wfmBPMMyBasketMaintenance" && dataItem.SenderID == "btnSearchActivities") {
                    nsCommon.GetDataItemFromDivID(ns.viewModel.currentModel).SenderID = "";
                }
                if (lblnIsIntraAppForm) {
                    nsIntraAppCommunication.OpenFromNavigator(dataItem.divID, dataItem.PrimaryKey, dataItem.navParams);
                    ns.displayActivity(false);
                    ns.activityComplete();
                }
                else {
                    ns.viewModel.currentModel = dataItem.modelID;
                    nsCommon.sessionSet([dataItem.modelID, "_Params"].join(''), dataItem.PrimaryKey);
                    nsEvents.raiseEvent(ns.getData);
                }
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

function formatDate(value) {
    return value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
}

fmHandleAjaxError = nsRequest.HandleAjaxError;
nsRequest.HandleAjaxError = function (jqXHR, textStatus, errorThrown, nsDeferred) {
    $("#custom-loader").hide();
    fmHandleAjaxError(jqXHR, textStatus, errorThrown, nsDeferred);
}
