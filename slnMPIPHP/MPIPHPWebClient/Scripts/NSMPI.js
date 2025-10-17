// This Flag with determine whether we are coming via PostBack or we are just Client Side
// It will help to populate the values based on the right condition

var QrdoLoadFlag = true;

var TaxIdentifierVal = "";
var BenDistType = "";
var PlanId = 0;


function SetQdroBenefitScreenInfo() {
    var lddlPlanId = $$("ddlPlanId");
    QrdoLoadFlag = true;
    lddlPlanId.change(SetDefaultValuesforQdro);
}


function SetDefaultValuesforQdro() {
    var lddlPlanId = $$("ddlPlanId");
    var lddlDroModelId = $$("ddlDroModelId")

    if (lddlPlanId[0].length == 0 && QrdoLoadFlag == false) {
        $$("chkAltPayeeIncrease")[0].checked = false;
        $$("chkAltPayeeEarlyRet")[0].checked = false;
        $$("txtBenefitPerc")[0].value = "";
    }

    if (lddlPlanId[0].length == 0 || lddlDroModelId[0].length == 0 || QrdoLoadFlag == true) {
        QrdoLoadFlag = false;
        return;
    }
    var current_plan_id = lddlPlanId[0].value;
    var current_dro_id = lddlDroModelId[0].value;

    if (current_plan_id == "2" && (current_dro_id == "STRF" || current_dro_id == "STAF" || current_dro_id == "SPDQ")) {
        $$("chkAltPayeeIncrease")[0].checked = true;
    }
    else {
        $$("chkAltPayeeIncrease")[0].checked = false;
    }


    if (current_plan_id == "2" && current_dro_id == "STAF") {
        $$("chkAltPayeeEarlyRet")[0].checked = true;
    }
    else {
        $$("chkAltPayeeEarlyRet")[0].checked = false;
    }




    if (current_dro_id != "SSUP" && current_dro_id != "OTHR" && current_dro_id != "CSUP" && current_plan_id != "") {
        $$("txtBenefitPerc")[0].value = 50;
    }
    else {
        $$("txtBenefitPerc")[0].value = "";
    }
    $$("txtBenefitAmt")[0].value = "";
}
var tmp;

function OnSelectedIndexChange() {
    var lddlPlanId = $$("ddlIaintPlan1");
    var lddlBenType = $$("ddlBeneficiaryTypeValue");
    if (lddlPlanId[0].length > 0 && lddlBenType[0].length > 0) {
        var current_plan_id = lddlPlanId[0].value;
        var Ben_Type = lddlBenType[0].value;
        if (current_plan_id != "0") {
            GetPlanExists(current_plan_id, Ben_Type);
        }

    }
}
function GetPlanExists(Planid, BenType) {
    $.ajax({
        beforeSend: function (request) { request.setRequestHeader('FWN', window.name); },
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "SagitecWebServices.asmx/GetPlanExists",
        data: "{astrPlanId: \"" + Planid + "\", astrBenType: \"" + BenType + "\"}",
        dataType: "json",
        success: function (result) {
            tmp = result.d;
            if (result.d == "100") {
                $$("txtDistPercent")[0].value = 100;
            }
            else if (result.d == "50") {
                $$("txtDistPercent")[0].value = 50;
            }
        }
    });
}

//Functions related to modal jQuery dialog
function OpenVIPDialog() {
    //Initialize the Cancel and Sbumit button jQuery handlers
    $("#btnjQueryCancel").click(function (e) {
        HideDialog();
        e.preventDefault();
    });

    //Show the jQuery modal dialog
    ShowDialog(true);
}

function ShowDialog(modal) {
    $("#overlay").show();
    $("#dialog").fadeIn(300);

    if (modal) {
        //FM upgrade: 6.0.1.1 changes - Upgrade Framework with latest jQuery version 3.2.1
        //$("#overlay").unbind("click");
        $("#overlay").off("click");
    }
    else {
        $("#overlay").click(function (e) {
            HideDialog();
        });
    }
}

function HideDialog() {
    $("#overlay").hide();
    $("#dialog").fadeOut(300);
}


function SetJoinderInfo() {
    var ldtJoinderRcvdDate = $$("txtJoinderRecvDate");
    QrdoLoadFlag = true;
    ldtJoinderRcvdDate.change(SetJoinderOnFile);
}

function SetJoinderOnFile() {
    var ldtJoinderRcvdDate = $$("txtJoinderRecvDate");

    if (ldtJoinderRcvdDate[0].value == "" || ldtJoinderRcvdDate[0].value == null || ldtJoinderRcvdDate[0].value.endsWith("_")) {
        $$("chkJoinderOnFile")[0].checked = false;
    }
    else {
        $$("chkJoinderOnFile")[0].checked = true;
    }

}

function GetLaserFische() {
    $.ajax({
        beforeSend: function (request) { request.setRequestHeader('FWN', window.name); },
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "SagitecWebServices.asmx/GetLaserFicheUrlfromDB",
        data: "",
        dataType: "json",
        success: function (result) {
            // tmp = result.d;
            var obj = jQuery.parseJSON(result.d);
            $(obj).each(function (i, val) {
                $.each(val, function (key, val) {

                    window.open("" + val + "#view=search;search={[]:[Department] = \"Retirement Benefits\"} & {[]:[MPID] = \"" + $$("btnMpiPersonId").text() + "\"}", "OpenURL", "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes", null);

                });
            });

        }
    });
}

