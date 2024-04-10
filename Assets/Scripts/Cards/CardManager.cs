using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private CardCollectionSO fullCollection;
    [SerializeField] private CardCollectionSO usedCollection;
    [SerializeField] private GameObject cardTemplate;
    [SerializeField] private GameObject ui;

    private IDrawingStrategy currentDrawingStrategy;
    private DrawingStrategyService drawingStrategyService = new DrawingStrategyService();
    private CardFactory cardFactory;

    private List<Card> player1Cards = new List<Card>();
    private List<Card> player1Hand = new List<Card>();
    private List<Card> player1Graveyard = new List<Card>();

    [SerializeField] private GameObject player1DeckObject;
    [SerializeField] private GameObject player1HandObject;
    [SerializeField] private GameObject player1GraveyardObject;

    [SerializeField] private int numToDraw = 1;
    [SerializeField] private int handMaxSize = 10;
    [SerializeField] private bool overdrawDiscards = true;
    [SerializeField] private DrawingStrategy drawingStrategy = DrawingStrategy.Random;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ui.SetActive(!ui.activeSelf);
        }
    }

    private void Setup()
    {
        cardFactory = new CardFactory(cardTemplate);
        SelectDrawingStrategy();
    }

    private void LoadDecks()
    {
        foreach (CardEntry cardEntry in usedCollection.cards)
        {
            for (int i = 0; i < cardEntry.cardCount; i++)
            {
                Card card = cardFactory.CreateCard(cardEntry.cardData);
                player1Cards.Add(card);
                MoveCard(card, player1DeckObject.transform);
                SetCardFacing(card, false);
            }
        }

        if (player1Cards.Count > 0)
        {
            Debug.Log("Deck loaded successfully.");
        }

        string order = "";
        foreach (Card card in player1Cards)
        {
            order += card.ToString() + ", ";
        }
        Debug.Log("order of cards is: " + order);
    }

    public void SetStrategy(IDrawingStrategy drawingStrategy)
    {
        currentDrawingStrategy = drawingStrategy;
        Debug.Log("Changed strategy to: " + drawingStrategy.GetType().ToString());
    }

    public void DrawCards(int amount)
    {
        int player1HandSize = player1Hand.Count;

        List<Card> drawnCards = currentDrawingStrategy.drawCards(player1Cards, amount);

        foreach (Card card in drawnCards)
        {
            if (player1Hand.Count < handMaxSize)
            {
                player1Hand.Add(card);
                SetCardFacing(card, true);
                MoveCard(card, player1HandObject.transform);
            }
            else if (overdrawDiscards)
            {
                Overdraw(card);
            }
            else
            {
                player1Cards.Add(card);
            }
        }
    }

    private void Overdraw(Card card)
    {
        Debug.Log("Hand is full. Discarding card to graveyard.");
        player1Graveyard.Add(card);
        MoveCard(card, player1GraveyardObject.transform);
        SetCardFacing(card, true);
    }

    private void MoveCard(Card card, Transform newParent)
    {
        GameObject cardObject = card.GetCardObject();
        cardObject.transform.SetParent(newParent);

        if(newParent == player1HandObject.transform)
        {
            AdjustHandCards();
        }
        else
        {
            cardObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }

    private void SetCardFacing(Card card, bool faceUp)
    {

        GameObject cardObject = card.GetCardObject();
        List<Image> images = new List<Image>(cardObject.GetComponentsInChildren<Image>());
        foreach (Image image in images)
        {
            if (image.gameObject == cardObject)
            {
                image.enabled = faceUp;
            }
            else
            {
                image.enabled = !faceUp;
            }
        }
    }

    private void AdjustHandCards()
    {
        int numCards = player1Hand.Count;
        float cardWidth = cardTemplate.GetComponent<RectTransform>().rect.width;

        float defaultSpacing = cardWidth + 25;
        float availableWidth = 8 * cardWidth + 7 * defaultSpacing;


        float requiredWidth = numCards * cardWidth + (numCards - 1) * defaultSpacing;

        float spacing;
        if (requiredWidth > availableWidth)
        {
            // Allow overlap by reducing spacing, since requiredWidth exceeds availableWidth
            spacing = (availableWidth - (numCards * cardWidth)) / (numCards - 1);
        }
        else
        {
            spacing = defaultSpacing; // Use default spacing if all cards can fit without overlapping
        }

        // Calculate start position based on adjusted spacing
        float totalCardsWidthWithSpacing = (numCards - 1) * spacing + numCards * cardWidth;
        float startX = -(totalCardsWidthWithSpacing / 2) + (cardWidth / 2); // Center the cards group

        for (int i = 0; i < numCards; i++)
        {
            float positionX = startX + (i * (cardWidth + spacing));
            RectTransform cardRect = player1Hand[i].GetCardObject().GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector3(positionX, 0, 0);
        }
    }
    [Button("LoadDecks")]
    public void LoadDeck()
    {
        CleanUp();
        Setup();
        LoadDecks();
    }

    [Button("Shuffle")]
    public void Shuffle()
    {
        if (player1Cards.Count > 0)
        {
            currentDrawingStrategy.Shuffle(player1Cards);
        }

        string order = "";
        foreach (Card card in player1Cards)
        {
            order += card.ToString() + ", ";
        }
        Debug.Log("New order of cards is: " + order);
    }

    [Button("Draw")]
    public void Draw()
    {
        int numberToDraw = Mathf.Min(numToDraw, player1Cards.Count);
        if (player1Cards.Count > 0)
        {
            DrawCards(numberToDraw);
        }
        else
        {
            Debug.Log("No cards left in deck.");
        }
    }

    [Button("CleanUp")]
    private void CleanUp()
    {
        foreach (Card card in player1Cards)
        {
            DestroyImmediate(card.GetCardObject());
        }
        player1Cards.Clear();

        foreach (Card card in player1Hand)
        {
            DestroyImmediate(card.GetCardObject());
        }
        player1Hand.Clear();

        foreach (Card card in player1Graveyard)
        {
            DestroyImmediate(card.GetCardObject());
        }
        player1Graveyard.Clear();
    }

    private enum DrawingStrategy
    {
        Random,
        TopDeck,
        BottomDeck,
        MiddleDeck
    }

    [Button("Select Drawing Strategy")]
    private void SelectDrawingStrategy()
    {
        switch (drawingStrategy)
        {
            case DrawingStrategy.Random:
                SetStrategy(drawingStrategyService.GetStrategy<RandomDrawingStrategy>());
                break;
            case DrawingStrategy.TopDeck:
                SetStrategy(drawingStrategyService.GetStrategy<TopDeckStrategy>());
                break;
            case DrawingStrategy.BottomDeck:
                SetStrategy(drawingStrategyService.GetStrategy<BottomDeckStrategy>());
                break;
            case DrawingStrategy.MiddleDeck:
                SetStrategy(drawingStrategyService.GetStrategy<MiddleDrawStrategy>());
                break;
        }
    }
}
    