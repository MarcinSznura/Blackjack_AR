using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCardsValues : MonoBehaviour
{
    [SerializeField] int aiCardsValues;

    public int GetCardValue()
    {
        return aiCardsValues;
    }
}
