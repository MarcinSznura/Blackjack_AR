using UnityEngine;
using Vuforia;

public class Validator : MonoBehaviour
{
    [Header("Children")]
    [SerializeField] CardTrackableEventHandler[] children;
    [SerializeField] float timeToCountCard = 1f;
    [SerializeField] float minDistanceBetweenCards;

    [Header("Cards parameters")]
    [SerializeField] int cardValue;
    [SerializeField] int countedCards;

    private GameMaster gameMaster;

    private void Awake()
    {
        children = GetComponentsInChildren<CardTrackableEventHandler>();
        gameMaster = FindObjectOfType<GameMaster>();
    }

    void Update()
    {
        if (CanIRead())
        {
            if (NumberOfMyChildrenInfrontOfCamera() > countedCards)
            {
                countedCards++;
                gameMaster.IncreasePlayerScore(cardValue);
                gameMaster.playerHand.Add(cardValue);
                Debug.Log("I counted card: " + cardValue.ToString());
            }
        }

        foreach (CardTrackableEventHandler child in children)
        {
            foreach (CardTrackableEventHandler child2 in children)
            {
                if (child.id != child2.id)
                {
                    if (Vector3.Distance(child.transform.position, child2.transform.position) < minDistanceBetweenCards
                        && child.tracked && child2.tracked)
                    {
                        child.presentTime = 0;
                        print(Vector3.Distance(child.transform.position, child2.transform.position));
                        //Debug.LogError("TOO FUCKING CLOSE!");
                    }
                    else
                    {
                       
                    }
                }
            }
        }
    }

    private int NumberOfMyChildrenInfrontOfCamera()
    {
        int cardNumber = 0;
        foreach (CardTrackableEventHandler child in children)
        {
            if (child.presentTime > timeToCountCard)
            {
                cardNumber += 1;
            }
        }
        return cardNumber;
    }

    private bool CanIRead()
    {
        if (gameMaster.GetCurrentStage() == 4
            || gameMaster.GetCurrentStage() == 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RestartCountedCards()
    {
        countedCards = 0;
    }

}
