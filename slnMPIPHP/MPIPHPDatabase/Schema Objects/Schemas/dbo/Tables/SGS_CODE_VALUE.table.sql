CREATE TABLE [dbo].[SGS_CODE_VALUE] (
    [CODE_SERIAL_ID]   [dbo].[UDT_IDENTITY]   IDENTITY (10000, 1) NOT NULL,
    [CODE_ID]          [dbo].[UDT_ID]         NOT NULL,
    [CODE_VALUE]       [dbo].[UDT_CODE_VALUE] NOT NULL,
    [DESCRIPTION]      [dbo].[UDT_LONGDESC]   NOT NULL,
    [DATA1]            [dbo].[UDT_FILE_NAME]  NULL,
    [DATA2]            [dbo].[UDT_FILE_NAME]  NULL,
    [DATA3]            [dbo].[UDT_FILE_NAME]  NULL,
    [COMMENTS]         [dbo].[UDT_LONGDESC]   NULL,
    [START_DATE]       [dbo].[UDT_DATE]       NULL,
    [END_DATE]         [dbo].[UDT_DATE]       NULL,
    [CODE_VALUE_ORDER] [dbo].[UDT_INT]        NULL,
    [LEGACY_CODE_ID]   [dbo].[UDT_CODE]       NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]     NOT NULL
);

