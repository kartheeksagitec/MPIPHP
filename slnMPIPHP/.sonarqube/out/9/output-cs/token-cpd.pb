»
LC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\WorkflowConstants.cs
	namespace 	
MPIPHP
 
. 
Common 
{ 
public 

class 
WorkflowConstants "
{		 
public 
const 
string 5
)WorkflowProcessInstanceStatusNotProcessed E
=F G
$strH N
;N O
public 
const 
string 2
&WorkflowProcessInstanceStatusProcessed B
=C D
$strE K
;K L
public 
const 
string 3
'WorkflowProcessInstanceStatusInProgress C
=D E
$strF L
;L M
public 
const 
string 0
$WorkflowProcessInstanceStatusAborted @
=A B
$strC I
;I J
public 
const 
string +
ActivityInstanceStatusInitiated ;
=< =
$str> D
;D E
public 
const 
string +
ActivityInstanceStatusProcessed ;
=< =
$str> D
;D E
public 
const 
string +
ActivityInstanceStatusSuspended ;
=< =
$str> D
;D E
public 
const 
string +
ActivityInstanceStatusCancelled ;
=< =
$str> D
;D E
public 
const 
string 
WorkflowServiceURL .
=/ 0
$str1 O
;O P
public 
const 
string %
IncomingSecureMessageXAML 5
=6 7
$str8 R
;R S
public 
const 
string 6
*IncomingSecureMessageXAML_Incoming_Message F
=G H
$strI [
;[ \
public 
const 
string &
ProcessMemberCrosswalkXAML 6
=7 8
$str9 R
;R S
public 
const 
string 
FlagYes #
=$ %
$str& )
;) *
public   
const   
string   
FlagNo   "
=  # $
$str  % (
;  ( )
public## 
const## 
string## 
SourceDescription## -
=##. /
$str##0 D
;##D E
public$$ 
const$$ 
string$$ 
ProcessDescription$$ .
=$$/ 0
$str$$1 F
;$$F G
public%% 
const%% 
string%% 
ProcessName%% '
=%%( )
$str%%* 8
;%%8 9
public)) 
const)) 
string)) 
DATATYPE_STRING)) +
=)), -
$str)). 6
;))6 7
public,, 
const,, 
string,, /
#ActivityInstanceStatus_INPC_Or_RESU,, ?
=,,@ A
$str,,B Q
;,,Q R
public-- 
const-- 
string-- /
#ActivityInstanceStatus_UNPC_Or_RELE-- ?
=--@ A
$str--B Q
;--Q R
public00 
const00 
string00 
BuildWhereClause_In00 /
=000 1
$str002 6
;006 7
public11 
const11 
string11  
BuildWhereClause_And11 0
=111 2
$str113 :
;11: ;
public33 
const33 
string33 
Operator_EqualTo33 ,
=33- .
$str33/ 2
;332 3
public77 
const77 
int77 
Message_Id_407677 (
=77) *
$num77+ /
;77/ 0
};; 
}<< ˝
RC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Properties\AssemblyInfo.cs
[ 
assembly 	
:	 

AssemblyTitle 
( 
$str '
)' (
]( )
[		 
assembly		 	
:			 

AssemblyDescription		 
(		 
$str		 !
)		! "
]		" #
[

 
assembly

 	
:

	 
!
AssemblyConfiguration

  
(

  !
$str

! #
)

# $
]

$ %
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyProduct 
( 
$str )
)) *
]* +
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str 0
)0 1
]1 2
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyCulture 
( 
$str 
) 
] 
[ 
assembly 	
:	 


ComVisible 
( 
false 
) 
] 
[ 
assembly 	
:	 

Guid 
( 
$str 6
)6 7
]7 8
[## 
assembly## 	
:##	 

AssemblyVersion## 
(## 
$str## $
)##$ %
]##% &
[$$ 
assembly$$ 	
:$$	 

AssemblyFileVersion$$ 
($$ 
$str$$ (
)$$( )
]$$) *‘E
PC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\FileDownloadContainer.cs
	namespace 	
MPIPHP
 
. 
Common 
{		 
[

 
Serializable

 
(

 
)

 
]

 
public 

class !
FileDownloadContainer &
:' (
ISerializable) 6
{ 
public 
const 
string 

sessionKey &
=' (
$str) @
;@ A
public 
const 
string #
defaultDownloadMimeType 3
=4 5
$str6 P
;P Q
	protected 
const 
string 
_snFileContent -
=. /
$str0 =
;= >
	protected 
const 
string 
_snFileMimeType .
=/ 0
$str1 ?
;? @
	protected 
const 
string 
_snFileName *
=+ ,
$str- 7
;7 8
	protected 
const 
string 
_snOpenInBrowser /
=0 1
$str2 A
;A B
	protected 
const 
string 
_snErrorList +
=, -
$str. 9
;9 :
	protected 
byte 
[ 
] 
_iFileContent &
;& '
	protected 
string 

_iFileName #
;# $
	protected 
string 
_iFileMimeType '
;' (
	protected 
bool 
_iblnOpenInBrowser )
;) *
	protected 
utlErrorList 
_iErrorList *
;* +
public   
byte   
[   
]   
iFileContent   "
{  # $
get  % (
{  ) *
return  + 1
_iFileContent  2 ?
;  ? @
}  A B
}  C D
public!! 
string!! 
	iFileName!! 
{!!  !
get!!" %
{!!& '
return!!( .

_iFileName!!/ 9
;!!9 :
}!!; <
}!!= >
public"" 
string"" 
iFileMimeType"" #
{""$ %
get""& )
{""* +
return"", 2
_iFileMimeType""3 A
;""A B
}""C D
}""E F
public## 
bool## 
iblnOpenInBrowser## %
{##& '
get##( +
{##, -
return##. 4
_iblnOpenInBrowser##5 G
;##G H
}##I J
set##K N
{##O P
_iblnOpenInBrowser##Q c
=##d e
value##f k
;##k l
}##m n
}##o p
public$$ 
bool$$ 

iHasErrors$$ 
{$$  
get$$! $
{$$% &
return$$' -
($$. /
_iErrorList$$/ :
.$$: ;
Count$$; @
>$$A B
$num$$C D
)$$D E
;$$E F
}$$G H
}$$I J
public%% 
utlErrorList%% 

