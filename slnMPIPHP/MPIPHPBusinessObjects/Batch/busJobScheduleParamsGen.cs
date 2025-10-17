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
	public class busJobScheduleParamsGen : busMPIPHPBase
    {
		public busJobScheduleParamsGen()
		{

		}

		private cdoJobScheduleParams _icdoJobScheduleParams;
		public cdoJobScheduleParams icdoJobScheduleParams
		{
			get
			{
				return _icdoJobScheduleParams;
			}
			set
			{
				_icdoJobScheduleParams = value;
			}
		}

		public bool FindJobScheduleParams(int Aintjobscheduleparamsid)
		{
			bool lblnResult = false;
			if (_icdoJobScheduleParams == null)
			{
				_icdoJobScheduleParams = new cdoJobScheduleParams();
			}
			if (_icdoJobScheduleParams.SelectRow(new object[1] { Aintjobscheduleparamsid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
