#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPersonAccountBeneficiary:
	/// Inherited from doPersonAccountBeneficiary, the class is used to customize the database object doPersonAccountBeneficiary.
	/// </summary>
    [Serializable]
    public class cdoPersonAccountBeneficiary : doPersonAccountBeneficiary
    {
        public cdoPersonAccountBeneficiary()
            : base()
        {
        }

        public string istrPlan { get; set; }
        private int _aintPlanId;
        public int iaintPlan
        {
            get { return _aintPlanId; }
            set
            {
                _aintPlanId = value;
                if (value == 0)
                    istrPlan = "";
                else
                {
                    DataTable ldtbPlan = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan", null);
                    if (ldtbPlan.Rows.Count > 0 && value!=0)
                    {
                        DataRow[] ldtrRow = ldtbPlan.Select("PLAN_ID=" + value.ToString());
                        if(ldtrRow.Length>0)
                            istrPlan = ldtrRow[0]["PLAN_NAME"].ToString();
                    }
                }
            }
        }

        //public int iaintPlan { get; set; }

        public string istrBenFullName { get; set; }

        public int iaintBenID { get; set; }

        public DateTime dtdateOfbirth { get; set; }

        public DateTime dtdateOfdeath { get; set; }

        public DateTime dtdateOfMarriage { get; set; }

        public decimal idecage { get; set; }
        
        //uat pir 170
        public string istrBenMPID { get; set; }

        public DateTime istrDOB { get; set; }

        public int iaintBenPersonID { get; set; }

        public int iaintBenOrgID { get; set; }

        public string istrRelationShipValue { get; set; }

        public string istrBenefeficiaryFromValue { get; set; }
        public int iaintBeneficiaryOf { get; set; }

        public DateTime idtBenDateOfDeath { get; set; }

        public string istrBenLastName { get; set; }
        public string istrNamePrefixValue { get; set; }
        
        public int istrPersonId { get; set; }
        //public long iintAPrimaryKey { get; set; }
    }
} 
