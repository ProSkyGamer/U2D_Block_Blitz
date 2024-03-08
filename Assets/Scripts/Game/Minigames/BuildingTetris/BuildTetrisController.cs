using System;
using UnityEngine;

public class BuildTetrisController : MonoBehaviour
{
    public static BuildTetrisController Instance { get; private set; }

    #region Events

    public event EventHandler OnBuildingTetrisBoardClose;
    public event EventHandler OnBuildingTetrisGameStart;
    public event EventHandler<OnTimerChangedEventArgs> OnTimerChanged;

    public class OnTimerChangedEventArgs : EventArgs
    {
        public int newTimeInt;
    }

    #endregion

    #region variables & References

    [SerializeField] private BuildingTetrisBoard buildingTetrisBoard;

    [SerializeField] private float maxGameTime = 60f;

    private float gameTimer;
    private int previousGameTimeInt;
    private bool isGameStarted;

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

        isGameStarted = false;
    }

    private void BuildingTetrisBoard_OnGameOver(object sender, EventArgs e)
    {
        Hide();
        OnBuildingTetrisBoardClose?.Invoke(this, EventArgs.Empty);
    }

    private void ChooseMinigameUIOnPlayBuildingBuildTetrisButtonPressed(object sender, EventArgs e)
    {
        Show();
        OnBuildingTetrisGameStart?.Invoke(this, EventArgs.Empty);

        gameTimer = maxGameTime;
        previousGameTimeInt = (int)maxGameTime;

        isGameStarted = true;
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

    #region Update

    private void Update()
    {
        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0f)
        {
            Hide();
            OnBuildingTetrisBoardClose?.Invoke(this, EventArgs.Empty);
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

    public float GetMaxGameTime()
    {
        return maxGameTime;
    }
}
