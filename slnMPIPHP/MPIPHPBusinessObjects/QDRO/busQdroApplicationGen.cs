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
    /// Class MPIPHP.BusinessObjects.busQdroApplicationGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroApplication and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroApplicationGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busQdroApplicationGen
        /// </summary>
		public busQdroApplicationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroApplicationGen.
        /// </summary>
        public cdoDroApplication icdoDroApplication { get; set; }

        /// <summary>
        /// MPIPHP.busQdroApplicationGen.FindQdroApplication():
        /// Finds a particular record from cdoQdroApplication with its primary key. 
        /// </summary>
        /// <param name="aintDroApplicationId">A primary key value of type int of cdoQdroApplication on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindQdroApplication(int aintDroApplicationId)
        {
            bool lblnResult = false;
            if (icdoDroApplication == null)
            {
                icdoDroApplication = new cdoDroApplication();
            }
            if (icdoDroApplication.SelectRow(new object[1] { aintDroApplicationId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

	}
}
