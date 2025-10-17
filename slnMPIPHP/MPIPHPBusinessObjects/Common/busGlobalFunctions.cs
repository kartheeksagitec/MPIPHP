using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Web;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using MPIPHP.Common;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Data.SqlClient;
using MPIPHP.DataObjects;
using System.Windows.Forms;
using System.Linq;
using System.Data.SqlTypes;
using System.Xml.Linq;
//using Microsoft.Office.Interop.Word;

namespace MPIPHP.BusinessObjects
{
    public static class busGlobalFunctions
    {
        #region [Transaction]

        //public static int GetOrgIdFromOrgCode(string lstrOrgCode)
        //{
        //    int lintOrgId = 0;
        //    if (String.IsNullOrEmpty(lstrOrgCode)) return lintOrgId;
        //    DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { "org_code" },
        //          new object[1] { lstrOrgCode }, null, null);
        //    if (ldtbOrganization.Rows.Count > 0)
        //    {
        //        lintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0]["org_id"]);
        //    }
        //    return lintOrgId;
        //}

        /// <summary>
        /// Determines whether the string is not null or empty.
        /// </summary>
        /// <returns>Boolean indicating whether the string is not null or not empty</returns>
        public static bool IsNotNullOrEmpty(this string astrText)
        {
            return !String.IsNullOrEmpty(astrText);
        }

        /// <summary>
        /// Determines whether a string is null or empty
        /// </summary>
        /// <returns>True if string is null or empty. False if not null or not empty</returns>
        public static bool IsNullOrEmpty(this string astrText)
        {
            return string.IsNullOrEmpty(astrText);
        }

        #endregion

        #region Common Methods

        public static DataTable ExecuteSPtoGetDataTable(string storedProcedureName, string ConnectionString, utlPassInfo adbConnection = null, params SqlParameter[] arrParam)
        {
            DataTable dt = new DataTable();

            if (adbConnection.IsNotNull())
            {
                //IDbCommand cmd = adbConnection.iconFramework.CreateCommand();
                //cmd.Parameters.Clear();
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = storedProcedureName;
                //cmd.CommandTimeout = 0;

                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                // Handle the parameters 
                if (arrParam != null)
                {
                    foreach (SqlParameter param in arrParam)
                    {
                        IDbDataParameter x = DBFunction.GetDBParameter();
                        x.ParameterName = param.ParameterName;
                        x.DbType = param.DbType;
                        x.Value = param.Value;
                        lcolParameters.Add(x);
                    }
                }

                IDataReader ldr = DBFunction.DBExecuteProcedureResult(storedProcedureName, lcolParameters, adbConnection.iconFramework, adbConnection.itrnFramework);
                dt.Load(ldr);
            }
            else
            {
                // Open the connection 
                //using (SqlConnection cnn = new SqlConnection("Data Source=.\sqlexpress;Initial Catalog=AcmeRentals;Integrated Security=True")) 
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    try
                    {

                        cnn.Open();

                        // Define the command 
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Parameters.Clear();
                            cmd.Connection = cnn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = storedProcedureName;
                            cmd.CommandTimeout = 0;

                            // Handle the parameters 
                            if (arrParam != null)
                            {
                                foreach (SqlParameter param in arrParam)
                                    cmd.Parameters.Add(param);
                            }

                            // Define the data adapter and fill the dataset 
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                try
                                {
                                    if (da != null)
                                        da.Fill(dt);
                                    if (cmd.Parameters.Contains("@RETURN_VALUE"))
                                    {
                                        foreach (SqlParameter param in arrParam)
                                        {
                                            if (param.ParameterName == "@RETURN_VALUE")
                                            {
                                                param.Value = cmd.Parameters["@RETURN_VALUE"].Value;
                                            }

                                        }
                                    }


                                }
                                catch
                                {
                                    cmd.Parameters.Clear();
                                    return dt;
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        cnn.Dispose();
                    }
                }

            }
            return dt;
        }



        /// <summary>
        /// Check whether the e-mail is having valid 
        /// </summary>
        /// <param name="astrEmail"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string astrEmail)
        {
            Regex lrexEmail = new Regex(@"(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})");
            return (lrexEmail.IsMatch(astrEmail));
        }

        /// <summary>
        /// Check whether two date range overlaps each other.
        /// </summary>
        /// <param name="adtStart1"></param>
        /// <param name="adtEnd1"></param>
        /// <param name="adtStart2"></param>
        /// <param name="adtEnd2"></param>
        /// <returns></returns>
        public static bool IsDateRangeOverlaps(DateTime adtStart1, DateTime adtEnd1, DateTime adtStart2, DateTime adtEnd2)
        {
            bool lblnResult = false;
            DateTime sstart = adtStart1 > adtStart2 ? adtStart1 : adtStart2; // max of starts 
            DateTime send = adtEnd1 < adtEnd2 ? adtEnd1 : adtEnd2; // min of ends 
            if (sstart <= send)
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// Convert string to ProperCase
        /// </summary>
        /// <param name="astrInput">Input String</param>
        /// <returns>Propercase string</returns>
        public static string ToProperCase(this string astrInput)
        {
            if (string.IsNullOrWhiteSpace(astrInput))
                return string.Empty;
            else
            {
                CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                TextInfo lobjTextInfo = cultureInfo.TextInfo;
                return lobjTextInfo.ToTitleCase(astrInput.ToLower());
            }
        }

        /// <summary>
        /// Format String input to remove SQL Injection charaacters.
        /// </summary>
        /// <param name="astrInput">Input String</param>
        /// <returns>Safe String</returns>
        public static string FormatStringtoPreventSQLInjection(string astrInput)
        {
            string lstrOutput = astrInput.Trim().Replace("'", "''");
            return lstrOutput;
        }

        public static int DateDiffByMonth(DateTime adtStartDate, DateTime adtEndDate)
        {
            adtStartDate = new DateTime(adtStartDate.Year, adtStartDate.Month, 1);
            adtEndDate = new DateTime(adtEndDate.Year, adtEndDate.Month, 1);

            //Calculate Total Months Difference
            int lintTotalDueMonths = 0;
            while (adtStartDate < adtEndDate)
            {
                lintTotalDueMonths++;
                adtEndDate = adtEndDate.AddMonths(-1);
            }

            return lintTotalDueMonths;
        }

        public static DateTime GetLastDayofMonth(this DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                return new DateTime(adtDateTime.Year, adtDateTime.Month, DateTime.DaysInMonth(adtDateTime.Year, adtDateTime.Month));
            }
            return adtDateTime;
        }

        /// <summary>
        /// Method to get the first day of the month
        /// </summary>
        /// <param name="adtDateTime">Date</param>
        /// <returns>First date of the month</returns>
        public static DateTime GetFirstDayofMonth(this DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                return new DateTime(adtDateTime.Year, adtDateTime.Month, 1);
            }
            return adtDateTime;
        }

