using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GIUManager : MonoBehaviour
{
    GameMaster gameMaster;
    [SerializeField] TextMeshProUGUI balanceText;
    [SerializeField] TextMeshProUGUI betText;
    [SerializeField] TextMeshProUGUI betPropositionText;
    [SerializeField] TextMeshProUGUI enemyScoreText;
    [SerializeField] TextMeshProUGUI playerScoreText;


    private void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
    }

    void Update()
    {
        balanceText.text = gameMaster.GetPlayerBalance().ToString();
        enemyScoreText.text = gameMaster.GetEnemyScore().ToString();
        playerScoreText.text = gameMaster.GetPlayerScore().ToString();

        if (gameMaster.GetCurrentStage() == 1)
        {
            betText.text = gameMaster.GetPlayerBetProposition().ToString();
        }
        else
        {
            betText.text = gameMaster.GetPlayerBet().ToString();
        }
    }


}
