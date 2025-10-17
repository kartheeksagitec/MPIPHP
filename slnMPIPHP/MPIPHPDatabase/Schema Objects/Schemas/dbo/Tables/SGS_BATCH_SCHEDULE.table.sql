CREATE TABLE [dbo].[SGS_BATCH_SCHEDULE] (
    [BATCH_SCHEDULE_ID]         [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [STEP_NO]                   [dbo].[UDT_INT]        NULL,
    [STEP_NAME]                 [dbo].[UDT_LONGDESC]   NULL,
    [STEP_DESCRIPTION]          [dbo].[UDT_LONGDESC]   NULL,
    [FREQUENCY_IN_DAYS]         [dbo].[UDT_INT]        NULL,
    [FREQUENCY_IN_MONTHS]       [dbo].[UDT_INT]        NULL,
    [NEXT_RUN_DATE]             [dbo].[UDT_DATE]       NULL,
    [STEP_PARAMETERS]           [dbo].[UDT_LONGDESC]   NULL,
    [ACTIVE_FLAG]               [dbo].[UDT_FLAG]       NOT NULL,
    [REQUIRES_TRANSACTION_FLAG] [dbo].[UDT_FLAG]       NOT NULL,
    [EMAIL_NOTIFICATION]        [dbo].[UDT_NOTES]      NULL,
    [ORDER_NO]                  [dbo].[UDT_CODE]       NULL,
    [CUTOFF_START]              [dbo].[UDT_CODE_VALUE] NULL,
    [CUTOFF_END]                [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]                [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]              [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]               [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]             [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                [dbo].[UDT_UPDSEQ]     NOT NULL
);

