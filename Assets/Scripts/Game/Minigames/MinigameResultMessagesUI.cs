using System;
using UnityEngine;
using UnityEngine.UI;

public class MinigameResultMessagesUI : MonoBehaviour
{
    #region Events

    public static event EventHandler OnMessageClosed;

    #endregion

    #region Variables & References

    [SerializeField] private MinigameBase trackingMinigame;

    [SerializeField] private Transform loseTimeOutMessageTransform;
    [SerializeField] private Transform loseFailedMessageTransform;
    [SerializeField] private Transform winMessageTransform;

    [SerializeField] private Button closeMessageButton;

    #endregion

    #region Initialization

    private void Awake()
    {
        closeMessageButton.onClick.AddListener(() =>
        {
            OnMessageClosed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
    }

    private void Start()
    {
        trackingMinigame.OnGameOver += TrackingMinigame_OnGameOver;

        Hide();
    }

    private void TrackingMinigame_OnGameOver(object sender, MinigameBase.OnGameOverEventArgs e)
    {
        ShowMessage(e.gameOverReason);
    }

    #endregion

    #region Visual

    private void ShowMessage(MinigameBase.GameOverReason gameOverReason)
    {
        Show();

        switch (gameOverReason)
        {
            case MinigameBase.GameOverReason.TimeOut:
                loseTimeOutMessageTransform.gameObject.SetActive(true);
                break;
            case MinigameBase.GameOverReason.Failed:
                loseFailedMessageTransform.gameObject.SetActive(true);
                break;
            case MinigameBase.GameOverReason.Win:
                winMessageTransform.gameObject.SetActive(true);
                break;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void HideAllMessages()
    {
        loseTimeOutMessageTransform.gameObject.SetActive(false);
        loseFailedMessageTransform.gameObject.SetActive(false);
        winMessageTransform.gameObject.SetActive(false);
    }

    private void Hide()
    {
        HideAllMessages();
        gameObject.SetActive(false);
    }

    #endregion
}
