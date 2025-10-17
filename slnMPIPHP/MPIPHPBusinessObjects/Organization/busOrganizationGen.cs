#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using    MPIPHP.CustomDataObjects;
using    MPIPHP.DataObjects;

#endregion

namespace    MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class    MPIPHP.BusinessObjects.busOrganizationGen:
    /// Inherited from busBase, used to create new business object for main table cdoOrganization and its children table. 
    /// </summary>
	[Serializable]
	public class busOrganizationGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busOrganizationGen
        /// </summary>
		public busOrganizationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busOrganizationGen.
        /// </summary>
		public cdoOrganization icdoOrganization { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busOrgAddress. 
        /// </summary>
		public Collection<busOrgAddress> iclbOrgAddress { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busOrgBank. 
        /// </summary>
		public Collection<busOrgBank> iclbOrgBank { get; set; }



        /// <summary>
        ///    MPIPHP.busOrganizationGen.FindOrganization():
        /// Finds a particular record from cdoOrganization with its primary key. 
        /// </summary>
        /// <param name="aintOrgId">A primary key value of type int of cdoOrganization on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindOrganization(int aintOrgId)
		{
			bool lblnResult = false;
			if (icdoOrganization == null)
			{
				icdoOrganization = new cdoOrganization();
			}
			if (icdoOrganization.SelectRow(new object[1] { aintOrgId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busOrganizationGen.LoadOrgAddresss():
        /// Loads Collection object iclbOrgAddress of type busOrgAddress.
        /// </summary>
		public virtual void LoadOrgAddresss()
		{
			DataTable ldtbList = Select<cdoOrgAddress>(
				new string[1] { enmOrgAddress.org_id .ToString() },
				new object[1] { icdoOrganization.org_id }, null, null);
			iclbOrgAddress = GetCollection<busOrgAddress>(ldtbList, "icdoOrgAddress");
		}

        /// <summary>
        ///    MPIPHP.busOrganizationGen.LoadOrgBanks():
        /// Loads Collection object iclbOrgBank of type busOrgBank.
        /// </summary>
		public virtual void LoadOrgBanks()
		{
			DataTable ldtbList = Select<cdoOrgBank>(
				new string[1] { enmOrgBank.org_id.ToString() },
				new object[1] { icdoOrganization.org_id }, null, null);
			iclbOrgBank = GetCollection<busOrgBank>(ldtbList, "icdoOrgBank");
		}

	}
}
