GRANT CONNECT TO [dbo]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[InsertInstanceState] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[UnlockInstanceState] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[RetrieveInstanceState] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[RetrieveNonblockingInstanceStateIds] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[RetrieveANonblockingInstanceStateId] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[RetrieveExpiredTimerIds] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[InsertCompletedScope] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[DeleteCompletedScope] TO [state_persistence_users]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[RetrieveCompletedScope] TO [state_persistence_users]
    AS [dbo];


GO
GRANT DELETE
    ON OBJECT::[System.Activities.DurableInstancing].[Instances] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT DELETE
    ON OBJECT::[System.Activities.DurableInstancing].[ServiceDeployments] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[RecoverInstanceLocks] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[CreateLockOwner] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[DeleteLockOwner] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[ExtendLock] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[UnlockInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[DetectRunnableInstances] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[LoadInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[TryLoadRunnableInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[DeleteInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[CreateServiceDeployment] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[SaveInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[InsertPromotedProperties] TO [System.Activities.DurableInstancing.InstanceStoreUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[RecoverInstanceLocks] TO [System.Activities.DurableInstancing.WorkflowActivationUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[CreateLockOwner] TO [System.Activities.DurableInstancing.WorkflowActivationUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[DeleteLockOwner] TO [System.Activities.DurableInstancing.WorkflowActivationUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[ExtendLock] TO [System.Activities.DurableInstancing.WorkflowActivationUsers]
    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters] TO [System.Activities.DurableInstancing.WorkflowActivationUsers]
    AS [dbo];


GO
GRANT SELECT
    ON OBJECT::[System.Activities.DurableInstancing].[Instances] TO [System.Activities.DurableInstancing.InstanceStoreObservers]
    AS [dbo];


GO
GRANT SELECT
    ON OBJECT::[System.Activities.DurableInstancing].[ServiceDeployments] TO [System.Activities.DurableInstancing.InstanceStoreObservers]
    AS [dbo];


GO
GRANT SELECT
    ON OBJECT::[System.Activities.DurableInstancing].[InstancePromotedProperties] TO [System.Activities.DurableInstancing.InstanceStoreObservers]
    AS [dbo];