        public static int DateDiffInDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            TimeSpan ltsTimeSpan = adtEndDate - adtStartDate;
            return ltsTimeSpan.Days;
        }
        /// <summary>
        /// Function to return LastDateMonth for the Given Date.
        /// </summary>		
        /// <param name="adtDate">Date</param>
        /// <returns>DateTime</returns>       
        /// <summary>
        /// Function to Validate the Given Begin, End Date Overlapping with other Begin and End Date
        /// </summary>		
        /// <param name="adtGivenDate">Given Date</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>bool</returns>
        public static bool CheckDateOverlapping(DateTime adtGivenStartDate, DateTime adtGivenEndDate, DateTime adtStartDate, DateTime adtEndDate)
        {
            bool lblnStartDateResult = CheckDateOverlapping(adtGivenStartDate, false, adtStartDate, adtEndDate);
            bool lblnEndDateResult = CheckDateOverlapping(adtGivenEndDate, true, adtStartDate, adtEndDate);
            if ((lblnStartDateResult) || (lblnEndDateResult))
                return true;
            return false;
        }
        public static Collection<busActivityInstance> LoadRunningWorkflowByDocumentAndOrgCode(string astrOrgCode, string astrDocumentCode)
        {
            Collection<busActivityInstance> lclbActivityInstance = new Collection<busActivityInstance>();
            DataTable ldtpActivityInstance = busMPIPHPBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndOrgCode",
                new object[2] { astrOrgCode, astrDocumentCode });
            busBase lobjBase = new busBase();
            lclbActivityInstance = lobjBase.GetCollection<busActivityInstance>(ldtpActivityInstance, "icdoActivityInstance");
            return lclbActivityInstance;
        }
        public static Collection<busActivityInstance> LoadRunningWorkflowByDocumentAndPerson(int aintPersonID, string astrDocumentCode)
        {
            Collection<busActivityInstance> lclbActivityInstance = new Collection<busActivityInstance>();
            DataTable ldtpActivityInstance = busMPIPHPBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndPerson",
                new object[2] { aintPersonID, astrDocumentCode });
            busBase lobjBase = new busBase();
            lclbActivityInstance = lobjBase.GetCollection<busActivityInstance>(ldtpActivityInstance, "icdoActivityInstance");
            return lclbActivityInstance;
        }

        public static Collection<busActivityInstance> LoadRunningWorkflowByPerson(int aintPersonID)
        {
            Collection<busActivityInstance> lclbActivityInstance = new Collection<busActivityInstance>();
            DataTable ldtpActivityInstance = busMPIPHPBase.Select("cdoActivityInstance.LoadRunningWorkflowbyPerson",
                new object[1] { aintPersonID });
            busBase lobjBase = new busBase();
            lclbActivityInstance = lobjBase.GetCollection<busActivityInstance>(ldtpActivityInstance, "icdoActivityInstance");
            return lclbActivityInstance;
        }

        public static cdoCodeValue GetCodeValueByDescription(int aintCodeId, string astrDescription)
        {
            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(
               new string[2] { "code_id", "description" },
               new object[2] { aintCodeId, astrDescription }, null, null);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lcdoCodeValue.LoadData(ldtbCodeValue.Rows[0]);
            }
            return lcdoCodeValue;
        }

        public static cdoCodeValue GetCodeValueDescriptionByValue(int aintCodeId, string astrCodeValue)
        {
            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(
               new string[2] { "code_id", "code_value" },
               new object[2] { aintCodeId, astrCodeValue }, null, null);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lcdoCodeValue.LoadData(ldtbCodeValue.Rows[0]);
            }
            return lcdoCodeValue;
        }
        /// <summary>
        /// Function to Validate the Given Date is Overlapping with other dates
        /// </summary>		
        /// <param name="adtGivenDate">Given Date</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>bool</returns>
        public static bool CheckDateOverlapping(DateTime adtGivenDate, DateTime adtStartDate, DateTime adtEndDate)
        {
            return CheckDateOverlapping(adtGivenDate, true, adtStartDate, adtEndDate);
        }

        public static bool CheckDateOverlapping(DateTime adtGivenDate, DateTime? adtStartDate, DateTime? adtEndDate)
        {
            DateTime ldtStartDate = DateTime.MinValue;
            if (adtStartDate.HasValue)
                ldtStartDate = adtStartDate.Value;

            DateTime ldtEndDate = DateTime.MaxValue;
            if (adtEndDate.HasValue)
                ldtEndDate = adtEndDate.Value;

            return CheckDateOverlapping(adtGivenDate, true, ldtStartDate, ldtEndDate);
        }

        public static bool CheckDateOverlapping(DateTime adtGivenDate, bool IsGivenDateSetToMaxValue, DateTime adtStartDate, DateTime adtEndDate)
        {

            //Given Date is NULL, based on the FLAG (IsGivenDateSetToMaxValue), it will update the 
            //Given Date into MIN VALUE or MAX VALUE
            if (((adtGivenDate == null) || (adtGivenDate == DateTime.MinValue))
                && IsGivenDateSetToMaxValue)
            {
                adtGivenDate = DateTime.MaxValue;
            }
            else if (((adtGivenDate == null) || (adtGivenDate == DateTime.MaxValue))
                && (!IsGivenDateSetToMaxValue))
            {
                adtGivenDate = DateTime.MinValue;
            }

            //if End Date is NULL, making it MaxValue 
            if (adtEndDate == DateTime.MinValue)
            {
                adtEndDate = DateTime.MaxValue;
            }

            if ((adtGivenDate.Date >= adtStartDate.Date) && (adtGivenDate.Date <= adtEndDate.Date))
            {
                return true;
            }

            return false;
        }


        public static int CalulateAge(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            if (adtDateOfBirth == DateTime.MinValue || adtAgeCalculationDate == DateTime.MinValue)
                return 0;

            int years = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if (adtAgeCalculationDate.Month < adtDateOfBirth.Month ||
                 (adtAgeCalculationDate.Month == adtDateOfBirth.Month && adtAgeCalculationDate.Day < adtDateOfBirth.Day))
            {
                years--;
            }
            return years;
        }

        public static decimal CalculatePersonAge(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            if (adtDateOfBirth == DateTime.MinValue || adtAgeCalculationDate == DateTime.MinValue)
                return 0;

            decimal lPersonAge;
            int lyear = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
            DateTime lCompleteLastYear = adtDateOfBirth.AddYears(lyear);
            if (lCompleteLastYear > adtAgeCalculationDate)
            {
                lCompleteLastYear = lCompleteLastYear.AddYears(-1);
                lyear--;
            }

            lPersonAge = Convert.ToDecimal(lyear);
            //decimal ldays = (adtAgeCalculationDate - lCompleteLastYear).Days;
            //lPersonAge = lyear + (ldays / 365);
            //lPersonAge = Math.Round(lPersonAge, 4);
            return lPersonAge;
        }

        /// <summary>
        /// MPI Age Calculation Based On Months .
        /// </summary>
        /// <param name="adtDateOfBirth"></param>
        /// <param name="adtAgeCalculationDate"></param>
        /// <returns></returns>
        public static decimal CalculatePersonAgeInDec(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            if (adtDateOfBirth == DateTime.MinValue || adtAgeCalculationDate == DateTime.MinValue)
                return 0;
            //PIR 1035         
            decimal lPersonAge = decimal.Zero;
          
            if (adtAgeCalculationDate.Day >= adtDateOfBirth.Day && adtAgeCalculationDate.Month >= adtDateOfBirth.Month)
            {
                lPersonAge = Math.Round(Convert.ToDecimal((adtAgeCalculationDate.Year - adtDateOfBirth.Year) + Convert.ToDecimal((adtAgeCalculationDate.Month - adtDateOfBirth.Month)) / 12), 4);
            }
            else
            {
                lPersonAge = Math.Round(Convert.ToDecimal((adtAgeCalculationDate.Year - adtDateOfBirth.Year - 1) + Convert.ToDecimal((12 + adtAgeCalculationDate.Month - 1 - adtDateOfBirth.Month)) / 12), 4);
            }

            /*
            decimal lPersonAge = decimal.Zero;
            int lyear = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
            DateTime lCompleteLastYear = adtDateOfBirth.AddYears(lyear);
            if (lCompleteLastYear > adtAgeCalculationDate)
            {
                lCompleteLastYear = lCompleteLastYear.AddYears(-1);
                lyear--;
            }
            lPersonAge = Convert.ToDecimal(lyear);
            int lintMonths = GetMonthsBetweenTwoDates(lCompleteLastYear, adtAgeCalculationDate);
            lPersonAge += Convert.ToDecimal(lintMonths) / 12;
            lPersonAge = Math.Round(lPersonAge, 4);*/

            return lPersonAge;
        }

        public static int GetMonthsBetweenTwoDates(DateTime adtStartDate, DateTime adtEndDate)
        {
            if (adtStartDate == DateTime.MinValue || adtStartDate == DateTime.MinValue)
                return 0;
            int lintMonths = 0;
            if (adtStartDate.Year == adtEndDate.Year)
            {
                lintMonths = adtEndDate.Month - adtStartDate.Month;
                //DateTime ldtCompleteMonth = adtStartDate.AddMonths(lintMonths); //PIR 616: This is modified as it should calculte month from date 21 to 20 and not 21 to 21
                DateTime ldtCompleteMonth = adtStartDate.AddMonths(lintMonths).AddDays(-1);
                if (ldtCompleteMonth > adtEndDate)
                    lintMonths--;
            }
            else
            {
                int lintYearDiff = adtEndDate.Year - adtStartDate.Year;
                DateTime ldtCompleteYear = adtStartDate.AddYears(lintYearDiff);
                if (ldtCompleteYear > adtEndDate)
                {
                    lintYearDiff--;
                    ldtCompleteYear = ldtCompleteYear.AddYears(-1);
                }
                while (ldtCompleteYear < adtEndDate)
                {
                    ldtCompleteYear = ldtCompleteYear.AddMonths(1);
                    if (ldtCompleteYear <= adtEndDate)
                    {
                        lintMonths++;
                    }
                }
                lintMonths += lintYearDiff * 12;
            }


            return lintMonths;
        }

        public static void GetDetailTimeSpan(DateTime adtAgeCalculationDate, DateTime adtDateOfBirth, out int aintYears, out int aintMonths, out int aintDays)
        {
            if (adtAgeCalculationDate < adtDateOfBirth)
            {
                DateTime ldtTempDate = adtDateOfBirth;
                adtDateOfBirth = adtAgeCalculationDate;
                adtAgeCalculationDate = ldtTempDate;
            }

            aintMonths = 12 * (adtAgeCalculationDate.Year - adtDateOfBirth.Year) + (adtAgeCalculationDate.Month - adtDateOfBirth.Month);


            if (adtAgeCalculationDate.Day < adtDateOfBirth.Day)
            {
                aintMonths--;
                aintDays = DateTime.DaysInMonth(adtDateOfBirth.Year, adtDateOfBirth.Month) - adtDateOfBirth.Day + adtAgeCalculationDate.Day;
            }
            else
            {
                aintDays = adtAgeCalculationDate.Day - adtDateOfBirth.Day;
            }

            aintYears = aintMonths / 12;
            aintMonths -= aintYears * 12;
        }

        public static Collection<cdoPlan> GetPlanValues(int iaintPersonId)
        {
            Collection<cdoPlan> lColPlans = null;
            DataTable ldtbList = busMPIPHPBase.Select("cdoPersonAccount.GetPlanFromPersonID", new object[1] { iaintPersonId });
            if (ldtbList.Rows.Count > 0)
            {
                lColPlans = Sagitec.DataObjects.doBase.GetCollection<cdoPlan>(ldtbList);
            }
            return lColPlans;
        }



        public static bool IsHoliday(this DateTime adtDate)
        {
            return busHoliday.IsHoliday(adtDate);
        }
        /// <summary>
        /// Send Mail 
        /// </summary>
        /// <param name="astrToAddress">To Address</param>
        /// <param name="astrFromAddress">From Address</param>
        /// <param name="astrBody">Message Body</param>
        /// <param name="astrSubject">Subject</param>
        /// <param name="astrSmtpServer">SMTP Server</param>
        /// <param name="astrUserName">User Name</param>
        /// <param name="astrPassword">Password</param>
        /// <param name="aintPort">Port</param>
        public static void SendMail(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject, string astrSmtpServer, string astrUserName, string astrPassword, int aintPort)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(new MailAddress(astrToAddress));
            mail.From = new MailAddress(astrFromAddress);
            mail.Subject = astrSubject;
            mail.Body = astrBody;

            SmtpClient lobjSmtpClient = new SmtpClient(astrSmtpServer, aintPort);
            lobjSmtpClient.Credentials = new NetworkCredential(astrUserName, astrPassword);
            lobjSmtpClient.Send(mail);
        }

        public static void SendMail(string astrFrom, string astrTo, string astrHeading, string astrMessage, bool ablnHighPriority, bool ablnHtmlFormat)
        {
            if ((astrTo != null) && (astrTo.Trim() != ""))
            {
                string appSettings = MPIPHP.Common.ApplicationSettings.Instance.SmtpServer;
                foreach (string lstrTo in astrTo.Split(';'))
                {
                    string lstrParsedTo = lstrTo.Replace(";", "");
                    if (lstrParsedTo.IsNotNullOrEmpty())
                    {
                        MailMessage message = new MailMessage(astrFrom, lstrParsedTo);
                        message.Subject = astrHeading;
                        message.Body = astrMessage;
                        message.IsBodyHtml = ablnHtmlFormat;
                        if (ablnHighPriority)
                            message.Priority = MailPriority.High;
                        new SmtpClient(appSettings).Send(message);
                    }
                }
            }
        }


        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="astrToAddress">To Address</param>
        /// <param name="astrFromAddress">From Address</param>
        /// <param name="astrBody">Message Body</param>
        /// <param name="astrSubject">Subject</param>
        public static void SendMail(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject)
        {
            string lstrSmtpServer = MPIPHP.Common.ApplicationSettings.Instance.SMTP_HOST_PATH;
            string lstrUserName = MPIPHP.Common.ApplicationSettings.Instance.SMTP_USERNAME;
            string lstrPassword = MPIPHP.Common.ApplicationSettings.Instance.SMTP_PASSWORD;
            int lintPort = Convert.ToInt32(MPIPHP.Common.ApplicationSettings.Instance.SMTP_HOST_PORT);

            SendMail(astrToAddress, astrFromAddress, astrBody, astrSubject, lstrSmtpServer, lstrUserName, lstrPassword, lintPort);
        }


        /// <summary>
        /// Returns the first date of the month of the given datetime value.
        /// </summary>
        /// <param name="adtDateTime">Datetime value.</param>
        /// <returns>First Day of the given datetime value.</returns>
        public static DateTime FirstDayOfMonthFromDateTime(DateTime adtDateTime)
        {
            return new DateTime(adtDateTime.Year, adtDateTime.Month, 1);
        }


        public static Collection<busNotes> LoadNotes(int aintPersonID, int aintOrgID, string FormName)
        {
            Collection<busNotes> iclbNotes = new Collection<busNotes>();
            DataTable ldtbNotes = null;

            if (aintPersonID != 0)
            {
                //Query to load Person Notes
                ldtbNotes = busMPIPHPBase.Select("cdoNotes.FindNotesForPerson", new object[] { aintPersonID });
                if (ldtbNotes.Rows.Count > 0)
                {
                    busMPIPHPBase lbusMPIPHPBase = new busMPIPHPBase();
                    iclbNotes = lbusMPIPHPBase.GetCollection<busNotes>(ldtbNotes, "icdoNotes");
                    if (iclbNotes != null)
                        iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
                }
            }
            else
            {
                //Query to load ORG Notes
                ldtbNotes = busMPIPHPBase.Select("cdoNotes.FindNotesForOrganisation", new object[] { aintOrgID });
                if (ldtbNotes.Rows.Count > 0)
                {
                    busMPIPHPBase lbusMPIPHPBase = new busMPIPHPBase();
                    iclbNotes = lbusMPIPHPBase.GetCollection<busNotes>(ldtbNotes, "icdoNotes");
                    if (iclbNotes != null)
                        iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
                }
            }
            return iclbNotes;
        }
        /// <summary>
        /// Returns the last daty of the month of the given datetime value.
        /// </summary>
        /// <param name="adtDateTime">Datetime value.</param>
        /// <returns>Last Day of the given datetime value.</returns>
        public static DateTime LastDayOfMonthFromDateTime(DateTime adtDateTime)
        {
            DateTime ldtFirstDayOfTheMonth = FirstDayOfMonthFromDateTime(adtDateTime);
            return ldtFirstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Toggle Flag
        /// </summary>
        /// <param name="astrFlag">Flag Value</param>
        /// <returns>Toggled Value</returns>
        public static string ToggleFlag(string astrFlag)
        {
            if (astrFlag == busConstant.FLAG_YES)
                return busConstant.FLAG_NO;
            else
                return busConstant.FLAG_YES;
        }

        public static DateTime GetLastDateOfComputationYear(int lintYear)
        {
            DateTime ldtSaturdayBeforeLastThrusday = DateTime.MinValue;

            DateTime dtLastDayOfMonth = new DateTime(lintYear, 12, 31);
            DateTime DayInWeek = dtLastDayOfMonth.Date;

            while (DayInWeek.DayOfWeek != DayOfWeek.Thursday)
                DayInWeek = DayInWeek.AddDays(-1);

            ldtSaturdayBeforeLastThrusday = DayInWeek.AddDays(-5);

            return ldtSaturdayBeforeLastThrusday;
        }

        public static DateTime GetFirstDateOfComputationYear(int lintYear)
        {
            DateTime ldtSundayBeforeLastThrusday = DateTime.MinValue;

            DateTime dtLastDayOfMonth = new DateTime(lintYear - 1, 12, 31);
            DateTime DayInWeek = dtLastDayOfMonth.Date;

            while (DayInWeek.DayOfWeek != DayOfWeek.Thursday)
                DayInWeek = DayInWeek.AddDays(-1);

            ldtSundayBeforeLastThrusday = DayInWeek.AddDays(-4);

            return ldtSundayBeforeLastThrusday;
        }

        /// <summary>
        /// This method calculates the last Payroll date of the month.
        /// </summary>
        /// <param name="aintMonth"> Month of Payroll </param>
        /// <param name="aintYear"> Year of Payroll </param>
        /// <returns> Last Date of the Payroll Month </returns>
        public static DateTime GetLastPayrollDayOfMonth(int aintYear, int aintMonth)
        {
            DateTime ldtLastPayrollDate = DateTime.MinValue;
            DateTime ldtLastDayOfMonth = new DateTime(aintYear, aintMonth, DateTime.DaysInMonth(aintYear, aintMonth));
            DateTime DayInWeek = ldtLastDayOfMonth.Date;

            while (DayInWeek.DayOfWeek != DayOfWeek.Thursday)
                DayInWeek = DayInWeek.AddDays(-1);

            ldtLastPayrollDate = DayInWeek.AddDays(-5);

            return ldtLastPayrollDate;
        }

        //Payroll Month Fix - 12052019

        public static DateTime GetLastPayrollDayOfMonthFromEADB(DateTime adtGivenDate)
        {
            DateTime ldtLastPayrollDate = DateTime.MinValue;
            DataTable ldtLastDayOfMonth = busMPIPHPBase.Select("cdoPayeeAccount.GetPayrollStartAndEndDatesFromEADB", new object[1] { adtGivenDate });

            if (ldtLastDayOfMonth != null && ldtLastDayOfMonth.Rows.Count > 0)
            {
                if (Convert.ToString(ldtLastDayOfMonth.Rows[0]["PAYROLL_END_DATE"]).IsNotNullOrEmpty())
                    ldtLastPayrollDate = Convert.ToDateTime(ldtLastDayOfMonth.Rows[0]["PAYROLL_END_DATE"]);
            }

            return ldtLastPayrollDate;
        }

        /// <summary>
        /// This method calculates the first Payroll date of the month.
        /// </summary>
        /// <param name="aintMonth"> Month of Payroll </param>
        /// <param name="aintYear"> Year of Payroll </param>
        /// <returns> First Date of the Payroll Month </returns>
        public static DateTime GetFirstPayrollDayOfMonth(int aintYear, int aintMonth)
        {
            DateTime ldtFirstPayrollDate = DateTime.MinValue;
            int lintYear = aintMonth - 1 == busConstant.ZERO_INT ? aintYear - 1 : aintYear;
            int lintMonth = aintMonth - 1 == busConstant.ZERO_INT ? 12 : aintMonth - 1;
            DateTime ldtLastDayOfMonth = new DateTime(lintYear, lintMonth, DateTime.DaysInMonth(lintYear, lintMonth));
            DateTime DayInWeek = ldtLastDayOfMonth.Date;

            while (DayInWeek.DayOfWeek != DayOfWeek.Thursday)
                DayInWeek = DayInWeek.AddDays(-1);

            ldtFirstPayrollDate = DayInWeek.AddDays(-4);

            return ldtFirstPayrollDate;
        }

        public static int GetPayrollMonth(DateTime adtDate)
        {
            int lintPayrollMonth = 1;
            DateTime ldtLastPayrollDate = busGlobalFunctions.GetLastPayrollDayOfMonth(adtDate.Year, adtDate.Month);
            if (adtDate.Day > ldtLastPayrollDate.Day)
            {
                lintPayrollMonth = adtDate.Month + 1 > 12 ? 1 : adtDate.Month + 1;
            }
            else
            {
                lintPayrollMonth = adtDate.Month;
            }

            return lintPayrollMonth;
        }

        public static DateTime GetDateFromEA(string astrEADate)
        {
            DateTime ldtEADate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(astrEADate))
            {
                astrEADate = astrEADate.Insert(4, "/");
                astrEADate = astrEADate.Insert(7, "/");
                ldtEADate = Convert.ToDateTime(astrEADate);
                return ldtEADate;
            }
            return ldtEADate;
        }

        public static int WorkingDays(DateTime adtfirstDay, DateTime adtlastDay)
        {
            int businessDays = 0;
            DateTime ldtTemp = new DateTime();
            adtfirstDay = adtfirstDay.Date;
            adtlastDay = adtlastDay.Date;
            ldtTemp = adtfirstDay;
            ldtTemp = ldtTemp.Date;

            while (ldtTemp <= adtlastDay)
            {

                if (ldtTemp.DayOfWeek != DayOfWeek.Saturday && ldtTemp.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
                ldtTemp = ldtTemp.AddDays(1);
            }

            return businessDays;
        }
        //public static Collection<cdoPersonBridgeHours> PopulateYearsInDropDown()
        //{
        //    Collection<cdoPersonBridgeHours> lclbYears = new Collection<cdoPersonBridgeHours>();
        //    int lintCurrentYear = DateTime.Now.Year;
        //    for (int i = 1950; i <= lintCurrentYear; i++)
        //    {
        //        cdoPersonBridgeHours lcdoPersonBridgeHours = new cdoPersonBridgeHours();
        //       // lcdoPersonBridgeHours.computation_year=i;
        //        lclbYears.Add(lcdoPersonBridgeHours);
        //    }

        //    return lclbYears;
        //}

        public static int GetPlanIdFromPlanCode(string astrPlanCode)
        {
            busPlan lbusPlan = new busPlan() { icdoPlan = new cdoPlan() };
            DataTable ldtPlan = busBase.Select<cdoPlan>(new string[1] { enmPlan.plan_code.ToString() }, new object[1] { astrPlanCode }, null, null);
            if (ldtPlan != null && ldtPlan.Rows.Count > 0)
            {
                lbusPlan.icdoPlan.LoadData(ldtPlan.Rows[0]);
            }

            return lbusPlan.icdoPlan.plan_id;
        }


        public static int DataMigrationForRelativeValue(int aintBenefitProvisionID)              //Rohan
        {
            try
            {
                DataTable ldtTempData = busMPIPHPBase.Select("cdoBenefitFactorLocal.GetTempData", new object[] { });
                if (ldtTempData.Rows.Count > 0)
                {
                    int lColumnCount = ldtTempData.Columns.Count;
                    foreach (DataRow ldrData in ldtTempData.AsEnumerable())
                    {
                        for (int i = 1; i < lColumnCount; i++)
                        {
                            if (!ldtTempData.Columns[i].ColumnName.Contains("F") && ldrData[0] != DBNull.Value)
                            {
                                cdoBenefitProvisionBenefitOptionFactor lcdoBenefitProvisionBenefitOptionFactor = new cdoBenefitProvisionBenefitOptionFactor();

                                lcdoBenefitProvisionBenefitOptionFactor.benefit_provision_id = aintBenefitProvisionID;
                                lcdoBenefitProvisionBenefitOptionFactor.benefit_account_type_id = 1502;
                                lcdoBenefitProvisionBenefitOptionFactor.benefit_account_type_value = busConstant.BENEFIT_TYPE_RETIREMENT;
                                lcdoBenefitProvisionBenefitOptionFactor.plan_benefit_id = 27;

                                lcdoBenefitProvisionBenefitOptionFactor.participant_age = Convert.ToDecimal(ldrData[0]);

                                if (Convert.ToString(ldtTempData.Columns[i].ColumnName).Contains("#"))
                                {
                                    lcdoBenefitProvisionBenefitOptionFactor.spouse_age = Convert.ToDecimal(Convert.ToString(ldtTempData.Columns[i].ColumnName).ReplaceWith("#", "."));
                                }
                                else
                                {
                                    lcdoBenefitProvisionBenefitOptionFactor.spouse_age = Convert.ToDecimal(ldtTempData.Columns[i].ColumnName);
                                }
                                if (Convert.ToString(ldrData[i]) == string.Empty)
                                {
                                    lcdoBenefitProvisionBenefitOptionFactor.benefit_option_factor = 0;
                                }
                                else
                                {
                                    lcdoBenefitProvisionBenefitOptionFactor.benefit_option_factor = Math.Round(Convert.ToDecimal(Convert.ToDecimal(ldrData[i])), 3);
                                }
                                lcdoBenefitProvisionBenefitOptionFactor.created_by = "rohan.adgaonkar";
                                lcdoBenefitProvisionBenefitOptionFactor.created_date = DateTime.Now;
                                lcdoBenefitProvisionBenefitOptionFactor.modified_by = "rohan.adgaonkar";
                                lcdoBenefitProvisionBenefitOptionFactor.modified_date = DateTime.Now;
                                lcdoBenefitProvisionBenefitOptionFactor.update_seq = 0;
                                lcdoBenefitProvisionBenefitOptionFactor.Insert();
                            }
                        }
                    }

                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }




        public static string ConvertDateIntoDifFormat(DateTime adtDate)  //for converting date into different format.e.g. '04/01/2012' den it will be 'April 1, 2012'.
        {
            string lstr = string.Empty;
            lstr = String.Format("{0:MMMM dd, yyyy}", adtDate);
            return lstr;
        }
        /// <summary>
        /// Method to convert amount to words
        /// </summary>
        /// <param name="astrAmount">Amount</param>
        /// <returns>Amount in Words</returns>
        public static string AmountToWords(string astrAmount)
        {
            long lintInputNum = 0;
            string lstrDollarsPart = string.Empty, lstrCents = string.Empty, lstrCentsPart = string.Empty;
            try
            {
                string[] lstrSplits = new string[2];
                lstrSplits = astrAmount.Split('.');
                lintInputNum = Convert.ToInt64(lstrSplits[0]);
                lstrCents = lstrSplits[1];
                if (lstrCents.Length == 1)
                {
                    lstrCents += "0";
                }
                lstrCents = (lstrCents.Length < 3) ? lstrCents : lstrCents.Substring(0, 2);
                lstrCentsPart = CentsPart(lstrCents);
            }
            catch
            {
                lintInputNum = Convert.ToInt32(astrAmount);
            }

            if (lintInputNum == 0)
            {
                if (string.IsNullOrEmpty(lstrCentsPart))
                    return lstrCentsPart;
                else
                    return lstrCentsPart + " cents";
            }
            else
            {
                lstrDollarsPart = DollarPart(lintInputNum.ToString());
                if (string.IsNullOrEmpty(lstrCentsPart))
                    return lstrDollarsPart;
                else
                    return lstrDollarsPart + " and " + lstrCentsPart + " cents";
            }
        }
        /// <summary>
        /// Method to convert Cents part to words
        /// </summary>
        /// <param name="astrCents">Cents</param>
        /// <returns>Cents in words</returns>
        public static string CentsPart(string astrCents)
        {
            string lstrCents = string.Empty;
            string[] lstrOnes = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] lstrTens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            int lintDig1, lintDig2, lintCents;

            lintCents = Convert.ToInt32(astrCents);
            if (lintCents < 20)
                lstrCents = lstrCents + lstrOnes[lintCents];
            else
            {
                lintDig1 = lintCents / 10;
                lintDig2 = lintCents % 10;
                lstrCents = lstrCents + lstrTens[lintDig1] + " " + lstrOnes[lintDig2];
            }
            return lstrCents;
        }

        /// <summary>
        /// Method to convert Dollar part to words
        /// </summary>
        /// <param name="astrInputNum">Dollars</param>
        /// <returns>Dollars in words</returns>
        public static string DollarPart(string astrInputNum)
        {
            string lstrLastThree = string.Empty, lstrDollars = string.Empty;
            string[] lstrOnes = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] lstrTens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            string[] lstrThous = { "", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion" };
            int lintDig1, lintDig2, lintDig3, lintLevel = 0, lintLastTwo, lintThreeDigits;

            while (astrInputNum.Length > 0)
            {
                if (astrInputNum.Length > 0)
                {
                    //Get the three rightmost characters
                    lstrLastThree = (astrInputNum.Length < 3) ? astrInputNum : astrInputNum.Substring(astrInputNum.Length - 3, 3);

                    // Separate the three digits
                    lintThreeDigits = int.Parse(lstrLastThree);
                    lintLastTwo = lintThreeDigits % 100;
                    lintDig1 = lintThreeDigits / 100;
                    lintDig2 = lintLastTwo / 10;
                    lintDig3 = (lintThreeDigits % 10);

                    // append a "thousand" where appropriate
                    if (lintLevel > 0 && lintDig1 + lintDig2 + lintDig3 > 0)
                    {
                        lstrDollars = lstrThous[lintLevel] + " " + lstrDollars;
                        lstrDollars = lstrDollars.Trim();
                    }

                    // check that the last two digits is not a zero
                    if (lintLastTwo > 0)
                    {
                        if (lintLastTwo < 20)
                        {
                            // if less than 20, use "ones" only
                            lstrDollars = lstrOnes[lintLastTwo] + " " + lstrDollars;
                        }
                        else
                        {
                            // otherwise, use both "tens" and "ones" array
                            lstrDollars = lstrTens[lintDig2] + " " + lstrOnes[lintDig3] + " " + lstrDollars;
                        }
                        if (astrInputNum.Length < 3)
                        {
                            return lstrDollars + " dollars";
                        }
                    }

                    // if a hundreds part is there, translate it
                    if (lintDig1 > 0)
                    {
                        lstrDollars = lstrOnes[lintDig1] + " hundred " + lstrDollars;
                    }
                    astrInputNum = (astrInputNum.Length - 3) > 0 ? astrInputNum.Substring(0, astrInputNum.Length - 3) : "";
                    lintLevel++;
                }
            }
            return lstrDollars + " dollars";
        }

        /*
        public static byte[] RenderWordAsPDF(string astrFileName)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document ldoc = new Microsoft.Office.Interop.Word.Document(astrFileName);
            //ldoc.ReadOnlyRecommended = true;

                        var TheDocument = wordApp.Documents.Open(ldoc);

                        TheDocument.ExportAsFixedFormat(ldoc.Name.Replace(".docx", ".pdf"),
                   Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF, 
                   OptimizeFor: Microsoft.Office.Interop.Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen, 
                   BitmapMissingFonts: true, DocStructureTags: false);
                        ((Microsoft.Office.Interop.Word._Document)TheDocument).Close();
                        return null;
//((Word._Document)TheDocument).Close();

//            byte[] pdfBytes;

//            using (MemoryStream pdfStream = new MemoryStream())
//            {
//                ldoc.SaveToPdf(0, doc.PageCount, pdfStream, null);
//                pdfBytes = pdfStream.ToArray();
//            }

//            return pdfBytes;

//            ldoc.SaveAs(astrFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF,);
//            MemoryStream loutStream = new MemoryStream();

//            byte[] myByteArray = File.ReadAllBytes(astrFileName);
//            return myByteArray;
            //System.Web.HttpResponse
            //Response.Clear();
           
        }
        */
        public static byte[] RenderWordAsPDF(string astrfilename, string astrActiveFlag = busConstant.FLAG_YES, string astrAddressType = "", bool ablnFlagPdf = false, string astrLastName = null, string astrMPID = null) // PROD PIR 845
        {
            Microsoft.Office.Interop.Word.Application WordApp = null;

            try
            {
                if (WordApp == null)
                {
                    WordApp = new Microsoft.Office.Interop.Word.Application();
                }
                string file_extension = Path.GetExtension(astrfilename);
                //PROD PIR 814
                string astrImagedFileName = string.Empty;
                //PIR-827
                //if (astrfilename.IsNotNullOrEmpty() && astrfilename.Contains("DIS-0018"))
                if ((astrfilename.IsNotNullOrEmpty() && astrfilename.Contains(busConstant.PROOF_OF_SSA_CONTINUOUS_DISABILITY)) || ablnFlagPdf) // PROD PIR 845
                    astrImagedFileName = astrfilename.Replace(file_extension, astrAddressType + ".pdf");
                else
                    astrImagedFileName = astrfilename.Replace(file_extension, ".pdf");

                //if (file_extension == ".doc")
                //{
                //    astrfilename = astrfilename + "x";
                //}

                // PIR - 783 for seperate pdf file path 
                utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
                if (lobjPassInfo.idictParams["ID"] != null)
                {
                    if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "RetireeIncreaseCorrespondenceBatch")
                    {
                        busSystemManagement iobjSystemManagement = null;
                        iobjSystemManagement = new busSystemManagement();
                        iobjSystemManagement.FindSystemManagement();

                        string istrfilepath = "";
                        if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                        {
                            //PIR 977
                            istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "RetireeIncreaseCorr\\" + DateTime.Now.Year;
                            if (!Directory.Exists(istrfilepath))
                            {
                                Directory.CreateDirectory(istrfilepath);
                            }
                            istrfilepath = istrfilepath + "\\";
                            // checking bad address 
                            if (astrActiveFlag == busConstant.FLAG_NO)
                            {
                                istrfilepath = istrfilepath + "BadAddress";
                                if (!Directory.Exists(istrfilepath))
                                {
                                    Directory.CreateDirectory(istrfilepath);
                                }
                                istrfilepath = istrfilepath + "\\";
                            }
                        }
                        astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);
                    }

                    else if(Convert.ToString(lobjPassInfo.idictParams["ID"]) == "EEUVHPStatementBatch")
                    {
                        busSystemManagement iobjSystemManagement = null;
                        iobjSystemManagement = new busSystemManagement();
                        iobjSystemManagement.FindSystemManagement();

                        string istrfilepath = "";
                        if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                        {
                            //PIR 977
                            istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\" + "UVHP&EE\\" + DateTime.Now.Year;
                            if (!Directory.Exists(istrfilepath))
                            {
                                Directory.CreateDirectory(istrfilepath);
                            }
                            istrfilepath = istrfilepath + "\\";
                            // checking bad address 
                            if (astrActiveFlag == busConstant.FLAG_NO)
                            {
                                istrfilepath = istrfilepath + "BadAddress";
                                if (!Directory.Exists(istrfilepath))
                                {
                                    Directory.CreateDirectory(istrfilepath);
                                }
                                istrfilepath = istrfilepath + "\\";
                            }
                        }
                        astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);

                    }
                    else if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "RetirementWorkshopBatch")
                    {
                        busSystemManagement iobjSystemManagement = null;
                        iobjSystemManagement = new busSystemManagement();
                        iobjSystemManagement.FindSystemManagement();

                        string istrfilepath = "";
                        if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                        {
                            //PIR 977
                            istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\" + "MPI_Retirement_WorkShop\\" + DateTime.Now.Year;
                            if (!Directory.Exists(istrfilepath))
                            {
                                Directory.CreateDirectory(istrfilepath);
                            }
                            istrfilepath = istrfilepath + "\\";
                            // checking bad address 
                            if (astrActiveFlag == busConstant.FLAG_NO)
                            {
                                istrfilepath = istrfilepath + "BadAddress";
                                if (!Directory.Exists(istrfilepath))
                                {
                                    Directory.CreateDirectory(istrfilepath);
                                }
                                istrfilepath = istrfilepath + "\\";
                            }
                        }
                        astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);

                    }
                    //PIR 337
                    else if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "RetirementAffidavitBatch" && astrActiveFlag != busConstant.FLAG_NO)
                    {
                        busSystemManagement iobjSystemManagement = null;
                        iobjSystemManagement = new busSystemManagement();
                        iobjSystemManagement.FindSystemManagement();

                        string istrfilepath = "";
                        if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                        {
                            istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\" + "RetirementAffidavitBatch\\Temp\\";
                            if (!Directory.Exists(istrfilepath))
                            {
                                Directory.CreateDirectory(istrfilepath);
                            }
                            istrfilepath = istrfilepath + "\\";
                            // checking bad address 
                        }
                        astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);
                    }
                    //PIR 1003
                    else if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "AnnualBenefitSummaryCorrespondenceBatch")
                    {
                        busSystemManagement iobjSystemManagement = null;
                        iobjSystemManagement = new busSystemManagement();
                        iobjSystemManagement.FindSystemManagement();

                        string istrfilepath = "";
                        if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                        {
                            istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_Annual_Benefit_Correspondence_Temp;
                            if (!Directory.Exists(istrfilepath))
                            {
                                Directory.CreateDirectory(istrfilepath);
                            }
                            istrfilepath = istrfilepath + "\\";
                            // checking bad address 
                        }
                        astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);
                    }
                    //rid 80600
                    else if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "PensionBenefitVerificationBatch")
                    {
                        if (!string.IsNullOrEmpty(astrLastName))
                        {
                            astrImagedFileName = Path.GetDirectoryName(astrImagedFileName) + @"\" + astrLastName + "_" + Path.GetFileName(astrImagedFileName);
                        }
                    }
                    else if (Convert.ToString(lobjPassInfo.idictParams["ID"]) == "IAPRequiredMinimumDistributionBatch")
                    {

                        astrImagedFileName = Path.GetDirectoryName(astrImagedFileName) + @"\" + Path.GetFileName(astrImagedFileName);

                    }
                    else
                    {
                        if (astrMPID.IsNotNullOrEmpty())
                        {
                            astrImagedFileName = Path.GetDirectoryName(astrImagedFileName) + @"\" + astrMPID + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + Path.GetFileName(astrImagedFileName);
                        }
                    }

                }
                //Open Word Doc
                WordApp.Documents.Open(astrfilename);
                WordApp.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;

                //Target Format PDF
                object oSaveName = new object();
                oSaveName = astrImagedFileName;
                object oPDFFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;
                object oLockComments = false;
                object oPassword = "";
                object oAddToRecentFiles = false;
                object oWritePassword = "";
                object oReadOnlyRecommended = false;
                object oEmbedTrueTypeFonts = false;
                object oSaveNativePictureFormat = false;
                object oSaveFormsData = false;
                object oSaveAsAOCELetter = false;
                object missing = System.Reflection.Missing.Value;
                object save_changes = Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges;

                WordApp.ActiveDocument.SaveAs2(ref oSaveName, ref oPDFFormat,
                    ref oLockComments, ref oPassword,
                    ref oAddToRecentFiles, ref oWritePassword, ref oReadOnlyRecommended, ref oEmbedTrueTypeFonts,
                    ref oSaveNativePictureFormat, ref oSaveFormsData, ref oSaveAsAOCELetter, ref missing, ref missing, ref missing, ref missing, ref missing);

                WordApp.ActiveDocument.Close(ref save_changes, ref missing, ref missing);
                WordApp.Visible = false;
                WordApp.Quit(ref missing, ref missing, ref missing);

                //FileDownloadContainer result = null;
                ////busCorTracking lbusCorTracking = new busCorTracking();
                ////if (lbusCorTracking.FindCorTracking(aintCorTrackingId))
                ////{
                //    //string lstrFullFileName = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr") + lbusCorTracking.icdoCorTracking.generated_file_name;
                //    FileInfo lobjFileInfo = new FileInfo(astrImagedFileName);
                //    byte[] lobjFileContent = null;
                //    using (FileStream lobjFileStream = lobjFileInfo.OpenRead())
                //    {
                //        lobjFileContent = new byte[lobjFileStream.Length];
                //        lobjFileStream.Read(lobjFileContent, 0, (int)lobjFileStream.Length);
                //    }
                //    result = new FileDownloadContainer(astrImagedFileName, "application/pdf", lobjFileContent);
                //}
                //return result;

                byte[] bytes = System.IO.File.ReadAllBytes(astrImagedFileName);

                return bytes;

            }
            catch (Exception ex)
            {
                object missing = System.Reflection.Missing.Value;
                object save_changes = Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges;
                WordApp.ActiveDocument.Close(ref save_changes, ref missing, ref missing);
                WordApp.Quit(ref missing, ref missing, ref missing);
                return null;
            }
        }

        public static byte[] RenderPDF(string astr_filename, string astrActiveFlag = busConstant.FLAG_YES, string astrAddressType = "", bool ablnFlagPdf = false)
        {
            string astrImagedFileName = string.Empty;
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            //  if (lobjPassInfo.idictParams["ID"] != null)
            // {
                astrImagedFileName = astr_filename;
                busSystemManagement iobjSystemManagement = null;
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();

                string istrfilepath = "";
                if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                {
                    
                    istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\ReCalculateIAPAllocation\\Report";
                    if (!Directory.Exists(istrfilepath))
                    {
                        Directory.CreateDirectory(istrfilepath);
                    }
                    istrfilepath = istrfilepath + "\\";
                    // checking bad address 
                    if (astrActiveFlag == busConstant.FLAG_NO)
                    {
                        istrfilepath = istrfilepath + "BadAddress";
                        if (!Directory.Exists(istrfilepath))
                        {
                            Directory.CreateDirectory(istrfilepath);
                        }
                        istrfilepath = istrfilepath + "\\";
                    }
                }
                astrImagedFileName = istrfilepath + Path.GetFileName(astrImagedFileName);
         //   }
              
                byte[] bytes = System.IO.File.ReadAllBytes(astrImagedFileName);

                return bytes;
     }

        public static int GetPreviousQuarter(DateTime adtRetirementDate)
        {
            //PIR 885
            if (adtRetirementDate != DateTime.MinValue)
            {
                if (adtRetirementDate.Month >= 4 && adtRetirementDate.Month <= 6)
                    return 1;
                else if (adtRetirementDate.Month >= 7 && adtRetirementDate.Month <= 9)
                    return 2;
                else if (adtRetirementDate.Month >= 10 && adtRetirementDate.Month <= 12)
                    return 3;
                else
                    return 0;
            }
            return 0;
        }

        /// <summary>
        /// adtTime : Will always be the end of quarter.
        /// </summary>
        /// <param name="adtTime"></param>
        /// <returns></returns>
        public static int GetCurrentQuarter(DateTime adtTime)
        {
            if (adtTime != DateTime.MinValue)
            {
                if (adtTime.Month == 3)
                    return 1;
                if (adtTime.Month == 6)
                    return 2;
                if (adtTime.Month == 9)
                    return 3;
                if (adtTime.Month == 12)
                    return 4;
            }
            return 0;
        }

        public static DateTime GetLastDateOfPreviousQuarter(DateTime adtDate)
        {
            DateTime ldtLastDayOfPreviousQuarter = new DateTime();

            if (adtDate != DateTime.MinValue)
            {
                if (adtDate.Month <= 3)
                {
                    ldtLastDayOfPreviousQuarter = new DateTime(adtDate.Year - 1, 12, 31);

                }
                else if (adtDate.Month > 3 && adtDate.Month <= 6)
                {
                    ldtLastDayOfPreviousQuarter = new DateTime(adtDate.Year, 03, 31);

                }
                else if (adtDate.Month > 6 && adtDate.Month <= 9)
                {
                    ldtLastDayOfPreviousQuarter = new DateTime(adtDate.Year, 06, 30);

                }
                else if (adtDate.Month > 9 && adtDate.Month <= 12)
                {
                    ldtLastDayOfPreviousQuarter = new DateTime(adtDate.Year, 09, 30);
                }
            }

            return ldtLastDayOfPreviousQuarter;
        }

        public static string GetScreenName(string astrFormName)
        {
            astrFormName = astrFormName.Replace("wfm", "");
            astrFormName = astrFormName.Replace("Maintenance", "");
            astrFormName.ToProperCase();
            if (astrFormName == "QDROApplication")
                astrFormName = "QDRO Application";
            else
                astrFormName = Regex.Replace(astrFormName, "([a-z])([A-Z])", @"$1 $2");

            return astrFormName;

        }

        //public static string GetMIMEType(string fileName)
        //{
        //    if (MIMETypesDictionary.ContainsKey(Path.GetExtension(fileName).Remove(0, 1)))
        //    {
        //        return MIMETypesDictionary[Path.GetExtension(fileName).Remove(0, 1)];
        //    }
        //    return "application/octet-stream";
        //}

        #endregion

        #region Extension Methods
        /// <summary>
        /// Formats a string with a list of literal placeholders.
        /// </summary>
        /// <param name="astrText">The extension text</param>
        /// <param name="args">The argument list</param>
        /// <returns>The formatted string</returns>
        public static string Format(this string astrText, params object[] args)
        {
            return string.Format(astrText, args);
        }

        /// <summary>
        /// Determines whether a substring exists within a string.
        /// </summary>
        /// <param name="astrText">String to search.</param>
        /// <param name="astrSubString">Substring to match when searching.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>Indicator of substring presence within the string.</returns>
        public static bool Contains(this string astrText, string astrSubString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Contains(astrSubString);
            }
            else
            {
                return astrText.ToLower().IndexOf(astrSubString.ToLower(), 0) >= 0;
            }
        }

        /// <summary>
        /// Detects if a string can be parsed to a valid date.
        /// </summary>
        /// <param name="astrText">Value to inspect.</param>
        /// <returns>Whether or not the string is formatted as a date.</returns>
        public static bool IsDate(this string astrText)
        {
            try
            {
                System.DateTime dtDateTime = System.DateTime.Parse(astrText);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is null or empty.</returns>
        public static bool IsEmpty(this string astrText)
        {
            return (astrText == null) || (astrText.Length == 0);
            // return astrText.Equals("\"\"") ? true : astrText.Equals("") ? true : false;
        }

        /// <summary>
        /// Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is not empty</returns>
        public static bool IsNotEmpty(this string astrText)
        {
            return (!astrText.IsEmpty());
        }

        /// <summary>
        /// Checks whether the string is null and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfNull(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNull() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfEmpty(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNotEmpty() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the specified object is null
        /// </summary>
        /// <returns>Boolean indicating whether the object is null</returns>
        public static bool IsNull(this object aobjObject)
        {
            return object.ReferenceEquals(aobjObject, null);
        }

        /// <summary>
        /// Determines whether the specified object is not null
        /// </summary>
        /// <returns>Boolean indicating whether the object is not null</returns>
        public static bool IsNotNull(this object aobjObject)
        {
            return !object.ReferenceEquals(aobjObject, null);
        }


        /// <summary>
        /// Creates a type from the given name
        /// </summary>
        /// <typeparam name="T">The type being created</typeparam>      
        /// <param name="args">Arguments to pass into the constructor</param>
        /// <returns>An instance of the type</returns>
        public static T CreateType<T>(this string astrTypeName, params object[] args)
        {
            Type ltypType = Type.GetType(astrTypeName, true, true);
            return (T)Activator.CreateInstance(ltypType, args);
        }

        /// <summary>
        /// Determines if a string can be converted to an integer.
        /// </summary>
        /// <returns>True if the string is numeric.</returns>
        public static bool IsNumeric(this string astrText)
        {
            if (astrText.IsNotNullOrEmpty())
                astrText = astrText.Trim();
            System.Text.RegularExpressions.Regex regularExpression = new System.Text.RegularExpressions.Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(astrText).Success;
        }

        /// <summary>
        /// Detects whether this instance is a valid email address.
        /// </summary>
        /// <returns>True if instance is valid email address</returns>
        public static bool IsValidEmailAddress(this string astrText)
        {
            return IsValidEmail(astrText);
        }

        /// <summary>
        /// Detects whether the supplied string is a valid IP address.
        /// </summary>
        /// <returns>True if the string is valid IP address.</returns>
        public static bool IsValidIPAddress(this string astrText)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        /// <summary>
        /// Checks if url is valid.
        /// </summary>
        /// <returns>True if the url is valid.</returns>
        public static bool IsValidUrl(this string astrURL)
        {
            string lstrRegex = "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" // user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
            return new System.Text.RegularExpressions.Regex(lstrRegex).IsMatch(astrURL);
        }


        /// <summary>
        /// Retrieves the left x characters of a string.
        /// </summary>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Left(this string astrText, int count)
        {
            return astrText.Substring(0, count);
        }

        /// <summary>
        /// Retrieves the right x characters of a string.
        /// </summary>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Right(this string astrText, int count)
        {
            return astrText.Substring(astrText.Length - count, count);
        }

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>      
        public static string Capitalize(this string astrText)
        {
            if (astrText.Length == 0)
            {
                return astrText;
            }
            if (astrText.Length == 1)
            {
                return astrText.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            }
            return astrText.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.InvariantCulture) + astrText.Substring(1);
        }
        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string astrRegexPattern)
        {
            return IsMatchingTo(astrText, astrRegexPattern, System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="aRegexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string aRegexPattern, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, aRegexPattern, options);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="aRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string value, string aRegexPattern, string astrReplaceValue)
        {
            return ReplaceWith(value, aRegexPattern, astrReplaceValue, System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string astrText, string astrRegexPattern, string astrReplaceValue, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(astrText, astrRegexPattern, astrReplaceValue, options);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="astrText">The string to examine.</param>
        /// <param name="astrOldString">The new value to be inserted.</param>
        /// <param name="astrNewString">The value to replace.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>The resulting string.</returns>
        public static string Replace(this string astrText, string astrOldString, string astrNewString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Replace(astrOldString, astrNewString);
            }
            else
            {
                System.Text.RegularExpressions.Regex aRegex = new System.Text.RegularExpressions.Regex(astrOldString, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

                return aRegex.Replace(astrText, astrNewString);
            }
        }

        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <param name="astrText">The string to reverse.</param>
        /// <returns>The resulting string.</returns>
        public static string Reverse(this string astrText)
        {
            char[] arrChar = astrText.ToCharArray();
            Array.Reverse(arrChar);
            return new string(arrChar);
        }

        /// <summary>
        /// Splits a string into an array by delimiter.
        /// </summary>
        /// <param name="astrText">String to split.</param>
        /// <param name="delimiter">Delimiter string.</param>
        /// <returns>Array of strings.</returns>
        public static string[] Split(this string astrText, string delimiter)
        {
            return astrText.Split(delimiter.ToCharArray());
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters until the next whitespace on or after 
        /// the total character count has been reached for that line.  
        /// Uses the environment new line symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aCharCount)
        {
            return WordWrap(astrText, aCharCount, false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aCharCount, bool ablnCutOff)
        {
            return WordWrap(astrText, aCharCount, ablnCutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the supplied breakText
        /// for line breaks.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <param name="astrBreakText">The line break text to use.</param>
        /// <returns>The resulting string</returns>
        public static string WordWrap(this string astrText, int aCharCount, bool ablnCutOff, string astrBreakText)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(astrText.Length + 100);
            int counter = 0;

            if (ablnCutOff)
            {
                while (counter < astrText.Length)
                {
                    if (astrText.Length > counter + aCharCount)
                    {
                        sb.Append(astrText.Substring(counter, aCharCount));
                        sb.Append(astrBreakText);
                    }
                    else
                    {
                        sb.Append(astrText.Substring(counter));
                    }

                    counter += aCharCount;
                }
            }
            else
            {
                string[] strings = astrText.Split(' ');

                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1; // the added one is to represent the inclusion of the space.

                    if (i != 0 && counter > aCharCount)
                    {
                        sb.Append(astrBreakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }

            return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <returns>Return converted type</returns>
        public static T? ConvertTo<T>(this string astrText) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T));
            }

            return ret;
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static T? ConvertTo<T>(this string astrText, IFormatProvider provider) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T), provider);

            }

            return ret;
        }

        /// <summary>
        /// Returns string converted from char.
        /// </summary>
        /// <param name="achrText"></param>
        /// <returns>Return string</returns>
        public static string ToString(this char? achrText)
        {
            return achrText.HasValue ? achrText.Value.ToString() : String.Empty;
        }

        /// <summary>
        /// Returns a Boolean value indicating whether a variable is of the indicated type.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <param name="atypType">The Type to check the object against.</param>
        /// <returns>Result of the comparison.</returns>
        public static object IsType(this object aobjObject, Type atypType)
        {
            return aobjObject.GetType().Equals(atypType);
        }

        /// <summary>
        /// Creates an instance of the generic type specified using the default constructor.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="atypType">The System.Type being instantiated.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <example>
        /// typeof(MyObject).CreateInstance();
        /// </example>
        public static T CreateInstance<T>(this System.Type atypType) where T : new()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Determines whether an expression evaluates to the DBNull class.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <returns>Returns true if the object is DBNull.</returns>
        public static object IsDBNull(this object aobjObject)
        {
            return aobjObject.IsType(typeof(DBNull));
        }

        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <param name="aintDecimalPoints">The number of decimal points to round the output value to.</param>
        /// <returns>A rounded decimal.</returns>
        public static decimal RoundDecimalPoints(this decimal adecValue, int aintDecimalPoints)
        {
            return Math.Round(adecValue, aintDecimalPoints);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <returns>A decimal value rounded to two decimal points.</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal adecValue)
        {
            return Math.Round(adecValue, 2);
        }

        /// <summary>
        /// Determine whether the collection/list is null or empty;
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is null or empty.</returns>
        public static bool IsNullOrEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Determine whether the collection/list is empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is empty.</returns>
        public static bool IsEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Checks a System.Type to see if it implements a given interface.
        /// </summary>
        /// <param name="source">The System.Type to check.</param>
        /// <param name="iface">The System.Type interface to check for.</param>
        /// <returns>True if the source implements the interface type, false otherwise.</returns>
        public static bool IsImplementationOf(this Type atypSource, Type atypInterfaceType)
        {
            if (atypSource == null)
                throw new ArgumentNullException("source");

            return atypSource.GetInterface(atypInterfaceType.FullName) != null;
        }

        /// <summary>
        /// Get age in Years for the given birth date
        /// </summary>
        /// <param name="adtDateOfBirth">Date of Birth</param>
        /// <returns>Age In Years</returns>
        public static int AgeInYears(this DateTime adtDateOfBirth)
        {
            return adtDateOfBirth.AgeInYearsAsOfDate(DateTime.Today);
        }

        /// <summary>
        /// Get age in Years for the given birth date
        /// </summary>
        /// <param name="adtDateOfBirth">Date of Birth</param>
        /// <param name="adtAsOfDate">As of Date</param>
        /// <returns>Age in Years</returns>
        public static int AgeInYearsAsOfDate(this DateTime adtDateOfBirth, DateTime adtAsOfDate)
        {
            // find the difference in days, months and years
            int lintNoOfDays = adtAsOfDate.Day - adtDateOfBirth.Day;
            int lintNoOfMonths = adtAsOfDate.Month - adtDateOfBirth.Month;
            int lintNoOfYears = adtAsOfDate.Year - adtDateOfBirth.Year;

            if (lintNoOfDays < 0)
            {
                lintNoOfDays += DateTime.DaysInMonth(adtAsOfDate.Year, adtAsOfDate.Month);
                lintNoOfMonths--;
            }

            if (lintNoOfMonths < 0)
            {
                lintNoOfMonths += 12;
                lintNoOfYears--;
            }

            return lintNoOfYears;
        }

        /// <summary>
        /// Method to filter data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Sourc data table</param>
        /// <param name="dataType">datatype of column</param>
        /// <param name="filterFieldName">column name</param>
        /// <param name="filterFieldValue">value</param>
        /// <returns>Filtered rows</returns>
        public static DataRow[] FilterTable<T>(this DataTable source, utlDataType dataType, string filterFieldName, T filterFieldValue)
        {
            DataRow[] larrRows;
            if (dataType == utlDataType.String)
            {
                larrRows = source.Select(filterFieldName + " = '" + filterFieldValue + "'");
            }
            else
            {
                larrRows = source.Select(filterFieldName + " = " + filterFieldValue);
            }
            return larrRows;
        }

        /// <summary>
        /// Method to change rowcollection into datatable
        /// </summary>
        /// <param name="rowCollection">Filtered row collection</param>
        /// <returns>datatable</returns>
        public static DataTable AsDataTable(this EnumerableRowCollection<DataRow> rowCollection)
        {
            DataTable ldtbResult = new DataTable();
            if (rowCollection.AsDataView().Count > 0)
                ldtbResult = rowCollection.CopyToDataTable();
            return ldtbResult;
        }

        public static string CalculateMinDistributionDate(DateTime adtDateOfBirth, DateTime adtVestedDt = new DateTime())//for PIR-522 added 2nd parameter 
        {
            string lstrMinDistriDate = string.Empty;
            DateTime ldtMinDate;

            adtDateOfBirth = adtDateOfBirth.AddYears(70);
            adtDateOfBirth = adtDateOfBirth.AddMonths(6);

            //for pir-522
            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtDateOfBirth)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }

            return lstrMinDistriDate;
        }

        //RMD72Project
        public static DateTime GetMinDistributionDate(DateTime adtDateOfBirth, DateTime adtVestedDt = new DateTime())
        {
            DateTime ldtMinDate;
            adtDateOfBirth = adtDateOfBirth.AddYears(70);
            adtDateOfBirth = adtDateOfBirth.AddMonths(6);

            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtDateOfBirth)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
            }
            else
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
            }

            return ldtMinDate;
        }

        //RMD72Project
        public static DateTime Get72MinDistributionDate(DateTime adtDateOfBirth, DateTime adtVestedDt = new DateTime())
        {
            DateTime ldtMinDate;

            adtDateOfBirth = adtDateOfBirth.AddYears(72);

            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtDateOfBirth)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
            }
            else
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
            }

            return ldtMinDate;
        }

        //RMD72Project
        public static string Calculate72MinDistributionDate(DateTime adtDateOfBirth, DateTime adtVestedDt = new DateTime())//for PIR-522 added 2nd parameter 
        {
            string lstrMinDistriDate = string.Empty;
            DateTime ldtMinDate;

            adtDateOfBirth = adtDateOfBirth.AddYears(72);

            //for pir-522
            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtDateOfBirth)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }

            return lstrMinDistriDate;
        }

        //WI 23550 Ticket 143336
        public static string Calculate73MinDistributionDate(DateTime adtDateOfBirth, DateTime adtVestedDt = new DateTime()) 
        {
            string lstrMinDistriDate = string.Empty;
            DateTime ldtMinDate;

            adtDateOfBirth = adtDateOfBirth.AddYears(73);

            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtDateOfBirth)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }

            return lstrMinDistriDate;
        }

        //RMD72Project
        public static DateTime GetVestedDate(int aintPerson_id, int aintPlan_Id)
        {
            DateTime ldtVestedDate = new DateTime();
            DataTable ldtblVestedDate = busBase.Select("cdoPersonAccountEligibility.GetVestedDate", new object[2]{ aintPerson_id, aintPlan_Id});
            if (ldtblVestedDate != null && ldtblVestedDate.Rows.Count > 0)
            {
                if (Convert.ToString(ldtblVestedDate.Rows[0][0]).IsNotNullOrEmpty())
                {
                    ldtVestedDate = Convert.ToDateTime(ldtblVestedDate.Rows[0][0]);
                }
            }
            return ldtVestedDate;
        }

        //RMD72Project
        public static string CalculateMinDistributionDate(int person_id, DateTime adtVestedDt = new DateTime())//for PIR-522 added 2nd parameter 
        {

            DataTable ldtbList = busBase.Select("cdoPerson.GetMDAgeById", new object[1] { person_id });
            DateTime adtMDDate = new DateTime();
            if (ldtbList.Rows.Count > 0 && ldtbList.Rows[0]["DATE_OF_BIRTH"].ToString().IsNotNullOrEmpty())
            {
                adtMDDate = Convert.ToDateTime(ldtbList.Rows[0]["DATE_OF_BIRTH"]);

            }
            string lstrMinDistriDate = string.Empty;
            DateTime ldtMinDate;

            if(Convert.ToInt32(ldtbList.Rows[0]["MD_AGE_OPT_ID"])== 0)
            {

                adtMDDate = adtMDDate.AddYears(70);
                adtMDDate = adtMDDate.AddMonths(6);

            }
            else
            {
                int years = (int)Math.Floor(Convert.ToDecimal(ldtbList.Rows[0]["MD_AGE"]));
                decimal months = Convert.ToDecimal(ldtbList.Rows[0]["MD_AGE"]) - years;
                int actualMonths = (int)Math.Floor(months * 12);

                adtMDDate = adtMDDate.AddYears(years);
                adtMDDate = adtMDDate.AddMonths(actualMonths);

            }
           
            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtMDDate)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                ldtMinDate = new DateTime(adtMDDate.Year + 1, 04, 01);
                lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }

            return lstrMinDistriDate;
        }


        //RMD72Project
        public static decimal GetMinDistributionAge(int person_id, DateTime adtVestedDt = new DateTime())
        {
            decimal lMDAge = 0.00m;
            DataTable ldtbList = busBase.Select("cdoPerson.GetMDAgeById", new object[1] { person_id });
             if (ldtbList.Rows.Count > 0)
            {
                if (Convert.ToInt32(ldtbList.Rows[0]["MD_AGE_OPT_ID"]) == 0)
                {
                    lMDAge = busConstant.BenefitCalculation.AGE_70_HALF;
                }
                else
                {
                    lMDAge = Convert.ToDecimal(ldtbList.Rows[0]["MD_AGE"]);

                }
                   

            }

            return lMDAge;
        }

        //RMD72Project
        public static DateTime GetMinDistributionDate(int person_id, DateTime adtVestedDt = new DateTime())//for PIR-522 added 2nd parameter 
        {

            DataTable ldtbList = busBase.Select("cdoPerson.GetMDAgeById", new object[1] { person_id });
            DateTime adtMDDate = new DateTime();
            if (ldtbList.Rows.Count > 0 && ldtbList.Rows[0]["DATE_OF_BIRTH"].ToString().IsNotNullOrEmpty())
            {
                adtMDDate = Convert.ToDateTime(ldtbList.Rows[0]["DATE_OF_BIRTH"]);

            }
           
            DateTime ldtMinDate;

            if (Convert.ToInt32(ldtbList.Rows[0]["MD_AGE_OPT_ID"]) == 0)
            {

                adtMDDate = adtMDDate.AddYears(70);
                adtMDDate = adtMDDate.AddMonths(6);

            }
            else
            {
                int years = (int)Math.Floor(Convert.ToDecimal(ldtbList.Rows[0]["MD_AGE"]));
                decimal months = Convert.ToDecimal(ldtbList.Rows[0]["MD_AGE"]) - years;
                int actualMonths = (int)Math.Floor(months * 12);

                adtMDDate = adtMDDate.AddYears(years);
                adtMDDate = adtMDDate.AddMonths(actualMonths);

            }

            if (adtVestedDt != DateTime.MinValue && adtVestedDt > adtMDDate)
            {
                ldtMinDate = new DateTime(adtVestedDt.Year + 1, 01, 01);
               // lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                ldtMinDate = new DateTime(adtMDDate.Year + 1, 04, 01);
              //  lstrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }

            return ldtMinDate;
        }

        public static string CalculateFebDate(DateTime adtDateOfBirth)
        {
            string lstrFebDate = string.Empty;
            DateTime ldtMinDate;
            DateTime ldtFebDate;
            adtDateOfBirth = adtDateOfBirth.AddYears(70);
            adtDateOfBirth = adtDateOfBirth.AddMonths(6);
            //iintYear = Convert.ToInt32(ldtDob.Year);

            //istrMinDistriDate = busGlobalFunctions.CalculateMinDistributionDate(icdoPerson.idtDateofBirth);

            if (adtDateOfBirth.Month > 4)
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
                //istrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else if (adtDateOfBirth.Month < 4)
            {
                ldtMinDate = new DateTime(adtDateOfBirth.Year, 04, 01);
                //istrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
            }
            else
            {
                if (adtDateOfBirth.Day > 1)
                {
                    ldtMinDate = new DateTime(adtDateOfBirth.Year + 1, 04, 01);
                    //istrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
                }
                else
                {
                    ldtMinDate = adtDateOfBirth;
                    //istrMinDistriDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMinDate);
                }
            }
            if (DateTime.IsLeapYear(ldtMinDate.Year))
            {
                ldtFebDate = new DateTime(ldtMinDate.Year, 02, 29);
                lstrFebDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtFebDate);
            }
            else
            {
                ldtFebDate = new DateTime(ldtMinDate.Year, 02, 28);
                lstrFebDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtFebDate);
            }

            return lstrFebDate;
        }

        public static DateTime GetFirstDayofMonthAfterGivenDate(DateTime adtGivenDate)
        {
            DateTime ldtFirstDayofMonthAfterGivenDate = new DateTime();
            if (adtGivenDate != DateTime.MinValue)
            {
                ldtFirstDayofMonthAfterGivenDate = GetLastDayofMonth(adtGivenDate).AddDays(1);
            }
            return ldtFirstDayofMonthAfterGivenDate;
        }


        public static bool IsDecimal(this string astrText)
        {
            System.Text.RegularExpressions.Regex regularExpression = new System.Text.RegularExpressions.Regex(@"[0-9]+(\.[0-9][0-9]?)?");
            return regularExpression.Match(astrText).Success;
        }

        //Get First Day Of Week
        public static DateTime GetFirstDayOfWeek(DateTime adtGivenDate)
        {
            if (adtGivenDate != DateTime.MinValue)
            {
                while (adtGivenDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    adtGivenDate = adtGivenDate.AddDays(-1);
                }
            }
            return adtGivenDate;
        }

        //Get Last Day Of Week
        public static DateTime GetLastDayOfWeek(DateTime adtGivenDate)
        {
            if (adtGivenDate != DateTime.MinValue)
            {
                while (adtGivenDate.DayOfWeek != DayOfWeek.Saturday)
                {
                    adtGivenDate = adtGivenDate.AddDays(1);
                }
            }
            return adtGivenDate;
        }

        public static DateTime GetPaymentDayForIAP(DateTime adtGivenDate)
        {
            if (adtGivenDate != DateTime.MinValue)
            {
                while (adtGivenDate.DayOfWeek != DayOfWeek.Friday)
                {
                    adtGivenDate = adtGivenDate.AddDays(1);
                }
            }
            return adtGivenDate;
        }

        public static bool CheckIfPrecisionExceeds(decimal ldecAmount, int aintPrecision)
        {
            SqlDecimal lSqldecAmount;
            ldecAmount = Math.Round(ldecAmount, 2);

            lSqldecAmount = new SqlDecimal(ldecAmount);

            if (lSqldecAmount.Precision > aintPrecision)
            {
                return true;
            }
            return false;
        }



        #endregion

        #region Get Last Day Of Quarter
        public static DateTime GetLastDateOfQuarter(DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                int intYear = adtDateTime.Year;
                DateTime idtLastQuarterDate = DateTime.MinValue;

                DateTime[] endOfQuarters = new DateTime[] {
                                       new DateTime(intYear, 3, 31),
                                       new DateTime(intYear, 6, 30),
                                       new DateTime(intYear, 9, 30),
                                       new DateTime(intYear, 12, 31)};

                idtLastQuarterDate = endOfQuarters.Where(item => item.Subtract(adtDateTime).Days >= 0).First();
                return idtLastQuarterDate;
            }
            else
                return adtDateTime;
        }
        #endregion

        public static void SetXmlAttributeValue(XmlDocument xmlDoc, XmlElement aElement, string astrAttrName, string astrAttrValue)
        {
            if (!string.IsNullOrEmpty(astrAttrValue))
            {
                aElement.SetAttribute(astrAttrName, astrAttrValue);
                //XmlAttribute theAttr = xmlDoc.CreateAttribute(astrAttrName);
                //theAttr.Value = astrAttrValue;
                //node.Attributes.Append(theAttr);
            }
        }

        public static string AddOrdinal(int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString() + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num.ToString() + "st";
                case 2:
                    return num.ToString() + "nd";
                case 3:
                    return num.ToString() + "rd";
                default:
                    return num.ToString() + "th";
            }

        }

        public static string GetFileName(int aintFileID)
        {
            string lstrFileName = string.Empty;
            if (aintFileID == 1)
            {
                lstrFileName = "DeathReport";
            }
            else if (aintFileID == 2)
            {
                lstrFileName = "PreNoteACH";
            }
            else if (aintFileID == 4)
            {
                lstrFileName = "ACHFile";
            }
            else if (aintFileID == 6)
            {
                lstrFileName = "1099";
            }
            else if (aintFileID == 1004)
            {
                lstrFileName = "HealthEligibilityActuary";
            }
            else if (aintFileID == 1005)
            {
                lstrFileName = "PensionActuary";
            }
            else if (aintFileID == 1007)
            {
                lstrFileName = "EDDFile";
            }
            else if (aintFileID == 1008)
            {
                lstrFileName = "CheckRecionciliationServiceOutboundFile";
            }
            else if (aintFileID == 1009)
            {
                lstrFileName = "SmallWorldOutboundFile";
            }
            return lstrFileName;
        }
        public static string ExtractDigits(string input)
        {
            if (input.IsNotNullOrEmpty())
                return new string(input.Where(char.IsDigit).ToArray());
            else return input;
        }

        /// <summary>
        /// Determines whether a string is null or empty
        /// </summary>
        /// <param name="astrText">The text.</param>
        /// <returns>True if string is null or empty. False if not null or not empty</returns>
        public static bool EqualsWithNullCheck(this string astrText, string astrCompareText)
        {
            if (string.IsNullOrEmpty(astrText)) return false;
            if (string.IsNullOrEmpty(astrCompareText)) return false;
            if (astrText.Trim().ToLower() == astrCompareText.Trim().ToLower()) return true;
            return false;
        }
    }
}
