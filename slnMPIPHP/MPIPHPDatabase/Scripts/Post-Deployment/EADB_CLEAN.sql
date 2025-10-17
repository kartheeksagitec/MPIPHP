
USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]    Script Date: 08/12/2016 11:01:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/**********************************************************************************************
Proc Name		: usp_GETYEARENDEXTRACTIONDATAYEARLY
Date create		: 06/30/2013
Create by		: Sagitec Solutions & MPIPHP


Description:
   This SP pulls Hours information for participants from EADB

History:
	Date			Name				Modification
	------------    --------			-------------
	09/04/2015		Suresh Kolape		PIR 753
	04/20/2016		Rohan Adgaonkar		PIR 1052
	08/01/2016		Sagitec Team		F/w Upgrade
	
***********************************************************************************************/


ALTER PROC [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY] @TEMPTABLE VARCHAR(1) = 'N' --PIR 1052
AS    
BEGIN    
SET NOCOUNT ON    
    
DECLARE @temp TABLE(    
 [SSN] [varchar](9) NULL,    
 [PensionYear] [int] NULL     
 )     
  
INSERT INTO @temp      
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO @TEMPTABLE  --PIR 1052
    
declare @year int    
set select top(1) @year = PensionYear from @temp    
    
delete from OPUS_AnnualStmt_Participant_List    
    
INSERT INTO OPUS_AnnualStmt_Participant_List    
select * from  @temp     
   
declare @PlanStartDate datetime    
declare @PlanEndDate datetime    
declare @CutOffDate datetime    
select @PlanStartDate = cutoffdate from eadb.dbo.period where qualifyingenddate = eadb.dbo.fn_PlanYearEndDate(@year-1)  
set @PlanStartDate = DATEADD(DAY,1,@PlanStartDate)  
select @PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)    
select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate    
      
CREATE TABLE [#PensionWorkHistory](    
 --[ReportID] [varchar](18) NULL,    
 [EmpAccountNo] [int] NULL,    
 [ComputationYear] [int] NULL,    
 [FromDate] [smalldatetime] NULL,    
 [ToDate] [smalldatetime] NULL,    
 [Weeks] [char](2) NULL,    
 --[Received] [smalldatetime] NULL,    
 [Processed] [smalldatetime] NULL,    
 --[HoursID] [varchar](24) NULL,    
 [SSN] [varchar](9) NULL,    
 --[LastName] [varchar](50) NULL,    
 --[FirstName] [varchar](50) NULL,    
 [UnionCode] [int] NULL,    
 [PensionPlan] [smallint] NULL,    
 --[PensionCredit] [numeric](7, 3) NULL,    
 --[L52VestedCredit] [smallint] NULL,    
 [PensionHours] [numeric](7, 1) NULL,    
 [IAPHours] [numeric](7, 1) NULL,    
 --[IAPHoursA2] [numeric](7, 1) NULL,    
 --[IAPPercent] [money] NULL,    
 --[ActiveHealthHours] [numeric](7, 1) NULL,    
 --[RetireeHealthHours] [numeric](7, 1) NULL,    
 --[PersonId] [varchar](15) NULL,    
 --[RateGroup] [varchar](4) NULL,    
 --[HoursStatus] [int] NULL,    
 [LateMonthly] [varchar](1) NOT NULL,    
 [LateAnnual] [varchar](1) NOT NULL,  
[EmployerName] [varchar] (255) NULL  
--[UnionMisc] [char](2) NULL,    
 --[HoursWorked] [numeric](7, 1) NULL,    
 --[IAPHourlyRate] [money] NULL,    
 --[Source] [varchar](4) NOT NULL,    
 --[ToHealthSystem] [int] NULL,    
 --[ToPensionSystem] [int] NULL,    
 --[IsActiveHealth] [int] NULL,    
 --[IsRetireeHealth] [int] NULL,    
 --[IsPension] [int] NULL,    
 --[IsIAPHourly] [int] NULL    
 --, [OldEmployerNum] [varchar](6)    
)     
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)    
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'    
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),    
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)    
 SSN = convert(char(9),Hours.SSN),    
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)    
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)    
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)    
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked     
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)    
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)    
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)    
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.    
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)    
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)    
 --NULL PersonId, --varchar(15) no change    
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)    
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.    
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,    
 LateAnnual = case when Report.ProcessDate > coalesce(PlanCutoff.cutoffdate, Report.ProcessDate) then 'Y' else '' end ,  
EmployerName = E.EmployerName   
 --------------------------------------------------------------------------------------------------------------    
 --UnionMisc = Hours.UnionMisc, --New field. char(2)    
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.     
        --It is required because for those records where we do not have any rate group info    
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.    
 --IAPHourlyRate = rgb.Individual  --New field. money    
 --, Source = 'C/S '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN    
 inner join eadb.dbo.Report report on report.reportid = hours.reportid     
 --and hours.SSN = @SSN     
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate    
 --and report.ToDate between @FromDate and @ToDate     
 left outer join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP    
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate     
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate    
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate     
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
where EmpAccountNo not in (14002,13363,3596,3597,12904) and    
--Report.RecDate <= isnull(@CutOffDate,Report.RecDate)    
Report.ProcessDate <= isnull(@CutOffDate,Report.RecDate)    
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.    
--Employer id for Locals Pre-Merger hours.    
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)    
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = HPTransactions.Ber,    
 ----EmpAccountNo = convert(int, HPTransactions.Employer),    
 EmpAccountNo = E.EmployerId,    
 PensionYear = PY.PensionYear,    
 FromDate = convert(smalldatetime, HPTransactions.StartDate),    
 ToDate = convert(smalldatetime, HPTransactions.EndDate),    
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),     
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),    
 --Received = convert(smalldatetime, HPTransactions.DateReceived),    
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.    
 --Processed = convert(smalldatetime,hb.Updated),    
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),    
 SSN = convert(char(9),HPTransactions.SSN),    
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),    
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),    
 UnionCode = convert(int,HPTransactions.UnionCode),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),    
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later     
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,HPTransactions.IAPDollars),    
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),    
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),     
 --HoursStatus = 0,    
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,    
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,  
EmployerName = E.EmployerName    
 ----------------------------------------------------------------------------------------------------------------    
 --UnionMisc = HPTransactions.UNMisc,    
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'H/P '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN    
 left outer join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP     
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch     
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')  
      (not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')  
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')  
    )  
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and erkey = e.employerid and hrsact = convert(numeric(7, 1), HPTransactions.Hours))  
--and HPTransactions.SSN = @SSN    
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate    
 --and HPTransactions.EndDate between @FromDate and @ToDate    
    
--CPAS View    
insert into [#PensionWorkHistory]    
select     
 --ReportID = left(cpas.erractid,18),    
 ----EmpAccountNo = convert(int, cpas.ERKey),    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,    
 FromDate = convert(smalldatetime, cpas.FDate),    
 ToDate = convert(smalldatetime, cpas.TDate),    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),    
 --Received = convert(smalldatetime, cpas.PDate),    
 Processed = convert(smalldatetime, cpas.PDate),    
 --HoursId = convert(varchar(24),cpas.erractid),    
 SSN = convert(char(9),cpas.MKey),    
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,cpas.LOC_NO),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 --PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours    
 PensionHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 --IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later    
 IAPHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth    
 ----we are not checking rate item to identify hours for Pension, Health, or IAP    
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,cpas.PanOnEarn),    
 ----MM 12/26/12    
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),    
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),    
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),cpas.RateGroup),    
 --HoursStatus = 0, --all the hours comming from CPAS are processed.    
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,    
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
EmployerName = E.EmployerName   
 -----------------------------------------------------------------------    
 --UnionMisc = null,    
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'CPAS'    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, E.OldEmployerNum    
 from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey    
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 where [Plan]=2    
 --and cpas.mkey = @SSN    
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate    
 --and cpas.TDate between @FromDate and @ToDate    
  
-- RAP IAP$  
insert into [#PensionWorkHistory]  
select        
      --ReportID = left(rap.erractid,18),  
      --EmpAccountNo = convert(int, cpas.ERKey),  
      EmpAccountNo = isnull(E.EmployerId,'0'),  
      ComputationYear = rap.Plan_Year, -- PY.PensionYear,  
      FromDate = convert(smalldatetime, rap.FDate),  
      ToDate = convert(smalldatetime, rap.TDate),  
      Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),  
      --Received = convert(smalldatetime, rap.PDate),  
      Processed = convert(smalldatetime, rap.PDate),  
      --HoursId = convert(varchar(24),rap.erractid),  
      SSN = convert(char(9),rap.MKey),  
      --LastName = NULL, --convert(char(50),p.LastName),  
      --FirstName = NULL, --convert(char(50),p.FirstName),  
      UnionCode = convert(int,rap.LOC_NO),  
      PensionPlan = convert(smallint, 2), -- MPI   
      --PensionCredit = convert(numeric(7, 3),0),  
      --L52VestedCredit = convert(smallint,0),  
      PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
      IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
      --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
      --we are not checking rate item to identify hours for Pension, Health, or IAP  
      --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
      --IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP  
      --IAPPercent = convert(money,rap.PanOnEarn),  
      --MM 12/26/12  
      --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
      --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
      --ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --NULL AS PersonId,  
      --RateGroup = convert(varchar(4),rap.RateGroup),  
      --HoursStatus = 0, --all the hours comming from CPAS are processed.  
      LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,  
      LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,  
      EmployerName = E.EmployerName  
      ---------------------------------------------------------------------  
      --UnionMisc = null,  
      --HoursWorked = convert(numeric(7, 1), rap.HRSACT),  
      --IAPHourlyRate = rgb.Individual  
      --, Source = 'RAP'  
      --, rgc.ToHealthSystem  
      --, rgc.ToPensionSystem  
      --, IsActiveHealth = rgc.ActiveHealth  
      --, IsRetireeHealth = rgc.RetireeHealth  
      --, IsPension = rgc.Pension  
      --, IsIAPHourly = rgc.IAP  
      --, E.OldEmployerNum  
      from OPUS_AnnualStmt_Participant_List list    
      inner join EADB.dbo.RAP_IAP$ rap on rap.mkey=list.SSN  
      left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
      left outer join eadb.dbo.vwRateGroupClassification_all RGC   
         on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate   
      left outer join eadb.dbo.vwRateGroupBreakDown_all rgb   
            on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate  
      inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
      left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate  
      left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
      --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
      where [Plan]=2  
      --and rap.mkey = @SSN  
  
    
--PreMerger View.    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,    
 EmpAccountNo = convert(int, Pre.EmployerId),     
 ComputationYear = Pre.Plan_Year,    
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date    
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53    
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date     
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date    
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id    
 SSN = convert(char(9),Pre.SSN),     
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,Pre.UnionCode),     
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)    
      when [Local]='L666' then convert(smallint, 4)    
      when [Local]='L700' then convert(smallint, 6)    
      when [Local]='L52' then convert(smallint, 7)    
      when [Local]='L161' then convert(smallint, 8)    
      else null end,     
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),    
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),    
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),    
 IAPHours = convert(numeric(7, 1), 0),    
 --IAPHoursA2 = convert(numeric(7, 1), 0),     
 --IAPPercent = convert(money, 0),     
 --ActiveHealthHours = convert(numeric(7, 1), 0),     
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?    
 --NULL PersonId,    
 --RateGroup = Pre.RateGroup,--?    
 --HoursStatus = convert(int, 0),    
 LateMonthly = '',     
 LateAnnual = '' ,  
EmployerName = E.EmployerName   
 -------------------------------------------------------------------    
 --UnionMisc = convert(char(2),''),    
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'PM  '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = Pre.EmployerId    
from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId  
--left outer join pid.dbo.Person p on Pre.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
--where --Pre.SSN = @SSN    
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate    
 --Pre.EndDate between @FromDate and @ToDate    
    
--select isnull(EmpAccountNo, 0) as EmpAccountNo, ComputationYear, SSN, UnionCode, PensionHours, case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,   
--isnull(EmployerName, '') as EmployerName from [#PensionWorkHistory] where   
--PensionPlan = 2   
                          
 /*
 select ssn,computationyear,
sum(isnull(pensionhours,0.0)) TotalPensionHours,  
sum(isnull(latehours,0.0)) TotalLateHours,
UnionCode,EmpAccountno,EmployerName,
PlanStartDate,firstHourReported
 from         
(        
select ssn,computationyear,pensionhours,
case when (LateAnnual = 'Y' or v.ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,  
unioncode,empaccountno,e.employername,processed,v.PensionPlan,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN AND cast(ComputationYear as int) = cast(v.ComputationYear as int)) as firstHourReported   
from [#PensionWorkHistory] v        
left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid    
where v.ComputationYear <= @year and PensionPlan = 2     
)a        
group by ssn,computationyear,unioncode,empaccountno,employername,PensionPlan,PlanStartDate,firstHourReported
 */

 
 
--Below script we use for 2014 statements but for remaining statements i have temporarily commented the code 
--SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
--FirstHoursReported = MIN(FromDate)
--into #Summary
--FROM (
--SELECT SSN, ComputationYear, PensionHours, 
--LateHours = case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate THEN PensionHours ELSE 0 END,
--FromDate FROM #PensionWorkHistory
--WHERE PensionPlan = 2 AND ComputationYear <= @year
--) A GROUP BY SSN, ComputationYear



SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
FirstHoursReported = MIN(FromDate)
into #Summary
FROM (
SELECT SSN, ComputationYear, CASE WHEN PensionPlan = 2 THEN PensionHours ELSE 0 END AS PensionHours, 
LateHours = CASE WHEN PensionPlan = 2 
			THEN case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate 
												THEN PensionHours ELSE 0 END
			ELSE 0 END,
FromDate FROM #PensionWorkHistory
WHERE ComputationYear <= @year
) A GROUP BY SSN, ComputationYear


TRUNCATE TABLE OPUS..SGT_HEALTH_ELIGIBLITY_WORKHISTORY_DATA --F/W Upgrade

INSERT INTO OPUS..SGT_DATA_EXTRACTION_WORKHISTORY_DATA --F/W Upgrade
SELECT S.SSN, S.ComputationYear, TotalPensionHours, TotalLateHours,
PlanStartDate = Y.FirstHoursReported, S.FirstHoursReported as firstHourReported
FROM #Summary S 
INNER JOIN 
(SELECT SSN, FirstHoursReported = MIN(FirstHoursReported) FROM #Summary GROUP BY SSN) Y ON S.SSN = Y.SSN 
    
drop table [#PensionWorkHistory]  
  
end  
go

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Suresh Kolape
-- Date - 11/08/2016
-- Purpose - sp usp_GetIAPSnapShotInfo changed
----------------------------------------------------------------------------------------------------------------------------------
USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetIAPSnapShotInfo]    Script Date: 08/10/2016 22:34:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GetIAPSnapShotInfo](@COMPUTATIONYEAR int, @OPTIONALPARAMETER int = 0) 
AS
BEGIN
SET NOCOUNT ON
--------------------------------------------------------------------------------------------

CREATE TABLE [#PensionWorkHistory](	  
      [ReportID] [varchar](18) NULL,
      [EmpAccountNo] [int] NULL,
      [ComputationYear] [smallint] NULL,
      [FromDate] [smalldatetime] NULL,
      [ToDate] [smalldatetime] NULL,
      [Weeks] [char](2) NULL,
      [Received] [smalldatetime] NULL,
      [Processed] [smalldatetime] NULL,
      [HoursID] [varchar](24) NULL,
      [SSN] [char](9) NULL,
      [LastName] [varchar](50) NULL,
      [FirstName] [varchar](50) NULL,
      [UnionCode] [int] NULL,
      [PensionPlan] [smallint] NULL,
      [PensionCredit] [numeric](7, 3) NULL,
      [L52VestedCredit] [smallint] NULL,
      [PensionHours] [numeric](7, 1) NULL,
      [IAPHours] [numeric](7, 1) NULL,
      [IAPHoursA2] [numeric](7, 1) NULL,
      [IAPPercent] [money] NULL,
      [ActiveHealthHours] [numeric](7, 1) NULL,
      [RetireeHealthHours] [numeric](7, 1) NULL,
      [PersonId] [varchar](15) NULL,
      [RateGroup] [varchar](4) NULL,
      [HoursStatus] [int] NULL,
      [LateMonthly] [varchar](1) NULL,
      [LateAnnual] [varchar](1) NULL,
      [UnionMisc] [char](2) NULL,
      [HoursWorked] [numeric](7, 1) NULL,
      [IAPHourlyRate] [money] NULL,
      [Source] [varchar](4) NULL,
      [ToHealthSystem] [int] NULL,
      [ToPensionSystem] [int] NULL,
      [IsActiveHealth] [int] NULL,
      [IsRetireeHealth] [int] NULL,
      [IsPension] [int] NULL,
      [IsIAPHourly] [int] NULL
      , [OldEmployerNum] [varchar](6) null      
) 
insert into [#PensionWorkHistory]
exec usp_pensioninterface4opus_by_dates @COMPUTATIONYEAR,null,null


IF (@OPTIONALPARAMETER = 0)
BEGIN
      TRUNCATE TABLE OPUS.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT 
	  
      INSERT INTO OPUS.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT
      SELECT * FROM
      (
            SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN,IAPHOURS , IAPHOURSA2, IAPPERCENT, 'N' AS LATE_FLAG,FromDate,ToDate,Weeks
            FROM [#PensionWorkHistory] 
            WHERE ComputationYear = @COMPUTATIONYEAR
            
            UNION ALL
            
            SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN, IAPHOURS, IAPHOURSA2, IAPPERCENT, 'Y' AS LATE_FLAG,FromDate,ToDate,Weeks
            FROM [#PensionWorkHistory]
            WHERE ComputationYear < @COMPUTATIONYEAR 
            --AND (LateMonthly = 'Y' or LateAnnual = 'Y')
      ) A
      ORDER BY SSN,COMPUTATIONYEAR DESC,EMPACCOUNTNO
      
       --F/W Upgrade Changes
       --IF OBJECT_ID('OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT') IS NOT NULL
       --BEGIN
		--DROP TABLE OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT		
	   --END	   
	  
	   TRUNCATE TABLE OPUS.DBO.SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT
	         
      --SELECT * INTO OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT FROM [#PensionWorkHistory]      
      INSERT INTO OPUS.DBO.SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT SELECT * FROM [#PensionWorkHistory] ORDER BY SSN,COMPUTATIONYEAR DESC,EMPACCOUNTNO      
      
      DELETE FROM [#PensionWorkHistory] 
      
      DECLARE @PREVCOMPUTATIONYEAR INT = @COMPUTATIONYEAR - 1
      IF OBJECT_ID('OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT_FOR_PREV_YEAR') IS NOT NULL
	  BEGIN
	  DROP TABLE OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT_FOR_PREV_YEAR
	  END
      
      INSERT INTO [#PensionWorkHistory]
	  EXEC usp_pensioninterface4opus_by_dates @PREVCOMPUTATIONYEAR,null,null
	  SELECT * 
	  INTO OPUS..SGT_ALL_IAP_FULL_WORKHISTORY_4_SNAPSHOT_FOR_PREV_YEAR 
	  FROM
      (
            SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN,IAPHOURS , IAPHOURSA2, IAPPERCENT, 'N' AS LATE_FLAG,FromDate,ToDate,Weeks
            FROM [#PensionWorkHistory] 
            WHERE ComputationYear = @PREVCOMPUTATIONYEAR
      ) A
      ORDER BY SSN,COMPUTATIONYEAR DESC,EMPACCOUNTNO
	  
      
END

ELSE

BEGIN -- Changes for late Pension hours 
      IF OBJECT_ID('OPUS.DBO.TEMP_TABLE_FOR_LATE_PENSION_HOURS') IS NOT NULL
      BEGIN
            DROP TABLE OPUS.DBO.TEMP_TABLE_FOR_LATE_PENSION_HOURS
      END

      SELECT * INTO OPUS.DBO.TEMP_TABLE_FOR_LATE_PENSION_HOURS FROM
      (
            SELECT SUM(ISNULL(PensionHours,0)) AS PENSIONHOURS, SSN, 'Y' AS LATE_FLAG
            FROM [#PensionWorkHistory]
            WHERE ComputationYear < @COMPUTATIONYEAR 
            AND PENSIONPlAN = 2
            GROUP BY SSN
      ) B

END


drop table [#PensionWorkHistory]

END
----------------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
--Created By	:	Rohan Adgaonkar
--Created On	:	17th July 2012
--Description	:	Scripts for creation of View vwRateGroupBreakdown_all
------------------------------------------------------------------------------------------------------------------------

CREATE View [dbo].[vwRateGroupBreakdown_all]  
AS  
--All contracts
--Summarized contract, collaps all consecutive contract period to show only one entry per rate
--Assumption is, for each RateGroup and composit rate, all periods are cosiquetive or within, no break.  
Select RateGroup, Composite, LastContract = max(LastContract),FromDate = min(FROMDATE),ToDate = max(TODATE)
, Medical, Dental, Vision, PensionEmployee
, PensionEmployer, Individual, NewIAP, HWRetMedical, HWRetDental, HWRetVision
, CSATF1, CSATF2
From(
 SELECT TMP.RateGroup, TMP.Composite, LastContract = TMP.RGCONTRACTID, TMP.FromDate, TMP.ToDate,  
  Medical = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 0),0),  
  Dental = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 1),0),  
  Vision = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 2),0),  
  PensionEmployee = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 3),0),  
  PensionEmployer = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 4),0),  
  Individual = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 5),0),  
  NewIAP = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 6),0),  
  HWRETMEDICAL = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 7),0),  
  HWRETDENTAL = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 8),0),  
  HWRETVISION = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 9),0),  
  CSATF1 = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 10),0),  
  CSATF2 = isnull((select amount from eadb.dbo.RateItems where RGContractID = TMP.RGContractId and RateItemTypeId = 11),0)
  FROM eadb.dbo.Ratecontract TMP
  WHERE Composite <>0 and RGContractID not in (860, 1129, 915, 948, 978, 979)  
)X
  group by RateGroup, Composite, Medical, Dental, Vision, PensionEmployee
, PensionEmployer, Individual, NewIAP, HWRetMedical, HWRetDental, HWRetVision
, CSATF1, CSATF2
--  order by rategroup, fromdate
GO
 
CREATE View [dbo].[vwRateGroupClassification_all]  
AS  
 SELECT distinct RateGroup, Composite, FromDate, ToDate,  
  ToHealthSystem  = case when Medical > 0 or Dental > 0 or Vision > 0 then 1 else 0 end,  
  ToPensionSystem = case when PensionEmployee > 0 or PensionEmployer > 0 or Individual > 0 or NewIAP > 0 then 1 else 0 end,  
  ActiveHealth  = case when Medical > 0 or Dental > 0 or Vision > 0 then 1 else 0 end,  
  Pension = case when PensionEmployee > 0 or PensionEmployer > 0 then 1 else 0 end,  
  IAP = case when Individual > 0 then 1 else 0 end,  
  IAPPercent = case when NewIAP > 0 then 1 else 0 end,  
  RetireeHealth = case when HWRETMEDICAL > 0 or HWRETDENTAL > 0 or HWRETVISION > 0 then 1 else 0 end,  
  CSATF = case when CSATF1 > 0 or CSATF2 > 0 then 1 else 0 end  
 from vwRateGroupBreakdown_all  
GO



-- ===================================================================== --Abhi
-- Author:        Rohan Adgaonkar
-- Create date:	  04/09/2013
-- Description:   usp_PensionInterface4OPUS
-- ==================================================================== 
CREATE Procedure [dbo].[usp_PensionInterface4OPUS] (@SSN char(9)=null)  
AS
BEGIN
set nocount on
		
CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1) NOT NULL,
	[LateAnnual] [varchar](1) NOT NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

insert into [#PensionWorkHistory]
select	
	ReportID = convert(varchar(18), Report.ReportID),		--old was char(10), but in order to include HP id increased to varchar(18)
	--EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)
	EmpAccountNo = E.EmployerId,
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
	FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
	ToDate = Report.ToDate,		-- old was char(8) yyyymmdd, now no conversion it is smalldatetime
	Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),
	Received = Report.RecDate,		-- old was char(8) yyyymmdd, now no conversion it is smalldatetime
	Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
	HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)
	SSN = convert(char(9),Hours.SSN),
	LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)
	FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)
	IAPHours = case when report.rategroup = 8 then Hours.HoursWorked 
					when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)
					else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)
	IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)
	IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.
	ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)
	RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)
 NULL AS PersonId, --varchar(15) no change  
	RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)
	HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.
	LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,
	LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,
	--------------------------------------------------------------------------------------------------------------
	UnionMisc = Hours.UnionMisc, --New field. char(2)
	HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system. 
								--It is required because for those records where we do not have any rate group info
								--it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.
	IAPHourlyRate = rgb.Individual  --New field. money
 , Source = 'C/S '  
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, OldEmployerNum = e.OldEmployerNum
from	eadb.dbo.Report report
	inner join eadb.dbo.Hours hours on report.reportid = hours.reportid 
	and hours.SSN = @SSN 
	--and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate
	--and report.ToDate between @FromDate and @ToDate
	inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
	inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904)	--Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.
--Employer id for Locals Pre-Merger hours.
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)

insert into [#PensionWorkHistory]
select	
	ReportID = HPTransactions.Ber,
	--EmpAccountNo = convert(int, HPTransactions.Employer),
	EmpAccountNo = E.EmployerId,
	PensionYear = PY.PensionYear,
	FromDate = convert(smalldatetime, HPTransactions.StartDate),
	ToDate = convert(smalldatetime, HPTransactions.EndDate),
	--Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),	
	Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),
	Received = convert(smalldatetime, HPTransactions.DateReceived),
	--Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.
	Processed = convert(smalldatetime,hb.Updated),
	HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),
	SSN = convert(char(9),HPTransactions.SSN),
 LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
 FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
	UnionCode = convert(int,HPTransactions.UnionCode),
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),
	IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later 
	IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP
	IAPPercent = convert(money,HPTransactions.IAPDollars),
	ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),
	RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),
 NULL AS PersonId,  
	RateGroup = convert(varchar(4),HPTransactions.RateGroup), 
	HoursStatus = 0,
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
	--------------------------------------------------------------------------------------------------------------
	UnionMisc = HPTransactions.UNMisc,
	HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),
	IAPHourlyRate = rgb.Individual
 , Source = 'H/P '  
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, e.OldEmployerNum
from eadb.dbo.HPTransactions HPTransactions
	inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
		on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate 
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
		on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch 
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
and HPTransactions.SSN = @SSN
	--and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate
	--and HPTransactions.EndDate between @FromDate and @ToDate

