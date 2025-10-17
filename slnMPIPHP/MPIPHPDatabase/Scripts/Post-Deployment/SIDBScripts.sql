-- =============================================
-- Script Template
-- =============================================

USE [SIDB]
GO
/****** Object:  StoredProcedure [dbo].[OPUS_spw_ConfirmationUpdate]    Script Date: 05/01/2013 09:04:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:        Igor Kilimnik
-- Create date: 10/04/2011
-- Description:   Confirmation Update
-- =============================================
CREATE PROCEDURE [dbo].[OPUS_spw_ConfirmationUpdate]
      @ID  int,
      @StartDate datetime

AS
BEGIN

  UPDATE [OPUS].[dbo].SGT_Go_Green_History
  SET Start_Date = @StartDate,
        Confirmed_Flag = 'Y',
        Is_Processed = 0      
  WHERE GO_GREEN_HISTORY_ID = @ID
  

		UPDATE [OPUS].[dbo].SGT_PERSON SET COMMUNICATION_PREFERENCE_ID=6027 , 
		COMMUNICATION_PREFERENCE_VALUE='EMAL',
		MODIFIED_BY='OPUS_spw_ConfirmationUpdate',
		MODIFIED_DATE=GETDATE() 
		WHERE MPI_PERSON_ID= (SELECT TOP(1) MPI_PERSON_ID FROM [OPUS].[dbo].SGT_GO_GREEN_HISTORY WHERE GO_GREEN_HISTORY_ID = @ID)

END
GO
