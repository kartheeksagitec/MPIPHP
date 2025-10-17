/// <summary>
/// Summary description for UIConstants
/// </summary>
public class UIConstants
{
    public const int ZERO = 0;
    public const string FILE_UPLOAD_LIMIT = "FileUploadLimit";
    
    //UPLOAD
    public const string UploadFileMaintenance = "wfmUploadFileMaintenance";
    public const string UploadHeadlineFile = "wfmUploadHeadlineMaintenance";
    public const string UploadECMDocumentMaintenance = "wfmECMUploadDocumentMaintenance";
    // F/W Upgrade : code conversion of btnUploadFile_Click method.
    public const string PirMaintenance = "wfmPirMaintenance";

    //Parameters related to show on the footer of master page 
    public const string REGION = "Region";
    public const string USER_NAME = "UserName";
    public const string BATCH_DATE = "BATCH_DATE";
    public const string USE_APPLICATION_DATE = "USE_APPLICATION_DATE";
    public const string SYSTEM_FLAG = "SYSTEM_FLAG";
    public const string RELEASE_DATE = "ReleaseDate";
    public const string REGION_VALUE = "region_value";
    public const string PRODUCT_REGION = "ProductRegion";
    public const string APPLICATION_NAME = "ApplicationName";
    public const string DICT_MASTER_PARAMS = "dictMasterParams";
    public const string REQUEST_IP_ADDRESS = "RequestIPAddress";
    public const string REQUEST_APP_SERVER = "RequestAppServer";
    public const string REQUEST_MACHINE_NAME = "RequestMachineName";

    public const int PERSON_PRIVACY_SETTING_RESOURCE_ID = 30100007;
    public const string IS_USER_HAVE_PRIVACY_SETTING_ACCESS = "IsUserHavePrivacySettingAccess";
}

public sealed class JobServiceCodes
{
    public const string PROCESS_EMPLOYER_REPORTING_FILE = "JS_5001";
    public const string PROCESS_PERSON_ENROLLMENT_FILE = "JS_13";
    public const string PROCESS_FUND_BALANCE_INBOUND_FILE = "JS_15003";
}