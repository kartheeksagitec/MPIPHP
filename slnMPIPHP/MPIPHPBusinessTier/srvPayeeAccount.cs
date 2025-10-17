#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.Common;
using MPIPHP.DataObjects;
using Sagitec.DBUtility;
using System.Collections.ObjectModel;
using System.Linq;
using Sagitec.BusinessObjects;
using Sagitec.Interface;
using System.Threading.Tasks;
using System.Threading;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvPayeeAccount : srvMPIPHP
	{
		public srvPayeeAccount()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//LA Sunset - Payment Directives
		public busPayeeAccount FindPayeeAccount(int aintPayeeAccountId,bool ablnPaymentDirective = false)
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
                lobjPayeeAccount.LoadPayeeAccountWireDetail();
                lobjPayeeAccount.LoadBenefitDetails();
                lobjPayeeAccount.LoadDRODetails();
                lobjPayeeAccount.LoadNextBenefitPaymentDate();
                lobjPayeeAccount.LoadTotalRolloverAmount();
                lobjPayeeAccount.LoadGrossAmount();
                lobjPayeeAccount.LoadPayeeAccountDeduction();
                lobjPayeeAccount.LoadNonTaxableAmount();
                lobjPayeeAccount.GetCalculatedTaxAmount();
                lobjPayeeAccount.LoadDeathNotificationStatus();
                lobjPayeeAccount.LoadWithholdingInformation();
                lobjPayeeAccount.GetCuurentPayeeAccountStatus();
                lobjPayeeAccount.CheckAnnuity();
                lobjPayeeAccount.LoadLastPaymentDate();
                lobjPayeeAccount.LoadPayeeAccountIAPPaybacks(); //06/15/2020 - IAP Paybacks            

                //LA Sunset - Payment Directives
                if (!ablnPaymentDirective)
                {
                    lobjPayeeAccount.LoadPaymentHistoryHeaderDetails();
                    lobjPayeeAccount.LoadSoftErrors();//PIR-527
                }

                //Payee Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccount.icdoPayeeAccount.person_id);
                    lobjPayeeAccount.ibusPayee.LoadPersonAddresss();
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

				//LA Sunset - Payment Directives
                lobjPayeeAccount.istrModifiedBy = iobjPassInfo.istrUserID;


                #region Load Object For Correspondence
				
				//LA Sunset - Payment Directives
                if (!ablnPaymentDirective)
                {
                    lobjPayeeAccount.ibusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    lobjPayeeAccount.ibusBenefitCalculationHeader.FindBenefitCalculationHeader(lobjPayeeAccount.icdoPayeeAccount.iintBenefitCalculationID);
                    lobjPayeeAccount.ibusBenefitCalculationHeader.ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    lobjPayeeAccount.ibusBenefitCalculationHeader.ibusBenefitCalculationRetirement.icdoBenefitCalculationHeader = lobjPayeeAccount.ibusBenefitCalculationHeader.icdoBenefitCalculationHeader;
                    lobjPayeeAccount.ibusBenefitCalculationHeader.ibusBenefitCalculationRetirement.ibusPerson = lobjPayeeAccount.ibusParticipant;
                    lobjPayeeAccount.ibusBenefitCalculationHeader.ibusBenefitCalculationRetirement.ibusBenefitApplication = lobjPayeeAccount.ibusBenefitApplication;
                }
                #endregion

      
            }

        

            lobjPayeeAccount.LoadBreakDownDetails();
			return lobjPayeeAccount;
		}
        
        public busPayeeAccount NewPayeeAccount(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.icdoPayeeAccount = new cdoPayeeAccount();
            return lobjPayeeAccount;
        }

        public busPayeeAccountWireDetail FindPayeeAccountWireDetail(int aintPayeeAccountWireDetailId)
        {
            busPayeeAccountWireDetail lobjPayeeAccountWireDetail = new busPayeeAccountWireDetail();
            if (lobjPayeeAccountWireDetail.FindPayeeAccountWireDetail(aintPayeeAccountWireDetailId))
            {
            }

            return lobjPayeeAccountWireDetail;
        }

        public busPayeeAccountAchDetail FindPayeeAccountAchDetail(int aintPayeeAccountAchDetailId)
		{
			busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail();
			if (lobjPayeeAccountAchDetail.FindPayeeAccountAchDetail(aintPayeeAccountAchDetailId))
			{
			}

			return lobjPayeeAccountAchDetail;
		}

		public busPaymentItemType FindPaymentItemType(int aintPaymentItemTypeId)
		{
			busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
			if (lobjPaymentItemType.FindPaymentItemType(aintPaymentItemTypeId))
			{
				lobjPaymentItemType.LoadPayeeAccountPaymentItemTypes();
				lobjPaymentItemType.LoadPayeeAccountRetroPaymentDetails();
				lobjPaymentItemType.LoadPayeeAccountTaxWithholdingItemDetails();
			}

			return lobjPaymentItemType;
		}

		public busPayeeAccountRetroPayment FindPayeeAccountRetroPayment(int aintPayeeAccountRetroPaymentId)
		{
			busPayeeAccountRetroPayment lobjPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
			if (lobjPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(aintPayeeAccountRetroPaymentId))
			{
                lobjPayeeAccountRetroPayment.LoadPayeeAccountRetroPaymentDetails();
                if (lobjPayeeAccountRetroPayment.iclbPayeeAccountRetroPaymentDetail != null)
                {
                    foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail in lobjPayeeAccountRetroPayment.iclbPayeeAccountRetroPaymentDetail)
                    {
                        lobjPayeeAccountRetroPaymentDetail.LoadPaymentItemType();
                        lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.istrPaymentItemTypedesription = lobjPayeeAccountRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_description;
                    }
                }

                if (lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id != 0)
                {
                    lobjPayeeAccountRetroPayment.ibusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccountRetroPayment.ibusPayeeAccount.FindPayeeAccount(lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id);
                    if (lobjPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id != 0)
                    {
                        lobjPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                        lobjPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id);
                    }
                }

                lobjPayeeAccountRetroPayment.LoadBenefitMonthwiseAdjustmentDetails();
                
			}
			return lobjPayeeAccountRetroPayment;
		}

        public busPayeeAccountRolloverDetail NewPayeeAccountRolloverDetail(int aintPayeeAccountID)
        {
            busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };
            lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = aintPayeeAccountID;

            lbusPayeeAccountRolloverDetail.ibusPayeeAccount = new busPayeeAccount();
            lbusPayeeAccountRolloverDetail.ibusPayeeAccount.FindPayeeAccount(aintPayeeAccountID);

            lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = lbusPayeeAccountRolloverDetail.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
            return lbusPayeeAccountRolloverDetail;
        }

		public busPayeeAccountRolloverDetail FindPayeeAccountRolloverDetail(int aintPayeeAccountRolloverDetailId)
		{
			busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail();
			if (lobjPayeeAccountRolloverDetail.FindPayeeAccountRolloverDetail(aintPayeeAccountRolloverDetailId))
			{
				lobjPayeeAccountRolloverDetail.LoadPayeeAccountRolloverItemDetails();
			}

			return lobjPayeeAccountRolloverDetail;
       
		}

		public busPayeeAccountStatus FindPayeeAccountStatus(int aintPayeeAccountStatusId)
		{
			busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus();
			if (lobjPayeeAccountStatus.FindPayeeAccountStatus(aintPayeeAccountStatusId))
			{
			}

			return lobjPayeeAccountStatus;
		}

		public busPayeeAccountTaxWithholding FindPayeeAccountTaxWithholding(int aintPayeeAccountTaxWithholdingId)
		{
			busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
			if (lobjPayeeAccountTaxWithholding.FindPayeeAccountTaxWithholding(aintPayeeAccountTaxWithholdingId))
			{
                lobjPayeeAccountTaxWithholding.LoadPayeeAccountTaxWithholdingItemDetails();
			}

			return lobjPayeeAccountTaxWithholding;
		}

        #region Payee Account Taxwithholding Details
        public busPayeeAccount FindPayeeAccountTaxWithholdingDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();      

            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                lobjPayeeAccount.LoadBenefitDetails();
                lobjPayeeAccount.LoadDRODetails();
                lobjPayeeAccount.GetCalculatedTaxAmount();
                lobjPayeeAccount.LoadPayeeAccountStatuss();

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

                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);

                    lobjPayeeAccount.ibusCalculation = new busCalculation();
                    lobjPayeeAccount.iblnIsQualifiedSpouse = lobjPayeeAccount.ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id, lobjPayeeAccount.icdoPayeeAccount.person_id);
                    if (lobjPayeeAccount.iblnIsQualifiedSpouse == busConstant.BOOL_FALSE)
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_NO;
                    else
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_YES;
                }
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE;
                if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0)
                {
                    busPayeeAccountTaxWithholding lbusPayeeAccount = lobjPayeeAccount.iclbPayeeAccountTaxWithholding.FirstOrDefault();
                    lobjPayeeAccount.icdoPayeeAccount.istrBenefitDistributionType = lbusPayeeAccount.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value;
                }
            }
            return lobjPayeeAccount;
        }
        public busPayeeAccount FindPayeeAccountFedTaxWithholdingDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();

            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                lobjPayeeAccount.LoadBenefitDetails();
                lobjPayeeAccount.LoadDRODetails();
                lobjPayeeAccount.GetCalculatedTaxAmount();
                lobjPayeeAccount.LoadPayeeAccountStatuss();

                //Payee Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccount.icdoPayeeAccount.person_id);
                }
                lobjPayeeAccount.idtWizardStartDate = DateTime.Now;
                //Organization Details
                if (lobjPayeeAccount.icdoPayeeAccount.org_id != 0)
                {
                    lobjPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    lobjPayeeAccount.ibusOrganization.FindOrganization(lobjPayeeAccount.icdoPayeeAccount.org_id);
                }

                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);

                    lobjPayeeAccount.ibusCalculation = new busCalculation();
                    lobjPayeeAccount.iblnIsQualifiedSpouse = lobjPayeeAccount.ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id, lobjPayeeAccount.icdoPayeeAccount.person_id);
                    if (lobjPayeeAccount.iblnIsQualifiedSpouse == busConstant.BOOL_FALSE)
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_NO;
                    else
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_YES;
                }
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_FED_WIZARD_TAXWITHHOLDING_MAINTENANCE;

            }
            return lobjPayeeAccount;
        }

        
         public busPayeeAccount FindPayeeAccountStateTaxWithholdingDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();

            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                lobjPayeeAccount.LoadBenefitDetails();
                lobjPayeeAccount.LoadDRODetails();
                lobjPayeeAccount.GetCalculatedTaxAmount();
                lobjPayeeAccount.LoadPayeeAccountStatuss();

                //Payee Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccount.icdoPayeeAccount.person_id);
                    lobjPayeeAccount.ibusPayee.ibusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    lobjPayeeAccount.ibusPayee.LoadPersonAddresss();

                    lobjPayeeAccount.idtWizardStartDate = DateTime.Now;

                    if (lobjPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "VA").Count() > 0)
                    {
                        lobjPayeeAccount.istrWizardTaxIdentifier = busConstant.VA_STATE_TAX;
                        lobjPayeeAccount.istrWizardStateDescription = busConstant.VA_DESCRIPTION;
                    }
                       
                    else if(lobjPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "NC").Count() > 0)
                    {
                        lobjPayeeAccount.istrWizardTaxIdentifier = busConstant.NC_STATE_TAX;
                        lobjPayeeAccount.istrWizardStateDescription = busConstant.NC_DESCRIPTION;
                    }
                    else if(lobjPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "OR").Count() > 0)
                    {
                        lobjPayeeAccount.istrWizardTaxIdentifier = busConstant.OR_STATE_TAX;
                        lobjPayeeAccount.istrWizardStateDescription = busConstant.OR_DESCRIPTION;
                    }
                    else if(lobjPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue) && i.icdoPersonAddress.addr_state_value == "GA").Count() > 0)
                    {
                        lobjPayeeAccount.istrWizardTaxIdentifier = busConstant.GA_STATE_TAX;
                        lobjPayeeAccount.istrWizardStateDescription = busConstant.GA_DESCRIPTION;

                    }
                    else
                    {
                        lobjPayeeAccount.istrWizardTaxIdentifier = busConstant.CA_STATE_TAX;
                        lobjPayeeAccount.istrWizardStateDescription = busConstant.CA_DESCRIPTION;
                    }

                   // lobjPayeeAccount.istrWizardStateDescription = Convert.ToString(lobjPayeeAccount.ibusPayee.iclbPersonAddress.Where(i => (i.icdoPersonAddress.end_date == null || i.icdoPersonAddress.end_date == DateTime.MinValue)).Select(y => y.icdoPersonAddress.addr_state_description).FirstOrDefault());

                }

                
                //Organization Details
                if (lobjPayeeAccount.icdoPayeeAccount.org_id != 0)
                {
                    lobjPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    lobjPayeeAccount.ibusOrganization.FindOrganization(lobjPayeeAccount.icdoPayeeAccount.org_id);
                }

                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);

                    lobjPayeeAccount.ibusCalculation = new busCalculation();
                    lobjPayeeAccount.iblnIsQualifiedSpouse = lobjPayeeAccount.ibusCalculation.CheckIfSurvivorIsQualifiedSpouse(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id, lobjPayeeAccount.icdoPayeeAccount.person_id);
                    if (lobjPayeeAccount.iblnIsQualifiedSpouse == busConstant.BOOL_FALSE)
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_NO;
                    else
                        lobjPayeeAccount.istrQualifiedSpouse = busConstant.FLAG_YES;
                }
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_STATE_WIZARD_TAXWITHHOLDING_MAINTENANCE;

            }
            return lobjPayeeAccount;
        }
        #endregion

        //6/15/2015 - IAP Payback
        #region Payee Account IAP Payback Details
        public busPayeeAccount FindPayeeAccountIAPPaybackDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();

            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountIAPPaybacks();
                lobjPayeeAccount.LoadPaymentHistoryHeader(aintPayeeAccountId);
                lobjPayeeAccount.LoadPayeeAccountPaymentItemType();

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
            }
                return lobjPayeeAccount;
         }
        #endregion

        #region Payee Account ACH Details
        
            public busPayeeAccount FindPayeeAccountAchWizardDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountAchDetails();
                lobjPayeeAccount.LoadPayeeAccountStatuss();
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
                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                lobjPayeeAccount.istrWizardPrenoteFlag = "Y";
                //Ticket#69506
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_ACH_WIZARD_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        public busPayeeAccount FindPayeeAccountAchDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountAchDetails();
                lobjPayeeAccount.LoadPayeeAccountStatuss();
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
                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                //Ticket#69506
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        #endregion
        #region Payee Account Wire Details
        public busPayeeAccount FindPayeeAccountWireDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountWireDetail();
                lobjPayeeAccount.LoadPayeeAccountStatuss();
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
                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                //Ticket#69506
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        #endregion

        #region Payee Account Rollover Details
        public busPayeeAccount FindPayeeAccountRolloverDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountRolloverDetails();
                lobjPayeeAccount.LoadPayeeAccountStatuss();
                //lobjPayeeAccount.LoadOrganization();
                lobjPayeeAccount.LoadNextBenefitPaymentDate();
                lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                lobjPayeeAccount.LoadParticipantAddress();
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
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_ROLLOVER_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        #endregion

        #region Payee Account Status Details
        public busPayeeAccount FindPayeeAccountStatusDetails(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountStatuss();
                lobjPayeeAccount.LoadPayeeAccountRolloverDetails();
                lobjPayeeAccount.LoadPayeeAccountAchDetails();
                //Payment Adjustment
                lobjPayeeAccount.LoadPayeeAccountBenefitOverPayment();

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

                //Participant Account Details 
                //PROD PIR 295 Fix
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                }
            }
            lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_STATUS_MAINTENANCE;
            return lobjPayeeAccount;
        }
        #endregion

        public busPayeeAccountRetroPaymentDetail FindPayeeAccountRetroPaymentDetail(int aintPayeeAccountRetroPaymentDetailId)
		{
			busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
			if (lobjPayeeAccountRetroPaymentDetail.FindPayeeAccountRetroPaymentDetail(aintPayeeAccountRetroPaymentDetailId))
			{
			}

			return lobjPayeeAccountRetroPaymentDetail;
		}

		public busPayeeAccountTaxWithholdingItemDetail FindPayeeAccountTaxWithholdingItemDetail(int aintPayeeAccountTaxWithholdingItemDtlId)
		{
			busPayeeAccountTaxWithholdingItemDetail lobjPayeeAccountTaxWithholdingItemDetail = new busPayeeAccountTaxWithholdingItemDetail();
			if (lobjPayeeAccountTaxWithholdingItemDetail.FindPayeeAccountTaxWithholdingItemDetail(aintPayeeAccountTaxWithholdingItemDtlId))
			{
			}

			return lobjPayeeAccountTaxWithholdingItemDetail;
		}

		public busPayeeAccountRolloverItemDetail FindPayeeAccountRolloverItemDetail(int aintPayeeAccountRolloverItemDetailId)
		{
			busPayeeAccountRolloverItemDetail lobjPayeeAccountRolloverItemDetail = new busPayeeAccountRolloverItemDetail();
			if (lobjPayeeAccountRolloverItemDetail.FindPayeeAccountRolloverItemDetail(aintPayeeAccountRolloverItemDetailId))
			{
			}

			return lobjPayeeAccountRolloverItemDetail;
		}

		public busPayeeAccountPaymentItemType FindPayeeAccountPaymentItemType(int aintPayeeAccountPaymentItemTypeId)
		{
			busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType();
			if (lobjPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(aintPayeeAccountPaymentItemTypeId))
			{
				lobjPayeeAccountPaymentItemType.LoadPayeeAccountRolloverItemDetails();
				lobjPayeeAccountPaymentItemType.LoadPayeeAccountTaxWithholdingItemDetails();
			}

			return lobjPayeeAccountPaymentItemType;
		}

		public busFedStateTaxRate FindFedStateTaxRate(int aintFedStateTaxId)
		{
			busFedStateTaxRate lobjFedStateTaxRate = new busFedStateTaxRate();
			if (lobjFedStateTaxRate.FindFedStateTaxRate(aintFedStateTaxId))
			{
			}

			return lobjFedStateTaxRate;
		}

		public busFedStateFlatTaxRate FindFedStateFlatTaxRate(int aintFedStateFlatTaxId)
		{
			busFedStateFlatTaxRate lobjFedStateFlatTaxRate = new busFedStateFlatTaxRate();
			if (lobjFedStateFlatTaxRate.FindFedStateFlatTaxRate(aintFedStateFlatTaxId))
			{
			}

			return lobjFedStateFlatTaxRate;
		}

		public busPaymentSchedule FindPaymentSchedule(int aintPaymentScheduleId)
		{
			busPaymentSchedule lobjPaymentSchedule = new busPaymentSchedule();
			if (lobjPaymentSchedule.FindPaymentSchedule(aintPaymentScheduleId))
			{
                lobjPaymentSchedule.LoadPaymentScheduleSteps();
                lobjPaymentSchedule.EvaluateInitialLoadRules();
			}

			return lobjPaymentSchedule;
		}

        public busPaymentSchedule NewPaymentSchedule(string astrPaymentSchedule)
        {
            busPaymentSchedule lobjPaymentSchedule = new busPaymentSchedule();
            lobjPaymentSchedule.icdoPaymentSchedule = new cdoPaymentSchedule();
            lobjPaymentSchedule.icdoPaymentSchedule.schedule_type_id = busConstant.SCHEDULE_TYPE_CODE_ID;
            lobjPaymentSchedule.icdoPaymentSchedule.schedule_type_value = astrPaymentSchedule;
            lobjPaymentSchedule.icdoPaymentSchedule.status_id = busConstant.STATUS_CODE_ID;
            if (lobjPaymentSchedule.icdoPaymentSchedule.schedule_type_value != busConstant.PaymentScheduleVendor)
            {
                lobjPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusPending;
            }
            else
            {
                lobjPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleActionStatusReadyforFinal;
            }

            lobjPaymentSchedule.LoadInitialData();
            lobjPaymentSchedule.EvaluateInitialLoadRules(utlPageMode.New);
            return lobjPaymentSchedule;
        }

		public busRetroItemType FindRetroItemType(int aintRetroItemTypeId)
		{
			busRetroItemType lobjRetroItemType = new busRetroItemType();
			if (lobjRetroItemType.FindRetroItemType(aintRetroItemTypeId))
			{
			}

			return lobjRetroItemType;
		}

		public busPayeeBenefitAccount FindPayeeBenefitAccount(int aintPayeeBenefitAccountId)
		{
			busPayeeBenefitAccount lobjPayeeBenefitAccount = new busPayeeBenefitAccount();
			return lobjPayeeBenefitAccount;
		}

		public busPayeeAccountLookup LoadPayeeAccounts(DataTable adtbSearchResult)
		{
			busPayeeAccountLookup lobjPayeeAccountLookup = new busPayeeAccountLookup();
			lobjPayeeAccountLookup.LoadPayeeAccounts(adtbSearchResult);
			return lobjPayeeAccountLookup;
		}
        public busReturnToWorkRequestLookup LoadReturnToToWorkRequest(DataTable adtbSearchResult)
        {
            busReturnToWorkRequestLookup lobjbusReturnToWorkLookup = new busReturnToWorkRequestLookup();
            lobjbusReturnToWorkLookup.LoadReturnToWork(adtbSearchResult);
            return lobjbusReturnToWorkLookup;
        }

		public busPaymentScheduleLookup LoadPaymentSchedules(DataTable adtbSearchResult)
		{
			busPaymentScheduleLookup lobjPaymentScheduleLookup = new busPaymentScheduleLookup();
			lobjPaymentScheduleLookup.LoadPaymentSchedules(adtbSearchResult);


            return lobjPaymentScheduleLookup;
		}

        public busReturnToWorkRequest NewReturnToWorkRequest(string aintMPIPersonId, string aintRequestTypeValue, string aintSourceValue)
        {
            busReturnToWorkRequest lbusReturnToWorkRequest = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            return lbusReturnToWorkRequest.NewReturnToWorkRequest(aintMPIPersonId, aintRequestTypeValue, aintSourceValue);
        }


        public busReturnToWorkRequest FindReturnToWorkRequest(int aintReEmploymentNotificationId)
        {
            busReturnToWorkRequest lobjReturnToWork = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            lobjReturnToWork.LoadDefaultProperties();
            if (lobjReturnToWork.FindReturnToWorkRequest(aintReEmploymentNotificationId))
            {
                lobjReturnToWork.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                lobjReturnToWork.ibusPayee.FindPerson(lobjReturnToWork.icdoReturnToWorkRequest.person_id);
                lobjReturnToWork.ibusPayee.LoadPersonAccounts();
                lobjReturnToWork.LoadPayeeAccount();
                lobjReturnToWork.CheckMemberIsRetiree();
                lobjReturnToWork.LoadCorTracking();
                lobjReturnToWork.LoadReturnToWorkHistory();
                if (lobjReturnToWork.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
                    lobjReturnToWork.ValidateReturnToWorkRequest();
                if (lobjReturnToWork.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
                    lobjReturnToWork.ValidateEndOfReturnToWorkRequestSoftErrors();
                lobjReturnToWork.iclbNotes = lobjReturnToWork.LoadNotesForRTW(lobjReturnToWork.ibusPayee.icdoPerson.person_id, busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM, aintReEmploymentNotificationId);
                lobjReturnToWork.LoadSoftErrors();
                lobjReturnToWork.LoadAuditingNotes();
                lobjReturnToWork.EvaluateInitialLoadRules();
            }
            return lobjReturnToWork;
        }

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNewChild(string astrFormName, object aobjParentObject, Type atypBusObject, Hashtable ahstParams)
        {
            ArrayList larrErrors = new ArrayList();
            
            if (astrFormName == busConstant.PAYEE_ACCOUNT_ROLLOVER_MAINTENANCE)
            {
                busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_ROLLOVER_DETAIL)
                {
                    larrErrors = lbusPayeeAccountRolloverDetail.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }

            if (astrFormName == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE)
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING)
                {
                    larrErrors = lbusPayeeAccountTaxWithholding.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }

            if (astrFormName == busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE)
            {
                busPayeeAccountAchDetail lbusPayeeAccountAchDetail = new busPayeeAccountAchDetail();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_ACH)
                {
                    larrErrors = lbusPayeeAccountAchDetail.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }
            if (astrFormName == busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE)
            {
                busPayeeAccountWireDetail lbusPayeeAccountWireDetail = new busPayeeAccountWireDetail();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_WIRE)
                {
                    larrErrors = lbusPayeeAccountWireDetail.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }
            if (astrFormName == busConstant.PAYEE_ACCOUNT_STATUS_MAINTENANCE)
            {
                busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_STATUS)
                {
                    larrErrors = lbusPayeeAccountStatus.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }
            if (astrFormName == busConstant.PAYEE_ACCOUNT_DEDUCTION_MAINTENANCE)
            {
                busPayeeAccountDeduction lbusPayeeAccountDeduction = new busPayeeAccountDeduction();
                if (atypBusObject.Name == busConstant.PAYEE_ACCOUNT_DEDUCTION)
                {
                    larrErrors = lbusPayeeAccountDeduction.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }
                        
            if (astrFormName == busConstant.WITHHOLDING_INFORMATION_MAINTENANCE)
            {
                busWithholdingInformation lbusWithholdingInformation = new busWithholdingInformation();
                if (atypBusObject.Name == busConstant.WITHHODING_INFORMATION)
                {
                    larrErrors = lbusWithholdingInformation.CheckErrorOnAddButton(aobjParentObject as busPayeeAccount, ahstParams, ref larrErrors);
                }
            }
            if (astrFormName == busConstant.PAYMENT_REISSUE_DETAIL_MAINTENANCE)
            {
                busPaymentReissueDetail lbusPaymentReissueDetail = new busPaymentReissueDetail();

                if (atypBusObject.Name == busConstant.PAYMENT_REISSUE_DETAIL)
                {
                    ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"] = ahstParams["reissue_payment_type_value"];
                    ahstParams["icdoPaymentReissueDetail.reissue_reason_value"] = ahstParams["reissue_reason_value"];
                    ahstParams["icdoPaymentReissueDetail.recipient_rollover_org_id"] = ahstParams["recipient_rollover_org_id"];
                    ahstParams["icdoPaymentReissueDetail.iintSurvivorId"] = ahstParams["iintSurvivorId"];
                    ahstParams["icdoPaymentReissueDetail.iintPartitipantID"] = ahstParams["iintPartitipantID"];
                    larrErrors = lbusPaymentReissueDetail.CheckErrorOnAddButton(aobjParentObject as busPaymentHistoryDistribution, ahstParams, ref larrErrors);

                }
            }
            if (astrFormName == busConstant.REPAYMENT_SCHEDULE_MAINTENANCE)
            {
                busReimbursementDetails lbusReimbursementDetails = new busReimbursementDetails();
                if (atypBusObject.Name == busConstant.REIMBURSEMENT_DETAILS)
                {
                    larrErrors = lbusReimbursementDetails.CheckErrorOnAddButton(aobjParentObject as busRepaymentSchedule, ahstParams, ref larrErrors);
                }
            }
            if(astrFormName == busConstant.PAYEE_ACCOUNT_MAINTENANCE)
            {
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                decimal idecIAPTotalBalanceAmount = 0;

                if (aobjParentObject is busPayeeAccount)
                {
                    lbusPayeeAccount = aobjParentObject as busPayeeAccount;

                    decimal idecTotalIAPPaybackReceived = 0.0M;
                    idecTotalIAPPaybackReceived = lbusPayeeAccount.iclbIAPHardshipPayback.Sum(item => item.icdoIapHardshipPayback.check_amount);

                    foreach (busIapHardshipPayback item in lbusPayeeAccount.iclbIAPHardshipPayback)
                    {
                        idecIAPTotalBalanceAmount += item.icdoIapHardshipPayback.check_amount;
                        item.icdoIapHardshipPayback.idecRunningIAPPaybackBalance = idecIAPTotalBalanceAmount;
                    }

                    larrErrors = lbusPayeeAccount.CheckErrorOnAddButton(aobjParentObject as busRepaymentSchedule, ahstParams, ref larrErrors);
                }
                

            }

            return larrErrors;
        }

       protected override ArrayList ValidateGridUpdateChild(string astrFormName, object aobjParentObject, object aobjChildObject, Hashtable ahstParams)
        {
            ArrayList iarrResult = new ArrayList();
            utlError lobjError = null;
            busMainBase lbusMainBase = new busMainBase();

            if (astrFormName == busConstant.PAYEE_ACCOUNT_ROLLOVER_MAINTENANCE)
            {
                decimal adecPercentage; string ContactName = string.Empty;
                string astrRolloverOption = string.Empty;
                string astrStatus = string.Empty;
                string astrAccountNumber = string.Empty;
                string astrAddress = string.Empty;
                string astrRolloverType = string.Empty;
                int iintRolloveOrgId = 0; decimal ldecPercentage = 0.0M;
                decimal ldecAmountOfTaxable = 0.0M;
                int iintPayeeAccRolloverdetailid = 0;
                astrRolloverOption = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]);
                astrStatus = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.status_value"]);
                astrAddress = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.addr_line_1"]);
                astrAccountNumber = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.account_number"]);
                astrRolloverType = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_type_value"]);

                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]).IsNotNullOrEmpty())
                {
                    iintRolloveOrgId = Convert.ToInt32(ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]);
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]) != "$0.00")
                {
                    ldecAmountOfTaxable = Convert.ToDecimal(Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]).Replace("$", String.Empty));
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty())
                {
                    ldecPercentage = Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id"])))
                {
                    iintPayeeAccRolloverdetailid = Convert.ToInt32(ahstParams["icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id"]);
                }
                ContactName = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.contact_name"]);

                if (iintRolloveOrgId == 0)
                {
                    lobjError = lbusMainBase.AddError(6010, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (iintRolloveOrgId != 0 && astrStatus.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6011, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrAddress.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(1114, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                busPayeeAccount lobjPayeeAccounts = aobjParentObject as busPayeeAccount;
                if (iintRolloveOrgId != 0)
                {
                    busPayeeAccountRolloverDetail ibusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };
                    if (iintPayeeAccRolloverdetailid != 0)
                    {
                        ibusPayeeAccountRolloverDetail.LoadPayeeAccountRolloverItemDetail(iintPayeeAccRolloverdetailid);
                    }
                    ibusPayeeAccountRolloverDetail.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    ibusPayeeAccountRolloverDetail.ibusOrganization.FindOrganization(iintRolloveOrgId);
                    ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank = new Collection<busOrgBank>();

                    if (ibusPayeeAccountRolloverDetail.ibusOrganization != null)
                    {
                        if (!(ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.status_value == "A" &&
                                              ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.org_type_value == "RLIT"))
                        {
                            lobjError = lbusMainBase.AddError(6058, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                        if (ibusPayeeAccountRolloverDetail.ibusOrganization.icdoOrganization.payment_type_value == busConstant.PAYMENT_METHOD_ACH)
                        {
                            ibusPayeeAccountRolloverDetail.ibusOrganization.LoadOrgBanks4Organization(iintRolloveOrgId);
                            if (ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank.IsNullOrEmpty() || ibusPayeeAccountRolloverDetail.ibusOrganization.iclbOrgBank.Where(obj => obj.icdoOrgBank.status_value == busConstant.STATUS_ACTIVE).Count() > 0)
                            {
                                lobjError = lbusMainBase.AddError(6103, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                    int idecTotalAmount = (from obj in lobjPayeeAccounts.iclbPayeeAccountRolloverDetail
                                           where obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id == iintPayeeAccRolloverdetailid
                                           && obj.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusProcessed && obj.icdoPayeeAccountRolloverDetail.status_value != astrStatus
                                           select obj).Count();

                    if (idecTotalAmount >= 1)
                    {
                        lobjError = lbusMainBase.AddError(6276, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                    if (astrStatus == busConstant.PayeeAccountRolloverDetailStatusProcessed)
                    {
                        lobjError = lbusMainBase.AddError(6277, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.Count > 0)
                {
                    int iintActiveRolloverCount = (from obj in lobjPayeeAccounts.iclbPayeeAccountRolloverDetail
                                                   where obj.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive
                                                   && obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid
                                                   select obj).Count();
                    if (iintActiveRolloverCount > 3)
                    {
                        lobjError = lbusMainBase.AddError(6012, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.Count > 0)
                {
                    int iintActiveRolloverCount = (from obj in lobjPayeeAccounts.iclbPayeeAccountRolloverDetail
                                                   where obj.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross
                                                   && obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid
                                                   select obj).Count();
                    if (iintActiveRolloverCount > 0)
                    {
                        lobjError = lbusMainBase.AddError(6022, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                //153521
                //if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNullOrEmpty())
                //{
                //    lobjError = lbusMainBase.AddError(6072, "");
                //    iarrResult.Add(lobjError);
                //    return iarrResult;
                //}
                if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNotNullOrEmpty() && astrRolloverOption.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6071, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrAccountNumber.IsNullOrEmpty() && Convert.ToString(ContactName).IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6075, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ldecPercentage).IsNotNullOrEmpty())
                {
                    adecPercentage = lobjPayeeAccounts.iclbPayeeAccountRolloverDetail
                                                                                  .Where(obj => obj.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != iintPayeeAccRolloverdetailid)
                                                                                  .Sum(item => item.icdoPayeeAccountRolloverDetail.percent_of_taxable) + Convert.ToDecimal(ldecPercentage);
                    if (adecPercentage > 100)
                    {
                        lobjError = lbusMainBase.AddError(6013, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull())
                {
                    if (lobjPayeeAccounts.icdoPayeeAccount.payee_benefit_account_id != 0)
                    {
                        if (ldecAmountOfTaxable > lobjPayeeAccounts.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.gross_amount)
                        {
                            lobjError = lbusMainBase.AddError(6015, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && astrRolloverOption.IsNotNullOrEmpty()
                    && Convert.ToString(ldecPercentage).IsNotNullOrEmpty() && Convert.ToString(ahstParams["amount"]).IsNotNullOrEmpty())
                {
                    if ((astrRolloverOption == busConstant.PayeeAccountRolloverOptionAllOfGross || astrRolloverOption == busConstant.PayeeAccountRolloverOptionAllOfTaxable) && Convert.ToDecimal(ldecPercentage) != 0)
                    {
                        lobjError = lbusMainBase.AddError(6016, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && astrRolloverOption.IsNotNullOrEmpty()
                        && (Convert.ToString(ldecPercentage).IsNullOrEmpty() || ldecPercentage == 0))
                {
                    if (astrRolloverOption == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                    {
                        lobjError = lbusMainBase.AddError(6029, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && astrRolloverOption.IsNotNullOrEmpty() && Convert.ToString(ldecPercentage).IsNotNullOrEmpty())
                {
                    if (astrRolloverOption != busConstant.PayeeAccountRolloverOptionPercentageOfTaxable && Convert.ToDecimal(ldecPercentage) != 0)
                    {
                        lobjError = lbusMainBase.AddError(6017, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (astrRolloverOption != busConstant.PayeeAccountRolloverOptionDollorOfGross && astrRolloverOption != busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                {
                    if (ldecAmountOfTaxable > 0 || ldecAmountOfTaxable != 0)
                    {
                        lobjError = lbusMainBase.AddError(6028, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && astrRolloverOption.IsNotNullOrEmpty())
                {
                    if (astrRolloverOption == busConstant.PayeeAccountRolloverOptionDollorOfGross || astrRolloverOption == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                    {
                        if (ldecAmountOfTaxable == 0)
                        {
                            lobjError = lbusMainBase.AddError(6018, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                }
                if (lobjPayeeAccounts.iclbPayeeAccountRolloverDetail.IsNotNull() && astrRolloverOption.IsNotNullOrEmpty())
                {
                    if (astrRolloverOption == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                    {
                        if (lobjPayeeAccounts.IsNotNull() && ldecAmountOfTaxable > lobjPayeeAccounts.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.starting_taxable_amount)
                        {
                            lobjError = lbusMainBase.AddError(6164, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                }
            }
            else if (astrFormName == busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE)
            {
                string astrMaritalStatus = string.Empty; DateTime EndDate = DateTime.MinValue;
                string astrTaxOption = string.Empty; DateTime StartDate = DateTime.MinValue;
                string astrTaxIdentifier = string.Empty;
                string astrBenefitDistributionType = string.Empty;
                decimal ldectaxPercentage = 0; string TaxAllowance = string.Empty;
                int ainttaxwithholdingid = 0; string AdditionalTaxAmount = string.Empty;
                decimal ldecAdditionalTaxAmount = 0; string MaritalStatusValue = string.Empty;

                busPayeeAccount lobjPayeeAccount = aobjParentObject as busPayeeAccount;
                astrMaritalStatus = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.marital_status_value"]);
                astrTaxOption = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_option_value"]);
                astrTaxIdentifier = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_identifier_value"]);
                astrBenefitDistributionType = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]);
                EndDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.end_date"]);
                StartDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountTaxWithholding.start_date"]);
                TaxAllowance = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_allowance"]);
                MaritalStatusValue = Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.marital_status_value"]);

                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"])))
                {
                    ldectaxPercentage = Convert.ToDecimal(ahstParams["icdoPayeeAccountTaxWithholding.tax_percentage"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"])))
                {
                    ainttaxwithholdingid = Convert.ToInt32(ahstParams["icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"])) && Convert.ToString(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]) != "$0.00")
                {
                    ldecAdditionalTaxAmount = Convert.ToDecimal(ahstParams["icdoPayeeAccountTaxWithholding.additional_tax_amount"]);
                }

                if (EndDate > DateTime.MinValue)
                {
                    lobjPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue = lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL").Select(Y => Y.icdoPayeeAccountTaxWithholding.marital_status_value).FirstOrDefault();
                    lobjPayeeAccount.icdoPayeeAccount.istrtax_option_value = lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL" && i.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == "MNBF").Select(y => y.icdoPayeeAccountTaxWithholding.tax_option_value).FirstOrDefault();
                    lobjPayeeAccount.icdoPayeeAccount.istrSavingMode = "UPDATE";
                }
                if (astrTaxIdentifier.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6042, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (lobjPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_MEMBER
                        || lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_ALTERNATE_PAYEE)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                        {
                            if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 10 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                lobjError = lbusMainBase.AddError(6044, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                            else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 10 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                            {
                                lobjError = lbusMainBase.AddError(6044, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                        else
                        {
                            if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                            {
                                lobjError = lbusMainBase.AddError(6043, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                            else if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 20 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID && lobjPayeeAccount.isCOVID19PayFlag != "Y")
                            {
                                lobjError = lbusMainBase.AddError(6043, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                    else if ((lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                           || lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_JOINT_ANNUITANT)
                           && lobjPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                    {
                        if (lobjPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage != 20 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                        {
                            lobjError = lbusMainBase.AddError(6043, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                        else if (lobjPayeeAccount.iblnIsQualifiedSpouse && astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage < 20 && lobjPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.IAP_PLAN_ID)
                        {
                            lobjError = lbusMainBase.AddError(6043, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                    else
                    {
                        if (astrTaxIdentifier == busConstant.FEDRAL_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                        {
                            if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.BenefitCalculation.ACCOUNT_RELATIONSHIP_BENEFICIARY
                                && lobjPayeeAccount.icdoPayeeAccount.family_relation_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                            {

                            }
                            else if (ldectaxPercentage != 10)
                            {
                                lobjError = lbusMainBase.AddError(6044, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                }
                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT)
                {
                    if (ldectaxPercentage <= 0)
                    {
                        lobjError = lbusMainBase.AddError(6029, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0 && lobjPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION)
                {
                    if (astrTaxIdentifier == busConstant.CA_STATE_TAX && astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage <= 0)
                    {
                        lobjError = lbusMainBase.AddError(6029, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrBenefitDistributionType.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6047, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (lobjPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_LumpSum
                             && lobjPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                {
                    lobjError = lbusMainBase.AddError(6048, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (lobjPayeeAccount.icdoPayeeAccount.istrBenefitOption != busConstant.LUMP_SUM_DESCRIPTION && astrBenefitDistributionType != busConstant.Benefit_Distribution_Type_Monthly_Benefit)
                {
                    lobjError = lbusMainBase.AddError(6063, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxOption.IsNullOrEmpty() && EndDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(6049, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (!lobjPayeeAccount.iclbPayeeAccountTaxWithholding.IsNullOrEmpty())
                {
                    int CountRecordwithEndDateNull = (from obj in lobjPayeeAccount.iclbPayeeAccountTaxWithholding
                                                      where obj.icdoPayeeAccountTaxWithholding.end_date == null || (obj.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                                                      && obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                                      && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                                                      select obj).Count();
                    if (CountRecordwithEndDateNull > 0)
                    {
                        lobjError = lbusMainBase.AddError(6050, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                    if ((from obj in lobjPayeeAccount.iclbPayeeAccountTaxWithholding
                         where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                         select obj).Count() > 0)
                    {
                        var MaxEndDate = (from obj in lobjPayeeAccount.iclbPayeeAccountTaxWithholding
                                          where obj.icdoPayeeAccountTaxWithholding.tax_identifier_value == astrTaxIdentifier
                                          && obj.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != ainttaxwithholdingid
                                          select obj.icdoPayeeAccountTaxWithholding.end_date).Max();
                        if (StartDate > DateTime.MinValue && MaxEndDate > StartDate)
                        {
                            lobjError = lbusMainBase.AddError(6051, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                }
                if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding.IsNotNull() && lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0 && Convert.ToString(ainttaxwithholdingid).IsNotNullOrEmpty() && ainttaxwithholdingid != 0)
                {
                    var oldicdoPayeeAccountTaxWithholding = (from item in lobjPayeeAccount.iclbPayeeAccountTaxWithholding.AsEnumerable()
                                                             where item.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id == ainttaxwithholdingid
                                                             select new
                                                             {
                                                                 tax_identifier_value = item.icdoPayeeAccountTaxWithholding.tax_identifier_value == null ? "" : Convert.ToString(item.icdoPayeeAccountTaxWithholding.ihstOldValues["tax_identifier_value"]),
                                                                 benefit_distribution_type_value = item.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == null ? "" : item.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value,
                                                                 marital_status_value = item.icdoPayeeAccountTaxWithholding.marital_status_value == null ? "" : item.icdoPayeeAccountTaxWithholding.marital_status_value,
                                                                 tax_allowance = item.icdoPayeeAccountTaxWithholding.tax_allowance == 0 ? "" : item.icdoPayeeAccountTaxWithholding.tax_allowance.ToString(),
                                                                 additional_tax_amount = item.icdoPayeeAccountTaxWithholding.additional_tax_amount,
                                                                 start_date = item.icdoPayeeAccountTaxWithholding.start_date,
                                                                 tax_option_value = item.icdoPayeeAccountTaxWithholding.tax_option_value == null ? "" : item.icdoPayeeAccountTaxWithholding.tax_option_value,

                                                             }).FirstOrDefault();
                   
                    string istrTaxAllowance = TaxAllowance == "0" ? string.Empty:TaxAllowance ;
                    if (oldicdoPayeeAccountTaxWithholding != null && lobjPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                    {
                        if (oldicdoPayeeAccountTaxWithholding.start_date < DateTime.Today && EndDate == DateTime.MinValue)
                        {
                            if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != astrTaxIdentifier
                                || oldicdoPayeeAccountTaxWithholding.benefit_distribution_type_value != astrBenefitDistributionType
                                || (oldicdoPayeeAccountTaxWithholding.tax_option_value != "FLAD" && oldicdoPayeeAccountTaxWithholding.tax_option_value != "FLAP" && oldicdoPayeeAccountTaxWithholding.tax_option_value != astrTaxOption)
                                || oldicdoPayeeAccountTaxWithholding.tax_allowance.ToString() != istrTaxAllowance
                                || DateTime.Compare(Convert.ToDateTime(oldicdoPayeeAccountTaxWithholding.start_date.ToShortDateString()), StartDate) != 0)
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                            if (Convert.ToString(ldecAdditionalTaxAmount).IsNotNullOrEmpty())
                            {
                                if (oldicdoPayeeAccountTaxWithholding.additional_tax_amount != Convert.ToDecimal(ldecAdditionalTaxAmount.ToString().Replace("$", "")))
                                {
                                    lobjError = lbusMainBase.AddError(6004, "");
                                    iarrResult.Add(lobjError);
                                    return iarrResult;
                                }
                            }
                            else if (oldicdoPayeeAccountTaxWithholding.additional_tax_amount != 0)
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                        else if (oldicdoPayeeAccountTaxWithholding.start_date >= DateTime.Today && lobjPayeeAccount.icdoPayeeAccount.istrBenefitOption == busConstant.LUMP_SUM_DESCRIPTION
                              && (oldicdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NO_STATE_TAX || oldicdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NO_FEDRAL_TAX))
                        {
                            if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != astrTaxIdentifier || oldicdoPayeeAccountTaxWithholding.benefit_distribution_type_value != astrBenefitDistributionType
                            || oldicdoPayeeAccountTaxWithholding.marital_status_value != MaritalStatusValue || oldicdoPayeeAccountTaxWithholding.tax_allowance.ToString() != istrTaxAllowance
                            || DateTime.Compare(Convert.ToDateTime(oldicdoPayeeAccountTaxWithholding.start_date.ToShortDateString()), StartDate) != 0)
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                        else
                        {
                            if (oldicdoPayeeAccountTaxWithholding.tax_identifier_value != astrTaxIdentifier)
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                }
                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrBenefitDistributionType.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6007, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxIdentifier.IsNotNullOrEmpty() && astrTaxOption.IsNullOrEmpty() && EndDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(6008, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrMaritalStatus == busConstant.MARITAL_STATUS_MARRIED && TaxAllowance.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(5482, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE) && (astrMaritalStatus.IsNullOrEmpty()) && EndDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(5481, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                {
                    if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE) && TaxAllowance.IsNullOrEmpty())
                    {
                        lobjError = lbusMainBase.AddError(5482, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (astrTaxIdentifier != busConstant.VA_STATE_TAX && astrTaxIdentifier != busConstant.FEDRAL_STATE_TAX)
                {
                    if (astrTaxOption != busConstant.NO_STATE_TAX && astrTaxOption != busConstant.FLAT_DOLLAR && astrTaxOption != busConstant.FLAT_PERCENT)
                    {
                        if (astrMaritalStatus.IsNullOrEmpty() && EndDate==DateTime.MinValue)
                        {
                            lobjError = lbusMainBase.AddError(5481, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                }
                if (StartDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(5113, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (EndDate > DateTime.MinValue && StartDate > DateTime.MinValue && EndDate <= StartDate)
                {
                    lobjError = lbusMainBase.AddError(5111, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxOption == busConstant.FLAT_PERCENT && ldectaxPercentage == 0)
                {
                    lobjError = lbusMainBase.AddError(6061, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxIdentifier != busConstant.VA_STATE_TAX)
                {
                    if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE_ADDITIONAL_TAX || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional
                    || astrTaxOption == busConstant.FLAT_DOLLAR) && (Convert.ToString(ldecAdditionalTaxAmount).IsNullOrEmpty() || Convert.ToString(ldecAdditionalTaxAmount) == "0"))
                    {
                        lobjError = lbusMainBase.AddError(6060, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if ((astrTaxOption == busConstant.FEDRAL_TAX_IRS_TABLE || astrTaxOption == busConstant.StateTaxOptionFedTaxBasedOnIRS || astrTaxOption == busConstant.FLAT_PERCENT) && Convert.ToString(ldecAdditionalTaxAmount).IsNotNullOrEmpty() && ldecAdditionalTaxAmount != 0)
                {
                    lobjError = lbusMainBase.AddError(6076, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrTaxOption != busConstant.NO_FEDRAL_TAX && astrTaxOption != busConstant.NO_STATE_TAX)
                {
                    if (astrTaxOption != busConstant.FLAT_PERCENT && Convert.ToString(ldectaxPercentage).IsNotNullOrEmpty() && ldectaxPercentage != 0)
                    {
                        lobjError = lbusMainBase.AddError(6077, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (ldecAdditionalTaxAmount > 0)
                {
                    Decimal ldecTotalTaxableAmount = 0.0m;
                    if (lobjPayeeAccount.iclbPayeeAccountPaymentItemType.IsNotNull())
                        ldecTotalTaxableAmount = (from item in lobjPayeeAccount.iclbPayeeAccountPaymentItemType
                                                  where busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                  item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                  && ((item.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                                  && (item.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFedTax
                                                  || item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck))
                                                  select item.icdoPayeeAccountPaymentItemType.amount * item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
                    if (ldecAdditionalTaxAmount > ldecTotalTaxableAmount)
                    {
                        lobjError = lbusMainBase.AddError(6081, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && astrMaritalStatus.IsNotNullOrEmpty()
                            && lobjPayeeAccount.icdoPayeeAccount.retiree_incr_flag != busConstant.FLAG_YES)
                {
                    lobjError = lbusMainBase.AddError(6107, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrBenefitDistributionType == busConstant.Benefit_Distribution_Type_LumpSum && TaxAllowance.IsNotNullOrEmpty() && Convert.ToDecimal(TaxAllowance) != Decimal.Zero)
                {
                    lobjError = lbusMainBase.AddError(6108, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.PAYEE_ACCOUNT_ACH_MAINTENANCE)
            {
                int iintOrgBankID = 0; string astrAccountType = string.Empty; int PayeeAccountAchDetailId = 0; DateTime StartDate = DateTime.MinValue;
                DateTime EndDate = DateTime.MinValue; string istrRoutingNumber = string.Empty;
                astrAccountType = Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.bank_account_type_value"]);
                StartDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]);
                EndDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_end_date"]);
                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.payee_account_ach_detail_id"]).IsNotNullOrEmpty())
                {
                    PayeeAccountAchDetailId = Convert.ToInt32(ahstParams["icdoPayeeAccountAchDetail.payee_account_ach_detail_id"]);
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]).IsNotNullOrEmpty())
                {
                    iintOrgBankID = Convert.ToInt32(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]);
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).IsNotNullOrEmpty())
                {
                    istrRoutingNumber = Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]);
                }
                busPayeeAccount objPayeeAccount = aobjParentObject as busPayeeAccount;
                if (StartDate > DateTime.MinValue && Convert.ToString(PayeeAccountAchDetailId).IsNotNullOrEmpty())
                {
                    var CurrentAchDetailR = objPayeeAccount.iclbPayeeAccountAchDetail.Where(item => item.icdoPayeeAccountAchDetail.payee_account_ach_detail_id == Convert.ToInt32(PayeeAccountAchDetailId)).FirstOrDefault();
                    if (CurrentAchDetailR != null)
                    {
                        if (CurrentAchDetailR.icdoPayeeAccountAchDetail.ihstOldValues.ContainsKey("ach_end_date") && Convert.ToDateTime(CurrentAchDetailR.icdoPayeeAccountAchDetail.ihstOldValues["ach_end_date"])!=DateTime.MinValue)
                        {
                            lobjError = lbusMainBase.AddError(6281, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                        if (DateTime.Today > StartDate)
                        {
                            if (DateTime.Compare(CurrentAchDetailR.icdoPayeeAccountAchDetail.ach_start_date.Date, StartDate) != 0)
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                        else
                        {
                            var otherAchDetailR = objPayeeAccount.iclbPayeeAccountAchDetail.Where(item => item.icdoPayeeAccountAchDetail.payee_account_ach_detail_id != PayeeAccountAchDetailId);
                            if (otherAchDetailR != null && otherAchDetailR.Count() > 0)
                            {
                                if (Convert.ToString(StartDate).IsNotNullOrEmpty() && StartDate < otherAchDetailR.Select(item => item.icdoPayeeAccountAchDetail.ach_start_date).Max())
                                {
                                    lobjError = lbusMainBase.AddError(6004, "");
                                    iarrResult.Add(lobjError);
                                    return iarrResult;
                                }
                            }
                        }
                    }
                }
                if (StartDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(5113, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrAccountType.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6020, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (EndDate > DateTime.MinValue && StartDate > DateTime.MinValue && EndDate <= StartDate)
                {
                    lobjError = lbusMainBase.AddError(5111, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (EndDate > DateTime.MinValue && StartDate > DateTime.MinValue && StartDate > EndDate)
                {
                    lobjError = lbusMainBase.AddError(5139, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.account_number"]).IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6026, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (istrRoutingNumber.IsNullOrEmpty() && istrRoutingNumber.Length != 9)
                {
                    lobjError = lbusMainBase.AddError(6059, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (istrRoutingNumber.IsNotNullOrEmpty() || istrRoutingNumber.Length == 9)
                {
                    DataTable ldtbRoutingNumber = objPayeeAccount.LoadOrgDetailsByRoutingNumber(istrRoutingNumber);
                    if (ldtbRoutingNumber != null && ldtbRoutingNumber.Rows.Count > 0)
                    {
                    }
                    else
                    {
                        lobjError = lbusMainBase.AddError(6178, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
            }
            else if (astrFormName == busConstant.PAYEE_ACCOUNT_WIRE_MAINTENANCE)
            {
                int iintOrgBankID = 0; string PayeeAccountAchDetailId = string.Empty; DateTime StartDate = DateTime.MinValue;
                DateTime EndDate = DateTime.MinValue; string BeneficiaryAccountNumber = string.Empty; string istrAbaSwiftBankCode = string.Empty;

                StartDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]);
                EndDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_end_date"]);
                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.payee_account_wire_detail_id"]).IsNotNullOrEmpty())
                {
                    PayeeAccountAchDetailId = Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.payee_account_wire_detail_id"]);
                }
                BeneficiaryAccountNumber = Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.beneficiary_account_number"]);
                istrAbaSwiftBankCode = Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]);
                if (StartDate > DateTime.MinValue && PayeeAccountAchDetailId.IsNotNullOrEmpty())
                {
                    busPayeeAccount objPayeeAccount = aobjParentObject as busPayeeAccount;
                    var CurrentWireDetailR = objPayeeAccount.iclbPayeeAccountWireDetail.Where(item => item.icdoPayeeAccountWireDetail.payee_account_wire_detail_id == Convert.ToInt32(PayeeAccountAchDetailId)).FirstOrDefault();

                    if (CurrentWireDetailR != null)
                    {
                        if (CurrentWireDetailR.icdoPayeeAccountWireDetail.wire_end_date != DateTime.MinValue)
                        {
                            lobjError = lbusMainBase.AddError(6281, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                    if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.call_back_flag"])=="Y")
                    {
                        CurrentWireDetailR.icdoPayeeAccountWireDetail.call_back_completion_date = DateTime.Now.Date;
                    }
                    if (DateTime.Today > StartDate)
                    {
                        if (DateTime.Compare(CurrentWireDetailR.icdoPayeeAccountWireDetail.wire_start_date.Date, Convert.ToDateTime(StartDate)) != 0)
                        {
                            lobjError = lbusMainBase.AddError(6004, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                    else
                    {
                        var otherAchDetailR = objPayeeAccount.iclbPayeeAccountWireDetail.Where(item => item.icdoPayeeAccountWireDetail.payee_account_wire_detail_id != Convert.ToInt32(PayeeAccountAchDetailId));
                        if (otherAchDetailR != null && otherAchDetailR.Count() > 0)
                        {
                            if (Convert.ToString(StartDate).IsNotNullOrEmpty() && Convert.ToDateTime(StartDate) < otherAchDetailR.Select(item => item.icdoPayeeAccountWireDetail.wire_start_date).Max())
                            {
                                lobjError = lbusMainBase.AddError(6004, "");
                                iarrResult.Add(lobjError);
                                return iarrResult;
                            }
                        }
                    }
                }
                if (StartDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(5113, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.bank_org_id"]).IsNotNullOrEmpty())
                {
                    iintOrgBankID = Convert.ToInt32(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]);
                }
                if (EndDate > DateTime.MinValue && StartDate > DateTime.MinValue && StartDate <= EndDate)
                {
                    lobjError = lbusMainBase.AddError(5111, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (EndDate > DateTime.MinValue && StartDate > DateTime.MinValue && EndDate > StartDate)
                {
                    lobjError = lbusMainBase.AddError(5139, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (BeneficiaryAccountNumber.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6026, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (istrAbaSwiftBankCode.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6311, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.WITHHOLDING_INFORMATION_MAINTENANCE)
            {
                DateTime WithholdingDateFrom = DateTime.MinValue;
                WithholdingDateFrom = Convert.ToDateTime(ahstParams["icdoWithholdingInformation.withholding_date_from"]);
                if (WithholdingDateFrom > DateTime.MinValue)
                {
                    if (WithholdingDateFrom < DateTime.Today)
                    {
                        lobjError = lbusMainBase.AddError(5112, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                busPayeeAccount objPayeeAccount = aobjParentObject as busPayeeAccount;
                if (objPayeeAccount.iclbWithholdingInformation.Count > 0)
                {
                    int iintCount = (from item in objPayeeAccount.iclbWithholdingInformation
                                     where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                     && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                     && WithholdingDateFrom < item.icdoWithholdingInformation.withholding_date_from
                                     select item).Count();
                    if (iintCount > 0)
                    {
                        lobjError = lbusMainBase.AddError(6166, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                    int lintCount = (from item in objPayeeAccount.iclbWithholdingInformation
                                     where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                     && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                     && item.icdoWithholdingInformation.withholding_date_from == WithholdingDateFrom
                                     select item).Count();
                    if (lintCount > 0)
                    {
                        lobjError = lbusMainBase.AddError(6109, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
            }
            else if (astrFormName == busConstant.PAYEE_ACCOUNT_DEDUCTION_MAINTENANCE)
            {
                string astrDeductionType = string.Empty; DateTime StartDate = DateTime.MinValue;
                string astrPayTo = string.Empty; string astrotherDeductionTypeDescription = string.Empty;
                decimal ldecDeductionAmount = Decimal.Zero; DateTime EndDate = DateTime.MinValue;
                int iintOrgID = 0; int iintPersonID = 0;

                astrDeductionType = Convert.ToString(ahstParams["icdoPayeeAccountDeduction.deduction_type_value"]);
                astrPayTo = Convert.ToString(ahstParams["icdoPayeeAccountDeduction.pay_to_value"]);
                StartDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.start_date"]);
                EndDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.end_date"]);
                astrotherDeductionTypeDescription = Convert.ToString(ahstParams["icdoPayeeAccountDeduction.other_deduction_type_description"]);
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPayeeAccountDeduction.amount"])))
                {
                    ldecDeductionAmount = Convert.ToDecimal(ahstParams["icdoPayeeAccountDeduction.amount"]);
                }
                if (Convert.ToString(ldecDeductionAmount).IsNotNullOrEmpty())
                {
                    if (busGlobalFunctions.IsDecimal(Convert.ToString(ldecDeductionAmount)))
                    {
                        ldecDeductionAmount = Convert.ToDecimal(ldecDeductionAmount);
                        if (ldecDeductionAmount < 0)
                        {
                            lobjError = lbusMainBase.AddError(6141, "");
                            iarrResult.Add(lobjError);
                            return iarrResult;
                        }
                    }
                    else
                    {
                        lobjError = lbusMainBase.AddError(6069, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (astrDeductionType.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6023, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrDeductionType == busConstant.CANCELLATION_REASON_OTHER && astrotherDeductionTypeDescription.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6032, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.org_id"]).IsNotNullOrEmpty())
                {
                    iintOrgID = Convert.ToInt32(ahstParams["icdoPayeeAccountDeduction.org_id"]);
                }
                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.person_id"]).IsNotNullOrEmpty())
                {
                    iintPersonID = Convert.ToInt32(ahstParams["icdoPayeeAccountDeduction.person_id"]);
                }
                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo != busConstant.SURVIVOR_TYPE_PERSON) && (iintOrgID.IsNull() || iintOrgID == 0))
                {
                    lobjError = lbusMainBase.AddError(6025, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNull() || iintPersonID == 0))
                {
                    lobjError = lbusMainBase.AddError(6070, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (astrDeductionType.IsNotNullOrEmpty() && astrPayTo.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(6024, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldecDeductionAmount == Decimal.Zero)
                {
                    lobjError = lbusMainBase.AddError(6027, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }

                busPayeeAccount objPayeeAccount = aobjParentObject as busPayeeAccount;
                decimal idecTotalAmount = (from item in objPayeeAccount.iclbPayeeAccountPaymentItemType
                                           where busGlobalFunctions.CheckDateOverlapping(objPayeeAccount.idtNextBenefitPaymentDate,
                                           item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                           && !((item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "FEDX"
                                           && item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "STTX")
                                           && item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1
                                           && item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != "RRED"
                                           && item.ibusPaymentItemType.icdoPaymentItemType.item_type_code != busConstant.ITEM53)
                                           select item.icdoPayeeAccountPaymentItemType.amount * item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();

                if (idecTotalAmount < ldecDeductionAmount)
                {
                    lobjError = lbusMainBase.AddError(6202, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (StartDate > DateTime.MinValue)
                {
                    var oldicdoPayeeAccountDeduction = (from item in objPayeeAccount.iclbPayeeAccountDeduction.AsEnumerable()
                                                        where item.icdoPayeeAccountDeduction.payee_account_deduction_id ==
                                                        (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.payee_account_deduction_id"]) == string.Empty ? 0 : Convert.ToInt32(ahstParams["icdoPayeeAccountDeduction.payee_account_deduction_id"]))
                                                        select new
                                                        {
                                                            start_date = item.icdoPayeeAccountDeduction.start_date,
                                                        }).FirstOrDefault();

                    if (oldicdoPayeeAccountDeduction.start_date < DateTime.Today && StartDate != oldicdoPayeeAccountDeduction.start_date)
                    {
                        lobjError = lbusMainBase.AddError(6203, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                    if (oldicdoPayeeAccountDeduction.start_date >= DateTime.Today && StartDate < DateTime.Today)
                    {
                        lobjError = lbusMainBase.AddError(5112, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                    if (EndDate > DateTime.MinValue && EndDate <= StartDate)
                    {
                        lobjError = lbusMainBase.AddError(5111, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                else if (StartDate == DateTime.MinValue)
                {
                    lobjError = lbusMainBase.AddError(5113, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (EndDate > DateTime.MinValue && EndDate < DateTime.Today)
                {
                    lobjError = lbusMainBase.AddError(5081, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNotNull() || iintPersonID != 0)
                           && objPayeeAccount.icdoPayeeAccount.person_id == iintPersonID)
                {
                    lobjError = lbusMainBase.AddError(6283, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_ORG) && (iintOrgID.IsNotNull() || iintOrgID != 0)
                 && objPayeeAccount.icdoPayeeAccount.org_id == iintOrgID)
                {

                    lobjError = lbusMainBase.AddError(6283, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }

            if (astrFormName == "wfmPayeeAccountDeductionMaintenance"
              && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PAYEE_ACCOUNT_DEDUCTION_GRID)
            {
                if (aobjChildObject is busPayeeAccountDeduction)
                {
                    Hashtable lhstParam = new Hashtable();
                    busPayeeAccountDeduction lbusPayeeAccountDeduction = (busPayeeAccountDeduction)aobjChildObject;

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

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetails", lhstParam, iobjPassInfo.idictParams);

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

                        string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                        DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPerson.GetPersonDetails", lhstParam, iobjPassInfo.idictParams);

                        if (ldtblist.Rows.Count > 0)
                        {
                            lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrPersonName = Convert.ToString(ldtblist.Rows[0][0]);
                        }
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountACHDetailsMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountAchDetail")
            {
                if (aobjChildObject is busPayeeAccountAchDetail)
                {
                    busPayeeAccountAchDetail lbusPayeeAccountAchDetail = (busPayeeAccountAchDetail)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("ROUTING_NUMBER", lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.istrRoutingNumber);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtbOrgBankName = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsByRoutingNumber", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbOrgBankName.Rows.Count > 0)
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.istrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0]["ORG_NAME"]);
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountWireDetailMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountWireDetail")
            {
                if (aobjChildObject is busPayeeAccountWireDetail)
                {
                    busPayeeAccountWireDetail lbusPayeeAccountWireDetail = (busPayeeAccountWireDetail)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("ABA_SWIFT_BANK_CODE", lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.istrAbaSwiftBankCode);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtbOrgBankName = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgsDetailsByAbaSwiftBankCode", lhstParam, iobjPassInfo.idictParams);

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
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountTaxwithholdingMaintenance"
               && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountTaxWithholding")
            {
                if (aobjParentObject is busPayeeAccount)
                {

                    busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = (busPayeeAccountTaxWithholding)aobjChildObject;

                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)aobjParentObject;

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
                }
            }

            return base.ValidateGridUpdateChild(astrFormName, aobjParentObject, aobjChildObject, ahstParams);
        }
            
        protected override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        {
            if (iobjPassInfo.istrFormName == "wfmPayeeAccountStatusMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_GRID)
            {
                if (aobjChildObject is busPayeeAccountStatus)
                {
                    busPayeeAccountStatus lbusPayeeAccountStatus = (busPayeeAccountStatus)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.PAYEE_ACCOUNT_STATUS_GRID];

                    DateTime ldtStatusEffectiveDate = DateTime.MinValue;
                    if (Param.ContainsKey("status_effective_date") && !string.IsNullOrEmpty(Convert.ToString(Param["status_effective_date"])))
                    {
                        ldtStatusEffectiveDate = Convert.ToDateTime(Param["status_effective_date"]);
                    }

                    if (ldtStatusEffectiveDate == DateTime.MinValue)
                        lbusPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountMaintenance"
               && iobjPassInfo.idictParams["MVVMGridItemAddUpdate"].ToString() == "grvIAPPaybackDetails")
            {
                if (aobjChildObject is busPayeeAccountStatus)
                {
                    busIapHardshipPayback lbusIapHardshipPayback = (busIapHardshipPayback)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvIAPPaybackDetails"];
                    lbusIapHardshipPayback.icdoIapHardshipPayback.payment_posted_date = DateTime.Now;
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountMaintenance"
              && iobjPassInfo.idictParams["MVVMGridItemAddUpdate"].ToString() == "grvIAPPaybackDetails1")
            {
                if (aobjChildObject is busIapHardshipPayback)
                {
                    busIapHardshipPayback lbusIapHardshipPayback = (busIapHardshipPayback)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvIAPPaybackDetails1"];
                    lbusIapHardshipPayback.icdoIapHardshipPayback.payment_posted_date = DateTime.Now;
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountACHDetailsMaintenance"
             && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountAchDetail")
            {
                if (aobjChildObject is busPayeeAccountAchDetail)
                {
                    busPayeeAccountAchDetail lbusPayeeAccountAchDetail = (busPayeeAccountAchDetail)aobjChildObject;
                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)aobjParentObject;

                   lbusPayeeAccountAchDetail.EvaluateInitialLoadRules();
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvPayeeAccountAchDetail"];
                    if (Convert.ToString(Param["ach_start_date"]).IsNullOrEmpty())
                    {
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.Now;
                    }
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_flag = "Y";
                    if(lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.joint_account_flag.IsNullOrEmpty())
                    {
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.joint_account_flag = "N";
                    }

                    Hashtable lhstParam = new Hashtable();
                    string listrRoutingNumber = string.Empty;
                    if (Param.ContainsKey("icdoPayeeAccountAchDetail.istrRoutingNumber") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountAchDetail.istrRoutingNumber"])))
                    {
                        listrRoutingNumber = Convert.ToString(Param["icdoPayeeAccountAchDetail.istrRoutingNumber"]);
                    }

                    lhstParam.Add("ROUTING_NUMBER", listrRoutingNumber);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtbOrgBankName = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetailsByRoutingNumber", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbOrgBankName.Rows.Count > 0)
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.istrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0]["ORG_NAME"]);
                    if (lbusPayeeAccount.iclbPayeeAccountAchDetail.Count > 0)
                   {
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_completion_date = lbusPayeeAccount.iclbPayeeAccountAchDetail[0].icdoPayeeAccountAchDetail.pre_note_completion_date;
                    }

                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountWireDetailMaintenance"
              && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountWireDetail")
            {
                if (aobjChildObject is busPayeeAccountStatus)
                {
                    busPayeeAccountWireDetail lbusPayeeAccountWireDetail = (busPayeeAccountWireDetail)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvPayeeAccountWireDetail"];

                    if (Convert.ToString(Param["call_back_flag"]) == "Y")
                    {
                        lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_completion_date = DateTime.Now;
                    }
                    if (Convert.ToString(Param["wire_start_date"]).IsNullOrEmpty())
                    {
                        lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.wire_start_date = DateTime.Now;
                    }
                }
                if (aobjChildObject is busPayeeAccountWireDetail)
                {
                    busPayeeAccountWireDetail lbusPayeeAccountWireDetail = (busPayeeAccountWireDetail)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvPayeeAccountWireDetail"];

                    Hashtable lhstParam = new Hashtable();

                    string listrAbaSwiftBankCode = string.Empty;
                    if (Param.ContainsKey("icdoPayeeAccountWireDetail.istrAbaSwiftBankCode") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"])))
                    {
                        listrAbaSwiftBankCode = Convert.ToString(Param["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]);
                    }

                    lhstParam.Add("ABA_SWIFT_BANK_CODE", listrAbaSwiftBankCode);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtbOrgBankName = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgsDetailsByAbaSwiftBankCode", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbOrgBankName.Rows.Count > 0)
                        lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.istrOrgName = Convert.ToString(ldtbOrgBankName.Rows[0]["ORG_NAME"]);

                    DateTime ldtCallBackCompletionDate = DateTime.MinValue;
                    if (Param.ContainsKey("icdoPayeeAccountWireDetail.call_back_completion_date") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountWireDetail.call_back_completion_date"])))
                    {
                        ldtCallBackCompletionDate = Convert.ToDateTime(Param["icdoPayeeAccountWireDetail.call_back_completion_date"]);
                    }

                    if (ldtCallBackCompletionDate == DateTime.MinValue)
                    {
                        string listrCallBackFlag = string.Empty;
                        if (Param.ContainsKey("icdoPayeeAccountWireDetail.call_back_flag") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountWireDetail.call_back_flag"])))
                        {
                            listrCallBackFlag = Convert.ToString(Param["icdoPayeeAccountWireDetail.call_back_flag"]);
                        }

                        if (listrCallBackFlag == "Y")
                        {
                            lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_completion_date = DateTime.Now;
                        }
                    }
                    if (lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_flag.IsNullOrEmpty())
                    {
                        lbusPayeeAccountWireDetail.icdoPayeeAccountWireDetail.call_back_flag = "N";
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountTaxwithholdingMaintenance"
               && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountTaxWithholding")
            {
                if (aobjChildObject is busPayeeAccountTaxWithholding)
                {
                    busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = (busPayeeAccountTaxWithholding)aobjChildObject;
                    lbusPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvPayeeAccountTaxWithholding"];
                    if (Convert.ToString(Param["tax_identifier_value"]) == "FDRL" && Convert.ToString(Param["benefit_distribution_type_value"]) == "MNBF")
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_percentage = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance = 0;
                    }
                    if (Convert.ToString(Param["tax_identifier_value"]) != "FDRL")
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.step_2_b_3 = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.step_3_amount = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.step_4_a = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.step_4_b = 0;
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.step_4_c = 0;
                    }
                    if (Convert.ToString(Param["tax_identifier_value"]) == busConstant.VA_STATE_TAX)
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance = 0; 
                        if (Convert.ToString(Param["tax_option_value"]) == "STST" || Convert.ToString(Param["tax_option_value"]) == "NSTX")
                        {
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.voluntary_withholding = 0;
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = 0;
                        }
                    }
                }
                if (aobjParentObject is busPayeeAccount)
                {
                    busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = (busPayeeAccountTaxWithholding)aobjChildObject;
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvPayeeAccountTaxWithholding"];

                    busPayeeAccount lbusPayeeAccount = (busPayeeAccount)aobjParentObject;

                    if (lbusPayeeAccount.icdoPayeeAccount.istrSavingMode == "UPDATE")
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountTaxWithholding.end_date"])) && lbusPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue != null)
                        {
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = lbusPayeeAccount.icdoPayeeAccount.istrMaritalStatusValue;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(Param["icdoPayeeAccountTaxWithholding.end_date"])) && Convert.ToString(Param["icdoPayeeAccountTaxWithholding.benefit_distribution_type_value"]) == "MNBF" && lbusPayeeAccount.icdoPayeeAccount.istrtax_option_value == "FTIA")
                        {
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = lbusPayeeAccount.icdoPayeeAccount.istrtax_option_value;

                        }
                    }
                }
            }
            if (iobjPassInfo.istrFormName == "wfmPayeeAccountDeductionMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PAYEE_ACCOUNT_DEDUCTION_GRID)
            {
                if (aobjChildObject is busPayeeAccountDeduction)
                {
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_grvBusPayeeAccountDeduction"];
                    Hashtable lhstParam = new Hashtable();
                    busPayeeAccountDeduction lbusPayeeAccountDeduction = (busPayeeAccountDeduction)aobjChildObject;
                    if (Param["icdoPayeeAccountDeduction.pay_to_value"].ToString() == busConstant.SURVIVOR_TYPE_PERSON)
                    {
                        lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.org_id = 0;
                    }
                    else
                    {
                        lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.person_id = 0;
                    }
                    if (Convert.ToString(Param["istrOrgMPID"]).IsNotNullOrEmpty())
                    {
                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        lhstParam.Add(enmOrganization.mpi_org_id.ToString().ToUpper(), Convert.ToString(Param["istrOrgMPID"]).ToString());

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoOrganization.GetOrgDetails", lhstParam, iobjPassInfo.idictParams);

                        if (ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToString(Param["istrOrgMPID"]).IsNotNullOrEmpty())
                            {
                                lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrOrgName = Convert.ToString(ldtblist.Rows[0][0]);
                            }
                        }
                    }
                    else if (Convert.ToString(Param["istrPersonMPID"]).IsNotNullOrEmpty())
                    {
                        lhstParam.Add(enmPerson.mpi_person_id.ToString().ToUpper(), Convert.ToString(Param["istrPersonMPID"]));

                        string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                        IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                        DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPerson.GetPersonDetails", lhstParam, iobjPassInfo.idictParams);

                        if (ldtblist.Rows.Count > 0)
                        {
                            lbusPayeeAccountDeduction.icdoPayeeAccountDeduction.istrPersonName = Convert.ToString(ldtblist.Rows[0][0]);
                        }
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPaymentReissueDetailMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PAYMENT_REISSUE_DETAIL_GRID)
            {
                if (aobjChildObject is busPaymentReissueDetail)
                {
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.PAYMENT_REISSUE_DETAIL_GRID];

                    busPaymentReissueDetail lbusPaymentReissueDetail = (busPaymentReissueDetail)aobjChildObject;

                    if (Param.ContainsKey("icdoPaymentReissueDetail.reissue_payment_type_value") && Convert.ToString(Param["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
                    {
                        Hashtable lhstParamForOrg = new Hashtable();

                        int lintSurvivorId = 0;
                        if (Param.ContainsKey("icdoPaymentReissueDetail.iintSurvivorId") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPaymentReissueDetail.iintSurvivorId"])))
                        {
                            lintSurvivorId = Convert.ToInt32(Param["icdoPaymentReissueDetail.iintSurvivorId"]);
                        }

                        int lintPartitipantID = 0;
                        if (Param.ContainsKey("icdoPaymentReissueDetail.iintPartitipantID") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPaymentReissueDetail.iintPartitipantID"])))
                        {
                            lintPartitipantID = Convert.ToInt32(Param["icdoPaymentReissueDetail.iintPartitipantID"]);
                        }
                        lhstParamForOrg.Add("ORG_ID", lintSurvivorId);
                        lhstParamForOrg.Add("Person_Id", lintPartitipantID);

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtbRecipientOrgMPID = lsrvBusinessTier.ExecuteQuery("cdoPaymentReissueDetail.GetOrgDetailsByOrgID", lhstParamForOrg, iobjPassInfo.idictParams);
                        if (ldtbRecipientOrgMPID.Rows.Count > 0)
                        {
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientOrgMPID.Rows[0]["MPI_ORG_ID"]);
                            lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_org_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId;

                        }
                        else
                        {
                            //PIR-622
                            Hashtable lhstParam = new Hashtable();
                            lhstParam.Add("Survivor_ID", lintSurvivorId);
                            lhstParam.Add("Person_Id", lintPartitipantID);

                            string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                            IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbRecipientPersonMPID = isrvBusinessTier.ExecuteQuery("cdoPaymentReissueDetail.GetPersonDetailsByPersonID", lhstParam, iobjPassInfo.idictParams);
                            if (ldtbRecipientPersonMPID.Rows.Count > 0)
                            {
                                lbusPaymentReissueDetail.icdoPaymentReissueDetail.istrRMPID = Convert.ToString(ldtbRecipientPersonMPID.Rows[0]["MPI_PERSON_ID"]);
                                //  lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId;
                                lbusPaymentReissueDetail.icdoPaymentReissueDetail.recipient_person_id = lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId > 0 ? lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId : lintSurvivorId;

                            }
                        }
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmRepaymentScheduleMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.REIMBURSEMENT_DETAILS_GRID)
            {
                if (aobjChildObject is busReimbursementDetails)
                {
                    busReimbursementDetails lbusReimbursementDetails = (busReimbursementDetails)aobjChildObject;
                    lbusReimbursementDetails.EvaluateInitialLoadRules();
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.REIMBURSEMENT_DETAILS_GRID];

                    decimal ldecAmountPaid = 0M;

                    decimal ldecStateTaxAmount = 0M;
                    decimal ldecFedTaxAmount = 0M;

                    decimal ldecAmountPaidParam = 0M;
                    if (Param.ContainsKey("icdoReimbursementDetails.amount_paid") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoReimbursementDetails.amount_paid"])))
                    {
                        ldecAmountPaidParam = Convert.ToDecimal(Param["icdoReimbursementDetails.amount_paid"]);
                    }

                    decimal ldecStateTaxAmountParam = 0M;
                    if (Param.ContainsKey("icdoReimbursementDetails.state_tax") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoReimbursementDetails.state_tax"])))
                    {
                        ldecStateTaxAmountParam = Convert.ToDecimal(Param["icdoReimbursementDetails.state_tax"]);
                    }

                    decimal ldecFedTaxAmountParam = 0M;
                    if (Param.ContainsKey("icdoReimbursementDetails.fed_tax") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoReimbursementDetails.fed_tax"])))
                    {
                        ldecFedTaxAmountParam = Convert.ToDecimal(Param["icdoReimbursementDetails.fed_tax"]);
                    }

                    busRepaymentSchedule lbusRepaymentSchedule = (busRepaymentSchedule)aobjParentObject;

                    if (lbusRepaymentSchedule.iclbReimbursementDetails != null && lbusRepaymentSchedule.iclbReimbursementDetails.Count > 0)
                    {
                        ldecAmountPaid = lbusRepaymentSchedule.iclbReimbursementDetails.Sum(item => item.icdoReimbursementDetails.amount_paid);
                    }
                    else
                    {
                        ldecAmountPaid = ldecAmountPaidParam;
                    }

                    lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = ldecAmountPaidParam;

                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add(enmRepaymentSchedule.repayment_schedule_id.ToString().ToUpper(), lbusRepaymentSchedule.icdoRepaymentSchedule.repayment_schedule_id);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtblRepaymentDetails = lsrvBusinessTier.ExecuteQuery("cdoReimbursementDetails.GetRepaymentDetails", lhstParam, iobjPassInfo.idictParams);

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

                                string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPayeeAccount");
                                IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                                DataTable ldtblTaxes = isrvBusinessTier.ExecuteQuery("cdoPaymentHistoryHeader.GetFedTax&StateTax", lhstParam, iobjPassInfo.idictParams);

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

                        lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = ldecAmountPaidParam +
                                 ldecStateTaxAmountParam + ldecFedTaxAmountParam;

                        if ((ldecAmountPaid) ==
                            lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount)
                        {

                        }
                        else if ((ldecAmountPaid) ==
                            (lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount - (ldecStateTaxAmount + ldecFedTaxAmount)))
                        {
                            lbusReimbursementDetails.icdoReimbursementDetails.state_tax = ldecStateTaxAmount;
                            lbusReimbursementDetails.icdoReimbursementDetails.fed_tax = ldecFedTaxAmount;
                            lbusReimbursementDetails.icdoReimbursementDetails.gross_amount = ldecAmountPaidParam +
                                 ldecStateTaxAmountParam + ldecFedTaxAmountParam;
                        }
                    }

                    lbusReimbursementDetails.icdoReimbursementDetails.payment_option_value = busConstant.REIMBURSEMENT_PAYMENT_OPTION_CHECK;
                    lbusReimbursementDetails.icdoReimbursementDetails.payment_option_description = busConstant.REIMBURSEMENT_PAYMENT_OPTION_CHECK_DESC;
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPayeeAccountRolloverMaintenance"
               && iobjPassInfo.idictParams["RelatedGridID"].ToString() == "grvPayeeAccountRolloverDetail")
            {
                if (aobjChildObject is busPayeeAccountRolloverDetail)
                {
                    busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = (busPayeeAccountRolloverDetail)aobjChildObject;
                    lbusPayeeAccountRolloverDetail.EvaluateInitialLoadRules();
                }
            }
            base.InitializeNewChildObject(aobjParentObject, aobjChildObject);
        }

        public override string GetMessageText(string astrMessage, int aintBusMessageID, int aintButtonMessageID, int aintDefaultMessgeId, params object[] aarrParam)
        {
            if ((iobjPassInfo.istrSenderForm == "wfmPayeeAccountTaxwithholdingMaintenance" || iobjPassInfo.istrSenderForm == "wfmPayeeAccountACHDetailsMaintenance" || 
                 iobjPassInfo.istrSenderForm == "wfmPayeeAccountDeductionMaintenance" || iobjPassInfo.istrSenderForm == "wfmPayeeAccountStatusMaintenance" ||
                 iobjPassInfo.istrSenderForm == "wfmPayeeAccountWireDetailMaintenance" || iobjPassInfo.istrSenderForm == "wfmPayeeAccountRolloverMaintenance" ||
                 iobjPassInfo.istrSenderForm == "wfmWithholdingInformationMaintenance") && iobjPassInfo.istrSenderID == "btnSave" && iobjPassInfo.idictParams.ContainsKey("SaveMesssagePayeeAccount"))
            {
                astrMessage = "No changes to save.";
            }
            return base.GetMessageText(astrMessage, aintBusMessageID, aintButtonMessageID, aintDefaultMessgeId, aarrParam);
        }

        //public busPayeeAccountDeduction FindPayeeAccountDeduction(int aintPayeeAccountDeductionId)
        //{
        //    busPayeeAccountDeduction lobjPayeeAccountDeduction = new busPayeeAccountDeduction();
        //    if (lobjPayeeAccountDeduction.FindPayeeAccountDeduction(aintPayeeAccountDeductionId))
        //    {

        //    }
        //    return lobjPayeeAccountDeduction;
        //}

        #region Payee Account Taxwithholding Details
        public busPayeeAccount FindPayeeAccountDeduction(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadPayeeAccountDeduction();
                lobjPayeeAccount.LoadPayeeAccountStatuss();

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
                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_DEDUCTION_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        #endregion


        public busFedStateDeduction FindFedStateDeduction(int aintFedStateDeductionId)
		{
			busFedStateDeduction lobjFedStateDeduction = new busFedStateDeduction();
			if (lobjFedStateDeduction.FindFedStateDeduction(aintFedStateDeductionId))
			{
			}

			return lobjFedStateDeduction;
		}
        public busRepaymentSchedule NewRepaymentSchedule(int aintPayeeAccountRetroPaymentId)
        {
            busRepaymentSchedule lbusRepaymentSchedule = new busRepaymentSchedule() { icdoRepaymentSchedule = new cdoRepaymentSchedule() };
            lbusRepaymentSchedule.icdoRepaymentSchedule.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentId;
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(aintPayeeAccountRetroPaymentId);
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.FindPayeeAccount(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id);
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadNextBenefitPaymentDate();

            //Participant Account Details
            if (lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
            {
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.FindPerson(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
            }

            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            lbusRepaymentSchedule.icdoRepaymentSchedule.payee_account_id = lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;

           int lPayeeBeneficiaryParticipantsCount =  lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.GetPayeeBeneficiaryCount(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.person_id,
                                                                                                                                                  lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.iintPlanId);
            lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.FindPerson(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);

            if (lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.icdoPerson.date_of_death != DateTime.MinValue)
            {

                if (lPayeeBeneficiaryParticipantsCount == 1 || lPayeeBeneficiaryParticipantsCount == 0)
                {
                    lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount =
                 lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount;
                    lbusRepaymentSchedule.icdoRepaymentSchedule.idecRemainingOverPaymentAmount = lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount;
                }
                else
                {
                    lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount =
                 Math.Round(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount / lPayeeBeneficiaryParticipantsCount, 2);
                    lbusRepaymentSchedule.icdoRepaymentSchedule.idecRemainingOverPaymentAmount = Math.Round(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount / lPayeeBeneficiaryParticipantsCount, 2);
                }
            }
            else
            {
                lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount =
                 lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount;
                lbusRepaymentSchedule.icdoRepaymentSchedule.idecRemainingOverPaymentAmount = lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.gross_payment_amount;
            }


            lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_status_value = busConstant.REIMBURSEMENT_STATUS_PENDING;
            lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_status_description = busConstant.REIMBURSEMENT_STATUS_PENDING_DESC;
            lbusRepaymentSchedule.iclbReimbursementDetails = new Collection<busReimbursementDetails>();

            lbusRepaymentSchedule.Load1099RBatchRanYear();

            return lbusRepaymentSchedule;

        }


        public busRepaymentSchedule FindRepaymentSchedule(int aintRepaymentScheduleId)
        {
            busRepaymentSchedule lbusRepaymentSchedule = new busRepaymentSchedule();
            if (lbusRepaymentSchedule.FindRepaymentSchedule(aintRepaymentScheduleId))
            {
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(lbusRepaymentSchedule.icdoRepaymentSchedule.payee_account_retro_payment_id);

                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.FindPayeeAccount(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id);
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadNextBenefitPaymentDate();
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                //Participant Account Details
                if (lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusParticipant.FindPerson(lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }

                
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.iclbPaymentItemType = new Collection<busPaymentItemType>();
                lbusRepaymentSchedule.ibusPayeeAccountRetroPayment.ibusPayeeAccount.LoadPaymentItemType();

                lbusRepaymentSchedule.iclbReimbursementDetails = new Collection<busReimbursementDetails>();
                lbusRepaymentSchedule.LoadReimbursementDetails();

                lbusRepaymentSchedule.Load1099RBatchRanYear();

                lbusRepaymentSchedule.icdoRepaymentSchedule.idecRemainingOverPaymentAmount = lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount;

                if (lbusRepaymentSchedule.iclbReimbursementDetails != null && lbusRepaymentSchedule.iclbReimbursementDetails.Count > 0)
                {
                    decimal ldecAmountPaid = 0M;

                    foreach (busReimbursementDetails lbusReimbursementDetails in lbusRepaymentSchedule.iclbReimbursementDetails)
                    {
                        ldecAmountPaid += lbusReimbursementDetails.icdoReimbursementDetails.amount_paid;
                    }

                    lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount_paid = ldecAmountPaid;

                    lbusRepaymentSchedule.icdoRepaymentSchedule.idecRemainingOverPaymentAmount =
                        lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount - lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_amount_paid;
                }

                

            }

            return lbusRepaymentSchedule;
        }

   

		public busPaymentHistoryHeader FindPaymentHistoryHeader(int aintPaymentHistoryHeaderId)
		{
			busPaymentHistoryHeader lobjPaymentHistoryHeader = new busPaymentHistoryHeader();
			if (lobjPaymentHistoryHeader.FindPaymentHistoryHeader(aintPaymentHistoryHeaderId))
			{
				lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
				lobjPaymentHistoryHeader.LoadPaymentHistoryDistributions();
				lobjPaymentHistoryHeader.LoadPaymentHistoryDistributionStatusHistorys();
                

                if (lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution != null && lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.Count > 0)
                {
                    lobjPaymentHistoryHeader.istrPaymentHistoryDistributionId = 
                        lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.First().icdoPaymentHistoryDistribution.payment_history_distribution_id;

                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPaymentMethod  = lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.First().icdoPaymentHistoryDistribution.payment_method_value;
                }
                //Payee Account Details
                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id != 0)
                {
                    lobjPaymentHistoryHeader.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPaymentHistoryHeader.ibusPayee.FindPerson(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id);
                }
                //Organization Details
                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id != 0)
                {
                    lobjPaymentHistoryHeader.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                    lobjPaymentHistoryHeader.ibusOrganization.FindOrganization(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id);
                }

                if(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id != 0)
                {
                    lobjPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    if (lobjPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id))
                    {
                        lobjPaymentHistoryHeader.ibusPayeeAccount.LoadBreakDownDetails(aintPaymentHistoryHeaderId);
                        lobjPaymentHistoryHeader.ibusPayeeAccount.LoadBenefitDetails();
                        lobjPaymentHistoryHeader.ibusPayeeAccount.LoadDRODetails();
                    }
                }
                if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail != null)
                {
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lobjPaymentHistoryDetail.LoadPaymentItemType();
                    }
                }

			}

			return lobjPaymentHistoryHeader;
		}

		public busPaymentHistoryDetail FindPaymentHistoryDetail(int aintPaymentHistoryDetailId)
		{
			busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail();
			if (lobjPaymentHistoryDetail.FindPaymentHistoryDetail(aintPaymentHistoryDetailId))
			{

			}

			return lobjPaymentHistoryDetail;
		}

		public busPaymentHistoryDistribution FindPaymentHistoryDistribution(int aintPaymentHistoryDistributionId)
		{
			busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution();
			if (lobjPaymentHistoryDistribution.FindPaymentHistoryDistribution(aintPaymentHistoryDistributionId))
			{
                lobjPaymentHistoryDistribution.LoadPaymentHistoryDistributionStatusHistorys();
                lobjPaymentHistoryDistribution.LoadPayee();
                
			}

			return lobjPaymentHistoryDistribution;
		}

		public busPaymentHistoryDistributionStatusHistory FindPaymentHistoryDistributionStatusHistory(int aintDistributionStatusHistoryId)
		{
			busPaymentHistoryDistributionStatusHistory lobjPaymentHistoryDistributionStatusHistory = new busPaymentHistoryDistributionStatusHistory();
			if (lobjPaymentHistoryDistributionStatusHistory.FindPaymentHistoryDistributionStatusHistory(aintDistributionStatusHistoryId))
			{
			}

			return lobjPaymentHistoryDistributionStatusHistory;
		}

		public busPaymentHistoryHeaderLookup LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
		{
			busPaymentHistoryHeaderLookup lobjPaymentHistoryHeaderLookup = new busPaymentHistoryHeaderLookup();
			lobjPaymentHistoryHeaderLookup.LoadPaymentHistoryHeaders(adtbSearchResult);
			return lobjPaymentHistoryHeaderLookup;
		}

		public busPaymentScheduleStep FindPaymentScheduleStep(int aintPaymentScheduleStepId)
		{
			busPaymentScheduleStep lobjPaymentScheduleStep = new busPaymentScheduleStep();
			if (lobjPaymentScheduleStep.FindPaymentScheduleStep(aintPaymentScheduleStepId))
			{
			}

			return lobjPaymentScheduleStep;
		}

		public busPaymentCheckBook FindPaymentCheckBook(int aintCheckBookId)
		{
			busPaymentCheckBook lobjPaymentCheckBook = new busPaymentCheckBook();
			if (lobjPaymentCheckBook.FindPaymentCheckBook(aintCheckBookId))
			{
			}

			return lobjPaymentCheckBook;
		}

        public busPayeeAccountTaxWithholding LoadPayAccountTaxWithholdingCalculator()
        {
            busPayeeAccountTaxWithholding lobjPayAccountTaxWithholdingCalculator = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
            lobjPayAccountTaxWithholdingCalculator.LoadTaxYear();

            return lobjPayAccountTaxWithholdingCalculator;

        }

       

		public busPaymentStepRef FindPaymentStepRef(int aintPaymentStepId)
		{
			busPaymentStepRef lobjPaymentStepRef = new busPaymentStepRef();
			if (lobjPaymentStepRef.FindPaymentStepRef(aintPaymentStepId))
			{
				lobjPaymentStepRef.LoadPaymentScheduleSteps();
			}

			return lobjPaymentStepRef;
		}

        #region Wihtholding Information
        public busPayeeAccount FindWithholdingInformation(int aintPayeeAccountId)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
         
            if (lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lobjPayeeAccount.LoadWithholdingInformation();
                lobjPayeeAccount.LoadPayeeAccountStatuss();

                foreach (busWithholdingInformation lbusWithholdingInformation in lobjPayeeAccount.iclbWithholdingInformation)
                {
                    lbusWithholdingInformation.iclbWithholdingInformationHistoryDetail = new Collection<busWithholdingInformationHistoryDetail>();
                    lbusWithholdingInformation.LoadWithholdingInformationHistoryDetail();
                }

                lobjPayeeAccount.LoadWithholdingInformationHistoryDetail();
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
                //Participant Account Details
                if (lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id != 0)
                {
                    lobjPayeeAccount.ibusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lobjPayeeAccount.ibusPayeeBenefitAccount.FindPayeeBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.payee_benefit_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.ibusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id);
                }
                //RID 83533
                lobjPayeeAccount.astrCheckCurrentPage = busConstant.WITHHOLDING_INFORMATION_MAINTENANCE;
            }
            return lobjPayeeAccount;
        }
        #endregion

        public busActiveRetireeIncreaseContract FindActiveRetireeIncreaseContract(int aintActiveRetireeIncreaseContractId)
		{
			busActiveRetireeIncreaseContract lobjActiveRetireeIncreaseContract = new busActiveRetireeIncreaseContract();
			if (lobjActiveRetireeIncreaseContract.FindActiveRetireeIncreaseContract(aintActiveRetireeIncreaseContractId))
			{
                lobjActiveRetireeIncreaseContract.LoadActiveRetireeIncreaseContractHistorys();
			}

			return lobjActiveRetireeIncreaseContract;
		}

		public busOnetimeRetireePaymentContract LoadOnetimeRetireePaymentContracts(DataTable adtbSearchResult)
		{
            busOnetimeRetireePaymentContract lobjnetimeRetireePaymentContractLookup = new busOnetimeRetireePaymentContract();
            lobjnetimeRetireePaymentContractLookup.LoadOnetimeRetireePaymentContracts(adtbSearchResult);
			return lobjnetimeRetireePaymentContractLookup;
		}

        public busOnetimeRetireePaymentContract FindOneTimeRetireePaymentContract(int aintOneTimeRetireePaymentContractId)
        {
            busOnetimeRetireePaymentContract lobjOnetimeRetireePaymentContract = new busOnetimeRetireePaymentContract();
            if (lobjOnetimeRetireePaymentContract.FindOnetimeRetireePaymentContract(aintOneTimeRetireePaymentContractId))
            {
                //lobjOnetimeRetireePaymentContract.LoadOnetimeRetireePaymentContracts();
            }

            return lobjOnetimeRetireePaymentContract;
        }

        public busActiveRetireeIncreaseContractLookup LoadActiveRetireeIncreaseContracts(DataTable adtbSearchResult)
        {
            busActiveRetireeIncreaseContractLookup lobjActiveRetireeIncreaseContractLookup = new busActiveRetireeIncreaseContractLookup();
            lobjActiveRetireeIncreaseContractLookup.LoadActiveRetireeIncreaseContracts(adtbSearchResult);
            return lobjActiveRetireeIncreaseContractLookup;
        }

        public busActiveRetireeIncreaseContractHistory FindActiveRetireeIncreaseContractHistory(int aintActiveRetireeIncreaseContractHistoryId)
		{
			busActiveRetireeIncreaseContractHistory lobjActiveRetireeIncreaseContractHistory = new busActiveRetireeIncreaseContractHistory();
			if (lobjActiveRetireeIncreaseContractHistory.FindActiveRetireeIncreaseContractHistory(aintActiveRetireeIncreaseContractHistoryId))
			{
			}

			return lobjActiveRetireeIncreaseContractHistory;
		}

		public busDisabilityRetireeIncrease FindDisabilityRetireeIncrease(int aintDisabilityRetireeIncreaseId)
		{
			busDisabilityRetireeIncrease lobjDisabilityRetireeIncrease = new busDisabilityRetireeIncrease();
			if (lobjDisabilityRetireeIncrease.FindDisabilityRetireeIncrease(aintDisabilityRetireeIncreaseId))
			{
			}

			return lobjDisabilityRetireeIncrease;
		}

        #region Validate New
        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {

            ArrayList larrErrors = null;
            iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == busConstant.PAYMENT_SCHEDULE_LOOKUP)
                {
                    busPaymentScheduleLookup lbusPaymentScheduleLookup = new busPaymentScheduleLookup();
                    larrErrors = lbusPaymentScheduleLookup.ValidateNew(ahstParam);
                }
                if (astrFormName == busConstant.RECIPIENT_ROLLOVER_ORGANISATION_LOOKUP)
                {
                    busRecipientRolloverOrganisationLookup lbusRecipientRolloverOrganisationLookup = new busRecipientRolloverOrganisationLookup();
                    larrErrors = lbusRecipientRolloverOrganisationLookup.ValidateNew(ahstParam);
                }
                if(astrFormName == "wfmPayeeAccountBenefitOverpaymentMaintenance")
                {
                    busPayeeAccountBenefitOverpayment lbusPayeeAccountBenefitOverpayment = new busPayeeAccountBenefitOverpayment();
                    larrErrors = lbusPayeeAccountBenefitOverpayment.ValidateNew(ahstParam);
                }
                if (astrFormName == busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_LOOKUP_FORM)
                {
                    busReturnToWorkRequestLookup lbusReturnToWorkRequestLookup = new busReturnToWorkRequestLookup();
                    larrErrors = lbusReturnToWorkRequestLookup.ValidateNew(ahstParam);
                }

            }
            finally
            {
                iobjPassInfo.iconFramework.Close(); 
            }
            return larrErrors;
        }
        #endregion

        #region Load Recipient Rollover Organisations
        public busRecipientRolloverOrganisationLookup LoadRecipientRolloverOrganisations(DataTable adtbSearchResult)
		{
			busRecipientRolloverOrganisationLookup lobjRecipientRolloverOrganisationLookup = new busRecipientRolloverOrganisationLookup();
			lobjRecipientRolloverOrganisationLookup.LoadPayeeAccountRolloverDetails(adtbSearchResult);
			return lobjRecipientRolloverOrganisationLookup;
		}
        #endregion

        #region FindPaymentReissueDetail
        public busPaymentHistoryDistribution FindPaymentReissueDetail(int aintPaymentHistoryDistributionId,string astrBenefitOptionValue)
        {
            busPaymentHistoryDistribution lbusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
            lbusPaymentHistoryDistribution.FindPaymentHistoryDistribution(aintPaymentHistoryDistributionId);
            lbusPaymentHistoryDistribution.iclbPaymentReissueDetail = new Collection<busPaymentReissueDetail>();
       

            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader();
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.FindPaymentHistoryHeader(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id);
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id);
            lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.iintPayeeAccountID = lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id;
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadBenefitDetails();
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadDRODetails();
            lbusPaymentHistoryDistribution.LoadInitialData(astrBenefitOptionValue);
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            lbusPaymentHistoryDistribution.LoadOrgMpiID( lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id);
            //lbusPaymentHistoryDistribution.astrCheckCurrentPage = busConstant.PAYEE_ACCOUNT_TAXWITHHOLDING_MAINTENANCE;
            return lbusPaymentHistoryDistribution;
        }
        #endregion

        #region Find Benefit Monthwise Adjustment Detail
        public busBenefitMonthwiseAdjustmentDetail FindBenefitMonthwiseAdjustmentDetail(int aintBenefitMonthwiseAdjustmentDetailId)
		{
			busBenefitMonthwiseAdjustmentDetail lobjBenefitMonthwiseAdjustmentDetail = new busBenefitMonthwiseAdjustmentDetail();
			if (lobjBenefitMonthwiseAdjustmentDetail.FindBenefitMonthwiseAdjustmentDetail(aintBenefitMonthwiseAdjustmentDetailId))
			{
			}

			return lobjBenefitMonthwiseAdjustmentDetail;
		}
        #endregion

        #region Find Payee Account Benefit Overpayment
        public busPayeeAccountBenefitOverpayment FindPayeeAccountBenefitOverpayment(int aintPayeeAccountRetroPaymentId)
		{
			busPayeeAccountBenefitOverpayment lobjPayeeAccountBenefitOverpayment = new busPayeeAccountBenefitOverpayment();
			if (lobjPayeeAccountBenefitOverpayment.FindPayeeAccountBenefitOverpayment(aintPayeeAccountRetroPaymentId))
			{
                lobjPayeeAccountBenefitOverpayment.iclbRepaymentSchedule = new Collection<busRepaymentSchedule>();
                lobjPayeeAccountBenefitOverpayment.iclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                if (lobjPayeeAccountBenefitOverpayment.icdoPayeeAccountRetroPayment.payee_account_id != 0)
                {
                    lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.FindPayeeAccount(lobjPayeeAccountBenefitOverpayment.icdoPayeeAccountRetroPayment.payee_account_id);
                    if (lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.icdoPayeeAccount.person_id != 0)
                    {
                        lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
                        lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.ibusPayee.FindPerson(lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.icdoPayeeAccount.person_id);
                    }
                    //Organization Details
                    if (lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.icdoPayeeAccount.org_id != 0)
                    {
                        lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                        lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.ibusOrganization.FindOrganization(lobjPayeeAccountBenefitOverpayment.ibusPayeeAccount.icdoPayeeAccount.org_id);
                    }
                }

                lobjPayeeAccountBenefitOverpayment.LoadBenefitMonthwiseAdjustmentDetails();
                lobjPayeeAccountBenefitOverpayment.LoadRepaymentSchedule();
                lobjPayeeAccountBenefitOverpayment.LoadPayeeAccountRetroPaymentDetails();

                if (lobjPayeeAccountBenefitOverpayment.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RETRO_PAYMENT_ADJUSTMENT_BATCH)
                {

                    if (lobjPayeeAccountBenefitOverpayment.iclbRepaymentSchedule.Count == 0)
                    {
                        lobjPayeeAccountBenefitOverpayment.LoadPreviousRepaymentSchedule();
                    }
                    else
                    {
                        foreach (busRepaymentSchedule lbusRepaymentSchedule in lobjPayeeAccountBenefitOverpayment.iclbRepaymentSchedule)
                        {
                            if (lbusRepaymentSchedule.icdoRepaymentSchedule.reimbursement_status_value != busConstant.REIMBURSEMENT_STATUS_COMPLETED)
                            {
                                lobjPayeeAccountBenefitOverpayment.LoadPreviousRepaymentSchedule();
                                break;
                            }
                        }
                    }
                }
                
			}

			return lobjPayeeAccountBenefitOverpayment;
		}
        #endregion

        #region Payment Check Books
        public busPaymentCheckBookLookup LoadPaymentCheckBooks(DataTable adtbSearchResult)
		{
			busPaymentCheckBookLookup lobjPaymentCheckBookLookup = new busPaymentCheckBookLookup();
			lobjPaymentCheckBookLookup.LoadPaymentCheckBooks(adtbSearchResult);
			return lobjPaymentCheckBookLookup;
		}

        public busPaymentCheckBook NewPaymentCheckBook()
        {
            busPaymentCheckBook lbusPaymentCheckBook = new busPaymentCheckBook() { icdoPaymentCheckBook = new cdoPaymentCheckBook() };
            return lbusPaymentCheckBook;

        }
        #endregion

        #region Excess Refund Payment Find and Lookup
        public busExcessRefund FindExcessRefund(int aintExcessRefunId)
		{
			busExcessRefund lobjExcessRefund = new busExcessRefund();
			if (lobjExcessRefund.FindExcessRefund(aintExcessRefunId))
			{
			}

			return lobjExcessRefund;
		}

		public busExcessRefundLookup LoadExcessRefunds(DataTable adtbSearchResult)
		{
			busExcessRefundLookup lobjExcessRefundLookup = new busExcessRefundLookup();
			lobjExcessRefundLookup.LoadExcessRefunds(adtbSearchResult);
			return lobjExcessRefundLookup;
        }
        #endregion

		public busPaymentReissueItemDetail FindPaymentReissueItemDetail(int aintPaymentReissueItemDetailId)
		{
			busPaymentReissueItemDetail lobjPaymentReissueItemDetail = new busPaymentReissueItemDetail();
			if (lobjPaymentReissueItemDetail.FindPaymentReissueItemDetail(aintPaymentReissueItemDetailId))
			{
			}

			return lobjPaymentReissueItemDetail;
		}

		public busRetireeIncreaseLookup LoadRetireeIncreases(DataTable adtbSearchResult)
		{
			busRetireeIncreaseLookup lobjRetireeIncreaseLookup = new busRetireeIncreaseLookup();
			lobjRetireeIncreaseLookup.LoadPayeeAccounts(adtbSearchResult);
			return lobjRetireeIncreaseLookup;
		}
    }
}