function ShowWebExtenderURL() {
    $.ajax({
        beforeSend: function (request) { request.setRequestHeader('FWN', window.name); },
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "SagitecWebServices.asmx/GetWebExFlagDB",
        data: "",
        dataType: "json",
        success: function (result) {
            // tmp = result.d;
            var obj = jQuery.parseJSON(result.d);
            $(obj).each(function (i, val) {


                $.each(val, function (key, val) {
                    if (val != "Y") {
                        //LaserFische Url
                        GetLaserFische();

                    } else {
                        //OpusWebExtendUrl
                        var mpi_person_id = $$("lblSsnWeb");
                        window.open("http://webx/AppXtender/ISubmitQuery.aspx?Appname=PENSION_DOCS&DataSource=Imaging&QueryType=0&SSN=" + mpi_person_id[0].outerText, "OpenURL", "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes", null);
                        return false;

                    }



                });
            });

        }
    });


}


function SetPayeeMPIDValue() {
    var lddlReissuePaymentType = $$("ddlReissuePaymentTypeId");
    lddlReissuePaymentType.change(SetDefaultPayeeMPIDValue);
}

function SetDefaultPayeeMPIDValue() {

    var lddlReissuePaymentType = $$("ddlReissuePaymentTypeId");
    var current_identifier = lddlReissuePaymentType[0].value;

    var txtPayeeMPID = $$("txtIstrPayeeMPID");
    var current_txtPayeeMPID = txtPayeeMPID[0].value;


    var txtOrgMPID = $$("txtIstrOrgMPID");
    var current_OrgId = txtOrgMPID[0].value;


    if (current_identifier == "PYEE" || current_identifier == "ROTP") {
        $$("txtIstrRMPIDAddPayee")[0].value = current_txtPayeeMPID;
    }
    else {
        $$("txtIstrRMPIDAddPayee")[0].value = " ";
    }
    if (current_identifier == "TORG" && current_OrgId != "") {
        $$("txtIstrRMPIDAddPayee")[0].value = current_OrgId;
    }


}

function SetRepaymentFlatPercentage() {
    var lddlRepaymentTypeValue = $$("ddlRepaymentTypeValue");
    lddlRepaymentTypeValue.change(SetDefaultRepaymentFlatPercentage);
}

function SetDefaultRepaymentFlatPercentage() {

    var ddlRepaymentTypeValue = $$("ddlRepaymentTypeValue");
    var current_identifier = ddlRepaymentTypeValue[0].value;

    if (current_identifier == "PERC") {

        if ($$("txtFlatPercentage")[0].value == 0 || $$("txtFlatPercentage")[0].value == null) {

            $$("txtFlatPercentage")[0].value = 25;
            $$("txtNextAmountDue")[0].value = 0;
            $$("txtFlatPercentage").trigger("change");
        }
    }
    else {
        $$("txtFlatPercentage")[0].value = 0;
    }

}

function SetDisableNextDueAmount() {
    var lddlRepaymentTypeValue = $$("ddlRepaymentTypeValue");
    lddlRepaymentTypeValue.change(SetDefaultDisableNextDueAmount);
}

function ONResumeButtonClick() {
    var lbtnResume = $$("btnResumeBenefits");
    lbtnResume.trigger("click");
}



function SetDefaultDisableNextDueAmount() {

    var ddlRepaymentTypeValue = $$("ddlRepaymentTypeValue");
    var current_identifier = ddlRepaymentTypeValue[0].value;

    if (current_identifier == "PERC" || current_identifier == null) {
        $$("txtNextAmountDue").attr("disabled", true);
    }
    else {
        $$("txtNextAmountDue").removeAttr("disabled");
    }
    $$("txtNextAmountDue").trigger("change");
}

function SetTaxWithHoldingValues() {

    //    var lddlTaxOption = $$("ddlTaxOptionValue11");
    //    lddlTaxOption.change(SetDefaultValuesforTaxWithHolding);

    //    var lddlBenefitDistributionType = $$("ddlBenefitDistributionTypeValue1");
    //    lddlBenefitDistributionType.change(SetDefaultValuesforTaxWithHolding);
}

function processmanually() {
    var txtIstrPaymentMethod = $$("txtIstrPaymentMethod");
    if (txtIstrPaymentMethod[0].value == "Wire" || txtIstrPaymentMethod[0].value == "WIRE") {
        confirm('Please contact Accounting Department/Bank to process manually');
        return true;
    }


}


$(document).on("change", "#cphCenterMiddle_ddlIAPBenefitOpt", function (e) {

    var benefitOptType = $$("ddlIAPBenefitOpt").find('option:selected').val();

    if (benefitOptType == "J100" || benefitOptType == "JA66" || benefitOptType == "JP50" || benefitOptType == "JPOP" || benefitOptType == "JS66" ||
        benefitOptType == "JS75" || benefitOptType == "JSAA") {

        $$("lblIAPJointAnnunantName").show();
        $$("ddlIAPJointAnnunantName").show();
        $$("capIAPSpouseConsent").show();
        $$("chkIAPSpouseConsent").show();

    } else if (benefitOptType == "QJ50") {

        $$("lblIAPJointAnnunantName").show();
        $$("ddlIAPJointAnnunantName").show();

    } else {

        $$("lblIAPJointAnnunantName").hide();
        $$("ddlIAPJointAnnunantName").hide();

        var IntPlan = $$("ddlIintPlanID").find('option:selected').val();
        if (IntPlan == 2) {

            $$("capIAPSpouseConsent").show();
            $$("chkIAPSpouseConsent").show();

        }
    }



});

