#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busDeathNotificationLookup:
	/// Inherited from busDeathNotificationLookupGen, this class is used to customize the lookup business object busDeathNotificationLookupGen. 
	/// </summary>
	[Serializable]
	public class busDeathNotificationLookup : busDeathNotificationLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);

            //In the Following code I cannot use LoadData() method since I have 2 cdoPerson in the same DataRow
            //The method will not know what to bind with that
            //Hence we are getting only the necessary information in the LookUp Query
            busDeathNotification lbusDeathNotification = (busDeathNotification)abusBase;
            lbusDeathNotification.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };

            lbusDeathNotification.ibusPerson.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
            lbusDeathNotification.ibusPerson.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
            lbusDeathNotification.ibusPerson.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
            lbusDeathNotification.ibusPerson.icdoPerson.mpi_person_id = adtrRow[busConstant.MPI_ID].ToString();
            lbusDeathNotification.ibusPerson.icdoPerson.ssn = adtrRow[enmPerson.ssn.ToString()].ToString();
            lbusDeathNotification.ibusPerson.icdoPerson.istrSSNNonEncrypted = lbusDeathNotification.ibusPerson.icdoPerson.ssn;
        }
	}
}
