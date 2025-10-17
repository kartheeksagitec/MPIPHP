-- =============================================
-- HEDB Scripts
-- =============================================
-------------********IMP*******-----------------
---Was not part of the scripts which MPI gave---
-------------********IMP*******-----------------
/****** Object:  StoredProcedure [dbo].[USP_GetNextTaxId]    Script Date: 01/16/2012 17:35:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[USP_GetNextTaxId] (@TaxID varchar(10) output)
as
begin
    declare @ID int 
    select @ID = convert(int,NextTaxId) from IDS
    update IDS set NextTaxId = convert(varchar(10), @ID +1)
    set @TaxID = convert(varchar(10), @ID)
    return @@Error
end



Create table dbo.OPUSPersonInfoUpdateActivity
(
PID				varchar(9)
,SSN			varchar(10)
,UpdateDate		smalldatetime	
,UpdateUser		varchar(50)
,ActivityType	varchar(15)
,Processed		bit	
,AddressStartDate datetime
)
go

Grant insert, select, update on dbo.OPUSPersonInfoUpdateActivity to Eligibility
go


/****** Object:  StoredProcedure [dbo].[Addr_sp_PersonAddress_chk]    Script Date: 03/15/2013 12:55:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*--------------------------------------------------------------------------------------------------------------
--04/06/2006	VN		Added check to ensure that country is not null
--08/22/2006    VN      Added check so that for domestic addr, postalcode and city are present.
--11/28/2006	VN		Added check, so that if State and City are local, but the country is not US give error and stop update.
--03/27/2007	VN		Check EligibilityManager role instead of DeleteUser Role
--02/20/2009	MM		Using new AD role for secure address update.
--03/15/2013	Raj		Removed the alert to check if person updating the address is a member of 
						'MPIDOM\PIDSecureAddressManager' group or not, assuming that this will happen in OPUS frontend.
---------------------------------------------------------------------------------------------------------------*/


ALTER  Procedure [dbo].[Addr_sp_PersonAddress_chk] ( @dbaction varchar(255)
, @SSN		char(10)
, @Attention		varchar(30)
, @Street		varchar(60)
, @City			varchar(30)
, @State		varchar(20)
, @PostalCode		varchar(10)
, @Country		varchar(25)
, @ForeignAddr 		bit
, @ReceivedFrom	varchar(25)
, @DoNotChange	bit
, @ReturnedMailDate	datetime
, @AuditDate		datetime
, @AuditUser		varchar(30)
, @Phone                varchar(15)   = NULL  
, @Fax                  varchar(15)   = NULL  
, @Email                varchar(50)   = NULL  
, @Mobile               varchar(15)   = NULL  
)
AS
BEGIN
declare	@errornumber	int,	@errormessage	varchar(255), @lastuser	varchar(50),	@AuditDateOrig	datetime

-- UPDATE/INSERT SECTION
if @dbaction in ('update','insert')
begin
    if @ssn is null
    begin
        select @errornumber = 620010
        select @errormessage = 'The Ssn number may not be blank'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end

      if not exists(select 1 from Person where SSN = @SSN)
      begin
         select @errornumber = 99999
         select @errormessage = 'Error inserting data into PersonAddress table because SSN does not exist in Person table.'
         exec showerror @errornumber, @errormessage
         return @errornumber
      end
    
    if @ReturnedMailDate > getdate()
    begin
        select @errornumber = 620110
        select @errormessage = 'Returned mail date may not be a date in the future.'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end

    if (((@Street is not null and ltrim(rtrim(@Street)) <> '') or 
         (@City is not null   and  ltrim(rtrim(@City)) <> '' ) or
         (@State is not null  and  ltrim(rtrim(@State)) <> '') or 
         (@PostalCode is not null and  ltrim(rtrim(@PostalCode)) <> ''))
        and (@Country is null or ltrim(rtrim(@Country)) =''))
    begin
        select @errornumber = 620110
        select @errormessage = 'Please select Country.'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end
    
    if (@ForeignAddr =0 and
         (@City is not null   and  ltrim(rtrim(@City)) <> '' ) and
         (@State is not null  and  ltrim(rtrim(@State)) <> '') and
         (@PostalCode is null or  ltrim(rtrim(@PostalCode)) = ''))        
    begin
        select @errornumber = 620110
        select @errormessage = 'Postal Code cannot be blank.'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end

    if (@ForeignAddr =0 and
         (@State  is not null and ltrim(rtrim(@State)) <> '' ) and
         (@PostalCode is not null and ltrim(rtrim(@PostalCode)) <> '') and
         (@City  is null or  ltrim(rtrim(@City )) = ''))        
    begin
        select @errornumber = 620110
        select @errormessage = 'City cannot be blank.'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end

	if (@ForeignAddr =1 and	
		len(ltrim(rtrim(@State))) =2 and
		exists (select 1 from zipcode where state = @state
				and (ltrim(rtrim(@City)) = City
					 or
					 substring(ltrim(rtrim(@PostalCode)),0,5) = substring(ltrim(rtrim(Zip)),0,5)))
		and ltrim(rtrim(@Country)) <> 'United States')
    begin
        select @errornumber = 620110
        select @errormessage = 'Country is incorrect, Please select United States, if Domestic Address.'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end
    
/*
    if (ltrim(rtrim(@Street)) <> '' and (@Country is null or ltrim(rtrim(@Country)) =''))
    begin
        select @errornumber = 620110
        select @errormessage = 'Please select Country'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end

    if ((@Street is null or ltrim(rtrim(@Street)) = '') and 
        (@City is null or ltrim(rtrim(@City)) = '' ) and 
        (@State is null or ltrim(rtrim(@State)) = '') and 
        (@PostalCode is null or ltrim(rtrim(@PostalCode)) = ''))
    begin
        select @errornumber = 620110
        select @errormessage = 'Address cannot be empty'
        exec showerror @errornumber, @errormessage
        return @errornumber
    end
*/
end

--03/15/2013 (Raj, commented the below alert.)
/*-------------------------------
if @dbaction in ('update','delete')
Begin

--02/20/2009	MM		Using new AD role for secure address update.
      --if  IS_MEMBER('EligibilityManager') =0
      if  IS_MEMBER('MPIDOM\PIDSecureAddressManager') =0
         and exists (select 1 from personaddress where ssn = @ssn and DoNotChange =1)
         and (not exists (select 1 from personaddress where ssn = @ssn
                	   and rtrim(coalesce(Attention, ''))	  = rtrim(coalesce(@Attention, ''))
                	   and rtrim(coalesce(Street, ''))        = rtrim(coalesce(@Street, ''))
                	   and rtrim(coalesce(City, ''))	  = rtrim(coalesce(@City, ''))
                	   and rtrim(coalesce(State, ''))         = rtrim(coalesce(@State, ''))
                	   and rtrim(coalesce(Country, ''))       = rtrim(coalesce(@Country, ''))
                	   and rtrim(coalesce(PostalCode, ''))	  = rtrim(coalesce(@PostalCode, ''))
                	   and rtrim(coalesce(ReceivedFrom, ''))  = rtrim(coalesce(@ReceivedFrom, ''))
                	   and DoNotChange 		          = @DoNotChange
                	   and ForeignAddr 		          = @ForeignAddr)
                OR
                not exists (select 1 from person where ssn = @ssn
                	   and rtrim(coalesce(Phone1, ''))	  = rtrim(coalesce(@Phone, ''))
                	   and rtrim(coalesce(Phone2, ''))        = rtrim(coalesce(@Fax, ''))
                	   and rtrim(coalesce(Email, ''))	  = rtrim(coalesce(@Email, ''))
                	   and rtrim(coalesce(Mobile, ''))         = rtrim(coalesce(@Mobile, ''))))

        Begin
                select @errornumber = 620110
                select @errormessage = 'This is a secure Address, you donnot have permission to change secure Address'
                exec showerror @errornumber, @errormessage
                return @errornumber
        End
         
End
-------------------------------*/

-- UPDATE SECTION
if @dbaction IN ('update','delete')
   begin
       -- check to see if record exists
      select @AuditDateOrig = AuditDate, @lastuser = AuditUser from PersonAddress where SSN = @SSN
--       if @AuditDateOrig = NULL
--          begin
--             select @errornumber = 99999
--             select @errormessage = 'Command failed because record does not exist in PersonAddress table.'
--             exec showerror @errornumber, @errormessage
--             return @errornumber
--          end
       -- check to see if record has been modified
      if @AuditDateOrig <> @AuditDate
         begin
            select @errornumber = 99998
            select @errormessage = 'Record has been modified by ' + @lastuser + '.  Refresh your data and try again.' + 'SSN='+@SSN+'AuditDate='+convert(varchar(23),@AuditDAte,121)+'OrigAuditDate='+convert(varchar(23),@AuditDAteOrig,121)
            exec showerror @errornumber, @errormessage
            return @errornumber
         end
   end

--  All is good
return 0
END



/****** Object:  StoredProcedure [dbo].[Addr_sp_PersonAddress_ins]    Script Date: 04/10/2013 18:17:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






/*
	11/28/2006	VN		Set ForiegnAddr to 0, if country is US but ForiegnAddr is still 1
	04/10/2013	Raj		Fixed a bug in selecting Country Description based on the given @CountryCode.
*/

ALTER Procedure [dbo].[Addr_sp_PersonAddress_ins] (
  @SSN                           char(10)      = NULL  
, @Attention                     varchar(30)   = NULL  
, @Street                        varchar(60)  = NULL  
, @City                          varchar(30)   = NULL  
, @State                         varchar(20)   = NULL  
, @PostalCode                    varchar(10)   = NULL  
, @Country                       varchar(25)   = NULL  
, @CountryCode                varchar(3)   = NULL  
, @ForeignAddr     bit   = NULL  
, @DoNotChange                   bit       = NULL  
, @ReceivedFrom                        varchar(25)   = NULL  
, @ReturnedMailDate              datetime      = NULL  
, @AuditDate                     datetime      = NULL  
, @AuditUser                     varchar(30)   = NULL  
)
AS
BEGIN
declare	@errornumber	int, @errormessage  varchar(255), @dbaction varchar(255), @lastuser varchar(50),	@noteid int, @rtn int

if @CountryCode is not null and @Country is null
Begin
     select @Country = isnull(Country,'') from Country where CountryCode = @CountryCode 
End

if @Country is not null and @CountryCode is null
Begin
     select @CountryCode = isnull(CountryCode,'') from Country where Country =@Country 
End

if (@Country is not null and @Country ='United States' and @ForeignAddr =1)
Begin
	select @ForeignAddr = 0
End

select @dbaction = 'insert'
--  trim strings section    --
exec sp_trimstring @inputstring = @SSN	output
exec sp_trimstring @inputstring = @Attention	output
exec sp_trimstring @inputstring = @Street	output
exec sp_trimstring @inputstring = @City		output
exec sp_trimstring @inputstring = @State	output
exec sp_trimstring @inputstring = @PostalCode	output
exec sp_trimstring @inputstring = @Country	output
exec sp_trimstring @inputstring = @CountryCode	output
exec sp_trimstring @inputstring = @ReturnedMailDate	output
exec sp_trimstring @inputstring = @ReceivedFrom	output
exec sp_trimstring @inputstring = @PostalCode	output
exec sp_trimstring @inputstring = @AuditUser	output

--  check procedure section    --
 exec @rtn = Addr_sp_PersonAddress_chk  @dbaction = @dbaction
,    @SSN = @SSN
,    @Attention = @Attention
,    @ReceivedFrom = @ReceivedFrom
,    @Street = @Street
,    @City = @City
,    @State = @State
,    @Country = @Country
,    @PostalCode = @PostalCode
,    @DoNotChange = @DoNotChange
,    @ReturnedMailDate = @ReturnedMailDate
,    @ForeignAddr = @ForeignAddr
,    @AuditDate = @AuditDate
,    @AuditUser = @AuditUser

--  check return value and exit if not 0   --
if @rtn != 0
   begin
      return @rtn
   end

-- If @AuditDate is null Set it to system date
if @AuditDate is null
begin
   select @AuditDate = getdate()
end

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

begin tran

        -- insert statement   --
        insert into PersonAddress   (SSN   , Attention   , ReceivedFrom   , Street   , City   , State   , Country, CountryCode   , PostalCode   , DoNotChange
                   , ReturnedMailDate   , ForeignAddr   , AuditDate   , AuditUser)
        values   (   @SSN   ,@Attention   ,@ReceivedFrom   ,@Street   ,@City   ,@State   ,@Country, @CountryCode   ,@PostalCode   ,@DoNotChange
                   ,@ReturnedMailDate   ,@ForeignAddr   ,@AuditDate   ,@AuditUser)


--  check error value  --
if @@error != 0
begin
      rollback tran
      raiserror 99999 'An error has occurred inserting data into the PersonAddress table.'
      return 99999
end
else
begin
      commit tran
end

if exists (select 1 from Participant where ssn = @SSN)
Begin
	exec sp_rollover_participant @ssn = @ssn
End


--  All is good
return 0
END


/****** Object:  StoredProcedure [dbo].[Addr_sp_PersonAddress_upd]    Script Date: 04/10/2013 18:15:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



/*
	11/28/2006	VN		Set ForiegnAddr to 0, if country is US but ForiegnAddr is still 1
	04/10/2013	Raj		Fixed a bug in selecting Country Description based on the given @CountryCode.
*/


ALTER Procedure [dbo].[Addr_sp_PersonAddress_upd] (
  @SSN                           char(10)      = NULL  
, @Attention                     varchar(30)   = NULL  
, @Street                        varchar(60)  = NULL  
, @City                          varchar(30)   = NULL  
, @State                         varchar(20)   = NULL  
, @PostalCode                    varchar(10)   = NULL  
, @Country                       varchar(25)   = NULL  
, @CountryCode               varchar(3)   = NULL  
, @ForeignAddr     bit   = NULL  
, @DoNotChange                   bit       = NULL  
, @ReceivedFrom                        varchar(25)   = NULL  
, @ReturnedMailDate              datetime      = NULL  
, @AuditDate                     datetime      = NULL  
, @AuditUser                     varchar(30)   = NULL  
)
AS
BEGIN
declare	@errornumber	int, 
		@errormessage  varchar(255), 
		@dbaction varchar(255), 
		@lastuser varchar(50),	
		@noteid int, 
		@rtn int,
		@RollParticipant bit

select @RollParticipant = 0

if @CountryCode is not null and @Country is null
Begin
     select @Country = isnull(Country,'') from Country where CountryCode = @CountryCode 
End

if @Country is not null and @CountryCode is null
Begin
     select @CountryCode = isnull(CountryCode,'') from Country where Country = @Country 
End

if (@Country is not null and @Country ='United States' and @ForeignAddr =1)
Begin
	select @ForeignAddr = 0
End

select @dbaction = 'update'
--  trim strings section    --
exec sp_trimstring @inputstring = @SSN	output
exec sp_trimstring @inputstring = @Attention	output
exec sp_trimstring @inputstring = @Street	output
exec sp_trimstring @inputstring = @City		output
exec sp_trimstring @inputstring = @State	output
exec sp_trimstring @inputstring = @PostalCode	output
exec sp_trimstring @inputstring = @Country	output
exec sp_trimstring @inputstring = @CountryCode	output
exec sp_trimstring @inputstring = @ReturnedMailDate	output
exec sp_trimstring @inputstring = @ReceivedFrom	output
exec sp_trimstring @inputstring = @PostalCode	output
exec sp_trimstring @inputstring = @AuditUser	output

--  check procedure section    --
 exec @rtn = Addr_sp_PersonAddress_chk  @dbaction = @dbaction
,    @SSN = @SSN
,    @Attention = @Attention
,    @ReceivedFrom = @ReceivedFrom
,    @Street = @Street
,    @City = @City
,    @State = @State
,    @Country = @Country
,    @PostalCode = @PostalCode
,    @DoNotChange = @DoNotChange
,    @ReturnedMailDate = @ReturnedMailDate
,    @ForeignAddr = @ForeignAddr
,    @AuditDate = @AuditDate
,    @AuditUser = @AuditUser

--  check return value and exit if not 0   --
if @rtn != 0
   begin
      return @rtn
   end

 select @AuditDate = getdate()

if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

/* UPDATE SECTION */
begin tran

if not exists (select 1 from PersonAddress where SSN = @SSN)
BEGIN
	/* Insert statement */
    insert into PersonAddress   (   SSN   , Attention   , ReceivedFrom   , Street   , City   , State   , Country, CountryCode   , PostalCode   , DoNotChange
       , ReturnedMailDate   , ForeignAddr   , AuditDate   , AuditUser   )
    values   (   @SSN   ,@Attention   ,@ReceivedFrom   ,@Street   ,@City   ,@State   ,@Country, @CountryCode   ,@PostalCode   ,@DoNotChange
       ,@ReturnedMailDate   ,@ForeignAddr   ,@AuditDate   ,@AuditUser)

	if exists (select 1 from Participant where ssn = @SSN)
		select @RollParticipant =1
END

/* Write to address history */
if not exists (select 1 from personaddress where ssn = @ssn
	   and rtrim(coalesce(Attention, ''))		= rtrim(coalesce(@Attention, ''))
	   and rtrim(coalesce(Street, ''))		      = rtrim(coalesce(@Street, ''))
	   and rtrim(coalesce(City, ''))		            = rtrim(coalesce(@City, ''))
	   and rtrim(coalesce(State, ''))		      = rtrim(coalesce(@State, ''))
	   and rtrim(coalesce(Country, ''))		      = rtrim(coalesce(@Country, ''))
	   and rtrim(coalesce(CountryCode, ''))		      = rtrim(coalesce(@CountryCode, ''))
	   and rtrim(coalesce(PostalCode, ''))		= rtrim(coalesce(@PostalCode, ''))
	   and rtrim(coalesce(ReturnedMailDate, ''))	= rtrim(coalesce(@ReturnedMailDate, ''))
	   and rtrim(coalesce(ReceivedFrom, ''))	= rtrim(coalesce(@ReceivedFrom, ''))
	   and DoNotChange 				      = @DoNotChange
	   and ForeignAddr 				            = @ForeignAddr
)
BEGIN
        insert personaddresshist (ssn, Type, Attention, ReceivedFrom, Street, City,State, Country, CountryCode, PostalCode, DoNotChange,
	    ReturnedMailDate, ForeignAddr, AuditDate, AuditUser)
        select ssn, Type, Attention, ReceivedFrom, Street, City, State, Country, CountryCode, PostalCode, DoNotChange,
                ReturnedMailDate, ForeignAddr, AuditDate, AuditUser
        from personaddress
        where ssn = @ssn and street is not null
        
        if exists (select 1 from Participant where ssn = @SSN) 
		and exists (select 1 from personaddress where ssn = @ssn
					   and( rtrim(coalesce(City, ''))		      <> rtrim(coalesce(@City, ''))
					   or rtrim(coalesce(State, ''))		      <> rtrim(coalesce(@State, ''))
					   or rtrim(coalesce(Country, ''))		      <> rtrim(coalesce(@Country, ''))
					   or rtrim(coalesce(PostalCode, ''))		  <> rtrim(coalesce(@PostalCode, ''))
					   or ForeignAddr 							  <> @ForeignAddr))
		Begin
			select @RollParticipant =1
		End	

	/* update statement */
	update PersonAddress set
	     Type = 1
	   , Attention = @Attention
	   , ReceivedFrom = @ReceivedFrom
	   , Street = @Street
	   , City = @City
	   , State = @State
	   , Country = @Country
	   , CountryCode = @CountryCode
	   , PostalCode = @PostalCode
	   , DoNotChange = @DoNotChange
	   , ReturnedMailDate = @ReturnedMailDate
	   , ForeignAddr = @ForeignAddr
	   , AuditDate = @AuditDate
	   , AuditUser = @AuditUser
	where SSN = @SSN
END


/* check error value */
if @@error != 0
begin
      rollback tran
      raiserror 99999 'An error has occurred updating data to the PersonAddress table.'
      return 99999
end
else
begin
      commit tran
end

if @RollParticipant =1
Begin
	exec sp_rollover_participant @ssn = @ssn
End


/* All is good */
return 0
END




