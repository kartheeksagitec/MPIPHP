#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using       MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.Bpm;

#endregion

namespace       MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class       MPIPHP.BusinessObjects.busPersonGen:
    /// Inherited from busBase, used to create new business object for main table cdoPerson and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for       MPIPHP.BusinessObjects.busPersonGen
        /// </summary>
		public busPersonGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonGen.
        /// </summary>
		public cdoPerson icdoPerson { get; set; }

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
        ///       MPIPHP.busPersonGen.FindPerson():
        /// Finds a particular record from cdoPerson with its primary key. 
        /// </summary>
        /// <param name="aintpersonid">A primary key value of type int of cdoPerson on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPerson(int aintpersonid)
		{
			bool lblnResult = false;
			if (icdoPerson == null)
			{
				icdoPerson = new cdoPerson();
			}
			if (icdoPerson.SelectRow(new object[1] { aintpersonid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///       MPIPHP.busPersonGen.LoadPersonAddress():
        /// Loads non-collection object ibusPersonAddress of type busPersonAddress.
        /// </summary>
		public virtual void LoadPersonAddress()
		{
			if (ibusPersonAddress == null)
			{
				ibusPersonAddress = new busPersonAddress();
			}
			ibusPersonAddress.FindPersonAddress(icdoPerson.person_id);
		}

        /// <summary>
        ///       MPIPHP.busPersonGen.LoadPersonAddresss():
        /// Loads Collection object iclbPersonAddress of type busPersonAddress.
        /// </summary>
		public virtual void LoadPersonAddresss()
		{
			DataTable ldtbList = Select<cdoPersonAddress>(
				new string[1] { enmPersonAddress.person_id.ToString() },
				new object[1] { icdoPerson.person_id }, null, null);
			iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
		}

        /// <summary>
        ///       MPIPHP.busPersonGen.LoadPersonContacts():
        /// Loads Collection object iclbPersonContact of type busPersonContact.
        /// </summary>
		public virtual void LoadPersonContacts()
		{
			DataTable ldtbList = Select<cdoPersonContact>(
				new string[1] { enmPersonContact.person_id.ToString() },
				new object[1] { icdoPerson.person_id }, null,null);
			iclbPersonContact = GetCollection<busPersonContact>(ldtbList, "icdoPersonContact");
		}

        public bool IsSubmitButtonVisible()
        {
            if (ibusBaseActivityInstance != null)
            {
                return true;
            }
            return false;
        }

	}
}
