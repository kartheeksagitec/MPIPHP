using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MPIPHP.Common
{
    public enum SERVICETYPE
    {
        NORMAL ,
        VESTING
    }

    public enum REPORTING_FREQUENCY
    {
        BIWEEKLY,
        MONTHLY,
        SEMIMONTHLY
    }
    public enum EMPLOYMENT_TYPE
    {
        EMPLOYEE,
        EMPLOYER            
    }

    public enum RETIREMENT_AMOUNT_BUCKET
    {
        PRETAX,
        POSTTAX,
        INTEREST
    }

    public enum LUMPSUM_BENEFICIARY
    {
        ORGANIZATION,
        MEMBER
    }

    public enum BENEFIT_ACCOUNT_USAGE
    {
        CALCULATION,
        REFUND
    }

    public enum PAYMENT_RECIPIENT
    {
        MEMBER,
        BENEFICIARY
    }

    public enum PAYEE_ACCOUNT_USAGE
    {
        CALCULATION,
        REFUND
    }

    // Enumerator used for setting the FrequencyType of a scheduled job.
    [Serializable]
    public enum FrequencyType
    {
        Once = 1,
        Daily = 4,
        Weekly = 8,
        Monthly = 16,
        MonthlyRelative = 32,
        Immediately = 64		// Added as a part of new scheme where if the job object 
        // is passed in with this enumeration then we know that it 
        // should be executed immediately.
    }

    [Serializable]
    [Flags]
    public enum WeeklyFrequencyInterval
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64
    }

    [Serializable]
    public enum MonthlyRelativeFrequencyInterval
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7,
        Day = 8,
        Weekday = 9,
        WeekendDay = 10
    }

    [Serializable]
    public enum FrequencySubDayType
    {
        AtTheSpecifiedTime = 1,
        Seconds = 2,
        Minutes = 4,
        Hours = 8,
        DuringBatchWindow = 16
    }

    [Serializable]
    public enum FrequencyRelativeInterval
    {
        First = 1,
        Second = 2,
        Third = 4,
        Fourth = 8,
        Last = 16
    }

    public struct BenefitDetails
    {
        public decimal TotalServiceCredit;
        public decimal MemberAnnualBenefit;
        public decimal FAS;
        public DateTime RetirementDate;
        public string RetirementAge;
    }
    
    [Serializable()]
    public enum enmCommunicationRecipientGroup
    {
        NotSet = 0,
        OrganizationContact = 1,
        Person = 2,
        InternalUser = 3
    }
}
