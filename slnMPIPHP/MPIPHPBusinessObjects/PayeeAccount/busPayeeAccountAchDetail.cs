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
	/// Class MPIPHP.BusinessObjects.busPayeeAccountAchDetail:
	/// Inherited from busPayeeAccountAchDetailGen, the class is used to customize the business object busPayeeAccountAchDetailGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountAchDetail : busPayeeAccountAchDetailGen
	{
        public bool IsStartDateNull()
        {
            if (this.icdoPayeeAccountAchDetail.ach_start_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsENDDateNull()
        {
            if (this.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            string astrPrimaryFlag = string.Empty;
            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;
            int iintBankOrgID = 0;

            ahstParams["icdoPayeeAccountAchDetail.bank_account_type_value"] = ahstParams["bank_account_type_value"];
            ahstParams["icdoPayeeAccountAchDetail.ach_start_date"] = ahstParams["ach_start_date"];
            ahstParams["icdoPayeeAccountAchDetail.ach_end_date"] = ahstParams["ach_end_date"];
            ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"] = ahstParams["istrRoutingNumber"];
            ahstParams["icdoPayeeAccountAchDetail.account_number"] = ahstParams["account_number"];
            ahstParams["icdoPayeeAccountAchDetail.bank_account_type_value"] = ahstParams["bank_account_type_value"];
            string astrAccountType = Convert.ToString(ahstParams["bank_account_type_value"]);
            ahstParams["icdoPayeeAccountAchDetail.bank_org_id"] = ahstParams["bank_org_id"];

            if (ahstParams.Count > 0)
            {
                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]) == "")
                    iintBankOrgID = 0;
                else
                    iintBankOrgID = Convert.ToInt32(ahstParams["icdoPayeeAccountAchDetail.bank_org_id"]);

                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).Length != 9)
                {
                    lobjError = AddError(6059, "");
                    aarrErrors.Add(lobjError);
                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).IsNotNullOrEmpty() || Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]).Length == 9)
                {
                    DataTable ldtblRoutingNumber = Select("cdoOrganization.GetOrgDetailsByRoutingNumber", new object[1] { Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.istrRoutingNumber"]) });
                    if (ldtblRoutingNumber != null && ldtblRoutingNumber.Rows.Count > 0)
                    { }
                    else
                    {
                        lobjError = AddError(6178, "");
                        aarrErrors.Add(lobjError);
                    }

                }

                if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.account_number"]).IsNullOrEmpty())
                {
                    lobjError = AddError(6026, "");
                    aarrErrors.Add(lobjError);
                }

                if (astrAccountType.IsNullOrEmpty())
                {
                    lobjError = AddError(6020, "");
                    aarrErrors.Add(lobjError);
                }

                if (!lbusPayeeAccount.iclbPayeeAccountAchDetail.IsNullOrEmpty())
                {
                    int CountRecordwithEndDateNull = (from obj in lbusPayeeAccount.iclbPayeeAccountAchDetail
                                                      where obj.icdoPayeeAccountAchDetail.ach_end_date == null || (obj.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
                                                      select obj).Count();
                    if (CountRecordwithEndDateNull > 0)
                    {
                        lobjError = AddError(0, "Please put end date to other record before adding new record");
                        aarrErrors.Add(lobjError);
                    }
                    var MaxEndDate = (from obj in lbusPayeeAccount.iclbPayeeAccountAchDetail

                                      select obj.icdoPayeeAccountAchDetail.ach_end_date).Max();
                    if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty()
                        && MaxEndDate >= Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]))
                    {
                        lobjError = AddError(0, "Start Date must be greater than last end date");
                        aarrErrors.Add(lobjError);
                    }
                  }

                   if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNullOrEmpty())
                   {
                        lobjError = AddError(5113, "");
                        aarrErrors.Add(lobjError);
                   }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty() && Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]) < DateTime.Today)
                    {
                        lobjError = AddError(5112, "");
                        aarrErrors.Add(lobjError);
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty()
                        && Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_end_date"]) <= Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]))
                    {
                        lobjError = AddError(5111, "");
                        aarrErrors.Add(lobjError);
                    }

                    if (Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_end_date"]).IsNotNullOrEmpty() && Convert.ToString(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]).IsNotNullOrEmpty()
                        && Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_start_date"]) > Convert.ToDateTime(ahstParams["icdoPayeeAccountAchDetail.ach_end_date"]))
                    {
                        lobjError = AddError(5139, "");
                        aarrErrors.Add(lobjError);
                    }
            }
            
            return aarrErrors;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            //utlError lobjError = null;
            //if (this.iarrErrors.IsNull())
            //    this.iarrErrors = new ArrayList();

            //if (this.icdoPayeeAccountAchDetail.iintRoutingNumber > 0 && Convert.ToString(this.icdoPayeeAccountAchDetail.iintRoutingNumber).Length != 9)
            //{
            //    lobjError = AddError(6059, "");
            //    iarrErrors.Add(lobjError);
            //}
        }

	}
}
