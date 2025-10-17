using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.ObjectModel;


[ServiceContract]
public interface IOPUSWebService
{
    //[OperationContract]
    bool IsSSNValid(string istrPrefix, string istrFirstName, string istrLastName, string istrMiddleName, string istrSuffix, string istrDateofBirth,string istrSSN);
   
    //[OperationContract]
    bool AddUpdatePersonAddress(string astrSSN, string astrAddressLine1, string astrAddressLine2, string astrCity, string astrState, string astrZipCode, string astrZipCode4, string astrCountryCode,
        string astrAddressType, string astrAddressEndDate);

    //[OperationContract]
    string GetPersonInformation(string astrSSN);

    //[OperationContract]
    string ReteiveMPIDFromOPUS(string astrSSN);

    [OperationContract]
    List<Person> GetPlanBenefitSummary(string SSN);

    [OperationContract]
    List<BenefitCalculationDetail> GetBenefitCalculationEstimate(string MPID, int BenefitPlanId, DateTime RetirementDate, DateTime SpouseDateOfBirth);

    [OperationContract]
    List<AnnualBenefitSummaryData> GetAnnualBenefitSummary(string astrMpiPersonId);

    [OperationContract]
    List<PayeeAccountBreakdownData> GetPayeeAccountBreakdown(int aintPayeeAccountId);

    [OperationContract]
    List<RetirementProcessTrackerData> GetRetirementProcessTracker(string astrMpiPersonId);

    [OperationContract]
    string GetRetireeHealthEligibilityFlag(string astrMpiPersonId);
}

#region WCF - Website and Mobile App
[DataContract]
public class Person
{
    [DataMember]
    public string Plan { get; set; }
    [DataMember]
    public string PlanStatus { get; set; }
    [DataMember]
    public decimal PensionHours { get; set; }
    [DataMember]
    public int QualifiedYears { get; set; }
    [DataMember]
    public string PensionCredit { get; set; }
    [DataMember]
    public DateTime VestedDate { get; set; }
    [DataMember]
    public string HealthHours { get; set; }
    [DataMember]
    public string HealthYears { get; set; }
    [DataMember]
    public string MonthlyBenefit { get; set; }
    [DataMember]
    public string IAPBalance { get; set; }
    [DataMember]
    public string AllocationAsOfYrEnd { get; set; }
    [DataMember]
    public string Comments { get; set; }
    [DataMember]
    public string MPIPersonId { get; set; }
    [DataMember]
    public bool IsSuccess { get; set; }
}

[DataContract]
public class BenefitCalculationDetail
{
    [DataMember]
    public int BenefitPlanId { get; set; }
    [DataMember]
    public string PlanName { get; set; }
    [DataMember]
    public DateTime VestedDate { get; set; }
    [DataMember]
    public decimal AccruedBenefitAmount { get; set; }
    [DataMember]
    public decimal LifeAnnuityBenefitAmount { get; set; }

    [DataMember]
    public decimal QDROOffset { get; set; }

    [DataMember]
    public decimal EarlyRetirementFactor { get; set; }

    [DataMember]
    public decimal IAPBalanceAmount { get; set; }

    [DataMember]
    public DateTime IAPAsOfDate { get; set; }

    [DataMember]
    public decimal Local52SpecialAccountBalanceAmount { get; set; }

    [DataMember]
    public decimal Local161SpecialAccountBalanceAmount { get; set; }

    [DataMember]
    public string MPIPersonId { get; set; }

    [DataMember]
    public string Comments { get; set; }

    [DataMember]
    public bool IsSuccess { get; set; }
    [DataMember]
    public string RetirementType { get; set; }

    [DataMember]
    public Collection<BenefitCalculationOption> clBenefitCalculationOption { get; set; }
}


[DataContract]
public class BenefitCalculationOption
{
    [DataMember]
    public int BenefitPlanId { get; set; }
    [DataMember]
    public string BenefitOption { get; set; }
    [DataMember]
    public decimal BenefitOptionFactor { get; set; }
    [DataMember]
    public decimal BenefitAmount { get; set; }
    [DataMember]
    public decimal SurvivorAmount { get; set; }

    [DataMember]
    public string RelativeValue { get; set; }

    [DataMember]
    public string MPIPersonId { get; set; }
}

[DataContract]
public class AnnualBenefitSummaryData
{
    [DataMember]
    public int Year { get; set; }
    [DataMember]
    public Decimal CreditedHours { get; set; }
    [DataMember]
    public decimal WithdrawnHours { get; set; }
    [DataMember]
    public int QualifiedYears { get; set; }
    [DataMember]
    public int VestedYears { get; set; }
    //public int bis_years_count { get; set; }
    [DataMember]
    public int NonQualifiedYearCount { get; set; } 
    [DataMember]
    public decimal RetireeHealthHours { get; set; }
    [DataMember]
    public int RetireeHealthYears { get; set; }
    [DataMember]
    public decimal EEContribution { get; set; }
    [DataMember]
    public decimal EEInterest { get; set; }
    [DataMember]
    public decimal UVHPContribution { get; set; }
    [DataMember]
    public decimal UVHPInterest { get; set; }
    [DataMember]
    public decimal AccruedBenefit { get; set; }
    [DataMember]
    public decimal AccruedBenefitLocal { get; set; }
    [DataMember]
    public decimal AccumulatedAccruedBenefit { get; set; }
    [DataMember]
    public string Comments { get; set; }
    [DataMember]
    public bool IsSuccess { get; set; }

}

[DataContract]
public class RetirementProcessTrackerData
{
    [DataMember]
    public string MPIPersonId { get; set; }
    [DataMember]
    public string PlanName { get; set; }
    [DataMember]
    public DateTime ApplicationMailed { get; set; }
    [DataMember]
    public DateTime ApplicationReceived { get; set; }
    [DataMember]
    public DateTime BenefitElectionPacketMailed { get; set; }
    [DataMember]
    public DateTime BenefitElectionPacketReceived { get; set; }
    [DataMember]
    public DateTime PayStatusDate { get; set; }
    [DataMember]
    public string PayStatus { get; set; }
    [DataMember]
    public string Comments { get; set; }
    [DataMember]
    public bool IsSuccess { get; set; }
}

[DataContract]
public class PayeeAccountBreakdownData
{
    [DataMember]
    public int PayeeAccountId { get; set; }
    [DataMember]
    public DateTime LastPaymentDate { get; set; }
    [DataMember]
    public decimal GrossAmount { get; set; }
    [DataMember]
    public DateTime NextPaymentDate { get; set; }
    [DataMember]
    public decimal NextMonthTaxableAmount { get; set; }
    [DataMember]
    public decimal NextMonthNonTaxableAmount { get; set; }
    [DataMember]
    public decimal NextGrossRolloverAmount { get; set; }
    [DataMember]
    public decimal NextNetRolloverAmount { get; set; }
    [DataMember]
    public decimal NextGrossPayment { get; set; }
    [DataMember]
    public decimal RetroAdjustmentAmount { get; set; }
    [DataMember]
    public decimal FederalTaxWithholding { get; set; }
    [DataMember]
    public decimal StateTaxWithholding { get; set; }
    [DataMember]
    public decimal Deductions { get; set; }
    [DataMember]
    public decimal PensionReceivable { get; set; }
    [DataMember]
    public decimal NextNetPayment { get; set; }
    [DataMember]
    public bool IsSuccess { get; set; }
    [DataMember]
    public string Comments { get; set; }
}


#endregion WCF - Website and Mobile App


