CREATE TABLE [dbo].[SGS_BATCH_PARAMETERS] (
    [BATCH_PARAMETERS_ID]  [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [STEP_NO]              [dbo].[UDT_INT]        NOT NULL,
    [PARAM_NAME]           [dbo].[UDT_SHORTNAME]  NOT NULL,
    [PARAM_DATATYPE]       [dbo].[UDT_SHORTNAME]  NOT NULL,
    [PARAM_VALUE]          [dbo].[UDT_LONGDESC]   NOT NULL,
    [REQUIRED_FLAG]        [dbo].[UDT_FLAG]       NOT NULL,
    [READONLY_FLAG]        [dbo].[UDT_FLAG]       NOT NULL,
    [REQUIRES_LOOKUP_FLAG] [dbo].[UDT_FLAG]       NOT NULL,
    [LOOKUP_FORM]          [dbo].[UDT_LONGDESC]   NULL,
    [RETURN_FIELD]         [dbo].[UDT_DESC]       NULL,
    [CREATED_BY]           [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]         [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]          [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]        [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]           [dbo].[UDT_INT]        NOT NULL
);

