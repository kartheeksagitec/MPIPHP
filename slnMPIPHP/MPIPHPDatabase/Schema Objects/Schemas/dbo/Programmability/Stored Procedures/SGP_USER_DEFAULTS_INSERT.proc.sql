
CREATE PROCEDURE [dbo].[SGP_USER_DEFAULTS_INSERT] (
@USER_SERIAL_ID int ,
@FORM_NAME VARCHAR(50),
@GROUP_CONTROL_ID VARCHAR(50),
@DEFAULT_SET_ID VARCHAR(50),
@DATA_FIELD VARCHAR(200),
@DEFAULT_VALUE VARCHAR(50) )
AS
/**********************************************************************
Author Name: XXXX XXXX
Create Date: 05/20/10
Purpose: To insert data into SGS_USER_DEFAULTS

File Name : SGP_USER_DEFAULTS_INSERT.sql         

Table: 
================================
SGS_USER_DEFAULTS

Change History:
=========================================================================
Name               Date         Change Details
=========================================================================
XXXX               05/20/10	    Proc Created 
=========================================================================*/
BEGIN
INSERT INTO SGS_USER_DEFAULTS
( USER_SERIAL_ID, FORM_NAME, GROUP_CONTROL_ID, DEFAULT_SET_ID, DATA_FIELD, DEFAULT_VALUE )
VALUES ( @USER_SERIAL_ID, @FORM_NAME, @GROUP_CONTROL_ID, @DEFAULT_SET_ID, @DATA_FIELD, @DEFAULT_VALUE )
END

