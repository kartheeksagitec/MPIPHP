#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busProcessGen:
    /// Inherited from busBase, used to create new business object for main table cdoProcess and its children table. 
    /// </summary>
	[Serializable]
	public class busProcessGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busProcessGen
        /// </summary>
		public busProcessGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProcessGen.
        /// </summary>
		public cdoProcess icdoProcess { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busDocumentProcessCrossref. 
        /// </summary>
		public Collection<busDocumentProcessCrossref> iclbDocumentProcessCrossref { get; set; }



        /// <summary>
        ///  MPIPHP.busProcessGen.FindProcess():
        /// Finds a particular record from cdoProcess with its primary key. 
        /// </summary>
        /// <param name="aintprocessid">A primary key value of type int of cdoProcess on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProcess(int aintprocessid)
		{
			bool lblnResult = false;
			if (icdoProcess == null)
			{
				icdoProcess = new cdoProcess();
			}
			if (icdoProcess.SelectRow(new object[1] { aintprocessid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busProcessGen.LoadDocumentProcessCrossrefs():
        /// Loads Collection object iclbDocumentProcessCrossref of type busDocumentProcessCrossref.
        /// </summary>
		public virtual void LoadDocumentProcessCrossrefs()
		{
			DataTable ldtbList = Select<cdoDocumentProcessCrossref>(
				new string[1] { enmDocumentProcessCrossref.process_id.ToString() },
				new object[1] { icdoProcess.process_id }, null, null);
			iclbDocumentProcessCrossref = GetCollection<busDocumentProcessCrossref>(ldtbList, "icdoDocumentProcessCrossref");
		}

	}
}
