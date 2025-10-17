CREATE TABLE [dbo].[SGS_FILE_DTL_ERROR] (
    [ERROR_ID]         [dbo].[UDT_IDENTITY]  IDENTITY (1, 1) NOT NULL,
    [FILE_DTL_ID]      [dbo].[UDT_ID]        NULL,
    [ERROR_MESSAGE_ID] [dbo].[UDT_TEXT]      NULL,
    [ERROR_MESSAGE]    [dbo].[UDT_TEXT]      NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY] NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]  NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_CREATEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]  NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]    NOT NULL
);

