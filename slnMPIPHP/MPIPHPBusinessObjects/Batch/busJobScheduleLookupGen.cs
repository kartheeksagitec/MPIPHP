#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busJobScheduleLookupGen : busMainBase
	{

		private Collection<busJobSchedule> _iclbJobSchedule;
		public Collection<busJobSchedule> iclbJobSchedule
		{
			get
			{
				return _iclbJobSchedule;
			}
			set
			{
				_iclbJobSchedule = value;
			}
		}

		public void LoadJobSchedules(DataTable adtbSearchResult)
		{
			_iclbJobSchedule = GetCollection<busJobSchedule>(adtbSearchResult, "icdoJobSchedule");
		}
	}
}
