using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardCollection", menuName = "Cards/Create new Card Collection", order = 1)]
public class CardCollectionSO : ScriptableObject
{
    public List<CardEntry> cards;
}
