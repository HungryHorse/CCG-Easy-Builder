using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabEvents : MonoBehaviour
{
    private Hand _relativeHand;

    public Hand RelativeHand { get => _relativeHand; set => _relativeHand = value; }

    public void FinishedDrawAnimation()
    {
        Debug.Log("FinishedAnim");
        this.GetComponent<Animator>().enabled = false;
        RelativeHand.AddCardToHand();
    }
}
