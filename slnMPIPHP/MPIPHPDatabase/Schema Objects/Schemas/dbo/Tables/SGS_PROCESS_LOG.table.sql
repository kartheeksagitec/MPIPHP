CREATE TABLE [dbo].[SGS_PROCESS_LOG] (
    [PROCESS_LOG_ID]         [dbo].[UDT_IDENTITY]   IDENTITY (7322, 1) NOT NULL,
    [CYCLE_NO]               [dbo].[UDT_INT]        NULL,
    [PROCESS_NAME]           [dbo].[UDT_DATA50]     NULL,
    [MESSAGE_TYPE_ID]        [dbo].[UDT_CODE_ID]    NOT NULL,
    [MESSAGE_TYPE_VALUE]     [dbo].[UDT_CODE_VALUE] NULL,
    [MESSAGE]                [dbo].[UDT_FILE_MAX]   NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [SUBSYSTEM_REFERENCE_ID] [dbo].[UDT_SHORTDESC]  NULL,
    [SUBSYSTEM_TABLE_NAME]   [dbo].[UDT_DBOBJ]      NULL,
    [JOB_HEADER_ID]          [dbo].[UDT_ID]         NULL
);

