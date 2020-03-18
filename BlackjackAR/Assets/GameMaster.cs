using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    enum States { NotStarted, Beting, SetingUp, PlayerSetup, PlayerMove, AiMove, Fighting };

    [Header("Parameters")]
    [SerializeField] int playerScore;
    [SerializeField] int aiScore;
    [SerializeField] int playerBalance;
    [SerializeField] int betProposition;
    [SerializeField] int bet;

    [Header("Decks and Hands")]
    [SerializeField] List<GameObject> allDeck = new List<GameObject>();
    [SerializeField] List<int> currentDeck = new List<int>();
    [SerializeField] List<int> aiHand = new List<int>();
    [SerializeField] List<int> playerHand = new List<int>();

    [Header("Current state")]
    [SerializeField] States state;

    void Start()
    {
        state = States.NotStarted;
        PutAllCardsInAiDeck();
    }

    void Update()
    {
        switch (state)
        {
            case States.NotStarted:
                //nothing but music... i duno yet
                break;

            case States.Beting:
                //nothing?... yet
                break;

            case States.SetingUp:
                GiveCardsToAi();
                state = States.PlayerSetup;
                break;

            case States.PlayerSetup:
                if (TwoCardsOnTable()) state = States.PlayerMove;
                else ShowMessagePut2CardsOnTable();
                break;

            case States.PlayerMove:
                PlayerReader();
                break;

            case States.AiMove:
                AiAlgoithm();
                break;

            case States.Fighting:
                if (PlayerWon())
                {
                    AnnounceWinner(0);
                    playerBalance += bet;
                }
                else if (playerScore == aiScore) AnnounceTie();
                else AnnounceWinner(1);
                aiScore = 0;
                playerScore = 0;
                state = States.Beting;
                break;
        }
    }


    #region("Initial Parameters")
    void PutAllCardsInAiDeck()
    {
        currentDeck.Clear();
        aiHand.Clear();
        for (int i = 0; i < 52; i++)
        {
            currentDeck.Add(i);
        }
    }

    public void StartGame()
    {
        playerBalance = 200;
        bet = 0;
        betProposition = 0;
        aiScore = 0;
        playerScore = 0;

        state = States.Beting;
    }
    #endregion


    #region("Beting")

    public void Raise10()
    {
        if (betProposition + 10 <= playerBalance)
        {
            betProposition += 10;
        }
    }

    public void Raise100()
    {
        if (betProposition + 100 <= playerBalance)
        {
            betProposition += 100;
        }
    }

    public void Raise1000()
    {
        if (betProposition + 1000 <= playerBalance)
        {
            betProposition += 1000;
        }
    }

    public void Reduce10()
    {
        betProposition -= 10;
        if (betProposition < 0)
        {
            betProposition = 0;
        }
    }

    public void Reduce100()
    {
        betProposition -= 100;
        if (betProposition < 0)
        {
            betProposition = 0;
        }
    }

    public void Reduce1000()
    {
        betProposition -= 1000;
        if (betProposition < 0)
        {
            betProposition = 0;
        }
    }

    public void MaxBet()
    {
        betProposition = playerBalance;
    }

    public void ZeroBet()
    {
        betProposition = 0;
    }

    public void Bet()
    {
        if (betProposition > 0)
        {
            bet = betProposition;
            playerBalance -= betProposition;
            betProposition = 0;
            state = States.SetingUp;
        }
    }

    #endregion


    #region("SettingUp Phase")
    void GiveCardsToAi()
    {
        DrawCard();
        DrawCard();
        //ShowCard(0); TODO do it
    }

    void DrawCard()
    {
        int newCard = Random.Range(0, currentDeck.Count);
        aiHand.Add(newCard);
        currentDeck.Remove(newCard);
        CalculateAiPoints();
    }

    void ShowCard()
    {
        //Object Card, Flip();
    }
    #endregion


    #region("Player Setup Phase")
    bool TwoCardsOnTable()
    {
        if (0==0)//!vuforia.detectedObjectIsTwo)
        {
            return true ;
            Debug.Log("Put 2 card on table");
        }
        else
        {
            return false;
        }

    }

    void ShowMessagePut2CardsOnTable()
    {
        //text.text put those cards;
    }
    #endregion


    #region("Player Move Phase")
    void PlayerReader()
    {
        CalculatePlayerPoints();
        if (IsPlayerOver21())
        {
            state = States.Fighting;
        }
        else
        {
            if (8==9)//IsEndMarkerOnTable)
            {
                state = States.AiMove;
            }
            else
            {
                // DO NOTHING
            }

        }
    }

    #endregion


    #region("Ai Move Phase")
    void AiAlgoithm()
    {
        //ShowCard(1); TODO make me work
        if (IsAiOver21())
        {
            state = States.Fighting;
        }
        else
        {
            if (aiScore < 17)
            {
                DrawCard();
            }
            else
            {
                state = States.Fighting;
            }

        }
    }


    void CalculateAiPoints()
    {
        aiScore = 0;
        foreach (var card in aiHand)
        {
            aiScore += card;
        }

        if (IsAiOver21())
        {
            foreach (var card in aiHand)
            {
                if (card == 11)
                    aiScore -= 10;
                if (aiScore < 22) break;
            }
        }
    }


    #endregion


    #region("Fighting Phase")
    void CalculatePlayerPoints()
    {
        playerScore = 0;
        foreach (int card in playerHand)
        {
            playerScore += card;
        }

        if (IsPlayerOver21())
        {
            foreach (int card in playerHand)
            {
                if (card == 11)
                    playerScore -= 10;
                if (playerScore < 22) break;
            }
        }
    }

    bool PlayerWon()
    {
        if (IsAiOver21()) return true;
        if (IsPlayerOver21()) return false;
        if (playerScore > aiScore) return true;
        else return false;
    }


    #endregion


    #region ("Logic for calculation")


    void AnnounceTie()
    {
        print("tie!");
    }

    void AnnounceWinner(int player)
    {
        if (player == 0)
        {
            print("Player wins");
        }

        if (player == 1)
        {
            print("AI wins");
        }
    }

    bool IsPlayerOver21()
    {
        if (playerScore > 21) return true;
        else return false;
    }

    bool IsAiOver21()
    {
        if (aiScore > 21) return true;
        else return false;
    }

    #endregion



    public void IncreasePlayerScore(int cardValue)
    {
        playerScore += cardValue;
    }

    public void ChangeState(int newState)
    {
        state = States.Fighting;
    }

    public int GetPlayerBalance()
    {
        return playerBalance;
    }

    public int GetEnemyScore()
    {
        return aiScore;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetCurrentStage()
    {
        switch (state)
        {
            case States.NotStarted:
                return 0;

            case States.Beting:
                return 1;

            case States.SetingUp:
                return 2;

            case States.PlayerSetup:
                return 3;

            case States.PlayerMove:
                return 4;

            case States.AiMove:
                return 5;

            case States.Fighting:
                return 6;

            default:
                return -1;
        }
    }


}
