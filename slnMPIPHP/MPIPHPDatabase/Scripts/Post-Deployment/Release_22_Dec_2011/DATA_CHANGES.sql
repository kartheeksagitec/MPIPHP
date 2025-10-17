----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/22/2011
-- Purpose - Added message_id- 1163 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1163)
PRINT 'MESSAGE_ID = 1163 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1163,'Please enter address for this person or change the Communication preference to blank.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1164)
PRINT 'MESSAGE_ID = 1164 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1164,'Start date already exist.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1165)
PRINT 'MESSAGE_ID = 1165 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1165,'You are not allowed to choose DRO Model Standard Post Death QDRO because there is already an active DRO.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO