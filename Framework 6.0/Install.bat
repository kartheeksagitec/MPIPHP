C:
cd C:\OPUS\MPIBPM_DEV\Framework 6.0

"NETFX 4.0 Tools\gacutil.exe" -u SagitecCommon,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecInterface,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecExceptionPub.Interfaces,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecExceptionPub,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBusinessObjects,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBusinessTier,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecDBCache,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecDBUtility,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecMetaDataCache,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecWebControls,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecWebClient,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecRules,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecMVVMClient,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBPM,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecCorBuilder,Version=6.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecQuestionnaire,Version=6.0.0.0

"NETFX 4.0 Tools\gacutil.exe" -u SagitecCommon,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecInterface,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecExceptionPub.Interfaces,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecExceptionPub,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBusinessObjects,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBusinessTier,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecDBCache,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecDBUtility,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecMetaDataCache,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecWebControls,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecWebClient,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecRules,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecMVVMClient,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecBPM,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecCorBuilder,Version=5.0.0.0
"NETFX 4.0 Tools\gacutil.exe" -u SagitecQuestionnaire,Version=5.0.0.0

"NETFX 4.0 Tools\gacutil.exe" -u OpenXmlPowerTools.dll
"NETFX 4.0 Tools\gacutil.exe" -u XelenceConnector.dll
"NETFX 4.0 Tools\gacutil.exe" -u BaseConnector.dll

pause

"NETFX 4.0 Tools\gacutil.exe" -i SagitecCommon.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecInterface.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecExceptionPub.Interfaces.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecExceptionPub.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecBusinessObjects.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecBusinessTier.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecDBCache.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecDBUtility.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecMetaDataCache.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecWebControls.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecWebClient.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecRules.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecCorBuilder.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecMVVMClient.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecBPM.dll
"NETFX 4.0 Tools\gacutil.exe" -i SagitecQuestionnaire.dll

"NETFX 4.0 Tools\gacutil.exe" -i Microsoft.Web.Infrastructure
"NETFX 4.0 Tools\gacutil.exe" -i OpenXmlPowerTools.dll
"NETFX 4.0 Tools\gacutil.exe" -i XelenceConnector.dll
"NETFX 4.0 Tools\gacutil.exe" -i BaseConnector.dll
pause
iisreset
pause