/****** Object:  StoredProcedure [dbo].[sp_Participant_ins]    Script Date: 01/13/2012 16:54:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/*** This stored procedure creates new participant by inserting records into Person, PersonAddress and Participant tables. ***/
/*
05/12/2006	VN	Changed Code, so that if person is present , info is updated instead of Although Error
01/08/2009  RG  Removed PID_server link server PID database is getting moved to the same server as HEDB and EADB
03/09/2013	Raj	Added more columns to #tmpPIDInfo table since OPUS sp is changed to give more columns in the result set.

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
, GoGreenStatus					varchar(4)	null
, StartDate						datetime null
, Source						varchar(50) null
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

begin tran

	if @recCount =1
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

sp_linkedservers

-- Ignore marital status
-- =============================================
-- Author:		Hari/Rozana Goldring
-- Create date: 11/22/2011
-- Description:	This procedure is to use to populate ReportDataMain_monthly table every night
-- Tables: RptParticipantEligData_monthly and RptDependentEligData_monthly
-- Data issue with ssn 100281916

03/09/2013	Raj	 Modified to point to OPUS from PID for all Personal Info related tables.
				??? Still need to figure out ReturnedMailDate and CLAIMS_SERVER. (?-CLAIMS_SERVER_Q5)

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
select distinct p.MPI_PERSON_ID,@datamonth,e.SSN,p.Last_Name,p.First_Name,p.Date_Of_Birth,
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
select distinct p.mpi_Person_Id,@datamonth,e.SSN,p.Last_Name,p.First_Name,p.Date_Of_Birth,
       round(convert(decimal(5,2),case when p.Date_Of_Birth is not null and p.Date_Of_Birth <> ''
                                        then datediff(mm,p.Date_Of_Birth,@lastofmonth) / 12.0											
							      else null							
			                      end),0,1)	,	
       p.GENDER_VALUE,e.StatusCode,e.EligEffective,e.EligCancellation,           -- line 1
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
       0 NumberOfEligDep,pa.ADDR_LINE_1,pa.ADDR_LINE_2,pa.ADDR_CITY,pa.ADDR_STATE_VALUE,pa.ADDR_ZIP_CODE,
       Case when pa.END_DATE is not NUll or pa.END_DATE <> '' then 'Yes' else 'No' end ReturnedMailFlag,
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
    inner join Opus.dbo.SGT_person p on e.ssn = p.ssn
        and e.eligcancellation >= @FirstOfMonth and e.eligeffective <= @lastofmonth and e.statuscode in ('PT','NI','CI')
	left outer join OPUS.dbo.SGT_PERSON_Address pa on p.person_id = pa.person_id
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
select distinct p.MPI_PERSON_ID,rt.datamonth,rt.SSN,ed.DepSSN,d.dependentcode,p.Last_Name,p.First_Name,p.Date_Of_Birth,
       round(convert(decimal(5,2),case when p.Date_Of_Birth is not null and p.Date_Of_Birth <> ''
                                        then datediff(mm,p.Date_Of_Birth,@lastofmonth) / 12.0											
							      else null							
			                      end),0,1)	,p.GENDER_VALUE,	'' MaritalStatus,rt.statuscode, ed.statuscode,   -- line1
       ed.eligeffective ,ed.EligCancellation,rt.CoverageType,
       case when rt.coveragetype in ('retiree','cobra','survivor')
             and round(convert(decimal(5,2),case when p.Date_Of_Birth is not null and p.Date_Of_Birth <> ''
                                                      then datediff(mm,p.Date_Of_Birth,@lastofmonth) / 12.0											
							                else null end),0,1) >=65 and ed.statuscode = 'ed' then 'Medicare' 
            when rt.coveragetype in ('retiree','Survivor') 
             and round(convert(decimal(5,2),case when p.Date_Of_Birth is not null and p.Date_Of_Birth <> ''
                                                      then datediff(mm,p.Date_Of_Birth,@lastofmonth) / 12.0											
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
       case when pa.MODIFIED_DATE >= '11/30/2010' then pa.ADDR_LINE_1 else rt.address1 end,
       case when pa.MODIFIED_DATE >= '11/30/2010' then pa.ADDR_LINE_2 else rt.address2 end,
       case when pa.MODIFIED_DATE >= '11/30/2010' then pa.ADDR_CITY else rt.city end,
       case when pa.MODIFIED_DATE >= '11/30/2010' then pa.ADDR_STATE_VALUE else rt.state end,
       case when pa.MODIFIED_DATE >= '11/30/2010' then pa.ADDR_ZIP_CODE else rt.zip end,  -- line 3
       Case when pa.END_DATE is not NUll or pa.END_DATE <> '' then 'Yes' else 'No' end ReturnedMailFlag,
       Case When exists (select 1 from CLAIMS_SERVER.MPIClmData.dbo.MptfServiceArea m 
                         where m.zip = left(case when pa.modifydate >= '11/30/2010' then pa.ADDR_ZIP_CODE else rt.zip end,5) 
                         and @rundate between m.effdate and m.termdate) then 'Yes' 
       else 'No' end InMPTFServArea,
       Case When ForeignAddress = 1 then 'Yes' else 'No' end ForeignFlag,
       getdate(),suser_sname(),getdate(),suser_sname(),getdate(),
       case when ed.statuscode <> 'id' then rt.EligibilityType else 'Not Eligible' end,
       case when ed.statuscode <> 'id' then rt.BenefitsCoverage  else '' end              -- line 4
from   hedb..dependenteligibility ed join dependent d on d.ssn = ed.ssn and d.depssn = ed.depssn
       join  RptParticipantEligData_monthly rt on rt.ssn = ed.ssn and rt.datamonth = @datamonth
		and  ed.eligcancellation >= @FirstOfMonth and ed.eligeffective <= @lastofmonth
       inner join OPUS.dbo.SGT_PERSON p on ed.depssn = p.ssn
	   left outer join OPUS.dbo.SGT_PERSON_ADDRESS pa on p.person_id = pa.person_id

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
left outer join OPUS.dbo.SGT_PERSON_CONTACT pc on rt.personid = pc.personid and rt.datamonth = @datamonth

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
--03/09/2013	Raj	Modifed code to use correct/current opus.dbo.SGT_PERSON table.
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
       where exists (select 1 from OPUS.DBO.SGT_PERSON p where p.SSN = t.SSN)  
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
  where not (exists (select 1 from OPUS.DBO.SGT_PERSON p where p.SSN = t.SSN)  
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
**  03/09/2013	Raj		Modifed to user MPI_Person_Id/Date_Of_Birth instead of PersonId/DateOfBirth from OPUS table.             
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
if exists(select * from OPUS.dbo.SGT_PERSON where MPI_PERSON_ID = @memberinfo and date_of_birth = @dob)
begin -- select * from pid.dbo.person
   select @sub_ssn = ssn from OPUS.dbo.SGT_PERSON where MPI_PERSON_ID = @memberinfo and date_of_birth = @dob
   set @memberinfotype = 'PRSID'
   set @prsid = @memberinfo
end
else if exists(select * from OPUS.dbo.SGT_PERSON where ssn = @memberinfo and date_of_birth = @dob)
begin
   select @prsid = MPI_PERSON_ID from OPUS.dbo.SGT_PERSON where ssn = @memberinfo and date_of_birth = @dob
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


update t set t.pid = prs.MPI_PERSON_ID
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




/****** Object:  StoredProcedure [dbo].[usp_834_Eligibility_Full_5010]    Script Date: 04/08/2013 14:49:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER  procedure [dbo].[usp_834_Eligibility_Full_5010]	@ProcessDate	smalldatetime = NULL, @Debug int = 0

AS

BEGIN

/* HISTORY LOG --------------------------------------------------------------------------------------------------------
-- 7/10/2007 Creation - cla
- 08/09/2006	VN		Also code, to save copy of file in BlueShield_History Folder				
	10/03/2006	VN		Changed code so that for dependents Relationship ='' IF fn ufn_ProvidersReportBSDepRelationship
						returns null, instaed of defaulting to 2, ie spouse.
	10/18/2006	VN		Chnaged code, so that IF @Address contains #, replace it with 'APT' as it is causing error on BS Side.						
	10/26/2006	VN		alter dep eliigbility dates based on Prticipant Dates
	11/01/2006	VN		Changed addressline3 FROM subscring(26) to substring(30)
	11/01/2006	VN		As per discussions with BS, added code, to send Termed Participant and Dependent Details.
	11/07/2006	VN		Code, to give cobra grace to dep also
	11/09/2006	VN	    Code, to set TerminationDate correcttly for survivors.                 
	12/14/2007	VN		Code to replace added in SELECT only , so that len is never >30
    01/12/2009  RG      Change \\mpicore2\G to \\mpicore2\G$
    02/09/2009  RG      Changed mpicore2\G$ to mpihome01
    10/27/2009  RG      Changed to use HCODetail instead of plan hard codding
    12/21/2009  RG      Changed to include Medicare out of state Logic for Medicare Cross Over.
                        Logic will exclude FROM the file families with out od CA address and have medicare number for all family members.
    01/12/2010  RG      UPDATE EligEffective and EligTermination FROM last run for the participant for Retirees out of state
    01/14/2010  RG      UPDATE to add Depssn to change of 01/12/2010
	01/25/2010  RG      UPDATE to fix foreign address: 
                        State = 'ZZ', posdtacode = '99999', country code added, 
                        State and Zip IF exists combined in to AddressLine2
    03/31/2010  RG      Exclude participants who doesn't have BENE card submitted
    03/31/2010  RG      Use dependent address for dependents in table dependent_addressused
    04/27/2010  RG      UPDATE to remove survivors out of state.
    06/02/2010  RG      UPDATE to revise Medicare Crossover logic as following:
                        Remove all family with out of state address (Teriree, Survivors, Cobra)
                        FROM the file IF one of the following conditions is true for participant and all dependents:
                        1) Is 65 or above; 2)has SSAward longer then 24 month; 3) has Medicare number
                        can not use function fn_IsMedicareXOverParticipant because need date whe becameMedicareXOverParticipant
    11/03/2010  RG      Change to remove FROM file MedicareXOver families who has no part B, but have part A Medicare
    12/29/2010  RG      Change to add C2 dependents ad use their address IF it's UPDATEd on or after 11/30/2010 
    03/07/2011  RG      UPDATE File path in email, users have access only to Tapes share
    06/01/2011  RG      UPDATE to set maximum EligTermination for couple when dependent getting Medicare later then participant
                        UPDATE MedicareXOver date to get last date of the month prior to the month when member become MedicareX over.  
    08/10/2011  RG      UPDATE after deleting the dates, because delete date logic is wrong 
    08/31/2011  RG      Add table MedicareXOverPartToDelete instead of temp table #PartToDelete                 
    12/19/2011  RG      Make changes for 5010. Remove Cursor. Use COB and Medicare information from QNXT
                        Change address logic to send MPI address id address is bad or empty
    05/10/2012  RG      Update to use ProvidersReportBSFILE_5010 instead of ProvidersReportBSFILE 
 
---------------------------------------------------------------------------------------------------------
    05/24/2012	Raj	New procedure is create to use in the generation of Elig. 834 file for ABC.
					The procedure is cloned from "usp_BlueShield_Eligibility_Full_5010"
					But we are using the same old underlying table to store the data.
					We added two more columns StatusCode and WorkDate in all tables.
					We used StatusCode in stead of GroupNumber(EligGrpNo) to find out the kind of Group.
	05/30/2012	Raj	Comment all the MedicareXOver related logic. Since for ABC we send that data as a seperate
					feed (in their proprietary Limited Liability file format).
	06/08/2012	Raj	Stopped using CLAIMS_SERVER.mpiclmdata.dbo.COBDATA_5010 linked table and in stead created the
					local table and populated using the linked sp.
	06/13/2012	Raj	Enhanced MPI default address logic.
					Added logic to update SV/DP participant's TermDate to DeathDate if it is greater.
					Replace spaces with blank in ZipCodes.
					Populate MiddleInitial instead of keeping it null.
	06/26/2012	Raj	Check the ZipCodes against PlanData.dbo.ZipCodes
	07/12/2012	Raj	Modified code to include RetrunMailDate field when we update the Dependent address fields with Participant
					address.
	08/14/2012	Raj	Created a new table dbo.ProvidersReportABC_5010 instead of using old dbo.ProvidersReportBS_5010 (BSC) table.
	09/11/2012	Raj	Modified to code to implement 3 new GroupNumbers for Bad addresses as mentioned below:
					CA Participants			:277163M023
					OOS ME Participants		:277163M113
					OOS NME Participants	:277163M114	
					Also make sure that if Subscriber has any of these 3 Bad Address GroupNumbers then all dependents should
					have the same number
	09/12/2012	Raj	Created a new table ProvidersReportABCFILE_5010 instead of using old ProvidersReportBSFILE_5010 table.
					Added code to remove members with out GroupNumber just in case. (It should not happen unless a data issue).
	10/08/2012	Raj	Apply Gender, DOB and BeneCard restriction in the begining selection of all participants.			
    12/27/2012  Rozana Added logic to include PremiumGroup 2 participants during Grace Period 
    01/07/2013	Raj	Modified the logic in updating Dependents EligTermination.
    02/05/2013	Raj	Fixed a bug in copying to History files.
    04/08/2013	Raj	Modified code to point PID.dbo.ZipCodes to OPUS.dbo.ZipCodes table as part of OPUS implementation.
    
-- TEST RUN
exec usp_834_Eligibility_Full_5010 '08/01/2006',0
exec usp_834_Eligibility_Full_5010 @ProcessDate	= NULL, @Debug = 0
-----------------------------------------------------------------------------------------------------------------*/


SET NOCOUNT ON
SET DEADLOCK_PRIORITY NORMAL
--drop table #AllBlueShieldElig

/*----------------
declare @ProcessDate	smalldatetime , @Debug int

set @ProcessDate = null
set @Debug = 1
-----------------------*/

declare	@RecordType char(1),
	@EligGrpNo char(15),
	@SubNo char(18), -- ssn
	@PersonNo varchar(3),
	@TranCode char (1)  ,
	@EligEffective char (8)  ,
	@EligTermination char (8)  ,
	@Relationship char (1)  ,
	@LastName char (25)  ,
	@FirstName char (15)  ,
	@DateOfBirth char (8)  ,
	@Gender char (1)  ,
	@Address char (55),
	@AddressLine2 char (55),
	@AddressLine3 char (30),
	@City char (30),
	@State char (2),
	@ZipCode char (9),
	@SSN char (11)  ,
	@CoverageCode char (1)  ,
	@division varchar(4),
	@Totalrecs numeric(9),
	@TerminationDateNotFormatted	smalldatetime,
	@EffectiveDateNotFormatted	smalldatetime,
	@EffectiveDate smalldatetime,
	@TerminationDate smalldatetime,
	@CobraGrace		integer,
	@WorkDate		smalldatetime,	/* This variable is used because setting @ProcessDate to GetDate() in case it was not supplied slows down process up to 10 times???   AZ */
	@DOSCommand	varchar(255),
	@LastProvidersReportBSrun smalldatetime,
	@Message varchar(255),
	@statuscode varchar(2),
	@CreationDate smalldatetime,
	@ClientAccountNbr varchar(20),
	@MedicareDisenrollmentType varchar(1),
	@SubsidyDate	smalldatetime,
    @CountryCode    char(2),
    @PersonId char(15),
    @DateOfDeath char(8),
    @CAPlanEndDate smalldatetime,
    @PlanCode varchar(2)

DECLARE @strRecipents varchar(256)

/* Clear the destination table */
TRUNCATE table dbOutBound..ProvidersReportABC_5010
TRUNCATE table dbOutBound..ProvidersReportBS_HDR_5010
TRUNCATE table dbOutBound..ProvidersReportBS_TRL_5010
TRUNCATE table dboutbound.dbo.MedicareXOverPartToDelete_5010
TRUNCATE table dboutbound.dbo.COBData_5010
TRUNCATE table dboutbound.dbo.COBData_5010_MPIClmData
TRUNCATE table dboutbound.dbo.MedicareXOverList_5010

SELECT @WorkDate =  coalesce(@ProcessDate, GetDate())
SELECT @CreationDate = dbo.fnFormatDate(getdate(),'mm/dd/yyyy')
SELECT @SubsidyDate = '01/01/2006'

EXEC CLAIMS_SERVER.mpiclmdata.dbo.usp_GetCOBData_5010 @WorkDate

Insert into dboutbound.dbo.COBData_5010_MPIClmData
	select * from CLAIMS_SERVER.mpiclmdata.dbo.COBDATA_5010

-- Create records of participant's eligible on run date
SELECT pt.SSN
	  ,CobraGrace = min(e.CobraGrace)
	  ,StatusCode = min(e.StatusCode)
	  ,WorkDate = @WorkDate
INTO #AllBlueShieldElig
FROM	HEDB..Participant pt (nolock)
inner join Hedb.dbo.Person p (nolock) on pt.SSN = p.SSN
INNER JOIN (SELECT	SSN,
					CobraGrace = 0,
					StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	@WorkDate between EligEffective and EligCancellation and
	 				StatusCode not in ('CI','NI','PT')
				UNION
				SELECT	SSN,
						CobraGrace = 1,
						StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	dateadd(month, -1, @WorkDate) between EligEffective and EligCancellation and
					StatusCode in ('CC','CN') 
				Union --RG added 12/27/2012 for PremiumGroup 2 participants
				SELECT	SSN,
						CobraGrace = 0,
						StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	@WorkDate between EligEffective and EligCancellation and
					StatusCode in ('NI'))as e
on	pt.SSN = e.SSN 
	and	pt.ssn in (SELECT SSN 
				   FROM HEDB..HCOEnrollment  (nolock)
				   WHERE hco in (SELECT hco FROM hedb..HCODetail (nolock) WHERE PlanGROUP = 'Anthem') 
				   and @WorkDate between StartDate and EndDate and GroupNumber <> 'ERROR')
where pt.SSN <> '999999999'
and (p.dateofbirth is not null and p.dateofbirth <> '')	-- exclude when no date of birth 
		and  hedb.dbo.fn_IsBeneCardSubmitted(p.SSN) = 1		-- exclude if not BeneCard
		and p.sexcode in ('F','M') -- exclude when no sexcode
GROUP	by pt.SSN
ORDER	by pt.SSN


SELECT	@WorkDate = DateAdd(day,7,@WorkDate)


INSERT #AllBlueShieldElig
SELECT pt.SSN
	  ,CobraGrace = min(e.CobraGrace)
	  ,StatusCode = min(e.StatusCode)
	  ,WorkDate = @WorkDate
FROM	HEDB..Participant pt
inner join Hedb.dbo.Person p (nolock) on pt.SSN = p.SSN
INNER JOIN (SELECT	SSN,
					CobraGrace = 0,
					StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	@WorkDate between EligEffective and EligCancellation and
	 				StatusCode not in ('CI','NI','PT')
				UNION
				SELECT	SSN,
						CobraGrace = 1,
						StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	dateadd(month, -1, @WorkDate) between EligEffective and EligCancellation and
					StatusCode in ('CC','CN') 
				Union --RG added 12/27/2012 for PremiumGroup 2 participants
				SELECT	SSN,
						CobraGrace = 0,
						StatusCode
				FROM	HEDB..Eligibility (nolock)
				WHERE	@WorkDate between EligEffective and EligCancellation and
					StatusCode in ('NI'))as e
on	pt.SSN = e.SSN 
	and	pt.ssn in (SELECT SSN 
				   FROM HEDB..HCOEnrollment (nolock) 
				   WHERE hco in (SELECT hco FROM hedb..HCODetail (nolock) WHERE PlanGROUP = 'Anthem') 
				   and @WorkDate between StartDate and EndDate and GroupNumber <> 'ERROR')
