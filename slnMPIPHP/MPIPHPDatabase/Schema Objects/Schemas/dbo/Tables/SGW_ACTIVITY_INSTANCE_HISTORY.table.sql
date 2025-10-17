CREATE TABLE [dbo].[SGW_ACTIVITY_INSTANCE_HISTORY] (
    [ACTIVITY_INSTANCE_HISTORY_ID] [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [ACTIVITY_INSTANCE_ID]         [dbo].[UDT_INT]        NOT NULL,
    [STATUS_ID]                    [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]                 [dbo].[UDT_CODE_VALUE] NULL,
    [ACTION_USER_ID]               [dbo].[UDT_NAME]       NULL,
    [START_TIME]                   [dbo].[UDT_DATETIME]   NULL,
    [END_TIME]                     [dbo].[UDT_DATETIME]   NULL,
    [COMMENTS]                     [dbo].[UDT_COMMENTS]   NULL,
    [CREATED_BY]                   [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                  [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                   [dbo].[UDT_UPDSEQ]     NOT NULL
);

