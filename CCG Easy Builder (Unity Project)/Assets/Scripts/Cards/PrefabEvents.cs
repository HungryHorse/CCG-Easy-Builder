using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrefabEvents : MonoBehaviour
{
    private Hand _relativeHand;
    private Card _thisCard;
    private GameObject _viewObject;
    private bool _isBeingHovered = false;
    private GameObject _originalCard;
    private float _timer;
    private bool _canBeHovered = false;
    private bool _beingHeld;
    private bool _returnToHand;
    private bool _canBeDestroyed;
    private bool _onStack = false;
    private bool _isCreatureCard = false;
    private bool _hasBeenToHand = false;

    [SerializeField]
    private float _yIncreaseOnHover;
    [SerializeField]
    private float _hoverScale;
    [SerializeField]
    private float _zoomSpeed;
    [SerializeField]
    private float _followSpeed;

    private Vector3 _targetScale; 
    private Vector3 _targetPosition; 

    public Hand RelativeHand { get => _relativeHand; set => _relativeHand = value; }
    public Card ThisCard { get => _thisCard; set => _thisCard = value; }
    public GameObject OriginalCard { get => _originalCard; set => _originalCard = value; }
    public bool CanBeHovered { get => _canBeHovered; set => _canBeHovered = value; }
    public float HoverScale { get => _hoverScale; set => _hoverScale = value; }
    public GameObject ViewObject { get => _viewObject; set => _viewObject = value; }
    public bool IsBeingHovered { get => _isBeingHovered; set => _isBeingHovered = value; }
    public bool HasBeenToHand { get => _hasBeenToHand; set => _hasBeenToHand = value; }

    private void Start()
    {
        _targetScale = new Vector3(_hoverScale, _hoverScale, 1);
        _targetPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + _yIncreaseOnHover, -4);
    }

    public void FinishedDrawAnimation()
    {
        Debug.Log("FinishedAnim");
        this.GetComponent<Animator>().enabled = false;
        RelativeHand.AddCardToHand();
    }

    private void Update()
    {
        if (!_onStack)
        {
            Vector3 zLockedMousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, transform.position.z);

            if (!_isBeingHovered && gameObject.transform.localScale.x != 1 && _hasBeenToHand)
            {
                gameObject.transform.localScale = new Vector3(1,1,1);
            }

            if (_isBeingHovered && Input.GetMouseButton(0) && _originalCard != null)
            {
                _beingHeld = true;
                _returnToHand = true;
            }
            else
            {
                _beingHeld = false;
            }

            if (_beingHeld)
            {
                Debug.Log("Held");
                transform.position = Vector3.Lerp(transform.position, zLockedMousePos, _followSpeed);
                GetComponent<BoxCollider2D>().size = new Vector2(10 * _hoverScale, 10 * _hoverScale);
                _canBeDestroyed = false;
                bool _outsideX = false;
                bool _outsideY = false;
                bool _outsideHand = false;
                if ((transform.position.x > RelativeHand.transform.position.x + RelativeHand.PhysicalHandSizeX / 2) || (transform.position.x < RelativeHand.transform.position.x - RelativeHand.PhysicalHandSizeX / 2))
                {
                    _outsideX = true;
                }
                if ((transform.position.y > RelativeHand.transform.position.y + RelativeHand.PhysicalHandSizeY / 2) || (transform.position.y < RelativeHand.transform.position.y - RelativeHand.PhysicalHandSizeY / 2))
                {
                    _outsideY = true;
                }

                if (_outsideX || _outsideY)
                {
                    _outsideHand = true;
                }

                if (_outsideHand && _originalCard != null)
                {
                    transform.localScale = _originalCard.transform.localScale;
                }
            }
            else
            {
                if (_originalCard != null && _returnToHand)
                {
                    bool _outsideX = false;
                    bool _outsideY = false;
                    bool _outsideHand = false;
                    if ((transform.position.x > RelativeHand.transform.position.x + RelativeHand.PhysicalHandSizeX / 2) || (transform.position.x < RelativeHand.transform.position.x - RelativeHand.PhysicalHandSizeX / 2))
                    {
                        _outsideX = true;
                    }
                    if ((transform.position.y > RelativeHand.transform.position.y + RelativeHand.PhysicalHandSizeY / 2) || (transform.position.y < RelativeHand.transform.position.y - RelativeHand.PhysicalHandSizeY / 2))
                    {
                        _outsideY = true;
                    }

                    if (_outsideX || _outsideY)
                    {
                        _outsideHand = true;
                    }

                    if (_outsideHand)
                    {
                        RelativeHand.RemoveCardFromHand(_thisCard);
                        GameManager.Instance.PlayCard(gameObject);
                    }
                    else if (_returnToHand)
                    {
                        GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 2f);
                        if (Vector3.Distance(transform.localPosition, _targetPosition - new Vector3(0, _yIncreaseOnHover, 0)) > 0.02f)
                        {
                            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition - new Vector3(0, _yIncreaseOnHover, 0), _zoomSpeed);
                            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, _zoomSpeed);
                        }
                        else
                        {
                            _canBeDestroyed = true;
                            _returnToHand = false;
                            if (_originalCard != null)
                            {
                                _originalCard.SetActive(true);
                                _originalCard.GetComponent<PrefabEvents>()._isBeingHovered = false;
                                Destroy(gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isBeingHovered && _originalCard != null && _timer >= 0.02f && !_onStack)
        {
            _originalCard.SetActive(true);
            _originalCard.GetComponent<PrefabEvents>()._isBeingHovered = false;
            Destroy(gameObject);
        }
        _timer += Time.deltaTime;
    }

    private void OnMouseOver()
    {
        if (!_onStack && GameManager.Instance.Stack.Count < 1)
        {
            CreateHoveredCard();
            _isBeingHovered = true;
            _timer = 0;
            if (_originalCard != null)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, _zoomSpeed);
                if (!_beingHeld)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, _zoomSpeed);
                }
            }
        }
    }

    private void OnMouseExit()
    {
        if (!_onStack)
        {
            if (_isBeingHovered && _canBeDestroyed)
            {
                if (_originalCard != null)
                {
                    _originalCard.SetActive(true);
                    _originalCard.GetComponent<PrefabEvents>()._isBeingHovered = false;
                    Destroy(gameObject);
                }
                _isBeingHovered = false;
            }

            _isBeingHovered = false;
        }
    }

    private void CreateHoveredCard()
    {
        try
        {
            if (_canBeHovered && _originalCard == null)
            {
                _viewObject = Instantiate(gameObject, _relativeHand.transform);

                _viewObject.transform.eulerAngles = new Vector3(0, 0, 0);

                _viewObject.GetComponent<BoxCollider2D>().size = new Vector2(_viewObject.GetComponent<BoxCollider2D>().size.x / _hoverScale, _viewObject.GetComponent<BoxCollider2D>().size.y);

                _viewObject.GetComponent<PrefabEvents>().OriginalCard = gameObject;

                _viewObject.GetComponent<PrefabEvents>().RelativeHand = _relativeHand;

                _viewObject.GetComponent<PrefabEvents>().ThisCard = _thisCard;

                _viewObject.GetComponent<PrefabEvents>()._targetScale = new Vector3(_hoverScale, _hoverScale, 1);

                gameObject.SetActive(false);

                Transform cardFront = _viewObject.transform.GetChild(0).transform.GetChild(0);
                Transform cardBack = _viewObject.transform.GetChild(0).transform.GetChild(1);

                cardFront.GetComponent<Canvas>().sortingOrder = 0;
                cardBack.GetComponent<Canvas>().sortingOrder = 0;

                cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = _thisCard.CardImage;

                cardFront.Find("CardName").GetComponent<TextMeshProUGUI>().text = _thisCard.CardName;
                cardFront.Find("CardDescription").GetComponent<TextMeshProUGUI>().text = _thisCard.Description;
                cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = _thisCard.Attack.ToString();
                cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = _thisCard.Health.ToString();
                cardFront.Find("CardCost").GetComponent<TextMeshProUGUI>().text = _thisCard.Cost.ToString();
            }
        }
        catch { }
    }

    private void OnDisable()
    {
        _onStack = true;
    }
    private void OnEnable()
    {
        _onStack = false;
    }
}
