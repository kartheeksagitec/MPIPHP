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
	/// Class MPIPHP.BusinessObjects.busPensionActuaryLookup:
	/// Inherited from busPensionActuaryLookupGen, this class is used to customize the lookup business object busPensionActuaryLookupGen. 
	/// </summary>
	[Serializable]
	public class busPensionActuaryLookup : busPensionActuaryLookupGen
	{

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            //busDataExtractionBatchInfo lbusDataExtractionBatchInfo = (busDataExtractionBatchInfo)aobjBus;
            //if (lbusDataExtractionBatchInfo.icdoDataExtractionBatchInfo.person_ssn != null)
            //{
            //    lbusDataExtractionBatchInfo.istrLast4DigitsofSSN = Sagitec.Common.HelperFunction.SagitecDecryptAES(lbusDataExtractionBatchInfo.icdoDataExtractionBatchInfo.person_ssn);
            //    if(lbusDataExtractionBatchInfo.istrLast4DigitsofSSN.Length >  4)
            //        lbusDataExtractionBatchInfo.istrLast4DigitsofSSN = lbusDataExtractionBatchInfo.istrLast4DigitsofSSN.Remove(0, 5);
            //}
        }


        public DataTable ExportInExcel(string astrFinalQuery)
        {
            string lstrFinalQuery = astrFinalQuery ; // "select * from SGT_DATA_EXTRACTION_BATCH_INFO";

            DataTable ldtPensionActuary = DBFunction.DBSelect(lstrFinalQuery, iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
           
            return ldtPensionActuary;
        }
	}
}
