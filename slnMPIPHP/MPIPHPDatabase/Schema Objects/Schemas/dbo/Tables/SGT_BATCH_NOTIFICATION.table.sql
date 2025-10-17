CREATE TABLE [dbo].[SGT_BATCH_NOTIFICATION] (
    [BATCH_NOTIFICATION_ID]   [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [DESCRIPTION]             [dbo].[UDT_DESC]       NULL,
    [STATUS_ID]               [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]            [dbo].[UDT_CODE_VALUE] NULL,
    [NOTIFICATION_ID]         [dbo].[UDT_ID]         NULL,
    [NOTIFICATION_TIMER_DAYS] [dbo].[UDT_INT]        NULL,
    [SQL_QUERY_NAME]          [dbo].[UDT_DBOBJ]      NULL,
    [CREATED_BY]              [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]            [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [MODIFIED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]              [dbo].[UDT_UPDSEQ]     NULL
);

