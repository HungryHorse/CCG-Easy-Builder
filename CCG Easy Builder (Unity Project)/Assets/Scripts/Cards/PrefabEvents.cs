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

    private void LateUpdate()
    {
        if (!_isBeingHovered && _originalCard != null && _timer >= 0.02f)
        {
            _originalCard.SetActive(true);
            Destroy(gameObject);
        }
        _timer += Time.deltaTime;

        if (_beingHeld)
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition, _followSpeed);
        }
    }

    private void OnMouseOver()
    {
        try
        {
            if (_canBeHovered)
            {
                _viewObject = Instantiate(gameObject, _relativeHand.transform);

                _viewObject.transform.eulerAngles = new Vector3(0, 0, 0);

                _viewObject.GetComponent<BoxCollider2D>().size = new Vector2(_viewObject.GetComponent<BoxCollider2D>().size.x / _hoverScale, _viewObject.GetComponent<BoxCollider2D>().size.y);

                _viewObject.GetComponent<PrefabEvents>().OriginalCard = gameObject;

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
        _isBeingHovered = true;
        _timer = 0;
        if (_originalCard != null)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, _zoomSpeed);
            if (Input.GetMouseButtonDown(0))
            {
                _beingHeld = true;
                Debug.Log("Held");
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, _zoomSpeed);
            }
        }
    }

    private void OnMouseExit()
    {
        if (_isBeingHovered)
        {
            if (_originalCard != null)
            {
                _originalCard.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
