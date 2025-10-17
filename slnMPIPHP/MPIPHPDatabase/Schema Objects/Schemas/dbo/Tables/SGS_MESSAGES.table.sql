CREATE TABLE [dbo].[SGS_MESSAGES] (
    [MESSAGE_ID]            [dbo].[UDT_IDENTITY]     NOT NULL,
    [DISPLAY_MESSAGE]       [dbo].[UDT_NOTES]        NOT NULL,
    [SEVERITY_ID]           [dbo].[UDT_CODE_ID]      NOT NULL,
    [SEVERITY_VALUE]        [dbo].[UDT_CODE_VALUE]   NULL,
    [INTERNAL_INSTRUCTIONS] [dbo].[UDT_INSTRUCTIONS] NULL,
    [EMPLOYER_INSTRUCTIONS] [dbo].[UDT_INSTRUCTIONS] NULL,
    [RESPONSIBILITY_ID]     [dbo].[UDT_CODE_ID]      NOT NULL,
    [RESPONSIBILITY_VALUE]  [dbo].[UDT_CODE_VALUE]   NULL,
    [CREATED_BY]            [dbo].[UDT_CREATEDBY]    NOT NULL,
    [CREATED_DATE]          [dbo].[UDT_DATETIME]     NOT NULL,
    [MODIFIED_BY]           [dbo].[UDT_MODIFIEDBY]   NOT NULL,
    [MODIFIED_DATE]         [dbo].[UDT_DATETIME]     NOT NULL,
    [UPDATE_SEQ]            [dbo].[UDT_UPDSEQ]       NOT NULL
);

