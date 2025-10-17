CREATE PROCEDURE DeleteInstance
	@id uniqueIdentifier,
	@hostId uniqueIdentifier,
	@lockTimeout int,
	@result int output
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @now datetime;
	SET @now = getutcdate();

	DELETE FROM [dbo].[InstanceData]
		WHERE (id = @id) AND ((@lockTimeout < 0) OR ((lockOwner = @hostId) AND (lockExpiration >= @now)));

	IF @@rowcount = 1
		SET @result = 0; -- Success
	ELSE
	BEGIN
		IF EXISTS (SELECT 1 FROM [dbo].[InstanceData] WHERE id = @id)
			SET @result = 2; -- Could not acquire lock
		ELSE
			SET @result = 1; -- Instance not found
	END
END
