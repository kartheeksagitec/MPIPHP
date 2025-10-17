using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Net.Mail;
using MPIPHP.Common;

namespace MPIPHP.BusinessObjects
{
    public static class HelperUtil
    {

        private const string _iPASSWORD_CHARS_LCASE = "abcdefghijklmnopqrstuvwxyz";
        private const string _iPASSWORD_CHARS_UCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _iPASSWORD_CHARS_NUMERIC = "1234567890";
        private const string _iPASSWORD_CHARS_SPECIAL = "@!#%*";
        private const string _iAllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789@!#%&*";
        private const string _iPattern = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
        public static string DateDiff(DateTime adtmDt1, DateTime adtmDt2)
        {
            TimeSpan ltsT1 = adtmDt2 - adtmDt1;
            Decimal ldtmD1 = Convert.ToDecimal(ltsT1.TotalDays) / 365.25M;
            Decimal ldtmD2 = Decimal.Floor(ldtmD1);
            Decimal ldecM1 = (ldtmD1 - ldtmD2) * 12;
            Decimal ldecM2 = Decimal.Floor(ldecM1);
            Decimal ldecDay1 = (ldecM1 - ldecM2) * 24;
            Decimal ldecDay2 = Decimal.Floor(ldecDay1);
            return ldtmD2 + " Years " + ldecM2 + " Months " + ldecDay2 + " Days ";
        }

        //public static void SendMail(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject, string smtpServer, string userName, string password, int port, int cdoBasic, int cdoSendUsingPort)
        //{
        //    MailMessage mail = new MailMessage();
        //    mail.To.Add(new MailAddress(astrToAddress));
        //    mail.From = new MailAddress(astrFromAddress);
        //    mail.Subject = astrSubject;
        //    mail.Body = astrBody;

        //    System.Net.Mail.SmtpClient lobjSmtpClient = new System.Net.Mail.SmtpClient(smtpServer, port);
        //    lobjSmtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
        //    lobjSmtpClient.Send(mail);
        //}

        //public static void SendEmailForMSS(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject)
        //{
        //    string smtpServer = MPIPHP.Common.ApplicationSettings.Instance.SMTP_HOST_PATH;
        //    string userName = MPIPHP.Common.ApplicationSettings.Instance.SMTP_USERNAME;
        //    string password = MPIPHP.Common.ApplicationSettings.Instance.SMTP_PASSWORD;
        //    int port = Convert.ToInt32(MPIPHP.Common.ApplicationSettings.Instance.SMTP_HOST_PORT);
        //    int cdoBasic = 1;
        //    int cdoSendUsingPort = 2;
        //    SendMail(astrToAddress, astrFromAddress, astrBody, astrSubject, smtpServer, userName, password, port, cdoBasic, cdoSendUsingPort);
        //}

        public static string CreateRandomPassword(int aintPasswordLength)
        {
            
            char[] chars = new char[aintPasswordLength];
            Random randNum = new Random();

            for (int i = 0; i < aintPasswordLength; i++)
            {
                chars[i] = _iAllowedChars[randNum.Next(0, _iAllowedChars.Length)];
            }
            return new string(chars);
        }

