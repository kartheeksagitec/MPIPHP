#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using Sagitec.Common;
using MPIPHP.Common;
using System.Collections.ObjectModel;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPersonAddress:
	/// Inherited from doPersonAddress, the class is used to customize the database object doPersonAddress.
	/// </summary>
    [Serializable]
	public class cdoPersonAddress : doPersonAddress
	{ 
        public bool iblnActualChange = false;
        public string istrPhysicalAddressType { get; set; }
        public string istrMailingAddressType { get; set; }
        public string istrAddSameAsParticipantFlag { get; set; }
        public string astrBenOrDep { get; set; }//Either Beneficiary or Dependent.
        public int iaintMainParticipantAddressID { get; set; } // lo to main participant address  if person is a beneficiary.

        public string ContactName { get; set; }
        public cdoPersonAddress() : base()
		{
            iblnCallAfterChange = true;
		}
       
        public string istrAlternateCorrespondenceAddress
        {
            get;
            set;
        }

        public string istrfull_address
        {
            get
            {
                StringBuilder lstrbAddress = new StringBuilder();
                lstrbAddress.Append(this.addr_line_1 + ", ");
                if (!string.IsNullOrEmpty(this.addr_line_2))
                    lstrbAddress.Append(this.addr_line_2 + ", ");
                if (!string.IsNullOrEmpty(this.addr_city))
                    lstrbAddress.Append(this.addr_city + ", ");
               
                if (!string.IsNullOrEmpty(this.addr_state_description) && this.addr_state_description!=busConstant.OTHER)
                    lstrbAddress.Append(this.addr_state_description + ", ");

                if (!string.IsNullOrEmpty(this.foreign_province))
                    lstrbAddress.Append(this.foreign_province + ", ");

                if (!string.IsNullOrEmpty(this.addr_country_description))
                    lstrbAddress.Append(this.addr_country_description + ", ");

                if (!string.IsNullOrEmpty(this.addr_zip_code))
                    lstrbAddress.Append(this.addr_zip_code + "-");

                if (!string.IsNullOrEmpty(this.addr_zip_4_code))
                    lstrbAddress.Append(this.addr_zip_4_code + ",");

                if (!string.IsNullOrEmpty(this.foreign_postal_code))
                    lstrbAddress.Append(this.foreign_postal_code + ",");

                return lstrbAddress.ToString().Remove(lstrbAddress.Length - 1);
            }
        }

        public override void AfterChange(string astrTableName, Sagitec.Common.ObjectState aenmObjectState, Collection<Sagitec.Common.utlAuditLogDetail> acolChangedValues)
        {
            base.AfterChange(astrTableName, aenmObjectState, acolChangedValues);
            if (acolChangedValues.Count > 2 && aenmObjectState == ObjectState.Update)
            {
                foreach (utlAuditLogDetail lObjAuditLog in acolChangedValues)
                {
                    if (lObjAuditLog.column_name.ToUpper() != busConstant.MODIFIEDDATE.ToUpper() && lObjAuditLog.column_name.ToUpper() != busConstant.UPDATESEQ.ToUpper())
                    {
                        string lstrNewValue = lObjAuditLog.new_value;
                        if (string.IsNullOrEmpty(lstrNewValue))
                        {
                            lstrNewValue = string.Empty;
                        }
                        string lstrOldValue = lObjAuditLog.old_value;
                        if (string.IsNullOrEmpty(lstrOldValue))
                        {
                            lstrOldValue = string.Empty;
                        }
                        if (lstrNewValue.Trim() != lstrOldValue.Trim())
                        {
                            iblnActualChange = true;
                        }
                    }
                }
            }

        }

        public string istrCompleteZipCode
        {
            get
            {
                string lstrZip = this.addr_zip_code;
                if (!string.IsNullOrEmpty(addr_zip_4_code))
                {
                    lstrZip += "-" + addr_zip_4_code;
                }
                return lstrZip;
            }
        }

        public string istrForeignProvince
        {
            get 
            {
                string lstrForeignProvince = this.addr_state_description;
                if (!string.IsNullOrEmpty(addr_country_value))
                {
                    int lintCountryValue = Convert.ToInt32(addr_country_value);

                    if (lintCountryValue == busConstant.USA || lintCountryValue == busConstant.AUSTRALIA || lintCountryValue == busConstant.CANADA ||
                        lintCountryValue == busConstant.MEXICO || lintCountryValue == busConstant.NewZealand)
                    {
                        lstrForeignProvince = addr_state_description ?? String.Empty;
                    }
                    else
                    {
                        lstrForeignProvince = foreign_province ?? String.Empty;
                    }
                }
                return lstrForeignProvince;
            } 
        }

        public string istrZipCode
        {
            get
            {
                string lstrZipCode = this.addr_zip_code;
                if (!string.IsNullOrEmpty(addr_country_value))
                {
                    int lintCountryValue = Convert.ToInt32(addr_country_value);
                    if (lintCountryValue == busConstant.USA)
                    {
                        lstrZipCode = addr_zip_code ?? String.Empty;
                        if ((addr_zip_4_code != null) && (addr_zip_4_code.Trim() != String.Empty))
                        {
                            lstrZipCode += "-" + addr_zip_4_code ?? String.Empty;
                        }
                    }
                    else
                    {
                        lstrZipCode = foreign_postal_code ?? String.Empty;
                    }
                }
                return lstrZipCode;
            }
        }
    } 
} 
