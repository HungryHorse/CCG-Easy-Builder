using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardType
{
    Creature, QuickSpell, SlowSpell, Static
}

public enum Phase
{
    StartOfTurn, Draw, MainOne, Combat, MainTwo, GenericMain, EndOfTurn
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }
    public List<Card> CurrentHand { get => _currentHand; set => _currentHand = value; }
    public float CardSize { get => _cardSize; set => _cardSize = value; }
    public float LerpSpeed { get => _lerpSpeed; set => _lerpSpeed = value; }
    public Vector2 StackPos { get => _stackPos; set => _stackPos = value; }
    public List<Card> Stack { get => _stack; set => _stack = value; }
    public bool StackEnabled { get => _stackEnabled; set => _stackEnabled = value; }
    public Phase CurrPhase { get => _currPhase; set => _currPhase = value; }
    public GameObject PlayerObject { get => _playerObject; set => _playerObject = value; }
    public LineRenderer TargetLine { get => _targetLine; set => _targetLine = value; }

    public GameObject creaturePrefab;

    [SerializeField]
    private bool _stackEnabled = true;
    [SerializeField]
    private List<Card> _stack = new List<Card>();
    [SerializeField]
    private List<Card> _currentHand;
    [SerializeField]
    private List<Card> _opponentHand;
    [SerializeField]
    private float _stackPositionX;
    [SerializeField]
    private float _stackPositionY;
    private Vector2 _stackPos;
    [SerializeField]
    private float _cardSize;
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private bool _seperateCombatPhase;

    [SerializeField]
    private GameObject _playerObject;
    [SerializeField]
    private LineRenderer _targetLine;

    private Phase[] _phaseList;
    private Phase _currPhase;
    private int _currPhaseIndex = 0;

    private GameObject _cardObjectInStack;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(_stackPositionX, _stackPositionY), new Vector3(_cardSize * 3, _cardSize * 4));
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = this;
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _stackPos = new Vector2(_stackPositionX, _stackPositionY);
        if (_seperateCombatPhase)
        {
            _phaseList = new Phase[] { Phase.StartOfTurn, Phase.Draw, Phase.MainOne, Phase.Combat, Phase.MainTwo, Phase.EndOfTurn };
        }
        else
        {
            _phaseList = new Phase[] { Phase.StartOfTurn, Phase.Draw, Phase.GenericMain, Phase.EndOfTurn };
        }

        _currPhase = _phaseList[_currPhaseIndex];
        StartPhase();
    }

    public void ProgressPhases()
    {
        if(_currPhaseIndex + 1 == _phaseList.Length)
        {
            _currPhaseIndex = 0;
        }
        else
        {
            _currPhaseIndex++;
        }
        _currPhase = _phaseList[_currPhaseIndex];
        StartPhase();
    }

    private void StartPhase()
    {
        switch (_currPhase)
        {
            case Phase.Draw:
                Deck.Instance.AddCardToHand();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCard(GameObject cardObject)
    {
        if (_stackEnabled)
        {
            StartCoroutine(MoveTowardsStackPosition(cardObject));
        }
        else
        {
            Resolve(cardObject.GetComponent<PrefabEvents>().ThisCard);
        }
    }

    private IEnumerator MoveTowardsStackPosition(GameObject cardObject)
    {
        Card cardInfo = cardObject.GetComponent<PrefabEvents>().ThisCard;
        cardObject.GetComponent<PrefabEvents>().enabled = false;
        for (; ; )
        {
            cardObject.transform.position = Vector3.Lerp(cardObject.transform.position, new Vector3(_stackPositionX, _stackPositionY, 0), _lerpSpeed);
            cardObject.transform.localScale = Vector3.Lerp(cardObject.transform.localScale, new Vector3(cardObject.GetComponent<PrefabEvents>().HoverScale, cardObject.GetComponent<PrefabEvents>().HoverScale, 1), _lerpSpeed);
            if (Vector3.Distance(cardObject.transform.position, new Vector3(_stackPositionX, _stackPositionY)) < 0.002f)
            {
                break;
            }
            yield return new WaitForSeconds(0.02f);
        }
        AddCardToStack(cardInfo);
    }

    public void AddCardToStack(Card card)
    {
        _stack.Add(card);

        bool resolve = canReslove();

        if (resolve)
        {
            ResolveTopCard();
        }
    }

    public bool canReslove()
    {
        bool canResolve = true;

        foreach (Card checkCard in _opponentHand)
        {
            if (checkCard.CardType == CardType.QuickSpell)
            {
                canResolve = false;
            }
        }

        return canResolve;
    }

    public void ResolveTopCard()
    {
        Card card = _stack[_stack.Count - 1];
        _stack.Remove(card);
        Destroy(card.CardGameObject.GetComponent<PrefabEvents>().ViewObject);
        Destroy(card.CardGameObject);

        CardType cardType = card.CardType;

        switch (cardType)
        {
            case CardType.Creature:
                Board.Instance.CreateCreature(card);
                break;
            case CardType.SlowSpell:
                break;
            case CardType.QuickSpell:
                break;
            case CardType.Static:
                break;
            default:
                break;
        }
    }

    public void Resolve(Card card)
    {
        Vector2 cardPosition = card.CardGameObject.transform.position;
        Destroy(card.CardGameObject.GetComponent<PrefabEvents>().ViewObject);
        Destroy(card.CardGameObject);

        CardType cardType = card.CardType;

        switch (cardType)
        {
            case CardType.Creature:
                Board.Instance.CreateCreatureFormatted(card);
                break;
            case CardType.SlowSpell:
                break;
            case CardType.QuickSpell:
                break;
            case CardType.Static:
                break;
            default:
                break;
        }
    }
}