iErrorList%% &
{%%' (
get%%) ,
{%%- .
return%%/ 5
_iErrorList%%6 A
;%%A B
}%%C D
}%%E F
public'' !
FileDownloadContainer'' $
(''$ %
string''% +
astrFileName'', 8
,''8 9
string'': @
astrFileMimeType''A Q
,''Q R
byte''S W
[''W X
]''X Y
aobjFileContent''Z i
)''i j
:(( 
this(( 
((( 
astrFileName(( 
,((  
astrFileMimeType((! 1
,((1 2
aobjFileContent((3 B
,((B C
false((D I
)((I J
{)) 	
}++ 	
public,, !
FileDownloadContainer,, $
(,,$ %
string,,% +
astrFileName,,, 8
,,,8 9
string,,: @
astrFileMimeType,,A Q
,,,Q R
byte,,S W
[,,W X
],,X Y
aobjFileContent,,Z i
,,,i j
bool,,k o
ablnOpenInBrowser	,,p Å
)
,,Å Ç
{-- 	
_iErrorList.. 
=.. 
new.. 
utlErrorList.. *
(..* +
)..+ ,
;.., -
utlError// 
e// 
;// 
if11 
(11 
string11 
.11 
IsNullOrWhiteSpace11 )
(11) *
astrFileName11* 6
)116 7
)117 8
{22 
e33 
=33 
new33 
utlError33  
(33  !
)33! "
;33" #
e44 
.44 
istrErrorMessage44 "
=44# $
$str44% A
;44A B
_iErrorList55 
.55 
Add55 
(55  
e55  !
)55! "
;55" #
}66 
if88 
(88 
aobjFileContent88 
==88  "
null88# '
)88' (
{99 
e:: 
=:: 
new:: 
utlError::  
(::  !
)::! "
;::" #
e;; 
.;; 
istrErrorMessage;; "
=;;# $
$str;;% ;
;;;; <
_iErrorList<< 
.<< 
Add<< 
(<<  
e<<  !
)<<! "
;<<" #
}== 
if?? 
(?? 
string?? 
.?? 
IsNullOrWhiteSpace?? )
(??) *
astrFileMimeType??* :
)??: ;
)??; <
{@@ 
_iFileMimeTypeAA 
=AA  #
defaultDownloadMimeTypeAA! 8
;AA8 9
}BB 
_iblnOpenInBrowserDD 
=DD  
ablnOpenInBrowserDD! 2
;DD2 3

_iFileNameEE 
=EE 
astrFileNameEE %
;EE% &
_iFileMimeTypeFF 
=FF 
astrFileMimeTypeFF -
;FF- .
_iFileContentGG 
=GG 
aobjFileContentGG +
;GG+ ,
}HH 	
publicJJ !
FileDownloadContainerJJ $
(JJ$ %
SerializationInfoJJ% 6
infoJJ7 ;
,JJ; <
StreamingContextJJ= M
contextJJN U
)JJU V
{KK 	
thisLL 
.LL 
_iFileContentLL 
=LL  
(LL! "
byteLL" &
[LL& '
]LL' (
)LL( )
infoLL) -
.LL- .
GetValueLL. 6
(LL6 7
_snFileContentLL7 E
,LLE F
typeofLLG M
(LLM N
byteLLN R
[LLR S
]LLS T
)LLT U
)LLU V
;LLV W
thisMM 
.MM 
_iFileMimeTypeMM 
=MM  !
infoMM" &
.MM& '
	GetStringMM' 0
(MM0 1
_snFileMimeTypeMM1 @
)MM@ A
;MMA B
thisNN 
.NN 

_iFileNameNN 
=NN 
infoNN "
.NN" #
	GetStringNN# ,
(NN, -
_snFileNameNN- 8
)NN8 9
;NN9 :
thisOO 
.OO 
_iblnOpenInBrowserOO #
=OO$ %
infoOO& *
.OO* +

GetBooleanOO+ 5
(OO5 6
_snOpenInBrowserOO6 F
)OOF G
;OOG H
thisPP 
.PP 
_iErrorListPP 
=PP 
(PP  
utlErrorListPP  ,
)PP, -
infoPP- 1
.PP1 2
GetValuePP2 :
(PP: ;
_snErrorListPP; G
,PPG H
typeofPPI O
(PPO P
utlErrorListPPP \
)PP\ ]
)PP] ^
;PP^ _
}QQ 	
publicRR 
voidRR 
GetObjectDataRR !
(RR! "
SerializationInfoRR" 3
infoRR4 8
,RR8 9
StreamingContextRR: J
contextRRK R
)RRR S
{SS 	
infoTT 
.TT 
AddValueTT 
(TT 
_snFileContentTT (
,TT( )
thisTT* .
.TT. /
_iFileContentTT/ <
)TT< =
;TT= >
infoUU 
.UU 
AddValueUU 
(UU 
_snFileNameUU %
,UU% &
thisUU' +
.UU+ ,

_iFileNameUU, 6
)UU6 7
;UU7 8
infoVV 
.VV 
AddValueVV 
(VV 
_snFileMimeTypeVV )
,VV) *
thisVV+ /
.VV/ 0
_iFileMimeTypeVV0 >
)VV> ?
;VV? @
infoWW 
.WW 
AddValueWW 
(WW 
_snOpenInBrowserWW *
,WW* +
thisWW, 0
.WW0 1
_iblnOpenInBrowserWW1 C
)WWC D
;WWD E
infoXX 
.XX 
AddValueXX 
(XX 
_snErrorListXX &
,XX& '
thisXX( ,
.XX, -
_iErrorListXX- 8
)XX8 9
;XX9 :
}YY 	
}ZZ 
}[[ Á
UC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\MPIPHPException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
{ 
[		 
Serializable		 
(		 
)		 
]		 
public

 

class

 
MPIPHPException

  
:

! "$
BaseApplicationException

# ;
{ 
public 
MPIPHPException 
( 
)  
: 
base 
( 
) 
{ 	
} 	
public 
MPIPHPException 
( 
string %
message& -
)- .
: 
base 
( 
message 
) 
{ 	
} 	
} 
} ™
nC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\SendCommunicationException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
[ 
Serializable 
( 
) 
] 
public		 

class		 &
SendCommunicationException		 +
:		, -"
CommunicationException		. D
{

 
public &
SendCommunicationException )
() *
int* -
aintCorTrackingID. ?
)? @
: 
base 
( 
$num 
, 
aintCorTrackingID '
,' (
$num) *
,* +
string, 2
.2 3
Empty3 8
)8 9
{ 	
} 	
} 
} Ø
qC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\InvalidCorTrackingIDException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
[ 
Serializable 
( 
) 
] 
public		 

class		 )
InvalidCorTrackingIDException		 .
:		/ 0"
CommunicationException		1 G
{

 
public )
InvalidCorTrackingIDException ,
(, -
int- 0
aintCorTracking1 @
)@ A
: 
base 
( 
$num 
, 
aintCorTracking %
,% &
$num' (
,( )
string* 0
.0 1
Empty1 6
)6 7
{ 	
} 	
} 
} ﬂ
wC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\InvalidCommunicationStatusException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
[ 
Serializable 
( 
) 
] 
public		 

class		 /
#InvalidCommunicationStatusException		 4
:		5 6"
CommunicationException		7 M
{

 
private 
string $
_istrCommunicationStatus /
;/ 0
	protected 
string #
istrCommunicationStatus 0
{ 	
get 
{ 
return $
_istrCommunicationStatus 1
;1 2
}3 4
} 	
public /
#InvalidCommunicationStatusException 2
(2 3
int3 6
aintTemplateID7 E
,E F
intG J
aintTrackingIDK Y
,Y Z
int[ ^#
aintCommSecureMessageID_ v
,v w
stringx ~$
astrCommunicationStatus	 ñ
,
ñ ó
string
ò û
message
ü ¶
)
¶ ß
: 
base 
( 
aintTemplateID !
,! "
aintTrackingID# 1
,1 2
$num3 4
,4 5
message6 =
)= >
{ 	$
_istrCommunicationStatus $
=% &#
astrCommunicationStatus' >
;> ?
} 	
} 
} ç
rC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\GenerateCommunicationException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
[ 
Serializable 
( 
) 
] 
public		 

class		 *
GenerateCommunicationException		 /
:		0 1"
CommunicationException		2 H
{

 
public *
GenerateCommunicationException -
(- .
int. 1
aintTemplateID2 @
,@ A
intB E
aintTrackingIDF T
,T U
stringV \
message] d
)d e
: 
base 
( 
aintTemplateID !
,! "
aintTrackingID# 1
,1 2
$num3 4
,4 5
message6 =
)= >
{ 	
} 	
} 
} í(
jC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\CommunicationException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
{ 
[ 
Serializable 
( 
) 
] 
public		 

class		 "
CommunicationException		 '
:		( )
MPIPHPException		* 9
{

 
private 
int 
_iintTrackingID #
;# $
private 
int 
_iintTemplateID #
;# $
private 
int $
_iintCommSecureMessageID ,
;, -
private 
int 
_iintPersonID !
;! "
private 
int 
_iintOrgContactID %
;% &
private 
int 
_iintInternalUserID '
;' (
	protected *
enmCommunicationRecipientGroup 0,
 _ienmCommunicationRecipientGroup1 Q
;Q R
	protected 
const 
string 
PERSON %
=& '
$str( 0
;0 1
	protected 
const 
string 
ORG_CONTACT *
=+ ,
$str- :
;: ;
	protected 
const 
string 
INTERNAL_USER_ID /
=0 1
$str2 A
;A B
	protected 
const 
string /
#ERROR_MESSAGE_UNDELIVERABLE_ADDRESS B
=C D
$str	E é
;
é è
	protected 
const 
string +
ERROR_MESSAGE_ADDRESS_NOT_FOUND >
=? @
$strA y
;y z
	protected 
const 
string 9
-ERROR_MESSAGE_COMMUNICATION_RECIPIENT_NOT_SET L
=M N
$strO w
;w x
public 
int #
iintCommSecureMessageID *
{ 	
get 
{ 
return $
_iintCommSecureMessageID 1
;1 2
}3 4
set 
{ $
_iintCommSecureMessageID *
=+ ,
value- 2
;2 3
}4 5
} 	
public   
int   
iintTemplateID   !
{!! 	
get"" 
{"" 
return"" 
_iintTemplateID"" (
;""( )
}""* +
set## 
{## 
_iintTemplateID## !
=##" #
value##$ )
;##) *
}##+ ,
}$$ 	
public%% 
int%% 
iintTrackingID%% !
{&& 	
get'' 
{'' 
return'' 
_iintTrackingID'' (
;''( )
}''* +
set(( 
{(( 
_iintTrackingID(( !
=((" #
value(($ )
;(() *
}((+ ,
})) 	
public++ 
int++ 
iintPersonID++ 
{,, 	
get-- 
{-- 
return-- 
_iintPersonID-- &
;--& '
}--( )
set.. 
{.. 
_iintPersonID.. 
=..  !
value.." '
;..' (
}..) *
}// 	
public11 
int11 
iintOrgContactID11 #
{22 	
get33 
{33 
return33 
_iintOrgContactID33 *
;33* +
}33, -
set44 
{44 
_iintOrgContactID44 #
=44$ %
value44& +
;44+ ,
}44- .
}55 	
public77 
int77 
iintInternalUserID77 %
{88 	
get99 
{99 
return99 
_iintInternalUserID99 ,
;99, -
}99. /
set:: 
{:: 
_iintInternalUserID:: %
=::& '
value::( -
;::- .
}::/ 0
};; 	
public== "
CommunicationException== %
(==% &
string==& ,
message==- 4
)==4 5
:>> 
base>> 
(>> 
message>> 
)>> 
{?? 	,
 _ienmCommunicationRecipientGroup@@ ,
=@@- .*
enmCommunicationRecipientGroup@@/ M
.@@M N
NotSet@@N T
;@@T U
}AA 	
publicCC "
CommunicationExceptionCC %
(CC% &
intCC& )
aintTemplateIDCC* 8
,CC8 9
intCC: =
aintTrackingIDCC> L
,CCL M
intCCN Q#
aintCommSecureMessageIDCCR i
,CCi j
stringCCk q
messageCCr y
)CCy z
:DD 
thisDD 
(DD 
messageDD 
)DD 
{EE 	$
_iintCommSecureMessageIDFF $
=FF% &#
aintCommSecureMessageIDFF' >
;FF> ?
_iintTemplateIDGG 
=GG 
aintTemplateIDGG ,
;GG, -
_iintTrackingIDHH 
=HH 
aintTrackingIDHH ,
;HH, -
}II 	
}KK 
}LL ˜
qC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\clsNonDeliverableAddressFound.cs
	namespace 	
MPIPHP
 
. 
BusinessObjects  
.  !
Communication! .
.. /

Exceptions/ 9
{ 
public		 

abstract		 
class		 2
&clsNonDeliverableAddressFoundException		 @
:		A B"
CommunicationException		C Y
{

 
public 2
&clsNonDeliverableAddressFoundException 5
(5 6
string6 <
astrException= J
)J K
: 
base 
( 
astrException  
)  !
{ 	
} 	
} 
public 

class 8
,clsPersonNonDeliverableAddressFoundException =
:> ?2
&clsNonDeliverableAddressFoundException@ f
{ 
public 
int 
iintPersonID 
{  !
get" %
;% &
set' *
;* +
}, -
public 
int 
iintPersonAddressID &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 8
,clsPersonNonDeliverableAddressFoundException ;
(; <
int< ?
aintPersonID@ L
,L M
intN Q
aintAddressIDR _
)_ `
: 
base 
( 
string 
. 
Format  
(  !/
#ERROR_MESSAGE_UNDELIVERABLE_ADDRESS! D
,D E
aintAddressIDF S
.S T
ToStringT \
(\ ]
)] ^
,^ _
PERSON` f
,f g
aintPersonIDh t
.t u
ToStringu }
(} ~
)~ 
)	 Ä
)
Ä Å
{ 	
iintPersonID 
= 
aintPersonID '
;' (
iintPersonAddressID 
=  !
aintAddressID" /
;/ 0
} 	
} 
public 

class <
0clsOrgContactNonDeliverableAddressFoundException A
:B C2
&clsNonDeliverableAddressFoundExceptionD j
{   
public!! 
int!! 
iintOrgContactID!! #
{!!$ %
get!!& )
;!!) *
set!!+ .
;!!. /
}!!0 1
public"" 
int"" #
iintOrgContactAddressID"" *
{""+ ,
get""- 0
;""0 1
set""2 5
;""5 6
}""7 8
public$$ <
0clsOrgContactNonDeliverableAddressFoundException$$ ?
($$? @
int$$@ C
aintOrgContactID$$D T
,$$T U
int$$V Y
aintAddressID$$Z g
)$$g h
:%% 
base%% 
(%% 
string%% 
.%% 
Format%%  
(%%  !/
#ERROR_MESSAGE_UNDELIVERABLE_ADDRESS%%! D
,%%D E
aintAddressID%%F S
.%%S T
ToString%%T \
(%%\ ]
)%%] ^
,%%^ _
ORG_CONTACT%%` k
,%%k l
aintOrgContactID%%m }
.%%} ~
ToString	%%~ Ü
(
%%Ü á
)
%%á à
)
%%à â
)
%%â ä
{&& 	
iintOrgContactID'' 
='' 
aintOrgContactID'' /
;''/ 0#
iintOrgContactAddressID(( #
=(($ %
aintAddressID((& 3
;((3 4
})) 	
}** 
}++ „
|C:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\clsCommunicationRecipientNotSetException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
public 

class 4
(clsCommunicationRecipientNotSetException 9
:: ;"
CommunicationException< R
{		 
public

 4
(clsCommunicationRecipientNotSetException

 7
(

7 8
)

8 9
: 
base 
( 9
-ERROR_MESSAGE_COMMUNICATION_RECIPIENT_NOT_SET @
)@ A
{ 	
} 	
} 
} —
oC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\Exceptions\Communication\clsAddressNotFoundException.cs
	namespace 	
MPIPHP
 
. 
Common 
. 

Exceptions "
." #
Communication# 0
{ 
public 

class '
clsAddressNotFoundException ,
:- ."
CommunicationException/ E
{		 
public

 '
clsAddressNotFoundException

 *
(

* +
int

+ .
aintTemplateID

/ =
,

= >
int

? B
aintRecipientID

C R
,

R S*
enmCommunicationRecipientGroup

T r,
aenmCommunicationRecipientGroup	

s í
)


í ì
: 
base 
( 
aintTemplateID !
,! "
$num# $
,$ %
$num& '
,' (
string) /
./ 0
Format0 6
( "
CommunicationException #
.# $+
ERROR_MESSAGE_ADDRESS_NOT_FOUND$ C
,C D+
aenmCommunicationRecipientGroupE d
.d e
ToStringe m
(m n
)n o
,o p
aintRecipientID	q Ä
)
Ä Å
) 
{ 	,
 _ienmCommunicationRecipientGroup ,
=- .+
aenmCommunicationRecipientGroup/ N
;N O
switch 
( +
aenmCommunicationRecipientGroup 3
)3 4
{ 
case *
enmCommunicationRecipientGroup 3
.3 4
InternalUser4 @
:@ A
this 
. 
iintInternalUserID +
=, -
aintRecipientID. =
;= >
break 
; 
case *
enmCommunicationRecipientGroup 3
.3 4
OrganizationContact4 G
:G H
this 
. 
iintOrgContactID )
=* +
aintRecipientID, ;
;; <
break 
; 
case *
enmCommunicationRecipientGroup 3
.3 4
Person4 :
:: ;
this 
. 
iintPersonID %
=& '
aintRecipientID( 7
;7 8
break 
; 
} 
} 	
} 
} ‘
TC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\doActivityInstanceHistory.cs
	namespace 	
MPIPHP
 
. 
DataObjects 
{ 
[ 
Serializable 
] 
public 

class %
doActivityInstanceHistory *
:+ ,
doBase- 3
{ 
[	 

NonSerialized
 
] 
public	 
static 
	Hashtable  

ihstFields! +
=, -
null. 2
;2 3
public	 %
doActivityInstanceHistory )
() *
)* +
:, -
base. 2
(2 3
)3 4
{	 

}	 

public	 
int (
activity_instance_history_id 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public	 
int  
activity_instance_id (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public	 
int 
	status_id 
{ 
get  #
;# $
set% (
;( )
}* +
public	 
string 
status_description )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public	 
string 
status_value #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public  	 
string   
action_user_id   %
{  & '
get  ( +
;  + ,
set  - 0
;  0 1
}  2 3
public!!	 
DateTime!! 

start_time!! #
{!!$ %
get!!& )
;!!) *
set!!+ .
;!!. /
}!!0 1
public""	 
DateTime"" 
end_time"" !
{""" #
get""$ '
;""' (
set"") ,
;"", -
}"". /
public##	 
string## 
comments## 
{##  !
get##" %
;##% &
set##' *
;##* +
}##, -
}$$ 
[%% 
Serializable%% 
]%% 
public&& 

enum&& &
enmActivityInstanceHistory&& *
{'' (
activity_instance_history_id((	 %
,((& ' 
activity_instance_id))	 
,)) 
	status_id**	 
,** 
status_description++	 
,++ 
status_value,,	 
,,, 
action_user_id--	 
,-- 

start_time..	 
,.. 
end_time//	 
,// 
comments00	 
,00 
}11 
}22 ò!
MC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\doActivityInstance.cs
	namespace 	
MPIPHP
 
. 
DataObjects 
{ 
[ 
Serializable 
] 
public 

class 
doActivityInstance #
:$ %
doBase& ,
{ 
[	 

NonSerialized
 
] 
public	 
static 
	Hashtable  

ihstFields! +
=, -
null. 2
;2 3
public	 
doActivityInstance "
(" #
)# $
:% &
base' +
(+ ,
), -
{	 

}	 

public	 
int  
activity_instance_id (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public	 
int 
process_instance_id '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public	 
int 
activity_id 
{  !
get" %
;% &
set' *
;* +
}, -
public	 
string 
checked_out_user '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public	 
long 
reference_id !
{" #
get$ '
;' (
set) ,
;, -
}. /
public  	 
int   
	status_id   
{   
get    #
;  # $
set  % (
;  ( )
}  * +
public!!	 
string!! 
status_description!! )
{!!* +
get!!, /
;!!/ 0
set!!1 4
;!!4 5
}!!6 7
public""	 
string"" 
status_value"" #
{""$ %
get""& )
;"") *
set""+ .
;"". /
}""0 1
public##	 
DateTime## !
suspension_start_date## .
{##/ 0
get##1 4
;##4 5
set##6 9
;##9 :
}##; <
public$$	 
int$$ 
suspension_minutes$$ &
{$$' (
get$$) ,
;$$, -
set$$. 1
;$$1 2
}$$3 4
public%%	 
DateTime%% 
suspension_end_date%% ,
{%%- .
get%%/ 2
;%%2 3
set%%4 7
;%%7 8
}%%9 :
public&&	 
string&& "
return_from_audit_flag&& -
{&&. /
get&&0 3
;&&3 4
set&&5 8
;&&8 9
}&&: ;
public''	 
int'' 
resume_action_id'' $
{''% &
get''' *
;''* +
set'', /
;''/ 0
}''1 2
public((	 
string(( %
resume_action_description(( 0
{((1 2
get((3 6
;((6 7
set((8 ;
;((; <
}((= >
public))	 
string)) 
resume_action_value)) *
{))+ ,
get))- 0
;))0 1
set))2 5
;))5 6
}))7 8
public**	 
string** 
comments** 
{**  !
get**" %
;**% &
set**' *
;*** +
}**, -
}++ 
[,, 
Serializable,, 
],, 
public-- 

enum-- 
enmActivityInstance-- #
{..  
activity_instance_id//	 
,// 
process_instance_id00	 
,00 
activity_id11	 
,11 
checked_out_user22	 
,22 
reference_id33	 
,33 
	status_id44	 
,44 
status_description55	 
,55 
status_value66	 
,66 !
suspension_start_date77	 
,77  
suspension_minutes88	 
,88 
suspension_end_date99	 
,99 "
return_from_audit_flag::	 
,::  !
resume_action_id;;	 
,;; %
resume_action_description<<	 "
,<<# $
resume_action_value==	 
,== 
comments>>	 
,>> 
}?? 
}@@ ◊
UC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\cdoActivityInstanceHistory.cs
	namespace 	
MPIPHP
 
. 
CustomDataObjects "
{		 
[ 
Serializable 
] 
public 
class &
cdoActivityInstanceHistory (
:) *%
doActivityInstanceHistory+ D
{ 
public &
cdoActivityInstanceHistory	 #
(# $
)$ %
:& '
base( ,
(, -
)- .
{ 
} 
public 
string "
end_time_null_as_empty ,
{ 	
get 
{ 
if 
( 
end_time 
== 
DateTime  (
.( )
MinValue) 1
)1 2
return 
string !
.! "
Empty" '
;' (
return 
end_time 
.  
ToString  (
(( )
)) *
;* +
} 
} 	
} 
} ¢:
NC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\cdoActivityInstance.cs
	namespace 	
MPIPHP
 
. 
CustomDataObjects "
{ 
[ 
Serializable 
] 
public 

class 
cdoActivityInstance $
:% &
doActivityInstance' 9
{ 
public 
cdoActivityInstance "
(" #
)# $
: 
base 
( 
) 
{ 	
} 	
public 
busBase 
	busObject  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
UserId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
DateTime 

START_DATE "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
DateTime 
END_DATE  
{! "
get# &
;& '
set( +
;+ ,
}- .
public## 
int## 
UserSerialId## 
{##  !
get##" %
;##% &
set##' *
;##* +
}##, -
public$$ 
bool$$ 
iblnNeedHistory$$ #
{$$$ %
get$$& )
;$$) *
set$$+ .
;$$. /
}$$0 1
public&& 
string&& 
istrRelativeVipFlag&& )
{&&* +
get&&, /
;&&/ 0
set&&1 4
;&&4 5
}&&6 7
public(( 
string(( 
istrProcessName(( %
{((& '
get((( +
;((+ ,
set((- 0
;((0 1
}((2 3
public)) 
string)) 
istrActivityName)) &
{))' (
get))) ,
;)), -
set)). 1
;))1 2
}))3 4
public++ 
override++ 
int++ 
Update++ "
(++" #
)++# $
{,, 	
if.. 
(.. 
!.. 
iblnNeedHistory..  
)..  !
{// 
if00 
(00 
ihstOldValues00 !
.00! "
Count00" '
>00( )
$num00* +
&&00, .
ihstOldValues00/ <
[00< =
$str00= K
]00K L
as00M O
string00P V
!=00W Y
status_value00Z f
)00f g
iblnNeedHistory11 #
=11$ %
true11& *
;11* +
}22 
int44 

lintResult44 
=44 
base44 !
.44! "
Update44" (
(44( )
)44) *
;44* +
if55 
(55 
iblnNeedHistory55 
)55  
{66 )
UpdateActivityInstanceHistory77 -
(77- .
)77. /
;77/ 0
iblnNeedHistory88 
=88  !
false88" '
;88' (
}99 
return:: 

lintResult:: 
;:: 
};; 	
public== 
override== 
int== 
Insert== "
(==" #
)==# $
{>> 	
int?? 

lintResult?? 
=?? 
base?? !
.??! "
Insert??" (
(??( )
)??) *
;??* +&
cdoActivityInstanceHistoryAA &&
newActivityInstanceHistoryAA' A
=AAB C
newAAD G&
cdoActivityInstanceHistoryAAH b
(AAb c
)AAc d
;AAd e&
newActivityInstanceHistoryBB &
.BB& ' 
activity_instance_idBB' ;
=BB< = 
activity_instance_idBB> R
;BBR S&
newActivityInstanceHistoryCC &
.CC& '

start_timeCC' 1
=CC2 3
DateTimeCC4 <
.CC< =
NowCC= @
;CC@ A&
newActivityInstanceHistoryDD &
.DD& '
status_valueDD' 3
=DD4 5
status_valueDD6 B
;DDB C&
newActivityInstanceHistoryEE &
.EE& '
action_user_idEE' 5
=EE6 7
iobjPassInfoEE8 D
.EED E

istrUserIDEEE O
;EEO P&
newActivityInstanceHistoryFF &
.FF& '
InsertFF' -
(FF- .
)FF. /
;FF/ 0
returnGG 

lintResultGG 
;GG 
}HH 	
privateJJ 
voidJJ )
UpdateActivityInstanceHistoryJJ 2
(JJ2 3
)JJ3 4
{KK 	
	DataTableMM #
ldtbLastActivityHistoryMM -
=MM. /
busBaseMM0 7
.MM7 8
SelectMM8 >
(MM> ?
$str	MM? Ñ
,
MMÑ Ö
newNN? B
objectNNC I
[NNI J
$numNNJ K
]NNK L
{NNM N 
activity_instance_idNNO c
}NNd e
)NNe f
;NNf g
ifOO 
(OO #
ldtbLastActivityHistoryOO '
.OO' (
RowsOO( ,
.OO, -
CountOO- 2
>OO3 4
$numOO5 6
)OO6 7
{PP &
cdoActivityInstanceHistoryQQ **
lobjcdoActivityInstanceHistoryQQ+ I
=QQJ K
newQQL O&
cdoActivityInstanceHistoryQQP j
(QQj k
)QQk l
;QQl m*
lobjcdoActivityInstanceHistoryRR .
.RR. /
LoadDataRR/ 7
(RR7 8#
ldtbLastActivityHistoryRR8 O
.RRO P
RowsRRP T
[RRT U
$numRRU V
]RRV W
)RRW X
;RRX Y*
lobjcdoActivityInstanceHistorySS .
.SS. /
end_timeSS/ 7
=SS8 9
DateTimeSS: B
.SSB C
NowSSC F
;SSF G*
lobjcdoActivityInstanceHistoryTT .
.TT. /
UpdateTT/ 5
(TT5 6
)TT6 7
;TT7 8
}UU &
cdoActivityInstanceHistoryXX &&
newActivityInstanceHistoryXX' A
=XXB C
newXXD G&
cdoActivityInstanceHistoryXXH b
(XXb c
)XXc d
;XXd e&
newActivityInstanceHistoryYY &
.YY& ' 
activity_instance_idYY' ;
=YY< = 
activity_instance_idYY> R
;YYR S&
newActivityInstanceHistoryZZ &
.ZZ& '

start_timeZZ' 1
=ZZ2 3
DateTimeZZ4 <
.ZZ< =
NowZZ= @
;ZZ@ A
if[[ 
([[ 
([[ 
status_value[[ 
==[[  
$str[[! '
)[[' (
||[[) +
([[, -
status_value[[- 9
==[[: <
$str[[= C
)[[C D
||[[E G
([[H I
status_value[[I U
==[[V X
$str[[Y _
)[[_ `
||[[a c
([[d e
status_value[[e q
==[[r t
$str[[u {
)[[{ |
)[[| }&
newActivityInstanceHistory\\ *
.\\* +
end_time\\+ 3
=\\4 5
DateTime\\6 >
.\\> ?
Now\\? B
;\\B C&
newActivityInstanceHistory]] &
.]]& '
status_value]]' 3
=]]4 5
status_value]]6 B
;]]B C&
newActivityInstanceHistory^^ &
.^^& '
action_user_id^^' 5
=^^6 7
iobjPassInfo^^8 D
.^^D E

istrUserID^^E O
;^^O P&
newActivityInstanceHistory__ &
.__& '
Insert__' -
(__- .
)__. /
;__/ 0
}`` 	
}aa 
}bb Ñ,
IC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\busEnumeration.cs
	namespace 	
MPIPHP
 
. 
Common 
{ 
public 

enum 
SERVICETYPE 
{		 
NORMAL

 
,

 
VESTING 
} 
public 

enum 
REPORTING_FREQUENCY #
{ 
BIWEEKLY 
, 
MONTHLY 
, 
SEMIMONTHLY 
} 
public 

enum 
EMPLOYMENT_TYPE 
{ 
EMPLOYEE 
, 
EMPLOYER 
} 
public 

enum $
RETIREMENT_AMOUNT_BUCKET (
{ 
PRETAX 
, 
POSTTAX 
, 
INTEREST 
} 
public!! 

enum!! 
LUMPSUM_BENEFICIARY!! #
{"" 
ORGANIZATION## 
,## 
MEMBER$$ 
}%% 
public'' 

enum'' !
BENEFIT_ACCOUNT_USAGE'' %
{(( 
CALCULATION)) 
,)) 
REFUND** 
}++ 
public-- 

enum-- 
PAYMENT_RECIPIENT-- !
{.. 
MEMBER// 
,// 
BENEFICIARY00 
}11 
public33 

enum33 
PAYEE_ACCOUNT_USAGE33 #
{44 
CALCULATION55 
,55 
REFUND66 
}77 
[:: 
Serializable:: 
]:: 
public;; 

enum;; 
FrequencyType;; 
{<< 
Once== 
=== 
$num== 
,== 
Daily>> 
=>> 
$num>> 
,>> 
Weekly?? 
=?? 
$num?? 
,?? 
Monthly@@ 
=@@ 
$num@@ 
,@@ 
MonthlyRelativeAA 
=AA 
$numAA 
,AA 
ImmediatelyBB 
=BB 
$numBB 
}EE 
[GG 
SerializableGG 
]GG 
[HH 
FlagsHH 

]HH
 
publicII 

enumII #
WeeklyFrequencyIntervalII '
{JJ 
SundayKK 
=KK 
$numKK 
,KK 
MondayLL 
=LL 
$numLL 
,LL 
TuesdayMM 
=MM 
$numMM 
,MM 
	WednesdayNN 
=NN 
$numNN 
,NN 
ThursdayOO 
=OO 
$numOO 
,OO 
FridayPP 
=PP 
$numPP 
,PP 
SaturdayQQ 
=QQ 
$numQQ 
}RR 
[TT 
SerializableTT 
]TT 
publicUU 

enumUU ,
 MonthlyRelativeFrequencyIntervalUU 0
{VV 
SundayWW 
=WW 
$numWW 
,WW 
MondayXX 
=XX 
$numXX 
,XX 
TuesdayYY 
=YY 
$numYY 
,YY 
	WednesdayZZ 
=ZZ 
$numZZ 
,ZZ 
Thursday[[ 
=[[ 
$num[[ 
,[[ 
Friday\\ 
=\\ 
$num\\ 
,\\ 
Saturday]] 
=]] 
$num]] 
,]] 
Day^^ 
=^^ 
$num^^ 
,^^ 
Weekday__ 
=__ 
$num__ 
,__ 

WeekendDay`` 
=`` 
$num`` 
}aa 
[cc 
Serializablecc 
]cc 
publicdd 

enumdd 
FrequencySubDayTypedd #
{ee 
AtTheSpecifiedTimeff 
=ff 
$numff 
,ff 
Secondsgg 
=gg 
$numgg 
,gg 
Minuteshh 
=hh 
$numhh 
,hh 
Hoursii 
=ii 
$numii 
,ii 
DuringBatchWindowjj 
=jj 
$numjj 
}kk 
[mm 
Serializablemm 
]mm 
publicnn 

enumnn %
FrequencyRelativeIntervalnn )
{oo 
Firstpp 
=pp 
$numpp 
,pp 
Secondqq 
=qq 
$numqq 
,qq 
Thirdrr 
=rr 
$numrr 
,rr 
Fourthss 
=ss 
$numss 
,ss 
Lasttt 
=tt 
$numtt 
}uu 
publicww 

structww 
BenefitDetailsww  
{xx 
publicyy 
decimalyy 
TotalServiceCredityy )
;yy) *
publiczz 
decimalzz 
MemberAnnualBenefitzz *
;zz* +
public{{ 
decimal{{ 
FAS{{ 
;{{ 
public|| 
DateTime|| 
RetirementDate|| &
;||& '
public}} 
string}} 
RetirementAge}} #
;}}# $
}~~ 
[
ÄÄ 
Serializable
ÄÄ 
(
ÄÄ 
)
ÄÄ 
]
ÄÄ 
public
ÅÅ 

enum
ÅÅ ,
enmCommunicationRecipientGroup
ÅÅ .
{
ÇÇ 
NotSet
ÉÉ 
=
ÉÉ 
$num
ÉÉ 
,
ÉÉ !
OrganizationContact
ÑÑ 
=
ÑÑ 
$num
ÑÑ 
,
ÑÑ  
Person
ÖÖ 
=
ÖÖ 
$num
ÖÖ 
,
ÖÖ 
InternalUser
ÜÜ 
=
ÜÜ 
$num
ÜÜ 
}
áá 
}àà Æ¶
FC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\BatchHelper.cs
	namespace 	
MPIPHP
 
. 
Common 
{ 
public 

abstract 
class 
JobServiceCodes )
{		 
} 
public 

class 
BatchHelper 
{ 
public 
delegate 
bool 
dlgBoolIntDelegate /
(/ 0
int0 3
aint4 8
)8 9
;9 :
public 
static 
int 
DateToInt32 %
(% &
DateTime& .

adtCurrent/ 9
)9 :
{ 	
int 
rv 
; 
StringBuilder 
lstrbString %
=& '
new( +
StringBuilder, 9
(9 :
): ;
;; <
lstrbString 
. 
Append 
( 

adtCurrent )
.) *
Year* .
.. /
ToString/ 7
(7 8
$str8 <
)< =
)= >
;> ?
lstrbString 
. 
Append 
( 

adtCurrent )
.) *
Month* /
./ 0
ToString0 8
(8 9
$str9 =
)= >
)> ?
;? @
lstrbString 
. 
Append 
( 

adtCurrent )
.) *
Day* -
.- .
ToString. 6
(6 7
$str7 ;
); <
)< =
;= >
rv 
= 
Convert 
. 
ToInt32  
(  !
lstrbString! ,
., -
ToString- 5
(5 6
)6 7
)7 8
;8 9
return 
rv 
; 
} 	
public 
static 
DateTime 
Int32ToDate *
(* +
int+ .
current/ 6
)6 7
{   	
DateTime!! 
rv!! 
;!! 
int## 
year## 
;## 
int$$ 
month$$ 
;$$ 
int%% 
day%% 
;%% 
int&& 
	remainder&& 
;&& 
year(( 
=(( 
Math(( 
.(( 
DivRem(( 
((( 
current(( &
,((& '
$num((( -
,((- .
out((/ 2
	remainder((3 <
)((< =
;((= >
month)) 
=)) 
Math)) 
.)) 
DivRem)) 
())  
	remainder))  )
,))) *
$num))+ .
,)). /
out))0 3
	remainder))4 =
)))= >
;))> ?
day** 
=** 
Math** 
.** 
DivRem** 
(** 
	remainder** '
,**' (
$num**) *
,*** +
out**, /
	remainder**0 9
)**9 :
;**: ;
rv-- 
=-- 
new-- 
DateTime-- 
(-- 
year-- "
,--" #
month--$ )
,--) *
day--+ .
)--. /
;--/ 0
return// 
rv// 
;// 
}00 	
public22 
static22 
TimeSpan22 
Int32ToTime22 *
(22* +
int22+ .
current22/ 6
)226 7
{33 	
TimeSpan44 
ltsTimeSpan44  
;44  !
int66 
hours66 
;66 
int77 
minutes77 
;77 
int88 
seconds88 
;88 
int99 
	remainder99 
;99 
hours;; 
=;; 
Math;; 
.;; 
DivRem;; 
(;;  
current;;  '
,;;' (
$num;;) .
,;;. /
out;;0 3
	remainder;;4 =
);;= >
;;;> ?
minutes<< 
=<< 
Math<< 
.<< 
DivRem<< !
(<<! "
	remainder<<" +
,<<+ ,
$num<<- 0
,<<0 1
out<<2 5
	remainder<<6 ?
)<<? @
;<<@ A
seconds== 
=== 
Math== 
.== 
DivRem== !
(==! "
	remainder==" +
,==+ ,
$num==- .
,==. /
out==0 3
	remainder==4 =
)=== >
;==> ?
ltsTimeSpan@@ 
=@@ 
new@@ 
TimeSpan@@ &
(@@& '
hours@@' ,
,@@, -
minutes@@. 5
,@@5 6
seconds@@7 >
)@@> ?
;@@? @
returnBB 
ltsTimeSpanBB 
;BB 
}CC 	
publicEE 
staticEE 
intEE 
TimeToInt32EE %
(EE% &
DateTimeEE& .

adtCurrentEE/ 9
)EE9 :
{FF 	
intGG 
rvGG 
;GG 
StringBuilderHH 
lstrbStringHH %
=HH& '
newHH( +
StringBuilderHH, 9
(HH9 :
)HH: ;
;HH; <
lstrbStringJJ 
.JJ 
AppendJJ 
(JJ 

adtCurrentJJ )
.JJ) *
	TimeOfDayJJ* 3
.JJ3 4
HoursJJ4 9
.JJ9 :
ToStringJJ: B
(JJB C
$strJJC G
)JJG H
)JJH I
;JJI J
lstrbStringKK 
.KK 
AppendKK 
(KK 

adtCurrentKK )
.KK) *
	TimeOfDayKK* 3
