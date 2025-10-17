using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPIPHP.Common.Exceptions;

namespace MPIPHP.BusinessObjects.Communication.Exceptions
{
    public abstract class clsNonDeliverableAddressFoundException : CommunicationException
    {
        public clsNonDeliverableAddressFoundException(string astrException)
            : base(astrException)
        {
        }
    }

    public class clsPersonNonDeliverableAddressFoundException : clsNonDeliverableAddressFoundException
    {
        public int iintPersonID { get; set; }
        public int iintPersonAddressID { get; set; }

        public clsPersonNonDeliverableAddressFoundException(int aintPersonID, int aintAddressID)
            : base(string.Format(ERROR_MESSAGE_UNDELIVERABLE_ADDRESS, aintAddressID.ToString(), PERSON, aintPersonID.ToString()))
        {
            iintPersonID = aintPersonID;
            iintPersonAddressID = aintAddressID;
        }
    }


    public class clsOrgContactNonDeliverableAddressFoundException : clsNonDeliverableAddressFoundException
    {
        public int iintOrgContactID { get; set; }
        public int iintOrgContactAddressID { get; set; }

        public clsOrgContactNonDeliverableAddressFoundException(int aintOrgContactID, int aintAddressID)
            : base(string.Format(ERROR_MESSAGE_UNDELIVERABLE_ADDRESS, aintAddressID.ToString(), ORG_CONTACT, aintOrgContactID.ToString()))
        {
            iintOrgContactID = aintOrgContactID;
            iintOrgContactAddressID = aintAddressID;
        }
    }
}
