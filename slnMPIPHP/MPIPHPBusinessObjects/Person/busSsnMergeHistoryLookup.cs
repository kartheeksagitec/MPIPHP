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
	/// Class MPIPHP.BusinessObjects.busSsnMergeHistoryLookup:
	/// Inherited from busSsnMergeHistoryLookupGen, this class is used to customize the lookup business object busSsnMergeHistoryLookupGen. 
	/// </summary>
	[Serializable]
	public class busSsnMergeHistoryLookup : busSsnMergeHistoryLookupGen
	{

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            busSsnMergeHistory lbusSsnMergeHistory = (busSsnMergeHistory) aobjBus;
            if (adtrRow["NEW_SSN"] != DBNull.Value)
                lbusSsnMergeHistory.icdoSsnMergeHistory.new_ssn = Convert.ToString(adtrRow["NEW_SSN"]);
            
            if (adtrRow["OLD_SSN"] != DBNull.Value)
                lbusSsnMergeHistory.icdoSsnMergeHistory.old_ssn = Convert.ToString(adtrRow["OLD_SSN"]);            
        }
	}
}
