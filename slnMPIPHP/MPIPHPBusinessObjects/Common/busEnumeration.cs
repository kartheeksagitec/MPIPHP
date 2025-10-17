using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public enum enmAppendCharacter
    {
        Comma = 1,
        Space = 2,
        CommaSpace = 3,
        Dash = 4
    }

    [Serializable]
    public enum enmRecurringFrequencyType
    {
        Daily = 4,
        Weekly = 8,
        Monthly = 16
    }

    [Serializable]    
    public enum enmWeeklyFrequencyInterval
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
    public enum enmMonthlyDBRelativeWeekInterval
    {
        First = 1,
        Second = 2,
        Third = 4,
        Fourth = 8,
        Last = 16
    }

    [Serializable]
    public enum enmMonthlyActualRelativeWeekInterval
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Last = 5
    }

    [Serializable]
    public enum enmExportFormat
    {
        PDF = 1,
        EXCEL = 2,
        CSV = 3
    }

    [Serializable]
    public enum enmLogiFolderType
    {
        Public = 0,
        Personal = 1,
        All = 2,
        Linked = 3
    }

    [Serializable]
    public enum enmDateInterval
    {
        Day,
        Month,
        Year
    }

    [Serializable]
    public struct stctDateDifference
    {
        public int InDays;
        public int InMonths;
        public int InYears;

        public stctDateDifference(int aInDays, int aInMonths, int aInYears)
        {
            InDays = aInDays;
            InMonths = aInMonths;
            InYears = aInYears;
        }
    }

    [Serializable]
    public enum enmAddressTable
    {
        SGT_PERSON_ADDRESS,
        SGT_ORG_ADDRESS,
        SGT_PERSON_REQUEST
    }

    [Serializable]
    public enum enmTreatErrorAs
    {
        HardError = 0,
        SoftError = 1
    }

    [Serializable]
    public enum enmPlanIndicator
    {
        PLAN_1 = 1,
        PLAN_2 = 2
    }
}
