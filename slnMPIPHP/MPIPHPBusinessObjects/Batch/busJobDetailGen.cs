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
	public class busJobDetailGen : busMPIPHPBase
    {
		public busJobDetailGen()
		{

		}

		private cdoJobDetail _icdoJobDetail;
		public cdoJobDetail icdoJobDetail
		{
			get
			{
				return _icdoJobDetail;
			}
			set
			{
				_icdoJobDetail = value;
			}
		}

		public bool FindJobDetail(int Aintjobdetailid)
		{
			bool lblnResult = false;
			if (_icdoJobDetail == null)
			{
				_icdoJobDetail = new cdoJobDetail();
			}
			if (_icdoJobDetail.SelectRow(new object[1] { Aintjobdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
