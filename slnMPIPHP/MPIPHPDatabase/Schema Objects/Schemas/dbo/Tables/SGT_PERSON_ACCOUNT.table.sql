CREATE TABLE [dbo].[SGT_PERSON_ACCOUNT] (
    [PERSON_ACCOUNT_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PERSON_ID]         [dbo].[UDT_ID]         NULL,
    [PLAN_ID]           [dbo].[UDT_ID]         NULL,
    [START_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [END_DATE]          [dbo].[UDT_DATETIME]   NULL,
    [STATUS_ID]         [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]      [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]        [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]      [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]       [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]        [dbo].[UDT_UPDSEQ]     NOT NULL
);

