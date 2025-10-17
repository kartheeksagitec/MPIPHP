IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\asharma')
BEGIN 
CREATE LOGIN [MPIDOM\asharma] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END
exec sp_addsrvrolemember N'MPIDOM\asharma', N'dbcreator'
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\EA FullAccess')
BEGIN 
CREATE LOGIN [MPIDOM\EA FullAccess] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\EA ReadOnly')
BEGIN 
CREATE LOGIN [MPIDOM\EA ReadOnly] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\hrao')
BEGIN 
CREATE LOGIN [MPIDOM\hrao] FROM WINDOWS WITH DEFAULT_DATABASE=[OPUS], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\IMG-BENEFITSRECOVERY')
BEGIN 
CREATE LOGIN [MPIDOM\IMG-BENEFITSRECOVERY] FROM WINDOWS WITH DEFAULT_DATABASE=[OPUS], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\IMG-CLAIMS-EXAMINERS')
BEGIN 
CREATE LOGIN [MPIDOM\IMG-CLAIMS-EXAMINERS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\IMG-PENSION')
BEGIN 
CREATE LOGIN [MPIDOM\IMG-PENSION] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\IMG-PSC')
BEGIN 
CREATE LOGIN [MPIDOM\IMG-PSC] FROM WINDOWS WITH DEFAULT_DATABASE=[OPUS], DEFAULT_LANGUAGE=[us_english]
END


IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\it_core')
BEGIN 
CREATE LOGIN [MPIDOM\it_core] FROM WINDOWS WITH DEFAULT_DATABASE=[OPUS], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles

exec sp_addsrvrolemember N'MPIDOM\it_core', N'securityadmin'
GO


IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\karora')
BEGIN 
CREATE LOGIN [MPIDOM\karora] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\kyoung')
BEGIN 
CREATE LOGIN [MPIDOM\kyoung] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\phaines')
BEGIN 
CREATE LOGIN [MPIDOM\phaines] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\PIDNCOAFullAccess')
BEGIN 
CREATE LOGIN [MPIDOM\PIDNCOAFullAccess] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\ppunjabi')
BEGIN 
CREATE LOGIN [MPIDOM\ppunjabi] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles

exec sp_addsrvrolemember N'MPIDOM\ppunjabi', N'dbcreator'
GO

exec sp_addsrvrolemember N'MPIDOM\ppunjabi', N'sysadmin'
GO


IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\rgoldring')
BEGIN 
CREATE LOGIN [MPIDOM\rgoldring] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\rmenta Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\rmenta')
BEGIN 
CREATE LOGIN [MPIDOM\rmenta] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\rwittmann Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\rwittmann')
BEGIN 
CREATE LOGIN [MPIDOM\rwittmann] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\scanop Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\scanop')
BEGIN 
CREATE LOGIN [MPIDOM\scanop] FROM WINDOWS WITH DEFAULT_DATABASE=[LOOKUP], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\sjain Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\sjain')
BEGIN 
CREATE LOGIN [MPIDOM\sjain] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles

exec sp_addsrvrolemember N'MPIDOM\sjain', N'dbcreator'
GO

exec sp_addsrvrolemember N'MPIDOM\sjain', N'sysadmin'
GO


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\SQLAdminT Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\SQLAdminT')
BEGIN 
CREATE LOGIN [MPIDOM\SQLAdminT] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles

exec sp_addsrvrolemember N'MPIDOM\SQLAdminT', N'sysadmin'
GO


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\vthomas Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\vthomas')
BEGIN 
CREATE LOGIN [MPIDOM\vthomas] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\wcfGoGreen Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\wcfGoGreen')
BEGIN 
CREATE LOGIN [MPIDOM\wcfGoGreen] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\wcfMembership Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\wcfMembership')
BEGIN 
CREATE LOGIN [MPIDOM\wcfMembership] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\wcfPRS Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\wcfPRS')
BEGIN 
CREATE LOGIN [MPIDOM\wcfPRS] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles


/*********** Permissions Script for 10.100.104.30 Login MPIDOM\WXIMPERSONATION Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MPIDOM\WXIMPERSONATION')
BEGIN 
CREATE LOGIN [MPIDOM\WXIMPERSONATION] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


--Scripting Server Roles

exec sp_addsrvrolemember N'MPIDOM\WXIMPERSONATION', N'bulkadmin'
GO

exec sp_addsrvrolemember N'MPIDOM\WXIMPERSONATION', N'sysadmin'
GO


/*********** Permissions Script for 10.100.104.30 Login sysop Generated 3/21/2013 4:10:45 PM ***************/


--Scripting Server Login (password is null) and Default Database

/* For security reasons the login is created disabled and with a random password. */
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'sysop')
BEGIN 
CREATE LOGIN [sysop] WITH PASSWORD=N'IÔKÚ?ö?XfZ!ÛQß¼²"ü¿:<dg?:çû?', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF,  CHECK_POLICY=OFF 
ALTER LOGIN [sysop] DISABLE
END


