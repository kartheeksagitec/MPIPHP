
-- =============================================
-- Author:		Peter Haines
-- Create date: May 3, 2011
-- Description:	Returns True Union, if found in the True Union or 
--				finds the Union with the most hours in the EA Hours Table
--				and returns that Union as True Union  
-- =============================================
create procedure [dbo].[fn_TrueUnionBySSN_test] 
(		
	@SSN char(9)
)
--RETURNS int
AS
BEGIN	
	DECLARE @TrueUnion as int
	
	set @TrueUnion = (Select top 1 TrueUnion from TrueUnion where TrueUnion.SSN = @SSN order by AuditDate desc)
	
	if @TrueUnion is null
		begin
			set @TrueUnion = (Select UnionCode
				from (Select top 1 Sum(HoursWorked) TotalHoursWorked, UnionCode				
				from Hours
				where Hours.SSN = @SSN
					and Hours.Status = 0
				group by UnionCode	
				order by TotalHoursWorked desc) as TU)
				
			if @TrueUnion is null
				begin
					set @TrueUnion = 0
				end
		end

	RETURN @TrueUnion

END
