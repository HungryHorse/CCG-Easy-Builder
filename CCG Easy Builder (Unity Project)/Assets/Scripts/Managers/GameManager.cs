using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardType { Creature, QuickSpell, SlowSpell, Static }

public enum Phase { StartOfTurn, Draw, MainOne, Combat, MainTwo, GenericMain, EndOfTurn, Block }

public enum Triggers { Null, Drawn, EntersBoard, Played, GenericCreatureEntersBoard, PlayerCreatureEntersBoard, OpponentCreatureEntersBoard, GenericCardPlayed, PlayerCardPlayed, OpponentCardPlayed, PlayerDrawsCard, OpponentDrawsCard }

public enum Targets { All_Creatures, All_Players, Everything, One_Creature, One_Player, One_Target }

public enum ResponseTypes { Cards, CardName, CardType }

public enum AttckStates { AttckerAdvantage, DefenderAdvantage }

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
    public GameObject OpponentObject { get => _opponentObject; set => _opponentObject = value; }
    public LineRenderer TargetLine { get => _targetLine; set => _targetLine = value; }
    public List<Card> PlayerBoard { get => _playerBoard; set => _playerBoard = value; }
    public List<Card> OpponentBoard { get => _opponentBoard; set => _opponentBoard = value; }
    public int PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
    public int OpponentHealth { get => _opponentHealth; set => _opponentHealth = value; }
    public Vector2 HoverPos { get => _hoverPos; set => _hoverPos = value; }
    public float StackPositionX { get => _stackPositionX; set => _stackPositionX = value; }
    public float StackPositionY { get => _stackPositionY; set => _stackPositionY = value; }
    public float HoverPositionX { get => _hoverPositionX; set => _hoverPositionX = value; }
    public float HoverPositionY { get => _hoverPositionY; set => _hoverPositionY = value; }
    public bool SeperateCombatPhase { get => _seperateCombatPhase; set => _seperateCombatPhase = value; }
    public CardGameEvent[] Events { get => _events; set => _events = value; }
    public AttckStates CurrentState { get => _currentState; set => _currentState = value; }

    public GameObject creaturePrefab;

    [SerializeField]
    private int _turnCounter;
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
    private int _playerHealth = 20;
    [SerializeField]
    private int _playerResource;
    [HideInInspector]
    public bool maxPlayerResourceEnabled;
    [HideInInspector]
    public int maxPlayerResource;
    private int _opponentHealth = 20;
    [SerializeField]
    private int _opponentResource;
    private float _stackPositionX;
    private float _stackPositionY;
    private float _hoverPositionX;
    private float _hoverPositionY;
    private Vector2 _stackPos;
    private Vector2 _hoverPos;
    private float _cardSize;
    [SerializeField]
    private float _lerpSpeed;
    private bool _seperateCombatPhase;
    private bool _targeting = false;
    private Card _waitingForTarget;

    public GameObject target;
    public GameObject targetArrow;

    [SerializeField]
    private GameObject _playerObject;
    [SerializeField]
    private GameObject _opponentObject;
    [SerializeField]
    private LineRenderer _targetLine;
    private Vector3[] _linePoints;

    private Phase[] _phaseList;
    private Phase _currPhase;
    private int _currPhaseIndex = 0;

    private GameObject _cardObjectInStack;

    private AttckStates _currentState;

    [SerializeField]
    private CardGameEvent[] _events;

    private int _readiedCounter = 0;
    [SerializeField]
    private GameObject _attackButton;

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
        target.SetActive(false);
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
        _turnCounter++;
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

        TestEnemyCard.Attack = 1;

        TestEnemyCard.MaxHealth = 10;

        TestEnemyCard.OnCreation();

        _opponentBoard.Add(TestEnemyCard);
    }

    public void ProgressPhases()
    {
        if(_currPhaseIndex + 1 == _phaseList.Length)
        {
            _currPhaseIndex = 0;
            _turnCounter++;
            ResetResource();
            AddResource(_turnCounter);
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
            Target target = ReturnTargetFromBoard(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
        Card cardPlayed = cardObject.GetComponent<PrefabEvents>().ThisCard;
        RemoveResource(cardPlayed.Cost);
        RaiseEvent(Triggers.Played, cardPlayed);
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
                target.SetActive(false);
                foreach(Effect effect in card.Effects)
                {
                    if(effect.Trigger == Triggers.Played)
                    {
                        effect.PerformEffect(card, card.Targets[0]);
                    }
                }
                break;
            case CardType.QuickSpell:
                _linePoints = new Vector3[2] { Vector3.zero, Vector3.zero };
                _targetLine.SetPositions(_linePoints);
                target.SetActive(false);
                foreach (Effect effect in card.Effects)
                {
                    if (effect.Trigger == Triggers.Played)
                    {
                        effect.PerformEffect(card, card.Targets[0]);
                    }
                }
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
                foreach (Effect effect in card.Effects)
                {
                    if (effect.Trigger == Triggers.Played)
                    {
                        effect.PerformEffect(card, card.Targets[0]);
                    }
                }
                break;
            case CardType.QuickSpell:
                foreach (Effect effect in card.Effects)
                {
                    if (effect.Trigger == Triggers.Played)
                    {
                        effect.PerformEffect(card, card.Targets[0]);
                    }
                }
                break;
            case CardType.Static:
                break;
            default:
                break;
        }
    }

    public Target ReturnTargetFromBoard(Vector3 targetPosition)
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

        if (Vector3.Distance(zNormalizedTarget, PlayerObject.transform.position) < 2)
        {
            return PlayerObject.GetComponent<Stats>().HealthObject;
        }
        if (Vector3.Distance(zNormalizedTarget, OpponentObject.transform.position) < 2)
        {
            return OpponentObject.GetComponent<Stats>().HealthObject;
        }

        return null;
    }

    public void Target(Vector3 targeter, Card card, Triggers triggerEvent)
    {
        targeter -= new Vector3(0,0,6);
        StartCoroutine(Targeting(targeter, card, triggerEvent));
    }

    public void Target(Vector3 targeter, Vector3 endOfLine)
    {
        if (!target.activeInHierarchy)
        {
            target.SetActive(true);
        }

        target.transform.position = endOfLine;
        Vector3 difference = targeter - targetArrow.transform.position;

        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, rotZ - 270f));
        target.transform.rotation = newRotation;
        _linePoints = new Vector3[2] { targeter, endOfLine + (difference * 0.6f) };
        _targetLine.SetPositions(_linePoints);
    }

    public IEnumerator Targeting(Vector3 targeter, Card card, Triggers triggerEvent)
    {
        for(; ; )
        {
            yield return new WaitForSeconds(0.001f);
            Vector3 zLockedMousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, targeter.z);
            Target(targeter, zLockedMousePos);
            if (Input.GetMouseButtonDown(0))
            {
                Target returnedTarget = ReturnTargetFromBoard(zLockedMousePos);
                if (returnedTarget != null)
                {
                    card.Targets.Add(returnedTarget);
                    target.SetActive(false);
                    _linePoints = new Vector3[2] { Vector3.zero, Vector3.zero };
                    _targetLine.SetPositions(_linePoints);
                    RaiseEvent(triggerEvent, card);
                    break;
                }
            }
        }

        StopAllCoroutines();
        yield return null;
    }

    public bool CheckCastingCost(int castingCost)
    {
        if(_playerResource - castingCost < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ResetResource()
    {
        _playerResource = 0;
    }

    public void AddResource()
    {
        _playerResource += 1;

        if (maxPlayerResourceEnabled)
        {
            _playerResource = Mathf.Clamp(_playerResource, 0, maxPlayerResource);
        }
    }

    public void AddResource(int amountToAdd)
    {
        _playerResource += amountToAdd;

        if (maxPlayerResourceEnabled)
        {
            _playerResource = Mathf.Clamp(_playerResource, 0, maxPlayerResource);
        }
    }

    public void RemoveResource()
    {
        _playerResource -= 1;
    }

    public void RemoveResource(int amountToRemove)
    {
        _playerResource -= amountToRemove;
    }

    public void RaiseEvent(Triggers trigger, Card triggeredCard)
    {
        switch (trigger)
        {
            case Triggers.EntersBoard:
                _events[0].RaiseCard(triggeredCard);
                break;
            case Triggers.Drawn:
                _events[1].RaiseCard(triggeredCard);
                break;
            case Triggers.Played:
                _events[2].RaiseCard(triggeredCard);
                break;
        }
    }

    public void CardReadied()
    {
        if(_readiedCounter == 0)
        {
            DisplayHideAttackButton();
        }
        _readiedCounter += 1;
    }

    public void CardUnreadied()
    {
        _readiedCounter -= 1;
        if(_readiedCounter == 0)
        {
            DisplayHideAttackButton();
        }
    }

    public void DisplayHideAttackButton()
    {
        _attackButton.SetActive(!_attackButton.activeInHierarchy);
    }

    public void AttackWithReadied()
    {
        //Used for AI or multiplayer when implimented
        //IMPROVEMENT: Add the ability to order after deciding what blocks
    }

    public void ResolveBlocks()
    {
        foreach (Card card in _playerBoard)
        {
            if (card.IsReadied)
            {
                if (card.BeingBlockedBy.Count != 0)
                {
                    int totalAttack = card.Attack;
                    foreach (Card blockingCard in card.BeingBlockedBy)
                    {
                        int healthBeforeAttack = blockingCard.Health;
                        blockingCard.Health -= totalAttack;
                        totalAttack -= healthBeforeAttack;

                        card.Health -= blockingCard.Attack;

                        if (totalAttack <= 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    OpponentHealth -= card.Attack;
                }
            }
        }
    }
}
