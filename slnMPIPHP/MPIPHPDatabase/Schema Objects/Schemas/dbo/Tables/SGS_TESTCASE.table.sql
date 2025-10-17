CREATE TABLE [dbo].[SGS_TESTCASE] (
    [TESTCASE_ID]   [dbo].[UDT_IDENTITY]      IDENTITY (1, 1) NOT NULL,
    [TESTCASE_KEY]  [dbo].[UDT_DATA]          NULL,
    [TESTCASE_DESC] [dbo].[UDT_DESC]          NULL,
    [USECASE_ID]    [dbo].[UDT_ID]            NULL,
    [NOTES]         [dbo].[UDT_NOTES]         NULL,
    [FILE_LOCATION] [dbo].[UDT_FILE_LOCATION] NULL,
    [STATUS_ID]     [dbo].[UDT_CODE_ID]       NOT NULL,
    [STATUS_VALUE]  [dbo].[UDT_CODE_VALUE]    NULL,
    [CREATED_BY]    [dbo].[UDT_CREATEDBY]     NOT NULL,
    [CREATED_DATE]  [dbo].[UDT_DATETIME]      NOT NULL,
    [MODIFIED_BY]   [dbo].[UDT_MODIFIEDBY]    NOT NULL,
    [MODIFIED_DATE] [dbo].[UDT_DATETIME]      NOT NULL,
    [UPDATE_SEQ]    [dbo].[UDT_UPDSEQ]        NOT NULL
);

