#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;

#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// partial class NeoSpin.BusinessObjects.busPersonCommunicationPreferences
    /// </summary>	
	public partial class busPersonCommunicationPreferences : busBase
	{
		
        /// <summary>
        /// Gets or sets the main-table object contained in busPersonCommunicationPreferences.
        /// </summary>
		public doPersonCommunicationPreferences icdoPersonCommunicationPreferences { get; set; }

		public virtual bool FindPersonCommunicationPreferences(int aiPerson_communication_preferences_id)
		{
			bool lblnResult = false;
			if (icdoPersonCommunicationPreferences == null)
			{
				icdoPersonCommunicationPreferences = new doPersonCommunicationPreferences();
			}
			if (icdoPersonCommunicationPreferences.SelectRow(new object[1] { aiPerson_communication_preferences_id }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
	
}
