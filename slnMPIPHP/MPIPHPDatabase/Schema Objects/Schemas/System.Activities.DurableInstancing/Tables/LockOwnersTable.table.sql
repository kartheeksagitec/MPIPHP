CREATE TABLE [System.Activities.DurableInstancing].[LockOwnersTable] (
    [Id]                              UNIQUEIDENTIFIER NOT NULL,
    [SurrogateLockOwnerId]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [LockExpiration]                  DATETIME         NOT NULL,
    [WorkflowHostType]                UNIQUEIDENTIFIER NULL,
    [MachineName]                     NVARCHAR (128)   NOT NULL,
    [EnqueueCommand]                  BIT              NOT NULL,
    [DeletesInstanceOnCompletion]     BIT              NOT NULL,
    [PrimitiveLockOwnerData]          VARBINARY (MAX)  DEFAULT (NULL) NULL,
    [ComplexLockOwnerData]            VARBINARY (MAX)  DEFAULT (NULL) NULL,
    [WriteOnlyPrimitiveLockOwnerData] VARBINARY (MAX)  DEFAULT (NULL) NULL,
    [WriteOnlyComplexLockOwnerData]   VARBINARY (MAX)  DEFAULT (NULL) NULL,
    [EncodingOption]                  TINYINT          DEFAULT ((0)) NULL
);

