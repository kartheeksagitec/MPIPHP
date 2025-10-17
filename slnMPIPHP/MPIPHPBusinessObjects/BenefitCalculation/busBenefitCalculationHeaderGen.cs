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
    /// Class    MPIPHP.BusinessObjects.busBenefitCalculationHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitCalculationHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitCalculationHeaderGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busBenefitCalculationHeaderGen
        /// </summary>
		public busBenefitCalculationHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitCalculationHeaderGen.
        /// </summary>
		public cdoBenefitCalculationHeader icdoBenefitCalculationHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busBenefitApplication.
        /// </summary>
		public busBenefitApplication ibusBenefitApplication { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busBenefitCalculationDetail. 
        /// </summary>
		public Collection<busBenefitCalculationDetail> iclbBenefitCalculationDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busDisabilityRetireeIncrease. 
        /// </summary>
		public Collection<busDisabilityRetireeIncrease> iclbDisabilityRetireeIncrease { get; set; }



        /// <summary>
        ///    MPIPHP.busBenefitCalculationHeaderGen.FindBenefitCalculationHeader():
        /// Finds a particular record from cdoBenefitCalculationHeader with its primary key. 
        /// </summary>
        /// <param name="aintBenefitCalculationHeaderId">A primary key value of type int of cdoBenefitCalculationHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitCalculationHeader(int aintBenefitCalculationHeaderId)
		{
			bool lblnResult = false;
			if (icdoBenefitCalculationHeader == null)
			{
				icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader();
			}
			if (icdoBenefitCalculationHeader.SelectRow(new object[1] { aintBenefitCalculationHeaderId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationHeaderGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoBenefitCalculationHeader.person_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationHeaderGen.LoadBenefitApplication():
        /// Loads non-collection object ibusBenefitApplication of type busBenefitApplication.
        /// </summary>
		public virtual void LoadBenefitApplication()
		{
			if (ibusBenefitApplication == null)
			{
				ibusBenefitApplication = new busBenefitApplication();
			}
			ibusBenefitApplication.FindBenefitApplication(icdoBenefitCalculationHeader.benefit_application_id);
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationHeaderGen.LoadBenefitCalculationDetails():
        /// Loads Collection object iclbBenefitCalculationDetail of type busBenefitCalculationDetail.
        /// </summary>
		public virtual void LoadBenefitCalculationDetails()
		{
			DataTable ldtbList = Select<cdoBenefitCalculationDetail>(
				new string[1] { enmBenefitCalculationDetail.benefit_calculation_header_id.ToString() },
				new object[1] { icdoBenefitCalculationHeader.benefit_calculation_header_id }, null, enmBenefitCalculationDetail.benefit_calculation_header_id.ToString());
			iclbBenefitCalculationDetail = GetCollection<busBenefitCalculationDetail>(ldtbList, "icdoBenefitCalculationDetail");
		}

        /// <summary>
        ///    MPIPHP.busBenefitCalculationHeaderGen.LoadDisabilityRetireeIncreases():
        /// Loads Collection object iclbDisabilityRetireeIncrease of type busDisabilityRetireeIncrease.
        /// </summary>
		public virtual void LoadDisabilityRetireeIncreases()
		{
			DataTable ldtbList = Select<cdoDisabilityRetireeIncrease>(
				new string[1] { enmDisabilityRetireeIncrease.benefit_calculation_header_id.ToString() },
				new object[1] { icdoBenefitCalculationHeader.benefit_calculation_header_id }, null, enmDisabilityRetireeIncrease.benefit_calculation_header_id.ToString());
			iclbDisabilityRetireeIncrease = GetCollection<busDisabilityRetireeIncrease>(ldtbList, "icdoDisabilityRetireeIncrease");
		}

	}
}