--CPAS View
insert into [#PensionWorkHistory]
select	
	ReportID = left(cpas.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = cpas.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, cpas.FDate),
	ToDate = convert(smalldatetime, cpas.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),
	Received = convert(smalldatetime, cpas.PDate),
	Processed = convert(smalldatetime, cpas.PDate),
	HoursId = convert(varchar(24),cpas.erractid),
	SSN = convert(char(9),cpas.MKey),
 LastName = NULL, --convert(char(50),p.LastName),  
 FirstName = NULL, --convert(char(50),p.FirstName),  
	UnionCode = convert(int,cpas.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
 PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
	IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP
	IAPPercent = convert(money,cpas.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),
	RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),
 NULL PersonId,  
	RateGroup = convert(varchar(4),cpas.RateGroup),
	HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,
	LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	UnionMisc = null,
	HoursWorked = convert(numeric(7, 1), cpas.HRSACT),
	IAPHourlyRate = rgb.Individual
 , Source = 'CPAS'  
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, E.OldEmployerNum
	from EADB.dbo.CPASPre95_11222011 cpas
	left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join EADB.dbo.vwRateGroupClassification_all RGC 
		on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate 
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
		on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	and cpas.mkey = @SSN
	--and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate
	--and cpas.TDate between @FromDate and @ToDate

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	LastName = NULL, --convert(char(50),p.LastName),
	FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	NULL AS PersonId,
	RateGroup = convert(varchar(4),rap.RateGroup),
	HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	UnionMisc = null,
	HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	IAPHourlyRate = rgb.Individual
	, Source = 'RAP'
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, E.OldEmployerNum
	from EADB.dbo.RAP_IAP$ rap
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	and rap.mkey = @SSN

--PreMerger View.
insert into [#PensionWorkHistory]
select	
 ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
	EmpAccountNo = convert(int, Pre.EmployerId), 
	ComputationYear = Pre.Plan_Year,
	FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date
	ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date
	Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53
	Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date 
	Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date
 HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
	SSN = convert(char(9),Pre.SSN), 
 LastName = NULL, --convert(char(50),p.LastName),  
 FirstName = NULL, --convert(char(50),p.FirstName),  
	UnionCode = convert(int,Pre.UnionCode), 
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
      when [Local]='L666' then convert(smallint, 4)  
      when [Local]='L700' then convert(smallint, 6)  
      when [Local]='L52' then convert(smallint, 7)  
      when [Local]='L161' then convert(smallint, 8)  
						else null end, 
	PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),
	L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),
	PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),
	IAPHours = convert(numeric(7, 1), 0),
	IAPHoursA2 = convert(numeric(7, 1), 0), 
	IAPPercent = convert(money, 0), 
	ActiveHealthHours = convert(numeric(7, 1), 0), 
	RetireeHealthHours = convert(numeric(7, 1), 0), -- ?
 NULL AS PersonId,  
	RateGroup = Pre.RateGroup,--?
	HoursStatus = convert(int, 0),
 LateMonthly = '',   
 LateAnnual = '' ,  
	-------------------------------------------------------------------
 UnionMisc = convert(char(2),''),  
	HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),
	IAPHourlyRate = rgb.Individual
 , Source = 'PM  '  
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, OldEmployerNum = Pre.EmployerId
from EADB.dbo.Pension_PreMerger_Annual_History Pre  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
		on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate 
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
		on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate
 --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where Pre.SSN = @SSN
	--(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate
	--Pre.EndDate between @FromDate and @ToDate

select * from [#PensionWorkHistory]
order by todate
drop table [#PensionWorkHistory]

END
GO

-- =====================================================================
-- Author:        Rohan Adgaonkar
-- Create date:	  04/09/2013
-- Description:   usp_GETYEARENDEXTRACTIONDATAYEARLY
-- ==================================================================== 
/****** Object:  StoredProcedure [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]    Script Date: 04/07/2013 23:56:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]  
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
 [SSN] [varchar](9) NULL,  
 [PensionYear] [int] NULL   
 )   

INSERT INTO @temp    
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO  
  
declare @year int  
set select top(1) @year = PensionYear from @temp  
  
delete from OPUS_AnnualStmt_Participant_List  
  
INSERT INTO OPUS_AnnualStmt_Participant_List  
select * from  @temp   
  
declare @PlanStartDate datetime  
declare @PlanEndDate datetime  
declare @CutOffDate datetime  
 select @PlanStartDate = eadb.dbo.fn_PlanYearStartDate(@year),@PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)  
    select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate  
    
CREATE TABLE [#PensionWorkHistory](  
 --[ReportID] [varchar](18) NULL,  
 [EmpAccountNo] [int] NULL,  
 [ComputationYear] [int] NULL,  
 [FromDate] [smalldatetime] NULL,  
 [ToDate] [smalldatetime] NULL,  
 [Weeks] [char](2) NULL,  
 --[Received] [smalldatetime] NULL,  
 [Processed] [smalldatetime] NULL,  
 --[HoursID] [varchar](24) NULL,  
 [SSN] [varchar](9) NULL,  
 --[LastName] [varchar](50) NULL,  
 --[FirstName] [varchar](50) NULL,  
 [UnionCode] [int] NULL,  
 [PensionPlan] [smallint] NULL,  
 --[PensionCredit] [numeric](7, 3) NULL,  
 --[L52VestedCredit] [smallint] NULL,  
 [PensionHours] [numeric](7, 1) NULL,  
 [IAPHours] [numeric](7, 1) NULL,  
 --[IAPHoursA2] [numeric](7, 1) NULL,  
 --[IAPPercent] [money] NULL,  
 --[ActiveHealthHours] [numeric](7, 1) NULL,  
 --[RetireeHealthHours] [numeric](7, 1) NULL,  
 --[PersonId] [varchar](15) NULL,  
 --[RateGroup] [varchar](4) NULL,  
 --[HoursStatus] [int] NULL,  
 [LateMonthly] [varchar](1) NOT NULL,  
 [LateAnnual] [varchar](1) NOT NULL,
 [EmployerName] [varchar] (255) NULL
 --[UnionMisc] [char](2) NULL,  
 --[HoursWorked] [numeric](7, 1) NULL,  
 --[IAPHourlyRate] [money] NULL,  
 --[Source] [varchar](4) NOT NULL,  
 --[ToHealthSystem] [int] NULL,  
 --[ToPensionSystem] [int] NULL,  
 --[IsActiveHealth] [int] NULL,  
 --[IsRetireeHealth] [int] NULL,  
 --[IsPension] [int] NULL,  
 --[IsIAPHourly] [int] NULL  
 --, [OldEmployerNum] [varchar](6)  
)   
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)  
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
 SSN = convert(char(9),Hours.SSN),  
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
 --NULL PersonId, --varchar(15) no change  
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
 LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end ,
 EmployerName = E.EmployerName 
 --------------------------------------------------------------------------------------------------------------  
 --UnionMisc = Hours.UnionMisc, --New field. char(2)  
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
        --It is required because for those records where we do not have any rate group info  
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
 --IAPHourlyRate = rgb.Individual  --New field. money  
 --, Source = 'C/S '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
 inner join eadb.dbo.Report report on report.reportid = hours.reportid   
 --and hours.SSN = @SSN   
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
 --and report.ToDate between @FromDate and @ToDate   
 inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate   
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904) and  
Report.RecDate <= isnull(@CutOffDate,Report.RecDate)  
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = HPTransactions.Ber,  
 ----EmpAccountNo = convert(int, HPTransactions.Employer),  
 EmpAccountNo = E.EmployerId,  
 PensionYear = PY.PensionYear,  
 FromDate = convert(smalldatetime, HPTransactions.StartDate),  
 ToDate = convert(smalldatetime, HPTransactions.EndDate),  
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),   
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
 --Received = convert(smalldatetime, HPTransactions.DateReceived),  
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
 --Processed = convert(smalldatetime,hb.Updated),  
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
 SSN = convert(char(9),HPTransactions.SSN),  
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
 UnionCode = convert(int,HPTransactions.UnionCode),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,HPTransactions.IAPDollars),  
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
 --HoursStatus = 0,  
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,
 EmployerName = E.EmployerName  
 ----------------------------------------------------------------------------------------------------------------  
 --UnionMisc = HPTransactions.UNMisc,  
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'H/P '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
 inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP   
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
 --and HPTransactions.SSN = @SSN  
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
 --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select   
 --ReportID = left(cpas.erractid,18),  
 ----EmpAccountNo = convert(int, cpas.ERKey),  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
 FromDate = convert(smalldatetime, cpas.FDate),  
 ToDate = convert(smalldatetime, cpas.TDate),  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
 --Received = convert(smalldatetime, cpas.PDate),  
 Processed = convert(smalldatetime, cpas.PDate),  
 --HoursId = convert(varchar(24),cpas.erractid),  
 SSN = convert(char(9),cpas.MKey),  
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,cpas.LOC_NO),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
 IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
 ----we are not checking rate item to identify hours for Pension, Health, or IAP  
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,cpas.PanOnEarn),  
 ----MM 12/26/12  
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),cpas.RateGroup),  
 --HoursStatus = 0, --all the hours comming from CPAS are processed.  
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
 EmployerName = E.EmployerName 
 -----------------------------------------------------------------------  
 --UnionMisc = null,  
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'CPAS'  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, E.OldEmployerNum  
 from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 where [Plan]=2  
 --and cpas.mkey = @SSN  
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
 --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	EmployerName = E.EmployerName
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	from OPUS_AnnualStmt_Participant_List list  
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey=list.SSN
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN

  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
 EmpAccountNo = convert(int, Pre.EmployerId),   
 ComputationYear = Pre.Plan_Year,  
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
 SSN = convert(char(9),Pre.SSN),   
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,Pre.UnionCode),   
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
      when [Local]='L666' then convert(smallint, 4)  
      when [Local]='L700' then convert(smallint, 6)  
      when [Local]='L52' then convert(smallint, 7)  
      when [Local]='L161' then convert(smallint, 8)  
      else null end,   
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
 IAPHours = convert(numeric(7, 1), 0),  
 --IAPHoursA2 = convert(numeric(7, 1), 0),   
 --IAPPercent = convert(money, 0),   
 --ActiveHealthHours = convert(numeric(7, 1), 0),   
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
 --NULL PersonId,  
 --RateGroup = Pre.RateGroup,--?  
 --HoursStatus = convert(int, 0),  
 LateMonthly = '',   
 LateAnnual = '' ,
 EmployerName = E.EmployerName 
 -------------------------------------------------------------------  
 --UnionMisc = convert(char(2),''),  
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'PM  '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = Pre.EmployerId  
from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId
 --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
 --Pre.EndDate between @FromDate and @ToDate  
  
--select isnull(EmpAccountNo, 0) as EmpAccountNo, ComputationYear, SSN, UnionCode, PensionHours, case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours, 
--isnull(EmployerName, '') as EmployerName from [#PensionWorkHistory] where 
--PensionPlan = 2 
				
																																		   
--insert into PensionWorkHistoryForStmt    
select ssn,computationyear,sum(isnull(pensionhours,0.0)) - sum(isnull(latehours,0.0)) TotalPensionHours,sum(isnull(latehours,0.0))TotalLateHours,UnionCode,EmpAccountno,EmployerName      
from       
(      
 select ssn,computationyear,pensionhours,case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,unioncode,empaccountno,e.employername,processed      
 from [#PensionWorkHistory] v      
 left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid  
  where v.PensionPlan = 2         
)a      
group by ssn,computationyear,unioncode,empaccountno,employername   

drop table [#PensionWorkHistory]

end  
GO


-- =====================================================================
-- Author:        Rohan Adgaonkar
-- Create date:	  04/09/2013
-- Description:   usp_GETYEARENDEXTRACTIONDATA
-- ==================================================================== 
/****** Object:  StoredProcedure [dbo].[usp_GETYEARENDEXTRACTIONDATA]    Script Date: 04/08/2013 00:08:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GETYEARENDEXTRACTIONDATA]  
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
 [SSN] [varchar](9) NULL,  
 [PensionYear] [int] NULL   
 )   

INSERT INTO @temp    
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO  
  
declare @year int  
set select top(1) @year = PensionYear from @temp  
  
delete from OPUS_AnnualStmt_Participant_List  
  
INSERT INTO OPUS_AnnualStmt_Participant_List  
select * from  @temp   
  
declare @PlanStartDate datetime  
declare @PlanEndDate datetime  
declare @CutOffDate datetime  
 select @PlanStartDate = eadb.dbo.fn_PlanYearStartDate(@year),@PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)  
    select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate  
    
CREATE TABLE [#PensionWorkHistory](  
 --[ReportID] [varchar](18) NULL,  
 [EmpAccountNo] [int] NULL,  
 [ComputationYear] [int] NULL,  
 [FromDate] [smalldatetime] NULL,  
 [ToDate] [smalldatetime] NULL,  
 [Weeks] [char](2) NULL,  
 --[Received] [smalldatetime] NULL,  
 [Processed] [smalldatetime] NULL,  
 --[HoursID] [varchar](24) NULL,  
 [SSN] [varchar](9) NULL,  
 --[LastName] [varchar](50) NULL,  
 --[FirstName] [varchar](50) NULL,  
 [UnionCode] [int] NULL,  
 [PensionPlan] [smallint] NULL,  
 --[PensionCredit] [numeric](7, 3) NULL,  
 --[L52VestedCredit] [smallint] NULL,  
 [PensionHours] [numeric](7, 1) NULL,  
 [IAPHours] [numeric](7, 1) NULL,  
 --[IAPHoursA2] [numeric](7, 1) NULL,  
 --[IAPPercent] [money] NULL,  
 --[ActiveHealthHours] [numeric](7, 1) NULL,  
 --[RetireeHealthHours] [numeric](7, 1) NULL,  
 --[PersonId] [varchar](15) NULL,  
 --[RateGroup] [varchar](4) NULL,  
 --[HoursStatus] [int] NULL,  
 [LateMonthly] [varchar](1) NOT NULL,  
 [LateAnnual] [varchar](1) NOT NULL,
 [EmployerName] [varchar] (255) NULL
 --[UnionMisc] [char](2) NULL,  
 --[HoursWorked] [numeric](7, 1) NULL,  
 --[IAPHourlyRate] [money] NULL,  
 --[Source] [varchar](4) NOT NULL,  
 --[ToHealthSystem] [int] NULL,  
 --[ToPensionSystem] [int] NULL,  
 --[IsActiveHealth] [int] NULL,  
 --[IsRetireeHealth] [int] NULL,  
 --[IsPension] [int] NULL,  
 --[IsIAPHourly] [int] NULL  
 --, [OldEmployerNum] [varchar](6)  
)   
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)  
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
 SSN = convert(char(9),Hours.SSN),  
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
 --NULL PersonId, --varchar(15) no change  
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
 LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end ,
 EmployerName = E.EmployerName 
 --------------------------------------------------------------------------------------------------------------  
 --UnionMisc = Hours.UnionMisc, --New field. char(2)  
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
        --It is required because for those records where we do not have any rate group info  
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
 --IAPHourlyRate = rgb.Individual  --New field. money  
 --, Source = 'C/S '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
 inner join eadb.dbo.Report report on report.reportid = hours.reportid   
 --and hours.SSN = @SSN   
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
 --and report.ToDate between @FromDate and @ToDate   
 inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate   
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904) and  
Report.RecDate <= isnull(@CutOffDate,Report.RecDate)  
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = HPTransactions.Ber,  
 ----EmpAccountNo = convert(int, HPTransactions.Employer),  
 EmpAccountNo = E.EmployerId,  
 PensionYear = PY.PensionYear,  
 FromDate = convert(smalldatetime, HPTransactions.StartDate),  
 ToDate = convert(smalldatetime, HPTransactions.EndDate),  
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),   
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
 --Received = convert(smalldatetime, HPTransactions.DateReceived),  
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
 --Processed = convert(smalldatetime,hb.Updated),  
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
 SSN = convert(char(9),HPTransactions.SSN),  
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
 UnionCode = convert(int,HPTransactions.UnionCode),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,HPTransactions.IAPDollars),  
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
 --HoursStatus = 0,  
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,
 EmployerName = E.EmployerName  
 ----------------------------------------------------------------------------------------------------------------  
 --UnionMisc = HPTransactions.UNMisc,  
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'H/P '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
 inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP   
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
 --and HPTransactions.SSN = @SSN
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
 --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select   
 --ReportID = left(cpas.erractid,18),  
 ----EmpAccountNo = convert(int, cpas.ERKey),  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
 FromDate = convert(smalldatetime, cpas.FDate),  
 ToDate = convert(smalldatetime, cpas.TDate),  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
 --Received = convert(smalldatetime, cpas.PDate),  
 Processed = convert(smalldatetime, cpas.PDate),  
 --HoursId = convert(varchar(24),cpas.erractid),  
 SSN = convert(char(9),cpas.MKey),  
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,cpas.LOC_NO),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
 IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
 ----we are not checking rate item to identify hours for Pension, Health, or IAP  
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,cpas.PanOnEarn),  
 ----MM 12/26/12  
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),cpas.RateGroup),  
 --HoursStatus = 0, --all the hours comming from CPAS are processed.  
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
 EmployerName = E.EmployerName 
 -----------------------------------------------------------------------  
 --UnionMisc = null,  
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'CPAS'  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, E.OldEmployerNum  
 from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 where [Plan]=2  
 --and cpas.mkey = @SSN  
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
 --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	EmployerName = E.EmployerName
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	from OPUS_AnnualStmt_Participant_List list 
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey = list.ssn
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
 EmpAccountNo = convert(int, Pre.EmployerId),   
 ComputationYear = Pre.Plan_Year,  
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
 SSN = convert(char(9),Pre.SSN),   
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,Pre.UnionCode),   
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
      when [Local]='L666' then convert(smallint, 4)  
      when [Local]='L700' then convert(smallint, 6)  
      when [Local]='L52' then convert(smallint, 7)  
      when [Local]='L161' then convert(smallint, 8)  
      else null end,   
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
 IAPHours = convert(numeric(7, 1), 0),  
 --IAPHoursA2 = convert(numeric(7, 1), 0),   
 --IAPPercent = convert(money, 0),   
 --ActiveHealthHours = convert(numeric(7, 1), 0),   
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
 --NULL PersonId,  
 --RateGroup = Pre.RateGroup,--?  
 --HoursStatus = convert(int, 0),  
 LateMonthly = '',   
 LateAnnual = '' ,
 EmployerName = E.EmployerName 
 -------------------------------------------------------------------  
 --UnionMisc = convert(char(2),''),  
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'PM  '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = Pre.EmployerId  
from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId
 --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
 --Pre.EndDate between @FromDate and @ToDate  
  
select ComputationYear AS ComputationYear, FromDate as FromDate, ToDate as ToDate, Weeks as Weeks,PensionHours as PensionHours,SSN as SSN from [#PensionWorkHistory] where 
PensionPlan = 2 AND ComputationYear in (1979,1988)
				
																																		   
--insert into PensionWorkHistoryForStmt    
--select ssn,computationyear,sum(isnull(pensionhours,0.0)) - sum(isnull(latehours,0.0)) TotalPensionHours,sum(isnull(latehours,0.0))TotalLateHours,UnionCode,EmpAccountno,EmployerName      
--from       
--(      
-- select ssn,computationyear,pensionhours,case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,unioncode,empaccountno,e.employername,processed      
-- from [#PensionWorkHistory] v      
-- left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid  
--  where v.PensionPlan = 2         
--)a      
--group by ssn,computationyear,unioncode,empaccountno,employername   

drop table [#PensionWorkHistory]

end  
go


-----------------------------------------------------------------------------------------------
--Name - Abhishek	
--Date - 18th April 2013
--Purpose - Stored Procedure CHanges to Get the Plan Start Date too as a new COLUMN - required for Vesting and MD and Late CAlcs
-----------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataTillGivenDate]    Script Date: 04/18/2013 10:12:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[usp_GetWorkDataTillGivenDate](@SSN char(10),@PLANCODE varchar(20),@RETIREMENT_DATE DATETIME=null)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

IF @RETIREMENT_DATE = '01/01/1753'
BEGIN
	SET @RETIREMENT_DATE=NULL
END

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN
 
IF @RETIREMENT_DATE <> NULL OR @RETIREMENT_DATE <> '' OR @RETIREMENT_DATE <> '01/01/1753'
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and ToDate <= @RETIREMENT_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END
END

ELSE 
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

END

END


-----------------------------------------------------------------------------------------------
--Name - Abhishek	
--Date - 18th April 2013
--Purpose - Stored Procedure CHanges to Get the Plan Start Date too as a new COLUMN - required for Vesting and MD and Late CAlcs
-----------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataForPersonOverview]    Script Date: 04/18/2013 11:02:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[usp_GetWorkDataForPersonOverview](@SSN char(9))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [int] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT DISTINCT VPIO.ComputationYear AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS QUALIFIED_HOURS,
 (SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS VESTED_HOURS,
 (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalIAPHours,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

END


-----------------------------------------------------------------------------------------------
--Name - Abhishek	
--Date - 18th April 2013
--Purpose - Stored Procedure CHanges to Get the Plan Start Date too as a new COLUMN - required for Vesting and MD and Late CAlcs
-----------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetHealthWorkHistoryForAllMpippParticipant]    Script Date: 04/18/2013 12:01:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetHealthWorkHistoryForAllMpippParticipant]
AS
BEGIN
SET NOCOUNT ON

DECLARE @temp TABLE(
                [SSN] [varchar](9) NULL,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                                                         
                ) 

INSERT INTO @temp  
EXEC OPUS.dbo.[GET_ALL_PARTICIPANT_SSN_FOR_HEALTH_ELIGBILITY]

delete from OPUS_Participant_List 

INSERT INTO OPUS_Participant_List
select * from  @temp 

                                
CREATE TABLE [#PensionWorkHistory](
                [ReportID] [varchar](18) NULL,
                [EmpAccountNo] [int] NULL,
                [ComputationYear] [smallint] NULL,
                [FromDate] [smalldatetime] NULL,
                [ToDate] [smalldatetime] NULL,
                [Weeks] [char](2) NULL,
                [Received] [smalldatetime] NULL,
                [Processed] [smalldatetime] NULL,
                [HoursID] [varchar](24) NULL,
                [SSN] [char](9) NULL,
                [LastName] [varchar](50) NULL,
                [FirstName] [varchar](50) NULL,
                [UnionCode] [int] NULL,
                [PensionPlan] [smallint] NULL,
                [PensionCredit] [numeric](7, 3) NULL,
                [L52VestedCredit] [smallint] NULL,
                [PensionHours] [numeric](7, 1) NULL,
                [IAPHours] [numeric](7, 1) NULL,
                [IAPHoursA2] [numeric](7, 1) NULL,
                [IAPPercent] [money] NULL,
                [ActiveHealthHours] [numeric](7, 1) NULL,
                [RetireeHealthHours] [numeric](7, 1) NULL,
                [PersonId] [varchar](15) NULL,
                [RateGroup] [varchar](4) NULL,
                [HoursStatus] [int] NULL,
                [LateMonthly] [varchar](1) NOT NULL,
                [LateAnnual] [varchar](1) NOT NULL,
                [UnionMisc] [char](2) NULL,
                [HoursWorked] [numeric](7, 1) NULL,
                [IAPHourlyRate] [money] NULL,
                [Source] [varchar](4) NOT NULL,
                [ToHealthSystem] [int] NULL,
                [ToPensionSystem] [int] NULL,
                [IsActiveHealth] [int] NULL,
                [IsRetireeHealth] [int] NULL,
                [IsPension] [int] NULL,
                [IsIAPHourly] [int] NULL
                , [OldEmployerNum] [varchar](6)
) 

insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)
                EmpAccountNo = E.EmployerId,
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),
                Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)
                SSN = convert(char(9),Hours.SSN),
                LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)
                FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)
                UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked 
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)
                IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)
                IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.
                ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)
                RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)
                NULL PersonId, --varchar(15) no change
                RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)
                HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.
                LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,
                LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = Hours.UnionMisc, --New field. char(2)
                HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system. 
                                                                                                                                --It is required because for those records where we do not have any rate group info
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.
                IAPHourlyRate = rgb.Individual  --New field. money
                , Source = 'C/S '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = e.OldEmployerNum
from OPUS_Participant_List list 
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN
                inner join eadb.dbo.Report report on report.reportid = hours.reportid 
                --and hours.SSN = @SSN 
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate
                --and report.ToDate between @FromDate and @ToDate             
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate 
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.
--Employer id for Locals Pre-Merger hours.
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)

insert into [#PensionWorkHistory]
select    
                ReportID = HPTransactions.Ber,
                --EmpAccountNo = convert(int, HPTransactions.Employer),
                EmpAccountNo = E.EmployerId,
                PensionYear = PY.PensionYear,
                FromDate = convert(smalldatetime, HPTransactions.StartDate),
                ToDate = convert(smalldatetime, HPTransactions.EndDate),
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),               
                Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),
                Received = convert(smalldatetime, HPTransactions.DateReceived),
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.
                Processed = convert(smalldatetime,hb.Updated),
                HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),
                SSN = convert(char(9),HPTransactions.SSN),
                LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),
                FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),
                UnionCode = convert(int,HPTransactions.UnionCode),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later 
                IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP
                IAPPercent = convert(money,HPTransactions.IAPDollars),
                ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),
                RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),
                NULL PersonId,
                RateGroup = convert(varchar(4),HPTransactions.RateGroup), 
                HoursStatus = 0,
                LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = HPTransactions.UNMisc,
                HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'H/P '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , e.OldEmployerNum
from OPUS_Participant_List list 
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP            
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch 
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate
                --and HPTransactions.EndDate between @FromDate and @ToDate

--CPAS View
insert into [#PensionWorkHistory]
select    
                ReportID = left(cpas.erractid,18),
                --EmpAccountNo = convert(int, cpas.ERKey),
                EmpAccountNo = E.EmployerId,
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,
                FromDate = convert(smalldatetime, cpas.FDate),
                ToDate = convert(smalldatetime, cpas.TDate),
                Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),
                Received = convert(smalldatetime, cpas.PDate),
                Processed = convert(smalldatetime, cpas.PDate),
                HoursId = convert(varchar(24),cpas.erractid),
                SSN = convert(char(9),cpas.MKey),
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,cpas.LOC_NO),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
				--we are not checking rate item to identify hours for Pension, Health, or IAP
				--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
				IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP
				IAPPercent = convert(money,cpas.PanOnEarn),
				--MM 12/26/12
				--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
				--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
				ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),
				RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),
                NULL PersonId,
                RateGroup = convert(varchar(4),cpas.RateGroup),
                HoursStatus = 0, --all the hours comming from CPAS are processed.
                LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,
                LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
                ---------------------------------------------------------------------
                UnionMisc = null,
                HoursWorked = convert(numeric(7, 1), cpas.HRSACT),
                IAPHourlyRate = rgb.Individual
                , Source = 'CPAS'
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , E.OldEmployerNum
                from OPUS_Participant_List list 
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
                where [Plan]=2
                --and cpas.mkey = @SSN
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate
                --and cpas.TDate between @FromDate and @ToDate
                
-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	LastName = NULL, --convert(char(50),p.LastName),
	FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	NULL AS PersonId,
	RateGroup = convert(varchar(4),rap.RateGroup),
	HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	UnionMisc = null,
	HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	IAPHourlyRate = rgb.Individual
	, Source = 'RAP'
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, E.OldEmployerNum
	from OPUS_Participant_List list 
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey = list.SSN
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN

               
--PreMerger View.
insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,
                EmpAccountNo = convert(int, Pre.EmployerId), 
                ComputationYear = Pre.Plan_Year,
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date
                ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53
                Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date 
                Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date
                HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id
                SSN = convert(char(9),Pre.SSN), 
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,Pre.UnionCode), 
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)
                                                                                                when [Local]='L666' then convert(smallint, 4)
                                                                                                when [Local]='L700' then convert(smallint, 6)
                                                                                                when [Local]='L52' then convert(smallint, 7)
                                                                                                when [Local]='L161' then convert(smallint, 8)
                                                                                                else null end, 
                PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),
                L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHours = convert(numeric(7, 1), 0),
                IAPHoursA2 = convert(numeric(7, 1), 0), 
                IAPPercent = convert(money, 0), 
                ActiveHealthHours = convert(numeric(7, 1), 0), 
                RetireeHealthHours = convert(numeric(7, 1), 0), -- ?
                NULL PersonId,
                RateGroup = Pre.RateGroup,--?
                HoursStatus = convert(int, 0),
                LateMonthly = '', 
                LateAnnual = '' ,
                -------------------------------------------------------------------
                UnionMisc = convert(char(2),''),
                HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'PM  '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = Pre.EmployerId
from OPUS_Participant_List list 
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
--where --Pre.SSN = @SSN
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate
                --Pre.EndDate between @FromDate and @ToDate

--select * from [#PensionWorkHistory]
--order by todate
--select * from [#PensionWorkHistory]
--order by todate

--insert into PensionWorkHistoryForStmt  

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,VPIO.SSN,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS idcPensionHours_healthBatch,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO ORDER BY VPIO.SSN,cast(VPIO.ComputationYear as int)
end

GO


-----------------------------------------------------------------------------------------------
--Name - Wasim Pathan	
--Date - 04/25/2013
--Purpose - CREATE Stored Procedure To get New Participant SSN
-----------------------------------------------------------------------------------------------
CREATE PROC [dbo].[usp_GetWorkHistoryForNewMpippParticipant] (@Year int)
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
                [SSN] [varchar](9) NULL ,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                         
                )   
  
INSERT INTO @temp    
EXEC OPUS.dbo.[GET_NEW_PARTICIPANT_SSN]  
  
delete from OPUS_Participant_List   
  
INSERT INTO OPUS_Participant_List  
select * from  @temp   
  
                                  
CREATE TABLE [#PensionWorkHistory](  
                --[ReportID] [varchar](18) NULL,  
                --[EmpAccountNo] [int] NULL,  
                [ComputationYear] [smallint] NULL,  
                [FromDate] [smalldatetime] NULL,  
                --[ToDate] [smalldatetime] NULL,  
                --[Weeks] [char](2) NULL,  
                --[Received] [smalldatetime] NULL,  
                --[Processed] [smalldatetime] NULL,  
                --[HoursID] [varchar](24) NULL,  
                [SSN] [char](9) NULL,  
                --[LastName] [varchar](50) NULL,  
                --[FirstName] [varchar](50) NULL,  
                --[UnionCode] [int] NULL,  
                [PensionPlan] [smallint] NULL,  
                --[PensionCredit] [numeric](7, 3) NULL,  
                --[L52VestedCredit] [smallint] NULL,  
                [PensionHours] [numeric](7, 1) NULL,  
                [IAPHours] [numeric](7, 1) NULL,  
                --[IAPHoursA2] [numeric](7, 1) NULL,  
                --[IAPPercent] [money] NULL,  
                --[ActiveHealthHours] [numeric](7, 1) NULL,  
                --[RetireeHealthHours] [numeric](7, 1) NULL,  
                --[PersonId] [varchar](15) NULL,  
                --[RateGroup] [varchar](4) NULL,  
                --[HoursStatus] [int] NULL,  
                --[LateMonthly] [varchar](1) NOT NULL,  
                --[LateAnnual] [varchar](1) NOT NULL,  
                --[UnionMisc] [char](2) NULL,  
                --[HoursWorked] [numeric](7, 1) NULL,  
                --[IAPHourlyRate] [money] NULL,  
                --[Source] [varchar](4) NOT NULL,  
                --[ToHealthSystem] [int] NULL,  
                --[ToPensionSystem] [int] NULL,  
                --[IsActiveHealth] [int] NULL,  
                --[IsRetireeHealth] [int] NULL,  
                --[IsPension] [int] NULL,  
                --[IsIAPHourly] [int] NULL, 
                --[OldEmployerNum] [varchar](6), 
                [CheckVesting] [varchar](1) 
)   
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)  
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
                --Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
                SSN = convert(char(9),Hours.SSN),  
                --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
                --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
                --UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
                --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
                --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
                --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
                --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
                --NULL PersonId, --varchar(15) no change  
                --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
                --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
                --LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = Hours.UnionMisc, --New field. char(2)  
                --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
                                                                                                                                --It is required because for those records where we do not have any rate group info  
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
                --IAPHourlyRate = rgb.Individual  --New field. money  
                --, Source = 'C/S '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = e.OldEmployerNum 
                [CheckVesting] = case when ((Year(Report.RecDate) = @Year and Report.FromDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y')then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
                inner join eadb.dbo.Report report on report.reportid = hours.reportid   
                --and hours.SSN = @SSN   
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
                --and report.ToDate between @FromDate and @ToDate               
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = HPTransactions.Ber,  
                --EmpAccountNo = convert(int, HPTransactions.Employer),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  
                FromDate = convert(smalldatetime, HPTransactions.StartDate),  
                --ToDate = convert(smalldatetime, HPTransactions.EndDate),  
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),                 
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
                --Received = convert(smalldatetime, HPTransactions.DateReceived),  
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
                --Processed = convert(smalldatetime,hb.Updated),  
                --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
                SSN = convert(char(9),HPTransactions.SSN),  
                --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
                --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
                --UnionCode = convert(int,HPTransactions.UnionCode),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
                --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
                --IAPPercent = convert(money,HPTransactions.IAPDollars),  
                --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
                --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
                --HoursStatus = 0,  
                --LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = HPTransactions.UNMisc,  
                --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'H/P '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, e.OldEmployerNum  
                 [CheckVesting] = case when ((Year(HPTransactions.DateReceived) = @Year and HPTransactions.StartDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP              
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN  
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
                --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select      
                --ReportID = left(cpas.erractid,18),  
                --EmpAccountNo = convert(int, cpas.ERKey),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
                FromDate = convert(smalldatetime, cpas.FDate),  
                --ToDate = convert(smalldatetime, cpas.TDate),  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
                --Received = convert(smalldatetime, cpas.PDate),  
                --Processed = convert(smalldatetime, cpas.PDate),  
                --HoursId = convert(varchar(24),cpas.erractid),  
                SSN = convert(char(9),cpas.MKey),  
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,cpas.LOC_NO),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
    --we are not checking rate item to identify hours for Pension, Health, or IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
    --IAPPercent = convert(money,cpas.PanOnEarn),  
    --MM 12/26/12  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),cpas.RateGroup),  
                --HoursStatus = 0, --all the hours comming from CPAS are processed.  
                --LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                --LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                ---------------------------------------------------------------------  
                --UnionMisc = null,  
                --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'CPAS'  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, E.OldEmployerNum  
                 [CheckVesting] = case when ((Year(cpas.PDate) = @Year and cpas.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                where [Plan]=2  
                --and cpas.mkey = @SSN  
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
                --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	--EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	--ToDate = convert(smalldatetime, rap.TDate),
	--Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	--Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	--UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	--LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	--LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	[CheckVesting] = case when ((Year(rap.PDate) = @Year and rap.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
	from OPUS_Participant_List list   
	inner join EADB.dbo.RAP_IAP$ rap on list.ssn = rap.mkey
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
                --EmpAccountNo = convert(int, Pre.EmployerId),   
                ComputationYear = Pre.Plan_Year,  
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
                --ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
                --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
                --Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
                --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
                SSN = convert(char(9),Pre.SSN),   
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,Pre.UnionCode),   
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
                                                                                                when [Local]='L666' then convert(smallint, 4)  
                                                                                                when [Local]='L700' then convert(smallint, 6)  
                                                                                                when [Local]='L52' then convert(smallint, 7)  
                                                                                                when [Local]='L161' then convert(smallint, 8)  
                                                                                                else null end,   
                --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
                --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
                IAPHours = convert(numeric(7, 1), 0),  
                --IAPHoursA2 = convert(numeric(7, 1), 0),   
                --IAPPercent = convert(money, 0),   
                --ActiveHealthHours = convert(numeric(7, 1), 0),   
                --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
                --NULL PersonId,  
                --RateGroup = Pre.RateGroup,--?  
                --HoursStatus = convert(int, 0),  
                --LateMonthly = '',   
                --LateAnnual = '' ,  
                -------------------------------------------------------------------  
                --UnionMisc = convert(char(2),''),  
                --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'PM  '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = Pre.EmployerId  
                [CheckVesting] = case when ((Year(Pre.MergeDate) = @Year and Pre.StartDate < list.[Vesting_Date])OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
                --Pre.EndDate between @FromDate and @ToDate  
  
--select * from [#PensionWorkHistory]  
--order by todate  
--select * from [#PensionWorkHistory]  
--order by todate 
--insert into PensionWorkHistoryForStmt   


--UPDATE [#PensionWorkHistory] SET CheckVesting='Y' WHERE SSN IN (SELECT DISTINCT TE.SSN FROM [#PensionWorkHistory] TE WHERE TE.CheckVesting='Y')
  

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR, VPIO.SSN, VPIO.CheckVesting,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS  
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,  
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,  
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,  
--(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,  
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,  
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,  
--(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS IAP_HOURS,
--(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO ORDER BY VPIO.SSN  

DROP TABLE [#PensionWorkHistory]

END  

Go


-- =====================================================================
-- Author:        Rohan Adgaonkar
-- Create date:	  04/05/2013
-- Description:   usp_GetWorkHistoryForActiveOutboundFile
-- ==================================================================== 

CREATE PROC [dbo].[usp_GetWorkHistoryForActiveOutboundFile] (@Year int)
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
                [SSN] [varchar](9) NULL ,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                         
                )   
  
INSERT INTO @temp    
EXEC OPUS_BIS.dbo.[GET_ALL_PARTICIPANT_SSN_FOR_ACTIVE_OUTBOUND_FILE]  
  
delete from OPUS_Participant_List   
  
INSERT INTO OPUS_Participant_List  
select * from  @temp   
  
                                  
CREATE TABLE [#PensionWorkHistory](  
                --[ReportID] [varchar](18) NULL,  
                --[EmpAccountNo] [int] NULL,  
                [ComputationYear] [smallint] NULL,  
                --[FromDate] [smalldatetime] NULL,  
                --[ToDate] [smalldatetime] NULL,  
                --[Weeks] [char](2) NULL,  
                --[Received] [smalldatetime] NULL,  
                --[Processed] [smalldatetime] NULL,  
                --[HoursID] [varchar](24) NULL,  
                [SSN] [char](9) NULL,  
                --[LastName] [varchar](50) NULL,  
                --[FirstName] [varchar](50) NULL,  
                --[UnionCode] [int] NULL,  
                [PensionPlan] [smallint] NULL,  
                --[PensionCredit] [numeric](7, 3) NULL,  
                --[L52VestedCredit] [smallint] NULL,  
                [PensionHours] [numeric](7, 1) NULL,  
                [IAPHours] [numeric](7, 1) NULL,  
                --[IAPHoursA2] [numeric](7, 1) NULL,  
                --[IAPPercent] [money] NULL,  
                --[ActiveHealthHours] [numeric](7, 1) NULL,  
                --[RetireeHealthHours] [numeric](7, 1) NULL,  
                --[PersonId] [varchar](15) NULL,  
                --[RateGroup] [varchar](4) NULL,  
                --[HoursStatus] [int] NULL,  
                --[LateMonthly] [varchar](1) NOT NULL,  
                --[LateAnnual] [varchar](1) NOT NULL,  
                --[UnionMisc] [char](2) NULL,  
                --[HoursWorked] [numeric](7, 1) NULL,  
                --[IAPHourlyRate] [money] NULL,  
                --[Source] [varchar](4) NOT NULL,  
                --[ToHealthSystem] [int] NULL,  
                --[ToPensionSystem] [int] NULL,  
                --[IsActiveHealth] [int] NULL,  
                --[IsRetireeHealth] [int] NULL,  
                --[IsPension] [int] NULL,  
                --[IsIAPHourly] [int] NULL, 
                --[OldEmployerNum] [varchar](6), 
                [CheckVesting] [varchar](1) 
)   
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)  
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
                --FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
                --Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
                SSN = convert(char(9),Hours.SSN),  
                --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
                --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
                --UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
                --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
                --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
                --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
                --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
                --NULL PersonId, --varchar(15) no change  
                --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
                --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
                --LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = Hours.UnionMisc, --New field. char(2)  
                --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
                                                                                                                                --It is required because for those records where we do not have any rate group info  
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
                --IAPHourlyRate = rgb.Individual  --New field. money  
                --, Source = 'C/S '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = e.OldEmployerNum 
                [CheckVesting] = case when ((Year(Report.RecDate) = @Year and Report.FromDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y')then 'Y' else '' end
                from OPUS_Participant_List list   
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
                inner join eadb.dbo.Report report on report.reportid = hours.reportid   
                --and hours.SSN = @SSN   
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
                --and report.ToDate between @FromDate and @ToDate               
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = HPTransactions.Ber,  
                --EmpAccountNo = convert(int, HPTransactions.Employer),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  
                --FromDate = convert(smalldatetime, HPTransactions.StartDate),  
                --ToDate = convert(smalldatetime, HPTransactions.EndDate),  
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),                 
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
                --Received = convert(smalldatetime, HPTransactions.DateReceived),  
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
                --Processed = convert(smalldatetime,hb.Updated),  
                --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
                SSN = convert(char(9),HPTransactions.SSN),  
                --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
                --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
                --UnionCode = convert(int,HPTransactions.UnionCode),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
                --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
                --IAPPercent = convert(money,HPTransactions.IAPDollars),  
                --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
                --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
                --HoursStatus = 0,  
                --LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = HPTransactions.UNMisc,  
                --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'H/P '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, e.OldEmployerNum  
                 [CheckVesting] = case when ((Year(HPTransactions.DateReceived) = @Year and HPTransactions.StartDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else '' end
from OPUS_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP              
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN  
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
                --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select      
                --ReportID = left(cpas.erractid,18),  
                --EmpAccountNo = convert(int, cpas.ERKey),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
                --FromDate = convert(smalldatetime, cpas.FDate),  
                --ToDate = convert(smalldatetime, cpas.TDate),  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
                --Received = convert(smalldatetime, cpas.PDate),  
                --Processed = convert(smalldatetime, cpas.PDate),  
                --HoursId = convert(varchar(24),cpas.erractid),  
                SSN = convert(char(9),cpas.MKey),  
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,cpas.LOC_NO),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
    --we are not checking rate item to identify hours for Pension, Health, or IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
    --IAPPercent = convert(money,cpas.PanOnEarn),  
    --MM 12/26/12  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),cpas.RateGroup),  
                --HoursStatus = 0, --all the hours comming from CPAS are processed.  
                --LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                --LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                ---------------------------------------------------------------------  
                --UnionMisc = null,  
                --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'CPAS'  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, E.OldEmployerNum  
                 [CheckVesting] = case when ((Year(cpas.PDate) = @Year and cpas.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else '' end
                from OPUS_Participant_List list   
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                where [Plan]=2  
                --and cpas.mkey = @SSN  
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
                --and cpas.TDate between @FromDate and @ToDate  
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
                --EmpAccountNo = convert(int, Pre.EmployerId),   
                ComputationYear = Pre.Plan_Year,  
                --FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
                --ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
                --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
                --Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
                --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
                SSN = convert(char(9),Pre.SSN),   
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,Pre.UnionCode),   
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
                                                                                                when [Local]='L666' then convert(smallint, 4)  
                                                                                                when [Local]='L700' then convert(smallint, 6)  
                                                                                                when [Local]='L52' then convert(smallint, 7)  
                                                                                                when [Local]='L161' then convert(smallint, 8)  
                                                                                                else null end,   
                --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
                --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
                IAPHours = convert(numeric(7, 1), 0),  
                --IAPHoursA2 = convert(numeric(7, 1), 0),   
                --IAPPercent = convert(money, 0),   
                --ActiveHealthHours = convert(numeric(7, 1), 0),   
                --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
                --NULL PersonId,  
                --RateGroup = Pre.RateGroup,--?  
                --HoursStatus = convert(int, 0),  
                --LateMonthly = '',   
                --LateAnnual = '' ,  
                -------------------------------------------------------------------  
                --UnionMisc = convert(char(2),''),  
                --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'PM  '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = Pre.EmployerId  
                [CheckVesting] = case when ((Year(Pre.MergeDate) = @Year and Pre.StartDate < list.[Vesting_Date])OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else '' end
from OPUS_Participant_List list   
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
                --Pre.EndDate between @FromDate and @ToDate  
  
--select * from [#PensionWorkHistory]  
--order by todate  
--select * from [#PensionWorkHistory]  
--order by todate 
--insert into PensionWorkHistoryForStmt   


UPDATE [#PensionWorkHistory] SET CheckVesting='Y' WHERE SSN IN (SELECT DISTINCT TE.SSN FROM [#PensionWorkHistory] TE WHERE TE.CheckVesting='Y')
  

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR, VPIO.SSN, VPIO.CheckVesting,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,  
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,  
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS IAP_HOURS  
FROM [#PensionWorkHistory] AS VPIO ORDER BY VPIO.SSN  


delete from [#PensionWorkHistory]

END
GO


-- =============================================
-- Author:        Rohan Adgaonkar
-- Create date:	  04/05/2013
-- Description:   Removed Refs of Pid
-- =============================================
CREATE PROCEDURE [dbo].[usp_PensionLateHours]
      @BatchRunDate smalldatetime
AS
DECLARE
@BEGINDATE smalldatetime,
@ENDDATE   smalldatetime
BEGIN
      SET NOCOUNT ON;
    
    SET @ENDDATE = (SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(@BatchRunDate)),@BatchRunDate),101))
    SET @BEGINDATE = (SELECT convert(varchar(25),DATEADD(dd,(-1) * (DATEPART(dd,@BatchRunDate) - 1) ,DATEADD(mm,-1,@BatchRunDate)),101))
      
      
         select  
                              LTRIM(RTRIM(P.MPI_PERSON_ID)) MPID,
                              P.SSN SSN,REPORT.PensionYear COMPUTATIONYEAR,
                              REPORT.FROMDATE PAYPERIODSTARTDATE,
                              REPORT.TODATE PAYPERIODENDDATE,
                              REPORT.PROCESSDATE PROCESSEDDATE,
                    HOURS.HOURSWORKED PENSIONHOURS,
                    IAPHours = case when report.rategroup = 8 then Hours.HoursWorked 
                                                      when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)
                                                      else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,
                              IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),
                              IAPPercent = Hours.IAPValue, EmpAccountNo,
                              REPORTSTATUS = case when report.todate < @BEGINDATE then 'L'
                                        when report.todate between @BEGINDATE and @ENDDATE then 'R'
                                   end     
                                                      
                  from  eadb.dbo.Report report
				  inner join eadb.dbo.hours hours on hours.reportid = report.reportid and hours.status = 0
                  inner join eadb.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate and rgc.ToPensionSystem = 1 and (rgc.Pension = 1 or rgc.IAP = 1)                
                  left outer join OPUS.dbo.SGT_PERSON p on hours.ssn = p.ssn 
                  where EmpAccountNo not in (14002,13363,3596,3597,12904)     and report.status = 0 and report.processdate between @BEGINDATE and @ENDDATE 
   
END
GO


-----------------------------------------------------------------------------------------------  
--Name - Rashmi Sheri.
--Date - 04/03/2013  
--Purpose - CREATE usp_GetWorkDataAfterRetirement in EADB  
-----------------------------------------------------------------------------------------------  

/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataAfterRetirement]    Script Date: 04/02/2013 23:49:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetWorkDataAfterRetirement](@SSN char(10),@RETIREMENT_DATE DateTime,@TO_DATE DateTime=NULL)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS ComputationYear, VPIO.FromDate AS FromDate, VPIO.ToDate ToDate, 
VPIO.PensionHours AS PensionHours,VPIO.IAPHours,VPIO.IAPHoursA2,VPIO.IAPPercent,VPIO.Weeks,VPIO.OldEmployerNum,EMPR.EmployerName,EMPR.Address1,EMPR.City,EMPR.Address2,EMPR.State,EMPR.Contact1,EMPR.PostalCode,EMPR.Contact2
,EMPRADR.Street
FROM @PensionWorkHistory AS VPIO INNER JOIN Employer EMPR ON VPIO.OldEmployerNum = EMPR.OldEmployerNum
LEFT OUTER join EmployerAddress EMPRADR ON EMPR.EmployerId=EMPRADR.EmployerId and EMPRADR.Type = 0 
where VPIO.SSN=@SSN AND  VPIO.ToDate>@RETIREMENT_DATE AND (VPIO.ToDate <=@TO_DATE OR @TO_DATE IS NULL)
END
GO



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 04/01/2013
-- Purpose - Updated SP usp_GetIAPSnapShotInfo
----------------------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[usp_GetIAPSnapShotInfo](@COMPUTATIONYEAR int) 
AS
BEGIN
SET NOCOUNT ON
--------------------------------------------------------------------------------------------

CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1) NULL,
	[LateAnnual] [varchar](1) NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 
insert into [#PensionWorkHistory]
exec usp_pensioninterface4opus_by_dates @COMPUTATIONYEAR,null,null

TRUNCATE TABLE OPUS.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT 

	INSERT INTO OPUS.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT
	SELECT * FROM
	(
		SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN,IAPHOURS , IAPHOURSA2, IAPPERCENT, 'N' AS LATE_FLAG,FromDate,ToDate,Weeks
		FROM [#PensionWorkHistory] 
		WHERE ComputationYear = @COMPUTATIONYEAR
		
		UNION ALL
		
		SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN, IAPHOURS, IAPHOURSA2, IAPPERCENT, 'Y' AS LATE_FLAG,FromDate,ToDate,Weeks
		FROM [#PensionWorkHistory]
		WHERE ComputationYear < @COMPUTATIONYEAR 
		AND (LateMonthly = 'Y' or LateAnnual = 'Y')
	) A
	ORDER BY SSN,COMPUTATIONYEAR DESC,EMPACCOUNTNO


drop table [#PensionWorkHistory]

END
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Abhishek Sharma
-- Date - 04/01/2013
-- Purpose - CREATE TABLE OPUS_AnnualStmt_Participant_List
----------------------------------------------------------------------------------------------------------------------------------

/****** Object:  Table [dbo].[OPUS_AnnualStmt_Participant_List]    Script Date: 03/29/2013 13:59:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[OPUS_AnnualStmt_Participant_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SSN] [varchar](9) NULL,
	[PensionYear] [int] NULL,
 CONSTRAINT [PK_OPUS_AnnualStmt_Participant_List] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



-----------------------------------------------------------------------------------------------
--Name - Abhishek Sharma
--Date - 01/04/2013
--Purpose - Added scripts for create table [OPUS_Participant_List]
-----------------------------------------------------------------------------------------------

/****** Object:  Table [dbo].[OPUS_Participant_List]    Script Date: 01/03/2013 14:32:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[OPUS_Participant_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SSN] [varchar](9) NULL,
	VESTING_DATE DATETIME NULL,
	RECALCULATE_VESTING VARCHAR(1) NULL
 CONSTRAINT [PK_OPUS_Participant_List] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


-----------------------------------------------------------------------------------------------  
--Name -Rohan Adgaonkar
--Date - 03/20/2013  
--Purpose - CREATE usp_GetIAPHourInfoForQuarterlyAllocation in EADB  
-----------------------------------------------------------------------------------------------  
CREATE PROC [dbo].[usp_GetIAPHourInfoForQuarterlyAllocation](@SSN char(10),@FROMDATE DATETIME,@TODATE DATETIME)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT PWH.ComputationYear as ComputationYear,PWH.EmpAccountNo as EmpAccountNo,(CASE WHEN SUM(PWH.IAPHours) IS NULL THEN 0 ELSE SUM(PWH.IAPHours) END) AS IAPHours,
       (CASE WHEN SUM(PWH.IAPHoursA2)IS NULL THEN 0 ELSE SUM(PWH.IAPHoursA2) END)AS IAPHoursA2,
       (CASE WHEN SUM(PWH.IAPPercent)IS NULL THEN 0 ELSE SUM(PWH.IAPPercent) END)AS IAPPercent,
       0.0 as lateiappercent
FROM @PensionWorkHistory PWH WHERE PWH.FromDate >= @FROMDATE AND PWH.ToDate <= @TODATE AND PWH.SSN=@SSN
GROUP BY PWH.ComputationYear,PWH.EmpAccountNo

END
GO


-----------------------------------------------------------------------------------------------  
--Name - Abhishek Sharma
--Date - 03/18/2013  
--Purpose - CREATE usp_GetVestingDate1HourAftGivenDate in EADB  
-----------------------------------------------------------------------------------------------  

/****** Object:  StoredProcedure [dbo].[usp_GetVestingDate1HourAftGivenDate]    Script Date: 03/18/2013 11:09:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetVestingDate1HourAftGivenDate](@SSN char(10),@PlanCode varchar(20),@GivenDate DateTime)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

DECLARE @COUNT int
DECLARE @FLAG INT
DECLARE @TABLECOUNT int
Declare @IAPHOURS int
declare @PENSIONHOURS int
DECLARE @Temp TABLE
(
	[SEQ_NO] int IDENTITY(1,1) NOT NULL, 
	IAPHours numeric(18,1),
	PensionHours numeric(18,1),
	FromDate smalldatetime,
	ToDate smalldatetime	
)

if @PlanCode = 'IAP'
begin
INSERT INTO @Temp
SELECT IAPHours,0,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and cast(FromDate AS datetime) >= @GivenDate order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @IAPHOURS=0
SET @FLAG =0 


IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @IAPHOURS = @IAPHOURS +  (select case when temp.IAPHours is null then 0 else temp.IAPHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @IAPHOURS >= 1.0 
begin 
select CAST(temp.ToDate AS DATETIME) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG =1
break  
end
set @COUNT = @COUNT + 1
end

IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
ELSE
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
end
-----For MPIPP
ELSE
BEGIN
INSERT INTO @Temp
SELECT 0,PensionHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and cast(FromDate AS datetime) > @GivenDate and PensionPlan=2 order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @PENSIONHOURS=0
SET @FLAG =0 


IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @PENSIONHOURS = @PENSIONHOURS +  (select case when temp.PensionHours is null then 0 else temp.PensionHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @PENSIONHOURS >= 1.0 
begin 
select CAST(temp.ToDate AS DATETIME) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG =1
break  
end
set @COUNT = @COUNT + 1
end

IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
ELSE
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
END
GO

--------------------------------------------------------------------------------------------------------------
--Name - Wasim Pathan
--Date - 03/04/2013
--Purpose - Modifying [usp_GetOccupationBasebOnUnion]
--------------------------------------------------------------------------------------------------------------
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetOccupationBasebOnUnion](@SSN char(9))
AS
BEGIN
SET NOCOUNT ON
SELECT MAX(r.ToDate), u.Name, u.UnionCode FROM dbo.Hours h WITH (NOLOCK)
inner join EADB.dbo.report r WITH (NOLOCK) on r.reportid = h.reportid
LEFT OUTER JOIN dbo.Unions u WITH (NOLOCK) on u.UnionCode = h.UnionCode
WHERE SSN = @SSN 
GROUP BY u.Name, u.UnionCode
END
GO


-----------------------------------------------------------------------------------------------
--Name - Abhishek Sharma
--Date - 01/17/2013
--Purpose - create proc [usp_GetHealthWorkData]
-----------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetHealthWorkData]    Script Date: 01/17/2013 17:57:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetHealthWorkData](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT ComputationYear AS YEAR,SUM(RetireeHealthHours) AS idecTotalHealthHours, SUM(CASE WHEN PENSIONPLAN = 2 THEN PensionHours ELSE 0.0 END) AS  idcPensionHours_healthBatch,
SUM(IAPHours) AS idcIAPHours_healthBatch
FROM @PensionWorkHistory
GROUP BY ComputationYear
ORDER BY ComputationYear
END
GO




--=============================================================================
--Modified By: Mahua Banerjee
--Modified On: 12/18/2012
--Purpose: Added Union code in select query
--=============================================================================
/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataForSSNMerge]    Script Date: 12/18/2012 14:38:54 ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetWorkDataForSSNMerge](@SSN char(10)) 
AS
BEGIN
SET NOCOUNT ON;

   DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT top(1) VPIO.ComputationYear AS ComputationYear, VPIO.FromDate AS FromDate, VPIO.ToDate ToDate, 
VPIO.PensionHours AS PensionHours,VPIO.IAPHours,VPIO.IAPHoursA2,VPIO.IAPPercent,VPIO.Weeks,VPIO.OldEmployerNum,
EMPR.EmployerName,EMPR.Address1,EMPR.City,EMPR.Address2,
EMPR.State,EMPR.Contact1,EMPR.PostalCode,EMPR.Contact2,VPIO.UnionCode
FROM @PensionWorkHistory AS VPIO left JOIN Employer EMPR ON VPIO.OldEmployerNum = EMPR.OldEmployerNum
where VPIO.SSN=@SSN and EMPR.EmployerName is not null and VPIO.ToDate is not null
order by VPIO.ToDate desc
END
GO



-- ==========================================================================
-- CREATED By:	Rohan Adgaonkar
-- Modified Date: 11/30/2012
-- Description:	Created stored procedure usp_HoursAfterRetirement
-- ==========================================================================

/****** Object:  StoredProcedure [dbo].[usp_HoursAfterRetirement]    Script Date: 04/11/2012 12:03:06 ******/
CREATE PROC [dbo].[usp_HoursAfterRetirement](@SSN char(10),@RETIREMENT_DATE datetime)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT ComputationYear AS ComputationYear, FromDate AS FromDate, ToDate ToDate, Processed AS ProcessedDate,
PensionHours AS PensionHours,IAPHours,IAPHoursA2,IAPPercent,Weeks,PensionPlan AS PensionPlan,OldEmployerNum FROM @PensionWorkHistory 
where SSN=@SSN and cast(Processed as datetime) > @RETIREMENT_DATE 
and (LateMonthly='Y' or LateAnnual='Y')
END
GO


-- ==========================================================================
-- CREATED By:	Rohan Adgaonkar
-- Modified Date: 11/30/2012
-- Description:	Created stored procedure usp_GetHoursProcessedPreviousDay
-- ==========================================================================

CREATE PROC dbo.usp_GetHoursProcessedPreviousDay
as begin
select h.SSN,r.fromdate,r.todate,h.hoursworked,convert(varchar(12),r.processdate,101)Processdate 
from EADB.dbo.hours h 
inner join EADB.dbo.report r on r.reportid = h.reportid and r.status = 0 and 
convert(varchar(12),r.processdate,101) = convert(varchar(12),getdate()-1,101)
end
GO


-- =============================================================
-- Author:		Rohan Adgaonkar
-- Create date: 08/21/2012
-- Update date: 10/05/2012
-- Description:	Get WorkData Between Two Dates
-- =============================================================

CREATE PROC [dbo].[usp_GetWorkDataBetweenTwoDates](@SSN char(10),@PLANCODE varchar(20),@FROM_DATE datetime,@TO_DATE datetime
,@PROCESSED_FROM_DATE DATETIME = null,@PROCESSED_TO_DATE DATETIME = null)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 


INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN


IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.FromDate >= @FROM_DATE and VPIO.ToDate <= @TO_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.FromDate >= @FROM_DATE and VPIO.ToDate <= @TO_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and FromDate >= @FROM_DATE and ToDate <= @TO_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and FromDate >= @FROM_DATE and ToDate <= @TO_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.FromDate >= @FROM_DATE and VPIO.ToDate <= @TO_DATE ORDER BY YEAR
END

IF @PROCESSED_FROM_DATE IS NOT NULL AND @PROCESSED_TO_DATE IS NOT NULL AND
 EXISTS (SELECT * FROM @PensionWorkHistory WHERE LateAnnual = 'Y' AND Processed BETWEEN @PROCESSED_FROM_DATE AND @PROCESSED_TO_DATE)
RETURN 1
ELSE
RETURN 0

END
GO



-- =============================================================
-- Author:		Rohan Adgaonkar
-- Create date: 08/21/2012
-- Update date: 09/04/2012
-- Description:	Get Previous Months LateHours And Contributions
-- =============================================================
CREATE PROC [dbo].[Get_PreviousMonths_LateHours_And_Contributions] (@BATCH_RUN_DATE DATETIME)
AS
BEGIN
SET NOCOUNT ON

DECLARE @PENSION_LATE_HOURS TABLE
(
	[SEQ_NO] INT IDENTITY(1,1) NOT NULL, 
	MPID NVARCHAR(20),
	SSN FLOAT,
	PAY_PERIOD_START_DATE SMALLDATETIME,
	PAY_PERIOD_END_DATE SMALLDATETIME,
	PROCESSED_DATE SMALLDATETIME,
	PENSION_HOURS FLOAT,
	IAPHOURS FLOAT,
	IAPHOURSA2 FLOAT,
	IAPPERCENT FLOAT,
	REPORT_STATUS CHAR(1)
)

DECLARE @ENDDATE DATETIME
DECLARE @BEGINDATE DATETIME

SET @ENDDATE = (SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(@BATCH_RUN_DATE)),@BATCH_RUN_DATE),101))
SET @BEGINDATE = (SELECT CONVERT(varchar(25),DATEADD(dd,(-1) * (DATEPART(dd,@BATCH_RUN_DATE) - 1) ,DATEADD(mm,-1,@BATCH_RUN_DATE)),101))

