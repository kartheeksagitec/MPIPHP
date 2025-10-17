CREATE TABLE [dbo].[SGW_ACTIVITY] (
    [ACTIVITY_ID]                    [dbo].[UDT_IDENTITY]   NOT NULL,
    [PROCESS_ID]                     [dbo].[UDT_ID]         NOT NULL,
    [NAME]                           [dbo].[UDT_LONGNAME]   NOT NULL,
    [DISPLAY_NAME]                   [dbo].[UDT_LONGNAME]   NULL,
    [STANDARD_TIME_IN_MINUTES]       [dbo].[UDT_INT]        NULL,
    [ROLE_ID]                        [dbo].[UDT_INT]        NULL,
    [SUPERVISOR_ROLE_ID]             [dbo].[UDT_INT]        NULL,
    [SORT_ORDER]                     [dbo].[UDT_INT]        NULL,
    [IS_DELETED_FLAG]                [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]                     [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]                   [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]                    [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]                  [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                     [dbo].[UDT_UPDSEQ]     NOT NULL,
    [ALLOW_INDEPENDENT_COMPLETE_IND] [dbo].[UDT_IND]        NULL
);