.KK3 4
MinutesKK4 ;
.KK; <
ToStringKK< D
(KKD E
$strKKE I
)KKI J
)KKJ K
;KKK L
lstrbStringLL 
.LL 
AppendLL 
(LL 

adtCurrentLL )
.LL) *
	TimeOfDayLL* 3
.LL3 4
SecondsLL4 ;
.LL; <
ToStringLL< D
(LLD E
$strLLE I
)LLI J
)LLJ K
;LLK L
rvNN 
=NN 
ConvertNN 
.NN 
ToInt32NN  
(NN  !
lstrbStringNN! ,
.NN, -
ToStringNN- 5
(NN5 6
)NN6 7
)NN7 8
;NN8 9
returnPP 
rvPP 
;PP 
}QQ 	
publicSS 
staticSS 
boolSS 
IsDateBetweenSS (
(SS( )
intSS) ,
currentSS- 4
,SS4 5
intSS6 9
startSS: ?
,SS? @
intSSA D
endSSE H
)SSH I
{TT 	
returnUU 
	IsBetweenUU 
(UU 
currentUU $
,UU$ %
startUU& +
,UU+ ,
endUU- 0
)UU0 1
;UU1 2
}VV 	
publicXX 
staticXX 
boolXX 
IsTimeBetweenXX (
(XX( )
intXX) ,
currentXX- 4
,XX4 5
intXX6 9
startXX: ?
,XX? @
intXXA D
endXXE H
)XXH I
{YY 	
returnZZ 
	IsBetweenZZ 
(ZZ 
currentZZ $
,ZZ$ %
startZZ& +
,ZZ+ ,
endZZ- 0
)ZZ0 1
;ZZ1 2
}[[ 	
public^^ 
static^^ 
bool^^ 
IsDateBetween^^ (
(^^( )
DateTime^^) 1
current^^2 9
,^^9 :
DateTime^^; C
start^^D I
,^^I J
DateTime^^K S
end^^T W
)^^W X
{__ 	
if`` 
(`` 
end`` 
==`` 
DateTime`` 
.``  
MinValue``  (
)``( )
endaa 
=aa 
DateTimeaa 
.aa 
MaxValueaa '
;aa' (
returncc 
(cc 
(cc 
currentcc 
>=cc 
startcc  %
)cc% &
&&cc' )
(cc* +
currentcc+ 2
<=cc3 5
endcc6 9
)cc9 :
)cc: ;
;cc; <
}ee 	
publicgg 
staticgg 
boolgg 
	IsBetweengg $
(gg$ %
intgg% (
currentgg) 0
,gg0 1
intgg2 5
startgg6 ;
,gg; <
intgg= @
endggA D
)ggD E
{hh 	
boolii 
rvii 
=ii 
falseii 
;ii 
rvkk 
=kk 
(kk 
(kk 
currentkk 
>=kk 
startkk #
)kk# $
&&kk% '
(kk( )
currentkk) 0
<=kk1 3
endkk4 7
)kk7 8
)kk8 9
;kk9 :
returnmm 
rvmm 
;mm 
}nn 	
publicuu 
staticuu 
stringuu #
ServiceInYearsAndMonthsuu 4
(uu4 5
decimaluu5 <
adecServiceuu= H
)uuH I
{vv 	
stringww '
lstrServiceInYearsAndMonthsww .
=ww/ 0
stringww1 7
.ww7 8
Emptyww8 =
;ww= >
intyy 
	lintYearsyy 
=yy 
(yy 
intyy  
)yy  !
Mathyy! %
.yy% &
Roundyy& +
(yy+ ,
adecServiceyy, 7
/yy8 9
$numyy: @
)yy@ A
;yyA B
intzz 

lintMonthszz 
=zz 
(zz 
intzz !
)zz! "
Mathzz" &
.zz& '
Roundzz' ,
(zz, -
adecServicezz- 8
%zz9 :
$numzz; A
)zzA B
;zzB C
if|| 
(|| 
	lintYears|| 
>|| 
$num|| 
&&||  

lintMonths||! +
>||, -
$num||. /
)||/ 0'
lstrServiceInYearsAndMonths}} +
=}}, -
string}}. 4
.}}4 5
Format}}5 ;
(}}; <
$str}}< Z
,}}Z [
	lintYears}}\ e
