#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busBatchSchedule : busMPIPHPBase
    {
        public busBatchSchedule()
		{
		}

        public cdoBatchSchedule icdoBatchSchedule { set; get; }

        
        public bool FindBatchSchedule(int aintBatchScheduleId)
		{
            bool lblnResult = false;
            if (icdoBatchSchedule == null)
			{
                icdoBatchSchedule = new cdoBatchSchedule();
			}
            if (icdoBatchSchedule.SelectRow(new object[1] { aintBatchScheduleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
