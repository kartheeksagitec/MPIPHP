CREATE TABLE [dbo].[SGW_WORKFLOW_REQUEST] (
    [WORKFLOW_REQUEST_ID]   [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [DOC_TYPE]         [dbo].[UDT_NAME]       NULL,
    [PROCESS_ID]            [dbo].[UDT_INT]        NULL,
    [REFERENCE_ID]          [dbo].[UDT_INT]        NULL,
    [FILENET_DOCUMENT_TYPE] [dbo].[UDT_NAME]       NULL,
    [IMAGE_DOC_CATEGORY]    [dbo].[UDT_NAME]       NULL,
    [PERSON_ID]             [dbo].[UDT_ID]         NULL,
    [ORG_CODE]              [dbo].[UDT_CODE]       NULL,
    [PROCESS_INSTANCE_ID]   [dbo].[UDT_INT]        NULL,
    [STATUS_ID]             [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]          [dbo].[UDT_CODE_VALUE] NULL,
    [SOURCE_ID]             [dbo].[UDT_ID]         NULL,
    [SOURCE_VALUE]          [dbo].[UDT_CODE_VALUE] NULL,
    [INITIATED_DATE]        [dbo].[UDT_DATETIME]   NULL,
    [CONTACT_TICKET_ID]     [dbo].[UDT_INT]        NULL,
    [CREATED_BY]            [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]          [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]           [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]         [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]            [dbo].[UDT_UPDSEQ]     NOT NULL,
    [ADDITIONAL_PARAMETER1] VARCHAR (255)          NULL
);

