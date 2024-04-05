/// <summary>
/// This code snippet represents an interface called IAttributeSource in the AttributeSystem namespace. 
/// It defines two properties: SourceAttributes, which is a list of AttributeInstance objects, and GetAttributes, which returns a HashSet of AttributeInstance objects. 
/// The GetAttributes method is virtual and returns the SourceAttributes list converted to a HashSet. 
/// </summary>

using System.Collections.Generic;
using System.Linq;

namespace AttributeSystem
{
    public interface IAttributeSource
    {
        string Id { get; }
        abstract void CalculateNewID();
        List<AttributeInstance> SourceAttributes { get; }
        virtual HashSet<AttributeInstance> GetAttributes() { return SourceAttributes.ToHashSet(); }
        virtual void SetInstanceIDs () {SourceAttributes.ForEach(instance => instance.CalculateNewID());}
    }
}