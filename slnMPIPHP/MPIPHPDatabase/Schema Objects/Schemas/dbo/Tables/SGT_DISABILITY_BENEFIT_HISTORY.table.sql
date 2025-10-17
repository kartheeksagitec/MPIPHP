CREATE TABLE [dbo].[SGT_DISABILITY_BENEFIT_HISTORY] (
    [DISABILITY_BENEFIT_HISTORY_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [BENEFIT_APPLICATION_ID]        [dbo].[UDT_ID]         NOT NULL,
    [DISABILITY_CONT_LETTER_DATE]   [dbo].[UDT_DATETIME]   NOT NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL
);

