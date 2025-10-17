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
    /// Class MPIPHP.BusinessObjects.busNotesGen:
    /// Inherited from busBase, used to create new business object for main table cdoNotes and its children table. 
    /// </summary>
	[Serializable]
	public class busNotesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busNotesGen
        /// </summary>
		public busNotesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busNotesGen.
        /// </summary>
		public cdoNotes icdoNotes { get; set; }




        /// <summary>
        /// MPIPHP.busNotesGen.FindNotes():
        /// Finds a particular record from cdoNotes with its primary key. 
        /// </summary>
        /// <param name="aintnoteid">A primary key value of type int of cdoNotes on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindNotes(int aintnoteid)
		{
			bool lblnResult = false;
			if (icdoNotes == null)
			{
				icdoNotes = new cdoNotes();
			}
			if (icdoNotes.SelectRow(new object[1] { aintnoteid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
