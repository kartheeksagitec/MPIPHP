CREATE TABLE [dbo].[SGT_ORGANIZATION] (
    [ORG_ID]         INT                    IDENTITY (1, 1) NOT NULL,
    [MPI_ORG_ID]     [dbo].[UDT_DATA]       NOT NULL,
    [ORG_NAME]       [dbo].[UDT_NAME]       NULL,
    [PHONE_NO]       [dbo].[UDT_PHONE]      NULL,
    [FAX_NO]         [dbo].[UDT_FAX]        NULL,
    [EMAIL_ADDRESS]  [dbo].[UDT_EMAIL]      NULL,
    [ORG_TYPE_ID]    [dbo].[UDT_ID]         NULL,
    [ORG_TYPE_VALUE] [dbo].[UDT_CODE_VALUE] NULL,
    [STATUS_ID]      [dbo].[UDT_ID]         NULL,
    [ROUTING_NUMBER] [dbo].[UDT_DATA]       NULL,
    [FEDERAL_ID]     [dbo].[UDT_DATA]       NULL,
    [STATUS_VALUE]   [dbo].[UDT_CODE_VALUE] NULL,
    [CREATED_BY]     [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]   [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]    [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]  [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]     [dbo].[UDT_UPDSEQ]     NOT NULL
);

