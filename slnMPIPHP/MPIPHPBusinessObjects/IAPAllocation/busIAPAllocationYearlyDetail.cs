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
    public class busIAPAllocationYearlyDetail : busPerson
    {
        public Collection<cdoPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }
        public string istrTransactionType { get; set; }
        public string istrContributionType { get; set; }
        public string istrContributionSubType { get; set; }
        public int lintPersonAccountId { get; set; }
        public int lintComputationYear { get; set; }
        public string istrAffiliate { get; set; }
        public string istrSpecialAccount { get; set; }
        //PIR 0989 added new variable for third parameter in query.
        public string istrIsPayment { get; set; }


        //PIR 0989 added third parameter in signature.
        public void LoadYearlyDetail(int aintPersonAccountId, int aintComputationYear, string astrIsPayment, string astrSpecialAccount = "")
        {
            //PIR 0989 added third parameter in query.
            DataTable ldtbYearlyDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationYearlyDetail", new object[3] { aintPersonAccountId, aintComputationYear, astrIsPayment });
            iclbPersonAccountRetirementContribution = new Collection<cdoPersonAccountRetirementContribution>();
            istrIsPayment = astrIsPayment;

            if (ldtbYearlyDetail.Rows.Count > 0)
            {
                iclbPersonAccountRetirementContribution = cdoPersonAccountRetirementContribution.GetCollection<cdoPersonAccountRetirementContribution>(ldtbYearlyDetail);
            }
            if (astrSpecialAccount.IsNotNullOrEmpty())
            {
                istrSpecialAccount = astrSpecialAccount;
                if (istrSpecialAccount == "L52")
                {
                    foreach (cdoPersonAccountRetirementContribution lcdoPersonAccountCont in iclbPersonAccountRetirementContribution)
                    {
                       lcdoPersonAccountCont.iap_balance_amount= lcdoPersonAccountCont.local52_special_acct_bal_amount;
                    }

                }
                else if (istrSpecialAccount == "L161")
                {
                    foreach (cdoPersonAccountRetirementContribution lcdoPersonAccountCont in iclbPersonAccountRetirementContribution)
                    {
                       lcdoPersonAccountCont.iap_balance_amount= lcdoPersonAccountCont.local161_special_acct_bal_amount ;
                    }

                }
            }
        }

        public ArrayList btn_ApplyFilter()
        {
            ArrayList larrList = new ArrayList();
            //PIR 0989 added third parameter in query.
            DataTable ldtbYearlyDetail = busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationYearlyDetail", new object[3] { lintPersonAccountId, lintComputationYear, istrIsPayment });
            iclbPersonAccountRetirementContribution = new Collection<cdoPersonAccountRetirementContribution>();

            if (ldtbYearlyDetail.Rows.Count > 0)
            {
                iclbPersonAccountRetirementContribution = cdoPersonAccountRetirementContribution.GetCollection<cdoPersonAccountRetirementContribution>(ldtbYearlyDetail);
                if (!iclbPersonAccountRetirementContribution.IsNullOrEmpty())
                {
                    if (istrSpecialAccount.IsNotNullOrEmpty())
                    {
                        if (istrSpecialAccount == "L52")
                        {
                            foreach (cdoPersonAccountRetirementContribution lcdoPersonAccountCont in iclbPersonAccountRetirementContribution)
                            {
                                lcdoPersonAccountCont.iap_balance_amount = lcdoPersonAccountCont.local52_special_acct_bal_amount;
                            }
                        }
                        else if (istrSpecialAccount == "L161")
                        {
                            foreach (cdoPersonAccountRetirementContribution lcdoPersonAccountCont in iclbPersonAccountRetirementContribution)
                            {
                                lcdoPersonAccountCont.iap_balance_amount = lcdoPersonAccountCont.local161_special_acct_bal_amount;
                            }
                        }
                    }
                }
            }

            if (istrTransactionType.IsNotNullOrEmpty())
            {
                this.iclbPersonAccountRetirementContribution = this.iclbPersonAccountRetirementContribution.Where(item=>item.transaction_type_value==istrTransactionType).OrderBy(item=>item.transaction_type_value).ToList().ToCollection();
            }
            if (istrContributionSubType.IsNotNullOrEmpty())
            {
                this.iclbPersonAccountRetirementContribution = this.iclbPersonAccountRetirementContribution.Where(item => item.contribution_subtype_value == istrContributionSubType).OrderBy(itemm => itemm.contribution_subtype_value).ToList().ToCollection();
            }
            if (istrContributionType.IsNotNullOrEmpty())
            {
                this.iclbPersonAccountRetirementContribution = this.iclbPersonAccountRetirementContribution.Where(item => item.contribution_type_value == istrContributionType).OrderBy(item => item.contribution_type_value).ToList().ToCollection();
            }

           larrList.Add(this);
           return larrList;
        }
    }
}