where pt.SSN <> '999999999'
and (p.dateofbirth is not null and p.dateofbirth <> '')	-- exclude when no date of birth 
and  hedb.dbo.fn_IsBeneCardSubmitted(p.SSN) = 1		-- exclude if not BeneCard
and p.sexcode in ('F','M') -- exclude when no sexcode
and pt.SSN not in (SELECT SSN FROM #AllBlueShieldElig)
GROUP	by pt.SSN
ORDER	by pt.SSN


INSERT INTO dbOutbound..ProvidersReportABC_5010(	
                RecordType,
				EligGrpNo,
				SubscriberNo,
				PersonNumber,
				TranCode,
				EligEffective,
				EligTermination,
				Relationship,
				LastName,
				FirstName,
				MiddleInitial,
				DateofBirth,
				Gender,
				SSN,
				CoverageCode,
				Address,
				AddressLine2,
				AddressLine3,
				City ,
				State,
				ZipCode,
                CountryCode,
				CreditableCvrgInd,
				PrimarySubsidyInd,
				PrimarySubsidyEffDate,
				PrimarySubsidyTermDate,
				ClientAccountNbr,
				MedicareDisenrollmentType,										
				AuditDate,
				ActionType,
				PersonId,
				DateOfDeath,
				CAPlanEndDate,
				PlanCode,
				ReturnedMailDate,
				StatusCode,
				WorkDate
				)
select 
      RecordType = 'E',
	  GroupNumber,
	  SubNo = p.SSN,
	  PersonNo = '001',
	  TranCode = 'P',	  
	  EligEffective = convert(char(8), StartDate, 112),			 
	  EligTermination = case when e.StatusCode = 'SV' and convert(char(8), EndDate, 112) >  convert(char(8), pr.dateofdeath, 112) 
																							and isnull(pr.dateofdeath,'') <> ''
																then convert(char(8), pr.dateofdeath, 112) 	
							 when e.StatusCode = 'SV' and isnull(pr.dateofdeath,'') <> ''
									and convert(char(8), pr.dateofdeath, 112) < convert(char(8), StartDate, 112)						
										then convert(char(8), StartDate, 112)
							else convert(char(8), EndDate, 112)
					   end,
	  Relationship = '1',
	  LastName =   pr.Lastname ,--5010 Removed truncate
      FirstName =   pr.Firstname ,--5010 Removed truncate
      MiddleInitial = left(ltrim(pr.MiddleName),1),
      DateOfBirth  = convert(varchar(8), pr.DateofBirth,112),
      Gender =  UPPER(convert(char(1), pr.SexCode)),
	  SubNo = p.SSN,
	  CoverageCode = CASE when pr.dateofdeath is not null then 'K'
						ELSE '6'
						END,
	  Address = substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
						  ELSE  REPLACE(pa.Street,'#','') END,1,55),
						  
	  --For USA/CA use populate State and Zip Fields for all foreign address make them blank.	  
	  AddressLine2 = CASE WHEN pa.ForeignAddr = 1 and isnull(pa.CountryCode,'') <> 'CA'
						THEN Ltrim(Rtrim(Rtrim(substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55)) 
											 + ' ' + Rtrim(IsNull(pa.State,'')) + ' ' + Rtrim(IsNull(pa.Postalcode,''))))
						ELSE (substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55))
						END,
	  AddressLine3 = '',
	  City = isNull(pa.city,''),
	  State =  Upper(case when pa.ForeignAddr = 0 
								then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then IsNull(pa.state,'') 
										else '' 
									end)
						  --for Canada
						  when pa.ForeignAddr = 1 and isnull(pa.CountryCode,'') = 'CA' then left(IsNull(pa.state,''),2)
						else ''
				  end),
	  ZipCode = Convert(varchar(9),case when pa.ForeignAddr = 0
											then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then isNull(Replace(pa.Postalcode,'-',''),'') 
													else '' 
												end)
										 --for Canada
										when pa.ForeignAddr = 1 and isnull(pa.CountryCode,'') = 'CA' then isNull(Replace(pa.Postalcode,'-',''),'') 
						else ''
				   end),
	  CountryCode = case when pa.ForeignAddr = 1 
							then case when IsNull(pa.CountryCode,'') = 'UK' then 'GB' 
										else IsNull(pa.CountryCode,'') 
								 end
						 else ''
					   end,
	  '',
	  '',
	  '',
	  '',
	  SubNo = p.SSN,
	  '',
	  dbo.fnFormatDate(getdate(),'mm/dd/yyyy'),--@CreationDate,-- come back
	  'W',
	  PersonId = PID,
	  DateOfDeath = convert(varchar(8), pr.DateOfDeath,112),		
      null,
      HCO,
      ReturnedMailDate,
      t.StatusCode,
      t.WorkDate
      
FROM    HEDB..Participant p (nolock)
	inner join #AllBlueShieldElig t on p.SSN = t.SSN
	inner join HEDB..Eligibility e (nolock) on p.ssn = e.ssn and t.WorkDate between E.EligEffective and E.EligCancellation
	inner join HEDB..HCOEnrollment he (nolock) on he.ssn = t.ssn and t.workdate between he.startdate and he.enddate
		and HCO in (SELECT hco FROM hedb..HCODetail (nolock) WHERE PlanGroup = 'Anthem') 
	inner join HEDB..Person pr (nolock) on p.ssn = pr.ssn
	left outer join HEDB..PersonAddress pa (nolock) on p.ssn = pa.ssn --and (pa.ReturnedMailDate is null or pa.ReturnedMailDate = '')
	left outer join HEDB..Eligibility_Pid_Reference pid (nolock) on p.ssn = pid.ssn
WHERE  (pr.dateofbirth is not null and pr.dateofbirth <> '') 
		and  hedb.dbo.fn_IsBeneCardSubmitted(p.SSN) = 1 -- exclude when no date of birth , 04/23/2009  exclude dateofbirth = '' ,03/31/2010 RG add to check IF Bene card is submitted
		and pr.sexcode in ('F','M') -- exclude when no sexcode
		
		
-- Update Medicare part D data
UPDATE bs
SET 	CreditableCvrgInd = m.CreditableCvrgInd,
		PrimarySubsidyInd = m.PrimarySubsidyInd,
		PrimarySubsidyEffDate = isnull(convert(varchar(8),m.PrimarySubsidyEffDate,112),''),
		PrimarySubsidyTermDate = isnull(convert(varchar(8),m.PrimarySubsidyTermDate,112),'')
FROM HEDB..MedicarePartDElig m (nolock)
	inner join dbOutbound..ProvidersReportABC_5010 bs on m.ssn = bs.subscriberno
WHERE Type in ( 'Participant','RetireeDisabled')
	and MedicareElig ='Y'
	and '01/01/2006' between m.PrimarySubsidyEffdate and isnull(m.PrimarySubsidyTermDate,dateadd(yy,1,m.PrimarySubsidyEffdate))
--come back

--dependents:
INSERT  INTO dbOutbound..ProvidersReportABC_5010(
	                RecordType,
					EligGrpNo,
					SubscriberNo,
					DepSSN,
					PersonNumber,
					TranCode,
					EligEffective,
					EligTermination,
					Relationship,
					LastName,
					FirstName,
					MiddleInitial,
					DateofBirth,
					Gender,
					ssn,
					Address,
					AddressLine2,
					AddressLine3,
					City ,
					State,
					ZipCode,
                    CountryCode,
					CreditableCvrgInd,
					PrimarySubsidyInd,
					PrimarySubsidyEffDate,
					PrimarySubsidyTermDate,
					MedicareDisenrollmentType,
					AuditDate,
					ActionType,
					PersonId,
		            DateOfDeath,	
					CAPlanEndDate,
					PlanCode,
					StatusCode,
					WorkDate)
	SELECT	
	    RecordType = 'E',
		EligGrpNo = '',
		SubNo = d.ssn,
		DepSSN = d.depssn,
		PersonNo = '0' + d.suffixcode,
		TranCode = 'P', -- Positive processing (weekly) or L maintenance Processing (Daily)
		EligEffective = Convert(char(8),de.eligeffective,112),
		EligTermination = convert(char(8),de.eligcancellation,112),
		Relationship = ISNULL(dbo.ufn_ProvidersReportBSDepRelationship(d.dependentcode),''),
		LastName =   p.Lastname ,--5010 Removed truncate
		FirstName =   p.Firstname ,--5010 Removed truncate
		MiddleInitial = left(ltrim(p.MiddleName),1),
		DateOfBirth  = convert(varchar(8), p.DateofBirth,112),
      	Gender =  UPPER(convert(char(1), p.SexCode)),
		ssn =  d.depssn,--case when len(d.depssn) > 9 then '' else d.depssn end, 
		Address = '',
		AddressLine2 = '',
		AddressLine3 = '',
		City = '',
		State = '',
		ZipCode = '',
        CountryCode = '',
		qm.CreditableCvrgInd,
		qm.PrimarySubsidyInd,
		isnull(convert(varchar(8),qm.PrimarySubsidyEffDate,112),''),
		isnull(convert(varchar(8),qm.PrimarySubsidyTermDate,112),''),
		'',
		dbo.fnFormatDate(getdate(),'mm/dd/yyyy'),--@CreationDate,-- come back
		'W',
		PersonId = pid.PID,
		DateOfDeath  = convert(varchar(8), p.DateOfDeath,112),
		null,
		'',
		t.StatusCode,
		t.WorkDate
	FROM  #AllBlueShieldElig t 
	    inner join HEDB..Dependent d  (nolock)on t.ssn = d.ssn
		inner join HEDB..DependentEligibility de (nolock) 
			on d.ssn = de.ssn and d.depssn = de.depssn and de.statuscode = 'ED'
				--and d.SSN = @SSN 
				and
				(
				(t.WorkDate between de.EligEffective and de.EligCancellation and CobraGrace = 0)
				or
				(dateadd(month, -1, t.WorkDate) between  de.EligEffective and de.EligCancellation and CobraGrace = 1)
				 or
				/*12/29/2010	RG	Add logic to check dependent eligible a week FROM now even participant is eligible today*/
				(dateadd(day,7, t.WorkDate) between de.EligEffective and de.EligCancellation and CobraGrace = 0)-- only for no COBRA , was getting two records for dependents, updated with 5010
				) 
		inner join HEDB..Person p (nolock) on d.depssn = p.ssn
				and (p.dateofbirth is not NULL and p.dateofbirth <> '') and  hedb.dbo.fn_IsBeneCardSubmitted(t.SSN) = 1 -- exclude when no date of birth , 04/23/2009  exclude dateofbirth = '' ,03/31/2010 RG add to check IF Bene card is submitted
				and p.sexcode in ('F','M') 
		left outer join HEDB..Eligibility_Pid_Reference pid (nolock) on de.ssn = pid.ssn
		left outer join (SELECT 	ssn,
									depssn,
									CreditableCvrgInd,
									PrimarySubsidyInd,
									PrimarySubsidyEffDate,
									PrimarySubsidyTermDate
						FROM HEDB..MedicarePartDElig
						WHERE '01/01/2006' between PrimarySubsidyEffdate --come back
						and isnull(PrimarySubsidyTermDate,dateadd(yy,1,PrimarySubsidyEffdate))
						and MedicareElig ='Y'
						) qm 
			    on d.ssn = qm.ssn and qm.depssn = d.depssn and
				'01/01/2006' between PrimarySubsidyEffdate and isnull(PrimarySubsidyTermDate,dateadd(yy,1,PrimarySubsidyEffdate))
	--come back


/*---Commented MedXOver logic-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
/*Plan Change*/
UPDATE bs
SET CAPlanEndDate = he1.StartDate 
--SELECT he1.StartDate 
FROM dbOutbound..ProvidersReportABC_5010 bs
inner join #AllBlueShieldElig t on bs.subscriberno = t.SSN
inner join HEDB..HCOEnrollment he1 on t.ssn = he1.ssn
inner join HEDB..HCOEnrollment he2 
	on he1.ssn = he2.ssn
	and t.WorkDate between he1.StartDate and he1.EndDate
	and DateAdd(day, 1, he2.EndDate) = he1.StartDate
	and he2.HCO in (SELECT hco FROM hedb..HCODetail WHERE (PlanGROUP = 'Anthem' and statelist like '%CA%') OR hco = 'NM')
	and he1.HCO in (SELECT hco FROM hedb..HCODetail WHERE PlanGROUP = 'Anthem' and (statelist not like '%CA%' or statelist is NULL))
	and he1.HCO <> he2.HCO
----------------------------------------------------------------------------------------------------------
---Commented MedXOver logic-----------------------------------------------------------------------------*/

	
--Address upate for dependent = to participant address
UPDATE d
SET     Address = s.Address,
		AddressLine2 = s.AddressLine2,
		AddressLine3 = s.AddressLine3,
		City = s.City,
		State = s.State,
		ZipCode = Convert(varchar(9),s.ZipCode),
        CountryCode = s.CountryCode,
        ReturnedMailDate = s.ReturnedMailDate,
        EligGrpNo = s.EligGrpNo,
        PlanCode = s.PlanCode,
        EligEffective  =  case when convert(smalldatetime,s.EligEffective) > d.eligeffective then s.EligEffective 
							   else convert(varchar(8),d.eligeffective ,112) end,
		EligTermination  = case when (convert(smalldatetime,s.EligTermination) > d.EligTermination and CobraGrace = 1) then s.EligTermination								
								--added on 01/07/2013
								when 	convert(smalldatetime,s.EligTermination) < d.EligTermination then s.EligTermination		
							else convert(varchar(8),d.EligTermination ,112)
							end
 FROM  dbOutbound..ProvidersReportABC_5010 s
    inner join #AllBlueShieldElig t on s.subscriberno = t.SSN
	inner join dbOutbound..ProvidersReportABC_5010 d 
		on s.SubscriberNo = d.SubscriberNo
		and s.depssn is null
		and d.depssn is not null
 

/*Determine IF MPI coverage is Active
MPI is Active for members taht are not Retiree, Survivor, COBRA or NY group
If member is DUAL and one file is active and another is not, both family are active

*/
UPDATE ProvidersReportABC_5010
SET IsMPIActiveForMedicare = 'Y'
--Select * from ProvidersReportABC_5010
WHERE not (
			StatusCode in ('RT', 'CC', 'CN', 'SV')
		   )
or SSN in
(SELECT a.SSN
--select a.ssn,* 
from ProvidersReportABC_5010 a
INNER JOIN ProvidersReportABC_5010 b on a.SSN = b.SSN
and a.EligGrpNo <> b.EligGrpNo
and (
		a.StatusCode in ('RT', 'CC', 'CN', 'SV')
     )
and not (b.StatusCode in ('RT', 'CC', 'CN', 'SV')
		 )
where a.SSN in
(
select SSN
from ProvidersReportABC_5010
where ssn <> ''
group by ssn
having count(*) > 1
)
)

/*SET MEDICARE fields only if MPI is Secondary (NOT ACTIVE)*/
UPDATE b
SET MedicarePlanCode  = 'C'
   ,MedicareEligReasonCode  = CASE WHEN CharIndex('ESRD',Carrier) > 0 THEN '2' ELSE '' END
   ,MedicareEffDate  = IsNull(CONVERT(char(8), c.EffDate, 112),'')
   ,MedicareTermDate  = CASE WHEN c.TermDate = '12/31/2078' THEN NULL ELSE CONVERT(char(8), c.TermDate, 112) END
   ,MedicareXOverDate = IsNull(CONVERT(char(8), c.EffDate, 112),'')
--select * 
FROM ProvidersReportABC_5010 b
INNER JOIN dbo.COBData_5010_MPIClmData c 
	on b.ssn = c.ssn
WHERE b.IsMPIActiveForMedicare is Null -- only for not MPI ACTIVE cases for Medicare Purposes
	and COBType = 'M'
	and not (b.StatusCode = 'SV' and personnumber = '001')


/*SET COB fields*/
--Get COBData
--truncate table dbo.COBData_5010
--select * from dbo.COBData_5010  
INSERT dbo.COBData_5010
(
	 SubscriberNo 
	,DEPSSN 
	,PersonNumber
	,COBType 
	,PrimaryStatus
	,COBEffDate 
	,COBTermDate 
	,Carrier 
	,CaseType 
	,COBStatus --primary/secondary/Tertiary /Uknown
	,COBCode --Code identifying whether there is a coordination of benefits
	,COBServiceTypeCode --1 for Medical Care
	,EnrollId
	,StatusCode
	,WorkDate
)
SELECT b.SubscriberNo
      ,b.DEPSSN
      ,b.PersonNumber
      ,COBType
      ,PrimaryStatus
	  ,COBEffDate  = IsNull(CONVERT(char(8), c.EffDate, 112),'')
      ,COBTermDate  =  IsNull(CASE WHEN c.TermDate = '12/31/2078' THEN NULL ELSE CONVERT(char(8), c.TermDate, 112) END, '')
	  ,Carrier
	  ,CaseType
	  ,COBStatus = IsNull(CASE WHEN CaseType = 'COB' THEN c.PrimaryStatus
                         WHEN (CaseType = 'COBMED' and IsMPIActiveForMedicare is NULL) 
                               or CaseType = 'COBMULTI' THEN 'T'
                         ELSE 'U'
                    END ,'')
      ,COBCode  =  IsNull(CASE WHEN CaseType = 'COB' and c.PrimaryStatus = 'P' THEN '1'
                        WHEN (CaseType = 'COBMED' and IsMPIActiveForMedicare is NULL) 
                               or CaseType = 'COBMULTI' THEN '5'
                        ELSE '6'
                    END ,'')   
      ,COBServiceTypeCode  = '1' --Always 1 for Madical care 
      ,EnrollId   
      ,b.StatusCode
      ,b.WorkDate
 FROM ProvidersReportABC_5010 b  
 INNER JOIN dbo.COBData_5010_MPIClmData c 
	on b.ssn = c.ssn
	--and CharIndex('COB',CaseType) > 0 
	and COBType = 'C'
	
	
/*---Commented MedXOver logic-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
--Medicare Date the latest between MedicareEffdate and Plan Change date
update t
    set MedicareXOverDate =  Convert(char(8),(SELECT Max([Value])
										  FROM ( SELECT Convert(smalldatetime,MedicareEffDate) AS [Value]
												  UNION ALL
												 SELECT IsNull(CAPlanEndDate,'01/01/1900')
												)  u
											),112)
                          --select *
    FROM dboutbound.dbo.ProvidersReportABC_5010 t
    where MedicarePlanCode is not NULL and CAPlanEndDate Is not Null


--Update Medicare Number    
UPDATE t
SET MedicareHICN = m.HICN
FROM dboutbound.dbo.ProvidersReportABC_5010 t
INNER JOIN hedb.dbo.MedicareXOverHICN m on t.ssn = m.ssn
WHERE MedicarePlanCode is not NULL


--12/21/2009 RG UPDATEd to remove out of state retirees or disabled who has Medicare Number and all dependents have Medicare Number 
--drop table #PartToDelete, #DepNotMedicare
INSERT MedicareXOverPartToDelete_5010
SELECT bs.*
--INTO #PartToDelete
FROM dbo.ProvidersReportABC_5010 bs
where bs.DEPSSN is NULL
and PlanCode in (select HCO from HEDB..HCODetail where (StateList not like '%CA%' or StateList is NULL) and HCOType = 'M' )
and MedicarePlanCode is not Null
            

--select * from MedicareXOverPartToDelete_5010 where subscriberno = '129383918'

--04/27/2010 Remove survivors also
INSERT MedicareXOverPartToDelete_5010
SELECT bs.*
FROM dbo.ProvidersReportABC_5010 bs
WHERE PlanCode in (select HCO from HEDB..HCODetail 
                              where (StateList not like '%CA%' or StateList is NULL) and HCOType = 'M' )
and StatusCode = 'SV' 
and bs.depssn is null  
and exists (SELECT 1 
            FROM dbo.ProvidersReportABC_5010 bs1 
            WHERE bs.SubscriberNo = bs1.SubscriberNo 
            and bs1.depssn is not null
			and MedicarePlanCode is not Null
			)
                                    

--Get Dependents that don't have Medicare number
SELECT bs.* 
INTO #DepNotMedicare --drop table #DepNotMedicare
FROM dbo.ProvidersReportABC_5010 bs 
inner join MedicareXOverPartToDelete_5010 p 
	on bs.Subscriberno = p.Subscriberno
       and bs.depssn is not null
       and bs.MedicarePlanCode is Null

--select * from #DepNotMedicare where subscriberno = '129383918'
--Remove FROM participant list above family WHERE at least one dependent doens't have Medicare
delete p
FROM MedicareXOverPartToDelete_5010 p inner join #DepNotMedicare d on p.Subscriberno = d.Subscriberno

delete t
--SELECT t.* 
FROM MedicareXOverPartToDelete_5010 t inner join hedb.dbo.personaddress pa 
						on t.subscriberno = pa.ssn and pa.ForeignAddr = 1


insert MedicareXOverList_5010						
select *
from ProvidersReportABC_5010
where SubscriberNo in (select SubscriberNo from MedicareXOverPartToDelete_5010)
						

/* Terminate date logic */
UPDATE r
set EligTermination = case when convert(smalldatetime,r.EligTermination) > DateAdd(day, -1,convert(smalldatetime,r.MedicareXOverDate))
                                and convert(smalldatetime,r.MedicareXOverDate) >= Convert(smalldatetime,'12/31/2009') then convert(char(8),DateAdd(day, -1,convert(smalldatetime,r.MedicareXOverDate)),112)
                           when convert(smalldatetime,r.EligTermination) > DateAdd(day, -1,convert(smalldatetime,r.MedicareXOverDate)) and DateAdd(day, -1,convert(smalldatetime,r.MedicareXOverDate)) < Convert(smalldatetime,'12/31/2009') then '20091231' 
                      else r.EligTermination end
                      --SELECT *
