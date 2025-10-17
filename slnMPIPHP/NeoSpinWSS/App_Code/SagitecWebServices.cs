using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using System.Configuration;
using Sagitec.Interface;
using Sagitec.DataObjects;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using System.Reflection;
using System.Linq;
using Sagitec.Common;
using Sagitec.WebClient.WebServices;
using Sagitec.WebClient;

/// <summary>
/// Summary description for SagitecWebServices
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class SagitecWebServices : wfmWebService
{
    public SagitecWebServices()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetPlanExists(string astrPlanId, string astrBenType)
    {
        try
        {
            if (astrPlanId != 9.ToString())
            {
                string lstrResult = "100";
                //FM upgrade: 6.0.2.1 build error changes - istrFWN to Framework.istrWindowName
                //object lobjMain = Session[istrFWN + "CenterMiddle"];
                object lobjMain = Session[Framework.istrWindowName + "CenterMiddle"];
                if (lobjMain is busPersonBeneficiary)
                {
                    busPersonBeneficiary lbusPersonBeneficiary = (busPersonBeneficiary)lobjMain;
                    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiary)
                    {
                        string lstrPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan.ToString();
                        string lstrBenType = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
                        if (lstrResult != "Y")
                        {
                            if (lstrPlanID == astrPlanId && astrBenType == lstrBenType)
                            {
                                lstrResult = "Y";
                                break;
                            }
                        }
                    }
                    if (lstrResult != "Y")
                    {
                        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiariesAll)
                        {
                            string lstrPlanID = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan.ToString();
                            string lstrBenType = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
                            if (lstrResult != "50")
                            {
                                if (lstrPlanID == astrPlanId && astrBenType == lstrBenType)
                                {
                                    lstrResult = "50";
                                    break;
                                }
                            }
                        }

                    }
                }

                return lstrResult;
            }
            else
            {
                return string.Empty;
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