SELECT DISTINCT 
		SP.PERSON_ID ,SP.MPI_PERSON_ID, 0 as UVHP_AMOUNT,0 as COMPUTATIONAL_YEAR,SPA.PAYEE_ACCOUNT_ID,SBCH.RETIREMENT_DATE,SBCH.MODIFIED_DATE, REPORTSTATUS = 'L'
FROM 
		SGT_PERSON SP 
		INNER JOIN SGT_BENEFIT_CALCULATION_HEADER SBCH ON SP.PERSON_ID = SBCH.PERSON_ID
		INNER JOIN SGT_BENEFIT_CALCULATION_DETAIL SBCD ON SBCH.BENEFIT_CALCULATION_HEADER_ID = SBCD.BENEFIT_CALCULATION_HEADER_ID
		INNER JOIN SGT_PAYEE_ACCOUNT SPA ON SPA.BENEFIT_CALCULATION_DETAIL_ID = SBCD.BENEFIT_CALCULATION_DETAIL_ID
		INNER JOIN SGT_PAYEE_ACCOUNT_STATUS SPAS ON SPA.PAYEE_ACCOUNT_ID = SPAS.PAYEE_ACCOUNT_ID
		AND SPAS.STATUS_ID =(SELECT TOP 1 Q.STATUS_ID FROM SGT_PAYEE_ACCOUNT_STATUS Q WHERE Q.PAYEE_ACCOUNT_ID = SPA.PAYEE_ACCOUNT_ID ORDER BY Q.MODIFIED_DATE DESC)
		
WHERE 
	((SBCD.PLAN_ID = 1 AND SPAS.STATUS_VALUE IN ('APRD','REVW','CMPL')) OR (SBCD.PLAN_ID <> 1 AND SPAS.STATUS_VALUE NOT IN ('CNCL')))
	AND SBCH.STATUS_VALUE = 'APPR'
		
UNION

SELECT DISTINCT
	  SPA.PERSON_ID,SP.MPI_PERSON_ID, SUM(UVHP_AMOUNT) AS UVHP_AMOUNT, cast(SPARC.COMPUTATIONAL_YEAR as int), PAYEE_ACCOUNT_ID,NULL AS RETIREMENT_DATE,NULL AS MODIFIED_DATE, REPORTSTATUS = 'R'
FROM
	 SGT_PERSON_ACCOUNT_RETIREMENT_CONTRIBUTION SPARC
	 join SGT_PERSON_ACCOUNT SPA ON SPA.PERSON_ACCOUNT_ID = SPARC.PERSON_ACCOUNT_ID
	 join SGT_PERSON SP ON SPA.PERSON_ID = SP.PERSON_ID
	 join SGT_BENEFIT_APPLICATION SBA on SBA.PERSON_ID = SP.PERSON_ID AND SBA.BENEFIT_TYPE_VALUE = 'RTMT'
	  LEFT JOIN SGT_PAYEE_ACCOUNT SPAE ON SPAE.PERSON_ID = SP.PERSON_ID AND SPAE.BENEFIT_ACCOUNT_TYPE_VALUE='WDRL'
WHERE
	SPARC.TRANSACTION_DATE >=  @BEGINDATE AND 
	       SPARC.TRANSACTION_DATE <= @ENDDATE 
	        AND exists (select * from SGT_BENEFIT_APPLICATION SBA where SBA.PERSON_ID = SP.PERSON_ID AND SBA.BENEFIT_TYPE_VALUE = 'RTMT' and SBA.APPLICATION_STATUS_VALUE = 'APPR'
	        and SPARC.EFFECTIVE_DATE <= SBA.RETIREMENT_DATE)
	       group by COMPUTATIONAL_YEAR, SPAE.PAYEE_ACCOUNT_ID,SP.MPI_PERSON_ID, SPA.PERSON_ID having SUM(SPARC.UVHP_AMOUNT)!= 0
	       
END
GO



------------------------------------------------------------------------------------------------------------------------
--Created By	:	Rohan Adgaonkar
--Created On	:	14th June 2012
--Description	:	Create Stored Procedure usp_GetQDROHoursTillDate
------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetQDROHoursTillDate]    Script Date: 06/14/2012 12:59:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetQDROHoursTillDate] (@SSN char(10),@PLAN_CODE VARCHAR(10),@DATE_OF_DETERMINATION DATETIME,@DENOMINATOR NUMERIC(7,1) OUTPUT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6),
	[SEQ] [int] IDENTITY(1,1) NOT NULL
) 

DECLARE @COUNT INT
DECLARE @TOTAL_COUNT INT


DECLARE @YEARS INT
DECLARE @COUNT_YEARLY INT
DECLARE @TOTAL_COUNT_YEARLY INT

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SET @TOTAL_COUNT = @@ROWCOUNT

SET @YEARS= 0
SET @DENOMINATOR=0.0

DECLARE @PensionWorkHistoryYearWise TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
) 

INSERT INTO @PensionWorkHistoryYearWise
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
0
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY = @@ROWCOUNT
SET @COUNT_YEARLY = 1

WHILE @COUNT_YEARLY <= @TOTAL_COUNT_YEARLY
BEGIN	
	IF (SELECT PWHY.QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ = @COUNT_YEARLY ) >= 400.0
	BEGIN
		SET @YEARS = @YEARS + 1
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	ELSE
	BEGIN
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	
	SET @COUNT_YEARLY = @COUNT_YEARLY + 1
END


-----------------------------------------------------------------------------------------------------------
--FOR DENOMINATOR
DECLARE @COUNT_YEARLY_FOR_DENOMINATOR INT
DECLARE @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR INT


DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) > @DATE_OF_DETERMINATION

DECLARE @PensionWorkHistoryYearDenominator TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
)


INSERT INTO @PensionWorkHistoryYearDenominator
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
(select QualifiedYearCount from @PensionWorkHistoryYearWise PWH where PWH.ComputationYear=VPIO.ComputationYear)
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR =@@ROWCOUNT
SET @COUNT_YEARLY_FOR_DENOMINATOR =1



PRINT @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR

	
	
	--FOR DENOMINATOR
	IF @PLAN_CODE = 'IAP'
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	ELSE
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
			IF (SELECT PWHYD.QualifiedYearCount FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR) < 20
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
				ELSE
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	
END
GO



------------------------------------------------------------------------------------------------------------------------
--Created By	:	Rohan Adgaonkar
--Created On	:	30th May 2012
--Description	:	Create Stored Procedure usp_GetHoursInformationForQDRO
------------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[usp_GetHoursInformationForQDRO] (@SSN char(10),@PLAN_CODE VARCHAR(10),@DATE_OF_MARRIAGE DATETIME,
@DATE_OF_SEPARATION DATETIME,@DATE_OF_DETERMINATION DATETIME,
@NUMERATOR NUMERIC(7,1) OUTPUT,@DENOMINATOR NUMERIC(7,1) OUTPUT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6),
	[SEQ] [int] IDENTITY(1,1) NOT NULL
) 

