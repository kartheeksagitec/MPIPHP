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
using System.Linq;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busMSSPlan : busPersonAccount
    {

        public Collection<busPersonAccountBeneficiary> iclbBeneficiaryPlans { get; set; }

        public string istrReemployedAsOfDate { get; set; }
        public DateTime idtReemployedAsOfDate { get; set; }
        public busPersonOverview ibusPersonOverview { get; set; }
        public string istrIsRetiree { get; set; }
        public bool iblnIsRetireeUnderPlan { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public bool iblnShowDirectDepositFrom { get; set; }


        public void LoadBeneficiariesForPersonAccount(int aintPersonAccountID)
        {
            DataTable ldtTable = busBase.Select("cdoMssBenefitCalculationHeader.GetBeneficiariesForPlan", new object[1] { aintPersonAccountID });
            iclbBeneficiaryPlans = GetCollection<busPersonAccountBeneficiary>(ldtTable, "icdoPersonAccountBeneficiary");
        }

        public void CheckReemploymentStatus(int aintPersonAccountID)
        {
            DataTable ldtTable = busBase.Select("cdoMssBenefitCalculationHeader.CheckReemploymentStatus", new object[1] { aintPersonAccountID });
            if (ldtTable.IsNotNull() && ldtTable.Rows.Count > 0)
            {
                istrReemployedAsOfDate = Convert.ToString(ldtTable.Rows[0][enmPayeeAccount.reemployed_flag.ToString()]);
                idtReemployedAsOfDate = Convert.ToDateTime(ldtTable.Rows[0][enmPayeeAccount.reemployed_flag_as_of_date.ToString()]);
                if (istrReemployedAsOfDate == busConstant.FLAG_YES)
                {
                    istrReemployedAsOfDate = busConstant.YES;
                }
            }
        }

        public busPayeeAccount GetPayeeAccountObject(int aintpersonid, int aintPlanID)
        {
            busPayeeAccount lbusPayeeAccount = null;
            iblnIsRetireeUnderPlan = false;
            ibusPayeeAccount = null;
            if (aintPlanID != 1)
            {
                DataTable ldtPayeeAccount = busBase.Select("cdoMssBenefitCalculationHeader.GetPensionPayeeAccountForParticipant", new object[2] { aintpersonid, aintPlanID });
                if (ldtPayeeAccount.IsNotNull() && ldtPayeeAccount.Rows.Count > 0)
                {
                    iblnIsRetireeUnderPlan = true;

                    lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(ldtPayeeAccount.Rows[0]);
                    lbusPayeeAccount.LoadBenefitDetails();
                    lbusPayeeAccount.LoadGrossAmount();
                    ibusPayeeAccount = lbusPayeeAccount;
                }
            }
            else
            {
                if (this.icdoPersonAccount.status_value == "RETR")
                {
                    iblnIsRetireeUnderPlan = true;
                    CheckIAPBalance();
                }
            }
            return lbusPayeeAccount;
        }


        public void CheckIAPBalance()
        {
            if (this.icdoPersonAccount.plan_id == 1)
            {
                iblnShowDirectDepositFrom = false;
                DataTable ldtbIAPBalance = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPBalanceForYear",
                                     new object[1] { this.icdoPersonAccount.person_account_id
                                    });
                if (ldtbIAPBalance.Rows.Count > 0)
                {
                    if (Convert.ToString(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"]) > 0)
                        {
                            iblnShowDirectDepositFrom = true;
                        }
                    }

                }
            }
            else
            {
                iblnShowDirectDepositFrom = true;
            }
        }
     




    }
}
