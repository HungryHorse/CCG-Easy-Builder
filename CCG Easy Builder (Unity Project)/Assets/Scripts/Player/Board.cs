using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    private static Board _instance;

    #region Representation of board in game
    [SerializeField]
    private float _physicalBoardSizeX;
    [SerializeField]
    private float _physicalBoardSizeY;
    #endregion

    #region
    [SerializeField]
    private List<Card> _playerBoard = new List<Card>();
    private List<Card> _opponentBoard = new List<Card>();
    #endregion

    public static Board Instance { get => _instance; set => _instance = value; }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_physicalBoardSizeX, _physicalBoardSizeY));
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector3(transform.position.x -_physicalBoardSizeX/2, transform.position.y, 0), new Vector3(transform.position.x + _physicalBoardSizeX / 2, transform.position.y, 0));
    }

    public void CreateCreature(Card card)
    {
        GameObject cardDrawnPrefab = Instantiate(GameManager.Instance.creaturePrefab, GameManager.Instance.StackPos, Quaternion.identity, gameObject.transform);

        Transform cardFront = cardDrawnPrefab.transform.GetChild(0).transform.GetChild(0);

        cardFront.GetChild(0).Find("Character").GetComponent<Image>().sprite = card.CardImage;

        cardFront.Find("CardAttack").GetComponent<TextMeshProUGUI>().text = card.Attack.ToString();
        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = card.Health.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
