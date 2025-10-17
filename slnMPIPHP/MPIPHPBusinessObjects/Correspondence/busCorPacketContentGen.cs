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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busCorPacketContentGen:
    /// Inherited from busBase, used to create new business object for main table cdoCorPacketContent and its children table. 
    /// </summary>
	[Serializable]
	public class busCorPacketContentGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busCorPacketContentGen
        /// </summary>
		public busCorPacketContentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCorPacketContentGen.
        /// </summary>
		public cdoCorPacketContent icdoCorPacketContent { get; set; }




        /// <summary>
        /// MPIPHP.busCorPacketContentGen.FindCorPacketContent():
        /// Finds a particular record from cdoCorPacketContent with its primary key. 
        /// </summary>
        /// <param name="a">A primary key value of type  of cdoCorPacketContent on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCorPacketContent(int a)
		{
			bool lblnResult = false;
			if (icdoCorPacketContent == null)
			{
				icdoCorPacketContent = new cdoCorPacketContent();
			}
			if (icdoCorPacketContent.SelectRow(new object[1] { a }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
