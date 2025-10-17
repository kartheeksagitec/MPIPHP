CREATE TABLE [dbo].[SGS_CUSTOM_ACCESS_POLICY] (
    [CUSTOM_ACCESS_POLICY_ID] [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [USER_SERIAL_ID]          [dbo].[UDT_ID]         NOT NULL,
    [WEEKDAY_ID]              [dbo].[UDT_CODE_ID]    NOT NULL,
    [WEEKDAY_VALUE]           [dbo].[UDT_CODE_VALUE] NULL,
    [FROM_TIME]               [dbo].[UDT_TIME]       NULL,
    [TO_TIME]                 [dbo].[UDT_TIME]       NULL,
    [CREATED_BY]              [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]            [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]             [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]              [dbo].[UDT_UPDSEQ]     NOT NULL
);

