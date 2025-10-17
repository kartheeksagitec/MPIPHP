CREATE TABLE [dbo].[SGT_ORG_CONTACT] (
    [ORG_CONTACT_ID]            [dbo].[UDT_ID]         IDENTITY (5001, 1) NOT NULL,
    [CONTACT_ID]                [dbo].[UDT_ID]         NULL,
    [ORG_ID]                    [dbo].[UDT_ID]         NOT NULL,
    [START_DATE]                [dbo].[UDT_DATE]       NOT NULL,
    [END_DATE]                  [dbo].[UDT_DATE]       NULL,
    [RECEIVE_NOTIFICATION_FLAG] [dbo].[UDT_FLAG]       NOT NULL,
    [NOTIFICATION_SENT_FLAG]    [dbo].[UDT_FLAG]       NOT NULL,
    [ORG_ADDRESS_ID]            [dbo].[UDT_ID]         NULL,
    [CONTACT_REASON_ID]         [dbo].[UDT_CODE_ID]    NOT NULL,
    [CONTACT_REASON_VALUE]      [dbo].[UDT_CODE_VALUE] NULL,
    [STATUS_ID]                 [dbo].[UDT_CODE_ID]    NOT NULL,
    [STATUS_VALUE]              [dbo].[UDT_CODE_VALUE] NOT NULL,
    [CREATED_BY]                [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]              [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]               [dbo].[UDT_CREATEDBY]  NOT NULL,
    [MODIFIED_DATE]             [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]                [dbo].[UDT_UPDSEQ]     NULL
);

