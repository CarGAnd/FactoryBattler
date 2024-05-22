using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Cards/Create New Card", order = 1)]
public class CardSO : ScriptableObject
{
    public string cardName;
    public ModuleSO associatedModule;
    //public List<Attribute> attributes
}
