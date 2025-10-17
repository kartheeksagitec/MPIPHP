#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

using Sagitec.Common;
using MPIPHP.DataObjects;
#endregion

namespace MPIPHP.CustomDataObjects
{
  [Serializable]
  public class cdoUser : doUser
	{
      public cdoUser() : base() 
      { 

      }

	 public string User_full_name
	 {
			get
			{
				return last_name + " " + first_name;
			}
	 }
      public int Loggedin_user_id
      {
          get
          {
              return iobjPassInfo.iintUserSerialID;
          }
      }

      // this property will be used for Appointment Confirmation Correspondence
      public string User_Name
      {
          get
          {
              return first_name+" " + last_name;
          }
      }

      private string _user_password_display;
      public string user_password_display
      {
          get
          {
              return _user_password_display;
          }
          set
          {
              _user_password_display = value;
          }
      }

      public override bool Select()
      {
          bool lblnReturn = base.Select();
          if (lblnReturn) GetUserPasswordDisplay();
          return lblnReturn;
      }

      public override bool SelectRow(object[] aarrKeyValues, bool ablnLockRow = false)
      {
          bool lblnReturn = base.SelectRow(aarrKeyValues);
          if (lblnReturn) GetUserPasswordDisplay();
          return lblnReturn;
      }

      //public override int Insert()
      //{
      //  //   SetUserPassword();
      //    return base.Insert();
      //}

      //public override int Update()
      //{
      //  //  SetUserPassword();
      //   // return base.Update();
      //}

      public void GetUserPasswordDisplay()
      {
         // _user_password_display = HelperFunction.SagitecDecrypt("", password);
      }

      public void SetUserPassword()
      {
         // password = HelperFunction.SagitecEncrypt("", _user_password_display);
      }

  } 
} 
