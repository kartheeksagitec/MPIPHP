CREATE TABLE [System.Activities.DurableInstancing].[RunnableInstancesTable] (
    [SurrogateInstanceId] BIGINT           NOT NULL,
    [WorkflowHostType]    UNIQUEIDENTIFIER NULL,
    [ServiceDeploymentId] BIGINT           NULL,
    [RunnableTime]        DATETIME         NOT NULL
);

