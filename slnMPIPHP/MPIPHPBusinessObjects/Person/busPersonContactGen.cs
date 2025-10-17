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

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busPersonContactGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonContact and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonContactGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busPersonContactGen
        /// </summary>
		public busPersonContactGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonContactGen.
        /// </summary>
		public cdoPersonContact icdoPersonContact { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAddress.
        /// </summary>




        /// <summary>
        ///  MPIPHP.busPersonContactGen.FindPersonContact():
        /// Finds a particular record from cdoPersonContact with its primary key. 
        /// </summary>
        /// <param name="aintpersoncontactid">A primary key value of type int of cdoPersonContact on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonContact(int aintpersoncontactid)
		{
			bool lblnResult = false;
			if (icdoPersonContact == null)
			{
				icdoPersonContact = new cdoPersonContact();
			}
			if (icdoPersonContact.SelectRow(new object[1] { aintpersoncontactid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busPersonContactGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoPersonContact.person_id);
		}


	}
}
