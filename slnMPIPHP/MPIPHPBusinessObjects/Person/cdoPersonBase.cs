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
	/// Class MPIPHP.CustomDataObjects.cdoPersonBase:
	/// Inherited from doPersonBase, the class is used to customize the database object doPersonBase.
	/// </summary>
    [Serializable]
	public class cdoPersonBase : doPersonBase
	{
		public cdoPersonBase() : base()
		{
		}

        #region Properties required for Minimum Distribution Batch

        public DateTime idtDatePersonAge70andHalf { get; set; }
        public int iintPlanId { get; set; }
        public string istrPLANCODE { get; set; }

        #endregion

        public string istrFullName
        {
            get
            {
                return first_name + " " + last_name;
            }
        }


        public string istrIsMPIPPPersonAccount { get; set; }

        public string istrIsIAPPersonAccount { get; set; }

        public string istrIsLifePersonAccount { get; set; }

        public string istrBenTypeValue { get; set; }

        public string istrRule { get; set; }

        public decimal idecBenPercentage { get; set; }

        public int iintBenId { get; set; }

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

        public DateTime idtDateOfMarriage { get; set; }
        /// <summary>
        /// Being Set To check For the VIPFLAG : FOR the OPEN DIALOG ON LOOKUP
        /// </summary>
        public string istrVipFlag
        {
            get
            {

                if (vip_flag == null || vip_flag == busConstant.FLAG_NO)
                {
                    return busConstant.FLAG_NO;
                }
                else
                {
                    return busConstant.FLAG_YES;
                }
            }
            set
            {
                ;
            }
        }

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
                        lstrPreFix = "Mrs";
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
            set { ;}
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
           
            this.istrSSNNonEncrypted = this.ssn;
            
            if (this.date_of_birth != DateTime.MinValue)
            {
                this.idtDateofBirth = this.date_of_birth;
            }
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
            if (this.date_of_birth != DateTime.MinValue)
            {
                this.idtDateofBirth = this.date_of_birth;
            }
        }

        public void SetDOB()
        {
            this.date_of_birth = this.idtDateofBirth;
        }

        public string istrDOB
        {
            get
            {
                string lstrDOB = idtDateofBirth.ToString("yyyy/MM/dd");

                return lstrDOB;
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
    }
} 