DECLARE @COUNT INT
DECLARE @TOTAL_COUNT INT


DECLARE @YEARS INT
DECLARE @COUNT_YEARLY INT
DECLARE @TOTAL_COUNT_YEARLY INT

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SET @TOTAL_COUNT = @@ROWCOUNT

SET @YEARS= 0
SET @NUMERATOR=0.0
SET @DENOMINATOR=0.0

DECLARE @PensionWorkHistoryYearWise TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
) 

INSERT INTO @PensionWorkHistoryYearWise
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
0
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY = @@ROWCOUNT
SET @COUNT_YEARLY = 1

WHILE @COUNT_YEARLY <= @TOTAL_COUNT_YEARLY
BEGIN	
	IF (SELECT PWHY.QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ = @COUNT_YEARLY ) >= 400.0
	BEGIN
		SET @YEARS = @YEARS + 1
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	ELSE
	BEGIN
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	
	--IF (SELECT QualifiedYearCount FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ=@COUNT_YEARLY) < 20
	--BEGIN
	--	SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.QualifiedHours >= 400.0 
	--	AND PWHY.YearlySEQ=@COUNT_YEARLY 
	--END
	--ELSE
	--BEGIN
	--	SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ=@COUNT_YEARLY 
	--END
	SET @COUNT_YEARLY = @COUNT_YEARLY + 1
END


-----------------------------------------------------------------------------------------------------------
--FOR DENOMINATOR
DECLARE @COUNT_YEARLY_FOR_DENOMINATOR INT
DECLARE @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR INT


DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) > @DATE_OF_DETERMINATION

DECLARE @PensionWorkHistoryYearDenominator TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
)


INSERT INTO @PensionWorkHistoryYearDenominator
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
(select QualifiedYearCount from @PensionWorkHistoryYearWise PWH where PWH.ComputationYear=VPIO.ComputationYear)
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR =@@ROWCOUNT
SET @COUNT_YEARLY_FOR_DENOMINATOR =1

--UPDATE @PensionWorkHistoryYearDenominator SET QualifiedYearCount = 0 WHERE QualifiedHours < 400.0

SELECT * FROM @PensionWorkHistoryYearDenominator
PRINT @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR


-----------------------------------------------------------------------------------------------------------


------------------------------------------------------------------------------------------------------------------
--FOR NUMERATOR

DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) < @DATE_OF_MARRIAGE OR 
CAST(todate AS DATETIME) > @DATE_OF_SEPARATION



DECLARE @PensionWorkHistoryYearWiseFinal TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
)

INSERT INTO @PensionWorkHistoryYearWiseFinal
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
(select QualifiedYearCount from @PensionWorkHistoryYearWise PWH where PWH.ComputationYear=VPIO.ComputationYear)
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
SET @TOTAL_COUNT_YEARLY = @@ROWCOUNT

--UPDATE @PensionWorkHistoryYearWiseFinal SET QualifiedYearCount = 0 WHERE QualifiedHours < 400.0

---------------------------------------------------------------------------------------------------------------------------------------------------


	--FOR NUMERATOR
	SET @COUNT_YEARLY =1 
	WHILE @COUNT_YEARLY <= @TOTAL_COUNT_YEARLY 
	BEGIN
		IF (SELECT QualifiedYearCount FROM @PensionWorkHistoryYearWiseFinal PWHYF WHERE PWHYF.YearlySEQ=@COUNT_YEARLY) < 20
			BEGIN
				SELECT  @NUMERATOR = @NUMERATOR +  QualifiedHours FROM @PensionWorkHistoryYearWiseFinal PWHYF WHERE PWHYF.QualifiedHours >= 400.0 
				AND PWHYF.YearlySEQ=@COUNT_YEARLY 
			END
			ELSE
			BEGIN
				SELECT  @NUMERATOR = @NUMERATOR +  QualifiedHours FROM @PensionWorkHistoryYearWiseFinal PWHYF WHERE PWHYF.YearlySEQ=@COUNT_YEARLY 
			END
				SET @COUNT_YEARLY = @COUNT_YEARLY + 1
	END
	
	
	
	--FOR DENOMINATOR
	WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
	BEGIN
		IF (SELECT PWHYD.QualifiedYearCount FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR) < 20
			BEGIN
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
				AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
			END
			ELSE
			BEGIN
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
			END
				SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
	END
	
END

GO



------------------------------------------------------------------------------------------------------------------------
--Created By	:	Rohan Adgaonkar
--Created On	:	18th May 2012
--Description	:	Create Stored Procedure usp_GetWorkDataTillGivenDate
------------------------------------------------------------------------------------------------------------------------



/****** Object:  StoredProcedure [dbo].[usp_GetCommunityServiceHours]    Script Date: 05/18/2012 21:04:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetCommunityServiceHours](@SSN char(10),@PLANCODE varchar(20),@DATE_OF_MARRIAGE DATETIME,
@END_OF_DATE_OF_MARRIAGE_COMP_YEAR DATETIME,@DATE_OF_SEPARATION DATETIME,@START_OF_DATE_OF_SEPARATION_COMP_YEAR DATETIME)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='MPIPP' THEN

(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2 
		and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2 
		and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)END)

WHEN @PLANCODE='IAP' THEN

(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear 
		and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear 
		and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear)END)


WHEN @PLANCODE='Local600' THEN
(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 
		and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 
		and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)END)

WHEN @PLANCODE='Local666' THEN
(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4
		 and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4
		 and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)END)


WHEN @PLANCODE='Local700' THEN
(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 
		and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6
		 and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)END)


WHEN @PLANCODE='Local52' THEN
(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7
		 and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7
		 and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)END)


WHEN @PLANCODE='Local161' THEN
(CASE WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_MARRIAGE) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 
		and FromDate >= @DATE_OF_MARRIAGE and ToDate <= @END_OF_DATE_OF_MARRIAGE_COMP_YEAR)
WHEN cast(VPIO.ComputationYear as int) = DATEPART(YEAR,@DATE_OF_SEPARATION) THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8
		 and ToDate <= @DATE_OF_SEPARATION and FromDate >= @START_OF_DATE_OF_SEPARATION_COMP_YEAR)
ELSE
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)END)


ELSE NULL END AS QDRO_HOURS
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.FromDate >= @DATE_OF_MARRIAGE
AND VPIO.ToDate <=@DATE_OF_SEPARATION ORDER BY YEAR
END

GO




----------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetVestingDateAft400Hours]    Script Date: 05/11/2012 15:01:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetVestingDateAft400Hours](@SSN char(10),@PLANCODE varchar(20),@YEAR int)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

DECLARE @COUNT int
DECLARE @FLAG INT
DECLARE @TABLECOUNT int
Declare @WorkingHours int
DECLARE @Temp TABLE
(
	[SEQ_NO] int IDENTITY(1,1) NOT NULL, 
	WorkingHours numeric(18,1),
	FromDate smalldatetime,
	ToDate smalldatetime
)

IF @PLANCODE='MPIPP' 
 BEGIN


INSERT INTO @Temp
SELECT PensionHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and ComputationYear=@YEAR AND 
	(PensionPlan = 2 OR PensionPlan =3 OR PensionPlan =4 OR PensionPlan =6 OR PensionPlan =7 OR PensionPlan =8) order by FromDate

SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0


IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 400.0 
BEGIN 
select CAST(temp.ToDate AS DATETIME) AS VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG =1
break  
end
set @COUNT = @COUNT + 1
end
IF @FLAG =0
BEGIN
select NULL AS VESTING_DATE from @Temp temp
END
END
ELSE 
BEGIN
select NULL AS VESTING_DATE from @Temp temp
END
END

ELSE IF @PLANCODE='IAP'
BEGIN

INSERT INTO @Temp
SELECT IAPHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and ComputationYear =@YEAR order by FromDate asc
--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG =0

IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 400.0 
begin 
select CAST(temp.ToDate AS DATETIME) AS VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG=1
break  
end
set @COUNT = @COUNT + 1
end

IF @FLAG =0
BEGIN
select NULL AS VESTING_DATE from @Temp temp
END
END
ELSE 
BEGIN
select NULL AS VESTING_DATE from @Temp temp
END

END

END

GO



---------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetVestingDate40HoursNonAffliate]    Script Date: 05/11/2012 15:00:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetVestingDate40HoursNonAffliate](@SSN char(10),@PLANCODE varchar(20))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

DECLARE @COUNT int
DECLARE @FLAG INT
DECLARE @TABLECOUNT int
Declare @WorkingHours int
DECLARE @Temp TABLE
(
	[SEQ_NO] int IDENTITY(1,1) NOT NULL, 
	WorkingHours numeric(18,1),
	FromDate smalldatetime,
	ToDate smalldatetime
)

IF @PLANCODE='MPIPP' 
 BEGIN

INSERT INTO @Temp
SELECT PensionHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN 
 and cast(FromDate AS datetime) >= '12/23/1989' and UnionCode in (09,59,79,89,99) and PensionPlan=2 order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0


IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 40.0 
begin 
select CAST(temp.ToDate AS DATETIME) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG =1
break  
end
set @COUNT = @COUNT + 1
end

IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
ELSE 
BEGIN
select NULL as VESTING_DATE from @Temp temp
END

END

ELSE IF @PLANCODE='IAP'
BEGIN

INSERT INTO @Temp
SELECT IAPHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and cast(FromDate AS datetime) >='12/23/1989' 
and UnionCode in (09,59,79,89,99) order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0

IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 40.0 
begin 
select cast(temp.ToDate as datetime) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG =1
break  
end
set @COUNT = @COUNT + 1
end
IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp 
END
END
ELSE 
BEGIN
select NULL as VESTING_DATE from @Temp temp
END

END

END
GO


------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetVestingDate40HoursInGivenYear]    Script Date: 05/11/2012 14:55:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetVestingDate40HoursInGivenYear](@SSN char(10),@PLANCODE varchar(20),@YEAR int)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

DECLARE @COUNT int
DECLARE @FLAG INT
DECLARE @TABLECOUNT int
Declare @WorkingHours int
DECLARE @Temp TABLE
(
	[SEQ_NO] int IDENTITY(1,1) NOT NULL, 
	WorkingHours numeric(18,1),
	FromDate smalldatetime,
	ToDate smalldatetime
)

IF @PLANCODE='MPIPP' 
 BEGIN

INSERT INTO @Temp
SELECT PensionHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN
 and ComputationYear=@Year and PensionPlan=2 order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0

IF @TABLECOUNT > 0
BEGIN
	WHILE (@COUNT <= @TABLECOUNT)
	Begin
		set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
		if @WorkingHours >= 40.0 
		begin 
			select cast(temp.ToDate as datetime) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
			SET @FLAG=1
			break  
		end
		set @COUNT = @COUNT + 1
	end
	
	IF @FLAG =0
	BEGIN
		select NULL as VESTING_DATE from @Temp temp
	END
END
ELSE
BEGIN
	select NULL as VESTING_DATE from @Temp temp
END
END

ELSE IF @PLANCODE='IAP'
BEGIN

INSERT INTO @Temp
SELECT IAPHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and ComputationYear= @Year order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0

IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 40.0 
begin 
select cast(temp.ToDate as datetime) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG=1
break  
end
set @COUNT = @COUNT + 1
end
IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
ELSE
BEGIN
select NULL as VESTING_DATE from @Temp temp
END

END
END
GO


-------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetVestingDate40HoursAftDateOfBirth]    Script Date: 05/11/2012 14:52:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetVestingDate40HoursAftDateOfBirth](@SSN char(10),@PLANCODE varchar(20),@YEAR int,@DATE_AT_AGE_65 datetime)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

DECLARE @COUNT int
DECLARE @FLAG INT
DECLARE @TABLECOUNT int
Declare @WorkingHours int
DECLARE @Temp TABLE
(
	[SEQ_NO] int IDENTITY(1,1) NOT NULL, 
	WorkingHours numeric(18,1),
	FromDate smalldatetime,
	ToDate smalldatetime
)

IF @PLANCODE='MPIPP' 
 BEGIN

INSERT INTO @Temp
SELECT PensionHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN
 and ComputationYear=@Year and cast(FromDate AS datetime) >= @DATE_AT_AGE_65 AND PensionPlan=2 order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0

IF @TABLECOUNT > 0
BEGIN
	WHILE (@COUNT <= @TABLECOUNT)
	Begin
		set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
		if @WorkingHours >= 40.0 
		begin 
			select CAST(temp.ToDate AS DATETIME) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
			SET @FLAG=1
			break  
		end
		set @COUNT = @COUNT + 1
	end
	
	IF @FLAG =0
	BEGIN
		select NULL as VESTING_DATE from @Temp temp
	END
END
ELSE
BEGIN
	select NULL as VESTING_DATE from @Temp temp
END
END

ELSE IF @PLANCODE='IAP'
BEGIN

INSERT INTO @Temp
SELECT IAPHours,FromDate,ToDate FROM @PensionWorkHistory where SSN=@SSN and ComputationYear= @Year and cast(FromDate AS datetime) >= @DATE_AT_AGE_65  order by FromDate asc

--select * from @Temp
SET @TABLECOUNT = @@ROWCOUNT
SET @COUNT = 1
SET @WorkingHours=0
SET @FLAG=0

IF @TABLECOUNT > 0
BEGIN
WHILE (@COUNT <= @TABLECOUNT)
Begin
set @WorkingHours = @WorkingHours +  (select case when temp.WorkingHours is null then 0 else temp.WorkingHours end from @Temp temp where temp.SEQ_NO=@COUNT)
if @WorkingHours >= 40.0 
begin 
select CAST(temp.ToDate AS DATETIME) as VESTING_DATE from @Temp temp where temp.SEQ_NO=@COUNT 
SET @FLAG=1
break  
end
set @COUNT = @COUNT + 1
end
IF @FLAG =0
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
ELSE
BEGIN
select NULL as VESTING_DATE from @Temp temp
END
END
END
GO



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Kunal Arora
-- Date - 05/10/2012
-- Purpose - create Last Working Date Script
----------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_CheckWorkingHoursAfterGivenDate]    Script Date: 05/10/2012 16:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_CheckWorkingHoursAfterGivenDate](@SSN char(10),@DATE DATETIME,@RESULT CHAR(1) OUTPUT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT @RESULT = CASE WHEN COUNT(*) > 0 THEN 'Y' ELSE 'N' END
from @PensionWorkHistory WHERE PensionHours > 0.0 AND ToDate > @DATE AND SSN=@SSN

END
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - vinovin
-- Date - 04/27/2012
-- Purpose - CREATE SP usp_GetWorkhistoryForIAPAllocation
----------------------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[usp_GetWorkhistoryForIAPAllocation](@SSN char(10),@FROMDATE DATETIME, @COMPUTATIONYEAR INT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT EMPACCOUNTNO,SSN,COMPUTATIONYEAR,SUM(IAPHOURS) IAPHOURS,SUM(IAPHOURSA2) IAPHOURSA2,SUM(IAPPERCENT) IAPPERCENT
FROM @PensionWorkHistory
WHERE ComputationYear < @COMPUTATIONYEAR
AND Processed < @FROMDATE
AND PensionPlan = 2
GROUP BY EMPACCOUNTNO,SSN,COMPUTATIONYEAR
END
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 04/26/2012
-- Purpose - CREATE SPs usp_GetLastWorkingDate 
----------------------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[usp_GetLastWorkingDate](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT Top 1 VPIO.ToDate AS LAST_WORKING_DATE
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN and VPIO.PensionHours > 0  ORDER BY VPIO.ToDate DESC
END
GO


------------------------------------------------------------------------------------------------------------------------
--Created By	:	Puneet Punjabi
--Created On	:	04/12/2012
--Description	:	Stored Proc for Calculation
------------------------------------------------------------------------------------------------------------------------ 

/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataForPlanYear]    Script Date: 04/25/2012 18:05:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetWorkDataForPlanYear](@SSN char(10),@PLANYEAR int, @PLANID int)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS ComputationYear, VPIO.FromDate AS FromDate, VPIO.ToDate ToDate, 
VPIO.Weeks AS Weeks, VPIO.PensionHours AS PensionHours
FROM @PensionWorkHistory AS VPIO 
where VPIO.SSN=@SSN AND VPIO.ComputationYear = @PLANYEAR AND VPIO.PensionPlan = @PLANID
END

GO



/****** Object:  StoredProcedure [dbo].[usp_GetCountForLateHours]    Script Date: 04/25/2012 18:17:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetCountForLateHours](@SSN char(10),@EVALUATION_DATE datetime,@VESTING_DATE datetime)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT COUNT(*) as Count_Late_Hours FROM @PensionWorkHistory 
where SSN=@SSN and cast(Received as datetime) > @EVALUATION_DATE 
and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')
END
GO


------------------------------------------------------------------------------------------------------------------------
--Created By	:	Puneet Punjabi
--Created On	:	04/11/2012
--Description	:	Stored Procedures for Eligibility and Calculations
------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_CheckHoursReportedInGivenInterval]    Script Date: 04/11/2012 12:02:24 ******/
CREATE PROC [dbo].[usp_CheckHoursReportedInGivenInterval] (@SSN char(10),@START_DATE datetime,@END_DATE datetime)
AS
DECLARE @RESULT INT
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SET @RESULT=0

select @RESULT = COUNT(*) FROM @PensionWorkHistory WHERE ((PensionPlan=2 AND PensionHours > 0) OR  (IAPHours  > 0)) AND
((CAST(FromDate as datetime) BETWEEN @START_DATE AND @END_DATE OR CAST(ToDate as datetime) BETWEEN @START_DATE AND @END_DATE))

RETURN @RESULT
END

GO



-----------------------------------------------------------------------------------------------
--Name - Abhishek	
--Date - 18th April 2013
--Purpose - Stored Procedure CHanges to Get the Plan Start Date too as a new COLUMN - required for Vesting and MD and Late CAlcs
-----------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[usp_GetWorkHistoryForAllMpippParticipant]    Script Date: 04/18/2013 11:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetWorkHistoryForAllMpippParticipant] (@Year int)
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
                [SSN] [varchar](9) NULL ,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                         
                )   
  
INSERT INTO @temp    
EXEC OPUS.dbo.[GET_ALL_PARTICIPANT_SSN]  
  
delete from OPUS_Participant_List   
  
INSERT INTO OPUS_Participant_List  
select * from  @temp   
  
                                  
CREATE TABLE [#PensionWorkHistory](  
                --[ReportID] [varchar](18) NULL,  
                --[EmpAccountNo] [int] NULL,  
                [ComputationYear] [smallint] NULL,  
                [FromDate] [smalldatetime] NULL,  
                --[ToDate] [smalldatetime] NULL,  
                --[Weeks] [char](2) NULL,  
                --[Received] [smalldatetime] NULL,  
                --[Processed] [smalldatetime] NULL,  
                --[HoursID] [varchar](24) NULL,  
                [SSN] [char](9) NULL,  
                --[LastName] [varchar](50) NULL,  
                --[FirstName] [varchar](50) NULL,  
                --[UnionCode] [int] NULL,  
                [PensionPlan] [smallint] NULL,  
                --[PensionCredit] [numeric](7, 3) NULL,  
                --[L52VestedCredit] [smallint] NULL,  
                [PensionHours] [numeric](7, 1) NULL,  
                [IAPHours] [numeric](7, 1) NULL,  
                --[IAPHoursA2] [numeric](7, 1) NULL,  
                --[IAPPercent] [money] NULL,  
                --[ActiveHealthHours] [numeric](7, 1) NULL,  
                --[RetireeHealthHours] [numeric](7, 1) NULL,  
                --[PersonId] [varchar](15) NULL,  
                --[RateGroup] [varchar](4) NULL,  
                --[HoursStatus] [int] NULL,  
                --[LateMonthly] [varchar](1) NOT NULL,  
                --[LateAnnual] [varchar](1) NOT NULL,  
                --[UnionMisc] [char](2) NULL,  
                --[HoursWorked] [numeric](7, 1) NULL,  
                --[IAPHourlyRate] [money] NULL,  
                --[Source] [varchar](4) NOT NULL,  
                --[ToHealthSystem] [int] NULL,  
                --[ToPensionSystem] [int] NULL,  
                --[IsActiveHealth] [int] NULL,  
                --[IsRetireeHealth] [int] NULL,  
                --[IsPension] [int] NULL,  
                --[IsIAPHourly] [int] NULL, 
                --[OldEmployerNum] [varchar](6), 
                [CheckVesting] [varchar](1) 
)   
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)  
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
                --Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
                SSN = convert(char(9),Hours.SSN),  
                --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
                --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
                --UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
                --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
                --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
                --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
                --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
                --NULL PersonId, --varchar(15) no change  
                --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
                --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
                --LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = Hours.UnionMisc, --New field. char(2)  
                --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
                                                                                                                                --It is required because for those records where we do not have any rate group info  
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
                --IAPHourlyRate = rgb.Individual  --New field. money  
                --, Source = 'C/S '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = e.OldEmployerNum 
                [CheckVesting] = case when ((Year(Report.RecDate) = @Year and Report.FromDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y')then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
                inner join eadb.dbo.Report report on report.reportid = hours.reportid   
                --and hours.SSN = @SSN   
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
                --and report.ToDate between @FromDate and @ToDate               
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = HPTransactions.Ber,  
                --EmpAccountNo = convert(int, HPTransactions.Employer),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  
                FromDate = convert(smalldatetime, HPTransactions.StartDate),  
                --ToDate = convert(smalldatetime, HPTransactions.EndDate),  
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),                 
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
                --Received = convert(smalldatetime, HPTransactions.DateReceived),  
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
                --Processed = convert(smalldatetime,hb.Updated),  
                --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
                SSN = convert(char(9),HPTransactions.SSN),  
                --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
                --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
                --UnionCode = convert(int,HPTransactions.UnionCode),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
                --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
                --IAPPercent = convert(money,HPTransactions.IAPDollars),  
                --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
                --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
                --HoursStatus = 0,  
                --LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = HPTransactions.UNMisc,  
                --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'H/P '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, e.OldEmployerNum  
                 [CheckVesting] = case when ((Year(HPTransactions.DateReceived) = @Year and HPTransactions.StartDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP              
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN  
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
                --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select      
                --ReportID = left(cpas.erractid,18),  
                --EmpAccountNo = convert(int, cpas.ERKey),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
                FromDate = convert(smalldatetime, cpas.FDate),  
                --ToDate = convert(smalldatetime, cpas.TDate),  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
                --Received = convert(smalldatetime, cpas.PDate),  
                --Processed = convert(smalldatetime, cpas.PDate),  
                --HoursId = convert(varchar(24),cpas.erractid),  
                SSN = convert(char(9),cpas.MKey),  
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,cpas.LOC_NO),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
    --we are not checking rate item to identify hours for Pension, Health, or IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
    --IAPPercent = convert(money,cpas.PanOnEarn),  
    --MM 12/26/12  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),cpas.RateGroup),  
                --HoursStatus = 0, --all the hours comming from CPAS are processed.  
                --LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                --LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                ---------------------------------------------------------------------  
                --UnionMisc = null,  
                --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'CPAS'  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, E.OldEmployerNum  
                 [CheckVesting] = case when ((Year(cpas.PDate) = @Year and cpas.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                where [Plan]=2  
                --and cpas.mkey = @SSN  
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
                --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	--EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	--ToDate = convert(smalldatetime, rap.TDate),
	--Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	--Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	--UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	--LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	--LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	[CheckVesting] = case when ((Year(rap.PDate) = @Year and rap.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
	from OPUS_Participant_List list   
	inner join EADB.dbo.RAP_IAP$ rap on list.ssn = rap.mkey
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
                --EmpAccountNo = convert(int, Pre.EmployerId),   
                ComputationYear = Pre.Plan_Year,  
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
                --ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
                --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
                --Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
                --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
                SSN = convert(char(9),Pre.SSN),   
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,Pre.UnionCode),   
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
                                                                                                when [Local]='L666' then convert(smallint, 4)  
                                                                                                when [Local]='L700' then convert(smallint, 6)  
                                                                                                when [Local]='L52' then convert(smallint, 7)  
                                                                                                when [Local]='L161' then convert(smallint, 8)  
                                                                                                else null end,   
                --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
                --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
                IAPHours = convert(numeric(7, 1), 0),  
                --IAPHoursA2 = convert(numeric(7, 1), 0),   
                --IAPPercent = convert(money, 0),   
                --ActiveHealthHours = convert(numeric(7, 1), 0),   
                --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
                --NULL PersonId,  
                --RateGroup = Pre.RateGroup,--?  
                --HoursStatus = convert(int, 0),  
                --LateMonthly = '',   
                --LateAnnual = '' ,  
                -------------------------------------------------------------------  
                --UnionMisc = convert(char(2),''),  
                --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'PM  '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = Pre.EmployerId  
                [CheckVesting] = case when ((Year(Pre.MergeDate) = @Year and Pre.StartDate < list.[Vesting_Date])OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
                --Pre.EndDate between @FromDate and @ToDate  
  
--select * from [#PensionWorkHistory]  
--order by todate  
--select * from [#PensionWorkHistory]  
--order by todate 
--insert into PensionWorkHistoryForStmt   


UPDATE [#PensionWorkHistory] SET CheckVesting='Y' WHERE SSN IN (SELECT DISTINCT TE.SSN FROM [#PensionWorkHistory] TE WHERE TE.CheckVesting='Y')
  

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR, VPIO.SSN, VPIO.CheckVesting,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,  
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,  
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS IAP_HOURS,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO ORDER BY VPIO.SSN  

DROP TABLE [#PensionWorkHistory]

END  
GO




-----------------------------------------------------------------------------------------------
--Name - Abhishek	
--Date - 29th April 2013
--Purpose - Stored Procedure for BIS batch. One Shot Weeekly Data 
-----------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[usp_GetWorkHistoryForAllMpippParticipantWeekly]    Script Date: 04/27/2013 23:24:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetWorkHistoryForAllMpippParticipantWeekly]  (@Year int)
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
                [SSN] [varchar](9) NULL ,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL  
 )   

INSERT INTO @temp    
EXEC OPUS.dbo.[GET_ALL_PARTICIPANT_SSN]  
  
delete from OPUS_Participant_List   
  
INSERT INTO OPUS_Participant_List  
select * from  @temp   

    
CREATE TABLE [#PensionWorkHistory](  
 --[ReportID] [varchar](18) NULL,  
 --[EmpAccountNo] [int] NULL,  
 [ComputationYear] [int] NULL,  
 [FromDate] [smalldatetime] NULL,  
 [ToDate] [smalldatetime] NULL,  
 --[Weeks] [char](2) NULL,  
 --[Received] [smalldatetime] NULL,  
 --[Processed] [smalldatetime] NULL,  
 --[HoursID] [varchar](24) NULL,  
 [SSN] [varchar](9) NULL,  
 --[LastName] [varchar](50) NULL,  
 --[FirstName] [varchar](50) NULL,  
 [UnionCode] [int] NULL,  
 [PensionPlan] [smallint] NULL,  
 --[PensionCredit] [numeric](7, 3) NULL,  
 --[L52VestedCredit] [smallint] NULL,  
 [PensionHours] [numeric](7, 1) NULL,  
 [IAPHours] [numeric](7, 1) NULL,  
 --[IAPHoursA2] [numeric](7, 1) NULL,  
 --[IAPPercent] [money] NULL,  
 --[ActiveHealthHours] [numeric](7, 1) NULL,  
 --[RetireeHealthHours] [numeric](7, 1) NULL,  
 --[PersonId] [varchar](15) NULL,  
 --[RateGroup] [varchar](4) NULL,  
 --[HoursStatus] [int] NULL,  
 --[LateMonthly] [varchar](1) NOT NULL,  
 --[LateAnnual] [varchar](1) NOT NULL,
 --[EmployerName] [varchar] (255) NULL
 --[UnionMisc] [char](2) NULL,  
 --[HoursWorked] [numeric](7, 1) NULL,  
 --[IAPHourlyRate] [money] NULL,  
 --[Source] [varchar](4) NOT NULL,  
 --[ToHealthSystem] [int] NULL,  
 --[ToPensionSystem] [int] NULL,  
 --[IsActiveHealth] [int] NULL,  
 --[IsRetireeHealth] [int] NULL,  
 --[IsPension] [int] NULL,  
 --[IsIAPHourly] [int] NULL  
 --, [OldEmployerNum] [varchar](6)  
)   
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)  
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
 --EmpAccountNo = E.EmployerId,  
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
 SSN = convert(char(9),Hours.SSN),  
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end  --old was numeric(18,1)  
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
 --NULL PersonId, --varchar(15) no change  
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
 --LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
 --LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end ,
 --EmployerName = E.EmployerName 
 --------------------------------------------------------------------------------------------------------------  
 --UnionMisc = Hours.UnionMisc, --New field. char(2)  
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
        --It is required because for those records where we do not have any rate group info  
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
 --IAPHourlyRate = rgb.Individual  --New field. money  
 --, Source = 'C/S '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = e.OldEmployerNum  
from OPUS_Participant_List list   
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
 inner join eadb.dbo.Report report on report.reportid = hours.reportid   
 --and hours.SSN = @SSN   
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
 --and report.ToDate between @FromDate and @ToDate   
 inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate   
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904) --and  
--Report.RecDate <= isnull(@CutOffDate,Report.RecDate)  
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = HPTransactions.Ber,  
 ----EmpAccountNo = convert(int, HPTransactions.Employer),  
 --EmpAccountNo = E.EmployerId,  
 PensionYear = PY.PensionYear,  
 FromDate = convert(smalldatetime, HPTransactions.StartDate),  
 ToDate = convert(smalldatetime, HPTransactions.EndDate),  
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),   
 --Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
 --Received = convert(smalldatetime, HPTransactions.DateReceived),  
 --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
 --Processed = convert(smalldatetime,hb.Updated),  
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
 SSN = convert(char(9),HPTransactions.SSN),  
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
 UnionCode = convert(int,HPTransactions.UnionCode),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension) -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,HPTransactions.IAPDollars),  
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
 --HoursStatus = 0,  
 --LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
 --LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,
 --EmployerName = E.EmployerName  
 ----------------------------------------------------------------------------------------------------------------  
 --UnionMisc = HPTransactions.UNMisc,  
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'H/P '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, e.OldEmployerNum  
from OPUS_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
 inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP   
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
 --and HPTransactions.SSN = @SSN
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
 --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select   
 --ReportID = left(cpas.erractid,18),  
 ----EmpAccountNo = convert(int, cpas.ERKey),  
 --EmpAccountNo = E.EmployerId,  
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
 FromDate = convert(smalldatetime, cpas.FDate),  
 ToDate = convert(smalldatetime, cpas.TDate),  
 --Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
 --Received = convert(smalldatetime, cpas.PDate),  
 --Processed = convert(smalldatetime, cpas.PDate),  
 --HoursId = convert(varchar(24),cpas.erractid),  
 SSN = convert(char(9),cpas.MKey),  
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,cpas.LOC_NO),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
 IAPHours = convert(numeric(7, 1), cpas.HRSACT)  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
 ----we are not checking rate item to identify hours for Pension, Health, or IAP  
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,cpas.PanOnEarn),  
 ----MM 12/26/12  
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),cpas.RateGroup),  
 --HoursStatus = 0, --all the hours comming from CPAS are processed.  
 --LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
 --LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
 --EmployerName = E.EmployerName 
 -----------------------------------------------------------------------  
 --UnionMisc = null,  
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'CPAS'  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, E.OldEmployerNum  
 from OPUS_Participant_List list   
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 where [Plan]=2  
 --and cpas.mkey = @SSN  
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
 --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	--EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	--Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	--Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT)  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	--LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	--LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	--EmployerName = E.EmployerName
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	from OPUS_Participant_List list 
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey = list.ssn
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
 --EmpAccountNo = convert(int, Pre.EmployerId),   
 ComputationYear = Pre.Plan_Year,  
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
 --Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
 --Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
 SSN = convert(char(9),Pre.SSN),   
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,Pre.UnionCode),   
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
      when [Local]='L666' then convert(smallint, 4)  
      when [Local]='L700' then convert(smallint, 6)  
      when [Local]='L52' then convert(smallint, 7)  
      when [Local]='L161' then convert(smallint, 8)  
      else null end,   
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
 IAPHours = convert(numeric(7, 1), 0)
 --IAPHoursA2 = convert(numeric(7, 1), 0),   
 --IAPPercent = convert(money, 0),   
 --ActiveHealthHours = convert(numeric(7, 1), 0),   
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
 --NULL PersonId,  
 --RateGroup = Pre.RateGroup,--?  
 --HoursStatus = convert(int, 0),  
 --LateMonthly = '',   
 --LateAnnual = '' ,
 --EmployerName = E.EmployerName 
 -------------------------------------------------------------------  
 --UnionMisc = convert(char(2),''),  
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'PM  '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = Pre.EmployerId  
from OPUS_Participant_List list   
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId
 --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
 --Pre.EndDate between @FromDate and @ToDate  
 
