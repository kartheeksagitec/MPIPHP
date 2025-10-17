#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;

#endregion

namespace MPIPHP.CustomDataObjects 
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoDataExtractionBatchInfo:
	/// Inherited from doDataExtractionBatchInfo, the class is used to customize the database object doDataExtractionBatchInfo.
	/// </summary>
    [Serializable]
	public class cdoDataExtractionBatchInfo : doDataExtractionBatchInfo
	{
		public cdoDataExtractionBatchInfo() : base()
		{
         
		}
        public string UNION_CODE { get; set; }
        public int YEAR { get; set; }
        public string istrPlan { get; set; }
        public string istrUnionCode { get; set; }

        public string istrPersonSSNNonEncrypted
        {
            get;
            set;
        }

        public DateTime idtPersonDateofBirth
        {
            get;
            set;
        }

        public string istrPersonDateofBirth
        {
            get;
            set;
        }

        public DateTime idtBeneficiaryDateofBirth
        {
            get;
            set;
        }

        public string istrBeneficiarySSNNonEncrypted
        {
            get;
            set;
        }

        public string istrBeneficiaryDateofBirth
        {
            get;
            set;
        }
        
        public string istrParticipantDOD
        {
            get;
            set;
        }
        public string istrBeneficiaryDOD
        {
            get;
            set;
        }
        public string istrDeterminationDate
        {
            get;
            set;
        }
        public string istrBen1stPaymentRcvDt
        {
            get;
            set;
        }
        public string istrPensionStopDt
        {
            get;
            set;
        }

        public string istrDeterminationDtforOutbound
        {
            get;
            set;
        }

        public string istrbeneficiary_first_payment_receive_dateOutboundFile
        {
            get;
            set;
        }
        public string istrpension_stop_date_for_OutboundFile
        {
            get;
            set;
        }
        public string istrbeneficiary_date_of_death_for_OutboundFile
        {
            get;
            set;
        }

        public string istrPerson_DOD_for_OutboundFile
        {
            get;
            set;
        }
        

        public override void LoadData(DataRow adtrRow)
        {
            base.LoadData(adtrRow);
            if (!Sagitec.Common.HelperFunction.IsNumeric(this.person_ssn))
            {
                this.istrPersonSSNNonEncrypted = this.person_ssn;
            }
            if (!Sagitec.Common.HelperFunction.IsNumeric(this.beneficiary_ssn))
            {
                this.istrBeneficiarySSNNonEncrypted = this.beneficiary_ssn;
            }
            if (this.person_dob != DateTime.MinValue)
            {
                this.idtPersonDateofBirth = this.person_dob;
            }
            if (this.beneficiary_dob != DateTime.MinValue)
            {
                this.idtBeneficiaryDateofBirth = this.beneficiary_dob;

            }

            //for Out bound File
            if (this.beneficiary_dob != DateTime.MinValue)
            {
                DateTime idtBeneficiaryDateofBirth = this.beneficiary_dob;
                this.istrBeneficiaryDateofBirth = Convert.ToString(idtBeneficiaryDateofBirth.ToShortDateString());
                if (this.istrBeneficiaryDateofBirth != null)
                {
                    this.istrBeneficiaryDateofBirth = this.istrBeneficiaryDateofBirth + "       ";
                }
            }
            if (this.determination_date != DateTime.MinValue)
            {
                istrDeterminationDtforOutbound = Convert.ToString(this.determination_date.ToShortDateString()) + "   "; 
            }
            if (this.beneficiary_first_payment_receive_date != DateTime.MinValue)
            {
                istrbeneficiary_first_payment_receive_dateOutboundFile = Convert.ToString(this.beneficiary_first_payment_receive_date.ToShortDateString()) + "   "; 
            }

            if (this.pension_stop_date != DateTime.MinValue)
            {
                istrpension_stop_date_for_OutboundFile = Convert.ToString(this.pension_stop_date.ToShortDateString()) + "   ";
            }

            if (this.beneficiary_date_of_death != DateTime.MinValue)
            {
                istrbeneficiary_date_of_death_for_OutboundFile = Convert.ToString(this.beneficiary_date_of_death.ToShortDateString()) + "          ";
            }

            if (this.participant_date_of_death != DateTime.MinValue)
            {
                istrPerson_DOD_for_OutboundFile = Convert.ToString(this.participant_date_of_death.ToShortDateString()) + "       ";
            }

            if (Convert.ToString(this.participant_date_of_death) == "1/1/1753 12:00:00 AM")
            {
                istrParticipantDOD = string.Empty;
            }
            else
            {
                istrParticipantDOD = Convert.ToString(this.participant_date_of_death);
            }
            if (Convert.ToString(this.beneficiary_date_of_death) == "1/1/1753 12:00:00 AM")
            {
                istrBeneficiaryDOD = string.Empty;
            }
            else
            {
                istrBeneficiaryDOD = Convert.ToString(this.beneficiary_date_of_death);
            }
            if (Convert.ToString(this.determination_date) == "1/1/1753 12:00:00 AM")
            {
                istrDeterminationDate = string.Empty;
            }
            else
            {
                istrDeterminationDate = Convert.ToString(this.determination_date);
            }
            if (Convert.ToString(this.beneficiary_first_payment_receive_date) == "1/1/1753 12:00:00 AM")
            {
                istrBen1stPaymentRcvDt = string.Empty;
            }
            else
            {
                istrBen1stPaymentRcvDt = Convert.ToString(this.beneficiary_first_payment_receive_date);
            }
            if (Convert.ToString(this.pension_stop_date) == "1/1/1753 12:00:00 AM")
            {
                istrPensionStopDt = string.Empty;
            }
            else
            {
                istrPensionStopDt = Convert.ToString(this.pension_stop_date);
            }
        }

    } 
} 