$(document).on("change", "#cphCenterMiddle_ddlBenefitOpt", function (e) {

    var benefitOptType1 = $$("ddlBenefitOpt").find('option:selected').val();

    if (benefitOptType1 == "J100" || benefitOptType1 == "JA66" || benefitOptType1 == "JP50" || benefitOptType1 == "JPOP" || benefitOptType1 == "JS66" ||
        benefitOptType1 == "JS75" || benefitOptType1 == "JSAA") {

        $$("lblJointAnnunantName").show();
        $$("ddlJointAnnunantName").show();
        $$("chkSpousalConsent").show();
        $$("capSpouseConsent").show();

    } else if (benefitOptType1 == "QJ50") {

        $$("lblJointAnnunantName").show();
        $$("ddlJointAnnunantName").show();

    } else {

        $$("lblJointAnnunantName").hide();
        $$("ddlJointAnnunantName").hide();
        $$("chkSpousalConsent").show();
        $$("capSpouseConsent").show();
        $$("capIAPSpouseConsent").hide();
        $$("chkIAPSpouseConsent").hide();

    }



});


$(document).on("change", "#cphCenterMiddle_ddlPaymentMethod", function (e) {

    var paymentType = $$("ddlPaymentMethod").find('option:selected').val();

    if (paymentType == "CHK") {

        $$("capIintRoutingNumber").hide();
        $$("lblLabelRoutingNumber").hide();
        $$("txtIintRoutingNumber").hide();
        $$("capIstrOrgName").hide();
        $$("txtIstrOrgName").hide();
        $$("lblLabelReq1").hide();
        $$("capAccountNumber").hide();
        $$("txtAccountNumber").hide();
        $$("lblLabelReq2").hide();
        $$("capBankAccountTypeValue").hide();
        $$("ddlBankAccountTypeValue").hide();
        $$("lblLabelReq3").hide();
        $$("capAchStartDate").hide();
        $$("txtAchStartDate").hide();
        $$("capAchEndDate").hide();
        $$("txtAchEndDate").hide();
        $$("txtIintRoutingNumber_autobutton").hide();
        $$("capPreNoteFlag").hide();
        $$("capPreNoteCompletionDate").hide();
        $$("chkPreNoteFlag").hide();
        $$("lblPreNoteCompletionDate").hide();
        $$("capJointAccountOwner").hide();
        $$("chkJointAccountOwner").hide();
        $$("lblIstrInformation").show();

    } else {
        $$("capIintRoutingNumber").show();
        $$("lblLabelRoutingNumber").show();
        $$("txtIintRoutingNumber").show();
        $$("capIstrOrgName").show();
        $$("txtIstrOrgName").show();
        $$("lblLabelReq1").show();
        $$("capAccountNumber").show();
        $$("txtAccountNumber").show();
        $$("lblLabelReq2").show();
        $$("capBankAccountTypeValue").show();
        $$("ddlBankAccountTypeValue").show();
        $$("lblLabelReq3").show();
        $$("capAchStartDate").show();
        $$("txtAchStartDate").show();
        $$("capAchEndDate").show();
        $$("txtAchEndDate").show();
        $$("txtIintRoutingNumber_autobutton").show();
        $$("capPreNoteFlag").show();
        $$("capPreNoteCompletionDate").show();
        $$("chkPreNoteFlag").show();
        $$("lblPreNoteCompletionDate").show();
        $$("capJointAccountOwner").show();
        $$("chkJointAccountOwner").show();
        $$("lblIstrInformation").hide();

    }



});

//$$("capEligibleIAPPlan").hide();
//$$("txtEligibleIAPPlan").hide();

$(document).on("change", "#cphCenterMiddle_ddlIintPlanID", function (e) {
    PlanId = $$("ddlIintPlanID").find('option:selected').val();

    var eligiblePlan = $$("ddlIintPlanID").find('option:selected').text();
    var txtEligiblePlan = $$("txtEligiblePlan");
    var txtEligibleIAPPlan = $$("txtEligibleIAPPlan");
    txtEligiblePlan[0].value = eligiblePlan;
    txtEligibleIAPPlan[0].value = "Individual Account Plan";


    if (PlanId == 2) {

        $$("capEligibleIAPPlan").show();
        $$("txtEligibleIAPPlan").show();
        $$("capIAPBenefitOption").show();
        $$("ddlIAPBenefitOpt").show();
        $$("capIAPSpouseConsent").show();
        $$("chkIAPSpouseConsent").show();
        $$("lblIAPJointAnnunantName").show();
        $$("ddlIAPJointAnnunantName").show();

        $$("capEligiblePlan").show();
        $$("txtEligiblePlan").show();
        $$("capBenefitOption").show();
        $$("ddlBenefitOpt").show();
        $$("capSpouseConsent").show();
        $$("chkSpouseConsent").show();
        $$("lblJointAnnunantName").show();
        $$("ddlJointAnnunantName").show();


    } else {

        $$("capEligiblePlan").show();
        $$("txtEligiblePlan").show();
        $$("capBenefitOption").show();
        $$("ddlBenefitOpt").show();
        $$("capSpouseConsent").show();
        $$("chkSpouseConsent").show();
        $$("lblJointAnnunantName").show();
        $$("ddlJointAnnunantName").show();

        $$("capEligibleIAPPlan").hide();
        $$("txtEligibleIAPPlan").hide();
        $$("capIAPBenefitOption").hide();
        $$("ddlIAPBenefitOpt").hide();
        $$("capIAPSpouseConsent").hide();
        $$("chkIAPSpouseConsent").hide();
        $$("lblIAPJointAnnunantName").hide();
        $$("ddlIAPJointAnnunantName").hide();
    }


});


