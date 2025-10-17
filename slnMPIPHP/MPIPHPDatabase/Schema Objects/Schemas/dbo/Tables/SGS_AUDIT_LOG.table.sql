CREATE TABLE [dbo].[SGS_AUDIT_LOG] (
    [AUDIT_LOG_ID]      [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [FORM_NAME]         [dbo].[UDT_DBOBJ]      NOT NULL,
    [TABLE_NAME]        [dbo].[UDT_DBOBJ]      NOT NULL,
    [PRIMARY_KEY]       [dbo].[UDT_INT]        NULL,
    [PERSON_ID]         [dbo].[UDT_ID]         NULL,
    [ORG_ID]            [dbo].[UDT_ID]         NULL,
    [ORG_PLAN_ID]       [dbo].[UDT_ID]         NULL,
    [CHANGE_TYPE]       [dbo].[UDT_FLAG]       NOT NULL,
    [MODIFIED_BY]       [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [CLIENT_IP_ADDRESS] [dbo].[UDT_DATA50]     NULL
);

