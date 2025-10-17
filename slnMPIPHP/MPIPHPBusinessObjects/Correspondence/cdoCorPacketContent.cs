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
	/// Class MPIPHP.CustomDataObjects.cdoCorPacketContent:
	/// Inherited from doCorPacketContent, the class is used to customize the database object doCorPacketContent.
	/// </summary>
    [Serializable]
	public class cdoCorPacketContent : doCorPacketContent
	{
		public cdoCorPacketContent() : base()
		{
		}

        public int iintPacketTemplateId { get; set; }
        public string istrPacketTemplateName { get; set; }
        public string istrPacketTemplateDesc { get; set; }


        public int istrDocTemplateId { get; set; }
        public string istrDocTemplateName { get; set; }
        public string istrDocTemplateDesc { get; set; }
    } 
} 