FROM ProvidersReportABC_5010 r 
	inner join MedicareXOverPartToDelete_5010 t on r.subscriberno = t.ssn 
    and r.MedicarePlanCode is not Null
    --and r.subscriberno ='129383918         '


/*06/01/2011 RG UPDATE to set maximum EligTermination for couple*/
SELECT r.*
     , EligTerminationMedicareXOver = (SELECT MAX(EligTermination) FROM ProvidersReportABC_5010 WHERE subscriberno = r.subscriberno GROUP by subscriberno)
     , t.SSN SSN_t
INTO #ProvidersReportBS_EligTerm --drop table #ProvidersReportBS_EligTerm
FROM ProvidersReportABC_5010 r 
inner join MedicareXOverPartToDelete_5010 t on r.subscriberno = t.ssn

UPDATE r
set EligTermination = t1.EligTerminationMedicareXOver
--SELECT *
FROM ProvidersReportABC_5010 r 
	inner join MedicareXOverPartToDelete_5010 t on r.subscriberno = t.ssn
	inner join #ProvidersReportBS_EligTerm t1 on r.subscriberno = t1.subscriberno and IsNull(r.DepSSN,'') = IsNull(t1.DepSSN,'')

--------------------------------------------------------------------------------------------------------
---------Commented MedXOver logic---------------------------------------------------------------------*/

--Set Termination date = to Eff date if Term Date < Eff Date
update r
set EligTermination = EligEffective
--select * 
from ProvidersReportABC_5010 r
where convert(smalldatetime,EligTermination) < convert(smalldatetime,EligEffective)

/*--------Commented MedXOver logic----------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------

--to keep data in ProvidersReportABC_5010 and MedicareXOverPartToDelete_5010 consistent
UPDATE t
set EligTermination = r.EligTermination
   , MedicareXOverDate = r.MedicareXOverDate
--SELECT *
FROM ProvidersReportABC_5010 r 
	inner join MedicareXOverList_5010 t on r.subscriberno = t.subscriberno and r.personnumber = t.personnumber
--and r.subscriberno ='129383918         '

--delete MedicareXOver Families 
SELECT	@WorkDate =  coalesce(@ProcessDate, GetDate())

delete 
--SELECT * 
FROM dbo.ProvidersReportABC_5010 
WHERE Subscriberno in 
(SELECT Subscriberno FROM MedicareXOverPartToDelete_5010)
and @WorkDate > convert(smalldatetime,EligTermination)
--and '12/30/2011' > convert(smalldatetime,EligTermination)
--and subscriberno ='129383918         '

--------------------------------------------------------------------------------------------------------
---------Commented MedXOver logic---------------------------------------------------------------------*/

-----------End Medicare logic
---
/*
declare	@RecordType char(1),
	@EligGrpNo char(15),
	@SubNo char(18), -- ssn
	@PersonNo varchar(3),
	@TranCode char (1)  ,
	@EligEffective char (8)  ,
	@EligTermination char (8)  ,
	@Relationship char (1)  ,
	@LastName char (25)  ,
	@FirstName char (15)  ,
	@DateOfBirth char (8)  ,
	@Gender char (1)  ,
	@Address char (55),
	@AddressLine2 char (55),
	@AddressLine3 char (30),
	@City char (30),
	@State char (2),
	@ZipCode char (9),
	@SSN char (11)  ,
	@CoverageCode char (1)  ,
	@division varchar(4),
	@Totalrecs numeric(9),
	@TerminationDateNotFormatted	smalldatetime,
	@EffectiveDateNotFormatted	smalldatetime,
	@EffectiveDate smalldatetime,
	@TerminationDate smalldatetime,
	@CobraGrace		integer,
	@WorkDate		smalldatetime,	/* This variable is used because setting @ProcessDate to GetDate() in case it was not supplied slows down process up to 10 times???   AZ */
	@DOSCommand	varchar(255),
	@LastProvidersReportBSrun smalldatetime,
	@Message varchar(255),
	@statuscode varchar(2),
	@CreationDate smalldatetime,
	@ClientAccountNbr varchar(20),
	@MedicareDisenrollmentType varchar(1),
	@SubsidyDate	smalldatetime,
    @CountryCode    char(2),
    @PersonId char(15),
    @DateOfDeath char(8),
    @CAPlanEndDate smalldatetime,
    @PlanCode varchar(2)
    
    
declare @ProcessDate	smalldatetime , @Debug int

set @ProcessDate = null
set @Debug = 1

SELECT	@WorkDate =  coalesce(@ProcessDate, GetDate())
*/
---


-- 11/01/2006	VN	Added this part, to give BS a list of deleted participants and dependents in Full File also
--SELECT	@WorkDate =  coalesce(@ProcessDate, GetDate())
SELECT @LastProvidersReportBSrun = (SELECT TOP 1 convert(smalldatetime,auditdate) FROM ProvidersReportABCHISTORY_5010 ORDER by convert(smalldatetime,auditdate) desc)

-- Create last deletion temp table for EE
SELECT distinct *
INTO #tmpDeletedEE
FROM ProvidersReportABCHISTORY_5010 H
WHERE h.actiontype = 'D' 
and h.auditdate <= @LastProvidersReportBSrun
and h.personnumber = '001'
and not exists (SELECT 1 FROM ProvidersReportABCHISTORY_5010 h2 WHERE
				 h.subscriberno = h2.subscriberno  and
				 h2.actiontype IN ('A','W') and convert(smalldatetime,h2.auditdate) > convert(smalldatetime,h.auditdate)) 


-- Create last deletion temp table for Dependents
SELECT distinct *
INTO #tmpDeletedDEP
FROM ProvidersReportABCHISTORY_5010 H
WHERE h.actiontype = 'D' 
and h.auditdate <= @LastProvidersReportBSrun
and h.personnumber <> '001'
and not exists (SELECT 1 FROM ProvidersReportABCHISTORY_5010 h2 WHERE 
				h.subscriberno = h2.subscriberno and 
				h.depssn = h2.depssn and
				h2.actiontype in ('AD','W') And convert(smalldatetime,h2.auditdate) > convert(smalldatetime,h.auditdate)) 

-- create a table with latest UPDATE
SELECT subscriberno, personnumber, lastauditdate = max(convert(smalldatetime,auditdate))
INTO #ProvidersReportBSlastauditdate
FROM ProvidersReportABCHISTORY_5010
GROUP by subscriberno, personnumber

--Deleted COB before
Select distinct *
into #tmpDeletedCOB
from COBDataHist_5010 H
where h.actiontype = 'D' 
and h.auditdate <= @LastProvidersReportBSrun
and not exists (select 1 from COBDataHist_5010 h2 where
				 h.subscriberno = h2.subscriberno  and
				 h.enrollid = h2.enrollid and
				 h2.actiontype IN ('A','W') and convert(smalldatetime,h2.auditdate) > convert(smalldatetime,h.auditdate)) 


