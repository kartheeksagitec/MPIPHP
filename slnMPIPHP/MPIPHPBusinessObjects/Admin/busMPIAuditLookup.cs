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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busMPIAuditLookup:
	/// Inherited from busMPIAuditLookupGen, this class is used to customize the lookup business object busMPIAuditLookupGen. 
	/// </summary>
	[Serializable]
    public class busMPIAuditLookup : busMainBase
    {
        

        public Collection<busMPIAudit> iclbLookupResult { get; set; }

        public void LoadSearchResult(DataTable adtbSearchResult)
        {
            iclbLookupResult = GetCollection<busMPIAudit>(adtbSearchResult, "icdoAuditLog");
        }

        protected override void LoadOtherObjects(System.Data.DataRow adtrRow, busBase aobjBus)
        {
            busMPIAudit lobjPERSAudit = (busMPIAudit)aobjBus;
            lobjPERSAudit.ibusPerson = new busPerson();
            lobjPERSAudit.ibusPerson.icdoPerson = new cdoPerson();
            lobjPERSAudit.ibusPerson.icdoPerson.LoadData(adtrRow);
        }
    }
}
