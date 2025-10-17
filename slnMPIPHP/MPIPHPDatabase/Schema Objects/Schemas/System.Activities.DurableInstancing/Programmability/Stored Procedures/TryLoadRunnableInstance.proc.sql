
create procedure [System.Activities.DurableInstancing].[TryLoadRunnableInstance]
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@operationType tinyint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@encodingOption tinyint,	
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	begin tran
	
	declare @instanceId uniqueIdentifier
	declare @now datetime
	set @now = getutcdate()
	
	select top (1) @instanceId = instances.[Id]
	from [RunnableInstancesTable] runnableInstances with (readpast, updlock)
		inner loop join [InstancesTable] instances with (readpast, updlock)
		on runnableInstances.[SurrogateInstanceId] = instances.[SurrogateInstanceId]
	where runnableInstances.[WorkflowHostType] = @workflowHostType
		  and 
	      runnableInstances.[RunnableTime] <= @now
    
    if (@@rowcount = 1)
    begin
		select 0 as 'Result', cast(1 as bit)				
		exec [System.Activities.DurableInstancing].[LoadInstance] @surrogateLockOwnerId, @operationType, @handleInstanceVersion, @handleIsBoundToLock, null, @instanceId, null, @encodingOption, null, null, @operationTimeout
    end	
    else
    begin
		select 0 as 'Result', cast(0 as bit)
    end
    
    if (@@trancount > 0)
    begin
		commit tran
    end
end
