-- =============================================
-- Script Template
-- =============================================


/****** Object:  UserDefinedDataType [dbo].[UDT_ZIP5]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ZIP5')
CREATE TYPE [UDT_ZIP5] FROM [varchar](5) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ZIP4]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ZIP4')
CREATE TYPE [UDT_ZIP4] FROM [varchar](4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_VALUE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_VALUE')
CREATE TYPE [UDT_VALUE] FROM [varchar](200) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_USERID]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_USERID')
CREATE TYPE [UDT_USERID] FROM [varchar](20) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_UPDSEQ]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_UPDSEQ')
CREATE TYPE [UDT_UPDSEQ] FROM [int] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_UNIQUE_IDENTIFIER]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_UNIQUE_IDENTIFIER')
CREATE TYPE [UDT_UNIQUE_IDENTIFIER] FROM [uniqueidentifier] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_TOT_AMT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_TOT_AMT')
CREATE TYPE [UDT_TOT_AMT] FROM [numeric](20, 2) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_TIME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_TIME')
CREATE TYPE [UDT_TIME] FROM [numeric](4, 2) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_TEXT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_TEXT')
CREATE TYPE [UDT_TEXT] FROM [varchar](256) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SSN]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SSN')
CREATE TYPE [UDT_SSN] FROM [varchar](256) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SINT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SINT')
CREATE TYPE [UDT_SINT] FROM [smallint] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SHORTNAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SHORTNAME')
CREATE TYPE [UDT_SHORTNAME] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SHORTDESC]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SHORTDESC')
CREATE TYPE [UDT_SHORTDESC] FROM [varchar](30) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SERVICE_TOTAL]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SERVICE_TOTAL')
CREATE TYPE [UDT_SERVICE_TOTAL] FROM [numeric](14, 4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_SERVICE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_SERVICE')
CREATE TYPE [UDT_SERVICE] FROM [numeric](7, 4) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ROUTINGNO]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ROUTINGNO')
CREATE TYPE [UDT_ROUTINGNO] FROM [varchar](20) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_RATE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_RATE')
CREATE TYPE [UDT_RATE] FROM [numeric](11, 6) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_PROVINCE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_PROVINCE')
CREATE TYPE [UDT_PROVINCE] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_PIN]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_PIN')
CREATE TYPE [UDT_PIN] FROM [varchar](256) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_PHONE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_PHONE')
CREATE TYPE [UDT_PHONE] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_PERC]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_PERC')
CREATE TYPE [UDT_PERC] FROM [numeric](7, 4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_PASSWD]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_PASSWD')
CREATE TYPE [UDT_PASSWD] FROM [varchar](20) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ORGNAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ORGNAME')
CREATE TYPE [UDT_ORGNAME] FROM [varchar](75) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_NOTES]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_NOTES')
CREATE TYPE [UDT_NOTES] FROM [varchar](2000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_NAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_NAME')
CREATE TYPE [UDT_NAME] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_MONTHS]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_MONTHS')
CREATE TYPE [UDT_MONTHS] FROM [numeric](5, 2) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_MODIFIEDBY]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_MODIFIEDBY')
CREATE TYPE [UDT_MODIFIEDBY] FROM [varchar](50) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_MESSAGE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_MESSAGE')
CREATE TYPE [UDT_MESSAGE] FROM [varchar](200) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_MAX_VALUE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_MAX_VALUE')
CREATE TYPE [UDT_MAX_VALUE] FROM [varchar](max) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_MAILNAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_MAILNAME')
CREATE TYPE [UDT_MAILNAME] FROM [varchar](100) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_LONGNAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_LONGNAME')
CREATE TYPE [UDT_LONGNAME] FROM [varchar](1000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_LONGDESC]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_LONGDESC')
CREATE TYPE [UDT_LONGDESC] FROM [varchar](2000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_KEY]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_KEY')
CREATE TYPE [UDT_KEY] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_INT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_INT')
CREATE TYPE [UDT_INT] FROM [int] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_INSTRUCTIONS]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_INSTRUCTIONS')
CREATE TYPE [UDT_INSTRUCTIONS] FROM [varchar](4096) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_IND]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_IND')
CREATE TYPE [UDT_IND] FROM [varchar](1) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_IMAGE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_IMAGE')
CREATE TYPE [UDT_IMAGE] FROM [image] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_IDENTITY]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_IDENTITY')
CREATE TYPE [UDT_IDENTITY] FROM [int] NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ID]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ID')
CREATE TYPE [UDT_ID] FROM [int] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_HOURS]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_HOURS')
CREATE TYPE [UDT_HOURS] FROM [numeric](14, 4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FY_YEAR]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FY_YEAR')
CREATE TYPE [UDT_FY_YEAR] FROM [numeric](4, 0) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FLAG]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FLAG')
CREATE TYPE [UDT_FLAG] FROM [varchar](1) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FILE_NAME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FILE_NAME')
CREATE TYPE [UDT_FILE_NAME] FROM [varchar](255) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FILE_MAX]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FILE_MAX')
CREATE TYPE [UDT_FILE_MAX] FROM [varchar](max) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FILE_LOCATION]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FILE_LOCATION')
CREATE TYPE [UDT_FILE_LOCATION] FROM [varchar](200) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FAX]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FAX')
CREATE TYPE [UDT_FAX] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_FACTOR]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_FACTOR')
CREATE TYPE [UDT_FACTOR] FROM [numeric](15, 13) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_EMAIL]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_EMAIL')
CREATE TYPE [UDT_EMAIL] FROM [varchar](70) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DESC]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DESC')
CREATE TYPE [UDT_DESC] FROM [varchar](100) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DBOBJ]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DBOBJ')
CREATE TYPE [UDT_DBOBJ] FROM [varchar](128) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATETIME]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATETIME')
CREATE TYPE [UDT_DATETIME] FROM [datetime] NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATE')
CREATE TYPE [UDT_DATE] FROM [datetime] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATA50]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATA50')
CREATE TYPE [UDT_DATA50] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATA40]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATA40')
CREATE TYPE [UDT_DATA40] FROM [varchar](40) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATA250]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATA250')
CREATE TYPE [UDT_DATA250] FROM [varchar](250) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATA100]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATA100')
CREATE TYPE [UDT_DATA100] FROM [varchar](100) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_DATA]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_DATA')
CREATE TYPE [UDT_DATA] FROM [varchar](20) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_CREATEDBY]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_CREATEDBY')
CREATE TYPE [UDT_CREATEDBY] FROM [varchar](50) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_COMMENTS]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_COMMENTS')
CREATE TYPE [UDT_COMMENTS] FROM [varchar](2000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_COL_VALUE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_COL_VALUE')
CREATE TYPE [UDT_COL_VALUE] FROM [varchar](1000) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_CODE_VALUE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_CODE_VALUE')
CREATE TYPE [UDT_CODE_VALUE] FROM [varchar](4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_CODE_ID]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_CODE_ID')
CREATE TYPE [UDT_CODE_ID] FROM [int] NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_CODE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_CODE')
CREATE TYPE [UDT_CODE] FROM [varchar](10) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_CITY]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_CITY')
CREATE TYPE [UDT_CITY] FROM [varchar](50) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_BIGINT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_BIGINT')
CREATE TYPE [UDT_BIGINT] FROM [bigint] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_AMT]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_AMT')
CREATE TYPE [UDT_AMT] FROM [numeric](11, 2) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_AGE_DEC]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_AGE_DEC')
CREATE TYPE [UDT_AGE_DEC] FROM [numeric](7, 4) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_AGE]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_AGE')
CREATE TYPE [UDT_AGE] FROM [numeric](3, 0) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ADDRESS]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ADDRESS')
CREATE TYPE [UDT_ADDRESS] FROM [varchar](60) NULL
GO
/****** Object:  UserDefinedDataType [dbo].[UDT_ACCTNO]    Script Date: 11/28/2012 14:59:19 ******/
IF NOT EXISTS (SELECT * FROM systypes WHERE name = N'UDT_ACCTNO')
CREATE TYPE [UDT_ACCTNO] FROM [varchar](20) NULL
GO
