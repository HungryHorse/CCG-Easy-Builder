using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private static Deck _instance;
    [SerializeField]
    private List<Card> _currentDeck;
    [SerializeField]
    private Hand _playerHand;

    #region Representation of deck in game
    [SerializeField]
    private float _physicalDeckSizeX;
    [SerializeField]
    private float _physicalDeckSizeY;
    #endregion

    public static Deck Instance { get => _instance; set => _instance = value; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_physicalDeckSizeX, _physicalDeckSizeY));
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

    public void AddCardToHand()
    {
        if (_currentDeck.Count > 0)
        {
            Card cardBeingDrawn = _currentDeck[0];
            _playerHand.AddCardFromDeck(cardBeingDrawn, gameObject.transform.position);
            _currentDeck.Remove(cardBeingDrawn);
        }
    }
    
    public void ShuffleDeck()
    {
        for(int i = 0; i < _currentDeck.Count - 1; i ++)
        {
            int j = Random.Range(i, _currentDeck.Count);
            Card temp = _currentDeck[i];
            _currentDeck[i] = _currentDeck[j];
            _currentDeck[j] = temp;
        }
        for (int i = _currentDeck.Count - 1; i > 1; i--)
        {
            int j = Random.Range(0, i+1);
            Card temp = _currentDeck[i];
            _currentDeck[i] = _currentDeck[j];
            _currentDeck[j] = temp;
        }
    }
}
