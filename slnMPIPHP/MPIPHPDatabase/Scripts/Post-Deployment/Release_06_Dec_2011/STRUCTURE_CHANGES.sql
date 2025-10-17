ALTER TABLE dbo.SGT_PERSON_ACCOUNT ADD SPECIAL_ACCOUNT UDT_FLAG;
ALTER TABLE dbo.SGT_PERSON_ACCOUNT ADD UVHP UDT_FLAG;
ALTER TABLE dbo.SGT_PERSON_ACCOUNT ADD EE_CONTR UDT_FLAG;

ALTER TABLE dbo.SGT_PERSON_ACCOUNT_ELIGIBILITY ADD PENSION_CREDITS UDT_RATE;

---------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/06/2011
-- Purpose - Added SSA_APPLICATION_DATE in to SGT_BENEFIT_APPLICATION
---------------------------------------------------------------------------------------------------------------------------------------


ALTER TABLE SGT_BENEFIT_APPLICATION
ADD SSA_APPLICATION_DATE UDT_DATETIME


--------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/06/2011
-- Purpose - Removed DISABILITY_RETIREMENT_DATE,RETIREMENT_APPLICATION_RECEIVED_DATE from SGT_BENEFIT_APPLICATION_DETAIL
--------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
DROP COLUMN DISABILITY_RETIREMENT_DATE

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
DROP COLUMN RETIREMENT_APPLICATION_RECEIVED_DATE