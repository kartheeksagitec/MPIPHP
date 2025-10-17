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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitCalculationYearlyDetail:
	/// Inherited from doBenefitCalculationYearlyDetail, the class is used to customize the database object doBenefitCalculationYearlyDetail.
	/// </summary>
    [Serializable]
	public class cdoBenefitCalculationYearlyDetail : doBenefitCalculationYearlyDetail
	{
		public cdoBenefitCalculationYearlyDetail() : base()
		{
		}

        //For Annual benefit summary overview
        public decimal idecTotalPensionHours { get; set; }
        public decimal idecTotalIAPHours { get; set; }
        public decimal idecTotalHealthHours { get; set; }
        public int iintHealthCount { get; set; }
        public decimal idecTotalVestedHours { get; set; }
        public int iintQualifiedYears { get; set; }
        public int iiVestedYears { get; set; }

        public decimal idecTotalBenefitAmount { get; set; }
        public decimal idecYTDBenefit { get; set; }
        //public bool iblnHoursAfterRetirement { get; set; }
    } 
} 
