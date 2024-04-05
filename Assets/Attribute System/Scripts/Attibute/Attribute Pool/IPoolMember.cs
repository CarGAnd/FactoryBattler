using System.Collections.Generic;

namespace AttributeSystem
{
    public interface IPoolMember
    {
        List<AttributeInstance> BaseAttributes { get; }
        float GetCalculatedAttributeValue(string mainAttributeName);
        void AddPoolAttributes(HashSet<AttributeInstance> poolAttributes);
        void RemovePoolAttributes(HashSet<AttributeInstance> poolAttributes); 
    }
}