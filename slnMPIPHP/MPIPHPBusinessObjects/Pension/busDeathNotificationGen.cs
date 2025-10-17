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
    /// Class MPIPHP.BusinessObjects.busDeathNotificationGen:
    /// Inherited from busBase, used to create new business object for main table cdoDeathNotification and its children table. 
    /// </summary>
	[Serializable]
	public class busDeathNotificationGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDeathNotificationGen
        /// </summary>
		public busDeathNotificationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDeathNotificationGen.
        /// </summary>
		public cdoDeathNotification icdoDeathNotification { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
        public busPerson ibusPerson { get; set; }

        /// <summary>
        /// MPIPHP.busDeathNotificationGen.FindDeathNotification():
        /// Finds a particular record from cdoDeathNotification with its primary key. 
        /// </summary>
        /// <param name="aintdeathnotificationid">A primary key value of type int of cdoDeathNotification on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDeathNotification(int aintdeathnotificationid)
		{
			bool lblnResult = false;
			if (icdoDeathNotification == null)
			{
				icdoDeathNotification = new cdoDeathNotification();
			}
			if (icdoDeathNotification.SelectRow(new object[1] { aintdeathnotificationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}


        /// <summary>
        ///    MPIPHP.busBenefitApplicationGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
        public virtual void LoadPerson()
        {
            if (ibusPerson == null)
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson(icdoDeathNotification.person_id);
        }

	}
}
