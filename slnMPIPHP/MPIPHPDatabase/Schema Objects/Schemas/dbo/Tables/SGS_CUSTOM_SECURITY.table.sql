CREATE TABLE [dbo].[SGS_CUSTOM_SECURITY] (
    [CUSTOM_SECURITY_ID]          INT          IDENTITY (1, 1) NOT NULL,
    [USER_SERIAL_ID]              INT          NULL,
    [RESOURCE_ID]                 INT          NULL,
    [OLD_SECURITY_Value]          INT          NOT NULL,
    [CUSTOM_SECURITY_Level_ID]    INT          NULL,
    [CUSTOM_SECURITY_Level_VALUE] VARCHAR (4)  NULL,
    [CREATED_BY]                  VARCHAR (50) NOT NULL,
    [CREATED_DATE]                DATETIME     NOT NULL,
    [MODIFIED_BY]                 VARCHAR (50) NOT NULL,
    [MODIFIED_DATE]               DATETIME     NOT NULL,
    [UPDATE_SEQ]                  INT          NOT NULL
);

