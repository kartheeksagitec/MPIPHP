using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using Sagitec.WebClient;
using Sagitec.WebControls;
using Sagitec.Common;
using System.Reflection;
using Sagitec.Interface;
//using CrystalDecisions.CrystalReports.Engine;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;
using System.Globalization;
using System.Text.RegularExpressions;
using MPIPHP.CustomDataObjects;
using System.Collections.Generic;

public partial class wfmDefault_aspx : wfmCustomPageBase
{
    // Page events are wired up automatically to methods 
    // with the following names:
    // Page_Load, Page_AbortTransaction, Page_CommitTransaction,
    // Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
    // Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
    // Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
    // Page_SaveStateComplete, Page_Unload
    public bool lblnQualifiedSpouse { get; set; }
    //private ReportDocument irptReportDocument = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        string lstrOnlineHelpFolderPath = ConfigurationManager.AppSettings["OnlineHelpFolderPath"] ?? String.Empty;
        string lstrURL = lstrOnlineHelpFolderPath + this.istrFormName + ".htm'";
        string lstrFeatures = "'left=600,top=100,width=500,height=500,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes'";
        string lstrScript = "<script language='javascript'> function ShowOnlineHelp(){" +
            "window.open(" + lstrURL + ", null , " + lstrFeatures + ");return false;}; </script>";
        //ClientScript.RegisterClientScriptBlock(this.GetType(), "Help Client", lstrScript);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Help Client", lstrScript, false);



    }


    protected override void LaunchImagePopup(busMainBase abusActivityInstance)
    {
        busActivityInstance lobjActivityInstance = (busActivityInstance)abusActivityInstance;

        string lstrFinalDisplayURL = String.Empty;

        string lstrDisplayURL = "http://" + ConfigurationManager.AppSettings["AppXtender_ServerName"] + "/ISubmitQuery.aspx?Appname=" + ConfigurationManager.AppSettings["AppXtender_AppName"] + "&DataSource=" + ConfigurationManager.AppSettings["AppXtender_DataSource"] + "&QueryType=0&SSN=" + lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.ssn;
        if (lstrDisplayURL.IsNotNullOrEmpty())
        {
            string lstrFeatures = "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes";
            string lstrScript = "fwkOpenPopupWindow('" + lstrDisplayURL + "','" + lstrFeatures + "','" + "');";
            lstrFinalDisplayURL += lstrScript;
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "LaunchImage", lstrFinalDisplayURL, true);
    }



    protected override void btnSave_Click(object sender, EventArgs e)
    {

        base.btnSave_Click(sender, e);


        if (istrFormName == "wfmBenefitCalculationRetirementMaintenance" || istrFormName == "wfmBenefitCalculationPreRetirementDeathMaintenance"
            || istrFormName == "wfmBenefitCalculationWithdrawalMaintenance")
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvBenefitCalculationDetail");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();

            }
        }

        if (istrFormName == "wfmBenefitCalculationRetirementMaintenance" || istrFormName == "wfmBenefitCalculationPreRetirementDeathMaintenance"
            || istrFormName == "wfmBenefitCalculationWithdrawalMaintenance")
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvBenefitCalculationDetailPension");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();
            }
        }
        if (istrFormName == busConstant.BenefitCalculation.DISABILITY_CALCULATION_MAINTENANCE)
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvMPIBenefitCalculationDetail");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();
            }
            lgrvBase = (sfwGridView)GetControl(itblParent, "grvLocalBenefitCalculationDetail");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();
            }
        }

        if (istrFormName == "wfmQDROCalculationMaintenance")
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvQdroCalculationDetail");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();
            }

            lgrvBase = (sfwGridView)GetControl(itblParent, "grvQdroCalculationOptions");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();
            }
        }
        if (istrFormName == busConstant.RETIREMENT_WIZARD)
        {
            busRetirementWizard lbusRetirementWizard = null;
            lbusRetirementWizard = (busRetirementWizard)Framework.SessionForWindow["CenterMiddle"];
            if (lbusRetirementWizard.iarrErrors.Count > 0)
            {

                //  DisplayError(utlMessageType.Solution, 6291, null);
                return;
            }
            else
            {
                Framework.SessionForWindow["PayeeAccountId"] = lbusRetirementWizard.PayeeAccountId;
                Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPayeeAccountACHDetailsWizardMaintenance");

            }

        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;

            if (lbusPayeeAccount.iarrErrors.Count > 0)
            {

                return;
            }
            else
            {
                Framework.SessionForWindow["PayeeAccountId"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPayeeAccountFedralWthHoldingWizardMaintenance");

            }



        }

        if (istrFormName == "wfmPayeeAccountFedralWthHoldingWizardMaintenance")
        {

            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;

            if (lbusPayeeAccount.iarrErrors.Count > 0)
            {

                return;
            }
            else
            {
                Framework.SessionForWindow["PayeeAccountId"] = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPayeeAccountStateWthHoldingWizardMaintenance");

            }
        }
        if (istrFormName == "wfmPayeeAccountStateWthHoldingWizardMaintenance")
        {

            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;

            if (lbusPayeeAccount.iarrErrors.Count > 0)
            {
                return;
            }
            else
            {
                Framework.SessionForWindow["PersonId"] = lbusPayeeAccount.icdoPayeeAccount.person_id;
                Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPersonMaintenance");

            }
        }
    }


    //protected override void btnCancel_Click(object sender, EventArgs e)
    //{
    //    if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE || istrFormName == busConstant.PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE || istrFormName == busConstant.PAYEE_ACCOUNT_STATE_WIZARD_TAXWITHHOLDING_MAINTENANCE)
    //    {
    //        busPayeeAccount lbusPayeeAccount = null;
    //        lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;

    //        if (lbusPayeeAccount != null)
    //        {
    //            isrvBusinessTier.ExecuteObjectMethod("Cancel_RetirementApplication_Wizard", lbusPayeeAccount, null, idictParams);

    //            Framework.SessionForWindow["PersonId"] = lbusPayeeAccount.icdoPayeeAccount.person_id;
    //            Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPersonMaintenance");
    //        }

    //    }

    //}





    //LA Sunset - Payment Directives
    protected override void btnCorrespondence_Click(object sender, EventArgs e)
    {
        base.btnCorrespondence_Click(sender, e);
        if (istrFormName == "wfmPayeeAccountMaintenance" && (this.ibusMain is busPayeeAccount))
        {
            (this.ibusMain as busPayeeAccount).istrTemplateName = string.Empty;

            if (((System.Web.UI.Control)sender).ID == "btnConfirmationLetter")
            {
                sfwButton lsfwButton = (sfwButton)GetControl(this, "btnConfirmationLetter");
                if (lsfwButton.IsNotNull())
                {
                    if ((this.ibusMain as busPayeeAccount).icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                    {

                        if ((this.ibusMain as busPayeeAccount).istrMoreInformation != null && (this.ibusMain as busPayeeAccount).istrMoreInformation.Contains("Hardship"))
                        {
                            // if ((this.ibusMain as busPayeeAccount).istrMoreInformation.Contains("Hardship")) // == "Hardship 2023 IAP Withdrawal")
                            (this.ibusMain as busPayeeAccount).istrTemplateName = busConstant.IAP_HARDSHIP_PAYMENT_CONFIRMATION;

                        }
                        else
                            (this.ibusMain as busPayeeAccount).istrTemplateName = busConstant.IAP_ANNUITY_QUOTE_CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE;
                    }
                    else
                    {
                        (this.ibusMain as busPayeeAccount).istrTemplateName = busConstant.CONFIRMATION_LETTER;
                    }
                }
            }
            else if (((System.Web.UI.Control)sender).ID == "btnVerificationLetter")
            {
                sfwButton lsfwButton = (sfwButton)GetControl(this, "btnVerificationLetter");
                if (lsfwButton.IsNotNull())
                {
                    (this.ibusMain as busPayeeAccount).istrTemplateName = busConstant.PENSION_INCOME_VERIFICATION;
                }
            }
        }
    }

    protected override void InitializeGridSelection()
    {
        base.InitializeGridSelection();
        if (this.istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance")
        {
            sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvBenefitCalculationDetailIAP");
            //if ((((busBenefitCalculationPostRetirementDeath)this.ibusMain)).iclbBenefitCalculationDetail.Count > 1)
            //{
            //    lsfwGridView.SelectedIndex = 1;
            //}
            //else
            //{
            lsfwGridView.SelectedIndex = -1;
            //}
        }

    }

    public override void sfwItem_DataBinding(object sender, EventArgs e)
    {
        base.sfwItem_DataBinding(sender, e);

        if (Framework.SessionForWindow["Logged_In_User_is_VIP"].IsNotNull() && Framework.SessionForWindow["Logged_In_User_is_VIP"].ToString() == "VIPAccessUser")
            return;

        //If the screen is person lookup then attach the script to launch the jQuery modal dialog if VIP record is being opened by the user.
        if (sender is sfwLinkButton)
        {
            string lstrVipFlag = string.Empty;
            string lstrRelativeVipFlag = string.Empty;

            GridViewRow ldgrItem = (GridViewRow)((Control)sender).NamingContainer;

            if (ldgrItem.DataItem.GetType().Name.Equals("busActivityInstance") && ldgrItem.DataItem.IsNotNull())
            {
                if (((busActivityInstance)ldgrItem.DataItem).icdoActivityInstance.istrRelativeVipFlag.IsNotNull())
                    lstrRelativeVipFlag = ((busActivityInstance)ldgrItem.DataItem).icdoActivityInstance.istrRelativeVipFlag;

                if (!string.IsNullOrEmpty(lstrRelativeVipFlag) && lstrRelativeVipFlag == "Y")
                {
                    ((sfwLinkButton)sender).OnClientClick = "OpenVIPDialog(); return false";
                }
            }

            if ((ldgrItem.DataItem.GetType().Name.Equals("busPerson") || ldgrItem.DataItem.GetType().Name.Equals("busBenefitApplication") || ldgrItem.DataItem.GetType().Name.Equals("busDeathNotification")
                || ldgrItem.DataItem.GetType().Name.Equals("busQdroApplication") || ldgrItem.DataItem.GetType().Name.Equals("busPayeeAccount")
                || ldgrItem.DataItem.GetType().Name.Equals("busPaymentHistoryHeader") || ldgrItem.DataItem.GetType().Name.Equals("busSSNMerge")
                || ldgrItem.DataItem.GetType().Name.Equals("busBenefitCalculationHeader") || ldgrItem.DataItem.GetType().Name.Equals("busQdroCalculationHeader"))
                || ldgrItem.DataItem.GetType().Name.Equals("busExcessRefund")
                && istrFormName.EndsWith("Lookup"))
            {
                PropertyInfo[] iclbProperries = ldgrItem.DataItem.GetType().GetProperties();

                foreach (PropertyInfo prop in iclbProperries)
                {
                    if (prop.Name.StartsWith("icdo"))
                    {
                        object lobjcdoObject = HelperFunction.GetObjectValue(ldgrItem.DataItem, prop.Name.ToString(), ReturnType.Object);

                        //object lobjVipFlag = HelperFunction.GetObjectValue(lobjcdoObject, "istrVipFlag", ReturnType.Object);
                        //lstrVipFlag = lobjVipFlag.ToString();


                        object lobjRelativeVipFlag = HelperFunction.GetObjectValue(lobjcdoObject, "istrRelativeVipFlag", ReturnType.Object);
                        if (lobjRelativeVipFlag.IsNotNull())
                            lstrRelativeVipFlag = lobjRelativeVipFlag.ToString();

                        //if (!string.IsNullOrEmpty(lstrVipFlag) && lstrVipFlag == "Y")
                        //{
                        //    string uniqueid = ((sfwLinkButton)sender).UniqueID;
                        //    ((sfwLinkButton)sender).OnClientClick = "OpenVIPDialog(); return false";
                        //}
                        //else
                        //{
                        if (!string.IsNullOrEmpty(lstrRelativeVipFlag) && lstrRelativeVipFlag == "Y")
                        {
                            ((sfwLinkButton)sender).OnClientClick = "OpenVIPDialog(); return false";
                        }
                        //}
                    }
                }

            }
        }
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        //Ticket#101090
        if (istrFormName == "wfmDisabiltyBenefitCalculationMaintenance")
        {
            sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvMPIBenefitCalculationDetail");
            if (lsfwGridView.IsNotNull() && !lsfwGridView.iblnNeoGrid)
            {
                //int selectedRow = lsfwGridView.SelectedIndex;  //Gets index of selected row
                if (lsfwGridView.Rows.Count > 0)  //MM 10/11/2022 Added fail safe check so if grid is empty it will not try to select row.
                {
                    GridViewRow selRow = lsfwGridView.SelectedRow; //Gets the selected row within the grid
                    if (selRow.IsNotNull() && selRow.Cells.IsNotNull() && selRow.Cells[9].HasControls())
                    {
                        sfwLabel calcId = (sfwLabel)selRow.Cells[9].Controls[0];  //Within the 10th column of the selected row, get the first contained control, which should be of type sfwLabel
                        if (calcId.Text.IsNumeric())
                        {
                            (ibusMain as busBenefitCalculationHeader).sel_benefit_calculation_detail_id = Convert.ToInt32(calcId.Text);
                        }
                    }
                }
            }
        }
        if (istrFormName == "wfmPersonMaintenance")
        {

            if (Framework.SessionForWindow["PersonId"] != null)
            {
                Framework.SessionForWindow["PersonId"] = null;

            }


        }


        //Associating a Script to QDRO Maintain Screen because we need to set some default values on ClientSide based on values of 
        //Casacading DropDowns 
        if (istrFormName == busConstant.QRDO_MAINTAINENCE)
        {
            string lstrSetQdroBenefitScreenInfo = "SetQdroBenefitScreenInfo();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetQdroBenefitScreenInfo", lstrSetQdroBenefitScreenInfo, true);

            string lstrSetJoinderInfo = "SetJoinderInfo();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetJoinderInfo", lstrSetJoinderInfo, true);
        }

        if (istrFormName == busConstant.RETIREMENT_WIZARD)
        {

            sfwCascadingDropDownList lsfwddlIAPJointAnnunantName = (sfwCascadingDropDownList)GetControl(this, "ddlIintPlanID");
            if (Convert.ToInt32(lsfwddlIAPJointAnnunantName.sfwSelectedValue) == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RetirementWizardHideControls", "RetirementWizardHideControls();", true);
            }

        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE)
        {

            sfwCascadingDropDownList lsfwddlIAPJointAnnunantName = (sfwCascadingDropDownList)GetControl(this, "ddlWizardBenefitDistributionTypeValue1");
            if (lsfwddlIAPJointAnnunantName.sfwSelectedValue.IsNullOrEmpty())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RetirementWizardFedTaxHolding()", "RetirementWizardFedTaxHolding();", true);
            }

        }




        if (istrFormName == busConstant.TAX_WITHHOLDING_CALCULATOR_MAINTENANCE)
        {
            sfwTextBox lsfwTextBoxFedAllowanceNumber = (sfwTextBox)GetControl(this, "txtStep2b3");
            if (lsfwTextBoxFedAllowanceNumber.Text == "0" || lsfwTextBoxFedAllowanceNumber.Text == string.Empty)
            {
                lsfwTextBoxFedAllowanceNumber.Text = Convert.ToString(0);
            }
            sfwTextBox lsfwTextStateAllowanceNumber = (sfwTextBox)GetControl(this, "txtStateAllowanceNumber");
            if (lsfwTextStateAllowanceNumber.Text == "0" || lsfwTextStateAllowanceNumber.Text == string.Empty)
            {
                lsfwTextStateAllowanceNumber.Text = Convert.ToString(0);
            }


        }
        if (istrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE)
        {
            if (this.ibusMain is busPersonBeneficiary)
            {
                if ((((busPersonBeneficiary)this.ibusMain)).iclbPersonAccountBeneficiary.Count == 0)
                {
                    sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtStartDate");
                    lsfwTextBox.Text = DateTime.Today.ToString("MM/dd/yyyy");
                }
            }
            sfwCascadingDropDownList lsfwDropDownList = (sfwCascadingDropDownList)GetControl(this, "ddlIaintPlan1");
            sfwDropDownList lsfwBenDropDownList = (sfwDropDownList)GetControl(this, "ddlBeneficiaryTypeValue");
            if (lsfwDropDownList != null)
            {
                lsfwDropDownList.Attributes.Add("onChange", "return OnSelectedIndexChange();");
            }
            if (lsfwDropDownList != null)
            {
                lsfwBenDropDownList.Attributes.Add("onChange", "return OnSelectedIndexChange();");
            }
        }
        if (istrFormName == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE)
        {
            sfwTextBox lsfwtxtTaxPercentage = (sfwTextBox)GetControl(this, "txtTaxPercentage");
            lsfwtxtTaxPercentage.Focus();
            //rid 80131
            sfwCascadingDropDownList lsfwDropDownList = (sfwCascadingDropDownList)GetControl(this, "ddlTaxIdentifierValue");
            if (((busPayeeAccountGen)ibusMain).icdoPayeeAccount.retiree_incr_flag == "Y" && ((busPayeeAccountGen)ibusMain).icdoPayeeAccount.istrBenefitDistributionType == null)
            {
                lsfwDropDownList.Enabled = false;
            }
            else
            {
                lsfwDropDownList.Enabled = true;
            }

            if (((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding.Count > 0)
            {
                (ibusMain as busPayeeAccount).icdoPayeeAccount.istrBenefitDistributionType = (((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding)[0].icdoPayeeAccountTaxWithholding.benefit_distribution_type_value;
            }

            //if (((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding.Count > 0)
            //{
            //    (ibusMain as busPayeeAccount).icdoPayeeAccount.istrBenefitDistributionType = (((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding)[0].icdoPayeeAccountTaxWithholding.benefit_distribution_type_value;

            //    if((((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding).Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL" && i.icdoPayeeAccountTaxWithholding.marital_status_value != string.Empty).Count() > 0)
            //    {
            //        (ibusMain as busPayeeAccount).icdoPayeeAccount.istrMaritalStatusValue = (((busPayeeAccountGen)ibusMain).iclbPayeeAccountTaxWithholding).Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL").Select(y => y.icdoPayeeAccountTaxWithholding.marital_status_value).FirstOrDefault();

            //    }
            //  }

        }
        if (istrFormName == busConstant.PAYMENT_REISSUE_DETAIL_MAINTENANCE)
        {
            string lstrSetPayeeMPIDValue = "SetPayeeMPIDValue();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetPayeeMPIDValue", lstrSetPayeeMPIDValue, true);

            if (this.ibusMain is busPaymentHistoryDistribution)
            {
                if ((((busPaymentHistoryDistribution)this.ibusMain)).ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.istrBenefitOptionValue != busConstant.LUMP_SUM)
                {
                    sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtIstrRecipientRollOverOrgMPID");
                    if (lsfwTextBox.IsNotNull())
                    {
                        lsfwTextBox.Visible = false;
                    }
                    sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "capIstrRecipientRollOverOrgMPID");
                    if (lsfwLabel.IsNotNull())
                    {
                        lsfwLabel.Visible = false;
                    }
                    sfwImageButton lsfwButton = (sfwImageButton)GetControl(this, "btnRetrieveRecipientRollOverOrgMPID");
                    if (lsfwButton.IsNotNull())
                    {
                        lsfwButton.Visible = false;
                    }

                }
            }


        }
        if (istrFormName == busConstant.RecalculateIAPAllocationDetailMaintenance)
        {
            busIAPAllocationDetailPersonOverview lbusIAPAllocationDetailPersonOverview = null;
            lbusIAPAllocationDetailPersonOverview = (busIAPAllocationDetailPersonOverview)Framework.SessionForWindow[UIConstants.CenterMiddle];


            //sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvIAPAllocationDetailPersonOverview");
            //if (lsfwGridView.IsNotNull())
            //{
            //    ArrayList arrOverriddenControls = GetControls(lsfwGridView, "GridViewRow");
            //    foreach (GridViewRow lgrdRow in arrOverriddenControls)
            //    {
            //        ArrayList arrOverriddenlgrdRow = GetControls(lgrdRow, "sfwLabel");

            //    }


            //}

            sfwGridView lsfwTotalGridView = (sfwGridView)GetControl(this, "grvTotalIAPAllocationDetailPersonOverview");
            if (lsfwTotalGridView.IsNotNull())
            {
                ArrayList arrOverriddenControls = GetControls(lsfwTotalGridView, "sfwTextBox");
                if (arrOverriddenControls.IsNotNull() && arrOverriddenControls.Count > 0)
                {
                    (arrOverriddenControls[0] as sfwTextBox).Visible = false;
                    (arrOverriddenControls[1] as sfwTextBox).Visible = false;
                }
            }
        }
        if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow[UIConstants.CenterMiddle];

            if (lbusPayeeAccount.iclbPayeeAccountAchDetail.Count == 0)
            {
                sfwCheckBox lcheckbox = (sfwCheckBox)GetControl(this, "chkPreNoteFlag");
                lcheckbox.Checked = true;
            }

        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow[UIConstants.CenterMiddle];

            //if (lbusPayeeAccount.iclbPayeeAccountWireDetail.Count == 0)
            //{
            //    sfwCheckBox lcheckbox = (sfwCheckBox)GetControl(this, "chkCallBackFlag");
            //    lcheckbox.Checked = true;
            //}

        }

        if (istrFormName == busConstant.REPAYMENT_SCHEDULE_MAINTENANCE)
        {
            string lstrSetDisableNextDueAmount = "SetDisableNextDueAmount();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetDisableNextDueAmount", lstrSetDisableNextDueAmount, true);

            string lstrSetRepaymentFlatPercentage = "SetRepaymentFlatPercentage();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetRepaymentFlatPercentage", lstrSetRepaymentFlatPercentage, true);


        }
        if (istrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE && Framework.SessionForWindow[UIConstants.CenterMiddle] is busWithdrawalApplication)
        {
            sfwPanel lpnlMain = (sfwPanel)GetControl(this, "pnlMain");
            if (lpnlMain != null)
            {
                busWithdrawalApplication lbusWithdrawalApplication = null;
                lbusWithdrawalApplication = (busWithdrawalApplication)Framework.SessionForWindow[UIConstants.CenterMiddle];
                if (lbusWithdrawalApplication != null && lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id > 0)
                    lpnlMain.sfwCaption = "Alternate Payee Details";
            }
        }
        if (istrFormName == "wfmPersonOverviewMaintenance")
        {
            sfwButton button = (sfwButton)GetControl(this, "btnLabel1231");
            if (button != null && ibusMain != null)
            {
                if (ibusMain is busPersonOverview)
                {
                    busPersonOverview lbusPersonOverview = ibusMain as busPersonOverview;

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Help Client", "ShowWebExtenderURL(" + lbusPersonOverview.icdoPerson.mpi_person_id.ToString() + ");", false);
                }
            }


        }
        //rid 72580
        if (istrFormName == "wfmPersonOverviewMaintenance" || istrFormName == "wfmPersonMaintenance")
        {
            sfwTextBox textbox = (sfwTextBox)GetControl(this, "txtNotes");
            textbox.Text = null;


        }

        if (istrFormName == "wfmPersonMaintenance")
        {
            sfwButton btnAddNew = (sfwButton)GetControl(this, "btnNew");
            btnAddNew.Visible = false;
            sfwButton btnBenDel = (sfwButton)GetControl(this, "btnDelete1");
            btnBenDel.Visible = false;
            sfwButton btnDependentNew = (sfwButton)GetControl(this, "btnNew2");
            btnDependentNew.Visible = false;
            sfwButton btnDelete = (sfwButton)GetControl(this, "btnDelete");
            btnDelete.Visible = false;



        }

        if (istrFormName == "wfmDeathNotificationLookup")
        {
            sfwButton btnAddNew = (sfwButton)GetControl(this, "btnNew");
            btnAddNew.Visible = false;

        }

        if (istrFormName == "wfmDeathNotificationMaintenance")
        {
            sfwButton btnNotDeceased = (sfwButton)GetControl(this, "btnNotDeceased");
            btnNotDeceased.Visible = false;

        }
        if (istrFormName == "wfmPersonDependentMaintenance")
        {
            sfwButton btnSave = (sfwButton)GetControl(this, "btnSave");
            btnSave.Visible = false;

        }



        //PIR RID 61485
        if (istrFormName == "wfmPersonAddressMaintenance")
        {
            sfwCheckBoxList lcblAddressTypeList = (sfwCheckBoxList)GetControl(this, "cblCheckBoxList");
            if (lcblAddressTypeList.IsNotNull())
            {
                lcblAddressTypeList.Items[0].Selected = true;
            }

        }

        //PIR RID 63893
        if (istrFormName == busConstant.BENEFICIARY_MAINTENANCE)
        {
            sfwCheckBoxList lcblAddressTypeList = (sfwCheckBoxList)GetControl(this, "cblIstrLongDescription");
            if (lcblAddressTypeList.IsNotNull())
            {
                lcblAddressTypeList.Items[0].Selected = true;
            }

            sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtStartDate");
            lsfwTextBox.Text = DateTime.Today.ToString("MM/dd/yyyy");

        }

        #region wfmPayeeAccountRolloverMaintenance
        if (istrFormName == "wfmPayeeAccountRolloverMaintenance" && Framework.SessionForWindow[UIConstants.CenterMiddle] is busPayeeAccount)
        {

            sfwPanel lpnlMain = (sfwPanel)GetControl(this, "pnlMain");
            if (lpnlMain != null)
            {
                busPayeeAccount lbusPayeeAccount = null;
                lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow[UIConstants.CenterMiddle];

                Hashtable lhstParam = new Hashtable();

                sfwCheckBox lcheckbox = (sfwCheckBox)GetControl(this, "chkSendToParticipant");
                sfwTextBox lsfwTextBoxPID = (sfwTextBox)GetControl(this, "txtPersonId");
                if (lcheckbox.Checked && lsfwTextBoxPID.Text != string.Empty)
                {
                    int lintPersonId = Convert.ToInt32(lsfwTextBoxPID.Text);
                    if (lintPersonId.IsNotNull() && lintPersonId > 0)
                    {
                        lhstParam.Clear();
                        lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), lintPersonId);
                        DataTable ldtbPersonAddress = isrvBusinessTier.ExecuteQuery("cdoPersonAddress.GetParticipantAddress", lhstParam, idictParams);

                        if (ldtbPersonAddress.IsNotNull() && ldtbPersonAddress.Rows.Count > 0)
                        {
                            sfwTextBox ltxtAddrLine1 = (sfwTextBox)GetControl(this, "txtAddrLine1");
                            ltxtAddrLine1.Text = ldtbPersonAddress.Rows[0]["ADDR_LINE_1"].ToString();

                            sfwTextBox ltxtAddrLine2 = (sfwTextBox)GetControl(this, "txtAddrLine2");
                            ltxtAddrLine2.Text = ldtbPersonAddress.Rows[0]["ADDR_LINE_2"].ToString();

                            sfwDropDownList lddlCountryValue = (sfwDropDownList)GetControl(this, "ddlCountryValue");
                            lddlCountryValue.SelectedValue = ldtbPersonAddress.Rows[0]["ADDR_COUNTRY_VALUE"].ToString();

                            sfwDropDownList lddlStateValue = (sfwDropDownList)GetControl(this, "ddlStateValue");
                            lddlStateValue.SelectedValue = ldtbPersonAddress.Rows[0]["ADDR_STATE_VALUE"].ToString();

                            sfwTextBox ltxtCity = (sfwTextBox)GetControl(this, "txtCity");
                            ltxtCity.Text = ldtbPersonAddress.Rows[0]["ADDR_CITY"].ToString();

                            sfwTextBox ltxtZipCode = (sfwTextBox)GetControl(this, "txtZipCode");
                            ltxtZipCode.Text = ldtbPersonAddress.Rows[0]["ADDR_ZIP_CODE"].ToString();

                            sfwTextBox ltxtZip4Code = (sfwTextBox)GetControl(this, "txtZip4Code");
                            ltxtZip4Code.Text = ldtbPersonAddress.Rows[0]["ADDR_ZIP_4_CODE"].ToString();

                            sfwTextBox ltxtContactName = (sfwTextBox)GetControl(this, "txtContactName");
                            ltxtContactName.Text = Convert.ToString(ldtbPersonAddress.Rows[0]["CO"]);
                        }
                    }
                }
                //else
                //{
                //    sfwTextBox lsfwTextBoxMPIOrgID = (sfwTextBox)GetControl(this, "txtRolloverOrgId");
                //    string lstrMPIOrgId = lsfwTextBoxMPIOrgID.Text;
                //    if (lstrMPIOrgId.IsNotNullOrEmpty())
                //    {
                //        lhstParam.Clear();
                //        lhstParam.Add(enmOrganization.mpi_org_id.ToString().ToUpper(), lstrMPIOrgId);
                //        DataTable ldtbOrgAddressResult = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsWithAddress", lhstParam, idictParams);

                //        if (ldtbOrgAddressResult.IsNotNull() && ldtbOrgAddressResult.Rows.Count > 0)
                //        {
                //            sfwTextBox ltxtAddrLine1 = (sfwTextBox)GetControl(this, "txtAddrLine1");
                //            ltxtAddrLine1.Text = ldtbOrgAddressResult.Rows[0]["ADDR_LINE_1"].ToString();

                //            sfwTextBox ltxtAddrLine2 = (sfwTextBox)GetControl(this, "txtAddrLine2");
                //            ltxtAddrLine2.Text = ldtbOrgAddressResult.Rows[0]["ADDR_LINE_2"].ToString();

                //            sfwDropDownList lddlCountryValue = (sfwDropDownList)GetControl(this, "ddlCountryValue");
                //            lddlCountryValue.SelectedValue = ldtbOrgAddressResult.Rows[0]["COUNTRY_VALUE"].ToString();

                //            sfwDropDownList lddlStateValue = (sfwDropDownList)GetControl(this, "ddlStateValue");
                //            lddlStateValue.SelectedValue = ldtbOrgAddressResult.Rows[0]["STATE_VALUE"].ToString();

                //            sfwTextBox ltxtCity = (sfwTextBox)GetControl(this, "txtCity");
                //            ltxtCity.Text = ldtbOrgAddressResult.Rows[0]["CITY"].ToString();

                //            sfwTextBox ltxtZipCode = (sfwTextBox)GetControl(this, "txtZipCode");
                //            ltxtZipCode.Text = ldtbOrgAddressResult.Rows[0]["ZIP_CODE"].ToString();

                //            sfwTextBox ltxtZip4Code = (sfwTextBox)GetControl(this, "txtZip4Code");
                //            ltxtZip4Code.Text = ldtbOrgAddressResult.Rows[0]["ZIP_4_CODE"].ToString();

                //            sfwTextBox ltxtContactName = (sfwTextBox)GetControl(this, "txtContactName");
                //            ltxtContactName.Text = Convert.ToString(ldtbOrgAddressResult.Rows[0]["CO"]);
                //        }
                //    }
                //}
            }
        }
        #endregion

        if (istrFormName == "wfmRecipientRolloverOrganisationMaintenance" && Framework.SessionForWindow[UIConstants.CenterMiddle] is busPaymentHistoryDistribution)
        {

            sfwPanel lpnlMain = (sfwPanel)GetControl(this, "pnlMain");
            if (lpnlMain != null)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = null;
                lbusPaymentHistoryDistribution = (busPaymentHistoryDistribution)Framework.SessionForWindow[UIConstants.CenterMiddle];

                Hashtable lhstParam = new Hashtable();

                sfwCheckBox lcheckbox = (sfwCheckBox)GetControl(this, "chkSendToParticipant");
                if (lcheckbox.Checked)
                {
                    sfwTextBox lsfwTextBoxPID = (sfwTextBox)GetControl(this, "txtPersonId");
                    int lintPersonId = Convert.ToInt32(lsfwTextBoxPID.Text);
                    if (lintPersonId.IsNotNull() && lintPersonId > 0)
                    {
                        lhstParam.Clear();
                        lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), lintPersonId);
                        DataTable ldtbPersonAddress = isrvBusinessTier.ExecuteQuery("cdoPersonAddress.GetParticipantAddress", lhstParam, idictParams);

                        if (ldtbPersonAddress.IsNotNull() && ldtbPersonAddress.Rows.Count > 0)
                        {
                            sfwTextBox ltxtAddrLine1 = (sfwTextBox)GetControl(this, "txtAddrLine1");
                            ltxtAddrLine1.Text = ldtbPersonAddress.Rows[0]["ADDR_LINE_1"].ToString();

                            sfwTextBox ltxtAddrLine2 = (sfwTextBox)GetControl(this, "txtAddrLine2");
                            ltxtAddrLine2.Text = ldtbPersonAddress.Rows[0]["ADDR_LINE_2"].ToString();

                            sfwDropDownList lddlCountryValue = (sfwDropDownList)GetControl(this, "ddlCountryValue");
                            lddlCountryValue.SelectedValue = ldtbPersonAddress.Rows[0]["ADDR_COUNTRY_VALUE"].ToString();

                            sfwDropDownList lddlStateValue = (sfwDropDownList)GetControl(this, "ddlStateValue");
                            lddlStateValue.SelectedValue = ldtbPersonAddress.Rows[0]["ADDR_STATE_VALUE"].ToString();

                            sfwTextBox ltxtCity = (sfwTextBox)GetControl(this, "txtCity");
                            ltxtCity.Text = ldtbPersonAddress.Rows[0]["ADDR_CITY"].ToString();

                            sfwTextBox ltxtZipCode = (sfwTextBox)GetControl(this, "txtZipCode");
                            ltxtZipCode.Text = ldtbPersonAddress.Rows[0]["ADDR_ZIP_CODE"].ToString();

                            sfwTextBox ltxtZip4Code = (sfwTextBox)GetControl(this, "txtZip4Code");
                            ltxtZip4Code.Text = ldtbPersonAddress.Rows[0]["ADDR_ZIP_4_CODE"].ToString();

                            sfwTextBox ltxtContactName = (sfwTextBox)GetControl(this, "txtContactName");
                            ltxtContactName.Text = Convert.ToString(ldtbPersonAddress.Rows[0]["CO"]);
                        }
                    }
                }
                else
                {
                    sfwTextBox lsfwTextBoxMPIOrgID = (sfwTextBox)GetControl(this, "txtRolloverOrgId");
                    string lstrMPIOrgId = lsfwTextBoxMPIOrgID.Text;
                    if (lstrMPIOrgId.IsNotNullOrEmpty())
                    {
                        lhstParam.Clear();
                        lhstParam.Add(enmOrganization.mpi_org_id.ToString().ToUpper(), lstrMPIOrgId);
                        DataTable ldtbOrgAddressResult = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsWithAddress", lhstParam, idictParams);

                        if (ldtbOrgAddressResult.IsNotNull() && ldtbOrgAddressResult.Rows.Count > 0)
                        {
                            sfwTextBox ltxtAddrLine1 = (sfwTextBox)GetControl(this, "txtAddrLine1");
                            ltxtAddrLine1.Text = ldtbOrgAddressResult.Rows[0]["ADDR_LINE_1"].ToString();

                            sfwTextBox ltxtAddrLine2 = (sfwTextBox)GetControl(this, "txtAddrLine2");
                            ltxtAddrLine2.Text = ldtbOrgAddressResult.Rows[0]["ADDR_LINE_2"].ToString();

                            sfwDropDownList lddlCountryValue = (sfwDropDownList)GetControl(this, "ddlCountryValue");
                            lddlCountryValue.SelectedValue = ldtbOrgAddressResult.Rows[0]["COUNTRY_VALUE"].ToString();

                            sfwDropDownList lddlStateValue = (sfwDropDownList)GetControl(this, "ddlStateValue");
                            lddlStateValue.SelectedValue = ldtbOrgAddressResult.Rows[0]["STATE_VALUE"].ToString();

                            sfwTextBox ltxtCity = (sfwTextBox)GetControl(this, "txtCity");
                            ltxtCity.Text = ldtbOrgAddressResult.Rows[0]["CITY"].ToString();

                            sfwTextBox ltxtZipCode = (sfwTextBox)GetControl(this, "txtZipCode");
                            ltxtZipCode.Text = ldtbOrgAddressResult.Rows[0]["ZIP_CODE"].ToString();

                            sfwTextBox ltxtZip4Code = (sfwTextBox)GetControl(this, "txtZip4Code");
                            ltxtZip4Code.Text = ldtbOrgAddressResult.Rows[0]["ZIP_4_CODE"].ToString();

                            sfwTextBox ltxtContactName = (sfwTextBox)GetControl(this, "txtContactName");
                            ltxtContactName.Text = Convert.ToString(ldtbOrgAddressResult.Rows[0]["CO"]);
                        }
                    }
                }
            }
        }
        if (istrFormName == busConstant.SSN_MERGE_DEMOGRAPHIC_MAINTENANCE && Framework.SessionForWindow[UIConstants.CenterMiddle] is busPerson)
        {
            if (((MPIPHP.BusinessObjects.busSSNMerge)(Framework.SessionForWindow[UIConstants.CenterMiddle])).iclbMergedPersonReord.IsNullOrEmpty())
            {

                sfwGridView ldgvNewGrid = (sfwGridView)GetControl(this, "grvNewPerson");



                sfwCheckBox lchkToSSN = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToSSNChecked");
                lchkToSSN.Checked = true;

                sfwCheckBox lchkToLastName = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToLastNameChecked");
                lchkToLastName.Checked = true;

                sfwCheckBox lchkToMiddleName = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToMiddleNameChecked");
                lchkToMiddleName.Checked = true;

                sfwCheckBox lchkToFirstName = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToFirstNameChecked");
                lchkToFirstName.Checked = true;

                sfwCheckBox lchkToPrefix = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToPrefixNameChecked");
                lchkToPrefix.Checked = true;

                sfwCheckBox lchkToSuffix = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToSuffixNameChecked");
                lchkToSuffix.Checked = true;

                sfwCheckBox lchkToDOB = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToDOBChecked");
                lchkToDOB.Checked = true;

                sfwCheckBox lchkToDOD = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToDODChecked");
                lchkToDOD.Checked = true;

                sfwCheckBox lchkToAddress = (sfwCheckBox)GetControl(ldgvNewGrid, "IsToAddressChecked");
                lchkToAddress.Checked = true;
            }

        }
    }


    // Initialize the report documnet. This event removes any databse logon information 
    // saved in the report. The call to Load the report in the above function fires this event.
    //private void OnReportDocInit(object sender, System.EventArgs e)
    //{
    //    ((ReportDocument)sender).SetDatabaseLogon("", "");
    //}

    protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
    {
        if (istrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE)
        {
            idictParams["Benefeciary_Maintenance_AddressFlag"] = Framework.SessionForWindow["BenefeciaryMaintenance_AddressFlag"];
        }
        if (istrFormName == busConstant.PERSON_DEPENDENT_MAINTENANCE)
        {
            idictParams["Dependent_Maintenance_AddressFlag"] = Framework.SessionForWindow["BenefeciaryMaintenance_AddressFlag"];
        }

        //if (istrFormName == busConstant.PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE)
        //{

        //    // sfwDropDownList lsfwddlddlWizardBenefitDistributionType = (sfwDropDownList)GetControl(this, "ddlWizardBenefitDistributionType");
        //    if (((sfwDropDownList)GetControl(this, "ddlWizardBenefitDistributionType")).SelectedValue == "")
        //    {
        //        ((sfwDropDownList)GetControl(this, "ddlWizardBenefitDistributionType")).SelectedValue = "MNBF";

        //    }

        //    if (((sfwDropDownList)GetControl(this, "ddlWizardTaxIdentifierValue")).SelectedValue == "")
        //    {
        //        ((sfwDropDownList)GetControl(this, "ddlWizardTaxIdentifierValue")).SelectedValue = "FDRL";

        //        //busPayeeAccount lbusPayeeAccount = null;
        //        //lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;
        //        //lbusPayeeAccount.GetMaritalStatusByTaxIdentifier("FDRL");
        //        //lbusPayeeAccount.GetTaxOptionsByIdentifier("FDRL", "MNBF", 0);

        //    }



        //}

        base.RaisePostBackEvent(sourceControl, eventArgument);
        if (istrFormName == busConstant.BENEFICIARY_MAINTENANCE)
        {
            if (sourceControl is sfwButton && ((System.Web.UI.Control)(sourceControl)).ID == "btnSave" && ibusMain is busPerson)
            {
                Framework.SessionForWindow["BenefeciaryMaintenance_AddressFlag"] = (ibusMain as busPerson).istrAddressSameAsParticipant;
            }
        }

        if ((istrFormName.ToLower().EndsWith("lookup") && (ibusMain != null) && (Framework.SessionForWindow[istrFormName + "ActivityInstance"] != null)))
        {
            ibusMain.ibusBaseActivityInstance =
                (busMainBase)Framework.SessionForWindow[istrFormName + "ActivityInstance"];
            bool lblnImageAlreadyLaunced = false;
            if (Framework.SessionForWindow["IsImageAlreadyLaunched"] != null)
            {
                lblnImageAlreadyLaunced = (bool)Framework.SessionForWindow["IsImageAlreadyLaunched"];
                if (!lblnImageAlreadyLaunced)
                {
                    LaunchImagePopup(ibusMain.ibusBaseActivityInstance);
                    Framework.SessionForWindow["IsImageAlreadyLaunched"] = true;
                }
            }
        }
    }


    protected override void AfterGetInitialData(string astrFormName)
    {
        if (astrFormName == "wfmWorkflowCenterLeftMaintenance")
        {
            Framework.SessionForWindow["SelectedActivityInstanceID"] = HelperFunction.GetObjectValue(ibusMain, "icdoActivityInstance.activity_instance_id", ReturnType.Object);

            // Important to set this to get the busActivityInstance value for setting the focus to the control, when workflow activity related lob screen 
            // is opended from the Left-Nav screen.
            Framework.SessionForWindow["CenterLeftActivityInstance"] = ibusMain;
        }

        if (astrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE || astrFormName == busConstant.PERSON_DEPENDENT_MAINTENANCE)
        {
            Framework.SessionForWindow["ParticipantId"] = HelperFunction.GetObjectValue(ibusMain, "ibusPerson.icdoPerson.person_id", ReturnType.Object);
        }
    }

    protected override void BeforeGetInitialData(string astrFormName, Hashtable ahstParams)
    {
        if (astrFormName == "wfmWorkflowCenterLeftMaintenance")
        {
            int lintInstanceID = 0;
            if (Framework.SessionForWindow["SelectedActivityInstanceID"] != null)
            {
                lintInstanceID = (int)Framework.SessionForWindow["SelectedActivityInstanceID"];
            }
            ahstParams.Clear();
            ahstParams.Add("aintActivityInstanceID", lintInstanceID);
        }
        if (astrFormName == busConstant.BENEFICIARY_MAINTENANCE)
        {
            idictParams.Add("SelectedParticipantId", Framework.SessionForWindow["ParticipantId"]);
        }


        base.BeforeGetInitialData(astrFormName, ahstParams);

        if (astrFormName == "wfmPayeeAccountFedralWthHoldingWizardMaintenance")
        {
            //busRetirementWizard lbusRetirementWizard = null;
            //lbusRetirementWizard = (busRetirementWizard)Framework.SessionForWindow["CenterMiddle"];
            if (Framework.SessionForWindow["PayeeAccountId"] != null)
            {
                ahstParams["aintPayeeAccountId"] = Convert.ToInt32(Framework.SessionForWindow["PayeeAccountId"]);

            }


        }

        if (astrFormName == busConstant.PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE)
        {

            if (Framework.SessionForWindow["PayeeAccountId"] != null)
            {
                ahstParams["aintPayeeAccountId"] = Convert.ToInt32(Framework.SessionForWindow["PayeeAccountId"]);

            }


        }

        if (astrFormName == "wfmPayeeAccountStateWthHoldingWizardMaintenance")
        {

            if (Framework.SessionForWindow["PayeeAccountId"] != null)
            {
                ahstParams["aintPayeeAccountId"] = Convert.ToInt32(Framework.SessionForWindow["PayeeAccountId"]);

            }


        }

        if (astrFormName == "wfmPersonMaintenance")
        {

            if (Framework.SessionForWindow["PersonId"] != null)
            {
                ahstParams["aintpersonid"] = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);

            }


        }

        if (astrFormName == busConstant.RETIREMENT_WIZARD)
        {
            if (Framework.SessionForWindow["MpiPersonId"] != null)
            {
                ahstParams["MpiPersonId"] = Convert.ToString(Framework.SessionForWindow["MpiPersonId"]);
            }

        }





    }

    protected override void GetSelectedData(string astrNavigationParameters, ArrayList aarrDataControls) //To pass value of Checkbox as navigation parameter in beneficiary griddetails
    {
        base.GetSelectedData(astrNavigationParameters, aarrDataControls);

        if ((istrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE || istrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE || istrFormName == busConstant.DISABILITY_APPLICATION_MAINTAINENCE) &&
            (ibtnPostBackAction.sfwMethodName == "btnGridViewAdd_Click" && ibtnPostBackAction.ID == "btnAdd"))
        {
            foreach (Control lControl in aarrDataControls)
            {
                if (lControl is sfwCheckBox && lControl.ID == busConstant.SPOUSAL_CHECKBOX_ID)
                {
                    if ((lControl as sfwCheckBox).Checked && (iarrSelectedRows[0] as Hashtable).ContainsKey(busConstant.SPOUSAL_CHECKBOX_OBJECTFIELD))
                    {
                        (iarrSelectedRows[0] as Hashtable)[busConstant.SPOUSAL_CHECKBOX_OBJECTFIELD] = busConstant.FLAG_YES;
                        break;
                    }
                    else
                    {
                        (iarrSelectedRows[0] as Hashtable)[busConstant.SPOUSAL_CHECKBOX_OBJECTFIELD] = busConstant.FLAG_NO;
                        break;
                    }
                }
            }
        }
    }

    public bool IsNegative(string astrNumber)
    {
        bool lblnValidPercentage = false;
        Regex lrexGex = new Regex("^[0-9,.]*$");
        if (!lrexGex.IsMatch(astrNumber))
        {
            lblnValidPercentage = true;
        }
        return lblnValidPercentage;
    }



    protected override void ResetChildGridControls(sfwGridView agrvBase, busBase aobjBase)
    {
        base.ResetChildGridControls(agrvBase, aobjBase);
        if (istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance" && agrvBase.ID == "grvBenefitCalculationDetailIAP")
        {
            agrvBase.SelectedIndex = -1;
        }
    }

    protected override void btnGridViewDelete_Click(object sender, EventArgs e)
    {
        if (istrFormName == "wfmPayeeAccountMaintenance" && ((sfwButton)sender).ID == "btnDeleteBusPayeeAccountRetroPayment")
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvBusPayeeAccountBenefitOverpayment");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.sfwObjectKeyName = "icdoPayeeAccountRetroPayment.payee_account_retro_payment_id";
            }
        }

        base.btnGridViewDelete_Click(sender, e);
    }




    //FM upgrade: 6.0.0.16 changes - method signature changed
    protected override void DisplayMessage(utlMessageType aenmMessageType, int aintMessageID)
    {

        if (istrFormName == busConstant.SSN_MERGE_LOOKUP && aintMessageID == 5)
        {
            //Fw upgrade: PIR ID : 28524:  Information message is not displaying on 'SSN Merge Lookup' screen.
            DisplayMessage(utlMessageType.Solution, 6215);
        }
        else
        {
            base.DisplayMessage(aenmMessageType, aintMessageID);
        }
    }


    //FM upgrade: 6.0.0.16 changes - method signature changed
    protected override void DisplayMessage(utlMessageType aenmMessageType, int aintMessageID, object[] aarrParam)
    {
        bool lblnIsMerge = false;

        if (aintMessageID == 1 && (istrFormName == busConstant.PERSON_LOOKUP || istrFormName == busConstant.BENEFIT_LOOKUP || istrFormName == busConstant.BENEFICIARY_LOOKUP || istrFormName == busConstant.DEATH_NOTIFICATION_LOOKUP
            || istrFormName == busConstant.BenefitCalculation.BENEFIT_CALCULATION_LOOKUP || istrFormName == busConstant.SSN_MERGE_LOOKUP))
        {
            string lstrOldSSN = string.Empty, lstrNewPersonMPID = string.Empty;
            sfwTextBox ltxtSsn = (sfwTextBox)GetControl(this, "txtSsn");
            sfwTextBox ltxtSSN = (sfwTextBox)GetControl(this, "txtSSN");


            string lstrOLdMPID = string.Empty;
            sfwTextBox ltxtOldMPID = (sfwTextBox)GetControl(this, "txtMpiPersonId");
            //Fw upgrade: PIR ID : 28529: MPID text field is not displayed when user clicks "MPID retrieve button" for Death Notification Lookup
            //txtMpiPersonId changed to txtMpiPersonId2 on person lookup for PIR 28529. Hence updated the same here
            if (istrFormName == busConstant.PERSON_LOOKUP)
                ltxtOldMPID = (sfwTextBox)GetControl(this, "txtMpiPersonId2");

            if (ltxtSsn != null)
            {
                lstrOldSSN = ltxtSsn.Text;

            }
            else if (ltxtSSN != null)
            {
                lstrOldSSN = ltxtSSN.Text;
            }

            if (ltxtOldMPID != null)
                lstrOLdMPID = ltxtOldMPID.Text;

            Hashtable lhstParam = new Hashtable();

            DataTable ldtbPersonInfo = new DataTable();
            if (lstrOldSSN.IsNotNullOrEmpty())
            {
                lstrOldSSN = lstrOldSSN.Replace("-", "");
                lhstParam.Add(enmSsnMergeHistory.old_ssn.ToString().ToUpper(), lstrOldSSN);
                ldtbPersonInfo = isrvBusinessTier.ExecuteQuery("cdoSsnMergeHistory.GetOldPersonBySSN", lhstParam, idictParams);
            }
            else if (lstrOLdMPID.IsNotNullOrEmpty())
            {

                lhstParam.Add(enmSsnMergeHistory.old_mpi_person_id.ToString(), lstrOLdMPID);
                ldtbPersonInfo = isrvBusinessTier.ExecuteQuery("cdoSsnMergeHistory.GetOldPersonByMPID", lhstParam, idictParams);
            }

            if (ldtbPersonInfo != null && ldtbPersonInfo.Rows.Count > 0)
            {
                if ((ldtbPersonInfo.Rows[0][enmSsnMergeHistory.new_mpi_person_id.ToString()]).ToString().IsNotNullOrEmpty())
                    lstrNewPersonMPID = Convert.ToString(ldtbPersonInfo.Rows[0][enmSsnMergeHistory.new_mpi_person_id.ToString()]);

                if (lstrNewPersonMPID.IsNotNullOrEmpty())
                {
                    aarrParam.Clear();
                    lblnIsMerge = true;

                    DisplayMessage("This SSN is merged to MPID" + ":" + lstrNewPersonMPID);


                }

            }

            //if (lstrOldSSN.IsNotNullOrEmpty())
            //{

            //    lstrOldSSN = lstrOldSSN.Replace("-", "");

            //    lhstParam.Add(enmSsnMergeHistory.old_ssn.ToString().ToUpper(), lstrOldSSN);
            //    DataTable ldtbGetNewPersonInfo = lsrvBusinessTier.ExecuteQuery("cdoSsnMergeHistory.GetOldPersonBySSN", lhstParam, idictParams);

            //    if (ldtbGetNewPersonInfo != null && ldtbGetNewPersonInfo.Rows.Count > 0)
            //    {
            //        if ((ldtbGetNewPersonInfo.Rows[0][enmSsnMergeHistory.new_mpi_person_id.ToString()]).ToString().IsNotNullOrEmpty())
            //            lstrNewPersonMPID = Convert.ToString(ldtbGetNewPersonInfo.Rows[0][enmSsnMergeHistory.new_mpi_person_id.ToString()]);

            //        if (lstrNewPersonMPID.IsNotNullOrEmpty())
            //        {
            //            aarrParam.Clear();
            //            lblnIsMerge = true;

            //            DisplayMessage("This SSN is merged to MPID" + ":" + lstrNewPersonMPID);


            //        }
            //    }

            //}
            //else if (lstrOLdMPID.IsNotNullOrEmpty())
            //{
            //    lhstParam.Add(enmSsnMergeHistory.old_mpi_person_id.ToString().ToUpper(), lstrOLdMPID);

            //}

        }
        if (!lblnIsMerge)
        {
            base.DisplayMessage(aenmMessageType, aintMessageID, aarrParam);
        }



    }


    protected override void btnGridViewAdd_Click(object sender, EventArgs e)
    {
        //Ticket - 64147
        if (istrFormName == busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE)
        {

            if (GetControl(itblParent, "grvPayeeAccountWireDetail") is sfwGridView)
            {
                //if (GetControl(this, "chkCallBackFlag").IsNotNull() && GetControl(this, "chkCallBackFlag") is sfwCheckBox)
                //{
                //    ((sfwCheckBox)GetControl(this, "chkCallBackFlag")).Checked = true;
                //}

                if (GetControl(this, "txtWireStartDate").IsNotNull() && GetControl(this, "txtWireStartDate") is sfwTextBox)
                {
                    if (((sfwTextBox)GetControl(this, "txtWireStartDate")).Text.IsNullOrEmpty())
                        ((sfwTextBox)GetControl(this, "txtWireStartDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
                }



                //if (((sfwLabel)GetControl(this, "lblCallBackCompletionDate")).Text.IsNullOrEmpty())
                //    ((sfwLabel)GetControl(this, "lblCallBackCompletionDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                //if (GetControl(this, "txtIntAbaSwiftBankCode").IsNotNull() && GetControl(this, "txtIntAbaSwiftBankCode") is sfwTextBox)
                //{
                //    (this.ibusMain as busPayeeAccountWireDetail).icdoPayeeAccountWireDetail.istrInterAbaSwiftBankCode = ((sfwTextBox)GetControl(this, "txtIntAbaSwiftBankCode")).Text.ToString();
                //}

                if (((sfwCheckBox)GetControl(this, "chkCallBackFlag")).Checked == true)
                {
                    ((sfwLabel)GetControl(this, "lblCallBackCompletionDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                }




            }
        }
        else if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE)
        {
            if (GetControl(itblParent, "grvPayeeAccountAchDetail") is sfwGridView)
            {
                if (GetControl(this, "chkPreNoteFlag").IsNotNull() && GetControl(this, "chkPreNoteFlag") is sfwCheckBox)
                {
                    ((sfwCheckBox)GetControl(this, "chkPreNoteFlag")).Checked = true;
                }

                if (GetControl(this, "txtAchStartDate").IsNotNull() && GetControl(this, "txtAchStartDate") is sfwTextBox)
                {
                    if (((sfwTextBox)GetControl(this, "txtAchStartDate")).Text.IsNullOrEmpty())
                        ((sfwTextBox)GetControl(this, "txtAchStartDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
                }
            }

        }
        else if (istrFormName == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE)
        {
            Hashtable lhstParams = null;
            sfwButton lbtnAddChild = (sfwButton)GetControl(this, "btnAdd");
            if (lbtnAddChild.sfwNavigationParameter != null)
            {
                iarrSelectedRows = new ArrayList();
                GetSelectedData(lbtnAddChild.sfwNavigationParameter, iarrDataControls);

                if (iarrSelectedRows.Count > 0)
                    lhstParams = (Hashtable)iarrSelectedRows[0];


                if (GetControl(itblParent, "grvPayeeAccountTaxWithholding") is sfwGridView)
                {
                    if (GetControl(this, "ddlTaxIdentifierValue").IsNotNull() && GetControl(this, "ddlTaxIdentifierValue") is sfwCascadingDropDownList)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"])))
                        {
                            ((sfwCascadingDropDownList)GetControl(this, "ddlTaxIdentifierValue")).SelectedValue =
                                Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]);
                        }
                    }
                    string istrbenefit_distribution_type_value = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]);
                    string istrtaxIdentifier = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]);

                    if (istrtaxIdentifier == "FDRL" && istrbenefit_distribution_type_value == "MNBF")
                    {
                        ((sfwTextBox)GetControl(this, "txtAdditionalTaxAmount1")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtTaxPercentage")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtTaxAllowance")).Text = "0";

                    }
                    //UserStory#18231 
                    if (istrtaxIdentifier != "FDRL")
                    {
                        ((sfwTextBox)GetControl(this, "txtStep2B")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtStep3Amount")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtStep4a")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtStep4b")).Text = "0";
                        ((sfwTextBox)GetControl(this, "txtStep4c")).Text = "0";
                    }

                    if (istrtaxIdentifier == busConstant.VA_STATE_TAX)
                    {
                        ((sfwTextBox)GetControl(this, "txtTaxAllowance")).Text = "0";

                        if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]) == "STST" || Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]) == "NSTX")
                        {
                            ((sfwTextBox)GetControl(this, "txtVoluntary_Withholding")).Text = "0";
                            ((sfwTextBox)GetControl(this, "txtAdditionalTaxAmount1")).Text = "0";

                        }

                    }


                    if (GetControl(this, "ddlBenefitDistributionTypeValue1").IsNotNull() && GetControl(this, "ddlBenefitDistributionTypeValue1") is sfwCascadingDropDownList)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"])))
                        {
                            ((sfwCascadingDropDownList)GetControl(this, "ddlBenefitDistributionTypeValue1")).SelectedValue =
                                Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]);
                        }
                    }

                    if (GetControl(this, "ddlTaxOptionValue11").IsNotNull() && GetControl(this, "ddlTaxOptionValue11") is sfwCascadingDropDownList)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"])))
                        {
                            ((sfwCascadingDropDownList)GetControl(this, "ddlTaxOptionValue11")).SelectedValue =
                                Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]);
                        }
                    }
                }
                (this.ibusMain as busPayeeAccount).icdoPayeeAccount.istrTaxIdentifier = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]);
                (this.ibusMain as busPayeeAccount).icdoPayeeAccount.istrBenefitDistributionType = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]);
                (this.ibusMain as busPayeeAccount).icdoPayeeAccount.istrTaxOption = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]);
                (this.ibusMain as busPayeeAccount).icdoPayeeAccount.istrSavingMode = "ADD";
            }
        }

        else if (istrFormName == busConstant.PAYEE_ACCOUNT_MAINTENANCE)
        {
            if (GetControl(itblParent, "grvIAPPaybackDetails") is sfwGridView)
            {
                if (GetControl(this, "txtPaybackCheckPostedDate").IsNotNull() && GetControl(this, "txtPaybackCheckPostedDate") is sfwTextBox)
                {
                    ((sfwTextBox)GetControl(this, "txtPaybackCheckPostedDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");
                }
            }
        }
        else if (istrFormName == busConstant.ORGANIZATION_BANK_MAINTENANCE)
        {
            if (GetControl(itblParent, "grvOrgBank") is sfwGridView)
            {
                if (GetControl(this, "ddlStatusValue").IsNotNull() && GetControl(this, "ddlStatusValue") is sfwDropDownList)
                {
                    (this.ibusMain as busOrganization).icdoOrganization.istrStatusValue = ((sfwDropDownList)GetControl(this, "ddlStatusValue")).SelectedValue;
                }
            }
        }

        base.btnGridViewAdd_Click(sender, e);
    }




    protected override void btnGridViewUpdate_Click(object sender, EventArgs e)
    {
        bool blnCheck = false;
        if (istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance")
        {
            busBenefitCalculationPostRetirementDeath lbusPersonBeneficiary = (busBenefitCalculationPostRetirementDeath)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPersonBeneficiary != null)
            {
                sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvBenefitCalculationOptions");
                if (lgrvBase.IsNotNull())
                {
                    lgrvBase.SelectedIndex = 0;

                    //BindDataToForm();
                }
                //if (lbusPersonBeneficiary.iclbBenefitCalculationDetail.Count == 0)
                //{
                //    DisplayError(5178, null);
                //    return;
                //}
                //Hashtable lhstParams = null;
                //sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                //if (lbtnUpdateChild.sfwNavigationParameter != null)
                //{
                //    iarrSelectedRows = new ArrayList();
                //    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);
                //}
            }
        }

        //RequestID: 68932
        if (istrFormName == busConstant.PERSON_OVERVIEW_MAINTENANCE)
        {
            busPersonOverview lbusPersonOverview = (busPersonOverview)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPersonOverview != null)
            {
                if (lbusPersonOverview.iclbPensionVerificationHistory.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;
                }

                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        DateTime ldtReceivedDate = DateTime.MinValue;
                        DateTime ldtConfirmationDate = DateTime.MinValue;
                        DateTime ldtResumptionDate = DateTime.MinValue;

                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPensionVerificationHistory.received_date"])))
                        {
                            ldtReceivedDate = Convert.ToDateTime(lhstParams["icdoPensionVerificationHistory.received_date"]);
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"])))
                        {
                            ldtConfirmationDate = Convert.ToDateTime(lhstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"]);
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPensionVerificationHistory.resumption_letter_sent"])))
                        {
                            ldtResumptionDate = Convert.ToDateTime(lhstParams["icdoPensionVerificationHistory.resumption_letter_sent"]);
                        }
                        //rid 83234
                        if (ldtReceivedDate.ToShortDateString() != DateTime.Now.ToShortDateString())
                        {
                            DisplayError("Only today's date is allowed.", null);
                            return;
                        }

                        if (ldtReceivedDate != DateTime.MinValue && ldtConfirmationDate != DateTime.MinValue && ldtReceivedDate > ldtConfirmationDate)
                        {
                            DisplayError(utlMessageType.Solution, 6291, null);
                            return;
                        }
                        if (ldtConfirmationDate != DateTime.MinValue && ldtResumptionDate != DateTime.MinValue && ldtConfirmationDate > ldtResumptionDate)
                        {
                            DisplayError(utlMessageType.Solution, 6292, null);
                            return;
                        }
                        if (ldtReceivedDate != DateTime.MinValue && ldtResumptionDate != DateTime.MinValue && ldtReceivedDate > ldtResumptionDate)
                        {
                            DisplayError(utlMessageType.Solution, 6293, null);
                            return;
                        }
                    }

                }
            }
        }

        if (istrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE)
        {
            busPersonBeneficiary lbusPersonBeneficiary = (busPersonBeneficiary)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPersonBeneficiary != null)
            {
                if (lbusPersonBeneficiary.iclbPersonAccountBeneficiary.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;
                }
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        int lintPlanId = 0;
                        string lstrBenefitType = string.Empty;
                        decimal ldecPercent = 0;
                        DateTime ldtStrtTime = DateTime.MinValue;
                        DateTime ldtEndTime = DateTime.MinValue;
                        string lstrIsPrimary = string.Empty;

                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.iaintPlan"])))
                        {
                            lintPlanId = Convert.ToInt32(lhstParams["icdoPersonAccountBeneficiary.iaintPlan"]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.dist_percent"])))
                        {
                            ldecPercent = Convert.ToDecimal(lhstParams["icdoPersonAccountBeneficiary.dist_percent"]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.start_date"])))
                        {
                            ldtStrtTime = Convert.ToDateTime(lhstParams["icdoPersonAccountBeneficiary.start_date"]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.end_date"])))
                        {
                            ldtEndTime = Convert.ToDateTime(lhstParams["icdoPersonAccountBeneficiary.end_date"]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"])))
                        {
                            lstrBenefitType = Convert.ToString(lhstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"]);
                        }
                        if (lintPlanId == 0)
                        {
                            DisplayError(utlMessageType.Solution, 1126, null);
                        }
                        if (lintPlanId != 9) // Only Plan is a mandatory field for plan 'LIFE' ie Plan id =9 
                        {
                            if (string.IsNullOrEmpty(lstrBenefitType))
                            {
                                DisplayError(utlMessageType.Solution, 1127, null);
                            }
                            if (ldecPercent > 100)
                            {
                                DisplayError(utlMessageType.Solution, 1121, null);
                            }
                            if (ldecPercent == 0)
                            {
                                DisplayError(utlMessageType.Solution, 1128, null);
                            }
                            else if (IsNonNegative(ldecPercent.ToString()))
                            {
                                DisplayError(utlMessageType.Solution, 5059, null);
                            }
                            if (ldtStrtTime == DateTime.MinValue)
                            {
                                DisplayError(utlMessageType.Solution, 1123, null);
                            }
                            if (ldtEndTime != DateTime.MinValue && ldtEndTime < ldtStrtTime)
                            {
                                DisplayError(utlMessageType.Solution, 1122, null);
                            }
                        }
                    }
                }
            }
        }
        else if (istrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE || istrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE
            || istrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE || istrFormName == busConstant.DISABILITY_APPLICATION_MAINTAINENCE)
        {
            busBenefitApplication lbusBenefitApplication = (busBenefitApplication)Framework.SessionForWindow["CenterMiddle"];
            Hashtable lhstParams = null;
            sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
            if (lbtnUpdateChild.sfwNavigationParameter != null)
            {
                iarrSelectedRows = new ArrayList();
                GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                if (iarrSelectedRows.Count > 0)
                    lhstParams = (Hashtable)iarrSelectedRows[0];
                if (lhstParams.Count > 0)
                {
                    if (lbusBenefitApplication != null)
                    {
                        foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusBenefitApplication.iclbBenefitApplicationDetail)
                        {
                            if (Convert.ToString(lhstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]) != string.Empty && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id == Convert.ToInt32(lhstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]))
                            {

                                lbusBenefitApplicationDetail.istrSubPlanDescription = Convert.ToString(lhstParams["istrSubPlan"]);
                                lbusBenefitApplicationDetail.istrSubPlan = Convert.ToString(lhstParams["istrSubPlan"]);

                            }

                        }

                        if (istrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE)
                        {
                            bool lblnCheckIfVestedForL161 = busConstant.BOOL_FALSE;
                            bool lblnCheckIfVestedForL700 = busConstant.BOOL_FALSE;
                            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                            Hashtable lhstParam = new Hashtable();

                            lhstParam.Add("abusBenefitApplication", lbusBenefitApplication);
                            lhstParam.Add("astrPlanCode", busConstant.Local_161);
                            lblnCheckIfVestedForL161 = Convert.ToBoolean(isrvBusinessTier.ExecuteMethod("CheckAlreadyVested", lhstParam, true, ldictParams));
                            if (Convert.ToInt32(lhstParams["iintPlan_ID"]) == busConstant.LOCAL_161_PLAN_ID && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                                && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON && (lblnCheckIfVestedForL161 == false || lbusBenefitApplication.QualifiedSpouseExistsForPlan == false))
                            {
                                DisplayError(utlMessageType.Solution, 5429, null);
                                return;
                            }


                            lhstParam.Clear();
                            lhstParam.Add("abusBenefitApplication", lbusBenefitApplication);
                            lhstParam.Add("astrPlanCode", busConstant.LOCAL_700);
                            lblnCheckIfVestedForL700 = Convert.ToBoolean(isrvBusinessTier.ExecuteMethod("CheckAlreadyVested", lhstParam, true, ldictParams));
                            if (Convert.ToInt32(lhstParams["iintPlan_ID"]) == busConstant.LOCAL_700_PLAN_ID && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                                && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON && (lblnCheckIfVestedForL700 == false || lbusBenefitApplication.QualifiedSpouseExistsForPlan == false))
                            {
                                DisplayError(utlMessageType.Solution, 5429, null);
                                return;
                            }

                            if ((lbusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_NO || lbusBenefitApplication.QualifiedSpouseExistsForPlan == false) && Convert.ToInt32(lhstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID
                                && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                                && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrSubPlan"]) == "" && Convert.ToString(lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON)
                            {
                                DisplayError(utlMessageType.Solution, 5429, null);
                            }
                        }
                        else if (istrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE)
                        {
                            if (Convert.ToInt32(lhstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID && Convert.ToString(lhstParams["istrSubPlan"]).IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 5424, null);
                            }
                        }
                    }
                }
            }
        }
        else if (istrFormName == busConstant.QRDO_MAINTAINENCE)
        {
            decimal ldecPercentage = 0;
            int lintPlanID = 0;
            string lstrtDroModel = string.Empty;
            busQdroApplication lbusQdroApplication = (busQdroApplication)Framework.SessionForWindow["CenterMiddle"];
            if (lbusQdroApplication != null)
            {
                if (lbusQdroApplication.iclbDroBenefitDetails.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;
                }
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];

                    foreach (busDroBenefitDetails lbusDroBenefitDetails in lbusQdroApplication.iclbDroBenefitDetails)
                    {
                        if (Convert.ToString(lhstParams["icdoDroBenefitDetails.dro_benefit_id"]) != string.Empty && lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id == Convert.ToInt32(lhstParams["icdoDroBenefitDetails.dro_benefit_id"]))
                        {
                            lbusDroBenefitDetails.istrSubPlan = Convert.ToString(lhstParams["istrSubPlan"]);
                            lbusDroBenefitDetails.istrSubPlanDescription = Convert.ToString(lhstParams["istrSubPlan"]);
                        }

                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.plan_id"])))
                    {
                        lintPlanID = Convert.ToInt32(lhstParams["icdoDroBenefitDetails.plan_id"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.dro_model_value"])))
                    {
                        lstrtDroModel = Convert.ToString(lhstParams["icdoDroBenefitDetails.dro_model_value"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.benefit_perc"])))
                    {
                        ldecPercentage = Convert.ToDecimal(lhstParams["icdoDroBenefitDetails.benefit_perc"]);
                    }
                    decimal ldecAmount = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.benefit_amt"])))
                    {
                        ldecAmount = Convert.ToDecimal(lhstParams["icdoDroBenefitDetails.benefit_amt"]);
                    }

                    if (ldecAmount != 0 || ldecPercentage != 0)
                    {
                        if (IsNegative(ldecAmount.ToString()) || IsNegative(ldecPercentage.ToString()))
                        {
                            DisplayError(utlMessageType.Solution, 2020, null);
                        }
                        if (ldecPercentage > 100)
                        {
                            DisplayError(utlMessageType.Solution, 2020, null);
                        }
                    }

                    if (ldecPercentage > 100)
                    {
                        DisplayError(utlMessageType.Solution, 1121, null);
                    }

                    //PIR-1015
                    if (Convert.ToInt32(lhstParams["icdoDroBenefitDetails.plan_id"]) == 1 &&
                        ((string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.net_investment_from_date"]))
                         && !string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.net_investment_to_date"]))
                        ) ||
                        (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.net_investment_from_date"]))
                         && string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoDroBenefitDetails.net_investment_to_date"]))
                        )
                       ))
                    {
                        DisplayError(utlMessageType.Solution, 6275, null);
                    }

                    sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvDroBenefitDetails");
                    int lintOffset = lsfwGridView.PageIndex * lsfwGridView.PageSize;

                    busDroBenefitDetails lobjBase = (busDroBenefitDetails)lbusQdroApplication.iclbDroBenefitDetails.Index(lintOffset + lsfwGridView.SelectedIndex);
                    foreach (busDroBenefitDetails objbusDroBenefitDetails in lbusQdroApplication.iclbDroBenefitDetails)
                    {
                        if (objbusDroBenefitDetails != lobjBase)
                        {
                            if (objbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == lintPlanID && objbusDroBenefitDetails.istrSubPlan == Convert.ToString(lhstParams["istrSubPlan"]))
                            {
                                DisplayError(utlMessageType.Solution, 2009, null);
                            }
                        }
                    }
                    foreach (busDroBenefitDetails lbusDroBenefitDetails in lbusQdroApplication.iclbOtherQLFDDroBenefitDetails)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == lintPlanID && lbusDroBenefitDetails.istrSubPlan == Convert.ToString(lhstParams["istrSubPlan"]))
                        {
                            DisplayError(utlMessageType.Solution, 2009, null);
                        }
                        if (lstrtDroModel == "SPDQ")
                        {
                            DisplayError(utlMessageType.Solution, 1165, null);
                        }
                    }
                }
            }
        }
        else if (istrFormName == busConstant.PERSON_MAINTENANCE)
        {
            busPerson lbusPerson = (busPerson)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPerson != null)
            {
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        if (Convert.ToString(lhstParams["icdoPersonBridgeHours.bridge_type_value"]) == "")
                        {
                            DisplayError(utlMessageType.Solution, 5135, null);
                        }

                        //if (Convert.ToString(lhstParams["icdoPersonBridgeHours.hours_reported"]) == "")
                        //{
                        //    DisplayError(5136, null);

                        //}
                        bool lblnValidDates = true;
                        if (string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonBridgeHours.bridge_start_date"])))
                        {
                            lblnValidDates = false;
                            DisplayError(utlMessageType.Solution, 5137, null);

                        }
                        if (string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoPersonBridgeHours.bridge_end_date"])))
                        {
                            lblnValidDates = false;
                            DisplayError(utlMessageType.Solution, 5138, null);
                        }

                        if (lblnValidDates)
                        {
                            if (Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"]) > Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"]))
                            {
                                DisplayError(utlMessageType.Solution, 5139, null);
                            }

                            //Ticket#85664
                            if (Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"]) > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"]).Year))
                            {
                                DisplayError("Bridge End Date cannot be Greater than Last Date of Computation Year", null);

                            }



                            foreach (busPersonBridgeHours lbusPersonBridgeHours in lbusPerson.iclbPersonBridgeHours)
                            {
                                if (lhstParams["icdoPersonBridgeHours.person_bridge_id"] != "" && lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id != Convert.ToInt32(lhstParams["icdoPersonBridgeHours.person_bridge_id"]))
                                {
                                    //UAT PIR 130
                                    if (busGlobalFunctions.CheckDateOverlapping(Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"]),
                                                                            lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                                        busGlobalFunctions.CheckDateOverlapping(Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"]),
                                                                            lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                                        busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date,
                                                                                Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"]), Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"])) ||
                                        busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date,
                                                                                Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"]), Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"])))
                                    //if (((lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date <= Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"])) &&
                                    //    (Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_end_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                                    //    ((lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date <= Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"])) &&
                                    //    (Convert.ToDateTime(lhstParams["icdoPersonBridgeHours.bridge_start_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date))))
                                    {
                                        DisplayError(utlMessageType.Solution, 5141, null);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (istrFormName == busConstant.DRO_CALCULATION_MAINTENANCE)
        {
            string lstrBenefitCalculationBasedOn = string.Empty;
            int lintBalanceAsofYear = 0;
            decimal ldectotalService = busConstant.ZERO_DECIMAL, ldecOverridenCommunityPropertyPeriod = busConstant.ZERO_DECIMAL,
                     ldecOverridenTotalService = busConstant.ZERO_DECIMAL, ldecFlatPercent = busConstant.ZERO_DECIMAL, ldecFlatAmount = busConstant.ZERO_DECIMAL,
                     ldecOverridenAccruedBenefitAmt = busConstant.ZERO_DECIMAL, ldecAccruedBenefitAmt = busConstant.ZERO_DECIMAL,
                     ldecCommunityPropertyPeriod = busConstant.ZERO_DECIMAL, ldecQdroPercent = busConstant.ZERO_DECIMAL;
            DateTime ldtNetInvestmentFromDate = new DateTime();
            DateTime ldtNetInvestmentToDate = new DateTime();
            DateTime ldtCommunityPropertyEndDate = new DateTime();

            busQdroCalculationHeader lbusQdroCalculationHeader = (busQdroCalculationHeader)Framework.SessionForWindow["CenterMiddle"];
            if (lbusQdroCalculationHeader != null)
            {
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];

                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.benefit_calculation_based_on_value"])))
                    {
                        lstrBenefitCalculationBasedOn = lhstParams["icdoQdroCalculationDetail.benefit_calculation_based_on_value"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.total_service"])))
                    {
                        ldectotalService = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.total_service"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.overriden_total_value"])))
                    {
                        ldecOverridenTotalService = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.overriden_total_value"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.community_property_service"])))
                    {
                        ldecCommunityPropertyPeriod = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.community_property_service"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.overriden_community_property_period"])))
                    {
                        ldecOverridenCommunityPropertyPeriod = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.overriden_community_property_period"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.net_investment_from_date"])))
                    {
                        ldtNetInvestmentFromDate = Convert.ToDateTime(lhstParams["icdoQdroCalculationDetail.net_investment_from_date"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.net_investment_to_date"])))
                    {
                        ldtNetInvestmentToDate = Convert.ToDateTime(lhstParams["icdoQdroCalculationDetail.net_investment_to_date"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.balance_as_of_plan_year"])))
                    {
                        lintBalanceAsofYear = Convert.ToInt32(lhstParams["icdoQdroCalculationDetail.balance_as_of_plan_year"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.community_property_end_date"])))
                    {
                        ldtCommunityPropertyEndDate = Convert.ToDateTime(lhstParams["icdoQdroCalculationDetail.community_property_end_date"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.flat_percent"])))
                    {
                        ldecFlatPercent = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.flat_percent"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.flat_amount"])))
                    {
                        string lstrFlatAmount = Convert.ToString(lhstParams["icdoQdroCalculationDetail.flat_amount"]).Substring(1);
                        ldecFlatAmount = Convert.ToDecimal(lstrFlatAmount);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.qdro_percent"])))
                    {
                        ldecQdroPercent = Convert.ToDecimal(lhstParams["icdoQdroCalculationDetail.qdro_percent"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.accrued_benefit_amt"])))
                    {
                        string lstrAccruedBenefitAmt = Convert.ToString(lhstParams["icdoQdroCalculationDetail.accrued_benefit_amt"]).Substring(1);
                        ldecOverridenAccruedBenefitAmt = Convert.ToDecimal(lstrAccruedBenefitAmt);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(lhstParams["icdoQdroCalculationDetail.early_reduced_benefit_amount"])))
                    {
                        string lstrAccruedBenefitAmt = Convert.ToString(lhstParams["icdoQdroCalculationDetail.early_reduced_benefit_amount"]).Substring(1);
                        ldecAccruedBenefitAmt = Convert.ToDecimal(lstrAccruedBenefitAmt);
                    }

                    if (ldecFlatPercent != 0 && ldecFlatPercent > 100)
                    {
                        DisplayError(utlMessageType.Solution, 1121, null);
                        return;
                    }
                    if (ldecQdroPercent != 0 && ldecQdroPercent > 100)
                    {
                        DisplayError(utlMessageType.Solution, 5475, null);
                        return;
                    }

                    if ((ldecFlatPercent != 0 && ldecFlatPercent < 0) || (ldecQdroPercent != 0 && ldecQdroPercent < 0))
                    {
                        DisplayError(utlMessageType.Solution, 5059, null);
                        return;
                    }

                    if (ldecFlatAmount == 0 && ldecFlatPercent == 0 && ldecQdroPercent == 0)
                    {
                        DisplayError(utlMessageType.Solution, 5490, null);
                        return;
                    }

                    if ((ldecOverridenAccruedBenefitAmt != 0 || ldecAccruedBenefitAmt != 0))
                    {
                        busCalculation lbusCalculation = new busCalculation();
                        decimal ldecAlternatePayeeFraction = 0, ldecTotalHoursWorked = 0, ldecHoursWorkedBetweenDateOfMArrAndSep = 0, ldecAltPayeeAmtBeforeCon = 0;

                        if (ldecOverridenCommunityPropertyPeriod != 0)
                            ldecHoursWorkedBetweenDateOfMArrAndSep = ldecOverridenCommunityPropertyPeriod;
                        else
                            ldecHoursWorkedBetweenDateOfMArrAndSep = ldecCommunityPropertyPeriod;

                        if (ldecOverridenTotalService != 0)
                            ldecTotalHoursWorked = ldecOverridenTotalService;
                        else
                            ldecTotalHoursWorked = ldectotalService;

                        if (ldecTotalHoursWorked != 0)
                        {
                            ldecAlternatePayeeFraction = Math.Round(((ldecHoursWorkedBetweenDateOfMArrAndSep / ldecTotalHoursWorked) * ldecQdroPercent) / 100, 3);

                            if (ldecOverridenAccruedBenefitAmt != 0)
                            {
                                ldecAltPayeeAmtBeforeCon = lbusCalculation.CalculateBenefitAmtBeforeConversion(ldecOverridenAccruedBenefitAmt, ldecAlternatePayeeFraction,
                                    ldecFlatAmount, ldecFlatPercent);

                                if (ldecAltPayeeAmtBeforeCon > ldecOverridenAccruedBenefitAmt)
                                {
                                    DisplayError(utlMessageType.Solution, 5474, null);
                                    return;
                                }
                            }
                            else
                            {
                                ldecAltPayeeAmtBeforeCon = lbusCalculation.CalculateBenefitAmtBeforeConversion(ldecAccruedBenefitAmt, ldecAlternatePayeeFraction,
                                  ldecFlatAmount, ldecFlatPercent);

                                if (ldecAltPayeeAmtBeforeCon > ldecAccruedBenefitAmt)
                                {
                                    DisplayError(utlMessageType.Solution, 5474, null);
                                    return;
                                }
                            }
                        }

                    }

                    if ((lstrBenefitCalculationBasedOn == busConstant.BenefitCalculation.CALCULATION_BASED_ON_DAYS ||
                        lstrBenefitCalculationBasedOn == busConstant.BenefitCalculation.CALCULATION_BASED_ON_MONTHS) &&
                        (ldecOverridenCommunityPropertyPeriod == 0 || ldecOverridenTotalService == 0))
                    {
                        DisplayError(utlMessageType.Solution, 5416, null);
                        return;
                    }
                    if (ldecOverridenTotalService < ldecOverridenCommunityPropertyPeriod)
                    {
                        DisplayError(utlMessageType.Solution, 5417, null);
                        return;
                    }
                    if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate < ldtNetInvestmentFromDate))
                    {
                        DisplayError(utlMessageType.Solution, 5442, null);
                        return;
                    }
                    if (lintBalanceAsofYear != 0 && ldtNetInvestmentFromDate != DateTime.MinValue && (lintBalanceAsofYear > ldtNetInvestmentFromDate.Year))
                    {
                        DisplayError(utlMessageType.Solution, 5443, null);
                        return;
                    }
                    if (ldtNetInvestmentFromDate != DateTime.MinValue && (ldtNetInvestmentFromDate.Day != 1 || ldtNetInvestmentFromDate.Month != 1))
                    {
                        DisplayError(utlMessageType.Solution, 5444, null);
                        return;
                    }
                    if (ldtCommunityPropertyEndDate != DateTime.MinValue && ldtCommunityPropertyEndDate > DateTime.Now)
                    {
                        DisplayError(utlMessageType.Solution, 5447, null);
                        return;
                    }
                    if ((ldecOverridenAccruedBenefitAmt != 0 && ldecOverridenAccruedBenefitAmt < ldecFlatAmount))
                    {
                        DisplayError(utlMessageType.Solution, 5449, null);
                        return;
                    }
                    else if (ldecOverridenAccruedBenefitAmt == 0 && ldecAccruedBenefitAmt < ldecFlatAmount)
                    {
                        DisplayError(utlMessageType.Solution, 5449, null);
                        return;
                    }

                    if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 3 && ldtNetInvestmentToDate.Day != 31))
                    {
                        DisplayError(utlMessageType.Solution, 5445, null);
                        return;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 6 && ldtNetInvestmentToDate.Day != 30))
                    {
                        DisplayError(utlMessageType.Solution, 5445, null);
                        return;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 9 && ldtNetInvestmentToDate.Day != 30))
                    {
                        DisplayError(utlMessageType.Solution, 5445, null);
                        return;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 12 && ldtNetInvestmentToDate.Day != 31))
                    {
                        DisplayError(utlMessageType.Solution, 5445, null);
                        return;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 1 || ldtNetInvestmentToDate.Month == 2 ||
                        ldtNetInvestmentToDate.Month == 4 || ldtNetInvestmentToDate.Month == 5 || ldtNetInvestmentToDate.Month == 7 || ldtNetInvestmentToDate.Month == 8 ||
                        ldtNetInvestmentToDate.Month == 10 || ldtNetInvestmentToDate.Month == 11))
                    {
                        DisplayError(utlMessageType.Solution, 5445, null);
                        return;
                    }
                }
            }
        }

        else if (istrFormName == busConstant.PAYEE_ACCOUNT_ROLLOVER_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];

                    if (lhstParams.Count > 0)
                    {
                        decimal adecPercentage;
                        string astrRolloverOption = Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]);
                        string astrStatus = Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.status_value"]);
                        string astrAccountNumber = Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.account_number"]);
                        string astrAddress = Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.addr_line_1"]);
                        string astrRolloverType = Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_type_value"]);
                        int iintRolloveOrgId = 0;
                        int iintPayeeAccRolloverdetailid = 0;
                        decimal ldecAmountOfTaxable = 0.0M;

                        if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id"]) == "")
                            iintPayeeAccRolloverdetailid = 0;
                        else
                            iintPayeeAccRolloverdetailid = Convert.ToInt32(lhstParams["icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id"]);


                        if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]) == "")
                            iintRolloveOrgId = 0;
                        else
                            iintRolloveOrgId = Convert.ToInt32(lhstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]);

                        if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.amount"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.amount"]) == "")
                            ldecAmountOfTaxable = 0.0M;
                        else
                            ldecAmountOfTaxable = Convert.ToDecimal(lhstParams["icdoPayeeAccountRolloverDetail.amount"]);

                        decimal ldecPercentage = 0.0M;
                        if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]) == "")
                            ldecPercentage = 0.0M;
                        else
                            ldecPercentage = Convert.ToDecimal(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]);
                        //
                        if (iintRolloveOrgId == 0)
                        {
                            DisplayError(utlMessageType.Solution, 6010, null);
                            return;

                        }
                        //
                        if (iintRolloveOrgId != 0 && astrStatus.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6011, null);
                            return;

                        }

                        if (astrAddress.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 1114, null);
                            return;
                        }


                        if (iintRolloveOrgId != 0)
                        {
                            //if (lbusPayeeAccount.iclbOrganization == null)
                            //{

                            //    lbusPayeeAccount.LoadOrganization();
                            //}
                            //int iintActiveRolloverCount = (from obj in lbusPayeeAccount.iclbOrganization.AsEnumerable()
                            //                               where obj.icdoOrganization.status_value == "A" &&
                            //                               obj.icdoOrganization.org_type_value == "RLIT" && obj.icdoOrganization.org_id == iintRolloveOrgId
                            //                               select obj).Count();
                            //if (iintActiveRolloverCount <= 0)
                            //{
                            //    DisplayError(6058, null);
                            //    return;

                            //}
                        }
                        if (iintRolloveOrgId != 0)
                        {
                            Hashtable lhstParam = new Hashtable();

                            busPayeeAccountRolloverDetail ibusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };

                            Hashtable lhstTempParam = new Hashtable();
                            ArrayList iarrResult1 = new ArrayList();

                            lhstParam.Add("aintPayeeAccountRolloverDetailId", iintPayeeAccRolloverdetailid);
                            //PIR 988
                            iarrResult1.Add(ibusPayeeAccountRolloverDetail);
                            if (iintPayeeAccRolloverdetailid != 0)
                            {
                                //FM upgrade: 6.0.0.29 changes
                                //ibusPayeeAccountRolloverDetail = (busPayeeAccountRolloverDetail)isrvBusinessTier.ExecuteMethod("busPayeeAccountRolloverDetail", "LoadPayeeAccountRolloverItemDetail", iarrResult1, lhstParam, idictParams);
                                ibusPayeeAccountRolloverDetail = (busPayeeAccountRolloverDetail)isrvBusinessTier.ExecuteObjectMethod("LoadPayeeAccountRolloverItemDetail", ibusPayeeAccountRolloverDetail, lhstParam, idictParams);
                            }

                            iarrResult1.Clear();
                            lhstParam.Clear();

                            ibusPayeeAccountRolloverDetail.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                            lhstParam.Add("aintOrgId", iintRolloveOrgId);
                            iarrResult1.Add(ibusPayeeAccountRolloverDetail.ibusOrganization);
                            Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                            //FM upgrade: 6.0.0.29 changes
                            //ibusPayeeAccountRolloverDetail.ibusOrganization = (busOrganization)isrvBusinessTier.ExecuteMethod("busOrganization", "LoadOrganization", iarrResult1, lhstParam, idictParams);
                            ibusPayeeAccountRolloverDetail.ibusOrganization = (busOrganization)isrvBusinessTier.ExecuteObjectMethod("LoadOrganization", ibusPayeeAccountRolloverDetail.ibusOrganization, lhstParam, idictParams);
                            ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank = new Collection<busOrgBank>();
                            if (ibusPayeeAccountRolloverDetail.ibusOrganization != null)
                            {
                                if (!(ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.status_value == "A" &&
                                                      ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.org_type_value == "RLIT"))
                                {
                                    DisplayError(utlMessageType.Solution, 6058, null);
                                    return;
                                }
                                if (ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.payment_type_value == busConstant.PAYMENT_METHOD_ACH)
                                {
                                    //FM upgrade: 6.0.0.29 changes
                                    //ibusPayeeAccountRolloverDetail.ibusOrganization = (busOrganization)isrvBusinessTier.ExecuteMethod("busOrganization", "LoadOrgBanks4Organization", iarrResult1, lhstParam, idictParams);
                                    ibusPayeeAccountRolloverDetail.ibusOrganization = (busOrganization)isrvBusinessTier.ExecuteObjectMethod("LoadOrgBanks4Organization", ibusPayeeAccountRolloverDetail.ibusOrganization, lhstParam, idictParams);
                                    if (ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank.IsNullOrEmpty() || ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank.Where(obj => obj.icdoOrgBank.status_value == busConstant.STATUS_ACTIVE).Count() > 0)
                                    {
                                        DisplayError(utlMessageType.Solution, 6103, null);
                                        return;
                                    }
                                }
                            }

                            //PIR 988
                            if (ibusPayeeAccountRolloverDetail != null
                                && ibusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusProcessed
                                && ibusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.status_value != astrStatus)
                            {
                                DisplayError(utlMessageType.Solution, 6276, null);
                                return;
                            }

                            //PIR 988
                            if (astrStatus == busConstant.PayeeAccountRolloverDetailStatusProcessed)
                            {
                                DisplayError(utlMessageType.Solution, 6277, null);
                                return;
                            }

                        }
                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0)
                        {
                            int iintActiveRolloverCount = (from obj in lbusPayeeAccount.iclbPayeeAccountRolloverDetail
                                                           where obj.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive
                                                           && obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid
                                                           select obj).Count();
                            if (iintActiveRolloverCount > 3)
                            {
                                DisplayError(utlMessageType.Solution, 6012, null);
                                return;

                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0)
                        {
                            int iintActiveRolloverCount = (from obj in lbusPayeeAccount.iclbPayeeAccountRolloverDetail
                                                           where obj.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross
                                                           && obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid
                                                           select obj).Count();
                            if (iintActiveRolloverCount > 0)
                            {
                                DisplayError(utlMessageType.Solution, 6022, null);
                                return;
                            }
                        }

                        if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6072, null);
                            return;

                        }

                        if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNotNullOrEmpty() && astrRolloverOption.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6071, null);
                            return;

                        }

                        if (astrAccountNumber.IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.contact_name"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6075, null);
                            return;
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull()
                            && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty())
                        {
                            adecPercentage = lbusPayeeAccount.iclbPayeeAccountRolloverDetail
                                                                                          .Where(obj => obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid)
                                                                                            .Sum(item => item.icdoPayeeAccountRolloverDetail.percent_of_taxable)
                                                            + Convert.ToDecimal(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]);
                            if (adecPercentage > 100)
                            {
                                DisplayError(utlMessageType.Solution, 6013, null);
                                return;

                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull())
                        {
                            if (ldecAmountOfTaxable > lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.gross_amount)
                            {
                                DisplayError(utlMessageType.Solution, 6015, null);
                                return;
                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                            && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.amount"]).IsNotNullOrEmpty())
                        {
                            if ((Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAllOfGross ||
                                Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAllOfTaxable)
                                && Convert.ToDecimal(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]) != 0)
                            {
                                DisplayError(utlMessageType.Solution, 6016, null);
                                return;

                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                            && (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNullOrEmpty() || ldecPercentage == 0))
                        {
                            if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                            {
                                DisplayError(utlMessageType.Solution, 6029, null);
                                return;

                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                            && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty())
                        {
                            if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionPercentageOfTaxable
                                && Convert.ToDecimal(lhstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]) != 0)
                            {
                                DisplayError(utlMessageType.Solution, 6017, null);
                                return;
                            }
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionDollorOfGross
                            && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                        {
                            if (ldecAmountOfTaxable > 0 || ldecAmountOfTaxable != 0)
                            {
                                DisplayError(utlMessageType.Solution, 6028, null);
                                return;
                            }
                        }

                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty())
                        {
                            if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionDollorOfGross ||
                                Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                            {
                                if (ldecAmountOfTaxable == 0)
                                {
                                    DisplayError(utlMessageType.Solution, 6018, null);
                                    return;
                                }
                            }
                        }
                        if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty())
                        {
                            if (Convert.ToString(lhstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                            {

                                if (lbusPayeeAccount.IsNotNull() && ldecAmountOfTaxable > lbusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.starting_taxable_amount)
                                {
                                    DisplayError(utlMessageType.Solution, 6164, null);
                                    return;
                                }

                            }
                        }



                    }
                }
            }
        }
        else if (istrFormName == busConstant.ORGANIZATION_BANK_MAINTENANCE)
        {
            busOrganization lbusOrganization = (busOrganization)Framework.SessionForWindow["CenterMiddle"];
            if (lbusOrganization != null)
            {
                if (lbusOrganization.iclbOrgBank.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;

                }
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);
                    int iintOrgBankID = 0;
                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        if (Convert.ToString(lhstParams["icdoOrgBank.org_bank_id"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoOrgBank.org_bank_id"]) == "")
                            iintOrgBankID = 0;
                        else
                            iintOrgBankID = Convert.ToInt32(lhstParams["icdoOrgBank.org_bank_id"]);
                        if (lbusOrganization.iclbOrgBank.IsNotNull() && Convert.ToString(lhstParams["icdoOrgBank.status_value"]) == busConstant.OrgBankStatusActive)
                        {
                            int CountOrgBankWithActiveStatus = (from obj in lbusOrganization.iclbOrgBank where obj.icdoOrgBank.status_value == busConstant.OrgBankStatusActive && obj.icdoOrgBank.org_bank_id != iintOrgBankID select obj).Count();
                            if (CountOrgBankWithActiveStatus > 0)
                            {
                                DisplayError(utlMessageType.Solution, 6105, null);
                                return;
                            }

                        }
                    }

                }
            }

        }
        else if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {
                if (lbusPayeeAccount.iclbPayeeAccountAchDetail.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;
                }
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        int iintBankOrgID = 0;
                        string astrAccountType = Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.bank_account_type_value"]);


                        if (lbusPayeeAccount.iclbPayeeAccountAchDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountAchDetail.Count > 0
                            && (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty())
                             && (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.payee_account_ach_detail_id"]).IsNotNullOrEmpty()))
                        {
                            var CurrentAchDetailR = lbusPayeeAccount.iclbPayeeAccountAchDetail.Where(item => item.icdoPayeeAccountAchDetail.payee_account_ach_detail_id == Convert.ToInt32(lhstParams["icdoPayeeAccountAchDetail.payee_account_ach_detail_id"])).FirstOrDefault();

                            if (CurrentAchDetailR != null)
                            {
                                //PIR 917
                                if (CurrentAchDetailR.icdoPayeeAccountAchDetail.ach_end_date != DateTime.MinValue)
                                {
                                    DisplayError(utlMessageType.Solution, 6281, null);
                                    return;
                                }


                                if (DateTime.Today > Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]))
                                {

                                    if (DateTime.Compare(CurrentAchDetailR.icdoPayeeAccountAchDetail.ach_start_date.Date, Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"])) != 0)
                                    {
                                        //Message is addede temp
                                        DisplayError(utlMessageType.Solution, 6004, null);
                                        return;
                                    }

                                }
                                else
                                {
                                    var otherAchDetailR = lbusPayeeAccount.iclbPayeeAccountAchDetail.Where(item => item.icdoPayeeAccountAchDetail.payee_account_ach_detail_id != Convert.ToInt32(lhstParams["icdoPayeeAccountAchDetail.payee_account_ach_detail_id"]));
                                    if (otherAchDetailR != null && otherAchDetailR.Count() > 0)
                                    {
                                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]) < otherAchDetailR.Select(item => item.icdoPayeeAccountAchDetail.ach_start_date).Max())
                                        {
                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }
                                    }
                                }

                            }
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 5113, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.bank_org_id"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.bank_org_id"]) == "")
                            iintBankOrgID = 0;
                        else
                            iintBankOrgID = Convert.ToInt32(lhstParams["icdoPayeeAccountAchDetail.bank_org_id"]);

                        if (astrAccountType.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6020, null);
                            return;
                        }



                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_end_date"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty()
                            && Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_end_date"]) <= Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]))
                        {
                            DisplayError(utlMessageType.Solution, 5111, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_end_date"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty()
                            && Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_start_date"]) > Convert.ToDateTime(lhstParams["icdoPayeeAccountAchDetail.ach_end_date"]))
                        {
                            DisplayError(utlMessageType.Solution, 5139, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.account_number"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6026, null);
                            return;
                        }
                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).Length != 9)
                        {
                            DisplayError(utlMessageType.Solution, 6059, null);
                            return;
                        }
                        if (Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).IsNotNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).Length == 9)
                        {
                            Hashtable lhstParamForQuery = new Hashtable();
                            lhstParamForQuery.Add("ROUTING_NUMBER", Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]));
                            DataTable ldtbRoutingNumber = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsByRoutingNumber", lhstParamForQuery, idictParams);
                            if (ldtbRoutingNumber != null && ldtbRoutingNumber.Rows.Count > 0)
                            { }
                            else
                            {
                                DisplayError(utlMessageType.Solution, 6178, null);
                                return;
                            }
                        }

                    }

                }
            }
        }
        else if (istrFormName == busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {
                if (lbusPayeeAccount.iclbPayeeAccountWireDetail.Count == 0)
                {
                    DisplayError(utlMessageType.Solution, 5178, null);
                    return;
                }
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");

                if (GetControl(itblParent, "grvPayeeAccountWireDetail") is sfwGridView)
                {
                    if (((sfwLabel)GetControl(this, "lblCallBackCompletionDate")).Text == "")
                    {
                        if (((sfwCheckBox)GetControl(this, "chkCallBackFlag")).Checked == true)
                        {
                            ((sfwLabel)GetControl(this, "lblCallBackCompletionDate")).Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                            //sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvPayeeAccountWireDetail");

                            //lgrvBase.Rows[0].Cells[7].Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                        }

                    }

                }



                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];
                    if (lhstParams.Count > 0)
                    {
                        int iintBankOrgID = 0;
                        string astrAccountType = Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.bank_account_type_value"]);


                        if (lbusPayeeAccount.iclbPayeeAccountWireDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountWireDetail.Count > 0
                            && (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty())
                             && (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.payee_account_wire_detail_id"]).IsNotNullOrEmpty()))
                        {
                            var CurrentWireDetailR = lbusPayeeAccount.iclbPayeeAccountWireDetail.Where(item => item.icdoPayeeAccountWireDetail.payee_account_wire_detail_id == Convert.ToInt32(lhstParams["icdoPayeeAccountWireDetail.payee_account_Wire_detail_id"])).FirstOrDefault();

                            if (CurrentWireDetailR != null)
                            {
                                //PIR 917
                                if (CurrentWireDetailR.icdoPayeeAccountWireDetail.wire_end_date != DateTime.MinValue)
                                {
                                    DisplayError(utlMessageType.Solution, 6281, null);
                                    return;
                                }


                                if (DateTime.Today > Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]))
                                {

                                    if (DateTime.Compare(CurrentWireDetailR.icdoPayeeAccountWireDetail.wire_start_date.Date, Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.Wire_start_date"])) != 0)
                                    {
                                        //Message is addede temp
                                        DisplayError(utlMessageType.Solution, 6004, null);
                                        return;
                                    }

                                }
                                else
                                {
                                    var otherAchDetailR = lbusPayeeAccount.iclbPayeeAccountWireDetail.Where(item => item.icdoPayeeAccountWireDetail.payee_account_wire_detail_id != Convert.ToInt32(lhstParams["icdoPayeeAccountWireDetail.payee_account_wire_detail_id"]));
                                    if (otherAchDetailR != null && otherAchDetailR.Count() > 0)
                                    {
                                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]) < otherAchDetailR.Select(item => item.icdoPayeeAccountWireDetail.wire_start_date).Max())
                                        {
                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }
                                    }
                                }

                            }
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 5113, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.bank_org_id"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.bank_org_id"]) == "")
                            iintBankOrgID = 0;
                        else
                            iintBankOrgID = Convert.ToInt32(lhstParams["icdoPayeeAccountWireDetail.bank_org_id"]);

                        //if (astrAccountType.IsNullOrEmpty())
                        //{
                        //    DisplayError(utlMessageType.Solution, 6020, null);
                        //    return;
                        //}



                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_end_date"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty()
                            && Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_end_date"]) <= Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]))
                        {
                            DisplayError(utlMessageType.Solution, 5111, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.Wire_end_date"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty()
                            && Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_start_date"]) > Convert.ToDateTime(lhstParams["icdoPayeeAccountWireDetail.wire_end_date"]))
                        {
                            DisplayError(utlMessageType.Solution, 5139, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.beneficiary_account_number"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6026, null);
                            return;
                        }

                        //if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.istrRoutingNumber"]).IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).Length != 9)
                        if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).IsNullOrEmpty())
                        {
                            //DisplayError("Please enter valid ABA Swift Bank Code", null);
                            DisplayError(utlMessageType.Solution, 6311, null);
                            return;
                        }
                        //if (Convert.ToString(lhstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).IsNotNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrAbaSwiftBankCode"]).Length < 21)
                        //{
                        //    Hashtable lhstParamForQuery = new Hashtable();
                        //    lhstParamForQuery.Add("ABA_SWIFT_BANK_CODE", Convert.ToString(lhstParams["icdoPayeeAccountAchDetail.istrAbaSwiftBankCode"]));
                        //    DataTable ldtbRoutingNumber = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgsDetailsByAbaSwiftBankCode", lhstParamForQuery, idictParams);
                        //    if (ldtbRoutingNumber != null && ldtbRoutingNumber.Rows.Count > 0)
                        //    { }
                        //    else
                        //    {
                        //        DisplayError("Selected Aba Swift Bank code does not exists", null);
                        //        return;
                        //    }
                        //}

                    }

                }
            }
        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {



                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)


                        lhstParams = (Hashtable)iarrSelectedRows[0];


                    if (lhstParams.Count > 0)
                    {
                        string astrMaritalStatus = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.marital_status_value"]);
                        string astrTaxOption = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]);
                        string astrTaxIdentifier = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]);
                        string astrBenefitDistributionType = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]);
                        decimal ldectaxPercentage = 0.0m;
                        int ainttaxwithholdingid = 0;
                        decimal ldecAdditionalTaxAmount = 0.0m;

                        if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty())
                        {
                            lbusPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue = lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL").Select(Y => Y.icdoPayeeAccountTaxWithholding.marital_status_value).FirstOrDefault();
                            lbusPayeeAccount.icdoPayeeAccount.istrtax_option_value = lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL" && i.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == "MNBF").Select(y => y.icdoPayeeAccountTaxWithholding.tax_option_value).FirstOrDefault();
                            lbusPayeeAccount.icdoPayeeAccount.istrSavingMode = "UPDATE";

                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]).IsNullOrEmpty()
                            || Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]) == "")
                            ldectaxPercentage = Decimal.Zero;
                        else
                            ldectaxPercentage = Convert.ToDecimal(lhstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]);

                        if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]).IsNullOrEmpty())
                            ainttaxwithholdingid = 0;
                        else
                            ainttaxwithholdingid = Convert.ToInt32(lhstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]);
                        if (lhstParams.Count > 0)
                        {
                            if (astrTaxIdentifier.IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 6042, null);
                                return;
                            }

                            if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                            {
                                if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER
                                    || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)
                                {
                                    //PIR 786-TusharT (Revised Rule of PIR-176)
                                    if (lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                                    {
                                        if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 10 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                                        {
                                            DisplayError(utlMessageType.Solution, 6044, null);
                                            return;
                                        }
                                        else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 10 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                                        {
                                            DisplayError(utlMessageType.Solution, 6044, null);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // PORD PIR 193
                                        if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                                        {
                                            DisplayError(utlMessageType.Solution, 6043, null);
                                            return;
                                        }
                                        else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && lbusPayeeAccount.isCOVID19PayFlag != "Y") //EmergencyOneTimePayment - 03/17/2020
                                        {
                                            DisplayError(utlMessageType.Solution, 6043, null);
                                            return;
                                        }
                                    }
                                }

                                else if ((lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                                     || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT)
                                     && lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                                {
                                    //PROD PIR 193
                                    if (lbusPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                                    {
                                        DisplayError(utlMessageType.Solution, 6043, null);
                                        return;
                                    }
                                    else if (lbusPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 20 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                                    {
                                        DisplayError(utlMessageType.Solution, 6043, null);
                                        return;
                                    }

                                }

                                else
                                {
                                    if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                                    {

                                        if (lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                                            && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                                        {

                                        }
                                        else if (ldectaxPercentage != 10)
                                        {
                                            DisplayError(utlMessageType.Solution, 6044, null);
                                            return;
                                        }
                                    }
                                }
                            }

                            // PROD PIR 176
                            if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                            {
                                //PIR 876
                                //if (ldectaxPercentage != 2 && (lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE
                                //   || lbusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.PERSON_TYPE_PARTICIPANT))
                                //{
                                //    DisplayError(6045, null);
                                //    return;
                                //}
                                //else if (ldectaxPercentage != 2 && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE
                                //    && lbusPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE
                                //    && lbusPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.PERSON_TYPE_PARTICIPANT) // PROD PIR 176
                                //{
                                //    DisplayError(6045, null);
                                //    return;
                                //}
                                //PIR 786-TusharT (Revised Rule of PIR-176)

                                //PIR 945
                                //if (ldectaxPercentage != 1 && lbusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                                //{
                                //    DisplayError(6046, null);
                                //    return;
                                //}
                                //PIR 876
                                if (ldectaxPercentage <= 0)
                                {
                                    DisplayError(utlMessageType.Solution, 6029, null);
                                    return;
                                }

                            }

                            if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0 && lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                            {
                                //PIR 876
                                if (astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage <= 0)
                                {
                                    DisplayError(utlMessageType.Solution, 6029, null);
                                    return;
                                    //if (lbusPayeeAccount.idtNextBenefitPaymentDate == null)
                                    //    lbusPayeeAccount.LoadNextBenefitPaymentDate();

                                    //int iintCheckFlatPercentage = 0;

                                    //iintCheckFlatPercentage = (from item in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                    //                           where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                    //                           item.icdoPayeeAccountTaxWithholding.start_date, item.icdoPayeeAccountTaxWithholding.end_date)
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_PERCENT
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_percentage >= 20
                                    //                           select item).Count();
                                    //// PROD PIR 193
                                    //if (iintCheckFlatPercentage > 0 && ldectaxPercentage != 2 && lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                                    //{
                                    //    DisplayError(6045, null);
                                    //    return;
                                    //}
                                    //iintCheckFlatPercentage = (from item in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                    //                           where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                    //                           item.icdoPayeeAccountTaxWithholding.start_date, item.icdoPayeeAccountTaxWithholding.end_date)
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FLAT_PERCENT
                                    //                           && item.icdoPayeeAccountTaxWithholding.tax_percentage < 20
                                    //                           select item).Count();

                                    //if (iintCheckFlatPercentage > 0 && ldectaxPercentage != 1)
                                    //{
                                    //    DisplayError(6046, null);
                                    //    return;
                                    //}
                                }
                            }

                            if (astrTaxIdentifier.IsNotNullOrEmpty() && astrBenefitDistributionType.IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 6047, null);
                                return;
                            }

                            //PIR 803
                            if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_LumpSum
                               && lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                            {
                                DisplayError(utlMessageType.Solution, 6048, null);
                                return;
                            }

                            if (lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption != busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_Monthly_Benefit)
                            {
                                DisplayError(utlMessageType.Solution, 6063, null);
                                return;
                            }

                            if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxOption.IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 6049, null);
                                return;
                            }
                            //if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxIdentifier == busConstant.CA_STATE_TAX )
                            //{
                            //    DisplayError(6309, null);
                            //    return;
                            //}
                            //if (!lbusPayeeAccount.iclbPayeeAccountTaxWithholding.IsNullOrEmpty())
                            //{
                            //    int CountRecordwithMaritalStatus = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                            //                                      where obj.icdoPayeeAccountTaxWithholding.end_date == null || (obj.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                            //                                      && obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier && obj.icdoPayeeAccountTaxWithholding.marital_status_value == astrMaritalStatus
                            //                                      && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                            //                                      select obj).Count();
                            //    if (CountRecordwithMaritalStatus == 0)
                            //    {
                            //        DisplayError(6309, null);
                            //        return;
                            //    }
                            //}

                            if (!lbusPayeeAccount.iclbPayeeAccountTaxWithholding.IsNullOrEmpty())
                            {
                                int CountRecordwithEndDateNull = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                                                  where obj.icdoPayeeAccountTaxWithholding.end_date == null || (obj.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                                                                  && obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                                                  && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                                                                  select obj).Count();
                                if (CountRecordwithEndDateNull > 0)
                                {
                                    DisplayError(utlMessageType.Solution, 6050, null);
                                    return;
                                }
                                if ((from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                     where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                     && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                                     select obj).Count() > 0)
                                {
                                    var MaxEndDate = (from obj in lbusPayeeAccount.iclbPayeeAccountTaxWithholding
                                                      where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                                      && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                                                      select obj.icdoPayeeAccountTaxWithholding.end_date).Max();
                                    //Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty() && 
                                    if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                                        && MaxEndDate > Convert.ToDateTime(lhstParams["icdoPayeeAccountTaxWithholding.start_date"]))
                                    {
                                        DisplayError(utlMessageType.Solution, 6051, null);
                                        return;
                                    }
                                }
                            }


                            if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0
                                 && (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]).IsNotNullOrEmpty()))
                            {
                                var oldicdoPayeeAccountTaxWithholding = (from item in lbusPayeeAccount.iclbPayeeAccountTaxWithholding.AsEnumerable()
                                                                         where item.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id == ainttaxwithholdingid
                                                                         select new
                                                                         {
                                                                             tax_identifier_value = item.icdoPayeeAccountTaxWithholding.tax_identifier_value == null ? "" : item.icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                                             benefit_distribution_type_value = item.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == null ? "" : item.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value,
                                                                             marital_status_value = item.icdoPayeeAccountTaxWithholding.marital_status_value == null ? "" : item.icdoPayeeAccountTaxWithholding.marital_status_value,
                                                                             tax_allowance = item.icdoPayeeAccountTaxWithholding.tax_allowance == 0 ? "" : item.icdoPayeeAccountTaxWithholding.tax_allowance.ToString(),
                                                                             additional_tax_amount = item.icdoPayeeAccountTaxWithholding.additional_tax_amount,// == null ? "" : item.icdoPayeeAccountTaxWithholding.additional_tax_amount.ToString(),
                                                                             start_date = item.icdoPayeeAccountTaxWithholding.start_date,
                                                                             tax_option_value = item.icdoPayeeAccountTaxWithholding.tax_option_value == null ? "" : item.icdoPayeeAccountTaxWithholding.tax_option_value,

                                                                         }).FirstOrDefault();

                                string istrTaxAllowance = string.Empty;
                                if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]) == "0")
                                    istrTaxAllowance = string.Empty;
                                else
                                    istrTaxAllowance = Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]);

                                if (oldicdoPayeeAccountTaxWithholding != null && lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)//rohan 10212014 PIR 803
                                {
                                    if (oldicdoPayeeAccountTaxWithholding.start_date < DateTime.Today && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                                    {
                                        if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"])
                                            || oldicdoPayeeAccountTaxWithholding.benefit_distribution_type_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"])
                                            //|| oldicdoPayeeAccountTaxWithholding.marital_status_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.marital_status_value"])
                                            || (oldicdoPayeeAccountTaxWithholding.tax_option_value != "FLAD" && oldicdoPayeeAccountTaxWithholding.tax_option_value != "FLAP" && oldicdoPayeeAccountTaxWithholding.tax_option_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]))
                                            || oldicdoPayeeAccountTaxWithholding.tax_allowance.ToString() != istrTaxAllowance
                                            || DateTime.Compare(Convert.ToDateTime(oldicdoPayeeAccountTaxWithholding.start_date.ToShortDateString()), Convert.ToDateTime(lhstParams["icdoPayeeAccountTaxWithholding.start_date"])) != 0)
                                        {

                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }

                                        if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNotNullOrEmpty())
                                        {
                                            if (oldicdoPayeeAccountTaxWithholding.additional_tax_amount != Convert.ToDecimal(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"].ToString().Replace("$", "")))
                                            {

                                                DisplayError(utlMessageType.Solution, 6004, null);
                                                return;
                                            }
                                        }
                                        else if (oldicdoPayeeAccountTaxWithholding.additional_tax_amount != 0)
                                        {
                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }
                                    }
                                    else if (oldicdoPayeeAccountTaxWithholding.start_date >= DateTime.Today && lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION
                                          && (oldicdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NO_STATE_TAX || oldicdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NO_FEDRAL_TAX))
                                    {
                                        if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"])
                                        || oldicdoPayeeAccountTaxWithholding.benefit_distribution_type_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"])
                                        || oldicdoPayeeAccountTaxWithholding.marital_status_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.marital_status_value"])
                                        || oldicdoPayeeAccountTaxWithholding.tax_allowance.ToString() != istrTaxAllowance
                                        || DateTime.Compare(Convert.ToDateTime(oldicdoPayeeAccountTaxWithholding.start_date.ToShortDateString()), Convert.ToDateTime(lhstParams["icdoPayeeAccountTaxWithholding.start_date"])) != 0)
                                        {
                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]))
                                        {
                                            DisplayError(utlMessageType.Solution, 6004, null);
                                            return;
                                        }
                                    }
                                }
                            }


                            if (astrTaxIdentifier.IsNotNullOrEmpty() && astrBenefitDistributionType.IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 6007, null);
                                return;

                            }

                            if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxOption.IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 6008, null);
                                return;

                            }
                            if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                            {
                                if (astrMaritalStatus == busConstant.MARITAL_STATUS_MARRIED && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty())
                                {
                                    DisplayError(utlMessageType.Solution, 5482, null);
                                    return;

                                }

                            }


                            if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE)
                                && (astrMaritalStatus.IsNullOrEmpty()) && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 5481, null);
                                return;

                            }
                            if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                            {
                                if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE) && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty())
                                {
                                    DisplayError(utlMessageType.Solution, 5482, null);
                                    return;

                                }
                            }

                            if (astrTaxIdentifier != busConstant.VA_STATE_TAX && astrTaxIdentifier != busConstant.FEDRAL_STATE_TAX)
                            {
                               if( astrTaxOption != busConstant.NO_STATE_TAX && astrTaxOption != busConstant.FLAT_DOLLAR && astrTaxOption != busConstant.FLAT_PERCENT)
                                {
                                    if (astrMaritalStatus.IsNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNullOrEmpty())
                                    {
                                        DisplayError(utlMessageType.Solution, 5481, null);
                                        return;

                                    }

                                }
                                

                            }


                            if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNullOrEmpty())
                            {
                                DisplayError(utlMessageType.Solution, 5113, null);
                                return;

                            }

                            if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]).IsNotNullOrEmpty() && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.start_date"]).IsNotNullOrEmpty()
                                && Convert.ToDateTime(lhstParams["icdoPayeeAccountTaxWithholding.end_date"]) <= Convert.ToDateTime(lhstParams["icdoPayeeAccountTaxWithholding.start_date"]))
                            {
                                DisplayError(utlMessageType.Solution, 5111, null);
                                return;
                            }

                            if (astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage == 0)
                            {
                                DisplayError(utlMessageType.Solution, 6061, null);
                                return;
                            }

                            if (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNullOrEmpty())// || Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) == "0")
                                ldecAdditionalTaxAmount = 0;
                            else
                                ldecAdditionalTaxAmount = Convert.ToDecimal(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]);
                            if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                            {
                                if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional
                                || astrTaxOption == busConstant.FLAT_DOLLAR)
                                && (Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) == "0"))
                                {
                                    DisplayError(utlMessageType.Solution, 6060, null);
                                    return;
                                }
                            }
                            if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRS || astrTaxOption == busConstant.FLAT_PERCENT) && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]).IsNotNullOrEmpty() && ldecAdditionalTaxAmount != 0)
                            {
                                DisplayError(utlMessageType.Solution, 6076, null);
                                return;
                            }
                            if(astrTaxOption != busConstant.NO_FEDRAL_TAX && astrTaxOption != busConstant.NO_STATE_TAX)
                            {
                                if (astrTaxOption != busConstant.FLAT_PERCENT && Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]).IsNotNullOrEmpty() && ldectaxPercentage != 0)
                                {
                                    DisplayError(utlMessageType.Solution, 6077, null);
                                    return;
                                }

                            }
                           
                            if (ldecAdditionalTaxAmount > 0)
                            {
                                Decimal ldecTotalTaxableAmount = 0.0m;

                                if (lbusPayeeAccount.iclbPayeeAccountPaymentItemType.IsNotNull())
                                    ldecTotalTaxableAmount = (from item in lbusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                              where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                              item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                              && ((item.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                                              && (item.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFedTax
                                                              || item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck))
                                                              select item.icdoPayeeAccountPaymentItemType.amount * item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
                                if (ldecAdditionalTaxAmount > ldecTotalTaxableAmount)
                                {
                                    DisplayError(utlMessageType.Solution, 6081, null);
                                    return;
                                }
                            }
                            if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrMaritalStatus.IsNotNullOrEmpty()
                                && lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                            {
                                DisplayError(utlMessageType.Solution, 6107, null);
                                return;
                            }
                            if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && !Convert.ToString(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]).IsNullOrEmpty() && Convert.ToDecimal(lhstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]) != Decimal.Zero)
                            {
                                DisplayError(utlMessageType.Solution, 6108, null);
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (istrFormName == busConstant.WITHHOLDING_INFORMATION_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];

                    if (lhstParams.Count > 0)
                    {
                        if (Convert.ToString(lhstParams["icdoWithholdingInformation.withholding_date_from"]) != "" || Convert.ToString(lhstParams["icdoWithholdingInformation.withholding_date_from"]).IsNotNullOrEmpty())
                        {
                            if (Convert.ToDateTime(lhstParams["icdoWithholdingInformation.withholding_date_from"]) < DateTime.Today)
                            {
                                DisplayError(utlMessageType.Solution, 5112, null);
                                return;
                            }
                        }


                        if (lbusPayeeAccount.iclbWithholdingInformation.Count > 0)
                        {
                            int iintCount = (from item in lbusPayeeAccount.iclbWithholdingInformation
                                             where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                             && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                             && Convert.ToDateTime(lhstParams["icdoWithholdingInformation.withholding_date_from"]) < item.icdoWithholdingInformation.withholding_date_from
                                             select item).Count();

                            if (iintCount > 0)
                            {
                                DisplayError(utlMessageType.Solution, 6166, null);
                                return;
                            }

                            int lintCount = (from item in lbusPayeeAccount.iclbWithholdingInformation
                                             where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                             && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                             && item.icdoWithholdingInformation.withholding_date_from == Convert.ToDateTime(lhstParams["icdoWithholdingInformation.withholding_date_from"])
                                             select item).Count();

                            if (lintCount > 0)
                            {
                                DisplayError(utlMessageType.Solution, 6109, null);
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_DEDUCTION_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow["CenterMiddle"];
            if (lbusPayeeAccount != null)
            {
                Hashtable lhstParams = null;
                sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnUpdate");
                if (lbtnUpdateChild.sfwNavigationParameter != null)
                {
                    iarrSelectedRows = new ArrayList();
                    GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                    if (iarrSelectedRows.Count > 0)
                        lhstParams = (Hashtable)iarrSelectedRows[0];

                    string astrDeductionType = Convert.ToString(lhstParams["icdoPayeeAccountDeduction.deduction_type_value"]);
                    string astrPayTo = Convert.ToString(lhstParams["icdoPayeeAccountDeduction.pay_to_value"]);
                    decimal ldecDeductionAmount = 0.00m;
                    int iintOrgID = 0; int iintPersonID;


                    if (lhstParams.Count > 0)
                    {
                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.amount"]).IsNullOrEmpty() || Convert.ToString(lhstParams["icdoPayeeAccountDeduction.amount"]) == "")
                            ldecDeductionAmount = Decimal.Zero;
                        else
                            if (busGlobalFunctions.IsDecimal(Convert.ToString(lhstParams["icdoPayeeAccountDeduction.amount"])))
                        {
                            ldecDeductionAmount = Convert.ToDecimal(lhstParams["icdoPayeeAccountDeduction.amount"]);
                            if (ldecDeductionAmount < 0)
                            {
                                DisplayError(utlMessageType.Solution, 6141, null);

                                return;
                            }
                        }
                        else
                        {
                            DisplayError(utlMessageType.Solution, 6069, null);
                            return;
                        }

                        if (astrDeductionType.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6023, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.deduction_type_value"]) == busConstant.CANCELLATION_REASON_OTHER && Convert.ToString(lhstParams["icdoPayeeAccountDeduction.other_deduction_type_description"]).IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6032, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.org_id"]).IsNullOrEmpty())
                            iintOrgID = 0;
                        else
                            iintOrgID = Convert.ToInt32(lhstParams["icdoPayeeAccountDeduction.org_id"]);

                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.person_id"]).IsNullOrEmpty())
                            iintPersonID = 0;
                        else
                            iintPersonID = Convert.ToInt32(lhstParams["icdoPayeeAccountDeduction.person_id"]);

                        if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo != busConstant.SURVIVOR_TYPE_PERSON) && (iintOrgID.IsNull() || iintOrgID == 0))
                        {
                            DisplayError(utlMessageType.Solution, 6025, null);
                            return;
                        }

                        if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNull() || iintPersonID == 0))
                        {
                            DisplayError(utlMessageType.Solution, 6070, null);
                            return;
                        }

                        if (astrDeductionType.IsNotNullOrEmpty() && astrPayTo.IsNullOrEmpty())
                        {
                            DisplayError(utlMessageType.Solution, 6024, null);
                            return;
                        }

                        if (ldecDeductionAmount == Decimal.Zero)
                        {
                            DisplayError(utlMessageType.Solution, 6027, null);
                            return;
                        }
                        decimal idecTotalAmount = (from item in lbusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                   where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                                   item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                   && !((item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "FEDX"
                                                   && item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "STTX")
                                                   && item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1
                                                   && item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != "RRED"
                                                   && item.ibusPaymentItemType.icdoPaymentItemType.item_type_code != busConstant.ITEM53)
                                                   select item.icdoPayeeAccountPaymentItemType.amount * item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
                        if (idecTotalAmount < ldecDeductionAmount)
                        {
                            DisplayError(utlMessageType.Solution, 6202, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.start_date"]).IsNotNullOrEmpty())
                        {

                            var oldicdoPayeeAccountDeduction = (from item in lbusPayeeAccount.iclbPayeeAccountDeduction.AsEnumerable()
                                                                where item.icdoPayeeAccountDeduction.payee_account_deduction_id ==
                                                                (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.payee_account_deduction_id"]) == string.Empty ? 0 : Convert.ToInt32(lhstParams["icdoPayeeAccountDeduction.payee_account_deduction_id"]))
                                                                select new
                                                                {
                                                                    start_date = item.icdoPayeeAccountDeduction.start_date,
                                                                }).FirstOrDefault();

                            if (oldicdoPayeeAccountDeduction.start_date < DateTime.Today && Convert.ToDateTime(lhstParams["icdoPayeeAccountDeduction.start_date"]) != oldicdoPayeeAccountDeduction.start_date)
                            {
                                DisplayError(utlMessageType.Solution, 6203, null);
                                return;
                            }

                            if (oldicdoPayeeAccountDeduction.start_date >= DateTime.Today && Convert.ToDateTime(lhstParams["icdoPayeeAccountDeduction.start_date"]) < DateTime.Today)
                            {
                                DisplayError(utlMessageType.Solution, 5112, null);
                                return;
                            }

                            if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.end_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(lhstParams["icdoPayeeAccountDeduction.end_date"]) <= Convert.ToDateTime(lhstParams["icdoPayeeAccountDeduction.start_date"]))
                            {
                                DisplayError(utlMessageType.Solution, 5111, null);
                                return;
                            }
                        }
                        else
                        {
                            DisplayError(utlMessageType.Solution, 5113, null);
                            return;
                        }

                        if (Convert.ToString(lhstParams["icdoPayeeAccountDeduction.end_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(lhstParams["icdoPayeeAccountDeduction.end_date"]) < DateTime.Today)
                        {
                            DisplayError(utlMessageType.Solution, 5081, null);
                            return;
                        }

                        //PIR 924
                        if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNotNull() || iintPersonID != 0)
                            && lbusPayeeAccount.icdoPayeeAccount.person_id == iintPersonID)
                        {
                            DisplayError(utlMessageType.Solution, 6283, null);
                            return;
                        }

                        if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_ORG) && (iintOrgID.IsNotNull() || iintOrgID != 0)
                        && lbusPayeeAccount.icdoPayeeAccount.org_id == iintOrgID)
                        {
                            DisplayError(utlMessageType.Solution, 6283, null);
                            return;
                        }

                    }
                }
            }
        }



        if (!blnCheck)
        {
            base.btnGridViewUpdate_Click(sender, e);
        }
    }



    public bool IsNonNegative(string astrNumber)
    {
        bool lblnValidPercentage = false;
        Regex lrexGex = new Regex("^[0-9,.]*$");
        if (!lrexGex.IsMatch(astrNumber))
        {
            lblnValidPercentage = true;
        }
        return lblnValidPercentage;
    }

    protected override void FrameworkInit(string astrRemoteServer = null)
    {
        base.FrameworkInit(astrRemoteServer);
        if (iblnInChildWindow)
        {
            if (istrFormName == busConstant.BENEFICIARY_MAINTENANCE && Framework.SessionForWindow[UIConstants.CenterMiddle] is busPersonDependent)
            {
                this.Title = "Dependent Person Maintenance";
                sfwPanel lpnlMain = (sfwPanel)GetControl(this, "pnlMain");
                if (lpnlMain != null)
                {
                    lpnlMain.sfwCaption = "Dependent Information";
                }
            }
            else if (istrFormName == busConstant.BENEFICIARY_LOOKUP && Framework.SessionForWindow[UIConstants.CenterMiddle] is busPersonDependent)
            {
                this.Title = "Dependent Lookup";
            }
        }
        //if (istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance" && HttpContext.Current.Session[UIConstants.CenterMiddle] is busBenefitCalculationPostRetirementDeath)
        //{
        //    sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvBenefitCalculationOptions");
        //    if (lgrvBase.IsNotNull())
        //    {
        //        lgrvBase.SelectedIndex = 0;
        //        BindDataToForm();

        //    }
        //}
    }
    protected void btnShowURLCLick(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// WorkFlow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMyBasketSearch_Click(object sender, EventArgs e)
    {
        Hashtable lhshList = new Hashtable();
        //Search Criteria control
        sfwPanel lpnlMain = (sfwPanel)GetControl(itblParent, "pnlMain");
        GetFormValue(lpnlMain, lhshList);
        Framework.SessionForWindow[istrFormName + "wfmAIM"] = lhshList;

        btnValidateExecuteBusinessMethod_Click(sender, e);
    }



    //protected override void btnOpen_Click(object sender, EventArgs e)
    //{
    //    if (HttpContext.Current.Session["Logged_In_User_is_VIP"].IsNotNull() && HttpContext.Current.Session["Logged_In_User_is_VIP"].ToString() == "VIPAccessUser")
    //    {
    //        base.btnOpen_Click(sender, e);
    //    }
    //    else
    //    {
    //        if (iarrSelectedRows == null)
    //        {
    //            sfwButton lsfwButton = new sfwButton();
    //            lsfwButton = sender as sfwButton;
    //            iarrSelectedRows = new ArrayList();
    //            GetSelectedData(lsfwButton.sfwNavigationParameter, iarrDataControls);
    //        }

    //        int lintIsVIPCount = 0;
    //        string lstrBusinessTierUrl = (string)HttpContext.Current.Session["BusinessTierUrl"];
    //        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

    //        Hashtable lhstParam = new Hashtable();
    //        lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), 2);

    //        DataTable ldtbPlanBenefitId = lsrvBusinessTier.ExecuteQuery("cdoPerson.CheckIfRelativeIsVIP", lhstParam, idictParams);
    //        if (ldtbPlanBenefitId != null && ldtbPlanBenefitId.Rows.Count > 0 && Convert.ToString(ldtbPlanBenefitId.Rows[0][0]).IsNotNullOrEmpty())
    //        {
    //            lintIsVIPCount = Convert.ToInt32(ldtbPlanBenefitId.Rows[0][0]);
    //        }

    //        if (lintIsVIPCount > 0)
    //        {
    //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ONVIPPopupHiddenButtonClick", "ONVIPPopupHiddenButtonClick();", true);
    //        }
    //        else
    //        {
    //           base.btnOpen_Click(sender, e);
    //        }
    //    }

    //    //DisplayError(5178, null);
    //    //base.btnOpen_Click(sender, e);
    //}
    //Overriding this method since in the BENEFIT INFORMATION GRID we want to see DRO MODEL NAME And PLAN NAME that is not part of the main object in the collection
    //So I need to fire a Query to populate the extra properties inside busDroBenefitDetails so that we can see the names in each item of the collection as it gets added to the grid
    protected override bool BindFormToData(ArrayList aarrDataControls, object aobjObject = null, string agrvBase = null)
    {
        bool temp = base.BindFormToData(aarrDataControls, aobjObject, agrvBase);

        if (agrvBase == busConstant.BENEFIT_INFORMATION_GRID)
        {
            if (aobjObject is busDroBenefitDetails)
            {
                busDroBenefitDetails lbusDroBenefitDetails = (busDroBenefitDetails)aobjObject;

                Hashtable lhstParam = new Hashtable();

                switch (lbusDroBenefitDetails.istrSubPlan)
                {
                    case busConstant.EE:
                        lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag = busConstant.FLAG_YES;
                        lbusDroBenefitDetails.istrSubPlanDescription = busConstant.EE;
                        break;
                    case busConstant.UVHP:
                        lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag = busConstant.FLAG_YES;
                        lbusDroBenefitDetails.istrSubPlanDescription = busConstant.UVHP;
                        break;
                    case busConstant.L52_SPL_ACC:
                        lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag = busConstant.FLAG_YES;
                        lbusDroBenefitDetails.istrSubPlanDescription = "Local-52 Special Account";
                        break;
                    case busConstant.L161_SPL_ACC:
                        lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag = busConstant.FLAG_YES;
                        lbusDroBenefitDetails.istrSubPlanDescription = "Local-161 Special Account";
                        break;
                    case "":
                        lbusDroBenefitDetails.istrSubPlanDescription = "";
                        break;
                }


                lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id);

                DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, idictParams);
                if (ldtblist.Rows.Count > 0)
                {
                    lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanName = ldtblist.Rows[0][busConstant.PLAN_NAME].ToString();
                    lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanCode = ldtblist.Rows[0][busConstant.PLAN_CODE].ToString();
                }

                lhstParam.Clear();
                lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue);
                lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id);
                DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, idictParams);

                if (ldtbPlanBenefitId.Rows.Count > 0)
                {
                    DataRow drRow = ldtbPlanBenefitId.Rows[0];
                    lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id = Convert.ToInt32(drRow[enmPlanBenefitXr.plan_benefit_id.ToString()]);
                    lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionDescription = drRow[busConstant.FIELD_DESCRIPTION].ToString();
                }
            }
        }

        // icdoDisabilityBenefitHistory.plan_id field is removed from Disability history tab.



        if (agrvBase == busConstant.PAYEE_ACCOUNT_DEDUCTION_GRID)
        {
            if (aobjObject is busPayeeAccountDeduction)
            {
                Hashtable lhstParam = new Hashtable();
                busPayeeAccountDeduction lbusPayeeAccountDeduction = (busPayeeAccountDeduction)aobjObject;
                if (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.pay_to_value == busConstant.SURVIVOR_TYPE_PERSON)
                {
                    lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.org_id = 0;
                }
                else
                {
                    lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.person_id = 0;
                }
                if (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrOrgMPID.IsNotNullOrEmpty())
                {

                    lhstParam.Add(enmOrganization.mpi_org_id.ToString().ToUpper(), lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrOrgMPID);

                    DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetails", lhstParam, idictParams);

                    if (ldtblist.Rows.Count > 0)
                    {
                        if (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrOrgMPID.IsNotNullOrEmpty())
                        {
                            lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrOrgName = Convert.ToString(ldtblist.Rows[0][0]);
                        }
                    }
                }
                else if (lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrPersonMPID.IsNotNullOrEmpty())
                {
                    lhstParam.Add(enmPerson.mpi_person_id.ToString().ToUpper(), lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrPersonMPID);

                    DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPerson.GetPersonDetails", lhstParam, idictParams);

                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrPersonName = Convert.ToString(ldtblist.Rows[0][0]);
                    }

                }

            }
        }

        else if (agrvBase == busConstant.PERSON_ACCOUNT_BENEFICIARY_GRID)
        {
            if (aobjObject is busPersonAccountBeneficiary)
            {
                busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjObject;
                Hashtable lhstParam = new Hashtable();

                lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan);
                DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, idictParams);
                if (ldtblist.Rows.Count > 0)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan = ldtblist.Rows[0][0].ToString();
                }
            }

        }

        else if (agrvBase == busConstant.PAYEE_ACCOUNT_STATUS_GRID)
        {
            if (aobjObject is busPayeeAccountStatus)
            {
                busPayeeAccountStatus lbusPayeeAccountStatus = (busPayeeAccountStatus)aobjObject;
                Hashtable lhstParam = new Hashtable();
                if (lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date == DateTime.MinValue)
                    lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
            }

        }

        else if (agrvBase == busConstant.PAYMENT_REISSUE_DETAIL_GRID)
        {
            if (aobjObject is busPaymentReissueDetail)
            {
                busPaymentReissueDetail lbusPaymentReissueDetail = (busPaymentReissueDetail)aobjObject;

                if (lbusPaymentReissueDetail.icdoPaymentReissueDetail.reissue_payment_type_value == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
                {
                    Hashtable lhstParamForOrg = new Hashtable();
                    lhstParamForOrg.Add("ORG_ID", lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId);
                    lhstParamForOrg.Add("Person_Id", lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintPartitipantID);
                    DataTable ldtbRecipientOrgMPID = isrvBusinessTier.ExecuteQuery("cdoPaymentReissueDetail.GetOrgDetailsByOrgID", lhstParamForOrg, idictParams);
                    if (ldtbRecipientOrgMPID.Rows.Count > 0)
                    {
                        lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientOrgMPID.Rows[0]["MPI_ORG_ID"]);
                        lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId;

                    }
                    else
                    {
                        //PIR-622
                        Hashtable lhstParam = new Hashtable();
                        lhstParam.Add("Survivor_ID", lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId);
                        lhstParam.Add("Person_Id", lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintPartitipantID);
                        DataTable ldtbRecipientPersonMPID = isrvBusinessTier.ExecuteQuery("cdoPaymentReissueDetail.GetPersonDetailsByPersonID", lhstParam, idictParams);
                        if (ldtbRecipientPersonMPID.Rows.Count > 0)
                        {
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientPersonMPID.Rows[0]["MPI_PERSON_ID"]);
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId;

                        }
                    }

                }

            }

        }
        else if (agrvBase == "grvPayeeAccountAchDetail")
        {
            if (aobjObject is busPayeeAccountAchDetail)
            {
                busPayeeAccountAchDetail lbusPayeeAccountAchDetail = (busPayeeAccountAchDetail)aobjObject;
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("ROUTING_NUMBER", lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.istrRoutingNumber);
                DataTable ldtbOrgBankName = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsByRoutingNumber", lhstParam, idictParams);

                if (ldtbOrgBankName.Rows.Count > 0)
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.istrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0]["ORG_NAME"]);
            }
        }
        else if (agrvBase == "grvPayeeAccountWireDetail")
        {
            if (aobjObject is busPayeeAccountWireDetail)
            {
                busPayeeAccountWireDetail lbusPayeeAccountWireDetail = (busPayeeAccountWireDetail)aobjObject;
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("ABA_SWIFT_BANK_CODE", lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.istrAbaSwiftBankCode);
                DataTable ldtbOrgBankName = isrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgsDetailsByAbaSwiftBankCode", lhstParam, idictParams);

                if (ldtbOrgBankName.Rows.Count > 0)
                    lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.istrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0]["ORG_NAME"]);

                if (lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_completion_date == DateTime.MinValue)
                {
                    if (lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_flag == "Y")
                    {
                        lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_completion_date = DateTime.Now;
                    }

                }


            }
        }
        else if (agrvBase == busConstant.REIMBURSEMENT_DETAILS_GRID)
        {
            if (aobjObject is busReimbursementDetails)
            {
                busReimbursementDetails lbusReimbursementDetails = (busReimbursementDetails)aobjObject;
                decimal ldecAmountPaid = 0M;

                decimal ldecStateTaxAmount = 0M;
                decimal ldecFedTaxAmount = 0M;

                busRepaymentSchedule lbusRepaymentSchedule = (busRepaymentSchedule)Framework.SessionForWindow["CenterMiddle"];

                if (lbusRepaymentSchedule.iclbReimbursementDetails != null && lbusRepaymentSchedule.iclbReimbursementDetails.Count > 0)
                {
                    ldecAmountPaid = lbusRepaymentSchedule.iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.amount_paid);
                }
                else
                {
                    ldecAmountPaid = lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;
                }

                lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;

                Hashtable lhstParam = new Hashtable();
                lhstParam.Add(enmRepaymentSchedule.repayment_schedule_id.ToString().ToUpper(), lbusRepaymentSchedule.icdoRepaymentSchedule.repayment_schedule_id);

                DataTable ldtblRepaymentDetails = isrvBusinessTier.ExecuteQuery("cdoReimbursementDetails.GetRepaymentDetails", lhstParam, idictParams);

                if (ldtblRepaymentDetails != null && ldtblRepaymentDetails.Rows.Count > 0)
                {
                    foreach (DataRow ldrRepaymentDetails in ldtblRepaymentDetails.Rows)
                    {
                        if (Convert.ToString(ldrRepaymentDetails[enmPayeeAccountRetroPayment.payment_history_header_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                        {
                            // Fix Tax value in Reimbursement details grid
                            //if (lbusRepaymentSchedule.iint1099RBatchRanYear < Convert.ToInt32(ldrRepaymentDetails["YEAR"]))
                            //{
                            lhstParam.Clear();
                            lhstParam.Add(enmPayeeAccountRetroPayment.payment_history_header_id.ToString().ToUpper(),
                                Convert.ToInt32(ldrRepaymentDetails[enmPayeeAccountRetroPayment.payment_history_header_id.ToString().ToUpper()]));

                            DataTable ldtblTaxes = isrvBusinessTier.ExecuteQuery("cdoPaymentHistoryHeader.GetFedTax&StateTax", lhstParam, idictParams);

                            if (ldtblTaxes != null && ldtblTaxes.Rows.Count > 0)
                            {

                                if (Convert.ToString(ldtblTaxes.Rows[0]["STATE_TAX_AMOUNT"]).IsNotNullOrEmpty())
                                {
                                    ldecStateTaxAmount += Convert.ToDecimal(ldtblTaxes.Rows[0]["STATE_TAX_AMOUNT"]);
                                }

                                if (Convert.ToString(ldtblTaxes.Rows[0]["FEDERAL_TAX_AMOUNT"]).IsNotNullOrEmpty())
                                {
                                    ldecFedTaxAmount += Convert.ToDecimal(ldtblTaxes.Rows[0]["FEDERAL_TAX_AMOUNT"]);
                                }
                            }
                            //}
                        }
                    }

                    lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = lbusReimbursementDetails.icdoReimbursementDetails.amount_paid +
                             lbusReimbursementDetails.icdoReimbursementDetails.state_tax + lbusReimbursementDetails.icdoReimbursementDetails.fed_tax;

                    if ((ldecAmountPaid) ==
                        lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount)
                    {

                    }
                    else if ((ldecAmountPaid) ==
                        (lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount - (ldecStateTaxAmount + ldecFedTaxAmount)))
                    {
                        lbusReimbursementDetails.icdoReimbursementDetails.state_tax = ldecStateTaxAmount;
                        lbusReimbursementDetails.icdoReimbursementDetails.fed_tax = ldecFedTaxAmount;
                        lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = lbusReimbursementDetails.icdoReimbursementDetails.amount_paid +
                           lbusReimbursementDetails.icdoReimbursementDetails.state_tax + lbusReimbursementDetails.icdoReimbursementDetails.fed_tax;
                    }
                }

                lbusReimbursementDetails.icdoReimbursementDetails.payment_option_value = busConstant.REIMBURSEMENT_PAYMENT_OPTION_CHECK;
                lbusReimbursementDetails.icdoReimbursementDetails.payment_option_description = busConstant.REIMBURSEMENT_PAYMENT_OPTION_CHECK_DESC;
            }
        }
        else if (agrvBase == "grvPayeeAccountTaxWithholding")
        {

            if (ibusMain is busPayeeAccount)
            {

                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = (busPayeeAccountTaxWithholding)aobjObject;

                busPayeeAccount lbusPayeeAccount = (busPayeeAccount)ibusMain;


                if (lbusPayeeAccount.icdoPayeeAccount.istrSavingMode == "UPDATE")
                {
                    if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != null && lbusPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue != null)
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = lbusPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue;

                    }

                    if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != null && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == "MNBF" && lbusPayeeAccount.icdoPayeeAccount.istrtax_option_value == "FTIA")
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = lbusPayeeAccount.icdoPayeeAccount.istrtax_option_value;

                    }

                }


