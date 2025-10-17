
---------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/09/2011
-- Purpose - Added AWARDED_ON_DATE in to SGT_BENEFIT_APPLICATION
---------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_BENEFIT_APPLICATION
ADD AWARDED_ON_DATE UDT_DATETIME


--------------------------------------------------------------------------------------------------------------------------------------
-- Name - Rashmi Sheri
-- Date - 12/09/2011
-- Purpose - Removed AWARDED_ON_DATE from SGT_BENEFIT_APPLICATION_DETAIL
--------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE SGT_BENEFIT_APPLICATION_DETAIL
DROP COLUMN AWARDED_ON_DATE

----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 12/09/2011
-- Purpose - Create table SGT_PERSON_BRIDGE_HOURS.
----------------------------------------------------------------------------------------------------------------------------------

/****** Object:  Table [dbo].[SGT_PERSON_BRIDGE_HOURS]    Script Date: 12/09/2011 18:07:00 ******/

CREATE TYPE [dbo].[UDT_SERVICE_TOTAL] FROM [numeric](14, 4) NULL
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGT_PERSON_BRIDGE_HOURS](
	[PERSON_BRIDGE_ID] [dbo].[UDT_IDENTITY] IDENTITY(1,1) NOT NULL,
	[PERSON_ID] [dbo].[UDT_ID] NOT NULL,
	[BRIDGE_TYPE_ID] [dbo].[UDT_ID] NOT NULL,
	[BRIDGE_TYPE_VALUE] [dbo].[UDT_CODE_VALUE] NULL,
	[HOURS_REPORTED] [dbo].[UDT_SERVICE_TOTAL] NULL,
	[BRIDGE_START_DATE] [dbo].[UDT_DATETIME] NULL,
	[BRIDGE_END_DATE] [dbo].[UDT_DATETIME] NULL,
	[CREATED_BY] [dbo].[UDT_CREATEDBY] NOT NULL,
	[CREATED_DATE] [dbo].[UDT_DATETIME] NOT NULL,
	[MODIFIED_BY] [dbo].[UDT_MODIFIEDBY] NOT NULL,
	[MODIFIED_DATE] [dbo].[UDT_DATETIME] NOT NULL,
	[UPDATE_SEQ] [dbo].[UDT_UPDSEQ] NOT NULL,
 CONSTRAINT [PK_PERSON_BRIDGE_HOURS] PRIMARY KEY CLUSTERED 
(
	[PERSON_BRIDGE_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



----------------------------------------------------------------------------------------------------------------------------------
-- Name - Rohan Adgaonkar
-- Date - 12/09/2011
-- Purpose - Create table SGT_PERSON_BRIDGE_HOURS_DETAIL.
----------------------------------------------------------------------------------------------------------------------------------


/****** Object:  Table [dbo].[SGT_PERSON_BRIDGE_HOURS_DETAIL]    Script Date: 12/09/2011 18:12:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGT_PERSON_BRIDGE_HOURS_DETAIL](
	[PERSON_BRIDGE_HOURS_DETAIL_ID] [dbo].[UDT_IDENTITY] IDENTITY(1,1) NOT NULL,
	[PERSON_BRIDGE_ID] [dbo].[UDT_ID] NOT NULL,
	[COMPUTATION_YEAR] [dbo].[UDT_INT] NULL,
	[HOURS] [dbo].[UDT_SERVICE_TOTAL] NULL,
	[CREATED_BY] [dbo].[UDT_CREATEDBY] NOT NULL,
	[CREATED_DATE] [dbo].[UDT_DATETIME] NOT NULL,
	[MODIFIED_BY] [dbo].[UDT_MODIFIEDBY] NOT NULL,
	[MODIFIED_DATE] [dbo].[UDT_DATETIME] NOT NULL,
	[UPDATE_SEQ] [dbo].[UDT_UPDSEQ] NOT NULL,
 CONSTRAINT [PK_SGT_PERSON_BRIDGE_HOURS_DETAIL] PRIMARY KEY CLUSTERED 
(
	[PERSON_BRIDGE_HOURS_DETAIL_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


