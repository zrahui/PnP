﻿using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Extensions;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class TermGroup : IEquatable<TermGroup>
    {
        #region Private Members
        private List<TermSet> _termSets = new List<TermSet>();
        private Guid _id;
        #endregion

        #region Public Members
        public Guid Id { get { return _id; } set { _id = value; } }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<TermSet> TermSets
        {
            get { return _termSets; }
            private set { _termSets = value; }
        }

        #endregion

        #region Constructors

        public TermGroup()
        {

        }

        public TermGroup(Guid id, string name, List<TermSet> termSets)
        {
            this.Id = id;
            this.Name = name;
            if (termSets != null)
            {
                this.TermSets.AddRange(termSets);
            }
        }
        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}",
                this.Id,
                this.Name,
                this.Description,
                 this.TermSets.Aggregate(0, (acc, next) => acc += next.GetHashCode())
                ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TermGroup))
            {
                return (false);
            }
            return (Equals((TermGroup)obj));
        }

        public bool Equals(TermGroup other)
        {
            return (this.Id == other.Id &&
                this.Name == other.Name &&
                this.Description == other.Description &&
                this.TermSets.DeepEquals(other.TermSets));
        }

        #endregion
    }
}
