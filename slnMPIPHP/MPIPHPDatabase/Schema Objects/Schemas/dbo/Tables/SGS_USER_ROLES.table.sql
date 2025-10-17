CREATE TABLE [dbo].[SGS_USER_ROLES] (
    [USER_SERIAL_ID]       [dbo].[UDT_IDENTITY]   NOT NULL,
    [ROLE_ID]              [dbo].[UDT_ID]         NOT NULL,
    [EFFECTIVE_START_DATE] [dbo].[UDT_DATE]       NULL,
    [EFFECTIVE_END_DATE]   [dbo].[UDT_DATE]       NULL,
    [CREATED_BY]           [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]         [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]          [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]           [dbo].[UDT_UPDSEQ]     NOT NULL
);

