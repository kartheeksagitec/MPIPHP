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
	/// Class MPIPHP.CustomDataObjects.cdoPaymentReissueDetail:
	/// Inherited from doPaymentReissueDetail, the class is used to customize the database object doPaymentReissueDetail.
	/// </summary>
    [Serializable]
	public class cdoPaymentReissueDetail : doPaymentReissueDetail
	{
		public cdoPaymentReissueDetail() : base()
		{
		}

        public string istrRMPID { get; set; }
        public string istrRecipientRollOverOrgMPID { get; set; }
        public int iintSurvivorId { get; set; }
        public int iintOrganisation { get; set; }
       public int  iintPartitipantID { get; set; }

    } 
} 
