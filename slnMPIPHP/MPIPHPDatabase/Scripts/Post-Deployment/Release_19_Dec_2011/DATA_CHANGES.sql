----------------------------------------------------------------------------------

--Name - Kunal Arora
--Date - 19 Dec 2011
--Purpose - Added message for DRO ,Percentage and Amount cannot be -ve

----------------------------------------------------------------------------------
declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 2020)
PRINT 'MESSAGE_ID = 2020 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (2020, 'Percentage or Flat amount cannot be negative.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1159)
PRINT 'MESSAGE_ID = 1159 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (1159, 'Date of Birth cannot be greater that Date of Death.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO

