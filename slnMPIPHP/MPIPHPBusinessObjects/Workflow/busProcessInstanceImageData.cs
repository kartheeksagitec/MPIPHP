#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busProcessInstanceImageData:
	/// Inherited from busProcessInstanceImageDataGen, the class is used to customize the business object busProcessInstanceImageDataGen.
	/// </summary>
	[Serializable]
	public class busProcessInstanceImageData : busProcessInstanceImageDataGen
	{
        // this property will be used as the link between screens
        public string istrViewLinks
        {
            get
            {
                return "View";
            }
        }

        private string _object_store;
        public string object_store
        {
            get
            {
                return _object_store;
            }

            set
            {
                _object_store = value;
            }
        }

        private string _version_series_id;
        public string version_series_id
        {
            get
            {
                return _version_series_id;
            }

            set
            {
                _version_series_id = value;
            }
        }

        private string _document_id;
        public string document_id
        {
            get
            {
                return _document_id;
            }

            set
            {
                _document_id = value;
            }
        }

        private string _document_title;
        public string document_title
        {
            get
            {
                return _document_title;
            }

            set
            {
                _document_title = value;
            }
        }

        private DateTime _initiated_date;
        public DateTime initiated_date
        {
            get
            {
                return _initiated_date;
            }
            set
            {
                _initiated_date = value;
            }
        }

        private string _short_name;
        public string short_name
        {
            get
            {
                return _short_name;
            }

            set
            {
                _short_name = value;
            }
        }

        private string _subject_title;
        public string subject_title
        {
            get
            {
                return _subject_title;
            }

            set
            {
                _subject_title = value;
            }
        }

        private string _indicator_flag;
        public string indicator_flag
        {
            get
            {
                return _indicator_flag;
            }

            set
            {
                _indicator_flag = value;
            }
        }
	}
}
