#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busUserActivityLogQueries:
	/// Inherited from busUserActivityLogQueriesGen, the class is used to customize the business object busUserActivityLogQueriesGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLogQueries : busUserActivityLogQueriesGen
	{
        public busUserActivityLogDetail ibusUserActivityLogDetail { get; set; }
	}
}
