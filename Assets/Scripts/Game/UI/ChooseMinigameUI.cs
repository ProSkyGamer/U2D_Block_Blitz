using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMinigameUI : MonoBehaviour
{
    #region Events

    public static event EventHandler OnPlayCandyCrushButtonPressed;
    public static event EventHandler OnPlayBuildingTetrisButtonPressed;
    public static event EventHandler OnPlaySuikaTetrisButtonPressed;

    #endregion

    #region Variables & References

    [SerializeField] private Button playCandyCrushButton;
    [SerializeField] private Button playTetrisButton;
    [SerializeField] private Button playTetrisSuikaButton;

    #endregion

    #region Intialization

    private void Awake()
    {
        playCandyCrushButton.onClick.AddListener(() =>
        {
            OnPlayCandyCrushButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        playTetrisButton.onClick.AddListener(() =>
        {
            OnPlayBuildingTetrisButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        playTetrisSuikaButton.onClick.AddListener(() =>
        {
            OnPlaySuikaTetrisButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
    }

    private void Start()
    {
        BuildingTetrisUI.OnTetrisGameClose += TetrisMinigameController_OnTetrisGameClose;
        SuikaTetrisUI.OnTetrisGameClose += SuikaMinigameControllerOnOnTetrisGameClose;
        CandyCrushUI.OnCandyCrushGameClose += CandyCrushUI_OnCandyCrushGameClose;
    }

    private void CandyCrushUI_OnCandyCrushGameClose(object sender, EventArgs e)
    {
        Show();
    }

    private void SuikaMinigameControllerOnOnTetrisGameClose(object sender, EventArgs e)
    {
        Show();
    }

    private void TetrisMinigameController_OnTetrisGameClose(object sender, EventArgs e)
    {
        Show();
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