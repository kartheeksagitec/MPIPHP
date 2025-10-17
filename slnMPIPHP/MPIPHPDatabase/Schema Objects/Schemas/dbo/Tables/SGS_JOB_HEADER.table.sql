CREATE TABLE [dbo].[SGS_JOB_HEADER] (
    [JOB_HEADER_ID]   [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [JOB_NAME]        [dbo].[UDT_LONGDESC]   NOT NULL,
    [JOB_SCHEDULE_ID] [dbo].[UDT_INT]        NULL,
    [STATUS_ID]       [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]    [dbo].[UDT_CODE_VALUE] NULL,
    [START_TIME]      [dbo].[UDT_DATETIME]   NULL,
    [END_TIME]        [dbo].[UDT_DATETIME]   NULL,
    [CREATED_BY]      [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]     [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]   [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]      [dbo].[UDT_UPDSEQ]     NOT NULL
);

