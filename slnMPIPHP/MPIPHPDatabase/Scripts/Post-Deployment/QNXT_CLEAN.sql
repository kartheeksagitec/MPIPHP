-- =============================================
-- QNXT Scripts
-- =============================================


Use MPICustom
go

Alter procedure spm_Opus_MemberClaimCnt
(
@NewPID			varchar(9)
,@OldPID		varchar(9)
--,@ResultOut		smallint output		-- 0: for success; -1: if either of the PIDs have claims associated.
,@NewPIDClmCnt	int output
,@OldPIDClmCnt	int output
,@Debug			bit = 0
)
as
/*-----Change History----------------------------------------------------------------------------------------------------
04/19/2013	Raj		Procedure created to give the finalized claim count for each PID passed.

--Testing:
declare @ResultOut smallint
exec spm_Opus_MemberClaimCnt @NewPID = 'M30268097', @OldPID = 'M30304606'
			, @ResultOut = @ResultOut output, @Debug = 0
select @ResultOut

declare @NewPIDClmCnt int, @OldPIDClmCnt	int
exec spm_Opus_MemberClaimCnt 
	@NewPID = 'M30268097'
	,@OldPID = 'M30304606'
	,@NewPIDClmCnt = @NewPIDClmCnt output
	,@OldPIDClmCnt = @OldPIDClmCnt output
	,@Debug = 0
select @NewPIDClmCnt, @OldPIDClmCnt

-----------------------------------------------------------------------------------------------------------------------*/
begin


	------------------------------------------------------------
	--declare 
	--		@NewPID			varchar(9)
	--		,@OldPID		varchar(9)
	--		,@NewPIDClmCnt	int	
	--		,@OldPIDClmCnt	int
	--		,@Debug bit 
			
	--select @NewPID = 'M30268097', @OldPID = 'M30304606', @Debug = 1
	----------------------------------------------------------
	
	declare @NewSSN varchar(10),
			@OldSSN	varchar(10)
			
	select @NewSSN = m.SSN
		from PlanData.dbo.Member m (nolock) 
		where m.SecondaryId = @NewPID
	
	select @OldSSN = m.SSN
		from PlanData.dbo.Member m (nolock) 
		where m.SecondaryId = @OldPID
		
	Select @NewPIDClmCnt = 
			(
				select COUNT(*) 
					from PlanData.dbo.Claim c (Nolock)
					inner join PlanData.dbo.Member m (nolock) on c.memid = m.MemId
					where c.memid = m.MemId
					and c.status in ('Paid', 'Reversed', 'Denied')
					and m.secondaryid = @NewPID
			)		
		+
			(
				select COUNT(*)
					from MPIHEALTH03.VIP.dbo.Claim C
					inner join MPIHEALTH03.VIP.dbo.Member m on c.MemId = m.MemId
					where m.SSN = @NewSSN
					and c.status in ('Paid', 'Reversed', 'Denied')
			)
		+
			(
				select COUNT(*)
					from MPIHEALTH03.PlanData.dbo.Claim C
					inner join MPIHEALTH03.PlanData.dbo.Member m on c.MemId = m.MemId
					where m.SSN = @NewSSN
					--and m.guardian <> 'VIP'
					and c.status in ('Paid', 'Reversed', 'Denied')
			)
		
		Select @OldPIDClmCnt = 
			(
				select COUNT(*) 
					from PlanData.dbo.Claim c (Nolock)
					inner join PlanData.dbo.Member m (nolock) on c.memid = m.MemId
					where c.memid = m.MemId
					and c.status in ('Paid', 'Reversed', 'Denied')
					and m.secondaryid = @OldPID
			)		
		+
			(
				select COUNT(*)
					from MPIHEALTH03.VIP.dbo.Claim C
					inner join MPIHEALTH03.VIP.dbo.Member m on c.MemId = m.MemId
					where m.SSN = @OldSSN
					and c.status in ('Paid', 'Reversed', 'Denied')
			)
		+
			(
				select COUNT(*)
					from MPIHEALTH03.PlanData.dbo.Claim C
					inner join MPIHEALTH03.PlanData.dbo.Member m on c.MemId = m.MemId
					where m.SSN = @OldSSN
					--and m.guardian <> 'VIP'
					and c.status in ('Paid', 'Reversed', 'Denied')
			)			
		
			
	if @Debug = 1
		select @NewPIDClmCnt, @OldPIDClmCnt, @NewSSN, @OldSSN
	
	/*
	if @NewPIDClmCnt > 0 or @OldPIDClmCnt > 0
		begin
			select @ResultOut = -1
			declare @StrMsg varchar(200)
			select @StrMsg = 'MPID:-' + @NewPID + ' has ' + CONVERT(varchar, @NewPIDClmCnt) + ' and MPID:-'
								+ @OldPID + ' has ' + CONVERT(varchar, @OldPIDClmCnt) + ' claims associated with'
					
			raiserror (N'%s.', -- Message text.
						10, -- Severity,
						1, -- State,
						@strMsg -- First argument.
						 );
			return		
		end
	else
		begin
			select @ResultOut = 0			
		end
	*/
	
end
go

/*------------------------------------------------------------
--sp_linkedservers

select * from PlanData.dbo.Member where HeadOfHouse = 'M30268097'

select * from PlanData.dbo.Member where Guardian = 'VIP'

select top 10 *
			from MPIHEALTH03.VIP.dbo.Claim C
--------------------------------------------------------*/