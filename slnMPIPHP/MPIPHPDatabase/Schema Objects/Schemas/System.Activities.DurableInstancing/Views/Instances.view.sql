
create view [System.Activities.DurableInstancing].[Instances] as
      select [InstancesTable].[Id] as [InstanceId],
             [PendingTimer],
             [CreationTime],
             [LastUpdated] as [LastUpdatedTime],
             [InstancesTable].[ServiceDeploymentId],
             [SuspensionExceptionName],
             [SuspensionReason],
             [BlockingBookmarks] as [ActiveBookmarks],
             case when [LockOwnersTable].[LockExpiration] > getutcdate()
				then [LockOwnersTable].[MachineName]
				else null
				end as [CurrentMachine],
             [LastMachineRunOn] as [LastMachine],
             [ExecutionStatus],
             [IsInitialized],
             [IsSuspended],
             [IsCompleted],
             [InstancesTable].[DataEncodingOption] as [EncodingOption],
             [PrimitiveDataProperties] as [ReadWritePrimitiveDataProperties],
             [WriteOnlyPrimitiveDataProperties],
             [ComplexDataProperties] as [ReadWriteComplexDataProperties],
             [WriteOnlyComplexDataProperties]
      from [System.Activities.DurableInstancing].[InstancesTable]
      left outer join [System.Activities.DurableInstancing].[LockOwnersTable]
      on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
