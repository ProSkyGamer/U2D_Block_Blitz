using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMinigameUI : MonoBehaviour
{
    public static event EventHandler OnPlayCandyCrushButtonPressed;
    public static event EventHandler OnPlayTetrisButtonPressed;
    public static event EventHandler OnPlayTetrisSuikaButtonPressed;

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
            OnPlayTetrisButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        playTetrisSuikaButton.onClick.AddListener(() =>
        {
            OnPlayTetrisSuikaButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
    }

    private void Start()
    {
        TetrisMinigameController.OnTetrisGameClose += TetrisMinigameController_OnTetrisGameClose;
        SuikaMinigameController.OnTetrisGameClose += SuikaMinigameControllerOnOnTetrisGameClose;
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
