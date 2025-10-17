

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 12/12/2011
-- Purpose - added the entries for the  Pre-Retirement Death WorkFlow
----------------------------------------------------------------------------------------------------------------------------------

INSERT dbo.sgw_process ([PROCESS_ID],[DESCRIPTION],[NAME],[PRIORITY],[TYPE_ID],[TYPE_VALUE],[STATUS_ID],[STATUS_VALUE],[USE_NEW_MAP_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(11,'Process PreRetirement DeathApplication','nfmProcessPreRetirementDeathApplication',0,1603,'PERS',5003,'ACT',NULL,'Studio','Dec 12 2011  4:12:56:793PM','Studio','Dec 12 2011  4:12:56:793PM',0)
INSERT dbo.sgw_activity ([ACTIVITY_ID],[PROCESS_ID],[NAME],[DISPLAY_NAME],[STANDARD_TIME_IN_MINUTES],[ROLE_ID],[SUPERVISOR_ROLE_ID],[SORT_ORDER],[IS_DELETED_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[ALLOW_INDEPENDENT_COMPLETE_IND])VALUES(11,11,'Enter/Update Pre-Retirement Survivor Benefit-Election','Enter/Update Pre-Retirement Survivor Benefit-Election',20,75,66,1,NULL,'Studio','Dec 12 2011  4:12:56:803PM','abhishek.sharma','Dec 12 2011  4:16:48:690PM',1,'Y')


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 13/12/2011
-- Purpose - added the message id 1156 & 1157
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1157)
PRINT 'MESSAGE_ID =1157 already exits in SGS_MESSAGES tables '
ELSE
INSERT dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (1157, 'Work Phone Length cannot be less than 10 digits.', 16, N'E', N'NULL', N'NULL', 16, N'NULL','kunal', @currentdatetime, N'kunal', @currentdatetime, 0)
GO

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 1158)
PRINT 'MESSAGE_ID =1158 already exits in SGS_MESSAGES tables '
ELSE
INSERT dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (1158, 'Home Phone Length cannot be less than 10 digits.', 16, N'E', N'NULL', N'NULL', 16, N'NULL','kunal', @currentdatetime, N'kunal', @currentdatetime, 0)
GO