,}}e f

lintMonths}}g q
)}}q r
;}}r s
else~~ 
if~~ 
(~~ 
	lintYears~~ 
>~~  
$num~~! "
)~~" #'
lstrServiceInYearsAndMonths +
=, -
string. 4
.4 5
Format5 ;
(; <
$str< I
,I J
	lintYearsK T
)T U
;U V
else
ÄÄ )
lstrServiceInYearsAndMonths
ÅÅ +
=
ÅÅ, -
string
ÅÅ. 4
.
ÅÅ4 5
Format
ÅÅ5 ;
(
ÅÅ; <
$str
ÅÅ< J
,
ÅÅJ K

lintMonths
ÅÅL V
)
ÅÅV W
;
ÅÅW X
return
ÉÉ )
lstrServiceInYearsAndMonths
ÉÉ .
;
ÉÉ. /
}
ÑÑ 	
public
åå 
const
åå 
string
åå 
MAX_DATE
åå $
=
åå% &
$str
åå' )
;
åå) *
public
çç 
const
çç 
string
çç &
JOB_HEADER_STATUS_REVIEW
çç 4
=
çç5 6
$str
çç7 =
;
çç= >
public
éé 
const
éé 
string
éé %
JOB_HEADER_STATUS_VALID
éé 3
=
éé4 5
$str
éé6 <
;
éé< =
public
èè 
const
èè 
string
èè 3
%JOB_HEADER_STATUS_SUBMIT_FOR_APPROVAL
èè A
=
èèB C
$str
èèD J
;
èèJ K
public
êê 
const
êê 
string
êê /
!CODE_VALUE_BATCH_EXECUTION_WINDOW
êê =
=
êê> ?
$str
êê@ F
;
êêF G
public
ëë 
const
ëë 
int
ëë ,
CODE_ID_BATCH_EXECUTION_WINDOW
ëë 7
=
ëë8 9
$num
ëë: <
;
ëë< =
public
íí 
const
íí 
string
íí )
JOB_HEADER_STATUS_PICKED_UP
íí 7
=
íí8 9
$str
íí: @
;
íí@ A
public
ìì 
const
ìì 
string
ìì &
JOB_HEADER_STATUS_QUEUED
ìì 4
=
ìì5 6
$str
ìì7 =
;
ìì= >
public
îî 
const
îî 
string
îî *
JOB_HEADER_STATUS_PROCESSING
îî 8
=
îî9 :
$str
îî; A
;
îîA B
public
ïï 
const
ïï 
string
ïï 6
(JOB_HEADER_STATUS_PROCESSED_SUCCESSFULLY
ïï D
=
ïïE F
$str
ïïG M
;
ïïM N
public
ññ 
const
ññ 
string
ññ 5
'JOB_HEADER_STATUS_PROCESSED_WITH_ERRORS
ññ C
=
ññD E
$str
ññF L
;
ññL M
public
óó 
const
óó 
string
óó 0
"JOB_HEADER_STATUS_CANCEL_REQUESTED
óó >
=
óó? @
$str
óóA G
;
óóG H
public
òò 
const
òò 
string
òò )
JOB_HEADER_STATUS_CANCELLED
òò 7
=
òò8 9
$str
òò: @
;
òò@ A
public
öö 
const
öö 
string
öö /
!JOB_SCHEDULE_HEADER_STATUS_REVIEW
öö =
=
öö> ?
$str
öö@ F
;
ööF G
public
õõ 
const
õõ 
string
õõ .
 JOB_SCHEDULE_HEADER_STATUS_VALID
