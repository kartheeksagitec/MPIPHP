#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Data.SqlClient;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPirAttachment:
	/// Inherited from busPirAttachmentGen, the class is used to customize the business object busPirAttachmentGen.
	/// </summary>
	[Serializable]
	public class busPirAttachment : busPirAttachmentGen
	{
        public void InsertAttachment(int aintPIRID, byte[] aarrAttachmentData, string astrMimeType, string astrFileName, string astrUserID)
        {

            string lstrs = utlPassInfo.iobjPassInfo.iconFramework.ConnectionString;
            using (SqlConnection lobjConn = new SqlConnection(lstrs))
            {

                using (SqlDataAdapter lobjDataAdapter = new SqlDataAdapter("select * from SGS_PIR_ATTACHMENT", lobjConn))
                {
                    DataSet ldstObjDataSet = new DataSet();
                    using (SqlCommandBuilder lobjCmdBuilder = new SqlCommandBuilder(lobjDataAdapter))
                    {
                        lobjDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        lobjConn.Open();
                        lobjDataAdapter.Fill(ldstObjDataSet, "Tabl");
                        DataRow ldtrDataRow = ldstObjDataSet.Tables["Tabl"].NewRow();
                        ldtrDataRow["PIR_ID"] = aintPIRID;
                        ldtrDataRow["ATTACHMENT_CONTENT"] = aarrAttachmentData;
                        ldtrDataRow["ATTACHMENT_GUID"] = Guid.NewGuid().ToString();
                        ldtrDataRow["ATTACHMENT_FILE_NAME"] = astrFileName;
                        ldtrDataRow["ATTACHMENT_MIME_TYPE"] = astrMimeType;
                        ldtrDataRow["CREATED_BY"] = astrUserID;
                        ldtrDataRow["CREATED_DATE"] = DateTime.Now;
                        ldtrDataRow["MODIFIED_BY"] = astrUserID;
                        ldtrDataRow["MODIFIED_DATE"] = DateTime.Now;

                        ldstObjDataSet.Tables["Tabl"].Rows.Add(ldtrDataRow);
                        lobjDataAdapter.Update(ldstObjDataSet.Tables["Tabl"]);
                        lobjConn.Close();
                    }

                }
            }
        }
	}
}
