#region [Using directives]
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using System;
using System.Data;
using System.Net;
#endregion [Using directives]

namespace NeoBase.BPM
{
    [Serializable]
    public class busNeobaseBpmUsersEscalationMessage : busBpmUsersEscalationMessage
    {
        public busNeobaseBpmUsersEscalationMessage() : base()
        {
            icdoBpmUsersEscalationMessage = new doBpmUsersEscMessage();
        }

        /// <summary>
        /// Property to hold the Escalation Message for Checkout Users.
        /// </summary>
        public string istrEscalationMessage { get; set; }

        /// <summary>
        /// Property to hold the Checkout User Details.
        /// </summary>
        public string istrCheckOutUser { get; set; }

        /// <summary>
        /// Property to hold the Escalation Message Type.
        /// </summary>
        public string istrEscalationMessageType { get; set; }

        /// <summary>
        /// This function is used to Load Other Objects associated with Checkout User.
        /// </summary>
        /// <param name="adtrRow"></param>
        /// <param name="abusBus"></param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBus)
        {
            //EM.ESCALATION_MESSAGE, UM.MARK_AS_READ, AI.CHECKED_OUT_USER
            if (adtrRow.Table.Columns.Contains("CHECKED_OUT_USER"))
            {
                if (adtrRow["CHECKED_OUT_USER"] != DBNull.Value)
                {
                    ((busNeobaseBpmUsersEscalationMessage)abusBus).istrCheckOutUser = adtrRow["CHECKED_OUT_USER"].ToString();
                }
                else
                {
                    ((busNeobaseBpmUsersEscalationMessage)abusBus).istrCheckOutUser = "";
                }
            }

            if (adtrRow["ESCALATION_MESSAGE"] != DBNull.Value && !string.IsNullOrEmpty(adtrRow["ESCALATION_MESSAGE"].ToString()))
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBus).istrEscalationMessage = WebUtility.HtmlEncode(adtrRow["ESCALATION_MESSAGE"].ToString());
            }
            else
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBus).istrEscalationMessage = "";
            }

            if (adtrRow["ESCALATION_MESSAGE_TYPE"] != DBNull.Value && !string.IsNullOrEmpty(adtrRow["ESCALATION_MESSAGE_TYPE"].ToString()))
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBus).istrEscalationMessageType = adtrRow["ESCALATION_MESSAGE_TYPE"].ToString();
            }
            else
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBus).istrEscalationMessageType = "";
            }
        }
    }
}
