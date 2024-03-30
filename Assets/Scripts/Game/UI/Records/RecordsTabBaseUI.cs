using UnityEngine;

public class RecordsTabBaseUI : MonoBehaviour
{
    [SerializeField] private MinigameRecords.MinigameTypes minigameType = MinigameRecords.MinigameTypes.SameGame;
    [SerializeField] private int maxDisplayedRecords = 15;
    [SerializeField] private RecordSingle recordPrefab;
    [SerializeField] private Transform winStreakRecordsGrid;
    [SerializeField] private Transform bestTimeRecordsGrid;

    private void UpdateRecords()
    {
        var allWinStreaksRecordsTransforms = winStreakRecordsGrid.GetComponentsInChildren<Transform>();

        foreach (var recordsTransform in allWinStreaksRecordsTransforms)
        {
            if (recordsTransform == winStreakRecordsGrid || recordsTransform == recordPrefab.transform) continue;

            Destroy(recordsTransform.gameObject);
        }

        var allBestTimeRecordsTransforms = bestTimeRecordsGrid.GetComponentsInChildren<Transform>();

        foreach (var recordsTransform in allBestTimeRecordsTransforms)
        {
            if (recordsTransform == bestTimeRecordsGrid || recordsTransform == recordPrefab.transform) continue;

            Destroy(recordsTransform.gameObject);
        }

        var bestTimeRecords =
            MinigameRecords.Instance.GetPlayerRecords(minigameType, MinigameRecords.RecordType.BestTime,
                maxDisplayedRecords);
        var winStreakRecords = MinigameRecords.Instance.GetPlayerRecords(minigameType,
            MinigameRecords.RecordType.WinStreak, maxDisplayedRecords);

        foreach (var bestTimeRecord in bestTimeRecords)
        {
            if (bestTimeRecord.playerRecord <= 0) break;

            var newRecordTransform = Instantiate(recordPrefab, bestTimeRecordsGrid);
            var newRecordSingle = newRecordTransform.GetComponent<RecordSingle>();
            newRecordSingle.InitializeRecord(bestTimeRecord.playerName, bestTimeRecord.playerRecord);
        }

        foreach (var winStreakRecord in winStreakRecords)
        {
            if (winStreakRecord.playerRecord <= 0) break;

            var newRecordTransform = Instantiate(recordPrefab, winStreakRecordsGrid);
            var newRecordSingle = newRecordTransform.GetComponent<RecordSingle>();
            newRecordSingle.InitializeRecord(winStreakRecord.playerName, winStreakRecord.playerRecord);
        }
    }

    protected void Show()
    {
        gameObject.SetActive(true);

        UpdateRecords();
    }

    protected void Hide()
    {
        gameObject.SetActive(false);
    }
}
