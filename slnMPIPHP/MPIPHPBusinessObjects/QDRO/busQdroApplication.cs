#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.CustomDataObjects;
using Sagitec.DataObjects;
using MPIPHP.Common;
using System.Text.RegularExpressions;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busQdroApplication:
    /// Inherited from busQdroApplicationGen, the class is used to customize the business object busQdroApplicationGen.
    /// </summary>
    [Serializable]
    public class busQdroApplication : busQdroApplicationGen
    {

        #region Properties

        public busPerson ibusParticipant { get; set; }
        public busPerson ibusAlternatePayee { get; set; }
        public Collection<busAttorney> iclbAttorney { get; set; }
        public Collection<busDroBenefitDetails> iclbDroBenefitDetails { get; set; }
        public Collection<busDroApplicationStatusHistory> iclbDroStatusHistory { get; set; }
        public Collection<busPlanBenefitXr> iclbPlanBenefitXr { get; set; }
        public bool iblIsNew { get; set; }

        // List of Qualified Plans for the same payee &  alternate payee 
        public Collection<busDroBenefitDetails> iclbOtherQLFDDroBenefitDetails { get; set; }

        private cdoDroApplication icdoDroApplicationOld { get; set; }

        public string istrAttorneyName { get; set; }
        public string istrApprovedByUser { get; set; }
        public string istrApprovedByUserInitials { get; set; }
        public string istrApprovedByUserFullName { get; set; }
        public string istrCurrentDate { get; set; }

        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatusByApplication { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        public string istrParticipantName { get; set; }
        public string istrExSpouseName { get; set; }
        public string istrDateOfMarrige { get; set; }
        public string istrDateOfSeparation { get; set; }
        public string istrTrue { get; set; }
        public string istrIsUSA { get; set; }
        public string istrRespondent { get; set; }
        public string istrEmailAddr { get; set; }
        #endregion

        #region Properties for WorkFlow
        public bool L52Plan_Selected { get; set; }
        public bool L161Plan_Selected { get; set; }
        public bool L600Plan_Selected { get; set; }
        public bool L700Plan_Selected { get; set; }
        public bool L666Plan_Selected { get; set; }
        public bool MPIPPPlan_Selected { get; set; }
        public bool IAPPlan_Selected { get; set; }
        public int MPI_Final_Calculation_Id { get; set; }
        public int IAP_Final_Calculation_Id { get; set; }

        public int L52_Final_Calculation_Id { get; set; }
        public int L161_Final_Calculation_Id { get; set; }
        public int L600_Final_Calculation_Id { get; set; }
        public int L700_Final_Calculation_Id { get; set; }
        public int L666_Final_Calculation_Id { get; set; }
        #endregion

        #region Public Methods

        public void LoadAllBenefitDetails()
        {
            DataTable ldtbList = Select("cdoDroApplication.CheckQualifiedPlans", new object[2] { this.icdoDroApplication.person_id, this.icdoDroApplication.alternate_payee_id });
            iclbOtherQLFDDroBenefitDetails = GetCollection<busDroBenefitDetails>(ldtbList, "icdoDroBenefitDetails");
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);

            if (this.icdoDroApplication.ienuObjectState == ObjectState.Insert)
                iblIsNew = true;
            else
                iblIsNew = false;
        }

        public void LoadBenefitDetails()
        {
            DataTable ldtbList = Select("cdoDroBenefitDetails.LoadBenefitDetailsByDroApplId", new object[1] { this.icdoDroApplication.dro_application_id });
            iclbDroBenefitDetails = GetCollection<busDroBenefitDetails>(ldtbList, "icdoDroBenefitDetails");

            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {

                if (lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag.IsNotNullOrEmpty() && lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES)
                {
                    lbusDroBenefitDetails.istrSubPlan = busConstant.EE;
                    lbusDroBenefitDetails.istrSubPlanDescription = busConstant.EE;

                }
                else if (lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag.IsNotNullOrEmpty() && lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES)
                {
                    lbusDroBenefitDetails.istrSubPlan = busConstant.UVHP;
                    lbusDroBenefitDetails.istrSubPlanDescription = busConstant.UVHP;
                }
                else if (lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag.IsNotNullOrEmpty() && lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES)
                {
                    lbusDroBenefitDetails.istrSubPlan = busConstant.L52_SPL_ACC;
                    lbusDroBenefitDetails.istrSubPlanDescription = "Local-52 Special Account";
                }
                else if (lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag.IsNotNullOrEmpty() && lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES)
                {
                    lbusDroBenefitDetails.istrSubPlan = busConstant.L161_SPL_ACC;
                    lbusDroBenefitDetails.istrSubPlanDescription = "Local-161 Special Account";
                }
            }
        }


        public void LoadAttorney()
        {
            DataTable ldtbList = Select<cdoAttorney>(
                new string[1] { enmAttorney.dro_application_id.ToString() },
                new object[1] { icdoDroApplication.dro_application_id }, null, null);

            iclbAttorney = GetCollection<busAttorney>(ldtbList, "icdoAttorney");
        }

        public void LoadDroStatusHistory()
        {
            DataTable ldtbList = Select<cdoDroApplicationStatusHistory>(
                new string[1] { enmDroApplicationStatusHistory.dro_application_id.ToString() },
                new object[1] { icdoDroApplication.dro_application_id }, null, "STATUS_DATE DESC");

            iclbDroStatusHistory = GetCollection<busDroApplicationStatusHistory>(ldtbList, "icdoDroApplicationStatusHistory");
        }

        public bool IsEmailValid()
        {

            foreach (busAttorney lbusAttorney in this.iclbAttorney)
            {
                if (string.IsNullOrEmpty(lbusAttorney.icdoAttorney.email_address))
                {
                    return true;
                }
                else
                {
                    return busGlobalFunctions.IsValidEmail(lbusAttorney.icdoAttorney.email_address);
                }
            }
            return true;

        }

        public bool IsQualifiedDateNull()
        {
            if (this.icdoDroApplication.qualified_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsReceivedDateNull()
        {
            if (this.icdoDroApplication.received_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsDateOfSeparationNull()
        {
            if (this.icdoDroApplication.date_of_divorce == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsJoinderRcvDateNull()
        {
            if (this.icdoDroApplication.joinder_recv_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsOrderDateNull()
        {
            if (this.icdoDroApplication.order_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsDateOfSeparationValid()
        {
            if (this.icdoDroApplication.date_of_divorce != DateTime.MinValue && this.icdoDroApplication.date_of_marriage != DateTime.MinValue)
            {
                if (this.icdoDroApplication.date_of_divorce < this.icdoDroApplication.date_of_marriage)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return true;
        }

        public ArrayList ValidateNewChild(Hashtable ahstParams, bool blnOnADD)
        {
            ArrayList larrErrors = new ArrayList();
            decimal ldecPercentage = 0;
            int lintPlanID = 0, lintBalanceAsofPlanYear = 0, lintAltPayeeBenefitCapYear = 0;
            string lstrtDroModel = string.Empty, lstrDroValue = string.Empty, lstrSubPlan = string.Empty,
                   lstrAltPayeeInc = string.Empty, lstrAltPayeeEarlyRet = string.Empty, lstrAltPayeeEntitledToParticipantsInc = string.Empty;
            decimal ldecBenefitFlatPerc = 0, ldecWithheldPercent = 0;
            DateTime ldtNetInvestmentFromDate = new DateTime();
            DateTime ldtNetInvestmentToDate = new DateTime();

            lstrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);

            if (lstrSubPlan == string.Empty)
            {
                lstrSubPlan = null;
            }


            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.plan_id"])))
            {
                lintPlanID = Convert.ToInt32(ahstParams["icdoDroBenefitDetails.plan_id"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.dro_model_value"])))
            {
                lstrtDroModel = Convert.ToString(ahstParams["icdoDroBenefitDetails.dro_model_value"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.benefit_perc"])))
            {
                ldecPercentage = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.benefit_perc"]);
            }
            decimal ldecAmount = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.benefit_amt"])))
            {
                ldecAmount = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.benefit_amt"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.benefit_flat_perc"])))
            {
                ldecBenefitFlatPerc = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.benefit_flat_perc"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.dro_withheld_perc"])))
            {
                ldecWithheldPercent = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.dro_withheld_perc"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.alt_payee_increase"])))
            {
                lstrAltPayeeInc = Convert.ToString(ahstParams["icdoDroBenefitDetails.alt_payee_increase"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.alt_payee_early_ret"])))
            {
                lstrAltPayeeEarlyRet = Convert.ToString(ahstParams["icdoDroBenefitDetails.alt_payee_early_ret"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.balance_as_of_plan_year"])))
            {
                lintBalanceAsofPlanYear = Convert.ToInt32(ahstParams["icdoDroBenefitDetails.balance_as_of_plan_year"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.alt_payee_benfit_cap_year"])))
            {
                lintAltPayeeBenefitCapYear = Convert.ToInt32(ahstParams["icdoDroBenefitDetails.alt_payee_benfit_cap_year"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.net_investment_from_date"])))
            {
                ldtNetInvestmentFromDate = Convert.ToDateTime(ahstParams["icdoDroBenefitDetails.net_investment_from_date"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.net_investment_to_date"])))
            {
                ldtNetInvestmentToDate = Convert.ToDateTime(ahstParams["icdoDroBenefitDetails.net_investment_to_date"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase"])))
            {
                lstrAltPayeeEntitledToParticipantsInc = Convert.ToString(ahstParams["icdoDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase"]);
            }



            if (lintPlanID == 0 || string.IsNullOrEmpty(lstrtDroModel))
            {
                utlError lobjError = null;
                lobjError = AddError(5090, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            //
            if (blnOnADD)
            {
                if (lintPlanID == busConstant.MPIPP_PLAN_ID || lintPlanID == busConstant.IAP_PLAN_ID)
                {

                    busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusBenefitApplication.ibusPerson = this.ibusParticipant;
                    lbusBenefitApplication.ibusPerson.iclbPersonAccount = this.ibusParticipant.iclbPersonAccount;

                    string lstrPlanCode = string.Empty;
                    if (lintPlanID == busConstant.MPIPP_PLAN_ID)
                    {
                        lstrPlanCode = busConstant.MPIPP;
                    }
                    else
                    {
                        lstrPlanCode = busConstant.IAP;
                    }

                    if (!lbusBenefitApplication.CheckAlreadyVested(lstrPlanCode) && lstrSubPlan == null)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5437, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }

                if (lintPlanID != busConstant.IAP_PLAN_ID && lstrAltPayeeEntitledToParticipantsInc == busConstant.FLAG_YES &&
                                lstrAltPayeeInc != busConstant.FLAG_YES)
                {
                    utlError lobjError = null;
                    lobjError = AddError(6068, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }

                if (lintPlanID == busConstant.IAP_PLAN_ID &&
                    Convert.ToString(ahstParams["icdoDroBenefitDetails.istrBenefitOptionValue"]) != busConstant.LUMP_SUM)
                {
                    utlError lobjError = null;
                    lobjError = AddError(5465, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
                if (lintPlanID != busConstant.IAP_PLAN_ID &&
                    (lintBalanceAsofPlanYear != 0 || lintAltPayeeBenefitCapYear != 0 ||
                    ldtNetInvestmentFromDate != DateTime.MinValue || ldtNetInvestmentToDate != DateTime.MinValue))
                {
                    utlError lobjError = null;
                    lobjError = AddError(6204, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
                else if (lintPlanID == busConstant.IAP_PLAN_ID)
                {
                    if (ldtNetInvestmentFromDate != DateTime.MinValue && (ldtNetInvestmentFromDate.Day != 1 || ldtNetInvestmentFromDate.Month != 1))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5444, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (lintBalanceAsofPlanYear != 0 && ldtNetInvestmentFromDate != DateTime.MinValue && (lintBalanceAsofPlanYear > ldtNetInvestmentFromDate.Year))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5443, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate < ldtNetInvestmentFromDate))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5442, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    //PIR - 1015
                    if ((ldtNetInvestmentFromDate != DateTime.MinValue && ldtNetInvestmentToDate == DateTime.MinValue)||(ldtNetInvestmentToDate != DateTime.MinValue && ldtNetInvestmentFromDate == DateTime.MinValue))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6275, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }

                    if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 3 && ldtNetInvestmentToDate.Day != 31))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5445, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 6 && ldtNetInvestmentToDate.Day != 30))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5445, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 9 && ldtNetInvestmentToDate.Day != 30))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5445, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 12 && ldtNetInvestmentToDate.Day != 31))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5445, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    else if (ldtNetInvestmentToDate != DateTime.MinValue && (ldtNetInvestmentToDate.Month == 1 || ldtNetInvestmentToDate.Month == 2 ||
                        ldtNetInvestmentToDate.Month == 4 || ldtNetInvestmentToDate.Month == 5 || ldtNetInvestmentToDate.Month == 7 || ldtNetInvestmentToDate.Month == 8 ||
                        ldtNetInvestmentToDate.Month == 10 || ldtNetInvestmentToDate.Month == 11))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5445, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }

                #region PIR 679 fix
                if (lstrtDroModel == busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT || lstrtDroModel == busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                {
                    if (Convert.ToString(ahstParams["icdoDroBenefitDetails.istrBenefitOptionValue"]) != busConstant.LUMP_SUM &&
                    Convert.ToString(ahstParams["icdoDroBenefitDetails.istrBenefitOptionValue"]) != busConstant.LIFE_ANNUTIY)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6191, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }

                    if (ldecWithheldPercent != 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6192, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }

                    if (ldecPercentage != 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6193, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (ldecPercentage != 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6193, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (lstrAltPayeeInc == busConstant.FLAG_YES)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6194, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (lstrAltPayeeEarlyRet == busConstant.FLAG_YES)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6195, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    if (ldecAmount == 0 && ldecBenefitFlatPerc == 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6196, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }
                #endregion
            }
            else
            {
                busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitApplication.ibusPerson = this.ibusParticipant;

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID || lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID)
                    {
                        string lstrPlanCode = string.Empty;
                        if (lintPlanID == busConstant.MPIPP_PLAN_ID)
                        {
                            lstrPlanCode = busConstant.MPIPP;
                        }
                        else
                        {
                            lstrPlanCode = busConstant.IAP;
                        }

                        if (!lbusBenefitApplication.CheckAlreadyVested(lstrPlanCode) && string.IsNullOrEmpty(lbusDroBenefitDetails.istrSubPlan))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5437, "");
                            larrErrors.Add(lobjError);
                            break;
                        }
                    }

                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase == busConstant.FLAG_YES &&
                        lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_increase != busConstant.FLAG_YES)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6068, "");
                        larrErrors.Add(lobjError);
                        break;
                    }


                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                       lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue != busConstant.LUMP_SUM)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5465, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }

                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id != busConstant.IAP_PLAN_ID &&
                   (lbusDroBenefitDetails.icdoDroBenefitDetails.balance_as_of_plan_year != 0 || lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_benefit_cap_year != 0 ||
                   lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date != DateTime.MinValue ||
                   lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(6204, "");
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                    else if (lintPlanID == busConstant.IAP_PLAN_ID)
                    {
                        //PIR - 1015
                        if ((lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date != DateTime.MinValue &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date == DateTime.MinValue) ||
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date == DateTime.MinValue &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue)
                          )
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6275, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }

                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date.Day != 1 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date.Month != 1))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5444, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.balance_as_of_plan_year != 0 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.balance_as_of_plan_year >
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date.Year))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5443, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date <
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_from_date))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5442, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }

                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 3 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Day != 31))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5445, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 6 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Day != 30))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5445, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 9 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Day != 30))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5445, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 12 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Day != 31))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5445, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date != DateTime.MinValue &&
                            (lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 1 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 2 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 4 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 5 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 7 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 8 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 10 ||
                            lbusDroBenefitDetails.icdoDroBenefitDetails.net_investment_to_date.Month == 11))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(5445, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }

                    #region PIR 679 fix

                    if (lstrtDroModel == busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT || lstrtDroModel == busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue != busConstant.LUMP_SUM &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue != busConstant.LIFE_ANNUTIY)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6191, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }

                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc != 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6192, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }

                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_perc != 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6193, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_increase == busConstant.FLAG_YES)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6194, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_early_ret == busConstant.FLAG_YES)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6195, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_flat_perc == 0 &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_amt == 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(6196, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }
                    #endregion
                }

            }

            //PIR 591 : Shared Interest could be a combination of BROWN + Flat Percentage + Flat Amount.

            //if (lstrtDroModel == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA && ldecBenefitFlatPerc == 0)
            //  {
            //      utlError lobjError = null;
            //      lobjError = AddError(2021, "");
            //      larrErrors.Add(lobjError);
            //      return larrErrors;
            //  }

            //if (lstrtDroModel == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA && ldecPercentage != 0)
            //{
            //    utlError lobjError = null;
            //    lobjError = AddError(2022, "");
            //    larrErrors.Add(lobjError);
            //    return larrErrors;
            //}  



            if (blnOnADD)
            {
                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if ((lbusDroBenefitDetails.istrSubPlan).IsNullOrEmpty())
                        lbusDroBenefitDetails.istrSubPlan = null;

                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == lintPlanID && lbusDroBenefitDetails.istrSubPlan == lstrSubPlan)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(2009, "");
                        larrErrors.Add(lobjError);
                        break;
                    }

                    if (ldecPercentage > 100)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1121, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }
            else if (CheckDuplicatePlan())
            {
                utlError lobjError = null;
                lobjError = AddError(2009, "");
                larrErrors.Add(lobjError);
            }
            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbOtherQLFDDroBenefitDetails)
            {
                //if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == Convert.ToInt32(ahstParams["icdoDroBenefitDetails.plan_id"]) 
                //     && lbusDroBenefitDetails.istrSubPlan == lstrSubPlan)
                //{
                //    utlError lobjError = null;
                //    lobjError = AddError(2009, "");
                //    larrErrors.Add(lobjError);
                //    break;
                //}
                if (lstrtDroModel == "SPDQ")
                {
                    utlError lobjError = null;
                    lobjError = AddError(1165, "");
                    larrErrors.Add(lobjError);
                }
            }
            if (ldecAmount != 0 || ldecPercentage != 0)
            {
                if (IsNegative(ldecAmount.ToString()) || IsNegative(ldecPercentage.ToString()))
                {
                    utlError lobjError = null;
                    lobjError = AddError(2020, "");
                    larrErrors.Add(lobjError);

                }
                if (ldecPercentage > 100)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1121, "");
                    larrErrors.Add(lobjError);
                }
            }

            if (!blnOnADD)
            {
                if (this.icdoDroApplication.date_of_divorce == DateTime.MinValue && this.icdoDroApplication.dro_status_value == busConstant.DRO_APPROVED)
                {
                    bool lblnTempFlag = true;

                    foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(1105, "");
                            larrErrors.Add(lobjError);
                            lblnTempFlag = false;
                            break;
                        }
                    }
                    if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1105, "");
                        larrErrors.Add(lobjError);
                    }
                }
                if (this.icdoDroApplication.date_of_marriage == DateTime.MinValue && this.icdoDroApplication.dro_status_value == busConstant.DRO_APPROVED)
                {
                    bool lblnTempFlag = true;

                    foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                            lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(1106, "");
                            larrErrors.Add(lobjError);
                            lblnTempFlag = false;
                            break;
                        }
                    }
                    if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1106, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }

            return larrErrors;
        }

        public bool CheckDuplicatePlan()
        {
            ArrayList larrItems = new ArrayList();
            ArrayList larrSubPlans = new ArrayList();
            bool lblnCheckDuplicate = false;
            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {
                int lintPlanID = lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id;

                if ((lbusDroBenefitDetails.istrSubPlan).IsNullOrEmpty())
                    lbusDroBenefitDetails.istrSubPlan = null;

                string lstrSubPlan = lbusDroBenefitDetails.istrSubPlan;
                if (!larrItems.Contains(lintPlanID) || !larrSubPlans.Contains(lstrSubPlan))
                {
                    larrItems.Add(lintPlanID);
                    larrSubPlans.Add(lstrSubPlan);
                }
                else
                {
                    return true;
                }
            }
            return lblnCheckDuplicate;
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

        public Collection<cdoDroModel> GetDroModelsForPerson()
        {
            Collection<cdoDroModel> lclcDroModels = null;
            DataTable ldtbResult = busBase.Select("cdoDroModel.GetDroModelRelatedtoPersonAcc", new object[1] { this.icdoDroApplication.person_id });
            lclcDroModels = doBase.GetCollection<cdoDroModel>(ldtbResult);
            return lclcDroModels;
        }

        public bool IsQualifiedDateMoreThanCurrentDate()
        {
            if (this.icdoDroApplication.qualified_date > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public bool IsDROReceivedDateMoreThanCurrentDate()
        {
            if (this.icdoDroApplication.received_date > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public bool IsEntryOfOrderDateMoreThanCurrentDate()
        {
            if (this.icdoDroApplication.order_date > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public Collection<cdoPlan> GetPlansBasedonDroModel(string astrDroModelValue)
        {
            Collection<cdoPlan> lclcPlans = null;
            DataTable ldtbResult = busBase.Select("cdoPlan.GetPlanRelatedtoDroandPersonAcc", new object[2] { this.icdoDroApplication.person_id, astrDroModelValue });
            lclcPlans = doBase.GetCollection<cdoPlan>(ldtbResult);
            return lclcPlans;
        }


        public void UpdateStatusHistoryValue()
        {
            busDroApplicationStatusHistory lbusDroApplicationStatusHistory = new busDroApplicationStatusHistory();
            lbusDroApplicationStatusHistory.InsertStatusHistory(icdoDroApplication.dro_application_id, icdoDroApplication.dro_status_value,
               icdoDroApplication.modified_date, icdoDroApplication.modified_by);

        }

        public ArrayList btnQualified_Clicked()
        {
            ArrayList larrList = new ArrayList();
            bool lblnflag = true;

            #region Conditions To Handle HardErrors

            //wasim - SSN is required.
            if (this.ibusAlternatePayee.icdoPerson.ssn.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(6198, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.ibusParticipant.icdoPerson.ssn.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(6197, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.order_date == DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(5174, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.qualified_date == DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(1112, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.case_number == null)
            {
                utlError lobjError = null;
                lobjError = AddError(1104, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.date_of_divorce == DateTime.MinValue)
            {
                bool lblnTempFlag = true;

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                        lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1105, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        lblnTempFlag = false;
                        break;
                    }
                }
                if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1105, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }
            if (this.icdoDroApplication.date_of_marriage == DateTime.MinValue)
            {
                bool lblnTempFlag = true;

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                        lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1106, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        lblnTempFlag = false;
                        break;
                    }
                }
                if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1106, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }
            if (this.icdoDroApplication.received_date == DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(1107, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.dro_commencement_date == DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(1180, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }


            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {
                if (lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_perc > 100)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1121, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }

            if (this.icdoDroApplication.qualified_date < this.icdoDroApplication.date_of_marriage && this.icdoDroApplication.qualified_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2010, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }


            if (this.icdoDroApplication.qualified_date < this.icdoDroApplication.date_of_divorce && this.icdoDroApplication.qualified_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2011, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            // PIR-362
            //if (this.icdoDroApplication.qualified_date < this.icdoDroApplication.received_date && this.icdoDroApplication.qualified_date != DateTime.MinValue)
            //{
            //    utlError lobjError = null;
            //    lobjError = AddError(2012, "");
            //    larrList.Add(lobjError);
            //    lblnflag = false;
            //}

            if (this.icdoDroApplication.qualified_date > DateTime.Now)
            {
                utlError lobjError = null;
                lobjError = AddError(2014, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (!IsDateOfSeparationValid())
            {
                utlError lobjError = null;
                lobjError = AddError(1119, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (!IsEmailValid())
            {
                utlError lobjError = null;
                lobjError = AddError(1118, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.iclbDroBenefitDetails.Count == 0)
            {
                utlError lobjError = null;
                lobjError = AddError(2007, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            //if (this.iclbAttorney.Count == 0) rohan
            //{
            //    utlError lobjError = null;
            //    lobjError = AddError(2017, "");
            //    larrList.Add(lobjError);
            //    lblnflag = false;
            //}

            if (this.icdoDroApplication.date_of_divorce > this.icdoDroApplication.received_date && this.icdoDroApplication.date_of_divorce != DateTime.MinValue
                && this.icdoDroApplication.received_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2015, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.date_of_divorce > this.icdoDroApplication.joinder_recv_date && this.icdoDroApplication.date_of_divorce != DateTime.MinValue
                && this.icdoDroApplication.joinder_recv_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2016, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (IsDROReceivedDateMoreThanCurrentDate())
            {
                utlError lobjError = null;
                lobjError = AddError(2019, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {
                if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                {
                    DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoDroApplication.person_id, lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id });
                    //if (ldtblPayeeAccounts.Rows.Count > 0)
                    //{
                    //    if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                    //    {
                    //        utlError lobjError = null;
                    //        lobjError = AddError(5510, "");
                    //        larrList.Add(lobjError);
                    //        lblnflag = false;
                    //        break;
                    //    }
                    //}


                    if (ldtblPayeeAccounts.Rows.Count > 0 && Convert.ToString(ldtblPayeeAccounts.Rows[0][enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        if (this.icdoDroApplication.is_participant_disabled_flag != busConstant.FLAG_YES)
                        {

                            utlError lobjError = null;
                            lobjError = AddError(5497, "");
                            larrList.Add(lobjError);
                            lblnflag = false;
                            break;
                        }
                    }
                }
            }

            #endregion

            if (lblnflag)
            {
                //TO-DO -- CHECK WITH SIDDARTHA, I DON'T THINK WE NEED THIS CONDITION ANYMORE 
                //if (ibusBaseActivityInstance != null)
                //{
                //    int lintCountForExistingNonNonCompletedDROsForPerson =(int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountforExistingNonCompletedDROsforPerson", new object[2] { this.icdoDroApplication.person_id,this.icdoDroApplication.dro_application_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                //    if (lintCountForExistingNonNonCompletedDROsForPerson == 0)
                //    {
                //        busWorkflowHelper.UpdateWorkflowActivityByEvent(ibusBaseActivityInstance as busActivityInstance, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                //    }
                //}
                if (this.icdoDroApplication.is_ammended_flag.IsNotNullOrEmpty() && this.icdoDroApplication.is_ammended_flag.Equals("Y"))
                {
                    this.icdoDroApplication.dro_status_id = 6005;
                    this.icdoDroApplication.dro_status_value = "AMQL";
                    this.icdoDroApplication.Update();
                    this.UpdateStatusHistoryValue();
                }
                else
                {
                    this.icdoDroApplication.dro_status_id = 6005;
                    this.icdoDroApplication.dro_status_value = "QLFD";
                    this.icdoDroApplication.Update();
                    this.UpdateStatusHistoryValue();

                    busRelationship lbusRelationship = new busRelationship();
                    lbusRelationship = lbusRelationship.CheckIfBeneficiaryIsSpouse(icdoDroApplication.person_id, icdoDroApplication.alternate_payee_id);

                    if (lbusRelationship != null)
                    {
                        lbusRelationship.icdoRelationship.relationship_value = busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE;
                        lbusRelationship.icdoRelationship.Update();
                    }
                }

                this.EvaluateInitialLoadRules(utlPageMode.Update); //Needed to evaluate Rules again for visibility and invisiblity of DRO buttons   
                if (this.iclbDroBenefitDetails.Count() > 0 && this.iclbDroBenefitDetails != null)
                {
                    foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                    {
                        lbusDroBenefitDetails.EvaluateInitialLoadRules();
                    }
                }
                larrList.Add(this);
                return larrList;
            }

            return larrList;
        }

        public ArrayList btnApproved_Clicked()
        {
            ArrayList larrList = new ArrayList();
            bool lblnflag = true;

            #region Conditions To Handle HardErrors

            if (this.icdoDroApplication.case_number == null)
            {
                utlError lobjError = null;
                lobjError = AddError(1104, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.date_of_divorce == DateTime.MinValue)
            {
                bool lblnTempFlag = true;

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                        lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1105, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        lblnTempFlag = false;
                        break;
                    }
                }
                if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1105, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }
            if (this.icdoDroApplication.date_of_marriage == DateTime.MinValue)
            {
                bool lblnTempFlag = true;

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_CHILD_SUPPORT &&
                        lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value != busConstant.DRO_MODEL_VALUE_SPOUSAL_SUPPORT)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1106, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        lblnTempFlag = false;
                        break;
                    }
                }
                if (lblnTempFlag && this.iclbDroBenefitDetails.Count <= 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1106, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }
            if (this.icdoDroApplication.received_date == DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(1107, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {
                if (lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_perc > 100)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1121, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }

                if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(6181, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                }
            }

            if (this.iclbDroBenefitDetails.Count == 0)
            {
                utlError lobjError = null;
                lobjError = AddError(2007, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            //if (this.iclbAttorney.Count == 0) rohan
            //{
            //    utlError lobjError = null;
            //    lobjError = AddError(2017, "");
            //    larrList.Add(lobjError);
            //    lblnflag = false;
            //}

            if (!IsDateOfSeparationValid())
            {
                utlError lobjError = null;
                lobjError = AddError(1119, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (!IsEmailValid())
            {
                utlError lobjError = null;
                lobjError = AddError(1118, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }


            if (this.icdoDroApplication.date_of_divorce > this.icdoDroApplication.received_date && this.icdoDroApplication.date_of_divorce != DateTime.MinValue
             && this.icdoDroApplication.received_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2015, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (this.icdoDroApplication.date_of_divorce > this.icdoDroApplication.joinder_recv_date && this.icdoDroApplication.date_of_divorce != DateTime.MinValue
                && this.icdoDroApplication.joinder_recv_date != DateTime.MinValue)
            {
                utlError lobjError = null;
                lobjError = AddError(2016, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }

            if (IsDROReceivedDateMoreThanCurrentDate())
            {
                utlError lobjError = null;
                lobjError = AddError(2019, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }
            #endregion

            if (lblnflag)
            {
                this.icdoDroApplication.approved_by_user = iobjPassInfo.istrUserID;
                this.icdoDroApplication.approved_date = DateTime.Now;
                this.icdoDroApplication.dro_status_id = 6005;
                this.icdoDroApplication.dro_status_value = "APRD";
                this.icdoDroApplication.Update();
                this.UpdateStatusHistoryValue();
                this.EvaluateInitialLoadRules(utlPageMode.Update);
                larrList.Add(this);
                return larrList;
            }

            return larrList;
        }

        public bool IsJoinderOnFileCheckBoxNotChecked()
        {
            if (this.icdoDroApplication.joinder_recv_date != DateTime.MinValue && this.icdoDroApplication.joinder_on_file == "N")
            {
                return true;
            }
            return false;
        }

        public bool IsjoinderRecvNullWhnJoinderOnFileChecked()
        {
            if (this.icdoDroApplication.joinder_on_file == "Y" && this.icdoDroApplication.joinder_recv_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }


        public ArrayList btnCancelled_Clicked()
        {
            ArrayList larrList = new ArrayList();
            bool lblnflag = true;


            //PIR 924  --this validation was already done for busBenefitApplication under PIR 792
            //if (!(string.IsNullOrEmpty(this.icdoDroApplication.cancellation_reason)))
            //{


            object lobjCheckDistributionStatusValue = null;
            int lintCheckDistributionStatusValue = 0;
             lobjCheckDistributionStatusValue = DBFunction.DBExecuteScalar("cdoDroApplication.GetPaymentCountForDROApplication", new object[1] { this.icdoDroApplication.dro_application_id },
                                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lobjCheckDistributionStatusValue != null)
            {
                lintCheckDistributionStatusValue = ((int)lobjCheckDistributionStatusValue);
            }

            if (lintCheckDistributionStatusValue > 0)
            {
                utlError lobjError = AddError(6284, "");
                larrList.Add(lobjError);
                return larrList;
            }
            //}

            if (this.icdoDroApplication.cancellation_reason == "" || this.icdoDroApplication.cancellation_reason == null)
            {
                utlError lobjError = null;
                lobjError = AddError(5057, "");
                larrList.Add(lobjError);
                lblnflag = false;
            }
            //else if (this.icdoDroApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_DECEASED)
            //{
            //    if (this.ibusParticipant.icdoPerson.date_of_death == DateTime.MinValue)
            //    {
            //        utlError lobjError = AddError(5158, "");
            //        larrList.Add(lobjError);
            //        lblnflag = false;
            //    }
            //}
           
            if (lblnflag)
            {

                this.icdoDroApplication.dro_status_id = 6005;
                this.icdoDroApplication.dro_status_value = "CNLD";
                this.icdoDroApplication.Update();
                this.UpdateStatusHistoryValue();

                #region Cancal All QDRO Calculations For this QDRO Application
                DataTable ldtblQDROCalculations = new DataTable();
                Collection<busQdroCalculationHeader> lclbbusQdroCalculationHeader = new Collection<busQdroCalculationHeader>();
                ldtblQDROCalculations = Select("cdoDroApplication.GetCalculationsForQDRO", new object[1] { this.icdoDroApplication.dro_application_id });
                lclbbusQdroCalculationHeader = GetCollection<busQdroCalculationHeader>(ldtblQDROCalculations, "icdoQdroCalculationHeader");

                if (lclbbusQdroCalculationHeader.Count > 0)
                {
                    foreach (busQdroCalculationHeader lbusQdroCalculationHeader in lclbbusQdroCalculationHeader)
                    {
                        lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                        lbusQdroCalculationHeader.icdoQdroCalculationHeader.Update();
                    }
                }
                #endregion Cancal All QDRO Calculations For this QDRO Application

                //TODO: WE might need here to Check that even if they come via App screens we need to complete a Corresponding WorkFlow if there exist one. 
                //That check can be easily added and done.
                if (ibusBaseActivityInstance != null)
                {
                    int lintCountForExistingNonNonCompletedDROsForPerson = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountforExistingNonCompletedDROsforPerson", new object[1] { this.icdoDroApplication.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCountForExistingNonNonCompletedDROsForPerson > 0)
                    {
                        busWorkflowHelper.UpdateWorkflowActivityByEvent(ibusBaseActivityInstance as busActivityInstance, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                    }
                }

                DataTable ldtblOpenWithholdingInformation = Select("cdoDroApplication.CheckOpenWithholdingInformation", new object[1] { icdoDroApplication.person_id });
                if (ldtblOpenWithholdingInformation != null && ldtblOpenWithholdingInformation.Rows.Count > 0)
                {
                    foreach (DataRow ldrOpenWithholdingInformation in ldtblOpenWithholdingInformation.Rows)
                    {
                        if (Convert.ToString(ldrOpenWithholdingInformation[enmPayeeAccount.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                            busWorkflowHelper.InitializeWorkflow(busConstant.UPDATE_PAYEE_ACCOUNT, this.icdoDroApplication.person_id, 0,
                                           Convert.ToInt32(ldrOpenWithholdingInformation[enmPayeeAccount.payee_account_id.ToString().ToUpper()]), null);
                    }
                }

                this.EvaluateInitialLoadRules(utlPageMode.Update);
                larrList.Add(this);

                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    lbusDroBenefitDetails.EvaluateInitialLoadRules();
                }

                return larrList;
            }

            // For Cancel Payee Account Status..
            this.LoadPayeeAccountStatusByApplicationID(this.icdoDroApplication.dro_application_id);
            foreach (busPayeeAccountStatus lobjPayeeAccountStatus in this.iclbPayeeAccountStatusByApplication)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
            }

            return larrList;
        }

        public void LoadPayeeAccountStatusByApplicationID(int aintApplicationID)
        {
            DataTable ldtbList = busBase.Select("cdoPayeeAccount.GetPayeeAccountStatusByDROApplicationID", new object[1] { aintApplicationID });
            iclbPayeeAccountStatusByApplication = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
        }


        public bool ShowConvertToDisabilityButton()
        {
            bool lblnShowConvertToDisabilityButton = false;

            if (icdoDroApplication.dro_status_value == busConstant.DRO_QUALIFIED)
            {
                if (iclbDroBenefitDetails != null && iclbDroBenefitDetails.Count > 0)
                {

                    //Shared DRO
                    foreach (busDroBenefitDetails lbusDroBenefitDetails in iclbDroBenefitDetails)
                    {
                        DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoDroApplication.person_id, lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id });
                        DataRow[] ldrDisabilityPayeeAccount;
                        ldrDisabilityPayeeAccount = ldtblPayeeAccounts.FilterTable(utlDataType.String, enmPayeeAccount.benefit_account_type_value.ToString().ToUpper(), busConstant.BENEFIT_TYPE_DISABILITY);

                        if (ldrDisabilityPayeeAccount != null && ldrDisabilityPayeeAccount.Count() > 0)
                        {
                            int lintSharedInterestDROCalc = (int)DBFunction.DBExecuteScalar("cdoDroApplication.ShowConvertDisabilityBenefitButton", new object[3] {this.icdoDroApplication.dro_application_id,
                            lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id , Convert.ToInt32(ldrDisabilityPayeeAccount[0][enmPayeeAccount.payee_account_id.ToString().ToUpper()])},
                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                            if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA && lintSharedInterestDROCalc == 0)
                            {
                                return true;
                            }
                        }
                    }

                    //Separate DRO
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoDroApplication.ShowConvertToDisabilityBenefitForSeparateInt", new object[1] { this.icdoDroApplication.person_id },
                                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (lintCount > 0 && (icdoDroApplication.is_disability_conversion != busConstant.FLAG_YES || icdoDroApplication.is_disability_conversion == ""))
                        return true;

                }
            }

            return lblnShowConvertToDisabilityButton;
        }

        public ArrayList btnConvertToDisabilityBenefits_Clicked()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = null;

            icdoDroApplication.Update();

            if (this.icdoDroApplication.waived_disability_entitlement_flag == busConstant.FLAG_YES)
            {
                lobjError = AddError(0, "Alternate Payee has Waived Disability Benefits");
                larrList.Add(lobjError);
                return larrList;
            }

            if (this.icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
            {
                if (iclbDroBenefitDetails != null && iclbDroBenefitDetails.Count > 0)
                {

                    foreach (busDroBenefitDetails lbusDroBenefitDetails in iclbDroBenefitDetails)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id != busConstant.IAP_PLAN_ID)
                        {
                            #region Cancel All QDRO Calculations For this QDRO Application and Payee Account

                            DataTable ldtblQDROCalculations = new DataTable();
                            Collection<busQdroCalculationHeader> lclbbusQdroCalculationHeader = new Collection<busQdroCalculationHeader>();
                            ldtblQDROCalculations = Select("cdoDroApplication.GetQDROCalculationFromDROAppDetailId", new object[1] { lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id });

                            if (ldtblQDROCalculations != null && ldtblQDROCalculations.Rows.Count > 0)
                                lclbbusQdroCalculationHeader = GetCollection<busQdroCalculationHeader>(ldtblQDROCalculations, "icdoQdroCalculationHeader");

                            if (lclbbusQdroCalculationHeader.Count > 0)
                            {
                                foreach (busQdroCalculationHeader lbusQdroCalculationHeader in lclbbusQdroCalculationHeader)
                                {
                                    lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_CANCELLED;
                                    lbusQdroCalculationHeader.icdoQdroCalculationHeader.Update();
                                }
                            }

                            // For Cancel Payee Account Status..
                            DataTable ldtbList = busBase.Select("cdoDroApplication.GetQDROPayeeAccountFromDROAppDetailId", new object[1] { lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id });
                            iclbPayeeAccountStatusByApplication = new Collection<busPayeeAccountStatus>();
                            if (ldtbList != null && ldtbList.Rows.Count > 0)
                            {
                                iclbPayeeAccountStatusByApplication = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
                            }

                            foreach (busPayeeAccountStatus lobjPayeeAccountStatus in this.iclbPayeeAccountStatusByApplication)
                            {
                                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
                            }

                            icdoDroApplication.is_disability_conversion = busConstant.FLAG_YES;

                            #endregion Cancel All QDRO Calculations For this QDRO Application and Payee Account

                            larrList = btn_CalculateBenefitClick(lbusDroBenefitDetails.icdoDroBenefitDetails.dro_benefit_id);
                        }
                    }

                }
            }
            else
            {

                lobjError = AddError(0, "Eligible for Participants Disability Benefit is unchecked");
                larrList.Add(lobjError);
                return larrList;
            }

            EvaluateInitialLoadRules();

            return larrList;
        }


        public ArrayList btn_CalculateBenefitClick(int aintDroBenefitId)
        {
            bool lblnParticipantPayeeAccount = busConstant.BOOL_FALSE;
            busPayeeAccount lbusPayeeAccount = null;

            decimal ldecParticipantAmount = new decimal();

            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            if (icdoDroApplication.dro_commencement_date == DateTime.MinValue)
            {
                lobjError = AddError(5410, string.Empty);
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            else if (icdoDroApplication.dro_commencement_date != DateTime.MinValue &&
                  icdoDroApplication.dro_commencement_date.Day != 1)
            {
                lobjError = AddError(6180, string.Empty);
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            else
            {
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.addr_state_value.ToString()]) != this.icdoDroApplication.addr_state_value)
                {
                    this.icdoDroApplication.addr_state_value = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.addr_state_value.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.case_number.ToString()]) != this.icdoDroApplication.case_number)
                {
                    this.icdoDroApplication.case_number = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.case_number.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.joinder_recv_date.ToString()]) != this.icdoDroApplication.joinder_recv_date)
                {
                    this.icdoDroApplication.joinder_recv_date = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.joinder_recv_date.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.order_date.ToString()]) != this.icdoDroApplication.order_date)
                {
                    this.icdoDroApplication.order_date = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.order_date.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.received_date.ToString()]) != this.icdoDroApplication.received_date)
                {
                    this.icdoDroApplication.received_date = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.received_date.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.date_of_marriage.ToString()]) != this.icdoDroApplication.date_of_marriage)
                {
                    this.icdoDroApplication.date_of_marriage = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.date_of_marriage.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.date_of_divorce.ToString()]) != this.icdoDroApplication.date_of_divorce)
                {
                    this.icdoDroApplication.date_of_divorce = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.date_of_divorce.ToString()]);
                }
                if (Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.qualified_date.ToString()]) != this.icdoDroApplication.qualified_date)
                {
                    this.icdoDroApplication.qualified_date = Convert.ToDateTime(icdoDroApplication.ihstOldValues[enmDroApplication.qualified_date.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.joinder_on_file.ToString()]) != this.icdoDroApplication.joinder_on_file)
                {
                    this.icdoDroApplication.joinder_on_file = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.joinder_on_file.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_ammended_flag.ToString()]) != this.icdoDroApplication.is_ammended_flag)
                {
                    this.icdoDroApplication.is_ammended_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_ammended_flag.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.cancellation_reason.ToString()]) != this.icdoDroApplication.cancellation_reason)
                {
                    this.icdoDroApplication.cancellation_reason = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.cancellation_reason.ToString()]);
                }
                //if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_participant_disabled_flag.ToString()]) != this.icdoDroApplication.is_participant_disabled_flag)
                //{
                //    this.icdoDroApplication.is_participant_disabled_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_participant_disabled_flag.ToString()]);
                //}
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.life_conversion_factor_flag.ToString()]) != this.icdoDroApplication.life_conversion_factor_flag)
                {
                    this.icdoDroApplication.life_conversion_factor_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.life_conversion_factor_flag.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.waived_disability_entitlement_flag.ToString()]) != this.icdoDroApplication.waived_disability_entitlement_flag)
                {
                    this.icdoDroApplication.waived_disability_entitlement_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.waived_disability_entitlement_flag.ToString()]);
                }
                if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.eligible_for_continuance_flag.ToString()]) != this.icdoDroApplication.eligible_for_continuance_flag)
                {
                    this.icdoDroApplication.eligible_for_continuance_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.eligible_for_continuance_flag.ToString()]);
                }
                //if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]) != this.icdoDroApplication.is_alt_payee_eligible_for_participant_retiree_increase)
                //{
                //    this.icdoDroApplication.is_alt_payee_eligible_for_participant_retiree_increase = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.is_alt_payee_eligible_for_participant_retiree_increase.ToString()]);
                //}
                //if (Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.alt_payee_retiree_increase_flag.ToString()]) != this.icdoDroApplication.alt_payee_retiree_increase_flag)
                //{
                //    this.icdoDroApplication.alt_payee_retiree_increase_flag = Convert.ToString(icdoDroApplication.ihstOldValues[enmDroApplication.alt_payee_retiree_increase_flag.ToString()]);
                //}               

                icdoDroApplication.Update();

                int lintBenefitId =
                    iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.plan_benefit_id;
                string lstrPlanCode =
                    iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.istrPlanCode;
                int lintPlanId =
                    iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.plan_id;

                //if (lintPlanId == busConstant.IAP_PLAN_ID && iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).Count() > 0 &&
                //         iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                //{
                //    lobjError = AddError(5503, string.Empty);
                //    larrErrors.Add(lobjError);
                //    return larrErrors;
                //}
                if (lintPlanId == busConstant.IAP_PLAN_ID)
                {
                    //PROD PIR 253 
                    DataTable lintIsIapBalancePaidOut = Select("cdoDroApplication.CheckIAPBalancePaidOut", new object[1] { icdoDroApplication.person_id });
                    if (lintIsIapBalancePaidOut != null && lintIsIapBalancePaidOut.Rows.Count > 0 && Convert.ToString(lintIsIapBalancePaidOut.Rows[0][0]).IsNotNullOrEmpty()
                        && Convert.ToDecimal(lintIsIapBalancePaidOut.Rows[0][0]) <= 0)
                    {
                        lobjError = AddError(5504, string.Empty);
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }
                }


                DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { this.icdoDroApplication.person_id, lintPlanId });

                ////Do not delete
                //if (iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).Count() > 0 &&
                //                 iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                //{
                //    if (icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
                //    {
                //        if (ldtblPayeeAccounts == null || (ldtblPayeeAccounts != null && ldtblPayeeAccounts.Rows.Count <= 0))
                //        {
                //            lobjError = AddError(6182, "");
                //            larrErrors.Add(lobjError);
                //            return larrErrors;
                //        }
                //        else
                //        {
                //            DataRow[] ldrDisabilityPayeeAccount;
                //            DataTable ldtDisabilityPayeeAccount = null;

                //            ldrDisabilityPayeeAccount = ldtblPayeeAccounts.FilterTable(utlDataType.String, enmPayeeAccount.benefit_account_type_value.ToString().ToUpper(), busConstant.BENEFIT_TYPE_DISABILITY);

                //            if (ldrDisabilityPayeeAccount != null && ldrDisabilityPayeeAccount.Count() > 0)
                //            {
                //                ldtDisabilityPayeeAccount = ldrDisabilityPayeeAccount.CopyToDataTable();
                //            }

                //            if (ldtDisabilityPayeeAccount == null || (ldtDisabilityPayeeAccount != null && ldtDisabilityPayeeAccount.Rows.Count <= 0))
                //            {
                //                lobjError = AddError(6182, "");
                //                larrErrors.Add(lobjError);
                //                return larrErrors;
                //            }
                //        }
                //    }
                //}

                if (ldtblPayeeAccounts.Rows.Count > 0)
                {
                    if (iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).Count() > 0 &&
                                  iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
                    {
                        iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblPayeeAccounts, "icdoPayeeAccount");
                        lblnParticipantPayeeAccount = busConstant.BOOL_TRUE;
                        lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        if (iclbPayeeAccount.Count() > 0)
                        {
                            lbusPayeeAccount = iclbPayeeAccount.FirstOrDefault();
                            lbusPayeeAccount.LoadBenefitDetails();

                            //if (icdoDroApplication.dro_commencement_date > lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate
                            //    && icdoDroApplication.waived_disability_entitlement_flag != busConstant.FLAG_YES)
                            //{
                            //    lobjError = AddError(5511, string.Empty);
                            //    larrErrors.Add(lobjError);
                            //    return larrErrors;
                            //}

                            if (lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)
                            {
                                if (this.icdoDroApplication.is_participant_disabled_flag != busConstant.FLAG_YES)
                                {
                                    lobjError = AddError(5497, string.Empty);
                                    larrErrors.Add(lobjError);
                                    return larrErrors;
                                }
                            }
                            //Conversion
                            lbusPayeeAccount.LoadNextBenefitPaymentDate();
                            DataTable ldtblParticipantAmount = busBase.Select("cdoQdroCalculationHeader.GetBenefitAmount", new object[] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                        lbusPayeeAccount.idtNextBenefitPaymentDate});
                            if (ldtblParticipantAmount.Rows.Count > 0)
                            {
                                ldecParticipantAmount = Convert.ToDecimal(ldtblParticipantAmount.Rows[0][0]);
                            }
                        }
                    }
                    //Need to check
                    //else
                    //{
                    //    lobjError = AddError(5510, "");
                    //    larrErrors.Add(lobjError);
                    //    return larrErrors;
                    //}
                }



                busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };

                //Post Retirement DRO
                if (lblnParticipantPayeeAccount)
                {
                    lbusQdroCalculationHeader.iblnParticipantPayeeAccount = lblnParticipantPayeeAccount;
                    lbusQdroCalculationHeader.idecParticipantAmount = ldecParticipantAmount;
                    lbusQdroCalculationHeader.iclbParticipantsPayeeAccount = new Collection<busPayeeAccount>();
                    lbusQdroCalculationHeader.iclbParticipantsPayeeAccount.Add(lbusPayeeAccount);
                }

                lbusQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusQdroCalculationHeader.ibusQdroApplication = this;
                lbusQdroCalculationHeader.ibusAlternatePayee = ibusAlternatePayee;

                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = ibusParticipant;
                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.LoadPersonAccounts();
                lbusQdroCalculationHeader.ibusParticipant = ibusParticipant;
                lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount = this.ibusParticipant.iclbPersonAccount;
                //lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = icdoDroApplication.dro_commencement_date;
                // PIR-507 For Final Calculation
                DateTime ldtLastWoringDayOfParticipant = lbusQdroCalculationHeader.ibusCalculation.GetLastWorkingDate(this.ibusParticipant.icdoPerson.ssn);
                string lstrdromodelvalue = iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.dro_model_value;
                if (!string.IsNullOrEmpty(lstrdromodelvalue) && lstrdromodelvalue == "STAF")
                {
                    lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = icdoDroApplication.dro_commencement_date > ldtLastWoringDayOfParticipant ? DateTime.Now : icdoDroApplication.dro_commencement_date;
                }
                else
                {
                    lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = icdoDroApplication.dro_commencement_date;
                }

                lbusQdroCalculationHeader.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
                lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                lbusQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();


                if (!this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution =
                        lbusQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(ibusParticipant.icdoPerson.person_id, this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = lbusQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(ibusParticipant.icdoPerson.person_id, null);
                }

                lbusQdroCalculationHeader.PopulateInitialDataQdroCalculationHeader(
                        ibusParticipant.icdoPerson.person_id, this.icdoDroApplication.dro_application_id,
                        ibusAlternatePayee, this.icdoDroApplication.dro_commencement_date,
                        lintPlanId, icdoDroApplication.date_of_marriage, icdoDroApplication.date_of_divorce, icdoDroApplication.is_participant_disabled_flag, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL);

                lbusQdroCalculationHeader.ibusBenefitApplication.idecAge = lbusQdroCalculationHeader.icdoQdroCalculationHeader.age;

                //Post Retirement DRO
                //if (!lblnParticipantPayeeAccount)               
                //{
                lbusQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                lbusQdroCalculationHeader.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();
                //}

                #region If participant disabled

                if (icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
                {
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = lbusQdroCalculationHeader.ibusParticipant;
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount;

                    //Post Retirement DRO
                    //if (!lblnParticipantPayeeAccount)
                    //{
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_MPI =
                    lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI;
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_IAP =
                    lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP;
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.Eligible_Plans =
                    lbusQdroCalculationHeader.ibusBenefitApplication.Eligible_Plans;
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.icdoBenefitApplication.retirement_date = this.icdoDroApplication.dro_commencement_date;
                    lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.DetermineBenefitSubTypeandEligibility_Disability();
                    //}
                }

                #endregion

                if (lbusQdroCalculationHeader.ibusBenefitApplication.CheckAlreadyVested(lstrPlanCode))
                {
                    busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                    lbusPlanBenefitXr.FindPlanBenefitXr(lintBenefitId);

                    lbusQdroCalculationHeader.SpawnFinalQdroCalculation(aintDroBenefitId,
                                        this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lintPlanId).FirstOrDefault().icdoPersonAccount.person_account_id,
                                        lintPlanId, lstrPlanCode,
                                        lbusQdroCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                        this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                                        lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value, lblnParticipantPayeeAccount);
                }
                try
                {
                    lbusQdroCalculationHeader.icdoQdroCalculationHeader.Insert();
                    //Ticket#84882
                    lbusQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.iblnIsNewRecord = true;
                    lbusQdroCalculationHeader.AfterPersistChanges();
                    iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.status_value =
                            busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
                    iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.Update();

                    #region Code for Workflow

                    switch (lstrPlanCode)
                    {
                        case busConstant.MPIPP:
                            MPI_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            MPIPPPlan_Selected = true;
                            break;

                        case busConstant.IAP:
                            IAP_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            IAPPlan_Selected = true;
                            break;

                        case busConstant.Local_52:
                            L52_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            L52Plan_Selected = true;
                            break;

                        case busConstant.Local_161:
                            L161_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            L161Plan_Selected = true;
                            break;

                        case busConstant.Local_600:
                            L600_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            L600Plan_Selected = true;
                            break;

                        case busConstant.Local_666:
                            L666_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            L666Plan_Selected = true;
                            break;

                        case busConstant.LOCAL_700:
                            L700_Final_Calculation_Id = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                            L700Plan_Selected = true;
                            break;
                    }

                    #endregion


                }
                catch
                {
                }


            }

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }

            EvaluateInitialLoadRules();
            if (this.iclbDroBenefitDetails.Count() > 0 && this.iclbDroBenefitDetails != null)
            {
                foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
                {
                    lbusDroBenefitDetails.EvaluateInitialLoadRules();
                }
            }
            larrErrors.Add(this);

            return larrErrors;
        }


        public Collection<cdoPlan> GetSubPlan(int aintPlan_Id)
        {
            Collection<cdoPlan> lclbSubPlans = new Collection<cdoPlan>();

            if (aintPlan_Id == busConstant.MPIPP_PLAN_ID)
            {


                if (this.ibusParticipant.iclbPersonAccount.IsNotNull() && this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitApplication.ibusPerson.FindPerson(this.icdoDroApplication.person_id);
                    lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusBenefitApplication.ibusPerson.LoadPersonAccounts();

                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(
                        this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);

                    if (lbusPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date != DateTime.MinValue)
                    {
                        DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfNonVestedEEIsPresent",
                            new object[2] { this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id ,
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date.Year});
                        if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                        {
                            cdoPlan lcdoPlan = new cdoPlan();
                            lcdoPlan.plan_code = busConstant.EE;
                            lcdoPlan.plan_name = busConstant.EE;
                            lclbSubPlans.Add(lcdoPlan);
                        }
                    }
                }


                if (this.ibusParticipant.iclbPersonAccount.IsNotNull() && this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccountRetirementContribution.CheckIfUVHPAmountPresent", new object[1] { this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                    {
                        cdoPlan lcdoPlan1 = new cdoPlan();
                        lcdoPlan1.plan_code = busConstant.UVHP;
                        lcdoPlan1.plan_name = busConstant.UVHP;
                        lclbSubPlans.Add(lcdoPlan1);
                    }
                }


            }
            else if (aintPlan_Id == busConstant.IAP_PLAN_ID)
            {
                if (this.ibusParticipant.iclbPersonAccount.IsNotNull() && this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_52_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                    {

                        cdoPlan lcdoPlan = new cdoPlan();
                        lcdoPlan.plan_code = busConstant.L52_SPL_ACC;
                        lcdoPlan.plan_name = "LOCAL-52 SPECIAL ACCOUNT";
                        lclbSubPlans.Add(lcdoPlan);

                    }
                }


                if (this.ibusParticipant.iclbPersonAccount.IsNotNull() && this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).Count() > 0)
                {
                    DataTable ldtblCount = busBase.Select("cdoPersonAccount.CheckPersonHasSpecialAccount", new object[1] { this.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.LOCAL_161_PLAN_ID).First().icdoPersonAccount.person_account_id });
                    if (ldtblCount.Rows.Count > 0 && Convert.ToInt32(ldtblCount.Rows[0][0]) > 0)
                    {


                        cdoPlan lcdoPlan1 = new cdoPlan();
                        lcdoPlan1.plan_code = busConstant.L161_SPL_ACC;
                        lcdoPlan1.plan_name = "LOCAL-161 SPECIAL ACCOUNT";
                        lclbSubPlans.Add(lcdoPlan1);
                    }
                }
            }

            return lclbSubPlans;
        }

        public bool IsParticipantEligibleForDisabilityBenefit()
        {
            if (icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
            {
                DataTable ldtbList = SelectWithOperator<cdoBenefitApplication>(new string[] {enmBenefitApplication.person_id.ToString(),
                                enmBenefitApplication.benefit_type_value.ToString(), enmBenefitApplication.application_status_value.ToString() },
                                   new string[] { busConstant.DBOperatorEquals, busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals },
                                   new object[] { icdoDroApplication.person_id, busConstant.BENEFIT_TYPE_DISABILITY, busConstant.BENEFIT_APPL_CANCELLED }, null);
                if (ldtbList.Rows.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Using for ReEmployment as Calculation Object will exist for the participant
        /// </summary>
        /// <param name="abusDroBenefitDetails"></param>
        /// <param name="abusBenefitCalculationDetail"></param>
        /// <param name="abusBenefitCalculationOptions"></param>


        #endregion

        #region Overriden Methods

        public override busBase GetCorPerson()
        {
            return this.ibusParticipant;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;

            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            istrApprovedByUser = this.icdoDroApplication.approved_by_user;

            if (this.icdoDroApplication.approved_by_user.IsNotNullOrEmpty())
            {
                DataTable ldtbApprovedByUserFullName = Select("cdoDroApplication.GetApprovedByUserFullName", new object[1] { this.icdoDroApplication.approved_by_user });
                if (ldtbApprovedByUserFullName.Rows.Count > 0)
                {
                    istrApprovedByUserFullName = Convert.ToString(ldtbApprovedByUserFullName.Rows[0][0]);
                }
            }

            string Firstinitial = string.Empty;
            string SecondInitial = string.Empty;
            if (!string.IsNullOrEmpty(istrApprovedByUser))
            {
                if (istrApprovedByUser.Contains(" ") || istrApprovedByUser.Contains("."))
                {
                    string[] split = istrApprovedByUser.Split(new Char[] { ' ', '.' });
                    if (split.Length > 0 && !string.IsNullOrEmpty(split[0]))
                        Firstinitial = split[0].Substring(0, 1);
                    if (split.Length > 1 && !string.IsNullOrEmpty(split[1]))
                        SecondInitial = split[1].Substring(0, 1);

                    istrApprovedByUserInitials = Firstinitial.ToUpper() + SecondInitial.ToUpper();
                }
                else
                {
                    if (!string.IsNullOrEmpty(istrApprovedByUser))
                    {
                        istrApprovedByUserInitials = istrApprovedByUser.Substring(0, 2).ToUpper();
                    }
                }
            }
            foreach (busPersonContact lbusPersonContact in this.ibusParticipant.iclbPersonContact)
            {
                if (lbusPersonContact.icdoPersonContact.contact_type_value == "ATRN")
                {
                    if ((lbusPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue && lbusPersonContact.icdoPersonContact.effective_start_date <= DateTime.Today) ||
                        lbusPersonContact.icdoPersonContact.effective_start_date <= DateTime.Today && lbusPersonContact.icdoPersonContact.effective_end_date > DateTime.Today)
                    {
                        istrAttorneyName = lbusPersonContact.icdoPersonContact.contact_name;
                    }
                }
            }
            if (astrTemplateName == busConstant.JOINDER_COVER_LETTER_TO_COURT || astrTemplateName == busConstant.NOTICE_OF_APPEARANCE_AND_RESPONSE_OF_EMPLOYEE_BENEFIT_PLAN_JOINDER)
            {
                this.ibusParticipant.LoadCourtAddress();
                this.ibusAlternatePayee.LoadInitialData();
                this.ibusAlternatePayee.LoadPersonAddresss();
                this.ibusAlternatePayee.LoadPersonContacts();
                this.ibusAlternatePayee.LoadCorrAddress();

                if (Convert.ToInt32(this.ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.USA)
                {
                    istrTrue = "1";
                }
                else
                {
                    istrTrue = "0";
                }
                if (Convert.ToInt32(this.ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_country_value) == busConstant.USA)
                {
                    istrIsUSA = "1";
                }
                else
                {
                    istrIsUSA = "0";
                }
            }

            #region DIS-0020
            if (astrTemplateName == busConstant.WAIVER_OF_DISABILITY_INTEREST_ALTERNATE_PAYEE)
            {


                if (this.ibusParticipant.IsNotNull())
                {
                    istrParticipantName = this.ibusParticipant.icdoPerson.istrFullName;
                }
                if (this.ibusAlternatePayee.IsNotNull())
                {
                    istrExSpouseName = this.ibusAlternatePayee.icdoPerson.istrFullName;
                }
                if (this.icdoDroApplication.date_of_marriage != DateTime.MinValue)
                {
                    istrDateOfMarrige = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoDroApplication.date_of_marriage);
                }
                if (this.icdoDroApplication.date_of_divorce != DateTime.MinValue)
                {
                    istrDateOfSeparation = busGlobalFunctions.ConvertDateIntoDifFormat(this.icdoDroApplication.date_of_divorce);
                }
            }
            #endregion

            #region DRO-0002
            if (astrTemplateName == busConstant.QDRO_STATUS_PENDING)
            {
                if (this.ibusParticipant.icdoPerson.email_address_1 != null)
                    istrEmailAddr = this.ibusParticipant.icdoPerson.email_address_1.ToLower();
            }
            #endregion
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            if (aobjBus is busDroBenefitDetails)
            {
                busDroBenefitDetails ibusDroBenefitDetails = (aobjBus as busDroBenefitDetails);
                ibusDroBenefitDetails.iobjbusQdroApplication = this;
            }

            if (aobjBus is busAttorney)
            {
                busAttorney lbusAttorney = (aobjBus as busAttorney);
                lbusAttorney.iobjbusQdroApplication = this;
                //ibusAttorney.EvaluateInitialLoadRules(utlPageMode.All);
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (busDroBenefitDetails lbusDroBenefitDetails in this.iclbDroBenefitDetails)
            {
                Hashtable lhstParams = new Hashtable();
                lhstParams["icdoDroBenefitDetails.plan_id"] = lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id;
                lhstParams["icdoDroBenefitDetails.benefit_perc"] = lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_perc;
                lhstParams["icdoDroBenefitDetails.benefit_amt"] = lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_amt;
                lhstParams["icdoDroBenefitDetails.dro_model_value"] = lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value;
                lhstParams["istrSubPlan"] = lbusDroBenefitDetails.istrSubPlan;
                lhstParams["icdoDroBenefitDetails.istrBenefitOptionValue"] = lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue;
                lhstParams["icdoDroBenefitDetails.benefit_flat_perc"] = lbusDroBenefitDetails.icdoDroBenefitDetails.benefit_flat_perc;
                lhstParams["icdoDroBenefitDetails.dro_withheld_perc"] = lbusDroBenefitDetails.icdoDroBenefitDetails.dro_withheld_perc;
                lhstParams["icdoDroBenefitDetails.alt_payee_increase"] = lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_increase;
                lhstParams["icdoDroBenefitDetails.alt_payee_early_ret"] = lbusDroBenefitDetails.icdoDroBenefitDetails.alt_payee_early_ret;

                ArrayList arrChild = ValidateNewChild(lhstParams, false);
                bool lblnFlag = busConstant.BOOL_FALSE;

                foreach (utlError lutlError in arrChild)
                {
                    foreach (utlError lError in this.iarrErrors)
                    {
                        if (lError.istrErrorID == lutlError.istrErrorID)
                        {
                            lblnFlag = busConstant.BOOL_TRUE;
                            break;
                        }
                    }

                    if (!lblnFlag)
                    {
                        this.iarrErrors.Add(lutlError);
                    }
                }
            }
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (this.icdoDroApplication.dro_status_id >= 0 && string.IsNullOrEmpty(this.icdoDroApplication.dro_status_value))
            {
                this.icdoDroApplication.dro_status_id = 6005;
                this.icdoDroApplication.dro_status_value = "RCVD";
                this.icdoDroApplication.batch_90day_flag = "N";
            }
            if (this.icdoDroApplication.is_ammended_flag.IsNullOrEmpty())
            {
                this.icdoDroApplication.is_ammended_flag = "N";
            }

            this.ibusParticipant.iclbNotes.ForEach(item =>
            {
                if (item.icdoNotes.person_id == 0)
                    item.icdoNotes.person_id = this.icdoDroApplication.person_id;
                item.icdoNotes.form_id = busConstant.Form_ID;
                item.icdoNotes.form_value = busConstant.QRDO_MAINTAINENCE_FORM;
            });

        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            this.UpdateStatusHistoryValue();

            if (this.ibusBaseActivityInstance.IsNull() && this.iblIsNew)
            {
                DataTable ldtbActivityInstance = busBase.Select("cdoActivityInstance.GetActivityInstanceIdByPersonIdForEstimate", new object[3] { this.icdoDroApplication.person_id, busConstant.QDRO_WORKFLOW_NAME, busConstant.QDRO_APPLICATION_ACTIVITY_NAME });

                if (ldtbActivityInstance.Rows.Count > 0)
                {
                    busActivityInstance lbusActivityInstance = new busActivityInstance();

                    int lintActivityInstance = Convert.ToInt32(ldtbActivityInstance.Rows[0][enmActivityInstance.activity_instance_id.ToString()]);

                    if (lbusActivityInstance.FindActivityInstance(lintActivityInstance))
                    {
                        lbusActivityInstance.LoadActivity();
                        lbusActivityInstance.LoadProcessInstance();
                        lbusActivityInstance.ibusProcessInstance.ibusProcess = lbusActivityInstance.ibusActivity.ibusProcess;
                        lbusActivityInstance.ibusProcessInstance.LoadPerson();
                        lbusActivityInstance.EvaluateInitialLoadRules();
                        this.ibusBaseActivityInstance = lbusActivityInstance;
                        this.SetProcessInstanceParameters();
                        lbusActivityInstance.icdoActivityInstance.reference_id = this.icdoDroApplication.dro_application_id;
                        lbusActivityInstance.icdoActivityInstance.Update();
                    }
                }
            }
            else if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
            if (this.iarrChangeLog.Count==0)
            {
                iobjPassInfo.idictParams["SaveMesssageDroBenefitDetails"] = "false";
            }
        }

        #endregion

        public bool CheckValidDROCommencementDate()
        {
            if (icdoDroApplication.dro_commencement_date != DateTime.MinValue &&
                icdoDroApplication.dro_commencement_date.Day != 1)
            {
                return false;
            }
            return true;
        }
    }
}

