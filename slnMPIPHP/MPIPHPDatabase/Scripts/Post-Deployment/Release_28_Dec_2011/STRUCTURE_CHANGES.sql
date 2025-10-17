----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/28/2011
-- Purpose - Added column COUNTY in SGT_PERSON_ADDRESS and  SGT_PERSON_CONTACT table.
----------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_PERSON_ADDRESS
ADD COUNTY varchar(50) 

ALTER TABLE SGT_PERSON_CONTACT
ADD COUNTY varchar(50) 

