----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhsihek Sharma	
-- Date - 12/07/2011
-- Purpose - Added message_id-5133 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5133)
PRINT 'MESSAGE_ID = 5133 already exits in SGS_MESSAGES tables'
ELSE
INSERT dbo.sgs_messages ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[RESPONSIBILITY_ID],[RESPONSIBILITY_VALUE],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES(5133,'Participant is Eligible for Pension as well as IAP plan; both plans must be processed simultaneously.',16,'E',NULL,NULL,16,NULL,'asharma',GETDATE(),'asharma',GETDATE(),0)
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri 
-- Date - 12/07/2011
-- Purpose - Added message_id-5132 in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------

declare @currentdatetime datetime
set @currentdatetime = getdate()
IF EXISTS (SELECT MESSAGE_ID FROM SGS_MESSAGES WHERE MESSAGE_ID = 5132)
PRINT 'MESSAGE_ID = 5132 already exits in SGS_MESSAGES tables'
ELSE
INSERT into dbo.SGS_MESSAGES (MESSAGE_ID, DISPLAY_MESSAGE, SEVERITY_ID, SEVERITY_VALUE, INTERNAL_INSTRUCTIONS, EMPLOYER_INSTRUCTIONS, RESPONSIBILITY_ID, RESPONSIBILITY_VALUE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ) 
VALUES (5132, 'Retirement Date should be greater than Awarded On Date.', 16 , 'E', Null, Null, 16, null, 'rashmi.sheri', @currentdatetime, 'rashmi.sheri', @currentdatetime, 0)
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 12/07/2011
-- Purpose - Added messages in to the SGS_MESSAGES table.
----------------------------------------------------------------------------------------------------------------------------------
insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5134,'Please select Computation Year',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5135,'Please select Type Of Hours',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5136,'Please enter Hours Reported',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5137,'Please enter Bridge Start Date',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5138,'Please enter Bridge End Date',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)


insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5139,'Start Date cannot be greater than End Date',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5140,'Start Date and End Date should be in same Computaion Year',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)

insert into SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,RESPONSIBILITY_ID,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) 
values (5141,'Cannot Add Overlapping Period for bridging Hours',16,'E',16,'Rohan',GETDATE(),'Rohan',GETDATE(),0)


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 12/07/2011
-- Purpose - Added Code-Value pair in to the SGS_CODE and SGS_CODE_VALUE table.
----------------------------------------------------------------------------------------------------------------------------------

INSERT INTO SGS_CODE (CODE_ID,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ) VALUES 
(6038,'Bridge Type','rohan.adgaonkar',GETDATE(),'rohan.adgaonkar',GETDATE(),0)


INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (6038,'DSBL','Disability','rohan.adgaonkar',getdate(),'rohan.adgaonkar',getdate(),0)

INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (6038,'PNCC','Pregnancy/Child Care','rohan.adgaonkar',getdate(),'rohan.adgaonkar',getdate(),0)

INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (6038,'FMLA','Family and Medical Leave Act','rohan.adgaonkar',getdate(),'rohan.adgaonkar',getdate(),0)

INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (6038,'MLSR','Military Service','rohan.adgaonkar',getdate(),'rohan.adgaonkar',getdate(),0)

INSERT INTO SGS_CODE_VALUE (CODE_ID,CODE_VALUE,DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (6038,'CTEP','Contiguous Employment','rohan.adgaonkar',getdate(),'rohan.adgaonkar',getdate(),0)