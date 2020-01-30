using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private List<Card> _currentDeck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
