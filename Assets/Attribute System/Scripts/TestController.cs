using AttributeSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestController : MonoBehaviour 
{
    [SerializeField]
    AttributePool attributePool = new AttributePool();

    [SerializeField]
    private TestTower testTower;

    
    private void Start() {
        attributePool.AddMember(testTower.PoolMember);
    } 

    [Button]
    private void Request(string attributeName) {
        testTower.PoolMember.GetCalculatedAttributeValue(attributeName);
    }
}