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
    /// Class   MPIPHP.BusinessObjects.busPersonBaseGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonBase and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonBaseGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for   MPIPHP.BusinessObjects.busPersonBaseGen
        /// </summary>
		public busPersonBaseGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonBaseGen.
        /// </summary>
		public cdoPersonBase icdoPersonBase { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAddress.
        /// </summary>
		public busPersonAddress ibusPersonAddress { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPersonAddress. 
        /// </summary>
		public Collection<busPersonAddress> iclbPersonAddress { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPersonAccountBeneficiary. 
        /// </summary>
		public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPersonContact. 
        /// </summary>
		public Collection<busPersonContact> iclbPersonContact { get; set; }



        /// <summary>
        ///   MPIPHP.busPersonBaseGen.FindPersonBase():
        /// Finds a particular record from cdoPersonBase with its primary key. 
        /// </summary>
        /// <param name="aintPersonId">A primary key value of type int of cdoPersonBase on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonBase(int aintPersonId)
		{
			bool lblnResult = false;
			if (icdoPersonBase == null)
			{
				icdoPersonBase = new cdoPersonBase();
			}
			if (icdoPersonBase.SelectRow(new object[1] { aintPersonId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///   MPIPHP.busPersonBaseGen.LoadPersonAddress():
        /// Loads non-collection object ibusPersonAddress of type busPersonAddress.
        /// </summary>
		public virtual void LoadPersonAddress()
		{
			if (ibusPersonAddress == null)
			{
				ibusPersonAddress = new busPersonAddress();
			}
			ibusPersonAddress.FindPersonAddress(icdoPersonBase.person_id);
		}

        /// <summary>
        ///   MPIPHP.busPersonBaseGen.LoadPersonAddresss():
        /// Loads Collection object iclbPersonAddress of type busPersonAddress.
        /// </summary>
		public virtual void LoadPersonAddresss()
		{
			DataTable ldtbList = Select<cdoPersonAddress>(
				new string[1] { enmPersonAddress.person_id.ToString() },
				new object[1] { icdoPersonBase.person_id }, null, null);
			iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
		}

        /// <summary>
        ///   MPIPHP.busPersonBaseGen.LoadPersonAccountBeneficiarys():
        /// Loads Collection object iclbPersonAccountBeneficiary of type busPersonAccountBeneficiary.
        /// </summary>
        //public virtual void LoadPersonAccountBeneficiarys()
        //{
        //    DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
        //        new string[1] { enmPersonAccountBeneficiary.person_id.ToString() },
        //        new object[1] { icdoPersonBase.person_id }, null, null);
        //    iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");
        //}

        /// <summary>
        ///   MPIPHP.busPersonBaseGen.LoadPersonContacts():
        /// Loads Collection object iclbPersonContact of type busPersonContact.
        /// </summary>
		public virtual void LoadPersonContacts()
		{
			DataTable ldtbList = Select<cdoPersonContact>(
				new string[1] { enmPersonContact.person_id.ToString() },
				new object[1] { icdoPersonBase.person_id }, null, null);
			iclbPersonContact = GetCollection<busPersonContact>(ldtbList, "icdoPersonContact");
		}

	}
}
