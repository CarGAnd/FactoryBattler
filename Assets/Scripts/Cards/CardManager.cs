    using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

    public class CardManager : MonoBehaviour
    {
        //The full collection will be necessary to know every card in the game and can be used for randomly generating a card.
        [SerializeField] private CardCollectionSO fullCollection;
        [SerializeField] private CardCollectionSO usedCollection;
        [SerializeField] private GameObject cardTemplate;

        private IDrawingStrategy currentDrawingStrategy;
        private DrawingStrategyService drawingStrategyService = new DrawingStrategyService();
        private CardFactory cardFactory;

        private List<Card> player1Cards = new List<Card>();
        private List<Card> player1Hand = new List<Card>();
        private List<Card> player1Graveyard = new List<Card>();

        private GameObject player1DeckObject;
        private GameObject player1HandObject;
        private GameObject player1GraveyardObject;
        [SerializeField] private int numToDraw = 1;
        [SerializeField] private int handMaxSize = 10;
        [SerializeField] private bool overdrawDiscards = true;
        [SerializeField] private DrawingStrategy drawingStrategy = DrawingStrategy.Random;


    private void Awake()
    {
        Setup();
    }



    private void Start()
    {
        LoadDecks();
    }
    private void Setup()
    {
        cardFactory = new CardFactory(cardTemplate);
        currentDrawingStrategy = drawingStrategyService.GetStrategy<RandomDrawingStrategy>();
    }
    private void LoadDecks()
    {
        foreach (CardEntry cardEntry in usedCollection.cards)
        {
            for (int i = 0; i < cardEntry.cardCount; i++)
            {
                Card card = cardFactory.CreateCard(cardEntry.cardData);
                player1Cards.Add(card);
            }
        }
        
        if(player1Cards.Count > 0)
        {
            Debug.Log("Deck loaded successfully.");
        }
        player1DeckObject = new GameObject("Player1Deck");
        player1HandObject = new GameObject("Player1Hand");
        player1GraveyardObject = new GameObject("Player1Graveyard");
        player1DeckObject.transform.SetParent(transform);
        player1HandObject.transform.SetParent(transform);
        player1GraveyardObject.transform.SetParent(transform);
        foreach (Card card in player1Cards)
        {
            card.GetCardObject().transform.SetParent(player1DeckObject.transform);
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
        int spaceInHand = handMaxSize - player1HandSize;

        List<Card> rawDrawnCards = currentDrawingStrategy.drawCards(player1Cards, amount);
        // Only proceed if there's space in the hand
        if (spaceInHand > 0)
        {
            List<Card> drawnCards = currentDrawingStrategy.drawCards(player1Cards, amount);

            foreach (Card card in drawnCards)
            {
                if (player1Hand.Count < handMaxSize)
                {
                    // Add card to the hand if there's space
                    player1Hand.Add(card);
                    player1Cards.Remove(card);
                    card.GetCardObject().transform.SetParent(player1HandObject.transform);
                }
                else if (overdrawDiscards)
                {
                    Overdraw(card);
                }
            }
        }
        else if (overdrawDiscards)
        {
            foreach (Card card in rawDrawnCards)
                {
                    Overdraw(card);
                }
            }
        else
        {
            Debug.Log("Hand is already full. Cannot draw more cards.");
        }
    }

    private void Overdraw(Card card)
    {
        // If overdraw is set to discard, move the card to the graveyard
        Debug.Log("Hand is full. Discarding card to graveyard.");
        player1Graveyard.Add(card);
        player1Cards.Remove(card);
        card.GetCardObject().transform.SetParent(player1GraveyardObject.transform);
    }

    [Button("LoadDecks")]
    public void LoadDeck()
    {
        if(player1Cards.Count == 0)
        {
            Setup();
            LoadDecks();
        }
        else
        {
            CleanUp();
            LoadDeck();
        }
        
    }
    [Button("Shuffle")]
    public void Shuffle()
    {
        if(player1Cards.Count > 0)
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
        if(player1Cards.Count > 0)
        DrawCards(numberToDraw);
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

        DestroyImmediate(player1DeckObject);
        DestroyImmediate(player1HandObject);
        DestroyImmediate(player1GraveyardObject);
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
    