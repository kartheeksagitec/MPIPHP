----------------------------------------------------------------------------------

--Name - Abhishek Sharma
--Date - 15 Dec 2011
--Purpose - To Accomodate the Change in Eligibility

----------------------------------------------------------------------------------
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=1953 where ELIGIBILITY_TYPE_VALUE='R1'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=1976 where ELIGIBILITY_TYPE_VALUE='R2'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=1989 where ELIGIBILITY_TYPE_VALUE='R3A'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=1989 where ELIGIBILITY_TYPE_VALUE='R3B'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=1990 where ELIGIBILITY_TYPE_VALUE='R4'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=2000 where ELIGIBILITY_TYPE_VALUE='R5'

UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY SET EFFECTIVE_YEAR=2000 where ELIGIBILITY_TYPE_VALUE='R6'


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/12/2011
-- Purpose - Added message_id- 5147,5148 and updated message-id-5123 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5147)
PRINT 'MESSAGE_ID = 5147 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5147, 'An Approved Pre Death Retirement Application for this Person already exists.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO


declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5148)
PRINT 'MESSAGE_ID = 5148 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5148, 'An Approved Post Death Retirement Application for this Person already exists.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO

update SGS_MESSAGES set DISPLAY_MESSAGE='An Approved Retirement Application for this Person already exists.' where MESSAGE_ID=5123