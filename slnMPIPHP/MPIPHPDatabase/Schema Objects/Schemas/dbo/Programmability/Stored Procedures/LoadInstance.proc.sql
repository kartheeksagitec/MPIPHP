CREATE PROCEDURE LoadInstance
	@id uniqueidentifier,
	@lockInstance bit,
	@hostId uniqueidentifier,
	@lockTimeout int,
	@result int output
AS
BEGIN
	SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

	DECLARE @createdTransaction bit;
	
	DECLARE @now datetime, @lockExpiration datetime;
	SET @now = getutcdate();
	SET @result = 0;

	IF (@lockTimeout < 0) OR (@lockInstance = 'FALSE')
	BEGIN
		SELECT 
			'instance' = instance,
			'instanceXml' = instanceXml,
			'isXml' = CASE
				WHEN instanceXml is NOT NULL THEN 1
				ELSE 0
			END
			FROM [dbo].[InstanceData]
			WHERE id = @id;

		IF @@rowcount = 0
			SET @result = 1; -- Instance not found
	END
	ELSE
	BEGIN
		IF @lockTimeout = 0
			SET @lockExpiration = '9999-12-31T23:59:59';
		ELSE 
			SET @lockExpiration = dateadd(second, @lockTimeout, @now);

		IF @@trancount = 0
		BEGIN
			SET @createdTransaction = 'TRUE';
			BEGIN TRANSACTION;
		END

		UPDATE [dbo].[InstanceData] SET
			lockOwner = @hostId,
			lockExpiration = @lockExpiration
			WHERE (id = @id) AND ((lockOwner is NULL) OR (lockOwner = @hostId) OR (lockExpiration < @now));

		IF @@rowcount = 1
		BEGIN
			SELECT 
				instance,
				instanceXml,
				'isXml' = CASE
					WHEN instanceXml is NOT NULL THEN 1
					ELSE 0
				END
				FROM [dbo].[InstanceData]
				WHERE id = @id;

			IF @@error <> 0
			BEGIN
				ROLLBACK TRANSACTION
				RETURN
			END
		END
		ELSE
		BEGIN 
			IF EXISTS (SELECT 1 FROM [dbo].[InstanceData] WHERE id = @id)
				SET @result = 2; -- Could not acquire lock
			ELSE
				SET @result = 1; -- Instance not found
		END

		IF @createdTransaction = 'TRUE'
			COMMIT TRANSACTION
	END
END
