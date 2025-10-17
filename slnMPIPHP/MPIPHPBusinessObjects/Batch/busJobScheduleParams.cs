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
	public class busJobScheduleParams : busJobScheduleParamsGen
	{
        public busJobParameters ConvertToJobDetailParams()
        {
            busJobParameters lobjJobParameters = new busJobParameters();
            lobjJobParameters.icdoJobParameters = new cdoJobParameters();
            lobjJobParameters.icdoJobParameters.param_name = icdoJobScheduleParams.param_name;
            lobjJobParameters.icdoJobParameters.param_value = icdoJobScheduleParams.param_value;

            return lobjJobParameters;
        }
	}
}
