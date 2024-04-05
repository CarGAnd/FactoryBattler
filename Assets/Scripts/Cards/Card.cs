using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private GameObject cardObject;
    private string cardName;
    //public List<Attribute> attributes;

    public Card(GameObject cardObject, CardSO cardData = null)
    {
        this.cardObject = cardObject;
        if(cardData != null)
        {
            this.cardName = cardData.cardName;   
            cardObject.name = cardName;
            //this.attributes = cardData.attributes;
        }
        else
        {
            cardObject.name = "Blank Card";
        }
    }

    public GameObject GetCardObject()
    {
        return cardObject;
    }

    public override string ToString()
    {
        return cardName;
    }
}
