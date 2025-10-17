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
	public class busJobScheduleDetailGen : busMPIPHPBase
    {
		public busJobScheduleDetailGen()
		{

		}

		private cdoJobScheduleDetail _icdoJobScheduleDetail;
		public cdoJobScheduleDetail icdoJobScheduleDetail
		{
			get
			{
				return _icdoJobScheduleDetail;
			}
			set
			{
				_icdoJobScheduleDetail = value;
			}
		}

		public bool FindJobScheduleDetail(int Aintjobscheduledetailid)
		{
			bool lblnResult = false;
			if (_icdoJobScheduleDetail == null)
			{
				_icdoJobScheduleDetail = new cdoJobScheduleDetail();
			}
			if (_icdoJobScheduleDetail.SelectRow(new object[1] { Aintjobscheduledetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
