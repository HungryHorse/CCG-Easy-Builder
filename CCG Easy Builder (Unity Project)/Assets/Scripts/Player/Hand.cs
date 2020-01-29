using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hand : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject handObject;

    private GameObject _currCard;

    private float _targetPositionX = 0;
    [SerializeField]
    private float _maxPositionY = 0.2f;
    private float _targetScale = 1;
    private float _targetRot;
    [SerializeField]
    private float _maximumRot = 10;
    [SerializeField]
    private float _cardSize;
    [SerializeField]
    private float _physicalHandSizeX;
    [SerializeField]
    private float _physicalHandSizeY;

    private int _orderInSortingLayer = 0;

    [SerializeField]
    private List<Card> _currentHand;

    [HideInInspector]
    public bool maxHandSizeOn;
    [HideInInspector]
    public int maxHandSize;

    public List<Card> CurrentHand { get => _currentHand; set => _currentHand = value; }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_physicalHandSizeX, _physicalHandSizeY));
    }

    private void Start()
    {
        DrawLatestCard();
    }

    public void AddCardNumber()
    {
        _currentHand.Add(Card.CreateInstance<Card>());
        //_currentHand[_currentHand.Count - 1].CardName = _orderInSortingLayer.ToString();
    }

    public void DrawLatestCard()
    {
        Card cardDrawn = _currentHand[_currentHand.Count - 1];

        Debug.Log(cardDrawn.CardName);

        GameObject cardDrawnPrefab = Instantiate(cardPrefab, handObject.transform);

        Transform cardFront = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(0);
        Transform cardBack = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(1);

        cardFront.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;
        cardBack.GetComponent<Canvas>().sortingOrder = _orderInSortingLayer;

        _orderInSortingLayer++;

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
        Card cardToAddToHand = _currentHand[_currentHand.Count - 1];

        if (_currentHand.Count > 3)
        {
            _targetRot = _maximumRot;
            if (_physicalHandSizeX/2 >= _targetPositionX + (_cardSize * 2))
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

        _currCard = cardgo;

        StartCoroutine(CardToHand(cardgo));
    }

    public IEnumerator CardToHand(GameObject cardgo)
    {
        UpdateHandPositions();
        for (; ; )
        {
            cardgo.transform.position = Vector3.Lerp(cardgo.transform.position, new Vector3(_targetPositionX, transform.position.y, 0), 0.08f);
            cardgo.transform.localScale = Vector3.Lerp(cardgo.transform.localScale, new Vector3(_targetScale, _targetScale, cardgo.transform.localScale.z), 0.08f);
            cardgo.transform.rotation = Quaternion.Lerp(cardgo.transform.rotation, new Quaternion(cardgo.transform.rotation.x, cardgo.transform.rotation.y, Quaternion.Euler(0,0,-_targetRot).z, cardgo.transform.rotation.w), 0.08f);
            if (Mathf.Abs(transform.position.y - cardgo.transform.position.y) < 0.002f)
            {
                break;
            }
            yield return new WaitForSeconds(0.02f);
        }
        StopCoroutine("CardToHand");
    }

    private void UpdateHandPositions()
    {
        if(CurrentHand.Count <= 3)
        {
            float position = -_targetPositionX;
            foreach (Card card in _currentHand)
            {
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
            }
        }
    }
}
