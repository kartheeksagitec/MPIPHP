CREATE TABLE [dbo].[SGS_AUDIT_LOG_DETAIL] (
    [AUDIT_LOG_DETAIL_ID] [dbo].[UDT_IDENTITY]  IDENTITY (1, 1) NOT NULL,
    [AUDIT_LOG_ID]        [dbo].[UDT_ID]        NULL,
    [COLUMN_NAME]         [dbo].[UDT_DBOBJ]     NOT NULL,
    [OLD_VALUE]           [dbo].[UDT_COL_VALUE] NULL,
    [NEW_VALUE]           [dbo].[UDT_COL_VALUE] NULL
);

