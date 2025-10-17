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
    /// Class MPIPHP.CustomDataObjects.cdoBenefitApplication:
    /// Inherited from doBenefitApplication, the class is used to customize the database object doBenefitApplication.
    /// </summary>
    [Serializable]
    public class cdoBenefitApplication : doBenefitApplication
    {
        public cdoBenefitApplication()
            : base()
        {
        }
        public string istrPlanDescription { get; set; }
        public string istrPlanCode { get; set; }
        public int iintPlanId { get; set; }
        public string istrJoinderOnFile { get; set; }
        public string istrRelativeVipFlag { get; set; }
        public string istrMpid { get; set; }
        public string istrFullname { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrSurvivor { get; set; }
        public string istrSurvivorMPID { get; set; }
        public DateTime dtDateOfMarriage { get; set; }
        public DateTime dtEarlyRetirementDate { get; set; }
        public string istrFunds { get; set; } //PIR RID 68080

        public string VALID_ADDR_FLAG { get; set; }//PIR 1002

        ////PROD PIR 799
        //public string istrMessageforPlan { get; set; }
        //Code-Abhishek
        //Affliate -- Set of Properties
        //public int aintTotalQualifiedYears { get; set; }
        //public int aintTotalVestedYears { get; set; }
        //public int aintTotalAnniversaryYears { get; set; }
        //public string istrIsBISParticipant { get; set; }
        //public string istrIsForfieture { get; set; }        
        ////For Non_Affiliate -- Set of Properties
        //public int aintTotalQualifiedYearsNA { get; set; }
        //public int aintTotalVestedYearsNA { get; set; }
        //public int aintTotalAnniversaryYearsNA { get; set; }
        //public string istrIsBISParticipantNA { get; set; }
        //public string istrIsForfietureNA { get; set; }

        public string istrIsPersonVestedinMPI { get; set; }
        public string istrIsPersonVestedinIAP { get; set; }
        public string istrVestedInRuleDescription { get; set; }
        public DateTime adtMPIVestingDate { get; set; }
        public DateTime adtIAPVestingDate { get; set; }
        public string istrDocument { get; set; }
        public decimal idecIAPBenefitAmount { get; set; } //RequestID: 64733

        public string istrMinDistributionFlag
        {
            get
            {
                if (this.min_distribution_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }
        //Code-Abhishek
    }
}
