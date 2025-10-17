#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using MPIPHP.BusinessObjects;
#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountAchDetail:
	/// Inherited from doPayeeAccountAchDetail, the class is used to customize the database object doPayeeAccountAchDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountAchDetail : doPayeeAccountAchDetail
	{
		public cdoPayeeAccountAchDetail() : base()
		{
		}
        public string istrOrgMPID {get;set;}
        public string istrOrgName { get; set; }
        public string istrPrimaryFlag { get; set; }
        public int iintRoutingNumber { get; set; }
        public string istrRoutingNumber { get; set; }

        public string istrPreNoteFlag 
        {
            get
            {
                if (this.pre_note_flag == busConstant.FLAG_YES)
                {
                    return busConstant.YES;
                }
                else
                {
                    return busConstant.NO;
                }
            }
            set
            {
                ;
            }
        }
        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    } 
} 
