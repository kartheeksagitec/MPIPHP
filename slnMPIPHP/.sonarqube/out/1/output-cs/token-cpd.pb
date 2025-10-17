Î7
LC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPInterface\WorkflowResult.cs
	namespace 	
MPIPHP
 
. 
	Interface 
{ 
[ 
Serializable 
] 
public 

class 
WorkflowResult 
{		 
private 
WorkflowStatus 
_status &
;& '
private 

Dictionary 
< 
string !
,! "
object# )
>) *
_outputs+ 3
;3 4
private 
Guid 
_instanceId  
;  !
private 
	Exception 

_exception $
;$ %
public 
WorkflowStatus 
Status $
{ 	
get 
{ 
return 
_status  
;  !
}" #
} 	
public 

Dictionary 
< 
string  
,  !
object" (
>( )
OutputParameters* :
{ 	
get 
{ 
return 
_outputs !
;! "
}# $
} 	
public 
Guid 

InstanceId 
{ 	
get 
{ 
return 
_instanceId $
;$ %
}& '
} 	
public!! 
int!! 
process_instance_id!! &
{!!' (
get!!) ,
;!!, -
set!!. 1
;!!1 2
}!!3 4
public"" 
	Exception"" 
	Exception"" "
{## 	
get$$ 
{$$ 
return$$ 

_exception$$ #
;$$# $
}$$% &
}%% 	
private)) 
WorkflowResult)) 
()) 
)))  
{** 	
}++ 	
private-- 
WorkflowResult-- 
(-- 1
%WorkflowApplicationCompletedEventArgs-- D
args--E I
)--I J
{.. 	
ArgumentIsNotNull// 
(// 
args// "
,//" #
$str//$ *
)//* +
;//+ ,
_outputs00 
=00 
args00 
.00 
Outputs00 #
as00$ &

Dictionary00' 1
<001 2
string002 8
,008 9
object00: @
>00@ A
;00A B
_instanceId11 
=11 
args11 
.11 

InstanceId11 )
;11) *
}22 	
private44 
WorkflowResult44 
(44 :
.WorkflowApplicationUnhandledExceptionEventArgs44 M
args44N R
)44R S
{55 	
ArgumentIsNotNull66 
(66 
args66 "
,66" #
$str66$ *
)66* +
;66+ ,
_instanceId77 
=77 
args77 
.77 

InstanceId77 )
;77) *

_exception88 
=88 
args88 
.88 
UnhandledException88 0
;880 1
}99 	
private;; 
WorkflowResult;; 
(;; /
#WorkflowApplicationAbortedEventArgs;; B
args;;C G
);;G H
{<< 	
ArgumentIsNotNull== 
(== 
args== "
,==" #
$str==$ *
)==* +
;==+ ,
_instanceId>> 
=>> 
args>> 
.>> 

InstanceId>> )
;>>) *