--Scripting Server Roles



USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\asharma')
CREATE USER [MPIDOM\asharma] FOR LOGIN [MPIDOM\asharma] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_owner', N'MPIDOM\asharma'
GO


exec sp_addrolemember N'db_securityadmin', N'MPIDOM\asharma'
GO


--Create Database Role

USE [OPUS]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'persistenceUsers' AND type = 'R')
CREATE ROLE [persistenceUsers] AUTHORIZATION [MPIDOM\asharma]


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[DeleteInstance] TO [persistenceUsers]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[InsertInstance] TO [persistenceUsers]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[LoadInstance] TO [persistenceUsers]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[UnlockInstance] TO [persistenceUsers]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[UpdateInstance] TO [persistenceUsers]  
GO


exec sp_addrolemember N'persistenceUsers', N'MPIDOM\asharma'
GO


--Create Database Role

USE [OPUS]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'state_persistence_users' AND type = 'R')
CREATE ROLE [state_persistence_users] AUTHORIZATION [MPIDOM\asharma]


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[DeleteCompletedScope] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[InsertCompletedScope] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[InsertInstanceState] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RetrieveANonblockingInstanceStateId] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RetrieveCompletedScope] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RetrieveExpiredTimerIds] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RetrieveInstanceState] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RetrieveNonblockingInstanceStateIds] TO [state_persistence_users]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[UnlockInstanceState] TO [state_persistence_users]  
GO


exec sp_addrolemember N'state_persistence_users', N'MPIDOM\asharma'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\asharma]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\EA FullAccess')
CREATE USER [MPIDOM\EA FullAccess] FOR LOGIN [MPIDOM\EA FullAccess]


--Create Database Role

USE [OPUS]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'EA_Access' AND type = 'R')
CREATE ROLE [EA_Access] AUTHORIZATION [dbo]


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_TEXT] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_VALUE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[GET_ENCRYPTED] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[INSERT_PERSON_INFO_INTO_OPUS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGS_CODE_VALUE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS_CHKLIST] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryByCode] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryCodeByName] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[usp_GetPidInfo] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[Zipcodes] TO [EA_Access]  
GO


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'EA_Access', N'MPIDOM\EA FullAccess'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\EA FullAccess]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\EA ReadOnly')
CREATE USER [MPIDOM\EA ReadOnly] FOR LOGIN [MPIDOM\EA ReadOnly]


--Create Database Role

