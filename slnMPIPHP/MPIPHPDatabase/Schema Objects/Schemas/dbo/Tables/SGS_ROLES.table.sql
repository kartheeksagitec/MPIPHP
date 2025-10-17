CREATE TABLE [dbo].[SGS_ROLES] (
    [ROLE_ID]          INT                    IDENTITY (1, 1) NOT NULL,
    [ROLE_DESCRIPTION] [dbo].[UDT_LONGDESC]   NOT NULL,
    [CREATED_BY]       [dbo].[UDT_CREATEDBY]  NOT NULL,
    [CREATED_DATE]     [dbo].[UDT_DATETIME]   NOT NULL,
    [MODIFIED_BY]      [dbo].[UDT_MODIFIEDBY] NOT NULL,
    [MODIFIED_DATE]    [dbo].[UDT_DATETIME]   NOT NULL,
    [UPDATE_SEQ]       [dbo].[UDT_UPDSEQ]     NOT NULL
);

