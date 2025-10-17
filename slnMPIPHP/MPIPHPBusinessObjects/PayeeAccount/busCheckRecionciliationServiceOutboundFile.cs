using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Data;
using MPIPHP.CustomDataObjects;
using Sagitec.CustomDataObjects;

namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busCheckRecionciliationServiceOutboundFile : busFileBaseOut
    {
        #region Properties
        busBase lobjBase = new busBase();
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDistributionOutboundFileData { get; set; }
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDistributionOutboundFileCountData { get; set; }
        public busSystemManagement iobjSystemManagement { get; set; }
        public string istrRecordLine; //NeoTrack PIR 231
        #endregion

        public void LoadCheckRecionciliationServiceOutboundFile(DataTable adtbOutboundFileData)
        {
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }
            adtbOutboundFileData = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckReconciliationOutBoundFileData", new object[1] { Convert.ToInt32(iarrParameters[0]) });
            if (adtbOutboundFileData.Rows.Count > 0)
            {
               iclbPaymentHistoryDistributionOutboundFileData = lobjBase.GetCollection<busPaymentHistoryDistribution>(adtbOutboundFileData, "icdoPaymentHistoryDistribution");
            }

            var query = from row in adtbOutboundFileData.AsEnumerable()
                        group row by new { ID = row.Field<DateTime>("CHECK_ACH_EFFECTIVE_DATE")}//, ID1 = row.Field<Int32>("PLAN_ID") }
                        into grp
                        select new
                        {
                            idtIssueDate = Convert.ToString(grp.Key.ID),
                            //istrAccountNumber = Convert.ToString(grp.Key.ID1),
                            istrAccountNumber = grp.Select(row => row["Account_Number"]).FirstOrDefault(),
                            intControlAmount = grp.Sum(row => row["NET_AMOUNT"] == DBNull.Value ? 0.0M : (decimal)row["NET_AMOUNT"]),
                            intControlCount = grp.Count()
                        };

            if (query.Count() > 0)
            {
                DataTable adtbOutboundFileDataCount = new DataTable();
                adtbOutboundFileDataCount.Columns.Add("istrAccountNumber");
                adtbOutboundFileDataCount.Columns.Add("idtIssueDate");
                adtbOutboundFileDataCount.Columns.Add("intControlAmount");
                adtbOutboundFileDataCount.Columns.Add("intControlCount");

                foreach (var row in query)
                {
                    DataRow dr = adtbOutboundFileDataCount.NewRow();
                    dr["istrAccountNumber"] =Convert.ToString(row.istrAccountNumber);
                    dr["idtIssueDate"] = Convert.ToString(String.Format("{0:MMddyyyy}", Convert.ToDateTime(row.idtIssueDate)));
                    dr["intControlAmount"] = Convert.ToString(row.intControlAmount.ToString().Replace(".","")).PadLeft(12, '0');
                    dr["intControlCount"] = Convert.ToString(row.intControlCount).PadLeft(10, '0');
                    adtbOutboundFileDataCount.Rows.Add(dr);

                }
                if (adtbOutboundFileDataCount.Rows.Count > 0)
                {
                    iclbPaymentHistoryDistributionOutboundFileCountData = lobjBase.GetCollection<busPaymentHistoryDistribution>(adtbOutboundFileDataCount, "icdoPaymentHistoryDistribution");
                }
            }
        }

        //PROD PIR 53
        public override void InitializeFile()
        {
            base.InitializeFile();

            if (Convert.ToString(iarrParameters[1]) == "Pension")
                istrFileName = istrFileName + "xf00.d900061i.d100.Check_Pension" + busConstant.FileFormattxt;
            else
                istrFileName = istrFileName + "xf00.d900061i.d100.Check_IAP" + busConstant.FileFormattxt;

        }

        #region NeoTrack PIR 231 - Need to insert a new line in between a record to split data over a next line.
        public override void BeforeWriteRecord()
        {
            istrRecordLine = string.Empty;
            istrRecordLine = istrRecord;
            //istrRecord = istrRecord.Substring(0, 40);
            ////Fw upgrade: PIR ID : 29285: PostProd: LOB- Space and line break issue in 'CheckRecionciliationServiceOutboundFilePension’ file
            //if (this.ibusFile.icdoFile.xml_layout_file == "fleCheckRecionciliationServiceOutboundFile")
            //    istrRecord += Environment.NewLine;
            //istrRecordLine = istrRecordLine.Substring(40);
        }

        public override void AfterWriteRecord()
        {
            ////Fw upgrade: PIR ID : 29285: PostProd: LOB - Space and line break issue in 'CheckRecionciliationServiceOutboundFilePension’ file
            //if (this.ibusFile.icdoFile.xml_layout_file == "fleCheckRecionciliationServiceOutboundFile")
            //    iswrOut.Write(istrRecordLine);
            //else
            //    iswrOut.WriteLine(istrRecordLine);
            if (istrRecordLine.IsNotNullOrEmpty())
            {
                cdoFileDtl lobjFileDtl = new cdoFileDtl();
                lobjFileDtl.file_hdr_id = iobjFileHdr.file_hdr_id;
                lobjFileDtl.status_value = "PROC";
                lobjFileDtl.transaction_code_value = iobjCurrentLayout.istrTransactionCode;
                lobjFileDtl.record_data = istrRecordLine;
                lobjFileDtl.line_no = iintRecordCount;
                lobjFileDtl.Insert();
            }
            base.AfterWriteRecord();

        }
        #endregion
    }
}