
/****** Object:  StoredProcedure [dbo].[USP_PID_Person_ins]    Script Date: 01/16/2012 17:43:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information
-- RG 04/10/2009 - If invalid SexCode, replace with ''
-- =============================================
ALTER procedure [dbo].[USP_PID_Person_ins]
(
  @PID								varchar(15)
, @SSN                          	varchar(10)    	 = NULL
, @ParticipantPID					varchar(15)		 = NULL -- This field will have value when adding dependent, it will the partssn
, @EntityTypeCode						varchar(3)		 = 'P'	-- This field have value 'P' for Person Record and 'T' or 'O' for Trust or Organization
, @RelationType						char(1)			 = NULL -- this will have value when adding dependent D-Dependent,B- Beneficiary
-- Person Information
, @FirstName                    	varchar(50) 	 = NULL
, @MiddleName                   	varchar(50) 	 = NULL
, @LastName                     	varchar(50) 	 = NULL
, @Gender                    		char(1)     	 = NULL
, @DateOfBirth                  	datetime    	 = NULL
, @DateOfDeath                  	datetime    	 = NULL
--Contact Info
, @HomePhone						varchar(15)   = NULL  
, @CellPhone							varchar(15)   = NULL  
, @Fax								varchar(15)   = NULL  
, @Email							varchar(50)   = NULL 
-- Others
, @AuditUser                    	varchar(30)   = NULL
)
AS
BEGIN
declare @rtn     	int
declare @ParticipantSSN	varchar(10)
set nocount on

--if relationtype is dependent, check date of birth and gender
declare	@errornumber	int,
	    @errormessage	varchar(255),
	    @TaxID varchar(10)
	

/*For Dependents allow no SSN in OPUS, create next new SSN*/	

if @RelationType in ('B','D')
begin
	If @SSN is Null
	Begin
		exec USP_GetNextTaxId @TaxID output
	End
	
	if @TaxID is Null
	Begin
		select @errornumber = 619060
		select @errormessage = 'Error to generate SSN.'
		exec showerror @errornumber, @errormessage
		return @errornumber
	end
	
	select @SSN = @TaxID
end

if @RelationType not in ('B','D') and @SSN is Null
begin
	select @errornumber = 619060
	select @errormessage = 'Participant SSN may not be blank.'
	exec showerror @errornumber, @errormessage
	return @errornumber
end
/*Change for OPUS*/

-- RG 04/10/2009 - If invalid SexCode, replace with ''
if @Gender not in ('','M','F')
begin
	set @Gender = ''
end

if (@EntityTypeCode = 'P') and (@RelationType = 'D')
begin
	if @DateOfBirth is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The Date of Birth may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
	if @Gender is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The gender may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
end

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

select @ParticipantSSN=SSN from Eligibility_PID_Reference where PID = @ParticipantPID

		-- insert the record
		if not exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
		Begin
			insert into Eligibility_PID_Reference(PID,SSN,CreateDate,CreateUser) 
			VALUES (@PID,@SSN,getdate(),@AuditUser)
			if @@error != 0
			begin
				raiserror 99999 'PID insert failed in Eligibility system.'
				return @@error
			end
			
			-- Add Person Record
			if @EntityTypeCode ='P'
			Begin
				exec @rtn = person_ins  @ssn 				= @ssn,
										@sexcode			= @Gender,
										@firstname			= @firstname,
										@middlename 		= @middlename,
										@lastname 			= @lastname,
										@dateofbirth		= @dateofbirth,
										@dateofdeath		= @dateofdeath,
										@Phone1				= @HomePhone,
									    @Phone2				= @Fax   ,
									    @Email				= @Email ,
									    @Mobile				= @CellPhone  ,
									    @AuditUser			= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End

			if @EntityTypeCode in ('T','O')
			Begin
        		EXEC @rtn = Organization_ins	@TaxID	    = @SSN,
        										@Name		= @FirstName,
        										@AuditUser	= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End
		End

		-- This means that person is participant
		if (@ParticipantSSN is not null)
		Begin
			--MM Added code to verify existence in Person table before adding in reference table.
			if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
				and (exists (select 1 from Person where SSN = @SSN) OR exists (select 1 from Organization WHERE Taxid = @SSN))
			BEGIN
				if not exists (select 1 from Elig_PID_NewDependentBeneficiaries
								where SSN = @ParticipantSSN and DepSSN = @SSN 
								and Type = @RelationType)
				  and ((@RelationType ='D' and not exists (select 1 from Dependent where SSN = @ParticipantSSN and DepSSN = @SSN))
						or
					   (@RelationType ='B' and not exists (select 1 from LifeInsBeneficiary where SSN = @ParticipantSSN and BeneSSN = @SSN)))
				Begin
					insert into Elig_PID_NewDependentBeneficiaries
					select @ParticipantSSN,@SSN,@RelationType,getdate(),@AuditUser
					if @@error != 0
					begin
						raiserror 99999 'PID insert failed for new dependent/beneficiaries.'
						return @@error
					end
				End
			END
			ELSE
			begin
					set @errormessage = 'PID '+@PID+' could not be inserted in Eligibility.'
					raiserror 99999 @errormessage 
					return 99999
			end
		End

--  All is good
return 0
END


------------------------------------


/****** Object:  StoredProcedure [dbo].[sp_Participant_ins]    Script Date: 01/13/2012 16:54:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*** This stored procedure creates new participant by inserting records into Person, PersonAddress and Participant tables. ***/
/*
05/12/2006	VN	Changed Code, so that if person is present , info is updated instead of Although Error
01/08/2009  RG  Removed PID_server link server PID database is getting moved to the same server as HEDB and EADB

*/
ALTER   procedure [dbo].[sp_Participant_ins]
(
	@ssn 			varchar(10),
	@PGID 			int,
	@PTID 			int,
	@ParticipantGroup 	varchar(50),
	@participanttype 	varchar(50),
	@TrueUnion		int		= NULL,
	@DefaultUnion		int		= NULL,
	@ignore_person_error	bit		= 0
)
as
begin
-- declare variables
declare @errornumber	int,
	@errormessage 	varchar(255),
	@rtn 		int,
	@fdc 		smalldatetime,
	@maritalstatus 	char(1),
	@Credentials	varchar(30),
	@recCount	int

declare   @pid							char(15)     
		, @FirstName                    	varchar(50)
		, @LastName                     	varchar(50)
		, @MiddleName                   	varchar(50)
		, @Gender                    	char(1)    
		, @DateOfBirth                  	datetime   
		, @DateOfDeath                  	datetime   
		, @HomePhone						varchar(15)  
		, @CellPhone						varchar(15)  
		, @Email							varchar(50) 
		, @Fax							varchar(15)  
		, @Attention						varchar(30) 	
		, @Address1						varchar(60) 
		, @Address2						varchar(60) 
		, @City							varchar(30) 
		, @State							varchar(20)
		, @PostalCode					varchar(10)  
		, @Country						varchar(25) 
		, @CountryCode					varchar(3) 
		, @ForeignAddr					bit			   
		, @ReturnedMail					datetime   
		, @DoNotUpdate					bit			   
		, @addresstype					char(1)
		, @AuditDate					datetime
		, @AuditUser					varchar(30)

-- check the userlevel of the employee who is logged in
exec @rtn = sp_AllowAccess @ssn = @ssn
if @rtn != 0
	begin
	raiserror 9999 'You donot have the permissions to perform this action'
	return 99999
	end
--

if @firstname is null
	begin
	select @firstname = 'XXX'
	end

-- default the address type to 1
select @addresstype = 1
-- default the participanttype
SELECT
@ParticipantGroup = PG.PGroup,
@ParticipantType = PT.PType
FROM PTParticipantGroup PG LEFT OUTER JOIN
PTParticipantType PT ON PG.PGID = PT.PGID WHERE PG.PGID = @PGID and PT.PTID = @PTID

if rtrim(@participanttype) is null
begin
select @participanttype = 'R'
end

-- check to see if this person is already a participant
if exists(select 1 from participant where ssn = @ssn)
begin
	select @errornumber = 99999
	select @errormessage = 'Cannot add participant because there is already and entry with this ssn.'
	exec showerror @errornumber, @errormessage
	return @errornumber
end


create table #tmpPIDInfo (
  pid							char(15)     
, ssn							char(10)	
, FirstName                    	varchar(50)   NULL
, LastName                     	varchar(50)   NULL
, MiddleName                   	varchar(50)   NULL
, Gender                    	char(1)       NULL
, Ethnicity					    varchar(50)	  NULL          
, DateOfBirth                  	datetime      NULL
, DateOfDeath                  	datetime      NULL
, HomePhone						varchar(15)   NULL  
, CellPhone						varchar(15)   NULL  
, WorkPhone						varchar(50)	  NULL
, Email							varchar(50)   NULL 
, Email2						varchar(150)  NULL
, Fax							varchar(15)   NULL  
, Pager							varchar(50)   NULL
, Attention						varchar(30)   NULL 	
, Address1						varchar(60)   NULL 
, Address2						varchar(60)   NULL 
, City							varchar(30)   NULL 
, State							varchar(20)   NULL
, PostalCode					varchar(10)   NULL  
, Country						varchar(25)   NULL 
, CountryCode					varchar(3)    NULL
, ForeignAddr					bit			  NULL 
, ReturnedMail					datetime      NULL
, DoNotUpdate					bit			  NULL 
, effectivedate					datetime      NULL
, termdate						datetime      NULL
--, SecurityQn					varchar(300)  NULL
--, SecurityAns					varchar(300)  NULL
--, OverrideQn					varchar(300)  NULL
--, OverrideAns					varchar(300)  NULL
--, Comment						text		  NULL
)

-- set the fdc equal to the last day of the prior period
select @fdc = max(IneligibleEndDate)
from	period
where 	IneligibleEndDate < GetDate()

SELECT @ParticipantGroup = PGroup FROM PTParticipantGroup WHERE PGID = @PGID
SELECT @participanttype = PType FROM PTParticipantType WHERE PGID = @PGID and PTID = @PTID

-- put code here to update marital status field
-- start the transaction

	-- Need to bring info from PID, if not present
	if not exists (select 1 from Eligibility_PID_Reference where SSN = @SSN)
	Begin

		insert into #tmpPIDInfo
		exec OPUS.DBO.usp_GetPidInfo @ssn =@ssn -- RG 01/08/2009 removed PID_server link server. 
                                               -- PID database is getting moved to the same server as HEDB and EADB

		select    @pid				= pid				
				, @FirstName		= FirstName                    	
				, @LastName			= LastName                     	
				, @MiddleName       = MiddleName           	
				, @Gender           = Gender         	
				, @DateOfBirth      = DateOfBirth            	
				, @DateOfDeath      = DateOfDeath            	
				, @HomePhone		= HomePhone				
				, @CellPhone		= CellPhone				
				, @Email			= Email				
				, @Fax				= Fax			
				, @Attention		= Attention				
				, @Address1			= Address1			
				, @Address2			= Address2			
				, @City				= City			
				, @State			= State				
				, @PostalCode		= PostalCode			
				, @Country			= Country
				, @CountryCode		= CountryCode
				, @ForeignAddr		= isnull(ForeignAddr,0)
				, @ReturnedMail		= ReturnedMail
				, @DoNotUpdate		= isnull(DoNotUpdate,0)
			
		from #tmpPIDInfo	

	End

select @recCount = count(*) from #tmpPIDInfo

begin tran 	if @recCount =1
	Begin	
		exec @rtn = usp_PID_Person_ins @PID				= @PID			
									, @SSN              = @SSN
									, @EntityTypeCode	= 'P'	
									, @FirstName        = @FirstName                    	 
									, @MiddleName       = @MiddleName
									, @LastName         = @LastName
									, @Gender           = @Gender
									, @DateOfBirth      = @DateOfBirth
									, @DateOfDeath		= @DateOfDeath
									, @HomePhone		= @HomePhone
									, @CellPhone		= @CellPhone
									, @Fax				= @Fax
									, @Email			= @Email
									--, @AuditDate        = @AuditDate
									, @AuditUser        = @AuditUser
		if @rtn != 0
		begin
			raiserror 99999 'An error has occurred during insert into Person Table.'
			return @rtn
		end
		
		exec @rtn = usp_PID_PersonAddress_ins  @PID				= @PID			
--											, @SSN              = @SSN
											, @EntityTypeCode	= 'P'	
											, @Attention		= @Attention
											, @Address1			= @Address1						
											, @Address2			= @Address2
											, @City				= @City
											, @State			= @State
											, @PostalCode		= @PostalCode
											, @Country			= @Country
											, @CountryCode		= @CountryCode
											, @ForeignAddr		= @ForeignAddr
											, @ReturnedMail		= @ReturnedMail
											, @DoNotUpdate		= @DoNotUpdate
--											, @AuditDate        = @AuditDate
											, @AuditUser        = @AuditUser

		if @rtn != 0
		begin
			raiserror 99999 'An error has occurred during insert into PersonAddress Table.'
			return @rtn
		end

	End

	-- insert to the participant table
	exec @rtn = participant_ins
		@ssn 		= @ssn,
		@division 	= 'PTN',
		@active 	= 1,
		@fdc		= @fdc,
		@comment 	= NULL,
		@TrueUnion	= @TrueUnion,
		@DefaultUnion	= @DefaultUnion,
		@PGID		= @PGID,
		@PTID		= @PTID,
		@PGroup		= @ParticipantGroup,
		@type 		= @participanttype
		
	if @rtn != 0
	begin
		raiserror 99999 'An error has occurred during insert into Participant Table.'
		return @rtn
	end

-- all is ok
commit tran

-- Rollover just created participant to initialize Eligibility table
exec @rtn = sp_Rollover_Participant	@SSN = @SSN
if @rtn != 0
	begin
	return @rtn
	end



return 0
end



-----------------------------------------------------



