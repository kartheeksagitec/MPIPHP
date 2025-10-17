-- =============================================
-- Schema Changes TEmplate
-- =============================================
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 11164/2011
-- Purpose - changing the datatype to the right datatype for some columns
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE SGT_PERSON_ACCOUNT_ELIGIBILITY ALTER COLUMN LAST_EVALUATED_DATE UDT_DATETIME NULL


ALTER TABLE SGT_PERSON_ACCOUNT_ELIGIBILITY ALTER COLUMN FORFEITURE_DATE UDT_DATETIME NULL


ALTER TABLE SGT_PERSON_ACCOUNT_ELIGIBILITY ALTER COLUMN VESTED_DATE UDT_DATETIME NULL

ALTER TABLE SGT_PERSON_ACCOUNT_ELIGIBILITY ALTER COLUMN VESTING_RULE_ID UDT_ID NULL


 EXEC sp_rename 'SGT_PERSON_ACCOUNT_ELIGIBILITY.VESTING_RULE_ID', 'VESTING_RULE', 'COLUMN'

ALTER TABLE SGT_PERSON_ACCOUNT_ELIGIBILITY ALTER COLUMN VESTING_RULE UDT_COMMENTS NULL