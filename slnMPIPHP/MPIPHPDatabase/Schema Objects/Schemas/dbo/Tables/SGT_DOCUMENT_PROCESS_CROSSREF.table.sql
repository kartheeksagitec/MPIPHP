CREATE TABLE [dbo].[SGT_DOCUMENT_PROCESS_CROSSREF] (
    [DOCUMENT_PROCESS_CROSSREF_ID] INT                    IDENTITY (1, 1) NOT NULL,
    [DOCUMENT_ID]                  [dbo].[UDT_ID]         NOT NULL,
    [PROCESS_ID]                   [dbo].[UDT_ID]         NOT NULL,
    [DOCUMENT_TYPE_ACTION_ID]      [dbo].[UDT_ID]         NULL,
    [DOCUMENT_TYPE_ACTION_VALUE]   [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]                   [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                  [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                   [dbo].[UDT_UPDSEQ]     NOT NULL
);