/****** Object:  StoredProcedure [dbo].[sp_Populate_RptEligData_monthly]    Script Date: 01/13/2012 18:35:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
select * from pid..personcontact
exec sp_Populate_RptEligData_monthly '12/01/2010','no',0
go
exec sp_Populate_RptEligData_monthly '09/01/2011','no',0
go
exec sp_Populate_RptEligData_monthly '11/01/2011','no',0
go

truncate table select * from RptParticipantEligData_monthly
truncate table RptDependentEligData_monthly
-- updating eligibilitytype and eligibilitycoverage
per Celso email 11/21/2011 8:20 am
1.	EligibilityType	ByHours	ByExtension	Cobra	Ret/Sur
					CE		BH			CC		RT
					CT		TM			CN		SV
					TN		TD		
					NE		DP		
2.	CoverageType  = “Full” or “Partial”

For EligibilityType the possible values are “ByHours”, “ByExtension” , “COBRA”, “Ret/Sur” and the related values are 
listed the yellow highlight.
For 2.	CoverageType Full indicates the records all the benefits and 
Partial indicates missing some benefits (Core Cobra is an example or Hospital only retirees or C2)

The logic below makes sense to populate family type:
Single 1 = Only participant
Family 2 = Participant and Spouse or Domestic Partner (no children)
Family 3 or the actual number of family members.  Participant and Spouse and children or Participant and children and no Spouse

For Survivors/DP participants:
Single Spouse  - Only Surviving spouse
Family – Spouse with children or just children

Monthly Hours group:
0 - all hours from >=0 and <1
1-39.9
40-79.9
80-119.9
120-139.9
140 or higher

Hari:
Please add three more columns to the table: 
1.	Telephone number (should come from PRS)              -- Phone
2.	Email address (should come from PRS)                 -- Email
3.	NumberOfEligibles (per our discussion this morning)  -- TotalFamilyEligibles

Please let me know once the data is refreshed.

Thanks,

Celso Perez    (email 12/5/2011 8:52am)

--RG 12/15/2011 added new field SSN_Dual to RptDependentEligData_monthly table
to set dual_ssn for spouses



-- Ignore marital status
-- =============================================
-- Author:		Hari/Rozana Goldring
-- Create date: 11/22/2011
-- Description:	This procedure is to use to populate ReportDataMain_monthly table every night
-- Tables: RptParticipantEligData_monthly and RptDependentEligData_monthly
-- Data issue with ssn 100281916
-- =============================================  */


/****** Object:  StoredProcedure [dbo].[sp_ProcessHoursNew]    Script Date: 01/13/2012 18:39:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_ProcessHoursNew]  @ProcessDate smalldatetime = NULL  
AS  
BEGIN  
/* Declare variables */  
declare @SSN char(9), @HoursID int, @HoursWorked numeric(7,1), @StartDate datetime, @EndDate datetime, @TrueUnion int,  
 @LastName varchar(50), @FirstName varchar(50), @MiddleName varchar(50),  
 @Late bit, @recdate smalldatetime, @cutoffdate smalldatetime,  
 @ReturnValue int   
  
declare @priorityCount int,@processedCount int  
declare @query   varchar(255),  
 @reccount   int,  
 @subject  varchar(255),  
 @message   varchar(255),  
 @attach_results varchar(5),  
 @PGID int,  
 @PTID int,  
 @Pgroup  char(10),  
 @PType  char(10),  
 @count int,  
 @EmpAcctNo char(10),  
 @Name varchar(50),  
    @ErrNum int,  
    @ErrMessage varchar(500)  
  
set nocount on  
  
if @ProcessDate is null select @ProcessDate = GETDATE()  
  
/*-------------------------------------------------------------------------------------------  
--ITS#1584 - ProcessHoursJobImprovement  
--03/02/05 VN Changing code so that records added to #RolloverQueue are not duplicated.  
--04/06/2005 VN Changed join condn for finding Ineligibles  
--   Also set the max no of participants to be rolled to no of critical participants or 3000 which ever is greater.  
--07/29/2005 VN Setting New Participants as priority 1  
--06/21/2007 VN Codse, so that if EmpNo is 540 ie MPI then set type as 'EM'   
--     when adding participant and send mail to users to verify.  
--04/16/2009 Raj Modified code to run this process only for those SSNs where we have data in PID system.  
     Excluding those SSNs which are not in PID system and Eligibility Person table   
     from the cursor (#RolloverQueue)  
--05/11/2009    RG  Changed to use variable for @@error and created new table _errLog to keep errors and send message to users  
                    Removed change made by Raj, instead put all errors in _errlog table  
--06/24/2009    RG  Put back checking if person exists in Person table  
--02/10/2010 Raj Added code to insert records in DBOutBound.dbo.EAP for new Employees, if they do not exist.  
------------------------------------------------------------------------------------------*/  
  
Create Table #RolloverQueue (RowId int identity (1,1), SSN char(9))  
  
if object_id('_EmployeeAdded') is not NULL  
 drop table _EmployeeAdded  
  
  
if object_id('_ErrLog') is not NULL  
 drop table _ErrLog  
  
create table _EmployeeAdded (SSN char(10),Name varchar(50))  
  
Create table _ErrLog (SSN char(10), ErrMessage Varchar(500))  
  
IF DATENAME(DW, GETDATE()) = 'Sunday' or DATENAME(DW, GETDATE()) = 'Saturday'  
BEGIN  
    insert into #RolloverQueue(SSN)  
    SELECT distinct ssn  
    FROM HOURSTOPROCESS hp where processed = 0  
