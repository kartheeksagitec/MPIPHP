CREATE PROCEDURE InsertInstance
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

	INSERT INTO [dbo].[InstanceData] (id, instance, instanceXml, created, lastUpdated, lockOwner, lockExpiration)
		VALUES (@id, @instance, @instanceXml, @now, @now, @newOwner, @lockExpiration);

	IF @@rowcount = 0
	BEGIN
		IF EXISTS(SELECT 1 FROM [dbo].[InstanceData] WHERE id = @id)
			SET @result = 1; -- The instance already existed.
		ELSE
			SET @result = 2; -- Some other non-fatal error caused us not to insert
	END
END
