CREATE TABLE [dbo].[SGS_COR_TRACKING] (
    [TRACKING_ID]             [dbo].[UDT_IDENTITY]   IDENTITY (1, 1) NOT NULL,
    [TEMPLATE_ID]             [dbo].[UDT_ID]         NOT NULL,
    [PERSON_ID]               [dbo].[UDT_ID]         NULL,
    [ORG_ID]                  [dbo].[UDT_ID]         NULL,
    [PLAN_ID]                 [dbo].[UDT_ID]         NULL,
    [CONTACT_ID]              [dbo].[UDT_INT]        NULL,
    [ORG_CONTACT_ID]          [dbo].[UDT_ID]         NULL,
    [COR_STATUS_ID]           [dbo].[UDT_CODE_ID]    NOT NULL,
    [COR_STATUS_VALUE]        [dbo].[UDT_CODE_VALUE] NULL,
    [GENERATED_DATE]          [dbo].[UDT_DATE]       NULL,
    [PRINT_ON_DATE]           [dbo].[UDT_DATE]       NULL,
    [PRINTED_DATE]            [dbo].[UDT_DATE]       NULL,
    [IMAGING_SERIAL_NO]       [dbo].[UDT_INT]        NULL,
    [COMMENTS]                [dbo].[UDT_COMMENTS]   NULL,
    [IMAGED_DATE]             [dbo].[UDT_DATE]       NULL,
    [CONVERTED_TO_IMAGE_FLAG] [dbo].[UDT_FLAG]       NULL,
    [CREATED_BY]              [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]            [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]             [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]           [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]              [dbo].[UDT_UPDSEQ]     NOT NULL
);

