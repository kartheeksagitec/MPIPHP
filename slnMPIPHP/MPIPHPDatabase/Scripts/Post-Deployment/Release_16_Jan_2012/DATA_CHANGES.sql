-- =============================================
-- Script Template
-- =============================================
---------------------------------------------------------------------------------------------------------------------------------
-- Name - Puneet Punjabi
-- Date - 01/16/2012
-- Purpose - Added SGS_MESSAGES for QDRO App with Message_Id = 1169
----------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 5174)
BEGIN
	PRINT 'Adding Message Id 5174 - Entry Order date is required.'
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,RESPONSIBILITY_ID,
	RESPONSIBILITY_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES (5174, 'Entry of Order Date is required.', 16, 'E', NULL, NULL, 16, NULL, 'ppunjabi', GETDATE(), 'ppunjabi', GETDATE(), 0)
END
GO

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 5175)
BEGIN
	PRINT 'Adding Message Id 5175 - Entry Order date cannot be greater than current date.'
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,RESPONSIBILITY_ID,
	RESPONSIBILITY_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES (5175, 'Entry Order date cannot be greater than current date.', 16, 'E', NULL, NULL, 16, NULL, 'ppunjabi', GETDATE(), 'ppunjabi', GETDATE(), 0)
END
GO

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 5176)
BEGIN
	PRINT 'Adding Message Id 5176 - Entry Order date cannot be less than Date of Separation.'
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,RESPONSIBILITY_ID,
	RESPONSIBILITY_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES (5176, 'Entry Order date cannot be less than Date of Separation.', 16, 'E', NULL, NULL, 16, NULL, 'ppunjabi', GETDATE(), 'ppunjabi', GETDATE(), 0)
END
GO


IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 5177)
BEGIN
	PRINT 'Adding Message Id 5177 - Beneficiary End Date cannot be a Future Date.'
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,RESPONSIBILITY_ID,
	RESPONSIBILITY_VALUE,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
	VALUES (5177, 'Beneficiary End Date cannot be a Future Date.', 16, 'E', NULL, NULL, 16, NULL, 'ppunjabi', GETDATE(), 'ppunjabi', GETDATE(), 0)
END
GO


---------------------------------------------------------------------------------------------------------------------------------
-- Name - Vinovin P Thomas
-- Date - 01/16/2012
-- Purpose - 
----------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID = 5173)
BEGIN
	PRINT 'Adding Message Id 5173 - Please enter Withdrawal date as first of the month following Application Received date'
	insert into sgs_messages
	values (5173,'Please enter Withdrawal date as first of the month following Application Received date', 16,'E',null,null,16,null,'vthomas',getdate(),'vthomas',getdate(),0)
END
GO


update sgs_code
set data1_caption = 'Type',data1_type = 'str',modified_by = 'vthomas',modified_date = getdate()
where code_id = 6000

update sgs_code_value
set description = 'Trust',modified_by = 'vthomas',modified_date = getdate()
where CODE_ID = 6000 AND CODE_VALUE = 'TRST'

update sgs_messages
set display_message = 'Overlapping bridging service dates are not allowed for the same Type of Hours', modified_by = 'vthomas',modified_date = getdate()
where message_id = 5141