function RetirementWizardHideControls() {
    $$("capEligibleIAPPlan").hide();
    $$("txtEligibleIAPPlan").hide();
    $$("capIAPBenefitOption").hide();
    $$("ddlIAPBenefitOpt").hide();
    $$("capIAPSpouseConsent").hide();
    $$("chkIAPSpouseConsent").hide();
    $$("lblIAPJointAnnunantName").hide();
    $$("ddlIAPJointAnnunantName").hide();

    $$("capEligiblePlan").hide();
    $$("txtEligiblePlan").hide();
    $$("capBenefitOption").hide();
    $$("ddlBenefitOpt").hide();
    $$("capSpouseConsent").hide();
    $$("chkSpouseConsent").hide();
    $$("lblJointAnnunantName").hide();
    $$("ddlJointAnnunantName").hide();

}

function RetirementWizardFedTaxHolding() {

    $$("lblWtxtStep2B").show();
    $$("txtWStep2B").show();
    $$("lbltxtWStep3Amount").show();
    $$("txtWStep3Amount").show();
    $$("lbltxtWStep4a").show();
    $$("txtWStep4a").show();
    $$("lblWtxtStep4b").show();
    $$("txtWStep4b").show();
    $$("lblWtxtStep4c").show();
    $$("txtWStep4c").show();
    $$("lblwtxtTaxAllowance").hide();
    $$("txtwTaxAllowance").hide();
    $$("lblwtxtAdditionalTaxAmount1").hide();
    $$("txtwAdditionalTaxAmount1").hide();
    $$("lblwtxtTaxPercentage").hide();
    $$("txtwTaxPercentage").hide();

}

$(document).on("change", "#cphCenterMiddle_ddlWizardBenefitDistributionTypeValue1", function (e) {

    var WizardBenefitDistributionTypeValue1 = $$("ddlWizardBenefitDistributionTypeValue1").find('option:selected').val();

    if (WizardBenefitDistributionTypeValue1 == "LSDB") {

        $$("lblwtxtTaxAllowance").show();
        $$("txtwTaxAllowance").show();
        $$("lblwtxtAdditionalTaxAmount1").show();
        $$("txtwAdditionalTaxAmount1").show();
        $$("lblwtxtTaxPercentage").show();
        $$("txtwTaxPercentage").show();


        $$("lblWtxtStep2B").hide();
        $$("txtWStep2B").hide();
        $$("lbltxtWStep3Amount").hide();
        $$("txtWStep3Amount").hide();
        $$("lbltxtWStep4a").hide();
        $$("txtWStep4a").hide();
        $$("lblWtxtStep4b").hide();
        $$("txtWStep4b").hide();
        $$("lblWtxtStep4c").hide();
        $$("txtWStep4c").hide();

    } else {

        RetirementWizardFedTaxHolding();
    }


});


//FM upgrade: 6.0.1.1 changes - replaced live by on
//$$("ddlTaxIdentifierValue").on("change", function () {
$(document).on("change", "#cphCenterMiddle_ddlTaxIdentifierValue", function (e) {
    TaxIdentifierVal = $$("ddlTaxIdentifierValue").find('option:selected').val();
    //GetTaxWithHoldingScreenConfiguratorColumns(TaxIdentifierVal);
    //(TaxIdentifierVal != 'FDRL' && BenDistType)
    if (BenDistType != "") {
        GetTaxWithHoldingScreenConfiguratorColumns(TaxIdentifierVal, BenDistType);
    }

    var lddlMaritalStatusValue11 = $$("ddlMaritalStatusValue11");
    lddlMaritalStatusValue11 = lddlMaritalStatusValue11[0].value;

    var lddlTaxOption = $$("ddlTaxOptionValue11");
    lddlTaxOption = lddlTaxOption[0].value;


    if (lddlMaritalStatusValue11 == "" && lddlTaxOption == "") {

        var dt = new Date($.now());

        var DateCreated = new Date(Date.parse(dt)).format("MM/dd/yyyy");

        $$("txtStartDate1")[0].value = DateCreated;
        $$("txtEndDate1")[0].value = "";
        $$("txtTaxAllowance")[0].value = 0;
        $$("txtAdditionalTaxAmount1")[0].value = 0;
        $$("txtTaxPercentage")[0].value = 0;
        $$("txtStep2B")[0].value = 0;
        $$("txtStep3Amount")[0].value = 0;
        $$("txtStep4a")[0].value = 0;
        $$("txtStep4b")[0].value = 0;
        $$("txtStep4c")[0].value = 0;
        $$("txtPersonalExemptions")[0].value = 0;
        $$("txtAgeandBlindnessExemptions")[0].value = 0;
        $$("txtVoluntary_Withholding")[0].value = 0;


    }

    if (TaxIdentifierVal == "VAST") {

        $$("ddlMaritalStatusValue11").hide();

    } else {

        $$("ddlMaritalStatusValue11").show();
    }

    //SetFederalTaxFieldVisibleStatus();
});

