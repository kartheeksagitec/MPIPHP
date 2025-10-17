CREATE PROC [dbo].[TrueUnionByySSN]
(		
	@SSN char(9)
)
AS
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