USE [OPUS]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'EA_AccessRO ' AND type = 'R')
CREATE ROLE [EA_AccessRO ] AUTHORIZATION [dbo]


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_TEXT] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_VALUE] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[GET_ENCRYPTED] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[INSERT_PERSON_INFO_INTO_OPUS] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGS_CODE_VALUE] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS_CHKLIST] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_BASE] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryByCode] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryCodeByName] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[usp_GetPidInfo] TO [EA_AccessRO ]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[Zipcodes] TO [EA_AccessRO ]  
GO


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'EA_AccessRO ', N'MPIDOM\EA ReadOnly'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\EA ReadOnly]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\hrao')
CREATE USER [mpidom\hrao] FOR LOGIN [MPIDOM\hrao] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'mpidom\hrao'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\hrao]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\IMG-BENEFITSRECOVERY')
CREATE USER [mpidom\IMG-BENEFITSRECOVERY] FOR LOGIN [MPIDOM\IMG-BENEFITSRECOVERY]




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\IMG-BENEFITSRECOVERY]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\IMG-CLAIMS-EXAMINERS')
CREATE USER [mpidom\IMG-CLAIMS-EXAMINERS] FOR LOGIN [MPIDOM\IMG-CLAIMS-EXAMINERS]




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\IMG-CLAIMS-EXAMINERS]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\IMG-PENSION')
CREATE USER [mpidom\IMG-PENSION] FOR LOGIN [MPIDOM\IMG-PENSION]




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\IMG-PENSION]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\IMG-PSC')
CREATE USER [mpidom\IMG-PSC] FOR LOGIN [MPIDOM\IMG-PSC]




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\IMG-PSC]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\it_core')
CREATE USER [MPIDOM\it_core] FOR LOGIN [MPIDOM\it_core]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_owner', N'MPIDOM\it_core'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\it_core]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\karora')
CREATE USER [MPIDOM\karora] FOR LOGIN [MPIDOM\karora] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\karora'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\karora]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\kyoung')
CREATE USER [MPIDOM\kyoung] FOR LOGIN [MPIDOM\kyoung] WITH DEFAULT_SCHEMA=[dbo]




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\kyoung]  
GO


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[INSERT_WORKFLOW_REQUEST_INFO_INTO_OPUS] TO [MPIDOM\kyoung]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RETRIEVE_INDEXING_INFO_FROM_OPUS] TO [MPIDOM\kyoung]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_DOCUMENT] TO [MPIDOM\kyoung]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[vw_GET_DEMOGRAPHIC_INFO] TO [MPIDOM\kyoung]  
GO



USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\mbanerjee')
CREATE USER [MPIDOM\mbanerjee] FOR LOGIN [MPIDOM\mbanerjee] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\mbanerjee'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\mbanerjee]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\phaines')
CREATE USER [MPIDOM\phaines] FOR LOGIN [MPIDOM\phaines] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\phaines'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\phaines]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mpidom\PIDNCOAFullAccess')
CREATE USER [mpidom\PIDNCOAFullAccess] FOR LOGIN [MPIDOM\PIDNCOAFullAccess]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'mpidom\PIDNCOAFullAccess'
GO


--Create Database Role

USE [OPUS]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'EA_Access' AND type = 'R')
CREATE ROLE [EA_Access] AUTHORIZATION [dbo]


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_TEXT] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_VALUE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[GET_ENCRYPTED] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[INSERT_PERSON_INFO_INTO_OPUS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGS_CODE_VALUE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGT_PERSON_ADDRESS] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_ADDRESS_CHKLIST] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGT_PERSON_BASE] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryByCode] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[ufn_CountryCodeByName] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[usp_GetPidInfo] TO [EA_Access]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[Zipcodes] TO [EA_Access]  
GO


exec sp_addrolemember N'EA_Access', N'mpidom\PIDNCOAFullAccess'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [mpidom\PIDNCOAFullAccess]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\ppunjabi')
CREATE USER [MPIDOM\ppunjabi] FOR LOGIN [MPIDOM\ppunjabi] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_owner', N'MPIDOM\ppunjabi'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\ppunjabi]  
GO

USE [OPUS]
GO
Grant CREATE ASSEMBLY ON Database::[OPUS] TO [MPIDOM\ppunjabi]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\rgoldring')
CREATE USER [MPIDOM\rgoldring] FOR LOGIN [MPIDOM\rgoldring] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\rgoldring'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\rgoldring]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\rmenta')
CREATE USER [MPIDOM\rmenta] FOR LOGIN [MPIDOM\rmenta] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\rmenta'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\rmenta]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\rwittmann')
CREATE USER [MPIDOM\rwittmann] FOR LOGIN [MPIDOM\rwittmann] WITH DEFAULT_SCHEMA=[MPIDOM\rwittmann]




--Scripting Object and Statement Permissions






USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\scanop')
CREATE USER [MPIDOM\scanop] FOR LOGIN [MPIDOM\scanop] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\scanop'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\scanop]  
GO