;

            }

        }
        else if (agrvBase == busConstant.PERSON_BRIDGED_HOURS_GRID)
        {
            if (aobjObject is busPersonBridgeHours)
            {
                busPersonBridgeHours lobjPersonBridgeHours = aobjObject as busPersonBridgeHours;

                if (lobjPersonBridgeHours.iclbPersonBridgeHoursDetails == null)
                {
                    lobjPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
                }

                //Ticket#85664            
                Hashtable lhstParam = new Hashtable();

                //Ticket#85664
                if (lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "DSBL")
                {

                    Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                    sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtSsn");
                    lhstParam.Add("YR", Convert.ToInt32(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date.Year));
                    lhstParam.Add("SSN", lsfwTextBox.Text.Replace("-", ""));


                    DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                    decimal lintbrigdeCalculatedHours = 0;

                    if (ldtblist.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                        {
                            //  lintbrigdeCalculatedHours = Convert.ToDecimal(ldtblist.Rows[0][0]);

                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                    }
                    else
                    {
                        lintbrigdeCalculatedHours = Convert.ToDecimal(ldtblist.Rows[0][0]);

                    }
                    if (lintbrigdeCalculatedHours > 200)
                    {
                        //  lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = lintbrigdeCalculatedHours;
                        lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) * 8);
                    }

                }
                //RID 119820 - Covid hardship bridging.
                else if (lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "COVD")
                {

                    Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                    sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtSsn");
                    lhstParam.Add("YR", Convert.ToInt32(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date.Year));
                    lhstParam.Add("SSN", lsfwTextBox.Text.Replace("-", ""));


                    DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                    if (ldtblist != null && ldtblist.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                        else
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(ldtblist.Rows[0][0]);
                        }
                    }
                    else
                    {
                        lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                    }

                }
                else
                {
                    lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) * 8);

                }


            }
        }
        else if (agrvBase == busConstant.BENEFIT_APPLICATION_DETAIL_GRID)
        {
            if (aobjObject is busBenefitApplicationDetail)
            {
                busBenefitApplicationDetail lbusBenefitApplicationDetail = (busBenefitApplicationDetail)aobjObject;
                Hashtable lhstParam = new Hashtable();

                if (lbusBenefitApplicationDetail.istrSubPlan.IsNull())
                    lbusBenefitApplicationDetail.istrSubPlan = String.Empty;

                switch (lbusBenefitApplicationDetail.istrSubPlan)
                {
                    case busConstant.EE:
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                        break;
                    case busConstant.UVHP:
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                        break;
                    case busConstant.EE_UVHP:
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE_UVHP;
                        break;
                    case busConstant.L52_SPL_ACC:
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-52 Special Account";
                        break;
                    case busConstant.L161_SPL_ACC:
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                        lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-161 Special Account";
                        break;
                }

                lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusBenefitApplicationDetail.iintPlan_ID);
                DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, idictParams);

                if (ldtblist.Rows.Count > 0)
                {
                    lbusBenefitApplicationDetail.istrPlanName = ldtblist.Rows[0][0].ToString();
                    lbusBenefitApplicationDetail.istrPlanCode = ldtblist.Rows[0][enmPlan.plan_code.ToString()].ToString();
                }

                if (ibusMain is busWithdrawalApplication)
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty())
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = string.Empty;
                }

                lhstParam.Clear();
                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNotNullOrEmpty())
                {
                    lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);
                }
                else
                {
                    lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue);
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                }

                lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusBenefitApplicationDetail.iintPlan_ID);
                DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, idictParams);

                if (ldtbPlanBenefitId.Rows.Count > 0)
                {
                    DataRow drRow = ldtbPlanBenefitId.Rows[0];
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = Convert.ToInt32(drRow[busConstant.PLAN_BENEFIT_ID]);
                }


                lhstParam.Clear();
                lhstParam.Add(enmPlanBenefitXr.plan_benefit_id.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id);
                DataTable ldtbBenefitDescription = isrvBusinessTier.ExecuteQuery("cdoBenefitApplicationDetail.GetBenDescriptionFromID", lhstParam, idictParams);

                if (ldtbBenefitDescription.Rows.Count > 0)
                {
                    lbusBenefitApplicationDetail.istrPlanBenefitDescription = ldtbBenefitDescription.Rows[0][0].ToString();

                }

                lhstParam.Clear();
                int aintParticipantId = 0;

                if (ibusMain is busRetirementApplication)
                {
                    busRetirementApplication lobjRetirementApp = (busRetirementApplication)ibusMain;
                    aintParticipantId = lobjRetirementApp.ibusPerson.icdoPerson.person_id;
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value = lobjRetirementApp.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID).First().icdoPersonAccount.istrRetirementSubType;
                }
                else if (ibusMain is busWithdrawalApplication)
                {
                    busWithdrawalApplication lobjWithdrawalApp = (busWithdrawalApplication)ibusMain;
                    aintParticipantId = lobjWithdrawalApp.ibusPerson.icdoPerson.person_id;
                }
                else if (ibusMain is busDisabilityApplication)
                {
                    busDisabilityApplication lobjDisabilityApplication = (busDisabilityApplication)ibusMain;
                    aintParticipantId = lobjDisabilityApplication.ibusPerson.icdoPerson.person_id;
                }
                else if (ibusMain is busDeathPreRetirement)
                {
                    busDeathPreRetirement lobjDeathPreRetirement = (busDeathPreRetirement)ibusMain;
                    aintParticipantId = lobjDeathPreRetirement.ibusPerson.icdoPerson.person_id;
                }

                int strBeneficiaryId = 0;

                if (ibusMain is busDeathPreRetirement)
                {
                    // if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PERSON &&
                         lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id = 0;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                        strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id;
                        lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), strBeneficiaryId);
                        DataTable ldtbBenefitaryFullName = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPersonsFullname", lhstParam, idictParams);

                        if (ldtbBenefitaryFullName.Rows.Count > 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                        }
                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                        }

                        lhstParam.Clear();
                        lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                        lhstParam.Add("SUVIVOR_ID", strBeneficiaryId);
                        lhstParam.Add("PLAN_CODE", lbusBenefitApplicationDetail.istrPlanCode);
                        DataTable ldtbBenefitary = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetSurvivorDetailsFromSurvivorId", lhstParam, idictParams);

                        if (ldtbBenefitary.Rows.Count > 0)
                        {
                            DataRow ldtrRow = ldtbBenefitary.Rows[0];
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                        }

                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                        }

                    }
                    //   else if(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_ORG &&
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id = 0;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                        strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id;
                        lhstParam.Add(enmOrganization.org_id.ToString().ToUpper(), strBeneficiaryId);
                        DataTable ldtbBenefitaryFullName = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetOrgFullName", lhstParam, idictParams);
                        if (ldtbBenefitaryFullName.Rows.Count > 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                        }
                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                        }

                        lhstParam.Clear();
                        lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                        lhstParam.Add(enmRelationship.beneficiary_org_id.ToString().ToUpper(), strBeneficiaryId);
                        DataTable ldtbBenefitary = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetOrgDetailsFromOrgId", lhstParam, idictParams);
                        if (ldtbBenefitary.Rows.Count > 0)
                        {
                            DataRow ldtrRow = ldtbBenefitary.Rows[0];
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                        }
                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                        }

                    }
                }
                else
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID != 0)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                    }

                    strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                    lhstParam.Add(enmRelationship.person_id.ToString(), aintParticipantId);
                    lhstParam.Add(enmRelationship.beneficiary_person_id.ToString(), strBeneficiaryId);
                    object larrTemp = isrvBusinessTier.ExecuteMethod("LoadParticipantDetails", lhstParam, true, idictParams);

                    if (larrTemp is ArrayList && (larrTemp as ArrayList).Count > 0)
                    {
                        ArrayList larr = larrTemp as ArrayList;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = larr[0].ToString();
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idtDOB = Convert.ToDateTime(larr[3]);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = larr[1].ToString();
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = larr[2].ToString();

                    }
                    else
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = string.Empty;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB = string.Empty;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = string.Empty;
                    }
                }
            }
        }
        else if (agrvBase == "grvBenefitCalculationDetailIAP")
        {
            if (aobjObject is busBenefitCalculationDetail)
            {
                busBenefitCalculationDetail lbusBenefitCalculationDetail = (busBenefitCalculationDetail)aobjObject;
                Hashtable lhstParam = new Hashtable();

                switch (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccount)
                {
                    case busConstant.L52_SPL_ACC:
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-52 Special Account)";
                        break;
                    case busConstant.L161_SPL_ACC:
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP (Local-161 Special Account)";
                        break;
                    default:
                        lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrSpecialAccountDescrioption = "IAP";
                        break;
                }


                lhstParam.Add("benefit_option_value", lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValue);
                DataTable ldtbBenefitDescription = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetBenefitOptionDescription", lhstParam, idictParams);

                if (ldtbBenefitDescription.Rows.Count > 0)
                {
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrBenefitOptionValueDescrioption = Convert.ToString(ldtbBenefitDescription.Rows[0][0]);
                }
            }
        }
        else if (agrvBase == busConstant.BUS_DISABILITY_BENEFIT_HISTORY)
        {

        }

        return temp;
    }

    protected override void AddToValidationSummary(string astrMessageID, string astrMessage, string astrFocusControl = null, string astrTooltip = null)
    {
        if (istrFormName == busConstant.ORGANIZATION_BANK_MAINTENANCE && astrMessageID == "120" && astrMessage.StartsWith("120 Account No - Invalid value:"))
        {
            astrMessageID = "113";
            astrMessage = "Account No cannot enter more than 20 characters";
        }
        base.AddToValidationSummary(astrMessageID, astrMessage, astrFocusControl, astrTooltip);
    }
    protected void btn_Resume_Benefits_1(object sender, EventArgs e)
    {
        busPayeeAccount lbusPayeeAccount = (busPayeeAccount)Framework.SessionForWindow[UIConstants.CenterMiddle];
        if (lbusPayeeAccount.IsNotNull())
        {
            bool lblnShowPopUP = false;
            //FM upgrade: 6.0.0.29 changes
            //lblnShowPopUP = (bool)isrvBusinessTier.ExecuteMethod("busPayeeAccount", "CheckIfConversionRecordForReemployedParticipants", new ArrayList() { lbusPayeeAccount }, null, idictParams);
            lblnShowPopUP = (bool)isrvBusinessTier.ExecuteObjectMethod("CheckIfConversionRecordForReemployedParticipants", lbusPayeeAccount, null, idictParams);
            if (lblnShowPopUP)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ONResumeButtonClick", "ONResumeButtonClick();", true);
            }
            else
            {
                if (lbusPayeeAccount.icdoPayeeAccount.reemployed_flag == busConstant.FLAG_YES && lbusPayeeAccount.icdoPayeeAccount.reemployed_flag_as_of_date != DateTime.MinValue)
                {
                    Hashtable lhstParams = new Hashtable();
                    lhstParams.Add("adtRunDate", DateTime.Now);
                    //FM upgrade: 6.0.0.29 changes
                    //isrvBusinessTier.ExecuteMethod("busPayeeAccount", "btn_ResumeBenefits", new ArrayList() { lbusPayeeAccount }, null, idictParams);
                    isrvBusinessTier.ExecuteObjectMethod("btn_ResumeBenefits", lbusPayeeAccount, null, idictParams);
                }

            }
        }

    }

    protected void btn_ExportTable(object sender, EventArgs e)
    {
        Hashtable lhst = new Hashtable();
        //Need to add a query textbox on lookup with this ID 
        sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txbSql");
        if (lsfwTextBox.IsNotNull())
        {
            string lstrFinalQuery = lsfwTextBox.Text;
            lstrFinalQuery = lstrFinalQuery.Replace("TOP 100", "");
            lhst.Add("astrFinalQuery", lstrFinalQuery);
            DataTable ldtTable = (DataTable)isrvBusinessTier.ExecuteMethod("ExportInExcel", lhst, false, idictParams);
            if (ldtTable.IsNotNull() && ldtTable.Rows.Count > 0)
            {
                Framework.SessionForWindow["ExportForm"] = istrFormName;
                sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvExport");
                lsfwGridView.GridLines = GridLines.Both;
                lsfwGridView.HeaderStyle.Height = Unit.Pixel(20);
                lsfwGridView.PageSize = ldtTable.Rows.Count;
                lsfwGridView.DataSource = ldtTable;
                lsfwGridView.DataBind();

                //FM upgrade: 6.0.0.21 changes
                //ClearExportGridControls(lsfwGridView);
                //FM upgrade: 6.0.0.24 changes
                //ExportGridViewToExcel(lsfwGridView);
                ICollection lcolData = (ICollection)Framework.SessionForWindow["ExportDataSource"];
                ExportGridViewToExcel(lsfwGridView, lcolData);
            }
        }


    }

    //public void btnGo_Click(object sender, EventArgs e)
    //{
    //   // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenVIPDialog", "OpenVIPDialog();", true);

    //    if (istrFormName == busConstant.PERSON_MAINTENANCE)
    //    {


    //        busPerson lbusperson = null;
    //        lbusperson = (busPerson)Framework.SessionForWindow["CenterMiddle"];

    //        Framework.SessionForWindow["MpiPersonId"] = lbusperson.icdoPerson.mpi_person_id;
    //        Framework.Redirect("~/wfmDefault.aspx?FormID=wfmRetirementWizardMaintenance");
    //    }



    //}



    public void btnLoginInternal_Click(object sender, EventArgs e)
    {
        sfwTextBox lsfwTextMPID = (sfwTextBox)GetControl(this, "txtMpiPersonId");
        if (lsfwTextMPID.IsNotNull())
        {
            if (lsfwTextMPID.Text.IsNotNullOrEmpty())
            {
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
                //FM upgrade changes - Remoting to WCF
                //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
                IBusinessTier isrvNeoSpinMSSBusinessTier = null;
                try
                {
                    isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("astrMpiPersonId", lsfwTextMPID.Text);
                    busPerson lobjPerson = (busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPersonWithMPID", lhstParam, false, idictParams);
                    bool lblnCanAccess = true;
                    if (lobjPerson.icdoPerson.vip_flag == busConstant.Flag_Yes)
                    {
                        if (Framework.SessionForWindow["Logged_In_User_is_VIP"].IsNotNull() && Convert.ToString(Framework.SessionForWindow["Logged_In_User_is_VIP"]) == "VIPAccessUser")
                        {
                            lblnCanAccess = true;
                        }
                        else
                        {
                            lblnCanAccess = false;
                        }
                    }
                    if (lblnCanAccess)
                    {
                        FormsAuthentication.SetAuthCookie(lsfwTextMPID.Text, true);
                        string lstrMssApp = isrvDBCache.GetConstantValue("MSSA");

                        string lstrUserNameCookie = "MPID_MSS_LOGIN_FROM_APPLICATION";
                        HttpCookie lcokNeoSpinCookie = new HttpCookie(lstrUserNameCookie);
                        lcokNeoSpinCookie.Value = lsfwTextMPID.Text;
                        Response.Cookies.Add(lcokNeoSpinCookie);

                        //Need to replace localhost with the server name.
                        ContentPlaceHolder c = (ContentPlaceHolder)base.Master.FindControl("cphCenterMiddle");
                        string lstrMSSWindowName = Guid.NewGuid().ToString();
                        c.Controls.Add(new PopupWindowControl("/" + lstrMssApp + "/" + "wfmEntryPoint.aspx?FWN=" + lstrMSSWindowName));
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenVIPDialog", "OpenVIPDialog();", true);
                    }
                }
                finally
                {
                    HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
                }
            }
        }

    }
    protected override void InitializeWizard()
    {
        if (istrFormName == "wfmReturnMailWizard" || istrFormName == "wfmReturnMailOrganizationWizard")
        {

            InitializeWizardSessionVariables(istrFormName);
            ienmPageMode = utlPageMode.New;


        }


        base.InitializeWizard();
        if (istrFormName == "wfmReturnMailWizard" || istrFormName == "wfmReturnMailOrganizationWizard")
        {
            if (Framework.SessionForWindow["Logged_In_User_is_VIP"].IsNotNull() && Framework.SessionForWindow["Logged_In_User_is_VIP"].ToString() == "VIPAccessUser")
            {
                busReturnedMail lbusReturnedMail = Framework.SessionForWindow["CenterMiddle"] as busReturnedMail;

                lbusReturnedMail.astrVipAcc = "Y";
                Framework.SessionForWindow["CenterMiddle"] = lbusReturnedMail;
            }

        }
    }
    #region Commented-Code
    //protected void btnReassignWorkSearch_Click(object sender, EventArgs e)
    //{
    //    busReassignWork lobjReassignWork = HttpContext.Current.Session[UIConstants.CenterMiddle] as busReassignWork;

    //    if (lobjReassignWork.IsNotNull())
    //    {
    //        btnValidateExecuteBusinessMethod_Click(sender, e);

    //        lobjReassignWork = HttpContext.Current.Session[UIConstants.CenterMiddle] as busReassignWork;

    //        if (lobjReassignWork.iblnSearchCriteriaEntered)
    //        {
    //            if (lobjReassignWork.iintActualRecordCount == 0)
    //            {
    //                DisplayMessage(WorkflowConstants.Message_Id_2);
    //            }
    //            else if (lobjReassignWork.iintActualRecordCount > lobjReassignWork.iintMaxSearchCount)
    //            {
    //                DisplayMessage(WorkflowConstants.Message_Id_3, new object[2] { lobjReassignWork.iintActualRecordCount, lobjReassignWork.iintMaxSearchCount });
    //            }
    //            else
    //            {
    //                DisplayMessage(WorkflowConstants.Message_Id_1, new object[1] { lobjReassignWork.iintActualRecordCount });
    //            }
    //        }
    //        else
    //        {
    //            DisplayMessage(WorkflowConstants.Message_Id_5);
    //        }
    //    }
    //}

    //protected void btnReassign_Click(object sender, EventArgs e)
    //{
    //    busReassignWork lobjReassignWork = HttpContext.Current.Session[UIConstants.CenterMiddle] as busReassignWork;

    //    if (lobjReassignWork.IsNotNull())
    //    {
    //        btnExecuteBusinessMethodSelectRows_Click(sender, e);

    //        lobjReassignWork = HttpContext.Current.Session[UIConstants.CenterMiddle] as busReassignWork;

    //        if (lobjReassignWork.iblnAllWorkItemsReassignedSuccessfully)
    //        {
    //            if (lobjReassignWork.iintReassignQueueId > 0)
    //            {
    //                DisplayMessage(WorkflowConstants.Message_Id_1523, new object[1] { lobjReassignWork.iintReassignQueueId });
    //            }
    //            else if (lobjReassignWork.istrReassignUser.IsNotNullOrEmpty())
    //            {
    //                DisplayMessage(WorkflowConstants.Message_Id_1522, new object[1] { lobjReassignWork.istrReassignUser });
    //            }

    //            lobjReassignWork.istrReassignUser = string.Empty;
    //            lobjReassignWork.iintReassignQueueId = 0;
    //        }
    //    }
    //}

    //protected void btnOpenResource_Click(object sender, EventArgs e)
    //{
    //    IsfwButton lbtnBase = (IsfwButton)sender;

    //    if ((iarrSelectedRows == null) || (iarrSelectedRows.Count == 0))
    //    {
    //        GetSelectedData((IsfwButton)sender);
    //    }

    //    GetCurrentObject(lbtnBase.sfwParentForm);
    //    busQueue lobjQueue = (busQueue)ibusMain;

    //    foreach (Hashtable lhstParams in iarrSelectedRows)
    //    {
    //        lhstParams["resource_id"] = lobjQueue.istrQueueResources;
    //    }

    //    btnOpen_Click(sender, e);
    //}


    //protected void btn_SelectWorkItem(object sender, EventArgs e)
    //{
    //    sfwButton lbtnOpen = (sfwButton)sender;
    //    btnValidateExecuteBusinessMethod_Click(sender, e);
    //    Session["WorkflowActivities"] = HelperFunction.GetObjectValue(ibusMain, "icdoActivityInstance.activity_instance_id", ReturnType.Object);

    //    if (lbtnOpen.sfwRelatedControl.Equals("grvUserAssignedActivities"))
    //    {
    //        //Activity is selected.            
    //        IsfwButton lbtnCheckOut = (IsfwButton)GetControl(this, "btnCheckoutActivity");
    //        if (lbtnCheckOut != null)
    //        {
    //            btnWorkflowExecuteMethod_Click(lbtnCheckOut, e);
    //            UpdatePanel luppCenterMiddle = (UpdatePanel)Master.FindControl("uppCenterMiddle");
    //            luppCenterMiddle.Update();
    //        }
    //    }
    //}


    //protected override void OnInit(EventArgs e)
    //{
    //    base.OnInit(e);
    //    if (iblnInRetreivalMode)
    //    {
    //        System.Web.UI.WebControls.Table ltblFooter = (System.Web.UI.WebControls.Table)Master.FindControl("tblFooter");
    //        if (ltblFooter != null)
    //        {
    //            ltblFooter.Visible = false;
    //        }
    //    }
    //    Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
    //    Response.Cache.SetNoStore();
    //}

    #endregion

    # region NeoCerify
    protected void btnPositionGrid_Click(object sender, EventArgs e)
    {
        string lstrParam = Request.Form[((WebControl)sender).UniqueID];
        string[] larrParam = lstrParam.Split(",");
        sfwGridView lgrvTemp = (sfwGridView)GetControl(this, larrParam[0]);
        string strExpression = larrParam[1];
        bool blnIsRetrieve = false;
        if (larrParam.Length > 2)
        {
            if (larrParam[2] == "Y")
            {
                blnIsRetrieve = true;
            }
        }
        string strCenterLeftForm = string.Empty;
        if (larrParam.Length > 3)
        {
            strCenterLeftForm = larrParam[3];
        }
        if (!string.IsNullOrEmpty(strCenterLeftForm))
        {
            GetCurrentObject(strCenterLeftForm);
        }
        else
        {
            GetCurrentObject();
        }
        if (ienmPageType != sfwPageType.Lookup)
        {
            BindFormToData(iarrDataControls);
        }
        SetGridPage(lgrvTemp, strExpression, blnIsRetrieve, strCenterLeftForm);
        BindDataToForm();
        if (!string.IsNullOrEmpty(strCenterLeftForm))
        {
            SetCurrentObject(strCenterLeftForm);
            RefreshUpdatePanel("uppAccordian");
        }
        else
        {
            SetCurrentObject();
            RefreshUpdatePanel("uppCenterMiddle");
        }
    }

    protected string RemoveWhiteSpace(string astrInputString)
    {
        if (!string.IsNullOrEmpty(astrInputString))
        {
            astrInputString = astrInputString.Trim();
            if (astrInputString.Contains(" "))
            {
                string[] strInput = astrInputString.Split(' ');
                astrInputString = string.Empty;
                foreach (string s in strInput)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        astrInputString += s.Trim() + " ";
                    }
                }
                astrInputString = astrInputString.Trim();
            }
            if (astrInputString.IndexOf("<") > -1 && astrInputString.IndexOf(">") > 0)
            {
                string[] strSplitSpan = astrInputString.Split('>');
                int countSpan = 0;
                astrInputString = string.Empty;
                foreach (string strSpan in strSplitSpan)
                {
                    countSpan++;
                    string strInp = strSpan;
                    if (strInp.IndexOf("<") > -1 && countSpan < strSplitSpan.Length)
                    {
                        strInp = strInp.Substring(0, strInp.IndexOf("<"));
                    }
                    if (!string.IsNullOrEmpty(strInp))
                    {
                        astrInputString += strInp + " ";
                    }
                }
                astrInputString = astrInputString.Trim();
            }
        }
        return astrInputString;
    }

    protected void SetGridPage(sfwGridView aGridView, string astrExpression, bool blnRetrieve, string astrCenterLeftForm)
    {
        ArrayList arrBus = new ArrayList();
        if (!string.IsNullOrEmpty(astrCenterLeftForm))
        {
            arrBus.Add(Framework.SessionForWindow[astrCenterLeftForm]);
        }
        else if (blnRetrieve)
        {
            arrBus.Add(Framework.SessionForWindow["ChildCenterMiddle"]);
        }
        else
        {
            arrBus.Add(Framework.SessionForWindow["CenterMiddle"]);
        }
        //FM upgrade: 6.0.0.29 changes
        //ICollection lcolBase = (ICollection)HelperFunction.GetObjectFromResult(arrBus, aGridView.sfwObjectID);
        ICollection lcolBase = (ICollection)HelperFunction.GetObjectFromResult(arrBus, aGridView.sfwObjectField);
        int iPageSize = aGridView.PageSize;
        int iPageIndex = aGridView.PageIndex;
        if (lcolBase != null)
        {
            int iRowIndex = -1;
            if (int.TryParse(astrExpression, out iRowIndex))
            {
                if (iRowIndex >= iPageSize)
                {
                    //Handle last item in grid on a apage(removed + 1)
                    iPageIndex = ((iRowIndex) / iPageSize);
                    iRowIndex = iRowIndex - (iPageIndex * iPageSize);
                }
                else
                {
                    iPageIndex = 0;
                }
            }
            else
            {
                string[] strFields = astrExpression.Split(';');
                int iColIdx = 0;
                bool blnColIndx = false;
                //Chnage done for Soft Errors
                foreach (object objBase in lcolBase)
                {
                    bool blnMatch = true;
                    foreach (string strField in strFields)
                    {
                        string strObjectField = strField.Substring(0, strField.IndexOf('='));
                        string strFieldValue = strField.Substring(strField.IndexOf('=') + 1);
                        object objActualValue = HelperFunction.GetObjectValue(objBase, strObjectField, ReturnType.Object);
                        if (objActualValue is DateTime)
                        {
                            if (Convert.ToDateTime(strFieldValue) != Convert.ToDateTime(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else if (objActualValue is decimal)
                        {
                            if (Convert.ToDecimal(strFieldValue) != Convert.ToDecimal(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else if (objActualValue is int)
                        {
                            if (Convert.ToInt32(strFieldValue) != Convert.ToInt32(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else
                        {
                            if (RemoveWhiteSpace(Convert.ToString(strFieldValue)) != RemoveWhiteSpace(Convert.ToString(objActualValue)))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                    }
                    if (blnMatch)
                    {
                        iRowIndex = iColIdx;
                        blnColIndx = true;
                        if (iRowIndex >= iPageSize)
                        {
                            iPageIndex = ((iRowIndex + 1) / iPageSize);
                            iRowIndex = iRowIndex - (iPageIndex * iPageSize);
                        }
                        else
                        {
                            iPageIndex = 0;
                        }
                        break;
                    }
                    iColIdx++;
                }
                if (!blnColIndx)
                {
                    iRowIndex = -1;
                }
            }
            aGridView.PageIndex = iPageIndex;
            HiddenField hfldRowIndex = (HiddenField)GetControl(this, "hfldRowIndex");
            hfldRowIndex.Value = Convert.ToString(iRowIndex);
        }
    }



    #endregion

    public void FinishReturnMailWizard(object sender, EventArgs e)
    {
        if (istrFormName == "wfmReturnMailWizard" || istrFormName == "wfmReturnMailOrganizationWizard")
        {
            sfwButton lbtn = (sfwButton)GetControl(itblParent, "btnFinish");
            if (lbtn.IsNotNull())
            {
                base.btnWizardFinish_Click(lbtn, e);
                busReturnedMail lbusReturnedMail = Framework.SessionForWindow["CenterMiddle"] as busReturnedMail;
                if (lbusReturnedMail.iarrErrors.IsNull() || (lbusReturnedMail.iarrErrors.IsNotNull() && lbusReturnedMail.iarrErrors.Count == 0))
                {

                    Framework.Redirect("wfmDefault.aspx?FormID=" + istrFormName);
                }
            }
        }

    }

    protected void btn_OpenPDF(object sender, EventArgs e)
    {
        Hashtable lhstParams = new Hashtable();
        sfwButton lsfwlnkbtn = (sfwButton)sender;
        if (lsfwlnkbtn.IsNotNull())
        {
            if (GetControl(this, "lblPersonId1").IsNotNull() && GetControl(this, "lblPersonId1") is sfwLabel)
            {
                sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "lblPersonId1");
                lhstParams.Add("aintPersonID", Convert.ToInt32(lsfwLabel.Text));
            }
            if (GetControl(this, "txtPersonId").IsNotNull() && GetControl(this, "txtPersonId") is sfwTextBox)
            {
                sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtPersonId");

                if (Convert.ToString(lsfwTextBox.Text).IsNotNullOrEmpty())
                    lhstParams.Add("aintPersonID", Convert.ToInt32(lsfwTextBox.Text));
            }
            //LA Sunset - Payment Directives
            #region Payment Directives
            if (GetControl(this, "lblPayeeAccountId").IsNotNull() && GetControl(this, "lblPayeeAccountId") is sfwLabel)
            {
                sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "lblPayeeAccountId");
                lhstParams.Add("aintPayeeAccountId", Convert.ToInt32(lsfwLabel.Text));
            }
            if (GetControl(this, "txtSpecialInstructions").IsNotNull() && GetControl(this, "txtSpecialInstructions") is sfwTextBox)
            {
                sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtSpecialInstructions");
                lhstParams.Add("astrSpecialInstructions", Convert.ToString(lsfwTextBox.Text));
            }
            if (GetControl(this, "txtAdhocPaymentDate").IsNotNull() && GetControl(this, "txtAdhocPaymentDate") is sfwTextBox)
            {
                sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txtAdhocPaymentDate");
                lhstParams.Add("adtAdhocPaymentDate", Convert.ToString(lsfwTextBox.Text).IsNotNullOrEmpty() ? Convert.ToDateTime(lsfwTextBox.Text) : DateTime.MinValue);
            }
            if (GetControl(this, "lbModifiedBy").IsNotNull() && GetControl(this, "lbModifiedBy") is sfwLabel)
            {
                sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "lbModifiedBy");
                lhstParams.Add("astrModifiedBy", Convert.ToString(lsfwLabel.Text));
            }
            if (lsfwlnkbtn.ID == "btnRetrieveDirectives" && GetControl(this, "ddlPaymentCycleDate").IsNotNull() && GetControl(this, "ddlPaymentCycleDate") is sfwDropDownList)
            {
                sfwDropDownList lsfwDropDownList = (sfwDropDownList)GetControl(this, "ddlPaymentCycleDate");
                if (Convert.ToString(lsfwDropDownList.Text).IsNullOrEmpty())
                {
                    DisplayError("Please select a date from below dropdown", null);
                    return;
                }
                else
                {
                    lhstParams.Add("adtPaymentCycleDate", Convert.ToString(lsfwDropDownList.Text).IsNotNullOrEmpty() ? Convert.ToDateTime(lsfwDropDownList.Text) : DateTime.MinValue);
                }
            }

            if (lsfwlnkbtn.ID == "btnRetrieveDeletedDirectives" && GetControl(this, "ddlDeletedDirectiveCreatedDate").IsNotNull() && GetControl(this, "ddlDeletedDirectiveCreatedDate") is sfwDropDownList)
            {
                sfwDropDownList lsfwDropDownList = (sfwDropDownList)GetControl(this, "ddlDeletedDirectiveCreatedDate");
                if (Convert.ToString(lsfwDropDownList.Text).IsNullOrEmpty())
                {
                    DisplayError("Please select a date from below dropdown", null);
                    return;
                }
                else
                {
                    lhstParams.Add("aintDeletedPaymentDirectiveId", Convert.ToString(lsfwDropDownList.Text).IsNotNullOrEmpty() ? Convert.ToInt32(lsfwDropDownList.Text) : 0);
                }
            }

            #endregion Payment Directives
        }

        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                lhstParams, true, idictParams);

        if (iobjMethodResult != null)
        {
            byte[] lbyteFile = (byte[])iobjMethodResult;
            Session["PDFFile"] = iobjMethodResult;

            string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,dependent=yes,alwaysRaised=yes'";
            //'WND_Webhelp'
            string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
        }
        else
        {
            DisplayMessage(utlMessageType.Solution, 6225);
        }
    }

    protected void btn_OpnPDF(object sender, EventArgs e)
    {

        IsfwButton btn = (IsfwButton)sender;
        GridViewRow grv = (GridViewRow)btn.NamingContainer;
        sfwLabel filename = (sfwLabel)grv.FindControl("lblFileName");

        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("astr_filename", filename.Text.ToString() + ".pdf");

        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                lhstParams, true, idictParams);

        if (iobjMethodResult != null)
        {
            byte[] lbyteFile = (byte[])iobjMethodResult;
            Session["PDFFile"] = iobjMethodResult;

            string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,dependent=yes,alwaysRaised=yes'";

            string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
        }
        else
        {
            DisplayMessage(utlMessageType.Solution, 6225);
        }
    }

    protected void btnChangeBenefitOption_Click(object sender, EventArgs e)
    {


        if (istrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE || istrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE
           || istrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE || istrFormName == busConstant.DISABILITY_APPLICATION_MAINTAINENCE)
        {
            busBenefitApplication lbusBenefitApplication = (busBenefitApplication)Framework.SessionForWindow["CenterMiddle"];
            Hashtable lhstParams = null;
            sfwButton lbtnUpdateChild = (sfwButton)GetControl(this, "btnChangeBenefitOption");
            if (lbtnUpdateChild.sfwNavigationParameter != null)
            {
                iarrSelectedRows = new ArrayList();
                GetSelectedData(lbtnUpdateChild.sfwNavigationParameter, iarrDataControls);

                if (iarrSelectedRows.Count > 0)
                    lhstParams = (Hashtable)iarrSelectedRows[0];
                if (lhstParams.Count > 0)
                {
                    if (lbusBenefitApplication != null)
                    {
                        foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusBenefitApplication.iclbBenefitApplicationDetail)
                        {

                            if (Convert.ToString(lhstParams["benefit_application_detail_id"]) != string.Empty && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id == Convert.ToInt32(lhstParams["benefit_application_detail_id"]))
                            {

                                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue != Convert.ToString(lhstParams["istrBenefitOptionValue"]))
                                {

                                    //    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(lhstParams["istrBenefitOptionValue"]);
                                    iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                                    lhstParams, true, idictParams);
                                    this.btnCancel_Click(sender, e);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (istrFormName == busConstant.PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE || istrFormName == busConstant.PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE || istrFormName == busConstant.PAYEE_ACCOUNT_STATE_WIZARD_TAXWITHHOLDING_MAINTENANCE)
        {
            busPayeeAccount lbusPayeeAccount = null;
            lbusPayeeAccount = Framework.SessionForWindow["CenterMiddle"] as busPayeeAccount;

            if (lbusPayeeAccount != null)
            {
                isrvBusinessTier.ExecuteObjectMethod("Cancel_RetirementApplication_Wizard", lbusPayeeAccount, null, idictParams);

                Framework.SessionForWindow["PersonId"] = lbusPayeeAccount.icdoPayeeAccount.person_id;
                Framework.Redirect("~/wfmDefault.aspx?FormID=wfmPersonMaintenance");
            }

        }

    }

    protected void btnGenToExcel_Click(object sender, EventArgs e)


    {
        HtmlForm lhtfBase = this.Form;

        sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "dgrResult");

        sfwTextBox lsfwRetirementDateFrom = (sfwTextBox)GetControl(this, "txtCpctRetirementDateFrom");
        sfwTextBox lsfwRetirementDateTo = (sfwTextBox)GetControl(this, "txtCpctRetirementDateTo");

        Hashtable lhst = new Hashtable();
        lhst.Add("DateFrom", lsfwRetirementDateFrom.Text);
        lhst.Add("DateTo", lsfwRetirementDateTo.Text);
        isrvBusinessTier.ExecuteMethod("GenerateReportInExcel", lhst, false, idictParams);
    }

    protected override void OnPreInit(EventArgs e)
    {
        if ((string)Session["IsNewSession"] == "true")
        {
            Response.Redirect("wfmLogin.aspx");
        }
        //Fw upgrade: PIR ID : 28998: SIT day 2- After login when clicked browser’s back or forward button getting runtime error
        if (Session.IsCookieless)
        {
            Response.Redirect("SessionError.html?ErrorCode=500", true);
        }
        ihflLoginWindowName = (HiddenField)GetControl(this, "hfldLoginWindowName");
        if (String.IsNullOrEmpty(Framework.istrWindowName) && ihflLoginWindowName == null && Master != null)
        {
            ihflLoginWindowName = (HiddenField)Master.FindControl("hfldLoginWindowName");
        }
        if (ihflLoginWindowName == null)
        {
            if (String.IsNullOrEmpty(Framework.istrWindowName))
            {
                Response.Redirect("SessionError.html?ErrorCode=100", true);
            }
            string lstrUserLoggedOn = Framework.SessionForWindow["UserLoggedOn"] as string;
            if ((lstrUserLoggedOn == null) || (lstrUserLoggedOn != "true"))
            {
                Response.Redirect("SessionError.html?ErrorCode=200", true);
            }
            if (Framework.istrWindowName != (string)Framework.SessionForWindow["WindowName"])
            {
                Response.Redirect("SessionError.html?ErrorCode=300", true);
            }
        }
        base.OnPreInit(e);
    }
}
