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
	/// Class MPIPHP.BusinessObjects.busPersonSuspendibleMonth:
	/// Inherited from busPersonSuspendibleMonthGen, the class is used to customize the business object busPersonSuspendibleMonthGen.
	/// </summary>
	[Serializable]
	public class busPersonSuspendibleMonth : busPersonSuspendibleMonthGen
	{
        public string istrPersonID { get; set; }
        public busPerson ibusPerson { get; set; }


        public void InsertSuspendibleMonths(int aintPersonID, int aintPlanYear, string astrSuspendibleMonth, string astrStatus)
        {
            icdoPersonSuspendibleMonth = new cdoPersonSuspendibleMonth();
            icdoPersonSuspendibleMonth.person_id = aintPersonID;
            icdoPersonSuspendibleMonth.plan_year = aintPlanYear;
            icdoPersonSuspendibleMonth.suspendible_month_id = busConstant.SUSPENDIBLE_MONTH_ID;
            icdoPersonSuspendibleMonth.suspendible_month_value = astrSuspendibleMonth;
            icdoPersonSuspendibleMonth.status_id = busConstant.SUSPENDIBLE_MONTH_STATUS_ID;
            icdoPersonSuspendibleMonth.status_value = astrStatus;
            icdoPersonSuspendibleMonth.Insert();

        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (icdoPersonSuspendibleMonth.suspendible_month_id <= 0)
            {
                this.icdoPersonSuspendibleMonth.status_id = busConstant.SUSPENDIBLE_MONTH_STATUS_ID;
                this.icdoPersonSuspendibleMonth.status_value = busConstant.SUSPENDIBLE_MONTH_STATUS_PENDING;
                this.icdoPersonSuspendibleMonth.status_description = busConstant.SUSPENDIBLE_MONTH_STATUS_PENDING_DESC;
                if (ibusPerson.icdoPerson.person_id != 0)
                {
                    this.icdoPersonSuspendibleMonth.person_id = ibusPerson.icdoPerson.person_id;
                }
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(utlPageMode.All);
            if (this.iarrErrors.Count==0)
            {
                if (string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.plan_year)) || this.icdoPersonSuspendibleMonth.plan_year == 0)
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(6078, " ");
                    this.iarrErrors.Add(lutlError);
                }
                if (string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.suspendible_month_value)))
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(6079, " ");
                    this.iarrErrors.Add(lutlError);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.suspendible_month_value)) && (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.plan_year)) && this.icdoPersonSuspendibleMonth.plan_year != 0))
                {
                    int lintCount = (int)DBFunction.DBExecuteScalar("cdoPersonSuspendibleMonth.GetCountSuspendibleMonth", new object[3] { this.icdoPersonSuspendibleMonth.suspendible_month_value , 
            this.icdoPersonSuspendibleMonth.plan_year, this.icdoPersonSuspendibleMonth.person_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lintCount > 0)
                    {
                        utlError lutlError = new utlError();
                        lutlError = AddError(6140, " ");
                        this.iarrErrors.Add(lutlError);
                    }


                }

                if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.plan_year)) && this.icdoPersonSuspendibleMonth.plan_year != 0)
                {
                    if (this.icdoPersonSuspendibleMonth.plan_year > DateTime.Now.Year)
                    {
                        utlError lutlError = new utlError();
                        lutlError = AddError(6143, " ");
                        this.iarrErrors.Add(lutlError);
                    }

                }

                if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.suspendible_month_value)) && (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonSuspendibleMonth.plan_year)) && this.icdoPersonSuspendibleMonth.plan_year != 0))
                {
                    if (this.icdoPersonSuspendibleMonth.plan_year == DateTime.Now.Year && Convert.ToInt32(this.icdoPersonSuspendibleMonth.suspendible_month_value) > DateTime.Now.Month)
                    {
                        utlError lutlError = new utlError();
                        lutlError = AddError(6144, " ");
                        this.iarrErrors.Add(lutlError);
                    }
                }
            }
          
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            DataTable ldtbList = Select("cdoDroApplication.GetPayeeAccountsOfPaticipant", new object[1] { this.icdoPersonSuspendibleMonth.person_id });
            if (ldtbList.Rows.Count > 0)
            {
                int lintPayeeAccountID = 0;
                foreach (DataRow ldr in ldtbList.Rows)
                {
                    lintPayeeAccountID = Convert.ToInt32(ldr["PAYEE_ACCOUNT_ID"]);
                    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_REEMPLOYMENT, this.icdoPersonSuspendibleMonth.person_id, 0, lintPayeeAccountID, new Hashtable());

                }
            }
          

        }
	}
}
