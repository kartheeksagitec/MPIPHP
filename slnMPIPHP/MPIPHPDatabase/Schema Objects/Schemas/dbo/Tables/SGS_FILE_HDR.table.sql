CREATE TABLE [dbo].[SGS_FILE_HDR] (
    [FILE_HDR_ID]            [dbo].[UDT_IDENTITY]      IDENTITY (1, 1) NOT NULL,
    [MAILBOX_FILE_NAME]      [dbo].[UDT_FILE_LOCATION] NULL,
    [PROCESSED_FILE_NAME]    [dbo].[UDT_FILE_LOCATION] NULL,
    [PROCESSED_DATE]         [dbo].[UDT_DATE]          NULL,
    [FILE_ID]                [dbo].[UDT_ID]            NOT NULL,
    [REFERENCE_ID]           [dbo].[UDT_ID]            NULL,
    [STATUS_ID]              [dbo].[UDT_CODE_ID]       NOT NULL,
    [STATUS_VALUE]           [dbo].[UDT_CODE_VALUE]    NULL,
    [NO_OF_ROWS]             [dbo].[UDT_INT]           NULL,
    [CYCLE_NO]               [dbo].[UDT_INT]           NULL,
    [COMMENTS]               [dbo].[UDT_NOTES]         NULL,
    [ERROR_MESSAGE]          [dbo].[UDT_COMMENTS]      NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]     NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATETIME]      NOT NULL,
    [MODIFIED_BY]            [dbo].[UDT_CREATEDBY]     NOT NULL,
    [MODIFIED_DATE]          [dbo].[UDT_DATETIME]      NOT NULL,
    [UPDATE_SEQ]             [dbo].[UDT_UPDSEQ]        NOT NULL,
    [NOTIFICATION_SENT_DATE] [dbo].[UDT_DATE]          NULL
);

