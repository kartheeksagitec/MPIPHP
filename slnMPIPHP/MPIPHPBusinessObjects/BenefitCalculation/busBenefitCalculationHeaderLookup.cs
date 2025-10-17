#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busBenefitCalculationHeaderLookup:
	/// Inherited from busBenefitCalculationHeaderLookupGen, this class is used to customize the lookup business object busBenefitCalculationHeaderLookupGen. 
	/// </summary>
	[Serializable]
	public class busBenefitCalculationHeaderLookup : busBenefitCalculationHeaderLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            if (aobjBus is busBenefitCalculationHeader)
            {
                busBenefitCalculationHeader lbusBenefitCalculationHeader = (busBenefitCalculationHeader)aobjBus;
                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.PopulateDescriptions();
              
                if (adtrRow[enmBenefitCalculationDetail.benefit_subtype_id.ToString()] != DBNull.Value && adtrRow[enmBenefitCalculationDetail.benefit_subtype_value.ToString()] != DBNull.Value)
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.istrRetirementType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(Convert.ToInt32(adtrRow[enmBenefitCalculationDetail.benefit_subtype_id.ToString()]), adtrRow[enmBenefitCalculationDetail.benefit_subtype_value.ToString()].ToString());
                }

                if (adtrRow[enmBenefitCalculationHeader.dro_application_id.ToString()] != DBNull.Value && Convert.ToInt32(adtrRow[enmBenefitCalculationHeader.dro_application_id.ToString()]) > 0)
                {
                    lbusBenefitCalculationHeader.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationHeader.ibusAlternatePayee = new busPerson() { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationHeader.ibusAlternatePayee.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                    lbusBenefitCalculationHeader.ibusAlternatePayee.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                    lbusBenefitCalculationHeader.ibusAlternatePayee.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                    lbusBenefitCalculationHeader.ibusAlternatePayee.icdoPerson.mpi_person_id = adtrRow[busConstant.MPI_ID].ToString();
                }
                else
                {
                    lbusBenefitCalculationHeader.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusBenefitCalculationHeader.ibusPerson.icdoPerson.LoadData(adtrRow);
                }
            }
        }
	}
}
