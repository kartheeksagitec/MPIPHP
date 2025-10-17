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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busQdroCalculationLookup:
	/// Inherited from busQdroCalculationLookupGen, this class is used to customize the lookup business object busQdroCalculationLookupGen. 
	/// </summary>
	[Serializable]
	public class busQdroCalculationLookup : busQdroCalculationLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            if (aobjBus is busQdroCalculationHeader)
            {
                busQdroCalculationHeader lbusQdroCalculationHeader = (busQdroCalculationHeader)aobjBus;
               
                lbusQdroCalculationHeader.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                lbusQdroCalculationHeader.ibusParticipant.icdoPerson.LoadData(adtrRow);
                lbusQdroCalculationHeader.ibusParticipant.istrFullName = adtrRow["ParticipantName"].ToString();

                lbusQdroCalculationHeader.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };
               // lbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.LoadData(adtrRow);
                lbusQdroCalculationHeader.ibusAlternatePayee.istrFullName = adtrRow["AlternatePayeeName"].ToString();
                lbusQdroCalculationHeader.ibusAlternatePayee.icdoPerson.mpi_person_id = adtrRow["AlternatePayeeMPIId"].ToString();

            }
        }
	}
}
