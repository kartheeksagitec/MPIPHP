-- =============================================
-- Schema Changes TEmplate
-- =============================================


----------------------------------------------------------------------------------------------------------------------------------
-- Name -Rohan Adgaonkar
-- Date - 29/11/2011
-- Purpose - Adding the columns in to SGW_WORKFLOW_REQUEST 
----------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGW_WORKFLOW_REQUEST
ADD TRACKING_ID INT NULL


ALTER TABLE SGW_WORKFLOW_REQUEST
ADD APP_NAME VARCHAR(500) NULL

ALTER TABLE SGW_WORKFLOW_REQUEST
ADD TIMESTAMP DATETIME NULL


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 11/29/2011
-- Purpose - Adding the columns in to SGT_BENEFIT_APPLICATION_DETAIL 
----------------------------------------------------------------------------------------------------------------------------------

alter table [SGT_BENEFIT_APPLICATION_DETAIL] add RETIREMENT_APPLICATION_RECEIVED_DATE UDT_DATETIME

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD DISABILITY_RETIREMENT_DATE UDT_DATETIME

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD AWARDED_ON_DATE UDT_DATETIME



----------------------------------------------------------------------------------------------------------------------------------
-- Name -Rohan Adgaonkar
-- Date - 29/11/2011
----------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE SGW_WORKFLOW_REQUEST
DROP COLUMN APP_NAME

ALTER TABLE SGT_DOCUMENT
ADD APP_NAME VARCHAR(500) NULL

ALTER TABLE SGT_DOCUMENT
ALTER COLUMN DOC_TYPE UDT_DATA50 NULL