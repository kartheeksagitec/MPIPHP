CREATE TABLE [dbo].[SGT_PERSON_ACCOUNT_BENEFICIARY_ERROR] (
    [PERSON_ACCOUNT_BENEFICIARY_ERROR_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [PERSON_ACCOUNT_BENEFICIARY_ID]       [dbo].[UDT_ID]         NOT NULL,
    [MESSAGE_ID]                          [dbo].[UDT_ID]         NOT NULL,
    [PARAMETER_VALUES]                    [dbo].[UDT_LONGDESC]   NULL,
    [CREATED_BY]                          [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [CREATED_DATE]                        [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                         [dbo].[UDT_CREATEDBY]  NOT NULL,
    [MODIFIED_DATE]                       [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                          [dbo].[UDT_UPDSEQ]     NULL
);