õõ <
=
õõ= >
$str
õõ? E
;
õõE F
public
úú 
const
úú 
string
úú <
.JOB_SCHEDULE_HEADER_STATUS_SUBMIT_FOR_APPROVAL
úú J
=
úúK L
$str
úúM S
;
úúS T
public
ùù 
const
ùù 
string
ùù 1
#JOB_SCHEDULE_HEADER_STATUS_APPROVED
ùù ?
=
ùù@ A
$str
ùùB H
;
ùùH I
public
üü 
const
üü 
string
üü /
!JOB_SCHEDULE_DETAIL_STATUS_REVIEW
üü =
=
üü> ?
$str
üü@ F
;
üüF G
public
†† 
const
†† 
string
†† .
 JOB_SCHEDULE_DETAIL_STATUS_VALID
†† <
=
††= >
$str
††? E
;
††E F
public
¢¢ 
const
¢¢ 
string
¢¢ &
JOB_DETAIL_STATUS_REVIEW
¢¢ 4
=
¢¢5 6
$str
¢¢7 =
;
¢¢= >
public
££ 
const
££ 
string
££ %
JOB_DETAIL_STATUS_VALID
££ 3
=
££4 5
$str
££6 <
;
££< =
public
•• 
const
•• 
string
•• &
JOB_DETAIL_STATUS_QUEUED
•• 4
=
••5 6
$str
••7 =
;
••= >
public
¶¶ 
const
¶¶ 
string
¶¶ *
JOB_DETAIL_STATUS_PROCESSING
¶¶ 8
=
¶¶9 :
$str
¶¶; A
;
¶¶A B
public
ßß 
const
ßß 
string
ßß 6
(JOB_DETAIL_STATUS_PROCESSED_SUCCESSFULLY
ßß D
=
ßßE F
$str
ßßG M
;
ßßM N
public
®® 
const
®® 
string
®® 5
'JOB_DETAIL_STATUS_PROCESSED_WITH_ERRORS
®® C
=
®®D E
$str
®®F L
;
®®L M
public
©© 
const
©© 
string
©© '
JOB_DETAIL_STATUS_SKIPPED
©© 5
=
©©6 7
$str
©©8 >
;
©©> ?
public
™™ 
const
™™ 
string
™™ )
JOB_DETAIL_STATUS_CANCELLED
™™ 7
=
™™8 9
$str
™™: @
;
™™@ A
public
´´ 
const
´´ 
string
´´ 0
"JOB_DETAIL_STATUS_CANCEL_REQUESTED
´´ >
=
´´? @
$str
´´A G
;
´´G H
public
≠≠ 
const
≠≠ 
int
≠≠ +
CODE_ID_FOR_JOB_HEADER_STATUS
≠≠ 6
=
≠≠7 8
$num
≠≠9 =
;
≠≠= >
public
ÆÆ 
const
ÆÆ 
int
ÆÆ +
CODE_ID_FOR_JOB_DETAIL_STATUS
ÆÆ 6
=
ÆÆ7 8
$num
ÆÆ9 =
;
ÆÆ= >
public
ØØ 
const
ØØ 
int
ØØ 5
'CODE_ID_FOR_JOB_SCHEDULE_FREQUENCY_TYPE
ØØ @
=
ØØA B
$num
ØØC G
;
ØØG H
public
∞∞ 
const
∞∞ 
int
∞∞ -
CODE_ID_FOR_JOB_SCHEDULE_STATUS
∞∞ 8
=
∞∞9 :
$num
∞∞; ?
;
∞∞? @
public
±± 
const
±± 
int
±± 4
&CODE_ID_FOR_JOB_SCHEDULE_DETAIL_STATUS
±± ?
=
±±@ A
$num
±±B F
;
±±F G
public
≥≥ 
const
≥≥ 
string
≥≥ +
JOB_SCHEDULE_END_DATE_PRESENT
≥≥ 9
=
≥≥: ;
$str
≥≥< N
;
≥≥N O
public
¥¥ 
const
¥¥ 
string
¥¥ .
 JOB_SCHEDULE_NO_END_DATE_PRESENT
