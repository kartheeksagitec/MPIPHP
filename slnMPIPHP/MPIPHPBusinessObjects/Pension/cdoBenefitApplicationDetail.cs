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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitApplicationDetail:
	/// Inherited from doBenefitApplicationDetail, the class is used to customize the database object doBenefitApplicationDetail.
	/// </summary>
    [Serializable]
	public class cdoBenefitApplicationDetail : doBenefitApplicationDetail
	{
		public cdoBenefitApplicationDetail() : base()
		{
		}

        public string istrBenefitOptionValue { get; set; }
        public string istrSubPlanBenefitOptionValue { get; set; }
        public int iintJointAnnuaintID { get; set; }
        public string istrRelationShip { get; set; }
        public string istrGender { get; set; }
        public string istrFullName { get; set; }
        public string istrDOB { get; set; }
        public string istrSurvivorFullName { get; set; }
        public string istrOrganizationName { get; set; }
        public string istrSurvivorTypeValue { get; set; }
        public DateTime idtDOB { get; set; }
        public decimal idecPercentage { get; set; }
        public string istrPlanDescription { get; set; }

        public string istrPlanCode { get; set; }

        public int iintPlan_id { get; set;}

        public string istrSpousalConsent
        {
            get
            {
                if (this.spousal_consent_flag == busConstant.FLAG_YES)
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

        public string istrL52SpecialAccount
        {
            get
            {
                if (this.l52_spl_acc_flag == busConstant.FLAG_YES)
                {
                    return busConstant.FLAG_YES;
                }
                else
                {
                    return busConstant.FLAG_NO;
                }
            }
            set
            {
                ;
            }
        }

        public string istrL161SpecialAccount
        {
            get
            {
                if (this.l161_spl_acc_flag == busConstant.FLAG_YES)
                {
                    return busConstant.FLAG_YES;
                }
                else
                {
                    return busConstant.FLAG_NO;
                }
            }
            set
            {
                ;
            }
        }
    } 
} 
