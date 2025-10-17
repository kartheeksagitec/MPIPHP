----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 12/28/2011
-- Purpose - making changes to the SGT_PLAN_BENEFIT_XR Table
----------------------------------------------------------------------------------------------------------------------------------

INSERT dbo.sgt_plan_benefit_xr ([PLAN_ID],[BENEFIT_OPTION_ID],[BENEFIT_OPTION_VALUE],[RETIREMENT_FLAG],[WITHDRAWAL_FLAG],[DISABILITY_FLAG],[DEATH_PRE_RETIREMENT_FLAG],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(1,1504,'5YLA','N','Y','N','N','asharma',GETDATE(),'asharma',GETDATE(),0)

declare @currentdatetime datetime
set @currentdatetime = getdate()  
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5155)
PRINT 'MESSAGE_ID = 5155 already exits in SGS_MESSAGES tables'
ELSE
INSERT dbo.sgs_messages ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(5155,'Not Eligible for Pre-Retirement Death.',16,'E',NULL,NULL,16,NULL,'asharma',GETDATE(),'asharma',GETDATE(),0)
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/28/2011
-- Purpose - Added template in SGS_COR_TEMPLATES table.
----------------------------------------------------------------------------------------------------------------------------------

IF EXISTS (SELECT TEMPLATE_NAME FROM SGS_COR_TEMPLATES WHERE TEMPLATE_DESC = 'QDRO Request Form')
PRINT 'TEMPLATE = QDRO Request Form already exits in SGS_COR_TEMPLATES tables'
ELSE
INSERT dbo.SGS_COR_TEMPLATES ([TEMPLATE_NAME],[TEMPLATE_DESC],[TEMPLATE_GROUP_ID],[TEMPLATE_GROUP_VALUE],[ACTIVE_FLAG],[DESTINATION_ID],[DESTINATION_VALUE],[ASSOCIATED_FORMS],[FILTER_OBJECT_ID],[FILTER_OBJECT_FIELD],[FILTER_OBJECT_VALUE],[CONTACT_ROLE_ID],[CONTACT_ROLE_VALUE],[BATCH_FLAG],[ONLINE_FLAG],[AUTO_PRINT_FLAG],[PRINTER_NAME_ID],[PRINTER_NAME_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ],[IMAGE_DOC_CATEGORY_ID],[IMAGE_DOC_CATEGORY_VALUE],[FILENET_DOCUMENT_TYPE_ID],[FILENET_DOCUMENT_TYPE_VALUE],[DOC_TYPE])
VALUES('DRO-0006','QDRO Request Form',19,'MMBR','Y',0,NULL,'wfmQDROApplicationMaintenance;',NULL,NULL,NULL,6011,NULL,'N','Y','N',44,NULL,'gagan.dhamija','Dec 28 2011  1:10:13:707PM','gagan.dhamija','Dec 28 2011  1:10:13:707PM',0,0,NULL,0,NULL,NULL)