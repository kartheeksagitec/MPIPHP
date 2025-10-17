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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busYearEndProcessRequest:
	/// Inherited from busYearEndProcessRequestGen, the class is used to customize the business object busYearEndProcessRequestGen.
	/// </summary>
	[Serializable]
	public class busYearEndProcessRequest : busYearEndProcessRequestGen
	{
        #region Validate

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            string astrYearEndProcess = Convert.ToString(ahstParam["astr_year_end_process_value"]);

            if (astrYearEndProcess.IsNullOrEmpty())
            {
                utlError lobjError = null;
                lobjError = AddError(6110, "");
                larrErrors.Add(lobjError);

            }
            return larrErrors;
        }

        public override void ValidateHardErrors(Sagitec.Common.utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            DataTable ldtblYearEndProcessStatus = busBase.Select("cdoYearEndProcessRequest.GetYearEndProcessStatus", new object[3] { this.icdoYearEndProcessRequest.year_end_process_value, this.icdoYearEndProcessRequest.year,this.icdoYearEndProcessRequest.year_end_process_request_id });
            if (ldtblYearEndProcessStatus.Rows.Count > 0 && ldtblYearEndProcessStatus.IsNotNull())
            {
               
                    if (this.icdoYearEndProcessRequest.year_end_process_value != busConstant.CORRECTED_1099R)
                    {
                        lobjError = AddError(6111, "");
                        this.iarrErrors.Add(lobjError);
                    }
                    else
                        if (Convert.ToString(ldtblYearEndProcessStatus.Rows[0]["STATUS_VALUE"]) == busConstant.BatchRequest1099rStatusPending)
                        {
                            lobjError = AddError(6111, "");
                            this.iarrErrors.Add(lobjError);
                        }
                
            }
            if (this.icdoYearEndProcessRequest.year == 0)
            {
                lobjError = AddError(0, "Please Enter a year");
                this.iarrErrors.Add(lobjError);
            }
            if (this.icdoYearEndProcessRequest.year_end_process_value == busConstant.CORRECTED_1099R)
            {
                DataTable ldtblCount = busBase.Select("cdoYearEndProcessRequest.GetAnnual1099RCount", new object[2] { busConstant.ANNUAL_1099R, busConstant.YEAR_END_PROCESS_REQUEST_COMPLETED });
                if (ldtblCount.Rows.Count > 0 && (Convert.ToInt32(ldtblCount.Rows[0][0]) > 0))
                {
                    lobjError = AddError(6112, "");
                    this.iarrErrors.Add(lobjError);
                }

                if (this.icdoYearEndProcessRequest.status_date.Month > busConstant.OCTOBER)
                {
                    lobjError = AddError(6113, "");
                    this.iarrErrors.Add(lobjError);
                }
                else if (this.icdoYearEndProcessRequest.status_date.Month == busConstant.OCTOBER && this.icdoYearEndProcessRequest.status_date.Day >= 15)
                {
                    lobjError = AddError(6113, "");
                    this.iarrErrors.Add(lobjError);
                }
            }

            base.ValidateHardErrors(aenmPageMode);
        }
        #endregion

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();

            if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            {
                this.icdoYearEndProcessRequest.status_value = busConstant.YEAR_END_PROCESS_REQUEST_PENDING;
                this.icdoYearEndProcessRequest.status_date = DateTime.Now;
            }
        }

	}
}
