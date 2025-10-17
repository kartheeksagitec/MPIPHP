create procedure [System.Activities.DurableInstancing].[CreateInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint,
	@surrogateInstanceId bigint output,
	@result int output
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	set @surrogateInstanceId = 0
	set @result = 0
	
	begin try
		insert into [InstancesTable] ([Id], [SurrogateLockOwnerId], [CreationTime], [WorkflowHostType], [ServiceDeploymentId], [Version])
		values (@instanceId, @surrogateLockOwnerId, getutcdate(), @workflowHostType, @serviceDeploymentId, 1)
		
		set @surrogateInstanceId = scope_identity()		
	end try
	begin catch
		if (error_number() != 2601)
		begin
			set @result = 99
			select @result as 'Result'
		end
	end catch
end
