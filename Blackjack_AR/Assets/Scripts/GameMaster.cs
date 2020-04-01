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
    [SerializeField] bool aiPlaying = false;
    [SerializeField] bool waitingForLastRoundToEnd = false;
    [SerializeField] bool jokerAppeared = false;
    public string textInfo = "";

    [Header("Decks and Hands")]
    [SerializeField] List<GameObject> allDeck = new List<GameObject>();
    [SerializeField] List<int> currentDeck = new List<int>();
    [SerializeField] List<int> aiHand = new List<int>();
    public List<int> playerHand = new List<int>();
    [SerializeField] GameObject hidenCardPrefab;

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

    [Header("Spawn Position")]
    [SerializeField] GameObject spawnPosition1;

    void Start()
    {
        mainMenuCanvas.enabled = true;
        gameCanvas.enabled = false;
        playerBalance = 200;
        state = States.NotStarted;
        //PutAllCardsInAiDeck();
    }

    void Update()
    {
        switch (state)
        {
            case States.NotStarted:
                betButtons.SetActive(false);
                break;

            case States.Beting:
                PutAllCardsInAiDeck();
                betButtons.SetActive(true);
                waitingForLastRoundToEnd = false;
                break;

            case States.SetingUp:
                jokerAppeared = false;
                if (aiHand.Count == 0)
                {
                    aiScore = 0;
                    playerScore = 0;
                    HideAnnouncements();
                    betButtons.SetActive(false);
                    GiveStartCardsToAi();
                    aiScore = allDeck[aiHand[1]].GetComponent<AiCardsValues>().GetCardValue();
                    StartCoroutine(WaitBeforeChangingPhase(1f,States.PlayerSetup));
                }
                break;

            case States.PlayerSetup:
                if (CardsOnTable())
                {
                    textInfo = "";
                    StartCoroutine(WaitBeforeChangingPhase(0, States.PlayerMove));
                }
                else textInfo = "Put cards on table";

                if (jokerAppeared)
                {
                    textInfo = "";
                    state = States.AiMove;
                }
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
                    
                    StartCoroutine(WaitBeforeChangingPhase(1f, States.Beting));

                }
                break;
        }
    }


    #region STATES HANDELER
    IEnumerator WaitBeforeChangingPhase(float time, States nextState)
    {
        yield return new WaitForSeconds(time);
        state = nextState;
    }

    #endregion //STATES HANDELER

    #region AI CARDS DEALER
    void PutAllCardsInAiDeck()
    {
        currentDeck.Clear();
        for (int i = 0; i < 52; i++)
        {
            currentDeck.Add(i);
        }
    }

    void GiveStartCardsToAi()
    {
        SpawnHidenCard(0);
        DrawCard();
        DrawCard();
        ShowCard(aiHand[1], 1);
    }

    void DrawCard()
    {
        int newCard = Random.Range(0, currentDeck.Count);
        aiHand.Add(newCard);
        currentDeck.Remove(newCard);
        CalculateAiPoints();
    }

    void RemoveAiCardsFromScreen()
    {
        var cards = GameObject.FindGameObjectsWithTag("AiCards");
        foreach (var card in cards)
        {
            Destroy(card);
        }
    }

    void SpawnHidenCard(int handIndex)
    {
            Instantiate(hidenCardPrefab,
                new Vector2(spawnPosition1.transform.position.x + (200 * handIndex), spawnPosition1.transform.position.y),
                Quaternion.identity, gameCanvas.transform);
    }

    void ShowCard(int cardIndex, int handIndex)
    {
        if (handIndex < 4)
        {
            Instantiate(allDeck[cardIndex],
                new Vector2(spawnPosition1.transform.position.x + (200 * handIndex), spawnPosition1.transform.position.y),
                Quaternion.identity, gameCanvas.transform);
        }
        else
        {
            Instantiate(allDeck[cardIndex],
               new Vector2(spawnPosition1.transform.position.x + (200 * (handIndex-4)), spawnPosition1.transform.position.y-300),
               Quaternion.identity, gameCanvas.transform);
        }
    }

    void ClearHands()
    {
        aiHand.Clear();
        playerHand.Clear();
    }


    #endregion //AI CARDS DEALER

    #region BEATING/BUTTONS

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
        if (betProposition + 500 <= playerBalance)
        {
            betProposition += 500;
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
        betProposition -= 500;
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

            RemoveAiCardsFromScreen();
            ClearHands();
        }
    }

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

    public void OpenSettings()
    {
        Debug.Log("Do it!");
    }


    #endregion // BEATING + BUTTONS

    #region PLAYER SETUP PHASE
    bool CardsOnTable()
    {
        if (playerHand.Count > 0)
        {
            return true ;
        }
        else
        {
            return false;
        }
    }
    #endregion //PLAYER SETUP PHASE

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
        if (playerScore == 21)
        {
            state = States.AiMove;
        }
    }

    public void SetJokerAppeared(bool joker)
    {
        jokerAppeared = joker;
    }

    #endregion //PLAYER MOVE PHASE

    #region AI MOVE PHASE
    IEnumerator AiAlgoithm (float time)
    {
        ShowCard(aiHand[0],0);
        CalculateAiPoints();
        print("Corutine ai start");
        if (IsAiOver21())
        {
            aiPlaying = false;
            state = States.Fighting;
        }
        else
        {
            if (!WillAiWinNow())
            {
                if (aiScore == playerScore && aiScore < 17)
                {
                    yield return new WaitForSeconds(time);
                    aiPlaying = false;
                    state = States.Fighting;
                }
                else
                {
                    yield return new WaitForSeconds(time);
                    DrawCard();
                    ShowCard(aiHand[aiHand.Count - 1], aiHand.Count - 1);
                    aiPlaying = false;
                }
            }
            else
            {
                yield return new WaitForSeconds(time);
                aiPlaying = false;
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
                if (allDeck[card].GetComponent<AiCardsValues>().GetCardValue() == 11)
                    aiScore -= 10;
                if (aiScore < 22) break;
            }
        }
    }
    #endregion

    #region FIGHTING PHASE
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
    #endregion //FIGHTING PHASE

    #region SCORE SUMMARY 
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

    private bool WillAiWinNow()
    {
        if (IsPlayerOver21() || aiScore == 21)
        {
            return true;
        }
        else
        {
            if (aiScore > playerScore) return true;
            else return false;
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

    #endregion //SCORE SUMMARY 

    #region PUBLIC GETS

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

    #endregion //PUBLIC GETS
}
