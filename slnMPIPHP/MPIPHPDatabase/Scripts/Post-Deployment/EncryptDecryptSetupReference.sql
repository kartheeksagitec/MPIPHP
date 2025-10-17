sp_configure 'show advanced options', 1; 
GO 
RECONFIGURE; 
GO 
sp_configure 'clr enabled', 1; 
GO 
RECONFIGURE; 
GO 
ALTER DATABASE OPUS SET TRUSTWORTHY ON 
GO 

--DROP ASSEMBLY EncryptionDecryptionClassLibrary 
--Provide unsafe assembly permission to the user or the group that the user is part of. Permission is when you right click on server name and go to Properties->Permissions.
--In our case group name is MPIDOM\SAGITEC 

CREATE ASSEMBLY EncryptionDecryptionClassLibrary 
AUTHORIZATION [dbo]
FROM 'C:\OPUS\Dev\Bin\EncryptionDecryptionClassLibrary.dll' 
WITH PERMISSION_SET = UNSAFE
GO

