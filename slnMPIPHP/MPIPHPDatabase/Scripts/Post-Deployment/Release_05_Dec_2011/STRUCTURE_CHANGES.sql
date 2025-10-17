-- =============================================
-- Schema Changes TEmplate
-- =============================================
--------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 12/01/2011
-- Purpose - Start Date made null. 
----------------------------------------------------------------------------------------------------------------------------------

alter table dbo.SGT_PERSON_ACCOUNT_BENEFICIARY 
alter column START_DATE UDT_DATETIME NULL


--------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/05/2011
-- Purpose - Removed ssd_award_letter_date,awarded_on_date from SGT_BENEFIT_APPLICATION
--------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_BENEFIT_APPLICATION
 DROP COLUMN ssd_award_letter_date

alter table sgt_benefit_application drop column awarded_on_date

---------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/05/2011
-- Purpose - Added RETIREMENT_APPLICATION_RECEIVED_DATE,DISABILITY_RETIREMENT_DATE,AWARDED_ON_DATE in to SGT_BENEFIT_APPLICATION_DETAIL
---------------------------------------------------------------------------------------------------------------------------------------

alter table [SGT_BENEFIT_APPLICATION_DETAIL] add RETIREMENT_APPLICATION_RECEIVED_DATE UDT_DATETIME

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD DISABILITY_RETIREMENT_DATE UDT_DATETIME

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
ADD AWARDED_ON_DATE UDT_DATETIME