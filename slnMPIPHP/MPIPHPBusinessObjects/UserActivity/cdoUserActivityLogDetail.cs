#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Collections;
using Sagitec.Common;

#endregion

namespace MPIPHP.CustomDataObjects
{
    /// <summary>
    /// Class MPIPHP.CustomDataObjects.cdoUserActivityLogDetail:
    /// Inherited from doUserActivityLogDetail, the class is used to customize the database object doUserActivityLogDetail.
    /// </summary>
    [Serializable]
    public class cdoUserActivityLogDetail : doUserActivityLogDetail
    {
        public cdoUserActivityLogDetail()
            : base()
        {
        }

        public Int32 iintTimeTakenInMS
        {
            get
            {
                return end_time.Subtract(start_time).Milliseconds;
            }
        }

        public string istrShortMessage
        {
            get
            {
                if (response != null && response.Length > 30)
                {
                    return response.Substring(0, 27) + "...";
                }
                else
                    return response;
            }
        }

        public int param_count { get; set; }
        public int query_count { get; set; }
        public string audit_count { get; set; }
    }
}
