CREATE TABLE [dbo].[SGS_SYSTEM_PATHS] (
    [PATH_ID]          [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PATH_CODE]        [dbo].[UDT_CODE]       NULL,
    [PATH_VALUE]       [dbo].[UDT_NOTES]      NULL,
    [PATH_DESCRIPTION] [dbo].[UDT_NOTES]      NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]     NOT NULL
);

