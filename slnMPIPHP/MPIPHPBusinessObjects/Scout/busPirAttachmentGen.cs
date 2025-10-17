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
    /// Class MPIPHP.BusinessObjects.busPirAttachmentGen:
    /// Inherited from busBase, used to create new business object for main table cdoPirAttachment and its children table. 
    /// </summary>
	[Serializable]
	public class busPirAttachmentGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPirAttachmentGen
        /// </summary>
		public busPirAttachmentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPirAttachmentGen.
        /// </summary>
		public cdoPirAttachment icdoPirAttachment { get; set; }




        /// <summary>
        /// MPIPHP.busPirAttachmentGen.FindPirAttachment():
        /// Finds a particular record from cdoPirAttachment with its primary key. 
        /// </summary>
        /// <param name="aintpirattachmentid">A primary key value of type int of cdoPirAttachment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPirAttachment(int aintpirattachmentid)
		{
			bool lblnResult = false;
			if (icdoPirAttachment == null)
			{
				icdoPirAttachment = new cdoPirAttachment();
			}
			if (icdoPirAttachment.SelectRow(new object[1] { aintpirattachmentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
