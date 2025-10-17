CREATE NONCLUSTERED INDEX [NCIX_RunnableInstancesTable_RunnableTime]
    ON [System.Activities.DurableInstancing].[RunnableInstancesTable]([RunnableTime] ASC)
    INCLUDE([WorkflowHostType], [ServiceDeploymentId]) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

