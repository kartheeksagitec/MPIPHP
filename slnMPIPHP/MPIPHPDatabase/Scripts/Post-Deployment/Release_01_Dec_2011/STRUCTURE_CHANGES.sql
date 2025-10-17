-- =============================================
-- Schema Changes TEmplate
-- =============================================


---------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 12/01/2011
-- Purpose - Added date of marriage in sgt_relationship
----------------------------------------------------------------------------------------------------------------------------------
alter table sgt_Relationship
add  DATE_OF_MARRIAGE UDT_DATETIME
GO

---------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/01/2011
-- Purpose - Removed DATE_OF_MARRIAGE and DATE_OF_DIVORCE from SGT_PERSON, Added REPORTED_BY,RELATIONSHIP,PHONE_NUMBER in SGT_DEATH_NOTIFICATION
----------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_DEATH_NOTIFICATION
ADD REPORTED_BY VARCHAR(50) NULL

ALTER TABLE SGT_DEATH_NOTIFICATION
ADD RELATIONSHIP VARCHAR(50) NULL

ALTER TABLE SGT_DEATH_NOTIFICATION
ADD PHONE_NUMBER UDT_PHONE NULL

ALTER TABLE SGT_PERSON
DROP COLUMN DATE_OF_MARRIAGE

ALTER TABLE SGT_PERSON
DROP COLUMN DATE_OF_DIVORCE