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
    /// Class    MPIPHP.BusinessObjects.busQdroCalculationDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoQdroCalculationDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busQdroCalculationDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busQdroCalculationDetailGen
        /// </summary>
		public busQdroCalculationDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busQdroCalculationDetailGen.
        /// </summary>
		public cdoQdroCalculationDetail icdoQdroCalculationDetail { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busQdroCalculationOptions. 
        /// </summary>
		public Collection<busQdroCalculationOptions> iclbQdroCalculationOptions { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busQdroCalculationYearlyDetail. 
        /// </summary>
		public Collection<busQdroCalculationYearlyDetail> iclbQdroCalculationYearlyDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busQdroIapAllocationDetail. 
        /// </summary>
		public Collection<busQdroIapAllocationDetail> iclbQdroIapAllocationDetail { get; set; }



        /// <summary>
        ///    MPIPHP.busQdroCalculationDetailGen.FindQdroCalculationDetail():
        /// Finds a particular record from cdoQdroCalculationDetail with its primary key. 
        /// </summary>
        /// <param name="aintQdroCalculationDetailId">A primary key value of type int of cdoQdroCalculationDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindQdroCalculationDetail(int aintQdroCalculationDetailId)
		{
			bool lblnResult = false;
			if (icdoQdroCalculationDetail == null)
			{
				icdoQdroCalculationDetail = new cdoQdroCalculationDetail();
			}
			if (icdoQdroCalculationDetail.SelectRow(new object[1] { aintQdroCalculationDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busQdroCalculationDetailGen.LoadQdroCalculationOptionss():
        /// Loads Collection object iclbQdroCalculationOptions of type busQdroCalculationOptions.
        /// </summary>
		public virtual void LoadQdroCalculationOptionss()
		{
			DataTable ldtbList = Select<cdoQdroCalculationOptions>(
				new string[1] { enmQdroCalculationOptions.qdro_calculation_detail_id.ToString() },
				new object[1] { icdoQdroCalculationDetail.qdro_calculation_detail_id }, null, null);
			iclbQdroCalculationOptions = GetCollection<busQdroCalculationOptions>(ldtbList, "icdoQdroCalculationOptions");
		}

        /// <summary>
        ///    MPIPHP.busQdroCalculationDetailGen.LoadQdroCalculationYearlyDetails():
        /// Loads Collection object iclbQdroCalculationYearlyDetail of type busQdroCalculationYearlyDetail.
        /// </summary>
		public virtual void LoadQdroCalculationYearlyDetails()
		{
			DataTable ldtbList = Select<cdoQdroCalculationYearlyDetail>(
				new string[1] { enmQdroCalculationYearlyDetail.qdro_calculation_detail_id.ToString() },
				new object[1] { icdoQdroCalculationDetail.qdro_calculation_detail_id }, null, null);
			iclbQdroCalculationYearlyDetail = GetCollection<busQdroCalculationYearlyDetail>(ldtbList, "icdoQdroCalculationYearlyDetail");
		}

        /// <summary>
        ///    MPIPHP.busQdroCalculationDetailGen.LoadQdroIapAllocationDetails():
        /// Loads Collection object iclbQdroIapAllocationDetail of type busQdroIapAllocationDetail.
        /// </summary>
		public virtual void LoadQdroIapAllocationDetails()
		{
			DataTable ldtbList = Select<cdoQdroIapAllocationDetail>(
				new string[1] { enmQdroIapAllocationDetail.qdro_calculation_detail_id.ToString() },
				new object[1] { icdoQdroCalculationDetail.qdro_calculation_detail_id }, null, null);
			iclbQdroIapAllocationDetail = GetCollection<busQdroIapAllocationDetail>(ldtbList, "icdoQdroIapAllocationDetail");
		}

	}
}
