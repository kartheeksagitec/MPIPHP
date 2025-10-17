#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;
using MPIPHP.BusinessObjects;
using Sagitec.Common;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPerson:
	/// Inherited from doPerson, the class is used to customize the database object doPerson.
	/// </summary>
    [Serializable]
	public class cdoPerson : doPerson
	{
		public cdoPerson() : base()
		{
		}

        #region Properties required for Minimum Distribution Batch

        public DateTime idtDatePersonAge70andHalf { get; set; }
        public int iintPlanId { get; set; }
        public string istrPLANCODE { get; set; }

        public string ValidAddress { get; set; }

        #endregion

        public string istrFullName
        {
            get
            {
                return first_name + " " + last_name;
            }
        }
        public string istrFullName1
        {

            get
            {
                return first_name + " " + last_name;
            }
        }
        #region PROD PIR 822
        public string istrPayeeFullName
        {
            get {
                if (middle_name == null || middle_name == "" || string.IsNullOrEmpty(middle_name))
                    middle_name = string.Empty;
                else
                    middle_name = middle_name + " ";
                return first_name + " " + middle_name + last_name;
            }
        }

        public string istrParticipantFullName
        {
            get
            {
                if (middle_name == null || middle_name == "" || string.IsNullOrEmpty(middle_name))
                    middle_name = string.Empty;
                else
                    middle_name = middle_name + " ";
                return first_name + " " + middle_name + last_name;
            }
        }
        #endregion

        public string istrIsMPIPPPersonAccount { get; set; }

        public string istrIsIAPPersonAccount { get; set; }

        public string istrIsLifePersonAccount { get; set; }

        public string istrBenTypeValue { get; set; }

        public string istrRule { get; set; }

        public decimal idecBenPercentage { get; set; }

        public int iintBenId { get; set; }
        //PIR 622
        public int iintOrganisation { get; set; }

        public string iintNewMergedMPIID { get; set; }
        public string istrLast4DigitsofSSN
        {
            get
            {                
                string lstrSSn = string.Empty;

                if (!string.IsNullOrEmpty(istrSSNNonEncrypted) && istrSSNNonEncrypted.Length > 4)
                {
                    //!Should not use ssn property directly as it will manipulate the ssn too, therefore have to copy into a local variable
                    lstrSSn = istrSSNNonEncrypted;
                    lstrSSn = istrSSNNonEncrypted.Remove(0, 5);                   
                }
                return lstrSSn;
            }
        }

        public decimal idecSurvivorAgeAtDeath { get; set; }

        public decimal idecAgeAtEarlyRetirement { get; set; }

        public decimal idecAgeAtRetirment { get; set; }

        public decimal idecParticipantsAgeAsOfCalculationDate { get; set; }

        public decimal idecSurvivorsAgeAsOfCalculationDate { get; set; }

        public string UnionCode { get; set; } //Required to store the Union Code for a Person

        public string istrEmployerName { get; set; }

        public string istrParticipantAddress { get; set; }

        public string analystName { get; set; }
        public int analyst_id { get; set; }

        public int md_option_id { get; set; }

        public string md_description { get; set; }
       // public decimal md_age { get; set; }

        

        //used specifically from the BIS and PreNOtification BIS batch
        public string IS_VALID_ADDRESS_PRESENT { get; set; }
        public string IS_VESTED_IN_MPI { get; set; }

        public DateTime idtDateOfMarriage{get;set;}


        public string istrRelativeVipFlag { get; set; }

        /// <summary>
        /// Being Set To check For the VIPFLAG : FOR the OPEN DIALOG ON LOOKUP
        /// </summary>

        public string istrVipFlagOverview
        {
            get
            {

                if (vip_flag == null || vip_flag == busConstant.FLAG_NO)
                {
                    return busConstant.NO;
                }
                else
                {
                    return busConstant.YES;
                }
            }
            set
            {
                ;
            }
        }

        public string istrPreFix
        {

            get
            {
                string lstrPreFix = name_prefix_description;
                if (string.IsNullOrEmpty(name_prefix_value))
                {
                    if (gender_value == "M")
                    {
                        lstrPreFix = "Mr.";
                    }
                    else
                    {
                        if (marital_status_description == "Single")
                        {
                            lstrPreFix = "Ms";
                        }
                        else
                        {
                            lstrPreFix = "Mrs";
                        }

                    }

                }
                return lstrPreFix;
            }
            set
            {
                ;
            }
        }

        public string istrSpousePrefix
        {
            get
            {
                string lstrSpPrefix = name_prefix_description;
                if (string.IsNullOrEmpty(name_prefix_value))
                {
                    if (gender_value == "M")
                    {
                        lstrSpPrefix = "Mrs.";
                    }
                    else
                    {
                        lstrSpPrefix = "Mr.";
                    }
                }
                else if (name_prefix_value == "MR")
                {
                    lstrSpPrefix = "Mrs.";
                }
                else
                {
                    lstrSpPrefix = "Mr.";
                }
                return lstrSpPrefix;
            }
            set {;}
        }

        public string istrMaritalStatus
        {
            get
            {
                string lstrMaritalStatus = marital_status_value;
                if (!string.IsNullOrEmpty(marital_status_value))
                {
                    if (marital_status_value == "M")
                    {
                        lstrMaritalStatus = "Married";
                    }
                    else
                    {
                        lstrMaritalStatus = "Single";
                    }
                }
                return lstrMaritalStatus;
            }
            set { ; }
        }
        
        public string istrPersonName
        {
            get 
            {
                StringBuilder lstrPersonFullName = new StringBuilder();
                if (!string.IsNullOrEmpty(this.name_prefix_description))
                    lstrPersonFullName.Append(this.name_prefix_description + " ");
                lstrPersonFullName.Append(this.first_name + " ");
                if (!string.IsNullOrEmpty(this.middle_name))
                    lstrPersonFullName.Append(this.middle_name + " ");
                lstrPersonFullName.Append(this.last_name);

                return lstrPersonFullName.ToString();    
              
            }
        }


        public string istrSSNNonEncrypted
        {
            get;
            set;
        }

        public DateTime idtDateofBirth
        {
            get;
            set;
        }

        public override void LoadData(DataRow adtrRow)
        {
            base.LoadData(adtrRow);
            //string lstrDeccryptedDOB = string.Empty;
            //if (!Sagitec.Common.HelperFunction.IsNumeric(this.ssn))
            //{
                this.istrSSNNonEncrypted = this.ssn;
            //}
            //if (!string.IsNullOrEmpty(this.date_of_birth))
            //{
            //    lstrDeccryptedDOB = this.date_of_birth;
            //    if (!string.IsNullOrEmpty(lstrDeccryptedDOB))
            //    {
            //        this.idtDateofBirth = Convert.ToDateTime(lstrDeccryptedDOB);
            //    }
            //}

            this.idtDateofBirth = this.date_of_birth;
        }

        public string istrSSN
        {
            get
            {
                string lstrSsn = string.Empty;
                if (!string.IsNullOrEmpty(istrSSNNonEncrypted))
                {
                    lstrSsn = istrSSNNonEncrypted;
                    lstrSsn = lstrSsn.Remove(0, 5);
                    return "XXX-XX-" + lstrSsn;
                }
                else
                    return lstrSsn;
            }
        }

        /// <summary>
        /// For Death Report : Outbound File
        /// </summary>
        public string istrPersonType { get; set; }
        public string istrEADBFlag { get; set; }
        

        public override bool Select()
        {
            bool lblnReturn = base.Select();
            if (lblnReturn) GetDOBDisplay();
            return lblnReturn;
        }

        public override bool SelectRow(object[] aarrKeyValues, bool ablnLockRow = false)
        {
            bool lblnReturn = base.SelectRow(aarrKeyValues);
            if (lblnReturn) GetDOBDisplay();
            return lblnReturn;
        }

        public override int Insert()
        {
            SetDOB();
            return base.Insert();
        }

        public override int Update()
        {
            SetDOB();
            return base.Update();
        }

        public void GetDOBDisplay()
        {
            string lstrDecryptedDOB = string.Empty;
            //if (!string.IsNullOrEmpty(this.date_of_birth))
            //{
            //    lstrDecryptedDOB = Sagitec.Common.HelperFunction.SagitecDecryptAES(this.date_of_birth);
            //    if (!string.IsNullOrEmpty(lstrDecryptedDOB))
            //    {
                    this.idtDateofBirth = this.date_of_birth;
            //    }
            //}
        }


        public void SetDOB()
        {
            this.date_of_birth = this.idtDateofBirth;
        }

        public DateTime istrDOB
        {
            get
            {
                return idtDateofBirth;
            }
            set
            {
                ;
            }
        }

        public string istrDOD
        {
            get
            {
                string lstrDOD = busGlobalFunctions.ConvertDateIntoDifFormat(this.date_of_death);
                return lstrDOD;
            }
            set 
            {
                ;
            }
        }
        
        public string istrRetireeHealthEligibleFlag
        {
            get
            {
                if (health_eligible_flag == busConstant.FLAG_YES)
                    return "Yes";
                else
                    return "No";
            }
        }

        public string istrAdverseInterestFlag
        {
            get
            {
                if (adverse_interest_flag == busConstant.FLAG_YES)
                    return "Yes";
                else
                    return "No";
            }
        }

        public string istrMPID        
        {
            get
            {
                string lstrMPID = this.mpi_person_id;
                return lstrMPID;
            }
            set
            {
                ;
            }
        }

        public DateTime idtWithdrawalDate
        {
            get;
            set;
        }

        public DateTime idtForfietureDate
        {
            get;
            set;
        }

        public DateTime idtVestingDate
        {
            get;
            set;
        }



        public DateTime idtLastDate
        {
            get;
            set;
        }
        public string istrSelectedFlag { get; set; }

        //Ticket - 68547
        public string istrRelationship { get; set; }

        public string istrMPIPersonID 
        {
            get
            {
                FindByPrimaryKey(this.iintPrimaryKey);
                return this.mpi_person_id;
            }
        }
    } 
} 
