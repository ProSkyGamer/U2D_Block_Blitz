using System;
using UnityEngine;

public class SuikaTetrisController : MinigameBase
{
    public static SuikaTetrisController Instance { get; private set; }

    #region variables & References

    [SerializeField] private SuikaTetrisBoard suikaTetrisBoard;

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        ChooseMinigameUI.OnPlaySuikaTetrisButtonPressed += ChooseMinigameUIOnPlaySuikaTetrisButtonPressed;
        SuikaTetrisUI.OnCloseButtonPressed += SuikaTetrisUIOnCloseButtonPressed;

        suikaTetrisBoard.OnGameOver += SuikaTetrisBoard_OnGameOver;
    }

    private void SuikaTetrisBoard_OnGameOver(object sender, TetrisBoardBasic.OnGameOverEventArgs e)
    {
        EndGame(e.isWin ? GameOverReason.Win : GameOverReason.Failed);
    }

    private void SuikaTetrisUIOnCloseButtonPressed(object sender, EventArgs e)
    {
        HideMinigame();

        isGameStarted = false;
    }

    private void ChooseMinigameUIOnPlaySuikaTetrisButtonPressed(object sender, EventArgs e)
    {
        StartGame();
    }

    #endregion
}