function GetTaxWithHoldingScreenConfiguratorColumns(TaxIdentifierVal) {

    var lddlTaxOption = $$("ddlTaxOptionValue11");
    var currrent_taxoption = lddlTaxOption[0].value;
    $.ajax({
        beforeSend: function (request) { request.setRequestHeader('FWN', window.name); },
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "SagitecWebServices.asmx/GetTaxWithHoldingScreenConfiguratorColumns",
        //  data: "{astrTaxIdentifierVal: \"" + TaxIdentifierVal + "\"}",
        data: "{astrTaxIdentifierVal: \"" + TaxIdentifierVal + "\", astrBenType: \"" + BenDistType + "\"}",
        dataType: "json",
        success: function (result) {
            var obj = jQuery.parseJSON(result.d);

            $(obj).each(function (i, val) {
                //collection keys are table column alias names - do not change the column name.
                $.each(val, function (key, val) {
                    //alert(key, val);
                    //alert(currrent_taxoption);
                    if ($$(key).length > 0) {
                        if (val != "Y") {

                            $$(key).hide()
                            $$("lbl" + key).hide()
                        } else {

                            $$("lbl" + key).show();
                            $$(key).show();
                        }

                    }

                    if (TaxIdentifierVal == "VAST") {
                        $$("ddlMaritalStatusValue11").hide();
                        if (currrent_taxoption == "FLAD") {

                            $$("lbltxtVoluntary_Withholding").show();
                            $$("txtVoluntary_Withholding").show();
                            $$("lbltxtPersonalExemptions").hide();
                            $$("txtPersonalExemptions").hide();
                            $$("lbltxtAgeandBlindnessExemptions").hide();
                            $$("txtAgeandBlindnessExemptions").hide();

                        } else {
                            $$("lbltxtVoluntary_Withholding").hide();
                            $$("txtVoluntary_Withholding").hide();
                            $$("lbltxtPersonalExemptions").show();
                            $$("txtPersonalExemptions").show();
                            $$("lbltxtAgeandBlindnessExemptions").show();
                            $$("txtAgeandBlindnessExemptions").show();

                        }

                        if (currrent_taxoption == "STAT") {

                            $$("lbltxtAdditionalTaxAmount1").show();
                            $$("txtAdditionalTaxAmount1").show();

                        } else {
                            $$("lbltxtAdditionalTaxAmount1").hide();
                            $$("txtAdditionalTaxAmount1").hide();

                        }

                    }


                });
            });



        }
    });



}

$(document).on("change", "#cphCenterMiddle_ddlDropDownList12", function (e) {

    if (this.value == "VAST") {

        $$("txtPersonalExemption").removeAttr("disabled");
        $$("txtAgeandBlindnessExemptions").removeAttr("disabled");

    } else {
        $$("txtPersonalExemption")[0].value = 0;
        $$("txtAgeandBlindnessExemptions")[0].value = 0;
        $$("txtPersonalExemption").attr("disabled", true);
        $$("txtAgeandBlindnessExemptions").attr("disabled", true);
    }



});

$(document).on("change", "#cphCenterMiddle_ddlTaxOptionValue11", function (e) {

    if (TaxIdentifierVal == "VAST") {

        if (this.value == "FLAD") {

            $$("lbltxtVoluntary_Withholding").show();
            $$("txtVoluntary_Withholding").show();
            $$("lbltxtPersonalExemptions").hide();
            $$("txtPersonalExemptions").hide();
            $$("lbltxtAgeandBlindnessExemptions").hide();
            $$("txtAgeandBlindnessExemptions").hide();

        } else {
            $$("lbltxtVoluntary_Withholding").hide();
            $$("txtVoluntary_Withholding").hide();
            $$("lbltxtPersonalExemptions").show();
            $$("txtPersonalExemptions").show();
            $$("lbltxtAgeandBlindnessExemptions").show();
            $$("txtAgeandBlindnessExemptions").show();

        }

        if (this.value == "STAT") {

            $$("lbltxtAdditionalTaxAmount1").show();
            $$("txtAdditionalTaxAmount1").show();

        } else {
            $$("lbltxtAdditionalTaxAmount1").hide();
            $$("txtAdditionalTaxAmount1").hide();

        }

    }


    if (this.value == "FTIR" || this.value == "FTIA" || this.value == "STAT" || this.value == "STST" || this.value == "FLAP" || this.value == "FLAD" || this.value == "NSTX" || this.value == "NFTX") {
        if ($$("txtTaxAllowance")[0].value != 0 || $$("txtTaxAllowance")[0].value != "") {
            $$("txtTaxAllowance")[0].value = 0;
        }
    }


    //Ticket#73404
    SetDefaultValuesforTaxWithHolding();



});
//FM upgrade: 6.0.1.1 changes - replaced live by on
//$$("ddlTaxOptionValue11").on("change", function () {



