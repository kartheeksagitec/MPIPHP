#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using MPIPHP.Common;
#endregion

namespace MPIPHP.CustomDataObjects
{
    [Serializable]
	public class cdoJobSchedule : doJobSchedule
	{
		public cdoJobSchedule() : base()
		{
		}



        public string StartTimeForOnceUI
        {
            get
            {
                return string.Format("{0:hh:mm:ss tt}", StartTime_For_Once);
            }
            set
            {
                StartTime_For_Once = Convert.ToDateTime("01/01/1900 " + value);
            }
        }

        public string StartTimeForEveryUI
        {
            get
            {
                return string.Format("{0:hh:mm:ss tt}", StartTime_For_Every);
            }
            set
            {
                StartTime_For_Every = Convert.ToDateTime("01/01/1900 " + value);
            }
        }

        public string EndTimeForUI
        {
            get
            {
                return string.Format("{0:hh:mm:ss tt}", end_time);
            }
            set
            {
                end_time = Convert.ToDateTime("01/01/1900 " + value);
            }
        }

        // Properties added for Schedule class 
        public int ActiveStartDate
        {
            get
            {
                return BatchHelper.DateToInt32(start_date);
            }
        }

        //Sid Jain 02222013
        public int ActiveEndDate
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    end_date = DateTime.MaxValue;
                return BatchHelper.DateToInt32(end_date);
            }
        }

        public int ActiveStartTime
        {
            get
            {
                return BatchHelper.TimeToInt32(start_time);
            }
        }

        //Sid Jain 02222013
        public int ActiveEndTime
        {
            get
            {
                if (end_time == DateTime.MinValue)
                    end_time = DateTime.MaxValue;
                return BatchHelper.TimeToInt32(end_time);
            }
        }


        // These properties are added purely for the purpose of UI.
        public int Freq_SubDay_Type_On_Screen { get; set; }

        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int Sunday { get; set; }

        public int UI_Freq_Interval_For_Weekly_Frequency
        {
            get
            {
                return Monday + Tuesday + Wednesday + Thursday + Friday + Saturday + Sunday;
            }
        }

        public DateTime StartTime_For_Once { get; set; }
        public DateTime StartTime_For_Every { get; set; }

        // Only for Daily Frequency Type
        public string lstrSubdayFrequency { get; set; }

        public DateTime ldtOnceStartTime;
        public DateTime ldtEveryStartTime;
        public DateTime ldtEveryEndTime;

        // Prem Added for Duration tab related values only for UI
        // Only for Daily Frequency Type
        public string lstrEndDatePresent { get; set; }

        public int UI_Freq_Interval_For_Daily_Frequency { get; set; }
        public int UI_Freq_Interval_For_Monthly_Frequency { get; set; }
        public int UI_Freq_Interval_For_Monthly_Relative_Frequency { get; set; }

        public int UI_Freq_Rec_Factor_Weekly  { get; set; }
        public int UI_Freq_Rec_Factor_Monthly { get; set; }
        public int UI_Freq_Rec_Factor_Monthly_Relative { get; set; }
	}
}
