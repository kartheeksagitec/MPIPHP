CREATE TABLE [dbo].[SGW_PROCESS_INSTANCE] (
    [PROCESS_INSTANCE_ID]    [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [PROCESS_ID]             [dbo].[UDT_ID]         NOT NULL,
    [REQUEST_ID]             [dbo].[UDT_ID]         NULL,
    [WORKFLOW_INSTANCE_GUID] UNIQUEIDENTIFIER       NOT NULL,
    [PERSON_ID]              [dbo].[UDT_ID]         NULL,
    [ORG_ID]                 [dbo].[UDT_ID]         NULL,
    [PRIORITY]               [dbo].[UDT_INT]        NOT NULL,
    [STATUS_ID]              [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]           [dbo].[UDT_CODE_VALUE] NULL,
    [CONTACT_TICKET_ID]      [dbo].[UDT_INT]        NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]            [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]          [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]             [dbo].[UDT_UPDSEQ]     NOT NULL,
    [ADDITIONAL_PARAMETER1]  [dbo].[UDT_SSN]        NULL
);

