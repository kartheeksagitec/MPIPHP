

CREATE PROC [dbo].[P_GENERATE_INSERTS](@table_name VARCHAR(100), @owner VARCHAR(10))
AS
BEGIN

SET NOCOUNT ON

--Variable declarations
DECLARE		@Column_ID int, 		
		@Column_List varchar(8000), 
		@Column_Name varchar(128), 
		@Start_Insert varchar(786), 
		@Data_Type varchar(128), 
		@Actual_Values varchar(8000),	--This is the string that will be finally executed to generate INSERT statements
		@IDN varchar(128),		--Will contain the IDENTITY column's name in the table
    @Count INT
    
DECLARE  @TABLE TABLE(SEQ_NO INT IDENTITY(1,1), INSERT_VALUE VARCHAR(MAX))

--Variable Initialization
SET @IDN = ''
SET @Column_ID = 0
SET @Column_Name = ''
SET @Column_List = ''
SET @Actual_Values = ''


IF NOT EXISTS (SELECT name FROM sysobjects WHERE id = OBJECT_ID(@Table_Name))
    BEGIN
      PRINT '--Table '+ @Table_Name + ' Not Exists in the database '--+ DB_Name()
      RETURN
    END
--To get the first column's ID

SELECT	@Column_ID = MIN(ORDINAL_POSITION) 	
FROM	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
WHERE 	TABLE_NAME = @table_name AND (@owner IS NULL OR TABLE_SCHEMA = @owner)



--Loop through all the columns of the table, to get the column names and their data types
WHILE @Column_ID IS NOT NULL
	BEGIN
		SELECT 	@Column_Name = QUOTENAME(COLUMN_NAME), 
		        @Data_Type = DATA_TYPE 
		FROM 	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
		WHERE ORDINAL_POSITION = @Column_ID 
		AND 	TABLE_NAME = @table_name AND (@owner IS NULL OR TABLE_SCHEMA = @owner)

 		--Making sure to output SET IDENTITY_INSERT ON/OFF in case the table has an IDENTITY column
		IF (SELECT COLUMNPROPERTY( OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name),SUBSTRING(@Column_Name,2,LEN(@Column_Name) - 2),'IsIdentity')) = 1 
				SET @IDN = @Column_Name
		
    
		--Determining the data type of the column and depending on the data type, the VALUES part of
		--the INSERT statement is generated. Care is taken to handle columns with NULL values. Also
		--making sure, not to lose any data from flot, real, money, smallmomey, datetime columns
		SET @Actual_Values = @Actual_Values  +
		CASE 
			WHEN @Data_Type IN ('char','varchar','nchar','nvarchar') 
				THEN 
					'COALESCE('''''''' + REPLACE(RTRIM(' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'
			WHEN @Data_Type IN ('datetime','smalldatetime') 
				THEN 
					'COALESCE('''''''' + RTRIM(CONVERT(char,' + @Column_Name + ',109))+'''''''',''NULL'')'
			WHEN @Data_Type IN ('uniqueidentifier') 
				THEN  
					'COALESCE('''''''' + REPLACE(CONVERT(char(255),RTRIM(' + @Column_Name + ')),'''''''','''''''''''')+'''''''',''NULL'')'
			WHEN @Data_Type IN ('text','ntext') 
				THEN  
					'COALESCE('''''''' + REPLACE(CONVERT(char(8000),' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'					
			WHEN @Data_Type IN ('binary','varbinary') 
				THEN  
					'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')'  
			WHEN @Data_Type IN ('timestamp','rowversion') 
				THEN 'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')'  
			WHEN @Data_Type IN ('float','real','money','smallmoney')
				THEN
					'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ',2)' + ')),''NULL'')' 
			ELSE 
				'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ')' + ')),''NULL'')' 
		END   + '+' +  ''',''' + ' + '
		
		--Generating the column list for the INSERT statement
		SET @Column_List = @Column_List +  @Column_Name + ','	

		SKIP_LOOP: --The label used in GOTO

		SELECT 	@Column_ID = MIN(ORDINAL_POSITION) 
		FROM 	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
		WHERE 	TABLE_NAME = @table_name AND 
		ORDINAL_POSITION > @Column_ID AND
		(@owner IS NULL OR TABLE_SCHEMA = @owner)


	--Loop ends here!
	END

--To get rid of the extra characters that got concatenated during the last run through the loop
SET @Column_List = LEFT(@Column_List,len(@Column_List) - 1)
SET @Actual_Values = LEFT(@Actual_Values,len(@Actual_Values) - 6)


SET @Start_Insert = 'INSERT ' + 'dbo.' + @table_name		

--Forming the final string that will be executed, to output the INSERT statements
--IF (@include_column_list <> 0)
	BEGIN
		SET @Actual_Values = 
			'SELECT ' +  
			--CASE WHEN @top IS NULL OR @top < 0 THEN '' ELSE ' TOP ' + LTRIM(STR(@top)) + ' ' END + 
			'''' + RTRIM(@Start_Insert) + 
			' ''+' + '''(' + RTRIM(@Column_List) +  '''+' + ''')''' + 
			' +''VALUES(''+ ' +  @Actual_Values  + '+'')''' + ' ' + 
			' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE LTRIM(RTRIM(@owner)) + '.' END +  rtrim(@table_name) +  '(NOLOCK)'
			--COALESCE(@from,' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE '[' + LTRIM(RTRIM(@owner)) + '].' END + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')
	END






DECLARE @l_MIn INT, @l_Max INT, @l_Text VARCHAR(MAX)

INSERT INTO @TABLE
EXEC (@Actual_Values)
SELECT @l_MAX = @@ROWCOUNT, @l_Min = 1

--Check data Exists in @table_name
IF @l_MAX >0 
BEGIN
  PRINT '/****** Object:  Table ' + @owner + '.' + @table_name + '  ********/'
  --Determining whether to print IDENTITY_INSERT or not
  IF (@IDN <> '')
	BEGIN
		PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' ON'
		PRINT 'GO'
	END
END
ELSE
  RETURN




WHILE (@l_Min <= @l_MAX)
BEGIN

SElect @l_Text =  INSERT_VALUE from @TABLE WHERE SEQ_NO = @l_Min
PRINT @l_Text

SET @l_Min = @l_Min + 1

END


--PRINT 'PRINT ''Done'''
--PRINT ''
--PRINT ''


IF (@IDN <> '')
	BEGIN
		PRINT 'GO'
		PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' OFF'
		PRINT 'GO'
	END

--PRINT 'SET NOCOUNT OFF'


SET NOCOUNT OFF
RETURN 0 --Success. We are done!
END

