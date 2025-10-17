#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace MPIPHP.BusinessTier
{
	public class srvOrganization : srvMPIPHP
	{
		public srvOrganization()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        private string istrVIPAccess;
        private void SetWebParameters()
        {
            if (iobjPassInfo.idictParams.ContainsKey("Logged_In_User_is_VIP"))
                istrVIPAccess = (string)iobjPassInfo.idictParams["Logged_In_User_is_VIP"];
            
        }

        public busOrganization NewOrganization(string astrOrgType)
        {
            busOrganization lobjbusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjbusOrganization.icdoOrganization.org_type_value = astrOrgType;

            switch (lobjbusOrganization.icdoOrganization.org_type_value)
            {
                case busConstant.Organization.OrgPaymentTypeRolloverOrg:
                    lobjbusOrganization.icdoOrganization.payment_type_value = busConstant.PAYMENT_METHOD_ROLLOVER_CHECK;
                    break;

                default:
                    lobjbusOrganization.icdoOrganization.payment_type_value = busConstant.PAYMENT_METHOD_CHECK;
                    break;
            }

            lobjbusOrganization.icdoOrganization.PopulateDescriptions();
            lobjbusOrganization.EvaluateInitialLoadRules(utlPageMode.New);
            lobjbusOrganization.iclbNotes = new Collection<busNotes>(); 
            return lobjbusOrganization;
        }

		public busOrganization FindOrganization(int aintorgid)
		{
			busOrganization lobjOrganization = new busOrganization();
			if (lobjOrganization.FindOrganization(aintorgid))
			{
                lobjOrganization.LoadOrgAddresss();
                lobjOrganization.LoadOrgBanks();
                lobjOrganization.iclbNotes = busGlobalFunctions.LoadNotes(0, lobjOrganization.icdoOrganization.org_id, busConstant.ORG_MAINTAINENCE_FORM);

			}

			return lobjOrganization;
		}

		public busOrganizationLookup LoadOrganizations(DataTable adtbSearchResult)
		{
			busOrganizationLookup lobjOrganizationLookup = new busOrganizationLookup();
			lobjOrganizationLookup.LoadOrganizations(adtbSearchResult);
			return lobjOrganizationLookup;
		}

		public busOrgAddress FindOrgAddress(int aintorgaddressid)
		{
			busOrgAddress lobjOrgAddress = new busOrgAddress();
			if (lobjOrgAddress.FindOrgAddress(aintorgaddressid))
			{
                lobjOrgAddress.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjOrgAddress.ibusOrganization.FindOrganization(lobjOrgAddress.icdoOrgAddress.org_id);
                lobjOrgAddress.ibusOrganization.LoadOrgAddresss();
			}

			return lobjOrgAddress;
		}

        public busOrganization FindOrgBank(int aintOrgId)
        {
            busOrganization lbusOrganization = new busOrganization();

            if (lbusOrganization.FindOrganization(aintOrgId))
            {
               
                lbusOrganization.LoadOrgBanks();
                lbusOrganization.iclbNotes = busGlobalFunctions.LoadNotes(0, lbusOrganization.icdoOrganization.org_id, busConstant.ORG_MAINTAINENCE_FORM);
                //lbusOrganization.iclbOrgAddress = new Collection<busOrgAddress>();
                //lbusOrganization.iclbOrgBank = new Collection<busOrgBank>();
               
            }
            return lbusOrganization;
        }

		public busOrgAddress NewOrgAddress(int aintOrgAddressId)
		{
			busOrgAddress lobjOrgAddress = new busOrgAddress();
            lobjOrgAddress.icdoOrgAddress = new cdoOrgAddress();
            lobjOrgAddress.icdoOrgAddress.org_id = aintOrgAddressId;
            lobjOrgAddress.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjOrgAddress.ibusOrganization.FindOrganization(aintOrgAddressId);
            lobjOrgAddress.ibusOrganization.LoadOrgAddresss();


			return lobjOrgAddress;
		}


        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == busConstant.ORG_LOOKUP)
                {
                    busOrganizationLookup lbusOrganizationLookup = new busOrganizationLookup();
                    larrErrors =lbusOrganizationLookup.ValidateNew(ahstParam);
                }
            }
            finally
            {
                iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNewChild(string astrFormName, object aobjParentObject, Type atypBusObject, Hashtable ahstParams)
         {
            ArrayList larrErrors = new ArrayList();
            if (astrFormName == busConstant.ORGANIZATION_BANK_MAINTENANCE)
            {
              
                busOrgBank lbusOrgBank = new busOrgBank();
                if (atypBusObject.Name == busConstant.ORG_BANK)
                {
                    larrErrors = lbusOrgBank.CheckErrorOnAddButton(aobjParentObject as busOrganization, ahstParams, ref larrErrors);
                }
            }


            return larrErrors;

        }
        protected override ArrayList ValidateGridUpdateChild(string astrFormName, object aobjParentObject, object aobjChildObject, Hashtable ahstParams)
        {
            if (astrFormName == busConstant.ORGANIZATION_BANK_MAINTENANCE)
            {
                ArrayList iarrResult = new ArrayList();
                utlError lobjError = null;
                busMainBase lbusMainBase = new busMainBase();

                int iintOrgBankID = 0; string StatusValue = string.Empty;
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoOrgBank.org_bank_id"])))
                {
                    iintOrgBankID = Convert.ToInt32(ahstParams["icdoOrgBank.org_bank_id"]);
                }
                StatusValue = Convert.ToString(ahstParams["icdoOrgBank.status_value"]);

                if (StatusValue == busConstant.OrgBankStatusActive)
                {
                    busOrganization lbusOrganization1 = aobjParentObject as busOrganization;
                    int CountOrgBankWithActiveStatus = (from obj in lbusOrganization1.iclbOrgBank where obj.icdoOrgBank.status_value == busConstant.OrgBankStatusActive && obj.icdoOrgBank.org_bank_id != iintOrgBankID select obj).Count();
                    if (CountOrgBankWithActiveStatus > 0)
                    {
                        lobjError = lbusMainBase.AddError(6105, "");
                        iarrResult.Add(lobjError);
                        return iarrResult;
                    }
                }
                if (iarrResult.Count > 0)
                {
                    return iarrResult;
                }
            }
            return base.ValidateGridUpdateChild(astrFormName, aobjParentObject, aobjChildObject, ahstParams);
        }

		public busOrganizationBankLookup LoadOrganizationBanks(DataTable adtbSearchResult)
		{
			busOrganizationBankLookup lobjOrganizationBankLookup = new busOrganizationBankLookup();
			lobjOrganizationBankLookup.LoadOrgBanks(adtbSearchResult);
			return lobjOrganizationBankLookup;
		}
        public busReturnedMail NewReturnedMail()
        {
            SetWebParameters();
            busReturnedMail lbusReturnedMail = new busReturnedMail { icdoReturnedMail = new cdoReturnedMail() };
            lbusReturnedMail.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            lbusReturnedMail.ibusLookupOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            if(iobjPassInfo.istrFormName == "wfmReturnMailOrganizationWizard" && istrVIPAccess.IsNotNullOrEmpty() && istrVIPAccess == "VIPAccessUser")
            {
                lbusReturnedMail.astrVipAcc = "Y";
            }
            
            return lbusReturnedMail;
        }
        public busReturnedMail FindReturnedMail(int aintReturnedMailId)
        {
            busReturnedMail lbusReturnedMail = new busReturnedMail();
            if (lbusReturnedMail.FindReturnedMail(aintReturnedMailId))
            {
                lbusReturnedMail.LoadAllOrgAddress();
            }

            return lbusReturnedMail;
        }
        public busOrganizationLookup LoadOrganizationsReturnedMail(DataTable adtbSearchResult)
        {
            busOrganizationLookup lobjOrganizationLookup = new busOrganizationLookup();
            lobjOrganizationLookup.LoadOrganizations(adtbSearchResult);
            return lobjOrganizationLookup;
        }
        public busOrganization FindOrgReturnMail(int aintOrgid)
        {
            busOrganization lobjOrg = new busOrganization();
            if (lobjOrg.FindOrganization(aintOrgid))
            {
                lobjOrg.LoadReturnedMail();
            }
            return lobjOrg;
        }
        public override string GetMessageText(string astrMessage, int aintBusMessageID, int aintButtonMessageID, int aintDefaultMessgeId, params object[] aarrParam)
        {
            if (iobjPassInfo.istrSenderForm == "wfmOrganizationBankMaintenance" && iobjPassInfo.istrSenderID == "btnSave" && iobjPassInfo.idictParams.ContainsKey("SaveMesssageOnOrganization"))
            {
                astrMessage = "No changes to save.";
            }
            return base.GetMessageText(astrMessage, aintBusMessageID, aintButtonMessageID, aintDefaultMessgeId, aarrParam);
        }
    }
}
