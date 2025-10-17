CREATE TABLE [dbo].[SGS_PIR_HISTORY] (
    [PIR_HISTORY_ID]   [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PIR_ID]           [dbo].[UDT_ID]         NOT NULL,
    [LONG_DESCRIPTION] [dbo].[UDT_COMMENTS]   NULL,
    [STATUS_ID]        [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]     [dbo].[UDT_CODE_VALUE] NULL,
    [ASSIGNED_TO_ID]   [dbo].[UDT_ID]         NOT NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]     NOT NULL
);

