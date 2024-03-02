using System;
using UnityEngine;

public class BuildTetrisController : MonoBehaviour
{
    public static BuildTetrisController Instance { get; private set; }

    #region Events

    public event EventHandler OnBuildingTetrisBoardClose;
    public event EventHandler OnBuildingTetrisGameStart;

    #endregion

    #region variables & References

    [SerializeField] private BuildingTetrisBoard buildingTetrisBoard;

    private bool isFirstUpdate = true;

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
        ChooseMinigameUI.OnPlayBuildingTetrisButtonPressed += ChooseMinigameUIOnPlayBuildingBuildTetrisButtonPressed;
        BuildingTetrisUI.OnTetrisGameClose += BuildingTetrisUI_OnBuildingTetrisGameClose;

        buildingTetrisBoard.OnGameOver += BuildingTetrisBoard_OnGameOver;
    }

    private void BuildingTetrisUI_OnBuildingTetrisGameClose(object sender, EventArgs e)
    {
        Hide();
    }

    private void BuildingTetrisBoard_OnGameOver(object sender, EventArgs e)
    {
        Hide();
        OnBuildingTetrisBoardClose?.Invoke(this, EventArgs.Empty);
    }

    private void ChooseMinigameUIOnPlayBuildingBuildTetrisButtonPressed(object sender, EventArgs e)
    {
        Debug.Log("Work");
        Show();
        OnBuildingTetrisGameStart?.Invoke(this, EventArgs.Empty);
    }

    private void LateUpdate()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Hide();
        }
    }

    #endregion

    #region Visual

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
