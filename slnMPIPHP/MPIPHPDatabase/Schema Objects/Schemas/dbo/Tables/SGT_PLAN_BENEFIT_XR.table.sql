CREATE TABLE [dbo].[SGT_PLAN_BENEFIT_XR] (
    [PLAN_BENEFIT_ID]      [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PLAN_ID]              [dbo].[UDT_ID]         NOT NULL,
    [BENEFIT_OPTION_ID]    [dbo].[UDT_ID]         NOT NULL,
    [BENEFIT_OPTION_VALUE] [dbo].[UDT_CODE_VALUE] NULL,
    [RETIREMENT_FLAG]      [dbo].[UDT_FLAG]       NULL,
    [WITHDRAWAL_FLAG]      [dbo].[UDT_FLAG]       NULL,
    [DISABILITY_FLAG]      [dbo].[UDT_FLAG]       NULL,
    [DEATH_FLAG]           [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]           [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]         [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]          [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]           [dbo].[UDT_UPDSEQ]     NOT NULL
);

