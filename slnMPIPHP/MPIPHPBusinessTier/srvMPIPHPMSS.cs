#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Sagitec.BusinessTier;
using Sagitec.BusinessObjects;
using MPIPHP.Interface;
using Sagitec.Common;
using System.Collections;
using System.Linq;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.ExceptionPub;
using Sagitec.CorBuilder;
using System.IO;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
#endregion

namespace MPIPHP.BusinessTier
{
    /// <summary>
    /// Summary description for srvPension.
    /// </summary>
    public class srvMPIPHPMSS : srvMPIPHP
    {
        public srvMPIPHPMSS()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //FM upgrade: 6.0.0.31 changes
        //public override object GetBusinessTierDetails()
        // {
        //    if (busMPIPHPBase.iutlServerDetail == null)
        //    {
        //        busMPIPHPBase lobjN = new busMPIPHPBase();
        //    }

        //    utlServerDetail lutlServerDetail = new utlServerDetail();
        //    lutlServerDetail.istrIPAddress = busMPIPHPBase.iutlServerDetail.istrIPAddress;
        //    lutlServerDetail.istrReleaseDate = busMPIPHPBase.iutlServerDetail.istrReleaseDate;

        //    return lutlServerDetail;
        //}

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = base.ValidateNew(astrFormName, ahstParam);
            if (astrFormName == "wfmMssBenefitEstimateMaintenance")
            {
                DateTime ldtDateTime = new DateTime();
                if (Convert.ToString(ahstParam["ibusPerson.icdoPerson.date_of_birth"]).IsNotNullOrEmpty())
                {
                    ldtDateTime = Convert.ToDateTime(ahstParam["ibusPerson.icdoPerson.date_of_birth"]);
                }
                if (ldtDateTime == DateTime.MinValue || ldtDateTime.Year == 1753)
                {
                    if (larrErrors.IsNull())
                        larrErrors = new ArrayList();
                    busMSSHome lbus = new busMSSHome();
                    utlError lobjError = null;
                    lobjError = lbus.AddError(10000, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
            }
           
            return larrErrors;
        }
        #region Employment History 
        public byte[] CreateEmploymentHistory(int aintpersonid, string astrReportName)
        {
           busPerson lbusPerson = new busPerson();
           if(lbusPerson.FindPerson(aintpersonid))
           {
              return lbusPerson.CreatePDFReport(astrReportName);
           }
           return null;
        }

        public string CreateCorrespondenceForMss(Dictionary<string, object> adictParams, string astrTemplateName, int aintpersonid, int aintplanid)
        {
            string lstrGeneratedFileName = string.Empty;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            if (lbusPerson.FindPerson(aintpersonid))
            {
                ArrayList larrList = new ArrayList();
                if (astrTemplateName == busConstant.MSS.WORK_HISTORY_REQUEST_MSS || astrTemplateName == busConstant.MSS.MSS_PENSION_IAP_VERIFICATION)
                {
                    larrList.Add(lbusPerson);
                }
                else
                {
                    busMSSPlan lbusMSSPlan = new busMSSPlan();
                    busPayeeAccount lbusPayeeAccount = lbusMSSPlan.GetPayeeAccountObject(aintpersonid, aintplanid);
                    if (lbusPayeeAccount.IsNotNull())
                    {
                        lbusPayeeAccount.ibusParticipant = lbusPerson;
                        larrList.Add(lbusPayeeAccount);
                    }

                }
                string lstrFilePath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
                lstrGeneratedFileName = this.CreateCorrespondence(astrTemplateName, larrList, null, adictParams);
                lstrGeneratedFileName = lstrFilePath + lstrGeneratedFileName;
            }
            return lstrGeneratedFileName;
        }

        #endregion


        #region Home Page
        public busMSSHome FindMSSHome(int aintpersonid)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintpersonid))
            {
                lbusMSSHome.EvaluateInitialLoadRules();
            }
            return lbusMSSHome;
        }

        #endregion

        #region Estimates

        public busMssQdroCalculationHeader NewQdroCalculationHeader(int aintpersonid)
        {
            busMssQdroCalculationHeader lbusQdroCalculationHeader = new busMssQdroCalculationHeader { icdoQdroCalculationHeader = new cdoQdroCalculationHeader() };
            lbusQdroCalculationHeader.iclbQdroCalculationDetail = new Collection<busQdroCalculationDetail>();
            lbusQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();
            lbusQdroCalculationHeader.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
            lbusQdroCalculationHeader.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            lbusQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusQdroCalculationHeader.ibusBenefitApplicationForDisability = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

            lbusQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_id = busConstant.BenefitCalculation.CALCULATION_TYPE_CODE_ID;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_id = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_CODE_ID;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value = busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
            lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = aintpersonid;
            //lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_application_id = aintQdroApplicationId;
            //lbusQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId = aintPlanId;
            //lbusQdroCalculationHeader.icdoQdroCalculationHeader.PopulateDescriptions();

            //lbusQdroCalculationHeader.ibusQdroApplication = new busQdroApplication();

            //if (lbusQdroCalculationHeader.ibusQdroApplication.FindQdroApplication(aintQdroApplicationId))
            //{
            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.person_id;
            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.alternate_payee_id;
            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.date_of_marriage = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.date_of_marriage;
            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.date_of_seperation = lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.date_of_divorce;

            //    lbusQdroCalculationHeader.ibusQdroApplication.LoadBenefitDetails();

            //    if (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails != null &&
            //        (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails.Count > 0))
            //    {
            //        if (lbusQdroCalculationHeader.ibusQdroApplication.iclbDroBenefitDetails.Where(item => item.icdoDroBenefitDetails.plan_id == 1).Count() != 0)
            //            lbusQdroCalculationHeader.icdoQdroCalculationHeader.is_alt_payee_eligible_for_iap = busConstant.FLAG_YES;
            //    }

            //    lbusQdroCalculationHeader.ibusParticipant.FindPerson(lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id);
            //    lbusQdroCalculationHeader.ibusAlternatePayee.FindPerson(lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id);

            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lbusQdroCalculationHeader.ibusParticipant.icdoPerson.person_id;
            //    lbusQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.person_id;

            //    if (lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.is_participant_disabled_flag == busConstant.FLAG_YES)
            //        lbusQdroCalculationHeader.icdoQdroCalculationHeader.is_participant_disabled = busConstant.FLAG_YES;

            //    //if (lbusQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.is_participant_dead_flag == busConstant.FLAG_YES)
            //    //    lbusQdroCalculationHeader.iblnIsParticipantDead = busConstant.FLAG_YES;        

            //}
            if (lbusQdroCalculationHeader.ibusParticipant.FindPerson(aintpersonid))
            {
                lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusQdroCalculationHeader.ibusParticipant.LoadPersonAccounts();

                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson = lbusQdroCalculationHeader.ibusParticipant;
                lbusQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount;
                lbusQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek (Imp to have the Work-History Loaded)

                #region Set Data for Disabilty

                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();

                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson = lbusQdroCalculationHeader.ibusParticipant;
                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.ibusPerson.iclbPersonAccount = lbusQdroCalculationHeader.ibusParticipant.iclbPersonAccount;
                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_MPI =
                                                                                lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_MPI;
                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.aclbPersonWorkHistory_IAP =
                                                                                lbusQdroCalculationHeader.ibusBenefitApplication.aclbPersonWorkHistory_IAP;
                lbusQdroCalculationHeader.ibusBenefitApplicationForDisability.Eligible_Plans =
                                                                                lbusQdroCalculationHeader.ibusBenefitApplication.Eligible_Plans;


                #endregion

                lbusQdroCalculationHeader.ibusBenefitApplication.DetermineVesting();
                lbusQdroCalculationHeader.iblnVestingHasBeenChecked = true;
                lbusQdroCalculationHeader.GetEarliestRetiremenDate();

                lbusQdroCalculationHeader.icdoQdroCalculationHeader.qdro_commencement_date = lbusQdroCalculationHeader.icdoQdroCalculationHeader.retirement_date;

                lbusQdroCalculationHeader.icdoQdroCalculationHeader.ienuObjectState = ObjectState.Insert;
                lbusQdroCalculationHeader.iarrChangeLog.Add(lbusQdroCalculationHeader.icdoQdroCalculationHeader);

                lbusQdroCalculationHeader.EvaluateInitialLoadRules();
            }
            return lbusQdroCalculationHeader;
        }

        public busMssQdroCalculationHeader FindMssQdroCalculationHeader(int aintqdroheaderid)
        {
            busMssQdroCalculationHeader lobjQdroCalculationHeader = new busMssQdroCalculationHeader();
            if (lobjQdroCalculationHeader.FindQdroCalculationHeader(aintqdroheaderid))
            {
                //lobjQdroCalculationHeader.ibusQdroApplication = new busQdroApplication();
                lobjQdroCalculationHeader.ibusBenefitCalculationHeader = new busBenefitCalculationHeader();
                lobjQdroCalculationHeader.ibusParticipant = new busPerson();
                lobjQdroCalculationHeader.ibusAlternatePayee = new busPerson();
                lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                lobjQdroCalculationHeader.ibusParticipant.iclbPersonContact = new Collection<busPersonContact>();

                //if (lobjQdroCalculationHeader.ibusQdroApplication.FindQdroApplication(lobjQdroCalculationHeader.icdoQdroCalculationHeader.qdro_application_id))
                //{
                //    lobjQdroCalculationHeader.icdoQdroCalculationHeader.person_id = lobjQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.person_id;
                //    lobjQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id = lobjQdroCalculationHeader.ibusQdroApplication.icdoDroApplication.alternate_payee_id;

                //}

                lobjQdroCalculationHeader.ibusParticipant.FindPerson(lobjQdroCalculationHeader.icdoQdroCalculationHeader.person_id);
                lobjQdroCalculationHeader.ibusAlternatePayee.FindPerson(lobjQdroCalculationHeader.icdoQdroCalculationHeader.alternate_payee_id);

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintAltPayeeAgeAtRetirement =
                       Convert.ToInt32(Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(Convert.ToDateTime(lobjQdroCalculationHeader.ibusAlternatePayee.icdoPerson.idtDateofBirth),
                        lobjQdroCalculationHeader.GetRetirementDateforCalculation())));

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintParticipantAtRetirement = Convert.ToInt32(Math.Floor(lobjQdroCalculationHeader.icdoQdroCalculationHeader.age));

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.istrSurvivorMPID =
                        lobjQdroCalculationHeader.ibusAlternatePayee.icdoPerson.mpi_person_id;

                lobjQdroCalculationHeader.ibusParticipant.LoadPersonAccounts();
                lobjQdroCalculationHeader.LoadQdroCalculationDetails();
                //lobjQdroCalculationHeader.LoadDisabilityRetireeIncreases();

                //lobjQdroCalculationHeader.ibusParticipant.LoadPersonContacts();

                foreach (busQdroCalculationDetail lbusQdroCalculationDetail in lobjQdroCalculationHeader.iclbQdroCalculationDetail)
                {

                    lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrPlanCode =
                        lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                            lbusQdroCalculationDetail.icdoQdroCalculationDetail.plan_id).First().icdoPersonAccount.istrPlanCode;

                    lbusQdroCalculationDetail.LoadPlanDescription();
                    //lbusQdroCalculationDetail.LoadQdroCalculationOptionss();
                    //lbusQdroCalculationDetail.LoadQdroCalculationYearlyDetails();
                    //lbusQdroCalculationDetail.LoadQdroIapAllocationDetails();

                    if (lobjQdroCalculationHeader.icdoQdroCalculationHeader.is_participant_disabled == busConstant.FLAG_YES)
                        lbusQdroCalculationDetail.icdoQdroCalculationDetail.istrRetirementTypeDisability = busConstant.BENEFIT_TYPE_DISABILITY_DESC;
                }

                lobjQdroCalculationHeader.icdoQdroCalculationHeader.istrRetirementType = lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.benefit_subtype_value;
                //Code to Fill Proper PLAN ID in the HEADER OBJECT
                if (lobjQdroCalculationHeader.iclbQdroCalculationDetail.Count > 1 && (!lobjQdroCalculationHeader.iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty()))
                {
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId = lobjQdroCalculationHeader.iclbQdroCalculationDetail.Where(
                        item => item.icdoQdroCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoQdroCalculationDetail.plan_id;
                }
                else
                {
                    lobjQdroCalculationHeader.icdoQdroCalculationHeader.iintPlanId =
                        lobjQdroCalculationHeader.iclbQdroCalculationDetail.FirstOrDefault().icdoQdroCalculationDetail.plan_id;
                }

                if (!lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lobjQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(lobjQdroCalculationHeader.ibusParticipant.icdoPerson.person_id,
                        lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lobjQdroCalculationHeader.ibusBenefitCalculationHeader.LoadAllRetirementContributions(lobjQdroCalculationHeader.ibusParticipant.icdoPerson.person_id, null);
                }

                // Initial Setup for Checking Eligbility
                //lobjQdroCalculationHeader.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                //lobjQdroCalculationHeader.ibusBenefitApplicationForDisability = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                //lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                //lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                //lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson = lobjQdroCalculationHeader.ibusParticipant;
                //lobjQdroCalculationHeader.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lobjQdroCalculationHeader.ibusParticipant.iclbPersonAccount;
                //lobjQdroCalculationHeader.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lobjQdroCalculationHeader.GetRetirementDateforCalculation();
                //lobjQdroCalculationHeader.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek (Imp to have the Work-History Loaded)

                //if(lobjQdroCalculationHeader.icdoQdroCalculationHeader.calculation_type_value == busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE)
                //    lobjQdroCalculationHeader.GetEarliestRetiremenDate(); 

            }
            return lobjQdroCalculationHeader;
        }

        //public busMssBenefitCalculationHeader FindMssBenefitCalculationHeader(int aintBenefitMssCalculationHeaderId)
        //{
        //    busMssBenefitCalculationHeader lobjMssBenefitCalculationHeader = new busMssBenefitCalculationHeader();
        //    if (lobjMssBenefitCalculationHeader.FindMssBenefitCalculationHeader(aintBenefitMssCalculationHeaderId))
        //    {
        //        lobjMssBenefitCalculationHeader.LoadMssBenefitCalculationDetails();
        //    }

        //    return lobjMssBenefitCalculationHeader;
        //}

        //public busMssBenefitCalculationHeader NewMssBenefitCalculationHeader()
        //{
        //    busMssBenefitCalculationHeader lobjMssBenefitCalculationHeader = new busMssBenefitCalculationHeader();
        //                lobjMssBenefitCalculationHeader.icdoMssBenefitCalculationHeader = new cdoMssBenefitCalculationHeader();
        //    return lobjMssBenefitCalculationHeader;
        //}

		public busMssBenefitCalculationYearlyDetail FindMssBenefitCalculationYearlyDetail(int aintBenefitMssCalculationYearlyDetailId)
		{
			busMssBenefitCalculationYearlyDetail lobjMssBenefitCalculationYearlyDetail = new busMssBenefitCalculationYearlyDetail();
			if (lobjMssBenefitCalculationYearlyDetail.FindBenefitCalculationYearlyDetail(aintBenefitMssCalculationYearlyDetailId))
			{
			}

			return lobjMssBenefitCalculationYearlyDetail;
		}

		public busMssBenefitCalculationYearlyDetail NewMssBenefitCalculationYearlyDetail()
		{
			busMssBenefitCalculationYearlyDetail lobjMssBenefitCalculationYearlyDetail = new busMssBenefitCalculationYearlyDetail();
                        lobjMssBenefitCalculationYearlyDetail.icdoBenefitCalculationYearlyDetail = new cdoBenefitCalculationYearlyDetail();
			return lobjMssBenefitCalculationYearlyDetail;
		}

		public busMssBenefitCalculationOptions FindMssBenefitCalculationOptions(int aintBenefitMssCalculationOptionId)
		{
			busMssBenefitCalculationOptions lobjMssBenefitCalculationOptions = new busMssBenefitCalculationOptions();
			if (lobjMssBenefitCalculationOptions.FindBenefitCalculationOptions(aintBenefitMssCalculationOptionId))
			{
			}

			return lobjMssBenefitCalculationOptions;
		}

        //public busMssBenefitCalculationOptions NewMssBenefitCalculationOptions()
        //{
        //    busMssBenefitCalculationOptions lobjMssBenefitCalculationOptions = new busMssBenefitCalculationOptions();
        //                lobjMssBenefitCalculationOptions.icdoBenefitCalculationOptions = new cdoMssBenefitCalculationOptions();
        //    return lobjMssBenefitCalculationOptions;
        //}

		public busMssBenefitCalculationDetail FindMssBenefitCalculationDetail(int aintBenefitMssCalculationDetailId)
		{
			busMssBenefitCalculationDetail lobjMssBenefitCalculationDetail = new busMssBenefitCalculationDetail();
			if (lobjMssBenefitCalculationDetail.FindBenefitCalculationDetail(aintBenefitMssCalculationDetailId))
			{
				lobjMssBenefitCalculationDetail.LoadBenefitCalculationOptionss();
				lobjMssBenefitCalculationDetail.LoadBenefitCalculationYearlyDetails();
			}

			return lobjMssBenefitCalculationDetail;
		}

		public busMssBenefitCalculationDetail NewMssBenefitCalculationDetail()
		{
			busMssBenefitCalculationDetail lobjMssBenefitCalculationDetail = new busMssBenefitCalculationDetail();
                        lobjMssBenefitCalculationDetail.icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail();
			return lobjMssBenefitCalculationDetail;
		}

        public busMssBenefitCalculationRetirement NewBenefitCalculationRetirement(string astr_person_mpi_id, string astr_benefit_type, int aint_plan_id) 
        {
            busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusPerson.FindPerson(astr_person_mpi_id);
            lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


            lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();



            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id,
                                                                                        busConstant.ZERO_INT, busConstant.ZERO_INT, astr_benefit_type, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                        DateTime.MinValue, busConstant.ZERO_DECIMAL, aint_plan_id);
           
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
            lbusBenefitCalculationRetirement.iarrChangeLog.Add(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader);
           
            return lbusBenefitCalculationRetirement;
        }

        public busMssBenefitCalculationRetirement NewBenefitCalculationRetirementWizard(int aintpersonid)
        {
            busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusPerson.FindPerson(aintpersonid);
            lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)


            lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();



            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id,
                                                                                       busConstant.ZERO_INT, busConstant.ZERO_INT, busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                     DateTime.MinValue, busConstant.ZERO_DECIMAL, 0);
            if (lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationRetirement.GetDefaultRetirementDateForTheParticipant();
            } 
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.mss_flag = busConstant.FLAG_YES;
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
            lbusBenefitCalculationRetirement.iarrChangeLog.Add(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader);

            return lbusBenefitCalculationRetirement;
        }

        public busMssBenefitCalculationRetirement FindBenefitCalculationRetirement(int aintBenefitCalculationId)
        {
            busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement();
            if (lbusBenefitCalculationRetirement.FindBenefitCalculationHeader(aintBenefitCalculationId))
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date);

                //DataTable ldtbPersonMPID = busBase.Select("cdoPerson.GetPersonMPID", new object[1] { lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_id });
                //if (ldtbPersonMPID.Rows.Count > 0)
                //{
                //    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrSurvivorMPID = ldtbPersonMPID.Rows[0][enmPerson.mpi_person_id.ToString()].ToString();
                //}

                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.PopulateDescriptions();
                lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                lbusBenefitCalculationRetirement.ibusPerson.FindPerson(lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.person_id);
                lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();
                lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                if (!lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationRetirement.LoadAllRetirementContributions(lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationRetirement.LoadAllRetirementContributions(null);
                }
                lbusBenefitCalculationRetirement.LoadBenefitCalculationDetails();

                // Initial Setup for Checking Eligbility
                ////lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                ////lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                ////lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                ////lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
                //lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;
                //lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date;
                //lbusBenefitCalculationRetirement.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date);

                //lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added -Abhishek (Imp to have work history state in background)

                if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 1)
                {
                    {
                        if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                    }
                }
                else if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.plan_id;
                }

                if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.FirstOrDefault().icdoBenefitCalculationDetail.benefit_subtype_value;
                }
            }
            return lbusBenefitCalculationRetirement;
        }

        #endregion

        #region Person Profile

        public busPerson FindPerson(int aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                lobjPerson.istrFullName = "Welcome, " + lobjPerson.icdoPerson.first_name + " " + lobjPerson.icdoPerson.last_name;
                lobjPerson.ibusPersonAddress = new busPersonAddress() { icdoPersonAddress = new cdoPersonAddress() };
                lobjPerson.ibusPersonAddress.LoadActiveAddress(aintpersonid);
                lobjPerson.LoadActiveAddressOfMember();
                lobjPerson.LoadPersonContacts();
            }
            return lobjPerson;
        }

        public busPerson LoadPerson(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintPersonID))
            {
                lobjPerson.CheckIFRetireeForMss();
                //if (lobjPerson.CheckMemberIsRetiree())
                //{
                //    lobjPerson.istrRetiree = busConstant.FLAG_YES;
                //}
            }
            return lobjPerson;
        }


        public busPerson LoadPersonWithMPID(string astrMpiPersonId)
        {

            busPerson lbusPerson = new busPerson();
            DataTable ldtbPerson = busBase.Select<cdoPerson>(new string[1] { enmPerson.mpi_person_id.ToString() }, new object[1] { astrMpiPersonId }, null, null);
            if (ldtbPerson.Rows.Count > 0)
            {
                lbusPerson.icdoPerson = new cdoPerson();
                lbusPerson.icdoPerson.LoadData(ldtbPerson.Rows[0]);
                lbusPerson.CheckIFRetireeForMss();
            }

            return lbusPerson;
        }

        public busPersonContact FindPersonContact(int aintpersoncontactid)
        {
            busPersonContact lbusPersonContact = new busPersonContact();
            if (lbusPersonContact.FindPersonContact(aintpersoncontactid))
            {
            }
            return lbusPersonContact;
        }

        #endregion

        #region Plan Details
        public busPerson FindPersonBeneficiariesandDependents(int aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                lobjPerson.istrFullName = "Welcome, " + lobjPerson.icdoPerson.first_name + " " + lobjPerson.icdoPerson.last_name;
                lobjPerson.ibusPersonAddress = new busPersonAddress() { icdoPersonAddress = new cdoPersonAddress() };
                lobjPerson.ibusPersonAddress.LoadActiveAddress(aintpersonid);
                lobjPerson.LoadBeneficiaries();
                lobjPerson.LoadPersonDependents();
            }
            return lobjPerson;
        }

        public busPersonOverview FindPersonPlan(int aintpersonid)
        {
            busPersonOverview lobjPerson = new busPersonOverview();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                //lobjPerson.LoadParticipantPlan();
                //lobjPerson.LoadBeneficiaries();
                //lobjPerson.LoadBenefitApplication();
                //lobjPerson.LoadWorkHistory();
                lobjPerson.LoadPlanDetails();

            }
            return lobjPerson;
        }

        public busMSSPlan FindPersonAccount(int aintpersonaccountid,string istrretiree)
        {
            busMSSPlan lobjPersonAccount = new busMSSPlan();
            if(lobjPersonAccount.FindPersonAccount(aintpersonaccountid))
            {
                lobjPersonAccount.ibusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
                busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusPlan.FindPlan(lobjPersonAccount.icdoPersonAccount.plan_id);
                lobjPersonAccount.icdoPersonAccount.istrPlanDesc = lbusPlan.icdoPlan.plan_name;
                lobjPersonAccount.LoadBeneficiariesForPersonAccount(aintpersonaccountid);
                lobjPersonAccount.CheckReemploymentStatus(aintpersonaccountid);
                lobjPersonAccount.GetPayeeAccountObject(lobjPersonAccount.icdoPersonAccount.person_id, lobjPersonAccount.icdoPersonAccount.plan_id);
                if (istrretiree == busConstant.FLAG_YES)
                {
                    lobjPersonAccount.istrIsRetiree = busConstant.FLAG_YES;
                }
            }
            return lobjPersonAccount;
        }

        public DataTable GetPlanValues(int aintpersonid)
        {
            return DBFunction.DBSelect("cdoMssBenefitCalculationHeader.GetPlanForMember", new object[1] { aintpersonid },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        public DataTable GetBenefitTypes(int aintpersonid)
        {
            int lintQualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoMssBenefitCalculationHeader.GetCountOfQualifiedDRO", new object[1] { aintpersonid }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintQualifiedDROExists > 0)
            {
                return DBFunction.DBSelect("cdoMssBenefitCalculationHeader.GetBenefitTypesForMssPlusQDRO", new object[1] { aintpersonid },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                return DBFunction.DBSelect("cdoMssBenefitCalculationHeader.GetBenefitTypesForMss", new object[1] { aintpersonid },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

        }

        #endregion

        #region ABS
        public busMssAnnualBenefitSummary FindMssAnnualBenefitSummary(int aintpersonid)
        {
            busMssAnnualBenefitSummary lobjbusAnnualBenefitSummaryOverview = new busMssAnnualBenefitSummary();
            if (lobjbusAnnualBenefitSummaryOverview.FindPerson(aintpersonid))
            {
                lobjbusAnnualBenefitSummaryOverview.LoadPersonAccounts();
                lobjbusAnnualBenefitSummaryOverview.LoadBenefitApplication();
                lobjbusAnnualBenefitSummaryOverview.LoadWorkHistory(false,1);


                //lobjbusAnnualBenefitSummaryOverview.iclbHealthWorkHistory = new Collection<cdoDummyWorkData>();
                //lobjbusAnnualBenefitSummaryOverview.iclbHealthWorkHistory = lobjbusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;
                //lobjbusAnnualBenefitSummaryOverview.LoadMssPlanDetails();
            }

            return lobjbusAnnualBenefitSummaryOverview;
        }

        public busPersonOverview FindPersonOverview(int aintpersonid)
        {
            busPersonOverview lobjPersonoverview = new busPersonOverview();
            if (lobjPersonoverview.FindPerson(aintpersonid))
            {

                lobjPersonoverview.LoadInitialData(false);
                lobjPersonoverview.GetCurrentAge();
                lobjPersonoverview.LoadActiveContacts();
                lobjPersonoverview.LoadPersonAddresss();
                lobjPersonoverview.LoadCorrAddress();
                lobjPersonoverview.LoadBeneficiariesForOverview();
                //lobjPersonoverview.LoadParticipantPlan();
                lobjPersonoverview.LoadPersonDependents();
                lobjPersonoverview.LoadPersonDROApplications();
                lobjPersonoverview.LoadDeathNotifications();
                lobjPersonoverview.LoadBenefitApplication();
                lobjPersonoverview.LoadParticipantWorkFlows();
                lobjPersonoverview.LoadPersonNotes();
                lobjPersonoverview.LoadWorkHistory();

                if (lobjPersonoverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    lobjPersonoverview.iblnParticipant = busConstant.YES;

                lobjPersonoverview.LoadPlanDetails();
                //LoadWorkHistory updates the sgt_person record, so need to load it again
                lobjPersonoverview.icdoPerson.Select();

                //lobjPersonoverview.GetRetireeHealthHours();
                lobjPersonoverview.iclbHealthWorkHistory = new Collection<cdoDummyWorkData>();
                lobjPersonoverview.iclbHealthWorkHistory = lobjPersonoverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;

                if (!lobjPersonoverview.iclbHealthWorkHistory.IsNullOrEmpty())
                {
                    lobjPersonoverview.CheckRetireeHealthEligibilityAndUpdateFlag();
                }
                lobjPersonoverview.LoadPayeeAccount();
            }

            return lobjPersonoverview;
        }


        #endregion

        #region Payment History
        public busMssHistoryHeaderLookup LoadHistoryHeaders(DataTable adtbSearchResult)
        {
            busMssHistoryHeaderLookup lobjHistoryHeaderLookup = new busMssHistoryHeaderLookup();
            lobjHistoryHeaderLookup.LoadPaymentHistoryHeaders(adtbSearchResult);
            return lobjHistoryHeaderLookup;
        }
        #endregion

        #region Workflow

        public busMSSHome FindPersonProcess(int aintpersonid)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintpersonid))
            {
                lbusMSSHome.LoadPersonWorkflows(aintpersonid);
            }
            return lbusMSSHome;
        }


        public busMSSHome FindActivitiesForProcess(int aintprocessinstanceid,int aintprocessid, int aintpersonid)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintpersonid))
            {
                lbusMSSHome.iclbApplicationActivitiesForUser = new Collection<Application_Process>();
                lbusMSSHome.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                lbusMSSHome.ibusProcessInstance = new busProcessInstance { icdoPrevActivityInstance = new cdoActivityInstance() };
                lbusMSSHome.ibusProcessInstance.FindProcessInstance(aintprocessinstanceid);
                lbusMSSHome.ibusProcess.FindProcess(aintprocessid);
                lbusMSSHome.LoadAllActivitiesAssociatedWithTheProcess(aintprocessinstanceid);
            }
            return lbusMSSHome;
        }

        #endregion

        #region Annual Statements
        public busMSSHome FindAnnualStatements(int aintpersonid)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            if (lbusMSSHome.LoadPerson(aintpersonid))
            {
                lbusMSSHome.GetLatestAnnualStatementFile(aintpersonid);
            }
            return lbusMSSHome;
        }

        public byte[] CreateMSSMemberAnnualStatement(int aintpersonid, int lintCompYear)
        {
            busPerson lbusPerson = new busPerson();
            return lbusPerson.GenerateAnnualStatements(aintpersonid, lintCompYear);

        }
        #endregion


        #region WCF - Website and Mobile App

        public DataTable GetRetirementProcessTracker(string astrMpiPersonId)
        {
            return DBFunction.DBSelect("cdoCorPacketContent.GetRetirementProcessTracker", new object[1] { astrMpiPersonId },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        public busPayeeAccount GetPayeeAccountBreakDownDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountAchDetails();
                lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                lobjPayeeAccount.LoadPayeeAccountRetroPayments();
                lobjPayeeAccount.LoadPayeeAccountRetroPaymentDetails();

                //Payment Adjustment
                lobjPayeeAccount.LoadPayeeAccountBenefitOverPayment();
                // lobjPayeeAccount.LoadPayeeAccountOverPaymentPaymentDetails();
                lobjPayeeAccount.LoadAllRepaymentSchedules();

                lobjPayeeAccount.LoadPayeeAccountRolloverDetails();
                lobjPayeeAccount.LoadPayeeAccountStatuss();
                lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                //lobjPayeeAccount.LoadBenefitDetails();
                //lobjPayeeAccount.LoadDRODetails();
                lobjPayeeAccount.LoadNextBenefitPaymentDate();
                lobjPayeeAccount.LoadTotalRolloverAmount();
                lobjPayeeAccount.LoadGrossAmount();
                lobjPayeeAccount.LoadPayeeAccountDeduction();
                lobjPayeeAccount.LoadNonTaxableAmount();
                lobjPayeeAccount.GetCalculatedTaxAmount();
                //lobjPayeeAccount.LoadDeathNotificationStatus();
                lobjPayeeAccount.LoadWithholdingInformation();
                lobjPayeeAccount.GetCuurentPayeeAccountStatus();
                //lobjPayeeAccount.CheckAnnuity();
                lobjPayeeAccount.LoadLastPaymentDate();
                /*
                //Payee Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccount.icdoPayeeAccount.person_id);
                }
                //Organization Details
                if (lobjPayeeAccount.icdoPayeeAccount.org_id != 0)
                {
                    lobjPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    lobjPayeeAccount.ibusOrganization.FindOrganization(lobjPayeeAccount.icdoPayeeAccount.org_id);
                }

                //TransferOrg Details
                if (lobjPayeeAccount.icdoPayeeAccount.transfer_org_id != 0)
                {
                    busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    if (lbusOrganization.FindOrganization(lobjPayeeAccount.icdoPayeeAccount.transfer_org_id))
                    {
                        lobjPayeeAccount.icdoPayeeAccount.istrOrgMPID = lbusOrganization.icdoOrganization.mpi_org_id;
                        lobjPayeeAccount.icdoPayeeAccount.istrOrgName = lbusOrganization.icdoOrganization.org_name;
                    }
                }

                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }


                if (lobjPayeeAccount.icdoPayeeAccount.adjustment_payment_eligible_flag == busConstant.Flag_Yes)
                {
                    lobjPayeeAccount.iblnAdjustmentPaymentEliglbleFlag = busConstant.YES;
                }
                */

                lobjPayeeAccount.LoadBreakDownDetails();
                if (lobjPayeeAccount.idecRetroSpkAmount > 0)
                {
                    lobjPayeeAccount.idecNextGrossPaymentACH = lobjPayeeAccount.idecRetroSpkAmount;
                    lobjPayeeAccount.idecFederalTaxWithHolding = lobjPayeeAccount.idecRetroSpkFederalTaxWithHolding;
                    lobjPayeeAccount.idecStateTaxWithHolding = lobjPayeeAccount.idecRetroSpkStateTaxWithHolding;
                    lobjPayeeAccount.idecNextNetPaymentACH = lobjPayeeAccount.idecRetroSpkNextNetPaymentACH;

                }
            }
            return lobjPayeeAccount;

        }

        public busAnnualBenefitSummaryOverview GetAnnualBenefitSummaryOverview(string astrMpiPersonId)
        {
            busAnnualBenefitSummaryOverview lobjbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
            if (lobjbusAnnualBenefitSummaryOverview.FindPerson(astrMpiPersonId))
            {
                //lobjbusAnnualBenefitSummaryOverview.LoadInitialData(false);
                lobjbusAnnualBenefitSummaryOverview.LoadWorkHistory(true,0,true);  //For CRM Bug 9922
                lobjbusAnnualBenefitSummaryOverview.GetTotalHours();
            }

            return lobjbusAnnualBenefitSummaryOverview;
        }

        public busMssBenefitCalculationRetirement NewRetirementCalculation(string astrMPID,int aintPlanId,DateTime adtRetirementDate,DateTime? adtSpouseDOB  = null)
        {
     
            busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            lbusBenefitCalculationRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusPerson.FindPerson(astrMPID);
            lbusBenefitCalculationRetirement.ibusPerson.LoadPersonAccounts();

            lbusBenefitCalculationRetirement.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Initial Setup for Checking Eligbility
            lbusBenefitCalculationRetirement.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson = lbusBenefitCalculationRetirement.ibusPerson;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.ibusPerson.iclbPersonAccount = lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount;
            lbusBenefitCalculationRetirement.ibusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);
            lbusBenefitCalculationRetirement.ibusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(); 

            lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

            //IN NEW MODE WHATEVER VALUES WE KNOW WE COULD FILL THOSE ATLEAST WHILE COMING TO THE MNTN SCREEN
            lbusBenefitCalculationRetirement.PopulateInitialDataBenefitCalculationHeader(lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.person_id,
                                                                                       busConstant.ZERO_INT, busConstant.ZERO_INT, busConstant.BENEFIT_TYPE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_ESTIMATE,
                                                                                     DateTime.MinValue, busConstant.ZERO_DECIMAL, aintPlanId);
            if (lbusBenefitCalculationRetirement.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
            {
                if (adtRetirementDate == DateTime.MinValue || adtRetirementDate == null)
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = lbusBenefitCalculationRetirement.GetDefaultRetirementDateForTheParticipant();
                }
                else
                {
                    lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.retirement_date = Convert.ToDateTime(adtRetirementDate);
                }
            }

            if (adtSpouseDOB == DateTime.MinValue || adtSpouseDOB == null)
            {
                adtSpouseDOB = DateTime.MinValue;
            }
            else
            {
                lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth = Convert.ToDateTime(adtSpouseDOB);
            }
 
            lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.mss_flag = busConstant.FLAG_YES;

            lbusBenefitCalculationRetirement.BeforeValidate(utlPageMode.All);

            if (!lbusBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementType.IsNullOrEmpty())
            {
              lbusBenefitCalculationRetirement.Setup_MSS_Retirement_Calculations();
            }

            return lbusBenefitCalculationRetirement;
        }

        public busPersonOverview GetPlanBenefitSummary(string astrSSN)
        {
            busPersonOverview lobjPerson = new busPersonOverview();
            if (lobjPerson.FindPersonSSN(astrSSN))
            {
                lobjPerson.LoadPlanDetails(true);  //For CRM Bug 9922
            }
            return lobjPerson;
        }

        public busPersonOverview GetPensionIAPSummary(string astrMpiPersonId)
        {
            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };

            if (lbusPersonOverview.FindPerson(astrMpiPersonId))
            {
                lbusPersonOverview.LoadPlanDetails(true);  
            }
            return lbusPersonOverview;
        }

        public string GetRetireeHealthEligibilityFlag(string astrMpiPersonId)
        {
            string lstrRetrHlthElgblFlg = string.Empty;

            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
            lbusPersonOverview.FindPerson(astrMpiPersonId);
            //lbusPersonOverview.LoadPersonAddresss();
            //lbusPersonOverview.LoadPersonContacts();
            lbusPersonOverview.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, DateTime.Now);
            lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            //lbusPersonOverview.lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(iobjSystemManagement.icdoSystemManagement.batch_date).AddDays(1);
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.LoadPersonAccounts();
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.LoadBenefitApplication();
            lbusPersonOverview.lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
            // lbusPersonOverview.GetRetireeHealthHours();
            lbusPersonOverview.iclbHealthWorkHistory = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;
            lbusPersonOverview.CheckRetireeHealthEligibilityAndUpdateFlag();
            //var lvarHealthHours = (from item in lbusPersonOverview.iclbHealthWorkHistory select item.idecTotalHealthHours).Sum();
            //var lvarHealthQualifiedYears = (from item in lbusPersonOverview.iclbHealthWorkHistory orderby item.year select item.iintHealthCount).Last();

            if (lbusPersonOverview.icdoPerson.health_eligible_flag != null)
            {
                lstrRetrHlthElgblFlg = lbusPersonOverview.icdoPerson.health_eligible_flag;
            }

            return lstrRetrHlthElgblFlg;
        }

        #endregion WCF - Website and Mobile App
    }
}
