CREATE TABLE [System.Activities.DurableInstancing].[KeysTable] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [SurrogateKeyId]      BIGINT           IDENTITY (1, 1) NOT NULL,
    [SurrogateInstanceId] BIGINT           NULL,
    [EncodingOption]      TINYINT          NULL,
    [Properties]          VARBINARY (MAX)  NULL,
    [IsAssociated]        BIT              NULL
);

