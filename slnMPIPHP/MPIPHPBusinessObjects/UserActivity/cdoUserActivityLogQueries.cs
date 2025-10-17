#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoUserActivityLogQueries:
	/// Inherited from doUserActivityLogQueries, the class is used to customize the database object doUserActivityLogQueries.
	/// </summary>
    [Serializable]
	public class cdoUserActivityLogQueries : doUserActivityLogQueries
	{
		public cdoUserActivityLogQueries() : base()
		{
		}

        public Int32 iintTimeTakenInMS
        {
            get
            {
                return end_time.Subtract(start_time).Milliseconds;
            }
        }

        public int param_count { get; set; }

    } 
} 
