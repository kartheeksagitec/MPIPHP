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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitCalculationDetail:
	/// Inherited from doBenefitCalculationDetail, the class is used to customize the database object doBenefitCalculationDetail.
	/// </summary>
    [Serializable]
	public class cdoBenefitCalculationDetail : doBenefitCalculationDetail
	{
		public cdoBenefitCalculationDetail() : base()
		{
               
		}
        public string istrPlanCode { get; set; }
        public string istrPlanDescription { get; set; }
        public int iintIAPasYear { get; set; }
        public decimal idecRemainingBenefits { get; set; }
        public decimal idecAlternatePayeePurecontribution { get; set; }

        public decimal idecQuaterllyAllocation { get; set; }
        public int iintQuater { get; set; }
        public decimal idecRate { get; set; }
        public decimal idecTotal { get; set; }
        public decimal idecPrevYearEndingBalance { get; set; }
        public string istrBenefitOptionValue { get; set; }
        public string istrBenefitOptionValueDescrioption { get; set; }
        public string istrSpecialAccount { get; set; }
        public string istrSpecialAccountDescrioption { get; set; }
        public decimal idecSurvivorAmount { get; set; }
        public string istrSurvivorRelationshipDescription { get; set; }

        public decimal idecAltPayee_Fraction { get; set; }
        public decimal idecAlt_payee_ee_contribution { get; set; }

        public int iintPersonId { get; set; }




        public decimal idecBenefitsAfterQDROOffSet
        {
            set { ;}
            get
            {
                return unreduced_benefit_amount - qdro_offset;
            }
        }
    } 
} 
