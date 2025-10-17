----------------------------------------------------------------------------------

--Name - Abhishek Sharma
--Date - 15 Dec 2011
--Purpose - To Accomodate the Change in Eligibility

----------------------------------------------------------------------------------

alter table SGT_BENEFIT_PROVISION_ELIGIBILITY
drop column EFFECTIVE_DATE_ID


alter table SGT_BENEFIT_PROVISION_ELIGIBILITY
drop column EFFECTIVE_DATE_VALUE

ALTER TABLE SGT_BENEFIT_PROVISION_ELIGIBILITY
ADD  EFFECTIVE_YEAR UDT_ID
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 12/15/2011
-- Purpose - add ress history added source desc ,removed id value pair(desc to be directly coming from convrsn)
----------------------------------------------------------------------------------------------------------------------------------
alter table dbo.SGT_PERSON_ADDRESS_HISTORY add ADDR_SOURCE_DESC varchar(250) null

ALTER TABLE SGT_PERSON_ADDRESS_HISTORY
DROP COLUMN ADDR_SOURCE_ID

ALTER TABLE SGT_PERSON_ADDRESS_HISTORY
DROP COLUMN ADDR_SOURCE_VALUE

