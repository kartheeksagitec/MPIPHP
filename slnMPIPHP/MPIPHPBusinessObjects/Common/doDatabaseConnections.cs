#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion
namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doDatabaseConnections:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDatabaseConnections : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDatabaseConnections() : base()
         {
         }
         public int database_connection_id { get; set; }
         public string connection_name { get; set; }
         public string connection_type { get; set; }
         public string connection_string { get; set; }
         public string dbfactoty_provider { get; set; }
         public string password_encrypted_flag { get; set; }
    }
    [Serializable]
    public enum enmDatabaseConnections
    {
         database_connection_id ,
         connection_name ,
         connection_type ,
         connection_string ,
         dbfactoty_provider ,
         password_encrypted_flag ,
    }
}
