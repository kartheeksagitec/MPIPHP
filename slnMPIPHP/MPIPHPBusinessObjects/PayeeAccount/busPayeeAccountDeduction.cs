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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPayeeAccountDeduction:
	/// Inherited from busPayeeAccountDeductionGen, the class is used to customize the business object busPayeeAccountDeductionGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountDeduction : busPayeeAccountDeductionGen
	{
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public busPayeeAccountPaymentItemType ibusPayeeAccountPaymentItemType { get; set; }       

        #region Check Error On Add Button
        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {

            utlError lobjError = null;

            ahstParams["icdoPayeeAccountDeduction.deduction_type_value"] = ahstParams["deduction_type_value"];
            ahstParams["icdoPayeeAccountDeduction.pay_to_value"] = ahstParams["pay_to_value"];
            ahstParams["icdoPayeeAccountDeduction.amount"] = ahstParams["amount"];
            ahstParams["icdoPayeeAccountDeduction.org_id"] = ahstParams["org_id"];
            ahstParams["icdoPayeeAccountDeduction.person_id"] = ahstParams["person_id"];
            ahstParams["icdoPayeeAccountDeduction.other_deduction_type_description"] = ahstParams["other_deduction_type_description"];
            ahstParams["icdoPayeeAccountDeduction.start_date"] = ahstParams["start_date"];
            ahstParams["icdoPayeeAccountDeduction.end_date"] = ahstParams["end_date"];

            string astrDeductionType = Convert.ToString(ahstParams["icdoPayeeAccountDeduction.deduction_type_value"]);
            string astrPayTo = Convert.ToString(ahstParams["icdoPayeeAccountDeduction.pay_to_value"]);
            int iintOrgID = 0;
            int iintPersonID = 0;
            decimal ldecDeductionAmount = 0.00m;

            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;

            if (ahstParams.Count > 0)
            {

                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.amount"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountDeduction.amount"]) == "")
                    ldecDeductionAmount = Decimal.Zero;
                else
                    if (busGlobalFunctions.IsDecimal(Convert.ToString(ahstParams["icdoPayeeAccountDeduction.amount"])))
                {
                    ldecDeductionAmount = Convert.ToDecimal(ahstParams["icdoPayeeAccountDeduction.amount"]);
                    if (ldecDeductionAmount < 0)
                    {
                        lobjError = AddError(6141, " ");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }
                else
                {
                    lobjError = AddError(6069, " ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.org_id"]).IsNullOrEmpty())
                    iintOrgID = 0;
                else
                    iintOrgID = Convert.ToInt32(ahstParams["icdoPayeeAccountDeduction.org_id"]);
                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.person_id"]).IsNullOrEmpty())
                    iintPersonID = 0;
                else
                    iintPersonID = Convert.ToInt32(ahstParams["icdoPayeeAccountDeduction.person_id"]);
                if (astrDeductionType.IsNullOrEmpty())
                {
                    lobjError = AddError(6023, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo != busConstant.SURVIVOR_TYPE_PERSON) && (iintOrgID.IsNull() || iintOrgID == 0))
                {
                    lobjError = AddError(6025, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNull() || iintPersonID == 0))
                {
                    lobjError = AddError(6070, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (astrDeductionType == busConstant.CANCELLATION_REASON_OTHER && Convert.ToString(ahstParams["icdoPayeeAccountDeduction.other_deduction_type_description"]).IsNullOrEmpty())
                {
                    lobjError = AddError(6032, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (astrDeductionType.IsNullOrEmpty() && astrPayTo.IsNullOrEmpty())
                {
                    lobjError = AddError(6024, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (ldecDeductionAmount == Decimal.Zero)
                {
                    lobjError = AddError(6027, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                else
                {
                    if (lbusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    if (lbusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lbusPayeeAccount.LoadNextBenefitPaymentDate();
                    // Active Payee Account Payment Item type

                    decimal idecTotalAmount = (from item in lbusPayeeAccount.iclbPayeeAccountPaymentItemType
                                               where busGlobalFunctions.CheckDateOverlapping(lbusPayeeAccount.idtNextBenefitPaymentDate,
                                               item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                               && !((item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "FEDX"
                                               && item.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value != "STTX")
                                               && item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1
                                               && item.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != "RRED"
                                               && item.ibusPaymentItemType.icdoPaymentItemType.item_type_code != busConstant.ITEM53)
                                               select item.icdoPayeeAccountPaymentItemType.amount * item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();

                    if (lbusPayeeAccount.iclbPayeeAccountDeduction.Count > 0)
                    {
                        ldecDeductionAmount = ldecDeductionAmount + lbusPayeeAccount.iclbPayeeAccountDeduction.Sum(obj => obj.icdoPayeeAccountDeduction.amount);
                    }

                    if (idecTotalAmount < ldecDeductionAmount)
                    {
                        lobjError = AddError(6202, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                }

                //PIR 924
                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_PERSON) && (iintPersonID.IsNotNull() || iintPersonID != 0)
                    && lbusPayeeAccount.icdoPayeeAccount.person_id == iintPersonID)
                {
                    lobjError = AddError(6283, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if ((astrPayTo.IsNotNullOrEmpty() && astrPayTo == busConstant.SURVIVOR_TYPE_ORG) && (iintOrgID.IsNotNull() || iintOrgID != 0)
                && lbusPayeeAccount.icdoPayeeAccount.org_id == iintOrgID)
                {
                    lobjError = AddError(6283, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.start_date"]).IsNotNullOrEmpty())
                {
                    DateTime adtStartDate = Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.start_date"]);

                    if (adtStartDate < DateTime.Today)
                    {
                        lobjError = AddError(5112, "");
                        aarrErrors.Add(lobjError);
                    }
                    if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.end_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.end_date"]) <= Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.start_date"]))
                    {
                        lobjError = AddError(5111, "");
                        aarrErrors.Add(lobjError);
                    }

                }
                else
                {
                    lobjError = AddError(5113, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountDeduction.end_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoPayeeAccountDeduction.end_date"]) < DateTime.Today)
                {
                    lobjError = AddError(5081, "");
                    aarrErrors.Add(lobjError);
                }

            }
            return aarrErrors;
        }
        #endregion

        public bool CheckMailAddress(DateTime PaymentDate)
        {
            DataTable dtlAddress=new DataTable();
            if (this.icdoPayeeAccountDeduction.pay_to_value == busConstant.SURVIVOR_TYPE_PERSON)
            {
                dtlAddress = busBase.Select("cdoPerson.GetPersonMailingAddress", new object[2] { PaymentDate, this.icdoPayeeAccountDeduction.person_id });
            }
            else
            {
                dtlAddress = busBase.Select("cdoOrganization.GetOrgMailingAddress", new object[2] { PaymentDate, this.icdoPayeeAccountDeduction.org_id });
            }
            if (dtlAddress.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
	}
}