¥¥ <
=
¥¥= >
$str
¥¥? T
;
¥¥T U
public
∂∂ 
const
∂∂ 
string
∂∂ 0
"JOB_SCHEDULE_SUBDAY_FREQUENCY_ONCE
∂∂ >
=
∂∂? @
$str
∂∂A G
;
∂∂G H
public
∑∑ 
const
∑∑ 
string
∑∑ 1
#JOB_SCHEDULE_SUBDAY_FREQUENCY_EVERY
∑∑ ?
=
∑∑@ A
$str
∑∑B I
;
∑∑I J
public
∏∏ 
const
∏∏ 
string
∏∏ 8
*JOB_SCHEDULE_SUBDAY_FREQUENCY_BATCH_WINDOW
∏∏ F
=
∏∏G H
$str
∏∏I V
;
∏∏V W
public
∫∫ 
const
∫∫ 
int
∫∫ /
!JOB_SCHEDULE_FREQUENCY_TYPE_DAILY
∫∫ :
=
∫∫; <
$num
∫∫= >
;
∫∫> ?
public
ªª 
const
ªª 
int
ªª 0
"JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY
ªª ;
=
ªª< =
$num
ªª> ?
;
ªª? @
public
ºº 
const
ºº 
int
ºº 1
#JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY
ºº <
=
ºº= >
$num
ºº? A
;
ººA B
public
ΩΩ 
const
ΩΩ 
int
ΩΩ :
,JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE
ΩΩ E
=
ΩΩF G
$num
ΩΩH J
;
ΩΩJ K
public
ææ 
const
ææ 
int
ææ 3
%JOB_SCHEDULE_FREQUENCY_TYPE_IMMEDIATE
ææ >
=
ææ? @
$num
ææA C
;
ææC D
public
¿¿ 
const
¿¿ 
int
¿¿ ;
-JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_MINUTES
¿¿ F
=
¿¿G H
$num
¿¿I J
;
¿¿J K
public
¡¡ 
const
¡¡ 
int
¡¡ 9
+JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_HOURS
¡¡ D
=
¡¡E F
$num
¡¡G H
;
¡¡H I
public
¬¬ 
const
¬¬ 
decimal
¬¬ 1
#CAP_FOR_JS_BASIC_BENEFIT_MULTIPLIER
¬¬ @
=
¬¬A B
$num
¬¬C G
;
¬¬G H
public
√√ 
const
√√ 
decimal
√√ %
DIFFERENCE_IN_AGE_CHECK
√√ 4
=
√√5 6
$num
√√7 ;
;
√√; <
public
ƒƒ 
const
ƒƒ 
string
ƒƒ 
COMMA
ƒƒ !
=
ƒƒ" #
$str
ƒƒ$ '
;
ƒƒ' (
public
≈≈ 
const
≈≈ 
string
≈≈ 
SPACE
≈≈ !
=
≈≈" #
$str
≈≈$ '
;
≈≈' (
public
∆∆ 
const
∆∆ 
int
∆∆ %
NUMBER_OF_DAYS_IN_MONTH
∆∆ 0
=
∆∆1 2
$num
∆∆3 5
;
∆∆5 6
public
«« 
const
«« 
int
«« &
NUMBER_OF_MONTHS_IN_YEAR
«« 1
=
««2 3
$num
««4 6
;
««6 7
public
»» 
const
»» 
int
»» $
NUMBER_OF_DAYS_IN_YEAR
»» /
=
»»0 1
$num
»»2 5
;
»»5 6
public
…… 
const
…… 
int
…… )
NUMBER_OF_DAYS_IN_LEAP_YEAR
…… 4
=
……5 6
$num
……7 :
;
……: ;
public
   
