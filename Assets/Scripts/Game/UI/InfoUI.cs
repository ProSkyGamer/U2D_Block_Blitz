using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    #region Events

    public static event EventHandler OnTabClosed;

    #endregion

    #region Enums

    private enum TabTypes
    {
        GameInfo,
        GameControls,
        GameRecords
    }

    private enum GameTabTypes
    {
        SameGame,
        BuildingTetris,
        SuikaTetris
    }

    #endregion

    #region Variables & References

    [SerializeField] private Button closeButton;

    [SerializeField] private Button gameInfoTabButton;
    private Image gameInfoTabButtonImage;
    [SerializeField] private Button gameControlsTabButton;
    private Image gameControlsTabButtonImage;
    [SerializeField] private Button gameRecordsTabButton;
    private Image gameRecordsTabButtonImage;

    [SerializeField] private Button sameGameTabButton;
    private Image sameGameTabButtonImage;
    [SerializeField] private Button buildingTetrisTabButton;
    private Image buildingTetrisTabButtonImage;
    [SerializeField] private Button suikaTetrisTabButton;
    private Image suikaTetrisTabButtonImage;

    [SerializeField] private Sprite activeTabButtonSprite;
    [SerializeField] private Sprite notActiveTabButtonSprite;

    [SerializeField] private Transform gameInfoSameGameTabTransform;
    [SerializeField] private Transform gameInfoBuildingTetrisTabTransform;
    [SerializeField] private Transform gameInfoSuikaTetrisTabTransform;
    [SerializeField] private Transform controlsSameGameTabTransform;
    [SerializeField] private Transform controlsBuildingTetrisTabTransform;
    [SerializeField] private Transform controlsSuikaTetrisTabTransform;
    [SerializeField] private Transform recordsSameGameTabTransform;
    [SerializeField] private Transform recordsBuildingTetrisTabTransform;
    [SerializeField] private Transform recordsSuikaTetrisTabTransform;


    private TabTypes currentTabType = TabTypes.GameInfo;
    private GameTabTypes currentGameTabType = GameTabTypes.SameGame;

    #endregion

    #region Initialization

    private void Awake()
    {
        gameInfoTabButtonImage = gameInfoTabButton.GetComponent<Image>();
        gameControlsTabButtonImage = gameControlsTabButton.GetComponent<Image>();
        gameRecordsTabButtonImage = gameRecordsTabButton.GetComponent<Image>();

        sameGameTabButtonImage = sameGameTabButton.GetComponent<Image>();
        buildingTetrisTabButtonImage = buildingTetrisTabButton.GetComponent<Image>();
        suikaTetrisTabButtonImage = suikaTetrisTabButton.GetComponent<Image>();

        closeButton.onClick.AddListener(() =>
        {
            Hide();
            OnTabClosed?.Invoke(this, EventArgs.Empty);
        });

        gameInfoTabButton.onClick.AddListener(() =>
        {
            currentTabType = TabTypes.GameInfo;
            ChangeToActiveTab();
        });
        gameControlsTabButton.onClick.AddListener(() =>
        {
            currentTabType = TabTypes.GameControls;
            ChangeToActiveTab();
        });
        gameRecordsTabButton.onClick.AddListener(() =>
        {
            currentTabType = TabTypes.GameRecords;
            ChangeToActiveTab();
        });

        sameGameTabButton.onClick.AddListener(() =>
        {
            currentGameTabType = GameTabTypes.SameGame;
            ChangeToActiveTab();
        });
        buildingTetrisTabButton.onClick.AddListener(() =>
        {
            currentGameTabType = GameTabTypes.BuildingTetris;
            ChangeToActiveTab();
        });
        suikaTetrisTabButton.onClick.AddListener(() =>
        {
            currentGameTabType = GameTabTypes.SuikaTetris;
            ChangeToActiveTab();
        });
    }

    private void Start()
    {
        ChooseMinigameUI.OnInfoButtonPressed += ChooseMinigameUI_OnInfoButtonPressed;

        Hide();
    }

    private void ChooseMinigameUI_OnInfoButtonPressed(object sender, EventArgs e)
    {
        Show();
    }

    #endregion

    #region Visual

    private void Show()
    {
        gameObject.SetActive(true);

        currentTabType = TabTypes.GameInfo;
        currentGameTabType = GameTabTypes.SameGame;
        ChangeToActiveTab();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ChangeToActiveTab()
    {
        ResetAllActiveTabButtons();
        ResetAllActiveTabs();

        switch (currentTabType)
        {
            default:
            case TabTypes.GameInfo:
                ChangeActiveTabButtonImage(gameInfoTabButtonImage);
                switch (currentGameTabType)
                {
                    default:
                    case GameTabTypes.SameGame:
                        ChangeActiveTabButtonImage(sameGameTabButtonImage);
                        gameInfoSameGameTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.BuildingTetris:
                        ChangeActiveTabButtonImage(buildingTetrisTabButtonImage);
                        gameInfoBuildingTetrisTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.SuikaTetris:
                        ChangeActiveTabButtonImage(suikaTetrisTabButtonImage);
                        gameInfoSuikaTetrisTabTransform.gameObject.SetActive(true);
                        break;
                }

                break;
            case TabTypes.GameControls:
                ChangeActiveTabButtonImage(gameControlsTabButtonImage);
                switch (currentGameTabType)
                {
                    default:
                    case GameTabTypes.SameGame:
                        ChangeActiveTabButtonImage(sameGameTabButtonImage);
                        controlsSameGameTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.BuildingTetris:
                        ChangeActiveTabButtonImage(buildingTetrisTabButtonImage);
                        controlsBuildingTetrisTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.SuikaTetris:
                        ChangeActiveTabButtonImage(suikaTetrisTabButtonImage);
                        controlsSuikaTetrisTabTransform.gameObject.SetActive(true);
                        break;
                }

                break;
            case TabTypes.GameRecords:
                ChangeActiveTabButtonImage(gameRecordsTabButtonImage);
                switch (currentGameTabType)
                {
                    default:
                    case GameTabTypes.SameGame:
                        ChangeActiveTabButtonImage(sameGameTabButtonImage);
                        recordsSameGameTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.BuildingTetris:
                        ChangeActiveTabButtonImage(buildingTetrisTabButtonImage);
                        recordsBuildingTetrisTabTransform.gameObject.SetActive(true);
                        break;
                    case GameTabTypes.SuikaTetris:
                        ChangeActiveTabButtonImage(suikaTetrisTabButtonImage);
                        recordsSuikaTetrisTabTransform.gameObject.SetActive(true);
                        break;
                }

                break;
        }
    }

    private void ChangeActiveTabButtonImage(Image newActiveTab)
    {
        newActiveTab.sprite = activeTabButtonSprite;
    }

    private void ResetAllActiveTabButtons()
    {
        gameInfoTabButtonImage.sprite = notActiveTabButtonSprite;
        gameControlsTabButtonImage.sprite = notActiveTabButtonSprite;
        gameRecordsTabButtonImage.sprite = notActiveTabButtonSprite;

        sameGameTabButtonImage.sprite = notActiveTabButtonSprite;
        buildingTetrisTabButtonImage.sprite = notActiveTabButtonSprite;
        suikaTetrisTabButtonImage.sprite = notActiveTabButtonSprite;
    }

    private void ResetAllActiveTabs()
    {
        gameInfoSameGameTabTransform.gameObject.SetActive(false);
        gameInfoBuildingTetrisTabTransform.gameObject.SetActive(false);
        gameInfoSuikaTetrisTabTransform.gameObject.SetActive(false);
        controlsSameGameTabTransform.gameObject.SetActive(false);
        controlsBuildingTetrisTabTransform.gameObject.SetActive(false);
        controlsSuikaTetrisTabTransform.gameObject.SetActive(false);
        recordsSameGameTabTransform.gameObject.SetActive(false);
        recordsBuildingTetrisTabTransform.gameObject.SetActive(false);
        recordsSuikaTetrisTabTransform.gameObject.SetActive(false);
    }

    #endregion
}
