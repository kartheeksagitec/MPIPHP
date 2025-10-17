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
    [Serializable]
	public class cdoJobHeader : doJobHeader
	{
		public cdoJobHeader() : base()
		{
            
		}
        // declare to know status about generate merged pdf files PIR 845
        public bool iblnGeneratePdfFlag = false; 
    } 
} 
