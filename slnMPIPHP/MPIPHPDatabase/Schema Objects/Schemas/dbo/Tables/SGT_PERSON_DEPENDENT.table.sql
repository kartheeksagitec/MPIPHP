CREATE TABLE [dbo].[SGT_PERSON_DEPENDENT] (
    [DEPENDENT_ID]                  [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PERSON_ID]                     [dbo].[UDT_ID]         NULL,
    [DEPENDENT_PERSON_ID]           [dbo].[UDT_ID]         NULL,
    [RELATIONSHIP_ID]               [dbo].[UDT_ID]         NULL,
    [RELATIONSHIP_VALUE]            [dbo].[UDT_CODE_VALUE] NULL,
    [EFFECTIVE_START_DATE]          [dbo].[UDT_DATETIME]   NULL,
    [EFFECTIVE_END_DATE]            [dbo].[UDT_DATETIME]   NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL,
    [ADDR_SAME_AS_PARTICIPANT_FLAG] [dbo].[UDT_FLAG]       NULL
);

