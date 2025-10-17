CREATE TABLE [dbo].[SGT_PLAN] (
    [PLAN_ID]            [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PLAN_CODE]          [dbo].[UDT_NAME]       NULL,
    [PLAN_NAME]          [dbo].[UDT_NAME]       NULL,
    [BENEFIT_TYPE_ID]    [dbo].[UDT_ID]         NULL,
    [BENEFIT_TYPE_VALUE] [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]         [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]       [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]        [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]      [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]         [dbo].[UDT_UPDSEQ]     NOT NULL
);

