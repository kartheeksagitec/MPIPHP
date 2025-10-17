using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public partial class busNeoSpinBase : busBase
    {

        public int iintPersonID { get; set; }

        public string istrMPIPersonID { get; set; }

        #region [Public Methods]

        /// <summary>
        /// This method is virtual which can be overriden by child class. It return empty string.
        /// </summary>
        /// <returns>Empty string</returns>
        public virtual string GetRecipientEmail()
        {
            return string.Empty;
        }


        public string GetMessage(int aintMessageID)
        {
            return utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(aintMessageID);
        }

        /// <summary>
        /// Gets the message string from database cache server, substitutes parameters 
        /// </summary>
        /// <param name="aintMessageID">Specifies message ID</param>
        /// <param name="aarrParam">Specifies array parameter</param>
        public string GetMessage(int aintMessageID, object[] aarrParam)
        {
            string lstrMessage = string.Empty;
            lstrMessage = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(aintMessageID);
            if (!string.IsNullOrEmpty(lstrMessage))
            {
                lstrMessage = String.Format(lstrMessage, aarrParam);
            }
            return lstrMessage;
        }

        /// <summary>
        /// Gets the message string with square bracket.
        /// </summary>
        /// <param name="astrMessage">Specifies message string</param>
        public string GetMessage(string astrMessage)
        {
            if (!string.IsNullOrEmpty(astrMessage))
            {
                astrMessage = " [ " + astrMessage + " ]";
            }
            return astrMessage;
        }
        /// <summary>
        /// Author                  : Base Solution
        /// Modified By             : NA
        /// Applies To Use cases    : All 
        /// Usage                   : Derive Mime Type From File Name
        /// </summary>
        /// <param name="astrFileName">File Name</param>
        /// <returns>Mime Type From File Name</returns>
        public static string DeriveMimeTypeFromFileName(string astrFileName)
        {
            string lstrMimeType = "application/octet-stream";
            string[] larrDotSplit = astrFileName.Split(".");
            if (larrDotSplit != null && larrDotSplit.Length > 0)
            {
                string lstrFileExtension = larrDotSplit[larrDotSplit.Length - 1];
                switch (lstrFileExtension.ToLower())
                {
                    case "pdf":
                        lstrMimeType = "application/pdf";
                        break;
                    case "doc":
                    case "docx":
                        lstrMimeType = "application/msword";
                        break;
                    case "html":
                        lstrMimeType = "application/iexplore";
                        break;
                }
            }
            return lstrMimeType;
        }
        #endregion

        #region Overridden Methods

        /// <summary>
        /// This function is used to validate the activity instance checklist
        /// </summary>
        /// <returns></returns>
        public override ArrayList OnBpmSubmit()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();
            busSolBpmActivityInstance lbusSolBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
            if (lbusSolBpmActivityInstance != null)
            {
                if (!lbusSolBpmActivityInstance.HasAllRequiredChecklistsCompleted())
                {
                    lutlError = AddError(1565, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (lbusSolBpmActivityInstance.IsCompletedDateFutureDate())
                {
                    lutlError = AddError(1568, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }

                larrResult.AddRange(base.OnBpmSubmit());
            }
            else
            {
                larrResult.AddRange(base.OnBpmSubmit());
            }
            return larrResult;
        }
        #endregion
    }
}
