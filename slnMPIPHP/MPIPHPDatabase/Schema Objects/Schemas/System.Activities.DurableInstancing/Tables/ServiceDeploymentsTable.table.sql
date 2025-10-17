CREATE TABLE [System.Activities.DurableInstancing].[ServiceDeploymentsTable] (
    [Id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ServiceDeploymentHash]   UNIQUEIDENTIFIER NOT NULL,
    [SiteName]                NVARCHAR (MAX)   NOT NULL,
    [RelativeServicePath]     NVARCHAR (MAX)   NOT NULL,
    [RelativeApplicationPath] NVARCHAR (MAX)   NOT NULL,
    [ServiceName]             NVARCHAR (MAX)   NOT NULL,
    [ServiceNamespace]        NVARCHAR (MAX)   NOT NULL
);