USE [OPUS]
GO
Deny EXECUTE ON [dbo].[INSERT_WORKFLOW_REQUEST_INFO_INTO_OPUS] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RETRIEVE_INDEXING_INFO_FROM_OPUS] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_DOCUMENT] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\scanop]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[vw_GET_DEMOGRAPHIC_INFO] TO [MPIDOM\scanop]  
GO



USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\sjain')
CREATE USER [MPIDOM\sjain] FOR LOGIN [MPIDOM\sjain] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_owner', N'MPIDOM\sjain'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\sjain]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\vthomas')
CREATE USER [MPIDOM\vthomas] FOR LOGIN [MPIDOM\vthomas] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_owner', N'MPIDOM\vthomas'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\vthomas]  
GO




USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\wcfGoGreen')
CREATE USER [MPIDOM\wcfGoGreen] FOR LOGIN [MPIDOM\wcfGoGreen] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\wcfGoGreen'
GO


exec sp_addrolemember N'db_datawriter', N'MPIDOM\wcfGoGreen'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\wcfGoGreen]  
GO


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_TEXT] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_VALUE] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGT_GO_GREEN_HISTORY] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_GO_GREEN_HISTORY] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGT_GO_GREEN_HISTORY] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[SP_GoGreen_Get_By_ID] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[SP_GoGreen_Get_By_PID] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[SP_GoGreen_Insert] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[SP_GoGreen_Update_By_ID] TO [MPIDOM\wcfGoGreen]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[SP_GoGreen_Update_By_PID_StartDate] TO [MPIDOM\wcfGoGreen]  
GO



USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\wcfMembership')
CREATE USER [MPIDOM\wcfMembership] FOR LOGIN [MPIDOM\wcfMembership] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\wcfMembership'
GO


exec sp_addrolemember N'db_datawriter', N'MPIDOM\wcfMembership'
GO




--Scripting Object and Statement Permissions



USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\wcfMembership]  
GO


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_TEXT] TO [MPIDOM\wcfMembership]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[fn_GET_DECRYPTED_VALUE] TO [MPIDOM\wcfMembership]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_PERSON] TO [MPIDOM\wcfMembership]  
GO



USE [OPUS]
GO



--Grant Database Access 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\wcfPRS')
CREATE USER [MPIDOM\wcfPRS] FOR LOGIN [MPIDOM\wcfPRS] WITH DEFAULT_SCHEMA=[dbo]

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\wcfPRS'
GO


exec sp_addrolemember N'db_datawriter', N'MPIDOM\wcfPRS'
GO

USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\wcfPRS]  
GO


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[UpdatePersonContact] TO [MPIDOM\wcfPRS]  
GO

USE [OPUS]
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MPIDOM\WXIMPERSONATION')
CREATE USER [MPIDOM\WXIMPERSONATION] FOR LOGIN [MPIDOM\WXIMPERSONATION] WITH DEFAULT_SCHEMA=[dbo]


--Scripting Database Role Members

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'MPIDOM\WXIMPERSONATION'
GO

USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [MPIDOM\WXIMPERSONATION]  
GO


USE [OPUS]
GO
Grant EXECUTE ON [dbo].[INSERT_WORKFLOW_REQUEST_INFO_INTO_OPUS] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant EXECUTE ON [dbo].[RETRIEVE_INDEXING_INFO_FROM_OPUS] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGT_DOCUMENT] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant INSERT ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant UPDATE ON [dbo].[SGW_WORKFLOW_REQUEST] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO
Grant SELECT ON [dbo].[vw_GET_DEMOGRAPHIC_INFO] TO [MPIDOM\WXIMPERSONATION]  
GO

USE [OPUS]
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'sysop')
CREATE USER [sysop] FOR LOGIN [sysop] WITH DEFAULT_SCHEMA=[dbo]

USE [OPUS]
GO

exec sp_addrolemember N'db_datareader', N'sysop'
GO

USE [OPUS]
GO
Grant CONNECT ON Database::[OPUS] TO [sysop]  
GO



