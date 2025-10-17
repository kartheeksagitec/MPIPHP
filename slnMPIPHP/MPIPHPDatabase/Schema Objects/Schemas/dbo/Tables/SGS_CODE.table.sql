CREATE TABLE [dbo].[SGS_CODE] (
    [CODE_ID]                [dbo].[UDT_IDENTITY]   NOT NULL,
    [DESCRIPTION]            [dbo].[UDT_DESC]       NULL,
    [DATA1_CAPTION]          [dbo].[UDT_DATA]       NULL,
    [DATA1_TYPE]             [dbo].[UDT_CODE_VALUE] NULL,
    [DATA2_CAPTION]          [dbo].[UDT_DATA]       NULL,
    [DATA2_TYPE]             [dbo].[UDT_CODE_VALUE] NULL,
    [DATA3_CAPTION]          [dbo].[UDT_DATA]       NULL,
    [DATA3_TYPE]             [dbo].[UDT_CODE_VALUE] NULL,
    [FIRST_LOOKUP_ITEM]      [dbo].[UDT_KEY]        NULL,
    [FIRST_MAINTENANCE_ITEM] [dbo].[UDT_KEY]        NULL,
    [COMMENTS]               [dbo].[UDT_LONGDESC]   NULL,
    [LEGACY_CODE_ID]         [dbo].[UDT_CODE]       NULL,
    [CREATED_BY]             [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]           [dbo].[UDT_DATE]       NOT NULL,
    [MODIFIED_BY]            [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]          [dbo].[UDT_DATE]       NOT NULL,
    [UPDATE_SEQ]             [dbo].[UDT_UPDSEQ]     NOT NULL
);

