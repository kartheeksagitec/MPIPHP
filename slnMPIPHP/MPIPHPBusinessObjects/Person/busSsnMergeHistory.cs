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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busSsnMergeHistory:
	/// Inherited from busSsnMergeHistoryGen, the class is used to customize the business object busSsnMergeHistoryGen.
	/// </summary>
	[Serializable]
	public class busSsnMergeHistory : busSsnMergeHistoryGen
	{

      public int InsertSSNMergeHistoryOfPerson(int aintOldPersonID, string astrOldSSN, string astrOldMPIPersonID, int aintNewPersonID, string astrNewSSN, string astrNewMPIPersonID)
        {
            icdoSsnMergeHistory = new cdoSsnMergeHistory();
            icdoSsnMergeHistory.old_person_id = aintOldPersonID;
            icdoSsnMergeHistory.old_ssn = astrOldSSN;
            icdoSsnMergeHistory.old_mpi_person_id = astrOldMPIPersonID;
            icdoSsnMergeHistory.new_person_id = aintNewPersonID;
            icdoSsnMergeHistory.new_ssn = astrNewSSN;
            icdoSsnMergeHistory.new_mpi_person_id = astrNewMPIPersonID;
            icdoSsnMergeHistory.merge_date = DateTime.Now;
            icdoSsnMergeHistory.merged_by = utlPassInfo.iobjPassInfo.istrUserID;
            icdoSsnMergeHistory.Insert();

            return icdoSsnMergeHistory.ssn_merge_history_id;

        }

     
      public busSsnMergeHistory LoadSNNMergeHistory(string astrSSN)
      {
          DataTable ldtbMergedSSNInfo = busBase.Select<cdoSsnMergeHistory>(
                  new string[1] { enmSsnMergeHistory.old_ssn.ToString() },
                  new object[1] { astrSSN }, null, null);


          busSsnMergeHistory lbusSsnMergeHistory = new busSsnMergeHistory { icdoSsnMergeHistory = new cdoSsnMergeHistory() };

          if (ldtbMergedSSNInfo.Rows.Count > 0)
          {
              lbusSsnMergeHistory.icdoSsnMergeHistory.LoadData(ldtbMergedSSNInfo.Rows[0]);
              return lbusSsnMergeHistory;
          }
          else
          {
              return null;
          }
      }
      public busSsnMergeHistory LoadSNNMergeHistoryByOldMPID(string astrOldMPID)
      {
          DataTable ldtbMergedSSNInfo = busBase.Select<cdoSsnMergeHistory>(
                new string[1] { enmSsnMergeHistory.old_mpi_person_id.ToString() },
                new object[1] { astrOldMPID }, null, null);

          busSsnMergeHistory lbusSsnMergeHistory = new busSsnMergeHistory { icdoSsnMergeHistory = new cdoSsnMergeHistory() };

          if (ldtbMergedSSNInfo.Rows.Count > 0)
          {
              lbusSsnMergeHistory.icdoSsnMergeHistory.LoadData(ldtbMergedSSNInfo.Rows[0]);
              return lbusSsnMergeHistory;
          }
          else
          {
              return null;
          }
      }
	}
}
