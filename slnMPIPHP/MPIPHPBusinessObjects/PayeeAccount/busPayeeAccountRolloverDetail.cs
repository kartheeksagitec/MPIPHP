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
using Sagitec.DataObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.DataObjects;
#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayeeAccountRolloverDetail:
	/// Inherited from busPayeeAccountRolloverDetailGen, the class is used to customize the business object busPayeeAccountRolloverDetailGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountRolloverDetail : busPayeeAccountRolloverDetailGen
    {

        public decimal idecRolloverAmountRequired { get; set; }
        public string istrPayableTo { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public busOrganization ibusOrganization { get; set; }
        #region Check Error On Add Button
        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            object lobj = null;
            decimal adecPercentage;
            decimal total_amount;
			
            ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"] = ahstParams["rollover_option_value"];
            ahstParams["icdoPayeeAccountRolloverDetail.status_value"] = ahstParams["status_value"];
            ahstParams["icdoPayeeAccountRolloverDetail.account_number"] = ahstParams["account_number"];
            ahstParams["icdoPayeeAccountRolloverDetail.addr_line_1"] = ahstParams["addr_line_1"];
            ahstParams["icdoPayeeAccountRolloverDetail.rollover_type_value"] = ahstParams["rollover_type_value"];
            ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"] = ahstParams["rollover_org_id"];
            ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"] = ahstParams["percent_of_taxable"];
            ahstParams["icdoPayeeAccountRolloverDetail.amount"] = ahstParams["amount"];
            ahstParams["icdoPayeeAccountRolloverDetail.contact_name"] = ahstParams["contact_name"];

            string astrRolloverOption = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]);
            string astrStatus = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.status_value"]);
            string astrAccountNumber = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.account_number"]);
            string astrAddress = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.addr_line_1"]);
            string astrRolloverType = Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_type_value"]);
            decimal ldecAmountOfTaxable = 0.0M;
            int iintRolloveOrgId = 0;
            decimal ldecPercentage = 0.0M;
            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;

            DataTable ldtblRetireeIncreaseAccount = Select("cdoPayeeAccount.GetRetireeIncreaseAccount", new object[1] { lbusPayeeAccount.icdoPayeeAccount.payee_account_id });
            if (ldtblRetireeIncreaseAccount.Rows.Count > 0)
            {
                DateTime ldtDateAtAge70AndHalf = lbusPayeeAccount.ibusPayee.icdoPerson.idtDateofBirth.AddYears(70).AddMonths(6);

                DateTime ldtVestedDt = new DateTime();
                DataTable ldtGetVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDateForMD", new object[1] { lbusPayeeAccount.ibusPayee.icdoPerson.person_id });
                if (ldtGetVestedDate != null && ldtGetVestedDate.Rows.Count > 0 && (Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]) != DateTime.MinValue))
                {
                    ldtVestedDt = Convert.ToDateTime(ldtGetVestedDate.Rows[0]["VESTED_DATE"]);
                }

                ldtDateAtAge70AndHalf = busGlobalFunctions.GetMinDistributionDate(lbusPayeeAccount.ibusPayee.icdoPerson.person_id, ldtVestedDt); // busConstant.BenefitCalculation.AGE_70_HALF

                if (ldtDateAtAge70AndHalf.Year == DateTime.Now.Year)
                {
                    lobjError = AddError(6073, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                decimal ldecAmount = Convert.ToDecimal(ldtblRetireeIncreaseAccount.Rows[0][0]);
                if (ldecAmount < 750)
                {
                    lobjError = AddError(6074, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                } 
            }

            if (ahstParams.Count > 0)
            {
                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]) == "")
                    iintRolloveOrgId = 0;
                else
                    iintRolloveOrgId = Convert.ToInt32(ahstParams["icdoPayeeAccountRolloverDetail.rollover_org_id"]);


                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]) == "")
                    ldecAmountOfTaxable = 0.0M;
                else
                    ldecAmountOfTaxable = Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.amount"]);

                
                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]) == "")
                    ldecPercentage = 0.0M;
                else
                    ldecPercentage = Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]);
                
                if (iintRolloveOrgId == 0)
                {
                    lobjError = AddError(6010, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                
                if (iintRolloveOrgId != 0 && astrStatus.IsNullOrEmpty())
                {
                    lobjError = AddError(6011, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                //PIR 988
                if (astrStatus.IsNotNullOrEmpty() && astrStatus != busConstant.PayeeAccountRolloverDetailStatusActive)
                {
                    lobjError = AddError(6277, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (astrAddress.IsNullOrEmpty())
                {
                    lobjError = AddError(1114, " ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0)
                {
                    int iintActiveRolloverCount = (from obj in lbusPayeeAccount.iclbPayeeAccountRolloverDetail
                                                   where obj.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive
                                                   && obj.icdoPayeeAccountRolloverDetail.rollover_org_id == iintRolloveOrgId
                                                   //&& obj.icdoPayeeAccountRolloverDetail.rollover_option_value == astrRolloverOption
                                                   select obj).Count();
                    if (iintActiveRolloverCount > 0)
                    {
                        lobjError = AddError(6009, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (iintRolloveOrgId != 0)
                {
                    ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    ibusOrganization.FindOrganization(iintRolloveOrgId);
                    if (ibusOrganization != null)
                    {
                        if (!(ibusOrganization.icdoOrganization.status_value == "A" &&
                                                       ibusOrganization.icdoOrganization.org_type_value == "RLIT"))
                        {
                            lobjError = AddError(6058, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                        if (ibusOrganization.icdoOrganization.payment_type_value == busConstant.PAYMENT_METHOD_ACH)
                        {
                            ibusOrganization.LoadOrgBanks();
                            if (ibusOrganization.iclbOrgBank.IsNullOrEmpty() || ibusOrganization.iclbOrgBank.Where(obj => obj.icdoOrgBank.status_value == busConstant.STATUS_ACTIVE).Count() > 0)
                            {
                                lobjError = AddError(6103, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                            }
                        }
                    }
                }

                //153521
                //if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNullOrEmpty())
                //{
                //    lobjError = AddError(6072, "");
                //    aarrErrors.Add(lobjError);
                //    return aarrErrors;
                //}

                if (iintRolloveOrgId != 0 && astrStatus.IsNotNullOrEmpty() && astrRolloverType.IsNotNullOrEmpty() && astrRolloverOption.IsNullOrEmpty())
                {
                    lobjError = AddError(6071, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (astrAccountNumber.IsNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.contact_name"]).IsNullOrEmpty())
                {
                    lobjError = AddError(6075, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Count > 0)
                {
                    int iintActiveRolloverCount = (from obj in lbusPayeeAccount.iclbPayeeAccountRolloverDetail
                                                   where obj.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive
                                                   select obj).Count();
                    if (iintActiveRolloverCount >= 3)
                    {
                        lobjError = AddError(6012, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull()
                    && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty())
                {
                    adecPercentage = lbusPayeeAccount.iclbPayeeAccountRolloverDetail.Sum(item => item.icdoPayeeAccountRolloverDetail.percent_of_taxable)
                                                    + Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]);
                    if (adecPercentage > 100)
                    {
                        lobjError = AddError(6013, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull())
                {
                    lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lbusPayeeAccount.LoadBreakDownDetails();
                    if (ldecAmountOfTaxable > lbusPayeeAccount.idecNextNetPaymentACH)
                    {
                        lobjError = AddError(6015, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                    && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.amount"]).IsNotNullOrEmpty())
                {
                    if ((Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAllOfGross ||
                        Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAllOfTaxable)
                        && Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]) != 0)
                    {
                        lobjError = AddError(6016, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }
                // Should not be empty
                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                    && (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNullOrEmpty() || ldecPercentage == 0))
                {
                    if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                    {
                        lobjError = AddError(6029, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionDollorOfGross
                    && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                {
                    if (ldecAmountOfTaxable > 0 || ldecAmountOfTaxable != 0)
                    {
                        lobjError = AddError(6028, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty()
                    && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"]).IsNotNullOrEmpty() && Convert.ToDecimal(ahstParams["icdoPayeeAccountRolloverDetail.percent_of_taxable"])!=0)
                {
                    if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) != busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                    {
                        lobjError = AddError(6017, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty())
                {
                    if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionDollorOfGross ||
                        Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                    {
                        if (ldecAmountOfTaxable == 0 || ldecAmountOfTaxable < 0)
                        {
                            lobjError = AddError(6018, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }
                    }
                }

                if (lbusPayeeAccount.iclbPayeeAccountRolloverDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]).IsNotNullOrEmpty())
                {
                    if (Convert.ToString(ahstParams["icdoPayeeAccountRolloverDetail.rollover_option_value"]) == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                    {
                        if (lbusPayeeAccount.IsNotNull() && ldecAmountOfTaxable > lbusPayeeAccount.idecNextMonthTaxable)
                        {
                            lobjError = AddError(6164, "");
                            aarrErrors.Add(lobjError);
                            return aarrErrors;
                        }

                    }
                }
            }
            return aarrErrors;
        }
        #endregion

        #region Insert into rollover item detail
        public void InsertIntoRolloverItemDetail(busPayeeAccountPaymentItemType aobjRolloverItems)
        {
            busPayeeAccountRolloverItemDetail lobjRolloverItemDetail = new busPayeeAccountRolloverItemDetail() { icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail() };
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id
                                                = icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id
                                                = aobjRolloverItems.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Insert();
        }

        #endregion

        public bool IsRolloverItemDetailExists(int aintPARDId)
        {
            bool lblnResult = false;
            if (aintPARDId != 0 || aintPARDId.IsNotNull())
            {
                DataTable ldtRolloverItemDtl = Select<cdoPayeeAccountRolloverItemDetail>(new string[1] { "payee_account_rollover_detail_id" },
                                                    new object[1] { aintPARDId }, null, null);
                if (ldtRolloverItemDtl.Rows.Count > 0)
                    lblnResult = true;
            }
            return lblnResult;
        }

        public Collection<cdoCodeValue> GetStateOrProvinceBasedOnCountry(int aintCountryValue)
        {
            Collection<cdoCodeValue> lclcStates = null;
            DataTable ldtbResult = null;
            if (aintCountryValue == busConstant.USA)
            {
                ldtbResult = busBase.Select("cdoPersonAddress.GetStatesForUSA", new object[0] { });
                lclcStates = doBase.GetCollection<cdoCodeValue>(ldtbResult);
            }
            else if (aintCountryValue == busConstant.AUSTRALIA || aintCountryValue == busConstant.CANADA || aintCountryValue == busConstant.MEXICO
                || aintCountryValue == busConstant.NewZealand || aintCountryValue == busConstant.OTHER_PROVINCE)
            {
                ldtbResult = busBase.Select("cdoPersonAddress.GetProvinces", new object[1] { aintCountryValue });
                lclcStates = doBase.GetCollection<cdoCodeValue>(ldtbResult);
            }

            return lclcStates;
        }

        public cdoOrgAddress GetOrganizationAddress(string astrMPIOrgID)
        {
            cdoOrgAddress lcdoOrgAddress = new cdoOrgAddress();
            if (astrMPIOrgID.IsNotNullOrEmpty())
            {
                DataTable ldtbOrgAddressResult = busBase.Select("cdoOrganization.GetOrgDetailsWithAddress", new object[1] { astrMPIOrgID });
                if (ldtbOrgAddressResult.IsNotNull() && ldtbOrgAddressResult.Rows.Count > 0)
                {
                    foreach (DataRow drOrgAddress in ldtbOrgAddressResult.Rows)
                    {
                        lcdoOrgAddress.LoadData(drOrgAddress);
                        lcdoOrgAddress.istrOrgName = Convert.ToString(drOrgAddress["ORG_NAME"]);
                        lcdoOrgAddress.istrContactName = Convert.ToString(drOrgAddress["CO"]);
                    }
                }
            }
            return lcdoOrgAddress;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode) 
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            
            if (icdoPayeeAccountRolloverDetail.rollover_org_id == 0)
            {
                lobjError = AddError(6115, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoPayeeAccountRolloverDetail.rollover_org_id != 0 && icdoPayeeAccountRolloverDetail.status_value.IsNullOrEmpty())
            {
                lobjError = AddError(6011, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoPayeeAccountRolloverDetail.rollover_org_id != 0 && icdoPayeeAccountRolloverDetail.status_value.IsNotNullOrEmpty() && icdoPayeeAccountRolloverDetail.rollover_type_value.IsNotNullOrEmpty() && icdoPayeeAccountRolloverDetail.rollover_option_value.IsNullOrEmpty())
            {
                lobjError = AddError(6071, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoPayeeAccountRolloverDetail.account_number.IsNullOrEmpty() && icdoPayeeAccountRolloverDetail.contact_name.IsNullOrEmpty())
            {
                lobjError = AddError(6075, " ");
                this.iarrErrors.Add(lobjError);
            }

//130812 - Business requested to reomve this rollover validation
/*
            DataTable ldtblGetRolloverOption = Select("cdoPayeeAccountRolloverDetail.GetRolloverOptionOfOldRolloverOrg", new object[1] { icdoPayeeAccountRolloverDetail.payee_account_id });
            if (ldtblGetRolloverOption != null && ldtblGetRolloverOption.Rows.Count > 0 &&
                Convert.ToString(ldtblGetRolloverOption.Rows[0][enmPayeeAccountRolloverDetail.rollover_option_value.ToString().ToUpper()]).IsNotNullOrEmpty()
                && Convert.ToString(ldtblGetRolloverOption.Rows[0][enmPayeeAccountRolloverDetail.rollover_type_value.ToString().ToUpper()]).IsNotNullOrEmpty())
            {
                string lstrOldRolloverOption = Convert.ToString(ldtblGetRolloverOption.Rows[0][enmPayeeAccountRolloverDetail.rollover_option_value.ToString().ToUpper()]);
                string lstrOldRolloverType = Convert.ToString(ldtblGetRolloverOption.Rows[0][enmPayeeAccountRolloverDetail.rollover_type_value.ToString().ToUpper()]);

                if (icdoPayeeAccountRolloverDetail.rollover_option_value.Trim() != lstrOldRolloverOption.Trim() ||
                    icdoPayeeAccountRolloverDetail.rollover_type_value.Trim() != lstrOldRolloverType.Trim())
                {
                    busCodeValue lbusCodeValueOption = new busCodeValue();
                    lbusCodeValueOption.icdoCodeValue = lbusCodeValueOption.GetCodeValue(busConstant.ROLLOVER_OPTION_ID, lstrOldRolloverOption);

                     busCodeValue lbusCodeValueType = new busCodeValue();
                    lbusCodeValueType.icdoCodeValue = lbusCodeValueType.GetCodeValue(busConstant.ROLLOVER_TYPES_ID, lstrOldRolloverType);


                    //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                    //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6139);
                    //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                    utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(6139);
                    string lstrMessage = lobjutlMessageInfo.display_message;
                    string lstrErrorMsg = FormatMessageParameters(6139, lstrMessage, lbusCodeValueOption.icdoCodeValue.description + ";" + lbusCodeValueType.icdoCodeValue.description);

                    lobjError = AddError(0, lstrErrorMsg);
                    this.iarrErrors.Add(lobjError);
                }
            }
*/
        }
        public override void BeforePersistChanges()
        {
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {

                ChangeContactName();
            }       
        }
        public void ChangeContactName()
        {

            if (this.icdoPayeeAccountRolloverDetail.status_value
                        == busConstant.PayeeAccountRolloverDetailStatusActive)
            {
                if (this.icdoPayeeAccountRolloverDetail.send_to_participant != "Y")
                {
                    if (this.icdoPayeeAccountRolloverDetail.contact_name.IsNotNullOrEmpty())
                    {
                        if (!(this.icdoPayeeAccountRolloverDetail.contact_name.Contains("ATTN")))
                        {
                            this.icdoPayeeAccountRolloverDetail.contact_name = "ATTN: " + this.icdoPayeeAccountRolloverDetail.contact_name.Trim();

                        }
                    }
                }
            }
        }


        public busPayeeAccountRolloverDetail LoadPayeeAccountRolloverItemDetail(int aintPayeeAccountRolloverDetailId)
        {
            busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };

            lbusPayeeAccountRolloverDetail.FindPayeeAccountRolloverDetail(aintPayeeAccountRolloverDetailId);
            return lbusPayeeAccountRolloverDetail;

        }
    }
}
