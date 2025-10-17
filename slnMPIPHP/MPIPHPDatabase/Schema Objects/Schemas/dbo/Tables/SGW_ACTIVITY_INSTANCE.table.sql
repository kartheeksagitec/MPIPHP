CREATE TABLE [dbo].[SGW_ACTIVITY_INSTANCE] (
    [ACTIVITY_INSTANCE_ID]   [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [PROCESS_INSTANCE_ID]    [dbo].[UDT_INT]        NOT NULL,
    [ACTIVITY_ID]            [dbo].[UDT_INT]        NULL,
    [CHECKED_OUT_USER]       [dbo].[UDT_NAME]       NULL,
    [REFERENCE_ID]           [dbo].[UDT_ID]         NOT NULL,
    [STATUS_ID]              [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]           [dbo].[UDT_CODE_VALUE] NULL,
    [SUSPENSION_START_DATE]  [dbo].[UDT_DATETIME]   NULL,
    [SUSPENSION_MINUTES]     [dbo].[UDT_INT]        NULL,
    [SUSPENSION_END_DATE]    [dbo].[UDT_DATETIME]   NULL,
    [RETURN_FROM_AUDIT_FLAG] CHAR (1)               NULL,
    [RESUME_ACTION_ID]       [dbo].[UDT_ID]         NULL,
    [RESUME_ACTION_VALUE]    [dbo].[UDT_CODE_VALUE] NULL,
    [COMMENTS]               [dbo].[UDT_COMMENTS]   NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]            [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]          [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]             [dbo].[UDT_UPDSEQ]     NOT NULL
);

