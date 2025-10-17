#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using      MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace      MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class      MPIPHP.BusinessObjects.busQdroCalculationHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroCalculationHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroCalculationHeaderGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for      MPIPHP.BusinessObjects.busQdroCalculationHeaderGen
        /// </summary>
		public busQdroCalculationHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroCalculationHeaderGen.
        /// </summary>
		public cdoQdroCalculationHeader icdoQdroCalculationHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busQdroApplication.
        /// </summary>
		public busQdroApplication ibusQdroApplication { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busQdroCalculationDetail. 
        /// </summary>
		public Collection<busQdroCalculationDetail> iclbQdroCalculationDetail { get; set; }


        /// <summary>
        ///      MPIPHP.busQdroCalculationHeaderGen.FindQdroCalculationHeader():
        /// Finds a particular record from cdoQdroCalculationHeader with its primary key. 
        /// </summary>
        /// <param name="aintQdroCalculationHeaderId">A primary key value of type int of cdoQdroCalculationHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindQdroCalculationHeader(int aintQdroCalculationHeaderId)
		{
			bool lblnResult = false;
			if (icdoQdroCalculationHeader == null)
			{
				icdoQdroCalculationHeader = new cdoQdroCalculationHeader();
			}
			if (icdoQdroCalculationHeader.SelectRow(new object[1] { aintQdroCalculationHeaderId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///      MPIPHP.busQdroCalculationHeaderGen.LoadQdroApplication():
        /// Loads non-collection object ibusQdroApplication of type busQdroApplication.
        /// </summary>
		public virtual void LoadQdroApplication()
		{
			if (ibusQdroApplication == null)
			{
				ibusQdroApplication = new busQdroApplication();
			}
			ibusQdroApplication.FindQdroApplication(icdoQdroCalculationHeader.qdro_application_id);
		}
    }
}
