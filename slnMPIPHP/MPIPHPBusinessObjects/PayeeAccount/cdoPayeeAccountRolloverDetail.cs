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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountRolloverDetail:
	/// Inherited from doPayeeAccountRolloverDetail, the class is used to customize the database object doPayeeAccountRolloverDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountRolloverDetail : doPayeeAccountRolloverDetail
	{
		public cdoPayeeAccountRolloverDetail() : base()
		{
		}
        public string istrOrgMPID { get; set; }
        public string istrOrgName { get; set; }
        public string istrBlank { get; set; }
        public string istrSendToParticipant
        {
            get
            {
                if (this.send_to_participant == busConstant.FLAG_YES)
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


        public string istrParticipantPickup
        {
            get
            {
                if (this.participant_pickup == busConstant.FLAG_YES)
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

        public string istrFullAddress
        {
            get
            {
                StringBuilder lstrbAddress = new StringBuilder();
                lstrbAddress.Append(this.addr_line_1 + ",");
                if (!string.IsNullOrEmpty(this.addr_line_2))
                    lstrbAddress.Append(this.addr_line_2 + ",");
                if (!string.IsNullOrEmpty(this.city))
                    lstrbAddress.Append(this.city + ",");
                if (!string.IsNullOrEmpty(this.state_description))
                    lstrbAddress.Append(this.state_description + ",");
                //if (!string.IsNullOrEmpty(this.foreign_province))
                //    lstrbAddress.Append(this.foreign_province + ",");
                if (!string.IsNullOrEmpty(this.country_description))
                    lstrbAddress.Append(this.country_description + ",");
                if (!string.IsNullOrEmpty(this.zip_code))
                    lstrbAddress.Append(this.zip_code + ",");
                if (!string.IsNullOrEmpty(this.zip_4_code))
                    lstrbAddress.Append(this.zip_4_code + ",");
                //if (!string.IsNullOrEmpty(this.foreign_postal_code))
                //    lstrbAddress.Append(this.foreign_postal_code + ",");
                return lstrbAddress.ToString().Remove(lstrbAddress.Length - 1);
            }
        }
        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    } 
} 
