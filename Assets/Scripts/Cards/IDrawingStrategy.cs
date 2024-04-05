using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDrawingStrategy
{
    List<Card> drawCards(List<Card> deck, int amount);
    void Shuffle(List<Card> deck)
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }
}

public class RandomDrawingStrategy : IDrawingStrategy
{
    public List<Card> drawCards(List<Card> deck, int amount)
    {
        amount = Mathf.Min(amount, deck.Count);
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            drawnCards.Add(deck[randomIndex]);
        }
        return drawnCards;
    }
}
public class TopDeckStrategy : IDrawingStrategy
{
    public List<Card> drawCards(List<Card> deck, int amount)
    {
        amount = Mathf.Min(amount, deck.Count);
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            drawnCards.Add(deck[i]);
        }
        return drawnCards;
    }
}

public class BottomDeckStrategy : IDrawingStrategy
{
    public List<Card> drawCards(List<Card> deck, int amount)
    {
        amount = Mathf.Min(amount, deck.Count);
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            drawnCards.Add(deck[^1]);
        }
        return drawnCards;
    }
}

public class MiddleDrawStrategy : IDrawingStrategy
{
    public List<Card> drawCards(List<Card> deck, int amount)
    {
        amount = Mathf.Min(amount, deck.Count);
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            drawnCards.Add(deck[deck.Count / 2]);
        }
        return drawnCards;
    }
}