const
   
string
   G
9NUMBER_OF_DAYS_DIFFERENCE_FOR_MONTHLY_REPORTING_FREQUENCY
   U
=
  V W
$str
  X ^
;
  ^ _
public
ÀÀ 
const
ÀÀ 
string
ÀÀ K
=NUMBER_OF_DAYS_DIFFERENCE_FOR_SEMIMONTHLY_REPORTING_FREQUENCY
ÀÀ Y
=
ÀÀZ [
$str
ÀÀ\ b
;
ÀÀb c
public
ÃÃ 
const
ÃÃ 
string
ÃÃ H
:NUMBER_OF_DAYS_DIFFERENCE_FOR_BIWEEKLY_REPORTING_FREQUENCY
ÃÃ V
=
ÃÃW X
$str
ÃÃY _
;
ÃÃ_ `
public
ÕÕ 
const
ÕÕ 
string
ÕÕ *
REPORTING_FREQUENCY_BIWEEKLY
ÕÕ 8
=
ÕÕ9 :
$str
ÕÕ; A
;
ÕÕA B
public
ŒŒ 
const
ŒŒ 
string
ŒŒ .
 REPORTING_FREQUENCY_SEMI_MONTHLY
ŒŒ <
=
ŒŒ= >
$str
ŒŒ? E
;
ŒŒE F
public
œœ 
const
œœ 
string
œœ )
REPORTING_FREQUENCY_MONTHLY
œœ 7
=
œœ8 9
$str
œœ: @
;
œœ@ A
public
–– 
const
–– 
int
–– &
CODE_ID_SYSTEM_CONSTANTS
–– 1
=
––2 3
$num
––4 6
;
––6 7
public
““ 
const
““ 
string
““ 

BATCH_USER
““ &
=
““' (
$str
““) 7
;
““7 8
public
”” 
const
”” 
string
”” #
BATCH_MESSAGE_SUMMARY
”” 1
=
””2 3
$str
””4 :
;
””: ;
public
‘‘ 
const
‘‘ 
string
‘‘ !
BATCH_MESSAGE_ERROR
‘‘ /
=
‘‘0 1
$str
‘‘2 7
;
‘‘7 8
public
’’ 
const
’’ 
string
’’  
BATCH_MESSAGE_INFO
’’ .
=
’’/ 0
$str
’’1 7
;
’’7 8
}
ŸŸ 
}⁄⁄ Ì#
NC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\ApplicationSettings.cs
	namespace		 	
MPIPHP		
 
.		 
Common		 
{

 
[ 
Serializable 
] 
public 

class 
ApplicationSettings $
:% &
SystemSettings' 5
{ 
public 
new 
static 
ApplicationSettings -
Instance. 6
{ 	
get 
{ 
return 
SystemSettings %
.% &
Instance& .
as/ 1
ApplicationSettings2 E
;E F
} 
} 	
static 
ApplicationSettings "
(" #
)# $
{ 	
SystemSettings 
. $
InitializeSettingsObject 3
=4 5
delegate6 >
(? @
)@ A
{B C
returnD J
newK N
ApplicationSettingsO b
(b c
)c d
;d e
}f g
;g h
} 	
public 
static 
void 
MapSettingsObject ,
(, -
)- .
{ 	
SystemSettings   
.   $
InitializeSettingsObject   3
=  4 5
delegate  6 >
(  ? @
)  @ A
{  B C
return  D J
new  K N
ApplicationSettings  O b
(  b c
)  c d
;  d e
}  f g
;  g h
}!! 	
	protected## 
ApplicationSettings## %
(##% &
)##& '
:##( )
base##* .
(##. /
)##/ 0
{$$ 	
}&& 	
public(( 
ApplicationSettings(( "
(((" #
SerializationInfo((# 4
info((5 9
,((9 :
StreamingContext((; K
ctxt((L P
)((P Q
:((R S
base((T X
(((X Y
info((Y ]
,((] ^
ctxt((_ c
)((c d
{)) 	
}** 	
public,, 
override,, 
void,, 
GetObjectData,, *
(,,* +
SerializationInfo,,+ <
info,,= A
,,,A B
StreamingContext,,C S
context,,T [
),,[ \
{-- 	
base.. 
... 
GetObjectData.. 
(.. 
info.. #
,..# $
context..% ,
).., -
;..- .
}// 	
public22 
string22 
SMTP_HOST_PATH22 $
{22% &
get22' *
;22* +
	protected22, 5
set226 9
;229 :
}22: ;
public44 
string44 
SMTP_USERNAME44 #
{44$ %
get44& )
;44) *
	protected44+ 4
set445 8
;448 9
}449 :
public66 
string66 
SMTP_PASSWORD66 #
{66$ %
get66& )
;66) *
	protected66+ 4
set665 8
;668 9
}669 :
public88 
string88 
SMTP_HOST_PORT88 $
{88% &
get88' *
;88* +
	protected88, 5
set886 9
;889 :
}88: ;
public:: 
string:: 
NeoFlowMapPath:: $
{::% &
get::' *
;::* +
	protected::, 5
set::6 9
;::9 :
}::: ;
public<< 
string<< 
MetaDataCacheUrl<< &
{<<' (
get<<) ,
;<<, -
	protected<<. 7
set<<8 ;
;<<; <
}<<< =
public>> 
string>> 

DBCacheUrl>>  
{>>! "
get>># &
;>>& '
	protected>>( 1
set>>2 5
;>>5 6
}>>6 7
public@@ 
string@@ 
WebExtenderPath@@ %
{@@& '
get@@( +
;@@+ ,
	protected@@- 6
set@@7 :
;@@: ;
}@@; <
publicBB 
stringBB 

V3DataPathBB  
{BB! "
getBB# &
;BB& '
	protectedBB( 1
setBB2 5
;BB5 6
}BB7 8
publicDD 
stringDD #
StateTaxBatchFutureFlagDD -
{DD. /
getDD0 3
;DD3 4
	protectedDD5 >
setDD? B
;DDB C
}DDD E
publicGG 
stringGG (
NEOFLOW_SERVICE_WORKFLOW_URLGG 2
{GG3 4
getGG5 8
;GG8 9
	protectedGG: C
setGGD G
;GGG H
}GGH I
}HH 
}II Ω	
TC:\Sonarqube Codebase\MPIBPM_DEV\slnMPIPHP\MPIPHPCommon\ActivityInstanceEventArgs.cs
	namespace 	
MPIPHP
 
. 
Common 
{ 
[ 
Serializable 
] 
public 

class %
ActivityInstanceEventArgs *
{		 
public

 %
ActivityInstanceEventArgs

 (
(

( )
)

) *
{ 	
} 	
public 
cdoActivityInstance " 
icdoActivityInstance# 7
{8 9
get: =
;= >
set? B
;B C
}D E
public 
enmNextAction 
ienmNextAction +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
} 
[ 
Serializable 
] 
public 

enum 
enmNextAction 
{ 
Next 
, 
Previous 
, 
First 
, 
Return 
, 
Correspondance 
, 
Cancel 
, 

ReturnBack 
} 
} 