CREATE PROCEDURE UpdateInstance
	@id uniqueidentifier,
	@instance image = NULL,
	@instanceXml xml = NULL,
	@unlockInstance bit,
	@hostId uniqueidentifier,
	@lockTimeout int,
	@result int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

	SET @result = 0;

	DECLARE @now datetime, @lockExpiration datetime, @newOwner uniqueidentifier;
	SET @now = getutcdate();
	
	IF @lockTimeout < 0 OR @unlockInstance = 'TRUE'
	BEGIN
		SET @lockExpiration = NULL;
		SET @newOwner = NULL;
	END
	ELSE 
	BEGIN
		SET @newOwner = @hostId;

		IF @lockTimeout = 0
			SET @lockExpiration = '9999-12-31T23:59:59';
		ELSE
			SET @lockExpiration = dateadd(second, @lockTimeout, @now);
	END

	UPDATE [dbo].[InstanceData] SET
		instance = @instance,
		instanceXml = @instanceXml,
		lastUpdated = @now,
		lockOwner = @newOwner,
		lockExpiration = @lockExpiration
		WHERE (id = @id) AND ((@lockTimeout < 0) OR ((lockOwner = @hostId) AND (lockExpiration >= @now)));

	IF @@rowcount = 0
	BEGIN
		IF EXISTS(SELECT 1 FROM [dbo].[InstanceData] WHERE id = @id)
			SET @result = 2; -- Did not have lock
		ELSE
			SET @result = 1; -- Instance was not found in the database for update
	END
END
