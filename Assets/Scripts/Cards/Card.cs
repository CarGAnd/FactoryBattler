using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] BuildingSelector buildingSelector;
    private GameObject cardObject;
    private string cardName;
    private ModuleSO associatedModule;

    private bool isFaceUp = false;
    public bool IsFaceUp
    {
        get { return isFaceUp; }
        set { isFaceUp = value; }
    }
    //public List<Attribute> attributes;

    public void Initialize(CardSO cardData = null)
    {
        this.cardObject = gameObject; // Use gameObject, the property of MonoBehaviour 
        if(cardData != null)
        {
            this.cardName = cardData.cardName;   
            gameObject.name = cardName; // Use gameObject instead of cardObject
            associatedModule = cardData.associatedModule;
        }
        else
        {
            gameObject.name = "Blank Card";
        }
    }
    public void SelectCard()
    {
        buildingSelector.SetObjectSelection(associatedModule);
    }
    public GameObject GetCardObject()
    {
        return cardObject;
    }

    public override string ToString()
    {
        return cardName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isFaceUp)
        {
            SelectCard();
        }
    }
}
