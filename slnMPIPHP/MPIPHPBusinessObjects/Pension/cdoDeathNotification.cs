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
	/// Class MPIPHP.CustomDataObjects.cdoDeathNotification:
	/// Inherited from doDeathNotification, the class is used to customize the database object doDeathNotification.
	/// </summary>
    [Serializable]
	public class cdoDeathNotification : doDeathNotification
	{
		public cdoDeathNotification() : base()
		{
		}
        public int istrBenDepId { get; set; }
        public string istrBenDepMpid { get; set; }
        public string istrBenDepName { get; set; }
        public string istrBenDepRelation { get; set; }

        // For Report
        public decimal idecAge { get; set; }
        public int iintUnionCode { get; set; }
        public string istrUnionDescription { get; set; }
        public string istrMpiPersonId { get; set; }
        public string istrName { get; set; }
        public string istrSSn { get; set; }
        public string idtPreviousDateofDeath { get; set; }//InCorrectly Reported Deaths
        public DateTime idtDateOfBirth { get; set; }
        public string istrRetireeStatus { get; set; }//Retiree or Active ( A or R)

        public string istrDateofDeath{get;set;}
        public string istrStatus { get; set; }
        public string istrRelativeVipFlag { get; set; }

        public string istrPersonType { get; set; }//Tushar-806
        //PIR RID 56892
        public string istrMPIVested { get; set; }

    }
} 
