using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardEntry
{
    public CardSO cardData;
    public int cardCount;

    public CardEntry(CardSO cardData, int cardCount)
    {
        this.cardData = cardData;
        this.cardCount = cardCount;
    }
}
