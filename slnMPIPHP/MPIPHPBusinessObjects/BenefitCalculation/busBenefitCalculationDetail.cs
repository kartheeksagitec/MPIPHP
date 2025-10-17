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
using System.Data.SqlTypes;
using MPIPHP.DataObjects;
using System.Linq;
#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationDetail:
	/// Inherited from busBenefitCalculationDetailGen, the class is used to customize the business object busBenefitCalculationDetailGen.
	/// </summary>
	[Serializable]
	public class busBenefitCalculationDetail : busBenefitCalculationDetailGen
	{
        //Added For Dis to be shown On Screen
        public DateTime idtRetirementDate { get; set; }
        public string istrDisabilityType { get; set; }
        public decimal idecLumpSumBenefitAmount { get; set; }
        public Collection<busBenefitCalculationYearlyDetail> iclbAnnualBenfitSummayOverviewTotal { get; set; }
        public Collection<busQDROOffsetDetails> iclbQDROOffsetDetails { get; set; }
        public Collection<busIAPAmountDetails> iclbIAPAmountDetails { get; set; }

     




        #region ReducedAmount_AfterApplyingSurvivorPercentage_Properties
        //public decimal idecFinalPercentSurvivorAccrued { get; set; }
        //public decimal idecFinalPercentSurviviorEEUVHPAmount { get; set; }
        //public decimal idecFinalPercentSurviviorIAPAmount { get; set; }
        //public decimal idecFinalPercentSurviviorL161SpecialAccountAmount { get; set; }
        //public decimal idecFinalPercentSurviviorL52SpecialAccountAmount { get; set; }
        #endregion

        public override void LoadBenefitCalculationYearlyDetails()
        {
            DataTable ldtbList = Select<cdoBenefitCalculationYearlyDetail>(
                new string[1] { enmBenefitCalculationYearlyDetail.benefit_calculation_detail_id.ToString() },
                new object[1] { icdoBenefitCalculationDetail.benefit_calculation_detail_id }, null, enmBenefitCalculationYearlyDetail.plan_year.ToString());
            iclbBenefitCalculationYearlyDetail = GetCollection<busBenefitCalculationYearlyDetail>(ldtbList, "icdoBenefitCalculationYearlyDetail");

        }

        public void LoadBenefitCalculationYearlyDetailsBeforeRetirement()
        {
            DataTable ldtbList = busBase.Select("cdoBenefitCalculationYearlyDetail.LoadDetailsBeforeRetirement", new object[1] { this.icdoBenefitCalculationDetail.benefit_calculation_detail_id });
            
            iclbBenefitCalculationYearlyDetail = GetCollection<busBenefitCalculationYearlyDetail>(ldtbList, "icdoBenefitCalculationYearlyDetail");

        }
        public void LoadChildGridForBenefitCalculationMaintenances()
        {
            if (utlPassInfo.iobjPassInfo.istrFormName == "wfmDisabiltyBenefitCalculationMaintenance")
            {
                object lobjMainHeaderObject = GetObjectFromDB(utlPassInfo.iobjPassInfo.istrFormName, this.icdoBenefitCalculationDetail.benefit_calculation_header_id);
                if (lobjMainHeaderObject is busDisabiltyBenefitCalculation)
                {
                    busDisabiltyBenefitCalculation lbusDisabiltyBenefitCalculation = lobjMainHeaderObject as busDisabiltyBenefitCalculation;
                    lbusDisabiltyBenefitCalculation.sel_benefit_calculation_detail_id = this.icdoBenefitCalculationDetail.benefit_calculation_detail_id;
                    StoreObjectInDB(lbusDisabiltyBenefitCalculation, ablnWait: true);
                }
            }
            if (utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationWithdrawalMaintenance")
            {
                LoadBenefitCalculationOptionss();
            }
            else
            {
                if (this.ibusPerson == null)
                {
                    this.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    this.ibusPerson.FindPerson(this.icdoBenefitCalculationDetail.iintPersonId);
                    if (this.ibusPerson.iclbPersonAccount == null || this.ibusPerson.iclbPersonAccount.Count == 0)
                        this.ibusPerson.LoadPersonAccounts();
                }

                DateTime ldtForfietureDate = new DateTime();

                if (icdoBenefitCalculationDetail.istrPlanCode == busConstant.MPIPP &&
                    this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                {
                    busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility();
                    lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id);
                    if (lbusPersonAccountEligibility != null)
                        ldtForfietureDate = lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                }

                LoadBenefitCalculationOptionss();
                LoadBenefitCalculationYearlyDetails();
                LoadBenefitCalculationYearlyDetailsTotal(ldtForfietureDate);

                if (utlPassInfo.iobjPassInfo.istrFormName == "wfmDisabiltyBenefitCalculationMaintenance")
                {
                    decimal ldecReductionFactor = this.icdoBenefitCalculationDetail.early_reduction_factor;
                    if (ldecReductionFactor > 0)
                    {
                        this.iclbBenefitCalculationOptions.ForEach(option => option.idecRegularReductionFactor = ldecReductionFactor);
                    }
                }
                if (utlPassInfo.iobjPassInfo.istrFormName == "wfmBenefitCalculationPreRetirementDeathMaintenance")
                {
                    this.LoadBenefitCalculationHeader();

                    if (this.ibusBenefitCalculationHeader.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL ||
                        this.ibusBenefitCalculationHeader.icdoBenefitCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
                    {
                        if (this.iclbBenefitCalculationOptions != null
                        && this.iclbBenefitCalculationOptions.Count() > 0)
                        {
                            if (this.iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.survivor_amount != decimal.Zero)
                                this.iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.idecRemainingAmountToBePaid =
                                    this.iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.survivor_amount -
                                    this.iclbBenefitCalculationOptions[0].icdoBenefitCalculationOptions.paid_amount;
                        }
                    }
                }
            }
        }
        public void LoadBenefitCalculationYearlyDetailsTotal(DateTime adtForfietureDate, Decimal adecHoursTillLatestWithdrawal = Decimal.Zero)
        {
            if (iclbBenefitCalculationYearlyDetail != null && iclbBenefitCalculationYearlyDetail.Count > 0)
            {
                this.iclbAnnualBenfitSummayOverviewTotal = new Collection<busBenefitCalculationYearlyDetail>();
                busBenefitCalculationYearlyDetail lbusBenefitCalculationYearlyDetail = new busBenefitCalculationYearlyDetail { icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail() };
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.benefit_calculation_detail_id = this.icdoBenefitCalculationDetail.benefit_calculation_detail_id;

                if (adtForfietureDate != DateTime.MinValue && iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > adtForfietureDate.Year).Count() > 0)
                {
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalPensionHours = iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > adtForfietureDate.Year).Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours);
                    // PORD PIR 205
                    //lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalHealthHours = iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > adtForfietureDate.Year).Sum(item => item.icdoBenefitCalculationYearlyDetail.health_hours);
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalVestedHours = iclbBenefitCalculationYearlyDetail.Where(item => item.icdoBenefitCalculationYearlyDetail.plan_year > adtForfietureDate.Year).Sum(item => item.icdoBenefitCalculationYearlyDetail.vested_hours);
                }
                else
                {
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalVestedHours = iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.vested_hours);
                    lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalPensionHours = iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.annual_hours);
                }
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.iintQualifiedYears = iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.qualified_years_count;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.iiVestedYears = iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.vested_years_count;
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.idecTotalHealthHours = iclbBenefitCalculationYearlyDetail.Sum(item => item.icdoBenefitCalculationYearlyDetail.health_hours); // PROD PIR 205
                lbusBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail.iintHealthCount = iclbBenefitCalculationYearlyDetail.Last().icdoBenefitCalculationYearlyDetail.health_years_count;
                iclbAnnualBenfitSummayOverviewTotal.Add(lbusBenefitCalculationYearlyDetail);
            }
        }


        //This is done so that we do not have to FIRE extra queries for the Populating a SIMPLE DESCRIPTION
        public override void LoadBenefitCalculationOptionss()
        {
            base.LoadBenefitCalculationOptionss();
            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in iclbBenefitCalculationOptions)
            {
                lbusBenefitCalculationOptions.LoadPlanBenefitXr();
                lbusBenefitCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.PopulateDescriptions();
                lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription = lbusBenefitCalculationOptions.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_description;

                if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES && lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(EE and UVHP)";
                }
                else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_flag == busConstant.FLAG_YES)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(EE)";
                }
                else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.uvhp_flag == busConstant.FLAG_YES)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(UVHP)";
                }
              

                if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local52_special_acct_bal_flag == busConstant.FLAG_YES)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 52 Special Account)";
                }
                else if (lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.local161_special_acct_bal_flag == busConstant.FLAG_YES)
                {
                    lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription += "(Local 161 Special Account)";
                }
            }
        }

        public busBenefitCalculationDetail GetQDROOffsetDetails(int aintParticiapantID, int aintPlanId, string astrCalculationType,string astrBenefitTypeValue,
       string astrEEFlag, string astrUVHPFlag, string astrL52SplAccFlag , string astrL161SplAccFlag)
        {
            string lstrSubPlan = string.Empty;
            DataTable ldtlbQRDOOffset = new DataTable();
            DataTable ldtbQDROOffsetSubPlans = new DataTable();
            DataTable ldtlbQRDOOffsetSubPlansEstimates = new DataTable();
            ArrayList larrDROModel = new ArrayList();
            string lstrPlanName = string.Empty;
            string lstrParticipantMpid = string.Empty;
            DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { aintParticiapantID });
            if (ldtbPersonMPID.Rows.Count > 0)
            {
                lstrParticipantMpid = Convert.ToString(ldtbPersonMPID.Rows[0][0]);
            }

            DataTable ldtplan = Select("cdoPlan.GetPlanById", new object[1] { aintPlanId });
            if (ldtplan.Rows.Count > 0)
            {
                DataRow drRow = ldtplan.Rows[0];
                lstrPlanName = Convert.ToString(drRow[0]);
            }

            Collection<busQdroCalculationDetail> lclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();

            busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
            lbusBenefitCalculationDetail.iclbQDROOffsetDetails = new Collection<busQDROOffsetDetails>();

            if (astrCalculationType == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
            {
                # region QDRO Offset

                if (ldtlbQRDOOffset.IsNull() || ldtlbQRDOOffset.Rows.Count <= 0)
                {
                    ldtlbQRDOOffset = Select("cdoQdroCalculationHeader.GetQDROOffSet", new object[2] { aintParticiapantID, aintPlanId });

                    if (ldtlbQRDOOffset.Rows.Count > 0)
                        lclbQdroCalculationDetail = GetCollection<busQdroCalculationDetail>(ldtlbQRDOOffset, "icdoQdroCalculationDetail");
                }

                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lclbQdroCalculationDetail)
                {
                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag != busConstant.FLAG_YES && lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag != busConstant.FLAG_YES
                       && lbusQdroCalculationDetail.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusQdroCalculationDetail.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                    {

                        if (!(astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL && aintPlanId == busConstant.MPIPP_PLAN_ID))
                        {
                            busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
                            lbusQdroCalculationHeader.FindQdroCalculationHeader(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id); string lstrAlternateMpid = string.Empty;

                            DataTable ldtbAlternateMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id });
                            if (ldtbAlternateMPID.Rows.Count > 0)
                            {
                                lstrAlternateMpid = Convert.ToString(ldtbAlternateMPID.Rows[0][0]);
                            }



                            if (lclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.qdro_model_value == lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value &&
                                item.icdoQdroCalculationDetail.istrIsFinal == "Y").Count() > 0)
                            {
                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName;
                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                                lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                larrDROModel.Add(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value);
                            }
                            else if (!larrDROModel.Contains(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value))
                            {
                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName;
                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                                lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                            }

                            if (aintPlanId == busConstant.MPIPP_PLAN_ID && lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution != 0)
                            {
                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(EE)"; 
                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                                lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution;
                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                            }
                        }
                    }
                }

                # endregion QDRO Offset
                #region QDRO Offset for SubPlans

                int lintAlternatePayeeId = 0;
                DataTable ldtblAlternatePayee = Select("cdoBenefitCalculationHeader.GetAlternatePayeeIDForParticipant", new object[1] { aintParticiapantID });

                //Find Alternate Payee for Approved Final QDRO Estimate
                DataTable ldbtlAlternatePayeeFromQDROFinalEstimate = Select("cdoQdroCalculationHeader.GetAlternatePayeeFromFinalQDROEstimate", new object[2] { aintParticiapantID,aintPlanId });
                if (ldbtlAlternatePayeeFromQDROFinalEstimate.Rows.Count > 0)
                {
                    if (ldtblAlternatePayee.Rows.Count <= 0)
                    {
                        ldtblAlternatePayee = ldbtlAlternatePayeeFromQDROFinalEstimate;
                    }
                    else
                    {
                        if (ldbtlAlternatePayeeFromQDROFinalEstimate.Rows.Count > 0)
                        {
                            foreach (DataRow ldtr in ldbtlAlternatePayeeFromQDROFinalEstimate.Rows)
                            {
                                bool lblnFlag = false;
                                foreach (DataRow ldtRow in ldtblAlternatePayee.Rows)
                                {
                                    if (Convert.ToInt32(ldtRow[0]) == Convert.ToInt32(ldtr[0]))
                                    {
                                        lblnFlag = true;
                                    }
                                }
                                if (!lblnFlag)
                                {
                                    DataRow ldtDataRow = ldtblAlternatePayee.NewRow();
                                    ldtDataRow[0] = ldtr[0];
                                    ldtblAlternatePayee.Rows.Add(ldtDataRow);
                                }
                            }
                        }
                    }
                }

                if (ldtblAlternatePayee.Rows.Count > 0)
                {
                    foreach (DataRow ldtRow in ldtblAlternatePayee.Rows)
                    {
                        if (Convert.ToString(ldtRow[0]).IsNotNullOrEmpty())
                        {
                            lintAlternatePayeeId = Convert.ToInt32(ldtRow[0]);
                        }
                        if (lintAlternatePayeeId > 0)
                        {
                            #region QDRO Offset for SubPlans (Final Approved Withdrawal)
                            ldtbQDROOffsetSubPlans.Clear();
                            ldtbQDROOffsetSubPlans = Select("cdoBenefitCalculationHeader.GetQDROOffsetSubPlansDetails", new object[2] { lintAlternatePayeeId, aintPlanId });

                            string lstrAlternateMpid = string.Empty;
                            DataTable ldtbAlternateMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lintAlternatePayeeId });
                            if (ldtbAlternateMPID.Rows.Count > 0)
                            {
                                lstrAlternateMpid = Convert.ToString(ldtbAlternateMPID.Rows[0][0]);
                            }

                            if (ldtbQDROOffsetSubPlans.Rows.Count > 0)
                            {
                                foreach (DataRow ldrRow in ldtbQDROOffsetSubPlans.Rows)
                                {
                                    if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                                    {
                                        decimal idecEEContribution = 0;
                                        decimal idecUVHPContribution = 0;


                                        if ((Convert.ToString(ldrRow[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty()) ||
                                            (Convert.ToString(ldrRow[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty()))
                                        {

                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                idecEEContribution = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()])
                                                    + Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.non_vested_ee_interest.ToString().ToUpper()]);
                                            }
                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                idecUVHPContribution = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()])
                                                      + Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.total_uvhp_interest_amount.ToString().ToUpper()]);
                                            }

                                            busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                            lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                            lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                            lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UV&HP/EE)";
                                            lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                            lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRow[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                            lbusQDROOffsetDetails.idecQDROOffset = idecEEContribution + idecUVHPContribution;
                                            lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                        }

                                    }
                                    else
                                    {
                                        if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                                        {
                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(EE)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRow[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()])
                                                    + Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.non_vested_ee_interest.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);


                                            }
                                        }
                                        if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                                        {
                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UVHP)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRow[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()])
                                                      + Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.total_uvhp_interest_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                        if (aintPlanId == busConstant.IAP_PLAN_ID)
                                        {
                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.local52_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local52 Special Account)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRow[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.local52_special_acct_bal_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                        if (aintPlanId == busConstant.IAP_PLAN_ID)
                                        {
                                            if (Convert.ToString(ldrRow[enmBenefitCalculationDetail.local161_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local161 Special Account)"; ;
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRow[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRow[enmBenefitCalculationDetail.local161_special_acct_bal_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                    }
                                }
                            }
                            # endregion QDRO Offset for SubPlans (Final Approved Withdrawal)
                            #region QDRO Offset for SubPlans (QDRO Approved Estimate)

                            if ((ldtbQDROOffsetSubPlans.IsNull() || ldtbQDROOffsetSubPlans.Rows.Count <= 0))
                            {
                                ldtlbQRDOOffsetSubPlansEstimates.Clear();
                                ldtlbQRDOOffsetSubPlansEstimates = Select("cdoQdroCalculationHeader.GetQDROOffSetEstimateDetails", new object[3] { aintParticiapantID, lintAlternatePayeeId, aintPlanId });

                                decimal idecEEContribution = 0;
                                decimal idecUVHPContribution = 0;

                                if (ldtlbQRDOOffsetSubPlansEstimates.Rows.Count > 0)
                                {
                                        Collection<busQdroCalculationDetail> lcblQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
                                        lclbQdroCalculationDetail = GetCollection<busQdroCalculationDetail>(ldtlbQRDOOffsetSubPlansEstimates, "icdoQdroCalculationDetail");

                                        foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lclbQdroCalculationDetail)
                                        {
                                            if (aintPlanId == busConstant.MPIPP_PLAN_ID && astrBenefitTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                                {
                                                    idecEEContribution = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_interest_amount;
                                                }

                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                                {
                                                    idecUVHPContribution = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp_interest;
                                                }

                                                if (idecEEContribution > 0 || idecUVHPContribution > 0)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UV&HP/EE)";
                                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.idecQDROOffset = idecEEContribution + idecUVHPContribution;
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                                }


                                            }
                                            else
                                            {
                                                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                                                {
                                                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                                    {
                                                        busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                        lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                        lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                        lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(EE)";
                                                        lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                        lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                        lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_interest_amount; 
                                                        lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                    }


                                                }
                                                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                                                {
                                                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                                    {
                                                        busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                        lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                        lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                        lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UVHP)"; ;
                                                        lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                        lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                        lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp_interest;
                                                        lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                                    }
                                                }
                                                if (aintPlanId == busConstant.IAP_PLAN_ID)
                                                {
                                                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                                    {
                                                        busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                        lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                        lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                        lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local52 Special Account)";
                                                        lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                        lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                        lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                                        lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                    }
                                                }
                                                if (aintPlanId == busConstant.IAP_PLAN_ID)
                                                {
                                                    if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                                    {
                                                        busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                        lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                        lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                        lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local161 Special Account)";
                                                        lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                        lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                        lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                                        lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                    }
                                                }
                                            }
                                        }
                                }
                            }
                            #endregion  #region QDRO Offset for SubPlans (QDRO Approved Estimate)
                        }
                    }
                }


                #endregion QDRO Offset for SubPlans
            }
            else if (astrCalculationType == busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL || astrCalculationType == busConstant.BenefitCalculation.CALCULATION_TYPE_ADJUSTMENT)
            {
                # region QDRO Offset
                if (astrEEFlag != busConstant.FLAG_YES && astrUVHPFlag != busConstant.FLAG_YES
                    && astrL52SplAccFlag != busConstant.FLAG_YES && astrL161SplAccFlag != busConstant.FLAG_YES)
                {
                    if (ldtlbQRDOOffset.IsNull() || ldtlbQRDOOffset.Rows.Count <= 0)
                    {
                        ldtlbQRDOOffset = Select("cdoQdroCalculationHeader.GetQDROOffSet", new object[2] { aintParticiapantID, aintPlanId });

                        if (ldtlbQRDOOffset.Rows.Count > 0)
                            lclbQdroCalculationDetail = GetCollection<busQdroCalculationDetail>(ldtlbQRDOOffset, "icdoQdroCalculationDetail");
                    }

                    foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lclbQdroCalculationDetail)
                    {
                        if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag != busConstant.FLAG_YES && lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag != busConstant.FLAG_YES
                      && lbusQdroCalculationDetail.icdoQdroCalculationDetail.l52_spl_acc_flag != busConstant.FLAG_YES && lbusQdroCalculationDetail.icdoQdroCalculationDetail.l161_spl_acc_flag != busConstant.FLAG_YES)
                        {

                            if (!(astrBenefitTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL && aintPlanId == busConstant.MPIPP_PLAN_ID))
                            {
                                busQdroCalculationHeader lbusQdroCalculationHeader = new busQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
                                lbusQdroCalculationHeader.FindQdroCalculationHeader(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id); string lstrAlternateMpid = string.Empty;

                                DataTable ldtbAlternateMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id });
                                if (ldtbAlternateMPID.Rows.Count > 0)
                                {
                                    lstrAlternateMpid = Convert.ToString(ldtbAlternateMPID.Rows[0][0]);
                                }

                                if (lclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.qdro_model_value == lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value &&
                                    item.icdoQdroCalculationDetail.istrIsFinal == "Y").Count() > 0)
                                {
                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName;
                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                    larrDROModel.Add(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value);
                                }
                                else if (!larrDROModel.Contains(lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_model_value))
                                {
                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName;
                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_calculation_header_id;
                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                }
                            }
                        }
                    }
                }
                # endregion QDRO Offset
                #region QDRO Offset for SubPlans
                else
                {
                    int lintAlternatePayeeId = 0;
                    DataTable ldtblAlternatePayee = Select("cdoBenefitCalculationHeader.GetAlternatePayeeIDForParticipant", new object[1] { aintParticiapantID });

                    //Find Alternate Payee for Approved Final QDRO Estimate
                    DataTable ldbtlAlternatePayeeFromQDROFinalEstimate = Select("cdoQdroCalculationHeader.GetAlternatePayeeFromFinalQDROEstimate", new object[2] { aintParticiapantID, aintPlanId });
                    if (ldbtlAlternatePayeeFromQDROFinalEstimate.Rows.Count > 0)
                    {
                        if (ldtblAlternatePayee.Rows.Count <= 0)
                        {
                            ldtblAlternatePayee = ldbtlAlternatePayeeFromQDROFinalEstimate;
                        }
                        else
                        {
                            if (ldbtlAlternatePayeeFromQDROFinalEstimate.Rows.Count > 0)
                            {
                                foreach (DataRow ldtr in ldbtlAlternatePayeeFromQDROFinalEstimate.Rows)
                                {
                                    bool lblnFlag = false;
                                    foreach (DataRow ldtRow in ldtblAlternatePayee.Rows)
                                    {
                                        if (Convert.ToInt32(ldtRow[0]) == Convert.ToInt32(ldtr[0]))
                                        {
                                            lblnFlag = true;
                                        }
                                    }
                                    if (!lblnFlag)
                                    {
                                        DataRow ldtDataRow = ldtblAlternatePayee.NewRow();
                                        ldtDataRow[0] = ldtr[0];
                                        ldtblAlternatePayee.Rows.Add(ldtDataRow);
                                    }
                                }
                            }
                        }
                    }


                    if (ldtblAlternatePayee.Rows.Count > 0)
                    {
                        foreach (DataRow ldtRow in ldtblAlternatePayee.Rows)
                        {
                            if (Convert.ToString(ldtRow[0]).IsNotNullOrEmpty())
                            {
                                lintAlternatePayeeId = Convert.ToInt32(ldtRow[0]);
                            }
                            if (lintAlternatePayeeId > 0)
                            {
                                #region QDRO Offset for SubPlans (Final Approved Withdrawal)
                                ldtbQDROOffsetSubPlans.Clear();
                                ldtbQDROOffsetSubPlans = Select("cdoBenefitCalculationHeader.GetQDROOffsetSubPlansDetails", new object[2] { lintAlternatePayeeId, aintPlanId });


                                string lstrAlternateMpid = string.Empty;
                                DataTable ldtbAlternateMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lintAlternatePayeeId });
                                if (ldtbAlternateMPID.Rows.Count > 0)
                                {
                                    lstrAlternateMpid = Convert.ToString(ldtbAlternateMPID.Rows[0][0]);
                                }

                                if (ldtbQDROOffsetSubPlans.Rows.Count > 0)
                                {
                                    foreach (DataRow ldrRows in ldtbQDROOffsetSubPlans.Rows)
                                    {
                                        if (astrEEFlag == busConstant.FLAG_YES && astrUVHPFlag == busConstant.FLAG_YES)
                                        {
                                            decimal idecEEContribution = 0;
                                            decimal idecUVHPContribution = 0;


                                            if ((Convert.ToString(ldrRows[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty()) ||
                                                (Convert.ToString(ldrRows[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty()))
                                            {

                                                if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                                {
                                                    idecEEContribution = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()])
                                                        + Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.non_vested_ee_interest.ToString().ToUpper()]);
                                                }
                                                if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                                {
                                                    idecUVHPContribution = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()])
                                                        + Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.total_uvhp_interest_amount.ToString().ToUpper()]);
                                                }
                                                if (idecEEContribution > 0 || idecUVHPContribution > 0)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UV&HP/EE)";
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRows[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                    lbusQDROOffsetDetails.idecQDROOffset = idecEEContribution + idecUVHPContribution;
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                                }

                                            }

                                        }
                                        else if (astrEEFlag == busConstant.FLAG_YES)
                                        {
                                            if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(EE)"; ;
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRows[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.non_vested_ee_amount.ToString().ToUpper()])
                                                    + Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.non_vested_ee_interest.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                            }
                                        }
                                        else if (astrUVHPFlag == busConstant.FLAG_YES)
                                        {
                                            if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UVHP)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRows[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.total_uvhp_contribution_amount.ToString().ToUpper()])
                                                    + Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.total_uvhp_interest_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                        else if (astrL52SplAccFlag == busConstant.FLAG_YES)
                                        {
                                            if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.local52_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local52 Special Account)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRows[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.local52_special_acct_bal_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                        else if (astrL161SplAccFlag == busConstant.FLAG_YES)
                                        {
                                            if (Convert.ToString(ldrRows[enmBenefitCalculationDetail.local161_special_acct_bal_amount.ToString().ToUpper()]).IsNotNullOrEmpty())
                                            {
                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local161 Special Account)";
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.iintBenefitCalculationId = Convert.ToInt32(ldrRows[enmBenefitCalculationDetail.benefit_calculation_header_id.ToString().ToUpper()]);
                                                lbusQDROOffsetDetails.idecQDROOffset = Convert.ToDecimal(ldrRows[enmBenefitCalculationDetail.local161_special_acct_bal_amount.ToString().ToUpper()]);
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                            }
                                        }
                                    }
                                }
                                # endregion QDRO Offset for SubPlans (Final Approved Withdrawal)

                                #region QDRO Offset for SubPlans (QDRO Approved Estimate)

                                if ((ldtbQDROOffsetSubPlans.IsNull() || ldtbQDROOffsetSubPlans.Rows.Count <= 0))
                                {
                                    ldtlbQRDOOffsetSubPlansEstimates.Clear();
                                    ldtlbQRDOOffsetSubPlansEstimates = Select("cdoQdroCalculationHeader.GetQDROOffSetEstimateDetails", new object[3] { aintParticiapantID, lintAlternatePayeeId, aintPlanId });

                                    decimal idecEEContribution = 0;
                                    decimal idecUVHPContribution = 0;

                                    if (ldtlbQRDOOffsetSubPlansEstimates.Rows.Count > 0)
                                    {
                                        Collection<busQdroCalculationDetail> lcblQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
                                        lclbQdroCalculationDetail = GetCollection<busQdroCalculationDetail>(ldtlbQRDOOffsetSubPlansEstimates, "icdoQdroCalculationDetail");
                                        foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lclbQdroCalculationDetail)
                                        {

                                            if (astrEEFlag == busConstant.FLAG_YES && astrUVHPFlag == busConstant.FLAG_YES)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                                {
                                                    idecEEContribution = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_interest_amount;
                                                }

                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                                {
                                                    idecUVHPContribution = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp_interest;
                                                }

                                                busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UV&HP/EE)";
                                                lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                lbusQDROOffsetDetails.idecQDROOffset = idecEEContribution + idecUVHPContribution;
                                                lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                            }
                                            else if (astrEEFlag == busConstant.FLAG_YES)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_YES)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(EE)";
                                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_ee_contribution
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_interest_amount; 
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);
                                                }
                                            }
                                            else if (astrUVHPFlag == busConstant.FLAG_YES)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_YES)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(UVHP)";
                                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp
                                                        + lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_uvhp_interest;
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                }
                                            }
                                            else if (astrL52SplAccFlag == busConstant.FLAG_YES)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local52 Special Account)";
                                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                }
                                            }
                                            else if (astrL161SplAccFlag == busConstant.FLAG_YES)
                                            {
                                                if (lbusQdroCalculationDetail.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                                {
                                                    busQDROOffsetDetails lbusQDROOffsetDetails = new busQDROOffsetDetails();
                                                    lbusQDROOffsetDetails.istrParticipantMpid = lstrParticipantMpid;
                                                    lbusQDROOffsetDetails.istrAlternatePayeeMpid = lstrAlternateMpid;
                                                    lbusQDROOffsetDetails.istrPlanName = lstrPlanName + " " + "(Local161 Special Account)";
                                                    lbusQDROOffsetDetails.iintQDROCalculationId = lbusQdroCalculationDetail.icdoQdroCalculationDetail.qdro_calculation_header_id;
                                                    lbusQDROOffsetDetails.istrBenefitTypeValue = astrBenefitTypeValue;
                                                    lbusQDROOffsetDetails.idecQDROOffset = lbusQdroCalculationDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                                                    lbusBenefitCalculationDetail.iclbQDROOffsetDetails.Add(lbusQDROOffsetDetails);

                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion  #region QDRO Offset for SubPlans (QDRO Approved Estimate)
                            }
                        }
                    }

                }
                #endregion QDRO Offset for SubPlans
            }

            lbusBenefitCalculationDetail.iclbQDROOffsetDetails.OrderBy(i => i.istrPlanName).ToArray();
            return lbusBenefitCalculationDetail;
        }

        public void LoadData(int aintBenefitCalculationHeaderId, int aintPlanId, int aintPersonAccountId, DateTime adtVestedDate, string astrRetirementSubType, string astrBenefitType, DateTime? dtRetirement = null)
        {
            if (astrBenefitType == busConstant.BENEFIT_TYPE_RETIREMENT)
            {
                this.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                this.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                this.icdoBenefitCalculationDetail.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
                this.icdoBenefitCalculationDetail.person_account_id = aintPersonAccountId;
                this.icdoBenefitCalculationDetail.plan_id = aintPlanId;
                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.MPIPP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.MPIPP;
                }
                else if (aintPlanId == busConstant.IAP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.IAP;
                }
                this.icdoBenefitCalculationDetail.vested_date = adtVestedDate;
                this.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.benefit_subtype_value = astrRetirementSubType;
                this.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            }
            else if (astrBenefitType == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            {
                this.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                this.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                this.icdoBenefitCalculationDetail.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
                this.icdoBenefitCalculationDetail.person_account_id = aintPersonAccountId;
                this.icdoBenefitCalculationDetail.plan_id = aintPlanId;
                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.MPIPP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.MPIPP;
                }
                else if (aintPlanId == busConstant.IAP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.IAP;
                }
                this.icdoBenefitCalculationDetail.vested_date = adtVestedDate;
                this.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.benefit_subtype_value = astrRetirementSubType;
                this.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;

            }
            else if (astrBenefitType == busConstant.BENEFIT_TYPE_DISABILITY)
            {
                this.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                this.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                this.icdoBenefitCalculationDetail.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
                this.icdoBenefitCalculationDetail.person_account_id = aintPersonAccountId;
                this.icdoBenefitCalculationDetail.plan_id = aintPlanId;
                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.MPIPP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.MPIPP;
                }
                else if (aintPlanId == busConstant.IAP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.IAP;
                }
                this.icdoBenefitCalculationDetail.vested_date = adtVestedDate;
                this.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.benefit_subtype_value = astrRetirementSubType;
                this.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
                this.icdoBenefitCalculationDetail.retirement_date = Convert.ToDateTime(dtRetirement);
            }
            else if (astrBenefitType == busConstant.BENEFIT_TYPE_WITHDRAWAL)
            {
                this.iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
                this.iclbBenefitCalculationYearlyDetail = new Collection<busBenefitCalculationYearlyDetail>();
                this.icdoBenefitCalculationDetail.benefit_calculation_header_id = aintBenefitCalculationHeaderId;
                this.icdoBenefitCalculationDetail.person_account_id = aintPersonAccountId;
                this.icdoBenefitCalculationDetail.plan_id = aintPlanId;
                if (aintPlanId == busConstant.MPIPP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.MPIPP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.MPIPP;
                }
                else if (aintPlanId == busConstant.IAP_PLAN_ID)
                {
                    this.icdoBenefitCalculationDetail.istrPlanCode = busConstant.IAP;
                    this.icdoBenefitCalculationDetail.istrPlanDescription = busConstant.IAP;
                }
                this.icdoBenefitCalculationDetail.vested_date = adtVestedDate;
                this.icdoBenefitCalculationDetail.benefit_subtype_id = busConstant.RETIREMENT_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.benefit_subtype_value = astrRetirementSubType;
                this.icdoBenefitCalculationDetail.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
                this.icdoBenefitCalculationDetail.status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            }
        }

      
	}
}
