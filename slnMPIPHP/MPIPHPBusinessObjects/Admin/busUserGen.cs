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
    /// Class  MPIPHP.BusinessObjects.busUserGen:
    /// Inherited from busBase, used to create new business object for main table cdoUser and its children table. 
    /// </summary>
	[Serializable]
	public class busUserGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busUserGen
        /// </summary>
		public busUserGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserGen.
        /// </summary>
		public cdoUser icdoUser { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busCustomAccessPolicy. 
        /// </summary>
		public Collection<busCustomAccessPolicy> iclbCustomAccessPolicy { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busCustomSecurity. 
        /// </summary>
		public Collection<busCustomSecurity> iclbCustomSecurity { get; set; }



        /// <summary>
        ///  MPIPHP.busUserGen.FindUser():
        /// Finds a particular record from cdoUser with its primary key. 
        /// </summary>
        /// <param name="aintuserserialid">A primary key value of type int of cdoUser on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUser(int aintuserserialid)
		{
			bool lblnResult = false;
			if (icdoUser == null)
			{
				icdoUser = new cdoUser();
			}
			if (icdoUser.SelectRow(new object[1] { aintuserserialid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busUserGen.LoadCustomAccessPolicys():
        /// Loads Collection object iclbCustomAccessPolicy of type busCustomAccessPolicy.
        /// </summary>
		public virtual void LoadCustomAccessPolicys()
		{
			DataTable ldtbList = Select<cdoCustomAccessPolicy>(
				new string[1] { enmCustomAccessPolicy.user_serial_id.ToString() },
				new object[1] { icdoUser.user_serial_id }, null, null);
			iclbCustomAccessPolicy = GetCollection<busCustomAccessPolicy>(ldtbList, "icdoCustomAccessPolicy");
		}

        /// <summary>
        ///  MPIPHP.busUserGen.LoadCustomSecuritys():
        /// Loads Collection object iclbCustomSecurity of type busCustomSecurity.
        /// </summary>
		public virtual void LoadCustomSecuritys()
		{
			DataTable ldtbList = Select<cdoCustomSecurity>(
				new string[1] { enmCustomSecurity.user_serial_id.ToString() },
				new object[1] { icdoUser.user_serial_id }, null, null);
			iclbCustomSecurity = GetCollection<busCustomSecurity>(ldtbList, "icdoCustomSecurity");
		}

	}
}