truncate table OPUS.dbo.SGT_ALL_MPIPP_WEEKLY_WORKHISTORY 

INSERT into OPUS.dbo.SGT_ALL_MPIPP_WEEKLY_WORKHISTORY 
SELECT cast(ComputationYear as int) AS ComputationYear, FromDate as FromDate, ToDate as ToDate, 
ISNULL(PensionHours,0.0) as PensionHours, ISNULL(IAPHours,0.0) as IAPHours,
SSN as SSN,
UnionCode as UnionCode
from [#PensionWorkHistory] where 
PensionPlan = 2 
				
SELECT NULL 
				
drop table [#PensionWorkHistory]

end  



-- =====================================================================
-- Author:        Mahua Banerjee
-- Create date:	  04/12/2013
-- Description:   usp_GetQDROHoursBetweenForfeitureAndQDRODate
-- ==================================================================== 

GO
/****** Object:  StoredProcedure [dbo].[usp_GetQDROHoursBetweenForfeitureAndQDRODate]    Script Date: 04/12/2013 00:40:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetQDROHoursBetweenForfeitureAndQDRODate] (@SSN char(10),@PLAN_CODE VARCHAR(10),@DATE_OF_DETERMINATION DATETIME,
					 @FORFEITURE_DATE DATETIME,@DENOMINATOR NUMERIC(7,1) OUTPUT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6),
	[SEQ] [int] IDENTITY(1,1) NOT NULL
) 

DECLARE @COUNT INT
DECLARE @TOTAL_COUNT INT


DECLARE @YEARS INT
DECLARE @COUNT_YEARLY INT
DECLARE @TOTAL_COUNT_YEARLY INT

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SET @TOTAL_COUNT = @@ROWCOUNT

SET @YEARS= 0
SET @DENOMINATOR=0.0

DECLARE @PensionWorkHistoryYearWise TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
) 

INSERT INTO @PensionWorkHistoryYearWise
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
0
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY = @@ROWCOUNT
SET @COUNT_YEARLY = 1

WHILE @COUNT_YEARLY <= @TOTAL_COUNT_YEARLY
BEGIN	
	IF (SELECT PWHY.QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ = @COUNT_YEARLY ) >= 400.0
	BEGIN
		SET @YEARS = @YEARS + 1
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	ELSE
	BEGIN
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	
	SET @COUNT_YEARLY = @COUNT_YEARLY + 1
END


-----------------------------------------------------------------------------------------------------------
--FOR DENOMINATOR
DECLARE @COUNT_YEARLY_FOR_DENOMINATOR INT
DECLARE @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR INT


DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) <= @FORFEITURE_DATE
DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) > @DATE_OF_DETERMINATION

DECLARE @PensionWorkHistoryYearDenominator TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
)


INSERT INTO @PensionWorkHistoryYearDenominator
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
(select QualifiedYearCount from @PensionWorkHistoryYearWise PWH where PWH.ComputationYear=VPIO.ComputationYear)
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR =@@ROWCOUNT
SET @COUNT_YEARLY_FOR_DENOMINATOR =1



PRINT @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR

	
	
	--FOR DENOMINATOR
	IF @PLAN_CODE = 'IAP'
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	ELSE
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
			IF (SELECT PWHYD.QualifiedYearCount FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR) < 20
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
				ELSE
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	
END
go




-- =====================================================================
-- Author:        Siddartha Jain
-- Create date:	  05/13/2013
-- Description:   usp_GETYEARENDEXTRACTIONDATAYEARLY
-- ==================================================================== 

/****** Object:  StoredProcedure [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]    Script Date: 05/03/2013 17:26:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]  
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
 [SSN] [varchar](9) NULL,  
 [PensionYear] [int] NULL   
 )   

INSERT INTO @temp    
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO  
  
declare @year int  
set select top(1) @year = PensionYear from @temp  
  
delete from OPUS_AnnualStmt_Participant_List  
  
INSERT INTO OPUS_AnnualStmt_Participant_List  
select * from  @temp   
  
declare @PlanStartDate datetime  
declare @PlanEndDate datetime  
declare @CutOffDate datetime  
 select @PlanStartDate = eadb.dbo.fn_PlanYearStartDate(@year),@PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)  
    select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate  
    
CREATE TABLE [#PensionWorkHistory](  
 --[ReportID] [varchar](18) NULL,  
 [EmpAccountNo] [int] NULL,  
 [ComputationYear] [int] NULL,  
 [FromDate] [smalldatetime] NULL,  
 [ToDate] [smalldatetime] NULL,  
 [Weeks] [char](2) NULL,  
 --[Received] [smalldatetime] NULL,  
 [Processed] [smalldatetime] NULL,  
 --[HoursID] [varchar](24) NULL,  
 [SSN] [varchar](9) NULL,  
 --[LastName] [varchar](50) NULL,  
 --[FirstName] [varchar](50) NULL,  
 [UnionCode] [int] NULL,  
 [PensionPlan] [smallint] NULL,  
 --[PensionCredit] [numeric](7, 3) NULL,  
 --[L52VestedCredit] [smallint] NULL,  
 [PensionHours] [numeric](7, 1) NULL,  
 [IAPHours] [numeric](7, 1) NULL,  
 --[IAPHoursA2] [numeric](7, 1) NULL,  
 --[IAPPercent] [money] NULL,  
 --[ActiveHealthHours] [numeric](7, 1) NULL,  
 --[RetireeHealthHours] [numeric](7, 1) NULL,  
 --[PersonId] [varchar](15) NULL,  
 --[RateGroup] [varchar](4) NULL,  
 --[HoursStatus] [int] NULL,  
 [LateMonthly] [varchar](1) NOT NULL,  
 [LateAnnual] [varchar](1) NOT NULL,
 [EmployerName] [varchar] (255) NULL
 --[UnionMisc] [char](2) NULL,  
 --[HoursWorked] [numeric](7, 1) NULL,  
 --[IAPHourlyRate] [money] NULL,  
 --[Source] [varchar](4) NOT NULL,  
 --[ToHealthSystem] [int] NULL,  
 --[ToPensionSystem] [int] NULL,  
 --[IsActiveHealth] [int] NULL,  
 --[IsRetireeHealth] [int] NULL,  
 --[IsPension] [int] NULL,  
 --[IsIAPHourly] [int] NULL  
 --, [OldEmployerNum] [varchar](6)  
)   
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)  
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
 SSN = convert(char(9),Hours.SSN),  
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
 --NULL PersonId, --varchar(15) no change  
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
 LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end ,
 EmployerName = E.EmployerName 
 --------------------------------------------------------------------------------------------------------------  
 --UnionMisc = Hours.UnionMisc, --New field. char(2)  
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
        --It is required because for those records where we do not have any rate group info  
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
 --IAPHourlyRate = rgb.Individual  --New field. money  
 --, Source = 'C/S '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
 inner join eadb.dbo.Report report on report.reportid = hours.reportid   
 --and hours.SSN = @SSN   
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
 --and report.ToDate between @FromDate and @ToDate   
 inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate   
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904) and  
--Report.RecDate <= isnull(@CutOffDate,Report.RecDate)  
Report.ProcessDate <= isnull(@CutOffDate,Report.RecDate)  
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select   
 --ReportID = HPTransactions.Ber,  
 ----EmpAccountNo = convert(int, HPTransactions.Employer),  
 EmpAccountNo = E.EmployerId,  
 PensionYear = PY.PensionYear,  
 FromDate = convert(smalldatetime, HPTransactions.StartDate),  
 ToDate = convert(smalldatetime, HPTransactions.EndDate),  
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),   
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
 --Received = convert(smalldatetime, HPTransactions.DateReceived),  
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
 --Processed = convert(smalldatetime,hb.Updated),  
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
 SSN = convert(char(9),HPTransactions.SSN),  
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
 UnionCode = convert(int,HPTransactions.UnionCode),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,HPTransactions.IAPDollars),  
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
 --HoursStatus = 0,  
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,
 EmployerName = E.EmployerName  
 ----------------------------------------------------------------------------------------------------------------  
 --UnionMisc = HPTransactions.UNMisc,  
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'H/P '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, e.OldEmployerNum  
from OPUS_AnnualStmt_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
 inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP   
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
 --and HPTransactions.SSN = @SSN  
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
 --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select   
 --ReportID = left(cpas.erractid,18),  
 ----EmpAccountNo = convert(int, cpas.ERKey),  
 EmpAccountNo = E.EmployerId,  
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
 FromDate = convert(smalldatetime, cpas.FDate),  
 ToDate = convert(smalldatetime, cpas.TDate),  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
 --Received = convert(smalldatetime, cpas.PDate),  
 Processed = convert(smalldatetime, cpas.PDate),  
 --HoursId = convert(varchar(24),cpas.erractid),  
 SSN = convert(char(9),cpas.MKey),  
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,cpas.LOC_NO),  
 PensionPlan = convert(smallint, 2), -- MPI   
 --PensionCredit = convert(numeric(7, 3),0),  
 --L52VestedCredit = convert(smallint,0),  
 PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
 IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
 ----we are not checking rate item to identify hours for Pension, Health, or IAP  
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
 --IAPPercent = convert(money,cpas.PanOnEarn),  
 ----MM 12/26/12  
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
 --NULL PersonId,  
 --RateGroup = convert(varchar(4),cpas.RateGroup),  
 --HoursStatus = 0, --all the hours comming from CPAS are processed.  
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
 EmployerName = E.EmployerName 
 -----------------------------------------------------------------------  
 --UnionMisc = null,  
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'CPAS'  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, E.OldEmployerNum  
 from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
 where [Plan]=2  
 --and cpas.mkey = @SSN  
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
 --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	EmployerName = E.EmployerName
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	from OPUS_AnnualStmt_Participant_List list  
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey=list.SSN
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN

  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select   
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
 EmpAccountNo = convert(int, Pre.EmployerId),   
 ComputationYear = Pre.Plan_Year,  
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
 SSN = convert(char(9),Pre.SSN),   
 --LastName = NULL, --convert(char(50),p.LastName),  
 --FirstName = NULL, --convert(char(50),p.FirstName),  
 UnionCode = convert(int,Pre.UnionCode),   
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
      when [Local]='L666' then convert(smallint, 4)  
      when [Local]='L700' then convert(smallint, 6)  
      when [Local]='L52' then convert(smallint, 7)  
      when [Local]='L161' then convert(smallint, 8)  
      else null end,   
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
 IAPHours = convert(numeric(7, 1), 0),  
 --IAPHoursA2 = convert(numeric(7, 1), 0),   
 --IAPPercent = convert(money, 0),   
 --ActiveHealthHours = convert(numeric(7, 1), 0),   
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
 --NULL PersonId,  
 --RateGroup = Pre.RateGroup,--?  
 --HoursStatus = convert(int, 0),  
 LateMonthly = '',   
 LateAnnual = '' ,
 EmployerName = E.EmployerName 
 -------------------------------------------------------------------  
 --UnionMisc = convert(char(2),''),  
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
 --IAPHourlyRate = rgb.Individual  
 --, Source = 'PM  '  
 --, rgc.ToHealthSystem  
 --, rgc.ToPensionSystem  
 --, IsActiveHealth = rgc.ActiveHealth  
 --, IsRetireeHealth = rgc.RetireeHealth  
 --, IsPension = rgc.Pension  
 --, IsIAPHourly = rgc.IAP  
 --, OldEmployerNum = Pre.EmployerId  
from OPUS_AnnualStmt_Participant_List list   
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
 left outer join EADB.dbo.vwRateGroupClassification_all RGC   
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId
 --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
 --Pre.EndDate between @FromDate and @ToDate  
  
--select isnull(EmpAccountNo, 0) as EmpAccountNo, ComputationYear, SSN, UnionCode, PensionHours, case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours, 
--isnull(EmployerName, '') as EmployerName from [#PensionWorkHistory] where 
--PensionPlan = 2 
				
																																		   
--insert into PensionWorkHistoryForStmt    
select ssn,computationyear,sum(isnull(pensionhours,0.0)) - sum(isnull(latehours,0.0)) TotalPensionHours,sum(isnull(latehours,0.0))TotalLateHours,UnionCode,EmpAccountno,EmployerName      
from       
(      
 select ssn,computationyear,pensionhours,case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,unioncode,empaccountno,e.employername,processed      
 from [#PensionWorkHistory] v      
 left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid  
  --where v.PensionPlan = 2         
)a      
group by ssn,computationyear,unioncode,empaccountno,employername   

drop table [#PensionWorkHistory]

end  

GO





/****** Object:  StoredProcedure [dbo].[usp_PensionInterface4OPUS_By_Dates]    Script Date: 05/12/2013 23:36:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[usp_PensionInterface4OPUS_By_Dates]

(@Year int, @FromDate smalldatetime = null,@ToDate smalldatetime = null,@PreviousCutOffDate smalldatetime = null,@CutOffDate smalldatetime = null)
AS
BEGIN
set nocount on
                --Declare @FromDate smalldatetime,@ToDate smalldatetime,@Debug int
                --select @Debug = 0
                
                if ((@FromDate is null or @ToDate is null) and @Year is not null)
                begin
                                Select  @FromDate = StartDate, @ToDate = EndDate
                                From     EADB.dbo.PensionYear 
                                where   PensionYear = @Year

                                select @PreviousCutOffDate = pe.cutoffdate 
                                from eadb.dbo.Period pe
                                inner join eadb.dbo.Pensionyear py on py.enddate = pe.qualifyingenddate
                                where py.pensionyear = @Year - 1

                                select @CutOffDate = pe.cutoffdate 
                                from eadb.dbo.Period pe
                                inner join eadb.dbo.Pensionyear py on py.enddate = pe.qualifyingenddate
                                where py.pensionyear = @Year
                end
                

                                
CREATE TABLE [#PensionWorkHistory](
                [ReportID] [varchar](18) NULL,
                [EmpAccountNo] [int] NULL,
                [ComputationYear] [smallint] NULL,
                [FromDate] [smalldatetime] NULL,
                [ToDate] [smalldatetime] NULL,
                [Weeks] [char](2) NULL,
                [Received] [smalldatetime] NULL,
                [Processed] [smalldatetime] NULL,
                [HoursID] [varchar](24) NULL,
                [SSN] [char](9) NULL,
                [LastName] [varchar](50) NULL,
                [FirstName] [varchar](50) NULL,
                [UnionCode] [int] NULL,
                [PensionPlan] [smallint] NULL,
                [PensionCredit] [numeric](7, 3) NULL,
                [L52VestedCredit] [smallint] NULL,
                [PensionHours] [numeric](7, 1) NULL,
                [IAPHours] [numeric](7, 1) NULL,
                [IAPHoursA2] [numeric](7, 1) NULL,
                [IAPPercent] [money] NULL,
                [ActiveHealthHours] [numeric](7, 1) NULL,
                [RetireeHealthHours] [numeric](7, 1) NULL,
                [PersonId] [varchar](15) NULL,
                [RateGroup] [varchar](4) NULL,
                [HoursStatus] [int] NULL,
                [LateMonthly] [varchar](1) NOT NULL,
                [LateAnnual] [varchar](1) NOT NULL,
                [UnionMisc] [char](2) NULL,
                [HoursWorked] [numeric](7, 1) NULL,
                [IAPHourlyRate] [money] NULL,
                [Source] [varchar](4) NOT NULL,
                [ToHealthSystem] [int] NULL,
                [ToPensionSystem] [int] NULL,
                [IsActiveHealth] [int] NULL,
                [IsRetireeHealth] [int] NULL,
                [IsPension] [int] NULL,
                [IsIAPHourly] [int] NULL
                , [OldEmployerNum] [varchar](6)
) 

insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)
                EmpAccountNo = E.EmployerId,
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),
                Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)
                SSN = convert(char(9),Hours.SSN),
                LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)
                FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)
                UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked 
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)
                IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)
                IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.
                ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)
                RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)
                NULL PersonId, --varchar(15) no change
                RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)
                HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.
                LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,
                LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = Hours.UnionMisc, --New field. char(2)
                HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system. 
                                                                                                                                --It is required because for those records where we do not have any rate group info
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.
                IAPHourlyRate = rgb.Individual  --New field. money
                , Source = 'C/S '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = e.OldEmployerNum
from      eadb.dbo.Report report
                inner join eadb.dbo.Hours hours on report.reportid = hours.reportid 
                --and hours.SSN = @SSN 
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate
	and ((report.ToDate between @FromDate and @ToDate and report.processdate <= @CutOffDate) or (report.pensionyear < year(@Todate) and report.processdate > @PreviousCutOffDate and report.processdate <= @CutOffDate))
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate 
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.
--Employer id for Locals Pre-Merger hours.
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)


insert into [#PensionWorkHistory]
select    
                ReportID = HPTransactions.Ber,
                --EmpAccountNo = convert(int, HPTransactions.Employer),
                EmpAccountNo = E.EmployerId,
                PensionYear = PY.PensionYear,
                FromDate = convert(smalldatetime, HPTransactions.StartDate),
                ToDate = convert(smalldatetime, HPTransactions.EndDate),
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),               
                Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),
                Received = convert(smalldatetime, HPTransactions.DateReceived),
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.
                Processed = convert(smalldatetime,hb.Updated),
                HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),
                SSN = convert(char(9),HPTransactions.SSN),
                LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),
                FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),
                UnionCode = convert(int,HPTransactions.UnionCode),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later 
                IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP
                IAPPercent = convert(money,HPTransactions.IAPDollars),
                ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),
                RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),
                NULL PersonId,
                RateGroup = convert(varchar(4),HPTransactions.RateGroup), 
                HoursStatus = 0,
                LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = HPTransactions.UNMisc,
                HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'H/P '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , e.OldEmployerNum
from eadb.dbo.HPTransactions HPTransactions
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch 
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
                (not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
    and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                and HPTransactions.EndDate between @FromDate and @ToDate

--CPAS View
insert into [#PensionWorkHistory]
select    
                ReportID = left(cpas.erractid,18),
                --EmpAccountNo = convert(int, cpas.ERKey),
                EmpAccountNo = E.EmployerId,
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,
                FromDate = convert(smalldatetime, cpas.FDate),
                ToDate = convert(smalldatetime, cpas.TDate),
                Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),
                Received = convert(smalldatetime, cpas.PDate),
                Processed = convert(smalldatetime, cpas.PDate),
                HoursId = convert(varchar(24),cpas.erractid),
                SSN = convert(char(9),cpas.MKey),
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,cpas.LOC_NO),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
                --we are not checking rate item to identify hours for Pension, Health, or IAP
                --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
                IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP
                IAPPercent = convert(money,cpas.PanOnEarn),
                --MM 12/26/12
                --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
                --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
                ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),
                RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),
                NULL PersonId,
                RateGroup = convert(varchar(4),cpas.RateGroup),
                HoursStatus = 0, --all the hours comming from CPAS are processed.
                LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,
                LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
                ---------------------------------------------------------------------
                UnionMisc = null,
                HoursWorked = convert(numeric(7, 1), cpas.HRSACT),
                IAPHourlyRate = rgb.Individual
                , Source = 'CPAS'
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , E.OldEmployerNum
                from EADB.dbo.CPASPre95_11222011 cpas
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
                left outer join eadb.dbo.vwRateGroupClassification_all RGC 
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
--             left outer join pid.dbo.Person p on cpas.mkey = p.ssn
                where [Plan]=2
                
                --and cpas.mkey = @SSN
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate
                and cpas.TDate between @FromDate and @ToDate

-- RAP IAP$
insert into [#PensionWorkHistory]
select    
                ReportID = left(rap.erractid,18),
                --EmpAccountNo = convert(int, cpas.ERKey),
                EmpAccountNo = isnull(E.EmployerId,'0'),
                ComputationYear = rap.Plan_Year, -- PY.PensionYear,
                FromDate = convert(smalldatetime, rap.FDate),
                ToDate = convert(smalldatetime, rap.TDate),
                Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
                Received = convert(smalldatetime, rap.PDate),
                Processed = convert(smalldatetime, rap.PDate),
                HoursId = convert(varchar(24),rap.erractid),
                SSN = convert(char(9),rap.MKey),
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,rap.LOC_NO),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
                IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
                --we are not checking rate item to identify hours for Pension, Health, or IAP
                --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
                IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
                IAPPercent = convert(money,rap.PanOnEarn),
                --MM 12/26/12
                --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
                --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
                ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
                RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
                NULL AS PersonId,
                RateGroup = convert(varchar(4),rap.RateGroup),
                HoursStatus = 0, --all the hours comming from CPAS are processed.
                LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
                LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
                ---------------------------------------------------------------------
                UnionMisc = null,
                HoursWorked = convert(numeric(7, 1), rap.HRSACT),
                IAPHourlyRate = rgb.Individual
                , Source = 'RAP'
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , E.OldEmployerNum
                from EADB.dbo.RAP_IAP$ rap
                left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
                left outer join eadb.dbo.vwRateGroupClassification_all RGC 
                                on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
                left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
                                on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                where [Plan]=2
                --and rap.mkey = @SSN
    and rap.TDate between @FromDate and @ToDate
    
--PreMerger View.
insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,
                EmpAccountNo = convert(int, Pre.EmployerId), 
                ComputationYear = Pre.Plan_Year,
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date
                ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53
                Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date 
                Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date
                HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id
                SSN = convert(char(9),Pre.SSN), 
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,Pre.UnionCode), 
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)
                                                                                                when [Local]='L666' then convert(smallint, 4)
                                                                                                when [Local]='L700' then convert(smallint, 6)
                                                                                                when [Local]='L52' then convert(smallint, 7)
                                                                                                when [Local]='L161' then convert(smallint, 8)
                                                                                                else null end, 
                PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),
                L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHours = convert(numeric(7, 1), 0),
                IAPHoursA2 = convert(numeric(7, 1), 0), 
                IAPPercent = convert(money, 0), 
                ActiveHealthHours = convert(numeric(7, 1), 0), 
                RetireeHealthHours = convert(numeric(7, 1), 0), -- ?
                NULL PersonId,
                RateGroup = Pre.RateGroup,--?
                HoursStatus = convert(int, 0),
                LateMonthly = '', 
                LateAnnual = '' ,
                -------------------------------------------------------------------
                UnionMisc = convert(char(2),''),
                HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'PM  '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = Pre.EmployerId
from EADB.dbo.Pension_PreMerger_Annual_History Pre
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate
--             left outer join pid.dbo.Person p on Pre.ssn = p.ssn
where --Pre.SSN = @SSN
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate
                Pre.EndDate between @FromDate and @ToDate

select * from [#PensionWorkHistory]
order by todate
drop table [#PensionWorkHistory]

END



------------------------------------------------------------------------------------------------------------------------
--Created By	:	WASIM PATHAN
--Created On	:	05/13/2013
--Description	:	Scripts for usp_GetTrueUnionNamebyUnionCode
------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[usp_GetTrueUnionNamebyUnionCode]    Script Date: 05/13/2013 04:43:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================================
-- Author:        Wasim Pathan
-- Create date:	  05/13/2013
-- Description:   usp_GetTrueUnionNamebyUnionCode
-- ==================================================================== 

CREATE PROCEDURE [dbo].[usp_GetTrueUnionNamebyUnionCode] (@UnionCode char(5) = null)
AS
BEGIN
SET NOCOUNT ON;

select description as UNION_CODE_DESC, UnionCode AS UNION_CODE from dbo.vw_UnionByGroup where UnionCode = @UnionCode
END


-- =====================================================================
-- Author:        Rashmi Sheri
-- Create date:	  05/15/2013
-- Description:   usp_GetWeaklyWorkData
-- ==================================================================== 

/****** Object:  StoredProcedure [dbo].[usp_GetWeaklyWorkData]    Script Date: 05/15/2013 13:03:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_GetWeaklyWorkData](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS PlanYear,VPIO.FromDate AS BeginingDate,VPIO.ToDate AS EndingDate,VPIO.IAPHours AS IAPHours,VPIO.PensionHours AS MPIHours,VPIO.UnionCode AS UnionCode,EMPR.EmployerName AS Employer
FROM @PensionWorkHistory AS VPIO LEFT JOIN Employer EMPR ON VPIO.OldEmployerNum=EMPR.OldEmployerNum
WHERE VPIO.SSN=@SSN

END




/****** Object:  StoredProcedure [dbo].[usp_GetHealthWorkData]    Script Date: 05/27/2013 21:46:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[usp_GetHealthWorkData](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6)
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,VPIO.SSN,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS idcPensionHours_healthBatch,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate
FROM @PensionWorkHistory AS VPIO ORDER BY VPIO.SSN,cast(VPIO.ComputationYear as int)

END

-- =====================================================================
-- Author:       Tushar Chandak
-- Create date:	  05/28/2013
-- Description:   usp_GetWeaklyWorkData
-- ==================================================================== 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWeaklyWorkData](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS PlanYear,VPIO.FromDate AS BeginingDate,VPIO.ToDate AS EndingDate,VPIO.IAPHours AS IAPHours,VPIO.PensionHours AS MPIHours,VPIO.UnionCode AS UnionCode,EMPR.EmployerName AS Employer,
UGP.description as UnionCodeDesc
FROM @PensionWorkHistory AS VPIO 
LEFT JOIN Employer EMPR ON VPIO.OldEmployerNum=EMPR.OldEmployerNum
LEFT JOIN dbo.vw_UnionByGroup UGP on VPIO.UnionCode=UGP.UnionCode
WHERE VPIO.SSN=@SSN

END
GO

-- =====================================================================
-- Author:       Kunal Arora
-- Create date:	  05/29/2013
-- Description:   usp_GetQDROHoursBetweenForfeitureAndQDRODate PIR 1003. 
-- ==================================================================== 

USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetQDROHoursBetweenForfeitureAndQDRODate]    Script Date: 05/29/2013 04:27:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GetQDROHoursBetweenForfeitureAndQDRODate] (@SSN char(10),@PLAN_CODE VARCHAR(10),@DATE_OF_DETERMINATION DATETIME,
					 @FORFEITURE_DATE DATETIME,@DENOMINATOR NUMERIC(7,1) OUTPUT)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL,
	[OldEmployerNum] [varchar](6),
	[SEQ] [int] IDENTITY(1,1) NOT NULL
) 

DECLARE @COUNT INT
DECLARE @TOTAL_COUNT INT


DECLARE @YEARS INT
DECLARE @COUNT_YEARLY INT
DECLARE @TOTAL_COUNT_YEARLY INT

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SET @TOTAL_COUNT = @@ROWCOUNT

SET @YEARS= 0
SET @DENOMINATOR=0.0

DECLARE @PensionWorkHistoryYearWise TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
) 

INSERT INTO @PensionWorkHistoryYearWise
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
0
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY = @@ROWCOUNT
SET @COUNT_YEARLY = 1

WHILE @COUNT_YEARLY <= @TOTAL_COUNT_YEARLY
BEGIN	
	IF (SELECT PWHY.QualifiedHours FROM @PensionWorkHistoryYearWise PWHY WHERE PWHY.YearlySEQ = @COUNT_YEARLY ) >= 400.0
	BEGIN
		SET @YEARS = @YEARS + 1
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	ELSE
	BEGIN
		UPDATE @PensionWorkHistoryYearWise SET QualifiedYearCount = @YEARS WHERE YearlySEQ = @COUNT_YEARLY
	END
	
	SET @COUNT_YEARLY = @COUNT_YEARLY + 1
END


-----------------------------------------------------------------------------------------------------------
--FOR DENOMINATOR
DECLARE @COUNT_YEARLY_FOR_DENOMINATOR INT
DECLARE @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR INT


DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) <= @FORFEITURE_DATE
DELETE FROM @PensionWorkHistory WHERE CAST(FromDate AS DATETIME) > @DATE_OF_DETERMINATION

DECLARE @PensionWorkHistoryYearDenominator TABLE(
	[ComputationYear] [smallint] NULL,
	[QualifiedHours] [numeric](7, 1) NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[QualifiedYearCount] [smallint] NULL,
	[YearlySEQ] [int] IDENTITY(1,1) NOT NULL
)


INSERT INTO @PensionWorkHistoryYearDenominator
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLAN_CODE='MPIPP' THEN 
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 WHEN @PLAN_CODE='IAP' THEN
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
WHEN @PLAN_CODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLAN_CODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLAN_CODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLAN_CODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLAN_CODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
(SELECT TOP(1)FromDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS FROMDATE,
(SELECT TOP(1)ToDate FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear order by TODATE desc) AS TODATE,
(select QualifiedYearCount from @PensionWorkHistoryYearWise PWH where PWH.ComputationYear=VPIO.ComputationYear)
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

SET @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR =@@ROWCOUNT
SET @COUNT_YEARLY_FOR_DENOMINATOR =1



PRINT @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR

	
	
	--FOR DENOMINATOR
	IF @PLAN_CODE = 'IAP'
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
				IF  (SELECT  TOP(1) ComputationYear FROM @PensionWorkHistoryYearDenominator PWHYD WHERE YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR) > 1979
				begin
				
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				end
				else
				BEGIN
				SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE 
					 PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR
				end
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	ELSE
	BEGIN
		WHILE @COUNT_YEARLY_FOR_DENOMINATOR <= @TOTAL_COUNT_YEARLY_FOR_DENOMINATOR 
		BEGIN
			IF (SELECT PWHYD.QualifiedYearCount FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR) < 20
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.QualifiedHours >= 400.0 
					AND PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
				ELSE
				BEGIN
					SELECT  @DENOMINATOR = @DENOMINATOR +  QualifiedHours FROM @PensionWorkHistoryYearDenominator PWHYD WHERE PWHYD.YearlySEQ=@COUNT_YEARLY_FOR_DENOMINATOR 
				END
					SET @COUNT_YEARLY_FOR_DENOMINATOR = @COUNT_YEARLY_FOR_DENOMINATOR + 1
		END
	END
	
END


-- =====================================================================
-- Author:       WASIM PATHAN
-- Create date:	 05/30/2013
-- Description:  Alter Procedure [usp_GetWeeklyWorkData] 
-- ==================================================================== 
EXEC sp_rename 'usp_GetWeaklyWorkData', 'usp_GetWeeklyWorkData'

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[usp_GetWeeklyWorkData](@SSN char(10))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS PlanYear,VPIO.FromDate AS BeginingDate,VPIO.ToDate AS EndingDate,VPIO.IAPHours AS IAPHours,VPIO.PensionHours AS MPIHours,VPIO.UnionCode AS UnionCode,EMPR.EmployerName AS Employer,
UGP.Name as UnionCodeDesc
FROM @PensionWorkHistory AS VPIO 
LEFT JOIN Employer EMPR ON VPIO.OldEmployerNum=EMPR.OldEmployerNum
LEFT JOIN Unions UGP ON VPIO.UnionCode = UGP.UnionCode
WHERE VPIO.SSN=@SSN

END

go



-- =====================================================================
-- Author:       ABHISHEK SHARMA
-- Create date:	 06/2/2013
-- Description:  Alter function [fn_GetTrueUnionBy_SSN_N_Date_OldWay] 
-- ==================================================================== 

/****** Object:  UserDefinedFunction [dbo].[fn_GetTrueUnionBy_SSN_N_Date_OldWay]    Script Date: 05/24/2013 16:33:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER Function [dbo].[fn_GetTrueUnionBy_SSN_N_Date_OldWay](@SSN varchar(9), @FromDate smalldatetime,@ToDate smalldatetime)
returns int
AS
BEGIN
	--Declare @SSN varchar(9), @FromDate smalldatetime,@ToDate smalldatetime
	--Select  @FromDate = '12/27/2009', @ToDate = '12/25/2010'
	--Select @SSN = '367505993'

	declare @Union int, @LastUnion int, @PUnion int, @TrueUnion int, @multipleunion int

	select top 1 @LastUnion = unioncode
	from eadb.dbo.hours h inner join eadb.dbo.report r on h.reportid = r.reportid and r.todate between @FromDate and @Todate 
	and SSN = @SSN and h.status = 0 and unioncode not in (select productioncode from eadb.dbo.productioncodes)
	order by r.reportid desc

	if @LastUnion is null
	begin
		select top 1 @PUnion = unioncode
		from eadb.dbo.hours h inner join eadb.dbo.report r on h.reportid = r.reportid and r.todate between @FromDate and @Todate 
		and SSN = @SSN and h.status = 0 
		order by r.reportid desc
	end
	--Print '1 - '+convert(varchar(3), isnull(@LastUnion,0))+','+convert(varchar(3), isnull(@PUnion,0))+','+convert(varchar(3), isnull(@TrueUnion,0))+','+convert(varchar(3), isnull(@Union,0))
	if @LastUnion is null and @PUnion is null
	begin
		select top 1 @LastUnion = unioncode
		from eadb.dbo.hours h inner join eadb.dbo.report r on h.reportid = r.reportid and r.todate < @FromDate
		and SSN = @SSN and h.status = 0 and unioncode not in (select productioncode from eadb.dbo.productioncodes)
		order by r.reportid desc

		if @LastUnion is null
		begin
			select top 1 @PUnion = unioncode
			from eadb.dbo.hours h inner join eadb.dbo.report r on h.reportid = r.reportid and r.todate < @FromDate
			and SSN = @SSN and h.status = 0 
			order by r.reportid desc
		end
	end
	--Print '2 - '+convert(varchar(3), isnull(@LastUnion,0))+','+convert(varchar(3), isnull(@PUnion,0))+','+convert(varchar(3), isnull(@TrueUnion,0))+','+convert(varchar(3), isnull(@Union,0))
	
	if @LastUnion is null and @PUnion is null
	begin
		select top 1 @LastUnion = h.unioncode
		from eadb.dbo.hptransactions h 
		where h.ssn = @ssn and enddate  between @fromdate and @Todate and reportno <> '0.'
		order by convert(int,reportno) desc	     
	end
	--Print '3 - '+convert(varchar(3), isnull(@LastUnion,0))+','+convert(varchar(3), isnull(@PUnion,0))+','+convert(varchar(3), isnull(@TrueUnion,0))+','+convert(varchar(3), isnull(@Union,0))

	select top 1 @TrueUnion = trueunion from eadb.dbo.trueunion where ssn = @ssn and (auditdate <=@todate or auditdate is null)
	select @multipleunion = (select count(trueunion) from eadb.dbo.trueunion where ssn = @ssn and (auditdate <=@todate or auditdate is null))

	--Print '4 - '+convert(varchar(3), isnull(@LastUnion,0))+','+convert(varchar(3), isnull(@PUnion,0))+','+convert(varchar(3), isnull(@TrueUnion,0))+','+convert(varchar(3), isnull(@Union,0))
	set @union = case
				when @multipleunion > 1 and @LastUnion is not null then @LastUnion
				when @TrueUnion is not null then @TrueUnion
				when @LastUnion is not null then @LastUnion
				else @PUnion
				end
				
	--Print '5 - '+convert(varchar(3), isnull(@LastUnion,0))+','+convert(varchar(3), isnull(@PUnion,0))+','+convert(varchar(3), isnull(@TrueUnion,0))+','+convert(varchar(3), isnull(@Union,0))
	
	if @union is null
		select top 1 @union = unioncode from dbo.vw_Employer_Hours where ssn = @ssn
	
	--Print '6 - '+convert(varchar(3), isnull(@LastUnion,''))+','+convert(varchar(3), isnull(@PUnion,''))+','+convert(varchar(3), isnull(@TrueUnion,''))+','+convert(varchar(3), isnull(@Union,''))

	select @union = isnull(@union,0)				
	return @union
END



-- =====================================================================
-- Author:       ABHISHEK SHARMA
-- Create date:	 06/2/2013
-- Description:  Alter PROCEDURE [usp_GetWorkDataTillGivenDate] 
-- ==================================================================== 

/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataTillGivenDate]    Script Date: 05/31/2013 22:10:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWorkDataTillGivenDate](@SSN char(10),@PLANCODE varchar(20),@RETIREMENT_DATE DATETIME=null)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

IF @RETIREMENT_DATE = '01/01/1753'
BEGIN
	SET @RETIREMENT_DATE=NULL
END

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN
 
IF @RETIREMENT_DATE <> NULL OR @RETIREMENT_DATE <> '' OR @RETIREMENT_DATE <> '01/01/1753'
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and ToDate <= @RETIREMENT_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END
END

ELSE 
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

END

END




-- =====================================================================
-- Author:       ABHISHEK SHARMA
-- Create date:	 06/2/2013
-- Description:  Alter PROCEDURE [usp_GetWorkDataTillGivenDate] 
-- ==================================================================== 
/****** Object:  StoredProcedure [dbo].[S]    Script Date: 05/31/2013 22:20:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWorkDataForPersonOverview](@SSN char(9))
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [int] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT DISTINCT VPIO.ComputationYear AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS QUALIFIED_HOURS,
 (SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS VESTED_HOURS,
 (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalIAPHours,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR

END

-- =====================================================================
-- Author:       Tushar Chandak
-- Create date:	 06/19/2013
-- Description:  Alter PROCEDURE [usp_GetWorkDataAfterRetirement] 
-- ==================================================================== 

/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataAfterRetirement]    Script Date: 06/07/2013 02:01:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[usp_GetWorkDataAfterRetirement](@SSN char(10),@RETIREMENT_DATE DateTime,@TO_DATE DateTime=NULL)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[MPI_PERSON_ID] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

SELECT VPIO.ComputationYear AS ComputationYear, VPIO.FromDate AS FromDate, VPIO.ToDate ToDate, 
VPIO.PensionHours AS PensionHours,VPIO.IAPHours,VPIO.IAPHoursA2,VPIO.IAPPercent,VPIO.Weeks,VPIO.OldEmployerNum,EMPR.EmployerName,EMPR.Address1,EMPR.City,EMPR.Address2,EMPR.State,EMPR.Contact1,EMPR.PostalCode,EMPR.Contact2
,EMPRADR.Street,UGP.Name as UnionCodeDesc
FROM @PensionWorkHistory AS VPIO INNER JOIN Employer EMPR ON VPIO.OldEmployerNum = EMPR.OldEmployerNum
LEFT JOIN Unions UGP ON VPIO.UnionCode = UGP.UnionCode
LEFT OUTER join EmployerAddress EMPRADR ON EMPR.EmployerId=EMPRADR.EmployerId and EMPRADR.Type = 0 
where VPIO.SSN=@SSN AND  VPIO.ToDate>@RETIREMENT_DATE AND (VPIO.ToDate <=@TO_DATE OR @TO_DATE IS NULL)
END

go



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan
-- Date - 06/23/2013
-- Purpose - Alter SP usp_GetWorkhistoryForIAPAllocation
----------------------------------------------------------------------------------------------------------------------------------

ALTER PROC [dbo].[usp_GetWorkhistoryForIAPAllocation](@SSN char(10),@FROMDATE DATETIME, @COMPUTATIONYEAR INT,@RETIREMENTDATE DATETIME = NULL)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

IF @RETIREMENTDATE IS NOT NULL OR @RETIREMENTDATE <> ''
BEGIN
SELECT EMPACCOUNTNO,SSN,COMPUTATIONYEAR,SUM(IAPHOURS) IAPHOURS,SUM(IAPHOURSA2) IAPHOURSA2,SUM(IAPPERCENT) IAPPERCENT
FROM @PensionWorkHistory
WHERE ComputationYear < @COMPUTATIONYEAR
AND Processed < @FROMDATE AND FromDate > @RETIREMENTDATE
AND PensionPlan = 2
GROUP BY EMPACCOUNTNO,SSN,COMPUTATIONYEAR
END
ELSE
BEGIN
SELECT EMPACCOUNTNO,SSN,COMPUTATIONYEAR,SUM(IAPHOURS) IAPHOURS,SUM(IAPHOURSA2) IAPHOURSA2,SUM(IAPPERCENT) IAPPERCENT
FROM @PensionWorkHistory
WHERE ComputationYear < @COMPUTATIONYEAR
AND Processed < @FROMDATE
AND PensionPlan = 2
GROUP BY EMPACCOUNTNO,SSN,COMPUTATIONYEAR
END
END
go



USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWorkhistoryForIAPAllocation]    Script Date: 06/23/2013 11:26:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan
-- Date - 06/24/2013
-- Purpose - CREATE SP usp_GetWorkhistoryForIAPAllocation
----------------------------------------------------------------------------------------------------------------------------------

ALTER PROC [dbo].[usp_GetWorkhistoryForIAPAllocation](@SSN char(10),@FROMDATE DATETIME, @COMPUTATIONYEAR INT,@RETIREMENTDATE DATETIME = NULL)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN

IF @RETIREMENTDATE IS NOT NULL OR @RETIREMENTDATE <> ''
BEGIN
SELECT EMPACCOUNTNO,SSN,COMPUTATIONYEAR,SUM(IAPHOURS) IAPHOURS,SUM(IAPHOURSA2) IAPHOURSA2,SUM(IAPPERCENT) IAPPERCENT
FROM @PensionWorkHistory
WHERE ComputationYear < @COMPUTATIONYEAR
AND Processed < @FROMDATE AND FromDate > @RETIREMENTDATE
AND PensionPlan = 2
GROUP BY EMPACCOUNTNO,SSN,COMPUTATIONYEAR
END
ELSE
BEGIN
SELECT EMPACCOUNTNO,SSN,COMPUTATIONYEAR,SUM(IAPHOURS) IAPHOURS,SUM(IAPHOURSA2) IAPHOURSA2,SUM(IAPPERCENT) IAPPERCENT
FROM @PensionWorkHistory
WHERE ComputationYear < @COMPUTATIONYEAR
AND Processed < @FROMDATE
AND PensionPlan = 2
GROUP BY EMPACCOUNTNO,SSN,COMPUTATIONYEAR
END
END
go



/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataTillGivenDate]    Script Date: 06/30/2013 17:33:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWorkDataTillGivenDate](@SSN char(10),@PLANCODE varchar(20),@RETIREMENT_DATE DATETIME=null,@EVALUATION_DATE DATETIME=null,@VESTING_DATE DATETIME=null)
AS
BEGIN
SET NOCOUNT ON
CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

IF @RETIREMENT_DATE = '01/01/1753'
BEGIN
	SET @RETIREMENT_DATE=NULL
END


IF @EVALUATION_DATE = NULL
BEGIN
	SET @RETIREMENT_DATE='01/01/1753'
END

IF @VESTING_DATE = NULL
BEGIN
	SET @VESTING_DATE='01/01/1753'
END

INSERT INTO [#PensionWorkHistory] 
EXEC usp_PensionInterface4OPUS @SSN
 
IF @RETIREMENT_DATE <> NULL OR @RETIREMENT_DATE <> '' OR @RETIREMENT_DATE <> '01/01/1753'
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_PensionCredits,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')) as iintLateHourCount,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_PERCENT,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and ToDate <= @RETIREMENT_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END
END

ELSE 
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_PensionCredits,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')),
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_PERCENT,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

END

DROP TABLE [#PensionWorkHistory]
END

GO



USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataTillGivenDate]    Script Date: 07/02/2013 09:10:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWorkDataTillGivenDate](@SSN char(10),@PLANCODE varchar(20),@RETIREMENT_DATE DATETIME=null,@EVALUATION_DATE DATETIME=null,@VESTING_DATE DATETIME=null)
AS
BEGIN
SET NOCOUNT ON
CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

IF @RETIREMENT_DATE = '01/01/1753'
BEGIN
	SET @RETIREMENT_DATE=NULL
END


IF @EVALUATION_DATE = NULL
BEGIN
	SET @RETIREMENT_DATE='01/01/1753'
END

IF @VESTING_DATE = NULL
BEGIN
	SET @VESTING_DATE='01/01/1753'
END

INSERT INTO [#PensionWorkHistory] 
EXEC usp_PensionInterface4OPUS @SSN
 
IF @RETIREMENT_DATE <> NULL OR @RETIREMENT_DATE <> '' OR @RETIREMENT_DATE <> '01/01/1753'
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_PensionCredits,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')) as iintLateHourCount,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_PERCENT,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and ToDate <= @RETIREMENT_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END
END

ELSE 
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_PensionCredits,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')) as iintLateHourCount,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_PERCENT,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2,3,4,6,7,8)) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

END

DROP TABLE [#PensionWorkHistory]
END

GO

---------------------------------------------------------------------------------------------
--Created By	:	Kunal Arora
--Created On	:	08/13/2013
--Description	:	Stor Proc To Fetch Hours reported Last month
-----------------------------------------------------------------------------------

USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetHoursProcessedPreviousDay]    Script Date: 08/12/2013 23:24:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[usp_GetHoursProcessedPreviousDay](@FROM_DATE datetime,@TO_DATE datetime)
as begin
select h.SSN,r.fromdate,r.todate,h.hoursworked,convert(varchar(12),r.processdate,101)Processdate 
from EADB.dbo.hours h 
inner join EADB.dbo.report r on r.reportid = h.reportid and r.status = 0 and 
convert(varchar(12),r.processdate,101) between @FROM_DATE and @TO_DATE 
end



---------------------------------------------------------------------------------------------
--Created By	:	Rohan Adgaonkar
--Created On	:	02/25/2014
--Description	:	Stor Proc usp_GetWorkHistoryForAllMpippParticipant
---------------------------------------------------------------------------------------------

USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWorkHistoryForAllMpippParticipant]    Script Date: 02/25/2014 22:29:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GetWorkHistoryForAllMpippParticipant] (@Year int,@ISBIS VARCHAR(1) = 'Y')
AS  
BEGIN  
SET NOCOUNT ON  
  
DECLARE @temp TABLE(  
                [SSN] [varchar](9) NULL ,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                         
                )   
  
INSERT INTO @temp    
EXEC OPUS.dbo.[GET_ALL_PARTICIPANT_SSN] @ISBIS
  
delete from OPUS_Participant_List   
  
INSERT INTO OPUS_Participant_List  
select * from  @temp   
  
                                  
CREATE TABLE [#PensionWorkHistory](  
                --[ReportID] [varchar](18) NULL,  
                --[EmpAccountNo] [int] NULL,  
                [ComputationYear] [smallint] NULL,  
                [FromDate] [smalldatetime] NULL,  
                --[ToDate] [smalldatetime] NULL,  
                --[Weeks] [char](2) NULL,  
                --[Received] [smalldatetime] NULL,  
                --[Processed] [smalldatetime] NULL,  
                --[HoursID] [varchar](24) NULL,  
                [SSN] [char](9) NULL,  
                --[LastName] [varchar](50) NULL,  
                --[FirstName] [varchar](50) NULL,  
                --[UnionCode] [int] NULL,  
                [PensionPlan] [smallint] NULL,  
                --[PensionCredit] [numeric](7, 3) NULL,  
                --[L52VestedCredit] [smallint] NULL,  
                [PensionHours] [numeric](7, 1) NULL,  
                [IAPHours] [numeric](7, 1) NULL,  
                --[IAPHoursA2] [numeric](7, 1) NULL,  
                --[IAPPercent] [money] NULL,  
                --[ActiveHealthHours] [numeric](7, 1) NULL,  
                --[RetireeHealthHours] [numeric](7, 1) NULL,  
                --[PersonId] [varchar](15) NULL,  
                --[RateGroup] [varchar](4) NULL,  
                --[HoursStatus] [int] NULL,  
                --[LateMonthly] [varchar](1) NOT NULL,  
                --[LateAnnual] [varchar](1) NOT NULL,  
                --[UnionMisc] [char](2) NULL,  
                --[HoursWorked] [numeric](7, 1) NULL,  
                --[IAPHourlyRate] [money] NULL,  
                --[Source] [varchar](4) NOT NULL,  
                --[ToHealthSystem] [int] NULL,  
                --[ToPensionSystem] [int] NULL,  
                --[IsActiveHealth] [int] NULL,  
                --[IsRetireeHealth] [int] NULL,  
                --[IsPension] [int] NULL,  
                --[IsIAPHourly] [int] NULL, 
                --[OldEmployerNum] [varchar](6), 
                [CheckVesting] [varchar](1) 
)   
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)  
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'  
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),  
                --Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime  
                --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)  
                SSN = convert(char(9),Hours.SSN),  
                --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)  
                --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)  
                --UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)  
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked   
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)  
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)  
                --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)  
                --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.  
                --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)  
                --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)  
                --NULL PersonId, --varchar(15) no change  
                --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)  
                --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.  
                --LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --LateAnnual = case when Report.RecDate > coalesce(PlanCutoff.cutoffdate, Report.RecDate) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = Hours.UnionMisc, --New field. char(2)  
                --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.   
                                                                                                                                --It is required because for those records where we do not have any rate group info  
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.  
                --IAPHourlyRate = rgb.Individual  --New field. money  
                --, Source = 'C/S '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = e.OldEmployerNum 
                [CheckVesting] = case when ((Year(Report.RecDate) = @Year and Report.FromDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y')then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN  
                inner join eadb.dbo.Report report on report.reportid = hours.reportid   
                --and hours.SSN = @SSN   
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate  
                --and report.ToDate between @FromDate and @ToDate               
                inner join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP  
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate   
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate  
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.  
--Employer id for Locals Pre-Merger hours.  
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)  
  
insert into [#PensionWorkHistory]  
select      
                --ReportID = HPTransactions.Ber,  
                --EmpAccountNo = convert(int, HPTransactions.Employer),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = PY.PensionYear,  
                FromDate = convert(smalldatetime, HPTransactions.StartDate),  
                --ToDate = convert(smalldatetime, HPTransactions.EndDate),  
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),                 
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),  
                --Received = convert(smalldatetime, HPTransactions.DateReceived),  
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.  
                --Processed = convert(smalldatetime,hb.Updated),  
                --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),  
                SSN = convert(char(9),HPTransactions.SSN),  
                --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),  
                --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),  
                --UnionCode = convert(int,HPTransactions.UnionCode),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),  
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later   
                --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP  
                --IAPPercent = convert(money,HPTransactions.IAPDollars),  
                --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),  
                --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),HPTransactions.RateGroup),   
                --HoursStatus = 0,  
                --LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,  
                --------------------------------------------------------------------------------------------------------------  
                --UnionMisc = HPTransactions.UNMisc,  
                --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'H/P '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, e.OldEmployerNum  
                 [CheckVesting] = case when ((Year(HPTransactions.DateReceived) = @Year and HPTransactions.StartDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN  
                inner join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP              
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch   
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN  
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate  
                --and HPTransactions.EndDate between @FromDate and @ToDate  
  
--CPAS View  
insert into [#PensionWorkHistory]  
select      
                --ReportID = left(cpas.erractid,18),  
                --EmpAccountNo = convert(int, cpas.ERKey),  
                --EmpAccountNo = E.EmployerId,  
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,  
                FromDate = convert(smalldatetime, cpas.FDate),  
                --ToDate = convert(smalldatetime, cpas.TDate),  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),  
                --Received = convert(smalldatetime, cpas.PDate),  
                --Processed = convert(smalldatetime, cpas.PDate),  
                --HoursId = convert(varchar(24),cpas.erractid),  
                SSN = convert(char(9),cpas.MKey),  
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,cpas.LOC_NO),  
                PensionPlan = convert(smallint, 2), -- MPI   
                --PensionCredit = convert(numeric(7, 3),0),  
                --L52VestedCredit = convert(smallint,0),  
                PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
                IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
    --we are not checking rate item to identify hours for Pension, Health, or IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
    --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP  
    --IAPPercent = convert(money,cpas.PanOnEarn),  
    --MM 12/26/12  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
    --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
    --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),  
                --NULL PersonId,  
                --RateGroup = convert(varchar(4),cpas.RateGroup),  
                --HoursStatus = 0, --all the hours comming from CPAS are processed.  
                --LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                --LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
                ---------------------------------------------------------------------  
                --UnionMisc = null,  
                --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'CPAS'  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, E.OldEmployerNum  
                 [CheckVesting] = case when ((Year(cpas.PDate) = @Year and cpas.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
                from OPUS_Participant_List list   
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey  
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate  
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate  
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
                where [Plan]=2  
                --and cpas.mkey = @SSN  
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate  
                --and cpas.TDate between @FromDate and @ToDate  

-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	--ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	--EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	--ToDate = convert(smalldatetime, rap.TDate),
	--Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	--Received = convert(smalldatetime, rap.PDate),
	--Processed = convert(smalldatetime, rap.PDate),
	--HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	--LastName = NULL, --convert(char(50),p.LastName),
	--FirstName = NULL, --convert(char(50),p.FirstName),
	--UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	--PensionCredit = convert(numeric(7, 3),0),
	--L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	--IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	--IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	--ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	--NULL AS PersonId,
	--RateGroup = convert(varchar(4),rap.RateGroup),
	--HoursStatus = 0, --all the hours comming from CPAS are processed.
	--LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	--LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	--UnionMisc = null,
	--HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	--IAPHourlyRate = rgb.Individual
	--, Source = 'RAP'
	--, rgc.ToHealthSystem
	--, rgc.ToPensionSystem
	--, IsActiveHealth = rgc.ActiveHealth
	--, IsRetireeHealth = rgc.RetireeHealth
	--, IsPension = rgc.Pension
	--, IsIAPHourly = rgc.IAP
	--, E.OldEmployerNum
	[CheckVesting] = case when ((Year(rap.PDate) = @Year and rap.FDate < list.[Vesting_Date]) OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
	from OPUS_Participant_List list   
	inner join EADB.dbo.RAP_IAP$ rap on list.ssn = rap.mkey
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN
  
--PreMerger View.  
insert into [#PensionWorkHistory]  
select      
                --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,  
                --EmpAccountNo = convert(int, Pre.EmployerId),   
                ComputationYear = Pre.Plan_Year,  
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date  
                --ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date  
                --Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53  
                --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date   
                --Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date  
                --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id  
                SSN = convert(char(9),Pre.SSN),   
                --LastName = NULL, --convert(char(50),p.LastName),  
                --FirstName = NULL, --convert(char(50),p.FirstName),  
                --UnionCode = convert(int,Pre.UnionCode),   
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)  
                                                                                                when [Local]='L666' then convert(smallint, 4)  
                                                                                                when [Local]='L700' then convert(smallint, 6)  
                                                                                                when [Local]='L52' then convert(smallint, 7)  
                                                                                                when [Local]='L161' then convert(smallint, 8)  
                                                                                                else null end,   
                --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),  
                --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),  
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),  
                IAPHours = convert(numeric(7, 1), 0),  
                --IAPHoursA2 = convert(numeric(7, 1), 0),   
                --IAPPercent = convert(money, 0),   
                --ActiveHealthHours = convert(numeric(7, 1), 0),   
                --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?  
                --NULL PersonId,  
                --RateGroup = Pre.RateGroup,--?  
                --HoursStatus = convert(int, 0),  
                --LateMonthly = '',   
                --LateAnnual = '' ,  
                -------------------------------------------------------------------  
                --UnionMisc = convert(char(2),''),  
                --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),  
                --IAPHourlyRate = rgb.Individual  
                --, Source = 'PM  '  
                --, rgc.ToHealthSystem  
                --, rgc.ToPensionSystem  
                --, IsActiveHealth = rgc.ActiveHealth  
                --, IsRetireeHealth = rgc.RetireeHealth  
                --, IsPension = rgc.Pension  
                --, IsIAPHourly = rgc.IAP  
                --, OldEmployerNum = Pre.EmployerId  
                [CheckVesting] = case when ((Year(Pre.MergeDate) = @Year and Pre.StartDate < list.[Vesting_Date])OR list.VESTING_DATE IS NULL OR list.RECALCULATE_VESTING = 'Y') then 'Y' else 'N' end
from OPUS_Participant_List list   
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN  
                left outer join EADB.dbo.vwRateGroupClassification_all RGC   
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate   
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb   
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate  
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn  
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
--where --Pre.SSN = @SSN  
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate  
                --Pre.EndDate between @FromDate and @ToDate  
  
--select * from [#PensionWorkHistory]  
--order by todate  
--select * from [#PensionWorkHistory]  
--order by todate 
--insert into PensionWorkHistoryForStmt   


UPDATE [#PensionWorkHistory] SET CheckVesting='Y' WHERE SSN IN (SELECT DISTINCT TE.SSN FROM [#PensionWorkHistory] TE WHERE TE.CheckVesting='Y')
  

SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR, VPIO.SSN, VPIO.CheckVesting,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,  
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,  
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,  
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS IAP_HOURS,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO ORDER BY VPIO.SSN,cast(VPIO.ComputationYear as int)

DROP TABLE [#PensionWorkHistory]

END  


-------------------------------------------------------------------------------------------------------------------------------------------
--Name : Rohan Adgaonkar
--Date : 06/16/2014
---------------------------------------------------------------------------------------------------------------------------------------------

GO
/****** Object:  StoredProcedure [dbo].[usp_GetIAPSnapShotInfo]    Script Date: 06/16/2014 21:33:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GetIAPSnapShotInfo](@COMPUTATIONYEAR int) 
AS
BEGIN
SET NOCOUNT ON
--------------------------------------------------------------------------------------------

CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1) NULL,
	[LateAnnual] [varchar](1) NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 
insert into [#PensionWorkHistory]
exec usp_pensioninterface4opus_by_dates @COMPUTATIONYEAR,null,null

TRUNCATE TABLE OPUS_UAT_02.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT 

	INSERT INTO OPUS_UAT_02.DBO.SGT_ALL_IAP_WORKHISTORY_4_SNAPSHOT
	SELECT * FROM
	(
		SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN,IAPHOURS , IAPHOURSA2, IAPPERCENT, 'N' AS LATE_FLAG,FromDate,ToDate,Weeks
		FROM [#PensionWorkHistory] 
		WHERE ComputationYear = @COMPUTATIONYEAR

		UNION ALL

		SELECT EMPACCOUNTNO,COMPUTATIONYEAR,SSN, IAPHOURS, IAPHOURSA2, IAPPERCENT, 'Y' AS LATE_FLAG,FromDate,ToDate,Weeks
		FROM [#PensionWorkHistory]
		WHERE ComputationYear < @COMPUTATIONYEAR 
		--AND (LateMonthly = 'Y' or LateAnnual = 'Y')
	) A
	ORDER BY SSN,COMPUTATIONYEAR DESC,EMPACCOUNTNO


drop table [#PensionWorkHistory]

END


-----------------------------------------------------------------------------------------------
--Name - Tushar Chandak	
--Date - 06/12/2014
--Purpose - ALTER Stored Procedure To get Get Work DataFor PersonOverview
-----------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataForPersonOverview]    Script Date: 06/09/2014 21:43:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[usp_GetWorkDataForPersonOverview]
(
@SSN char(9),
@MERGER_DATE_L600 DateTime = null,
@MERGER_DATE_L666 DateTime = null,
@MERGER_DATE_L700 DateTime = null,
@MERGER_DATE_L52 DateTime = null,
@MERGER_DATE_L161 DateTime = null
)
AS
BEGIN
SET NOCOUNT ON
DECLARE @PensionWorkHistory TABLE(
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [int] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

INSERT INTO @PensionWorkHistory 
EXEC usp_PensionInterface4OPUS @SSN
declare @Last_Computation_Year int 
Select @Last_Computation_Year=MAX(ComputationYear) FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN
--sELECT * FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN 
IF @Last_Computation_Year <> 0 AND @Last_Computation_Year<2006
BEGIN
WHILE (@Last_Computation_Year <2006)
BEGIN
SET @Last_Computation_Year=@Last_Computation_Year+1
PRINT(@Last_Computation_Year)
INSERT INTO @PensionWorkHistory (ComputationYear,SSN,Source) values (@Last_Computation_Year,@SSN,'')
END

END
IF @MERGER_DATE_L600  = null OR @MERGER_DATE_L666  = null OR @MERGER_DATE_L700  = null OR @MERGER_DATE_L52 = null OR @MERGER_DATE_L161 = null
BEGIN
SELECT DISTINCT VPIO.ComputationYear AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS QUALIFIED_HOURS,
 (SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS VESTED_HOURS,
 (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalIAPHours,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END
ELSE
BEGIN
SELECT DISTINCT VPIO.ComputationYear AS YEAR,
(SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS QUALIFIED_HOURS,
 (SELECT SUM(PensionHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2)
 AS VESTED_HOURS,
 CASE WHEN YEAR(@MERGER_DATE_L600)=VPIO.ComputationYear 
      THEN
      (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN and PensionPlan=3)
      ELSE
      0
      END AS L600_HOURS,
 --(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
 CASE WHEN YEAR(@MERGER_DATE_L666)=VPIO.ComputationYear 
      THEN
      (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN and PensionPlan=4)
      ELSE
      0
      END AS L666_HOURS,
--(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
 CASE WHEN YEAR(@MERGER_DATE_L700)=VPIO.ComputationYear 
      THEN
      (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN and PensionPlan=6)
      ELSE
      0
      END AS L700_HOURS,
--(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
CASE WHEN YEAR(@MERGER_DATE_L52)=VPIO.ComputationYear 
      THEN
      (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN and PensionPlan=7)
      ELSE
      0
      END AS L52_HOURS,
--(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
CASE WHEN YEAR(@MERGER_DATE_L161)=VPIO.ComputationYear 
      THEN
      (SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN and PensionPlan=8)
      ELSE
      0
      END AS L161_HOURS,
--(SELECT SUM(PensionHours)  FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
(SELECT SUM(IAPHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalIAPHours,
(SELECT SUM(RetireeHealthHours) FROM @PensionWorkHistory WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from @PensionWorkHistory WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM @PensionWorkHistory AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END
end


END
GO

------------------------------------------------------------------------------------------------------------------------
--Modified By	:	Rohan Adgaonkar
--Modified On	:	1/25/2016
--Description	:	PIR 1024
------------------------------------------------------------------------------------------------------------------------

USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetHealthWorkHistoryForAllMpippParticipant]    Script Date: 01/25/2016 16:58:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[usp_GetHealthWorkHistoryForAllMpippParticipant] (@Year int)
AS
BEGIN
SET NOCOUNT ON

DECLARE @temp TABLE(
                [SSN] [varchar](9) NULL,
                [VESTING_DATE] DATETIME NULL,
                [RECALCULATE_VESTING] VARCHAR(1) NULL                                                         
                ) 

INSERT INTO @temp  
EXEC OPUS.dbo.[GET_ALL_PARTICIPANT_SSN_FOR_HEALTH_ELIGBILITY]

delete from OPUS_Participant_List 

INSERT INTO OPUS_Participant_List
select * from  @temp 

                                
CREATE TABLE [#PensionWorkHistory](
                [ReportID] [varchar](18) NULL,
                [EmpAccountNo] [int] NULL,
                [ComputationYear] [smallint] NULL,
                [FromDate] [smalldatetime] NULL,
                [ToDate] [smalldatetime] NULL,
                [Weeks] [char](2) NULL,
                [Received] [smalldatetime] NULL,
                [Processed] [smalldatetime] NULL,
                [HoursID] [varchar](24) NULL,
                [SSN] [char](9) NULL,
                [LastName] [varchar](50) NULL,
                [FirstName] [varchar](50) NULL,
                [UnionCode] [int] NULL,
                [PensionPlan] [smallint] NULL,
                [PensionCredit] [numeric](7, 3) NULL,
                [L52VestedCredit] [smallint] NULL,
                [PensionHours] [numeric](7, 1) NULL,
                [IAPHours] [numeric](7, 1) NULL,
                [IAPHoursA2] [numeric](7, 1) NULL,
                [IAPPercent] [money] NULL,
                [ActiveHealthHours] [numeric](7, 1) NULL,
                [RetireeHealthHours] [numeric](7, 1) NULL,
                [PersonId] [varchar](15) NULL,
                [RateGroup] [varchar](4) NULL,
                [HoursStatus] [int] NULL,
                [LateMonthly] [varchar](1) NOT NULL,
                [LateAnnual] [varchar](1) NOT NULL,
                [UnionMisc] [char](2) NULL,
                [HoursWorked] [numeric](7, 1) NULL,
                [IAPHourlyRate] [money] NULL,
                [Source] [varchar](4) NOT NULL,
                [ToHealthSystem] [int] NULL,
                [ToPensionSystem] [int] NULL,
                [IsActiveHealth] [int] NULL,
                [IsRetireeHealth] [int] NULL,
                [IsPension] [int] NULL,
                [IsIAPHourly] [int] NULL
                , [OldEmployerNum] [varchar](6)
) 

insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18), Report.ReportID),                          --old was char(10), but in order to include HP id increased to varchar(18)
                --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)
                EmpAccountNo = E.EmployerId,
                ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'
                FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                ToDate = Report.ToDate,                             -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),
                Received = Report.RecDate,                       -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime
                HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)
                SSN = convert(char(9),Hours.SSN),
                LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)
                FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)
                UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)
                IAPHours = case when report.rategroup = 8 then Hours.HoursWorked 
                                                                                when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)
                                                                                else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)
                IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)
                IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.
                ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)
                RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)
                NULL PersonId, --varchar(15) no change
                RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)
                HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.
                LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,
                LateAnnual = case when Report.ProcessDate > coalesce(PlanCutoff.cutoffdate, Report.ProcessDate) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = Hours.UnionMisc, --New field. char(2)
                HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system. 
                                                                                                                                --It is required because for those records where we do not have any rate group info
                                                                                                                                --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.
                IAPHourlyRate = rgb.Individual  --New field. money
                , Source = 'C/S '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = e.OldEmployerNum
from OPUS_Participant_List list 
                inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN
                inner join eadb.dbo.Report report on report.reportid = hours.reportid 
                --and hours.SSN = @SSN 
                --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate
                --and report.ToDate between @FromDate and @ToDate             
                left outer join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP
                inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate 
                inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate
                inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on hours.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
where EmpAccountNo not in (14002,13363,3596,3597,12904)      --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.
--Employer id for Locals Pre-Merger hours.
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)

insert into [#PensionWorkHistory]
select    
                ReportID = HPTransactions.Ber,
                --EmpAccountNo = convert(int, HPTransactions.Employer),
                EmpAccountNo = E.EmployerId,
                PensionYear = PY.PensionYear,
                FromDate = convert(smalldatetime, HPTransactions.StartDate),
                ToDate = convert(smalldatetime, HPTransactions.EndDate),
                --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),               
                Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),
                Received = convert(smalldatetime, HPTransactions.DateReceived),
                --Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.
                Processed = convert(smalldatetime,hb.Updated),
                HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),
                SSN = convert(char(9),HPTransactions.SSN),
                LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),
                FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),
                UnionCode = convert(int,HPTransactions.UnionCode),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),
                IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later 
                IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP
                IAPPercent = convert(money,HPTransactions.IAPDollars),
                ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),
                RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),
                NULL PersonId,
                RateGroup = convert(varchar(4),HPTransactions.RateGroup), 
                HoursStatus = 0,
                LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,
                --------------------------------------------------------------------------------------------------------------
                UnionMisc = HPTransactions.UNMisc,
                HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'H/P '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , e.OldEmployerNum
from OPUS_Participant_List list 
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN
                left outer join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP            
                inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate
                left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
                left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch 
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')
	(not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')
    )
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and erkey = e.employerid and hrsact = convert(numeric(7, 1), HPTransactions.Hours))
                --and HPTransactions.SSN = @SSN
                --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate
                --and HPTransactions.EndDate between @FromDate and @ToDate

--CPAS View
insert into [#PensionWorkHistory]
select    
                ReportID = left(cpas.erractid,18),
                --EmpAccountNo = convert(int, cpas.ERKey),
                EmpAccountNo = E.EmployerId,
                ComputationYear = cpas.Plan_Year, -- PY.PensionYear,
                FromDate = convert(smalldatetime, cpas.FDate),
                ToDate = convert(smalldatetime, cpas.TDate),
                Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),
                Received = convert(smalldatetime, cpas.PDate),
                Processed = convert(smalldatetime, cpas.PDate),
                HoursId = convert(varchar(24),cpas.erractid),
                SSN = convert(char(9),cpas.MKey),
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,cpas.LOC_NO),
                PensionPlan = convert(smallint, 2), -- MPI 
                PensionCredit = convert(numeric(7, 3),0),
                L52VestedCredit = convert(smallint,0),
                --PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
                PensionHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
                --IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
                IAPHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
                --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
				--we are not checking rate item to identify hours for Pension, Health, or IAP
				--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
				--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP
				IAPHoursA2 = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.IAP end),  -- $ 0.305 hourly IAP
				IAPPercent = convert(money,cpas.PanOnEarn),
				--MM 12/26/12
				--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
				--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
				--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),
				ActiveHealthHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.ActiveHealth end),
				--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),
				RetireeHealthHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.RetireeHealth end),
                NULL PersonId,
                RateGroup = convert(varchar(4),cpas.RateGroup),
                HoursStatus = 0, --all the hours comming from CPAS are processed.
                LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,
                LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,
                ---------------------------------------------------------------------
                UnionMisc = null,
                HoursWorked = convert(numeric(7, 1), cpas.HRSACT),
                IAPHourlyRate = rgb.Individual
                , Source = 'CPAS'
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , E.OldEmployerNum
                from OPUS_Participant_List list 
                inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey
                inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
                left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate
                left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate
                left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
                --left outer join pid.dbo.Person p on cpas.mkey = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
                where [Plan]=2
                --and cpas.mkey = @SSN
                --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate
                --and cpas.TDate between @FromDate and @ToDate
                
-- RAP IAP$
insert into [#PensionWorkHistory]
select	
	ReportID = left(rap.erractid,18),
	--EmpAccountNo = convert(int, cpas.ERKey),
	EmpAccountNo = isnull(E.EmployerId,'0'),
	ComputationYear = rap.Plan_Year, -- PY.PensionYear,
	FromDate = convert(smalldatetime, rap.FDate),
	ToDate = convert(smalldatetime, rap.TDate),
	Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),
	Received = convert(smalldatetime, rap.PDate),
	Processed = convert(smalldatetime, rap.PDate),
	HoursId = convert(varchar(24),rap.erractid),
	SSN = convert(char(9),rap.MKey),
	LastName = NULL, --convert(char(50),p.LastName),
	FirstName = NULL, --convert(char(50),p.FirstName),
	UnionCode = convert(int,rap.LOC_NO),
	PensionPlan = convert(smallint, 2), -- MPI 
	PensionCredit = convert(numeric(7, 3),0),
	L52VestedCredit = convert(smallint,0),
	PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours
	IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later
	--MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth
	--we are not checking rate item to identify hours for Pension, Health, or IAP
	--IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP
	IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP
	IAPPercent = convert(money,rap.PanOnEarn),
	--MM 12/26/12
	--ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),
	--RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),
	ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),
	RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),
	NULL AS PersonId,
	RateGroup = convert(varchar(4),rap.RateGroup),
	HoursStatus = 0, --all the hours comming from CPAS are processed.
	LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,
	LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,
	---------------------------------------------------------------------
	UnionMisc = null,
	HoursWorked = convert(numeric(7, 1), rap.HRSACT),
	IAPHourlyRate = rgb.Individual
	, Source = 'RAP'
	, rgc.ToHealthSystem
	, rgc.ToPensionSystem
	, IsActiveHealth = rgc.ActiveHealth
	, IsRetireeHealth = rgc.RetireeHealth
	, IsPension = rgc.Pension
	, IsIAPHourly = rgc.IAP
	, E.OldEmployerNum
	from OPUS_Participant_List list 
	inner join EADB.dbo.RAP_IAP$ rap on rap.mkey = list.SSN
	left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP
	left outer join eadb.dbo.vwRateGroupClassification_all RGC 
		on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate 
	left outer join eadb.dbo.vwRateGroupBreakDown_all rgb 
		on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate
	inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate
	left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate
	left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate
	--left outer join pid.dbo.Person p on cpas.mkey = p.ssn  
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance  
	where [Plan]=2
	--and rap.mkey = @SSN

               
--PreMerger View.
insert into [#PensionWorkHistory]
select    
                ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,
                EmpAccountNo = convert(int, Pre.EmployerId), 
                ComputationYear = Pre.Plan_Year,
                FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date
                ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date
                Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53
                Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date 
                Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date
                HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id
                SSN = convert(char(9),Pre.SSN), 
                LastName = NULL, --convert(char(50),p.LastName),
                FirstName = NULL, --convert(char(50),p.FirstName),
                UnionCode = convert(int,Pre.UnionCode), 
                PensionPlan = case when [Local]='L600' then convert(smallint, 3)
                                                                                                when [Local]='L666' then convert(smallint, 4)
                                                                                                when [Local]='L700' then convert(smallint, 6)
                                                                                                when [Local]='L52' then convert(smallint, 7)
                                                                                                when [Local]='L161' then convert(smallint, 8)
                                                                                                else null end, 
                PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),
                L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),
                PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHours = convert(numeric(7, 1), 0),
                IAPHoursA2 = convert(numeric(7, 1), 0), 
                IAPPercent = convert(money, 0), 
                ActiveHealthHours = convert(numeric(7, 1), 0), 
                RetireeHealthHours = convert(numeric(7, 1), 0), -- ?
                NULL PersonId,
                RateGroup = Pre.RateGroup,--?
                HoursStatus = convert(int, 0),
                LateMonthly = '', 
                LateAnnual = '' ,
                -------------------------------------------------------------------
                UnionMisc = convert(char(2),''),
                HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),
                IAPHourlyRate = rgb.Individual
                , Source = 'PM  '
                , rgc.ToHealthSystem
                , rgc.ToPensionSystem
                , IsActiveHealth = rgc.ActiveHealth
                , IsRetireeHealth = rgc.RetireeHealth
                , IsPension = rgc.Pension
                , IsIAPHourly = rgc.IAP
                , OldEmployerNum = Pre.EmployerId
from OPUS_Participant_List list 
                inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN
                left outer join EADB.dbo.vwRateGroupClassification_all RGC 
                                on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate 
                left outer join EADB.dbo.vwRateGroupBreakDown_all rgb 
                                on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate
                --left outer join pid.dbo.Person p on Pre.ssn = p.ssn
                --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance
--where --Pre.SSN = @SSN
                --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate
                --Pre.EndDate between @FromDate and @ToDate

--select * from [#PensionWorkHistory]
--order by todate
--select * from [#PensionWorkHistory]
--order by todate

--insert into PensionWorkHistoryForStmt 
 

DECLARE @Local600MergerYear INT
SELECT @Local600MergerYear =  YEAR(MERGER_DATE) FROM OPUS..SGT_PLAN WHERE PLAN_ID = 3

DECLARE @Local666MergerYear INT
SELECT @Local666MergerYear =  YEAR(MERGER_DATE) FROM OPUS..SGT_PLAN WHERE PLAN_ID = 4

DECLARE @Local700MergerYear INT
SELECT @Local700MergerYear =  YEAR(MERGER_DATE) FROM OPUS..SGT_PLAN WHERE PLAN_ID = 6

DECLARE @Local52MergerYear INT
SELECT @Local52MergerYear =  YEAR(MERGER_DATE) FROM OPUS..SGT_PLAN WHERE PLAN_ID = 7

DECLARE @Local161MergerYear INT
SELECT @Local161MergerYear =  YEAR(MERGER_DATE) FROM OPUS..SGT_PLAN WHERE PLAN_ID = 8


SELECT A.YEAR,A.SSN,SUM(A.idecTotalHealthHours) idecTotalHealthHours ,SUM(A.idcPensionHours_healthBatch) idcPensionHours_healthBatch,
SUM(A.idcIAPHours_healthBatch) idcIAPHours_healthBatch, a.PlanStartDate FROM
(

--PENSION
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,VPIO.SSN,
ISNULL((SELECT SUM(RetireeHealthHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2),0)
 AS idecTotalHealthHours,
ISNULL((SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2),0)  AS idcPensionHours_healthBatch,
ISNULL((SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2),0) AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN

UNION ALL

--LOCAL 600
SELECT DISTINCT cast(@Local600MergerYear as int) AS YEAR,VPIO.SSN,
0 AS idecTotalHealthHours,
0 AS idcPensionHours_healthBatch,
0 AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN

UNION ALL

--LOCAL 666
SELECT DISTINCT cast(@Local666MergerYear as int) AS YEAR,VPIO.SSN,
0 AS idecTotalHealthHours,
0 AS idcPensionHours_healthBatch,
0 AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN

UNION ALL

--LOCAL 700
SELECT DISTINCT cast(@Local700MergerYear as int) AS YEAR,VPIO.SSN,
0 AS idecTotalHealthHours,
0 AS idcPensionHours_healthBatch,
0 AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN


UNION ALL
--LOCAL 52
SELECT DISTINCT cast(@Local52MergerYear as int) AS YEAR,VPIO.SSN,
0 AS idecTotalHealthHours,
0 idcPensionHours_healthBatch,
0 AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN


UNION ALL
--LOCAL 161
SELECT DISTINCT cast(@Local161MergerYear as int) AS YEAR,VPIO.SSN,
0 AS idecTotalHealthHours,
0 idcPensionHours_healthBatch,
0 AS idcIAPHours_healthBatch,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate
FROM [#PensionWorkHistory] AS VPIO 
INNER JOIN OPUS..SGT_PERSON SP ON SP.SSN = VPIO.SSN
)A 
WHERE A.YEAR <= @Year
GROUP BY A.YEAR,A.SSN,A.PlanStartDate
ORDER BY A.SSN,A.YEAR

END
go


USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWorkDataTillGivenDate]    Script Date: 01/25/2016 17:01:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---------------------------------------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------------------------------
-- Date - 1/25/2016
-- Purpose - SP usp_GetWorkDataTillGivenDate
-- Updated For :  PIR 1024
----------------------------------------------------------------------------------------------------------------------------------

ALTER PROC [dbo].[usp_GetWorkDataTillGivenDate](@SSN char(10),@PLANCODE varchar(20),@RETIREMENT_DATE DATETIME=null,@EVALUATION_DATE DATETIME=null,@VESTING_DATE DATETIME=null)
AS
BEGIN
SET NOCOUNT ON
CREATE TABLE [#PensionWorkHistory](
	[ReportID] [varchar](18) NULL,
	[EmpAccountNo] [int] NULL,
	[ComputationYear] [smallint] NULL,
	[FromDate] [smalldatetime] NULL,
	[ToDate] [smalldatetime] NULL,
	[Weeks] [char](2) NULL,
	[Received] [smalldatetime] NULL,
	[Processed] [smalldatetime] NULL,
	[HoursID] [varchar](24) NULL,
	[SSN] [char](9) NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[UnionCode] [int] NULL,
	[PensionPlan] [smallint] NULL,
	[PensionCredit] [numeric](7, 3) NULL,
	[L52VestedCredit] [smallint] NULL,
	[PensionHours] [numeric](7, 1) NULL,
	[IAPHours] [numeric](7, 1) NULL,
	[IAPHoursA2] [numeric](7, 1) NULL,
	[IAPPercent] [money] NULL,
	[ActiveHealthHours] [numeric](7, 1) NULL,
	[RetireeHealthHours] [numeric](7, 1) NULL,
	[PersonId] [varchar](15) NULL,
	[RateGroup] [varchar](4) NULL,
	[HoursStatus] [int] NULL,
	[LateMonthly] [varchar](1)  NULL,
	[LateAnnual] [varchar](1)  NULL,
	[UnionMisc] [char](2) NULL,
	[HoursWorked] [numeric](7, 1) NULL,
	[IAPHourlyRate] [money] NULL,
	[Source] [varchar](4) NOT NULL,
	[ToHealthSystem] [int] NULL,
	[ToPensionSystem] [int] NULL,
	[IsActiveHealth] [int] NULL,
	[IsRetireeHealth] [int] NULL,
	[IsPension] [int] NULL,
	[IsIAPHourly] [int] NULL
	, [OldEmployerNum] [varchar](6) null
) 

IF @RETIREMENT_DATE = '01/01/1753'
BEGIN
	SET @RETIREMENT_DATE=NULL
END


IF @EVALUATION_DATE = NULL
BEGIN
	SET @RETIREMENT_DATE='01/01/1753'
END

IF @VESTING_DATE = NULL
BEGIN
	SET @VESTING_DATE='01/01/1753'
END

INSERT INTO [#PensionWorkHistory] 
EXEC usp_PensionInterface4OPUS @SSN

IF @RETIREMENT_DATE <> NULL OR @RETIREMENT_DATE <> '' OR @RETIREMENT_DATE <> '01/01/1753'
BEGIN
	--PIR 887 Get Last Day Of Week
	--SET @RETIREMENT_DATE = DATEADD(DD, 1, DATEADD(WW, DATEDIFF(WW, 0, @RETIREMENT_DATE), 4))
	SET @RETIREMENT_DATE = DATEADD(DAY, 7 - DATEPART(WEEKDAY, @RETIREMENT_DATE), CAST(@RETIREMENT_DATE AS DATE))

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2 and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
--PIR 753
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_HOURS,
0.0 AS L600_HOURS,
0.0 AS L666_HOURS,
0.0 AS L700_HOURS,
0.0 AS L52_HOURS,
0.0 AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2)) AS idecTotalHealthHours,--PIR 
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')) as iintLateHourCount,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and ToDate <= @RETIREMENT_DATE) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7 and ToDate <= @RETIREMENT_DATE)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8 and ToDate <= @RETIREMENT_DATE)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear  and ToDate <= @RETIREMENT_DATE) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2)) AS idecTotalHealthHours,(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN AND VPIO.ToDate <= @RETIREMENT_DATE ORDER BY YEAR
END
END

ELSE 
BEGIN

IF @PLANCODE='MPIPP' 
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2)  AS QUALIFIED_HOURS,
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan = 2) AS VESTED_HOURS,
--PIR 753
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_HOURS,
--(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_HOURS,
0.0 AS L600_HOURS,
0.0 AS L666_HOURS,
0.0 AS L700_HOURS,
0.0 AS L52_HOURS,
0.0 AS L161_HOURS,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3) AS L600_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4) AS L666_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6) AS L700_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7) AS L52_PensionCredits,
(SELECT SUM(PensionCredit)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8) AS L161_PensionCredits,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2)) AS idecTotalHealthHours,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS idecIAPHours,
(SELECT COUNT(*) as Count_Late_Hours FROM [#PensionWorkHistory]  where SSN=VPIO.SSN and cast(Received as datetime) > @EVALUATION_DATE and cast(ToDate as datetime) <= @VESTING_DATE and (LateMonthly='Y' or LateAnnual='Y')) as iintLateHourCount,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE IF @PLANCODE='IAP'
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
(SELECT SUM(IAPHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS QUALIFIED_HOURS,
(SELECT SUM(IAPHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS VESTED_HOURS,
(SELECT SUM(IAPHoursA2)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_HOURSA2,
(SELECT SUM(IAPPercent)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) AS IAP_PERCENT,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan IN (2)) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

ELSE
BEGIN
SELECT DISTINCT cast(VPIO.ComputationYear as int) AS YEAR,
CASE WHEN @PLANCODE='Local600' THEN
(SELECT SUM(PensionHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=3)
WHEN @PLANCODE='Local666' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=4)
WHEN @PLANCODE='Local700' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=6)
WHEN @PLANCODE='Local52' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=7)
WHEN @PLANCODE='Local161' THEN
(SELECT SUM(PensionHours)  FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=8)
ELSE NULL END AS QUALIFIED_HOURS,
CASE WHEN @PLANCODE='IAP' THEN
(SELECT SUM(IAPHours)AS VESTED_HOURS FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear) 
ELSE NULL END AS VESTED_HOURS,
(SELECT SUM(RetireeHealthHours) FROM [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND ComputationYear=VPIO.ComputationYear and PensionPlan=2) AS idecTotalHealthHours,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=VPIO.SSN AND cast(ComputationYear as int) = cast(VPIO.ComputationYear as int)) as firstHourReported
FROM [#PensionWorkHistory] AS VPIO where VPIO.SSN=@SSN ORDER BY YEAR
END

END

DROP TABLE [#PensionWorkHistory]
END  
go
-------------------------------------------------------------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------------------------------
-- Name - Suresh Kolape
-- Date - 09/04/2015
-- Purpose - SP usp_GETYEARENDEXTRACTIONDATAYEARLY Change
-- Updated By : Suresh for PIR 753
----------------------------------------------------------------------------------------------------------------------------------
ALTER PROC [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]    
AS    
BEGIN    
SET NOCOUNT ON    
    
DECLARE @temp TABLE(    
 [SSN] [varchar](9) NULL,    
 [PensionYear] [int] NULL     
 )     
  
INSERT INTO @temp      
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO    
    
declare @year int    
set select top(1) @year = PensionYear from @temp    
    
delete from OPUS_AnnualStmt_Participant_List    
    
INSERT INTO OPUS_AnnualStmt_Participant_List    
select * from  @temp     
   
declare @PlanStartDate datetime    
declare @PlanEndDate datetime    
declare @CutOffDate datetime    
select @PlanStartDate = cutoffdate from eadb.dbo.period where qualifyingenddate = eadb.dbo.fn_PlanYearEndDate(@year-1)  
set @PlanStartDate = DATEADD(DAY,1,@PlanStartDate)  
select @PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)    
select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate    
      
CREATE TABLE [#PensionWorkHistory](    
 --[ReportID] [varchar](18) NULL,    
 [EmpAccountNo] [int] NULL,    
 [ComputationYear] [int] NULL,    
 [FromDate] [smalldatetime] NULL,    
 [ToDate] [smalldatetime] NULL,    
 [Weeks] [char](2) NULL,    
 --[Received] [smalldatetime] NULL,    
 [Processed] [smalldatetime] NULL,    
 --[HoursID] [varchar](24) NULL,    
 [SSN] [varchar](9) NULL,    
 --[LastName] [varchar](50) NULL,    
 --[FirstName] [varchar](50) NULL,    
 [UnionCode] [int] NULL,    
 [PensionPlan] [smallint] NULL,    
 --[PensionCredit] [numeric](7, 3) NULL,    
 --[L52VestedCredit] [smallint] NULL,    
 [PensionHours] [numeric](7, 1) NULL,    
 [IAPHours] [numeric](7, 1) NULL,    
 --[IAPHoursA2] [numeric](7, 1) NULL,    
 --[IAPPercent] [money] NULL,    
 --[ActiveHealthHours] [numeric](7, 1) NULL,    
 --[RetireeHealthHours] [numeric](7, 1) NULL,    
 --[PersonId] [varchar](15) NULL,    
 --[RateGroup] [varchar](4) NULL,    
 --[HoursStatus] [int] NULL,    
 [LateMonthly] [varchar](1) NOT NULL,    
 [LateAnnual] [varchar](1) NOT NULL,  
[EmployerName] [varchar] (255) NULL  
--[UnionMisc] [char](2) NULL,    
 --[HoursWorked] [numeric](7, 1) NULL,    
 --[IAPHourlyRate] [money] NULL,    
 --[Source] [varchar](4) NOT NULL,    
 --[ToHealthSystem] [int] NULL,    
 --[ToPensionSystem] [int] NULL,    
 --[IsActiveHealth] [int] NULL,    
 --[IsRetireeHealth] [int] NULL,    
 --[IsPension] [int] NULL,    
 --[IsIAPHourly] [int] NULL    
 --, [OldEmployerNum] [varchar](6)    
)     
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)    
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'    
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),    
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)    
 SSN = convert(char(9),Hours.SSN),    
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)    
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)    
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)    
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked     
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)    
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)    
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)    
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.    
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)    
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)    
 --NULL PersonId, --varchar(15) no change    
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)    
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.    
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,    
 LateAnnual = case when Report.ProcessDate > coalesce(PlanCutoff.cutoffdate, Report.ProcessDate) then 'Y' else '' end ,  
EmployerName = E.EmployerName   
 --------------------------------------------------------------------------------------------------------------    
 --UnionMisc = Hours.UnionMisc, --New field. char(2)    
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.     
        --It is required because for those records where we do not have any rate group info    
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.    
 --IAPHourlyRate = rgb.Individual  --New field. money    
 --, Source = 'C/S '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN    
 inner join eadb.dbo.Report report on report.reportid = hours.reportid     
 --and hours.SSN = @SSN     
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate    
 --and report.ToDate between @FromDate and @ToDate     
 left outer join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP    
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate     
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate    
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate     
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
where EmpAccountNo not in (14002,13363,3596,3597,12904) and    
--Report.RecDate <= isnull(@CutOffDate,Report.RecDate)    
Report.ProcessDate <= isnull(@CutOffDate,Report.RecDate)    
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.    
--Employer id for Locals Pre-Merger hours.    
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)    
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = HPTransactions.Ber,    
 ----EmpAccountNo = convert(int, HPTransactions.Employer),    
 EmpAccountNo = E.EmployerId,    
 PensionYear = PY.PensionYear,    
 FromDate = convert(smalldatetime, HPTransactions.StartDate),    
 ToDate = convert(smalldatetime, HPTransactions.EndDate),    
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),     
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),    
 --Received = convert(smalldatetime, HPTransactions.DateReceived),    
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.    
 --Processed = convert(smalldatetime,hb.Updated),    
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),    
 SSN = convert(char(9),HPTransactions.SSN),    
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),    
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),    
 UnionCode = convert(int,HPTransactions.UnionCode),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),    
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later     
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,HPTransactions.IAPDollars),    
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),    
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),     
 --HoursStatus = 0,    
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,    
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,  
EmployerName = E.EmployerName    
 ----------------------------------------------------------------------------------------------------------------    
 --UnionMisc = HPTransactions.UNMisc,    
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'H/P '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN    
 left outer join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP     
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch     
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')  
      (not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')  
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')  
    )  
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and erkey = e.employerid and hrsact = convert(numeric(7, 1), HPTransactions.Hours))  
--and HPTransactions.SSN = @SSN    
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate    
 --and HPTransactions.EndDate between @FromDate and @ToDate    
    
--CPAS View    
insert into [#PensionWorkHistory]    
select     
 --ReportID = left(cpas.erractid,18),    
 ----EmpAccountNo = convert(int, cpas.ERKey),    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,    
 FromDate = convert(smalldatetime, cpas.FDate),    
 ToDate = convert(smalldatetime, cpas.TDate),    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),    
 --Received = convert(smalldatetime, cpas.PDate),    
 Processed = convert(smalldatetime, cpas.PDate),    
 --HoursId = convert(varchar(24),cpas.erractid),    
 SSN = convert(char(9),cpas.MKey),    
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,cpas.LOC_NO),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 --PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours    
 PensionHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 --IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later    
 IAPHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth    
 ----we are not checking rate item to identify hours for Pension, Health, or IAP    
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,cpas.PanOnEarn),    
 ----MM 12/26/12    
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),    
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),    
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),cpas.RateGroup),    
 --HoursStatus = 0, --all the hours comming from CPAS are processed.    
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,    
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
EmployerName = E.EmployerName   
 -----------------------------------------------------------------------    
 --UnionMisc = null,    
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'CPAS'    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, E.OldEmployerNum    
 from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey    
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 where [Plan]=2    
 --and cpas.mkey = @SSN    
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate    
 --and cpas.TDate between @FromDate and @ToDate    
  
-- RAP IAP$  
insert into [#PensionWorkHistory]  
select        
      --ReportID = left(rap.erractid,18),  
      --EmpAccountNo = convert(int, cpas.ERKey),  
      EmpAccountNo = isnull(E.EmployerId,'0'),  
      ComputationYear = rap.Plan_Year, -- PY.PensionYear,  
      FromDate = convert(smalldatetime, rap.FDate),  
      ToDate = convert(smalldatetime, rap.TDate),  
      Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),  
      --Received = convert(smalldatetime, rap.PDate),  
      Processed = convert(smalldatetime, rap.PDate),  
      --HoursId = convert(varchar(24),rap.erractid),  
      SSN = convert(char(9),rap.MKey),  
      --LastName = NULL, --convert(char(50),p.LastName),  
      --FirstName = NULL, --convert(char(50),p.FirstName),  
      UnionCode = convert(int,rap.LOC_NO),  
      PensionPlan = convert(smallint, 2), -- MPI   
      --PensionCredit = convert(numeric(7, 3),0),  
      --L52VestedCredit = convert(smallint,0),  
      PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
      IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
      --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
      --we are not checking rate item to identify hours for Pension, Health, or IAP  
      --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
      --IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP  
      --IAPPercent = convert(money,rap.PanOnEarn),  
      --MM 12/26/12  
      --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
      --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
      --ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --NULL AS PersonId,  
      --RateGroup = convert(varchar(4),rap.RateGroup),  
      --HoursStatus = 0, --all the hours comming from CPAS are processed.  
      LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,  
      LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,  
      EmployerName = E.EmployerName  
      ---------------------------------------------------------------------  
      --UnionMisc = null,  
      --HoursWorked = convert(numeric(7, 1), rap.HRSACT),  
      --IAPHourlyRate = rgb.Individual  
      --, Source = 'RAP'  
      --, rgc.ToHealthSystem  
      --, rgc.ToPensionSystem  
      --, IsActiveHealth = rgc.ActiveHealth  
      --, IsRetireeHealth = rgc.RetireeHealth  
      --, IsPension = rgc.Pension  
      --, IsIAPHourly = rgc.IAP  
      --, E.OldEmployerNum  
      from OPUS_AnnualStmt_Participant_List list    
      inner join EADB.dbo.RAP_IAP$ rap on rap.mkey=list.SSN  
      left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
      left outer join eadb.dbo.vwRateGroupClassification_all RGC   
         on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate   
      left outer join eadb.dbo.vwRateGroupBreakDown_all rgb   
            on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate  
      inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
      left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate  
      left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
      --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
      where [Plan]=2  
      --and rap.mkey = @SSN  
  
    
--PreMerger View.    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,    
 EmpAccountNo = convert(int, Pre.EmployerId),     
 ComputationYear = Pre.Plan_Year,    
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date    
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53    
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date     
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date    
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id    
 SSN = convert(char(9),Pre.SSN),     
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,Pre.UnionCode),     
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)    
      when [Local]='L666' then convert(smallint, 4)    
      when [Local]='L700' then convert(smallint, 6)    
      when [Local]='L52' then convert(smallint, 7)    
      when [Local]='L161' then convert(smallint, 8)    
      else null end,     
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),    
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),    
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),    
 IAPHours = convert(numeric(7, 1), 0),    
 --IAPHoursA2 = convert(numeric(7, 1), 0),     
 --IAPPercent = convert(money, 0),     
 --ActiveHealthHours = convert(numeric(7, 1), 0),     
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?    
 --NULL PersonId,    
 --RateGroup = Pre.RateGroup,--?    
 --HoursStatus = convert(int, 0),    
 LateMonthly = '',     
 LateAnnual = '' ,  
EmployerName = E.EmployerName   
 -------------------------------------------------------------------    
 --UnionMisc = convert(char(2),''),    
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'PM  '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = Pre.EmployerId    
from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId  
--left outer join pid.dbo.Person p on Pre.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
--where --Pre.SSN = @SSN    
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate    
 --Pre.EndDate between @FromDate and @ToDate    
    
--select isnull(EmpAccountNo, 0) as EmpAccountNo, ComputationYear, SSN, UnionCode, PensionHours, case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,   
--isnull(EmployerName, '') as EmployerName from [#PensionWorkHistory] where   
--PensionPlan = 2   
                          
 /*
 select ssn,computationyear,
sum(isnull(pensionhours,0.0)) TotalPensionHours,  
sum(isnull(latehours,0.0)) TotalLateHours,
UnionCode,EmpAccountno,EmployerName,
PlanStartDate,firstHourReported
 from         
(        
select ssn,computationyear,pensionhours,
case when (LateAnnual = 'Y' or v.ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,  
unioncode,empaccountno,e.employername,processed,v.PensionPlan,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN AND cast(ComputationYear as int) = cast(v.ComputationYear as int)) as firstHourReported   
from [#PensionWorkHistory] v        
left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid    
where v.ComputationYear <= @year and PensionPlan = 2     
)a        
group by ssn,computationyear,unioncode,empaccountno,employername,PensionPlan,PlanStartDate,firstHourReported
 */

 
 
--Below script we use for 2014 statements but for remaining statements i have temporarily commented the code 
--SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
--FirstHoursReported = MIN(FromDate)
--into #Summary
--FROM (
--SELECT SSN, ComputationYear, PensionHours, 
--LateHours = case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate THEN PensionHours ELSE 0 END,
--FromDate FROM #PensionWorkHistory
--WHERE PensionPlan = 2 AND ComputationYear <= @year
--) A GROUP BY SSN, ComputationYear



SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
FirstHoursReported = MIN(FromDate)
into #Summary
FROM (
SELECT SSN, ComputationYear, CASE WHEN PensionPlan = 2 THEN PensionHours ELSE 0 END AS PensionHours, 
LateHours = CASE WHEN PensionPlan = 2 
			THEN case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate 
												THEN PensionHours ELSE 0 END
			ELSE 0 END,
FromDate FROM #PensionWorkHistory
WHERE ComputationYear <= @year
) A GROUP BY SSN, ComputationYear

SELECT S.SSN, S.ComputationYear, TotalPensionHours, TotalLateHours,
PlanStartDate = Y.FirstHoursReported, S.FirstHoursReported as firstHourReported
FROM #Summary S 
INNER JOIN 
(SELECT SSN, FirstHoursReported = MIN(FirstHoursReported) FROM #Summary GROUP BY SSN) Y ON S.SSN = Y.SSN 
    
drop table [#PensionWorkHistory]  
  
end 

go

USE [EADB]
GO
/****** Object:  StoredProcedure [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY]    Script Date: 05/04/2016 11:42:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------------------------------------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------------------------------

-- Date - 05/04/2016
-- Purpose - SP usp_GETYEARENDEXTRACTIONDATAYEARLY Change
-- Updated for : PIR 1052
----------------------------------------------------------------------------------------------------------------------------------
ALTER PROC [dbo].[usp_GETYEARENDEXTRACTIONDATAYEARLY] @TEMPTABLE VARCHAR(1) = 'N' 
AS    
BEGIN    
SET NOCOUNT ON    
    
DECLARE @temp TABLE(    
 [SSN] [varchar](9) NULL,    
 [PensionYear] [int] NULL     
 )     
  
INSERT INTO @temp      
EXEC OPUS.dbo.GET_YEAR_END_DATA_EXTRACTION_INFO @TEMPTABLE  
    
declare @year int    
set select top(1) @year = PensionYear from @temp    
    
delete from OPUS_AnnualStmt_Participant_List    
    
INSERT INTO OPUS_AnnualStmt_Participant_List    
select * from  @temp     
   
declare @PlanStartDate datetime    
declare @PlanEndDate datetime    
declare @CutOffDate datetime    
select @PlanStartDate = cutoffdate from eadb.dbo.period where qualifyingenddate = eadb.dbo.fn_PlanYearEndDate(@year-1)  
set @PlanStartDate = DATEADD(DAY,1,@PlanStartDate)  
select @PlanEndDate = eadb.dbo.fn_PlanYearEndDate(@year)    
select @CutOffDate = cutoffdate from eadb.dbo.period where qualifyingenddate = @PlanEndDate    
      
CREATE TABLE [#PensionWorkHistory](    
 --[ReportID] [varchar](18) NULL,    
 [EmpAccountNo] [int] NULL,    
 [ComputationYear] [int] NULL,    
 [FromDate] [smalldatetime] NULL,    
 [ToDate] [smalldatetime] NULL,    
 [Weeks] [char](2) NULL,    
 --[Received] [smalldatetime] NULL,    
 [Processed] [smalldatetime] NULL,    
 --[HoursID] [varchar](24) NULL,    
 [SSN] [varchar](9) NULL,    
 --[LastName] [varchar](50) NULL,    
 --[FirstName] [varchar](50) NULL,    
 [UnionCode] [int] NULL,    
 [PensionPlan] [smallint] NULL,    
 --[PensionCredit] [numeric](7, 3) NULL,    
 --[L52VestedCredit] [smallint] NULL,    
 [PensionHours] [numeric](7, 1) NULL,    
 [IAPHours] [numeric](7, 1) NULL,    
 --[IAPHoursA2] [numeric](7, 1) NULL,    
 --[IAPPercent] [money] NULL,    
 --[ActiveHealthHours] [numeric](7, 1) NULL,    
 --[RetireeHealthHours] [numeric](7, 1) NULL,    
 --[PersonId] [varchar](15) NULL,    
 --[RateGroup] [varchar](4) NULL,    
 --[HoursStatus] [int] NULL,    
 [LateMonthly] [varchar](1) NOT NULL,    
 [LateAnnual] [varchar](1) NOT NULL,  
[EmployerName] [varchar] (255) NULL  
--[UnionMisc] [char](2) NULL,    
 --[HoursWorked] [numeric](7, 1) NULL,    
 --[IAPHourlyRate] [money] NULL,    
 --[Source] [varchar](4) NOT NULL,    
 --[ToHealthSystem] [int] NULL,    
 --[ToPensionSystem] [int] NULL,    
 --[IsActiveHealth] [int] NULL,    
 --[IsRetireeHealth] [int] NULL,    
 --[IsPension] [int] NULL,    
 --[IsIAPHourly] [int] NULL    
 --, [OldEmployerNum] [varchar](6)    
)     
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18), Report.ReportID),  --old was char(10), but in order to include HP id increased to varchar(18)    
 --EmpAccountNo = convert(int,Report.EmpAccountNo),  --old was char(6)    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = PY.PensionYear,  --smallint ,  old name was 'PensionYear'    
 FromDate = Report.FromDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 ToDate = Report.ToDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Report.FromDate, Report.ToDate)/7.0,0))),    
 --Received = Report.RecDate,  -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 Processed = Report.ProcessDate, -- old was char(8) yyyymmdd, now no conversion it is smalldatetime    
 --HoursID = convert(varchar(24), Hours.HoursID),  --old size was char(10), but in order to include HP id increased to varchar(24)    
 SSN = convert(char(9),Hours.SSN),    
 --LastName = Hours.LastName,   --old was char(18), now no conversion took default which is varchar(50)    
 --FirstName = Hours.FirstName, --old was char(14), now no conversion took default which is varchar(50)    
 UnionCode = Hours.UnionCode, --old was char(4), now no conversion it is 'int'    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension), -- old was numeric(18,1)    
 IAPHours = case when report.rategroup = 8 then Hours.HoursWorked     
     when report.rategroup = 66 or report.rategroup = 42 then convert(numeric(7, 1), 0)    
     else convert(numeric(7, 1), Hours.HoursWorked * rgc.Pension) end,  --old was numeric(18,1)    
 --IAPHoursA2 = convert(numeric(7, 1), Hours.HoursWorked * rgc.IAP),  -- $ 0.305 hourly IAP --old was numeric(18,1)    
 --IAPPercent = Hours.IAPValue,  --old was char(9), now no conversion it is money.    
 --ActiveHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.ActiveHealth), --old was numeric(18,1)    
 --RetireeHealthHours = convert(numeric(7, 1), Hours.HoursWorked * rgc.RetireeHealth), --old was numeric(18,1)    
 --NULL PersonId, --varchar(15) no change    
 --RateGroup = convert(varchar(4), report.RateGroup), -- old was char(4)    
 --HoursStatus = Hours.Status, --int now, old was tinyint -- 0 = Processed/posted , > 0 (1,2,...) unprocessed.    
 LateMonthly = case when Report.RecDate > coalesce(Period.cutoffdate, Report.RecDate) then 'Y' else '' end,    
 LateAnnual = case when Report.ProcessDate > coalesce(PlanCutoff.cutoffdate, Report.ProcessDate) then 'Y' else '' end ,  
EmployerName = E.EmployerName   
 --------------------------------------------------------------------------------------------------------------    
 --UnionMisc = Hours.UnionMisc, --New field. char(2)    
 --HoursWorked = convert(numeric(7, 1), Hours.HoursWorked), --New field to show whatever hours we have in system.     
        --It is required because for those records where we do not have any rate group info    
        --it will show 0 for PensionHours, ActiveHealthHours, and RetireeHealthHours.    
 --IAPHourlyRate = rgb.Individual  --New field. money    
 --, Source = 'C/S '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
 inner join eadb.dbo.Hours hours  on list.SSN = hours.SSN    
 inner join eadb.dbo.Report report on report.reportid = hours.reportid     
 --and hours.SSN = @SSN     
 --and (hours.SSN = @SSN or @SSN is null) and report.ToDate between @FromDate and @ToDate    
 --and report.ToDate between @FromDate and @ToDate     
 left outer join EADB.dbo.Employer E on convert(int,Report.EmpAccountNo) = E.EmployerId  -- taking care of Alpha numeric employer id in HP    
 inner join EADB.dbo.vwRateGroupClassification_all RGC on report.RateGroup = RGC.RateGroup and report.ToDate between rgc.FromDate and rgc.ToDate     
 inner join EADB.dbo.vwRateGroupBreakDown_all rgb on report.rategroup = rgb.rategroup and report.todate between rgb.FromDate and rgb.ToDate    
 inner join eadb.dbo.Period Period on Report.todate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join eadb.dbo.PensionYear PY on Report.todate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate     
 --left outer join pid.dbo.Person p on hours.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on hours.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
where EmpAccountNo not in (14002,13363,3596,3597,12904) and    
--Report.RecDate <= isnull(@CutOffDate,Report.RecDate)    
Report.ProcessDate <= isnull(@CutOffDate,Report.RecDate)    
 --Excluding pre-merger data to avoid duplication, since it is comming from PremergerView.    
--Employer id for Locals Pre-Merger hours.    
--(L161=14002; L52=13363; L600=3596; L666=3597; L700=12904)    
    
insert into [#PensionWorkHistory]    
select     
 --ReportID = HPTransactions.Ber,    
 ----EmpAccountNo = convert(int, HPTransactions.Employer),    
 EmpAccountNo = E.EmployerId,    
 PensionYear = PY.PensionYear,    
 FromDate = convert(smalldatetime, HPTransactions.StartDate),    
 ToDate = convert(smalldatetime, HPTransactions.EndDate),    
 --Weeks = datediff(week, HPTransactions.StartDate, dateadd(day,1,HPTransactions.EndDate)),     
 Weeks = convert(char(2), convert(int , round(DateDiff(day, HPTransactions.StartDate, HPTransactions.EndDate)/7.0,0))),    
 --Received = convert(smalldatetime, HPTransactions.DateReceived),    
 Processed = convert(smalldatetime, HPTransactions.DateReceived), -- we do not have processed date in HP table, so we are taking received date as process date.    
 --Processed = convert(smalldatetime,hb.Updated),    
 --HoursId = convert(varchar(24),HPTransactions.Ber + HPTransactions.Subreport + HPTransactions.Sequence),    
 SSN = convert(char(9),HPTransactions.SSN),    
 --LastName = convert(char(50),fpdb.dbo.fn_LastNameOrGen(HPTransactions.Name, 'LN')),    
 --FirstName = convert(char(50),fpdb.dbo.fn_FirstNameOrMid(HPTransactions.Name, 'FN')),    
 UnionCode = convert(int,HPTransactions.UnionCode),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 PensionHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension),    
 IAPHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.Pension), -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later     
 --IAPHoursA2 = convert(numeric(7, 1), HPTransactions.Hours * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,HPTransactions.IAPDollars),    
 --ActiveHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.ActiveHealth),    
 --RetireeHealthHours = convert(numeric(7, 1), HPTransactions.Hours * rgc.RetireeHealth),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),HPTransactions.RateGroup),     
 --HoursStatus = 0,    
 LateMonthly = case when HPTransactions.DateReceived > coalesce(Period.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end,    
 LateAnnual = case when HPTransactions.DateReceived > coalesce(PlanCutoff.cutoffdate, HPTransactions.DateReceived) then 'Y' else '' end ,  
EmployerName = E.EmployerName    
 ----------------------------------------------------------------------------------------------------------------    
 --UnionMisc = HPTransactions.UNMisc,    
 --HoursWorked = convert(numeric(7, 1), HPTransactions.Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'H/P '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, e.OldEmployerNum    
from OPUS_AnnualStmt_Participant_List list     
    inner join eadb.dbo.HPTransactions HPTransactions on list.SSN = HPTransactions.SSN    
 left outer join EADB.dbo.Employer E on HPTransactions.Employer = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP     
 inner join eadb.dbo.Period Period on HPTransactions.EndDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on HPTransactions.RateGroup = right(convert(varchar(4),1000+RGC.RateGroup),2) and HPTransactions.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on HPTransactions.rategroup = right(convert(varchar(4),1000+rgb.rategroup),2) and HPTransactions.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on HPTransactions.EndDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on HPTransactions.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on HPTransactions.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 left outer join eadb.dbo.HPBatch hb on HPTransactions.Batch = hb.Batch     
where --not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime, HPTransactions.DateReceived) <= '02/21/1995')  
      (not (convert(smalldatetime, HPTransactions.EndDate) <= '12/24/1994' and convert(smalldatetime,hb.Updated) <= '02/21/1995')  
     or (convert(smalldatetime, HPTransactions.DateReceived)>'02/21/1995')  
    )  
and not exists(select 1 from CPASPre95_11222011 where mkey = hptransactions.ssn and fdate = hptransactions.startdate and tdate = hptransactions.enddate and erkey = e.employerid and hrsact = convert(numeric(7, 1), HPTransactions.Hours))  
--and HPTransactions.SSN = @SSN    
 --and (HPTransactions.SSN = @SSN or @SSN is null) and HPTransactions.EndDate between @FromDate and @ToDate    
 --and HPTransactions.EndDate between @FromDate and @ToDate    
    
--CPAS View    
insert into [#PensionWorkHistory]    
select     
 --ReportID = left(cpas.erractid,18),    
 ----EmpAccountNo = convert(int, cpas.ERKey),    
 EmpAccountNo = E.EmployerId,    
 ComputationYear = cpas.Plan_Year, -- PY.PensionYear,    
 FromDate = convert(smalldatetime, cpas.FDate),    
 ToDate = convert(smalldatetime, cpas.TDate),    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, cpas.FDate, cpas.TDate)/7.0,0))),    
 --Received = convert(smalldatetime, cpas.PDate),    
 Processed = convert(smalldatetime, cpas.PDate),    
 --HoursId = convert(varchar(24),cpas.erractid),    
 SSN = convert(char(9),cpas.MKey),    
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,cpas.LOC_NO),    
 PensionPlan = convert(smallint, 2), -- MPI     
 --PensionCredit = convert(numeric(7, 3),0),    
 --L52VestedCredit = convert(smallint,0),    
 --PensionHours = convert(numeric(7, 1), cpas.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours    
 PensionHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 --IAPHours = convert(numeric(7, 1), cpas.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later    
 IAPHours = convert(numeric(7, 1), case when cpas.rategroup = 0 then cpas.HRSACT else cpas.HRSACT * rgc.pension end),
 ----MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth    
 ----we are not checking rate item to identify hours for Pension, Health, or IAP    
 ----IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP    
 --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT),  -- $ 0.305 hourly IAP    
 --IAPPercent = convert(money,cpas.PanOnEarn),    
 ----MM 12/26/12    
 ----ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),    
 ----RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),    
 --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT),    
 --NULL PersonId,    
 --RateGroup = convert(varchar(4),cpas.RateGroup),    
 --HoursStatus = 0, --all the hours comming from CPAS are processed.    
 LateMonthly = case when cpas.PDate > coalesce(Period.cutoffdate, cpas.PDate) then 'Y' else '' end,    
 LateAnnual = case when cpas.PDate > coalesce(PlanCutoff.cutoffdate, cpas.PDate) then 'Y' else '' end,  
EmployerName = E.EmployerName   
 -----------------------------------------------------------------------    
 --UnionMisc = null,    
 --HoursWorked = convert(numeric(7, 1), cpas.HRSACT),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'CPAS'    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, E.OldEmployerNum    
 from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.CPASPre95_11222011 cpas on list.SSN = cpas.mkey    
 inner join eadb.dbo.Period Period on cpas.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate    
 left outer join EADB.dbo.Employer E on cpas.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on cpas.RateGroup = RGC.RateGroup and cpas.TDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on cpas.rategroup = rgb.rategroup and cpas.TDate between rgb.FromDate and rgb.ToDate    
 left outer join eadb.dbo.PensionYear PY on cpas.TDate between PY.StartDate and PY.EndDate    
 left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate    
 --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
 where [Plan]=2    
 --and cpas.mkey = @SSN    
 --and (cpas.mkey = @SSN or @SSN is null) and cpas.TDate between @FromDate and @ToDate    
 --and cpas.TDate between @FromDate and @ToDate    
  
-- RAP IAP$  
insert into [#PensionWorkHistory]  
select        
      --ReportID = left(rap.erractid,18),  
      --EmpAccountNo = convert(int, cpas.ERKey),  
      EmpAccountNo = isnull(E.EmployerId,'0'),  
      ComputationYear = rap.Plan_Year, -- PY.PensionYear,  
      FromDate = convert(smalldatetime, rap.FDate),  
      ToDate = convert(smalldatetime, rap.TDate),  
      Weeks = convert(char(2), convert(int , round(DateDiff(day, rap.FDate, rap.TDate)/7.0,0))),  
      --Received = convert(smalldatetime, rap.PDate),  
      Processed = convert(smalldatetime, rap.PDate),  
      --HoursId = convert(varchar(24),rap.erractid),  
      SSN = convert(char(9),rap.MKey),  
      --LastName = NULL, --convert(char(50),p.LastName),  
      --FirstName = NULL, --convert(char(50),p.FirstName),  
      UnionCode = convert(int,rap.LOC_NO),  
      PensionPlan = convert(smallint, 2), -- MPI   
      --PensionCredit = convert(numeric(7, 3),0),  
      --L52VestedCredit = convert(smallint,0),  
      PensionHours = convert(numeric(7, 1), rap.HRSACT),  -- here we dont need to check 'rgc.Pension' flag because whatever is comming from CPAS is PensionHours  
      IAPHours = convert(numeric(7, 1), rap.HRSACT),  -- same as pension hours, RG 8, 66, and 42 issue was for 2003 and later  
      --MM 12/26/12 As per Ajay, Since CPAS data is upto 12/24/1994 and that time all the hours were eligible for $.305 and RetireeHealth  
      --we are not checking rate item to identify hours for Pension, Health, or IAP  
      --IAPHoursA2 = convert(numeric(7, 1), cpas.HRSACT * rgc.IAP),  -- $ 0.305 hourly IAP  
      --IAPHoursA2 = convert(numeric(7, 1), rap.HRSACT),  -- $ 0.305 hourly IAP  
      --IAPPercent = convert(money,rap.PanOnEarn),  
      --MM 12/26/12  
      --ActiveHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.ActiveHealth),  
      --RetireeHealthHours = convert(numeric(7, 1), cpas.HRSACT * rgc.RetireeHealth),  
      --ActiveHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --RetireeHealthHours = convert(numeric(7, 1), rap.HRSACT),  
      --NULL AS PersonId,  
      --RateGroup = convert(varchar(4),rap.RateGroup),  
      --HoursStatus = 0, --all the hours comming from CPAS are processed.  
      LateMonthly = case when rap.PDate > coalesce(Period.cutoffdate, rap.PDate) then 'Y' else '' end,  
      LateAnnual = case when rap.PDate > coalesce(PlanCutoff.cutoffdate, rap.PDate) then 'Y' else '' end,  
      EmployerName = E.EmployerName  
      ---------------------------------------------------------------------  
      --UnionMisc = null,  
      --HoursWorked = convert(numeric(7, 1), rap.HRSACT),  
      --IAPHourlyRate = rgb.Individual  
      --, Source = 'RAP'  
      --, rgc.ToHealthSystem  
      --, rgc.ToPensionSystem  
      --, IsActiveHealth = rgc.ActiveHealth  
      --, IsRetireeHealth = rgc.RetireeHealth  
      --, IsPension = rgc.Pension  
      --, IsIAPHourly = rgc.IAP  
      --, E.OldEmployerNum  
      from OPUS_AnnualStmt_Participant_List list    
      inner join EADB.dbo.RAP_IAP$ rap on rap.mkey=list.SSN  
      left outer join EADB.dbo.Employer E on rap.ERKey = E.OldEmployerNum  -- taking care of Alpha numeric employer id in HP  
      left outer join eadb.dbo.vwRateGroupClassification_all RGC   
         on rap.RateGroup = RGC.RateGroup and rap.TDate between rgc.FromDate and rgc.ToDate   
      left outer join eadb.dbo.vwRateGroupBreakDown_all rgb   
            on rap.rategroup = rgb.rategroup and rap.TDate between rgb.FromDate and rgb.ToDate  
      inner join eadb.dbo.Period Period on rap.TDate between Period.QualifyingStartDate and Period.QualifyingEnddate  
      left outer join eadb.dbo.PensionYear PY on rap.TDate between PY.StartDate and PY.EndDate  
      left outer join eadb.dbo.Period PlanCutoff on PY.EndDate = PlanCutoff.QualifyingEnddate  
      --left outer join pid.dbo.Person p on cpas.mkey = p.ssn    
    --left outer join OPUS.dbo.SGT_Person p on cpas.mkey = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
      where [Plan]=2  
      --and rap.mkey = @SSN  
  
    
--PreMerger View.    
insert into [#PensionWorkHistory]    
select     
 --ReportID = convert(varchar(18),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year)) ,    
 EmpAccountNo = convert(int, Pre.EmployerId),     
 ComputationYear = Pre.Plan_Year,    
 FromDate = convert(smalldatetime, Pre.StartDate), --Plan start date    
 ToDate = convert(smalldatetime, Pre.EndDate),  -- Plan end date    
 Weeks = convert(char(2), convert(int , round(DateDiff(day, Pre.StartDate, Pre.EndDate)/7.0,0))), --52/53    
 --Received = convert(smalldatetime, Pre.MergeDate), --Plan merger date     
 Processed = convert(smalldatetime, Pre.MergeDate), -- plan merger date    
 --HoursId = convert(varchar(24),Pre.Local + '_' + convert(varchar(4),Pre.Plan_Year) + '_' + convert(varchar(6),Pre.RecordId)), --create unique id    
 SSN = convert(char(9),Pre.SSN),     
 --LastName = NULL, --convert(char(50),p.LastName),    
 --FirstName = NULL, --convert(char(50),p.FirstName),    
 UnionCode = convert(int,Pre.UnionCode),     
 PensionPlan = case when [Local]='L600' then convert(smallint, 3)    
      when [Local]='L666' then convert(smallint, 4)    
      when [Local]='L700' then convert(smallint, 6)    
      when [Local]='L52' then convert(smallint, 7)    
      when [Local]='L161' then convert(smallint, 8)    
      else null end,     
 --PensionCredit = convert(numeric(7, 3),Pre.Pension_Credit),    
 --L52VestedCredit = convert(smallint,Pre.L52_Vested_Credit),    
 PensionHours = convert(numeric(7, 1), Pre.Credited_Hours),    
 IAPHours = convert(numeric(7, 1), 0),    
 --IAPHoursA2 = convert(numeric(7, 1), 0),     
 --IAPPercent = convert(money, 0),     
 --ActiveHealthHours = convert(numeric(7, 1), 0),     
 --RetireeHealthHours = convert(numeric(7, 1), 0), -- ?    
 --NULL PersonId,    
 --RateGroup = Pre.RateGroup,--?    
 --HoursStatus = convert(int, 0),    
 LateMonthly = '',     
 LateAnnual = '' ,  
EmployerName = E.EmployerName   
 -------------------------------------------------------------------    
 --UnionMisc = convert(char(2),''),    
 --HoursWorked = convert(numeric(7, 1), Pre.Credited_Hours),    
 --IAPHourlyRate = rgb.Individual    
 --, Source = 'PM  '    
 --, rgc.ToHealthSystem    
 --, rgc.ToPensionSystem    
 --, IsActiveHealth = rgc.ActiveHealth    
 --, IsRetireeHealth = rgc.RetireeHealth    
 --, IsPension = rgc.Pension    
 --, IsIAPHourly = rgc.IAP    
 --, OldEmployerNum = Pre.EmployerId    
from OPUS_AnnualStmt_Participant_List list     
 inner join EADB.dbo.Pension_PreMerger_Annual_History Pre on list.SSN = Pre.SSN    
 left outer join EADB.dbo.vwRateGroupClassification_all RGC     
  on Pre.RateGroup = RGC.RateGroup and Pre.EndDate between rgc.FromDate and rgc.ToDate     
 left outer join EADB.dbo.vwRateGroupBreakDown_all rgb     
  on Pre.rategroup = rgb.rategroup and Pre.EndDate between rgb.FromDate and rgb.ToDate    
 left outer join EADB.dbo.Employer E on E.EmployerId = pre.EmployerId  
--left outer join pid.dbo.Person p on Pre.ssn = p.ssn    
 --left outer join OPUS.dbo.SGT_Person p on Pre.ssn = OPUS.dbo.fn_GET_DECRYPTED_TEXT(p.ssn) --not using any field from sgt_person and decrytion is affecting performance    
--where --Pre.SSN = @SSN    
 --(Pre.SSN = @SSN or @SSN is null) and Pre.EndDate between @FromDate and @ToDate    
 --Pre.EndDate between @FromDate and @ToDate    
    
--select isnull(EmpAccountNo, 0) as EmpAccountNo, ComputationYear, SSN, UnionCode, PensionHours, case when lateannual = 'Y' and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,   
--isnull(EmployerName, '') as EmployerName from [#PensionWorkHistory] where   
--PensionPlan = 2   
                          
 /*
 select ssn,computationyear,
sum(isnull(pensionhours,0.0)) TotalPensionHours,  
sum(isnull(latehours,0.0)) TotalLateHours,
UnionCode,EmpAccountno,EmployerName,
PlanStartDate,firstHourReported
 from         
(        
select ssn,computationyear,pensionhours,
case when (LateAnnual = 'Y' or v.ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate then pensionhours else 0 end Latehours,  
unioncode,empaccountno,e.employername,processed,v.PensionPlan,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN) as PlanStartDate,
(SELECT MIN(FromDate) from [#PensionWorkHistory] WHERE SSN=v.SSN AND cast(ComputationYear as int) = cast(v.ComputationYear as int)) as firstHourReported   
from [#PensionWorkHistory] v        
left outer join EADB.dbo.Employer e on v.empaccountno = e.employerid    
where v.ComputationYear <= @year and PensionPlan = 2     
)a        
group by ssn,computationyear,unioncode,empaccountno,employername,PensionPlan,PlanStartDate,firstHourReported
 */

 
 
--Below script we use for 2014 statements but for remaining statements i have temporarily commented the code 
--SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
--FirstHoursReported = MIN(FromDate)
--into #Summary
--FROM (
--SELECT SSN, ComputationYear, PensionHours, 
--LateHours = case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate THEN PensionHours ELSE 0 END,
--FromDate FROM #PensionWorkHistory
--WHERE PensionPlan = 2 AND ComputationYear <= @year
--) A GROUP BY SSN, ComputationYear



SELECT SSN, ComputationYear, TotalPensionHours = SUM(PensionHours), TotalLateHours = SUM(LateHours),
FirstHoursReported = MIN(FromDate)
into #Summary
FROM (
SELECT SSN, ComputationYear, CASE WHEN PensionPlan = 2 THEN PensionHours ELSE 0 END AS PensionHours, 
LateHours = CASE WHEN PensionPlan = 2 
			THEN case when (LateAnnual = 'Y' or ComputationYear < @year) and processed between @PlanStartDate and @CutoffDate 
												THEN PensionHours ELSE 0 END
			ELSE 0 END,
FromDate FROM #PensionWorkHistory
WHERE ComputationYear <= @year
) A GROUP BY SSN, ComputationYear

SELECT S.SSN, S.ComputationYear, TotalPensionHours, TotalLateHours,
PlanStartDate = Y.FirstHoursReported, S.FirstHoursReported as firstHourReported
FROM #Summary S 
INNER JOIN 
(SELECT SSN, FirstHoursReported = MIN(FirstHoursReported) FROM #Summary GROUP BY SSN) Y ON S.SSN = Y.SSN 
    
drop table [#PensionWorkHistory]  
  
end  
   