-- create a table with latest record UPDATEd
IF exists (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[_ProvidersReportBSlatesthistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP table [dbo].[_ProvidersReportBSlatesthistory]

SELECT h.*
INTO _ProvidersReportBSlatesthistory
FROM ProvidersReportABCHISTORY_5010 h
	inner join #ProvidersReportBSlastauditdate t on h.SubscriberNo = t.SubscriberNo and h.PersonNumber = t.PersonNumber
WHERE convert(smalldatetime,h.auditdate) = t.lastauditdate

-- INSERT Deleted participant
Begin

		SELECT distinct h.subscriberno
		INTO #tmpDeletedParticipants
		FROM _ProvidersReportBSlatesthistory h
        WHERE not exists (SELECT 1 FROM ProvidersReportABC_5010 
						 WHERE subscriberno = h.subscriberno and personnumber = '001')
        and h.personnumber = '001'
        and subscriberno not in (SELECT subscriberno FROM #tmpDeletedEE)
		--and CONVERT(datetime,auditdate) >= @LastProvidersReportBSWeekly
        
        INSERT INTO ProvidersReportABC_5010  (	RecordType,
        			EligGrpNo,
        			SubscriberNo,
        			Depssn,
        			PersonNumber,
        			TranCode,
        			EligEffective,
        			EligTermination,
        			Relationship,
        			LastName,
        			FirstName,
        			MiddleInitial,
        			DateofBirth,
        			Gender,
        			ssn,
        			CoverageCode,
        			Address,
        			AddressLine2,
        			AddressLine3,
        			City,
        			State,
        			ZipCode,
                    CountryCode,
        			CreditableCvrgInd,
        			PrimarySubsidyInd,
        			PrimarySubsidyEffDate,
        			PrimarySubsidyTermDate,
        			ClientAccountNbr,
        			MedicareDisenrollmentType,
        			AuditDate,
        			ActionType,
        			PersonId,
        			DateOfDeath,
        			CAPlanEndDate,
					PlanCode
				   ,MedicarePlanCode 
				   ,MedicareEligReasonCode 
				   ,MedicareEffDate 
				   ,MedicareTermDate
				   ,StatusCode
				   ,WorkDate
				   )
        SELECT 	RecordType,
        	EligGrpNo,
        	h.SubscriberNo,
        	h.Depssn,
        	PersonNumber,
        	@TranCode,
        	EligEffective,
        	case when (e.eligcancellation is null or e.eligcancellation < EligEffective)
        		 then EligEffective
        		 else convert(char(10),e.eligcancellation,112)
        		 end,
        	Relationship,
        	LastName,
        	FirstName,
        	MiddleInitial,
        	DateofBirth,
        	Gender,

        	h.ssn,
        	CoverageCode,
        	Address,
        	AddressLine2,
        	AddressLine3,
        	City,
        	State,
        	ZipCode,
            CountryCode,
        	CreditableCvrgInd,
        	PrimarySubsidyInd,
        	PrimarySubsidyEffDate,
        	PrimarySubsidyTermDate,
        	ClientAccountNbr,
        	MedicareDisenrollmentType,
        	getdate(),
        	'D',
        	PersonId,
        	DateOfDeath,
        	CAPlanEndDate,
			PlanCode
		   ,MedicarePlanCode 
		   ,MedicareEligReasonCode 
		   ,MedicareEffDate 
		   ,MedicareTermDate
		   ,h.StatusCode
		   ,h.WorkDate
        FROM _ProvidersReportBSlatesthistory h	
        inner join #tmpDeletedParticipants t on t.SubscriberNo = h.SubscriberNo  and h.personnumber = '001'
		INNER join (SELECT SSN = he.ssn,eligcancellation = max(EndDate)
					FROM #tmpDeletedParticipants t left outer join hedb..HCOEnrollment he
					on he.ssn = t.SubscriberNo
					and he.HCO in (SELECT HCO FROM hedb..HCODetail WHERE HCOTYPE ='M')
					AND he.HCO <> 'NM'
					and he.EndDate <= @WorkDate
					GROUP by he.ssn) E on e.ssn = H.SubscriberNo
		WHERE exists (SELECT 1 FROM hedb..Eligibility e WHERE e.ssn = t.SubscriberNo
					  and @WorkDate between e.EligEffective and e.EligCancellation
					  and e.statuscode not in ('PT','NI','CI'))


        INSERT INTO ProvidersReportABC_5010  (	RecordType,
        			EligGrpNo,
        			SubscriberNo,
        			Depssn,
        			PersonNumber,
        			TranCode,
        			EligEffective,
        			EligTermination,
        			Relationship,
        			LastName,
        			FirstName,
        			MiddleInitial,
        			DateofBirth,
        			Gender,
        			ssn,
        			CoverageCode,
        			Address,
        			AddressLine2,
        			AddressLine3,
        			City,
        			State,
        			ZipCode,
                    CountryCode,
        			CreditableCvrgInd,
        			PrimarySubsidyInd,
        			PrimarySubsidyEffDate,
        			PrimarySubsidyTermDate,
        			ClientAccountNbr,
        			MedicareDisenrollmentType,
        			AuditDate,
        			ActionType,
        			PersonId,
        			DateOfDeath,
        			CAPlanEndDate,
					PlanCode
				   ,MedicarePlanCode 
				   ,MedicareEligReasonCode 
				   ,MedicareEffDate 
				   ,MedicareTermDate
				   ,StatusCode
				   ,WorkDate
				   )
        SELECT 	RecordType,
        	EligGrpNo,
        	h.SubscriberNo,
        	h.Depssn,
        	PersonNumber,
        	@TranCode,
        	EligEffective,
        	case when (e.eligcancellation is null or e.eligcancellation < EligEffective)
        		 then EligEffective
        		 else convert(char(10),e.eligcancellation,112)
        		 end,
        	Relationship,
        	LastName,
        	FirstName,
        	MiddleInitial,
        	DateofBirth,
        	Gender,
        	h.ssn,
        	CoverageCode,
        	Address,
        	AddressLine2,
        	AddressLine3,
        	City,
        	State,
        	ZipCode,
			CountryCode,
        	CreditableCvrgInd,
        	PrimarySubsidyInd,
        	PrimarySubsidyEffDate,
        	PrimarySubsidyTermDate,
        	ClientAccountNbr,
        	MedicareDisenrollmentType,
        	getdate(),
        	'D',
        	PersonId,
        	DateOfDeath,
			CAPlanEndDate,
			PlanCode
		   ,MedicarePlanCode 
		   ,MedicareEligReasonCode 
		   ,MedicareEffDate 
		   ,MedicareTermDate
		   ,h.StatusCode
		   ,h.WorkDate
        FROM _ProvidersReportBSlatesthistory h
        inner join #tmpDeletedParticipants t on t.SubscriberNo = h.SubscriberNo  and h.personnumber = '001'
		INNER join (SELECT SSN = isnull(E.ssn,t.SubscriberNo),eligcancellation = max(EligCancellation)
					FROM #tmpDeletedParticipants t inner join hedb..Eligibility E
					on e.ssn = t.SubscriberNo
					and e.statuscode not in ('PT','NI','CI','SV')
					and e.EligCancellation <= @WorkDate
					GROUP by isnull(E.ssn,t.SubscriberNo)) E on e.ssn = H.SubscriberNo
		WHERE not exists (SELECT 1 FROM ProvidersReportABC_5010 WHERE subscriberNo = h.subscriberno and personnumber ='001')															
        
	 drop table #tmpDeletedParticipants
End

-- INSERT deleted dependent
Begin

		SELECT distinct H.SubscriberNo,H.depssn
		INTO #tmpDeletedDependents
		FROM _ProvidersReportBSlatesthistory h
        WHERE not exists(SELECT 1 FROM ProvidersReportABC_5010 c WHERE h.subscriberno = c.subscriberno
									and h.depssn = c.depssn)
        and h.personnumber <> '001'
        and h.depssn not in (SELECT d.depssn FROM #tmpDeletedDEP d WHERE h.subscriberno = d.subscriberno)
        --and CONVERT(datetime,auditdate) >= @LastProvidersReportBSWeekly

		-- Dependents whose eligibility has terminated,
        INSERT INTO ProvidersReportABC_5010  (	RecordType,
        			EligGrpNo,
        			SubscriberNo,
        			Depssn,
        			PersonNumber,
        			TranCode,
        			EligEffective,
        			EligTermination,
        			Relationship,
        			LastName,
        			FirstName,
        			MiddleInitial,
        			DateofBirth,
        			Gender,
        			ssn,
        			CoverageCode,
        			Address,
        			AddressLine2,
        			AddressLine3,
        			City,
        			State,
        			ZipCode,
					CountryCode,
        			CreditableCvrgInd,
        			PrimarySubsidyInd,
        			PrimarySubsidyEffDate,
        			PrimarySubsidyTermDate,
        			ClientAccountNbr,
        			MedicareDisenrollmentType,
        			AuditDate,
        			ActionType,
        			PersonId,
        	        DateOfDeath,
					CAPlanEndDate,
					PlanCode
				   ,MedicarePlanCode 
				   ,MedicareEligReasonCode 
				   ,MedicareEffDate 
				   ,MedicareTermDate
				    ,StatusCode
					,WorkDate)
        SELECT 	RecordType,
        	EligGrpNo,
        	h.SubscriberNo,
        	h.Depssn,
        	PersonNumber,
        	@TranCode,
        	EligEffective,
        	case when (e.eligcancellation is null or e.eligcancellation < EligEffective)
        		 then EligEffective
        		 else convert(char(10),e.eligcancellation,112)
        		 end,

        	Relationship,
        	LastName,
        	FirstName,
        	MiddleInitial,
        	DateofBirth,
        	Gender,
        	h.SSN,
        	CoverageCode,
        	Address,
        	AddressLine2,
        	AddressLine3,
        	City,
        	State,
        	ZipCode,
			CountryCode,
        	CreditableCvrgInd,
        	PrimarySubsidyInd,
        	PrimarySubsidyEffDate,
        	PrimarySubsidyTermDate,
        	ClientAccountNbr,
        	MedicareDisenrollmentType,
        	getdate(),
        	'D',
        	PersonId,
        	DateOfDeath,
			CAPlanEndDate,
			PlanCode
		   ,MedicarePlanCode 
		   ,MedicareEligReasonCode 
		   ,MedicareEffDate 
		   ,MedicareTermDate
		   ,h.StatusCode
		   ,h.WorkDate
        FROM _ProvidersReportBSlatesthistory h
        inner join #tmpDeletedDependents t on t.SubscriberNo = h.SubscriberNo and h.depssn = t.depssn		
		inner join (SELECT SSN = E.ssn,eligcancellation = max(EndDate)
					FROM #tmpDeletedDependents t left outer join hedb..HCOEnrollment E
					on e.ssn = t.SubscriberNo
					and e.HCO in (SELECT HCO FROM hedb..HCODetail WHERE HCOTYPE ='m')
					AND E.HCO <> 'NM'
					and e.EndDate <= @WorkDate
					GROUP by E.ssn) E on e.ssn = H.SubscriberNo	
		WHERE exists (SELECT 1 FROM hedb..DependentEligibility de
					  WHERE de.ssn = t.SubscriberNo and de.depssn = t.depssn
					  and @WorkDate between EligEffective and EligCancellation
					  and de.statuscode not in ('ID'))

		-- participant himself is not eliigble or not in bluecross, so family will be ineliigble also.
        INSERT INTO ProvidersReportABC_5010  (	
					RecordType,
        			EligGrpNo,
        			SubscriberNo,
        			Depssn,
        			PersonNumber,
        			TranCode,
        			EligEffective,
        			EligTermination,
        			Relationship,
        			LastName,
        			FirstName,
        			MiddleInitial,
        			DateofBirth,
        			Gender,
        			ssn,
        			CoverageCode,
        			Address,
        			AddressLine2,
        			AddressLine3,
        			City,
        			State,
        			ZipCode,
					CountryCode,
        			CreditableCvrgInd,
        			PrimarySubsidyInd,
        			PrimarySubsidyEffDate,
        			PrimarySubsidyTermDate,
        			ClientAccountNbr,
        			MedicareDisenrollmentType,
        			AuditDate,
        			ActionType,
        			PersonId,
        	        DateOfDeath,
					CAPlanEndDate,
					PlanCode,
				    MedicarePlanCode, 
				    MedicareEligReasonCode, 
				    MedicareEffDate, 
				    MedicareTermDate,
				    StatusCode,
					WorkDate
				    )
        SELECT 	RecordType,
        	EligGrpNo,
        	h.SubscriberNo,
        	h.Depssn,
        	PersonNumber,
        	@TranCode,
        	EligEffective,
        	case when (e.eligcancellation is null or e.eligcancellation < EligEffective)
        		 then EligEffective
        		 else convert(char(10),e.eligcancellation,112)
        		 end,
        	Relationship,
        	LastName,
        	FirstName,
        	MiddleInitial,
        	DateofBirth,
        	Gender,
        	h.SSN,
        	CoverageCode,
        	Address,
        	AddressLine2,
        	AddressLine3,
        	City,
        	State,
        	ZipCode,
			CountryCode,
        	CreditableCvrgInd,
        	PrimarySubsidyInd,
        	PrimarySubsidyEffDate,
        	PrimarySubsidyTermDate,
        	ClientAccountNbr,
        	MedicareDisenrollmentType,
        	getdate(),
        	'D',
        	PersonId,
        	DateOfDeath,
			CAPlanEndDate,
			PlanCode,
		    MedicarePlanCode, 
		    MedicareEligReasonCode, 
		    MedicareEffDate, 
		    MedicareTermDate,
		    h.StatusCode,
		    h.WorkDate
        FROM _ProvidersReportBSlatesthistory h
        inner join #tmpDeletedDependents t on t.SubscriberNo = h.SubscriberNo and  h.depssn = t.depssn
		inner join (SELECT SSN = isnull(E.ssn,t.SubscriberNo),DepSSN = isnull(e.depssn,t.depssn),eligcancellation = max(eligcancellation)
					FROM #tmpDeletedDependents t left outer join hedb..DependentEligibility e
					on e.ssn = t.SubscriberNo and e.depssn = t.depssn
					and e.statuscode not in ('ID')
					and e.eligcancellation <= @WorkDate
					GROUP by isnull(E.ssn,t.SubscriberNo),isnull(e.depssn,t.depssn)) E on e.ssn = H.SubscriberNo and e.depssn = H.depssn				
		WHERE not exists (SELECT 1 FROM ProvidersReportABC_5010 WHERE subscriberNo = h.subscriberno and depssn = h.depssn)					
        ORDER by auditdate desc		
     
        drop table #tmpDeletedDependents
End

--Get Deleted Medicare and Update Medicare Termination date
Update c
set MedicareTermDate = IsNull(CONVERT(char(8), dbo.fn_GetMedicareTermDate(c.SSN),112),c.MedicareEffDate) 
from ProvidersReportABC_5010  C
	inner join _ProvidersReportBSlatesthistory h on c.subscriberno = h.SubscriberNo and c.PersonNumber = h.PersonNumber
where c.MedicarePlanCode is NULL and h.MedicarePlanCode is not NULL and c.IsMPIActiveForMedicare is NULL
and convert(smalldatetime,h.MedicareEffDate) <> convert(smalldatetime,IsNull(h.MedicareTermDate,'12/31/2078'))
and not (c.StatusCode = 'SV' and c.personnumber = '001')
and not exists (select 1 from #tmpDeletedEE m where c.subscriberno = m.subscriberno and c.personnumber = m.personnumber)
and not exists(select 1 from #tmpDeletedDEP d where c.subscriberno = d.subscriberno and c.depssn = d.depssn)


--Update Medicare Information for deleted records
UPDATE b
SET MedicarePlanCode  = 'C'
   ,MedicareEligReasonCode  = CASE WHEN CharIndex('ESRD',Carrier) > 0 THEN '2' ELSE '' END
   ,MedicareEffDate  = IsNull(CONVERT(char(8), c.EffDate, 112),'')
   ,MedicareTermDate  = CASE WHEN c.TermDate = '12/31/2078' THEN NULL ELSE CONVERT(char(8), c.TermDate, 112) END
   ,MedicareXOverDate = IsNull(CONVERT(char(8), c.EffDate, 112),'')
--select * 
FROM dboutbound.dbo.ProvidersReportABC_5010 b
INNER JOIN dbo.COBData_5010_MPIClmData c 
	on b.ssn = c.ssn and b.ActionType = 'D'
WHERE b.IsMPIActiveForMedicare is Null -- only for not MPI ACTIVE cases for Medicare Purposes
	and COBType = 'M' 
	and not (b.StatusCode = 'SV' and personnumber = '001')
	and IsNull(convert(smalldatetime,MedicareEffDate),'01/01/1900') <> IsNull(convert(smalldatetime,EffDate),'01/01/1900')

/*---Commented MedXOver logic-------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------
and b.SubscriberNo not in (select SubscriberNo from MedicareXOverPartToDelete_5010)
------------------------------------------------------------------------------------------------------------
---Commented MedXOver logic-------------------------------------------------------------------------------*/


--Deleted COB
select subscriberno, personnumber, lastauditdate = max(convert(smalldatetime,auditdate))
into #COBlastauditdate --drop table #COBlastauditdate
from COBDataHist_5010
group by subscriberno, personnumber

IF exists (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[_COBlatesthistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP table [dbo].[_COBlatesthistory]

select h.*
into _COBlatesthistory--drop table _COBlatesthistory
from COBDataHist_5010 h
	inner join #COBlastauditdate t on h.SubscriberNo = t.SubscriberNo and h.PersonNumber = t.PersonNumber
where convert(smalldatetime,h.auditdate) = t.lastauditdate


INSERT COBData_5010   
SELECT SubscriberNo 
      ,DepSSN 
      ,PersonNumber
      ,COBType
      ,PrimaryStatus
	  ,COBEffDate  = IsNull(CONVERT(char(8), ch.COBEffDate, 112),'')
      ,COBTermDate  =  IsNull(CONVERT(char(8), dbo.fn_GetCOBTermDate(ch.Enrollid), 112),CONVERT(char(8), ch.COBEffDate, 112))
	  ,Carrier
	  ,CaseType
	  ,COBStatus
	  ,COBCode
      ,COBServiceTypeCode  = '1' --Always 1 for Madical care   
      , 'D' 
      ,EnrollId
      ,ch.StatusCode
	  ,ch.WorkDate
 FROM _COBlatesthistory ch 
 where not exists (Select 1 from  dbo.COBData_5010_MPIClmData c 
						where (Case When DepSSN is NULL then ch.SubscriberNo else ch.DepSSN end) = c.SSN 
						and COBType = 'C' and c.Enrollid = ch.EnrollId)
 and not exists (select 1 from #tmpDeletedCOB t where ch.SubscriberNo = t.SubscriberNo 
				and ch.PersonNumber = t.PersonNumber and ch.Enrollid = t.EnrollId)
				
--drop table _COBlatesthistory

/*---Commented MedXOver logic-------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------

--RG 01/12/2010 UPDATE EligEffective and EligTermination FROM las trun for the participant for Retirees out of state
--RG 01/14/2010 UPDATE to add Depssn
--RG 08/10/2011 UPDATE termination date again because for deleted records logic is dIFferent for split medicareXover couple
UPDATE r
set EligTermination = t.EligTermination
  , MedicareXOverDate = t.MedicareXOverDate 
  ,  MedicarePlanCode = t.MedicarePlanCode
  ,  MedicareEligReasonCode = t.MedicareEligReasonCode
  ,  MedicareEffDate =  t.MedicareEffDate
  ,  MedicareTermDate = t.MedicareTermDate
   --SELECT r.*, t.*             
FROM ProvidersReportABC_5010 r 
inner join MedicareXOverList_5010 t
	 on r.subscriberno = t.subscriberno and r.personnumber = t.personnumber 
     and r.actiontype = 'D'
     and t.MedicarePlanCode is not Null
------------------------------------------------------------------------------------------------------------
---Commented MedXOver logic-------------------------------------------------------------------------------*/

--/*06/01/2011 RG UPDATE to set maximum EligTermination for couple*/
--SELECT r.*
--     , EligTerminationMedicareXOver = (SELECT MAX(EligTermination) FROM ProvidersReportABC_5010 WHERE subscriberno = r.subscriberno GROUP by subscriberno)
--     , t.SSN SSN_t
--INTO #ProvidersReportBS_EligTerm1 --drop table #ProvidersReportBS_EligTerm1
--FROM ProvidersReportABC_5010 r 
--inner join MedicareXOverPartToDelete_5010 t 
--	on r.subscriberno = t.ssn  
--	and r.actiontype = 'D'

--UPDATE r
--set EligTermination = t1.EligTerminationMedicareXOver
----SELECT *
--FROM ProvidersReportABC_5010 r 
--inner join MedicareXOverPartToDelete_5010 t on r.subscriberno = t.ssn
--inner join #ProvidersReportBS_EligTerm1 t1 
--	on r.subscriberno = t1.subscriberno and IsNull(r.DepSSN,'') = IsNull(t1.DepSSN,'')
--	and r.actiontype = 'D'
	
UPDATE r
SET  EligTermination = CASE WHEN convert(smalldatetime,r.EligTermination) < convert(smalldatetime,r.EligEffective)
	        			    THEN r.EligEffective
	        			    ELSE r.EligTermination
	        		   END
FROM ProvidersReportABC_5010 r 
--inner join MedicareXOverPartToDelete_5010 t on r.subscriberno = t.ssn
--	and r.actiontype = 'D'
	

--update r
--set EligTermination = EligEffective
----select * 
--from ProvidersReportABC_5010 r
--where convert(smalldatetime,EligTermination) < convert(smalldatetime,EligEffective)

/*----------------------*/
drop table _ProvidersReportBSlatesthistory,#ProvidersReportBSlastauditdate,#tmpDeletedDEP,#tmpDeletedEE
--drop table #ProvidersReportBS_EligTerm

-- delete dependent ssn change old rec info
delete ProvidersReportABC_5010
FROM ProvidersReportABC_5010 P
inner join HEDB..PersonSSNHistory PH on DepSSN = SSNOLd
WHERE p.ActionType ='D'
and p.EligEffective = p.EligTermination
and exists (SELECT 1 FROM ProvidersReportABC_5010 p1 
			WHERE P1.SubscriberNo = P.subscriberNo
			and ((P1.DepSSN = PH.SSN
				 and ltrim(rtrim(P1.DateOfBirth)) =ltrim(rtrim(P.DateOfBirth))
				 and  ltrim(rtrim(P1.FirstName)) = ltrim(rtrim(P.FirstName))
				 and PH.HistoryId = (SELECT max(historyid) FROM  HEDB..PersonSSNHistory
						WHERE ssn = p1.depssn and ssnold = p.depssn))
			    or
				(P1.DepSSN <> PH.SSN
				and ltrim(rtrim(P1.DateOfBirth)) =ltrim(rtrim(P.DateOfBirth))
				and  ltrim(rtrim(P1.LastName)) =  ltrim(rtrim(P.LastName))
				and  ltrim(rtrim(P1.FirstName)) = ltrim(rtrim(P.FirstName))
				and ( ltrim(rtrim(P1.MiddleInitial)) =  ltrim(rtrim(P.MiddleInitial))
					 or 
					 (P1.MiddleInitial is null and P.MiddleInitial is null))
				))
			and p1.ActionType ='W')

--UPDATE to use C2 dependent address IF it's UPDATEd on or after 11/30/2010
UPDATE t
set    	Address = substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
						  ELSE  REPLACE(pa.Street,'#','') END,1,55),
		AddressLine2 = CASE WHEN pa.ForeignAddr = 1 
                        THEN Ltrim(Rtrim(Rtrim(substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55)) 
                                             + ' ' + Rtrim(IsNull(pa.State,'')) + ' ' + Rtrim(IsNull(pa.Postalcode,''))))
                        ELSE (substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55))
                        END,
		AddressLine3 = '',					
----		@City = case when pa.ForeignAddr = 0 then substring(pa.city,1,26)
----					else substring(rtrim(ltrim(rtrim(isnull(pa.state,'')) + ' ' + rtrim(isnull(pa.PostalCode,'')) + ' ' + rtrim(isnull(pa.country,'')))),1,26)
----				end,
		City = isNull(pa.city,''),
		State =  Upper(case when pa.ForeignAddr = 0  
									then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then IsNull(pa.state,'') 
												else ''
										  end)
							--for Canada
							when pa.ForeignAddr = 1  and isnull(pa.CountryCode,'') = 'CA' then left(IsNull(pa.state,''),2)
							else ''
					  end),
		ZipCode = Convert(varchar(9),case when pa.ForeignAddr = 0 
												then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then isNull(Replace(pa.Postalcode,'-',''),'') 
															else '' 
														end)
														 --for Canada
										 when pa.ForeignAddr = 1 and isnull(pa.CountryCode,'') = 'CA' then isNull(Replace(pa.Postalcode,'-',''),'') 										
										 else ''
									end),
        CountryCode = case when pa.ForeignAddr = 1 then case when IsNull(pa.CountryCode,'') = 'UK' then 'GB' 
																else IsNull(pa.CountryCode,'') 
														end        
							else ''
                       end,
        ReturnedMailDate = pa.ReturnedMailDate
--SELECT len(pa.Postalcode),pa.Postalcode,* 
FROM dbOutbound..ProvidersReportABC_5010 t 
			inner join hedb..dependent d (nolock) on t.subscriberno = d.ssn  and t.depssn = d.depssn
            inner join hedb..personaddress pa (nolock) on d.depssn = pa.ssn
            
where not (pa.Street is NULL or pa.State is NULL or pa.City is Null or pa.PostalCode is NULL or pa.City = '' or pa.PostalCode = '' 
or (pa.ReturnedMailDate is not NULL and pa.ReturnedMailDate <> ''))
--Only overwrite in Dependent is 18 or above
and dateadd(yy, 18, convert(smalldatetime,t.DateOfBirth)) < @WorkDate

--select top 10 * from dbOutbound..ProvidersReportABC_5010


--UPDATE address for dependents FROM dependent_addressused table
UPDATE t
set    	Address = substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
						  ELSE  REPLACE(pa.Street,'#','') END,1,55),
		AddressLine2 = CASE WHEN pa.ForeignAddr = 1 
                        THEN Ltrim(Rtrim(Rtrim(substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55)) 
                                             + ' ' + Rtrim(IsNull(pa.State,'')) + ' ' + Rtrim(IsNull(pa.Postalcode,''))))
                        ELSE (substring(CASE WHEN CHARINDEX('APT',pa.Street) = 0 THEN REPLACE(pa.Street,'#','APT ')
										ELSE  REPLACE(pa.Street,'#','') END,56,55))
                        END,
		AddressLine3 = '',					
----		@City = case when pa.ForeignAddr = 0 then substring(pa.city,1,26)
----					else substring(rtrim(ltrim(rtrim(isnull(pa.state,'')) + ' ' + rtrim(isnull(pa.PostalCode,'')) + ' ' + rtrim(isnull(pa.country,'')))),1,26)
----				end,
		City = isNull(pa.city,''),
		State =  Upper(case when pa.ForeignAddr = 0  
									then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then IsNull(pa.state,'') 
												else '' 
										  end)
							when pa.ForeignAddr = 1 and IsNull(pa.CountryCode,'') = 'CA' then left(IsNull(pa.state,''),2)
							else ''
					  end),
		ZipCode = Convert(varchar(9),case when pa.ForeignAddr = 0 
												then (Case When exists (select 1 from hedb..States where StateCode = IsNull(pa.state,'')) then isNull(Replace(pa.Postalcode,'-',''),'') 
														else '' 
														end)
									when pa.ForeignAddr = 1 and IsNull(pa.CountryCode,'') = 'CA' then isNull(Replace(pa.Postalcode,'-',''),'')	
							else ''
					   end),
        CountryCode = case when pa.ForeignAddr = 1  then case when IsNull(pa.CountryCode,'') = 'UK' then 'GB' 
																else IsNull(pa.CountryCode,'') 
														end  
							else ''
                       end,
        ReturnedMailDate = pa.ReturnedMailDate
--SELECT len(pa.Postalcode),pa.Postalcode,* 
FROM ProvidersReportABC_5010 t inner join hedb..dependent d on t.subscriberno = d.ssn  and t.depssn = d.depssn
            inner join hedb..dependent_addressused da on d.ssn = da.ssn and d.depssn = da.depssn
                       and da.suffixcode = t.personnumber
            inner join hedb..personaddress pa on d.depssn = pa.ssn


--Send an email to business users to on invalid zipcodes
declare @sqlQuery varchar(255)
	select @sqlQuery = 'select PersonId, rtrim(LastName) LastName, rtrim(FirstName) FirstName, ZipCode from dbOutbound.dbo.ProvidersReportABC_5010 where CountryCode in ('''',''US'')' +
						' and left(ZipCode, 5) not in (select left(Zip, 5) from OPUS.dbo.ZipCodes)'

exec dboutbound..sp_send_notice
				@Message = 'Address for these members is set to default MPI Address.',
				@Subject = 'Invalid US ZipCodes in 834 file to ABC',
				@Warning = 'Y',
				--@recipients = 'EligAdmin@mpiphp.org;',
				@recipients = 'EligAdmin@mpiphp.org;',
				@query	= @sqlQuery,
				@attach_results	= 1

--added on 09/12/2012 to remove blank GroupNumber records.
--select * from dbOutbound.dbo.ProvidersReportABC_5010
if exists (select 1 from dbOutbound.dbo.ProvidersReportABC_5010
				where EligGrpNo = '' or EligGrpNo is null)
begin
	select @sqlQuery = 'select SubscriberNo, Depssn, PersonNumber, EligGrpNo from dbOutbound.dbo.ProvidersReportABC_5010 where EligGrpNo = '''' or EligGrpNo is null'

	exec dboutbound..sp_send_notice
				@Message = 'Members with blank HCO GroupNumber from ABC 834 data.',
				@Subject = 'Members with blank HCO GroupNumber from ABC 834 data.',
				@Warning = 'Y',
				--@recipients = 'EligAdmin@mpiphp.org;',
				@recipients = 'EligAdmin@mpiphp.org;',
				@query	= @sqlQuery,
				@attach_results	= 1	
end

--Then delete the ones with blank GroupNumber
delete from dbOutbound.dbo.ProvidersReportABC_5010
	where EligGrpNo = '' or EligGrpNo is null

           
--SET MPI ADDRESS FOR ALL BAD ADDRESSES AND NO ADDRESSES            
UPDATE dbOutbound.dbo.ProvidersReportABC_5010
SET 	Address = 'c/o MPI',
		AddressLine2 = 'P.O. Box 1999',
     	City = 'Studio City',
		State =  'CA',
		ZipCode = '916140999',
		CountryCode = ''
--select * from dbOutbound.dbo.CurrentEligibles_ProvidersReportABC_5010
WHERE Address is NULL or Address = '' or State is NULL or City is Null or ZipCode is NULL or City = '' 
--ZipCode can be blank for ForeignAddr except CA (canada)
or (ZipCode = '' and CountryCode in ('', 'CA'))
--State can not be blank for US and CA (canada)
or (State = '' and CountryCode in ('', 'CA'))
--all us invalid zipcodes
or (CountryCode in('','US') and left(ZipCode, 5) not in (select left(Zip, 5) from OPUS.dbo.ZipCodes))
or (ReturnedMailDate is not NULL and ReturnedMailDate <> '')

---For all the above Bad Addresses, set the GroupNumber as mentioned below----------------------------------
--CA Participants		:277163M023
--OOS ME Participants	:277163M113
--OOS NME Participants	:277163M114

--select * from Hedb.dbo.HCOGroup
--select * from dbOutbound.dbo.ProvidersReportABC_5010

Update a set a.EligGrpNo = case when g.HCO = 'AC' then '277163M023'
								when g.HCO = 'AO' and g.MEFlag = 1 then '277163M113'
								when g.HCO = 'AO' and g.MEFlag = 0 then '277163M114'
							end
	from dbOutbound.dbo.ProvidersReportABC_5010 a
	inner join Hedb.dbo.HCOGroup g on a.EligGrpNo = isnull(rtrim(g.CaseNumber),'') + isnull(g.GroupNumber,'')
	WHERE Address = 'c/o MPI'
	and AddressLine2 = 'P.O. Box 1999'	
	
--Make sure that entire family has same GroupNumber (We only update GroupNumbers not the Address fields)
--(If Participant has BadAddress GroupNumber then the entire family should have the same even for the dependents with valid address)
--(If Participant has Valid GroupNumber then the entire family should have the same even for the dependents with Bad address)

Update a set a.EligGrpNo = s.EligGrpNo
	from dbOutbound.dbo.ProvidersReportABC_5010 a
	inner join dbOutbound.dbo.ProvidersReportABC_5010 s on a.SubscriberNo = s.SubscriberNo
	where s.DepSSN is null and s.PersonNumber = '001'		--s: Subscriber table
	and a.DepSSN is not null and a.PersonNumber <> '001'	--a: Dependent table
	

------------------------------------------------------------------------------------------------------------

UPDATE s set s.ZipCode = replace(s.ZipCode,' ','') 
from dbOutbound.dbo.ProvidersReportABC_5010 s

--Set the TermDate for SV/DP subscribers based on the DeathDate
UPDATE s set s.EligTermination = s.DateOfDeath
from dbOutbound.dbo.ProvidersReportABC_5010 s
where StatusCode in ('SV', 'DP')
and s.EligTermination > s.DateOfDeath
and s.DateOfDeath is not null
and s.DepSSN is null

--Set Termination date = Eff Date id Term Date < Eff Date
update r
set EligTermination = EligEffective
--select * 
from dbOutbound.dbo.ProvidersReportABC_5010 r
where EligTermination < EligEffective

