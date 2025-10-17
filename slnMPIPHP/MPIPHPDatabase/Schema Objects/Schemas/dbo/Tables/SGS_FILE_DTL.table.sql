CREATE TABLE [dbo].[SGS_FILE_DTL] (
    [FILE_DTL_ID]            [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [FILE_HDR_ID]            [dbo].[UDT_ID]         NOT NULL,
    [TRANSACTION_CODE_VALUE] [dbo].[UDT_DATA50]     NULL,
    [HEADER_GROUP_VALUE]     [dbo].[UDT_DATA]       NULL,
    [LINE_NO]                [dbo].[UDT_INT]        NULL,
    [RECORD_DATA]            [dbo].[UDT_FILE_MAX]   NULL,
    [STATUS_ID]              [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]           [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]            [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]          [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]             [dbo].[UDT_UPDSEQ]     NOT NULL
);

