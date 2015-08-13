﻿using Microsoft.SharePoint.Client;
using Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static partial class ListItemExtensions
    {
        /// <summary>
        /// Used to get a value from a list item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string BaseGet(this ListItem item, string fieldName)
        {
            return item[fieldName] == null ? String.Empty : item[fieldName].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T BaseGet<T>(this ListItem item, string fieldName)
        {
            var value = item[fieldName];
            return (T)value;
        }

        /// <summary>
        /// Used to get a User Object from a list item
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="item"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static SiteUser BaseGetUser(this ListItem item, string field)
        {
            
            SiteUser _owner = new SiteUser();
            var _fieldUser = ((FieldUserValue)(item[field]));
            var ctx = item.Context as ClientContext;

            User _user = ctx.Web.EnsureUser(_fieldUser.LookupValue);
            ctx.Load(_user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
            ctx.ExecuteQuery();
            _owner.Name = _user.Email;
            return _owner;
        }

        /// <summary>
        /// Used to get a value from a list item and convert to Int
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int BaseGetInt(this ListItem item, string fieldName)
        {
            return Convert.ToInt32(item[fieldName]);
        }

        /// <summary>
        /// Used to get a value from a list item and convert to UInt
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static uint BaseGetUint(this ListItem item, string fieldName)
        {
            object _temp = item[fieldName];
            uint _result = new uint();
            if (_temp != null)
            {
                uint.TryParse(item[fieldName].ToString(), out _result);
                return _result;
            }
            return _result;

        }

        /// <summary>
        /// Method for working with User Fields
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static List<SiteUser> BaseGetUsers(this ListItem item, string fieldName)
        {
            List<SiteUser> _users = new List<SiteUser>();
         
            if (item[fieldName] != null)
            {
                var ctx = item.Context as ClientContext;
                foreach (FieldUserValue _userValue in item[fieldName] as FieldUserValue[])
                {
                    User _user = ctx.Web.EnsureUser(_userValue.LookupValue);
                    ctx.Load(_user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                    ctx.ExecuteQuery();

                    var _spUser = new SiteUser()
                    {
                        //Email = _user.Email,
                        //LoginName = _user.LoginName,
                        Name = _user.Email
                    };
                    _users.Add(_spUser);
                }
            }
            return _users;
        }
    }
}
