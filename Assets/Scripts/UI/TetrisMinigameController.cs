using System;
using UnityEngine;
using UnityEngine.UI;

public class TetrisMinigameController : MonoBehaviour
{
    public static event EventHandler OnTetrisGameStart;
    public static event EventHandler OnTetrisGameClose;
    public static event EventHandler OnRotatePieceE;
    public static event EventHandler OnRotatePieceQ;
    public static event EventHandler OnDropPiece;
    public static event EventHandler OnMovePieceRight;
    public static event EventHandler OnMovePieceLeft;
    public static event EventHandler OnMovePieceDown;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button rotatePieceEButton;
    [SerializeField] private Button rotatePieceQButton;
    [SerializeField] private Button dropPieceButton;
    [SerializeField] private Button movePieceRightButton;
    [SerializeField] private Button movePieceLeftButton;
    [SerializeField] private Button movePieceDownButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            OnTetrisGameClose?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        rotatePieceEButton.onClick.AddListener(() => { OnRotatePieceE?.Invoke(this, EventArgs.Empty); });
        rotatePieceQButton.onClick.AddListener(() => { OnRotatePieceQ?.Invoke(this, EventArgs.Empty); });
        dropPieceButton.onClick.AddListener(() => { OnDropPiece?.Invoke(this, EventArgs.Empty); });
        movePieceRightButton.onClick.AddListener(() => { OnMovePieceRight?.Invoke(this, EventArgs.Empty); });
        movePieceLeftButton.onClick.AddListener(() => { OnMovePieceLeft?.Invoke(this, EventArgs.Empty); });
        movePieceDownButton.onClick.AddListener(() => { OnMovePieceDown?.Invoke(this, EventArgs.Empty); });
    }

    private void Start()
    {
        ChooseMinigameUI.OnPlayTetrisButtonPressed += ChooseMinigameUI_OnPlayTetrisButtonPressed;

        TetrisBoardBasic.OnGameOver += BuildingTetrisBoard_OnGameOver;

        Hide();
    }

    private void BuildingTetrisBoard_OnGameOver(object sender, EventArgs e)
    {
        OnTetrisGameClose?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void ChooseMinigameUI_OnPlayTetrisButtonPressed(object sender, EventArgs e)
    {
        Show();
        OnTetrisGameStart?.Invoke(this, EventArgs.Empty);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
