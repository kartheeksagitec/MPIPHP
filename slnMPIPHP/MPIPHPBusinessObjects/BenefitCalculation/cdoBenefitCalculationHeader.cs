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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitCalculationHeader:
	/// Inherited from doBenefitCalculationHeader, the class is used to customize the database object doBenefitCalculationHeader.
	/// </summary>
    [Serializable]
	public class cdoBenefitCalculationHeader : doBenefitCalculationHeader
	{
        public decimal iintSurvivorAgeAtRetirement { get; set; }
        public string istrRetirementType { get; set; }

        /// <summary>
        /// For Death To know the Type at Earliest Retirement.
        /// </summary>
        public string istrEarliestRetirementType { get; set; }
        public string istrRelativeVipFlag { get; set; }
        public int iintPlanId { get; set; }
        public string istrPlanDescription { get; set; }
        public string istrSurvivorMPID { get; set; }
        public string istrApprovedBy { get; set; }
        public string istrSurvivorTypeValue { get; set; }
        public string istrOrganizationId { get; set; }
        public string istrOrganizationName { get; set; }       
        public string istrPersonMPID { get; set; }
        public string istrFunds { get; set; } //PIR RID 68080

        public string istrBenefitOptionValue { get; set; }

        public decimal idecOverriddenBenefitAmount { get; set; }

        public int iintParticipantAgeAtDeath { get; set; }

        //PIR 1035
        public DateTime idtNormalRetirementDate { get; set; }
       
        public decimal idecParticipantFullAge
        {
            get { return Math.Floor(age); }

            set { ;}
        }
        public decimal idecSurvivorFullAge
        {
            get { return Math.Floor(iintSurvivorAgeAtRetirement); }

            set { ;}
        }

        public string istrDisabilityType
        {
            get
            {
                string lstrDisType = busConstant.DISABILITY_TYPE_SSA;
                if (this.terminally_ill_flag == busConstant.FLAG_YES)
                {
                    lstrDisType = busConstant.DISABILITY_TYPE_TERMINAL;

                }
                return lstrDisType;
            }
            set
            { ;}
        }

        //RID - 54372 
        public string istrRetirementTypeDescription
        {
            get
            {
               return iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1501, istrRetirementType);
            }
            set
            {
                ;
            }

        }

        //PIR 944
        public string istrLumpSumPaymentFlag
        {
            get
            {
                if (lump_sum_payment == busConstant.FLAG_YES)
                    return "Yes";
                else
                    return "No";
            }
        }

        //10 Percent
        public int iintPayeeAccountId { set; get; }

        //PIR 894
        public bool iblnPopUpToLife { get; set; }
        public string istrOriginalBenefitOptionValue { get; set; }
        public DateTime idtJointAnnuitantDOD {get;set;}
        public string istrMoreInformation { get; set; }
        
		public cdoBenefitCalculationHeader() : base()
		{

		}
    } 
} 
