
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/21/2011
-- Purpose - Added message_id- 5149 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5149)
PRINT 'MESSAGE_ID = 5149 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5149, 'SSA Application Date must be prior to the Early Retirement Date', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/21/2011
-- Purpose - updated message_id- 5130 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_MESSAGES set DISPLAY_MESSAGE='Application Received date should be greater than SSA aaplication date.' where MESSAGE_ID=5130

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/21/2011
-- Purpose - Added message_id- 1160 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1160)
PRINT 'MESSAGE_ID = 1160 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1160,'Please provide the 10 digit phone number.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO


declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1161)
PRINT 'MESSAGE_ID = 1161 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1161,'Please provide the 10 digit fax number.',16,'E',16,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO


declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1162)
PRINT 'MESSAGE_ID = 1162 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1162,'Please provide the 9 digit SSN number.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/21/2011
-- Purpose - Added messages into the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime 
set @currentdatetime = getdate() 
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5150) 
PRINT 'MESSAGE_ID = 5150 already exits in SGS_MESSAGES tables' 
ELSE 
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 

VALUES (5150, 'An Approved Retirement Application for this Person and Plan already exists.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)

GO

declare @currentdatetime datetime 
set @currentdatetime = getdate() 
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5151) 
PRINT 'MESSAGE_ID = 5151 already exits in SGS_MESSAGES tables' 
ELSE 
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 

VALUES (5151, 'An Approved Withdrawal Application for this Person and Plan already exists.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)

GO


declare @currentdatetime datetime 
set @currentdatetime = getdate() 
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5152) 
PRINT 'MESSAGE_ID = 5152 already exits in SGS_MESSAGES tables' 
ELSE 
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 

VALUES (5152, 'An Approved Disability Application for this Person and Plan already exists.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)

GO