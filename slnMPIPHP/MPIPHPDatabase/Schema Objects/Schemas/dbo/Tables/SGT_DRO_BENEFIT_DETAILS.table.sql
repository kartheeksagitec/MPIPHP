CREATE TABLE [dbo].[SGT_DRO_BENEFIT_DETAILS] (
    [DRO_BENEFIT_ID]      [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [DRO_APPLICATION_ID]  [dbo].[UDT_ID]         NOT NULL,
    [DRO_MODEL_ID]        [dbo].[UDT_ID]         NOT NULL,
    [DRO_MODEL_VALUE]     [dbo].[UDT_CODE_VALUE] NULL,
    [PLAN_ID]             [dbo].[UDT_ID]         NULL,
    [BENEFIT_PERC]        [dbo].[UDT_PERC]       NULL,
    [BENEFIT_AMT]         [dbo].[UDT_AMT]        NULL,
    [ALT_PAYEE_INCREASE]  [dbo].[UDT_FLAG]       NULL,
    [ALT_PAYEE_EARLY_RET] [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]          [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]         [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]       [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]          [dbo].[UDT_UPDSEQ]     NOT NULL
);

