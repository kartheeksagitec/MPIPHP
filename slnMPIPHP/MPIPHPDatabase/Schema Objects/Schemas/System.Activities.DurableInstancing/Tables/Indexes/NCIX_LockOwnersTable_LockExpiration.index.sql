CREATE NONCLUSTERED INDEX [NCIX_LockOwnersTable_LockExpiration]
    ON [System.Activities.DurableInstancing].[LockOwnersTable]([LockExpiration] ASC)
    INCLUDE([WorkflowHostType], [MachineName]) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

