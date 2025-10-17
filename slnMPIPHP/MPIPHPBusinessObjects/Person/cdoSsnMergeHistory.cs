#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoSsnMergeHistory:
	/// Inherited from doSsnMergeHistory, the class is used to customize the database object doSsnMergeHistory.
	/// </summary>
    [Serializable]
	public class cdoSsnMergeHistory : doSsnMergeHistory
	{
        public cdoSsnMergeHistory()
            : base()
        {
        }

        public string istrRelativeVipFlag { get; set; }
    
    } 
} 
