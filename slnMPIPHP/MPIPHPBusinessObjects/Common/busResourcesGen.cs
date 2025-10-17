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
    /// Class MPIPHP.BusinessObjects.busResourcesGen:
    /// Inherited from busBase, used to create new business object for main table cdoResources and its children table. 
    /// </summary>
	[Serializable]
	public class busResourcesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busResourcesGen
        /// </summary>
		public busResourcesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busResourcesGen.
        /// </summary>
		public cdoResources icdoResources { get; set; }

     


        /// <summary>
        /// MPIPHP.busResourcesGen.FindResources():
        /// Finds a particular record from cdoResources with its primary key. 
        /// </summary>
        /// <param name="aintresourceid">A primary key value of type int of cdoResources on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindResources(int aintresourceid)
		{
			bool lblnResult = false;
			if (icdoResources == null)
			{
				icdoResources = new cdoResources();
			}
			if (icdoResources.SelectRow(new object[1] { aintresourceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
