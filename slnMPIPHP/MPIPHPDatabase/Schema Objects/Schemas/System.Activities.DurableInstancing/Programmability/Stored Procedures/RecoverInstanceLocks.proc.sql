
create procedure [System.Activities.DurableInstancing].[RecoverInstanceLocks]
as
begin
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;
	set deadlock_priority low;
    
	declare @now as datetime
	set @now = getutcdate()	
	
	insert into [RunnableInstancesTable] ([SurrogateInstanceId], [WorkflowHostType], [ServiceDeploymentId], [RunnableTime])
		select top (1000) instances.[SurrogateInstanceId], instances.[WorkflowHostType], instances.[ServiceDeploymentId], @now
		from [LockOwnersTable] lockOwners with (readpast) inner loop join
			 [InstancesTable] instances with (readpast)
				on instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
			where 
				lockOwners.[LockExpiration] <= @now and
				instances.[IsInitialized] = 1 and
				instances.[IsSuspended] = 0

	delete from [LockOwnersTable] with (readpast)
	from [LockOwnersTable] lockOwners
	where [LockExpiration] <= @now
	and not exists
	(
		select top (1) 1
		from [InstancesTable] instances with (nolock)
		where instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
	)
end
