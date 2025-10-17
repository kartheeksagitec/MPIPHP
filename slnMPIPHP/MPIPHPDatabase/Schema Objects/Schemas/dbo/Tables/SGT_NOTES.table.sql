CREATE TABLE [dbo].[SGT_NOTES] (
    [NOTE_ID]             [dbo].[UDT_IDENTITY]     IDENTITY (1, 1) NOT NULL,
    [PERSON_ID]           [dbo].[UDT_ID]           NULL,
    [ORG_ID]              [dbo].[UDT_ID]           NULL,
    [FORM_ID]             [dbo].[UDT_ID]           NULL,
    [FORM_VALUE]          [dbo].[UDT_CODE_VALUE]   NULL,
    [NOTES]               [dbo].[UDT_INSTRUCTIONS] NULL,
    [RESTRICTED_FLAG]     [dbo].[UDT_FLAG]         NULL,
    [CREATED_BY]          [dbo].[UDT_MODIFIEDBY]   NOT NULL,
    [CREATED_DATE]        [dbo].[UDT_DATETIME]     NOT NULL,
    [MODIFIED_BY]         [dbo].[UDT_CREATEDBY]    NOT NULL,
    [MODIFIED_DATE]       [dbo].[UDT_DATETIME]     NOT NULL,
    [UPDATE_SEQ]          [dbo].[UDT_UPDSEQ]       NULL,
    [PROCESS_INSTANCE_ID] [dbo].[UDT_ID]           NULL
);

