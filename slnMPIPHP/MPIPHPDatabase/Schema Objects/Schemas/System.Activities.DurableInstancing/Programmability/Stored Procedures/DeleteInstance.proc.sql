create procedure [System.Activities.DurableInstancing].[DeleteInstance]
	@surrogateInstanceId bigint = null
as
begin	
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	
	delete [InstancePromotedPropertiesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [KeysTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [InstanceMetadataChangesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [RunnableInstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [InstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

end
