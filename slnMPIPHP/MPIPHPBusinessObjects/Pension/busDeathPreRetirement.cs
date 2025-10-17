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
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busDeathPreRetirement : busBenefitApplication
    {
        public decimal idecAgeatDeath { get; set; }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            this.icdoBenefitApplication.person_id = this.ibusPerson.icdoPerson.person_id;

            //this.ibusPerson.iclbNotes.ForEach(item =>
            //{
            //    if (item.icdoNotes.person_id == 0)
            //        item.icdoNotes.person_id = this.icdoBenefitApplication.person_id;
            //    item.icdoNotes.form_id = busConstant.Form_ID;
            ////    item.icdoNotes.form_value = busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM;
            //});
        }

        //Code-Abhishek
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (this.icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
                {
                    this.LoadandProcessWorkHistory_ForAllPlans();
                }
                this.CheckIfQualifiedSpouseinDeath();
                this.DetermineVesting();
                SetupPrerequisitesDeath();
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Code-Abhishek

        public Collection<cdoPerson> GetAllPersonBeneficaryOfParticipantPreRetirementDeath(int aintPlanId)
        {
            Collection<cdoPerson> iclbParticipantRelationship = new Collection<cdoPerson>();
            DataTable ldtblist = new DataTable();
            if (this.QualifiedSpouseExists)
            {
                if (aintPlanId == busConstant.LOCAL_700_PLAN_ID)
                {
                    ldtblist = busBase.Select("cdoBenefitApplication.GetBeneficiaryforLocal700PersonDeathPreRetirement", new object[2] { this.ibusPerson.icdoPerson.person_id,aintPlanId });
                }
                else
                {
                    ldtblist = busBase.Select("cdoBenefitApplication.GetBeneficiaryforPersonDeathPreRetirement", new object[2] { this.ibusPerson.icdoPerson.person_id,aintPlanId });
                }
            }
            else //This List not only meant for IAP but for all plans.
                ldtblist = busBase.Select("cdoBenefitApplication.GetBeneficiariesforIAPPreDeath", new object[2] { this.icdoBenefitApplication.person_id, aintPlanId });
            
            if (ldtblist.Rows.Count > 0)
            {
                iclbParticipantRelationship = doBase.GetCollection<cdoPerson>(ldtblist);
            }
            return iclbParticipantRelationship;

        }

        public Collection<cdoOrganization> GetAllOrgBeneficaryOfParticipantabc(int person_id)
        {
            busBenefitApplicationDetail objbusBenefitApplicationDetail = new busBenefitApplicationDetail();
            return objbusBenefitApplicationDetail.GetAllOrgBeneficaryOfParticipant(this.ibusPerson.icdoPerson.person_id);
        }

        /*//Code-Abhishek
        private void SetupPrerequisites()
        {
            if (!this.ibusPerson.iclbPersonAccount.IsNullOrEmpty())
            {
                this.DetermineVesting();
                //TODO MORE NEEDS TO COME HERE TO SHOW ACCOUNT BALANCE AND THINGS IN BENEFIT CALCULATION
                DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.GetLatestDateofMarriageIfQualifiedSpouseExists", new object[1] { this.ibusPerson.icdoPerson.person_id });
                if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
                {
                    int Spouse_Person_ID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                    int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { Spouse_Person_ID, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                    if (QualifiedDROExists == 0)
                    {
                        this.QualifiedSpouseExists = true;
                    }
                }
                
                this.DetermineBenefitSubTypeandEligibility_DeathPreRetirement();

                if (this.QualifiedSpouseExists)
                {
                    DataTable ldtbBenAccounts = busBase.Select("cdoPersonAccountBeneficiary.GetPlanfoLatestrQualifiedSpouse", new object[1] { this.ibusPerson.icdoPerson.person_id });
                    if (ldtbBenAccounts.Rows.Count > 0)
                    {
                        lclbBenPlans = cdoPlan.GetCollection<cdoPlan>(ldtbBenAccounts);

                        if (Eligible_Plans.Count > lclbBenPlans.Count)
                            this.SurvivorEntitled4MorePlans = true;
                        else
                            this.SurvivorEntitled4MorePlans = false;
                    }
                }
                                    
                //ELSE CONDITION WILL COME HERE THAT IF NO QUALIFIED SPOUSE THEN    

            }
        }
        //Code-Abhishek*/
       
        public void GetAgeAtDeath()
        {
            if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                idecAgeatDeath = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.ibusPerson.icdoPerson.date_of_death);
            else
                idecAgeatDeath = 0;
        }

        public bool IsSubPlan()
        {
            DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.GetLatestDateofMarriageIfQualifiedSpouseExists", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
            {
                int Spouse_Person_ID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { Spouse_Person_ID, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                if (QualifiedDROExists == 0)
                {
                    this.QualifiedSpouseExists = true;
                }  
            }

            if (this.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_NO || QualifiedSpouseExists)
            {
                return true;
            }

           return false;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (this.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
            {
                lobjError = AddError(5086, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            //rohan
            DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirement", new object[1] { this.icdoBenefitApplication.person_id });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0 && this.iclbPayeeAccount.IsNullOrEmpty())
            {
                int lintCount = 0;
                lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    if (iclbPayeeAccount.IsNullOrEmpty())
                    {
                        //Ticket : 50555
                        object lobjMdWithNoIAP = null;
                        lobjMdWithNoIAP = DBFunction.DBExecuteScalar("cdoBenefitApplication.CheckPayeeAccountMinimumDistributionRetirementType", new object[1] { this.icdoBenefitApplication.person_id },
                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        int lintMdWithNoIAP = 0;
                        if (lobjMdWithNoIAP != null)
                        {
                            lintMdWithNoIAP = ((int)lobjMdWithNoIAP);
                        }

                        if (lintMdWithNoIAP <= 0)
                        {
                            lobjError = AddError(5144, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                       
                    }
                }
            }
            //wasim - SSN is required.
            if (this.ibusPerson.icdoPerson.ssn.IsNullOrEmpty())
            {
                lobjError = AddError(6197, "");
                this.iarrErrors.Add(lobjError);
                return;
            }

            DataTable ldtbPayeeAccountStatus = busBase.Select("cdoBenefitApplication.GetPayeeAccountInRecievingStatus", new object[1] { this.ibusPerson.icdoPerson.person_id });
            foreach (busBenefitApplicationDetail lbusBenefitApplication in this.iclbBenefitApplicationDetail)
            {
                //PIR-810
                if (lbusBenefitApplication.icdoBenefitApplicationDetail.istrRelationShip == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE_DESCRIPTION)
                {
                    busPerson lbojPerson = new busPerson();
                    if (lbojPerson.FindPerson(lbusBenefitApplication.icdoBenefitApplicationDetail.survivor_id))
                    {
                        if (lbojPerson.icdoPerson.ssn.IsNullOrEmpty())
                        {
                            lobjError = AddError(6199, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                        if (lbojPerson.icdoPerson.date_of_birth == DateTime.MinValue) //PIR-810
                        {
                            lobjError = AddError(6235, "");
                            this.iarrErrors.Add(lobjError);
                            return;
                        }
                    }
                }

                if (ldtbPayeeAccountStatus != null && ldtbPayeeAccountStatus.Rows.Count > 0)
                {
                    if (ldtbPayeeAccountStatus.AsEnumerable().Where(ldtRow =>  Convert.ToString(ldtRow["iintPlanId"]) == Convert.ToString(lbusBenefitApplication.iintPlan_ID)).Count() > 0)
                    {
                        //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
                        //DataTable ldtbMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5505);
                        //string lstrMessage = ldtbMessageInfo.Rows[0]["display_message"].ToString();
                        utlMessageInfo lobjutlMessageInfo = iobjPassInfo.isrvDBCache.GetMessageInfo(5505);
                        string lstrMessage = lobjutlMessageInfo.display_message;
                        lobjError = AddError(0, string.Format(lstrMessage, lbusBenefitApplication.istrPlanCode));
                        this.iarrErrors.Add(lobjError);

                    }
                }
            }

            if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
            {
                lobjError = AddError(5170, "");
                this.iarrErrors.Add(lobjError);
            }

            if ((icdoBenefitApplication.retirement_date != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                && icdoBenefitApplication.retirement_date <= this.ibusPerson.icdoPerson.date_of_death)
            {
                lobjError = AddError(5164, "");
                this.iarrErrors.Add(lobjError);
            }


            if ((icdoBenefitApplication.application_received_date != DateTime.MinValue && this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue) 
                && icdoBenefitApplication.application_received_date < this.ibusPerson.icdoPerson.date_of_death)
            {
                lobjError = AddError(5165, "");
                this.iarrErrors.Add(lobjError);
            }
           

            // As per PIR - 129 , Retirement Date for Deaths can be past or future date
            //if (icdoBenefitApplication.retirement_date != DateTime.MinValue && icdoBenefitApplication.retirement_date < DateTime.Today)
            //{
            //    if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            //    {
            //        lobjError = AddError(5028, " ");
            //        this.iarrErrors.Add(lobjError);
            //    }
            //    else if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //    {
            //        if (icdoBenefitApplication.retirement_date != Convert.ToDateTime(icdoBenefitApplication.ihstOldValues[enmBenefitApplication.retirement_date.ToString()]))
            //        {
            //            lobjError = AddError(5028, " ");
            //            this.iarrErrors.Add(lobjError);
            //        }
            //    }
            //}


            //if (Eligible_Plans.Contains(busConstant.MPIPP) && Eligible_Plans.Contains(busConstant.IAP) && this.iclbBenefitApplicationDetail.Count > 0)
            //{
            //    int count = 0;
            //    foreach (busBenefitApplicationDetail item in this.iclbBenefitApplicationDetail)
            //    {
            //        if (item.iintPlan_ID == busConstant.IAP_PLAN_ID || item.iintPlan_ID == busConstant.MPIPP_PLAN_ID) //Plan ID is Hard-Coded here do not have any option
            //        {
            //            count++;
            //        }
            //    }
            //    if (count > 0 && count < 2)
            //    {
            //        lobjError = AddError(5133, "");
            //        this.iarrErrors.Add(lobjError);
            //    }
            //}

            if (this.NotEligible.IsNotNull() && this.NotEligible)
            {
                lobjError = AddError(5155, " ");
                this.iarrErrors.Add(lobjError);
            }
            this.EvaluateInitialLoadRules();
            base.ValidateHardErrors(aenmPageMode);
            

            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {
                Hashtable lhstParams = new Hashtable();
                lhstParams["iintPlan_ID"] = lbusBenefitApplicationDetail.iintPlan_ID;
                lhstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id;
                lhstParams["icdoBenefitApplicationDetail.survivor_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id;
                lhstParams["icdoBenefitApplicationDetail.organization_id"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id;
                lhstParams["istrSubPlan"] = lbusBenefitApplicationDetail.istrSubPlan;

                if (string.IsNullOrEmpty(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue))
                {
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                    {
                        lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"] = busConstant.SURVIVOR_TYPE_PERSON;
                    }
                    else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                    {
                        lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"] = busConstant.SURVIVOR_TYPE_ORG;
                    }
                }
                else
                {
                    lhstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue;
                }

                lhstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue;


                this.iarrErrors = CheckErrorOnAddButtonDeath(this, lhstParams, ref iarrErrors, true);
            }
        }
      
        public ArrayList CheckErrorOnAddButtonDeath(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            busDeathPreRetirement lbusDeathPreRetirement = aobj as busDeathPreRetirement;
            string lstrSubPlan = string.Empty;
            string lstrPlanCode = string.Empty;
            lstrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);

            if (lstrSubPlan == string.Empty)
            {
                lstrSubPlan = null;
            }

            if(Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"])==string.Empty)
            {
                lobjError = AddError(5100, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            else
            {
                if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON
                    && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.survivor_id"]) == string.Empty)
                {
                    lobjError = AddError(5100, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                else if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_ORG
                   && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.organization_id"]) == string.Empty)
                {
                    lobjError = AddError(5100, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            
            
            if (Convert.ToString(ahstParams["iintPlan_ID"]) == "")
            {
                lobjError = AddError(5050, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == string.Empty && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"]) == string.Empty)
            {
                lobjError = AddError(5051, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            int lintPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]);
            busPlan lbusPaln = new busPlan();
            lbusPaln.FindPlan(lintPlanID);
            lstrPlanCode = lbusPaln.icdoPlan.plan_code;

            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON)
            {
                ahstParams["icdoBenefitApplicationDetail.organization_id"] = "";
            }
            else if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_ORG)
            {
                ahstParams["icdoBenefitApplicationDetail.survivor_id"] = "";
            }

            //ROHAN :temporarily commented out ,don not delete

            int lSurvivorID = 0;
            int lOrganizationID = 0;
            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.survivor_id"]).IsNullOrEmpty())
                lSurvivorID = 0;
            else
                lSurvivorID = Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.survivor_id"]);

            if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.organization_id"]).IsNullOrEmpty())
                lOrganizationID = 0;
            else
                lOrganizationID = Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.organization_id"]);

                DataTable  ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedPreDeathForGivenPlan",
                   new object[6] { lstrSubPlan,this.icdoBenefitApplication.person_id, lintPlanID, this.icdoBenefitApplication.benefit_application_id, lSurvivorID, lOrganizationID });
            
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    lobjError = AddError(5463, "");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }

            if (CheckDuplicatePlan(lbusDeathPreRetirement, lintPlanID, lstrSubPlan,lSurvivorID ,ablnHardError))   // Plan cannot be duplicated.
            {
                bool lbnlFlag = busConstant.BOOL_FALSE;
                foreach (utlError lError in aarrErrors)
                {
                    if (lError.istrErrorID == "5023")
                    {
                        lbnlFlag = busConstant.BOOL_TRUE;
                        break;
                    }
                }
                if (!lbnlFlag)
                {
                    lobjError = AddError(5023, "");
                    aarrErrors.Add(lobjError);
                }
            }

           
                      
            //if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID && lstrSubPlan.IsNullOrEmpty() && !iblnEligbile4MPIBenefitPreDeath)
            //{
            //    lobjError = AddError(5424, "");
            //    aarrErrors.Add(lobjError);        
            //}
                        if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON && this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            DataTable ldtbLatestSpouseInfo = busBase.Select("cdoRelationship.CheckIfQualifiedSpouseExists", new object[3] { this.ibusPerson.icdoPerson.person_id, Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.survivor_id"]), this.ibusPerson.icdoPerson.date_of_death });
                            if (ldtbLatestSpouseInfo.Rows.Count > 0 && ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
                            {
                                int Spouse_Person_ID = Convert.ToInt32(ldtbLatestSpouseInfo.Rows[0][enmRelationship.beneficiary_person_id.ToString()]);

                                int QualifiedDROExists = (int)DBFunction.DBExecuteScalar("cdoDroApplication.CheckCountofApprovedDROforPersonandPayee", new object[2] { Spouse_Person_ID, this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                //IF COUNT query Returns 1 then we know there is a DRO and hence no Qualified Spouse Exists
                                if (QualifiedDROExists == 0)
                                {
                                    this.QualifiedSpouseExistsForPlan = true;
                                }
                            }
                            else
                            {
                                this.QualifiedSpouseExistsForPlan = false;
                            }
                        }

            if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_161_PLAN_ID && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON && (this.CheckAlreadyVested(busConstant.Local_161) == false || this.QualifiedSpouseExistsForPlan == false))
            {
                lobjError = AddError(5429, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_700_PLAN_ID && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON && (this.CheckAlreadyVested(busConstant.LOCAL_700) == false || this.QualifiedSpouseExistsForPlan == false))
            {
                lobjError = AddError(5429, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID && (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_ORG || !this.QualifiedSpouseExistsForPlan)
                && Convert.ToString(ahstParams["istrSubPlan"]).IsNullOrEmpty())
            {
                lobjError = AddError(5451, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.IAP_PLAN_ID && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_ORG
                && this.QualifiedSpouseExistsForPlan == false && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY)
            {
                lobjError = AddError(5450, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            bool lblnEligibleForAnnuity = true;
            if(Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY
                && Convert.ToString(ahstParams["istrSubPlan"]) == "" && Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON)
            {
                if(!iblnEligbile4MPIBenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.MPIPP_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
                if (!iblnEligbile4L161BenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_161_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
                if (!iblnEligbile4L600BenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_600_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
                if (!iblnEligbile4L666BenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_666_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
                if (!iblnEligbile4L700BenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_700_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
                if (!iblnEligible4L52BenefitPreDeath && Convert.ToInt32(ahstParams["iintPlan_ID"]) == busConstant.LOCAL_700_PLAN_ID)
                {
                    lblnEligibleForAnnuity = false;
                }
            }

           if(!lblnEligibleForAnnuity)
            {
                lobjError = AddError(5429, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            return aarrErrors;
        }


        public override bool ValidateSoftErrors()
        {

            return base.ValidateSoftErrors();
        }

        public Collection<cdoPlan> GetPlanValues()
        {
            string lstrFinalQuery = string.Empty;
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            if (!Eligible_Plans.IsNullOrEmpty())
            {
                StringBuilder xyz = new StringBuilder();

                foreach (string plan_code in Eligible_Plans)
                {
                    if (!string.IsNullOrEmpty(xyz.ToString()))
                        xyz.Append(",");
                    xyz.Append("'" + plan_code + "'");
                }

                lstrFinalQuery = "select * from sgt_plan where plan_code in (" + xyz + ")";

                DataTable ldtbListofPLans = DBFunction.DBSelect(lstrFinalQuery, iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
                if (ldtbListofPLans.Rows.Count > 0)
                {
                    lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbListofPLans);
                }
            }

            return lColPlans;     
        }


        public ArrayList btn_CalculateDeathBenefit()
        {
            ArrayList iarrErrors = new ArrayList();
           
            //Tuple lTuple = new Tuple<int, int, bool>();
            //Call Eligibility Yet Again the Final Time Just Before doing Final Calculation
            List<Tuple<int, int>> listaTuple = new List<Tuple<int, int>>();
            this.CheckIfQualifiedSpouseinDeath();
            this.DetermineVesting();



            int lintMPIPPHeaderId = 0;
            int lintIAPHeaderId = 0;


            foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in this.iclbBenefitApplicationDetail)
            {
                #region Initialize Calculation Needed Objects
                GetPayeeAccountsInApprovedOrReviewSatus();
                bool lblnPlanCalculated = false;
                Tuple<int, int> lt = new Tuple<int, int>(lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id);
                if (!listaTuple.Contains(lt))
                {
                    listaTuple.Add(lt);
                }
                else
                {
                    lblnPlanCalculated = true;
                }
                DateTime ldtBenefitComDate = this.icdoBenefitApplication.retirement_date;
                busBenefitCalculationPreRetirementDeath lbusBenefitCalculationPreRetirementDeath = new busBenefitCalculationPreRetirementDeath { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication = this;
                lbusBenefitCalculationPreRetirementDeath.ibusPerson = this.ibusPerson;
                lbusBenefitCalculationPreRetirementDeath.ibusPerson.iclbPersonAccount = this.ibusPerson.iclbPersonAccount;
                if (this.iclbPayeeAccount.Where(item => item.icdoPayeeAccount.iintPlanId == lbusBenefitApplicationDetail.iintPlan_ID && (item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT || item.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY)).Count() > 0)
                {
                    lbusBenefitCalculationPreRetirementDeath.iblnCheckIfPreRetPostElection = true;
                }
                if (this.idecAge == 0)
                {
                    this.idecAge = busGlobalFunctions.CalculatePersonAge(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);
                }

                if (lbusBenefitCalculationPreRetirementDeath.ibusCalculation.IsNull())
                {
                    lbusBenefitCalculationPreRetirementDeath.ibusCalculation = new busCalculation();
                }
                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.age = busGlobalFunctions.CalculatePersonAgeInDec(this.ibusPerson.icdoPerson.idtDateofBirth, this.icdoBenefitApplication.retirement_date);                
                lbusBenefitCalculationPreRetirementDeath.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();
                lbusBenefitCalculationPreRetirementDeath.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                //lbusBenefitCalculationPreRetirementDeath.LoadAllRetirementContributions();

                if (!ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).IsNullOrEmpty())
                {
                    lbusBenefitCalculationPreRetirementDeath.LoadAllRetirementContributions(ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault());
                }
                else
                {
                    lbusBenefitCalculationPreRetirementDeath.LoadAllRetirementContributions(null);
                }


                #endregion
               
                if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                {
                    #region IAP PLAN FOUND IN GRID
                    bool lblnVested = this.CheckAlreadyVested(busConstant.IAP);
                    if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty())
                    {
                        lbusBenefitCalculationPreRetirementDeath.iblnCalculateIAPBenefit = true;
                    }
                    else if (lbusBenefitApplicationDetail.istrSubPlan == busConstant.L52_SPL_ACC && lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                    {
                        lbusBenefitCalculationPreRetirementDeath.iblnCalculateL52SplAccBenefit = true;
                    }
                    else if (lbusBenefitApplicationDetail.istrSubPlan == busConstant.L161_SPL_ACC && lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                    {
                        lbusBenefitCalculationPreRetirementDeath.iblnCalculateL161SplAccBenefit = true;
                    }

                    #region Setting Up Header for IAP
                    if (!lblnPlanCalculated)
                    {
                        lbusBenefitCalculationPreRetirementDeath.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id,
                                                                                     busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                                                     this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id,lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage);
                    }
                    else
                    {
                        if (lbusBenefitCalculationPreRetirementDeath.FindBenefitCalculationHeader(lintIAPHeaderId))
                        {
                            if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_id != 0)
                            {
                                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_PER;
                            }
                            else
                            {
                                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORG;
                            }
                            lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                            lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                        }
                    }
                    #endregion

                    #endregion
                }

                else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                {
                    if (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N")
                    {

                        #region MPIPP PLAN FOUNd in GRID
                        bool lblnVested = this.CheckAlreadyVested(busConstant.MPIPP);

                        if (lbusBenefitApplicationDetail.istrSubPlan.IsNullOrEmpty())
                        {
                            lbusBenefitCalculationPreRetirementDeath.iblnCalculateMPIPPBenefit = true;
                        }
                        else if (lbusBenefitApplicationDetail.istrSubPlan == busConstant.EE_UVHP && lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                        {
                            lbusBenefitCalculationPreRetirementDeath.iblnCalcualteUVHPBenefit = true;
                            lbusBenefitCalculationPreRetirementDeath.iblnCalcualteNonVestedEEBenefit = true;
                        }

                        #region Setting Up Header for MPIPP
                        if (!lblnPlanCalculated)
                        {
                            lbusBenefitCalculationPreRetirementDeath.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id,
                                                                                         busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                                                         this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage);
                        }
                        else
                        {
                            if (lbusBenefitCalculationPreRetirementDeath.FindBenefitCalculationHeader(lintMPIPPHeaderId))
                            {
                                if (lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_id != 0)
                                {
                                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_PER;
                                }
                                else
                                {
                                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrSurvivorTypeValue = busConstant.SURVIVOR_TYPE_ORG;
                                }

                                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintPlanId = lbusBenefitApplicationDetail.iintPlan_ID;
                                lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.iintSurvivorAgeAtRetirement = busGlobalFunctions.CalculatePersonAgeInDec(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.beneficiary_person_date_of_birth, this.icdoBenefitApplication.retirement_date);
                            }
                        }

                        #endregion
                        #endregion
                    }
                }

                else
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                    {

                        #region Local Plans Found
                        lbusBenefitCalculationPreRetirementDeath.PopulateInitialDataBenefitCalculationHeader(this.ibusPerson.icdoPerson.person_id, this.icdoBenefitApplication.benefit_application_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id,
                                                busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT, busConstant.BenefitCalculation.CALCULATION_TYPE_FINAL, this.icdoBenefitApplication.retirement_date,
                                                this.idecAge, lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage);
                        #endregion
                    }

                }
                //Load Earliest Retirement Date
                if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                {
                    lbusBenefitCalculationPreRetirementDeath.LoadPreRetirementDeathInitialData();
                    lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = ldtBenefitComDate;
                    lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_commencement_date = ldtBenefitComDate;
                    lbusBenefitCalculationPreRetirementDeath.SetUpVariablesForDeath(this.icdoBenefitApplication.retirement_date);



                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                    }
                    string lstrBenefitSubtype = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                    if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.LUMP_SUM)
                    {
                        if (!string.IsNullOrEmpty(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrEarliestRetirementType))
                        {
                            lstrBenefitSubtype = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrEarliestRetirementType;
                        }
                    }
                    lbusBenefitCalculationPreRetirementDeath.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                     this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                     lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                     lstrBenefitSubtype, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);

                }else
                {
                    if(lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                    {
                        lbusBenefitCalculationPreRetirementDeath.LoadPreRetirementDeathInitialData();
                        lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.retirement_date = ldtBenefitComDate;
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_commencement_date = ldtBenefitComDate;
                        lbusBenefitCalculationPreRetirementDeath.SetUpVariablesForDeath(this.icdoBenefitApplication.retirement_date);



                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty() && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue.IsNotNullOrEmpty())
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                        }
                        string lstrBenefitSubtype = this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.istrRetirementSubType;
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue == busConstant.LUMP_SUM)
                        {
                            if (!string.IsNullOrEmpty(lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrEarliestRetirementType))
                            {
                                lstrBenefitSubtype = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.istrEarliestRetirementType;
                            }
                        }
                        lbusBenefitCalculationPreRetirementDeath.SpawnFinalRetirementCalculation(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id,
                                                                                         this.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == lbusBenefitApplicationDetail.istrPlanCode).FirstOrDefault().icdoPersonAccount.person_account_id,
                                                                                         lbusBenefitApplicationDetail.iintPlan_ID, lbusBenefitApplicationDetail.istrPlanCode, this.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date,
                                                                                         lstrBenefitSubtype, lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue);


                    }
                }
                
                try
                {
                    if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID && !lblnPlanCalculated)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                        lbusBenefitCalculationPreRetirementDeath.PersistChanges();
                        lintIAPHeaderId = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    }

                    else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && !lblnPlanCalculated && (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                        lbusBenefitCalculationPreRetirementDeath.PersistChanges();
                        lintMPIPPHeaderId = lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id;
                        //lblnMPIPPCalculated = true;
                    }

                    else if (lbusBenefitApplicationDetail.iintPlan_ID != busConstant.IAP_PLAN_ID && lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                    {
                        lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.ienuObjectState = ObjectState.Insert;
                        lbusBenefitCalculationPreRetirementDeath.PersistChanges();

                    }
                    if(lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID && (lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == null || lbusBenefitCalculationPreRetirementDeath.ibusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag == "N"))
                    {
                        lbusBenefitCalculationPreRetirementDeath.AfterPersistChanges();
                        SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                    }else
                    {
                        if(lbusBenefitApplicationDetail.iintPlan_ID != busConstant.MPIPP_PLAN_ID)
                        lbusBenefitCalculationPreRetirementDeath.AfterPersistChanges();
                        SetWorkflowRelatedVariablesforFinalCalculation(lbusBenefitApplicationDetail.istrPlanCode, lbusBenefitCalculationPreRetirementDeath.icdoBenefitCalculationHeader.benefit_calculation_header_id);
                    }
                    
                }
                catch
                {
                }
            }

            if (this.ibusBaseActivityInstance.IsNotNull())
            {
                this.SetProcessInstanceParameters();
            }
            this.icdoBenefitApplication.final_calc_flag = busConstant.FLAG_YES;
            this.icdoBenefitApplication.change_benefit_option_flag = busConstant.FLAG_NO;
            this.icdoBenefitApplication.Update();
            this.EvaluateInitialLoadRules();
            iarrErrors.Add(this);

            return iarrErrors;
        }

    }
}
