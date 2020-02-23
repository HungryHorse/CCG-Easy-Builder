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
    public List<Card> PlayerBoard { get => _playerBoard; set => _playerBoard = value; }
    public List<Card> OpponentBoard { get => _opponentBoard; set => _opponentBoard = value; }
    public int PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
    public int OpponentHealth { get => _opponentHealth; set => _opponentHealth = value; }
    public Vector2 HoverPos { get => _hoverPos; set => _hoverPos = value; }

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
    private List<Card> _playerBoard = new List<Card>();
    [SerializeField]
    private List<Card> _opponentBoard = new List<Card>();
    [SerializeField]
    private int _playerHealth = 20;
    [SerializeField]
    private int _opponentHealth = 20;
    [SerializeField]
    private float _stackPositionX;
    [SerializeField]
    private float _stackPositionY;
    [SerializeField]
    private float _hoverPositionX;
    [SerializeField]
    private float _hoverPositionY;
    private Vector2 _stackPos;
    private Vector2 _hoverPos;
    [SerializeField]
    private float _cardSize;
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private bool _seperateCombatPhase;
    private bool _targeting = false;
    private Card _waitingForTarget;

    [SerializeField]
    private GameObject _playerObject;
    [SerializeField]
    private LineRenderer _targetLine;
    private Vector3[] _linePoints;

    private Phase[] _phaseList;
    private Phase _currPhase;
    private int _currPhaseIndex = 0;

    private GameObject _cardObjectInStack;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(_stackPositionX, _stackPositionY), new Vector3(_cardSize * 3, _cardSize * 4));
        Gizmos.DrawWireCube(new Vector3(_hoverPositionX, _hoverPositionY), new Vector3(_cardSize * 3, _cardSize * 4));
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
        _hoverPos = new Vector2(_hoverPositionX, _hoverPositionY);
        if (_seperateCombatPhase)
        {
            _phaseList = new Phase[] { Phase.StartOfTurn, Phase.Draw, Phase.MainOne, Phase.Combat, Phase.MainTwo, Phase.EndOfTurn };
        }
        else
        {
            _phaseList = new Phase[] { Phase.StartOfTurn, Phase.Draw, Phase.GenericMain, Phase.EndOfTurn };
        }

        //This is being used for testing
        ForDebug();

        _currPhase = _phaseList[_currPhaseIndex];
        StartPhase();
    }

    public void ForDebug()
    {
        GameObject TestEnemy = Instantiate(creaturePrefab, new Vector3(0, Board.Instance.OpponentBoardMiddle), Quaternion.identity, Board.Instance.transform);

        Card TestEnemyCard = ScriptableObject.CreateInstance<Card>();

        PrefabEvents prefabDetails = TestEnemy.GetComponent<PrefabEvents>();

        TestEnemyCard.CardName = "TestEnemy";

        prefabDetails.ThisCard = TestEnemyCard;

        TestEnemyCard.CardGameObject = TestEnemy;

        _opponentBoard.Add(TestEnemyCard);
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
        if (_targeting)
        {
            _linePoints = new Vector3[2] { _stackPos, Camera.main.ScreenToWorldPoint(Input.mousePosition) };
            _targetLine.SetPositions(_linePoints);
        }

        if (_targeting && Input.GetMouseButtonDown(0))
        {
            _targeting = false;
            Card target = ReturnTargetFromBoard(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(target != null)
            {
                _waitingForTarget.Targets.Add(target);
                AddCardToStack(_waitingForTarget);
                _waitingForTarget = null;
            }
        }
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
        if (cardInfo.CanTarget)
        {
            _targeting = true;
            _waitingForTarget = cardInfo;
        }
        else
        {
            AddCardToStack(cardInfo);
        }
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
                _linePoints = new Vector3[2] { Vector3.zero, Vector3.zero };
                _targetLine.SetPositions(_linePoints);
                Debug.Log(card.Targets[0].CardName);
                break;
            case CardType.QuickSpell:
                _linePoints = new Vector3[2] { Vector3.zero, Vector3.zero };
                _targetLine.SetPositions(_linePoints);
                Debug.Log(card.Targets[0].CardName);
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
                Debug.Log(card.Targets[0].CardName);
                break;
            case CardType.QuickSpell:
                Debug.Log(card.Targets[0].CardName);
                break;
            case CardType.Static:
                break;
            default:
                break;
        }
    }

    public Card ReturnTargetFromBoard(Vector3 targetPosition)
    {
        Vector3 zNormalizedTarget = new Vector3(targetPosition.x, targetPosition.y, 0);
        foreach (Card checkCard in _playerBoard)
        {
            if(Vector3.Distance(zNormalizedTarget, checkCard.CardGameObject.transform.position) < _cardSize)
            {
                return checkCard;
            }
        }
        foreach (Card checkCard in _opponentBoard)
        {
            if (Vector3.Distance(zNormalizedTarget, checkCard.CardGameObject.transform.position) < _cardSize)
            {
                return checkCard;
            }
        }

        return null;
    }
}
