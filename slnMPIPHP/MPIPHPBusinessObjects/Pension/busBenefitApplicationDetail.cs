#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busBenefitApplicationDetail:
    /// Inherited from busBenefitApplicationDetailGen, the class is used to customize the business object busBenefitApplicationDetailGen.
    /// </summary>
    [Serializable]
    public class busBenefitApplicationDetail : busBenefitApplicationDetailGen
    {
        public int iintPlan_ID { get; set; }
        public string istrPlanName { get; set; }
        public string istrPlanCode { get; set; }
        public string istrPlanBenefitDescription { get; set; }
        public string istrJointAnnunantMpid { get; set; }

        public string istrSubPlan { get; set; }
        public string istrSubPlanDescription { get; set; }      
     
        public Collection<cdoPerson> GetAllPersonBeneficaryOfParticipant(int person_id)
        {
            Collection<cdoPerson> iclbParticipantRelationship = null;

            DataTable ldtblist = busBase.Select("cdoBenefitApplication.GetBeneficaryofPerson", new object[1] { person_id });
            if (ldtblist.Rows.Count > 0)
            {
                iclbParticipantRelationship = doBase.GetCollection<cdoPerson>(ldtblist);
            }
            return iclbParticipantRelationship;
        }
        public Collection<cdoOrganization> GetAllOrgBeneficaryOfParticipantabc(int person_id)
        {
            busBenefitApplicationDetail objbusBenefitApplicationDetail = new busBenefitApplicationDetail();
            return objbusBenefitApplicationDetail.GetAllOrgBeneficaryOfParticipant(person_id);
        }
        public Collection<cdoOrganization> GetAllOrgBeneficaryOfParticipant(int person_id)
        {
            Collection<cdoOrganization> iclbOrganisationRelationship = null;

            DataTable ldtblist = busBase.Select("cdoBenefitApplication.GetAllOrgBeneficaryOfPersons", new object[1] { person_id });
            if (ldtblist.Rows.Count > 0)
            {
                iclbOrganisationRelationship = doBase.GetCollection<cdoOrganization>(ldtblist);
            }

            return iclbOrganisationRelationship;
        }


        public void btn_BenefitCalculation()
        {
            if (this.ibusBenefitApplication.idecAge == 0 || this.ibusBenefitApplication.idecAge.IsNull())
            {
                GetAgeAtRetirement();
            }

            //string lstrPlanCode = null; // Get Plan Code from PlanId

            busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader();
            lbusBenefitCalculationHeader.CalculateBenefit(this.ibusBenefitApplication.icdoBenefitApplication.person_id, this.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id,
                                                                     this.icdoBenefitApplicationDetail.survivor_id, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.ibusBenefitApplication.icdoBenefitApplication.benefit_type_value,
                                                                     busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING, this.ibusBenefitApplication.icdoBenefitApplication.retirement_date,
                                                                     this.ibusBenefitApplication.idecAge, this.ibusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value,
                                                                     this.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                     busConstant.ZERO_INT, this.ibusPlanBenefitXr.icdoPlanBenefitXr.plan_id, System.DateTime.MinValue,
                                                                     this.icdoBenefitApplicationDetail.benefit_subtype_value);
        }

        public void GetAgeAtRetirement()
        {
            DateTime ldtRetire = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
            this.ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, ldtRetire);
        }

        public Collection<cdoPlan> GetPlanValues()
        {
            busWithdrawalApplication lbusWithdrawalApplication = new busWithdrawalApplication();
            return lbusWithdrawalApplication.GetPlanValues();
        }


    }
}
