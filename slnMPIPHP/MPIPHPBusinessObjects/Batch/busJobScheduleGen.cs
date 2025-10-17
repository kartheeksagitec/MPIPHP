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
	[Serializable]
	public class busJobScheduleGen : busMPIPHPBase
    {
		public busJobScheduleGen()
		{

		}

		private cdoJobSchedule _icdoJobSchedule;
		public cdoJobSchedule icdoJobSchedule
		{
			get
			{
				return _icdoJobSchedule;
			}
			set
			{
				_icdoJobSchedule = value;
			}
		}

		public bool FindJobSchedule(int Aintjobscheduleid)
		{
			bool lblnResult = false;
			if (_icdoJobSchedule == null)
			{
				_icdoJobSchedule = new cdoJobSchedule();
			}
			if (_icdoJobSchedule.SelectRow(new object[1] { Aintjobscheduleid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
