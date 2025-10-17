using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using System.Runtime.Serialization;

namespace MPIPHP.Common
{
    [Serializable()]
    public class FileDownloadContainer : ISerializable
    {
        public const string sessionKey = "FileDownloadContainer";
        public const string defaultDownloadMimeType = "application/octet-stream";

        //Serialization constants
        protected const string _snFileContent = "FileContent";
        protected const string _snFileMimeType = "FileMimeType";
        protected const string _snFileName = "FileName";
        protected const string _snOpenInBrowser = "OpenInBrowser";
        protected const string _snErrorList = "ErrorList";



        protected byte[] _iFileContent;
        protected string _iFileName;
        protected string _iFileMimeType;
        protected bool _iblnOpenInBrowser;
        protected utlErrorList _iErrorList;


        public byte[] iFileContent { get { return _iFileContent; } }
        public string iFileName { get { return _iFileName; } }
        public string iFileMimeType { get { return _iFileMimeType; } }
        public bool iblnOpenInBrowser { get { return _iblnOpenInBrowser; } set { _iblnOpenInBrowser = value; } }
        public bool iHasErrors { get { return (_iErrorList.Count > 0); } }
        public utlErrorList iErrorList { get { return _iErrorList; } }

        public FileDownloadContainer(string astrFileName, string astrFileMimeType, byte[] aobjFileContent)
            : this(astrFileName, astrFileMimeType, aobjFileContent, false)
        {

        }
        public FileDownloadContainer(string astrFileName, string astrFileMimeType, byte[] aobjFileContent, bool ablnOpenInBrowser)
        {
            _iErrorList = new utlErrorList();
            utlError e;

            if (string.IsNullOrWhiteSpace(astrFileName))
            {
                e = new utlError();
                e.istrErrorMessage = "File name is null or empty";
                _iErrorList.Add(e);
            }

            if (aobjFileContent == null)
            {
                e = new utlError();
                e.istrErrorMessage = "File content is null";
                _iErrorList.Add(e);
            }

            if (string.IsNullOrWhiteSpace(astrFileMimeType))
            {
                _iFileMimeType = defaultDownloadMimeType;
            }

            _iblnOpenInBrowser = ablnOpenInBrowser;
            _iFileName = astrFileName;
            _iFileMimeType = astrFileMimeType;
            _iFileContent = aobjFileContent;
        }

        public FileDownloadContainer(SerializationInfo info, StreamingContext context)
        {
            this._iFileContent = (byte[])info.GetValue(_snFileContent, typeof(byte[]));
            this._iFileMimeType = info.GetString(_snFileMimeType);
            this._iFileName = info.GetString(_snFileName);
            this._iblnOpenInBrowser = info.GetBoolean(_snOpenInBrowser);
            this._iErrorList = (utlErrorList)info.GetValue(_snErrorList, typeof(utlErrorList));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(_snFileContent, this._iFileContent);
            info.AddValue(_snFileName, this._iFileName);
            info.AddValue(_snFileMimeType, this._iFileMimeType);
            info.AddValue(_snOpenInBrowser, this._iblnOpenInBrowser);
            info.AddValue(_snErrorList, this._iErrorList);
        }
    }
}
