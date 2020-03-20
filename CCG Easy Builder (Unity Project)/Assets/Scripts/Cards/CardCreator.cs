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
    [SerializeField]
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

        string compoundDescription = "";

        if(_effects.Count > 0)
        {
            foreach(Effect effect in _effects)
            {
                if (effect != null)
                {
                    compoundDescription += CreateDescriptionText(effect) + "\n";
                }
            }
            if (_description != "")
            {
                compoundDescription += "___________________\n" + _description;
            }
        }
        else
        {
            compoundDescription = _description;
        }

        cardFront.Find("CardDescription").GetComponent<TextMeshProUGUI>().text = compoundDescription;
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
        if (filePath.Length != 0 && customFilePath)
        {
            AssetDatabase.CreateAsset(instanceOfCard, filePath);
        }
        else
        {
            AssetDatabase.CreateAsset(instanceOfCard, "Assets/Prefabs/Cards/" + _cardName.Replace(" ", "") + ".asset");
        }

        Debug.Log("Card saved at location: " + "Assets/Prefabs/Effects/" + _cardName.Replace(" ", "") + ".asset");

        AssetDatabase.SaveAssets();
    }

    public string CreateDescriptionText(Effect effect)
    {
        string output = "";

        switch (effect.Trigger)
        {
            case Triggers.Drawn:
                output = "When this card is drawn ";
                break;
            case Triggers.Played:
                output = "When this card is played ";
                break;
            case Triggers.EntersBoard:
                output = "When this card enters the board ";
                break;


            case Triggers.GenericCardPlayed:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When a card is played ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When ";
                            for(int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " is played ";
                            }
                            break;

                        case ResponseTypes.CardName:
                            output = "When a card named ";
                            for(int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if(i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " is played ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " is played ";
                            break;
                    }
                }
                break;

            case Triggers.GenericCreatureEntersBoard:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When a creature enters the board ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " enters the board ";
                            }
                            break;
                        case ResponseTypes.CardName:
                            output = "When a card named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " enters the board ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " enters the board ";
                            break;
                    }
                }
                break;


            case Triggers.OpponentCardPlayed:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When an opponent plays a card ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When an opponent plays ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " ";
                            }
                            break;

                        case ResponseTypes.CardName:
                            output = "When an opponent plays a card named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When an opponent plays a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " ";
                            break;
                    }
                }
                break;

            case Triggers.OpponentCreatureEntersBoard:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When an opponent's creature enters the board ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When an opponent's creature ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " enters their board ";
                            }
                            break;

                        case ResponseTypes.CardName:
                            output = "When an opponent's creature named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " enters their board ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When an opponent's creature of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " enters the board ";
                            break;
                    }
                }
                break;

            case Triggers.OpponentDrawsCard:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When an opponent draws a card ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When an opponent draws ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " ";
                            }
                            break;

                        case ResponseTypes.CardName:
                            output = "When an opponent draws a card named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When an opponent draws a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " ";
                            break;
                    }
                }
                break;


            case Triggers.PlayerCardPlayed:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When you play a card ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When you play ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " ";
                            }
                            break;
                        case ResponseTypes.CardName:
                            output = "When you play a card named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When you play a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " ";
                            break;
                    }
                }
                break;

            case Triggers.PlayerCreatureEntersBoard:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When a creature enters your board ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " enters your board ";
                            }
                            break;
                        case ResponseTypes.CardName:
                            output = "When a creature named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " enters your board ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When a creature of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " enters your board ";
                            break;
                    }
                }
                break;

            case Triggers.PlayerDrawsCard:
                if (!effect.HasSpecificCardTriggers)
                {
                    output = "When you draw a card ";
                }
                else
                {
                    switch (effect.ResponseType)
                    {
                        case ResponseTypes.Cards:
                            output = "When you draw ";
                            for (int i = 0; i < effect.TriggerCards.Length; i++)
                            {
                                output += effect.TriggerCards[i].CardName;
                                if (i != effect.TriggerCards.Length - 1)
                                {
                                    output += " or ";
                                }
                                output += " ";
                            }
                            break;
                        case ResponseTypes.CardName:
                            output = "When you draw a card named ";
                            for (int i = 0; i < effect.TriggerCardNames.Length; i++)
                            {
                                output += effect.TriggerCardNames[i];
                                if (i != effect.TriggerCardNames.Length - 1)
                                {
                                    output += " or ";
                                }
                            }
                            output += " ";
                            break;
                        case ResponseTypes.CardType:
                            output = "When you draw a card of type " + effect.TriggerCardType.ToString().Replace("_", " ").ToLower() + " ";
                            break;
                    }
                }
                break;
        }

        foreach (Keyword response in effect.Responses)
        {
            if (effect.HasTarget)
            {
                output += "do " + response.EffectValue + " " + response.EffectDescription + " to " + effect.Target.ToString().Replace("_", " ").ToLower() + ".";
            }
            else
            {
                output += "do " + response.EffectValue + " " + response.EffectDescription + " to " + "this card" + ".";
            }
        }

        return output;
    }
}
