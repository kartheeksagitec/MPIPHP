----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharm
-- Date - 12/23/2011
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5153)
PRINT 'MESSAGE_ID = 5153 already exits in SGS_MESSAGES tables'
ELSE

INSERT dbo.sgs_messages ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(5153,'Not Eligible for WithDrawal.',16,'E','','',16,'','asharma',GETDATE(),'asharma',GETDATE(),0)
GO

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/23/2011
-- Purpose - Added message_id- 5154 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5154)
PRINT 'MESSAGE_ID = 5154 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5154, 'Application Received Date should be less than Withdrawal Date.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO