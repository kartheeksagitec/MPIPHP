#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using   MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace   MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class   MPIPHP.BusinessObjects.busPersonAddressGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAddress and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAddressGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for   MPIPHP.BusinessObjects.busPersonAddressGen
        /// </summary>
		public busPersonAddressGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAddressGen.
        /// </summary>
		public cdoPersonAddress icdoPersonAddress { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPersonAddressHistory. 
        /// </summary>
		public Collection<busPersonAddressHistory> iclbPersonAddressHistory { get; set; }


        /// <summary>
        /// Gets or sets the Sagitec.Common.utlCollection object of type cdoPersonAddressChklist. 
        /// </summary>
		public utlCollection<cdoPersonAddressChklist> iclcPersonAddressChklist { get; set; }

        /// <summary>
        ///   MPIPHP.busPersonAddressGen.FindPersonAddress():
        /// Finds a particular record from cdoPersonAddress with its primary key. 
        /// </summary>
        /// <param name="aintaddressid">A primary key value of type int of cdoPersonAddress on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAddress(int aintaddressid)
		{
			bool lblnResult = false;
			if (icdoPersonAddress == null)
			{
				icdoPersonAddress = new cdoPersonAddress();
			}
			if (icdoPersonAddress.SelectRow(new object[1] { aintaddressid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///   MPIPHP.busPersonAddressGen.LoadPersonAddressHistorys():
        /// Loads Collection object iclbPersonAddressHistory of type busPersonAddressHistory.
        /// </summary>
		public virtual void LoadPersonAddressHistorys()
		{
			DataTable ldtbList = Select<cdoPersonAddressHistory>(
				new string[1] { enmPersonAddressHistory.address_id.ToString() },
				new object[1] { icdoPersonAddress.address_id }, null, null);
			iclbPersonAddressHistory = GetCollection<busPersonAddressHistory>(ldtbList, "icdoPersonAddressHistory");
		}

        /// <summary>
        ///   MPIPHP.busPersonAddressGen.LoadPersonAddressChklists():
        /// Loads Sagitec.Common.utlCollection object iclcPersonAddressChklist of type cdoPersonAddressChklist.
        /// </summary>
		public virtual void LoadPersonAddressChklists()
		{
			iclcPersonAddressChklist = GetCollection<cdoPersonAddressChklist>(
				new string[1] { enmPersonAddressChklist.address_id.ToString() }, 
				new object[1] { icdoPersonAddress.address_id }, null, null);
		}

	}
}
