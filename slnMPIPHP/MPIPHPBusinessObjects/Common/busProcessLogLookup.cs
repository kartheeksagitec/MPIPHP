#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busProcessLogLookup : busMainBase
	{
        public Collection<cdoProcessLog> icolProcessLog { set; get; }
      

		public void LoadProcessLog(DataTable adtbSearchResult)
		{
            icolProcessLog = new Collection<cdoProcessLog>();
            cdoProcessLog lobjProcessLog;
            foreach (DataRow ldtrData in adtbSearchResult.Rows)
            {
                lobjProcessLog = new cdoProcessLog();
                lobjProcessLog.LoadData(ldtrData);
                icolProcessLog.Add(lobjProcessLog);
            } 
		}

	}
}