//        //Ticket#98349   
//    if (this.value == "FTIR" || this.value == "FTIA" || this.value == "STAT" || this.value == "STST"){
//        if ($$("txtTaxAllowance")[0].value == 0 || $$("txtTaxAllowance")[0].value == "") {
//            $$("txtTaxAllowance")[0].value = 0;
//        }
//    }
//     //Ticket#73404
//    SetDefaultValuesforTaxWithHolding();

//    //if (this.value == "FLAD") {

//    //    alert(current_identifier);

//    //  //  $$("txtVoluntary_Withholding").removeAttr("disabled");

//    //} else {
//    //    alert(txtFamilyRelationValue);
//    //  //  $$("txtVoluntary_Withholding").attr("disabled", true);

//    //}

//});

//FM upgrade: 6.0.1.1 changes - replaced live by on
//$$("ddlBenefitDistributionTypeValue1").on("change", function () {
$(document).on("change", "#cphCenterMiddle_ddlBenefitDistributionTypeValue1", function (e) {

    var lddlTaxIdentifier = $$("ddlTaxIdentifierValue");
    TaxIdentifierVal = lddlTaxIdentifier[0].value;

    var lddlBenefitDistributionTypeValue1 = $$("ddlBenefitDistributionTypeValue1");
    BenDistType = lddlBenefitDistributionTypeValue1[0].value;

    // SetDefaultValuesforTaxWithHolding();
    GetTaxWithHoldingScreenConfiguratorColumns(TaxIdentifierVal, BenDistType);
    if ($$("ddlBenefitDistributionTypeValue1")[0].value != "") {
        $$("ddlTaxIdentifierValue").removeAttr("disabled");
        $$("ddlBenefitDistributionTypeValue1").attr("disabled", true);
    }
    // SetFederalTaxFieldVisibleStatus();
});

//function SetFederalTaxFieldVisibleStatus (){

//    var lddlTaxIdentifier = $$("ddlTaxIdentifierValue");
//    var current_identifier = lddlTaxIdentifier[0].value;

//    var lddlTaxOption = $$("ddlTaxOptionValue11");
//    var currrent_taxoption = lddlTaxOption[0].value;

//    var lddlBenefitDistributionType = $$("ddlBenefitDistributionTypeValue1");
//    var current_BenefitDistributionType = lddlBenefitDistributionType[0].value;

//    var txtStep2B = $$("txtStep2B");
//    var txtStep3Amount = $$("txtStep3Amount");
//    var txtStep4a = $$("txtStep4a");
//    var txtStep4b = $$("txtStep4b");
//    var txtStep4c = $$("txtStep4c");
//    var txtTaxPercentage = $$("txtTaxPercentage");
//    var txtTaxAllowance = $$("txtTaxAllowance");
//    var txtAdditionalTaxAmount1 = $$("txtAdditionalTaxAmount1");
//    var lbltxtStep2B = $$("lbltxtStep2B");
//    var lbltxtStep3Amount = $$("lbltxtStep3Amount");
//    var lbltxtStep4a = $$("lbltxtStep4a");
//    var lbltxtStep4b = $$("lbltxtStep4b");
//    var lbltxtStep4c = $$("lbltxtStep4c");
//    var lbltxtTaxPercentage = $$("lbltxtTaxPercentage");
//    var lbltxtTaxAllowance = $$("lbltxtTaxAllowance");
//    var lbltxtAdditionalTaxAmount1 = $$("lbltxtAdditionalTaxAmount1");
//    var lblmaritalstatus = $$("lblmaritalstatus");
//    var lblfilingstatus = $$("lblfilingstatus");


//    if ((current_identifier == "" || current_identifier == "STAT")) {
//        txtStep2B.hide();
//        txtStep3Amount.hide();
//        txtStep4a.hide();
//        txtStep4b.hide();
//        txtStep4c.hide();
//        txtTaxPercentage.show();
//        txtTaxAllowance.show();
//        txtAdditionalTaxAmount1.show();
//        lbltxtStep2B.hide();
//        lbltxtStep3Amount.hide();
//        lbltxtStep4a.hide();
//        lbltxtStep4b.hide();
//        lbltxtStep4c.hide();
//        lbltxtTaxPercentage.show();
//        lbltxtTaxAllowance.show();
//        lbltxtAdditionalTaxAmount1.show();
//        lblmaritalstatus.show();
//        lblfilingstatus.hide();
//    }

//    if ((current_identifier == "FDRL" && current_BenefitDistributionType == "LSDB")) {
//        txtStep2B.hide();
//        txtStep3Amount.hide();
//        txtStep4a.hide();
//        txtStep4b.hide();
//        txtStep4c.hide();
//        txtTaxPercentage.show();
//        txtTaxAllowance.show();
//        txtAdditionalTaxAmount1.hide();
//        lbltxtStep2B.hide();
//        lbltxtStep3Amount.hide();
//        lbltxtStep4a.hide();
//        lbltxtStep4b.hide();
//        lbltxtStep4c.hide();
//        lbltxtTaxPercentage.show();
//        lbltxtTaxAllowance.show();
//        lbltxtAdditionalTaxAmount1.hide();
//        lblmaritalstatus.hide();
//        lblfilingstatus.show();
//    }
//    if (current_identifier == "FDRL" && current_BenefitDistributionType == "MNBF") {
//        txtStep2B.show();
//        txtStep3Amount.show();
//        txtStep4a.show();
//        txtStep4b.show();
//        txtStep4c.show();
//        txtTaxPercentage.hide();
//        txtTaxAllowance.hide();
//        txtAdditionalTaxAmount1.hide();
//        lbltxtStep2B.show();
//        lbltxtStep3Amount.show();
//        lbltxtStep4a.show();
//        lbltxtStep4b.show();
//        lbltxtStep4c.show();
//        lbltxtTaxPercentage.hide();
//        lbltxtTaxAllowance.hide();
//        lbltxtAdditionalTaxAmount1.hide();
//        lblmaritalstatus.hide();
//        lblfilingstatus.show();
//    }
//}

