using System;
using UnityEngine;

public class SuikaTetrisController : MonoBehaviour
{
    public static SuikaTetrisController Instance { get; private set; }

    #region Events

    public event EventHandler OnSuikaTetrisBoardClose;
    public event EventHandler OnSuikaTetrisGameStart;

    #endregion

    #region variables & References

    [SerializeField] private SuikaTetrisBoard suikaTetrisBoard;

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
    }

    private void ChooseMinigameUIOnPlaySuikaTetrisButtonPressed(object sender, EventArgs e)
    {
        Show();
        OnSuikaTetrisGameStart?.Invoke(this, EventArgs.Empty);
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
