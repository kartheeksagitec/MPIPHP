using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busPayment1099rFile : busMPIPHPBase
    {
        public cdoPayment1099r icdoPayment1099r { get; set; }

        //Property to load record sequence number

        public string istrRecordSequenceNumber { get; set; }

        //Prop to load Name Control
        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoPayment1099r.org_id);
        }

        public string istrNameControl
        { 
            get
            {

                //if (ibusPerson == null)
                //    LoadPerson();
                if (!string.IsNullOrEmpty(icdoPayment1099r.name))
                {
                    icdoPayment1099r.name = icdoPayment1099r.name.Trim();

                    string LastName = "";
                    if (icdoPayment1099r.name.Contains(' '))
                    {
                        string temp = string.Empty;
                        int count = icdoPayment1099r.name.Split(' ').Count();
                        if (count > 1)
                        {
                            temp = (icdoPayment1099r.name.Split(' '))[count - 1];
                        }
                        else
                        {
                            temp = (icdoPayment1099r.name.Split(' '))[0];
                        }

                        LastName = temp.Length > 4 ?
                        temp.Substring(0, 4).ToUpper() : temp.ToUpper();
                    }
                    else
                    {
                        LastName = icdoPayment1099r.name.Length > 4 ?
                        icdoPayment1099r.name.Substring(0, 4).ToUpper() : icdoPayment1099r.name.ToUpper();
                    }
                    return LastName;

                }
                else
                    return string.Empty;

            }
        }

        public string distribution_percentage { get; set; }
        //Prop  to load address state value
        public string addr_state { get; set; }
        //Prop  to laod payee account id
        public string istrMpiId { get; set; }
        public string PayeeName { get; set; }
        public int PersonID { get; set; }
        public string istrTotalDistributionIndicator { get; set; }
        public string istrTaxAmountNotDefined { get; set; }
        //Prop to Load Type of TIN
        public string istrTypeOfTIN { get; set; }
        public string istrForeignEntityIndicator { get; set; }
        //Prop to Load TIN(ssn or Federal ID
        public string istrTIN { get; set; }
        public string istrCorrectedIndicator { get; set; }
        public string istrPayerTaxNo { get; set; }

        //Prop to Load person data
        public busPerson ibusPerson { get; set; }
        //Load Person
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoPayment1099r.person_id);
        }
        //Prop to Load member contributions
        public decimal idecMemberContributions { get; set; }

        //Prop to Load Payee Account Info
        public busPayeeAccount ibusPayeeAccount { get; set; }

        //Load Payee Account
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPayment1099r.payee_account_id);
        }

        //formatted amount fields
        public string istrFormattedGrossAmount
        {
            get
            {
                int lintGrossAmount = Convert.ToInt32(icdoPayment1099r.gross_benefit_amount * 100);
                return lintGrossAmount.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedTaxableAmount
        {
            get
            {
                int lintTaxableAmount = Convert.ToInt32(icdoPayment1099r.taxable_amount * 100);
                return lintTaxableAmount.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedCapitalGain
        {
            get
            {
                //capital gain ask 
                int lintCapitalGain = Convert.ToInt32(icdoPayment1099r.non_taxable_amount * 100);
                return lintCapitalGain.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedFedTaxWithheld
        {
            get
            {
                int lintFedTaxWithheld = Convert.ToInt32(icdoPayment1099r.fed_tax_amount * 100);
                return lintFedTaxWithheld.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedNonTaxableAmount
        {
            get
            {
                int lintNonTaxableAmount = Convert.ToInt32(icdoPayment1099r.non_taxable_amount * 100);
                return lintNonTaxableAmount.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedMemberContributions
        {
            get
            {
                int lintMemberContributions = Convert.ToInt32(icdoPayment1099r.gross_benefit_amount * 100);
                return lintMemberContributions.ToString().PadLeft(12, '0');
            }
        }
        public string istrFormattedStateTax
        {
            get
            {
                int lintStateTax = Convert.ToInt32(icdoPayment1099r.state_tax_amount * 100);
                return lintStateTax.ToString().PadLeft(12, '0');
            }
        }
    }
}
