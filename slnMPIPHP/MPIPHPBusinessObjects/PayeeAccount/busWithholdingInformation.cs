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
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using Sagitec.Interface;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlTypes;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busWithholdingInformation:
	/// Inherited from busWithholdingInformationGen, the class is used to customize the business object busWithholdingInformationGen.
	/// </summary>
	[Serializable]
	public class busWithholdingInformation : busWithholdingInformationGen
	{
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public Collection<busWithholdingInformationHistoryDetail> iclbWithholdingInformationHistoryDetail { get; set; }

        public void LoadChildForWithholdingInformationMaintenance()
        {
            LoadWithholdingInformationHistoryDetail();
        }

        public void LoadWithholdingInformationHistoryDetail()
        {
            DataTable ldtbList = Select<cdoWithholdingInformationHistoryDetail>(
                new string[1] { enmWithholdingInformationHistoryDetail.withholding_information_id.ToString() },
                new object[1] { icdoWithholdingInformation.withholding_information_id }, null, enmWithholdingInformationHistoryDetail.withholding_information_id.ToString());
            iclbWithholdingInformationHistoryDetail = GetCollection<busWithholdingInformationHistoryDetail>(ldtbList, "icdoWithholdingInformationHistoryDetail");

        }

        #region Check Error On Add Button
        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;

            busPayeeAccount lbusPayeeAccount = aobj as busPayeeAccount;

            if(ahstParams.Count>0)
            {
                //if(Convert.ToString(ahstParams["icdoWithholdingInformation.withhold_flat_amount"]).IsNullOrEmpty()||Convert.ToString(ahstParams["icdoWithholdingInformation.withhold_flat_amount"])=="")               
                //{
                //    lobjError = AddError(6039, "");
                //    aarrErrors.Add(lobjError);
                //    return aarrErrors;
                //}
                ahstParams["icdoWithholdingInformation.withholding_percentage"] = ahstParams["withholding_percentage"];
                ahstParams["icdoWithholdingInformation.withholding_date_from"] = ahstParams["withholding_date_from"];

                if (Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_percentage"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_percentage"]) == "")
                {
                    lobjError = AddError(6029, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                if (Convert.ToDecimal(ahstParams["icdoWithholdingInformation.withholding_percentage"]) > 100)
                {
                    lobjError = AddError(1121, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                //rid 80243
                if (Convert.ToDecimal(ahstParams["icdoWithholdingInformation.withholding_percentage"]) <= 0)
                {
                    lobjError = AddError(6142, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                if (Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_date_from"]).IsNullOrEmpty() || Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_date_from"]) == "")
                {
                    lobjError = AddError(6040, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

                if (Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_date_from"]) != "" || Convert.ToString(ahstParams["icdoWithholdingInformation.withholding_date_from"]).IsNotNullOrEmpty())
                {
                    if (Convert.ToDateTime(ahstParams["icdoWithholdingInformation.withholding_date_from"]) < DateTime.Today)
                    {
                        lobjError = AddError(5112, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }
                }


                if (lbusPayeeAccount.iclbWithholdingInformation.Count>0)
                {

                    int iintCount = (from item in lbusPayeeAccount.iclbWithholdingInformation
                                     where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                     && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                     && Convert.ToDateTime(ahstParams["icdoWithholdingInformation.withholding_date_from"]) < item.icdoWithholdingInformation.withholding_date_from
                                     select item).Count();


                    if (iintCount > 0)
                    {
                        lobjError = AddError(6166, "");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;
                    }

                        int lintNotReleasecount = (from item in lbusPayeeAccount.iclbWithholdingInformation
                                     where item.icdoWithholdingInformation.withholding_date_to == DateTime.MinValue 
                                     && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                     //&& item.icdoWithholdingInformation.withholding_date_from != Convert.ToDateTime(ahstParams["icdoWithholdingInformation.withholding_date_from"])
                                     select item).Count();

                        if (lintNotReleasecount > 0)
                        {
                                lobjError = AddError(6207, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                        }


                        int lintCount = (from item in lbusPayeeAccount.iclbWithholdingInformation
                                         where item.icdoWithholdingInformation.withholding_date_to != DateTime.MinValue
                                         && item.icdoWithholdingInformation.withholding_date_from != DateTime.MinValue
                                         && item.icdoWithholdingInformation.withholding_date_from == Convert.ToDateTime(ahstParams["icdoWithholdingInformation.withholding_date_from"])
                                         select item).Count();

                        if (lintCount > 0)
                        {
                                lobjError = AddError(6109, "");
                                aarrErrors.Add(lobjError);
                                return aarrErrors;
                        }

                }
               

            }
            return aarrErrors;
        }
        #endregion


    }
}
