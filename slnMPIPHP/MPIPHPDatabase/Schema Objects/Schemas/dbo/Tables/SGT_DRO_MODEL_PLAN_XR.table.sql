CREATE TABLE [dbo].[SGT_DRO_MODEL_PLAN_XR] (
    [DRO_MODEL_PLAN_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [DRO_MODEL_ID]      [dbo].[UDT_ID]         NOT NULL,
    [DRO_MODEL_VALUE]   [dbo].[UDT_CODE_VALUE] NULL,
    [PLAN_ID]           [dbo].[UDT_ID]         NULL,
    [CREATED_BY]        [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]      [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]       [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]        [dbo].[UDT_UPDSEQ]     NOT NULL
);

