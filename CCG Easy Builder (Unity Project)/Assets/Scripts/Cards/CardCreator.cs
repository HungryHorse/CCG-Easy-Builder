using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardCreator : MonoBehaviour
{
    public GameObject CardObject;


    [SerializeField, Header("Card Info")]
    private string _cardName;
    [SerializeField]
    private string _description;
    [SerializeField]
    private int _cost;
    private int _health;
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int attack;
    public CardType cardType;
    private bool _canTarget;
    private List<Card> _targets;
    private bool _playerCard;
    private List<Effect> _effects = new List<Effect>();
    [SerializeField, Header("Card Visuals")]
    private Sprite _cardImage;

    [SerializeField]
    private CardFrames _frames;

    public void EditorUpdate()
    {
        Transform cardFront = CardObject.transform.GetChild(0).transform.GetChild(0);

        Image frameRenderer = cardFront.Find("Frame").GetComponent<Image>();

        switch (cardType)
        {
            case CardType.Creature:
                frameRenderer.sprite = _frames.creatureFrame;

                cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = attack.ToString();
                cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = maxHealth.ToString();
                break;                                                             
            case CardType.SlowSpell:                                               
                frameRenderer.sprite = _frames.spellFrame;                         
                                                                                   
                cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = "";
                cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = "";
                break;                                                             
            case CardType.QuickSpell:                                              
                frameRenderer.sprite = _frames.spellFrame;                         
                                                                                   
                cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = "";
                cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = "";
                break;                                                             
            case CardType.Static:                                                  
                frameRenderer.sprite = _frames.staticFrame;                        
                                                                                   
                cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = "";
                cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = "";
                break;
        }

        cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = _cardImage;

        cardFront.Find("CardName").GetComponent<TextMeshProUGUI>().text = _cardName;
        cardFront.Find("CardDescription").GetComponent<TextMeshProUGUI>().text = _description;
        cardFront.Find("CardCost").GetComponent<TextMeshProUGUI>().text = _cost.ToString();
    }
}
