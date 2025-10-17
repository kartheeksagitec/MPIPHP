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
using NeoSpin.DataObjects;
using System.Linq;
#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    ///  partial class NeoSpin.BusinessObjects.busPayeeAccountWireDetail
    /// </summary>
	[Serializable]
	public  partial class busPayeeAccountWireDetail : busMPIPHPBase
    {
		/// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayeeAccountWireDetail
        /// </summary>
		public busPayeeAccountWireDetail()
		{
		}

        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            string astrPrimaryFlag = string.Empty;
            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;
            int iintBankOrgID = 0;
            ahstParams["icdoPayeeAccountWireDetail.wire_start_date"] = ahstParams["wire_start_date"];
            ahstParams["icdoPayeeAccountWireDetail.wire_end_date"] = ahstParams["wire_end_date"];
            ahstParams["icdoPayeeAccountWireDetail.call_back_completion_date"] = ahstParams["call_back_completion_date"];
            ahstParams["icdoPayeeAccountWireDetail.bank_account_type_value"] = ahstParams["bank_account_type_value"];
            ahstParams["icdoPayeeAccountWireDetail.bank_org_id"] = ahstParams["bank_org_id"];
            ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"] = ahstParams["istrAbaSwiftBankCode"];
            ahstParams["icdoPayeeAccountWireDetail.beneficiary_account_number"] = ahstParams["beneficiary_account_number"];
            ahstParams["icdoPayeeAccountWireDetail.ach_end_date"] = ahstParams["ach_end_date"];
            ahstParams["icdoPayeeAccountWireDetail.call_back_flag"] = ahstParams["call_back_flag"];
            string astrAccountType = Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.bank_account_type_value"]);

            if (ahstParams.Count > 0)
            {
                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.bank_org_id"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.bank_org_id"]) == "")
                    iintBankOrgID = 0;
                else
                    iintBankOrgID = Convert.ToInt32(ahstParams["icdoPayeeAccountWireDetail.bank_org_id"]);

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).Length == 12)
                {
                    lobjError = AddError(6311, "");
                    //lobjError = AddError(0, "Please enter valid Beneficiary Account Number.");
                    aarrErrors.Add(lobjError);
                }

                //if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).IsNotNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]).Length == 9)
                //{
                //    DataTable ldtblRoutingNumber = Select("cdoOrganization.GetOrgDetailsByRoutingNumber", new object[1] { Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.istrAbaSwiftBankCode"]) });
                //    if (ldtblRoutingNumber != null && ldtblRoutingNumber.Rows.Count > 0)
                //    { }
                //    else
                //    {
                //        lobjError = AddError(6178, "");
                //        aarrErrors.Add(lobjError);
                //    }

                //}

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.beneficiary_account_number"]).IsNullOrEmpty())
                {
                    lobjError = AddError(0, "Please enter valid Beneficiary Account Number.");
                    aarrErrors.Add(lobjError);
                }

                //if (astrAccountType.IsNullOrEmpty())
                //{
                //    lobjError = AddError(6020, "");
                //    aarrErrors.Add(lobjError);
                //}

                if (!lbusPayeeAccount.iclbPayeeAccountWireDetail.IsNullOrEmpty())
                {
                    int CountRecordwithEndDateNull = (from obj in lbusPayeeAccount.iclbPayeeAccountWireDetail
                                                      where obj.icdoPayeeAccountWireDetail.wire_end_date == null || (obj.icdoPayeeAccountWireDetail.wire_end_date == DateTime.MinValue)
                                                      select obj).Count();

                                      
                    if (CountRecordwithEndDateNull > 0)
                    {
                        lobjError = AddError(0, "Please put end date to other record before adding new record");
                        aarrErrors.Add(lobjError);
                    }
                    var MaxEndDate = (from obj in lbusPayeeAccount.iclbPayeeAccountWireDetail

                                      select obj.icdoPayeeAccountWireDetail.wire_end_date).Max();
                    if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty()
                        && MaxEndDate >= Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]))
                    {
                        lobjError = AddError(0, "Start Date must be greater than last end date");
                        aarrErrors.Add(lobjError);
                    }
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNullOrEmpty())
                {
                    lobjError = AddError(5113, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]) < DateTime.Today)
                {
                    lobjError = AddError(5112, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.ach_end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty()
                    && Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.ach_end_date"]) <= Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]))
                {
                    lobjError = AddError(5111, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]).IsNotNullOrEmpty()
                    && Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_start_date"]) > Convert.ToDateTime(ahstParams["icdoPayeeAccountWireDetail.wire_end_date"]))
                {
                    lobjError = AddError(5139, "");
                    aarrErrors.Add(lobjError);
                }
            }

            return aarrErrors;
        }

    }
}
