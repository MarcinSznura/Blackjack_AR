using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GameMaster : MonoBehaviour
{
    enum States { NotStarted, Beting, SetingUp, PlayerSetup, PlayerMove, AiMove, Fighting };

    [Header("Parameters")]
    [SerializeField] int playerScore;
    [SerializeField] int aiScore;
    [SerializeField] int playerBalance;
    [SerializeField] int betProposition;
    [SerializeField] int bet;
    [SerializeField] bool aiPlaying = false;
    [SerializeField] bool waitingForLastRoundToEnd = false;
    [SerializeField] bool jokerAppeared = false;

    [Header("Decks and Hands")]
    [SerializeField] List<GameObject> allDeck = new List<GameObject>();
    [SerializeField] List<int> currentDeck = new List<int>();
    [SerializeField] List<int> aiHand = new List<int>();
    public List<int> playerHand = new List<int>();

    [Header("Current state")]
    [SerializeField] States state;

    [Header("Buttons")]
    [SerializeField] GameObject betButtons;

    [Header("Canvas")]
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] Canvas gameCanvas;

    [Header("Win/Lose info")]
    [SerializeField] GameObject playerWinInfo;
    [SerializeField] GameObject playerLoseInfo;
    [SerializeField] GameObject playerTieInfo;
    [SerializeField] GameObject aiWinInfo;
    [SerializeField] GameObject aiLoseInfo;
    [SerializeField] GameObject aiTieInfo;

    void Start()
    {
        mainMenuCanvas.enabled = true;
        gameCanvas.enabled = false;
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
                break;

            case States.Beting:
                betButtons.SetActive(true);
                waitingForLastRoundToEnd = false;
                break;

            case States.SetingUp:
                aiScore = 0;
                playerScore = 0;
                HideAnnouncements();
                jokerAppeared = false;
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
                if (!aiPlaying)
                {
                    aiPlaying = true;
                    StartCoroutine(AiAlgoithm(2));
                    Debug.Log("Stage 5");
                }
                break;
                

            case States.Fighting:
                if (!waitingForLastRoundToEnd)
                {
                    waitingForLastRoundToEnd = true;
                    if (PlayerWon())
                    {
                        AnnounceWinner(0);
                        playerBalance += 2 * bet;
                    }
                    else
                    {
                        if (playerScore == aiScore)
                        {
                            AnnounceTie();
                            playerBalance += bet;
                        }
                        else AnnounceWinner(1);
                    }

                    Validator[] validators = FindObjectsOfType<Validator>();
                    foreach (var validator in validators)
                    {
                        validator.RestartCountedCards();
                    }
                    
                    PutAllCardsInAiDeck();
                    DiscardAiCards();
                    StartCoroutine(WaitBeforeChangingPhase(2f, States.Beting));

                }
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
        ShowCard(aiHand[0],0);
        ShowCard(aiHand[1],1);
    }

    void DrawCard()
    {
        int newCard = Random.Range(0, currentDeck.Count);
        aiHand.Add(newCard);
        currentDeck.Remove(newCard);
        CalculateAiPoints();
    }

    void DiscardAiCards()
    {
        var cards = GameObject.FindGameObjectsWithTag("AiCards");
        foreach (var card in cards)
        {
            Destroy(card);
        }
    }

    void ShowCard(int cardIndex,int handIndex)
    {
        Instantiate(allDeck[cardIndex], new Vector2(200 + 250 * handIndex, 600), Quaternion.identity, gameCanvas.transform);
    }

    IEnumerator WaitBeforePlayerSetupPhase(float time)
    {
        yield return new WaitForSeconds(time);
        state = States.PlayerSetup;
    }

    IEnumerator WaitBeforeChangingPhase(float time, States nextState)
    {
        yield return new WaitForSeconds(time);
        state = nextState;
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
            if (jokerAppeared)
            {
                state = States.AiMove;
            }
            ShowMessagePut2CardsOnTable();
            return false;
        }

    }

    void ShowMessagePut2CardsOnTable()
    {
        //Debug.Log("Put 2 card on table");
        //text.text put those cards;
    }


    IEnumerator WaitBeforePlayerMovePhase(float time)
    {
        yield return new WaitForSeconds(time);
        state = States.PlayerMove;
    }

    #endregion


    #region PLAYER MOVE PHASE
    void PlayerReader()
    {
        CalculatePlayerPoints();
        if (IsPlayerOver21())
        {
            state = States.AiMove;
        }
        if (jokerAppeared)
        {
            state = States.AiMove;
        }
    }

    public void SetJokerAppeared(bool joker)
    {
        jokerAppeared = joker;
    }

    #endregion


    #region AI MOVE PHASE
    IEnumerator AiAlgoithm (float time)
    {
        CalculateAiPoints();
        //ShowCard(1); TODO make me work
        print("Corutine ai start");
        if (IsAiOver21())
        {
            aiPlaying = false;
            state = States.Fighting;
            print("Corutine ai end");
        }
        else
        {
            if (aiScore < 17)
            {
                yield return new WaitForSeconds(time);
                DrawCard();
                ShowCard(aiHand[aiHand.Count-1],aiHand.Count - 1);
                aiPlaying = false;
            }
            else
            {
                yield return new WaitForSeconds(time);
                aiPlaying = false;
                state = States.Fighting;
                print("Corutine ai end");
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
        aiTieInfo.SetActive(true);
        playerTieInfo.SetActive(true);
    }

    void AnnounceWinner(int player)
    {
        if (player == 0)
        {
            print("Player wins");
            playerWinInfo.SetActive(true);
            aiWinInfo.SetActive(false);

            playerLoseInfo.SetActive(false);
            aiLoseInfo.SetActive(true);
        }

        if (player == 1)
        {
            print("AI wins");
            aiWinInfo.SetActive(true);
            playerWinInfo.SetActive(false);

            playerLoseInfo.SetActive(true);
            aiLoseInfo.SetActive(false);
        }
    }

    void HideAnnouncements()
    {
        aiWinInfo.SetActive(false);
        playerWinInfo.SetActive(false);
        playerLoseInfo.SetActive(false);
        aiLoseInfo.SetActive(false);
        aiTieInfo.SetActive(false);
        playerTieInfo.SetActive(false);
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
        state = States.AiMove;
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
