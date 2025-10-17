----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/26/2011
-- Purpose - updated spelling in code ID Value Pair in sgs_code_value.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_CODE_VALUE set DESCRIPTION ='Guardian' where CODE_VALUE = 'GRDN' and CODE_ID = 6023

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/26/2011
-- Purpose - Added message_id- 1166 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------
declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1166)
PRINT 'MESSAGE_ID = 1166 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1166,'Please select your communication preference.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO


declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1167)
PRINT 'MESSAGE_ID = 1167 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1167,'Date of death cannot be earlier than effective start date specified in contact.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1168)
PRINT 'MESSAGE_ID = 1168 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1168,'Date of death cannot be earlier than effective start date specified in beneficary.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1169)
PRINT 'MESSAGE_ID = 1169 already exits in SGS_MESSAGES tables'
ELSE
INSERT INTO dbo.SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES (1169,'Date of death cannot be earlier than effective start date specified in beneficary of.' ,16 ,'E' ,16 ,'gagan',GETDATE(),'gagan',GETDATE(),0)
GO