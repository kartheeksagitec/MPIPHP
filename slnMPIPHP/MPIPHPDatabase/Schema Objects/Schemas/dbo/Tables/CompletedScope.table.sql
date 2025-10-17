CREATE TABLE [dbo].[CompletedScope] (
    [uidInstanceID]    UNIQUEIDENTIFIER NOT NULL,
    [completedScopeID] UNIQUEIDENTIFIER NOT NULL,
    [state]            IMAGE            NOT NULL,
    [modified]         DATETIME         NOT NULL
);

