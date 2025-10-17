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
	/// Class MPIPHP.CustomDataObjects.cdoDroApplication:
	/// Inherited from doDroApplication, the class is used to customize the database object doDroApplication.
	/// </summary>
    [Serializable]
	public class cdoDroApplication : doDroApplication
	{
		public cdoDroApplication() : base()
		{
		}

        public string istrRelativeVipFlag { set; get; }
        public string  istrAlternate_Payee_Mpid{ set; get; }
        public string istrAlternate_Payee_Fullname { set; get; }
        public string istrDROModel { set; get; }
        public string istrPlan { get; set; }
        public string istrMemeber_Mpid { set; get; }
        public string istrMemeber_Fullname { set; get; }
    } 

} 
