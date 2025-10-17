#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.BusinessObjects;
using System.Data;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
    [Serializable]
	public class cdoCorTemplates : doCorTemplates
	{
		public cdoCorTemplates() : base()
		{
		}

		public string formatted_associated_forms
		{
			get
			{
				if (associated_forms == null)
					return null;
				else
					return associated_forms.Replace(";", Environment.NewLine);
			}

			set
			{
				if (value == null)
					associated_forms = null;
				else
					associated_forms = value.Replace(Environment.NewLine, ";") + ";";
			}
		}

		public void LoadByTemplateName(string astrTemplateName)
		{
			DataTable ldtbList = busBase.Select<cdoCorTemplates>( new string[1] { "template_name" },
				new object[1] { astrTemplateName }, null, null);
			//If no records found create PersonAccount
			if (ldtbList.Rows.Count == 1)
			{
				LoadData(ldtbList.Rows[0]);
			}
			else if (ldtbList.Rows.Count > 1)
			{
				throw new Exception("LoadByTemplateName method : Multiple templates returned for given template name : " +
					astrTemplateName);
			}
		}

        public string istrBatchFlag
        {
            get
            {
                if (this.batch_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }

        public string istrOnlineFlag 
        {
            get
            {
                if (this.online_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }

        public string istrAutoPrinFlag 
        {
            get
            {
                if (this.auto_print_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }

        public string istrActiveFlag
        {
            get
            {
                if (this.active_flag == busConstant.Flag_Yes)
                    return busConstant.YES;
                else
                    return busConstant.NO;
            }
        }
        public string istrDcfnacode;
    } 
} 