_exception?? 
=?? 
args?? 
.?? 
Reason?? $
;??$ %
}@@ 	
privateBB 
WorkflowResultBB 
(BB 
GuidBB #

instanceIDBB$ .
)BB. /
{CC 	
_instanceIdDD 
=DD 

instanceIDDD $
;DD$ %
}EE 	
publicII 
staticII 
WorkflowResultII $*
CreateCompletedWorkflowResultsII% C
(IIC D1
%WorkflowApplicationCompletedEventArgsIID i
argsIIj n
)IIn o
{JJ 	
WorkflowResultKK 
resultsKK "
=KK# $
newKK% (
WorkflowResultKK) 7
(KK7 8
argsKK8 <
)KK< =
;KK= >
resultsLL 
.LL 
_statusLL 
=LL 
WorkflowStatusLL ,
.LL, -
	CompletedLL- 6
;LL6 7
returnMM 
resultsMM 
;MM 
}NN 	
publicPP 
staticPP 
WorkflowResultPP $+
CreateTerminatedWorkflowResultsPP% D
(PPD E:
.WorkflowApplicationUnhandledExceptionEventArgsPPE s
argsPPt x
)PPx y
{QQ 	
WorkflowResultRR 
resultsRR "
=RR# $
newRR% (
WorkflowResultRR) 7
(RR7 8
argsRR8 <
)RR< =
;RR= >
resultsSS 
.SS 
_statusSS 
=SS 
WorkflowStatusSS ,
.SS, -

TerminatedSS- 7
;SS7 8
returnTT 
resultsTT 
;TT 
}UU 	
publicWW 
staticWW 
WorkflowResultWW $(
CreateAbortedWorkflowResultsWW% A
(WWA B/
#WorkflowApplicationAbortedEventArgsWWB e
argsWWf j
)WWj k
{XX 	
WorkflowResultYY 
resultsYY "
=YY# $
newYY% (
WorkflowResultYY) 7
(YY7 8
argsYY8 <
)YY< =
;YY= >
resultsZZ 
.ZZ 
_statusZZ 
=ZZ 
WorkflowStatusZZ ,
.ZZ, -
AbortedZZ- 4
;ZZ4 5
return[[ 
results[[ 
;[[ 
}\\ 	
publicee 
staticee 
WorkflowResultee $(
CreateCreatedWorkflowResultsee% A
(eeA B
GuideeB F

instanceIDeeG Q
)eeQ R
{ff 	
WorkflowResultgg 
resultsgg "
=gg# $
newgg% (
WorkflowResultgg) 7
(gg7 8

instanceIDgg8 B
)ggB C
;ggC D
resultshh 
.hh 
_statushh 
=hh 
WorkflowStatushh ,
.hh, -
Createdhh- 4
;hh4 5
returnii 
resultsii 
;ii 
}jj 	
privatess 
voidss 
ArgumentIsNotNullss &
(ss& '
objectss' -
argumentss. 6
,ss6 7
stringss8 >
namess? C
)ssC D
{tt 	
ifuu 
(uu 
argumentuu 
==uu 
nulluu  
)uu  !
{vv 
throwww 
newww !
ArgumentNullExceptionww /
(ww/ 0
nameww0 4
)ww4 5
;ww5 6
}xx 
}yy 	
}{{ 
public}} 

enum}} 
WorkflowStatus}} 
{~~ 
Created 
, 
	Completed
ÄÄ 
,
ÄÄ 

Terminated
ÅÅ 
,
ÅÅ 
Aborted
ÇÇ 
,
ÇÇ 
Running
ÉÉ 
,
ÉÉ 
Unloaded
ÑÑ 
}
ÖÖ 
}ÜÜ ¡
MC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPInterface\IWorkflowEngine.cs
	namespace 	
MPIPHP
 
. 
	Interface 
{ 
public 

	interface 
IWorkflowEngine $
{ 
WorkflowResult		 
Run		 
(		 
int		 
aintSystemRequestID		 2
,		2 3
int		4 7
aintProcessID		8 E
,		E F
int		G J
aintPersonID		K W
,		W X
int		Y \
	aintOrgID		] f
,		f g
long		h l
aintReferenceID		m |
,		| }
string			~ Ñ
astrCreatedBy
		Ö í
,
		í ì
int
		ì ñ!
aintContactTicketID
		ó ™
)
		™ ´
;
		´ ¨
bool

 
ResumeBookmark

 
(

 
Guid

  
awinInstanceID

! /
,

/ 0%
ActivityInstanceEventArgs

1 J
aaieBookMarkValue

K \
)

\ ]
;

] ^
void 
Abort 
( 
Guid 
id 
, 
string "
reason# )
)) *
;* +
void 
Abort 
( 
Guid 
id 
) 
; 
void 
Cancel 
( 
Guid 
id 
) 
; 
void 
	Terminate 
( 
Guid 
id 
, 
string  &
reason' -
)- .
;. /
} 
} É
NC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPInterface\IMPIPHPInterface.cs
	namespace

 	
MPIPHP


 
.

 
	Interface

 
{ 
public 
	interface 
IMPIPHPDBCache  
:  !
IDBCache" *
{ 
string '
GetMPIPHPMessageDescription *
(* +
int+ .
aintMessageID/ <
)< =
;= >
} 
public 
	interface  
IMPIPHPMetaDataCache &
:& '
IMetaDataCache( 6
{ 
} 
public 
	interface 
IMPIPHPBusinessTier %
:% &
IBusinessTier' 4
{ 
} 
} ÷

JC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPInterface\AssemblyInfo.cs
[		 
assembly		 	
:			 

AssemblyTitle		 
(		 
$str		 
)		 
]		 
[

 
assembly

 	
:

	 

AssemblyDescription

 
(

 
$str

 !
)

! "
]

" #
[ 
assembly 	
:	 
!
AssemblyConfiguration  
(  !
$str! #
)# $
]$ %
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyProduct 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyCulture 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyVersion 
( 
$str "
)" #
]# $