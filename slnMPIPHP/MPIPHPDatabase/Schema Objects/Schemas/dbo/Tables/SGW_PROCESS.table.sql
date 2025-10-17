CREATE TABLE [dbo].[SGW_PROCESS] (
    [PROCESS_ID]       [dbo].[UDT_IDENTITY]   NOT NULL,
    [DESCRIPTION]      [dbo].[UDT_LONGNAME]   NOT NULL,
    [NAME]             [dbo].[UDT_LONGDESC]   NOT NULL,
    [PRIORITY]         [dbo].[UDT_INT]        NOT NULL,
    [TYPE_ID]          [dbo].[UDT_ID]         NULL,
    [TYPE_VALUE]       [dbo].[UDT_CODE_VALUE] NOT NULL,
    [STATUS_ID]        [dbo].[UDT_ID]         NULL,
    [STATUS_VALUE]     [dbo].[UDT_CODE_VALUE] NULL,
    [USE_NEW_MAP_FLAG] [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]     NOT NULL
);

