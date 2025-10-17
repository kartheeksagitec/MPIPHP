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

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busPensionActuaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPensionActuary and its children table. 
    /// </summary>
	[Serializable]
	public class busPensionActuaryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPensionActuaryGen
        /// </summary>
		public busPensionActuaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPensionActuaryGen.
        /// </summary>
		public cdoPensionActuary icdoPensionActuary { get; set; }
       
        /// <summary>
        /// Gets or sets the non-collection object of type busPlan.
        /// </summary>
		public busPlan ibusPlanID { get; set; }




        /// <summary>
        /// MPIPHP.busPensionActuaryGen.FindPensionActuary():
        /// Finds a particular record from cdoPensionActuary with its primary key. 
        /// </summary>
        /// <param name="aintPensionActuaryId">A primary key value of type int of cdoPensionActuary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPensionActuary(int aintPensionActuaryId)
		{
			bool lblnResult = false;
			if (icdoPensionActuary == null)
			{
				icdoPensionActuary = new cdoPensionActuary();
			}
			if (icdoPensionActuary.SelectRow(new object[1] { aintPensionActuaryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
       


        public Collection<busPensionActuary> iclbPensionActuary { get; set; }

    }
}
