using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardCreator : MonoBehaviour
{
    public GameObject CardObject;

    [HideInInspector]
    public string filePath = "";
    [HideInInspector]
    public bool customFilePath;
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
    [HideInInspector]
    public Keyword[] availableKeywords;
    [SerializeField, Header("Card Visuals")]
    private Sprite _cardImage;

    [SerializeField]
    private CardFrames _frames;

    public string CardName { get => _cardName; set => _cardName = value; }
    public List<Effect> Effects { get => _effects; set => _effects = value; }

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

    public void SaveCard()
    {
        Card instanceOfCard = ScriptableObject.CreateInstance<Card>();
        instanceOfCard.CardName = _cardName;
        instanceOfCard.Description = _description;
        instanceOfCard.Cost = _cost;
        instanceOfCard.CardImage = _cardImage;
        instanceOfCard.CardType = cardType;
        instanceOfCard.CanTarget = _canTarget;
        instanceOfCard.Attack = attack;
        instanceOfCard.MaxHealth = maxHealth;
        instanceOfCard.Health = maxHealth;
        instanceOfCard.Effects = _effects;
        if (filePath != "" && customFilePath)
        {
            AssetDatabase.CreateAsset(instanceOfCard, filePath + _cardName.Replace(" ", "") + ".asset");
        }
        else
        {
            AssetDatabase.CreateAsset(instanceOfCard, "Assets/Prefabs/Cards/" + _cardName.Replace(" ", "") + ".asset");
        }

        Debug.Log("Card saved at location: " + "Assets/Prefabs/Effects/" + _cardName.Replace(" ", "") + ".asset");

        AssetDatabase.SaveAssets();
    }
}
