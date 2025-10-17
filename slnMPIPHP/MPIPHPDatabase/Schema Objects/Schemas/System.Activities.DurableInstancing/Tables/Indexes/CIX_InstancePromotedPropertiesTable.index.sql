CREATE UNIQUE CLUSTERED INDEX [CIX_InstancePromotedPropertiesTable]
    ON [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]([SurrogateInstanceId] ASC, [PromotionName] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);

