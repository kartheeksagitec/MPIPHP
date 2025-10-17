#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busRelationship:
	/// Inherited from busRelationshipGen, the class is used to customize the business object busRelationshipGen.
	/// </summary>
	[Serializable]
	public class busRelationship : busRelationshipGen
    {
        #region Properties

        public busPerson ibusPerson { get; set; }
        public string istrSSN { get; set; }
        public string istrPersonID { get; set; }
        public string istrMpiPersonID{ get; set; }
        public string istrMpiOrgID { get; set; }
        public string istrSameAsParticipant { get; set; }
     
        #endregion

        #region Public Methods

        public override busBase GetCorPerson() 
        {
            if (this.ibusPerson.ibusPersonAddress == null)
            {
                this.ibusPerson.ibusPersonAddress = new busPersonAddress { icdoPersonAddress=new cdoPersonAddress() };
               // this.ibusPerson.ibusPersonAddress.LoadMailingAddress();
            }

            return this.ibusPerson;
        }

        public busPerson LoadExistingSpouseDetails(int aintPersonId)
        {
            DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.GetQualifiedSpouseDetails", new object[1] { aintPersonId });
            if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
            {
                int lintSpousePersonID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] 
                                { lintSpousePersonID, aintPersonId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (QualifiedDROExists == 0)
                {
                    busPerson lbusQualifiedSpouse = new busPerson { icdoPerson = new cdoPerson() };
                    lbusQualifiedSpouse.icdoPerson.LoadData(ldtbLatestSpouseInfo.Rows[0]);
                    return lbusQualifiedSpouse;
                    //this.icdoBenefitCalculationHeader.istrSurvivorMPID = lbusSpouse.icdoPerson.mpi_person_id;

                    // this.icdoBenefitCalculationHeader.beneficiary_person_name = lbusSpouse.icdoPerson.istrFullName;
                    //this.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth = lbusSpouse.icdoPerson.idtDateofBirth;
                }                
            }
            return null;
        }


        public busRelationship CheckIfBeneficiaryIsSpouse(int aintPersonId, int aintBeneficiaryPersonId)
        {
            DataTable ldtbList = Select<cdoRelationship>(
               new string[] { enmRelationship.person_id.ToString(), enmRelationship.beneficiary_person_id.ToString(), 
                                enmRelationship.relationship_value.ToString(), enmRelationship.effective_end_date.ToString() },
               new object[] { aintPersonId, aintBeneficiaryPersonId, busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE, DateTime.MinValue }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                busRelationship lbusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                lbusRelationship.icdoRelationship.LoadData(ldtbList.Rows[0]);
                return lbusRelationship;
            }
            return null;
        }

        public int InsertPersonRelationship(int aintPersonID, int aintBenPersonID, int aintDepPersonID, string astrRelation, 
                                string astrAddrFlag,DateTime adtMarriageDate,string aintBeneficiaryFrom,int aintBeneficiaryOf, 
                                DateTime adtStartDate, DateTime adtEndDate, int aintBenOrgID = 0)
        {
            if (icdoRelationship == null)
            {
                icdoRelationship = new cdoRelationship();               
            }
            icdoRelationship.person_id = aintPersonID;
            icdoRelationship.beneficiary_person_id = aintBenPersonID;
            icdoRelationship.dependent_person_id = aintDepPersonID;
            icdoRelationship.relationship_id = busConstant.BENEFICIARY_RELATIONSHIP_CODE_ID;
            icdoRelationship.relationship_value = astrRelation;
            icdoRelationship.effective_start_date = adtStartDate;
            icdoRelationship.effective_end_date = adtEndDate;
            icdoRelationship.addr_same_as_participant_flag = astrAddrFlag;
            icdoRelationship.date_of_marriage = adtMarriageDate;
            icdoRelationship.beneficiary_from_id = busConstant.BENEFICIARY_FORM_ID;
            icdoRelationship.beneficiary_from_value = aintBeneficiaryFrom;
            icdoRelationship.beneficiary_of = aintBeneficiaryOf;
            icdoRelationship.Insert();

            return icdoRelationship.person_relationship_id;
        }

        #endregion

        #region Public Overriden Methods
        
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPersonAccountBeneficiary)
            {
                busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjBus;
                lbusPersonAccountBeneficiary.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccountBeneficiary.ibusPerson.icdoPerson.LoadData(adtrRow);
            }
            base.LoadOtherObjects(adtrRow, aobjBus);

        }

        public override bool ValidateDelete()
        {
            return base.ValidateDelete();
        }

        public override int Delete()
        {
            return base.Delete();
        }

        #endregion
    }
}
