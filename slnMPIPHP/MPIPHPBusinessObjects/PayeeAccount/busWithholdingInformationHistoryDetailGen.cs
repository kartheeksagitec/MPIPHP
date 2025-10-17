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
    /// Class MPIPHP.BusinessObjects.busWithholdingInformationHistoryDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWithholdingInformationHistoryDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWithholdingInformationHistoryDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busWithholdingInformationHistoryDetailGen
        /// </summary>
		public busWithholdingInformationHistoryDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWithholdingInformationHistoryDetailGen.
        /// </summary>
		public cdoWithholdingInformationHistoryDetail icdoWithholdingInformationHistoryDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busWithholdingInformation.
        /// </summary>
		public busWithholdingInformation ibusWithholdingInformation { get; set; }




        /// <summary>
        /// MPIPHP.busWithholdingInformationHistoryDetailGen.FindWithholdingInformationHistoryDetail():
        /// Finds a particular record from cdoWithholdingInformationHistoryDetail with its primary key. 
        /// </summary>
        /// <param name="aintwithholdinginformationhistorydetailid">A primary key value of type int of cdoWithholdingInformationHistoryDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWithholdingInformationHistoryDetail(int aintwithholdinginformationhistorydetailid)
		{
			bool lblnResult = false;
			if (icdoWithholdingInformationHistoryDetail == null)
			{
				icdoWithholdingInformationHistoryDetail = new cdoWithholdingInformationHistoryDetail();
			}
			if (icdoWithholdingInformationHistoryDetail.SelectRow(new object[1] { aintwithholdinginformationhistorydetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busWithholdingInformationHistoryDetailGen.LoadWithholdingInformation():
        /// Loads non-collection object ibusWithholdingInformation of type busWithholdingInformation.
        /// </summary>
		public virtual void LoadWithholdingInformation()
		{
			if (ibusWithholdingInformation == null)
			{
				ibusWithholdingInformation = new busWithholdingInformation();
			}
			ibusWithholdingInformation.FindWithholdingInformation(icdoWithholdingInformationHistoryDetail.withholding_information_id);
		}

	}
}
