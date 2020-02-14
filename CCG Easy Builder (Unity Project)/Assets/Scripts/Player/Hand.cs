using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct CardFrames
{
    public Sprite creatureFrame;
    public Sprite spellFrame;
    public Sprite staticFrame;
    //public Sprite <- Add more card frames here 
}

public class Hand : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject handObject;

    public AnimationClip moveCard;

    private Card _currCard;

    private Vector3 _drawLocation;

    [SerializeField]
    private bool _canDrawCard = true;
    private int _bufferedCardDraw = 0;
    private List<Card> cardDrawQueue = new List<Card>();

    #region Hand Formatting
    private float _targetPositionX = 0;
    [SerializeField]
    private float _maxPositionY = 0.2f;
    private float _targetScale = 1;
    private float _targetRot;
    [SerializeField]
    private float _maximumRot = 10;
    private float _cardSize;
    private float _lerpSpeed;
    #endregion

    #region Representation of hand in game
    [SerializeField]
    private float _physicalHandSizeX;
    [SerializeField]
    private float _physicalHandSizeY;
    #endregion

    [SerializeField]
    private CardFrames _frames;

    private int _orderInSortingLayer = 0;

    [SerializeField]
    private List<Card> _currentHand;

    #region Max Hand Size
    // Used for nice inspector UI
    [HideInInspector]
    public bool maxHandSizeOn;
    [HideInInspector]
    public int maxHandSize;
    #endregion

    public List<Card> CurrentHand { get => _currentHand; set => _currentHand = value; }
    public float PhysicalHandSizeX { get => _physicalHandSizeX; set => _physicalHandSizeX = value; }
    public float PhysicalHandSizeY { get => _physicalHandSizeY; set => _physicalHandSizeY = value; }

    private int _counter = 0;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_physicalHandSizeX, _physicalHandSizeY));
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        _currentHand = GameManager.Instance.CurrentHand;
        _cardSize = GameManager.Instance.CardSize;
        _lerpSpeed = GameManager.Instance.LerpSpeed;
    }

    public void AddCardNumber()
    {
        if (!_canDrawCard)
        {
            _bufferedCardDraw++;
            Card newCard = Card.CreateInstance<Card>();
            newCard.CardName = _counter.ToString();
            _counter++;
            cardDrawQueue.Add(newCard);
        }
        else
        {
            _currentHand.Add(Card.CreateInstance<Card>());
        }
        //_currentHand[_currentHand.Count - 1].CardName = _orderInSortingLayer.ToString();
    }

    public void AddCardFromDeck(Card cardBeingAdded, Vector3 cardSpawnPosition)
    {
        if(_drawLocation != cardSpawnPosition)
        {
            _drawLocation = cardSpawnPosition;
        }

        if (!_canDrawCard)
        {
            _bufferedCardDraw++;
            Card newCard = cardBeingAdded;
            _counter++;
            cardDrawQueue.Add(newCard);
        }
        else
        {
            _currentHand.Add(cardBeingAdded);
            DrawLatestCard();
        }
    }

    public void DrawLatestCard()
    {
        if (_canDrawCard)
        {
            _canDrawCard = false;

            Card cardDrawn = _currentHand[_currentHand.Count - 1];

            _currCard = cardDrawn;

            Debug.Log(cardDrawn.CardName);

            SetUpCustomAnim();

            GameObject cardDrawnPrefab = Instantiate(cardPrefab, _drawLocation, Quaternion.identity, handObject.transform);

            cardDrawnPrefab.GetComponent<Animation>().AddClip(moveCard, moveCard.name);
            cardDrawnPrefab.GetComponent<Animation>().Play(moveCard.name);

            cardDrawnPrefab.GetComponent<PrefabEvents>().ThisCard = cardDrawn;

            Transform cardFront = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(0);
            Transform cardBack = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(1);

            cardFront.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;
            cardBack.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;

            _orderInSortingLayer++;

            Image frameRenderer = cardFront.Find("Frame").GetComponent<Image>();

            switch (cardDrawn.CardType)
            {
                case CardType.Creature:
                    frameRenderer.sprite = _frames.creatureFrame;

                    cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = cardDrawn.Attack.ToString();
                    cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = cardDrawn.Health.ToString();
                    break;
                case CardType.SlowSpell:
                    frameRenderer.sprite = _frames.spellFrame;
                    break;
                case CardType.QuickSpell:
                    frameRenderer.sprite = _frames.spellFrame;
                    break;
                case CardType.Static:
                    frameRenderer.sprite = _frames.staticFrame;
                    break;
            }

            cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = cardDrawn.CardImage;

            cardFront.Find("CardName").GetComponent<TextMeshProUGUI>().text = cardDrawn.CardName;
            cardFront.Find("CardDescription").GetComponent<TextMeshProUGUI>().text = cardDrawn.Description;
            cardFront.Find("CardCost").GetComponent<TextMeshProUGUI>().text = cardDrawn.Cost.ToString();

            cardDrawnPrefab.GetComponent<PrefabEvents>().RelativeHand = this;

            cardDrawn.CardGameObject = cardDrawnPrefab;
        }
    }

    public void DrawSpecificCard(Card specificCardToDraw)
    {
        _canDrawCard = false;

        Card cardDrawn = specificCardToDraw;

        _currCard = cardDrawn;

        Debug.Log(cardDrawn.CardName);

        SetUpCustomAnim();

        GameObject cardDrawnPrefab = Instantiate(cardPrefab, _drawLocation, Quaternion.identity, handObject.transform);

        cardDrawnPrefab.GetComponent<Animation>().AddClip(moveCard, moveCard.name);
        cardDrawnPrefab.GetComponent<Animation>().Play(moveCard.name);

        cardDrawnPrefab.GetComponent<PrefabEvents>().ThisCard = cardDrawn;

        Transform cardFront = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(0);
        Transform cardBack = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(1);

        cardFront.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;
        cardBack.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;

        _orderInSortingLayer++;

        Image frameRenderer = cardFront.Find("Frame").GetComponent<Image>();

        switch (cardDrawn.CardType)
        {
            case CardType.Creature:
                frameRenderer.sprite = _frames.creatureFrame;
                break;
            case CardType.SlowSpell:
                frameRenderer.sprite = _frames.spellFrame;
                break;
            case CardType.QuickSpell:
                frameRenderer.sprite = _frames.spellFrame;
                break;
            case CardType.Static:
                frameRenderer.sprite = _frames.staticFrame;
                break;
        }


        cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = cardDrawn.CardImage;

        cardFront.Find("CardName").GetComponent<TextMeshProUGUI>().text = cardDrawn.CardName;
        cardFront.Find("CardDescription").GetComponent<TextMeshProUGUI>().text = cardDrawn.Description;
        cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = cardDrawn.Attack.ToString();
        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = cardDrawn.Health.ToString();
        cardFront.Find("CardCost").GetComponent<TextMeshProUGUI>().text = cardDrawn.Cost.ToString();

        cardDrawnPrefab.GetComponent<PrefabEvents>().RelativeHand = this;

        cardDrawn.CardGameObject = cardDrawnPrefab;
    }

    public void AddCardToHand()
    {
        Card cardToAddToHand = _currCard;

        if (_currentHand.Count > 3)
        {
            _targetRot = _maximumRot;
            if (_physicalHandSizeX/2 >= _targetPositionX + _cardSize)
            {
                _targetPositionX += _cardSize;
            }
        }
        else
        {
            _targetRot = 0;
            if(_currentHand.Count > 1)
            {
                _targetPositionX += _cardSize;
            }
        }

        GameObject cardgo = cardToAddToHand.CardGameObject;

        StartCoroutine(CardToHand(cardgo));
    }

    public void AddCardToHand(Card cardToAdd)
    {
        Card cardToAddToHand = cardToAdd;

        if (_currentHand.Count > 3)
        {
            _targetRot = _maximumRot;
            if (_physicalHandSizeX / 2 >= _targetPositionX + (_cardSize * 2))
            {
                _targetPositionX += _cardSize;
            }
        }
        else
        {
            _targetRot = 0;
            if (_currentHand.Count > 1)
            {
                _targetPositionX += _cardSize;
            }
        }

        GameObject cardgo = cardToAddToHand.CardGameObject;

        StartCoroutine(CardToHand(cardgo));
    }

    public IEnumerator CardToHand(GameObject cardgo)
    {
        UpdateHandPositions();
        for (; ; )
        {
            cardgo.transform.position = Vector3.Lerp(cardgo.transform.position, new Vector3(_targetPositionX, transform.position.y, 0), _lerpSpeed);
            cardgo.transform.localScale = Vector3.Lerp(cardgo.transform.localScale, new Vector3(_targetScale, _targetScale, cardgo.transform.localScale.z), _lerpSpeed);
            cardgo.transform.rotation = Quaternion.Lerp(cardgo.transform.rotation, new Quaternion(cardgo.transform.rotation.x, cardgo.transform.rotation.y, Quaternion.Euler(0,0,-_targetRot).z, cardgo.transform.rotation.w), _lerpSpeed);
            if (Mathf.Abs(transform.position.y - cardgo.transform.position.y) < 0.002f)
            {
                break;
            }
            yield return new WaitForSeconds(0.02f);
        }
        cardgo.GetComponent<PrefabEvents>().CanBeHovered = true;
        cardgo.GetComponent<PrefabEvents>().HasBeenToHand = true;
        _canDrawCard = true;
        if(_bufferedCardDraw > 0)
        {
            _bufferedCardDraw--;
            Card cardToAdd = cardDrawQueue[0];
            _currentHand.Add(cardToAdd);
            cardDrawQueue.Remove(cardToAdd);
            DrawSpecificCard(cardToAdd);
        }
        StopCoroutine("CardToHand");
    }

    public void RemoveCardFromHand(Card card)
    {
        _currentHand.Remove(card);
        UpdateHandPositionsForRemoval();
    }

    private void UpdateHandPositions()
    {
        if(CurrentHand.Count <= 3)
        {
            float position = -_targetPositionX;
            foreach (Card card in _currentHand)
            {
                if (!card.CardGameObject.activeInHierarchy)
                {
                    card.CardGameObject.SetActive(true);
                    card.CardGameObject.GetComponent<PrefabEvents>().IsBeingHovered = false;
                    Destroy(card.CardGameObject.GetComponent<PrefabEvents>().ViewObject);
                }
                if (card != _currentHand[_currentHand.Count - 1])
                {
                    card.CardGameObject.transform.position = new Vector3(position, card.CardGameObject.transform.position.y, card.CardGameObject.transform.position.z);
                    position += ((_targetPositionX * 2) / (_currentHand.Count - 1));
                }
            }
        }
        else
        {
            float rotation = -_maximumRot;
            float position = -_targetPositionX;
            float height = _maxPositionY;
            int maxDistFromMiddle = Mathf.RoundToInt(((float)_currentHand.Count / 2f + 0.1f)) - 1;
            int distanceFromMiddle;
            int[] middle;
            bool inMiddle = false;
            if(_currentHand.Count % 2 == 0)
            {
                middle = new int[2];
                middle[0] = (_currentHand.Count / 2) - 1;
                middle[1] = middle[0] + 1;
            }
            else
            {
                middle = new int[1];
                middle[0] = (_currentHand.Count / 2);
            }
            foreach(Card card in _currentHand)
            {
                if (card != _currentHand[_currentHand.Count-1])
                {
                    foreach(int index in middle)
                    {
                        if(index == _currentHand.IndexOf(card))
                        {
                            inMiddle = true;
                        }
                    }
                    if (inMiddle)
                    {
                        height = _maxPositionY;
                    }
                    else
                    {
                        distanceFromMiddle = 0;
                        if (middle.Length > 1)
                        {
                            if (_currentHand.IndexOf(card) < middle[0])
                            {
                                distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[0]);
                            }
                            else
                            {
                                distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[1]);
                            }
                        }
                        else
                        {
                            distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[0]);
                        }
                        height = (1 - ((float)distanceFromMiddle / (float)maxDistFromMiddle)) * _maxPositionY;
                    }
                    card.CardGameObject.transform.eulerAngles = new Vector3(card.CardGameObject.transform.rotation.x, card.CardGameObject.transform.rotation.y, -rotation);
                    card.CardGameObject.transform.position = new Vector3(position, transform.position.y + height, card.CardGameObject.transform.position.z);
                    rotation += ((_maximumRot * 2) / (_currentHand.Count - 1));
                    position += ((_targetPositionX * 2) / (_currentHand.Count - 1));
                    inMiddle = false;
                }

                if (!card.CardGameObject.activeInHierarchy)
                {
                    card.CardGameObject.SetActive(true);
                    card.CardGameObject.GetComponent<PrefabEvents>().IsBeingHovered = false;
                    Destroy(card.CardGameObject.GetComponent<PrefabEvents>().ViewObject);
                }
            }
        }
    }

    private void UpdateHandPositionsForRemoval()
    {
        if (_currentHand.Count > 3)
        {
            _targetRot = _maximumRot;
            if (_physicalHandSizeX / 2 >= _cardSize * _currentHand.Count)
            {
                _targetPositionX -= _cardSize;
            }
        }
        else
        {
            _targetRot = 0;
            if (_currentHand.Count != 0)
            {
                _targetPositionX -= _cardSize;
            }
            else
            {
                _targetPositionX = 0;
            }
        }

        if (CurrentHand.Count <= 3)
        {
            float position = -_targetPositionX;
            foreach (Card card in _currentHand)
            {
                card.CardGameObject.transform.position = new Vector3(position, transform.position.y, card.CardGameObject.transform.position.z);
                card.CardGameObject.transform.eulerAngles = Vector3.zero;
                position += ((_targetPositionX * 2) / (_currentHand.Count - 1));
            }
        }
        else
        {
            float rotation = -_maximumRot;
            float position = -_targetPositionX;
            float height = _maxPositionY;
            int maxDistFromMiddle = Mathf.RoundToInt(((float)_currentHand.Count / 2f + 0.1f)) - 1;
            int distanceFromMiddle;
            int[] middle;
            bool inMiddle = false;
            if (_currentHand.Count % 2 == 0)
            {
                middle = new int[2];
                middle[0] = (_currentHand.Count / 2) - 1;
                middle[1] = middle[0] + 1;
            }
            else
            {
                middle = new int[1];
                middle[0] = (_currentHand.Count / 2);
            }
            foreach (Card card in _currentHand)
            {
                foreach (int index in middle)
                {
                    if (index == _currentHand.IndexOf(card))
                    {
                        inMiddle = true;
                    }
                }
                if (inMiddle)
                {
                    height = _maxPositionY;
                }
                else
                {
                    distanceFromMiddle = 0;
                    if (middle.Length > 1)
                    {
                        if (_currentHand.IndexOf(card) < middle[0])
                        {
                            distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[0]);
                        }
                        else
                        {
                            distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[1]);
                        }
                    }
                    else
                    {
                        distanceFromMiddle = Mathf.Abs(_currentHand.IndexOf(card) - middle[0]);
                    }
                    height = (1 - ((float)distanceFromMiddle / (float)maxDistFromMiddle)) * _maxPositionY;
                }
                card.CardGameObject.transform.eulerAngles = new Vector3(card.CardGameObject.transform.rotation.x, card.CardGameObject.transform.rotation.y, -rotation);
                card.CardGameObject.transform.position = new Vector3(position, transform.position.y + height, card.CardGameObject.transform.position.z);
                rotation += ((_maximumRot * 2) / (_currentHand.Count - 1));
                position += ((_targetPositionX * 2) / (_currentHand.Count - 1));
                inMiddle = false;
            }
        }
    }

    private void SetUpCustomAnim()
    {
        AnimationCurve curve;

        moveCard.legacy = true;

        float startXPos = _drawLocation.x - transform.position.x;
        float startYPos = _drawLocation.y - transform.position.y;
        float startZPos = _drawLocation.z - transform.position.z;

        float endYPos = 2.72f;
        float endZPos = -4f;

        Keyframe[] keys;
        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, startXPos);
        keys[1] = new Keyframe(0.75f, 0.0f);
        curve = new AnimationCurve(keys);
        moveCard.SetCurve("", typeof(Transform), "localPosition.x", curve);

        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, startYPos);
        keys[1] = new Keyframe(0.75f, endYPos);
        curve = new AnimationCurve(keys);
        moveCard.SetCurve("", typeof(Transform), "localPosition.y", curve);

        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, startZPos);
        keys[1] = new Keyframe(0.75f, endZPos);
        curve = new AnimationCurve(keys);
        moveCard.SetCurve("", typeof(Transform), "localPosition.z", curve);
    }
}
