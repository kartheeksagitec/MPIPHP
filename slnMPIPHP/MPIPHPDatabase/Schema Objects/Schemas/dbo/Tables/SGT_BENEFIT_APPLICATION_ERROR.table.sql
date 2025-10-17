CREATE TABLE [dbo].[SGT_BENEFIT_APPLICATION_ERROR] (
    [BENEFIT_APPLICATION_ERROR_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [BENEFIT_APPLICATION_ID]       [dbo].[UDT_ID]         NOT NULL,
    [MESSAGE_ID]                   [dbo].[UDT_ID]         NOT NULL,
    [PARAMETER_VALUES]             [dbo].[UDT_LONGDESC]   NULL,
    [CREATED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [CREATED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                  [dbo].[UDT_CREATEDBY]  NOT NULL,
    [MODIFIED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                   [dbo].[UDT_UPDSEQ]     NULL
);

