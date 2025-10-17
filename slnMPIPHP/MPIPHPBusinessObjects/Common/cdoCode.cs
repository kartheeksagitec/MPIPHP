#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	[Serializable]
	public class cdoCode : doCode
	{
		public cdoCode() : base()
		{

		}
		public string data1_caption_display
		{
			get
			{
				if ((data1_caption == null) || (data1_caption == ""))
					return "Data1 : ";
				else
					return data1_caption + " : ";
			}
		}

		public string data2_caption_display
		{
			get
			{
				if ((data2_caption == null) || (data2_caption == ""))
					return "Data2 : ";
				else
					return data2_caption + " : ";
			}
		}

		public string data3_caption_display
		{
			get
			{
				if ((data3_caption == null) || (data3_caption == ""))
					return "Data3 : ";
				else
					return data3_caption + " : ";
			}
		}

		public override ArrayList ValidateDelete()
		{
			ArrayList larrErrors = null;
			int lintCount = (int) DBFunction.DBExecuteScalar("select count(*) from sgs_code_value",  
				iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
			if (lintCount > 0)
			{
				//Cannot delete code, please delete code values associated with it 
				AddError(larrErrors, 50);
			}
			return larrErrors;
		}
	}
} 
