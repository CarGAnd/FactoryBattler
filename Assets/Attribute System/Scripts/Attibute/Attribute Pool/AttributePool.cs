using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AttributeSystem
{
    [Serializable]
    public class AttributePool
    {
        private List<IAttributeSource> attributeSources = new List<IAttributeSource>();
        private List<IPoolMember> members = new List<IPoolMember>();

        [Button]
        public void AddSource(IAttributeSource source)
        {
            if (!attributeSources.Contains(source))
            {
                attributeSources.Add(source);
                NotifyAllMembersOfNewSource(source);
            }
        }

        [Button]
        public void RemoveSource(IAttributeSource source)
        {
            if (attributeSources.Remove(source))
            {
                NotifyAllMembersOfRemovedSource(source);
            }
        }

        private void NotifyAllMembersOfNewSource(IAttributeSource source)
        {
            foreach (var member in members)
            {
                member.AddPoolAttributes(source.GetAttributes());
            }
        }

        private void NotifyAllMembersOfRemovedSource(IAttributeSource source)
        {
            foreach (var member in members)
            {
                member.RemovePoolAttributes(source.GetAttributes());
            }
        }

        [Button]
        public void AddMember(IPoolMember member)
        {
            if (!members.Contains(member))
            {
                members.Add(member);
                member.AddPoolAttributes(GetAllAttributes());
            }
        }

        [Button]
        public void RemoveMember(IPoolMember member)
        {
            if (members.Contains(member))
            {
                members.Remove(member);
                member.RemovePoolAttributes(GetAllAttributes());
            }
        }

        private HashSet<AttributeInstance> GetAllAttributes()
        {
            return attributeSources?.SelectMany(source => source.GetAttributes()).ToHashSet() ?? new HashSet<AttributeInstance>();
        }
    }
}