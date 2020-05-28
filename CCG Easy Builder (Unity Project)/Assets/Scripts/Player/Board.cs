using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    private static Board _instance;

    #region Representation of board in game
    private float _physicalBoardSizeX;
    private float _physicalBoardSizeY;
    private float _spacing;
    #endregion

    #region Boards
    [SerializeField]
    private List<Card> _playerBoard = new List<Card>();
    private List<Card> _opponentBoard = new List<Card>();
    #endregion

    private float _targetPositionX = 0;
    private float _cardSize;
    private float _lerpSpeed;
    private float _playerBoardMiddle;
    private float _opponentBoardMiddle;

    [SerializeField]
    private CardGameEvent _creatureEntersBoard;

    public static Board Instance { get => _instance; set => _instance = value; }
    public float OpponentBoardMiddle { get => _opponentBoardMiddle; set => _opponentBoardMiddle = value; }
    public float PlayerBoardMiddle { get => _playerBoardMiddle; set => _playerBoardMiddle = value; }
    public float PhysicalBoardSizeX { get => _physicalBoardSizeX; set => _physicalBoardSizeX = value; }
    public float PhysicalBoardSizeY { get => _physicalBoardSizeY; set => _physicalBoardSizeY = value; }
    public float Spacing { get => _spacing; set => _spacing = value; }

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
        _playerBoard = GameManager.Instance.PlayerBoard;
        _opponentBoard = GameManager.Instance.OpponentBoard;
        _cardSize = GameManager.Instance.CardSize;
        _lerpSpeed = GameManager.Instance.LerpSpeed;
        _playerBoardMiddle = transform.position.y - (_physicalBoardSizeY / 4);
        _opponentBoardMiddle = transform.position.y + (_physicalBoardSizeY / 4);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_physicalBoardSizeX, _physicalBoardSizeY));
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector3(transform.position.x -_physicalBoardSizeX/2, transform.position.y, 0), new Vector3(transform.position.x + _physicalBoardSizeX / 2, transform.position.y, 0));
    }

    public void CreateCreature(Card card)
    {
        _playerBoard.Add(card);

        GameObject creaturePrefab = Instantiate(GameManager.Instance.creaturePrefab, GameManager.Instance.StackPos, Quaternion.identity, gameObject.transform);

        creaturePrefab.GetComponent<PrefabEvents>().ThisCard = card;
        creaturePrefab.GetComponent<PrefabEvents>().IsCreatureCard = true;
        card.CardGameObject = creaturePrefab;

        Transform cardFront = creaturePrefab.transform.GetChild(0).transform.GetChild(0);

        cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = card.CardImage;

        cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = card.Attack.ToString();
        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = card.Health.ToString();

        if (_playerBoard.Count > 1)
        {
            _targetPositionX += (_cardSize + _spacing);
        }

        _creatureEntersBoard.RaiseCard(card);

        GameObject cardgo = card.CardGameObject;

        StartCoroutine(CreatureToBoard(cardgo));
    }

    public void CreateCreatureFormatted(Card card)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = new Vector3(mousePos.x, mousePos.y, 0);
        int index = CalcualteHoverIndex(position);
        if (index != -1)
        {
            _playerBoard.Insert(index, card);
        }
        else
        {
            if (_playerBoard.Count > 0)
            {
                _playerBoard.Insert(0, card);
            }
            else
            {
                _playerBoard.Add(card);
            }
        }

        GameObject creaturePrefab = Instantiate(GameManager.Instance.creaturePrefab, position, Quaternion.identity, gameObject.transform);

        creaturePrefab.GetComponent<PrefabEvents>().ThisCard = card;
        creaturePrefab.GetComponent<PrefabEvents>().FullCard = Instantiate(card.CardGameObject);
        card.CardGameObject = creaturePrefab;
        creaturePrefab.transform.position = new Vector3(0,_playerBoardMiddle,0);

        creaturePrefab.GetComponent<PrefabEvents>().IsCreatureCard = true;
        creaturePrefab.GetComponent<PrefabEvents>().CanBeHovered = true;

        Transform cardFront = creaturePrefab.transform.GetChild(0).transform.GetChild(0);

        cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = card.CardImage;

        cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = card.Attack.ToString();
        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = card.Health.ToString();

        if (_playerBoard.Count > 1)
        {
            _targetPositionX += (_cardSize + _spacing);
        }

        _creatureEntersBoard.RaiseCard(card);

        foreach (BaseAbility ability in card.Abilites)
        {
            ability.OnEnterEffect(card);
        }

        UpdatePlayerBoardState();
    }

    public int CalcualteHoverIndex(Vector3 newCardPos)
    {
        int index = -1;

        if(_playerBoard.Count >= 1)
        {
            index = 0;
            foreach(Card card in _playerBoard)
            {
                if(newCardPos.x > card.CardGameObject.transform.position.x)
                {
                    index++;
                }
                else
                {
                    return index;
                }
            }
        }

        return index;
    }

    public IEnumerator CreatureToBoard(GameObject cardgo)
    {
        UpdatePlayerBoardState();
        for (; ; )
        {
            cardgo.transform.position = Vector3.Lerp(cardgo.transform.position, new Vector3(_targetPositionX, _playerBoardMiddle, 0), _lerpSpeed);
            if (Mathf.Abs(_targetPositionX - cardgo.transform.position.x) < 0.002f)
            {
                break;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void UpdatePlayerBoardState()
    {
        float position = -_targetPositionX;
        foreach (Card card in _playerBoard)
        {
            if (card != _playerBoard[_playerBoard.Count - 1])
            {
                card.CardGameObject.transform.position = new Vector3(position, card.CardGameObject.transform.position.y, card.CardGameObject.transform.position.z);
                position += ((_targetPositionX * 2) / (_playerBoard.Count - 1));
            }
            else if (!GameManager.Instance.StackEnabled)
            {
                card.CardGameObject.transform.position = new Vector3(_targetPositionX, _playerBoardMiddle, 0);
            }
        }
    }

    public void RemoveCardFromBoard(Card card)
    {
        _playerBoard.Remove(card);
        _targetPositionX -= (_cardSize + _spacing);
        UpdatePlayerBoardState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
