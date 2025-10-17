#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoParticipantInformationDataExtraction:
	/// Inherited from doParticipantInformationDataExtraction, the class is used to customize the database object doParticipantInformationDataExtraction.
	/// </summary>
    [Serializable]
	public class cdoParticipantInformationDataExtraction : doParticipantInformationDataExtraction
	{
		public cdoParticipantInformationDataExtraction() : base()
		{
		}
    } 
} 
