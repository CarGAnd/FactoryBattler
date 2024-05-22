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
        Card card = cardObject.GetComponent<Card>();
        if(card == null)
        {
            card = cardObject.AddComponent<Card>();
        }
        card.Initialize(cardData); // Initialize the card with the data.
        if(cardData.cardName.Equals("CardRed"))
        {
            cardObject.GetComponent<Image>().color = Color.red;
        }
        //Additional stuff
        return card;
    }

    public Card CreateBlankCard()
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        Card card = cardObject.GetComponent<Card>(); // Add the Card component.
        if(card == null)
        {
            card = cardObject.AddComponent<Card>();
        }
        card.Initialize();
        return card;
    }
}
