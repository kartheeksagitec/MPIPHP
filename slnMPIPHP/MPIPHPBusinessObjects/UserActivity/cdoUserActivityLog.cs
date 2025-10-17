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
    /// Class MPIPHP.CustomDataObjects.cdoUserActivityLog:
    /// Inherited from doUserActivityLog, the class is used to customize the database object doUserActivityLog.
    /// </summary>
    [Serializable]
    public class cdoUserActivityLog : doUserActivityLog
    {
        public cdoUserActivityLog()
            : base()
        {
        }

        public string user_id { get; set; }
        public string user_name { get; set; }
        public int activity_count { get; set; }
    }
} 
