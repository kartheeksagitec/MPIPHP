CREATE TABLE [dbo].[SGW_PROCESS_INSTANCE_IMAGE_DATA] (
    [PROCESS_INSTANCE_IMAGE_DATA_ID] INT          IDENTITY (1, 1) NOT NULL,
    [PROCESS_INSTANCE_ID]            INT          NOT NULL,
    [FILENET_DOCUMENT_TYPE_ID]       INT          NULL,
    [FILENET_DOCUMENT_TYPE_VALUE]    VARCHAR (4)  NULL,
    [IMAGE_DOC_CATEGORY_ID]          INT          NULL,
    [IMAGE_DOC_CATEGORY_VALUE]       VARCHAR (4)  NULL,
    [DOC_TYPE]                  VARCHAR (50) NULL,
    [INITIATED_DATE]                 DATETIME     NULL,
    [CREATED_BY]                     VARCHAR (50) NOT NULL,
    [CREATED_DATE]                   DATETIME     NOT NULL,
    [MODIFIED_BY]                    VARCHAR (50) NOT NULL,
    [MODIFIED_DATE]                  DATETIME     NOT NULL,
    [UPDATE_SEQ]                     INT          NOT NULL
);

