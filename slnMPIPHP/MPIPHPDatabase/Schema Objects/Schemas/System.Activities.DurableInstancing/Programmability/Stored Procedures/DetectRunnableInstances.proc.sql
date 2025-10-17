
create procedure [System.Activities.DurableInstancing].[DetectRunnableInstances]
	@workflowHostType uniqueidentifier
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	
	declare @nextRunnableTime datetime

	select top 1 @nextRunnableTime = [RunnableInstancesTable].[RunnableTime]
			  from [RunnableInstancesTable] with (readpast)
			  where [WorkflowHostType] = @workflowHostType
			  order by [WorkflowHostType], [RunnableTime]
			  
	select 0 as 'Result', @nextRunnableTime, getutcdate()
end