//$$("ddlWithdrawalType").on("change", function () {
$(document).on("change", "#cphCenterMiddle_ddlWithdrawalType", function (e) {
    var lWithdrawalType = $$("ddlWithdrawalType").find('option:selected').val();
    var lblCovidRequestedAmount = $$("capCOVIDWithdrawalAmount");
    var lblCovidFedTaxPerc = $$("capCOVIDFedTaxPerc");
    var lblCovidStateTaxPerc = $$("capCOVIDStateTaxPerc");
    var txtCovidRequestedAmount = $$("txtIdecCOVIDWithdrawalAmount");
    var txtCovidFedTaxPerc = $$("txtCOVIDFederalPerc");
    var txtCovidStateTaxPerc = $$("txtCOVIDStatePerc");
    var chkEmergencyOneTimePayment = $$("chkEmergencyOneTimePayment");

    if (lWithdrawalType != "") {
        lblCovidRequestedAmount.show();
        lblCovidFedTaxPerc.show();
        lblCovidStateTaxPerc.show();
        txtCovidRequestedAmount.show();
        txtCovidFedTaxPerc.show();
        txtCovidStateTaxPerc.show();
    }
    else {
        lblCovidRequestedAmount.hide();
        lblCovidFedTaxPerc.hide();
        lblCovidStateTaxPerc.hide();
        txtCovidRequestedAmount[0].value = 0;
        txtCovidFedTaxPerc[0].value = 0;
        txtCovidStateTaxPerc[0].value = 0;
        txtCovidRequestedAmount.hide();
        txtCovidFedTaxPerc.hide();
        txtCovidStateTaxPerc.hide();

    }

});

//FM upgrade: 6.0.1.1 changes - replaced live by on
$$("chkEmergencyOneTimePayment").on("change", function () {
    //EmergencyOneTimePayment - 03/17/2020
    var lblCovidRequestedAmount = $$("capCOVIDWithdrawalAmount");
    var lblCovidFedTaxPerc = $$("capCOVIDFedTaxPerc");
    var lblCovidStateTaxPerc = $$("capCOVIDStateTaxPerc");
    var txtCovidRequestedAmount = $$("txtIdecCOVIDWithdrawalAmount");
    var txtCovidFedTaxPerc = $$("txtCOVIDFederalPerc");
    var txtCovidStateTaxPerc = $$("txtCOVIDStatePerc");
    var chkEmergencyOneTimePayment = $$("chkEmergencyOneTimePayment");

    if (chkEmergencyOneTimePayment[0].checked) {
        lblCovidRequestedAmount.show();
        lblCovidFedTaxPerc.show();
        lblCovidStateTaxPerc.show();
        txtCovidRequestedAmount.show();
        txtCovidFedTaxPerc.show();
        txtCovidStateTaxPerc.show();
    }
    else {
        lblCovidRequestedAmount.hide();
        lblCovidFedTaxPerc.hide();
        lblCovidStateTaxPerc.hide();
        txtCovidRequestedAmount[0].value = 0;
        txtCovidFedTaxPerc[0].value = 0;
        txtCovidStateTaxPerc[0].value = 0;
        txtCovidRequestedAmount.hide();
        txtCovidFedTaxPerc.hide();
        txtCovidStateTaxPerc.hide();

    }

});

