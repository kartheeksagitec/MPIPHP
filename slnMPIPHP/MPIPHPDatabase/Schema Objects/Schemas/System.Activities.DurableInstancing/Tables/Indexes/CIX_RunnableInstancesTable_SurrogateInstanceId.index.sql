CREATE UNIQUE CLUSTERED INDEX [CIX_RunnableInstancesTable_SurrogateInstanceId]
    ON [System.Activities.DurableInstancing].[RunnableInstancesTable]([SurrogateInstanceId] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = ON, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);

