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
	/// Class MPIPHP.BusinessObjects.busYearEndDataExtractionHeader:
	/// Inherited from busYearEndDataExtractionHeaderGen, the class is used to customize the business object busYearEndDataExtractionHeaderGen.
	/// </summary>
	[Serializable]
	public class busYearEndDataExtractionHeader : busYearEndDataExtractionHeaderGen
	{
        public busYearEndDataExtractionHeader InsertValuesInDataExtractionHeader(int aintYear, string astrIsAnnualStatemnetGenerated)
        {
            if (icdoYearEndDataExtractionHeader == null)
            {
                icdoYearEndDataExtractionHeader = new cdoYearEndDataExtractionHeader();
            }

            icdoYearEndDataExtractionHeader.year = aintYear;
            icdoYearEndDataExtractionHeader.is_annual_statement_generated_flag = astrIsAnnualStatemnetGenerated;
            icdoYearEndDataExtractionHeader.Insert();

            return this;
        }

        public void DeleteHeaderRecords(int aintYear)
        {
            DataTable ldtbHeader = busBase.Select<cdoYearEndDataExtractionHeader>(
               new string[1] {enmYearEndDataExtractionHeader.year.ToString() },
               new object[1] { aintYear }, null, null);

            if (ldtbHeader.Rows.Count > 0)
            {
                DataTable ldtbBatchInfo = busBase.Select<cdoDataExtractionBatchInfo>(
                    new string[1] { enmDataExtractionBatchInfo.year_end_data_extraction_header_id.ToString() },
                    new object[1] { Convert.ToInt32(ldtbHeader.Rows[0][enmDataExtractionBatchInfo.year_end_data_extraction_header_id.ToString()]) }, 
                                    null, null);
                
                foreach (DataRow ldr in ldtbBatchInfo.Rows)
                {
                    DataTable ldtbHourInfo = busBase.Select<cdoDataExtractionBatchHourInfo>(
                        new string[1] { enmDataExtractionBatchHourInfo.data_extraction_batch_info_id.ToString() },
                        new object[1] { aintYear }, null, null);
                }               
            }
        }
	}
}
