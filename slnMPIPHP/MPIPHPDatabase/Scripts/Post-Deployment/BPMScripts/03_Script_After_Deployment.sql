------------------------------------------------------------------Return to work map event--------------------------------------------------------------
IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_EVENT] WHERE EVENT_DESC = 'RETR-006 Document Received Notification'))
BEGIN 
INSERT INTO [dbo].[SGW_BPM_EVENT] ([EVENT_DESC],[EVENT_TYPE_ID],[EVENT_TYPE_VALUE],[RCPT_EMAIL_ID],[RCPT_FAX_NU],[DOC_TYPE],[SCREEN_ID]
      ,[PRIORITY_DOCUMENT_IND],[ECM_SECURITY_TEMPLATE_ID],[ECM_SUBSCRIPTION_FLAG],[STATUS_ID],[STATUS_VALUE]
      ,[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[DOCUMENT_CATEGORY],[ORG_ID_REQUIRED_IND],[PERSON_ID_REQUIRED_IND],[DOC_CLASS])
VALUES ('RETR-006 Document Received Notification',2010,'DOC',NULL,NULL,'RETR0006',NULL,
		NULL,NULL,'',2009,'ACTV',
		'BPM',GETDATE(),'BPM',GETDATE(),0,NULL,NULL,NULL,'RETR0006')
END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_EVENT] already contains row with EVENT_DESC = RETR-006 Document Received Notification');
END
GO

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_PROCESS_EVENT_XR] WHERE ACTIVITY_ID=(SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Document Received Notification')))
BEGIN 
INSERT INTO [dbo].[SGW_BPM_PROCESS_EVENT_XR] ([EVENT_ID],[PROCESS_ID],[ACTION_ID],[ACTION_VALUE]
      ,[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[EVENT_REASON_ID],[ACTIVITY_ID])
VALUES ((select BPM_EVENT_ID FROM  SGW_BPM_EVENT where EVENT_DESC='RETR-006 Document Received Notification'), (SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE DESCRIPTION='Return To Work Process'),2005,'RENE',
		'BPM',GETDATE(),'BPM',GETDATE(),0,NULL,(SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Document Received Notification'))

END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_PROCESS_EVENT_XR] already contains row with ACTIVITY_ID');
END
Go

---------------------------------------------------------service retirement map event------------------------------------------------------------------------
IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_EVENT] WHERE EVENT_DESC = 'Wait For Application Form'))
BEGIN 
INSERT [dbo].[SGW_BPM_EVENT] ([EVENT_DESC], [EVENT_TYPE_ID], [EVENT_TYPE_VALUE], [RCPT_EMAIL_ID], [RCPT_FAX_NU], [DOC_TYPE], [SCREEN_ID], [PRIORITY_DOCUMENT_IND], [ECM_SECURITY_TEMPLATE_ID], [ECM_SUBSCRIPTION_FLAG], [STATUS_ID], [STATUS_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [DOCUMENT_CATEGORY], [ORG_ID_REQUIRED_IND], [PERSON_ID_REQUIRED_IND], [DOC_CLASS]) VALUES (N'Wait For Application Form', 2010, N'DOC', NULL, NULL, N'SR-Wait For Application Form', NULL, NULL, NULL, N'', 2009, N'ACTV', 'BPM', GETDATE(), 'BPM',GETDATE(), 0, NULL, NULL, NULL, N'SR-Wait For Application Form')
END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_EVENT] already contains row with EVENT_DESC = Wait For Application Form');
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_EVENT] WHERE EVENT_DESC = 'Wait for Election Packet'))
BEGIN 
INSERT [dbo].[SGW_BPM_EVENT] ([EVENT_DESC], [EVENT_TYPE_ID], [EVENT_TYPE_VALUE], [RCPT_EMAIL_ID], [RCPT_FAX_NU], [DOC_TYPE], [SCREEN_ID], [PRIORITY_DOCUMENT_IND], [ECM_SECURITY_TEMPLATE_ID], [ECM_SUBSCRIPTION_FLAG], [STATUS_ID], [STATUS_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [DOCUMENT_CATEGORY], [ORG_ID_REQUIRED_IND], [PERSON_ID_REQUIRED_IND], [DOC_CLASS]) VALUES (N'Wait for Election Packet', 2010, N'DOC', NULL, NULL, N'SR-Wait for Election Packet', NULL, NULL, NULL, N'', 2009, N'ACTV', 'BPM',GETDATE(), 'BPM', GETDATE(), 0, NULL, NULL, NULL, N'SR-Wait for Election Packet')
END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_EVENT] already contains row with EVENT_DESC = Wait for Election Packet');
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_EVENT] WHERE EVENT_DESC = 'Cancellation Notice Received'))
BEGIN 
INSERT [dbo].[SGW_BPM_EVENT] ([EVENT_DESC], [EVENT_TYPE_ID], [EVENT_TYPE_VALUE], [RCPT_EMAIL_ID], [RCPT_FAX_NU], [DOC_TYPE], [SCREEN_ID], [PRIORITY_DOCUMENT_IND], [ECM_SECURITY_TEMPLATE_ID], [ECM_SUBSCRIPTION_FLAG], [STATUS_ID], [STATUS_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [DOCUMENT_CATEGORY], [ORG_ID_REQUIRED_IND], [PERSON_ID_REQUIRED_IND], [DOC_CLASS]) VALUES (N'Cancellation Notice Received', 2010, N'DOC', NULL, NULL, N'SR-Cancellation Notice Received', NULL, NULL, NULL, N'', 2009, N'ACTV', 'BPM', GETDATE(), 'BPM',GETDATE(), 0, NULL, NULL, NULL, N'SR-Cancellation Notice Received')
END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_EVENT] already contains row with EVENT_DESC = Cancellation Notice Received');
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_PROCESS_EVENT_XR] WHERE ACTIVITY_ID=  (SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Document Received')))
BEGIN 
INSERT [dbo].[SGW_BPM_PROCESS_EVENT_XR] ( [EVENT_ID], [PROCESS_ID], [ACTION_ID], [ACTION_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [EVENT_REASON_ID], [ACTIVITY_ID]) VALUES ((select BPM_EVENT_ID FROM  SGW_BPM_EVENT where EVENT_DESC='Wait For Application Form'), (SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE DESCRIPTION='Service Retirement Process'), 2005, N'RENE', 'BPM', GETDATE(), 'BPM', GETDATE(), 0, NULL,  (SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Document Received'))

END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_PROCESS_EVENT_XR] already contains row with ACTIVITY_ID');
END
Go
------------------------------------------------------------------------------------------------------------------------------------------------------

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_PROCESS_EVENT_XR] WHERE ACTIVITY_ID=  (SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Election Packet received')))
BEGIN 
INSERT [dbo].[SGW_BPM_PROCESS_EVENT_XR] ( [EVENT_ID], [PROCESS_ID], [ACTION_ID], [ACTION_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [EVENT_REASON_ID], [ACTIVITY_ID]) VALUES ((select BPM_EVENT_ID FROM  SGW_BPM_EVENT where EVENT_DESC='Wait for Election Packet'), (SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE DESCRIPTION='Service Retirement Process'), 2005, N'RENE', 'BPM', GETDATE(), 'BPM', GETDATE(), 0, NULL,  (SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Election Packet received'))

END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_PROCESS_EVENT_XR] already contains row with ACTIVITY_ID');
END
GO
------------------------------------------------------------------------------------------------------------------------------------------------------

IF(NOT EXISTS(SELECT 1 FROM [dbo].[SGW_BPM_PROCESS_EVENT_XR] WHERE ACTIVITY_ID=(SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Cancellation Notice received' and PROCESS_ID=(select PROCESS_ID from SGW_BPM_PROCESS where NAME='Cancel Application'))))
BEGIN 
INSERT [dbo].[SGW_BPM_PROCESS_EVENT_XR] ( [EVENT_ID], [PROCESS_ID], [ACTION_ID], [ACTION_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ], [EVENT_REASON_ID], [ACTIVITY_ID]) VALUES ((select BPM_EVENT_ID FROM  SGW_BPM_EVENT where EVENT_DESC='Cancellation Notice Received'), (SELECT PROCESS_ID FROM SGW_BPM_PROCESS WHERE DESCRIPTION='Cancel Application'), 2005, N'RENE', 'BPM', GETDATE(), 'BPM', GETDATE(), 0, NULL,  
(SELECT ACTIVITY_ID FROM SGW_BPM_ACTIVITY WHERE Name = 'Cancellation Notice received' and PROCESS_ID=(select PROCESS_ID from SGW_BPM_PROCESS where NAME='Cancel Application')))
END
ELSE
BEGIN
	PRINT('[dbo].[SGW_BPM_PROCESS_EVENT_XR] already contains row with ACTIVITY_ID');
END
Go
-----------------------------------------------------------------------------------------------------------------------------------------------------