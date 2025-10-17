-- =============================================
-- Script Template
-- =============================================

---------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 11/28/2011
-- Purpose - UPDATED SGS_COR_TEMPLATE
----------------------------------------------------------------------------------------------------------------------------------
/****** Object:  Table dbo.SGS_COR_TEMPLATES  ********/
INSERT dbo.SGS_COR_TEMPLATES ([TEMPLATE_NAME],[TEMPLATE_DESC],[TEMPLATE_GROUP_ID],[TEMPLATE_GROUP_VALUE],[ACTIVE_FLAG],[DESTINATION_ID],[DESTINATION_VALUE],[ASSOCIATED_FORMS],[FILTER_OBJECT_ID],[FILTER_OBJECT_FIELD],[FILTER_OBJECT_VALUE],[CONTACT_ROLE_ID],[CONTACT_ROLE_VALUE],[BATCH_FLAG],[ONLINE_FLAG],[AUTO_PRINT_FLAG],[PRINTER_NAME_ID],[PRINTER_NAME_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[IMAGE_DOC_CATEGORY_ID],[IMAGE_DOC_CATEGORY_VALUE],[FILENET_DOCUMENT_TYPE_ID],[FILENET_DOCUMENT_TYPE_VALUE],[DOC_TYPE])VALUES('PER-0003','New Participant Letter',19,'MMBR','Y',0,NULL,'wfmPersonMaintenance;',NULL,NULL,NULL,6011,NULL,'N','Y','N',44,NULL,'gagan.dhamija','Nov 28 2011  1:09:37:967PM','gagan.dhamija','Nov 28 2011  1:09:37:967PM',0,0,NULL,0,NULL,NULL)
INSERT dbo.SGS_COR_TEMPLATES ([TEMPLATE_NAME],[TEMPLATE_DESC],[TEMPLATE_GROUP_ID],[TEMPLATE_GROUP_VALUE],[ACTIVE_FLAG],[DESTINATION_ID],[DESTINATION_VALUE],[ASSOCIATED_FORMS],[FILTER_OBJECT_ID],[FILTER_OBJECT_FIELD],[FILTER_OBJECT_VALUE],[CONTACT_ROLE_ID],[CONTACT_ROLE_VALUE],[BATCH_FLAG],[ONLINE_FLAG],[AUTO_PRINT_FLAG],[PRINTER_NAME_ID],[PRINTER_NAME_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[IMAGE_DOC_CATEGORY_ID],[IMAGE_DOC_CATEGORY_VALUE],[FILENET_DOCUMENT_TYPE_ID],[FILENET_DOCUMENT_TYPE_VALUE],[DOC_TYPE])VALUES('RETR-0010','Retirement Application Request Form',19,'MMBR','Y',0,NULL,'wfmRetirementApplicationMaintenance;',NULL,NULL,NULL,6011,NULL,'N','Y','N',44,NULL,'gagan.dhamija','Nov 28 2011  1:10:13:707PM','gagan.dhamija','Nov 28 2011  1:10:13:707PM',0,0,NULL,0,NULL,NULL)


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11/25/2011
-- Purpose - Adding message_id-5118 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------
declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5118)
PRINT 'MESSAGE_ID = 5118 already exits in SGS_MESSAGES tables'
ELSE
INSERT dbo.sgs_messages ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(5118,'Participant is not Eligible anymore in the selected plan(s).Please modify choosen Benefit Details',16,'E',NULL,NULL,16,NULL,'abhishek',GETDATE(),'abhishek',GETDATE(),0)
GO

