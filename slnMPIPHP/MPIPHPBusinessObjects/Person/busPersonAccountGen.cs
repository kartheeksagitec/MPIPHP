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
    /// Class MPIPHP.BusinessObjects.busPersonAccountGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccount and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonAccountGen
        /// </summary>
		public busPersonAccountGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountGen.
        /// </summary>
		public cdoPersonAccount icdoPersonAccount { get; set; }


        public busPerson ibusPerson { get; set; }

        /// <summary>
        /// MPIPHP.busPersonAccountGen.FindPersonAccount():
        /// Finds a particular record from cdoPersonAccount with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountid">A primary key value of type int of cdoPersonAccount on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccount(int aintpersonaccountid)
		{
			bool lblnResult = false;
			if (icdoPersonAccount == null)
			{
				icdoPersonAccount = new cdoPersonAccount();
			}
			if (icdoPersonAccount.SelectRow(new object[1] { aintpersonaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public bool IsDateSignedApplicationDateMoreThanTodaysDate()
        {
            bool lblnResult = false;
            if (icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue && icdoPersonAccount.signed_application_forn_received_date > DateTime.Now)
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public bool IsQDROReviewDateMoreThanTodaysDate()
        {
            bool lblnResult = false;
            if (icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue && icdoPersonAccount.qdro_review_completed_date > DateTime.Now)
            {
                lblnResult = true;
            }
            return lblnResult;
        }

    }
}
