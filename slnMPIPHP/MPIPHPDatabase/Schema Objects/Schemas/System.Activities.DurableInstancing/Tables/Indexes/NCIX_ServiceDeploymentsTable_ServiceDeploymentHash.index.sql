CREATE UNIQUE NONCLUSTERED INDEX [NCIX_ServiceDeploymentsTable_ServiceDeploymentHash]
    ON [System.Activities.DurableInstancing].[ServiceDeploymentsTable]([ServiceDeploymentHash] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = ON, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

