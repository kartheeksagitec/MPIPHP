CREATE TABLE [dbo].[SGS_PIR_ATTACHMENT] (
    [PIR_ATTACHMENT_ID]    [dbo].[UDT_IDENTITY]          IDENTITY (5001, 1) NOT NULL,
    [PIR_ID]               [dbo].[UDT_ID]                NOT NULL,
    [ATTACHMENT_CONTENT]   VARBINARY (MAX)               NOT NULL,
    [ATTACHMENT_GUID]      [dbo].[UDT_UNIQUE_IDENTIFIER] NOT NULL,
    [ATTACHMENT_FILE_NAME] [dbo].[UDT_NOTES]             NOT NULL,
    [ATTACHMENT_MIME_TYPE] [dbo].[UDT_DESC]              NOT NULL,
    [CREATED_BY]           [dbo].[UDT_CREATEDBY]         NOT NULL,
    [CREATED_DATE]         [dbo].[UDT_DATE]              NOT NULL,
    [MODIFIED_BY]          [dbo].[UDT_MODIFIEDBY]        NOT NULL,
    [MODIFIED_DATE]        [dbo].[UDT_DATE]              NOT NULL
);