-- truncate table
truncate table ProvidersReportABCFILE_5010

IF @debug = 1
	begin
		SELECT 'after ProvidersReportABCFILE truncated' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCFILE_5010
	end
-- INSERT header
exec usp_BlueShield_Eligibility_HDR_5010 @ProcessMode = 'T', @UPDATEType = 'P', @Debug = 0

INSERT INTO ProvidersReportABCFILE_5010 (field1000)
SELECT field1000 = convert(varchar(1000),	Recordtype + 
						CustomerId +
						CustomerName + 
						JobName +
						RecordLength +
						[BlockSize] +
						CurrentDate +
						CurrentTime +
						ContactPerson +
						PhoneNo +
						ProcessMode+
						Filler1 +
						VersionNo +
						ReleaseNo +
						UPDATEType+
						GeneratedTermDate +
						TransNo)
FROM ProvidersReportBS_HDR_5010

IF @debug = 1
	begin
		SELECT 'after ProvidersReportABCFILE filled with hdr' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCFILE_5010
	end
-- INSERT detail

INSERT INTO ProvidersReportABCFILE_5010 (field1000)
SELECT field1000 = convert(varchar(2000),	RecordType +
	EligGrpNo +
	SubscriberNo + 
	PersonNumber +
	isnull(TranSeq,'')  +
	isnull(TranDate,'')  +
	isnull(TranCode,'')  +
	isnull(PatientBenfGrp,'')  +
	isnull(EligEffective,'')  +
	isnull(EligTermination,'')  +
	isnull(ReservedField11,'')  +
	isnull(ReservedField12,'')  +
	isnull(ReservedField13,'')  +
	isnull(ReservedField14,'')  +
	isnull(Relationship,'')  +
	isnull(LastName,'')  +
	isnull(FirstName,'')  +
	isnull(MiddleInitial,'')  +
	isnull(Title,'')  +
	isnull(NameExtension,'')  +
	isnull(DateOfBirth,'')  +
	isnull(MultipleBirthCode,'')  +
	isnull(Gender,'')  +
	isnull(SSN,'')  +
	isnull(AltGrpNo,'')  +
	isnull(AltIDCode,'')  +
	isnull(AltIDNo,'')  +
	isnull(AltPersonCode,'') +
	isnull(left(Address,30),'')  +
	isnull(left(AddressLine2,30),'')  +
	isnull(AddressLine3,'')  +
	isnull(left(City,26),'')  +
	isnull(State,'')  +
	isnull(ZipCode,'')  +
	isnull(CityCode,'')  +
	isnull(CountyCode,'')  +
	isnull(CountryCode,'')  +
	isnull(TelephoneNo,'')  +
	isnull(COBIndicator,'')  +
	isnull(COBEffective,'')  +
	isnull(ReservedField41,'')  +
	isnull(PrimaryCarePtype,'') +
	isnull(PrimaryCarePID,'')  +
	isnull(PrimaryCareEff,'')  +
	isnull(PrimaryCareTerm,'')  +
	isnull(CoverageCode,'')  +
	isnull(CoverageCodeEffective,'')  +
	isnull(ReserveField48,'')  +
	isnull(ReserveField49,'')  +
	isnull(ReserveField50,'')  +
	isnull(ReserveField51,'')  +
	isnull(NCPDPCoverage,'') +
	isnull(DependentMaxAge,'') +
	isnull(StudentMaxAge,'') +
	isnull(DisDepMaxAge,'') +
	isnull(AdultDepMaxAge,'') +
	isnull(UserGrpNo,'')  +
	isnull(CoverageID,'')  +
	isnull(BenfIndex,'')  +
	isnull(PatientRptId,'')  +
	isnull(ReservedField61,'')  +
	isnull(ReservedField62,'')  +
	isnull(ReservedField63,'')  +
	isnull(ReservedField64,'')  +
	isnull(ReservedField65,'')  +
	isnull(ReservedField66,'')  +
	isnull(UserSeq,'')  +
	isnull(InsuranceCode,'')  +
	isnull(PayrollClass,'')  +
	isnull(EmployeeStatus,'')  +
	isnull(CardQuantity,'')  +
	isnull(CardIndtr,'')  +
	isnull(NamePrintIndtr,'')  +
	isnull(ClientDeducIndtr,'')  +
	isnull(ClientPassThru,'') +
	isnull(MedicareIndtr,'')  +
	isnull(MedicareEffective,'')  +
	isnull(MedicareBNo,'')  +
	isnull(Filler,'') +
	isnull(NoCopay,'')  +
	isnull(PriorAuthFlag,'')  +
	isnull(PriorAuthType,'')  +
	isnull(Filler2,'')  +
	isnull(PatientID,'')  +
	isnull(PaidToDate,'')  +
	isnull(PrimaryCareProvClinicID,'')  +
	isnull(CreditableCvrgInd,'') +
	isnull(Filler88,'')  +
	isnull(PrimarySubsidyInd,'')  +
	isnull(PrimarySubsidyEffDate,'')  +
	isnull(PrimarySubsidyTermDate,'')  +
	isnull(Filler92,'')  +
	isnull(HICRRBCode,'')  +
	isnull(HICRRBNbr,'')  +
	isnull(Filler95,'')  +
	isnull(MailDirectInd,'')  +
	isnull(BenefitCapAmt,'')  +
	isnull(EligCapEff,'')  +
	isnull(BenefitLIFetimeAmt,'')  +
	isnull(MedHlthPlanId,'')  +
	isnull(CommunCardGrp,'') +
	isnull([Filler103],'') + 
	isnull(MedicareDisenrollmentType,'') + 
	isnull([Filler105],'') +
	isnull(PersonId,'') +
    isnull(DateOfDeath,'') +
	isnull(MedicarePlanCode,'') +  
	isnull(MedicareEligReasonCode,'') +  
	isnull(MedicareEffDate,'') +  
	isnull(MedicareTermDate,'') 
	)
FROM dbOutBound..ProvidersReportABC_5010
WHERE subscriberno is not null
ORDER by eliggrpno, subscriberno, personnumber

IF @debug = 1
	begin
		SELECT 'after ProvidersReportABCFILE filled with detail' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCFILE_5010
	end
-- INSERT trailer
--declare @Totalrecs numeric(9)
SELECT @Totalrecs = isnull((SELECT count(*) FROM ProvidersReportABC_5010)+2,0)
exec  usp_BlueShield_Eligibility_TRL_5010 @Totalrecs, @Debug = 0


INSERT INTO dbOutBound..ProvidersReportABCFILE_5010 (field1000)
SELECT field1000 = convert(varchar(1000),Recordtype +
					CustomerId +
					CustomerName +
					JobName +
					RecordLength +
					[BlockSize] +
					CurrentDate +
					CurrentTime +
					ContactPerson +
					PhoneNo +
					TotalRecs)
FROM ProvidersReportBS_TRL_5010


IF @debug = 1
	begin
		SELECT 'after ProvidersReportABCFILE filled with trl' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCFILE_5010
	end
-- INSERT INTO ProvidersReportBShistory

IF @debug = 1
	begin
		SELECT 'inside the fill of ProvidersReportABChistory' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCHISTORY_5010
	end
		
		INSERT INTO ProvidersReportABCHISTORY_5010
		SELECT 
		
			RecordType,
			EligGrpNo,
			SubscriberNo,
			depssn,
			PersonNumber,
			TranSeq,
			TranDate,
			TranCode,
			PatientBenfGrp,
			EligEffective,
			EligTermination,
			ReservedField11,
			ReservedField12,
			ReservedField13,
			ReservedField14,
			Relationship,
			LastName,
			FirstName,
			MiddleInitial,
			Title,
			NameExtension,
			DateOfBirth,
			MultipleBirthCode,
			Gender,
			SSN,
			AltGrpNo,
			AltIDCode,
			AltIDNo,
			AltPersonCode,
			Address,
			AddressLine2,
			AddressLine3,
			City,
			State,
			ZipCode,
			CityCode,
			CountyCode,
			CountryCode,
			TelephoneNo,
			COBIndicator,
			COBEffective,
			ReservedField41,
			PrimaryCarePtype,
			PrimaryCarePID,
			PrimaryCareEff,
			PrimaryCareTerm,
			CoverageCode,
			CoverageCodeEffective,
			ReserveField48,
			ReserveField49,
			ReserveField50,
			ReserveField51,
			NCPDPCoverage,
			DependentMaxAge,
			StudentMaxAge,
			DisDepMaxAge,
			AdultDepMaxAge,
			UserGrpNo,
			CoverageID,
			BenfIndex,
			PatientRptId,
			ReservedField61,
			ReservedField62,
			ReservedField63,
			ReservedField64,
			ReservedField65,
			ReservedField66,
			UserSeq,
			InsuranceCode,
			PayrollClass,
			EmployeeStatus,
			CardQuantity,
			CardIndtr,
			NamePrintIndtr,
			ClientDeducIndtr,
			ClientPassThru,
			MedicareIndtr,
			MedicareEffective,
			MedicareBNo,
			Filler,
			NoCopay,
			PriorAuthFlag,
			PriorAuthType,
			Filler2,
			PatientID,
			PaidToDate,
			PrimaryCareProvClinicID,
			CreditableCvrgInd,
			Filler88,
			PrimarySubsidyInd,
			PrimarySubsidyEffDate,
			PrimarySubsidyTermDate,
			Filler92,
			HICRRBCode,
			HICRRBNbr,
			Filler95,
			MailDirectInd,
			BenefitCapAmt,
			EligCapEff,
			BenefitLIFetimeAmt,
			MedHlthPlanId,
			CommunCardGrp,
			ClientAccountNbr,
			Filler103,
			MedicareDisenrollmentType,
			Filler105,
			convert(varchar(10),AuditDate,101),
			ActionType,
			PersonId,
			DateOfDeath
		   ,MedicarePlanCode 
		   ,MedicareEligReasonCode 
		   ,MedicareEffDate 
		   ,MedicareTermDate
		   ,CAPlanEndDate
		   ,PlanCode
		   ,IsMPIActiveForMedicare
		   ,MedicareHICN
		   ,ReturnedMailDate
		   ,MedicareXOverDate
		   ,StatusCode
		   ,WorkDate
		FROM dbOutBound..ProvidersReportABC_5010

	IF @debug = 1
	begin
		SELECT 'after/overpassing the fill of ProvidersReportABCHISTORY_5010' + convert(varchar(20),getdate())
		SELECT * FROM ProvidersReportABCHISTORY_5010
	end
	
	--Update COB history
	Insert dbo.COBDataHist_5010
	(
		SubscriberNo 
		,DEPSSN 
		,PersonNumber
		,COBType 
		,PrimaryStatus 
		,COBEffDate
		,COBTermDate 
		,Carrier
		,CaseType 
		,COBStatus 
		,COBCode 
		,COBServiceTypeCode 
		,ActionType 
		,AuditDate
		,Enrollid
		,StatusCode
		,WorkDate
	)
	SELECT SubscriberNo 
			,DEPSSN 
			,PersonNumber
			,COBType 
			,PrimaryStatus 
			,COBEffDate
			,COBTermDate 
			,Carrier
			,CaseType 
			,COBStatus 
			,COBCode 
			,COBServiceTypeCode 
			,ActionType
			,GetDate()
			,Enrollid
			,StatusCode
			,WorkDate
    FROM dbo.COBData_5010
	
	
-- Copy file INTO outbound

	exec master..xp_cmdshell 'bcp "SELECT Field1000 FROM dbOutBound.dbo.ProvidersReportABCFILE_5010 ORDER by recordid" queryout \\mpihome01\Eligibility\Outbound\Tapes\ProvidersReportABCFile_5010.txt -c -S MPICOREDB01 -T'

	DECLARE @strFROM varchar(100)
	DECLARE @strTo varchar(100)
	DECLARE @strCmd varchar(256)
	SELECT @strFROM = 'COPY \\mpihome01\Eligibility\Outbound\Tapes\ProvidersReportABCFile_5010.txt'
	SELECT @strTo = '\\mpihome01\Eligibility\Outbound\Tapes\BlueShield_History\ProvidersReportABCFile_5010.txt' + '.' + CONVERT(varchar(10),GETDATE(),112)
	SELECT @strCmd = LTRIM(RTRIM(@strFROM)) + ' ' + LTRIM(RTRIM(@strTo))
	exec master..xp_cmdshell @strCmd


	-- IF total record counts less than 10, send warning meesage
	IF @Totalrecs < 10
	begin
	SELECT @Message = 'Warning!  The Blue Cross Weekly file record count is ' + convert(varchar(3),@Totalrecs) +  ', please verIFy.'
	exec dboutbound..sp_send_notice
				@Message = @Message,
				@Subject ='Blue Shield file Warning',
				@Warning ='Y'
	end
	else
	Begin
	-- Message of the Email	
	SELECT @strRecipents = 'EligIT@mpiphp.org; '
	SELECT @message = 'Blue Cross Weekly file had been created.  Please verIFy.'
	SELECT @message  = @message + char(13) + char(13) + 'file location:  \\mpihome01\Tapes\ProvidersReportABCFile_5010.txt' + char(13)
	SELECT @message = @message + char(13) + 'Total Record send:' + CONVERT(varchar(8), @Totalrecs)


	EXEC dboutbound..sp_send_notice
		@recipients = @strRecipents,
		@subject = 'Blue Cross Weekly file ready for verIFication',
		@message = @message
	end

 --Writing the log 
SELECT	@DOSCommand = '@echo ' + convert(varchar(30), getdate(), 109) + '   Blue Cross Weekly file has been successfully created for' +  convert(varchar(10),@WorkDate,101) + convert(char(7), count(*) - 2) + 'records written... >> \\mpihome01\Eligibility\Outbound\Tapes\TapeLog.txt' FROM ProvidersReportABCFile_5010
exec master..xp_cmdshell @DOSCommand
return 0

end
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON




/****** Object:  StoredProcedure [dbo].[USP_PID_Merge]    Script Date: 03/18/2013 14:50:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*-- =============================================
-- Author:		Vidya Nair
-- Create date: 11/08/2007
-- Description:	SP to call for PID Merge in Eligibility
--
--Modification History
--Mxie 5.2.2008   change @MergeDate's datatype from varchar to datetime.
--                when calling sp_SSNMerge, do not pass @RecdDate with value.
--RG  06/07/2012  Updated to fix merge for Eligibility when PID and SSN for final person record are not from the same record.
--Raj	03/18/2013	Since SSN can be blank in OPUS, and HE does not accept blank SSN (it creates its own SSN when
--					a member came with blank SSN) read the SSN from HE based on the PIDs passed.
--Raj	03/19/2013	Comment out the code to call eadb.[dbo].[USP_PID_Merge] since OPUS call directly EADB proc.
--Raj	03/20/2013	Since OPUS can cross swap (X) the SSNs and PIDs in SSN/PID merge, we need to change the logic as explained below:
					From the passed in parameters ignore the OldSSN (read from the OldPID passed).
					
					  P1(old)	     S1
						*         *
						  *	   *
							 *
						   *   *
						*         *
					  P2(new)       S2
					
					if both the SSNs to be merged is blank (dummy SSNs in HE), then keep one randomly.
--04/06/2013	Raj		Added code to insert current record into OPUSPersonInfoUpdateActivity table to be used
						to update QXNT data.
					
--Testing:
exec [dbo].[USP_PID_Merge]
	@OldPID				= 'M20019299'
	,@OldSSN			= ''
	,@NewPID			= 'M30168400'
	,@NewSSN			= '213800382'
	,@Notes				= ''
	,@ADH				= ''
	,@MergeDate			= '03/21/2013'
	,@debug				= 1
				
-- =============================================*/
ALTER PROCEDURE [dbo].[USP_PID_Merge]
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

	/*---For Testing----------------------------------------------------------
	declare @OldPID			varchar(15),
			@OldSSN			varchar(10),
			@NewPID			varchar(15),
			@NewSSN			varchar(10),
			@Notes			varchar(255),
			@ADH			varchar(255),
			@MergeDate		datetime ,			
			@debug bit 
			
	select @OldPID			= 'M20019299',
			@OldSSN			= '',
			@NewPID			= 'M30168400',
			@NewSSN			= '213800382',
			@Notes			= '',
			@ADH			= null,
			@MergeDate		= GETDATE(),
			@debug = 0
			
	--MPI_PERSON_ID	FIRST_NAME	LAST_NAME	DATE_OF_BIRTH	SSN
	--M30168400	AARON	BECKER	1967-08-03 00:00:00.000	
	--M20019299	AARON	BECKER	1974-07-14 00:00:00.000	213800382
	------------------------------------------------------------------------*/


declare   @pid			char(15)     
		, @rtn  		int	
		, @ssn 			varchar(10)
		, @ChangeType		varchar(10)
		, @PartSSN			char(10)
		, @errMsg			varchar(1000)
		, @OldPIDSSN		varchar(10)
		, @NewPIDSSN		varchar(10)

	if exists (select 1 from tempdb.sys.all_objects o where o.object_id = object_id('tempdb.dbo.#tmpErrorMsg') and type = 'U')
		drop table #tmpErrorMsg
	create table #tmpErrorMsg (ErrMsg  varchar(1000),StopChange bit)


	--03/20/2013 (Raj), get the existing SSN as they are before merge.
	select @OldPIDSSN = SSN from Eligibility_PID_Reference WHERE PID = @OldPID
		
	select @NewPIDSSN = SSN from Eligibility_PID_Reference WHERE PID = @NewPID
	
	if @debug = 0
		select @NewPIDSSN NewPIDSSN, @OldPIDSSN OldPIDSSN, @OldSSN OldSSN, @NewSSN NewSSN, @OldPID OldPID, @NewPID NewPID
	
	--Compare the @NewSSN input parameter with the above 2 SSNs
	
	--if both SSNs are dummy then keep one randomly (The @NewSSN got to be blank).
	if (len(@OldPIDSSN) = 10 and LEFT(@OldPIDSSN, 1) = '1')
		and (len(@NewPIDSSN) = 10 and LEFT(@NewPIDSSN, 1) = '1') 
		and isnull(@NewSSN,'') = ''
		begin
		select @OldSSN = @OldPIDSSN, @NewSSN = @NewPIDSSN
		goto SkipTo
		end
	---------------------------------------------------------------------------------------------------------
	--If @NewSSN is blank, but one of @OldPIDSSN and @NewPIDSSN is blank, keep the one which is dummy/blank.
	--@OldPIDSSN is dummy
	if (isnull(@NewSSN,'') = '' 
		and len(@OldPIDSSN) = 10 and LEFT(@OldPIDSSN, 1) = '1'
		and len(@NewPIDSSN) = 9
		)
		begin
		select @NewSSN = @OldPIDSSN, @OldSSN = @NewPIDSSN
		goto SkipTo
		end
				
	--@NewPIDSSN is dummy
	if (isnull(@NewSSN,'') = '' 
		and len(@NewPIDSSN) = 10 and LEFT(@NewPIDSSN, 1) = '1'
		and len(@OldPIDSSN) = 9
		)
		begin
		select @NewSSN = @NewPIDSSN, @OldSSN = @OldPIDSSN
		goto SkipTo
		end
	---------------------------------------------------------------------------------------------------------
	--If @NewSSN is not blank, but one of @OldPIDSSN and @NewPIDSSN is blank, keep the one which is not dummy/blank.
	--@NewSSN <> '' and @NewPIDSSN is dummy then keep @OldPIDSSN as @NewSSN
	if (isnull(@NewSSN,'') <> '' and len(@NewPIDSSN) = 10 and LEFT(@NewPIDSSN, 1) = '1')
		begin
		select @NewSSN = @OldPIDSSN, @OldSSN = @NewPIDSSN
		goto SkipTo
		end
		
	--@NewSSN <> '' and @OldPIDSSN is dummy then keep @NewPIDSSN as @NewSSN
	if (isnull(@NewSSN,'') <> '' and len(@OldPIDSSN) = 10 and LEFT(@OldPIDSSN, 1) = '1')
		begin
		select @NewSSN = @NewPIDSSN, @OldSSN = @OldPIDSSN
		goto SkipTo
		end
	---------------------------------------------------------------------------------------------------------
	--if both  @NewPIDSSN and @OldPIDSSN are not dummy then use the input parameters as we got them.
	
	
	SkipTo:
	if @Debug = 0
		select @OldSSN OldSSN, @NewSSN NewSSN, @OldPID OldPID, @NewPID NewPID, @NewPIDSSN NewPIDSSN, @OldPIDSSN OldPIDSSN
		
	
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

		--exec eadb.[dbo].[USP_PID_Merge] @OldPID,	@OldSSN, @NewPID, @NewSSN 

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
    -- update eligibility_pid_reference
		if exists (select 1 from Eligibility_PID_Reference where pid <> @NewPID and ssn = @NewSSN)
		begin
			update Eligibility_PID_Reference 
			set PID = @NewPID,
				ModifiedUser = suser_sname(),
				ModifiedDate = getdate()
			where SSN = @NewSSN --and pid = @OldPID
			if @@error != 0
				begin
					raiserror 99999 'Eligibility Reference Update failed.'
					return 99999	
				end
		end 
		if exists (select 1 from Eligibility_PID_Reference where pid = @OldPID)
		begin
			delete from Eligibility_PID_Reference 
			where pid = @OldPID
			if @@error != 0
			begin
				raiserror 99999 'Eligibility Reference Delete failed.'
				return 99999	
			end
		end    
	End
		
	insert into dbo.OPUSPersonInfoUpdateActivity (PID, SSN, UpdateDate, UpdateUser, ActivityType, Processed)
	select @NewPID, @NewSSN, GETDATE(), USER_NAME(), 'SSN Merge', 0
		
