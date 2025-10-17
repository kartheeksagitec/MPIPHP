
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/09/2011
-- Purpose - Removed message_id- 5119,5120,5128,5129 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

delete from SGS_MESSAGES where MESSAGE_ID=5119

delete from SGS_MESSAGES where MESSAGE_ID=5120

delete from SGS_MESSAGES where MESSAGE_ID=5128

delete from SGS_MESSAGES where MESSAGE_ID=5129
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 12/09/2011
-- Purpose - Added message_id-1156 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------
declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1156)
PRINT 'MESSAGE_ID =1156 already exits in SGS_MESSAGES tables '
ELSE
INSERT dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (1156, 'Zip code should be numeric.', 16, N'E', N'NULL', N'NULL', 16, N'NULL','kunal', @currentdatetime, N'kunal', @currentdatetime, 0)
GO

--------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/09/2011
-- Purpose - added Message in sgt_message for a hard Error
----------------------------------------------------------------------------------------------------------------------------------

INSERT INTO [SGS_MESSAGES]
          ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[RESPONSIBILITY_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
           VALUES (5142,'Start Date cannot be Future Date.',16,'E',16,'Gagan',GETDATE(),'Gagan',GETDATE(),0)
GO

