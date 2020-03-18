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
    public List<int> playerHand = new List<int>();

    [Header("Current state")]
    [SerializeField] States state;

    [Header("Buttons")]
    [SerializeField] GameObject betButtons;
    [SerializeField] GameObject startGameButtons;

    [Header("Canvas")]
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] Canvas gameCanvas;

    [Header("Ai cards holders")]
    [SerializeField] GameObject holder1;
    [SerializeField] GameObject holder2;

    void Start()
    {
        playerBalance = 200;
        state = States.NotStarted;
        PutAllCardsInAiDeck();
    }

    void Update()
    {
        switch (state)
        {
            case States.NotStarted:
                betButtons.SetActive(false);
                startGameButtons.SetActive(true);
                break;

            case States.Beting:
                betButtons.SetActive(true);
                startGameButtons.SetActive(false);
                break;

            case States.SetingUp:
                if (aiHand.Count == 0)
                {
                    betButtons.SetActive(false);
                    GiveCardsToAi();
                    StartCoroutine(WaitBeforePlayerSetupPhase(1));
                }
                break;

            case States.PlayerSetup:
                if (TwoCardsOnTable()) StartCoroutine(WaitBeforePlayerMovePhase(2)); 
                else ShowMessagePut2CardsOnTable();
                break;

            case States.PlayerMove:
                PlayerReader();
                break;

            case States.AiMove:
                AiAlgoithm();
                Debug.Log("Stage 5");
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
                PutAllCardsInAiDeck();
                state = States.Beting;
                Debug.Log("Stage 6");
                break;
        }
    }

    #region BUTTONS
    public void StartGame()
    {
        bet = 0;
        betProposition = 0;
        aiScore = 0;
        playerScore = 0;
        mainMenuCanvas.enabled = false;
        gameCanvas.enabled = true;
        state = States.Beting;
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    #endregion //BUTTONS


    #region("Initial Parameters")
    void PutAllCardsInAiDeck()
    {
        currentDeck.Clear();
        aiHand.Clear();
        playerHand.Clear();
        for (int i = 0; i < 52; i++)
        {
            currentDeck.Add(i);
        }
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


    #region SETTING UP PHASE
    void GiveCardsToAi()
    {
        DrawCard();
        DrawCard();
        ShowCard1(aiHand[0]);
        ShowCard2(aiHand[1]);
    }

    void DrawCard()
    {
        int newCard = Random.Range(0, currentDeck.Count);
        aiHand.Add(newCard);
        currentDeck.Remove(newCard);
        CalculateAiPoints();
    }

    void ShowCard1(int cardIndex)
    {
        var card = Instantiate(allDeck[cardIndex],holder1.transform.position,Quaternion.identity,holder1.transform);
        card.transform.eulerAngles = new Vector3(card.transform.eulerAngles.x - 90, 0, 180);
    }

    void ShowCard2(int cardIndex)
    {
        var card = Instantiate(allDeck[cardIndex], holder2.transform.position, Quaternion.identity, holder2.transform);
        card.transform.eulerAngles = new Vector3(card.transform.eulerAngles.x - 90, 0, 180);
    }

    IEnumerator WaitBeforePlayerSetupPhase(float time)
    {
        yield return new WaitForSeconds(time);
        state = States.PlayerSetup;
    }



    #endregion


    #region("Player Setup Phase")
    bool TwoCardsOnTable()
    {
        if (playerScore > 0)
        {
            return true ;
        }
        else
        {
            ShowMessagePut2CardsOnTable();
            return false;
        }

    }

    void ShowMessagePut2CardsOnTable()
    {
        Debug.Log("Put 2 card on table");
        //text.text put those cards;
    }


    IEnumerator WaitBeforePlayerMovePhase(float time)
    {
        yield return new WaitForSeconds(time);
        state = States.PlayerMove;
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
            aiScore += allDeck[card].GetComponent<AiCardsValues>().GetCardValue();
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
        Debug.Log("Player score:" + playerScore);
    }

    public void ChangeState(int newState)
    {
        state = States.Fighting;
    }

    public int GetPlayerBalance()
    {
        return playerBalance;
    }

    public int GetPlayerBetProposition()
    {
        return betProposition;
    }

    public int GetPlayerBet()
    {
        return bet;
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
