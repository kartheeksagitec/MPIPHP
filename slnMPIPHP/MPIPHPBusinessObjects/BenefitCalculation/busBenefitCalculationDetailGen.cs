#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using    MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace    MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class    MPIPHP.BusinessObjects.busBenefitCalculationDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitCalculationDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitCalculationDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busBenefitCalculationDetailGen
        /// </summary>
		public busBenefitCalculationDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitCalculationDetailGen.
        /// </summary>
		public cdoBenefitCalculationDetail icdoBenefitCalculationDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitCalculationHeader.
        /// </summary>
		public busBenefitCalculationHeader ibusBenefitCalculationHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitApplicationDetail.
        /// </summary>
		public busBenefitApplicationDetail ibusBenefitApplicationDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAccount.
        /// </summary>
		public busPersonAccount ibusPersonAccount { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPlan.
        /// </summary>
		public busPlan ibusPlan { get; set; }

        public busPerson ibusPerson { get; set; }
        /// <summary>
        /// Gets or sets the collection object of type busBenefitCalculationYearlyDetail. 
        /// </summary>
		public Collection<busBenefitCalculationYearlyDetail> iclbBenefitCalculationYearlyDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busBenefitCalculationOptions. 
        /// </summary>
		public Collection<busBenefitCalculationOptions> iclbBenefitCalculationOptions { get; set; }



        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.FindBenefitCalculationDetail():
        /// Finds a particular record from cdoBenefitCalculationDetail with its primary key. 
        /// </summary>
        /// <param name="aintbenefitcalculationdetailid">A primary key value of type int of cdoBenefitCalculationDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitCalculationDetail(int aintbenefitcalculationdetailid)
		{
			bool lblnResult = false;
			if (icdoBenefitCalculationDetail == null)
			{
				icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail();
			}
			if (icdoBenefitCalculationDetail.SelectRow(new object[1] { aintbenefitcalculationdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadBenefitCalculationHeader():
        /// Loads non-collection object ibusBenefitCalculationHeader of type busBenefitCalculationHeader.
        /// </summary>
		public virtual void LoadBenefitCalculationHeader()
		{
			if (ibusBenefitCalculationHeader == null)
			{
				ibusBenefitCalculationHeader = new busBenefitCalculationHeader();
			}
			ibusBenefitCalculationHeader.FindBenefitCalculationHeader(icdoBenefitCalculationDetail.benefit_calculation_header_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadBenefitApplicationDetail():
        /// Loads non-collection object ibusBenefitApplicationDetail of type busBenefitApplicationDetail.
        /// </summary>
		public virtual void LoadBenefitApplicationDetail()
		{
			if (ibusBenefitApplicationDetail == null)
			{
				ibusBenefitApplicationDetail = new busBenefitApplicationDetail();
			}
			ibusBenefitApplicationDetail.FindBenefitApplicationDetail(icdoBenefitCalculationDetail.benefit_application_detail_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadPersonAccount():
        /// Loads non-collection object ibusPersonAccount of type busPersonAccount.
        /// </summary>
		public virtual void LoadPersonAccount()
		{
			if (ibusPersonAccount == null)
			{
				ibusPersonAccount = new busPersonAccount();
			}
			ibusPersonAccount.FindPersonAccount(icdoBenefitCalculationDetail.person_account_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadPlan():
        /// Loads non-collection object ibusPlan of type busPlan.
        /// </summary>
		public virtual void LoadPlan()
		{
			if (ibusPlan == null)
			{
				ibusPlan = new busPlan();
			}
			ibusPlan.FindPlan(icdoBenefitCalculationDetail.plan_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadBenefitCalculationYearlyDetails():
        /// Loads Collection object iclbBenefitCalculationYearlyDetail of type busBenefitCalculationYearlyDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationYearlyDetails()
		{
			DataTable ldtbList = Select<cdoBenefitCalculationYearlyDetail>(
				new string[1] { enmBenefitCalculationYearlyDetail.benefit_calculation_detail_id.ToString() },
				new object[1] { icdoBenefitCalculationDetail.benefit_calculation_detail_id }, null, enmBenefitCalculationYearlyDetail.benefit_calculation_detail_id.ToString());
			iclbBenefitCalculationYearlyDetail = GetCollection<busBenefitCalculationYearlyDetail>(ldtbList, "icdoBenefitCalculationYearlyDetail");
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationDetailGen.LoadBenefitCalculationOptionss():
        /// Loads Collection object iclbBenefitCalculationOptions of type busBenefitCalculationOptions.
        /// </summary>
		public virtual void LoadBenefitCalculationOptionss()
		{
			DataTable ldtbList = Select<cdoBenefitCalculationOptions>(
				new string[1] { enmBenefitCalculationOptions.benefit_calculation_detail_id.ToString() },
				new object[1] { icdoBenefitCalculationDetail.benefit_calculation_detail_id }, null, enmBenefitCalculationOptions.benefit_calculation_detail_id.ToString());
			iclbBenefitCalculationOptions = GetCollection<busBenefitCalculationOptions>(ldtbList, "icdoBenefitCalculationOptions");
		}

	}
}
