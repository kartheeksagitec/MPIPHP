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
	/// Class MPIPHP.BusinessObjects.busPersonBridgeHours:
	/// Inherited from busPersonBridgeHoursGen, the class is used to customize the business object busPersonBridgeHoursGen.
	/// </summary>
	[Serializable]
	public class busPersonBridgeHours : busPersonBridgeHoursGen
	{
        public Collection<busPersonBridgeHoursDetail> iclbPersonBridgeHoursDetails { get; set; }

        public void LoadPersonBridgeHoursDetails()
        {            
            this.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
            DataTable ldtbPersonBridgeHoursDetail = busPerson.Select("cdoPersonBridgeHoursDetail.GetPersonBridgeDetail", new object[1] { icdoPersonBridgeHours.person_bridge_id });
            this.iclbPersonBridgeHoursDetails = GetCollection<busPersonBridgeHoursDetail>(ldtbPersonBridgeHoursDetail, "icdoPersonBridgeHoursDetail");
        }

        public Collection<busPersonBridgeHours> LoadBridgeHoursByPersonID(int aintPersonId)
        {
            Collection<busPersonBridgeHours> lclbPersonBridgeHours = new Collection<busPersonBridgeHours>();
            DataTable ldtPersonBridgeHours = busBase.Select("cdoPersonBridgeHours.GetPersonBridgeHours", new object[1] { aintPersonId });
            lclbPersonBridgeHours = GetCollection<busPersonBridgeHours>(ldtPersonBridgeHours, "icdoPersonBridgeHours");
            return lclbPersonBridgeHours;
        }

        public DataTable LoadBridgeHoursDetailsByPersonId(int aintPersonId)
        {
           // Collection<busPersonBridgeHoursDetail> lclbPersonBridgeHoursDetail = new Collection<busPersonBridgeHoursDetail>();
            
            DataTable ldtbPersonBridgeHoursDetail =
                busPerson.Select("cdoPersonBridgeHoursDetail.LoadBridgedDetails", new object[1] { aintPersonId });
            //lclbPersonBridgeHoursDetail = GetCollection<busPersonBridgeHoursDetail>(ldtbPersonBridgeHoursDetail, "icdoPersonBridgeHoursDetail");

            return ldtbPersonBridgeHoursDetail;
        }

        public void InsertPersonBridgeHours(int aintPersonId,string astrBridgeType,decimal adecHoursReported,DateTime adtBridgeStartDate,
            DateTime adtBridgeEndDate)
        {
            icdoPersonBridgeHours = new cdoPersonBridgeHours();
            icdoPersonBridgeHours.person_id = aintPersonId;
            icdoPersonBridgeHours.bridge_type_id = busConstant.BRIDGE_TYPE_ID;
            icdoPersonBridgeHours.bridge_type_value = astrBridgeType;
            icdoPersonBridgeHours.hours_reported = adecHoursReported;
            icdoPersonBridgeHours.bridge_start_date = adtBridgeStartDate;
            icdoPersonBridgeHours.bridge_end_date = adtBridgeEndDate;
            icdoPersonBridgeHours.Insert();

        }
    }
}
