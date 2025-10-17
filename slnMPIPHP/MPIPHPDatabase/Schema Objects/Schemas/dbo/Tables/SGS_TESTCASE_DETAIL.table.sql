CREATE TABLE [dbo].[SGS_TESTCASE_DETAIL] (
    [TESTCASE_DTL_ID]   [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [TESTCASE_ID]       [dbo].[UDT_ID]         NOT NULL,
    [TESTCASE_DTL_KEY]  [dbo].[UDT_DATA]       NULL,
    [TESTCASE_DTL_DESC] [dbo].[UDT_DESC]       NULL,
    [NOTES]             [dbo].[UDT_NOTES]      NULL,
    [CREATED_BY]        [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]      [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]       [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]        [dbo].[UDT_UPDSEQ]     NOT NULL
);