        public static string GenerateRandomPassword(int aintMinLength)
        {
            int maxLength = aintMinLength;
            // Make sure that input parameters are valid.
            if (aintMinLength <= 0 || maxLength <= 0 || aintMinLength > maxLength)
            return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] larrCharGroups = new char[][] 
            {
                _iPASSWORD_CHARS_LCASE.ToCharArray(),
                _iPASSWORD_CHARS_UCASE.ToCharArray(),
                _iPASSWORD_CHARS_NUMERIC.ToCharArray(),
                _iPASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] larrCharsLeftInGroup = new int[larrCharGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < larrCharsLeftInGroup.Length; i++)
                larrCharsLeftInGroup[i] = larrCharGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] larrLeftGroupsOrder = new int[larrCharGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < larrLeftGroupsOrder.Length; i++)
                larrLeftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] larrRandomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(larrRandomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (larrRandomBytes[0] & 0x7f) << 24 |
                        larrRandomBytes[1] << 16 |
                        larrRandomBytes[2] << 8 |
                        larrRandomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] larrPassword = null;

            // Allocate appropriate memory for the password.
            if (aintMinLength < maxLength)
                larrPassword = new char[random.Next(aintMinLength, maxLength + 1)];
            else
                larrPassword = new char[aintMinLength];

            // Index of the next character to be added to password.
            int lintNextCharIdx;

            // Index of the next character group to be processed.
            int lintNextGroupIdx;

            // Index which will be used to track not processed character groups.
            int lintNextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lintLastCharIdx;

            // Index of the last non-processed group.
            int lintLastLeftGroupsOrderIdx = larrLeftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < larrPassword.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lintLastLeftGroupsOrderIdx == 0)
                    lintNextLeftGroupsOrderIdx = 0;
                else
                    lintNextLeftGroupsOrderIdx = random.Next(0,
                                                         lintLastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                lintNextGroupIdx = larrLeftGroupsOrder[lintNextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lintLastCharIdx = larrCharsLeftInGroup[lintNextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lintLastCharIdx == 0)
                    lintNextCharIdx = 0;
                else
                    lintNextCharIdx = random.Next(0, lintLastCharIdx + 1);

                // Add this character to the password.
                larrPassword[i] = larrCharGroups[lintNextGroupIdx][lintNextCharIdx];

                // If we processed the last character in this group, start over.
                if (lintLastCharIdx == 0)
                    larrCharsLeftInGroup[lintNextGroupIdx] =
                                              larrCharGroups[lintNextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lintLastCharIdx != lintNextCharIdx)
                    {
                        char temp = larrCharGroups[lintNextGroupIdx][lintLastCharIdx];
                        larrCharGroups[lintNextGroupIdx][lintLastCharIdx] =
                                    larrCharGroups[lintNextGroupIdx][lintNextCharIdx];
                        larrCharGroups[lintNextGroupIdx][lintNextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    larrCharsLeftInGroup[lintNextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lintLastLeftGroupsOrderIdx == 0)
                    lintLastLeftGroupsOrderIdx = larrLeftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lintLastLeftGroupsOrderIdx != lintNextLeftGroupsOrderIdx)
                    {
                        int temp = larrLeftGroupsOrder[lintLastLeftGroupsOrderIdx];
                        larrLeftGroupsOrder[lintLastLeftGroupsOrderIdx] =
                                    larrLeftGroupsOrder[lintNextLeftGroupsOrderIdx];
                        larrLeftGroupsOrder[lintNextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lintLastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(larrPassword);
        }


        public static string FormatStringtoPreventSQLInjection(string astrInput)
        {
            string lstrOutput = astrInput.Trim().Replace("'", "''");
            return lstrOutput;
        }

        public static Hashtable lht_String_To_Reporting_Frequency = new Hashtable();
        static HelperUtil()
        {
            if (lht_String_To_Reporting_Frequency == null) return;
            lht_String_To_Reporting_Frequency.Add(BatchHelper.REPORTING_FREQUENCY_MONTHLY,
                                                  REPORTING_FREQUENCY.MONTHLY);
            lht_String_To_Reporting_Frequency.Add(BatchHelper.REPORTING_FREQUENCY_SEMI_MONTHLY,
                                                  REPORTING_FREQUENCY.SEMIMONTHLY);
            lht_String_To_Reporting_Frequency.Add(BatchHelper.REPORTING_FREQUENCY_BIWEEKLY,
                                                  REPORTING_FREQUENCY.BIWEEKLY);
        }

        public static bool CheckEmailAddress(string strEmailAddress)
        {
            bool lblnCheck = false;

            Regex lrexRg = new Regex(_iPattern);
            lblnCheck = lrexRg.IsMatch(strEmailAddress);
            return lblnCheck;
        }

        /// <summary>
        /// To limit the Decimal data type field, not to throw Overflow exception when given a bulk value
        /// Pass the Data Type Precision and Scale of the Data Field and Pass the Object Field
        /// </summary>
        /// <param name="lintPrecisionValue"></param>
        /// <param name="lintScaleValue"></param>
        /// <param name="lstrDatafield"></param>
        /// <returns></returns>
        public static bool CheckForDecimalFormat(int lintPrecisionValue, int lintScaleValue, string lstrDatafield)
        {
            int lintNumericValue = lintPrecisionValue - lintScaleValue;
            string lstrNumericVal = Convert.ToString(lintNumericValue);
            string lstrInput = @"^[0-9]{1," + lintNumericValue.ToString() + @"}(\.[0-9]{0," + lintScaleValue.ToString() + "})?$";
            Regex lrexReg = new Regex(lstrInput);
            if (!lrexReg.IsMatch(lstrDatafield))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check for special characters in the passed in string
        /// If special character is found returns True else False
        /// </summary>
        /// <param name="astrParam"></param>
        /// <returns></returns>
        public static bool CheckForSpecialChars(string astrParam)
        {
            bool lblnReturn = false;
            Regex lrexRe = new Regex("[0-9!@#$%&()+-=]");
            if (lrexRe.IsMatch(astrParam))
            {
                lblnReturn = true;
            }
            return lblnReturn;
        }

        // This method will omit the special char that needs to be allowed
        // If special character is found returns True else False
        public static bool CheckForSpecialChars(string astrParam, string astrInvChar)
        {
            bool lblnReturn = false;
            string lstrInvalidCharList = "[0-9!@#$%&()+-=]";

            if (astrInvChar != "")
            {
                lstrInvalidCharList = lstrInvalidCharList.Replace(astrInvChar, "");
            }
            Regex lrexRe = new Regex(lstrInvalidCharList);
            if (lrexRe.IsMatch(astrParam))
            {
                lblnReturn = true;
            }
            return lblnReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckForDecimal(string astrParam)
        {
            bool lblnResultVal = false;
            Regex lrexRe = new Regex("[.]");
            if (lrexRe.IsMatch(astrParam))
            {
                lblnResultVal = true;
            }
            return lblnResultVal;
        }

        ///// <summary>
        ///// Method to check whether the given field doesnot have any decimal value
        ///// </summary>
        ///// <returns></returns>
        public static bool IsInteger(string astrParam)
        {
            Regex lrexIsNumber = new Regex(@"^-?\d+$");
            Match m = lrexIsNumber.Match(astrParam);
            return m.Success;
        }

        /// <summary>
        /// Helper function to check whether the date that is passed as a parameter
        /// is the first day of the month.
        /// </summary>
        /// <param name="adtmCheck"></param>
        /// <param name="aenmReportingFrequency"></param>
        /// <returns></returns>
        public static bool CheckFirstDate(DateTime adtmCheck, REPORTING_FREQUENCY aenmReportingFrequency)
        {
            bool lblnCheck = false;
            if (aenmReportingFrequency == REPORTING_FREQUENCY.MONTHLY)
            {
                if (adtmCheck.Day == 1)
                    lblnCheck = true;
            }
            else if (aenmReportingFrequency == REPORTING_FREQUENCY.SEMIMONTHLY)
            {
                if (adtmCheck.Day == 1 || adtmCheck.Day == 16)
                    lblnCheck = true;
            }
            return lblnCheck;
        }

        /// <summary>
        /// Helper function to check whether the date that is passed as a parameter
        /// is the last day of the month.
        /// </summary>
        /// <param name="adtmCheck"></param>
        /// <returns></returns>
        public static bool CheckLastDate(DateTime adtmCheck, REPORTING_FREQUENCY aenmReportingFrequency)
        {
            bool lblnCheck = false;
            // Incase the above of reporting frequency being Semimonthly, the end date can be 15th as well.
            if (aenmReportingFrequency == REPORTING_FREQUENCY.SEMIMONTHLY)
            {
                DateTime dtMidDayOfMonth = new DateTime(adtmCheck.Year, adtmCheck.Month, 15);
                if (adtmCheck == dtMidDayOfMonth)
                    lblnCheck = true;
            }
            // If the above condition is not satisfied, for both SemiMonthly and Monthly Reporting Frequency
            // we need to check for last day of the month or not.
            if (!lblnCheck)
            {
                if (adtmCheck == GetLastDayOfMonth(adtmCheck))
                    lblnCheck = true;
                return lblnCheck;
            }

            return lblnCheck;
        }

        public static DateTime GetLastDayOfMonth(DateTime adtmCheck)
        {
            DateTime ldtLastday;
            int lintDaysinMonth;
            // Get the number of days in the month from the date that is being passed
            lintDaysinMonth = DateTime.DaysInMonth(adtmCheck.Year, adtmCheck.Month);
            // calculate the last day of the month by adding the number of days to the FirstDay of the month
            ldtLastday = GetFirstDayOfMonth(adtmCheck).AddDays(lintDaysinMonth - 1);
            return ldtLastday;
        }

        public static DateTime GetFirstDayOfMonth(DateTime adtmCheck)
        {
            // Get the firstday of the month that is being passed
            DateTime ldtFirstday = new DateTime(adtmCheck.Year, adtmCheck.Month, 1);
            return ldtFirstday;
        }

        // Static method to check if the Date is the First Day of the month and Return True or False
        public static bool CheckDateIsFirstDayOfMonth(DateTime adtmDatetime)
        {
            if (adtmDatetime == HelperUtil.GetFirstDayOfMonth(adtmDatetime))
            {
                return true; // throw error
            }
            return false;
        }

        /// <summary>
        /// Find the number of months encompassed between the given dates.
        /// </summary>
        /// <param name="adtmStartDate">Start Date</param>
        /// <param name="adtmEndDate">End Date</param>
        /// <returns>No of Months.</returns>
        public static int GetNoOfMonthsEncompassedByDates(DateTime adtmStartDate, DateTime adtmEndDate)
        {
            return Math.Abs(12 * (adtmStartDate.Year - adtmEndDate.Year) + adtmStartDate.Month - adtmEndDate.Month) + 1;            
        }

        //Helper method to get constant value from the database by passing the Code Value
        public static object GetData1FromConstant(string astrCodeValue)
        {
            object lobjReturn = null;
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(new string[2] {"code_id", "code_value"},
                                                          new object[2] { BatchHelper.CODE_ID_SYSTEM_CONSTANTS, astrCodeValue }, null, null);
            if (ldtbCodeValue.Rows.Count > 0)
            {
                lobjReturn = ldtbCodeValue.Rows[0]["data1"];
            }
            return lobjReturn;
        }
        /// <summary>
        /// Find the Maximum of two dates
        /// </summary>
        /// <param name="adtmDate1">Date 1</param>
        /// <param name="adtmDate2">Date 2</param>
        /// <returns>Maximum Date</returns>
        public static DateTime GetMaxDate(DateTime adtmDate1, DateTime adtmDate2)
        {
            return (adtmDate1 > adtmDate2 ? adtmDate1 : adtmDate2);
        }
         
        /// <summary>
        /// Function to get data1 valuefor the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrCodeValue"></param>
        /// <returns></returns>
        public static string GetData1ByCodeValue(int aintCodeId, string astrCodeValue)
        {
            //Helper method to get constant value from the database by passing the Code ID and Code Value
            string lstrResult = null;
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(
                new string[2] { "code_id", "code_value" },
                new object[2] { aintCodeId, astrCodeValue }, null, null);
            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["data1"].ToString();
            }
            return lstrResult;
        }

        /// <summary>
        /// Function to get data2 valuefor the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrCodeValue"></param>
        /// <returns></returns>
        public static string GetData2ByCodeValue(int aintCodeId, string astrCodeValue)
        {
            //Helper method to get constant value from the database by passing the Code ID and Code Value
            string lstrResult = null;
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(
                new string[2] { "code_id", "code_value" },
                new object[2] { aintCodeId, astrCodeValue }, null, null);
            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["data2"].ToString();
            }
            return lstrResult;
        }

        /// <summary>
        /// Function to get Codevalue data for the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrCodeValue"></param>
        /// <returns></returns>
        public static cdoCodeValue GetCodeValueDetails(int aintCodeId, string astrCodeValue)
        {
            cdoCodeValue lobjcdoCodeValue = null;
            DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] {"code_id", "code_value"},
                                                               new object[2] {aintCodeId, astrCodeValue}, null, null);
            if (ldtblList.Rows.Count > 0)
            {
                lobjcdoCodeValue = new cdoCodeValue();
                lobjcdoCodeValue.LoadData(ldtblList.Rows[0]);
            }
            return lobjcdoCodeValue;
        }

        public static cdoCodeValue GetCodeValueByDescription(int aintCodeId, string astrDesc)
        {
            cdoCodeValue lobjcdoCodeValue = null;
            DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] { "code_id", "description" },
                                                               new object[2] { aintCodeId, astrDesc }, null, null);
            if (ldtblList.Rows.Count > 0)
            {
                lobjcdoCodeValue = new cdoCodeValue();
                lobjcdoCodeValue.LoadData(ldtblList.Rows[0]);
            }
            return lobjcdoCodeValue;
        }

        public static string IsNull(string astrInput)
        {
            string lstrOutput = astrInput ?? "";
            // converting to lower since we do comparision to some other value in most places.
            return lstrOutput.ToLower();
        }


        public static bool CheckMaxDays(DateTime adtmstart, DateTime adtmEnd, REPORTING_FREQUENCY aenmReportingFrequency)
        {
            bool lblnCheck = false;
            switch (aenmReportingFrequency)
            {
                case REPORTING_FREQUENCY.MONTHLY:
                    {
                        // For Monthly reporting frequencies we should need to check Days Diference between start date and end date should not exceed 30 days.
                        // if it exceeds 30 days returns false.
                        TimeSpan ltsDiffBetweenDates = adtmEnd.Subtract(adtmstart);
                        int lintDaysDiff = ltsDiffBetweenDates.Days + 1;
                        if (lintDaysDiff <= (Convert.ToInt32(GetData1FromConstant(
                                                    BatchHelper.NUMBER_OF_DAYS_DIFFERENCE_FOR_MONTHLY_REPORTING_FREQUENCY))))
                            lblnCheck = true;
                        break;
                    }

                case REPORTING_FREQUENCY.SEMIMONTHLY:
                    {
                        // For Semi Monthly reporting frequencies we should need to check Days Diference between start date and end date should not exceed 15 days.
                        // if it exceeds 15 days returns false.
                        TimeSpan ltsDiffBetweenDates = adtmEnd.Subtract(adtmstart);
                        int lintDaysDiff = ltsDiffBetweenDates.Days + 1;
                        if (lintDaysDiff <= (Convert.ToInt32(GetData1FromConstant(
                                                    BatchHelper.NUMBER_OF_DAYS_DIFFERENCE_FOR_SEMIMONTHLY_REPORTING_FREQUENCY))))
                            lblnCheck = true;
                        break;
                    }

                case REPORTING_FREQUENCY.BIWEEKLY:
                    {
                        // changes made on 24th Nov'08 - followup 20th Nov as discussed with jaswinder to change the Bi-Weelky day diff from 14 days
                        // For biweekly reporting frequencies we would need to check only if the date range between start date and end date
                        // is 14 days.  If it's not 14 days then we would need to raise an hard error.
                        TimeSpan ltsDiffBetweenDates = adtmEnd.Subtract(adtmstart);
                        int lintDaysDiff = ltsDiffBetweenDates.Days + 1;
                        if (lintDaysDiff == (Convert.ToInt32(GetData1FromConstant(
                                                    BatchHelper.NUMBER_OF_DAYS_DIFFERENCE_FOR_BIWEEKLY_REPORTING_FREQUENCY))))
                            lblnCheck = true;
                        break;
                    }
            }
            return lblnCheck;
        }

        /// <summary>
        /// Method to Sort the Collection by Given Property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrSortExpression">Object Property</param>
        /// <param name="aclbCollection">Source Collection</param>
        /// <returns></returns>
        public static Collection<T> Sort<T>(string astrSortExpression, Collection<T> aclbCollection)
        {
            utlSortComparer lobjComparer = new utlSortComparer();
            lobjComparer.istrSortExpression = astrSortExpression;
            ArrayList larrBase = new ArrayList(aclbCollection);
            larrBase.Sort(lobjComparer);
            aclbCollection.Clear();
            foreach (object lobjTemp in larrBase)
            {
                aclbCollection.Add((T)lobjTemp);
            }
            return aclbCollection;
        }

        /// <summary>
        /// Merge Source Collection to Target Collection.
        /// </summary>
        /// <param name="aclbSourceCollection">Source</param>
        /// <param name="aclbTargetCollection">Target</param>
        public static void MergeCollection<T>(Collection<T> aclbSourceCollection, Collection<T> aclbTargetCollection)
        {
            // To avoid Null Object reference
            if (aclbSourceCollection == null || aclbTargetCollection == null)
                return;

            // Add the source items to the target collection.
            foreach (T lobjTemp in aclbSourceCollection)
            {
                aclbTargetCollection.Add(lobjTemp);
            }
        }

        /// <summary>
        /// Converts a collection of one type objects to a collection of another type of objects
        /// provided the types are convertible, e.g. base class to derived class and vice versa.
        /// </summary>
        /// <typeparam name="FromType">Source Type</typeparam>
        /// <typeparam name="ToType">Target Type</typeparam>
        /// <param name="clbFrom">Source Collection</param>
        /// <returns>Collection of converted objects</returns>
        public static Collection<ToType> ChangeType<FromType, ToType>(Collection<FromType> clbFrom)
        {
            Collection<ToType> clbTo = new Collection<ToType>();
            foreach (FromType objFrom in clbFrom)
            {
                if(!(objFrom != null && objFrom is ToType))
                    throw new InvalidCastException("Cannot convert type " + objFrom.GetType().ToString() + " to type " + typeof(ToType) + ".");
                clbTo.Add((ToType)(object)objFrom);
            }

            return clbTo;
        }

        /// <summary>
        /// This helper method is used to calculate the Member or Beneficiary Age for given date using Age Calculation Methodology
        /// Number of days in a Month will be considered as 30 days flat and Number of days in Year will be considered as 365 days flat.
        /// </summary>
        /// <param name="adtFromDate">Member/Beneficiary Birth date</param>
        /// <param name="adtmToDate">Date used to find the Age (Benefit Effective Date)</param>
        /// <returns>Returns the calculate Age for the given date</returns>
        public static decimal CalculateAgeForGivenDateUsingAgeCalculationMethodology(DateTime adtFromDate, DateTime adtmToDate)
        {
            int lintDifferenceInYears = 0;
            int lintDifferenceInDays = 0;
            int lintDifferenceInMonths = 0;
            int lintMonthToDays = 0;
            decimal ldecNumberOfDaysInFraction = 0.0m;
            decimal ldecAgeInFraction = 0.0m;

            //Calculates Age only when given date is greater than the birth date
            if (adtmToDate > adtFromDate)
            {
                // First find the difference in Years, Months and Days
                lintDifferenceInYears = adtmToDate.Year - adtFromDate.Year;
                lintDifferenceInDays = adtmToDate.Day - adtFromDate.Day;
                lintDifferenceInMonths = adtmToDate.Month - adtFromDate.Month;
                // Borrowed 1 year to months (12) : 2009 12       (Borrowed one year which is 12 months and added with the actual months 12+0 = 12 months)
                // Borrowed 1 month to days (30)  :      00 30+1  (Borrowed one month which is 30 days and added with actual days  30+1 = 31 days)
                // Example Benefit Effective Date : 2010 01 01  (yyyy,mm,dd format)
                // Member Birth date:               1953 06 14  
                //                                ---------------- (when subtraction the benefit calculation module has to consider that number of days in a month as 30 days and number of months in years as 12 months)
                // Subtracting the above gives        56  6 17   (56 years 6 months and 17 days would be members age) 

                // If difference in days is less than zero (negative value) then borrow one month from difference in months value
                if (lintDifferenceInDays < 0)
                {
                    // If difference in months is less than zero (negative value) then borrow one year from difference in years value
                    if (lintDifferenceInMonths < 0)
                    {
                        // To borrow one year we need to minus one year and add that one year which is 12 months with difference in months value
                        lintDifferenceInYears--;
                        lintDifferenceInMonths = lintDifferenceInMonths + BatchHelper.NUMBER_OF_MONTHS_IN_YEAR;
                    }
                    // To borrow one month we need to minus one month and add that one month which is 30 days with difference in days value
                    lintDifferenceInMonths--;
                    lintDifferenceInDays = lintDifferenceInDays + BatchHelper.NUMBER_OF_DAYS_IN_MONTH;
                }
                // If difference in months is less than zero (negative value) then borrow one year from difference in years value
                if (lintDifferenceInMonths < 0)
                {
                    // To borrow one year we need to minus one year and add that one year which is 12 months with difference in months value
                    lintDifferenceInYears--;
                    lintDifferenceInMonths = lintDifferenceInMonths + BatchHelper.NUMBER_OF_MONTHS_IN_YEAR;
                }

                //Convert Months to Days by multiplying 30 days for each month
                lintMonthToDays = lintDifferenceInMonths * BatchHelper.NUMBER_OF_DAYS_IN_MONTH;

                //Finds the fractional days in a year by adding months to days with difference in days and dividing them by flat 365 days
                ldecNumberOfDaysInFraction = Math.Round((lintMonthToDays + lintDifferenceInDays) / Convert.ToDecimal(BatchHelper.NUMBER_OF_DAYS_IN_YEAR), 4);
                ldecAgeInFraction = lintDifferenceInYears + ldecNumberOfDaysInFraction;
            }
            return ldecAgeInFraction;
        }

        // Commented by Suresh on Friday April 10, 2009. UAT issue Fix. This is done to remove the display fields Member Age at Benefit Effective Date & Beneficiary Age at Benefit Effective Date 
        // from Age Calc Tab which is calculated without using the Age Calculation Methodolgy.
        ///// <summary>
        ///// This helper method is used to calculate the Member or Beneficiaries Actual Age for the given date.
        ///// Number of days in the Month will be taken the actual days in that Month and Number of days in Year will be taken the actual days in that Year(365 or 366 on leap year)
        ///// </summary>
        ///// <param name="adtBirthDate">Member/Beneficiary Birth date</param>
        ///// <param name="adtGivenDate">Date used to find the Age (Benefit Effective Date)</param>
        ///// <returns>Returns the calculate Age for the given date</returns>
        //public static decimal CalculateAgeForGivenDate(DateTime adtBirthDate, DateTime adtGivenDate)
        //{
        //    int lintDifferenceInYears = 0;
        //    decimal ldecNumberOfDaysInFraction = 0.0m;
        //    decimal ldecAgeInFraction = 0.0m;
        //    int lintNumberOfDaysInYear = 0;
        //    DateTime ldtNewDate = DateTime.MinValue;

        //    //Calculates Age only when given date is greater than the birth date
        //    if (adtGivenDate > adtBirthDate)
        //    {
        //        //Finds the difference in Year
        //        lintDifferenceInYears = adtGivenDate.Year - adtBirthDate.Year;
        //        //Finds the fractional days in the last year by differenece in months and difference in days divided by number of days in the year of (dtGivenDate)
        //        ldtNewDate = DateTime.Parse(adtBirthDate.Month + "/" + adtBirthDate.Day + "/" + adtGivenDate.Year);
        //        TimeSpan ltsDifference = adtGivenDate.Subtract(ldtNewDate);
        //        //Consider Leap Year condition. If leap year then Number of days in Year will be taken as 366 days else 365 days
        //        lintNumberOfDaysInYear = HelperFunction.GetDaysInAYear(adtGivenDate.Year);
        //        ldecNumberOfDaysInFraction = Math.Round(ltsDifference.Days / Convert.ToDecimal(lintNumberOfDaysInYear), 4);

        //        ldecAgeInFraction = lintDifferenceInYears + ldecNumberOfDaysInFraction;
        //    }
        //    return ldecAgeInFraction;
        //}

        // by passing the Message Id, the system will get the Message Description from the DBCache
        public static string GetMessage(utlPassInfo aobjPassInfo, int aintMessageId)
        {
            string lstrMessage = string.Empty;
            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
            //if (aobjPassInfo.isrvDBCache.GetMessageInfo(aintMessageId).Rows.Count > 0)            
            //    lstrMessage = aobjPassInfo.isrvDBCache.GetMessageInfo(aintMessageId).Rows[0]["display_message"].ToString();            
            //else            
            //    lstrMessage = "Invalid Message ID : " + aintMessageId.ToString();
            utlMessageInfo lobjutlMessageInfo = aobjPassInfo.isrvDBCache.GetMessageInfo(aintMessageId);
            if (lobjutlMessageInfo != null)
                lstrMessage = lobjutlMessageInfo.display_message;
            else
                lstrMessage = "Invalid Message ID : " + aintMessageId.ToString();
            return lstrMessage;
        }

        /// <summary>
        /// Print the file to the specified printer.
        /// </summary>
        /// <param name="astrAbsoluteFileName">Absolute File Name</param>
        /// <param name="astrPrinterName">Printer Name</param>
        /// <param name="ablnLandscapeMode">Print in Landscape Mode or Not.</param>
        public static void PrintFile(string astrAbsoluteFileName, string astrPrinterName, bool ablnLandscapeMode)
        {
            //isrmReader = new StreamReader(astrAbsoluteFileName);
            //PrintDocument lprnDoc = new PrintDocument();
            //lprnDoc.PrinterSettings.PrinterName = astrPrinterName;
            //lprnDoc.PrintPage += new PrintPageEventHandler(lprnDoc_PrintPage);
            //lprnDoc.DefaultPageSettings.Landscape = ablnLandscapeMode;
            //lprnDoc.Print();
        }

        /// <summary>
        /// Print Page Event Handler
        /// </summary>    
        //private static void lprnDoc_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    //Graphics lgrp = e.Graphics;

        //    //// Print Settings
        //    //float lfltLinesPerPage = 0;
        //    //float lfltYPos = 0;
        //    //int lintCount = 0;
        //    //float llftLeftMargin = e.MarginBounds.Left;
        //    //float llftTopMargin = e.MarginBounds.Top;
        //    //Font lfnt = new Font(busConstant.FILE_PRINT_FONT, busConstant.FILE_PRINT_FONT_SIZE);
        //    //lfltLinesPerPage = e.MarginBounds.Height / lfnt.GetHeight(lgrp);

        //    //// Read the lines in the stream object and print.
        //    //string lstrline = null;
        //    //while (lintCount < lfltLinesPerPage && ((lstrline = isrmReader.ReadLine()) != null))
        //    //{
        //    //    lfltYPos = llftTopMargin + (lintCount * lfnt.GetHeight(lgrp));
        //    //    lgrp.DrawString(lstrline, lfnt, Brushes.Black, llftLeftMargin, lfltYPos, new StringFormat());
        //    //    lintCount++;
        //    //}

        //    //// More than one page.
        //    //if (lstrline != null)
        //    //    e.HasMorePages = true;
        //}

        /// <summary>
        /// To format the given string by replacing the comma by space
        /// </summary>
        /// <param name="astrInput"></param>
        /// <returns></returns>
        public static string FormatStringToReplaceCommaBySpace(string astrInput)
        {
            if (astrInput != null && astrInput.Trim().Length > 0)
                return astrInput.Trim().Replace(BatchHelper.COMMA, BatchHelper.SPACE);
            return astrInput;
        }

        // Added For Benefit PopUp - Saranya: 7th Jan 2010
        // TODO: Same method with same logic exists in the busJSPaymentOptionVisitor.cs file. Need to do changes for this method
        public static decimal CalculateBasicBenefitMultiplier(decimal adecMemberAge, decimal adecBeneficiaryAge,
            decimal adecSameAgeMultiplier, decimal adecMemberAgeGreaterThanBeneficiaryAgeMultiplier,
            decimal adecDifferenceInAgeMultiplierUptoNineYears, decimal adecDifferenceInAgeMultiplierOverNineYears, decimal adecAgeDiffAtBenefitEffctvDate)
        {
            decimal ldecBasicBenefitMultiplier = 0.00m;
            decimal ldecAgeReductionFactor = 0.00m;

            // Checks Member Age and Beneficiary Age are same, where the Basic Multiplier 
            // For JS 50% is 0.920 and For JS 100% is 0.861
            if (adecMemberAge == adecBeneficiaryAge)
            {
                ldecBasicBenefitMultiplier = adecSameAgeMultiplier;
                return ldecBasicBenefitMultiplier;
            }

            // If Member Age is greater than Beneficiary Age, then Age Reduction Factor will be 
            // For JS 50% --> Difference in Age Multiplied by 0.004 and Basic Benefit multiplier will be 0.920 minus result of Age Reduction Factor
            // For JS 100% --> Difference in Age Multiplied by 0.005 and Basic Benefit multiplier will be 0.861 minus result of Age Reduction Factor
            if (adecMemberAge > adecBeneficiaryAge)
            {
                ldecAgeReductionFactor = adecAgeDiffAtBenefitEffctvDate * adecMemberAgeGreaterThanBeneficiaryAgeMultiplier;
                ldecBasicBenefitMultiplier = adecSameAgeMultiplier - ldecAgeReductionFactor;
            }
            else //for Member Age less than Beneficiary Age
            {
                //If Difference in Age is upto 9 years, then Age Reduction Factor will be 
                //For JS 50% --> Difference in Age Multiplied by 0.006
                //For JS 100% --> Difference in Age Multiplied by 0.01
                if (adecAgeDiffAtBenefitEffctvDate <= BatchHelper.DIFFERENCE_IN_AGE_CHECK)
                {
                    ldecAgeReductionFactor = adecAgeDiffAtBenefitEffctvDate * adecDifferenceInAgeMultiplierUptoNineYears;
                }
                else
                {
                    //If Difference in Age is over 9 years, then Age Reduction Factor will be 
                    //For JS 50% --> Difference in Age multiplied by 0.004
                    //For JS 100% --> Difference in Age multiplied by 0.002

                    // Changed By: Suresh, January 27, 2010. Fix for Production PIR: 178
                    // Age Reduction Factor Calculation for Difference in Age over 9 years
                    // For JS 50% --> Find the age reduction factor upto 9 years by multiplying 0.006 * 9 yrs
                    //                then find the age reduction factor for over 9 years by multiplying 0.004 * remaining difference in age. Say 21.5918 yrs, that would be (21.5918 yrs - 9 yrs) * 0.004
                    //                Finally sum both the age reduction factor upto 9 yrs and over 9 yrs

                    // For JS 100% --> Find the age reduction factor upto 9 years by multiplying 0.01 * 9 yrs
                    //                 then find the age reduction factor for over 9 years by multiplying 0.002 * remaining difference in age. Say 21.5918 yrs, that would be (21.5918 yrs - 9 yrs) * 0.002
                    //                 Finally sum both the age reduction factor upto 9 yrs and over 9 yrs
                    decimal ldecAgeReductionFactorUptoNineYrs = BatchHelper.DIFFERENCE_IN_AGE_CHECK * adecDifferenceInAgeMultiplierUptoNineYears;
                    decimal ldecAgeReductionFactorOverNineYrs = (adecAgeDiffAtBenefitEffctvDate - BatchHelper.DIFFERENCE_IN_AGE_CHECK) * adecDifferenceInAgeMultiplierOverNineYears;
                    ldecAgeReductionFactor = Math.Round(ldecAgeReductionFactorUptoNineYrs + ldecAgeReductionFactorOverNineYrs, 4);
                }

                //Basic Benefit multiplier will be 
                //For JS 50% --> 0.920 plus result of Age Reduction Factor
                //For JS 100% --> 0.861 plus result of Age Reduction Factor
                ldecBasicBenefitMultiplier = adecSameAgeMultiplier + ldecAgeReductionFactor;
                // Changed By: Suresh, January 27, 2010. Fix for Production PIR: 178.
                // To cap the Basic Benefit Multiplier to 1, if at all its greater than 1.
                if (ldecBasicBenefitMultiplier > BatchHelper.CAP_FOR_JS_BASIC_BENEFIT_MULTIPLIER)
                    ldecBasicBenefitMultiplier = BatchHelper.CAP_FOR_JS_BASIC_BENEFIT_MULTIPLIER;
            }
            // This rounded to 4 decimal places as per Pam requested in PIR 144
            return Math.Round(ldecBasicBenefitMultiplier, 4);
        }
    }
}
