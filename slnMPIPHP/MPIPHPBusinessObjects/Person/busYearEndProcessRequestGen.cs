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

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busYearEndProcessRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoYearEndProcessRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busYearEndProcessRequestGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busYearEndProcessRequestGen
        /// </summary>
		public busYearEndProcessRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busYearEndProcessRequestGen.
        /// </summary>
		public cdoYearEndProcessRequest icdoYearEndProcessRequest { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayment1099r. 
        /// </summary>
		public Collection<busPayment1099r> iclbPayment1099r { get; set; }



        /// <summary>
        /// MPIPHP.busYearEndProcessRequestGen.FindYearEndProcessRequest():
        /// Finds a particular record from cdoYearEndProcessRequest with its primary key. 
        /// </summary>
        /// <param name="aintyearendprocessrequestid">A primary key value of type int of cdoYearEndProcessRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindYearEndProcessRequest(int aintyearendprocessrequestid)
		{
			bool lblnResult = false;
			if (icdoYearEndProcessRequest == null)
			{
				icdoYearEndProcessRequest = new cdoYearEndProcessRequest();
			}
			if (icdoYearEndProcessRequest.SelectRow(new object[1] { aintyearendprocessrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busYearEndProcessRequestGen.LoadPayment1099rs():
        /// Loads Collection object iclbPayment1099r of type busPayment1099r.
        /// </summary>
		public virtual void LoadPayment1099rs()
		{
			DataTable ldtbList = Select<cdoPayment1099r>(
				new string[1] { enmPayment1099r.year_end_process_request_id.ToString() },
				new object[1] { icdoYearEndProcessRequest.year_end_process_request_id }, null, null);
			iclbPayment1099r = GetCollection<busPayment1099r>(ldtbList, "icdoPayment1099r");
		}

	}
}
