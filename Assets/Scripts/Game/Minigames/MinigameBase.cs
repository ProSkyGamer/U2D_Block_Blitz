using System;
using UnityEngine;

public class MinigameBase : MonoBehaviour
{
    #region Enums

    public enum GameOverReason
    {
        TimeOut,
        Failed,
        Win
    }

    #endregion

    #region Events

    public event EventHandler<OnGameOverEventArgs> OnGameOver;

    public class OnGameOverEventArgs : EventArgs
    {
        public GameOverReason gameOverReason;
        public int completionTime;
    }

    public event EventHandler OnGameStarted;

    public event EventHandler<OnTimerChangedEventArgs> OnTimerChanged;

    public class OnTimerChangedEventArgs : EventArgs
    {
        public int newTimeInt;
    }

    #endregion

    #region Variables & References

    [SerializeField] protected float maxGameTime = 60f;
    [SerializeField] protected bool isWinPossibleAfterTimerEnds;
    [SerializeField] protected TetrisBoardBasic tetrisBoard;

    private float gameTimer;
    private int previousGameTimeInt;
    protected bool isGameStarted;

    private bool isFirstUpdate = true;

    #endregion

    #region Start Game

    protected virtual void StartGame()
    {
        ShowMinigame();
        OnGameStarted?.Invoke(this, EventArgs.Empty);

        gameTimer = maxGameTime;
        previousGameTimeInt = (int)maxGameTime;

        isGameStarted = true;
    }

    protected void EndGame(GameOverReason gameOverReason)
    {
        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            gameOverReason = gameOverReason, completionTime = (int)(maxGameTime - gameTimer)
        });

        isGameStarted = false;
    }

    #endregion

    #region Update

    private void Update()
    {
        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0f)
        {
            OnGameOver?.Invoke(this, new OnGameOverEventArgs
            {
                gameOverReason = isWinPossibleAfterTimerEnds
                    ? tetrisBoard.GetGameResult() ? GameOverReason.Win : GameOverReason.TimeOut
                    : GameOverReason.TimeOut,
                completionTime = (int)(maxGameTime - gameTimer)
            });
            isGameStarted = false;
            return;
        }

        if (previousGameTimeInt > (int)gameTimer)
        {
            previousGameTimeInt = (int)gameTimer;
            OnTimerChanged?.Invoke(this, new OnTimerChangedEventArgs
            {
                newTimeInt = previousGameTimeInt
            });
        }
    }

    private void LateUpdate()
    {
        if (isFirstUpdate)
        {
            MinigameResultMessagesUI.OnMessageClosed += MinigameResultMessagesUI_OnMessageClosed;

            isFirstUpdate = false;
            HideMinigame();
        }
    }

    private void MinigameResultMessagesUI_OnMessageClosed(object sender, EventArgs e)
    {
        HideMinigame();
    }

    #endregion

    #region Visual

    private void ShowMinigame()
    {
        gameObject.SetActive(true);
    }

    protected void HideMinigame()
    {
        gameObject.SetActive(false);
    }

    #endregion

    public float GetMaxGameTime()
    {
        return maxGameTime;
    }
}
