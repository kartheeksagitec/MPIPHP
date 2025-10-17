CREATE TABLE [dbo].[SGS_USER_SECURITY_HISTORY] (
    [SGS_USER_SECURITY_HISTORY_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [USER_SERIAL_ID]               [dbo].[UDT_ID]         NULL,
    [RESOURCE_ID]                  [dbo].[UDT_ID]         NULL,
    [OLD_SECURITY_LEVEL]           [dbo].[UDT_DESC]       NULL,
    [CUSTOM_SECURITY_LEVEL_ID]     [dbo].[UDT_CODE_ID]    NOT NULL,
    [CUSTOM_SECURITY_LEVEL_VALUE]  [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]                   [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                  [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                   [dbo].[UDT_UPDSEQ]     NOT NULL
);

