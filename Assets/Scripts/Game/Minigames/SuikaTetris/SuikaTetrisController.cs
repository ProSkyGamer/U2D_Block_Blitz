using System;
using UnityEngine;

public class SuikaTetrisController : MonoBehaviour
{
    public static SuikaTetrisController Instance { get; private set; }

    #region Events

    public event EventHandler OnSuikaTetrisBoardClose;
    public event EventHandler OnSuikaTetrisGameStart;

    public event EventHandler<OnTimerChangedEventArgs> OnTimerChanged;

    public class OnTimerChangedEventArgs : EventArgs
    {
        public int newTimeInt;
    }

    #endregion

    #region variables & References

    [SerializeField] private SuikaTetrisBoard suikaTetrisBoard;

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
        ChooseMinigameUI.OnPlaySuikaTetrisButtonPressed += ChooseMinigameUIOnPlaySuikaTetrisButtonPressed;
        SuikaTetrisUI.OnTetrisGameClose += SuikaTetrisUI_OnTetrisGameClose;

        suikaTetrisBoard.OnGameOver += SuikaTetrisBoard_OnGameOver;
    }

    private void SuikaTetrisBoard_OnGameOver(object sender, EventArgs e)
    {
        Hide();
        OnSuikaTetrisBoardClose?.Invoke(this, EventArgs.Empty);
    }

    private void SuikaTetrisUI_OnTetrisGameClose(object sender, EventArgs e)
    {
        Hide();

        isGameStarted = false;
    }

    private void ChooseMinigameUIOnPlaySuikaTetrisButtonPressed(object sender, EventArgs e)
    {
        Show();
        OnSuikaTetrisGameStart?.Invoke(this, EventArgs.Empty);

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
            OnSuikaTetrisBoardClose?.Invoke(this, EventArgs.Empty);
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
