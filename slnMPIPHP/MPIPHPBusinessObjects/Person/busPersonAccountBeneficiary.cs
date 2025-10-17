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
using System.Text.RegularExpressions;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonAccountBeneficiary:
	/// Inherited from busPersonAccountBeneficiaryGen, the class is used to customize the business object busPersonAccountBeneficiaryGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountBeneficiary : busPersonAccountBeneficiaryGen
	{
        public busPerson ibusPerson { get; set; }
        public busRelationship ibusRelationship { get; set; }
        public int iaintMainPersonID { get; set; }
        //public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary { get; set; }
        public busPerson ibusParticipant 
        { get; set; }
        public busPersonBase ibusDuplicateParticipant { get; set; }

       

        #region Public Methods

       
        public Collection<busPersonAccountBeneficiary> LoadPersonAccountBeneficiaryByBeneficiaryId()
        {
            DataTable ldtbPersonAccountBeneficiary = Select<cdoPersonAccountBeneficiary>(
                 new string[1] { enmPersonAccountBeneficiary.person_relationship_id.ToString() },
                 new object[1] { icdoPersonAccountBeneficiary.person_relationship_id }, null, null);
           Collection<busPersonAccountBeneficiary> lclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbPersonAccountBeneficiary, "icdoPersonAccountBeneficiary");

           return lclbPersonAccountBeneficiary;
        }

        public bool CheckNumeric()
        {
            bool lblnValidPercentage =false;
            Regex lrexGex = new Regex("^[0-9,.]*$");
            if (! lrexGex.IsMatch(icdoPersonAccountBeneficiary.dist_percent.ToString()))
            {
                lblnValidPercentage = true;
            }
            return lblnValidPercentage;         
        }

        public Collection<busPersonAccountBeneficiary> LoadPersonAccountBeneficiaryByPersonAccountID()
        {
            Collection<busPersonAccountBeneficiary> lclbPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();

            DataTable ldtbPersonAccountBeneficiary = SelectWithOperator<cdoPersonAccountBeneficiary>(new string[2] {enmPersonAccountBeneficiary.person_account_id.ToString(),
                                                            enmPersonAccountBeneficiary.person_account_beneficiary_id.ToString()},
                new string[2] { busConstant.DBOperatorEquals, busConstant.DBOperatorNotEquals },
                new object[2] { icdoPersonAccountBeneficiary.person_account_id, icdoPersonAccountBeneficiary.person_account_beneficiary_id }, null);
            
            lclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbPersonAccountBeneficiary, "icdoPersonAccountBeneficiary");
            return lclbPersonAccountBeneficiary;
        }

        public decimal  CalculatePlanPercentage()
        {
            decimal ldecPercenatge = 0;
            DataTable ldtblist = busPerson.Select("cdoPersonAccountBeneficiary.CalculatePlanPercentage", new object[3] {iaintMainPersonID , this.icdoPersonAccountBeneficiary.iaintPlan, this.icdoPersonAccountBeneficiary.beneficiary_type_value });
            string lstrBeneficiaryFrom = string.Empty;
            if (ldtblist.Rows.Count > 0)
            {
                if (this.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue == busConstant.PERSON_TYPE_PARTICIPANT || string.IsNullOrEmpty(this.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue))
                {
                    if (ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_PARTICIPANT || string.IsNullOrEmpty(Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]))).Count() > 0)
                    {
                        ldecPercenatge = ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_PARTICIPANT || string.IsNullOrEmpty(Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]))).Select(row1 => row1["Percentage"] == DBNull.Value ? 0.0M : Convert.ToDecimal(row1["Percentage"])).Sum();
                    }
                }
                else if (this.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue == busConstant.PERSON_TYPE_SURVIVOR)
                {
                    if (ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_SURVIVOR && Convert.ToInt32(row[enmRelationship.beneficiary_of.ToString()]) == this.icdoPersonAccountBeneficiary.iaintBeneficiaryOf).Count() > 0)
                    {
                        ldecPercenatge = ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_SURVIVOR && Convert.ToInt32(row[enmRelationship.beneficiary_of.ToString()]) == this.icdoPersonAccountBeneficiary.iaintBeneficiaryOf).Select(row1 => row1["Percentage"] == DBNull.Value ? 0.0M : Convert.ToDecimal(row1["Percentage"])).Sum();
                    }
                }
                else if (this.icdoPersonAccountBeneficiary.istrBenefeficiaryFromValue == busConstant.PERSON_TYPE_ALTERNATE_PAYEE)
                {
                    if (ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_ALTERNATE_PAYEE && Convert.ToInt32(row[enmRelationship.beneficiary_of.ToString()]) == this.icdoPersonAccountBeneficiary.iaintBeneficiaryOf).Count() > 0)
                    {
                        ldecPercenatge = ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmRelationship.beneficiary_from_value.ToString()]) == busConstant.PERSON_TYPE_ALTERNATE_PAYEE && Convert.ToInt32(row[enmRelationship.beneficiary_of.ToString()]) == this.icdoPersonAccountBeneficiary.iaintBeneficiaryOf).Select(row1 => row1["Percentage"] == DBNull.Value ? 0.0M : Convert.ToDecimal(row1["Percentage"])).Sum();
                    }
                }
            }

            return ldecPercenatge;
        }

        public void DeleteBeneficiary()
        {
            
        }

        public void InsertValuesInPersonAccBeneficiary(int aintPersonRelationshipId, int aintPersonAccountId, DateTime adtStartDate, DateTime adtEndDate,
                                                       decimal adecPercent, string astrBeneficiaryTypeValue, string astrStatusValue)
        {
            if (icdoPersonAccountBeneficiary == null)
            {
                icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
            }

            icdoPersonAccountBeneficiary.person_relationship_id = aintPersonRelationshipId;
            icdoPersonAccountBeneficiary.person_account_id = aintPersonAccountId;
            icdoPersonAccountBeneficiary.start_date = adtStartDate;
            icdoPersonAccountBeneficiary.end_date = adtEndDate;
            icdoPersonAccountBeneficiary.dist_percent = adecPercent;
            icdoPersonAccountBeneficiary.beneficiary_type_id = busConstant.BENEFICIARY_TYPE_ID;
            icdoPersonAccountBeneficiary.beneficiary_type_value = astrBeneficiaryTypeValue;
            icdoPersonAccountBeneficiary.dist_percent = adecPercent;
            icdoPersonAccountBeneficiary.status_id = busConstant.BENEFICIARY_STATUS_ID;
            icdoPersonAccountBeneficiary.status_value = astrStatusValue;
            icdoPersonAccountBeneficiary.Insert();
        }


        #endregion

        #region Public Overriden Methods

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
           
        }

        public override bool ValidateSoftErrors()
        {
            if (this.ibusParticipant.IsNull() && this.iaintMainPersonID > 0)
            {
                this.ibusParticipant.FindPerson(this.iaintMainPersonID);

            }
            if (this.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue && DateTime.Compare(this.icdoPersonAccountBeneficiary.end_date, DateTime.Now.Date) < 0 && (this.ibusParticipant.IsNotNull() && this.ibusParticipant.icdoPerson.date_of_death != this.icdoPersonAccountBeneficiary.end_date))
            {
                return true;
            }
            else
            {
                return base.ValidateSoftErrors();
            }
            
        }

        public override void UpdateValidateStatus()
        {
            base.UpdateValidateStatus();
            if (this.icdoPersonAccountBeneficiary.status_value == busConstant.STATUS_VALID || (this.ibusParticipant.IsNotNull() && this.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue && this.ibusParticipant.icdoPerson.date_of_death != this.icdoPersonAccountBeneficiary.end_date))
            {
                if (this.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue && DateTime.Compare(this.icdoPersonAccountBeneficiary.end_date, DateTime.Now.Date) <= 0)
                {
                    this.icdoPersonAccountBeneficiary.status_value = "INAC";
                    this.icdoPersonAccountBeneficiary.Update();
                    FindPersonAccountBeneficiary(this.icdoPersonAccountBeneficiary.person_relationship_id);
                }
            }
            // Change : 5th March 2013 : Beneficiary can be added even after date of death 
            // Soft Error validation needed even when record is end date
            //if (this.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue && DateTime.Compare(this.icdoPersonAccountBeneficiary.end_date, DateTime.Now.Date) <= 0)
            //{
            //    this.icdoPersonAccountBeneficiary.status_value = "INAC";
            //    this.icdoPersonAccountBeneficiary.Update();
            //    FindPersonAccountBeneficiary(this.icdoPersonAccountBeneficiary.person_relationship_id);
            //}
            //else
            //{
            //    base.UpdateValidateStatus();
            //}
        }

        public override bool ValidateDelete()
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            Collection<busPersonAccountBeneficiary> lclbPersonAccountBeneficiary = LoadPersonAccountBeneficiaryByBeneficiaryId();
            
            if (lclbPersonAccountBeneficiary != null && lclbPersonAccountBeneficiary.Count > 0)
            {
                int lintBenefitApplication = (int)DBFunction.DBExecuteScalar("cdoPerson.GetBenefitApplicationDetail", new object[2] { this.ibusParticipant.icdoPerson.mpi_person_id,this.icdoPersonAccountBeneficiary.istrBenMPID  },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                
                if (lintBenefitApplication > 0 )
                {
                    lobjError = AddError(6053, ""); 
                    this.iarrErrors.Add(lobjError);
                    return false;
                }
               
                this.LoadErrors();
                if (this.ibusSoftErrors.IsNotNull() && !this.ibusSoftErrors.iclbError.IsNullOrEmpty())
                {
                    this.ibusSoftErrors.DeleteErrors();
                }
                icdoPersonAccountBeneficiary.Delete();

                if (lclbPersonAccountBeneficiary.Count == 1)
                {
                    
                    busRelationship lbusRelationship = new busRelationship();
                    lbusRelationship.FindRelationship(icdoPersonAccountBeneficiary.person_relationship_id);
                    if (lbusRelationship.icdoRelationship.dependent_person_id == 0)
                    {
                        lbusRelationship.icdoRelationship.Delete();
                    }
                    else
                    {
                        lbusRelationship.icdoRelationship.beneficiary_person_id = 0;
                        lbusRelationship.icdoRelationship.Update();
                    }
                }

                if (ibusParticipant != null)
                {
                    ibusParticipant.LoadBeneficiaries();

                    if (ibusParticipant.iclbPersonAccountBeneficiary != null && ibusParticipant.iclbPersonAccountBeneficiary.Count > 0)
                    {
                        //iclbPersonAccountBeneficiary = lbusPerson.iclbPersonAccountBeneficiary;
                        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in ibusParticipant.iclbPersonAccountBeneficiary)
                        {
                            if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                            {
                                busPersonAccount lbusPersonAccount = new busPersonAccount();
                                lbusPersonAccount.FindPersonAccount(lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id);
                                lbusPersonAccountBeneficiary.iaintMainPersonID = ibusParticipant.icdoPerson.person_id;
                                lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan = lbusPersonAccount.icdoPersonAccount.plan_id;
                                lbusPersonAccountBeneficiary.ValidateSoftErrors();
                                lbusPersonAccountBeneficiary.FindPersonAccountBeneficiary(lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id);
                                lbusPersonAccountBeneficiary.UpdateValidateStatus();
                            }
                        }
                    }
                }

            }
            else
            {
                busRelationship lbusRelationship = new busRelationship();
                lbusRelationship.FindRelationship(icdoPersonAccountBeneficiary.person_relationship_id);
                if (lbusRelationship.icdoRelationship.dependent_person_id == 0)
                {
                    lbusRelationship.icdoRelationship.Delete();
                }
                else
                {
                    lbusRelationship.icdoRelationship.beneficiary_person_id = 0;
                    lbusRelationship.icdoRelationship.Update();
                }
                if (ibusParticipant != null)
                {
                    ibusParticipant.LoadBeneficiaries();
                }
            }

            return base.ValidateDelete();
        }


        #endregion
    }
}
