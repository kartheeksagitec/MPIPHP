CREATE TABLE [dbo].[SGT_BENEFIT_APPLICATION_STATUS_HISTORY] (
    [BENEFIT_APPLICATION_STATUS_HISTORY_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [BENEFIT_APPLICATION_ID]                [dbo].[UDT_ID]         NOT NULL,
    [STATUS_ID]                             [dbo].[UDT_ID]         NOT NULL,
    [STATUS_VALUE]                          [dbo].[UDT_CODE_VALUE] NOT NULL,
    [STATUS_DATE]                           [dbo].[UDT_DATETIME]   NOT NULL,
    [CREATED_BY]                            [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                          [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                           [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                         [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                            [dbo].[UDT_UPDSEQ]     NOT NULL
);

