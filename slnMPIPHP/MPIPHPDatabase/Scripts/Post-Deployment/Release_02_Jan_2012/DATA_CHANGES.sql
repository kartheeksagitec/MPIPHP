----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 01/02/2012
-- Purpose - Updated message_id-5118,1168,1169 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_MESSAGES set DISPLAY_MESSAGE ='Participant is not Eligible anymore in the selected plan(s).Please modify chosen Benefit Details' where MESSAGE_ID=5118

update SGS_MESSAGES set DISPLAY_MESSAGE ='Date of death cannot be earlier than effective start date specified in beneficiary of.' where MESSAGE_ID=1169

update SGS_MESSAGES set DISPLAY_MESSAGE ='Date of death cannot be earlier than effective start date specified in beneficiary.' where MESSAGE_ID=1168

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 01/02/2012
-- Purpose - Updated message_id-5150,5151,5152 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE='An Pending/Approved Retirement Application for this Person and {0} Plan already exists.'
WHERE MESSAGE_ID=5150

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE='An Pending/Approved Withdrawal Application for this Person and {0} Plan already exists.'
WHERE MESSAGE_ID=5151

UPDATE SGS_MESSAGES SET DISPLAY_MESSAGE='An Pending/Approved Disability Application for this Person and {0} Plan already exists.'
WHERE MESSAGE_ID=5152