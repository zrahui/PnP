﻿using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class PropertyBagEntry : IEquatable<PropertyBagEntry>
    {
        #region Properties

        public string Key { get; set; }
            
        public string Value { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}",
                this.Key,
                this.Value).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PropertyBagEntry))
            {
                return(false);
            }
            return (Equals((PropertyBagEntry)obj));
        }

        public bool Equals(PropertyBagEntry other)
        {
            return (this.Key == other.Key &&
                this.Value == other.Value);
        }

        #endregion
    }
}
