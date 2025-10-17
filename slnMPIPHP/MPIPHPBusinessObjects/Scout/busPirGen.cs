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
    /// Class MPIPHP.BusinessObjects.busPirGen:
    /// Inherited from busBase, used to create new business object for main table cdoPir and its children table. 
    /// </summary>
	[Serializable]
	public class busPirGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPirGen
        /// </summary>
		public busPirGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPirGen.
        /// </summary>
		public cdoPir icdoPir { get; set; }




        /// <summary>
        /// MPIPHP.busPirGen.FindPir():
        /// Finds a particular record from cdoPir with its primary key. 
        /// </summary>
        /// <param name="aintpirid">A primary key value of type int of cdoPir on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPir(int aintpirid)
		{
			bool lblnResult = false;
			if (icdoPir == null)
			{
				icdoPir = new cdoPir();
			}
			if (icdoPir.SelectRow(new object[1] { aintpirid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
