-- =============================================
-- Schema Changes TEmplate
-- =============================================

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11/03/2011
-- Purpose - Adding the columns in to SGT_DISABLILITY APPLICATION to accomodates new changes in Test Case
----------------------------------------------------------------------------------------------------------------------------------

alter table SGT_BENEFIT_APPLICATION
add ENTITLEMENT_DATE UDT_DATETIME
GO

alter table SGT_BENEFIT_APPLICATION
add DISABILITY_CONVERSION_DATE UDT_DATETIME
GO

alter table SGT_BENEFIT_APPLICATION
add AWARDED_ON_DATE UDT_DATETIME
GO

ALTER TABLE SGT_DISABILITY_BENEFIT_HISTORY
ADD PLAN_ID UDT_ID not null
GO



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Jeevan Awatramani
-- Date - 11/03/2011
-- Purpose - Some Data Stuff and Schema changes 
----------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [SGT_PERSON_ADDRESS_CHKLIST]  WITH CHECK ADD  CONSTRAINT [FK_SGT_PERSON_ADDRESS_CHKLIST_ADDRESS_ID_SGT_PERSON_ADDRESS_ADDRESS_ID] FOREIGN KEY([ADDRESS_ID])
REFERENCES [SGT_PERSON_ADDRESS] ([ADDRESS_ID])
GO

ALTER TABLE [SGT_PERSON_ACCOUNT] WITH CHECK ADD CONSTRAINT [FK_SGT_PERSON_ACCOUNT_PLAN_ID_SGT_PLAN_PLAN_ID] FOREIGN KEY([PLAN_ID])
REFERENCES [SGT_PLAN] ([PLAN_ID])
GO

ALTER TABLE [SGT_DRO_BENEFIT_DETAILS] WITH CHECK ADD CONSTRAINT [FK_SGT_DRO_BENEFIT_DETAILS_PLAN_ID_SGT_PLAN_PLAN_ID] FOREIGN KEY([PLAN_ID])
REFERENCES [SGT_PLAN] ([PLAN_ID])
GO

ALTER TABLE [SGT_PERSON_ADDRESS_HISTORY]
add ADDR_SOURCE_ID UDT_ID
GO

ALTER TABLE [SGT_PERSON_ADDRESS_HISTORY]
add ADDR_SOURCE_VALUE UDT_CODE_VALUE
GO

EXEC sp_rename 'SGT_BENEFIT_APPLICATION.[TERMINALLY_ILL]', TERMINALLY_ILL_FLAG, 'COLUMN'
GO

ALTER TABLE [SGT_BENEFIT_APPLICATION]
DROP COLUMN LAST_EMPLOYER
GO

ALTER TABLE [SGT_PERSON_ADDRESS]
DROP COLUMN FOREIGN_ADDR_FLAG
GO

ALTER TABLE [SGT_PERSON_ADDRESS_HISTORY]
DROP COLUMN FOREIGN_ADDR_FLAG
GO

ALTER TABLE [SGT_PERSON_ADDRESS_HISTORY]
DROP COLUMN ADDRESS_TYPE_ID
GO

ALTER TABLE [SGT_PERSON_ADDRESS_HISTORY]
DROP COLUMN ADDRESS_TYPE_VALUE
GO

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 11/03/2011
-- Purpose - Added 'Benefit Provision ID' column in Plan Table 
----------------------------------------------------------------------------------------------------------------------------------

Alter Table [SGT_PLAN] add 
[BENEFIT_PROVISION_ID] [UDT_ID] NULL


ALTER TABLE [dbo].[SGT_PLAN]  WITH CHECK ADD  CONSTRAINT [FK_SGT_PLAN_SGT_BENEFIT_PROVISION] FOREIGN KEY([BENEFIT_PROVISION_ID])
REFERENCES [dbo].[SGT_BENEFIT_PROVISION] ([BENEFIT_PROVISION_ID])
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan 
-- Date - 11/04/2011
-- Purpose - Alter Table Scripts
----------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD SURVIVOR_ID UDT_ID

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD ORGANIZATION_ID UDT_ID

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD DEATH_NOTIFICATION_RECEIVED_DATE UDT_DATETIME

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD DEATH_NOTIFICATION_ID UDT_ID

ALTER TABLE SGT_DRO_APPLICATION
ADD CANCELLATION_REASON_ID UDT_ID

ALTER TABLE SGT_DRO_APPLICATION
ADD CANCELLATION_REASON_VALUE UDT_CODE_VALUE

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD CANCELLATION_REASON_ID UDT_ID

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD CANCELLATION_REASON_VALUE UDT_CODE_VALUE

exec sp_rename 'SGT_PLAN_BENEFIT_XR.DEATH_FLAG','DEATH_PRE_RETIREMENT_FLAG','COLUMN'

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11/07/2011
-- Purpose - DROPING AND ADDING COLUMNS SINCE BENEFIT_SUBTYPE IS NOT INSIDE SGT_BENEFIT_APPLICATION_DETAIL
----------------------------------------------------------------------------------------------------------------------------------

alter table sgt_benefit_application drop column benefit_subtype_id
alter table sgt_benefit_application drop column benefit_subtype_value

alter table sgt_benefit_application_detail add BENEFIT_SUBTYPE_ID UDT_ID
alter table sgt_benefit_application_detail add BENEFIT_SUBTYPE_VALUE UDT_CODE_VALUE


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 11/07/2011
-- Purpose - Add Column Reason_Description in SGT_DRO_APPLICATION AND SGT_BENEFIT_APPLICATION to save cancellation reason details 
---if cancellation reason is set to other 
----------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE SGT_DRO_APPLICATION
ADD REASON_DESCRIPTION UDT_DATA100

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD REASON_DESCRIPTION UDT_DATA100

EXEC sp_rename 'SGT_PLAN_BENEFIT_XR.DEATH_FLAG','DEATH_PRE_RETIREMENT_FLAG','COLUMN'



-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 11/14/2011
-- Purpose - Notification Change Date column in SGT_DEATH_NOTIFICATION (If status is changed to either Incorrectly Reported or Not Deceased this date will be populated with System Date)
---if cancellation reason is set to other 
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_DEATH_NOTIFICATION
ADD NOTIFICATION_CHANGE_DATE UDT_DATETIME NULL


