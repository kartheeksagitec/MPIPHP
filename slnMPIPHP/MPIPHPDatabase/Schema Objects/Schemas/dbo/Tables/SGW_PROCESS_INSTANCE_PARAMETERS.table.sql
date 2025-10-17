CREATE TABLE [dbo].[SGW_PROCESS_INSTANCE_PARAMETERS] (
    [PROCESS_INSTANCE_PARAMETER_ID] [dbo].[UDT_ID]         IDENTITY (1, 1) NOT NULL,
    [PROCESS_INSTANCE_ID]           [dbo].[UDT_INT]        NOT NULL,
    [PARAMETER_NAME]                [dbo].[UDT_NAME]       NOT NULL,
    [PARAMETER_VALUE]               [dbo].[UDT_COMMENTS]   NULL,
    [CREATED_BY]                    [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                   [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                 [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                    [dbo].[UDT_UPDSEQ]     NOT NULL
);