var CalledFirstTime = true;
function SetDefaultValuesforTaxWithHolding() {
    if (CalledFirstTime === true) {
        CalledFirstTime = false;
        return;
    }

    //PROD PIR 193
    var lddltaxpercentage = $$("txtTaxPercentage");
    var current_taxpercentage = lddltaxpercentage[0].value;

    var ltxtPlanid = $$("txtIintPlanId");
    var current_ltxtPlanid = ltxtPlanid[0].value;

    var lddlTaxIdentifier = $$("ddlTaxIdentifierValue");
    var current_identifier = lddlTaxIdentifier[0].value;

    var lddlTaxOption = $$("ddlTaxOptionValue11");
    var currrent_taxoption = lddlTaxOption[0].value;



    var txtBenfitOption = $$("txtIstrBenefitOption");
    var current_txtBenfitOption = txtBenfitOption[0].value;

    var txtPayeeType = $$("txtAccountRelationValue");
    var current_txtPayeeType = txtPayeeType[0].value;


    var IsQualifiedSpouse = $$("txtIsQualifiedSpouse");
    var current_IsQualifiedSpouse = IsQualifiedSpouse[0].value;

    var lddlBenefitDistributionType = $$("ddlBenefitDistributionTypeValue1");
    var current_BenefitDistributionType = lddlBenefitDistributionType[0].value;

    //EmergencyOneTimePayment - 03/17/2020
    var lddlOneMoreInformation = $$("lblIstrMoreInformation");
    var current_MoreInformation = lddlOneMoreInformation[0].value;

    if (current_identifier != "" && (current_identifier == "FDRL" || current_identifier == "STAT")) {

        if (current_BenefitDistributionType == "LSDB") {
            $$("ddlMaritalStatusValue11")[0].value = "";
            $$("txtTaxAllowance")[0].value = 0;
        }
        else if (currrent_taxoption == "") {

            $$("txtTaxAllowance")[0].value = 3;
            $$("ddlMaritalStatusValue11")[0].value = "M";
        }
        else {
            if (($$("txtTaxAllowance")[0].value != 3 && $$("txtTaxAllowance")[0].value != "") || $$("txtTaxAllowance")[0].value == "") {

                if ($$("txtTaxAllowance")[0].value == "") {
                    $$("txtTaxAllowance")[0].value = 0;
                }

            }
            else {

                $$("txtTaxAllowance")[0].value = 3;
            }

            if ($$("ddlMaritalStatusValue11")[0].value != "M" && $$("txtTaxAllowance")[0].value != "") {

            }
            else {
                $$("ddlMaritalStatusValue11")[0].value = "M";
            }
        }
    }
    else {
        if (current_BenefitDistributionType == "LSDB") {
            $$("ddlMaritalStatusValue11")[0].value = "";
            $$("txtTaxAllowance")[0].value = 0;
        }
        else {
            $$("txtTaxAllowance")[0].value = "";
            $$("ddlMaritalStatusValue11")[0].value = "";
        }
    }


    var txtFamilyRelationValue = $$("txtFamilyRelationValue");
    var current_txtFamilyRelationValue = txtFamilyRelationValue[0].value;
    if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum" && (current_txtPayeeType == "PART" || current_txtPayeeType == "ALTP")) {
        //PROD PIR 786
        if (current_txtFamilyRelationValue == "EXSP") {
            $$("txtTaxPercentage")[0].value = 10;
        }
        else if (current_MoreInformation != "COVID 19 IAP Hardship Payment.") {
            $$("txtTaxPercentage")[0].value = 20;
        }
    }
    else if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum" && (current_txtPayeeType == "BENE" || current_txtPayeeType == "JANT") && current_IsQualifiedSpouse == "Y") {
        $$("txtTaxPercentage")[0].value = 20;
    }
    else {
        if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum") {
            $$("txtTaxPercentage")[0].value = 10;
        }
    }

    //PROD PIR 193
    if (current_taxpercentage != "" && current_ltxtPlanid == 1) {
        if (current_taxpercentage > $$("txtTaxPercentage")[0].value) {
            $$("txtTaxPercentage")[0].value = current_taxpercentage;
        }
    }


    //PROD PIR 876
    //    if (current_identifier == "STAT" && currrent_taxoption == "FLAP" && current_BenefitDistributionType == "LSDB" && (current_txtPayeeType == "PART" || current_IsQualifiedSpouse == "Y")) {
    //        $$("txtTaxPercentage")[0].value = 2;
    //    }
    //    else if (current_identifier == "STAT" && currrent_taxoption == "FLAP" && current_BenefitDistributionType == "LSDB" && current_txtPayeeType != "PART" && current_txtFamilyRelationValue != "EXSP" && current_IsQualifiedSpouse != "Y") {
    //        $$("txtTaxPercentage")[0].value = 2;
    //    }
    //PROD PIR 786

    ////PIR 945
    //else if (current_identifier == "STAT" && currrent_taxoption == "FLAP" && current_BenefitDistributionType == "LSDB" && current_txtFamilyRelationValue == "EXSP") {
    //    $$("txtTaxPercentage")[0].value = 1;
    //}

}

//$$("txtAge").live("blur", function () { SetAge(); });
function SetAge() {

    try {
        var llblDateOfBirth = $$("lblDateOfBirth");
        var current_lblDateOfBirth = llblDateOfBirth[0].innerHTML;
        var txtAge = $$("txtAge");
        var currrent_txtAge = txtAge[0].value;

        if ((current_lblDateOfBirth != "" && current_lblDateOfBirth != null) && (currrent_txtAge != "" && currrent_txtAge != null)) {
            var a = new Date(current_lblDateOfBirth);
            var txtRetirementDate = $$("txtRetirementDate");

            var years = parseInt(a.getFullYear()) + parseInt(currrent_txtAge);
            var currnet_txtRetirementDate = new Date(years, a.getMonth(), a.getDay())
            $$("txtRetirementDate")[0].value = currnet_txtRetirementDate.format("MM/dd/yyyy");
        }
        else
            $$("txtRetirementDate")[0].value = "";
    }
    catch (err) {

    }
}

// Set focus of Other document type if document type is "OTHER".
//FM upgrade: 6.0.1.1 changes - replaced live by on
$$("ddlDocumentTypeValue").on("change", function () { SetFocusForOtherDocument(); });
function SetFocusForOtherDocument() {
    if ($$("ddlDocumentTypeValue")[0].value == "0,PEND") {
        var txtOtherDocumentType = $$("txtOtherDocumentType");
        txtOtherDocumentType.focus();
    }
}

