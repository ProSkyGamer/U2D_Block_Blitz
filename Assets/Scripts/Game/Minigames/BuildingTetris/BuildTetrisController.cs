using System;
using UnityEngine;

public class BuildTetrisController : MinigameBase
{
    public static BuildTetrisController Instance { get; private set; }

    #region variables & References

    [SerializeField] private BuildingTetrisBoard buildingTetrisBoard;

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
        ChooseMinigameUI.OnPlayBuildingTetrisButtonPressed += ChooseMinigameUI_OnPlayBuildingTetrisButtonPressed;

        BuildingTetrisUI.OnCloseButtonPressed += BuildingTetrisUIOnBuildingCloseButtonPressed;
        buildingTetrisBoard.OnGameOver += BuildingTetrisBoard_OnGameOver;
    }

    private void BuildingTetrisBoard_OnGameOver(object sender, TetrisBoardBasic.OnGameOverEventArgs e)
    {
        EndGame(e.isWin ? GameOverReason.Win : GameOverReason.Failed);
    }

    private void BuildingTetrisUIOnBuildingCloseButtonPressed(object sender, EventArgs e)
    {
        HideMinigame();

        isGameStarted = false;
    }

    private void ChooseMinigameUI_OnPlayBuildingTetrisButtonPressed(object sender, EventArgs e)
    {
        StartGame();
    }

    #endregion
}
