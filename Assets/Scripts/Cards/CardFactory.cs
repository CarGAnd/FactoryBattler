using UnityEngine;
using UnityEngine.UI;

public class CardFactory
{
    private GameObject cardPrefab;
    public CardFactory(GameObject cardPrefab)
    {
        this.cardPrefab = cardPrefab;
    }
    public Card CreateCard(CardSO cardData)
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        Card card = new Card(cardObject, cardData);
        if(cardData.cardName.Equals("No"))
        {
            cardObject.GetComponent<Image>().color = Color.red;
        }
        //Additional stuff
        return card;
    }

    public Card CreateBlankCard()
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        Card card = new Card(cardObject);
        return card;
    }
}
