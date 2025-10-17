CREATE TABLE [dbo].[SGT_PERSON_ADDRESS_CHKLIST_HISTORY] (
    [ADDRESS_CHKLIST_HISTORY_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PERSON_ADDRESS_HISTORY_ID]  [dbo].[UDT_ID]         NULL,
    [ADDRESS_TYPE_ID]            [dbo].[UDT_ID]         NOT NULL,
    [ADDRESS_TYPE_VALUE]         [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]                 [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]               [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]              [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                 [dbo].[UDT_UPDSEQ]     NOT NULL
);

