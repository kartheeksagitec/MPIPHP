CREATE TABLE [dbo].[SGT_DOCUMENT] (
    [DOCUMENT_ID]         INT                    IDENTITY (1, 1) NOT NULL,
    [DOC_TYPE]       [dbo].[UDT_INT]        NOT NULL,
    [DOCUMENT_NAME]       [dbo].[UDT_LONGDESC]   NULL,
    [CREATED_BY]          [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]         [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]       [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]          [dbo].[UDT_UPDSEQ]     NOT NULL,
    [IGNORE_PROCESS_FLAG] [dbo].[UDT_FLAG]       NULL
);

