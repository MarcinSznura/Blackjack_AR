using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GIUManager : MonoBehaviour
{
    GameMaster gameMaster;
    [SerializeField] TextMeshProUGUI balanceText;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] TextMeshProUGUI enemyScoreText;
    [SerializeField] TextMeshProUGUI playerScoreText;


    private void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
    }

    void Update()
    {
        balanceText.text = "Balance: " + gameMaster.GetPlayerBalance().ToString();
        stageText.text = "Stage: " + gameMaster.GetCurrentStage().ToString();
        enemyScoreText.text = "Ai score: " + gameMaster.GetEnemyScore().ToString();
        playerScoreText.text = "Player score: " + gameMaster.GetPlayerScore().ToString();
    }


}
