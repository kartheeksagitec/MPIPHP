using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busCheckFileData : busMPIPHPBase
    {
        public string istrPayeeName { get; set; }
        public string istrAccountNumber { get; set; }
        public string istrFractionalRoutingNumber { get; set; }
        public decimal idecGrossAmount { get; set; }
        public decimal idecTaxableAmount { get; set; }
        public decimal idecNonTaxableAmount { get; set; }
        public decimal idecStateTaxAmount { get; set; }
        public decimal idecFederalTaxAmount { get; set; }
        public DateTime idtPayDate { get; set; }
        public string istrSSN { get; set; }
        
        public string fbo_co_firstName { get; set; }
        public string fbo_co_LastName { get; set; }
        public busPaymentHistoryDistribution ibusPaymentHistoryDistribution { get; set; }
        public string istrPlanName { get; set; }
        public string istrPlanID { get; set; }
        public void InitializeObjects()
        {
         
            ibusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
        }
        //public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }

        //public busPaymentHistoryDistribution ibusPaymentHistoryDistribution { get; set; }

        //public int iintPersonID { get; set; }

        //public string istrPersonIDOrgCodeID { get; set; }

        //public string istrPersonName { get; set; }

        //public string istrPlanName { get; set; }

        public string istrBenefitType { get; set; }

        //public string istrBenefitOptionDesc { get; set; }

        //public decimal idecTotalRHICAmount { get; set; }

        //public decimal idecGroupHealthPremium { get; set; }

        //public decimal idecMedicarePartDPremium { get; set; }

        //public decimal idecRHICApplied { get; set; }

        //public decimal idecNetPremium { get; set; }

        //#region Payment Section
        //public string istrPayCheckCompDesc1 { get; set; }
        //public decimal idecPayAmount1 { get; set; }
        //public decimal idecPayCurrent1 { get; set; }
        //public decimal idecPayYTD1 { get; set; }

        //public string istrPayCheckCompDesc2 { get; set; }
        //public decimal idecPayAmount2 { get; set; }
        //public decimal idecPayCurrent2 { get; set; }
        //public decimal idecPayYTD2 { get; set; }

        //public string istrPayCheckCompDesc3 { get; set; }
        //public decimal idecPayAmount3 { get; set; }
        //public decimal idecPayCurrent3 { get; set; }
        //public decimal idecPayYTD3 { get; set; }

        //public string istrPayCheckCompDesc4 { get; set; }
        //public decimal idecPayAmount4 { get; set; }
        //public decimal idecPayCurrent4 { get; set; }
        //public decimal idecPayYTD4 { get; set; }

        //public string istrPayCheckCompDesc5 { get; set; }
        //public decimal idecPayAmount5 { get; set; }
        //public decimal idecPayCurrent5 { get; set; }
        //public decimal idecPayYTD5 { get; set; }

        //public string istrPayCheckCompDesc6 { get; set; }
        //public decimal idecPayAmount6 { get; set; }
        //public decimal idecPayCurrent6 { get; set; }
        //public decimal idecPayYTD6 { get; set; }

        //public string istrPayCheckCompDesc7 { get; set; }
        //public decimal idecPayAmount7 { get; set; }
        //public decimal idecPayCurrent7 { get; set; }
        //public decimal idecPayYTD7 { get; set; }

        //public string istrPayCheckCompDesc8 { get; set; }
        //public decimal idecPayAmount8 { get; set; }
        //public decimal idecPayCurrent8 { get; set; }
        //public decimal idecPayYTD8 { get; set; }

        //public string istrPayCheckCompDesc9 { get; set; }
        //public decimal idecPayAmount9 { get; set; }
        //public decimal idecPayCurrent9 { get; set; }
        //public decimal idecPayYTD9 { get; set; }

        //public string istrPayCheckCompDesc10 { get; set; }
        //public decimal idecPayAmount10 { get; set; }
        //public decimal idecPayCurrent10 { get; set; }
        //public decimal idecPayYTD10 { get; set; }
        //#endregion

        //public decimal idecPayTotals { get; set; }

        //#region Deduction Section

        //public string istrDedCheckCompDesc1 { get; set; }
        public decimal idecDedAmount1 { get; set; }
        //public decimal idecDedCurrent1 { get; set; }
        //public decimal idecDedYTD1 { get; set; }

        //public string istrDedCheckCompDesc2 { get; set; }
        public decimal idecDedAmount2 { get; set; }
        //public decimal idecDedCurrent2 { get; set; }
        //public decimal idecDedYTD2 { get; set; }

        //public string istrDedCheckCompDesc3 { get; set; }
        public decimal idecDedAmount3 { get; set; }
        //public decimal idecDedCurrent3 { get; set; }
        //public decimal idecDedYTD3 { get; set; }

        //public string istrDedCheckCompDesc4 { get; set; }
        public decimal idecDedAmount4 { get; set; }
        public string istrDedAmount4 { get; set; }
        //public decimal idecDedCurrent4 { get; set; }
        //public decimal idecDedYTD4 { get; set; }

        //public string istrDedCheckCompDesc5 { get; set; }
        public decimal idecDedAmount5 { get; set; }
        //public decimal idecDedCurrent5 { get; set; }
        //public decimal idecDedYTD5 { get; set; }

        //public string istrDedCheckCompDesc6 { get; set; }
        public decimal idecDedAmount6 { get; set; }
        //public decimal idecDedCurrent6 { get; set; }
        //public decimal idecDedYTD6 { get; set; }

        //public string istrDedCheckCompDesc7 { get; set; }
        //public decimal idecDedAmount7 { get; set; }
        //public decimal idecDedCurrent7 { get; set; }
        //public decimal idecDedYTD7 { get; set; }

        //public string istrDedCheckCompDesc8 { get; set; }
        //public decimal idecDedAmount8 { get; set; }
        //public decimal idecDedCurrent8 { get; set; }
        //public decimal idecDedYTD8 { get; set; }

        //public string istrDedCheckCompDesc9 { get; set; }
        //public decimal idecDedAmount9 { get; set; }
        //public decimal idecDedCurrent9 { get; set; }
        //public decimal idecDedYTD9 { get; set; }

        //public string istrDedCheckCompDesc10 { get; set; }
        //public decimal idecDedAmount10 { get; set; }
        //public decimal idecDedCurrent10 { get; set; }
        //public decimal idecDedYTD10 { get; set; }

        //public string istrDedCheckCompDesc11 { get; set; }
        //public decimal idecDedAmount11 { get; set; }
        //public decimal idecDedCurrent11 { get; set; }
        //public decimal idecDedYTD11 { get; set; }

        //public string istrDedCheckCompDesc12 { get; set; }
        //public decimal idecDedAmount12 { get; set; }
        //public decimal idecDedCurrent12 { get; set; }
        //public decimal idecDedYTD12 { get; set; }

        //public string istrDedCheckCompDesc13 { get; set; }
        //public decimal idecDedAmount13 { get; set; }
        //public decimal idecDedCurrent13 { get; set; }
        //public decimal idecDedYTD13 { get; set; }

        //public string istrDedCheckCompDesc14 { get; set; }
        //public decimal idecDedAmount14 { get; set; }
        //public decimal idecDedCurrent14 { get; set; }
        //public decimal idecDedYTD14 { get; set; }

        //public string istrDedCheckCompDesc15 { get; set; }
        //public decimal idecDedAmount15 { get; set; }
        //public decimal idecDedCurrent15 { get; set; }
        //public decimal idecDedYTD15 { get; set; }

        //public string istrDedCheckCompDesc16 { get; set; }
        //public decimal idecDedAmount16 { get; set; }
        //public decimal idecDedCurrent16 { get; set; }
        //public decimal idecDedYTD16 { get; set; }

        //public string istrDedCheckCompDesc17 { get; set; }
        //public decimal idecDedAmount17 { get; set; }
        //public decimal idecDedCurrent17 { get; set; }
        //public decimal idecDedYTD17 { get; set; }

        //public string istrDedCheckCompDesc18 { get; set; }
        //public decimal idecDedAmount18 { get; set; }
        //public decimal idecDedCurrent18 { get; set; }
        //public decimal idecDedYTD18 { get; set; }

        //public string istrDedCheckCompDesc19 { get; set; }
        //public decimal idecDedAmount19 { get; set; }
        //public decimal idecDedCurrent19 { get; set; }
        //public decimal idecDedYTD19 { get; set; }

        //public string istrDedCheckCompDesc20 { get; set; }
        //public decimal idecDedAmount20 { get; set; }
        //public decimal idecDedCurrent20 { get; set; }
        //public decimal idecDedYTD20 { get; set; }

        //#endregion

        //public decimal idecDedTotals { get; set; }

        //public decimal idecNetAmount { get; set; }

        //public string istrAmountInWords { get; set; }

        public string istrLabel {get;set;}
      
    }
}