return 0
END
/*--------------Testing--------------------------------------------------------------------------

select * from OPUS.dbo.Person

select * from Opus.dbo.SGT_PERSON
	where SSN = ''
	
select 
	P1.PERSON_ID, P1.MPI_PERSON_ID, P1.FIRST_NAME, P1.LAST_NAME, P1.DATE_OF_BIRTH, P1.SSN
	,P2.PERSON_ID, P2.MPI_PERSON_ID, P2.FIRST_NAME, P2.LAST_NAME, P2.DATE_OF_BIRTH, P2.SSN
 
	from Opus.dbo.SGT_PERSON p1
	inner join opus.dbo.SGT_PERSON p2 on p1.FIRST_NAME = p2.FIRST_NAME and p1.LAST_NAME = P2.LAST_NAME
	where p1.MPI_PERSON_ID <> P2.MPI_PERSON_ID
	and p1.DATE_OF_BIRTH is not null and p2.DATE_OF_BIRTH is not null
	order by P1.FIRST_NAME, P1.LAST_NAME, P1.DATE_OF_BIRTH
	
select * from Opus.dbo.SGT_PERSON
	where 1 = 1
	and First_name = 'AARON'
	and last_name = 'Johnson'
	
MPI_PERSON_ID	FIRST_NAME	LAST_NAME	DATE_OF_BIRTH	SSN
M30168400	AARON	BECKER	1967-08-03 00:00:00.000	
M20019299	AARON	BECKER	1974-07-14 00:00:00.000	213800382

declare @OldSSN varchar(10), @NewSSN varchar(10)
select @OldSSN = '', @NewSSN = ''

select * from Hedb.dbo.Eligibility_PID_Reference where SSN in (@OldSSN, @NewSSN)
select * from Participant where SSN in ('', '')
LifeInsBeneficiary where SSN in ('', '')
Dependent where SSN in ('', '')
Person where SSN in ('', '')
Organization where SSN in ('', '')
PersonAddress where SSN in ('', '')
PersonAddressHist where SSN in ('', '')
LifeInsDocLog where SSN in ('', '')
LifeInsPaymentLog
LifeInsPaymentLog_Audit
LifeInsBeneficiary_Audit
Elig_PID_NewDependentBeneficiaries
PersonSSNHistory

select * from Hedb.dbo.Eligibility_PID_Reference where PID in ('M30168400', 'M20019299')

--M30168400	AARON	BECKER	1967-08-03 00:00:00.000	
	--M20019299	AARON	BECKER	1974-07-14 00:00:00.000	213800382
-----------------------------------------------------------------------------------------------*/




/****** Object:  StoredProcedure [dbo].[USP_PID_Person_ins]    Script Date: 01/16/2012 17:43:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





/*-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information
-- RG 04/10/2009 - If invalid SexCode, replace with ''
-- Raj	03/14/2013	Check the input @SSN and @Gender for blank condition.
-- Raj	04/06/2013	While adding beneficiary with no SSN, OPUS calls  [USP_PID_Person_ins] SP.
					Check If the beneficiary already exists or not before creating a new person record.
-- =============================================*/
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
	
	--Raj added on 04/06/2013. While adding beneficiary with no SSN, OPUS calls  [USP_PID_Person_ins] SP.
	--Check If the beneficiary already exists or not before creating a new person record.
	select @SSN = SSN 
		from Eligibility_PID_Reference (nolock) 
		where PID = @PID


if @RelationType in ('B','D')
begin
	If isnull(@SSN,'') = ''
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
if isnull(@Gender,'') not in ('','M','F')
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




/****** Object:  StoredProcedure [dbo].[USP_PID_Person_UPD]    Script Date: 03/15/2013 12:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/*-- =============================================
-- Author:		Vidya Nair
-- Create date: 10/25/2007
-- Description:	New sp to add person along with PID information		
--				If SSN is being changed will call Bene SSN Change,
--				Dep SSN Change or Part SSN Change respectively.		
--Modified History:
--3/17/2008   run rollover sp when DOD or SSN is changed...
--4/02/2008   if the updated person is a dependent, the DOB and gender can not be blank...
--4/18/2008   ParticipantSSN can not be null because the sp need it to run rollover. So if participantPID is not passed,
--            this sp will use @PID as @participantPID...	
--06/10/2009  MX Sending old name to the personalias table if name was changed	
--03/15/2013	Raj		Since SSN is not madantory in OPUS and it can be blank. And we generate our own dummy SSN
						in Eligibility so using the given @PID get the SSN from Eligibility and use it in the procedure
						to update other tables.
--03/22/2013	Raj		Commented out eadb.[dbo].[USP_PID_Merge] call since Opus calls EA directly.
--04/06/2013	Raj		Added code to insert current record into OPUSPersonInfoUpdateActivity table to be used
						to update QXNT data.
-- =============================================*/
ALTER procedure [dbo].[USP_PID_Person_UPD]
(
  @PID								varchar(15)
, @SSN                          	varchar(10)    	 = NULL
, @ParticipantPID					varchar(15)		 = NULL 
, @EntityTypeCode					varchar(3)		 = 'P'	-- This field have value 'P' for Person Record and 'T' or 'O' for Trust or Organization
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
--, @AuditDate                    	datetime      = NULL
, @AuditUser                    	varchar(30)   = NULL
)
AS
BEGIN
set nocount on

--if relationtype is dependent, check date of birth and gender
declare	@errornumber	int,
	@errormessage	varchar(255)

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

declare @AuditDate datetime
if @AuditUser is null or @AuditUser =''
	select @AuditUser = suser_sname()

	select @AuditDate = getdate()


declare	  @rtn     			int	
		, @OrgDateofDeath	datetime
		, @SuffixCode		varchar(2)
		, @DependentCode 	varchar(50)
		, @DivorceDate		datetime
		, @MarriageDate		datetime
		, @RemarriageDate	datetime
		, @StartDate		datetime
		, @CutOffDate		datetime
		, @EligEffective	datetime
		, @EligCancellation	datetime
		, @AuditDate1 		datetime
	    , @NotificationDate datetime
		, @DNAuditDate datetime
		, @LSAuditDate datetime 
		, @FEAuditDate datetime																
		, @OrgSSN			char(10)
		, @ChangeType		varchar(10)
		, @PartSSN			char(10)
		, @errMsg			varchar(1000)


--MXIE sp needs to pass deathcode to the sub-sp
declare @DeathCode char(1)
select @DeathCode = DeathCode from deathnotification where ssn = @SSN

declare @ParticipantSSN varchar(10)

--03/15/2013 (Raj)
if isnull(@SSN,'') = ''
	select @SSN = ssn from Eligibility_PID_Reference WHERE PID = @PID


if @ParticipantPID is not null and not exists(select 1 from Eligibility_PID_Reference WHERE PID = @ParticipantPID)
begin
		raiserror 99999 'ParticipantPID does not existed in Eligibility.'
		return 99999
end

if @ParticipantPID is not null and exists(select 1 from Eligibility_PID_Reference WHERE PID = @ParticipantPID)
begin	
		select @ParticipantSSN = SSN from Eligibility_PID_Reference WHERE PID = @ParticipantPID
end

declare @rolloverstartdate datetime
select @rolloverstartdate = rolloverstartdate from participant where SSN = @ParticipantSSN

--************Sending old name to the personalias table if name was changed.************
if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
Begin
	declare  @FirstName_hedb varchar(50)
			, @MiddleName_hedb varchar(50)
			, @LastName_hedb varchar(50)
	select @FirstName_hedb = firstname,
			@MiddleName_hedb = middlename,
			@LastName_hedb = lastname
	from person 
	where ssn = @SSN

	if (rtrim(ltrim(isnull(@FirstName_hedb, ''))) <>  rtrim(ltrim(isnull(@FirstName, '')))
		 or rtrim(ltrim(isnull(@MiddleName_hedb, ''))) <> rtrim(ltrim(isnull(@MiddleName, '')))
		 or rtrim(ltrim(isnull(@LastName_hedb, ''))) <> rtrim(ltrim(isnull(@LastName, ''))))
	Begin
		if not exists(select * from personAlias where ssn = @SSN
													and firstname = @FirstName_hedb
													and middlename = @MiddleName_hedb
													and lastname = @LastName_hedb)
			insert into personAlias (SSN, FirstName, MiddleName, LastName, StartDate, AuditUser, AuditDate)
			values(@SSN, @FirstName_hedb, @MiddleName_hedb, @LastName_hedb, getdate(), @AuditUser, getdate())
	End

End
--**************************************************************************************

	 create table #tmpErrorMsg (ErrMsg  varchar(1000),StopChange bit)
	
		-- update the record only if exists
		if  exists (select 1 from Eligibility_PID_Reference WHERE PID = @PID)
		Begin

			select @OrgSSN = SSN from Eligibility_PID_Reference WHERE PID = @PID
			-- SSN has been changed, call required process & then continue		
			if LTRIM(RTRIM(@OrgSSN)) <> LTRIM(RTRIM(@SSN))
			Begin
	
				--exec eadb.[dbo].[USP_PID_Merge] @PID, @OrgSSN, @PID, @SSN 
				-- means Beneficiary				
				if exists (select 1 from LifeInsBeneficiary where BeneSSN = @OrgSSN)
					and not exists (select 1 from  Participant where SSN = @OrgSSN)  
					and not exists (select 1 from  Dependent where DEPSSN = @OrgSSN)  
				Begin

					exec @rtn = sp_BeneficiaryMerge  @OldSSN	= @OrgSSN,
													 @NewSSN	= @SSN
					if @rtn <> 0
					begin
						raiserror 99999 'Beneficiary SSN Merge Failed.'
						return 99999	
					end
				End

				if exists (select 1 from  Participant where SSN = @OrgSSN)
					select @ChangeType ='Part'

					if @ChangeType is null and 
					   exists (select 1 from  Dependent where DepSSN = @OrgSSN)
						select @ChangeType ='Dep'

				if @ChangeType is not null
				Begin
					if @changetype ='Part'
					begin
						insert into #tmpErrorMsg
						exec sp_SSNChangeValid @OldSSN = @OrgSSN,@NewSSN = @SSN,@PartSSN = NULL,@ChangeType = @ChangeType
					end
					else
					begin
						declare crec cursor for select SSN from Dependent where DepSSN = @OrgSSN
						open crec
						fetch next from crec into @PartSSN
						while @@fetch_status =0
						begin
							insert into #tmpErrorMsg
							exec sp_SSNChangeValid @OldSSN = @OrgSSN,@NewSSN = @SSN,@PartSSN = @PartSSN,@ChangeType = @ChangeType	

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
							exec @rtn = sp_SSNMerge @SSNOld	= @OrgSSN,
													@SSNNew	= @SSN,
													@Notes = 'SSN Merged by PID System'
		
							if @rtn <> 0
							begin
								raiserror 99999 'Participant SSN Merge Failed.'
								return 99999	
							end
							exec sp_Rollover_Participant @SSN, @rolloverstartdate
						 end
						 else
						 begin
							exec @rtn = sp_ChangeDepSSN @DepSSN = @OrgSSN,
														@SSN	= @PartSSN,
														@NewSSN	= @SSN
							if @rtn <> 0
							begin
								raiserror 99999 'Dependent SSN Merge Failed.'
								return 99999	
							end
						 end
					End				
				End
--				--RUN ROLLOVER AFTER SSN CHANGED...
--				--if this ssn is a participant, the procedure need to get the new participant ssn...
--				select @ParticipantSSN = SSN from Eligibility_PID_Reference WHERE PID = @ParticipantPID
--				exec sp_Rollover_Participant @ParticipantSSN, @rolloverstartdate

				--For those new created person, when ssn changed...
				if not exists (select 1 from LifeInsBeneficiary where BeneSSN = @OrgSSN)
					and not exists (select 1 from  Participant where SSN = @OrgSSN)  
					and not exists (select 1 from  Dependent where DEPSSN = @OrgSSN)
					and exists(select 1 from person where ssn = @OrgSSN)
				begin
					update Eligibility_PID_Reference
					set ssn = @SSN,
						ModifiedDate = getdate(),
						ModifiedUser = @AuditUser
					where pid = @PID
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Eligibility Reference Information'
						return 99999
					End	

					update person
					set ssn = @SSN,
						AuditDate = getdate(),
						AuditUser = @AuditUser
					where ssn = @OrgSSN
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Person Information'
						return 99999
					End	

					update personaddress
					set ssn = @SSN,
						AuditDate = getdate(),
						AuditUser = @AuditUser
					where ssn = @OrgSSN
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Person Address Information'
						return 99999
					End	
					
					update Elig_PID_NewDependentBeneficiaries
					set depssn = @SSN,
						AuditDate = getdate(),
						AuditUser = @AuditUser
					where --ssn = @ParticipantSSN	and type = @RelationType and 
						depssn = @OrgSSN
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Eligibility Relationship Information'
						return 99999
					End	
			
				end
			End

				--For those new created organization beneficiary, when ssn changed...
			--MM 07/08/08 Added code to update Organization tax id is it is changed by user.
				if not exists (select 1 from LifeInsBeneficiary where BeneSSN = @OrgSSN)
					and exists(select 1 from Organization where Taxid = @OrgSSN)
				begin
					update Eligibility_PID_Reference
					set ssn = @SSN,
						ModifiedDate = getdate(),
						ModifiedUser = @AuditUser
					where pid = @PID
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Eligibility Reference Information'
						return 99999
					End	

					update Elig_PID_NewDependentBeneficiaries
					set depssn = @SSN,
						AuditDate = getdate(),
						AuditUser = @AuditUser
					where --ssn = @ParticipantSSN	and type = @RelationType and 
						depssn = @OrgSSN
					if @@error <> 0
					Begin
						--rollback tran
						raiserror 99999 'Error updating Eligibility Relationship Information'
						return 99999
					End	

					update Organization Set Taxid = @SSN WHERE Taxid = @OrgSSN


				end
			
			
			-- Update Person Record
			if exists (select 1 from Person WHERE SSN = @SSN)
			Begin
				select @AuditDate = AuditDate,
					   @OrgDateofDeath = DateofDeath
					   from Person where SSN = @SSN

				exec @rtn = person_upd  @ssn 				= @ssn,
										@sexcode			= @Gender,
										@firstname			= @firstname,
										@middlename 		= @middlename,
										@lastname 			= @lastname,
										@dateofbirth		= @dateofbirth,
										@dateofdeath		= @dateofdeath,
										@Phone1				= @HomePhone,
									    @Phone2				= @Fax   ,
									    @Email				= @Email ,
									    @Mobile				= @CellPhone,
										@AuditDate			= @AuditDate,
									    @AuditUser			= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end


				SELECT @NotificationDate = NotificationDate FROM DeathNotification WHERE SSN = @ssn
				SELECT @FEAuditDate = AuditDate FROM FuneralExpenses WHERE SSN = @ssn
				SELECT @DNAuditDate = AuditDate FROM DeathNotification WHERE SSN = @ssn
				SELECT @LSAuditDate = AuditDate FROM LifeInsPaymentStatus WHERE SSN = @ssn

				--IF Date of Death was not null and being CHANGED.
				if (@OrgDateofDeath is NOT null and @DateofDeath is not null and @DateofDeath <> @OrgDateofDeath)
				Begin
					if exists (select 1 from Participant where SSN = @SSN)
					Begin
						--if payment exists do not change date of death.
						if exists(select * FROM  LifeInsPaymentLog where SSN = @SSN)
						begin
							raiserror 99999 'Error updating date of death because payment has been made.'
							return 99999
						end
						--Add audit date parameter for related nested SPs.
						exec [dbo].[sp_DeathNotification_upd]  @SSN = @SSN
																, @NotificationDate = @NotificationDate
																, @DateOfDeath = @DateofDeath
																, @DeathCode = @DeathCode --MXIE, deathcode info is necessary
																, @DNAuditDate = @DNAuditDate
																, @LSAuditDate = @LSAuditDate
																, @AuditUser = @AuditUser
						
					End
				End

				--IF Date of Death is being removed.
				if ((@OrgDateofDeath is NOT null)
					and (@DateofDeath is null OR @DateofDeath = ''))
				Begin

					if exists (select 1 from Participant where SSN = @SSN)
					Begin
						--if payment exists do not delete date of death.
						if exists(select * FROM  LifeInsPaymentLog where SSN = @SSN)
						begin
							raiserror 99999 'Error removing date of death because payment has been made.'
							return 99999
						end

						--Add audit date parameter for related nested SPs.
						exec [dbo].[sp_DeathNotification_del]  @SSN = @SSN
																, @NotificationDate = @NotificationDate
																, @DateOfDeath = @DateofDeath 
																--, @DeathCode = @DeathCode --MXIE, deathcode info is necessary
																, @DNAuditDate = @DNAuditDate
																, @FEAuditDate = @FEAuditDate
																, @LSAuditDate = @LSAuditDate
																, @AuditUser = @AuditUser
						
					End
					Else
					Begin
						-- Dependent
						if exists (select 1 from Dependent where DepSSN = @SSN 
								   and DependentCode in ('SD'))
						Begin
							 select @PartSSN				 = SSN
								  , @DependentCode           = DependentCode
								  , @SuffixCode			     = SuffixCode
								  , @DivorceDate             = DivorceDate             
								  , @MarriageDate            = MarriageDate            
								  , @ReMarriageDate          = ReMarriageDate          
								  , @StartDate               = StartDate               
								  , @CutoffDate              = CutoffDate              
								  , @EligEffective           = EligEffective           
								  , @EligCancellation        = EligCancellation         
								  , @AuditDate1              = AuditDate
							   from Dependent where DepSSN = @SSN 
								   and DependentCode in ('SD')

							exec @rtn = sp_Dependent_upd  
									@SSN					 = @SSN
								  , @ParticipantSSN		     = @PartSSN
								  , @DependentCode           = 'SP'
								  , @DateOfDeath			 = @DateOfDeath 
								  , @SuffixCode			     = @SuffixCode 
								  , @DivorceDate             = @DivorceDate             
								  , @MarriageDate            = @MarriageDate            
								  , @ReMarriageDate          = @ReMarriageDate          
								  , @StartDate               = @StartDate               
								  , @CutoffDate              = @CutoffDate              
								  , @EligEffective           = @EligEffective           
								  , @EligCancellation        = @EligCancellation         
								  , @AuditDate               = @AuditDate1
							if @rtn != 0
							begin
								return @rtn
							end
						End
					End		
				End

				--IF Date of Death is being inserted.
				if ((@OrgDateofDeath is null or @OrgDateofDeath ='1/1/1900')
					and (@DateofDeath is not null and @DateofDeath <> '1/1/1900'))
				Begin
					-- Participant
					if exists (select 1 from Participant where SSN = @SSN)
					Begin
						select @NotificationDate = getdate()
						exec @rtn = sp_DeathNotification_ins  @SSN				= @SSN
															, @NotificationDate	= @NotificationDate
															, @DateOfDeath		= @DateOfDeath
															--, @DeathCode = @DeathCode --MXIE, deathcode info is necessary
															, @AuditUser		= @AuditUser
						

						if @rtn != 0
						begin
							return @rtn
						end
					End
					Else
					Begin
						-- Dependent
						if exists (select 1 from Dependent where DepSSN = @SSN 
								   and DependentCode in ('SP'))
						Begin
							 select @PartSSN				 = SSN
								  , @DependentCode           = DependentCode
								  , @SuffixCode			     = SuffixCode
								  , @DivorceDate             = DivorceDate             
								  , @MarriageDate            = MarriageDate            
								  , @ReMarriageDate          = ReMarriageDate          
								  , @StartDate               = StartDate               
								  , @CutoffDate              = CutoffDate              
								  , @EligEffective           = EligEffective           
								  , @EligCancellation        = EligCancellation         
								  , @AuditDate1              = AuditDate
							   from Dependent where DepSSN = @SSN 
								   and DependentCode in ('SP')

							exec @rtn = sp_Dependent_upd  
									@SSN					 = @SSN
								  , @ParticipantSSN		     = @PartSSN
								  , @DependentCode           = 'SD'
								  , @DateOfDeath			 = @DateOfDeath 
								  , @SuffixCode			     = @SuffixCode 
								  , @DivorceDate             = @DivorceDate             
								  , @MarriageDate            = @MarriageDate            
								  , @ReMarriageDate          = @ReMarriageDate          
								  , @StartDate               = @StartDate               
								  , @CutoffDate              = @CutoffDate              
								  , @EligEffective           = @EligEffective           
								  , @EligCancellation        = @EligCancellation         
								  , @AuditDate               = @AuditDate1
							if @rtn != 0
							begin
								return @rtn
							end
						End		
					End	

				End
				
				if exists (select 1 from LifeInsBeneficiary where BeneSSN = @SSN)
				Begin
						update LifeInsBeneficiary
						set Dateofdeath = @dateofdeath   ,
							AuditDate	= getdate(),
							AuditUser	= @AuditUser
 						where BeneSSN = @SSN
					if @@error <> 0
					Begin
						raiserror 99999 'Error updating Beneficary Information'
						return 99999
					End			
				End

				--RUN ROLLOVER FOR DEPENDENT AFTER DATE OF DEATH CHANGED...
				--MXIE add more condition when @OrgDateofDeath or @DateofDeath is NULL value.
				if (exists (select 1 from Dependent where DepSSN = @SSN ) 
					and 	((@OrgDateofDeath <> @DateofDeath)
							or (@OrgDateofDeath is null)
							or (@DateofDeath is null)))
				Begin
					--select 'DOD is changed.'
					declare crec cursor for Select SSN from Dependent where DepSSN = @SSN
					open crec
					fetch next from crec into @PartSSN
					while @@fetch_status = 0
					begin
						exec sp_rollover_participant @ssn = @PartSSN
						fetch next from crec into @PartSSN
					end
					close crec
					deallocate crec
				End		

			End

