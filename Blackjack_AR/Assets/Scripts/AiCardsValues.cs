using UnityEngine;

public class AiCardsValues : MonoBehaviour
{
    [SerializeField] int aiCardsValues;

    public int GetCardValue()
    {
        return aiCardsValues;
    }
}