-- =============================================
-- Script Template
-- =============================================
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11/20/2011
-- Purpose - CHANGES TO ELIGBILITY RULES
----------------------------------------------------------------------------------------------------------------------------------
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=55 
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=36
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=58
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=44
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=45
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=39
UPDATE SGT_BENEFIT_PROVISION_ELIGIBILITY set MAX_AGE=65 where BENEFIT_PROVISION_ELIGIBILITY_ID=64


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 11/21/2011
-- Purpose - Adding message_id-5104 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5104)
PRINT 'MESSAGE_ID = 5104 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5104, 'SSD Award Letter Date is required', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11/21/2011
-- Purpose - ADDING A NEW MESSAGE FOR ELIGIBILITY
----------------------------------------------------------------------------------------------------------------------------------
declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5105)
PRINT 'MESSAGE_ID = 5105 already exits in SGS_MESSAGES tables'
ELSE
INSERT dbo.sgs_messages ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5105,'Participant is vested under Pension and IAP; both plans must be processed simultaneously.',16,'E','','',16,'','asharma',GETDATE(),'asharma',GETDATE(),0)
GO



		----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 11/22/2011
-- Purpose - INSERT MESSAGES IN SGS_MESSAGE TABLE
----------------------------------------------------------------------------------------------------------------------------------


INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5106,'Cannot delete Role, User is associated with this role.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:08:17:147PM','gagan','Nov 22 2011  4:08:17:147PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5107,'Activity Profile must not set for Document Type Action "Resume or Never Initiate',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:08:39:087PM','gagan','Nov 22 2011  4:08:39:087PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5110,'Role must be selected.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:08:54:810PM','gagan','Nov 22 2011  4:08:54:810PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5111,'End date can not be less than start date.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:09:11:260PM','gagan','Nov 22 2011  4:09:11:260PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5112,'Start Date cannot be past date.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:09:25:560PM','gagan','Nov 22 2011  4:09:25:560PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5113,'Start Date is required.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:09:56:357PM','gagan','Nov 22 2011  4:09:56:357PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5114,'You cannot modify your own record.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:10:10:470PM','gagan','Nov 22 2011  4:10:10:470PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5115,'The Role already Exists.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:10:21:430PM','gagan','Nov 22 2011  4:10:21:430PM',0)
INSERT dbo.SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(5117,'Date cannot be less than already entered start date.',16,'E',NULL,NULL,16,NULL,'gagan','Nov 22 2011  4:10:31:860PM','gagan','Nov 22 2011  4:10:31:860PM',0)

---------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 11/22/2011
-- Purpose - UPDATED SGS_COR_TEMPLATE Added Retiement Counselling correspondence for Person as well.
----------------------------------------------------------------------------------------------------------------------------------
update sgs_cor_templates set ASSOCIATED_FORMS = 'wfmRetirementApplicationMaintenance;wfmPersonMaintenance;;'
where template_id = 9

update sgs_cor_templates set TEMPLATE_DESC = 'Retirement Counseling Summary and Pending Items-FINAL'
where template_id = 26
