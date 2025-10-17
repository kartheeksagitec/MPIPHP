#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
    [Serializable]
	public class cdoCorTracking : doCorTracking
	{
		public cdoCorTracking() : base()
		{
		}

        public string istrMpiPersonID { get; set; }
        public string istrFullName { get; set; }

        public string template_name { get; set; }

        public string analystname { get; set; }


    } 
} 
