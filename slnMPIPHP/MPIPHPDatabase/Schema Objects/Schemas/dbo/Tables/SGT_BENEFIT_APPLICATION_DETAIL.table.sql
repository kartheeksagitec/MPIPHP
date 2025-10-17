CREATE TABLE [dbo].[SGT_BENEFIT_APPLICATION_DETAIL] (
    [BENEFIT_APPLICATION_DETAIL_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [BENEFIT_APPLICATION_ID]        [dbo].[UDT_ID]         NOT NULL,
    [PLAN_BENEFIT_ID]               [dbo].[UDT_ID]         NOT NULL,
    [JOINT_ANNUITANT_ID]            [dbo].[UDT_ID]         NOT NULL,
    [SPOUSAL_CONSENT_FLAG]          [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL
);

