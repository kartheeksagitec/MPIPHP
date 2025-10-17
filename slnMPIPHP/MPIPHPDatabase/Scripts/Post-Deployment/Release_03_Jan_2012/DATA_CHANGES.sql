----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 01/03/2012
-- Purpose - Updated message_id-5119 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_MESSAGES set DISPLAY_MESSAGE ='Date of Birth cannot be greater than Date of Death.' where MESSAGE_ID = 1159



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 01/03/2012
-- Purpose - Add messages in SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------
declare @currentdatetime datetime 
set @currentdatetime = getdate() 
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5156) 
PRINT 'MESSAGE_ID = 5156 already exits in SGS_MESSAGES tables' 
ELSE 
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 

VALUES (5156, 'Survivor Benefit Election Received Date is required', 16 , 'E', Null, Null, 16, null, 'rohan', @currentdatetime, 'rohan', @currentdatetime, 0)

GO

declare @currentdatetime datetime 
set @currentdatetime = getdate() 
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5157) 
PRINT 'MESSAGE_ID = 5157 already exits in SGS_MESSAGES tables' 
ELSE 
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 

VALUES (5157, 'Survivor Benefit Election Received Date cannot be future date', 16 , 'E', Null, Null, 16, null, 'rohan', @currentdatetime, 'rohan', @currentdatetime, 0)

GO


