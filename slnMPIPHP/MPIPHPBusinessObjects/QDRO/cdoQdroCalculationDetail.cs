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
	/// Class MPIPHP.CustomDataObjects.cdoQdroCalculationDetail:
	/// Inherited from doQdroCalculationDetail, the class is used to customize the database object doQdroCalculationDetail.
	/// </summary>
    [Serializable]
	public class cdoQdroCalculationDetail : doQdroCalculationDetail
	{
		public cdoQdroCalculationDetail() : base()
		{
		}

        public string istrPlanCode { get; set; }
        public string istrPlanDescription { get; set; }
        public string istrIsFinal { get; set; }
        public bool iblnIsNewRecord { get; set; }
        public decimal idecParticipantAmount { get; set; }

        //public Decimal idecParticipantTotalEE
        //{
        //    set { ;}

        //    get { return total_ee_contribution_amount + total_ee_interest_amount; }
        //}

        public Decimal idecAltPayeeTotalEE
        {
            set { ;}

            get {return alt_payee_ee_contribution + alt_payee_interest_amount; }
        }

        public Decimal idecParticipantTotalUVHP
        {
            set { ;}

            get 
            {
                if (overriden_uvhp_amount != 0 && overriden_uv_uvhp_int_amount != 0)
                {
                    return overriden_uvhp_amount + overriden_uv_uvhp_int_amount;
                }
                else
                {
                    return total_uvhp_contribution_amount + total_uvhp_interest_amount;
                }
            }
        }

        public Decimal idecAltPayeeUVHP
        {
            set { ;}

            get {return alt_payee_uvhp + alt_payee_uvhp_interest; }
        }

        public string istrRetirementTypeDisability { get; set; }

    } 
} 
