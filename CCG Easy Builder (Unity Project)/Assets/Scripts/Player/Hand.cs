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
    private float _targetScale = 1;
    private float _targetRot;
    [SerializeField]
    private float _percentRot = 1;

    [SerializeField]
    private List<Card> currentHand;

    [HideInInspector]
    public bool maxHandSizeOn;
    [HideInInspector]
    public int maxHandSize;

    public List<Card> CurrentHand { get => currentHand; set => currentHand = value; }

    private void Start()
    {
        DrawLatestCard();
    }

    public void DrawLatestCard()
    {
        Card cardDrawn = currentHand[currentHand.Count - 1];

        Debug.Log(cardDrawn.CardName);

        GameObject cardDrawnPrefab = Instantiate(cardPrefab, handObject.transform);

        Transform cardFront = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(0);

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
        Debug.Log("AddToHand");
        Card cardToAddToHand = currentHand[currentHand.Count - 1];

        if (currentHand.Count > 1)
        {
            _targetRot = currentHand.Count * _percentRot;
        }
        else
        {
            _targetRot = 0;
        }

        GameObject cardgo = cardToAddToHand.CardGameObject;

        _currCard = cardgo;

        StartCoroutine(CardToHand(cardgo));
    }

    public IEnumerator CardToHand(GameObject cardgo)
    {
        for (; ; )
        {
            cardgo.transform.position = Vector3.Lerp(cardgo.transform.position, new Vector3(_targetPositionX, transform.position.y, cardgo.transform.position.z), 0.08f);
            cardgo.transform.localScale = Vector3.Lerp(cardgo.transform.localScale, new Vector3(_targetScale, _targetScale, cardgo.transform.localScale.z), 0.08f);
            cardgo.transform.rotation = Quaternion.Lerp(cardgo.transform.rotation, new Quaternion(cardgo.transform.rotation.x, cardgo.transform.rotation.y, Quaternion.Euler(0,0,-_targetRot).z, cardgo.transform.rotation.w), 0.08f);
            if(Mathf.Abs(transform.position.y - cardgo.transform.position.y) < 0.002f)
            {
                break;
            }
            Debug.Log("UpdatingPos");
            yield return new WaitForSeconds(0.02f);
        }
        StopCoroutine("CardToHand");
    }
}