END  
ELSE  
BEGIN  
    -- Maximum 3000 will be processed overnight to avoid job running in working hours. -------  
  
    --  Priority1 New Participants  
    Select distinct ssn  
    into #NewParticipants  
    FROM HOURSTOPROCESS hp   
    where processed = 0  
    and not exists (select 1 from participant where ssn = hp.ssn)  
  
    insert into #RolloverQueue(SSN)  
    select ssn from #NewParticipants  
  
    -- Priority 2  
    SELECT distinct ssn  
    into #InEligibles   
    FROM HOURSTOPROCESS hp where processed = 0  
    and exists (select 1 from eligibility e   
    inner join period p on p.eligiblestartdate between e.eligeffective and e.eligcancellation  
            where e.ssn = hp.ssn and hp.ToDate between p.qualifyingstartdate and p.qualifyingEndDate  
                        and e.statuscode in (select statuscode from statuscode where division = 'PTN')  
   and e.eligeffective <= getdate())  
      
      
    insert into #RolloverQueue(SSN)  
    select ssn from #InEligibles  
  
    -- Priority 3      
    SELECT distinct ssn  
    into #NegHrs   
    FROM HOURSTOPROCESS hp where processed = 0  
    and hoursworked < 0   
    and ssn not in (select distinct ssn from #RolloverQueue) --03/02/05 VN - Condn added to avoid adding duplicate SSN's  
  
    insert into #RolloverQueue(SSN)  
    select ssn from #NegHrs  
  
    -- Priority 4      
    SELECT distinct ssn  
    into #lateHrs   
    FROM HOURSTOPROCESS hp where processed = 0  
    and late = 1   
    and ssn not in (select distinct ssn from #RolloverQueue) --03/02/05 VN - Condn added to avoid adding duplicate SSN's  
  
    insert into #RolloverQueue(SSN)  
    select ssn from #lateHrs  
      
  
    select @priorityCount = count(*) from #RolloverQueue  
      
    select *   
    into #totalPriorityTable  
    from #RolloverQueue  
  
    select distinct SSN into #HoursToProcess  
    from HoursToProcess  
    where Processed = 0   
--    and ssn not in (select ssn from #InEligibles)  
    and ssn not in (select distinct ssn from #RolloverQueue) --03/02/05 VN - Condn added to avoid adding duplicate SSN's  
  
    insert into #RolloverQueue(SSN)  
    SELECT distinct ssn  
    FROM #HoursToProcess   
  
    select @processedCount = case when @priorityCount > 3000 then @priorityCount  
        else 3000  
        end  
  
    select PriorityCount =@priorityCount, processedCount = @processedCount, totalTableCount =count(*) from #RolloverQueue  
  
    delete from #RolloverQueue where RowId > @processedCount  
  
    drop table #InEligibles, #HoursToProcess, #NegHrs, #LateHrs,#NewParticipants  
  
END  
-- User #RolloverQueue table to create cursor for rollover.  
  
--Make sure that the SSN should exist in either Hedb.dbo.Person or PID.dbo.Person tables.  
declare cHours cursor 
/*1/6/2012 gm rg */
STATIC LOCAL
for select SSN from #RolloverQueue t  
       where exists (select 1 from OPUS.dbo.Person p where p.SSN = t.SSN)  
        or exists (select 1 from Hedb.dbo.Person e where e.SSN = t.SSN)  
  
open cHours  

fetch next from cHours into @SSN  
  
while @@fetch_status = 0  
begin   
 if len(rtrim(ltrim(@SSN))) <> 9 goto skipssn  
 select top 1 @TrueUnion = UnionCode, @LastName = LastName, @FirstName = FirstName, @MiddleName = MiddleName  
 from HoursToProcess  
 where SSN = @SSN and Processed = 0   
 order by auditdate, hoursid  
  
 select @EmpAcctNo = NULL  
  
 select @EmpAcctNo = isnull(EmpAccountNo,'')  
 from  HoursToProcess  
 where SSN = @SSN and Processed = 0   
 and EmpAccountNo = 540  
  
 select @EmpAcctNo = isnull(@EmpAcctNo,'')  
  if exists(select 1 from Participant where SSN = @SSN)  begin  
  update Participant  
  set TrueUnion = @TrueUnion  
  where SSN = @SSN  
  
set @ErrNum = @@Error  
  
  if @ErrNum <> 0  
  begin  
            set @ErrMessage = 'Error updating True Union for ' + @SSN + ' : ' + convert(varchar(255),GetDate(),109)    
   Print @ErrMessage  
            insert _ErrLog  
            select  @SSN, @ErrMessage  
   goto SkipSSN  
  end  
 end  
 else  
 begin  
  if @EmpAcctNo is not null and @EmpAcctNo <> ''  
  Begin  
   --Set Type 'EM'  
   select @PGID =1,@PTID =2,@Pgroup = 'Original',@PType = 'EM'   
   select @Name =  ''  
   select @Name =  ltrim(rtrim(@LastName)) + ', ' +  ltrim(rtrim(@FirstName))  
   insert into _EmployeeAdded values(@SSN,@Name)  
   select SSN = @SSN,Name = @Name  
  End  
  Else  
  Begin  
   --Set Type 'R'  
   select @PGID =1,@PTID =1,@Pgroup = 'Original',@PType = 'R'     
  End  
    
  Print 'Begin adding new participant ' + @SSN + ' - ' + @LastName + ', ' + @FirstName + ' ' + @MiddleName + ' : ' + convert(varchar(255),GetDate(),109)  
  exec @ReturnValue = sp_Participant_ins   
       @SSN   = @SSN,  
       @PGID     = @PGID,  
       @PTID              = @PTID,  
       @ParticipantGroup  = @Pgroup,  
       @ParticipantType = @PType,  
       @TrueUnion  = @TrueUnion,  
       @DefaultUnion  = null,  
       @ignore_person_error = 1  
set @ErrNum = @@Error  
  
  if @ReturnValue <> 0 or @ErrNum <> 0  
  begin  
   set @ErrMessage = 'Error inserting participant ' + @SSN + ' : ' + convert(varchar(255),GetDate(),109)    
            Print @ErrMessage  
            insert _ErrLog  
            select  @SSN, @ErrMessage  
   goto SkipSSN  
  end  
 end  
 BEGIN  
  --Print 'Running rollover MPI for ' + @SSN + ' : ' + convert(varchar(255),GetDate(),109)    
  exec @ReturnValue = sp_Rollover_Participant @SSN = @SSN, @RolloverStartDate = NULL  
  
set @ErrNum = @@Error  
  
  if @ReturnValue <> 0 or @ErrNum <> 0  
  begin  
   set @ErrMessage = 'Error Running rollover for participant: ' + @SSN + ' : ' + convert(varchar(255),GetDate(),109)    
            Print @ErrMessage  
            insert _ErrLog  
            select  @SSN, @ErrMessage  
   goto SkipSSN  
  end  
 END  
  
 if @ErrNum = 0  
 begin  
  Update HoursToProcess Set Processed = 1  
  where SSN = @SSN and Processed = 0  
 end  
SkipSSN:  
 fetch next from cHours into @SSN  
end  --End while  
  
deallocate cHours  
  
IF DATENAME(DW, GETDATE()) <> 'Sunday' and DATENAME(DW, GETDATE()) <> 'Saturday'  
Begin  
 Print 'Priority Records remaining :'  
 select * from #totalPriorityTable t  
 where not exists (select 1 from #RolloverQueue where ssn = t.ssn)  
  
 drop table #totalPriorityTable  
End  
  
select @count = 0  
select * from _EmployeeAdded  
select @count = count(SSN) from _EmployeeAdded  
  
--select * from DBOutBound.dbo.EAP  
Insert into DBOutBound.dbo.EAP (LastName, FirstName, SSN, CurrentMedPlan, EAPType, AuditDate, AuditUser)  
select s.LastName, s.FirstName, s.SSN  
 ,CurrentMedPlan = null  
 ,EAPType = null, getdate(), user_name()  
 from Person s (nolock)  
 inner join _EmployeeAdded a on s.SSN = a.SSN  
 inner join Participant p (nolock) on p.SSN = a.SSN and p.Type = 'EM'  
 where not exists (select 1 from DBOutBound.dbo.EAP e where p.SSN = e.SSN)  
  
  
if @count > 0  
Begin  
 select @subject ='Employees Added - ' + convert(char(10),getdate(),101)  
 select @query = 'select * from  hedb.._EmployeeAdded'  
 select @message ='The following participants have been added/updated to EM.Please verify.' + char(13) + char(13)   
 select @attach_results = 'False'  
  
 EXEC dboutbound..sp_send_notice  
  @recipients = 'EligAdmin@mpiphp.org;',   
  @copy_recipients = ' EligIT@mpiphp.org;',  
  @query = @query,  
  @subject = @subject,  
  @message = @message,  
  @attach_results =@attach_results, @width = 450,  
  @Warning ='O'  
  
End  
  
select @count = 0  
insert _ErrLog  
select SSN , 'Participant doesn''t exist in Person table.'  
from #RolloverQueue t  
  where not (exists (select 1 from OPUS.dbo.Person p where p.SSN = t.SSN)  
   or exists (select 1 from Hedb.dbo.Person e where e.SSN = t.SSN))  
  
select * from _ErrLog  
select @count = count(SSN) from _ErrLog  
  
if @count > 0  
Begin  
 select @subject ='Process Hours errors - ' + convert(char(10),getdate(),101)  
 select @query = 'select * from  hedb.._ErrLog'  
 select @message ='The following participants have errors in process hours.' + char(13) + char(13)   
 select @attach_results = 'False'  
  
 EXEC dboutbound..sp_send_notice  
  @recipients = 'EligAdmin@mpiphp.org;',   
  @copy_recipients = ' EligIT@mpiphp.org;',  
  @query = @query,  
  @subject = @subject,  
  @message = @message,  
  @attach_results =@attach_results, @width = 450,  
  @Warning ='O'  
End  
  
drop table #RolloverQueue  
  
return 0  
  
END  
  
  
  
  
---------------------------------------------------------------







/****** Object:  StoredProcedure [dbo].[spwElibility_ProviderEligibility_sel]    Script Date: 01/24/2012 10:23:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





ALTER procedure [dbo].[spwElibility_ProviderEligibility_sel]
( @Authenticated bit = 0
 ,@memberinfo varchar(30)
 ,@dob char(10)
 ,@dosfrom smalldatetime 
 ,@dosto smalldatetime 
 ,@debug bit = 0)
as

/******************************************************************************  
**  File: spwElibility_ProviderEligibility_sel   
**  Name:   
**  Desc: Procedure gets Eligibility Information for the web based on the set criteria.
**        If the expected/valid parameters are not passed the procedure would 
**        return a negative value indicating invalid parameters.  
**          
**                      
**                      
**                      
**                
**  Return values: For valid parameters a record set and 0
**                 -1 Not Autehticated call.
**                 -3 @memberinfo/PRS#/Participant/SSN not in the PID database. not handled because of same filed can carry either value
**                 -6 Invalid date range
**                 
**  Called by:     Web
**                
**                                     
**  
**  Auth: Hari
**  Date: 12/9/2010 
*******************************************************************************  
**  Change History  Any changes must be approved by IT Health Team
*******************************************************************************  
**  Date:       Author:  Description:  
**  --------    -------- -----------------  
**  12/17/2010  Hari     DOB missed  
**                      
**                      
**                      
**                
*******************************************************************************/

declare  @rtn smallint
        ,@Sub_SSN varchar(10)
        ,@SSN varchar(10)
        ,@prsid varchar(15)
        ,@memberinfotype char(10)
        ,@dt smalldatetime
        ,@i int
        ,@j int
        ,@IsDual char(3)
        ,@dualssn varchar(10)
        ,@pid varchar(10)
        ,@dos smalldatetime
        ,@QNXTPlan varchar(60)
        ,@db smalldatetime


/*
     set @memid = case left(@id,2) 
                       when 'M1' then upper('qmxmb')+substring(@id,3,7) 
                       when 'M2' then upper('mpimb')+substring(@id,3,7)
                       when 'M0' then upper('qmxmb')+substring(@id,4,7)
                       when 'M3' then @id
                  end 

*/
declare @daterange table (slno int, mnth int,ssn varchar(10))

set @dosfrom = isnull(@dosfrom,convert(char(10),getdate(),101))
set @dosto = isnull(@dosto,convert(char(10),getdate(),101))
set @isDual = 'No'
set @dualssn = ''
set @db = @dob

if @dosfrom <> convert(char(10),getdate(),101) and @dosto = convert(char(10),getdate(),101)
   set @dosto = dateadd(month,6,@dosfrom)

declare @Elig table (slno int identity,
                     fullname varchar(50),dob char(10),
                     effdate char(10), termdate char(10),
                     statuscode char(2),status varchar(30),
                     dep_statuscode char(2),hco char(2),
                     EnrolledPlan varchar(50),mnth int,
                     ssn varchar(10),depssn varchar(10),
                     IsDual char(3),PID varchar(10),
                     QNXTPlan varchar(60) )


declare @DualEligibility TABLE(
	SSN char(10) ,
	DualSSN char(10),
	FromDate smalldatetime,
	ToDate smalldatetime ) 

if @Authenticated <> 1
begin
   set @rtn = -1
   goto done
end   

if isdate(@dob) <> 1   -- select isdate('1/2/2010')
begin
   set @rtn = -1
   goto done
end 

if @memberinfo != ''
begin
  set @memberinfo = case left(LTRIM(rtrim(@memberinfo)),3) when 'aox' then substring(LTRIM(rtrim(@memberinfo)),4,30)
                    else LTRIM(rtrim(@memberinfo)) end
end

-- check memberinfo type
if exists(select * from OPUS.dbo.SGT_PERSON where personid = @memberinfo and dateofbirth = @dob)
begin -- select * from pid.dbo.person
   select @sub_ssn = ssn from OPUS.dbo.SGT_PERSON where personid = @memberinfo and dateofbirth = @dob
   set @memberinfotype = 'PRSID'
   set @prsid = @memberinfo
end
else if exists(select * from OPUS.dbo.SGT_PERSON where ssn = @memberinfo and dateofbirth = @dob)
begin
   select @prsid = personid from OPUS.dbo.SGT_PERSON where ssn = @memberinfo and dateofbirth = @dob
   set @memberinfotype = 'SSN'
   set @sub_ssn = @memberinfo
end
else
begin
   set @rtn = -3
   goto done
end

select @isdual = 'Yes',@dualssn = dualssn from hedb.dbo.dualeligibility where ssn = @sub_ssn
select @isdual = 'Yes',@dualssn = ssn from hedb.dbo.dualeligibility where dualssn = @sub_ssn
insert @DualEligibility exec sp_DualEligibility_sel @sub_ssn

set @i = 1
set @dt = @dosfrom
while @dt <= @dosto -- create a one row every month based on the date range the data is sought.
begin
   insert @daterange 
   values(@i,convert(char(6),@dt,112),@sub_ssn)
   set @i = @i + 1
   set @dt = dateadd(month,1,@dt)
   if @debug = 1 select * from @daterange
end

if @debug = 1 select @rtn rtn,@memberinfotype memberinfotype,@memberinfo memberinfo,@sub_ssn
if @debug = 1 select * from @daterange

-- Populate the base data for every month for every member in the family.
insert @Elig (fullname,dob,ssn,depssn,mnth,isdual )
select isnull(rtrim(firstname),'')+' '+isnull(rtrim(lastname),''),convert(char(10),dateofbirth,101) dob,
       p.ssn,p.ssn,mnth*100+1,@isdual
from   hedb.dbo.person p 
join   @daterange t on t.ssn = p.ssn
where  p.ssn = @sub_ssn
and    dateofbirth is not null

insert @Elig (fullname,dob,ssn,depssn,mnth,isdual )
select isnull(rtrim(firstname),'')+' '+isnull(rtrim(lastname),''),convert(char(10),dateofbirth,101) dob,
       d.ssn,d.depssn,mnth*100+1,@isdual
from   hedb.dbo.dependent d join hedb.dbo.person p on p.ssn = d.depssn --and d.ssn = @sub_ssn
join   @daterange t on t.ssn = d.ssn
where  d.ssn = @sub_ssn
and    p.dateofbirth is not null
order by dateofbirth


update t set t.pid = prs.personid
from   @elig t join OPUS.dbo.SGT_PERSON prs on t.depssn = prs.ssn

if @debug = 1 select * from @elig order by fullname,mnth

-- Get eligibility and plan data for participant
update t set t.statuscode = e.statuscode,t.hco = h.hco,t.dep_statuscode = e.statuscode,
             t.effdate = convert(char(10),e.eligeffective,101),t.termdate = convert(char(10),e.eligcancellation,101)
from  hedb..eligibility e
join  hedb..hcoenrollment h on e.ssn = h.ssn and eligeffective between startdate and enddate
join  hedb..hcodetail hd on hd.hco = h.hco and hd.hcotype = 'm'
join  @elig t on t.ssn = e.ssn 
      and convert(smalldatetime,cast(t.mnth as char(8))) between eligeffective and eligcancellation
      --cast(t.mnth as char(6))+'01') between eligeffective and eligcancellation

-- Get dependent eligibility data
update t set t.dep_statuscode = e.statuscode,
             t.effdate = convert(char(10),e.eligeffective,101),t.termdate = convert(char(10),e.eligcancellation,101)
from  hedb..dependenteligibility e
join  @elig t on t.depssn = e.depssn 
      and convert(smalldatetime,cast(t.mnth as char(8))) between eligeffective and eligcancellation
      --cast(t.mnth as char(6))+'01') between eligeffective and eligcancellation

update t set status = s.name
from   @elig t join hedb.dbo.statuscode s on t.dep_statuscode = s.statuscode
where  t.ssn not in (select ssn from @dualeligibility union select dualssn from @dualeligibility)

update t set status = 'Dual'
from   @elig t join hedb.dbo.statuscode s on t.dep_statuscode = s.statuscode
where  t.ssn in (select ssn from @dualeligibility union select dualssn from @dualeligibility)


update t set t.enrolledplan = case dep_statuscode when 'ID' then 'Not Enrolled' else s.name end
from   @elig t join hedb.dbo.hco s on t.hco = s.hco

update t set t.isdual = 'Yes'
from   @elig t 
join @dualeligibility s on convert(smalldatetime,cast(t.mnth as char(8))) between fromdate and todate

set @i = 1
select @j = max(slno) from @elig

while @i <= @j
begin
   select @pid = pid,@dos = cast(mnth as char(8)) from @elig where slno = @i
   exec claims_server.mpiclmdata.dbo.spwClaims_ProviderEligibility_sel 
                                     @pid = @pid
                                    ,@dos = @dos
                                    ,@plan = @qnxtPlan output
   update @elig set qnxtplan = @qnxtplan where slno = @i
   set @i = @i + 1
   set @qnxtplan = ''
end

select * from @elig --order by fullname,mnth

set @rtn = 0   

done:
if @debug = 1 select @rtn rtn,@memberinfotype memberinfotype,@memberinfo memberinfo

return @rtn






/****** Object:  StoredProcedure [dbo].[USP_GetNextTaxId]    Script Date: 01/16/2012 17:35:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[USP_GetNextTaxId] (@TaxID varchar(10) output)
as
begin
    declare @ID int 
    select @ID = convert(int,NextTaxId) from IDS
    update IDS set NextTaxId = convert(varchar(10), @ID +1)
    set @TaxID = convert(varchar(10), @ID)
    return @@Error
end

------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[sp_Populate_RptEligData_monthly]    Script Date: 01/13/2012 18:35:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
select * from pid..personcontact
exec sp_Populate_RptEligData_monthly '12/01/2010','no',0
go
exec sp_Populate_RptEligData_monthly '09/01/2011','no',0
go
exec sp_Populate_RptEligData_monthly '11/01/2011','no',0
go

truncate table select * from RptParticipantEligData_monthly
truncate table RptDependentEligData_monthly
-- updating eligibilitytype and eligibilitycoverage
per Celso email 11/21/2011 8:20 am
1.	EligibilityType	ByHours	ByExtension	Cobra	Ret/Sur
					CE		BH			CC		RT
					CT		TM			CN		SV
					TN		TD		
					NE		DP		
2.	CoverageType  = “Full” or “Partial”

For EligibilityType the possible values are “ByHours”, “ByExtension” , “COBRA”, “Ret/Sur” and the related values are 
listed the yellow highlight.
For 2.	CoverageType Full indicates the records all the benefits and 
Partial indicates missing some benefits (Core Cobra is an example or Hospital only retirees or C2)

The logic below makes sense to populate family type:
Single 1 = Only participant
Family 2 = Participant and Spouse or Domestic Partner (no children)
Family 3 or the actual number of family members.  Participant and Spouse and children or Participant and children and no Spouse

For Survivors/DP participants:
Single Spouse  - Only Surviving spouse
Family – Spouse with children or just children

Monthly Hours group:
0 - all hours from >=0 and <1
1-39.9
40-79.9
80-119.9
120-139.9
140 or higher

Hari:
Please add three more columns to the table: 
1.	Telephone number (should come from PRS)              -- Phone
2.	Email address (should come from PRS)                 -- Email
3.	NumberOfEligibles (per our discussion this morning)  -- TotalFamilyEligibles

Please let me know once the data is refreshed.

Thanks,

Celso Perez    (email 12/5/2011 8:52am)

--RG 12/15/2011 added new field SSN_Dual to RptDependentEligData_monthly table
to set dual_ssn for spouses



-- Ignore marital status
-- =============================================
-- Author:		Hari/Rozana Goldring
-- Create date: 11/22/2011
-- Description:	This procedure is to use to populate ReportDataMain_monthly table every night
-- Tables: RptParticipantEligData_monthly and RptDependentEligData_monthly
-- Data issue with ssn 100281916
-- =============================================  */
ALTER PROCEDURE [dbo].[sp_Populate_RptEligData_monthly]
(
@date char(10) ,
@Reload char(3) = 'No',
@debug bit = 0
)
AS
BEGIN
 --SET NOCOUNT ON  added to prevent extra result sets from interfering with SELECT statements.
SET NOCOUNT ON;

--declare @date char(10) 
--       ,@Reload char(3) 
--       ,@debug bit 
--
--select @date = '11/01/2011',@Reload = 'yes',@debug = 0

Declare @RunDate datetime
       ,@PensionYearStartDate datetime
       ,@PensionYearEndDate datetime
       ,@datamonth int
       ,@msg varchar(200)
       ,@FirstOfMonth smalldatetime
       ,@LastOfMonth  smalldatetime

select @rundate = @date,@FirstOfMonth = dbo.fn_GetFirstOfMonth(@date),@LastOfMonth =dbo.fn_LastDayOfMonth(@date)

declare @workhistory table (ssn varchar(10),hours float,datamonth int)

set @msg = 'Extraction start '+ cast(getdate() as char)
if @debug = 1 print @msg

set @rundate = cast(@date as datetime)
select @datamonth = cast(left(convert(char(8),@rundate,112),6) as int)
if @debug = 1 select getdate() Process_Start

insert @workhistory
select ssn,sum(hours) hours ,@datamonth
from workhistory 
where enddate between @FirstOfMonth and @LastOfMonth
group by ssn
having sum(hours) > 0

set @msg = 'Health Work History gathered '+ cast(getdate() as char)
if @debug = 1
begin
   print @msg
   select @rundate rundate,@datamonth datamonth,@msg msg
end

if @reload = 'yes'
begin
   delete RptParticipantEligData_monthly
   where  datamonth = @datamonth
   delete RptDependentEligData_monthly
   where  datamonth = @datamonth
end 
else
begin
   if exists(select * from RptParticipantEligData_monthly where datamonth = @datamonth)
   begin
      set @msg = 'Data exists for the month specified ' + @date
      if @debug = 1 print @msg
      return
   end
end  

select @PensionYearStartDate = startdate ,@PensionYearEndDate = enddate
from   eadb..pensionyear
where  pensionyear = Year(@RunDate)

insert RptParticipantEligData_monthly
      (PersonID,DataMonth,SSN,LastName,FirstName,DateOfBirth,AgeAtDataMonth,Gender,Statuscode,EligEffective,EligCancellation,
       QualifyingStartDate,QualifyingEndPeriod,HoursWorkedQualified,BankOfHours,HoursWorkedDataMonth,CoverageType,CoverageSubType,
       MedicalPlanCode,MedicalPlanDescription,DentalPlanCode,DentalPlanDescription,VisionPlanCode,VisionPlanDescription,RxCoverage,
       LifeInsFlag,BehavioralHealthCoverage,NumberOfEligDep,Address1,Address2,City,State,Zip,ReturnedMailFlag,InMPTFServArea,
       ForeignFlag,createdate,createid,LastUpdate,Updateid,Date_Extract,EligibilityType,BenefitsCoverage)
select distinct p.Person_Id,@datamonth,e.SSN,p.Last_Name,p.First_Name,p.Date_Of_Birth,
       round(convert(decimal(5,2),case when p.Date_Of_Birth is not null and p.Date_Of_Birth <> ''
                                        then datediff(mm,p.Date_Of_Birth,@lastofmonth) / 12.0											
							      else null							
			                      end),0,1)	,	
       p.GENDER_VALUE,e.StatusCode,e.EligEffective,e.EligCancellation,           -- line 1
       pr.QualifyingStartDate,pr.QualifyingEndDate,
       Case when e.StatusCode ='TN' then e.TNHoursWorked else e.TargetHoursWorked end,
       BankOfHours,HoursWorkedDataMonth = 0,
       case when e.statuscode = 'rt' then 'Retiree'
            when e.statuscode = 'sv' then 'Survivor'
            when e.statuscode in ('cc','cn') then 'COBRA'
            when e.statuscode not in ('rt','sv','cc','cn') then 'Active'
       else '' end coveragetype, '' CoverageSubType,  -- line 2
       m.HCO MedicalPlanCode,case when m.groupnumber != '9702380NY0'  then md.Name 
                             else rtrim(md.Name)+'(Hospital Only)' end MedicalPlanDescription,
       d.hco DentalPlanCode,dd.name DentalPlanDescription,
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CC') or m.groupnumber = '9702380NY0' )
              then '' else 'VS' end VisionPlanCode,
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CC') or m.groupnumber = '9702380NY0' )
              then '' else 'Vision Service Plan' end VisionPlanDescription,
       case when (e.StatusCode in ('PT','NI','CI' ) or m.groupnumber = '9702380NY0') then 'No' else 'Yes' end RxCoverage,  -- line 3
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CN','CC') or m.groupnumber = '9702380NY0') then 'No' else 'Yes' end LifeInsFlag,
       case when (m.hco in ('bc','bn','c9','n9','c1','b2') and m.groupnumber != '9702380NY0')
                 or e.ssn in (select distinct ssn from dboutbound.dbo.eap) then 'Yes' else 'No' end BehavioralHealthCoverage,
       0 NumberOfEligDep,pa.ADDR_LINE_1,isnull(pa.ADDR_LINE_2,''),pa.ADDR_CITY
       ,pa.ADDR_STATE_VALUE
       ,pa.ADDR_ZIP_4_CODE,
       Case when pa.END_DATE is not NUll or pa.END_DATE <> '' then 'Yes' else 'No' end ReturnedMailFlag,
       Case When exists (select 1 from CLAIMS_SERVER.MPIClmData.dbo.MptfServiceArea m 
                         where m.zip = left(pa.ADDR_ZIP_4_CODE,5) 
                         and @rundate between m.effdate and m.termdate) then 'Yes' 
       else 'No' end InMPTFServArea,               -- line 4
       Case When pa.ADDR_COUNTRY_VALUE != '0001' then 'Yes' else 'No' end ForeignFlag,
       getdate(),suser_sname(),getdate(),suser_sname(),getdate(),
       case when statuscode in ('CE','CT','TN','NE') then 'ByHours' 
            when statuscode in ('BH','TM','TD','DP') then 'ByExtension' 
            when statuscode in ('cc','cn') then 'COBRA' 
            when statuscode in ('rt','sv') then 'Ret/Sur' 
            else 'Not Eligible' end EligibilityType,
       case when e.statuscode in ('cc','td') then 'Partial'
            when m.groupnumber = '9702380NY0'  then 'Partial'
            when statuscode in ('CE','CT','TN','NE','BH','TM','DP','rt','sv','cn') then 'Full' 
       else '' end BenefitsCoverage
-- declare @rundate datetime,@FirstOfMonth smalldatetime,@LastOfMonth smalldatetime set @rundate = '10/1/2011' select @FirstOfMonth = dbo.fn_GetFirstOfMonth(@rundate),@LastOfMonth =dbo.fn_LastDayOfMonth(@rundate) select *  
from hedb..eligibility e
    left outer join period pr on e.eligeffective = pr.eligiblestartdate
    inner join opus.dbo.SGT_PERSON p on e.ssn = p.ssn
        and e.eligcancellation >= @FirstOfMonth and e.eligeffective <= @lastofmonth and e.statuscode not in ('PT','NI','CI')
	left outer join OPUS.dbo.SGT_PERSON_ADDRESS pa on p.person_id = pa.person_id
	left outer join hedb..hcoenrollment m on p.ssn = m.ssn 
        and m.enddate >= @FirstOfMonth and m.startdate <= @lastofmonth
	inner join hedb..hcodetail md on m.hco = md.hco and md.HCOType = 'M' and md.hco <> 'nm'
	left outer join hedb..hcoenrollment d on p.ssn = d.ssn
        and d.enddate >= @FirstOfMonth and d.startdate <= @lastofmonth
	inner join hedb..hcodetail dd on d.hco = dd.hco and dd.HCOType = 'D'
--where e.ssn = '127167732 '
set @msg = 'Base data created ' + cast(getdate() as char)
if @debug = 1 print @msg

insert RptParticipantEligData_monthly
      (PersonID,DataMonth,SSN,LastName,FirstName,DateOfBirth,AgeAtDataMonth,Gender,Statuscode,EligEffective,EligCancellation,
       QualifyingStartDate,QualifyingEndPeriod,HoursWorkedQualified,BankOfHours,HoursWorkedDataMonth,CoverageType,CoverageSubType,
       MedicalPlanCode,MedicalPlanDescription,DentalPlanCode,DentalPlanDescription,VisionPlanCode,VisionPlanDescription,RxCoverage,
       LifeInsFlag,BehavioralHealthCoverage,NumberOfEligDep,Address1,Address2,City,State,Zip,ReturnedMailFlag,InMPTFServArea,
       ForeignFlag,createdate,createid,LastUpdate,Updateid,Date_Extract,EligibilityType,BenefitsCoverage)
select distinct p.PersonId,@datamonth,e.SSN,p.LastName,p.FirstName,p.DateOfBirth,
       round(convert(decimal(5,2),case when p.DateOfBirth is not null and p.DateOfBirth <> ''
                                        then datediff(mm,p.DateOfBirth,@lastofmonth) / 12.0											
							      else null							
			                      end),0,1)	,	
       p.Gender,e.StatusCode,e.EligEffective,e.EligCancellation,           -- line 1
       pr.QualifyingStartDate,pr.QualifyingEndDate,Case when e.StatusCode ='TN' then e.TNHoursWorked else e.TargetHoursWorked end,
       BankOfHours,HoursWorkedDataMonth = 0,
       '' coveragetype, '' CoverageSubType,  -- line 2
       m.HCO MedicalPlanCode,case when m.groupnumber != '9702380NY0'  then md.Name 
                             else rtrim(md.Name)+'(Hospital Only)' end MedicalPlanDescription,
       d.hco DentalPlanCode,dd.name DentalPlanDescription,
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CC') or m.groupnumber = '9702380NY0' )
              then '' else 'VS' end VisionPlanCode,
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CC') or m.groupnumber = '9702380NY0' )
              then '' else 'Vision Service Plan' end VisionPlanDescription,
       case when (e.StatusCode in ('PT','NI','CI' ) or m.groupnumber = '9702380NY0') then 'No' else 'Yes' end RxCoverage,  -- line 3
       case when (e.StatusCode in ('PT','NI','CI' ,'TD' ,'CN','CC') or m.groupnumber = '9702380NY0') then 'No' else 'Yes' end LifeInsFlag,
       case when (m.hco in ('bc','bn','c9','n9','c1','b2') and m.groupnumber != '9702380NY0')
                 or e.ssn in (select distinct ssn from dboutbound.dbo.eap) then 'Yes' else 'No' end BehavioralHealthCoverage,
       0 NumberOfEligDep,pa.Address1,pa.Address2,pa.City,pa.State,pa.Zip,
       Case when pa.ReturnedMailDate is not NUll or pa.ReturnedMailDate <> '' then 'Yes' else 'No' end ReturnedMailFlag,
       Case When exists (select 1 from CLAIMS_SERVER.MPIClmData.dbo.MptfServiceArea m 
                         where m.zip = left(pa.Zip,5) 
                         and @rundate between m.effdate and m.termdate) then 'Yes' 
       else 'No' end InMPTFServArea,               -- line 4
       Case When ForeignAddress = 1 then 'Yes' else 'No' end ForeignFlag,
       getdate(),suser_sname(),getdate(),suser_sname(),getdate(),
       case when statuscode in ('CE','CT','TN','NE') then 'ByHours' 
            when statuscode in ('BH','TM','TD','DP') then 'ByExtension' 
            when statuscode in ('cc','cn') then 'COBRA' 
            when statuscode in ('rt','sv') then 'Ret/Sur' 
            else 'Not Eligible' end EligibilityType,
       case when e.statuscode in ('cc','td') then 'Partial'
            when m.groupnumber = '9702380NY0'  then 'Partial'
            when statuscode in ('CE','CT','TN','NE','BH','TM','DP','rt','sv','cn') then 'Full' 
       else '' end BenefitsCoverage
-- declare @rundate datetime set @rundate = '1/1/2009' select * 
from hedb..eligibility e
    left outer join period pr on e.eligeffective = pr.eligiblestartdate
    inner join pid..person p on e.ssn = p.ssn
        and e.eligcancellation >= @FirstOfMonth and e.eligeffective <= @lastofmonth and e.statuscode in ('PT','NI','CI')
	left outer join pid..personaddress pa on p.personid = pa.personid
	left outer join hedb..hcoenrollment m on p.ssn = m.ssn 
        and m.enddate >= @FirstOfMonth and m.startdate <= @lastofmonth
	inner join hedb..hcodetail md on m.hco = md.hco and md.HCOType = 'M' 
	left outer join hedb..hcoenrollment d on p.ssn = d.ssn
        and d.enddate >= @FirstOfMonth and d.startdate <= @lastofmonth
	inner join hedb..hcodetail dd on d.hco = dd.hco and dd.HCOType = 'D'
where e.ssn in (select ssn from @workhistory where datamonth = @datamonth)
and   e.ssn not in (select ssn from RptParticipantEligData_monthly where datamonth = @datamonth)

set @msg = 'Work History only participants added ' +  cast(getdate() as char)
if @debug = 1 print @msg

-- update coveragesubtype

update RptParticipantEligData_monthly
set    CoverageSubType = case when coveragetype in ('retiree','cobra') and AgeAtDataMonth >=65 then 'Medicare' 
                              when coveragetype in ('retiree','suvivor') and AgeAtDataMonth < 65 then 'PreMedicare' 
                         else '' end
where datamonth = @datamonth

set @msg = 'Coverage sub type updated ' +  cast(getdate() as char)
if @debug = 1 print @msg

update rt set rt.ssn_dual = dualssn
from   RptParticipantEligData_monthly rt 
join dualeligibility d on d.ssn = rt.ssn and rt.datamonth = @datamonth and @rundate between d.fromdate and d.todate and d.ssn <> d.dualssn

set @msg = 'Dual SSN populated ' +  cast(getdate() as char)
if @debug = 1 print @msg

--Set Employer
update rt
set rt.LatestEmployerNo = r.EmpAccountNo
   ,rt.LatestEmployerName = (select EmployerName from eadb..employer where employerid = r.EmpAccountNo)
from RptParticipantEligData_monthly rt
inner join (select max(r1.reportid) reportid, ssn
		from eadb..hours h1 join eadb..report r1 on h1.reportid = r1.reportid
        where r1.todate between @FirstOfMonth and @lastofmonth
		group by ssn 
		) h on rt.ssn = h.ssn
inner join eadb..report r on h.reportid = r.reportid
and datamonth = @datamonth

set @msg = 'Employer data updated ' +  cast(getdate() as char)
if @debug = 1 print @msg

update rt
set rt.LatestEmployerNo = r.EmpAccountNo
   ,rt.LatestEmployerName = (select EmployerName from eadb..employer where employerid = r.EmpAccountNo)
from RptParticipantEligData_monthly rt
inner join (select max(r1.reportid) reportid, ssn
		from eadb..hours h1 join eadb..report r1 on h1.reportid = r1.reportid
        where r1.todate <= @rundate
		group by ssn 
		) h on rt.ssn = h.ssn
inner join eadb..report r on h.reportid = r.reportid and rt.datamonth = @datamonth
where rt.LatestEmployerNo is null
and   datamonth = @datamonth

set @msg = 'Employer data updated for non employed in the month ' +  cast(getdate() as char)
if @debug = 1 print @msg

update rt 
set HoursWorkedDataMonth = w.hours
   ,MonthlyHoursGroup = Case when w.hours >= 0 and w.hours < 1 then '0'
							 when w.hours >= 1 and w.hours < 40 then '1-39.9'
                             when w.hours >= 40 and w.hours < 80 then '40-79.9'
							 when w.hours >= 80 and w.hours < 120 then '80-119.9'
                             when w.hours >= 120 and w.hours < 140 then '120-139.9'
                             when w.hours >= 140 then '140 or higher'
                        else '' end
from   RptParticipantEligData_monthly rt 
join   @workhistory w on w.ssn = rt.ssn and w.datamonth = rt.datamonth

set @msg = 'Work History (Health Hours) data updated '  + cast(getdate() as char)
if @debug = 1 print @msg

-- Dependents
insert RptDependentEligData_monthly
      (PersonID,DataMonth,SSN,DepSSN,dependentcode,LastName,FirstName,DateOfBirth,AgeAtDataMonth,Gender,MaritalStatus,Statuscode_part,Statuscode,
       EligEffective ,EligCancellation,CoverageType,CoverageSubType,MedicalPlanCode,MedicalPlanDescription,DentalPlanCode,DentalPlanDescription ,
       VisionPlanCode,VisionPlanDescription,LifeInsFlag,RxCoverage,BehavioralHealthCoverage,Address1,Address2,City,State,Zip,
       ReturnedMailFlag,InMPTFServArea,ForeignFlag,createdate,createid,LastUpdate,Updateid,Date_Extract,EligibilityType,BenefitsCoverage)
select distinct p.personid,rt.datamonth,rt.SSN,ed.DepSSN,d.dependentcode,p.LastName,p.FirstName,p.DateOfBirth,
       round(convert(decimal(5,2),case when p.DateOfBirth is not null and p.DateOfBirth <> ''
                                        then datediff(mm,p.DateOfBirth,@lastofmonth) / 12.0											
							      else null							
			                      end),0,1)	,p.Gender,	'' MaritalStatus,rt.statuscode, ed.statuscode,   -- line1
       ed.eligeffective ,ed.EligCancellation,rt.CoverageType,
       case when rt.coveragetype in ('retiree','cobra','survivor')
             and round(convert(decimal(5,2),case when p.DateOfBirth is not null and p.DateOfBirth <> ''
                                                      then datediff(mm,p.DateOfBirth,@lastofmonth) / 12.0											
							                else null end),0,1) >=65 and ed.statuscode = 'ed' then 'Medicare' 
            when rt.coveragetype in ('retiree','Survivor') 
             and round(convert(decimal(5,2),case when p.DateOfBirth is not null and p.DateOfBirth <> ''
                                                      then datediff(mm,p.DateOfBirth,@lastofmonth) / 12.0											
							                else null end),0,1) < 65 and ed.statuscode = 'ed' then 'PreMedicare' 
       else '' end CoverageSubType,
       rt.MedicalPlanCode,rt.MedicalPlanDescription,
       case when (d.dependentcode != 'c2' or (d.dependentcode = 'c2'and  exists (select 1 from hedb..DependentCodeHistory dh 
                                                                                 where dh.SSN = d.SSN and dh.DepSSN = d.DepSSN
                                                                                 and dh.Dependentcode <> 'C2' and dh.changereason <> 'error' 
                                                                                 and @runDate between dh.EffectiveDate and dh.CancellationDate ))
                                          or exists(select * from pmt.dbo.subcobrachecks 
                                                    where partssn = ed.ssn and accountno = ed.depssn 
                                                    and @rundate between effdate and termdate))
                then rt.DentalPlanCode else 'ND' end DentalPlanCode,
       case when (d.dependentcode != 'c2' or (d.dependentcode = 'c2'and  exists (select 1 from hedb..DependentCodeHistory dh 
                                                                                 where dh.SSN = d.SSN and dh.DepSSN = d.DepSSN
                                                                                 and dh.Dependentcode <> 'C2' and dh.changereason <> 'error' 
                                                                                 and @runDate between dh.EffectiveDate and dh.CancellationDate ))
                                          or exists(select * from pmt.dbo.subcobrachecks 
                                                    where partssn = ed.ssn and accountno = ed.depssn 
                                                    and @rundate between effdate and termdate))
                then rt.DentalPlanDescription else 'Not Enrolled Dental' end DentalPlanDescription, -- line2
       case when (d.dependentcode != 'c2' or (d.dependentcode = 'c2'and  exists (select 1 from hedb..DependentCodeHistory dh 
                                                                                 where dh.SSN = d.SSN and dh.DepSSN = d.DepSSN
                                                                                 and dh.Dependentcode <> 'C2' and dh.changereason <> 'error' 
                                                                                 and @runDate between dh.EffectiveDate and dh.CancellationDate ))
                                          or exists(select * from pmt.dbo.subcobrachecks 
                                                    where partssn = ed.ssn and accountno = ed.depssn 
                                                    and @rundate between effdate and termdate))
                then rt.VisionPlanCode else '' end VisionPlanCode,
       case when (d.dependentcode != 'c2' or (d.dependentcode = 'c2'and  exists (select 1 from hedb..DependentCodeHistory dh 
                                                                                 where dh.SSN = d.SSN and dh.DepSSN = d.DepSSN
                                                                                 and dh.Dependentcode <> 'C2' and dh.changereason <> 'error' 
                                                                                 and @runDate between dh.EffectiveDate and dh.CancellationDate ))
                                          or exists(select * from pmt.dbo.subcobrachecks 
                                                    where partssn = ed.ssn and accountno = ed.depssn 
                                                    and @rundate between effdate and termdate))
                then rt.VisionPlanDescription else '' end VisionPlanDescription,
       '' LifeInsFlag,rt.RxCoverage,case when rt.statuscode <> 'td' then rt.BehavioralHealthCoverage else 'No' end,
       case when pa.modifydate >= '11/30/2010' then pa.Address1 else rt.address1 end,
       case when pa.modifydate >= '11/30/2010' then pa.Address2 else rt.address2 end,
       case when pa.modifydate >= '11/30/2010' then pa.City else rt.city end,
       case when pa.modifydate >= '11/30/2010' then pa.State else rt.state end,
       case when pa.modifydate >= '11/30/2010' then pa.Zip else rt.zip end,  -- line 3
       Case when pa.ReturnedMailDate is not NUll or pa.ReturnedMailDate <> '' then 'Yes' else 'No' end ReturnedMailFlag,
       Case When exists (select 1 from CLAIMS_SERVER.MPIClmData.dbo.MptfServiceArea m 
                         where m.zip = left(case when pa.modifydate >= '11/30/2010' then pa.Zip else rt.zip end,5) 
                         and @rundate between m.effdate and m.termdate) then 'Yes' 
       else 'No' end InMPTFServArea,
       Case When ForeignAddress = 1 then 'Yes' else 'No' end ForeignFlag,
       getdate(),suser_sname(),getdate(),suser_sname(),getdate(),
       case when ed.statuscode <> 'id' then rt.EligibilityType else 'Not Eligible' end,
       case when ed.statuscode <> 'id' then rt.BenefitsCoverage  else '' end              -- line 4
from   hedb..dependenteligibility ed join dependent d on d.ssn = ed.ssn and d.depssn = ed.depssn
       join  RptParticipantEligData_monthly rt on rt.ssn = ed.ssn and rt.datamonth = @datamonth
		and  ed.eligcancellation >= @FirstOfMonth and ed.eligeffective <= @lastofmonth
       inner join pid..person p on ed.depssn = p.ssn
	   left outer join pid..personaddress pa on p.personid = pa.personid

set @msg = 'Dependents added '  + cast(getdate() as char)
if @debug = 1 print @msg

-- updating BenefitsCoverage for dependents
update RptDependentEligData_monthly
set    BenefitsCoverage = case when BenefitsCoverage = 'Partial' then BenefitsCoverage
                               when BenefitsCoverage = 'full' and dependentcode = 'c2' and DentalPlanCode = 'nd' then 'Partial'
                         else BenefitsCoverage end
where datamonth = @datamonth

set @msg = 'Dependents BenefitsCoverage updated. '  + cast(getdate() as char)
if @debug = 1 print @msg

/*
-- update coveragesubtype

update RptDependentEligData_monthly
set    CoverageSubType = case when coveragetype in ('retiree','cobra') and AgeAtDataMonth >=65 and statuscode = 'ed' then 'Medicare' 
                              when coveragetype = 'retiree' and AgeAtDataMonth < 65 and statuscode = 'ed' then 'PreMedicare' 
                         else '' end
where datamonth = @datamonth
*/
--Set dual spouses only
update d
set SSN_Dual = p.SSN
--SELECT p.ssn_dual,  p.ssn, d.depssn, p.datamonth, d.datamonth
FROM RptParticipantEligData_monthly p
inner join RptDependentEligData_monthly d on p.ssn_dual = d.depssn and p.datamonth = d.datamonth
where p.datamonth = @DataMonth

-- Set dual children flag
update RptDependentEligData_monthly 
   set dualflag = 'Yes'
where  depssn in (select depssn from RptDependentEligData_monthly where statuscode = 'ed' and datamonth = @datamonth group by depssn having count(*) > 1)
	   and datamonth = @datamonth
	   
set @msg = 'Dual Elig Dependents flagged. '  + cast(getdate() as char)
if @debug = 1 print @msg

delete RptDependentEligData_monthly
where ssn in (select ssn from RptParticipantEligData_monthly rt
              where  statuscode = 'sv'
              and datamonth = @datamonth
              and    not exists(select 1 from RptDependentEligData_monthly where ssn = rt.ssn and statuscode = 'ed' and datamonth = @datamonth))
and datamonth = @datamonth

delete RptParticipantEligData_monthly
where  statuscode = 'sv'
and    ssn not in (select ssn from RptDependentEligData_monthly where datamonth = @datamonth)
and    datamonth = @datamonth

set @msg = 'SV without elig dependents deleted '  + cast(getdate() as char)
if @debug = 1 print @msg

update RptParticipantEligData_monthly 
set    ssn_dual = isnull(ssn_dual,''),
       maritalstatus = isnull(maritalstatus,''),
       unioncode = isnull(unioncode,''),
       unionname = isnull(unionname,''),
       affiliation = isnull(affiliation,''),
       agroup = isnull(agroup,''),
       address2 = isnull(address2,''),
       LatestEmployerNo = isnull(LatestEmployerNo,''),
       LatestEmployerName = isnull(LatestEmployerName,''),
       MonthlyHoursGroup = isnull(MonthlyHoursGroup,''),
       TotalFamilyEligibles = isnull(TotalFamilyEligibles,0),
       phone = isnull(phone,''),
       email = isnull(email,'')
where  datamonth = @datamonth

update RptDependentEligData_monthly 
set    maritalstatus = isnull(maritalstatus,''),
       address2 = isnull(address2,''),
       DualFlag = isnull(DualFlag,''),
       SV_Participant = isnull(SV_Participant,'')
where  datamonth = @datamonth

set @msg = 'Nulls reset '  + cast(getdate() as char)
if @debug = 1 print @msg

update RptDependentEligData_monthly set sv_participant = 'Yes'
where  statuscode_part = 'sv'
and    dependentcode in ('ss','sp','dp')
and    statuscode = 'ed'
and    datamonth = @datamonth

update RptDependentEligData_monthly 
set sv_participant = case when dbo.fn_isOldestSSChild(ssn,depssn,@rundate) = 'y' then 'Yes' else 'No' end
where  statuscode_part = 'sv'
and    dependentcode not in ('ss','sp','dp')
and    statuscode = 'ed'
and    ssn not in (select ssn from RptDependentEligData_monthly where sv_participant = 'Yes' and datamonth = @datamonth)
and    datamonth = @datamonth

set @msg = 'SV Participant updated '  + cast(getdate() as char)
if @debug = 1 print @msg

-- Family updated
update rt 
set    rt.NumberOfEligDep = IsNull(d.cnt,0) ,
       rt.FamilyType = case when IsNull(d.cnt,0) = 0 then 'Single'
                            when IsNull(d.cnt,0) = 1 and exists(select 1 from RptDependentEligData_monthly 
                                                      where ssn = d.ssn 
                                                      and datamonth = d.datamonth
                                                      and statuscode = 'ed' 
                                                      and dependentcode in ('sp','dp')) 
                               then 'Two-Party family'
                            when IsNull(d.cnt,0) = 1 and exists(select 1 from RptDependentEligData_monthly 
                                                      where ssn = d.ssn 
                                                      and datamonth = d.datamonth
                                                      and statuscode = 'ed' 
                                                      and dependentcode not in ('sp','dp')) 
                               then 'Family'   -- elig part and child
							else 'Family'
                       end,
       rt.TotalFamilyEligibles = isnull(d.cnt,0)+1
from   RptParticipantEligData_monthly rt
left outer join   (select ssn,datamonth, count(*) cnt 
        from RptDependentEligData_monthly where  statuscode = 'ed'
        group by ssn,datamonth,statuscode_part) d
on     rt.ssn = d.ssn and rt.datamonth = d.datamonth
where  rt.statuscode not in ('pt','ci','ni','sv','dp')

--FamilyType for Survivors/ DP
update rt 
set    rt.NumberOfEligDep = IsNull(d.cnt,0) ,
       rt.FamilyType = case when IsNull(d.cnt,0) = 1 and exists(select 1 from RptDependentEligData_monthly 
                                                      where ssn = d.ssn 
                                                      and datamonth = d.datamonth
                                                      and statuscode = 'ed' 
                                                      and dependentcode in ('sp','dp','ss')) 
                               then 'Single Spouse'
                            when IsNull(d.cnt,0) = 1 and exists(select 1 from RptDependentEligData_monthly 
                                                      where ssn = d.ssn 
                                                      and datamonth = d.datamonth
                                                      and statuscode = 'ed' 
                                                      and dependentcode not in ('sp','dp','ss')) 
                               then 'Family'   -- elig part and child
							else 'Family'
                       end,
       rt.TotalFamilyEligibles = d.cnt
from   RptParticipantEligData_monthly rt
left outer join   (select ssn,datamonth, count(*) cnt 
        from RptDependentEligData_monthly where  statuscode = 'ed'
        group by ssn,datamonth,statuscode_part) d
on     rt.ssn = d.ssn and rt.datamonth = d.datamonth
where  rt.statuscode in ('sv','dp')

set @msg = 'Family type updated '  + cast(getdate() as char)
if @debug = 1 print @msg

-- update phone and email

update rt 
   set rt.phone = pc.homephone,
       rt.email = isnull(pc.email1,'')
from   RptParticipantEligData_monthly rt
left outer join pid.dbo.personcontact pc on rt.personid = pc.personid and rt.datamonth = @datamonth

--special case to remove mulitiple elig records for this SV participant	

select top 1 * 
into #RptDependentEligData_monthly
from RptDependentEligData_monthly where ssn = '100281916'
and   dentalplancode <> 'nd'
and datamonth = @datamonth
 
delete from RptDependentEligData_monthly where ssn = '100281916' and datamonth = @datamonth

insert RptDependentEligData_monthly
select * from #RptDependentEligData_monthly

select top 1 * 
into  #RptParticipantEligData_monthly
from  RptParticipantEligData_monthly 
where ssn = '100281916' 
and datamonth = @datamonth 
and dentalplancode <> 'nd'

delete RptParticipantEligData_monthly 
where ssn = '100281916' 
and datamonth = @datamonth 

insert RptParticipantEligData_monthly
select * from #RptParticipantEligData_monthly 

set @msg = 'SSN of 100281916 handled. '  + cast(getdate() as char)
if @debug = 1 print @msg

drop table #RptParticipantEligData_monthly,#RptDependentEligData_monthly

end
--
--/*
--1.	view vw_MMillerReq11102011Ptp. This view should select all fields from table RptParticipantEligData_monthly
--2.	view vw_MMillerReq11102011Dep. This view should select all fields from table RptDependentEligData_monthly 
--*/
--
--create view vw_MMillerReq11102011Ptp
--as 
--select *
--from   RptParticipantEligData_monthly
--
--create view vw_MMillerReq11102011Dep
--as
--select *
--from   RptDependentEligData_monthly
--
--grant select on vw_MMillerReq11102011Dep to [mpidom\calejos]



------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[spr_EligPartsNDepsAsOfDt]    Script Date: 01/24/2012 10:39:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[spr_EligPartsNDepsAsOfDt]
	(
	@Date	smalldatetime = getdate
	,@MemType	char(1) = 'A'	--A: All; P: Participants; D: -Dependents
	)
as
Begin
	declare @AsOfDate	smalldatetime		
	
		if @Date is null
			set @Date = getdate()
			
		if isdate(@Date) = 0 
			begin
				raiserror ('Invalid date supplied',10,5)
				return
			end
		
		select	@AsOfDate	= @Date			
	
	if exists (select 1 from tempdb.dbo.sysobjects where id = object_id('tempdb.dbo.#Parts08312011') and type = 'U')
		drop table #Parts08312011
	if exists (select 1 from tempdb.dbo.sysobjects where id = object_id('tempdb.dbo.#tmpChild') and type = 'U')
		drop table #tmpChild
	if exists (select 1 from tempdb.dbo.sysobjects where id = object_id('tempdb.dbo.#Deps08312011') and type = 'U')
		drop table #Deps08312011
	
	-----All the Eligible participants---------------------------------------------------------------------
	select distinct e.SSN 
	,r.Person_Id PID
	,IsDeceased= case when p.DateOfDeath is null then 'N'
					else 'Y'
			  end
	,P.DateOfBirth
	,p.SexCode
	,e.StatusCode
	,Status = case when e.StatusCode = 'RT' then 'Retiree'
					when e.StatusCode = 'SV' then 'Deceased-SV'
					when e.StatusCode = 'DP' then 'Deceased-DP'
					when e.StatusCode in ('CC', 'CN') then 'Cobra'
					else 'Active'
			  end
	,m.Name MedicalName
	,d.Name DentalName
	,NoOfHours = convert(real,0)
	,HasSpouse = 'N'	
	,SpouseCode = convert(char(2),'')
	,HasChildren = 'N'
	,NoOfChildren = convert(tinyint,0)
	,DualScenario = convert(varchar(20),'')
	into #Parts08312011
	from Eligibility e (nolock)	
	inner join Person p (nolock) on e.SSN = p.SSN
	left outer join opus.dbo.SGT_PERSON r (nolock) on p.SSN = r.SSN
	
	left outer join hcoenrollment hm (nolock) on e.ssn = hm.ssn
		and @AsOfDate between hm.startdate and hm.enddate
	inner join hcodetail m (nolock) on hm.hco = m.hco and m.hcotype = 'M' --and hm.HCO <> 'NM'

	left outer join hcoenrollment hd (nolock) on e.ssn = hd.ssn
		and @AsOfDate between hd.startdate and hd.enddate
	inner join hcodetail d (nolock) on hd.hco = d.hco and d.hcotype = 'D'

	where @AsOfDate between e.EligEffective and e.EligCancellation
	and e.StatusCode not in ('PT', 'CI', 'NI')

	-------------------------------------------------------------------------------------------------------
	/*
	--Make sure that there are no overlaps (it should not happen).
	Select * from #Parts08312011 r
		where exists (select 1 from #Parts08312011 s 
						where r.SSN = s.SSN 
						and s.StatusCode <> r.StatusCode
					)
					
	--Make sure that there are no duplicates (it should not happen).
	Select SSN, count(*) from #Parts08312011 r
		group by SSN
		having count(*) > 1		
	*/
		
	print 'Delete this duplicate record, there is a data issue with this SSN'
	delete from #Parts08312011 where SSN = '100281916' and DentalName = 'Not Enrolled Dental'
	--select * from #Parts08312011 where SSN = '100281916'
	-------------------------------------------------------------------------------------------------------
	--Participant's Spouse data
	Update t set HasSpouse = 'Y', SpouseCode = c.DependentCode
	from #Parts08312011 t	
	inner join DependentEligibility d (nolock) on t.SSN = d.SSN
	left outer join Dependent c (nolock) on d.SSN = c.SSN and d.DepSSN = c.DepSSN
	where @AsOfDate between d.EligEffective and d.EligCancellation	
	and d.StatusCode = 'ED'
	and c.DependentCode in ('SP', 'SS', 'DS', 'DP')			
	-------------------------------------------------------------------------------------------------------
	----Participant's Dependents data
	select t.SSN, NoOfChildren	= count(*)
		into #tmpChild
	--Update t set HasChildren = 'Y', NoOfChildren	= count(*)
	from #Parts08312011 t	
	inner join DependentEligibility d (nolock) on t.SSN = d.SSN
	left outer join Dependent c (nolock) on d.SSN = c.SSN and d.DepSSN = c.DepSSN
	where @AsOfDate between d.EligEffective and d.EligCancellation	
	and d.StatusCode = 'ED'
	and c.DependentCode not in ('SP', 'SS', 'DS', 'DP')
	group by t.SSN
	
	--select * from #tmpChild	
	Update t 
		set HasChildren = 'Y', NoOfChildren	= c.NoOfChildren
	from #Parts08312011 t	
	inner join #tmpChild c on t.SSN = c.SSN		
	
	-----Delete Surviving participants who do not have surviving dependents-----------------------------
	Delete t
	--select * 
		from #Parts08312011 t
		where StatusCode in ('SV', 'DP')
		and HasSpouse = 'N' and HasChildren = 'N'
	--Examples '137098333 ','331078163 ','087052286 '
	
	-----Participant's Hours worked-------------------------------------------------------------------------
	--Update t set t.NoOfHours = h.Hours
	--from #Parts08312011 t	
	--inner join (select w.SSN, sum(w.Hours) Hours 
	--				from WorkHistory w (nolock) 
	--				inner join #Parts08312011 p on w.SSN = p.SSN 
	--				where w.EndDate >= @HrsFromDt and w.StartDate <= @HrsToDt
	--				group by w.SSN) h on t.SSN = h.SSN
	
	--select * from WorkHistory 
	-------------------------------------------------------------------------------------------------------
	----Dependents details of the above participants
	-- select t.SSN, t.StatusCode, HasSpouse = 'Y', SpouseCode	= c.DependentCode
	select 
		t.SSN PartSSN
		,t.StatusCode PartStatusCode
		,PartStatus = case when t.StatusCode = 'RT' then 'Retiree'
					when t.StatusCode = 'SV' then 'Deceased-SV'
					when t.StatusCode = 'DP' then 'Deceased-DP'
					when t.StatusCode in ('CC', 'CN') then 'Cobra'
					else 'Active'
			  end
		,d.DepSSN
		,c.DependentCode
		,p.DateOfBirth
		,p.SexCode
		,r.PersonId DepPID
		,s.PersonId PartPID
		,t.MedicalName
		,t.DentalName
		,DualScenario = convert(varchar(20),'')
	into #Deps08312011
	from #Parts08312011 t	
	inner join DependentEligibility d on t.SSN = d.SSN
	inner join Person p on d.DepSSN = p.SSN
	left outer join opus.dbo.SGT_PERSON r (nolock) on p.SSN = r.SSN
	left outer join opus.dbo.SGT_PERSON s (nolock) on d.SSN = s.SSN
	left outer join Dependent c on d.SSN = c.SSN and d.DepSSN = c.DepSSN
	where @AsOfDate between d.EligEffective and d.EligCancellation	
	and d.StatusCode = 'ED'
	
	
	print 'Make sure that there are no duplicates (it should not happen).'
	/*
	select getdate() 'Check for Dependent duplicates'
	Select DepSSN, PartSSN, count(*) from #Deps08312011 r
		group by DepSSN, PartSSN	
		having count(*) > 1	
	*/
	--select * from Eligibility_PID_Reference where SSN = '510722294'
	--select * from PID.dbo.Person where SSN = '510722294'
	--select SSN from PID.dbo.Person group by SSN having count(*) > 1	
	--select * from #Deps08312011 where DepSSN in ('610425632', '510722294', '622477949') order by DepSSN
	
	/*
	Select DepSSN, count(*) from #Deps08312011 r
		group by DepSSN
		having count(*) > 2		
	*/
	print 'Delete this duplicate record, there is a data issue with this DepSSN'
	
	select distinct * into #tmpDelDup 
		from #Deps08312011 where DepSSN = '110288231' and PartSSN = '100281916'
		
	delete t
		from #Deps08312011 t
		inner join #tmpDelDup d on t.PartSSN = d.PartSSN and t.DepSSN = d.DepSSN
	
	insert into #Deps08312011
		select * from #tmpDelDup
		
	drop table #tmpDelDup
	--select * from #Deps08312011 where DepSSN = '110288231'
	
	print 'Update Dual Scenarios'
	--------------------------------------------------------------------------------------
	--Parts
	Update p
		set p.DualScenario = 'Dual Participant'
	--select p.SSN
	from #Parts08312011 p	
	where exists (select 1 
					from #Deps08312011 d1					
					where p.SSN = d1.DepSSN
					and d1.PartSSN in (select DepSSN from #Deps08312011)
				  )
	--Test the above data with DualEligibility table.			  
	
	---???????? (discuss with Elig. business team. Looks like the following category is half dual.)
	--Parts	  
	Update p
		set p.DualScenario = 'Also a Dependent'
	--select p.SSN
	from #Parts08312011 p	
	where exists (select 1 
					from #Deps08312011 d1					
					where p.SSN = d1.DepSSN					
				  )
	and p.DualScenario = ''
	--select * from #Parts08312011 where DualScenario = 'Also a Dependent'
	
	--Deps	
	Update d
		set d.DualScenario = 'Dual Dependent'
	--select d.DepSSN
	from #Deps08312011 d	
	where DepSSN in (select DepSSN 
					from #Deps08312011 d1					
					group by DepSSN having count(*) > 1
				  )
	--and DepSSN not in (select SSN from #Parts08312011)
	
	--Deps	
	Update d
		set d.DualScenario = 'Also a Participant'
	--select d.DepSSN
	from #Deps08312011 d	
	where DepSSN in (select SSN 
					from #Parts08312011 d1			
				  )
	and d.DualScenario = '' 
	--order by DepSSN
	
	/*---------Testing Duals----------------------------------------------------------------
	select * from DualEligibility where SSN in (
	select p.SSN
	from #Parts08312011 p	
	where exists (select 1 
					from #Deps08312011 d1					
					where p.SSN = d1.DepSSN
					and d1.PartSSN in (select DepSSN from #Deps08312011)
				  )
	)
	and '06/30/2011' between FromDate and ToDate
	---------------------------------------------------------------------------*/

	-------------------------------------------------------------------------------------------------------
		
	/*
	print getdate() 'Participant counts by StatusCodes'	
	
	--Retiree counts
	select TotalRetirees = count(*)
			,Singles = sum(case when HasSpouse = 'N' and HasChildren = 'N' then 1 else 0 end)
			,SpouseOnly = sum(case when HasSpouse = 'Y' and HasChildren = 'N' then 1 else 0 end)
			,Families = sum(case when HasChildren = 'Y' then 1 else 0 end)
		from #Parts08312011 t
		where t.StatusCode = 'RT'
		
	--Active counts
	select TotalActives = count(*)
			,Singles = sum(case when HasSpouse = 'N' and HasChildren = 'N' then 1 else 0 end)
			,SpouseOnly = sum(case when HasSpouse = 'Y' and HasChildren = 'N' then 1 else 0 end)
			,Families = sum(case when HasChildren = 'Y' then 1 else 0 end)
		from #Parts08312011 t
		where t.StatusCode in ('TN', 'NE', 'CE', 'BH', 'CT', 'TM', 'TD')
		
	--Cobra counts
	select TotalCobras = count(*)
			,Singles = sum(case when HasSpouse = 'N' and HasChildren = 'N' then 1 else 0 end)
			,SpouseOnly = sum(case when HasSpouse = 'Y' and HasChildren = 'N' then 1 else 0 end)
			,Families = sum(case when HasChildren = 'Y' then 1 else 0 end)
		from #Parts08312011 t
		where t.StatusCode in ('CC', 'CN')	
		
	--Survivor counts
	select TotalSurvivors = count(*)
			--,Singles = sum(case when HasSpouse = 'N' and HasChildren = 'N' then 1 else 0 end)
			,SpouseOnly = sum(case when HasSpouse = 'Y' and HasChildren = 'N' then 1 else 0 end)
			,Families = sum(case when HasChildren = 'Y' then 1 else 0 end)
		from #Parts08312011 t
		where t.StatusCode = 'SV'
		--Make sure that the Surviving Participant should atleast have either a Surviving SP or Child, 
		--otherwise do not count the Participant
		and not (HasSpouse = 'N' and HasChildren = 'N')
		
	--Survivor counts
	select TotalDPs = count(*)
			--,Singles = sum(case when HasSpouse = 'N' and HasChildren = 'N' then 1 else 0 end)
			,SpouseOnly = sum(case when HasSpouse = 'Y' and HasChildren = 'N' then 1 else 0 end)
			,Families = sum(case when HasChildren = 'Y' then 1 else 0 end)
		from #Parts08312011 t
		where t.StatusCode = 'DP'
		
	----------Dependents data-----------------------------------------------------------------------------
	select distinct DualScenario from #Deps08312011
	------------------------------------------------------------------------------------------------------
	select * from #Deps08312011
		where DualScenario like 'Dual%'
	
	*/
	
	------------------------------------------------------------------------------------------------------
	if @MemType = 'A'
		begin
		select * from #Parts08312011 order by SSN
		select * from #Deps08312011 order by DepSSN
		end
	else if @MemType = 'P'
		select * from #Parts08312011 order by SSN
	else if @MemType = 'D'
		select * from #Deps08312011 order by DepSSN
	------------------------------------------------------------------------------------------------------
End


--------------------------------------------------


/****** Object:  StoredProcedure [dbo].[usp_CMSE02QueryFileCreation]    Script Date: 01/13/2012 18:50:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[usp_CMSE02QueryFileCreation]
as
Begin
if exists (select 1 from dbo.sysobjects where xtype = 'U' and id = object_id('_MedXOverE02EligEFile'))
		drop table _MedXOverE02EligEFile
	
	select  RecId = identity(int,1,1),
			Col1 = 
			'E00' + 
			right('00000' + '30272',10) + 
			convert(varchar,getdate(),112) +
			'CA' +
			convert(char(177),'')
		into _MedXOverE02EligEFile

	Insert into _MedXOverE02EligEFile
		(Col1)
		select 	
			'E02' + 
			right('00000' + '30272',10) + 
			convert(char(20),t.LastName) +
			convert(char(12),t.FirstName) +							
			convert(char(1),left(isnull(t.MiddleName,''),1)) +
			convert(varchar,isnull(t.DateOfBirth,'01/01/1900'),112) +
			case when left(isnull(t.SexCode,'M'),1) = 'M' then '1' else '2' end +	--M - 1, F -2 
			convert(char(9),t.SSN) + 
			convert(char(12),'') +		--HICN
			--convert(varchar,t.EligEffective,112) +		
			--convert(varchar,t.EligCancellation,112) +	

			--Insert spaces in stead of dates for Query purpose		
			convert(char(8),'') +	--	EligEffective
			convert(char(8),'') +	--	EligCancellation
			convert(char(1),'Q')  +		--Transaction Type
			convert(char(15),isnull(p.PERSON_ID,''))  +		--Document control number
			convert(char(10),'')  +		--NPlanId (future requirement)
			convert(char(1),'')  +		--NSURANCE Type code
			/*
			convert(char(3),case when t.DataSource = 'DependentEligibility' then 'Dep'
								 when t.DataSource = 'Participant' then 'Sub'
							end)  +		--Person code
			*/
			convert(char(3),'')  +		--Person code
			convert(char(20),'')  +		--RX ID/Policy number
			convert(char(15),'')  +		--RX group number
			convert(char(6),'')  +		--RX bin number
			convert(char(10),'')  +		--RX processor control number (pcn)
			convert(char(18),'')  +		--Toll free number
			convert(char(1),'')  +		--Nertwork benefit indicator
			convert(char(1),'')  +		--Creditable coverage indicator
			convert(char(7),'') 		--Filler					
		from _FinalEligFile_E02 t
		inner join OPUS.dbo.SGT_PERSON p (nolock) on t.SSN = p.SSN
			
	Insert into _MedXOverE02EligEFile
		(Col1)
		select
			'E99' + 
			right('000000' + convert(varchar,count(*)),7) +	--total Record count except 'E00','E99' 
			right('000000' + convert(varchar,0),7) +			--Record count of E01 
			right('000000' + convert(varchar,count(*)),7) +	--Record count of E02
			convert(char(176),'')
		from _FinalEligFile_E02 t

	declare @StrSql varchar(300),
			@DateProcessed varchar(8),
			@FileName varchar(50)

	set @DateProcessed = convert(varchar,getdate(),112)
	set @FileName = 'MXHP891.NONE.COB.M.CB30272.FUTURE.P'

	set @StrSql = 'bcp "SELECT Col1 FROM Hedb.[dbo].[_MedXOverE02EligEFile] order by RecId" queryout \\MPIHealth04\MedicareXOver\E02QueryFiles\Sent\' + @FileName + ' -c -S ' + @@ServerName + ' -T'
	exec master..xp_cmdshell @StrSql

	--copy the file with date extension to done folder for history or archiving.
	set @StrSql = 'Copy \\MPIHealth04\MedicareXOver\E02QueryFiles\Sent\' + @FileName + ' \\MPIHealth04\MedicareXOver\E02QueryFiles\Sent\Done\' + @FileName + '_' + @DateProcessed
	select @StrSql
	exec master..xp_cmdshell @StrSql
End

go

CREATE procedure [dbo].[USP_PID_Person_ins]
(
  @PID								varchar(15)
, @SSN                          	varchar(10)    	 = NULL
, @ParticipantPID					varchar(15)		 = NULL -- This field will have value when adding dependent, it will the partssn
, @EntityTypeCode						varchar(3)		 = 'P'	-- This field have value 'P' for Person Record and 'T' or 'O' for Trust or Organization
, @RelationType						char(1)			 = NULL -- this will have value when adding dependent D-Dependent,B- Beneficiary
-- Person Information
, @FirstName                    	varchar(50) 	 = NULL
, @MiddleName                   	varchar(50) 	 = NULL
, @LastName                     	varchar(50) 	 = NULL
, @Gender                    		char(1)     	 = NULL
, @DateOfBirth                  	datetime    	 = NULL
, @DateOfDeath                  	datetime    	 = NULL
--Contact Info
, @HomePhone						varchar(15)   = NULL  
, @CellPhone							varchar(15)   = NULL  
, @Fax								varchar(15)   = NULL  
, @Email							varchar(50)   = NULL 
-- Others
, @AuditUser                    	varchar(30)   = NULL
)
AS
BEGIN
declare @rtn     	int
declare @ParticipantSSN	varchar(10)
set nocount on

--if relationtype is dependent, check date of birth and gender
declare	@errornumber	int,
	    @errormessage	varchar(255),
	    @TaxID varchar(10)
	

/*For Dependents allow no SSN in OPUS, create next new SSN*/	

if @RelationType in ('B','D')
begin
	If @SSN is Null
	Begin
		exec USP_GetNextTaxId @TaxID output
	
	
		if @TaxID is Null
		Begin
			select @errornumber = 619060
			select @errormessage = 'Error to generate SSN.'
			exec showerror @errornumber, @errormessage
			return @errornumber
		end
	
		select @SSN = @TaxID
	end
End

if @RelationType not in ('B','D') and @SSN is Null
begin
	select @errornumber = 619060
	select @errormessage = 'Participant SSN may not be blank.'
	exec showerror @errornumber, @errormessage
	return @errornumber
end
/*Change for OPUS*/

-- RG 04/10/2009 - If invalid SexCode, replace with ''
if @Gender not in ('','M','F')
begin
	set @Gender = ''
end

if (@EntityTypeCode = 'P') and (@RelationType = 'D')
begin
	if @DateOfBirth is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The Date of Birth may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
	if @Gender is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The gender may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
end

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

select @ParticipantSSN=SSN from Eligibility_PID_Reference where PID = @ParticipantPID

		-- insert the record
		if not exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
		Begin
			insert into Eligibility_PID_Reference(PID,SSN,CreateDate,CreateUser) 
			VALUES (@PID,@SSN,getdate(),@AuditUser)
			if @@error != 0
			begin
				raiserror 99999 'PID insert failed in Eligibility system.'
				return @@error
			end
			
			-- Add Person Record
			if @EntityTypeCode ='P'
			Begin
				exec @rtn = person_ins  @ssn 				= @ssn,
										@sexcode			= @Gender,
										@firstname			= @firstname,
										@middlename 		= @middlename,
										@lastname 			= @lastname,
										@dateofbirth		= @dateofbirth,
										@dateofdeath		= @dateofdeath,
										@Phone1				= @HomePhone,
									    @Phone2				= @Fax   ,
									    @Email				= @Email ,
									    @Mobile				= @CellPhone  ,
									    @AuditUser			= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End

			if @EntityTypeCode in ('T','O')
			Begin
        		EXEC @rtn = Organization_ins	@TaxID	    = @SSN,
        										@Name		= @FirstName,
        										@AuditUser	= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End
		End

		-- This means that person is participant
		if (@ParticipantSSN is not null)
		Begin
			--MM Added code to verify existence in Person table before adding in reference table.
			if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
				and (exists (select 1 from Person where SSN = @SSN) OR exists (select 1 from Organization WHERE Taxid = @SSN))
			BEGIN
				if not exists (select 1 from Elig_PID_NewDependentBeneficiaries
								where SSN = @ParticipantSSN and DepSSN = @SSN 
								and Type = @RelationType)
				  and ((@RelationType ='D' and not exists (select 1 from Dependent where SSN = @ParticipantSSN and DepSSN = @SSN))
						or
					   (@RelationType ='B' and not exists (select 1 from LifeInsBeneficiary where SSN = @ParticipantSSN and BeneSSN = @SSN)))
				Begin
					insert into Elig_PID_NewDependentBeneficiaries
					select @ParticipantSSN,@SSN,@RelationType,getdate(),@AuditUser
					if @@error != 0
					begin
						raiserror 99999 'PID insert failed for new dependent/beneficiaries.'
						return @@error
					end
				End
			END
			ELSE
			begin
					set @errormessage = 'PID '+@PID+' could not be inserted in Eligibility.'
					raiserror 99999 @errormessage 
					return 99999
			end
		End

--  All is good
return 0
END





/****** Object:  StoredProcedure [dbo].[USP_PID_Person_ins]    Script Date: 01/16/2012 17:43:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information
-- RG 04/10/2009 - If invalid SexCode, replace with ''
-- =============================================
ALTER procedure [dbo].[USP_PID_Person_ins]
(
  @PID								varchar(15)
, @SSN                          	varchar(10)    	 = NULL
, @ParticipantPID					varchar(15)		 = NULL -- This field will have value when adding dependent, it will the partssn
, @EntityTypeCode						varchar(3)		 = 'P'	-- This field have value 'P' for Person Record and 'T' or 'O' for Trust or Organization
, @RelationType						char(1)			 = NULL -- this will have value when adding dependent D-Dependent,B- Beneficiary
-- Person Information
, @FirstName                    	varchar(50) 	 = NULL
, @MiddleName                   	varchar(50) 	 = NULL
, @LastName                     	varchar(50) 	 = NULL
, @Gender                    		char(1)     	 = NULL
, @DateOfBirth                  	datetime    	 = NULL
, @DateOfDeath                  	datetime    	 = NULL
--Contact Info
, @HomePhone						varchar(15)   = NULL  
, @CellPhone							varchar(15)   = NULL  
, @Fax								varchar(15)   = NULL  
, @Email							varchar(50)   = NULL 
-- Others
, @AuditUser                    	varchar(30)   = NULL
)
AS
BEGIN
declare @rtn     	int
declare @ParticipantSSN	varchar(10)
set nocount on

--if relationtype is dependent, check date of birth and gender
declare	@errornumber	int,
	    @errormessage	varchar(255),
	    @TaxID varchar(10)
	

/*For Dependents allow no SSN in OPUS, create next new SSN*/	

if @RelationType in ('B','D')
begin
	If @SSN is Null
	Begin
		exec USP_GetNextTaxId @TaxID output
	
	
		if @TaxID is Null
		Begin
			select @errornumber = 619060
			select @errormessage = 'Error to generate SSN.'
			exec showerror @errornumber, @errormessage
			return @errornumber
		end
		
		select @SSN = @TaxID
	End
end

if @RelationType not in ('B','D') and @SSN is Null
begin
	select @errornumber = 619060
	select @errormessage = 'Participant SSN may not be blank.'
	exec showerror @errornumber, @errormessage
	return @errornumber
end
/*Change for OPUS*/

-- RG 04/10/2009 - If invalid SexCode, replace with ''
if @Gender not in ('','M','F')
begin
	set @Gender = ''
end

if (@EntityTypeCode = 'P') and (@RelationType = 'D')
begin
	if @DateOfBirth is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The Date of Birth may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
	if @Gender is null
        begin
          select @errornumber = 619060
	  select @errormessage = 'The gender may not be blank.'
          exec showerror @errornumber, @errormessage
	  return @errornumber
        end
end

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

select @ParticipantSSN=SSN from Eligibility_PID_Reference where PID = @ParticipantPID

		-- insert the record
		if not exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
		Begin
			insert into Eligibility_PID_Reference(PID,SSN,CreateDate,CreateUser) 
			VALUES (@PID,@SSN,getdate(),@AuditUser)
			if @@error != 0
			begin
				raiserror 99999 'PID insert failed in Eligibility system.'
				return @@error
			end
			
			-- Add Person Record
			if @EntityTypeCode ='P'
			Begin
				exec @rtn = person_ins  @ssn 				= @ssn,
										@sexcode			= @Gender,
										@firstname			= @firstname,
										@middlename 		= @middlename,
										@lastname 			= @lastname,
										@dateofbirth		= @dateofbirth,
										@dateofdeath		= @dateofdeath,
										@Phone1				= @HomePhone,
									    @Phone2				= @Fax   ,
									    @Email				= @Email ,
									    @Mobile				= @CellPhone  ,
									    @AuditUser			= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End

			if @EntityTypeCode in ('T','O')
			Begin
        		EXEC @rtn = Organization_ins	@TaxID	    = @SSN,
        										@Name		= @FirstName,
        										@AuditUser	= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End
		End

		-- This means that person is participant
		if (@ParticipantSSN is not null)
		Begin
			--MM Added code to verify existence in Person table before adding in reference table.
			if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
				and (exists (select 1 from Person where SSN = @SSN) OR exists (select 1 from Organization WHERE Taxid = @SSN))
			BEGIN
				if not exists (select 1 from Elig_PID_NewDependentBeneficiaries
								where SSN = @ParticipantSSN and DepSSN = @SSN 
								and Type = @RelationType)
				  and ((@RelationType ='D' and not exists (select 1 from Dependent where SSN = @ParticipantSSN and DepSSN = @SSN))
						or
					   (@RelationType ='B' and not exists (select 1 from LifeInsBeneficiary where SSN = @ParticipantSSN and BeneSSN = @SSN)))
				Begin
					insert into Elig_PID_NewDependentBeneficiaries
					select @ParticipantSSN,@SSN,@RelationType,getdate(),@AuditUser
					if @@error != 0
					begin
						raiserror 99999 'PID insert failed for new dependent/beneficiaries.'
						return @@error
					end
				End
			END
			ELSE
			begin
					set @errormessage = 'PID '+@PID+' could not be inserted in Eligibility.'
					raiserror 99999 @errormessage 
					return 99999
			end
		End

--  All is good
return 0
END


--=======================================
-- Added By: Mahua Banerjee
-- Create date: 01/03/2013
-- Description:	SP to update SSN on HEDB
--========================================

/****** Object:  StoredProcedure [dbo].[USP_PID_Merge]    Script Date: 01/02/2013 23:23:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Vidya Nair
-- Create date: 11/08/2007
-- Description:	SP to call for PID Merge in Eligibility
--
--Modification History
--Mxie 5.2.2008   change @MergeDate's datatype from varchar to datetime.
--                when calling sp_SSNMerge, do not pass @RecdDate with value.
-- =============================================
CREATE PROCEDURE [dbo].[USP_PID_Merge]
		@OldPID			varchar(15),
		@OldSSN			varchar(10),
		@NewPID			varchar(15),
		@NewSSN			varchar(10),
		@Notes			varchar(255),
		@ADH			varchar(255) = null,
		@MergeDate		datetime = null,
		@debug bit = 0

AS
BEGIN
set nocount on

declare   @pid			char(15)     
		, @rtn  		int	
		, @ssn 			varchar(10)
		, @ChangeType		varchar(10)
		, @PartSSN			char(10)
		, @errMsg			varchar(1000)

create table #tmpErrorMsg (ErrMsg  varchar(1000),StopChange bit)

	-- If either of the PIDs exist
	if exists (select 1 from Eligibility_PID_Reference WHERE PID = @OldPID)
	or exists (select 1 from Eligibility_PID_Reference WHERE PID = @NewPID)
	Begin
		if not exists (select 1 from Eligibility_PID_Reference WHERE PID = @OldPID)
		begin
			return 0
		end

		if not exists (select 1 from Eligibility_PID_Reference WHERE PID = @NewPID)
		begin
			-- Add entry in Eligibility_PID_Reference
			insert into Eligibility_PID_Reference values(@NewSSN, @NewPID, null, getdate(), suser_sname(),null, null)
		end

		exec eadb.[dbo].[USP_PID_Merge] @OldPID,	@OldSSN, @NewPID, @NewSSN 

--		-- Now both IDs exist, so do merge
--		if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @OldPID)
--		and exists (select 1 from Eligibility_PID_Reference WHERE PID = @NewPID)
--		Begin	

			-- means Beneficiary				
			if exists (select 1 from LifeInsBeneficiary where BeneSSN = @OldSSN)
				and not exists (select 1 from  Participant where SSN = @OldSSN)  
				and not exists (select 1 from  Dependent where DEPSSN = @OldSSN)  
			Begin

				exec @rtn = sp_BeneficiaryMerge  @OldSSN	= @OldSSN,
												 @NewSSN	= @NewSSN
				if @rtn <> 0
				begin
					raiserror 99999 'Bene SSN Merge Failed.'
					return 99999	
				end
			End

			if exists (select 1 from  Participant where SSN = @OldSSN)
				select @ChangeType ='Part'

			if @ChangeType is null and 
			   exists (select 1 from  Dependent where DepSSN = @OldSSN)
				select @ChangeType ='Dep'

			if @ChangeType is not null
			Begin
				if @changetype ='Part'
				begin
					insert into #tmpErrorMsg
					exec sp_SSNChangeValid @OldSSN = @OldSSN,@NewSSN = @NewSSN,@PartSSN = NULL,@ChangeType = @ChangeType
				end
				else
				begin
					declare crec cursor for select SSN from Dependent where DepSSN = @OldSSN
					open crec
					fetch next from crec into @PartSSN
					while @@fetch_status =0
					begin
						insert into #tmpErrorMsg
						exec sp_SSNChangeValid @OldSSN = @OldSSN,@NewSSN = @NewSSN,@PartSSN = @PartSSN,@ChangeType = @ChangeType	

						fetch next from crec into @PartSSN
					end
					close crec
					deallocate crec	
				end

				if exists (select 1 from #tmpErrorMsg where StopChange =1)
				Begin
					select @errMsg = Errmsg from #tmpErrorMsg where StopChange =1
					raiserror 99999 @errMsg
					return 99999
				End
				Else
				Begin
					 if @changetype ='Part'
					 begin
						exec @rtn = sp_SSNMerge @SSNOld	= @OldSSN,
												@SSNNew	= @NewSSN,
												@Notes = @Notes,
												@ADH	= @ADH,
												-- @RecdDate = @MergeDate,
												@Debug = @Debug

	
						if @rtn <> 0
						begin
							raiserror 99999 'Participant SSN Merge Failed.'
							return 99999	
						end
						EXEC sp_rollover_participant @NewSSN
					 end
					 else
					 begin
						exec @rtn = sp_ChangeDepSSN @DepSSN = @OldSSN,
													@SSN	= @PartSSN,
													@NewSSN	= @NewSSN
						if @rtn <> 0
						begin
							raiserror 99999 'Dependent SSN Merge Failed.'

							return 99999	
						end
					 end
				End
			End
--		End   -- Now both IDs exist, so do merge
	End
		
return 0
END



GO



USE [HEDB]
GO
/****** Object:  StoredProcedure [dbo].[USP_PID_PersonAddress_UPD]    Script Date: 04/06/2013 22:55:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/*-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information

--04/06/2013	Raj		Added code to insert current record into OPUSPersonInfoUpdateActivity table to be used
						to update QXNT data.
--04/19/2013	Raj		Added one more parameter @AddressStartDate. This value is stored in OPUSPersonInfoUpdateActivity 
						to be used in QNXT for raiders.
-- =============================================*/
ALTER procedure [dbo].[USP_PID_PersonAddress_UPD]
(
  @PID								varchar(15)
--Address Information
, @Attention						varchar(30)   = NULL 	
, @Address1							varchar(60)   = NULL 
, @Address2							varchar(60)   = NULL 
, @City								varchar(30)   = NULL 
, @State							varchar(20)   = NULL
, @PostalCode						varchar(10)   = NULL  
, @Country							varchar(25)   = NULL 
, @CountryCode						varchar(3)    = NULL
, @ForeignAddr						bit			  = NULL 
, @ReturnedMail						datetime      = NULL
, @DoNotUpdate						bit			  = NULL 
-- Others
--, @AuditDate                    	datetime      = NULL
, @AuditUser                    	varchar(30)   = NULL
, @ReceivedFrom                     varchar(25)   = NULL
, @AddressStartDate					datetime	  = NULL
)
AS
BEGIN
	declare @rtn     	int	,
			@Name       varchar(150),
			@AuditDateOrg	  datetime
declare @AuditDate datetime
declare @SSN varchar(10)

select @Address1 = convert(varchar(60),ltrim(rtrim(isnull(@Address1,''))) + ' ' + ltrim(rtrim(isnull(@Address2,''))))

select @SSN =SSN from Eligibility_PID_Reference WHERE PID = @PID

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

	select @AuditDate = getdate()



	if exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
	Begin
	   		
		if exists (select 1 from Person WHERE SSN = @SSN)
		Begin
			select @AuditDate = AuditDate from PersonAddress where ssn =@SSN
		
			exec @rtn = Addr_sp_PersonAddress_upd
									  @SSN				= @SSN               
									, @Attention        = @Attention             
									, @Street           = @Address1             
									, @City             = @City             
									, @State			= @State              
									, @PostalCode		= @PostalCode               
									, @Country          = @Country            
									, @CountryCode      = @CountryCode          
									, @ForeignAddr		= @ForeignAddr
									, @DoNotChange      = @DoNotUpdate             
									, @ReturnedMailDate = @ReturnedMail             
									, @AuditDate        = @AuditDate             
									, @AuditUser        = @AuditUser 
									, @ReceivedFrom     = @ReceivedFrom            
			if @rtn != 0
			begin
				return @rtn
			end
		End

		-- Update Organization
		if exists (select 1 from Organization WHERE Taxid = @SSN)
		Begin
				-- Org is present, update addr
			   select  @Name = Name
					 , @AuditDateOrg = AuditDate
				from Organization where TaxID = @SSN

                EXEC @rtn = Organization_upd
        						@TaxID	    = @SSN,
        						@Name		= @Name,
        						@Street		= @Address1,
        						@City		= @City,
        						@State		= @State,
        						@PostalCode	= @PostalCode,
        						@Country	= @Country,
        						@ForeignAddr= @ForeignAddr,
        						@AuditUser	= @AuditUser,
								@AuditDate  = @AuditDateOrg

				if @rtn != 0
				begin
					return @rtn
				end
			
		End
	End

	insert into dbo.OPUSPersonInfoUpdateActivity (PID, SSN, UpdateDate, UpdateUser, ActivityType, Processed, AddressStartDate)
	select @PID, @SSN, GETDATE(), USER_NAME(), 'Address Update', 0, @AddressStartDate

--  All is good
return 0
END
go

USE [hedb]
GO
/****** Object:  StoredProcedure [dbo].[USP_PID_PersonAddress_ins]    Script Date: 04/08/2013 17:17:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/*-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information

--Change history
04/08/2013	Raj		Added code to insert current record into OPUSPersonInfoUpdateActivity table to be used
					to update QXNT data.
04/19/2013	Raj		Added one more parameter @AddressStartDate. This value is stored in OPUSPersonInfoUpdateActivity 
						to be used in QNXT for raiders.
-- =============================================*/
ALTER procedure [dbo].[USP_PID_PersonAddress_ins]
(
  @PID								varchar(15)
, @EntityTypeCode						varchar(3)		 = 'P'	-- This field have value 'P' for Person Record and 'T' or 'O' for Trust or Organization
--Address Information
, @Attention						varchar(30)   = NULL 	
, @Address1							varchar(60)   = NULL 
, @Address2							varchar(60)   = NULL 
, @City								varchar(30)   = NULL 
, @State							varchar(20)   = NULL
, @PostalCode						varchar(10)   = NULL  
, @Country							varchar(25)   = NULL 
, @CountryCode						varchar(3)    = NULL
, @ForeignAddr						bit			  = NULL 
, @ReturnedMail						datetime      = NULL
, @DoNotUpdate						bit			  = NULL 
-- Others
, @AuditUser                    	varchar(30)   = NULL
, @ReceivedFrom                     varchar(25)   = NULL
, @AddressStartDate					datetime	  = NULL
)
AS
BEGIN
	declare @rtn     	int	,
			@Name       varchar(150),
			@AuditDateOrg	  datetime
			,@AuditDate datetime 
declare @SSN	varchar(10)

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

if @AuditDate is null or @AuditDate =''
	select @AuditDate = getdate()

select @SSN = SSN from Eligibility_PID_Reference where PID = @PID

--	select @Address1 = convert(varchar(60),ltrim(rtrim(isnull(@Address1,''))) + ' ' + ltrim(rtrim(isnull(@Address2,''))))

	if exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
	Begin
	   		
		if @EntityTypeCode ='P'
		Begin
			if exists(select 1 from personaddress where ssn = @SSN)
			begin
				exec @rtn = USP_PID_PersonAddress_UPD
					  @PID								= @PID
					, @Attention						= @Attention 	
					, @Address1							= @Address1 
					, @Address2							= @Address2 
					, @City								= @City 
					, @State							= @State
					, @PostalCode						= @PostalCode  
					, @Country							= @Country 
					, @CountryCode						= @CountryCode
					, @ForeignAddr						= @ForeignAddr 
					, @ReturnedMail						= @ReturnedMail
					, @DoNotUpdate						= @DoNotUpdate
					, @AuditUser                    	= @AuditUser
					, @ReceivedFrom                     = @ReceivedFrom
				if @rtn != 0
				begin
					return @rtn
				end
			end
			else
			begin
				select @Address1 = convert(varchar(60),ltrim(rtrim(isnull(@Address1,''))) + ' ' + ltrim(rtrim(isnull(@Address2,''))))

				exec @rtn = Addr_sp_PersonAddress_ins
									  @SSN				= @SSN               
									, @Attention        = @Attention             
									, @Street           = @Address1             
									, @City             = @City             
									, @State			= @State              
									, @PostalCode		= @PostalCode               
									, @Country          = @Country            
									, @CountryCode      = @CountryCode          
									, @ForeignAddr		= @ForeignAddr
									, @DoNotChange      = @DoNotUpdate             
									, @ReturnedMailDate = @ReturnedMail             
									, @AuditDate        = @AuditDate             
									, @AuditUser        = @AuditUser 
									, @ReceivedFrom     = @ReceivedFrom            
				if @rtn != 0
				begin
					return @rtn
				end
			end
		End

		-- Update Organization
		if @EntityTypeCode in ('T','O')
		Begin
				-- Org is present, update addr
			   select  @Name = Name
					 , @AuditDateOrg = AuditDate
				from Organization where TaxID = @SSN

                EXEC @rtn = Organization_upd
        						@TaxID	    = @SSN,
        						@Name		= @Name,
        						@Street		= @Address1,
        						@City		= @City,
        						@State		= @State,
        						@PostalCode	= @PostalCode,
        						@Country	= @Country,
        						@ForeignAddr= @ForeignAddr,
        						@AuditUser	= @AuditUser,
								@AuditDate  = @AuditDateOrg


			
		End

	End
	Else
	Begin
				raiserror 99999 'PID Does not exist in Eligibility'
				return @@error
	End
	
	insert into dbo.OPUSPersonInfoUpdateActivity (PID, SSN, UpdateDate, UpdateUser, ActivityType, Processed, AddressStartDate)
	select @PID, @SSN, GETDATE(), USER_NAME(), 'Person Update', 0, @AddressStartDate
	
--  All is good
return 0
END




















