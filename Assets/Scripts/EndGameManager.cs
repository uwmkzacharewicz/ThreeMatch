using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}


public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    public GameObject movesLabel;
    public GameObject timeLabel;

    public GameObject youWinPanel;
    public GameObject tryAgainPanel;

    public TextMeshProUGUI counter;



    private Board board;

    public int currentCounterValue;
    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetupGame();
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + requirements.counterValue;
        currentCounterValue = requirements.counterValue;
    }

    public void DecreaseCounterValue()
    {
        if(board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                tryAgainPanel.SetActive(true);
                
                board.currentState = GameState.lose;
                Debug.Log("You lose");
                currentCounterValue = 0;
                counter.text = "" + currentCounterValue;
                //LoseGame();
            }

        }

    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        Debug.Log("You win");
        
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        if (fadePanel != null)
        {
            fadePanel.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
