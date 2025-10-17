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
    /// Class MPIPHP.BusinessObjects.busWithholdingInformationGen:
    /// Inherited from busBase, used to create new business object for main table cdoWithholdingInformation and its children table. 
    /// </summary>
	[Serializable]
	public class busWithholdingInformationGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busWithholdingInformationGen
        /// </summary>
		public busWithholdingInformationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWithholdingInformationGen.
        /// </summary>
		public cdoWithholdingInformation icdoWithholdingInformation { get; set; }




        /// <summary>
        /// MPIPHP.busWithholdingInformationGen.FindWithholdingInformation():
        /// Finds a particular record from cdoWithholdingInformation with its primary key. 
        /// </summary>
        /// <param name="aintwithholdinginformationid">A primary key value of type int of cdoWithholdingInformation on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWithholdingInformation(int aintwithholdinginformationid)
		{
			bool lblnResult = false;
			if (icdoWithholdingInformation == null)
			{
				icdoWithholdingInformation = new cdoWithholdingInformation();
			}
			if (icdoWithholdingInformation.SelectRow(new object[1] { aintwithholdinginformationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
