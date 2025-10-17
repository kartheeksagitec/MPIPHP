CREATE TABLE [dbo].[SGS_SYSTEM_MANAGEMENT] (
    [SYSTEM_MANAGEMENT_ID]      [dbo].[UDT_IDENTITY]      NOT NULL,
    [CURRENT_CYCLE_NO]          [dbo].[UDT_ID]            NULL,
    [REGION_ID]                 [dbo].[UDT_CODE_ID]       NULL,
    [REGION_VALUE]              [dbo].[UDT_CODE_VALUE]    NULL,
    [SYSTEM_AVAILABILITY_ID]    [dbo].[UDT_CODE_ID]       NULL,
    [SYSTEM_AVAILABILITY_VALUE] [dbo].[UDT_CODE_VALUE]    NULL,
    [BATCH_DATE]                [dbo].[UDT_DATE]          NULL,
    [BASE_DIRECTORY]            [dbo].[UDT_FILE_LOCATION] NULL,
    [CREATED_BY]                [dbo].[UDT_CREATEDBY]     NOT NULL,
    [CREATED_DATE]              [dbo].[UDT_DATETIME]      NOT NULL,
    [MODIFIED_BY]               [dbo].[UDT_MODIFIEDBY]    NOT NULL,
    [MODIFIED_DATE]             [dbo].[UDT_DATETIME]      NOT NULL,
    [UPDATE_SEQ]                [dbo].[UDT_UPDSEQ]        NOT NULL,
    [EMAIL_NOTIFICATION]        [dbo].[UDT_NOTES]         NULL
);

