using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI;
using Sagitec.WebClient;
using Sagitec.Common;
using MPIPHP.BusinessObjects;
using System.Collections.Generic;

public partial class wfmInUploadFile : wfmMainDB
{
    protected override void OnInit(EventArgs e)
    {
        istrFormName = "wfmInUploadFile";
        ScriptManager lscmMain = (ScriptManager)Master.FindControl("scmMain");
        lscmMain.RegisterPostBackControl(cmdSend);


        base.OnInit(e);
        if (iintSecurityLevel == 0)
        {
            TblUpload.Visible = false;
        }

    }

    /// <summary>
    /// Page Load Event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindListControl(ddlFileType);
        }
    }

    /// <summary>
    /// Process the Send Button Click Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cmdSend_Click(object sender, System.EventArgs e)
    {
        lblInfo.Text = string.Empty;

        //int lintOrgID = 0;
        //if ((!String.IsNullOrEmpty(txtOrgId.Text)) && HelperFunction.IsNumeric(txtOrgId.Text))
        //{
        //    lintOrgID = Convert.ToInt32(txtOrgId.Text);
        //}
        // Check to see if file was uploaded
        if (filMyFile.PostedFile != null)
        {
            // Get a reference to PostedFile object
            HttpPostedFile myFile = filMyFile.PostedFile;

            // Get size of uploaded file
            int nFileLen = myFile.ContentLength;

            try
            {
                // make sure the size of the file is > 0
                if (nFileLen > 0)
                {
                    // Allocate a buffer for reading of the file
                    byte[] myData = new byte[nFileLen];

                    // Read uploaded file from the Stream
                    myFile.InputStream.Read(myData, 0, nFileLen);
                    // Create a name for the file to store
                    string strFilename = Path.GetFileName(myFile.FileName);

                    // method to check the file has the exact number of values in the File that is uploaded.
                    CheckFileMaxColumns(strFilename, myData);

                    // Write data into a file
                    Hashtable lhstParams = new Hashtable();
                    lhstParams.Add("astrUserId", istrUserID);
                    //lhstParams.Add("aintReferenceId", lintOrgID);
                    lhstParams.Add("aintFileType", Convert.ToInt32(ddlFileType.SelectedValue));
                    lhstParams.Add("astrFileName", strFilename);
                    lhstParams.Add("aBuffer", myData);
                    lhstParams.Add("astrMailFrom", "MPIPHP.UploadFile@sagitec.com");
                    lhstParams.Add("astrEmailId", "jaswinder.singh@sagitec.com");
                    lhstParams.Add("astrSubject", strFilename + " has been successfully received ");
                    lhstParams.Add("astrMessage", strFilename + " will soon be uploaded into MPIPHP system and all the transaction edits will be run. " +
                        "Once the upload and edits are completed you will be informed via email");
                    lhstParams.Add("ablnAlwaysCreateFileHeader", true);

                    ArrayList larrErrors = (ArrayList)isrvBusinessTier.ExecuteMethod("ValidateAndWriteToFile", lhstParams, false, idictParams);
                    if (larrErrors.Count > 0)
                    {
                        Hashtable lshtErrors = new Hashtable();

                        foreach (utlError lobjError in larrErrors)
                        {
                            if(!lshtErrors.ContainsKey(lobjError.istrErrorID))
                            {
                                lshtErrors.Add(lobjError.istrErrorID, lobjError.istrErrorMessage);
                            }                            
                        }

                        foreach (DictionaryEntry hashEntry in lshtErrors)
                        {
                            AddToValidationSummary(hashEntry.Key.ToString(), hashEntry.Value.ToString());
                        }
                        // Set label's text
                        lblInfo.Text = "Filename : " + strFilename + "&nbsp;&nbsp" + " Size : " + nFileLen.ToString() + ", unable to load due to errors";
                    }
                    else
                    {
                        // Set label's text
                        lblInfo.Text = "Filename : " + strFilename + "&nbsp;&nbsp" + " Size : " + nFileLen.ToString() + " successfully loaded";
                    }
                    lblInfo.Visible = true;
                }

                // if the file has a size of zero, ie no data then throw error
                if (nFileLen == 0)
                {
                    lblInfo.Text = " Uploading the File Failed as the File is empty";
                    throw new Exception(lblInfo.Text);
                }
            }
            catch (Exception ex)
            {
                DisplayError("Error Occurred: ", ex);
            }
        }
    }

    // Reads the name of current web page
    private string GetMyName()
    {
        // Get the script name
        string strScript = Request.ServerVariables["SCRIPT_NAME"];

        // Get position of last slash
        int nPos = strScript.LastIndexOf("/");

        // Get everything after slash
        if (nPos > -1)
            strScript = strScript.Substring(nPos + 1);

        return strScript;
    }


    /// <summary>
    /// if the file length is less than min columns OR more than max columns, 
    /// then an entry should not be allowed to be created in the SGS_File_Hdr table and throws an exception
    /// </summary>
    /// <param name="strFilename">File Name</param>
    /// <param name="myData">Data</param>
    public void CheckFileMaxColumns(string strFilename, byte[] myData)
    {
        string lstrFile = Encoding.ASCII.GetString(myData);
        string[] lstrRecords = lstrFile.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        int RowCount = 0;
        int lintMinColumns = 0;
        int lintMaxColumns = 0;
        string lstrDelimiter = string.Empty;

        if (strFilename.EndsWith(".xls", true, null))
        {
            // Rajesh : We cannot test the number of columns in an excel sheet out here
            // For Now we can skip the number of columns check we will see how we can include this in the future.
            return;
        }

        // Get Min and Max columns allowed for file.
        GetMinMaxColumnsAndDelimiterForFile(Convert.ToInt32(ddlFileType.SelectedValue), out lintMinColumns, out lintMaxColumns, out lstrDelimiter);

        if (lintMaxColumns == 0)
            return;

        foreach (string lstrRecord in lstrRecords)
        {
            RowCount++;

            // to get individual column value using the Helper Function
            List<String> larrFields = HelperFunction.SplitQuoted(lstrRecord, lstrDelimiter);

            // if the file length is less than min columns OR more than max columns, 
            // then an entry should not be allowed to be created in the SGS_File_Hdr table and throws an exception
            if ((larrFields.Count < lintMinColumns) ||
                (larrFields.Count > lintMaxColumns))
            {
                lblInfo.Text = "The Filename - " + strFilename + "&nbsp;&nbsp" + " at line number : " +
                               RowCount.ToString() +
                               " contains " + larrFields.Count.ToString()
                               + " Columns, but the actual file needs minimum of " + lintMinColumns +
                               " columns and maximum of " + lintMaxColumns + " Columns Only";
                throw new Exception(lblInfo.Text);
            }
        }
    }

    /// <summary>
    /// Get the Min and Max column count for specified file
    /// </summary>
    /// <param name="aintFileId">File Id</param>
    /// <param name="aintMinColumns">Min Columns</param>
    /// <param name="aintMaxColumns">Max Columns</param>
    /// <param name="astrDelimiter">Delimiter</param>
    private void GetMinMaxColumnsAndDelimiterForFile(int aintFileId, out int aintMinColumns, out int aintMaxColumns, out string astrDelimiter)
    {
        aintMinColumns = 0;
        aintMaxColumns = 0;
        astrDelimiter = string.Empty;

        switch (aintFileId)
        {
            //case busConstant.OrgEvaluation.ACTUARY_EMPLOYEE_FILE:
            //    astrDelimiter = busConstant.OrgEvaluation.ACTUARY_EMPLOYEE_FILE_DELIMTTER;
            //    aintMinColumns = busConstant.OrgEvaluation.ACTUARY_EMPLOYEE_FILE_ALLOWED_COLS;
            //    aintMaxColumns = busConstant.OrgEvaluation.ACTUARY_EMPLOYEE_FILE_ALLOWED_COLS;
            //    break;
            //default:
            //    astrDelimiter = "~";
            //    aintMinColumns = 0;
            //    aintMaxColumns = 0;
            //    break;            
        }
    }
}


