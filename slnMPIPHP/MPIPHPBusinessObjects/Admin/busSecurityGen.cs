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
    /// Class MPIPHP.BusinessObjects.busSecurityGen:
    /// Inherited from busBase, used to create new business object for main table cdoSecurity and its children table. 
    /// </summary>
	[Serializable]
	public class busSecurityGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busSecurityGen
        /// </summary>
		public busSecurityGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busSecurityGen.
        /// </summary>
		public cdoSecurity icdoSecurity { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busCustomSecurity. 
        /// </summary>
		public Collection<busCustomSecurity> iclbCustomSecurity { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busResources. 
        /// </summary>
		public Collection<busResources> iclbResources { get; set; }



        /// <summary>
        /// MPIPHP.busSecurityGen.FindSecurity():
        /// Finds a particular record from cdoSecurity with its primary key. 
        /// </summary>
        /// <param name="aintresourceid">A primary key value of type int of cdoSecurity on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindSecurity(int aintresourceid)
		{
			bool lblnResult = false;
			if (icdoSecurity == null)
			{
				icdoSecurity = new cdoSecurity();
			}
			if (icdoSecurity.SelectRow(new object[1] { aintresourceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busSecurityGen.LoadCustomSecuritys():
        /// Loads Collection object iclbCustomSecurity of type busCustomSecurity.
        /// </summary>
		public virtual void LoadCustomSecuritys()
		{
            //DataTable ldtbList = Select<cdoCustomSecurity>(
            //    new string[1] { enmCustomSecurity.resource_id.ToString() },
            //    new object[1] { icdoSecurity.resource_id }, null, null);
            //iclbCustomSecurity = GetCollection<busCustomSecurity>(ldtbList, "icdoCustomSecurity");
		}

        /// <summary>
        /// MPIPHP.busSecurityGen.LoadResourcess():
        /// Loads Collection object iclbResources of type busResources.
        /// </summary>
		public virtual void LoadResourcess()
		{
            //DataTable ldtbList = Select<cdoResources>(
            //    new string[1] { enmResources.resource_id.ToString() },
            //    new object[1] { icdoSecurity.resource_id }, null, null);
            //iclbResources = GetCollection<busResources>(ldtbList, "icdoResources");
		}

	}
}
