CREATE TABLE [dbo].[SGT_PERSON_BENEFICIARY] (
    [BENEFICIARY_ID]                [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PERSON_ID]                     [dbo].[UDT_ID]         NULL,
    [BENEFICIARY_PERSON_ID]         [dbo].[UDT_ID]         NULL,
    [BENEFICIARY_ORG_ID]            [dbo].[UDT_ID]         NULL,
    [RELATIONSHIP_ID]               [dbo].[UDT_ID]         NULL,
    [RELATIONSHIP_VALUE]            [dbo].[UDT_CODE_VALUE] NULL,
    [ADDR_SAME_AS_PARTICIPANT_FLAG] [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL
);

