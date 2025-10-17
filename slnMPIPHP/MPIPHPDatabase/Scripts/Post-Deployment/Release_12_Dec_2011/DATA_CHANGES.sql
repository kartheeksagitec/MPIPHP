
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 12/12/2011
-- Purpose - added the entries for the Disability and Withdrawal and Pre-Retirement Death WorkFlow
----------------------------------------------------------------------------------------------------------------------------------

INSERT dbo.sgw_process ([PROCESS_ID],[DESCRIPTION],[NAME],[PRIORITY],[TYPE_ID],[TYPE_VALUE],[STATUS_ID],[STATUS_VALUE],[USE_NEW_MAP_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(9,'Process Disability Application','nfmProcessDisabilityApplication',0,1603,'PERS',5003,'ACT',NULL,'Studio','Dec 12 2011 12:56:42:087PM','Studio','Dec 12 2011 12:56:42:087PM',0)
INSERT dbo.sgw_activity ([ACTIVITY_ID],[PROCESS_ID],[NAME],[DISPLAY_NAME],[STANDARD_TIME_IN_MINUTES],[ROLE_ID],[SUPERVISOR_ROLE_ID],[SORT_ORDER],[IS_DELETED_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[ALLOW_INDEPENDENT_COMPLETE_IND])VALUES(9,9,'Enter/Update Disability Application','Enter/Update Disability Application',0,75,66,1,NULL,'Studio','Dec 12 2011 12:56:42:150PM','Studio','Dec 12 2011 12:56:42:150PM',0,'Y')


INSERT dbo.sgw_process ([PROCESS_ID],[DESCRIPTION],[NAME],[PRIORITY],[TYPE_ID],[TYPE_VALUE],[STATUS_ID],[STATUS_VALUE],[USE_NEW_MAP_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])VALUES(10,'Process Withdrawal Application','nfmProcessWithdrawalApplication',0,1603,'PERS',5003,'ACT',NULL,'Studio','Dec 12 2011  2:54:03:020PM','Studio','Dec 12 2011  2:54:03:020PM',0)
INSERT dbo.sgw_activity ([ACTIVITY_ID],[PROCESS_ID],[NAME],[DISPLAY_NAME],[STANDARD_TIME_IN_MINUTES],[ROLE_ID],[SUPERVISOR_ROLE_ID],[SORT_ORDER],[IS_DELETED_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[ALLOW_INDEPENDENT_COMPLETE_IND])VALUES(10,10,'Enter/Update Withdrawal Application','Enter/Update Withdrawal Application',20,75,66,1,NULL,'Studio','Dec 12 2011  2:54:03:040PM','abhishek.sharma','Dec 12 2011  3:03:33:083PM',1,'Y')



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/12/2011
-- Purpose - Added message_id- 5143 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5143)
PRINT 'MESSAGE_ID = 5143 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5143, 'This date should be greater than previous date.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO
