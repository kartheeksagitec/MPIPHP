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
using Sagitec.CustomDataObjects;
using System.Text.RegularExpressions;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busRelationship:
    /// Inherited from busRelationshipGen, the class is used to customize the business object busRelationshipGen.
    /// </summary>
    [Serializable]
    public class busPersonBeneficiary : busRelationship
    {
        # region properties

        public busPerson ibusParticipantBeneficiary { get; set; }
        public busPersonAccount ibusPersonAccount { get; set; }
        public busOrganization ibusOrganization { get; set; }

        /// <summary>
        /// To Push the data in sgt_person_account for beneficiary when adding beneficiary of beneficary(BOB).
        /// </summary>
        public Collection<busPersonAccount> iclbPersonAccount { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPersonAccountBeneficiary. 
        /// </summary>
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiaryOf { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiariesAll { get; set; }
        public Collection<busError> iclbBenfErrors { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbBeneficiaries { get; set; }

        public string istrBenType { get; set; }

        public bool iblnIsBenTypeOldValueIsDiff { get; set; }

        public bool iblnIsTrue { get; set; }
        public bool iblnIsPlan { get; set; }
        public bool iblnIsPlan1 { get; set; }
        public bool iblnDeleteHealthBen { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }
    
        public long iintAPrimaryKey { get; set; }
        #endregion


        #region public

        /// <summary>
        /// Load all person accounts for the beneficiary
        /// </summary>
        /// 

        //public void LoadPersonAccountBeneficiary()
        //{
        //    DataTable ldtbBeneficiary = Select<cdoPersonAccountBeneficiary>(
        //       new string[1] { enmPersonAccountBeneficiary.beneficiary_id.ToString() },
        //       new object[1] { icdoRelationship.relationship_id }, null, null);

        //    iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbBeneficiary, "icdoPersonAccountBeneficiary");
        //}

        /// <summary>
        /// Load all beneficiaries for the participant
        /// </summary>
        public void LoadAllPersonAccountBeneficiaries()
        {
            DataTable ldtbAllBeneficiaries = Select("cdoPersonAccountBeneficiary.LoadAllBeneficiaries",
                new object[2] { ibusPerson.icdoPerson.person_id, this.icdoRelationship.person_relationship_id });

            iclbPersonAccountBeneficiariesAll = GetCollection<busPersonAccountBeneficiary>(ldtbAllBeneficiaries, "icdoPersonAccountBeneficiary");
            //int i = 1;
            foreach (busPersonAccountBeneficiary lbuspersonac in iclbPersonAccountBeneficiariesAll)
            {
                lbuspersonac.ibusParticipant = this.ibusPerson;
                //if (lbuspersonac.ibusRelationship.IsNull() || lbuspersonac.ibusRelationship.icdoRelationship.IsNull())
                //    lbuspersonac.ibusRelationship = new busRelationship() { icdoRelationship = new cdoRelationship() };
                //lbuspersonac.ibusRelationship.icdoRelationship.iintAPrimaryKey = i++;
            }

        }

        public Collection<cdoPerson> FillBeneficiaryOfDetails(string astrBenFromValue)
        {
            //Only when person is a participant
            Collection<cdoPerson> lclcPerson = new Collection<cdoPerson>();
            DataTable ldtbPersonList = new DataTable();
            if (astrBenFromValue == busConstant.PERSON_TYPE_ALTERNATE_PAYEE)
            {
                ldtbPersonList = Select("cdoRelationship.GetAlternatePayeePartDetails", new object[1] { this.ibusPerson.icdoPerson.person_id });
                lclcPerson = doBase.GetCollection<cdoPerson>(ldtbPersonList);
            }
            else if (astrBenFromValue == busConstant.PERSON_TYPE_SURVIVOR)
            {
                ldtbPersonList = Select("cdoRelationship.GetSurvivorsPartDetails", new object[1] { this.ibusPerson.icdoPerson.person_id });
                lclcPerson = doBase.GetCollection<cdoPerson>(ldtbPersonList);
            }
            return lclcPerson;
        }
        public Collection<cdoPlan> GetPlanValues(string astrPersonType)
        {
            Collection<cdoPlan> lclcPlans = null;
            if (this.icdoRelationship != null && (astrPersonType == "SURV" || astrPersonType == "ALTP"))
            {
                DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlanForBOB", new object[2] { ibusPerson.icdoPerson.person_id, this.icdoRelationship.beneficiary_of });
                if (ldtbList.Rows.Count > 0)
                {
                    lclcPlans = doBase.GetCollection<cdoPlan>(ldtbList);
                }
            }
            else
            {
                DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlanForBeneficiary", new object[1] { ibusPerson.icdoPerson.person_id });
                if (ldtbList.Rows.Count > 0)
                {
                    lclcPlans = doBase.GetCollection<cdoPlan>(ldtbList);
                }
            }


            return lclcPlans;
        }

        public Collection<cdoCodeValue> GetRelationshipValues(string astrBenType)
        {
            Collection<cdoCodeValue> lclcValues = null;
            if (string.IsNullOrEmpty(astrBenType))
            {
                astrBenType = "PER";
            }
            DataTable ldtbList = Select("cdoRelationship.GetRelationshipValues", new object[1] { astrBenType });
            if (ldtbList.Rows.Count > 0)
            {
                lclcValues = doBase.LoadData<cdoCodeValue>(ldtbList);
            }
            return lclcValues;
        }

        public bool CheckBeneficiaryDOB()
        {
            DataTable ldtbDOB = Select("cdoRelationship.GetDependentDOB", new object[1] { this.icdoRelationship.beneficiary_person_id });

            if (ldtbDOB.Rows[0][enmPerson.date_of_birth.ToString()].ToString().IsNotNullOrEmpty())
            {
                string lstrDependentDOB = Convert.ToString(ldtbDOB.Rows[0][enmPerson.date_of_birth.ToString()]);
                DateTime ldtDependentDOB = Convert.ToDateTime(lstrDependentDOB);
                DateTime ldtPersonDOB = this.ibusPerson.icdoPerson.idtDateofBirth;

                if ((ldtPersonDOB.ToString().IsNotNullOrEmpty()) && ldtPersonDOB != DateTime.MinValue)
                {
                    if ((ldtDependentDOB != DateTime.MinValue) && (ldtDependentDOB <= ldtPersonDOB))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }



        /// <summary>
        ///   MPIPHP.busRelationshipGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(icdoRelationship.beneficiary_org_id);
        }

        /// <summary>
        ///   MPIPHP.busRelationshipGen.LoadPersonAccountBeneficiarys():
        /// Loads Collection object iclbPersonAccountBeneficiary of type busPersonAccountBeneficiary.
        /// </summary>
        public void LoadPersonAccountBeneficiarys()
        {
            DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
                new string[1] { enmPersonAccountBeneficiary.person_relationship_id.ToString() },
                new object[1] { icdoRelationship.person_relationship_id }, null, null);
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");

            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
            {
                lbusPersonAccountBeneficiary.iaintMainPersonID = this.ibusPerson.icdoPerson.person_id;
                lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue = this.icdoRelationship.beneficiary_from_value;
                lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintBeneficiaryOf = this.icdoRelationship.beneficiary_of;
                lbusPersonAccountBeneficiary.ibusParticipant = this.ibusPerson;
                DataTable ldtbPlan = Select("cdoPersonAccountBeneficiary.GetPlanFromAccountID", new object[1] { lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id });
                if (ldtbPlan.Rows.Count > 0)
                {
                    DataRow ldtrRow = ldtbPlan.Rows[0];
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan = Convert.ToString(ldtrRow[enmPlan.plan_name.ToString()]);
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan = Convert.ToInt32(ldtrRow[enmPlan.plan_id.ToString()]);
                }
            }
        }


        /// <summary>
        /// check if for the same beneficiary data already exists for same beneficiary type and plan type
        /// </summary>
        /// <returns></returns>
        /// 

     

        public bool CheckDuplicateBeneficiaryTypeandPlan()
        {
            ArrayList larrItems = new ArrayList();
            bool lblnCheckDuplicate = false;
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
            {
                string lstrPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan.ToString();
                string lstrBenType = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
                string strItem = lstrPlanID + ":" + lstrBenType;
                if (!larrItems.Contains(strItem))
                {
                    larrItems.Add(strItem);
                }
                else
                {
                    return true;
                }
            }
            return lblnCheckDuplicate;
        }

        public bool CheckDuplicatePlan()
        {
            ArrayList larrItems = new ArrayList();
            bool lblnCheckDuplicate = false;
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
            {
                int lintPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan;
                DateTime ldtEndDate = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date;
                if (!larrItems.Contains(lintPlanID))
                {
                    if (ldtEndDate == DateTime.MinValue || ldtEndDate > DateTime.Today)
                    {
                        larrItems.Add(lintPlanID);
                    }
                }
                else
                {
                    return true;
                }
            }
            return lblnCheckDuplicate;
        }

        public bool CheckEmptyRow()
        {
            bool blnCheckEmpty = false;
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
            {
                if (this.iclbPersonAccountBeneficiary.Count == 1)
                {
                    int lintPlan = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan;
                    if (lintPlan == 0)
                    {
                        return true;
                    }

                }

            }
            return blnCheckEmpty;
        }

        public bool IsPersonMerged()
        {
            DataTable ldtbResult = busBase.Select<cdoPerson>(
              new string[1] { enmPerson.person_id.ToString() },
              new object[1] { this.ibusPerson.icdoPerson.person_id }, null, null);
            if (ldtbResult.Rows.Count > 0)
                return false;
            else
                return true;
        }
        public bool IsNonNegative(string astrNumber)
        {
            bool lblnValidPercentage = false;
            Regex lrexGex = new Regex("^[0-9,.]*$");
            if (!lrexGex.IsMatch(astrNumber))
            {
                lblnValidPercentage = true;
            }
            return lblnValidPercentage;
        }

        public ArrayList ValidateNewChild(Hashtable ahstParams)
        {
            ArrayList larrErrors = new ArrayList();
            if (ahstParams.Count > 0)
            {
                utlError lobjError = null;
                int lintPlanId = 0;
                string lstrBenefitType = string.Empty;
                decimal ldecPercent = 0;
                DateTime ldtStrtTime = DateTime.MinValue;
                DateTime ldtEndTime = DateTime.MinValue;
                string lstrIsPrimary = string.Empty;
                DateTime ldtPrimaryBenDOD = DateTime.MinValue;
                ahstParams["icdoPersonAccountBeneficiary.iaintPlan"] =  ahstParams["iaintPlan"];
                ahstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"] =  ahstParams["beneficiary_type_value"];
                ahstParams["icdoPersonAccountBeneficiary.dist_percent"] = ahstParams["dist_percent"];
                ahstParams["icdoPersonAccountBeneficiary.start_date"] = ahstParams["start_date"];
                ahstParams["icdoPersonAccountBeneficiary.end_date"] = ahstParams["end_date"];
            
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.iaintPlan"])))
                {
                    lintPlanId = Convert.ToInt32(ahstParams["icdoPersonAccountBeneficiary.iaintPlan"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"])))
                {
                    lstrBenefitType = Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"]);
                    if (lstrBenefitType == "CONT")
                    {
                        DataTable ldtbPrimaryBen = Select("cdoPerson.GetPrimaryBenCount", new object[1] { this.ibusPerson.icdoPerson.person_id });
                        if (ldtbPrimaryBen.Rows.Count > 0)
                        {
                            iclbBeneficiaries = GetCollection<busPersonAccountBeneficiary>(ldtbPrimaryBen, "icdoPersonAccountBeneficiary");

                            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in iclbBeneficiaries)
                            {
                                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan == lintPlanId)
                                {
                                    iblnIsPlan = busConstant.BOOL_TRUE;
                                    #region PROD PIR 352
                                    //if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.idtBenDateOfDeath == DateTime.MinValue)
                                    //{
                                    //    lobjError = AddError(6206, "");
                                    //    larrErrors.Add(lobjError);
                                    //    break;
                                    //}
                                    #endregion
                                }
                            }
                            if (iblnIsPlan != busConstant.BOOL_TRUE)
                            {
                                lobjError = AddError(6205, "");
                                larrErrors.Add(lobjError);
                            }
                        }
                        else
                        {
                            lobjError = AddError(6205, "");
                            larrErrors.Add(lobjError);
                        }

                        //int lintPRIMCount = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPrimaryBenCount", new object[1] { this.icdoRelationship.person_id },
                        //                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                        //if (lintPRIMCount == 0)
                        //{
                        //    this.iarrErrors.Add(AddError(6205, " "));
                        //}
                    }
                }

                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.dist_percent"])))
                {
                    if (IsNonNegative(ahstParams["icdoPersonAccountBeneficiary.dist_percent"].ToString())) //Wasimk Pathan: For Check dist_percentage is Decimal 
                    {
                        lobjError = AddError(5059, "");
                        larrErrors.Add(lobjError);
                    }
                    else
                    {
                        ldecPercent = Convert.ToDecimal(ahstParams["icdoPersonAccountBeneficiary.dist_percent"]);
                    }
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.start_date"])))
                {
                    ldtStrtTime = Convert.ToDateTime(ahstParams["icdoPersonAccountBeneficiary.start_date"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.end_date"])))
                {
                    ldtEndTime = Convert.ToDateTime(ahstParams["icdoPersonAccountBeneficiary.end_date"]);
                }
                if (lintPlanId == 0)
                {
                    lobjError = AddError(1126, "");
                    larrErrors.Add(lobjError);
                }
                if (lintPlanId != 9) // Only Plan is a mandatory field for plan 'LIFE' ie Plan id =9 
                {
                    if (string.IsNullOrEmpty(lstrBenefitType))
                    {
                        lobjError = AddError(1127, "");
                        larrErrors.Add(lobjError);
                    }
                    if (ldecPercent > 100)
                    {
                        lobjError = AddError(1121, "");
                        larrErrors.Add(lobjError);
                    }
                    if (ldecPercent == 0)
                    {
                        lobjError = AddError(1128, "");
                        larrErrors.Add(lobjError);
                    }
                    else if (IsNonNegative(ldecPercent.ToString()))
                    {
                        lobjError = AddError(5059, "");
                        larrErrors.Add(lobjError);
                    }
                }
                if (ldtStrtTime == DateTime.MinValue)
                {
                    lobjError = AddError(1123, "");
                    larrErrors.Add(lobjError);
                }
                if (ldtEndTime != DateTime.MinValue && ldtEndTime <= ldtStrtTime)
                {
                    lobjError = AddError(1122, "");
                    larrErrors.Add(lobjError);
                }


                if (this.icdoRelationship.relationship_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                {
                    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
                    {
                        if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == "PRIM")
                        {
                            if (!this.iclbPersonAccountBeneficiariesAll.IsNullOrEmpty())
                            {
                                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiaryAll in this.iclbPersonAccountBeneficiariesAll)
                                {
                                    if (lbusPersonAccountBeneficiaryAll.icdoPersonAccountBeneficiary.beneficiary_type_value == "PRIM")
                                    {
                                        if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan == lbusPersonAccountBeneficiaryAll.icdoPersonAccountBeneficiary.iaintPlan)
                                        {

                                        }
                                    }
                                }

                            }

                        }
                    }
                }
                  //Check for Duplicate Plan
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
                {
                    int lintColPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan;
                    DateTime ldtEndDate = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date;
                    if (lintPlanId == lintColPlanID && (ldtEndDate == DateTime.MinValue || ldtEndDate > DateTime.Today))
                    {
                        lobjError = AddError(1130, "");
                        larrErrors.Add(lobjError);
                        break;
                    }
                }


            }
            return larrErrors;
        }

        public void LoadSoftErrors()
        {
            iclbBenfErrors = new Collection<busError>();
            DataTable ldtbError = Select("cdoRelationship.LoadSoftErrors", new object[1] { this.icdoRelationship.person_relationship_id });
            if (ldtbError.Rows.Count > 0)
            {

                foreach (DataRow ldtrError in ldtbError.Rows)
                {
                    busError lobjError = new busError();
                    sqlFunction.LoadQueryResult(lobjError, ldtrError, iobjPassInfo.iconFramework);
                    //lobjError.parameter_values = ldtrError[enmPersonAccountBeneficiary.person_account_beneficiary_id.ToString()].ToString();
                    string lstrParamValues = lobjError.parameter_values;

                    if (!string.IsNullOrEmpty(lstrParamValues) && lstrParamValues.IndexOf(';') > 0)
                    {
                        string lstrPlanID = lstrParamValues.Substring(lstrParamValues.IndexOf(';') + 1);
                        if (lstrPlanID.IndexOf(';') > 0)
                        {
                            lstrPlanID = lstrPlanID.Replace(";", string.Empty);
                        }
                        if (lstrPlanID.IsNumeric())
                        {
                            DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlanDescription", new object[1] { lstrPlanID });
                            if (ldtbList.Rows.Count > 0)
                            {
                                DataRow ldrRow = ldtbList.Rows[0];
                                string lstrPlanDesc = Convert.ToString(ldrRow[0]);
                                lstrParamValues = lstrParamValues.Replace(lstrPlanID, lstrPlanDesc);
                            }
                        }
                    }
                    lobjError.display_message = FormatMessageParameters(lobjError.message_id, lobjError.display_message, lstrParamValues);
                    iclbBenfErrors.Add(lobjError);
                }
            }
        }

        #endregion




        #region Public Overriden Methods

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (this.icdoRelationship.beneficiary_person_id != 0 || this.icdoRelationship.beneficiary_org_id != 0)
            {
                base.ValidateHardErrors(aenmPageMode);
            }
            else
            {
                this.iarrErrors.Add(AddError(1108, ""));
            }
            foreach (busPersonAccountBeneficiary objbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
            {
                if (objbusPersonAccountBeneficiary.ibusParticipant.IsNull())
                    objbusPersonAccountBeneficiary.ibusParticipant = this.ibusPerson;
                //Performance server PIR 838 fixed
                //if (objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.ihstOldValues != null && objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.ihstOldValues[enmPersonAccountBeneficiary.start_date.ToString()].ToString() !=
                //    objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date.ToString())
                //{
                //    this.iarrErrors.Add(AddError(1164, ""));                       // Start date cannot be Modified.
                //}
                //if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.icdoRelationship.relationship_value != busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
                //{
                //    this.iarrErrors.Add(AddError(1177, " "));
                //}

                //En PIR 66
                //if (objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan != busConstant.LIFE_PLAN_ID && this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.icdoRelationship.relationship_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE && objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent != 100)
                //{
                //    this.iarrErrors.Add(AddError(1178, " "));
                //}
                if (objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan != busConstant.LIFE_PLAN_ID && this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date != this.ibusPerson.icdoPerson.date_of_death)
                {
                    this.iarrErrors.Add(AddError(1179, " "));
                }
                if (objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == "CONT")
                {
                    //int lintPRIMCount = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPrimaryBenCount", new object[1] { this.icdoRelationship.person_id },
                    //                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                    //if (lintPRIMCount == 0)
                    //{
                    //    this.iarrErrors.Add(AddError(6205, " "));
                    //}
                    DataTable ldtbPrimaryBen = Select("cdoPerson.GetPrimaryBenCount", new object[1] { this.icdoRelationship.person_id });
                    if (ldtbPrimaryBen.Rows.Count > 0)
                    {
                        iclbBeneficiaries = GetCollection<busPersonAccountBeneficiary>(ldtbPrimaryBen, "icdoPersonAccountBeneficiary");

                        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in iclbBeneficiaries)
                        {
                            if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintBenPersonID != this.icdoRelationship.beneficiary_person_id)
                            {
                                iblnIsTrue = busConstant.BOOL_TRUE;
                                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan == objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan)
                                {
                                    iblnIsPlan1 = busConstant.BOOL_TRUE;
                                    #region PROD PIR 352
                                    //if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.idtBenDateOfDeath == DateTime.MinValue)
                                    //{
                                    //    this.iarrErrors.Add(AddError(6206, ""));
                                    //    break;
                                    //}    
                                    #endregion
                                }

                            }
                        }
                        if (iblnIsPlan1 != busConstant.BOOL_TRUE)
                        {
                            this.iarrErrors.Add(AddError(6205, ""));
                            break;
                        }
                    }
                    if (iblnIsTrue != busConstant.BOOL_TRUE)
                    {
                        this.iarrErrors.Add(AddError(6205, ""));
                    }
                }
            }

            if (this.ibusPerson.iblnParticipant != busConstant.YES && this.icdoRelationship.beneficiary_from_value == busConstant.BENEFICIARY_FORM_PARTICIPANT)
            {
                this.iarrErrors.Add(AddError(5501, " "));
            }
            if (this.ibusPerson.iblnBeneficiary != busConstant.YES && this.icdoRelationship.beneficiary_from_value == busConstant.BENEFICIARY_FORM_SURVIVOR)
            {
                this.iarrErrors.Add(AddError(5502, " "));
            }
            if (this.ibusPerson.iblnAlternatePayee != busConstant.YES && this.icdoRelationship.beneficiary_from_value == busConstant.BENEFICIARY_FORM_ALTERNATE_PAYEE)
            {
                this.iarrErrors.Add(AddError(5500, " "));
            }
            if ((this.icdoRelationship.beneficiary_from_value == "SURV" || this.icdoRelationship.beneficiary_from_value == "ALTP") && this.icdoRelationship.beneficiary_of == 0)
            {
                this.iarrErrors.Add(AddError(1176, ""));
            }
            //PIR-1086
            bool IsplanNotExist = true;
            if (this.iclbPersonAccountBeneficiary.Any())
            {
                IsplanNotExist = false;
            }
            //PIR RID 72008 to add beneficiary (BOB) for SURV or ALTP
            if (IsplanNotExist && this.icdoRelationship.beneficiary_from_value != "SURV" && this.icdoRelationship.beneficiary_from_value != "ALTP")
            {
                this.iarrErrors.Add(AddError(6289, ""));

            }
                      
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusPerson.icdoPerson.person_id != 0)
            {
                this.icdoRelationship.person_id = ibusPerson.icdoPerson.person_id;
                if (!string.IsNullOrEmpty(this.istrMpiPersonID))
                {
                    DataTable ldtbPersonID = Select<cdoPerson>(
                        new string[1] { enmPerson.mpi_person_id.ToString() },
                        new object[1] { this.istrMpiPersonID }, null, null);
                    if (ldtbPersonID.Rows.Count > 0)
                    {
                        DataRow dtrRow = ldtbPersonID.Rows[0];
                        this.icdoRelationship.beneficiary_person_id = Convert.ToInt32(dtrRow[enmPerson.person_id.ToString()]);
                        if (iobjPassInfo.idictParams.ContainsKey("Benefeciary_Maintenance_AddressFlag") && !string.IsNullOrEmpty(Convert.ToString(iobjPassInfo.GetParamValue("Benefeciary_Maintenance_AddressFlag"))))
                        {
                            this.icdoRelationship.addr_same_as_participant_flag = Convert.ToString(iobjPassInfo.GetParamValue("Benefeciary_Maintenance_AddressFlag"));
                        }
                        if (string.IsNullOrEmpty(this.icdoRelationship.addr_same_as_participant_flag))
                        {
                            this.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                        }

                    }
                }
                else if (!string.IsNullOrEmpty(this.istrMpiOrgID))
                {
                    DataTable ldtbORGID = Select<cdoOrganization>(
                          new string[1] { enmOrganization.mpi_org_id.ToString() },
                          new object[1] { this.istrMpiOrgID }, null, null);
                    if (ldtbORGID.Rows.Count > 0)
                    {
                        DataRow ldtrRow = ldtbORGID.Rows[0];
                        this.icdoRelationship.beneficiary_org_id = Convert.ToInt32(ldtrRow[enmOrganization.org_id.ToString()]);

                    }
                }
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (this.icdoRelationship.iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {
                string lstrBenType = Convert.ToString(this.icdoRelationship.ihstOldValues[enmRelationship.beneficiary_from_value.ToString()]);
                if (lstrBenType != this.icdoRelationship.beneficiary_from_value)
                {
                    iblnIsBenTypeOldValueIsDiff = true;
                }
                else
                {
                    iblnIsBenTypeOldValueIsDiff = false;
                }
            }
            busRelationship lbusRelationship = null;
            if (this.icdoRelationship.ienuObjectState == ObjectState.Insert && this.icdoRelationship.beneficiary_org_id == 0)
            {
                DataTable ldtbList = Select("cdoRelationship.CheckDependentExists", new object[2] { 
                   this.ibusPerson.icdoPerson.person_id, this.icdoRelationship.beneficiary_person_id });

                if (ldtbList.Rows.Count > 0)
                {
                    DataRow ldtRow = ldtbList.Rows[0];
                    lbusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtRow);
                    lbusRelationship.icdoRelationship.beneficiary_person_id = this.icdoRelationship.beneficiary_person_id;
                    lbusRelationship.icdoRelationship.relationship_value = this.icdoRelationship.relationship_value;
                    lbusRelationship.icdoRelationship.beneficiary_from_value = this.icdoRelationship.beneficiary_from_value;
                    lbusRelationship.icdoRelationship.Update();

                    this.iarrChangeLog.Remove(this.icdoRelationship);
                    this.icdoRelationship.ienuObjectState = ObjectState.None;
                    this.icdoRelationship = lbusRelationship.icdoRelationship;
                    this.iobjMainCDO = lbusRelationship.icdoRelationship;

                }
            }
            if (ibusPerson.icdoPerson.person_id != 0)
            {
                this.icdoRelationship.person_id = ibusPerson.icdoPerson.person_id;

                foreach (busPersonAccountBeneficiary objbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
                {
                    DataTable ldtbList = null;
                    objbusPersonAccountBeneficiary.iaintMainPersonID = this.ibusPerson.icdoPerson.person_id;
                    objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue = this.icdoRelationship.beneficiary_from_value;
                    objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintBeneficiaryOf = this.icdoRelationship.beneficiary_of;

                    if (this.icdoRelationship.beneficiary_from_value == busConstant.PERSON_TYPE_SURVIVOR || this.icdoRelationship.beneficiary_from_value == busConstant.PERSON_TYPE_ALTERNATE_PAYEE)
                    {
                        ldtbList = Select("cdoPersonAccountBeneficiary.GetAccountIDForSurvivorAndAltPayee", new object[2] { ibusPerson.icdoPerson.person_id, objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan });

                    }
                    else
                    {
                        ldtbList = Select("cdoPersonAccountBeneficiary.GetAccountID", new object[2] { ibusPerson.icdoPerson.person_id, objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan });

                    }
                    if (ldtbList.Rows.Count > 0)
                    {
                        objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id = Convert.ToInt32(ldtbList.Rows[0][enmPersonAccount.person_account_id.ToString()]);
                    }
                }
            }
            if (this.iarrChangeLog.Count > 0)
            {
                if (this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == busConstant.LIFE_PLAN_ID).Count() > 0)
                {
                    cdoPersonAccountBeneficiary lcdoPersonAccountBen = this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == busConstant.LIFE_PLAN_ID).FirstOrDefault().icdoPersonAccountBeneficiary;
                    if (this.iarrChangeLog.Contains(lcdoPersonAccountBen))
                    {
                        if (lcdoPersonAccountBen.end_date != DateTime.MinValue)
                        {
                            if (lcdoPersonAccountBen.ihstOldValues.Count > 0 && Convert.ToString(lcdoPersonAccountBen.ihstOldValues[enmPersonAccountBeneficiary.end_date.ToString()]).IsNotNullOrEmpty()
                                && Convert.ToDateTime(lcdoPersonAccountBen.ihstOldValues[enmPersonAccountBeneficiary.end_date.ToString()]) == DateTime.MinValue)
                            {

                                iblnDeleteHealthBen = true;
                            }
                        }
                    }
                }

            }
     
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();


            int laintMainPersonID = this.ibusPerson.icdoPerson.person_id;
            if (this.icdoRelationship.beneficiary_from_value == "ALTP" || this.icdoRelationship.beneficiary_from_value == "SURV")
            {
                LoadMainParticipantAccounts();
                InsertPersonAccountsForBeneficiary();
            }
            if (iblnIsBenTypeOldValueIsDiff)
            {
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Delete();
                }
            }
            // validate soft errors for all beneficiaries.
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiariesAll)
            {
                lbusPersonAccountBeneficiary.iaintMainPersonID = laintMainPersonID;
                //lbusPersonAccountBeneficiary.istrBenefeficiaryFromValue = this.icdoRelationship.beneficiary_from_value;
                //lbusPersonAccountBeneficiary.iaintBeneficiaryOf = this.icdoRelationship.beneficiary_of;
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue || lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= DateTime.Now ||
                    (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == this.ibusPerson.icdoPerson.date_of_death))
                {
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();
                }
            }
            if (icdoRelationship.beneficiary_person_id != 0)
            {
                ibusParticipantBeneficiary.FindPerson(icdoRelationship.beneficiary_person_id);

                //While adding a Beneficary - If the participant is a VIP then the Beneficiary also becomes a VIP
                if (this.ibusPerson.icdoPerson.vip_flag == busConstant.FLAG_YES)
                {
                    this.ibusParticipantBeneficiary.icdoPerson.vip_flag = busConstant.FLAG_YES;
                    this.ibusParticipantBeneficiary.icdoPerson.Update();
                    AuditLogHistoryForPerson();
                }
            }
            else if (icdoRelationship.beneficiary_org_id != 0)
            {
                ibusOrganization.FindOrganization(icdoRelationship.beneficiary_org_id);
            }
            //Load All applications
            if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.icdoRelationship.relationship_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
            {
                DataTable ldtApp = Select("cdoPersonAccountBeneficiary.GetApplicationWithJAndSoption", new object[1] { this.ibusPerson.icdoPerson.person_id });
                if (ldtApp.IsNotNull() && ldtApp.Rows.Count > 0)
                {
                    busBenefitApplicationDetail lbusBenefitAppDetail = null;
                    foreach (DataRow ldtRow in ldtApp.Rows)
                    {
                        lbusBenefitAppDetail = new busBenefitApplicationDetail { icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail() };
                        lbusBenefitAppDetail.icdoBenefitApplicationDetail.LoadData(ldtRow);

                        busPlanBenefitXr lbusPlanxr = new busPlanBenefitXr { icdoPlanBenefitXr = new cdoPlanBenefitXr() };
                        lbusPlanxr.icdoPlanBenefitXr.LoadData(ldtRow);
                        if (this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == lbusPlanxr.icdoPlanBenefitXr.plan_id).Count() > 0)
                        {
                            lbusBenefitAppDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.icdoRelationship.beneficiary_person_id;
                            lbusBenefitAppDetail.icdoBenefitApplicationDetail.Update();
                        }
                    }
                }
            }
            //Loading all beneficiaries...
            LoadPersonAccountBeneficiarys();
            LoadAllPersonAccountBeneficiaries();
            LoadSoftErrors();


            //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
            //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
            //{
            //    return;
            //}

            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                return;
            }
            // decommissioning demographics informations, since HEDB is retiring.
            //OPUS data push to Health Eligibility for New Beneficiary with life plan                //Commented - Rohan Code For data Push to HEDB  (Do not delete this)

            //if (this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == busConstant.LIFE_PLAN_ID).Count() > 0)
            //{

            //    //this.iobjPassInfo.ienmPageMode == utlPageMode.New && 
            //    utlPassInfo lobjPassInfo1 = new utlPassInfo();
            //    lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
            //    lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

            //    if (lobjPassInfo1.iconFramework != null)
            //    {

            //        string lstrBeneficiaryPersonId = string.Empty;

            //        string lstrFirstName = string.Empty;
            //        string lstrMiddleName = string.Empty;
            //        string lstrlastName = string.Empty;
            //        string lstrGender = string.Empty;


            //        string lstrHomePhone = string.Empty;
            //        string lstrCellPhone = string.Empty;
            //        string lstrFax = string.Empty;
            //        string lstrEmail = string.Empty;
            //        string lstrCreatedBy = iobjPassInfo.istrUserID;

            //        string lstrBeneficiaryPersonSSN = string.Empty;
            //        string lstrParticipantMPIId = this.ibusPerson.icdoPerson.mpi_person_id;
            //        string lstrParticipantSSN = this.ibusPerson.icdoPerson.istrSSNNonEncrypted;
            //        string lstrEntityTypeCode = "P";

            //        if (this.icdoRelationship.beneficiary_person_id > 0)
            //        {
            //            lstrBeneficiaryPersonId = this.ibusParticipantBeneficiary.icdoPerson.mpi_person_id;
            //            lstrBeneficiaryPersonSSN = this.ibusParticipantBeneficiary.icdoPerson.istrSSNNonEncrypted;

            //            lstrFirstName = this.ibusParticipantBeneficiary.icdoPerson.first_name;
            //            lstrMiddleName = this.ibusParticipantBeneficiary.icdoPerson.middle_name;
            //            lstrlastName = this.ibusParticipantBeneficiary.icdoPerson.last_name;
            //            lstrGender = this.ibusParticipantBeneficiary.icdoPerson.gender_value;

            //            lstrHomePhone = this.ibusParticipantBeneficiary.icdoPerson.home_phone_no;
            //            lstrCellPhone = this.ibusParticipantBeneficiary.icdoPerson.cell_phone_no;
            //            lstrFax = this.ibusParticipantBeneficiary.icdoPerson.fax_no;
            //            lstrEmail = this.ibusParticipantBeneficiary.icdoPerson.email_address_1;
            //            lstrCreatedBy = iobjPassInfo.istrUserID;


            //            lstrEntityTypeCode = "P";
            //        }
            //        else if (this.icdoRelationship.beneficiary_org_id > 0)
            //        {
            //            if (this.ibusOrganization.FindOrganization(this.icdoRelationship.beneficiary_org_id))
            //            {

            //                if (this.ibusOrganization.icdoOrganization.org_type_value == "TRST" || this.ibusOrganization.icdoOrganization.org_type_value == "MTFA")
            //                {
            //                    lstrEntityTypeCode = "T";
            //                }
            //                else
            //                {
            //                    lstrEntityTypeCode = "O";
            //                }
            //                lstrBeneficiaryPersonId = this.ibusOrganization.icdoOrganization.mpi_org_id;
            //                lstrBeneficiaryPersonSSN = this.ibusOrganization.icdoOrganization.federal_id;

            //                lstrFirstName = this.ibusOrganization.icdoOrganization.org_name;

            //                lstrHomePhone = this.ibusOrganization.icdoOrganization.phone_no;
            //                lstrFax = this.ibusOrganization.icdoOrganization.fax_no;
            //                lstrEmail = this.ibusOrganization.icdoOrganization.email_address;
            //            }
            //        }


            //        string strQuery = "select * from Elig_PID_NewDependentBeneficiaries where SSN = '" + lstrParticipantSSN + "' and DepSSN = '" + lstrBeneficiaryPersonSSN + "' and Type = 'B'";
            //        DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
            //        if (ldtbResult.Rows.Count > 0)
            //        {
            //            DeleteHealthBeneficiary();
            //            return;
            //        }


            //        string lstrRelationshipType = "B";

            //        if (lstrFirstName.IsNotNullOrEmpty())
            //        {
            //            lstrFirstName = lstrFirstName.ToUpper();
            //        }

            //        if (lstrMiddleName.IsNotNullOrEmpty())
            //        {
            //            lstrMiddleName = lstrMiddleName.ToUpper();
            //        }

            //        if (lstrlastName.IsNotNullOrEmpty())
            //        {
            //            lstrlastName = lstrlastName.ToUpper();
            //        }

            //        DateTime lstrDOB = DateTime.MinValue;
            //        DateTime lstrDOD = DateTime.MinValue;

            //        if (this.icdoRelationship.beneficiary_person_id > 0)
            //        {
            //            lstrDOB = this.ibusParticipantBeneficiary.icdoPerson.idtDateofBirth;
            //            lstrDOD = this.ibusParticipantBeneficiary.icdoPerson.date_of_death;

            //            if (this.ibusParticipantBeneficiary.icdoPerson.date_of_death == DateTime.MinValue)
            //            {
            //                DataTable ldtblGetDateOfDeath = Select("cdoDeathNotification.GetDateOfDeathInProgress", new object[1] { this.ibusParticipantBeneficiary.icdoPerson.person_id });
            //                if (ldtblGetDateOfDeath != null && ldtblGetDateOfDeath.Rows.Count > 0
            //                    && Convert.ToString(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
            //                {
            //                    lstrDOD = Convert.ToDateTime(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]);
            //                }
            //            }
            //            else
            //            {
            //                lstrDOD = this.ibusParticipantBeneficiary.icdoPerson.date_of_death;
            //            }
            //        }


            //        //string lstrCreatedBy = this.ibusParticipantBeneficiary.icdoPerson.created_by;
            //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            //        lobjParameter.ParameterName = "@PID";
            //        lobjParameter.DbType = DbType.String;
            //        lobjParameter.Value = lstrBeneficiaryPersonId;
            //        lcolParameters.Add(lobjParameter);
            //        //Sid Jain 04052013
            //        if (lstrBeneficiaryPersonSSN.IsNotNullOrEmpty())
            //        {
            //            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //            lobjParameter1.ParameterName = "@SSN";
            //            lobjParameter1.DbType = DbType.String;
            //            lobjParameter1.Value = lstrBeneficiaryPersonSSN;
            //            lcolParameters.Add(lobjParameter1);
            //        }
            //        IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //        lobjParameter2.ParameterName = "@ParticipantPID";
            //        lobjParameter2.DbType = DbType.String;
            //        lobjParameter2.Value = lstrParticipantMPIId.ToLower();
            //        lcolParameters.Add(lobjParameter2);

            //        IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            //        lobjParameter3.ParameterName = "@EntityTypeCode";
            //        lobjParameter3.DbType = DbType.String;
            //        lobjParameter3.Value = lstrEntityTypeCode;                  //we will always use Person
            //        lcolParameters.Add(lobjParameter3);

            //        IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
            //        lobjParameter4.ParameterName = "@RelationType";
            //        lobjParameter4.DbType = DbType.String;
            //        lobjParameter4.Value = lstrRelationshipType;
            //        lcolParameters.Add(lobjParameter4);

            //        IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
            //        lobjParameter5.ParameterName = "@FirstName";
            //        lobjParameter5.DbType = DbType.String;
            //        lobjParameter5.Value = lstrFirstName;
            //        lcolParameters.Add(lobjParameter5);

            //        IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
            //        lobjParameter6.ParameterName = "@MiddleName";
            //        lobjParameter6.DbType = DbType.String;
            //        lobjParameter6.Value = lstrMiddleName;
            //        lcolParameters.Add(lobjParameter6);

            //        IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
            //        lobjParameter7.ParameterName = "@LastName";
            //        lobjParameter7.DbType = DbType.String;
            //        lobjParameter7.Value = lstrlastName;
            //        lcolParameters.Add(lobjParameter7);

            //        IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
            //        lobjParameter9.ParameterName = "@Gender";
            //        lobjParameter9.DbType = DbType.String;
            //        lobjParameter9.Value = lstrGender;
            //        lcolParameters.Add(lobjParameter9);



            //        IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
            //        lobjParameter10.ParameterName = "@DateOfBirth";
            //        lobjParameter10.DbType = DbType.DateTime;
            //        if (lstrDOB != DateTime.MinValue)
            //        {
            //            lobjParameter10.Value = lstrDOB;
            //        }
            //        lcolParameters.Add(lobjParameter10);

            //        IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
            //        lobjParameter11.ParameterName = "@DateOfDeath";
            //        lobjParameter11.DbType = DbType.DateTime;

            //        if (lstrDOD != DateTime.MinValue)
            //        {
            //            lobjParameter11.Value = lstrDOD;
            //        }
            //        lcolParameters.Add(lobjParameter11);


            //        IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
            //        lobjParameter12.ParameterName = "@HomePhone";
            //        lobjParameter12.DbType = DbType.String;
            //        lobjParameter12.Value = lstrHomePhone;
            //        lcolParameters.Add(lobjParameter12);

            //        IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
            //        lobjParameter13.ParameterName = "@CellPhone";
            //        lobjParameter13.DbType = DbType.String;
            //        lobjParameter13.Value = lstrCellPhone;
            //        lcolParameters.Add(lobjParameter13);

            //        IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
            //        lobjParameter14.ParameterName = "@Fax";
            //        lobjParameter14.DbType = DbType.String;
            //        lobjParameter14.Value = lstrFax;
            //        lcolParameters.Add(lobjParameter14);

            //        IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
            //        lobjParameter15.ParameterName = "@Email";
            //        lobjParameter15.DbType = DbType.String;
            //        lobjParameter15.Value = lstrEmail;
            //        lcolParameters.Add(lobjParameter15);

            //        IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
            //        lobjParameter16.ParameterName = "@AuditUser";
            //        lobjParameter16.DbType = DbType.String;
            //        lobjParameter16.Value = lstrCreatedBy;
            //        lcolParameters.Add(lobjParameter16);


            //        try
            //        {
            //            lobjPassInfo1.BeginTransaction();
            //            DBFunction.DBExecuteProcedure("USP_PID_PERSON_INS", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //            bool lblnTrans = DeleteHealthBeneficiary();
            //            if (lblnTrans)
            //            {
            //                lobjPassInfo1.Commit();
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            lobjPassInfo1.Rollback();
            //            throw e;
            //        }
            //        finally
            //        {
            //            lobjPassInfo1.iconFramework.Close();
            //        }
            //    }
            //}//Commented - Rohan Code For data Push to HEDB

            //PIR 1050
            if (this.icdoRelationship != null && this.icdoRelationship.addr_same_as_participant_flag == busConstant.FLAG_YES)
            {
                this.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                this.icdoRelationship.Update();
            }
            if (iintAPrimaryKey==0 && icdoRelationship.relationship_id !=0 && iobjPassInfo.istrSenderForm== "wfmParticipantBeneficiaryMaintenance")
            {
                iintAPrimaryKey = icdoRelationship.relationship_id;
            }

            if (this.iarrChangeLog.Count == 0)
            {
                iobjPassInfo.idictParams["SaveMesssageParticipantBeneficiary"] = "false";
            }
        }


        //public override void AfterPersistChanges()
        //{
        //    base.AfterPersistChanges();
        //    int laintMainPersonID = this.ibusPerson.icdoPerson.person_id;
        //    if (this.icdoRelationship.beneficiary_from_value == "ALTP" || this.icdoRelationship.beneficiary_from_value == "SURV")
        //    {
        //        LoadMainParticipantAccounts();
        //        InsertPersonAccountsForBeneficiary();
        //    }
        //    if (iblnIsBenTypeOldValueIsDiff)
        //    {
        //        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiary)
        //        {
        //            lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Delete();
        //        }
        //    }
        //    // validate soft errors for all beneficiaries.
        //    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbPersonAccountBeneficiariesAll)
        //    {
        //        lbusPersonAccountBeneficiary.iaintMainPersonID = laintMainPersonID;
        //        //lbusPersonAccountBeneficiary.istrBenefeficiaryFromValue = this.icdoRelationship.beneficiary_from_value;
        //        //lbusPersonAccountBeneficiary.iaintBeneficiaryOf = this.icdoRelationship.beneficiary_of;
        //        if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue || lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= DateTime.Now ||
        //            (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == this.ibusPerson.icdoPerson.date_of_death))
        //        {
        //            lbusPersonAccountBeneficiary.ValidateSoftErrors();
        //            lbusPersonAccountBeneficiary.UpdateValidateStatus();
        //        }
        //    }
        //    if (icdoRelationship.beneficiary_person_id != 0)
        //    {
        //        ibusParticipantBeneficiary.FindPerson(icdoRelationship.beneficiary_person_id);

        //        //While adding a Beneficary - If the participant is a VIP then the Beneficiary also becomes a VIP
        //        if (this.ibusPerson.icdoPerson.vip_flag == busConstant.FLAG_YES)
        //        {
        //            this.ibusParticipantBeneficiary.icdoPerson.vip_flag = busConstant.FLAG_YES;
        //            this.ibusParticipantBeneficiary.icdoPerson.Update();
        //        }
        //    }
        //    else if (icdoRelationship.beneficiary_org_id != 0)
        //    {
        //        ibusOrganization.FindOrganization(icdoRelationship.beneficiary_org_id);
        //    }
        //    //Load All applications
        //    if (this.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && this.icdoRelationship.relationship_value == busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE)
        //    {
        //        DataTable ldtApp = Select("cdoPersonAccountBeneficiary.GetApplicationWithJAndSoption", new object[1] { this.ibusPerson.icdoPerson.person_id });
        //        if (ldtApp.IsNotNull() && ldtApp.Rows.Count > 0)
        //        {
        //            busBenefitApplicationDetail lbusBenefitAppDetail = null;
        //            foreach (DataRow ldtRow in ldtApp.Rows)
        //            {
        //                lbusBenefitAppDetail = new busBenefitApplicationDetail { icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail() };
        //                lbusBenefitAppDetail.icdoBenefitApplicationDetail.LoadData(ldtRow);

        //                busPlanBenefitXr lbusPlanxr = new busPlanBenefitXr { icdoPlanBenefitXr = new cdoPlanBenefitXr() };
        //                lbusPlanxr.icdoPlanBenefitXr.LoadData(ldtRow);
        //                if (this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == lbusPlanxr.icdoPlanBenefitXr.plan_id).Count() > 0)
        //                {
        //                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.joint_annuitant_id = this.icdoRelationship.beneficiary_person_id;
        //                    lbusBenefitAppDetail.icdoBenefitApplicationDetail.Update();
        //                }
        //            }
        //        }
        //    }
        //    //Loading all beneficiaries...
        //    LoadPersonAccountBeneficiarys();
        //    LoadAllPersonAccountBeneficiaries();
        //    LoadSoftErrors();


        //    //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
        //    //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
        //    //{
        //    //    return;
        //    //}

        //    if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
        //    {
        //        return;
        //    }

        //    //OPUS data push to Health Eligibility for New Beneficiary with life plan                //Commented - Rohan Code For data Push to HEDB  (Do not delete this)
        //    if (this.iobjPassInfo.ienmPageMode == utlPageMode.New && this.iclbPersonAccountBeneficiary.Where(item => item.icdoPersonAccountBeneficiary.iaintPlan == busConstant.LIFE_PLAN_ID).Count() > 0)
        //    {

        //        IDbConnection lconHELegacy = DBFunction.GetDBConnection("HELegacy");
        //        if (lconHELegacy != null)
        //        {

        //            string lstrBeneficiaryPersonId = this.istrMpiPersonID;
        //            string lstrBeneficiaryPersonSSN = this.ibusParticipantBeneficiary.icdoPerson.istrSSNNonEncrypted;
        //            string lstrParticipantMPIId = this.ibusPerson.icdoPerson.mpi_person_id;
        //            string lstrEntityTypeCode = "P";

        //            if (this.icdoRelationship.beneficiary_person_id > 0)
        //            {
        //                lstrEntityTypeCode = "P";
        //            }
        //            else if (this.icdoRelationship.beneficiary_org_id > 0)
        //            {
        //                this.ibusOrganization.FindOrganization(this.icdoRelationship.beneficiary_org_id);
        //                if (this.ibusOrganization.icdoOrganization.org_type_value == "TRST" || this.ibusOrganization.icdoOrganization.org_type_value == "MTFA")
        //                {
        //                    lstrEntityTypeCode = "T";
        //                }
        //                else
        //                {
        //                    lstrEntityTypeCode = "O";
        //                }

        //            }

        //            string lstrRelationshipType = "B";
        //            string lstrFirstName = this.ibusParticipantBeneficiary.icdoPerson.first_name;
        //            string lstrMiddleName = this.ibusParticipantBeneficiary.icdoPerson.middle_name;
        //            string lstrlastName = this.ibusParticipantBeneficiary.icdoPerson.last_name;
        //            string lstrGender = this.ibusParticipantBeneficiary.icdoPerson.gender_value;

        //            if (lstrFirstName.IsNotNullOrEmpty())
        //            {
        //                lstrFirstName = lstrFirstName.ToUpper();
        //            }

        //            if (lstrMiddleName.IsNotNullOrEmpty())
        //            {
        //                lstrMiddleName = lstrMiddleName.ToUpper();
        //            }

        //            if (lstrlastName.IsNotNullOrEmpty())
        //            {
        //                lstrlastName = lstrlastName.ToUpper();
        //            }

        //            DateTime lstrDOB = DateTime.MinValue;

        //            lstrDOB = this.ibusParticipantBeneficiary.icdoPerson.idtDateofBirth;

        //            DateTime lstrDOD = DateTime.MinValue;

        //            lstrDOD = this.ibusParticipantBeneficiary.icdoPerson.date_of_death;

        //            if (this.ibusParticipantBeneficiary.icdoPerson.date_of_death == DateTime.MinValue)
        //            {
        //                DataTable ldtblGetDateOfDeath = Select("cdoDeathNotification.GetDateOfDeathInProgress", new object[1] { this.ibusParticipantBeneficiary.icdoPerson.person_id });
        //                if (ldtblGetDateOfDeath != null && ldtblGetDateOfDeath.Rows.Count > 0
        //                    && Convert.ToString(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
        //                {
        //                    lstrDOD = Convert.ToDateTime(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]);
        //                }
        //            }
        //            else
        //            {
        //                lstrDOD = this.ibusParticipantBeneficiary.icdoPerson.date_of_death;
        //            }



        //            string lstrHomePhone = this.ibusParticipantBeneficiary.icdoPerson.home_phone_no;
        //            string lstrCellPhone = this.ibusParticipantBeneficiary.icdoPerson.cell_phone_no;
        //            string lstrFax = this.ibusParticipantBeneficiary.icdoPerson.fax_no;
        //            string lstrEmail = this.ibusParticipantBeneficiary.icdoPerson.email_address_1;
        //            string lstrCreatedBy = iobjPassInfo.istrUserID;
        //            //string lstrCreatedBy = this.ibusParticipantBeneficiary.icdoPerson.created_by;



        //            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //            lobjParameter.ParameterName = "@PID";
        //            lobjParameter.DbType = DbType.String;
        //            lobjParameter.Value = lstrBeneficiaryPersonId;
        //            lcolParameters.Add(lobjParameter);
        //            //Sid Jain 04052013
        //            if (lstrBeneficiaryPersonSSN.IsNotNullOrEmpty())
        //            {
        //                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //                lobjParameter1.ParameterName = "@SSN";
        //                lobjParameter1.DbType = DbType.String;
        //                lobjParameter1.Value = lstrBeneficiaryPersonSSN.ToLower();
        //                lcolParameters.Add(lobjParameter1);
        //            }
        //            IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
        //            lobjParameter2.ParameterName = "@ParticipantPID";
        //            lobjParameter2.DbType = DbType.String;
        //            lobjParameter2.Value = lstrParticipantMPIId.ToLower();
        //            lcolParameters.Add(lobjParameter2);

        //            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
        //            lobjParameter3.ParameterName = "@EntityTypeCode";
        //            lobjParameter3.DbType = DbType.String;
        //            lobjParameter3.Value = lstrEntityTypeCode;                  //we will always use Person
        //            lcolParameters.Add(lobjParameter3);

        //            IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        //            lobjParameter4.ParameterName = "@RelationType";
        //            lobjParameter4.DbType = DbType.String;
        //            lobjParameter4.Value = lstrRelationshipType;
        //            lcolParameters.Add(lobjParameter4);

        //            IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
        //            lobjParameter5.ParameterName = "@FirstName";
        //            lobjParameter5.DbType = DbType.String;
        //            lobjParameter5.Value = lstrFirstName;
        //            lcolParameters.Add(lobjParameter5);

        //            IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        //            lobjParameter6.ParameterName = "@MiddleName";
        //            lobjParameter6.DbType = DbType.String;
        //            lobjParameter6.Value = lstrMiddleName;
        //            lcolParameters.Add(lobjParameter6);

        //            IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        //            lobjParameter7.ParameterName = "@LastName";
        //            lobjParameter7.DbType = DbType.String;
        //            lobjParameter7.Value = lstrlastName;
        //            lcolParameters.Add(lobjParameter7);

        //            IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
        //            lobjParameter9.ParameterName = "@Gender";
        //            lobjParameter9.DbType = DbType.String;
        //            lobjParameter9.Value = lstrGender;
        //            lcolParameters.Add(lobjParameter9);



        //            IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
        //            lobjParameter10.ParameterName = "@DateOfBirth";
        //            lobjParameter10.DbType = DbType.DateTime;
        //            if (lstrDOB != DateTime.MinValue)
        //            {
        //                lobjParameter10.Value = lstrDOB;
        //            }
        //            lcolParameters.Add(lobjParameter10);




        //            IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
        //            lobjParameter11.ParameterName = "@DateOfDeath";
        //            lobjParameter11.DbType = DbType.DateTime;

        //            if (lstrDOD != DateTime.MinValue)
        //            {
        //                lobjParameter11.Value = lstrDOD;
        //            }
        //            lcolParameters.Add(lobjParameter11);


        //            IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
        //            lobjParameter12.ParameterName = "@HomePhone";
        //            lobjParameter12.DbType = DbType.String;
        //            lobjParameter12.Value = lstrHomePhone;
        //            lcolParameters.Add(lobjParameter12);

        //            IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
        //            lobjParameter13.ParameterName = "@CellPhone";
        //            lobjParameter13.DbType = DbType.String;
        //            lobjParameter13.Value = lstrCellPhone;
        //            lcolParameters.Add(lobjParameter13);

        //            IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
        //            lobjParameter14.ParameterName = "@Fax";
        //            lobjParameter14.DbType = DbType.String;
        //            lobjParameter14.Value = lstrFax;
        //            lcolParameters.Add(lobjParameter14);

        //            IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
        //            lobjParameter15.ParameterName = "@Email";
        //            lobjParameter15.DbType = DbType.String;
        //            lobjParameter15.Value = lstrEmail;
        //            lcolParameters.Add(lobjParameter15);

        //            IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
        //            lobjParameter16.ParameterName = "@AuditUser";
        //            lobjParameter16.DbType = DbType.String;
        //            lobjParameter16.Value = lstrCreatedBy;
        //            lcolParameters.Add(lobjParameter16);

        //            DBFunction.DBExecuteProcedure("USP_PID_PERSON_INS", lcolParameters, lconHELegacy, null);

        //            lconHELegacy.Close();

        //        }
        //    }//Commented - Rohan Code For data Push to HEDB

        //}


        public bool CheckBeneficiaryMaritalStatus()
        {
            DataTable ldtbMaritalStatus = Select("cdoRelationship.GetDependentMaritalStatus", new object[1] { this.icdoRelationship.beneficiary_person_id });
            DataRow ldr = ldtbMaritalStatus.Rows[0];
            int lintresult = 0;

            lintresult = string.Compare(Convert.ToString(ldr[enmPerson.marital_status_value.ToString()]), busConstant.MARITAL_STATUS_MARRIED);
            if (!(lintresult == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// To be called when adding BOB.
        /// </summary>
        public void LoadMainParticipantAccounts()
        {
            DataTable ldtbList = Select<cdoPersonAccount>(
                new string[1] { enmPersonAccount.person_id.ToString() },
                new object[1] { icdoRelationship.beneficiary_of }, null, null);
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtbList, "icdoPersonAccount");
        }

        //public virtual void LoadPersonAccountBeneficiarys()
        //{
        //    DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
        //        new string[1] { enmPersonAccountBeneficiary.person_relationship_id.ToString() },
        //        new object[1] { icdoRelationship.person_relationship_id }, null, null);
        //    iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");
        //}

        /// <summary>
        /// To insert person for beneficiaries(B) when adding beneficiary of beneficiary(BOB).
        /// </summary>
        public void InsertPersonAccountsForBeneficiary()
        {
            #region Load IF Person Account Exists for B
            Collection<busPersonAccount> lclbPersonAccount = new Collection<busPersonAccount>();
            DataTable ldtbList = Select("cdoRelationship.CheckIfPlanAddedForBeneficiary", new object[2] { this.icdoRelationship.person_id, this.icdoRelationship.beneficiary_of });
            if (ldtbList.Rows.Count > 0)
            {
                lclbPersonAccount = GetCollection<busPersonAccount>(ldtbList, "icdoPersonAccount");
            }


            #endregion
            cdoPersonAccount lcdoBeneficiaryPersonAccount;

            #region Insert Person Account For B

            foreach (busPersonAccount lbusPersonAccount in iclbPersonAccount)
            {
                if (lclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id).Count() > 0)
                {
                    continue;
                    //lcdoBeneficiaryPersonAccount = lclbPersonAccount.Where(plan => plan.icdoPersonAccount.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id).FirstOrDefault().icdoPersonAccount;
                }
                lcdoBeneficiaryPersonAccount = new cdoPersonAccount();
                lcdoBeneficiaryPersonAccount.plan_id = lbusPersonAccount.icdoPersonAccount.plan_id;
                lcdoBeneficiaryPersonAccount.benefeciary_person_id = this.icdoRelationship.person_id;
                lcdoBeneficiaryPersonAccount.benefeciary_of_person_id = lbusPersonAccount.icdoPersonAccount.person_id;
                lcdoBeneficiaryPersonAccount.Insert();
            }

            #endregion
        }

        /* public override void LoadPersonAccountBeneficiarys()
         {
             base.LoadPersonAccountBeneficiarys();
             foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
             {
                 lbusPersonAccountBeneficiary.iaintMainPersonID = this.ibusPerson.icdoPerson.person_id;
                 DataTable ldtbPlan = Select("cdoPersonAccountBeneficiary.GetPlanFromAccountID", new object[1] { lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id });
                 if (ldtbPlan.Rows.Count > 0)
                 {
                     DataRow ldtrRow = ldtbPlan.Rows[0];
                     lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan = Convert.ToString(ldtrRow[enmPlan.plan_name.ToString()]);
                     lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan = Convert.ToInt32(ldtrRow[enmPlan.plan_id.ToString()]);
                 }
             }

         }*/
        /*
        public override int PersistChanges()
        {
            int lint = 0;
            if (icdoRelationship.ienuObjectState == ObjectState.Insert)
            {
                DataTable ldtbList = Select("cdoRelationship.CheckDependentExists", new object[2] { 
                   this.ibusPerson.icdoPerson.person_id, this.icdoRelationship.beneficiary_person_id });

                if (ldtbList.Rows.Count > 0)
                {
                    DataRow ldtRow = ldtbList.Rows[0];
                    busRelationship lbusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtRow);
                    lbusRelationship.icdoRelationship.beneficiary_person_id = this.icdoRelationship.beneficiary_person_id;
                    lbusRelationship.icdoRelationship.Update();
                }
                else
                {
                    lint = base.PersistChanges();
                }
            }
            else
            {
                lint = base.PersistChanges();
            }
            return lint;
        }
        */
        // decommissioning demographics informations, since HEDB is retiring.
        //public bool DeleteHealthBeneficiary()
        //{
        //    bool lblnTransactionSuccessful = true;
        //    if (iblnDeleteHealthBen)
        //    {
        //        utlPassInfo lobjPassInfo1 = new utlPassInfo();
        //        lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
        //        lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

        //        if (lobjPassInfo1.iconFramework != null)
        //        {
        //            string lstrBeneficiaryMPID = string.Empty;
        //            string lstrParticipantMPID = string.Empty;

        //            if (this.icdoRelationship.beneficiary_person_id > 0)
        //            {
        //                lstrBeneficiaryMPID = this.ibusParticipantBeneficiary.icdoPerson.mpi_person_id;
        //                lstrParticipantMPID = this.ibusPerson.icdoPerson.mpi_person_id;


        //                //string lstrCreatedBy = this.ibusParticipantBeneficiary.icdoPerson.created_by;
        //                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                lobjParameter.ParameterName = "@BeneMPID";
        //                lobjParameter.DbType = DbType.String;
        //                lobjParameter.Value = lstrBeneficiaryMPID;
        //                lcolParameters.Add(lobjParameter);

        //                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //                lobjParameter1.ParameterName = "@MPID";
        //                lobjParameter1.DbType = DbType.String;
        //                lobjParameter1.Value = lstrParticipantMPID;
        //                lcolParameters.Add(lobjParameter1);

        //                try
        //                {
        //                    lobjPassInfo1.BeginTransaction();
        //                    DBFunction.DBExecuteProcedure("LifeInsBeneficiary_Opus_del", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
        //                    lobjPassInfo1.Commit();
        //                }
        //                catch (Exception e)
        //                {
        //                    lobjPassInfo1.Rollback();
        //                    lblnTransactionSuccessful = false;
        //                    throw e;
        //                }
        //                finally
        //                {
        //                    lobjPassInfo1.iconFramework.Close();
        //                }
        //            }
        //        }
        //    }

        //    return lblnTransactionSuccessful;
        //}

        #endregion

        public void AuditLogHistoryForPerson()
        {
            cdoFullAuditLog lcdoFullAuditLog = new cdoFullAuditLog();
            lcdoFullAuditLog.person_id = this.ibusParticipantBeneficiary.icdoPerson.person_id;
            lcdoFullAuditLog.primary_key = this.ibusParticipantBeneficiary.icdoPerson.person_id;
            lcdoFullAuditLog.form_name = "wfmPersonMaintenance";
            lcdoFullAuditLog.table_name = "sgt_person";
            //lcdoFullAuditLog.Insert();

            //cdoFullAuditLogDetail lcdoFullAuditLogDetail = new cdoFullAuditLogDetail();
            //lcdoFullAuditLogDetail.audit_log_id = lcdoFullAuditLog.audit_log_id;
            //lcdoFullAuditLogDetail.old_value = this.ibusPerson.icdoPerson.vip_flag;
            //lcdoFullAuditLogDetail.new_value = this.ibusParticipantBeneficiary.icdoPerson.vip_flag;
            //lcdoFullAuditLogDetail.column_name = "vip_flag";
            //lcdoFullAuditLogDetail.Insert();

            //Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
            var lcdoFullAuditLogDetail = new
            {
                column_name = "vip_flag",
                old_value = this.ibusPerson.icdoPerson.vip_flag,
                new_value = this.ibusParticipantBeneficiary.icdoPerson.vip_flag,
            };
            string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lcdoFullAuditLogDetail);
            lcdoFullAuditLog.audit_details = lsrtJSONAuditDetails;
            lcdoFullAuditLog.Insert();
        }
        public override long iintPrimaryKey
        {
            get
            {
                //if (iobjPassInfo.istrSenderForm == "wfmPersonMaintenance" || iobjPassInfo.istrSenderForm == "wfmPersonOverviewMaintenance" || iobjPassInfo.istrSenderForm == "wfmParticipantBeneficiaryMaintenance")
                //{
                //    return iintAPrimaryKey;
                //}
                return iintAPrimaryKey;
            }
        }

        //public override void AddToResponse(utlResponseData aobjResponseData)
        //{
        //    base.AddToResponse(aobjResponseData);
        //    aobjResponseData.ConcurrentKeysData[utlConstants.istrPrimaryKey] = Convert.ToString(icdoRelationship.iintAPrimaryKey > 0 ? icdoRelationship.iintAPrimaryKey : iintPrimaryKey);

        //}
    }
}
