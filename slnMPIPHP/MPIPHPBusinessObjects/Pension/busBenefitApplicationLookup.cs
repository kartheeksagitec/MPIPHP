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
	/// Class MPIPHP.BusinessObjects.busBenefitApplicationLookup:
	/// Inherited from busBenefitApplicationLookupGen, this class is used to customize the lookup business object busBenefitApplicationLookupGen. 
	/// </summary>
	[Serializable]
	public class busBenefitApplicationLookup : busBenefitApplicationLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);

            //In the Following code I cannot use LoadData() method since I have 2 cdoPerson in the same DataRow
            //The method will not know what to bind with that
            //Hence we are getting only the necessary information in the LookUp Query
            busBenefitApplication lbusBenefitApplication = (busBenefitApplication)abusBase;

            if (adtrRow[enmBenefitApplication.dro_application_id.ToString()] != DBNull.Value && Convert.ToInt32(adtrRow[enmBenefitApplication.dro_application_id.ToString()]) > 0)
            {
                lbusBenefitApplication.ibusAlternatePayee = new busPerson() { icdoPerson = new cdoPerson() };
                lbusBenefitApplication.ibusAlternatePayee.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                lbusBenefitApplication.ibusAlternatePayee.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                lbusBenefitApplication.ibusAlternatePayee.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                lbusBenefitApplication.ibusAlternatePayee.icdoPerson.mpi_person_id = adtrRow[busConstant.MPI_ID].ToString();
                lbusBenefitApplication.ibusAlternatePayee.icdoPerson.ssn = adtrRow[enmPerson.ssn.ToString()].ToString();
            }
            else
            {
                lbusBenefitApplication.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lbusBenefitApplication.ibusPerson.icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                lbusBenefitApplication.ibusPerson.icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                lbusBenefitApplication.ibusPerson.icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                lbusBenefitApplication.ibusPerson.icdoPerson.mpi_person_id = adtrRow[busConstant.MPI_ID].ToString();
                lbusBenefitApplication.ibusPerson.icdoPerson.ssn = adtrRow[enmPerson.ssn.ToString()].ToString();
            }
        }
	}
}
