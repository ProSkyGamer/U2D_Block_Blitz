using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMinigameUI : MonoBehaviour
{
    public static event EventHandler OnPlayCandyCrushButtonPressed;
    public static event EventHandler OnPlayBuildingTetrisButtonPressed;
    public static event EventHandler OnPlaySuikaTetrisButtonPressed;

    [SerializeField] private Button playCandyCrushButton;
    [SerializeField] private Button playTetrisButton;
    [SerializeField] private Button playTetrisSuikaButton;

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
    }

    private void SuikaMinigameControllerOnOnTetrisGameClose(object sender, EventArgs e)
    {
        Show();
    }

    private void TetrisMinigameController_OnTetrisGameClose(object sender, EventArgs e)
    {
        Show();
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