--			--MM Added code to update Organization tax id is it is changed by user.
--			if LTRIM(RTRIM(@OrgSSN)) <> LTRIM(RTRIM(@SSN)) and exists (select 1 from Organization WHERE Taxid = @OrgSSN)
--			begin
--				update Organization Set Taxid = @SSN WHERE Taxid = @OrgSSN
--			end

			if exists (select 1 from Organization WHERE Taxid = @SSN)
			Begin
				select @AuditDate = AuditDate from Organization WHERE Taxid = @SSN

        		EXEC @rtn = Organization_info_upd	@TaxID	    = @SSN,
        											@Name		= @FirstName,
													@AuditDate	= @AuditDate,
        											@AuditUser	= @AuditUser
				if @rtn != 0
				begin
					return @rtn
				end
			End
		End

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
					set @errormessage = 'PID '+@PID+' does not exists in Eligibility.'
					raiserror 99999 @errormessage 
					return 99999
			end
		End

	insert into dbo.OPUSPersonInfoUpdateActivity (PID, SSN, UpdateDate, UpdateUser, ActivityType, Processed)
	select @PID, @SSN, GETDATE(), USER_NAME(), 'Person Update', 0

--  All is good
drop table #tmpErrorMsg
return 0
END





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
, @AddressStartDate                  datetime      = NULL
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




-------------********IMP*******-----------------
---Was not part of the scripts which MPI gave---
-------------********IMP*******-----------------
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
	left outer join OPUS.dbo.SGT_PERSON r (nolock) on p.SSN = OPUS.dbo.fn_GET_DECRYPTED_TEXT(r.SSN)
	
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
		,r.MPI_PERSON_ID DepPID
		,s.MPI_PERSON_ID PartPID
		,t.MedicalName
		,t.DentalName
		,DualScenario = convert(varchar(20),'')
	into #Deps08312011
	from #Parts08312011 t	
	inner join DependentEligibility d on t.SSN = d.SSN
	inner join Person p on d.DepSSN = p.SSN
	left outer join OPUS.dbo.SGT_PERSON r (nolock) on p.SSN = OPUS.dbo.fn_GET_DECRYPTED_TEXT(r.SSN)
	left outer join OPUS.dbo.SGT_PERSON s (nolock) on d.SSN = OPUS.dbo.fn_GET_DECRYPTED_TEXT(s.SSN)
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




-------------********IMP*******-----------------
---Was not part of the scripts which MPI gave---
-------------********IMP*******-----------------
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
		inner join OPUS.dbo.SGT_PERSON p (nolock) on t.SSN = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.SSN)
			
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



USE [DOCS]
GO
/****** Object:  StoredProcedure [dbo].[sp_DocumentAddRequirement]    Script Date: 01/13/2012 16:47:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Vidya Nair
-- Create date: 05/04/2006
-- Description:	This SP will be used to add the Document Requirements for
--				a Dependent based on DependentType and othe factors.
--				This procedure will be called every time new dependent is added
--				or dependent type is changed.	

-- 08/23/2006	VN	No check for State, while adding DP Docs , if not Cal  then user can override.			
-- 12/28/2006	VN  Remove COB Req is Participant is HMO
-- 04/20/2007	VN	Use HardCode DocId instead of DocName
-- 05/02/2007	VN	Code, so that if Dual COB Reqmnt is Overriden	
-- 08/01/2007	VN	Code, so that id Part is not HMO as of @CurrentEffectiveDate , but in future.
--					then cob reqmnt is added, but overriden.		
-- 09/05/2007	VN	Code changed for checking child req	HN requirement is needed for very extn
-- 09/05/2007	VN	FAC Dpc Requirement is for oxford too.
-- 11/14/2008   RG  Add Required Docs for participant(EE).
-- 09/03/2009   RG  Update to use HCODetail instead of hardcodding plans 
-- 10/15/2010   RG  Override BENE card Req Doc if Dependent becomes participant (Individual COBRA)
-- 11/17/2010   RG  Update for Health Refor for dep  - HCR dep
-- 07/29/2011   RG  Update to use document effdate for HCR dependent from OPenEnrollment table
--01/13/2012    RG change for OPUS
--				Raj	Opus change. Changed from SGT_PERSON.dbo.person to OPUS.dbo.SGT_person.
-- =============================================
ALTER PROCEDURE [dbo].[sp_DocumentAddRequirement] (@SSN		char(9),
											@DepSSN		char(10),
											@OldDepCode	char(2),
											@NewDepCode	char(2),
											@DPEffDate smalldatetime = NULL)

AS
BEGIN
	SET NOCOUNT ON
	DECLARE @CurEffectiveDate	SMALLDATETIME,
			@FirstEffDate		SMALLDATETIME,
			@BirthCertId		int,
			@HMOFamilyAcctId	int,
			@DPNonTaxDocId		int,
			@DPCalDocId			int,
			@chkDate			smalldatetime,
			@DPTaxDocId			int,
			@COBDocId			int,
			@MinElig			smalldatetime,
			@MinNonCobReq		smalldatetime,
            @BeneCardDocId      int,
            @HCRDate            smalldatetime
			
	select @BirthCertId =     3		 -- 'Birth Certificate'
	select @HMOFamilyAcctId	= 11	 -- 'Family Account Change Form'
	select @DPNonTaxDocId	= 34	 -- 'Affidavit of "dependency"'
	select @DPTaxDocId	=     86	 -- 'Termination of Dependency for Tax Purposes'
	select @COBDocId =	      17	 -- 'Coordination of Benefits Forms'
    select @BeneCardDocId =   9      -- 'Bene card for dependent become participant	
    
    select @HCRDate = max(OpenEnrollDate) from HEDB.DBO.OpenEnrollmentConfiguration where OpenEnrollDate <= getdate()
	

    /* RG 11/14/2008 Add Required Docs for participant*/
    if @NewDepCode = 'EE' and @OldDepCode is null
		begin
			select @FirstEffDate = convert(char(10),getDate(),101)
		end
     else
        begin
			select @FirstEffDate = ISNULL(case when D.DependentCode in ('SP','SS','DS','SD') then
									D.MarriageDate
								when D.DependentCode in ('DP','DT') then
									(select min(StartDate) from hedb..DependentEligibilityDates 
									 where PartSSN = @SSN and SSN = @DepSSN and @DPEffDate between StartDate and isnull(CutoffDate,'12/31/2078'))
								when D.DependentCode in ('SA') and D.MarriageDate > P.DateofBirth then D.MarriageDate
								when D.DependentCode in ('SR') and D.DivorceDate is not null then D.DivorceDate									 									 
								when D.DependentCode in ('C2') or  @NewDepCode = 'C2' then @HCRDate --HCR OPen Enrollment - startdate
                                else P.DateofBirth
							end,'')
			from hedb..Dependent D
			inner join hedb..Person P on D.DepSSN = P.SSN
			where d.SSN = @SSN and d.DepSSN = @DepSSN	
			
		
        end
 
    /**************************************************/


	if @OldDepCode is null and @NewDepCode is not null
	Begin
	-- New Dependent Added
		select @CurEffectiveDate = @FirstEffDate	
	End
	
	if @OldDepCode is not null and @NewDepCode is not null and @OldDepCode <> @NewDepCode
	Begin
	-- Dependent DepCode Changed, eg SP - DS , CH -HN etc

		If exists (select 1 from hedb..DependentCodeHistory D where D.SSN = @SSN and D.DepSSN = @DepSSN 
					and ChangeReason <> 'Input Error') and 	@NewDepCode <> 'C2'				
		Begin
			select @CurEffectiveDate = dateadd(day,1,max(CancellationDate))
			from hedb..DependentCodeHistory  D
			where D.SSN = @SSN and D.DepSSN = @DepSSN	
			and ChangeReason <> 'Input Error'
			--and CancellationDate <= getdate()
		End	
		Else
		Begin
			select @CurEffectiveDate = @FirstEffDate
		end
	END
			
	select SSN = @SSN,DepSSN = @DepSSN,R.DocumentTypeId,
		   DepCode =@NewDepCode,EffDate = @CurEffectiveDate,
		   CancelDate = CONVERT(smalldatetime,NULL),
		   Submitted = 0,
		   Override	 =0,
		   OverrideReason = convert(varchar(50),'')
	INTO #tmpDocReq
	from RequiredDocs R
	where DependentCode = @NewDepCode
	and not exists (select 1 from DependentRequiredDocs where SSN = @SSN and DepSSN = @DepSSN
					and DocId = R.DocumentTypeId and ReqEffectiveDate = @CurEffectiveDate)

											
-- For DP's remove  requirement based on Taxable or NonTaxable
	if 	@NewDepCode ='DP'
	Begin
		-- dlete docs already added											
		if exists (select 1 from hedb.. DependentEligibilityDates DE  where PartSSN = @SSN and SSN = @DepSSN 
					and CutoffDate < @DPEffDate
					and not exists (select 1 from hedb..DependentCodeHistory D where D.SSN = @SSN and D.DepSSN = @DepSSN 
									and ChangeReason <> 'Input Error' and DependentCode ='DT'
									and CancellationDate > DE.CutoffDate))										
		begin
			SELECT @chkDate = min(StartDate)
			from hedb.. DependentEligibilityDates DE  where PartSSN = @SSN and SSN = @DepSSN 
			and CutoffDate < @DPEffDate
			and not exists (select 1 from hedb..DependentCodeHistory D where D.SSN = @SSN and D.DepSSN = @DepSSN 
							and ChangeReason <> 'Input Error' and DependentCode ='DT'
							and CancellationDate > DE.CutoffDate)		
			
			delete #tmpDocReq from #tmpDocReq R
			where exists (select 1 from DependentRequiredDocs where SSN = @SSN and DepSSN = @DepSSN
						and DocId = R.DocumentTypeId and ReqEffectiveDate >= @chkDate)
			and DocumentTypeId <> @DPTaxDocId
			and DocumentTypeId <> @DPNonTaxDocId			  						

		end					
						
		-- For Taxable No affidavit
		if exists (select 1 from hedb..DependentEligibilityDates 
					where PartSSN = @SSN and SSN = @DepSSN and @DPEffDate between StartDate and isnull(CutoffDate,'12/31/2078')
					and Type ='TD')
		Begin
			delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @DPNonTaxDocId	
			
		 -- Termination of Dependency doc not required when setting up DP for first time			
			if not exists (select 1 from hedb..DependentEligibilityDates 
					where PartSSN = @SSN and SSN = @DepSSN and dateadd(day,-1,@DPEffDate) between StartDate and isnull(CutoffDate,'12/31/2078'))	
			begin
					delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @DPTaxDocId	
			end					
		End			
		
		-- For Non Taxable No Termination of Dependency doc
		if exists (select 1 from hedb..DependentEligibilityDates 
					where PartSSN = @SSN and SSN = @DepSSN and @DPEffDate between StartDate and isnull(CutoffDate,'12/31/2078')
					and Type ='ND')
		Begin
			delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @DPTaxDocId			
		End							
	End

	-- if dual overide cob requirement
	if @NewDepCode in ('SP','DP')
	   and exists (select 1 from HEDB..Eligibility where SSN = @DepSSN and 
					@CurEffectiveDate between EligEffective and EligCancellation
					and StatusCode not in ('PT','NI','CI'))
	Begin
		update #tmpDocReq
		set Override = 1
		where DocumentTypeId = @COBDocId
	End
		
-- Remove Family Req Doc is not HMO
		if exists (select 1 from #tmpDocReq A where DocumentTypeId = @HMOFamilyAcctId)
			and not exists (select 1 from HEDB..HCOEnrollment where SSN = @SSN 
			--and (hco in ('K','H') or hco in (select hco from hedb..hco where name ='Oxford'))
            and hco in (select hco from  HEDB..hcodetail  
                                     where hcotype = 'M' and HCOsubtype = 'HMO' and isactive = 1)
			and @CurEffectiveDate between StartDate and EndDate)
		Begin
			delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @HMOFamilyAcctId
		End						

-- Remove COB req Doc is HMO
		if exists (select 1 from #tmpDocReq A where DocumentTypeId = @COBDocId)
		Begin
			if exists (select 1 from HEDB..HCOEnrollment where SSN = @SSN 
						--and hco not in ('BC','BN','B','B2','NM')
                        and hco in (select hco from HEDB..hcodetail  
                                                 where hcotype = 'M' and HCOsubtype = 'HMO' and isactive = 1)
						--and hco in (select hco from hedb..hco where hcotype ='M')
						and @CurEffectiveDate between StartDate and EndDate)
			Begin
				delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @COBDocId
			End	
			else
			Begin
				if not exists (select 1 from HEDB..HCOEnrollment where SSN = @SSN
								and @CurEffectiveDate between StartDate and EndDate
								and hco not in ('NM')
								and hco in (select hco from hedb..hcoDetail where hcotype ='M'))
				Begin
					select @MinElig = min(EligEffective) from HEDB..Eligibility
					where SSN = @SSN and StatusCode not in ('PT','NI','CI')

					if exists (select 1 from HEDB..HCOEnrollment where SSN = @SSN 
								--and hco not in ('BC','BN','B','B2','NM')
								--and hco in (select hco from hedb..hco where hcotype ='M')
                                and hco in (select hco from HEDB..hcodetail  
                                                         where hcotype = 'M' and HCOsubtype = 'HMO' and isactive = 1)
								and @MinElig between StartDate and EndDate)
					Begin
						delete #tmpDocReq from #tmpDocReq where DocumentTypeId = @COBDocId
					End									
				End
				Else	
				Begin
					select @MinNonCobReq = min(StartDate) from hedb..HCOEnrollment 
					where SSN = @SSN 
                    and hco in (select hco from HEDB..hcodetail  
                                             where hcotype = 'M' and HCOsubtype = 'HMO' and isactive = 1)
--					and hco not in ('BC','BN','B','B2','NM')
--					and hco in (select hco from hedb..hco where hcotype ='M')
					and StartDate >= @CurEffectiveDate

					if @MinNonCobReq is not null
					Begin
						update #tmpDocReq
						set Override = 1,
							CancelDate = dateadd(day,-1,@MinNonCobReq),
							OverrideReason ='PLAN CHANGE'
						where DocumentTypeId = @COBDocId
					End
				End
			End
		End

-- Remove Birth Cert Req if already present.
		if exists (select 1 from DependentRequiredDocs where SSN = @SSN and DepSSN = @DepSSN
					and DocId = @BirthCertId)
           or
    --HCR dep: if dependent code is 'C2', check when dependent was created. If dependent was craeted before 12/01/2010 remove Birth cert 
           (@NewDepCode = 'C2'
            and exists (select 1 from OPUS.dbo.SGT_person where ssn = @Depssn and CREATED_DATE < '12/01/2010')) -- OPUS            
		Begin
			delete #tmpDocReq from  #tmpDocReq A where DocumentTypeId = @BirthCertId
		End	
				
-- Removew Requirements if new and old DepCode are both of type children eg CH,HN,
	IF exists (select 1 from DependentRequiredDocs D 
					INNER JOIN #tmpDocReq T ON T.SSN = D.SSN AND T.DEPSSN = D.DEPSSN
					where T.SSN = @SSN and T.DepSSN = @DepSSN
					and ((D.DependentCode IN ('CH','ST','SA','AD','LG','HN')
						  and @NewDepCode IN ('CH','ST','SA','AD','LG'))
						or
						(D.DependentCode IN ('CH','ST','SA','AD','LG')
						  and @NewDepCode IN ('CH','ST','SA','AD','LG','HN'))
                         or 
                        (D.DependentCode IN ('CH','LG','AD','FC','SA','ST','HN') --HCR dep
                         and  @NewDepCode IN ('C2'))
                        or
                        (D.DependentCode IN ('C2')
                         and  @NewDepCode IN ('CH','LG','AD','FC','SA','ST','HN'))
                        )						
					and T.DocumentTypeId = D.DocId)
	Begin
		delete #tmpDocReq
		from DependentRequiredDocs D 
		INNER JOIN #tmpDocReq T ON T.SSN = D.SSN AND T.DEPSSN = D.DEPSSN
		where T.SSN = @SSN and T.DepSSN = @DepSSN
					and ((D.DependentCode IN ('CH','ST','SA','AD','LG','HN')
						  and @NewDepCode IN ('CH','ST','SA','AD','LG'))
						or
						(D.DependentCode IN ('CH','ST','SA','AD','LG')
						  and @NewDepCode IN ('CH','ST','SA','AD','LG','HN'))
                        or 
                        (D.DependentCode IN ('CH','LG','AD','FC','SA','ST','HN') --HCR dep
                         and  @NewDepCode IN ('C2'))
                        or
                        (D.DependentCode IN ('C2')
                         and  @NewDepCode IN ('CH','LG','AD','FC','SA','ST','HN')) --HCR dep
                         )						
		and T.DocumentTypeId = D.DocId
	End	

 
   --Override BENE card Req Doc if Dependent becomes participant
    If @NewDepCode = 'EE'
		and exists (select 1 from hedb..Dependent d 
                                    inner join hedb..eligibility e on d.SSN = e.SSN 
                                      and Getdate() between e.eligeffective 
                                      and e.eligcancellation and e.statuscode = 'NI'
                             where d.DepSSN = @DepSSN)
    Begin
       update #tmpDocReq
	    set  Override = 1,
			 OverrideReason ='COBRA'
		where DocumentTypeId = @BeneCardDocId
    End							
					
	
	BEGIN TRAN			
		insert into DependentRequiredDocs(SSN,DepSSN,DocId,DependentCode,ReqEffectiveDate,ReqCancellationDate,Submitted,Override,OverrideReason,CreateDate,CreateUser,ModifyDate,ModifyUser)
		select SSN,DepSSN,DocumentTypeId,DepCode,EffDate,CancelDate,Submitted,Override,OverrideReason,getdate(),suser_sname(),getdate(),suser_sname()	
		from #tmpDocReq	
		if @@error <> 0
		Begin
			rollback tran 
			return @@error
		End			
	
    COMMIT TRAN	
					
return 0
END