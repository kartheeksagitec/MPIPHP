

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/06/2011
-- Purpose - Changed text of message_id- 5046 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_MESSAGES set DISPLAY_MESSAGE='Application Received Date should be greater than Disability Onset Date.' where MESSAGE_ID=5046


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/06/2011
-- Purpose - Added message_id- 5130,5131 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5130)
PRINT 'MESSAGE_ID = 5130 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5130, 'Application Reveived date should be greater than SSA aaplication date.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO


declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5131)
PRINT 'MESSAGE_ID = 5131 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5131, 'SSA Application Date should be greater than Disability Onset Date.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO