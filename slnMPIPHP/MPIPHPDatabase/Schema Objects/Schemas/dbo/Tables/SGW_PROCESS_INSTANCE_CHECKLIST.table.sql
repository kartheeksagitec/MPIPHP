CREATE TABLE [dbo].[SGW_PROCESS_INSTANCE_CHECKLIST] (
    [PROCESS_INSTANCE_CHECKLIST_ID] [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [PROCESS_INSTANCE_ID]           [dbo].[UDT_ID]         NOT NULL,
    [DOCUMENT_ID]                   [dbo].[UDT_ID]         NOT NULL,
    [REQUIRED_FLAG]                 [dbo].[UDT_FLAG]       NULL,
    [APPROVED_FLAG]                 [dbo].[UDT_FLAG]       NULL,
    [RECEIVED_DATE]                 [dbo].[UDT_DATETIME]   NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL
);

