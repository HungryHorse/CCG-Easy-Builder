using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private List<Card> _currentDeck;
    [SerializeField]
    private Hand _playerHand;

    public void AddCardToHand()
    {
        if (_currentDeck.Count > 0)
        {
            Card cardBeingDrawn = _currentDeck[0];
            _playerHand.AddCardFromDeck(cardBeingDrawn);
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
