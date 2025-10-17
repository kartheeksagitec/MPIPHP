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
	public class busJobParametersGen : busMPIPHPBase
    {
		public busJobParametersGen()
		{

		}

		private cdoJobParameters _icdoJobParameters;
		public cdoJobParameters icdoJobParameters
		{
			get
			{
				return _icdoJobParameters;
			}
			set
			{
				_icdoJobParameters = value;
			}
		}

		public bool FindJobParameters(int Aintjobparametersid)
		{
			bool lblnResult = false;
			if (_icdoJobParameters == null)
			{
				_icdoJobParameters = new cdoJobParameters();
			}
			if (_icdoJobParameters.SelectRow(new object[1] { Aintjobparametersid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
