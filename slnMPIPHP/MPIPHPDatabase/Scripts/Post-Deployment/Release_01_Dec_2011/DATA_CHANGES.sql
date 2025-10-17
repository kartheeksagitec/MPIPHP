-- =============================================
-- Script Template
-- =============================================
---------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 12/01/2011
-- Purpose - added sgs_code_value cas6 & cas9 for code 6016
----------------------------------------------------------------------------------------------------------------------------------
declare  @currentdatetime datetime
declare @defaultcodevalueorder int
set @currentdatetime = getdate()
set @defaultcodevalueorder = (select coalesce((select max(code_value_order) from sgs_code_value where code_id=6016),0)+1)

IF EXISTS (SELECT CODE_ID FROM SGS_CODE_VALUE WHERE CODE_ID = 6016 AND CODE_VALUE = 'CAS6')
PRINT 'CODE_ID = 6000 already exits in SGS_CODE tables '
ELSE
INSERT INTO dbo.SGS_CODE_VALUE(CODE_ID,CODE_VALUE,DESCRIPTION, START_DATE,CODE_VALUE_ORDER, CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
VALUES (6016,'CAS6','Test case 006 - Retirement Eligibility', @currentdatetime,@defaultcodevalueorder, N'sjain', @currentdatetime, N'sjain', @currentdatetime,0)
GO

declare  @currentdatetime datetime
declare @defaultcodevalueorder int
set @currentdatetime = getdate()
set @defaultcodevalueorder = (select coalesce((select max(code_value_order) from sgs_code_value where code_id=6016),0)+1)

IF EXISTS (SELECT CODE_ID FROM SGS_CODE_VALUE WHERE CODE_ID = 6016 AND CODE_VALUE = 'CAS7')
PRINT 'CODE_ID = 6016 already exits in SGS_CODE tables '
ELSE
INSERT INTO dbo.SGS_CODE_VALUE(CODE_ID,CODE_VALUE,DESCRIPTION, START_DATE,CODE_VALUE_ORDER, CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
VALUES (6016,'CAS7','Test Case 007 - Disability Eligibility and Application', @currentdatetime,@defaultcodevalueorder, N'vthomas', @currentdatetime, N'vthomas', @currentdatetime,0)
GO




