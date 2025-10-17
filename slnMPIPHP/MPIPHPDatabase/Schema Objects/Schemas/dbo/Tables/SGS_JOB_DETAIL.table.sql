CREATE TABLE [dbo].[SGS_JOB_DETAIL] (
    [JOB_DETAIL_ID]               [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [JOB_HEADER_ID]               [dbo].[UDT_ID]         NOT NULL,
    [STEP_NO]                     [dbo].[UDT_INT]        NOT NULL,
    [ORDER_NUMBER]                [dbo].[UDT_INT]        NOT NULL,
    [STATUS_ID]                   [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]                [dbo].[UDT_CODE_VALUE] NULL,
    [START_TIME]                  [dbo].[UDT_DATETIME]   NULL,
    [END_TIME]                    [dbo].[UDT_DATETIME]   NULL,
    [RETURN_CODE]                 [dbo].[UDT_INT]        NULL,
    [DEPENDENT_STEP_NO]           [dbo].[UDT_INT]        NULL,
    [DEPENDENT_STEP_RETURN_VALUE] [dbo].[UDT_CODE]       NULL,
    [OPERATOR_ID]                 [dbo].[UDT_CODE_ID]    NOT NULL,
    [OPERATOR_VALUE]              [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]                  [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                 [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]               [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                  [dbo].[UDT_UPDSEQ]     NOT NULL,
    [PROGRESS_PERCENTAGE]         [dbo].[UDT_INT]        NULL
);

