using TMPro;
using UnityEngine;

public class RecordSingle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void InitializeRecord(string playerName, int score)
    {
        playerNameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
