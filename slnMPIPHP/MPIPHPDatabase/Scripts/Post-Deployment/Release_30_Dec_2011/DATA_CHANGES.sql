
----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/30/2011
-- Purpose - Updated message_id- 5055,5056,5082,5125,5140,5153 from SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

update SGS_MESSAGES set DISPLAY_MESSAGE='Set cancellation reason as deceased and cancel all the DRO application for this participant.' where MESSAGE_ID=5055

update SGS_MESSAGES set DISPLAY_MESSAGE='Set cancellation reason as deceased and cancel all the Benefit application for this participant.' where MESSAGE_ID=5056

update SGS_MESSAGES set DISPLAY_MESSAGE='Status is blank.' where MESSAGE_ID=5082

update SGS_MESSAGES set DISPLAY_MESSAGE='Retirement date should be equal for all plans.' where MESSAGE_ID=5125

update SGS_MESSAGES set DISPLAY_MESSAGE='Start Date and End Date should be in same Computation Year.' where MESSAGE_ID=5140

update SGS_MESSAGES set DISPLAY_MESSAGE='Not Eligible for Withdrawal.' where MESSAGE_ID=5153

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Gagan Dhamija
-- Date - 12/30/2011
-- Purpose - deleted template QDRO Request Form from SGS_COR_TEMPLATES table.
----------------------------------------------------------------------------------------------------------------------------------

IF EXISTS (SELECT TEMPLATE_NAME FROM SGS_COR_TEMPLATES WHERE TEMPLATE_DESC = 'QDRO Request Form')
Delete from SGS_COR_TEMPLATES where TEMPLATE_DESC = 'QDRO Request Form'
ELSE
PRINT 'TEMPLATE = QDRO Request Form does not exits in SGS_COR_TEMPLATES tables'

