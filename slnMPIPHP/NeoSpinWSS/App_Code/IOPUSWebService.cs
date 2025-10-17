using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


[ServiceContract]
public interface IOPUSWebService
{
    [OperationContract]
    bool IsSSNValid(string istrPrefix, string istrFirstName, string istrLastName, string istrMiddleName, string istrSuffix, string istrDateofBirth,string istrSSN);
   
    [OperationContract]
    bool AddUpdatePersonAddress(string astrSSN, string astrAddressLine1, string astrAddressLine2, string astrCity, string astrState, string astrZipCode, string astrZipCode4, string astrCountryCode,
        string astrAddressType, string astrAddressEndDate);

    [OperationContract]
    string GetPersonInformation(string astrSSN);

    [OperationContract]
    string ReteiveMPIDFromOPUS(string astrSSN);

}
