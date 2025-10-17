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
using System.Linq;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using System.Windows.Forms;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{

     [Serializable]
    public class busMssQdroCalculationHeader : busQdroCalculationHeader
    {

         public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

         public Collection<cdoPlan> GetPlanValues()
         {
             Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
             DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetPlanFromDROApplication", new object[1] { this.icdoQdroCalculationHeader.person_id });
             if (ldtplan.Rows.Count > 0)
             {
                 foreach (DataRow ldtRow in ldtplan.Rows)
                 {
                     cdoPlan lcdoPlan = null;
                     if (Convert.ToString(ldtRow[enmDroBenefitDetails.plan_id.ToString()]).IsNotNullOrEmpty())
                     {
                         int lintPlanD = Convert.ToInt32(ldtRow[enmDroBenefitDetails.plan_id.ToString()]);
                         if (lintPlanD != 1)
                         {
                             int lintDroAppId = Convert.ToInt32(ldtRow[enmDroBenefitDetails.dro_application_id.ToString()]);
                             if (lintDroAppId > 0)
                             {
                                 DataTable ldtDROCalc = busBase.Select("cdoMssBenefitCalculationHeader.GetCalculationForDRO", new object[2] { lintDroAppId,lintPlanD });
                                 if (ldtDROCalc.Rows.Count > 0)
                                 {
                                     lcdoPlan = new cdoPlan();
                                     lcdoPlan.LoadData(ldtRow);
                                     if (lColPlans.Where(item => item.plan_id == lcdoPlan.plan_id).Count() == 0)
                                     {
                                         lColPlans.Add(lcdoPlan);
                                     }
                                 }
                             }
                         }
                         else
                         {
                             
                             lcdoPlan = new cdoPlan();
                             lcdoPlan.LoadData(ldtRow);
                             if (lColPlans.Where(item => item.plan_id == lcdoPlan.plan_id).Count() == 0)
                             {
                                 lColPlans.Add(lcdoPlan);
                             }
                         }
                     }
                 }
                 //lColPlans = doBase.LoadData<cdoPlan>(ldtplan);
             }

             return lColPlans;
         }

         public Collection<cdoPerson> GetAlternatePayees(int aintplanid)
         {
             Collection<cdoPerson> lColPerson = new Collection<cdoPerson>();
             DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetAlternatePayeeID", new object[2] { this.icdoQdroCalculationHeader.person_id,aintplanid });
             if (ldtplan.Rows.Count > 0)
             {
                 lColPerson = doBase.LoadData<cdoPerson>(ldtplan);
             }

             return lColPerson;
         }

         public void FillQualifiedApplicationDetails()
         {
             DataTable ldtDROApp = busBase.Select("cdoMssBenefitCalculationHeader.GetApplicationForDRO", new object[3] { this.icdoQdroCalculationHeader.iintPlanId,this.icdoQdroCalculationHeader.person_id,this.icdoQdroCalculationHeader.alternate_payee_id });
             if (ldtDROApp.IsNotNull() && ldtDROApp.Rows.Count > 0)
             {
                 int lintDroAppId = Convert.ToInt32(ldtDROApp.Rows[0][enmDroApplication.dro_application_id.ToString()]);
                 this.icdoQdroCalculationHeader.qdro_application_id = lintDroAppId;
                 //lbusQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId = aintPlanId;
                 this.icdoQdroCalculationHeader.PopulateDescriptions();

                 this.ibusQdroApplication = new busQdroApplication();

                 if (this.ibusQdroApplication.FindQdroApplication(lintDroAppId))
                 {
                     //lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.person_id;
                     //lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.alternate_payee_id;
                     this.icdoQdroCalculationHeader.date_of_marriage = this.ibusQdroApplication.icdoDroApplication.date_of_marriage;
                     this.icdoQdroCalculationHeader.date_of_seperation = this.ibusQdroApplication.icdoDroApplication.date_of_divorce;

                     this.ibusQdroApplication.LoadBenefitDetails();

                     if (this.ibusQdroApplication.iclbDroBenefitDetails != null &&
                         (this.ibusQdroApplication.iclbDroBenefitDetails.Count > 0))
                     {
                         if (this.ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == 1).Count() != 0)
                             this.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap = busConstant.FLAG_YES;
                     }

                     this.ibusParticipant.FindPerson(this.icdoQdroCalculationHeader.person_id);
                     this.ibusAlternatePayee.FindPerson(this.icdoQdroCalculationHeader.alternate_payee_id);

                     if (this.ibusQdroApplication.icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
                         this.icdoQdroCalculationHeader.is_participant_disabled = busConstant.FLAG_YES;
                     this.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                     #region Set Data for Disabilty

                     this.ibusBenefitApplicationForDisability.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                     this.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                     this.ibusBenefitApplicationForDisability.ibusPerson = this.ibusParticipant;
                     this.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = this.ibusParticipant.iclbPersonAccount;
                     this.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_MPI =
                                                                                     this.ibusBenefitApplication.aclbPersonWorkHistory_MPI;
                     this.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_IAP =
                                                                                     this.ibusBenefitApplication.aclbPersonWorkHistory_IAP;
                     this.ibusBenefitApplicationForDisability.Eligible_Plans =
                                                                                     this.ibusBenefitApplication.Eligible_Plans;


                     #endregion

                     this.ibusBenefitApplication.DetermineVesting();
                     this.iblnVestingHasBeenChecked = true;
                     this.GetEarliestRetiremenDate();
                     this.icdoQdroCalculationHeader.qdro_commencement_date = this.icdoQdroCalculationHeader.retirement_date;
                     //if (lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.is_participant_dead_flag == busConstant.FLAG_YES)
                     //    lbusQdroCalculationHeader.iblnIsParticipantDead = busConstant.FLAG_YES;        

                 }
             }

         }

         public override void BeforePersistChanges()
         {
             this.icdoQdroCalculationHeader.mss_flag = busConstant.FLAG_YES;
             FillQualifiedApplicationDetails();
             BeforeValidate(utlPageMode.All);

             base.BeforePersistChanges();
         }

         public override int PersistChanges()
         {
             foreach (busQdroCalculationDetail lbusQdroDetail in this.iclbQdroCalculationDetail)
             {
                 if (lbusQdroDetail.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                 {
                     DataTable ldtGrossAmount = busBase.Select("cdoMssBenefitCalculationHeader.GetGrossAmountPartForIAP", new object[1] { lbusQdroDetail.icdoQdroCalculationDetail.qdro_application_detail_id });
                     if (ldtGrossAmount.IsNotNull() && ldtGrossAmount.Rows.Count > 0)
                     {
                         if(Convert.ToString(ldtGrossAmount.Rows[0]["Gross_Amount"]).IsNotNullOrEmpty())
                         {
                             lbusQdroDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion= Convert.ToDecimal(ldtGrossAmount.Rows[0]["Gross_Amount"]);
                             lbusQdroDetail.icdoQdroCalculationDetail.participant_benefit_amount = lbusQdroDetail.icdoQdroCalculationDetail.early_reduced_benefit_amount -
                                 lbusQdroDetail.icdoQdroCalculationDetail.alt_payee_amt_before_conversion;
                         }
                     }
                 }
             }
             return base.PersistChanges();
         }

         /*
         private void Setup_MSS_QDRO_Calculations()
         {
             DateTime ldtVestedDate = DateTime.MinValue;
             int lintDROApplicationDetailId = 0;

             #region SETUP BENEFIT CALCULATION DETAIL BASED ON WHAT ESTIMATE HAS BEEN ASKED FOR

             if (this.iclbQdroCalculationDetail == null)
             {
                 this.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
             }

             if (this.icdoQdroCalculationHeader.iintPlanId == busConstant.MPIPP_PLAN_ID)
             {
                 #region Setup Detail Records for MPIs Estimate

                 // Create one Detail Record for MPIPP
                 ldtVestedDate = DateTime.MinValue;

                 //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                 if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.MPIPP))
                 {
                     ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                     //Abhi-Code Added since if Participant is not NOT-VESTED how can you pay Alternate Payee from that
                     iblnIsParticipantVested = true;
                 }

                 if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                     (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                        item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).IsNullOrEmpty()))
                 {
                     lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                        item => item.icdoDroBenefitDetails.plan_id == busConstant.MPIPP_PLAN_ID &&
                            item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                 }

                 if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                 {
                     busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                     lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;

                     lbusQdroCalculationDetail.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.MPIPP,
                     this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                         this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                     ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                     busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true, this.idecParticipantAmount);

                     this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                 }

                 else if (!iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                 {
                     iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.retired_participant_amount = this.idecParticipantAmount;
                     iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                     iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                     iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.uvhp_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.ee_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.benefit_subtype_value =
                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).
                             First().icdoPersonAccount.istrRetirementSubType;
                 }

                 if (this.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE &&
                     icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES)
                 {
                     // Create one Detail Record for IAP Plan as well
                     ldtVestedDate = DateTime.MinValue;

                     //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                     if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                     {
                         ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                         iblnIsParticipantVestedinIAP = true;
                     }

                     if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                          (!ibusQdroApplication.iclbDroBenefitDetails.Where(
                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).IsNullOrEmpty()))
                     {
                         lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                            item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                                item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                     }


                     if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                     {
                         busQdroCalculationDetail lbusQdroCalculationDetailIAP = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                         lbusQdroCalculationDetailIAP.iobjMainCDO = lbusQdroCalculationDetailIAP.icdoQdroCalculationDetail;

                         lbusQdroCalculationDetailIAP.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, busConstant.IAP_PLAN_ID, busConstant.IAP,
                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).First().icdoPersonAccount.person_account_id,
                             ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                             busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                         this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetailIAP);
                     }
                     else if (!iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                     {
                         iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;

                         iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                         iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID &&
                                         item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                           this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;
                     }
                 }
                 #endregion
             }
             else if (this.icdoQdroCalculationHeader.iintPlanId == busConstant.IAP_PLAN_ID)
             {

                 #region Setup Detail Record for IAPs Estimate

                 // Create one Detail Record for IAP Plan 
                 ldtVestedDate = DateTime.MinValue;
                 busQdroCalculationDetail lbusQdroCalculationDetailIAP = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };

                 lbusQdroCalculationDetailIAP.iclbQdroCalculationOptions = new Collection<busQdroCalculationOptions>();
                 lbusQdroCalculationDetailIAP.iobjMainCDO = lbusQdroCalculationDetailIAP.icdoQdroCalculationDetail;

                 //Get the VESTED DATE FROM PERSON ACCOUNT ELIGIBILITY
                 if (this.ibusBenefitApplication.CheckAlreadyVested(busConstant.IAP))
                 {
                     ldtVestedDate = this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                     iblnIsParticipantVestedinIAP = true;
                 }

                 if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0 &&
                     (!(ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty())).IsNullOrEmpty()))
                 {
                     lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                        item => item.icdoDroBenefitDetails.plan_id == busConstant.IAP_PLAN_ID &&
                            item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id && item.istrSubPlan.IsNullOrEmpty()).First().icdoDroBenefitDetails.dro_benefit_id;
                 }

                 if ((iblIsNew && icdoQdroCalculationHeader.qdro_application_id == 0) || (iblIsNew && icdoQdroCalculationHeader.qdro_application_id != 0 && lintDROApplicationDetailId > 0))
                 {
                     lbusQdroCalculationDetailIAP.LoadData(this.icdoQdroCalculationHeader.qdro_calculation_header_id, this.icdoQdroCalculationHeader.iintPlanId, busConstant.IAP,
                         this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                         ldtVestedDate, this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.First().icdoPersonAccount.istrRetirementSubType, lintDROApplicationDetailId,
                         busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                     this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetailIAP);
                 }
                 else if (!iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID && item.icdoQdroCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_NO &&
                                                         item.icdoQdroCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_NO).IsNullOrEmpty())
                 {
                     iclbQdroCalculationDetail.Where(
                                                item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.vested_date = ldtVestedDate;
                     iclbQdroCalculationDetail.Where(
                                                item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                     iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == busConstant.IAP_PLAN_ID).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                           this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType;

                 }
                 #endregion
             }
             else
             {
                 #region Setup Detail Record for Locals

                 if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                 {
                     //Abhi-Code Added since if Participant is not NOT-VESTED how can you pay Alternate Payee from that
                     iblnIsParticipantVested = true;

                     if (iblIsNew)
                     {
                         if (ibusQdroApplication != null && ibusQdroApplication.iclbDroBenefitDetails != null && ibusQdroApplication.iclbDroBenefitDetails.Count > 0)
                         {
                             lintDROApplicationDetailId = ibusQdroApplication.iclbDroBenefitDetails.Where(
                                item => item.icdoDroBenefitDetails.plan_id == this.icdoQdroCalculationHeader.iintPlanId &&
                                    item.icdoDroBenefitDetails.dro_application_id == icdoQdroCalculationHeader.qdro_application_id).First().icdoDroBenefitDetails.dro_benefit_id;
                         }

                         busQdroCalculationDetail lbusQdroCalculationDetail = new busQdroCalculationDetail { icdoQdroCalculationDetail = new cdoQdroCalculationDetail() };
                         lbusQdroCalculationDetail.iobjMainCDO = lbusQdroCalculationDetail.icdoQdroCalculationDetail;
                         lbusQdroCalculationDetail.LoadData(icdoQdroCalculationHeader.qdro_calculation_header_id, icdoQdroCalculationHeader.iintPlanId,
                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode,
                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.person_account_id,
                             this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                             this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrRetirementSubType,
                             lintDROApplicationDetailId, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, busConstant.FLAG_NO, true);

                         this.iclbQdroCalculationDetail.Add(lbusQdroCalculationDetail);
                     }
                     else
                     {
                         if (this.ibusBenefitApplication.CheckAlreadyVested(this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(
                                                                         item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode))
                             iclbQdroCalculationDetail.Where(
                                                item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.vested_date =
                                                this.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                         iclbQdroCalculationDetail.Where(
                                                    item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.iblnIsNewRecord = false;

                         iclbQdroCalculationDetail.Where(item => item.icdoQdroCalculationDetail.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoQdroCalculationDetail.benefit_subtype_value =
                                           this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().
                                           icdoPersonAccount.istrRetirementSubType;
                     }
                 }
                 #endregion
             }

             #endregion

             #region SWITCH CASE - INITIATE CALCULATION BASED ON THE REQUIRED PLAN'S ESTIMATE
             if (!this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
             {
                 if (!this.iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                 {
                     switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                     {
                         case busConstant.Local_161:
                             CalculateLocal161BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                             break;

                         case busConstant.Local_52:
                             CalculateLocal52BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                             break;

                         case busConstant.Local_600:
                             CalculateLocal600BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                             break;

                         case busConstant.Local_666:
                             CalculateLocal666BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                             break;

                         case busConstant.LOCAL_700:
                             CalculateLocal700BenefitAmount(busConstant.BOOL_FALSE, busConstant.CodeValueAll);
                             break;
                     }
                 }

                 switch (this.ibusBenefitApplication.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == this.icdoQdroCalculationHeader.iintPlanId).First().icdoPersonAccount.istrPlanCode)
                 {

                     case busConstant.MPIPP:
                         if (this.ibusBenefitApplication.aclbPersonWorkHistory_MPI != null && this.ibusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                         {
                             this.CalculateFinalBenefitForPension(busConstant.CodeValueAll);
                         }

                         if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0 &&
                               this.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap == busConstant.FLAG_YES)
                         {
                             this.CalculateIAPBenefitAmount();
                         }
                         break;

                     case busConstant.IAP:
                         if (this.ibusBenefitApplication.aclbPersonWorkHistory_IAP != null && this.ibusBenefitApplication.aclbPersonWorkHistory_IAP.Count > 0)
                         {
                             this.CalculateIAPBenefitAmount();
                         }
                         break;
                 }
             }
             #endregion
         }

         
         public ArrayList btn_CalculateBenefitClick(int aintDroBenefitId)
         {
             bool lblnParticipantPayeeAccount = busConstant.BOOL_FALSE;
             busPayeeAccount lbusPayeeAccount = null;

             decimal ldecParticipantAmount = new decimal();

             ArrayList larrErrors = new ArrayList();
             utlError lobjError = null;

             busDroBenefitDetails lbusDroBenefitDetails = new busDroBenefitDetails { icdoDroBenefitDetails = new cdoDroBenefitDetails() };
             lbusDroBenefitDetails.FindDroBenefitDetails(aintDroBenefitId);

             busQdroApplication lbusQdroapp = new busQdroApplication { icdoDroApplication = new cdoDroApplication() };
             lbusQdroapp.FindQdroApplication(lbusDroBenefitDetails.icdoDroBenefitDetails.dro_application_id);
             {
                
                 int lintBenefitId =
                     lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id;
                 string lstrPlanCode =
                     lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanCode;
                 int lintPlanId =
                     lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id;
                 //if (lintPlanId == busConstant.IAP_PLAN_ID)
                 //{
                 //    int lintIsIapBalancePaidOut = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckIAPBalancePaidOut", new object[1] { icdoDroApplication.person_id },
                 //                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                 //    if (lintIsIapBalancePaidOut > 0)
                 //    {
                 //        lobjError = AddError(5504, string.Empty);
                 //        larrErrors.Add(lobjError);
                 //        return larrErrors;
                 //    }
                 //}


                 DataTable ldtblPayeeAccounts = busBase.Select("cdoDroApplication.GetParticipantsPayeeAccountForGivenPlan", new object[] { lbusQdroapp.icdoDroApplication.person_id, lintPlanId });

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
                     if (lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value == busConstant.DRO_MODEL_VALUE_STANDARD_RETIREE_FORMULA)
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
                                 if (lbusQdroapp.icdoDroApplication.is_participant_disabled_flag != busConstant.FLAG_YES)
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
                 lbusQdroCalculationHeader.ibusQdroApplication = lbusQdroapp;
                 lbusQdroCalculationHeader.ibusAlternatePayee = ibusAlternatePayee;

                 lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = ibusParticipant;
                 lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.LoadPersonAccounts();
                 lbusQdroCalculationHeader.ibusParticipant = ibusParticipant;
                 lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount = lbusQdroapp.ibusParticipant.iclbPersonAccount;
                 lbusQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = icdoDroApplication.dro_commencement_date;

                 lbusQdroCalculationHeader.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
                 lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                 lbusQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();


                 if (!lbusQdroapp.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                 {
                     lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution =
                         lbusQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(ibusParticipant.icdoPerson.person_id, lbusQdroapp.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                 }
                 else
                 {
                     lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = lbusQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(ibusParticipant.icdoPerson.person_id, null);
                 }

                 lbusQdroCalculationHeader.PopulateInitialDataQdroCalculationHeader(
                         ibusParticipant.icdoPerson.person_id, lbusQdroapp.icdoDroApplication.dro_application_id,
                         ibusAlternatePayee, lbusQdroapp.icdoDroApplication.dro_commencement_date,
                         lintPlanId, icdoDroApplication.date_of_marriage, icdoDroApplication.date_of_divorce, icdoDroApplication.is_participant_disabled_flag, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL);

                 lbusQdroCalculationHeader.ibusBenefitApplication.idecAge = lbusQdroCalculationHeader.icdoQdroCalculationHeader.age;

                 //Post Retirement DRO
                 //if (!lblnParticipantPayeeAccount)               
                 //{
                 lbusQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
                 lbusQdroCalculationHeader.ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_Retirement();
                 //}

                 #region If participant disabled

                 if (lbusQdroapp.icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
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
                     lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.icdoBenefitApplication.retirement_date = lbusQdroapp.icdoDroApplication.dro_commencement_date;
                     lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.DetermineBenefitSubTypeandEligibility_Disability();
                     //}
                 }

                 #endregion

                 if (lbusQdroCalculationHeader.ibusBenefitApplication.CheckAlreadyVested(lstrPlanCode))
                 {
                     busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                     lbusPlanBenefitXr.FindPlanBenefitXr(lintBenefitId);

                     lbusQdroCalculationHeader.SpawnFinalQdroCalculation(aintDroBenefitId,
                                         lbusQdroapp.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lintPlanId).FirstOrDefault().icdoPersonAccount.person_account_id,
                                         lintPlanId, lstrPlanCode,
                                         lbusQdroCalculationHeader.ibusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                         lbusQdroapp.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lintPlanId).FirstOrDefault().icdoPersonAccount.istrRetirementSubType,
                                         lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value, lblnParticipantPayeeAccount);
                 }
                 try
                 {
                     lbusQdroCalculationHeader.icdoQdroCalculationHeader.Insert();
                     lbusQdroCalculationHeader.AfterPersistChanges();
                     iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.status_value =
                             busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
                     iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.dro_benefit_id == aintDroBenefitId).FirstOrDefault().icdoDroBenefitDetails.Update();



                 }
                 catch
                 {
                 }


             }

             if (lbusQdroapp.ibusBaseActivityInstance.IsNotNull())
             {
                 lbusQdroapp.SetProcessInstanceParameters();
             }

             
             larrErrors.Add(this);

             return larrErrors;
         }
         */
    }
}
