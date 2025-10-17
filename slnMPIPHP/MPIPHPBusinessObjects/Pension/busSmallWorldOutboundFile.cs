using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Collections;
using System.Text.RegularExpressions;

namespace MPIPHP.BusinessObjects.Pension
{
    [Serializable]
    public class busSmallWorldOutboundFile : busFileBaseOut
    {
        busBase lobjBase = new busBase();
        public busSystemManagement iobjSystemManagement { get; set; }
        public Collection<busPerson> iclbActiveParticipant { get; set; }

        public void LoadSmallWorldOutboundFile(DataTable ldtSmallWorldOutboundFileData)
        {
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }

            ldtSmallWorldOutboundFileData = busBase.Select("cdoTempdata.GetSmallWorldOutboundFileData", new object[0] { });

            if (ldtSmallWorldOutboundFileData.Rows.Count > 0)
            {
                iclbActiveParticipant = lobjBase.GetCollection<busPerson>(ldtSmallWorldOutboundFileData, "icdoPerson");

                //PIR 1078
                foreach (busPerson lbusPerson in iclbActiveParticipant)
                {
                    lbusPerson.ibusPersonAddress = new busPersonAddress();

                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    DataTable ldtbActiveAddress = busBase.Select("cdoPersonContact.GetActiveAddress", new object[1] { lbusPerson.icdoPerson.person_id });
                    if (ldtbActiveAddress.Rows.Count > 0)
                    {
                        if (ldtbActiveAddress.Rows.Count == 1)
                        {
                            lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.LoadData(ldtbActiveAddress.Rows[0]);
                        }
                        else
                        {
                            foreach (DataRow dtRow in ldtbActiveAddress.Rows)
                            {
                                if (dtRow[enmPersonAddressChklist.address_type_value.ToString()].ToString() == busConstant.MAILING_ADDRESS)
                                {
                                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.LoadData(dtRow);
                                    break;
                                }

                            }
                        }
                    }


                    lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 =
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 + " " + lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2;

                    Regex lrgx = null;
                    if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1, " ");
                    }

                    if (Convert.ToInt32(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value) != busConstant.USA)
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = string.Empty;

                    if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value, " ");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city, " ");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code = lrgx.Replace(lbusPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.home_phone_no))
                    {
                        lrgx = new Regex("-");
                        lbusPerson.icdoPerson.home_phone_no = lrgx.Replace(lbusPerson.icdoPerson.home_phone_no, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.cell_phone_no))
                    {
                        lrgx = new Regex("-");
                        lbusPerson.icdoPerson.home_phone_no = lrgx.Replace(lbusPerson.icdoPerson.cell_phone_no, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.first_name))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.icdoPerson.first_name = lrgx.Replace(lbusPerson.icdoPerson.first_name, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.middle_name))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.icdoPerson.middle_name = lrgx.Replace(lbusPerson.icdoPerson.middle_name, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.last_name))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.icdoPerson.last_name = lrgx.Replace(lbusPerson.icdoPerson.last_name, "");
                    }

                    if (!string.IsNullOrEmpty(lbusPerson.icdoPerson.email_address_1))
                    {
                        lrgx = new Regex(",");
                        lbusPerson.icdoPerson.email_address_1 = lrgx.Replace(lbusPerson.icdoPerson.email_address_1, "");
                    }
                }
            }

        }

        public override void InitializeFile()
        {
            istrFileName = "SmallWorldOutboundPayeeFile" + "_" + DateTime.Now.ToString("MMMM") + DateTime.Now.Year + busConstant.FileFormattxt;
        }
    }
}
