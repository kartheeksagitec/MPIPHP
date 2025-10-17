CREATE TABLE [dbo].[SGS_JOB_SCHEDULE_DETAIL_ERROR] (
    [JOB_SCHEDULE_DETAIL_ERROR_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [JOB_SCHEDULE_DETAIL_ID]       [dbo].[UDT_INT]        NOT NULL,
    [MESSAGE_ID]                   [dbo].[UDT_INT]        NOT NULL,
    [CREATED_BY]                   [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                  [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                   [dbo].[UDT_UPDSEQ]     NOT NULL
);

