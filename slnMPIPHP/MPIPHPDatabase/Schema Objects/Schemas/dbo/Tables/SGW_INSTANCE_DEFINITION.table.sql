CREATE TABLE [dbo].[SGW_INSTANCE_DEFINITION] (
    [WORKFLOW_INSTANCE_GUID] UNIQUEIDENTIFIER NOT NULL,
    [FILE_PATH]              VARCHAR (512)    NOT NULL,
    [XAML_COPY]              XML              NULL,
    [XML_COPY]               XML              NULL,
    [ACTIVITIES_COPY]        XML              NULL
);

