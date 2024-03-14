using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuikaTetrisUI : MonoBehaviour
{
    public static event EventHandler OnTetrisGameStart;
    public static event EventHandler OnCloseButtonPressed;
    public static event EventHandler OnRotatePieceE;
    public static event EventHandler OnRotatePieceQ;
    public static event EventHandler OnDropPiece;
    public static event EventHandler OnMovePieceRight;
    public static event EventHandler OnMovePieceLeft;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button rotatePieceEButton;
    [SerializeField] private Button rotatePieceQButton;
    [SerializeField] private Button dropPieceButton;
    [SerializeField] private Button movePieceRightButton;
    [SerializeField] private Button movePieceLeftButton;

    [SerializeField] private Transform timerTilesGrid;
    [SerializeField] private Transform timerTilePrefab;
    private readonly List<Image> allTimerTiles = new();
    [SerializeField] private List<Sprite> timerTileSpritesPrefabs;
    private SameGameSingleTile[,] tilesField;

    private float gameTimer;
    private int previousGameTimeInt;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            OnCloseButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        rotatePieceEButton.onClick.AddListener(() => { OnRotatePieceE?.Invoke(this, EventArgs.Empty); });
        rotatePieceQButton.onClick.AddListener(() => { OnRotatePieceQ?.Invoke(this, EventArgs.Empty); });
        dropPieceButton.onClick.AddListener(() => { OnDropPiece?.Invoke(this, EventArgs.Empty); });
        movePieceRightButton.onClick.AddListener(() => { OnMovePieceRight?.Invoke(this, EventArgs.Empty); });
        movePieceLeftButton.onClick.AddListener(() => { OnMovePieceLeft?.Invoke(this, EventArgs.Empty); });
    }

    private void Start()
    {
        ChooseMinigameUI.OnPlaySuikaTetrisButtonPressed += ChooseMinigameUIOnPlaySuikaTetrisButtonPressed;

        MinigameResultMessagesUI.OnMessageClosed += MinigameResultMessagesUI_OnMessageClosed;

        SuikaTetrisController.Instance.OnTimerChanged += SuikaTetrisController_OnTimerChanged;

        Hide();
    }

    private void MinigameResultMessagesUI_OnMessageClosed(object sender, EventArgs e)
    {
        OnCloseButtonPressed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void SuikaTetrisController_OnTimerChanged(object sender, MinigameBase.OnTimerChangedEventArgs e)
    {
        ChangeTimerTiles(e.newTimeInt);
    }

    private void InitializeTimerTiles()
    {
        var timerTilesTransform = timerTilesGrid.GetComponentsInChildren<Transform>();

        foreach (var timerTileTransform in timerTilesTransform)
        {
            if (timerTileTransform == timerTilesGrid) continue;

            Destroy(timerTileTransform.gameObject);
        }

        allTimerTiles.Clear();

        var timerTilesCount =
            Mathf.CeilToInt(SuikaTetrisController.Instance.GetMaxGameTime() / timerTileSpritesPrefabs.Count);

        for (var i = 0; i < timerTilesCount; i++)
        {
            var newTimerTileTransform = Instantiate(timerTilePrefab, timerTilesGrid);
            var newTimerTileImage = newTimerTileTransform.GetComponent<Image>();
            allTimerTiles.Add(newTimerTileImage);
        }
    }

    private void ChangeTimerTiles(int newGameTimerInt)
    {
        var currentTileInt = newGameTimerInt % timerTileSpritesPrefabs.Count;
        var currentTileSprite = timerTileSpritesPrefabs[currentTileInt];
        if (currentTileInt == 0)
        {
            Destroy(allTimerTiles[0]);
            allTimerTiles.RemoveAt(0);
        }

        if (allTimerTiles.Count == 0) return;

        var currentTopTimerTile = allTimerTiles[0];

        currentTopTimerTile.sprite = currentTileSprite;
    }

    private void ChooseMinigameUIOnPlaySuikaTetrisButtonPressed(object sender, EventArgs e)
    {
        Show();
        OnTetrisGameStart?.Invoke(this, EventArgs.Empty);
        InitializeTimerTiles();
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
