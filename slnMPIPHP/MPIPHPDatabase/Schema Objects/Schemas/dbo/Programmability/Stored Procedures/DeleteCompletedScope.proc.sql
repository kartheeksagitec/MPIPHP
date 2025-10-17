CREATE PROCEDURE [dbo].[DeleteCompletedScope]
@completedScopeID uniqueidentifier
AS
DELETE FROM [dbo].[CompletedScope] WHERE completedScopeID=@completedScopeID
