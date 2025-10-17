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
	/// Class MPIPHP.BusinessObjects.busPirLookup:
	/// Inherited from busPirLookupGen, this class is used to customize the lookup business object busPirLookupGen. 
	/// </summary>
	[Serializable]
	public class busPirLookup : busPirLookupGen
	{
        public Collection<busPir> iclbLookupResult {get;set;}

        public void LoadPirs(DataTable adtbSearchResult)
        {
            iclbLookupResult = GetCollection<busPir>(adtbSearchResult, "icdoPir");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        //This is fired for every datarow found in the search result. 
        //Handle is returned to this method include the datarow and busObject being created
        //
        {
            ((busPir)aobjBus).LoadAssignedTo();
            ((busPir)aobjBus).LoadReportedBy();
            //PIR Scout/Effort Hours Implementation
            ((busPir)aobjBus).LoadTotalPirEffortHours();
          //  ((busPir)aobjBus).LoadTestCase();
        }

        public void UpdatePirs(ArrayList aarrSelectedPirs, string astrStatusValue)
        {
            string lstrAdditionalNotes = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, astrStatusValue);
            lstrAdditionalNotes = lstrAdditionalNotes.Substring(lstrAdditionalNotes.IndexOf('.') + 1).Trim();
            if (aarrSelectedPirs.IsNotNull() && aarrSelectedPirs.Count > 0)
            {
                foreach (busPir lbusPIR in aarrSelectedPirs)
                {
                    if (lbusPIR.icdoPir.status_value != astrStatusValue)
                    {
                        // Update the Status
                        lbusPIR.icdoPir.status_value = astrStatusValue;
                        lbusPIR.icdoPir.additional_notes = lstrAdditionalNotes;
                        lbusPIR.icdoPir.Update();

                        // Send Mail
                      //  lobjPIR.SendMail();
                    }
                }
            }
        }
	}
}
