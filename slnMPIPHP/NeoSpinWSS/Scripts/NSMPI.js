// This Flag with determine whether we are coming via PostBack or we are just Client Side
// It will help to populate the values based on the right condition

var QrdoLoadFlag = true;

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
        $("#overlay").unbind("click");
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


function ShowWebExtenderURL() {
    var mpi_person_id = $$("lblSsnWeb");
    window.open("http://webx/AppXtender/ISubmitQuery.aspx?Appname=PENSION_DOCS&DataSource=Imaging&QueryType=0&SSN=" + mpi_person_id[0].outerText, "OpenURL", "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes", null);
    return false;
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

    if (current_identifier == "PYEE" || current_identifier == "ROTP") {
        $$("txtIstrRMPIDAddPayee")[0].value = current_txtPayeeMPID;
    }
    else {
        $$("txtIstrRMPIDAddPayee")[0].value = " ";
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

$$("ddlTaxOptionValue11").live("change", function () { SetDefaultValuesforTaxWithHolding(); });
$$("ddlBenefitDistributionTypeValue1").live("change", function () { SetDefaultValuesforTaxWithHolding(); });

var CalledFirstTime = true;
function SetDefaultValuesforTaxWithHolding() {
    if (CalledFirstTime === true) {
        CalledFirstTime = false;
        return;
    }
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


    if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum" && (current_txtPayeeType == "PART" || current_txtPayeeType == "ALTP")) {
        $$("txtTaxPercentage")[0].value = 20;
    }
    else if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum" && (current_txtPayeeType == "BENE" || current_txtPayeeType == "JANT") && current_IsQualifiedSpouse == "Y") {
        $$("txtTaxPercentage")[0].value = 20;
    }
    else {
        if (current_identifier == "FDRL" && currrent_taxoption == "FLAP" && current_txtBenfitOption == "Lump Sum") {
            $$("txtTaxPercentage")[0].value = 10;
        }
    }
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