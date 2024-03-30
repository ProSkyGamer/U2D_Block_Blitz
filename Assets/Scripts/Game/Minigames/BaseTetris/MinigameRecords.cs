using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinigameRecords : MonoBehaviour
{
    public static MinigameRecords Instance { get; private set; }

    public enum RecordType
    {
        WinStreak,
        BestTime
    }

    public enum MinigameTypes
    {
        SameGame,
        BuildingTetris,
        SuikaTetris
    }

    [Serializable]
    public class FollowingMinigame
    {
        public MinigameBase followingMinigame;
        public MinigameTypes followingMinigameType;
    }

    public class PlayerRecord
    {
        public string playerName;
        public int playerRecord;
    }

    [SerializeField] private TMP_InputField currentPlayerName;
    [SerializeField] private List<FollowingMinigame> allFollowingMinigames;
    [SerializeField] private int storedTopCount = 10;
    private const string FOLLOWING_MINIGAME_WIN_STREAK_RECORD_BASE_PLAYER_PREFS = "Minigame_Win_Streak_Record";

    private const string FOLLOWING_MINIGAME_WIN_STREAK_PLAYER_NAME_RECORD_BASE_PLAYER_PREFS =
        "Minigame_Win_Streak_Player_Name_Record";

    private const string CURRENT_WIN_STREAK_RECORD_BASE_PLAYER_PREFS = "Minigame_Win_Streak_Current_Stored_Record";

    private const string FOLLOWING_MINIGAME_BEST_TIME_RECORD_BASE_PLAYER_PREFS = "Minigame_Best_Time_Record";

    private const string FOLLOWING_MINIGAME_BEST_TIME_PLAYER_NAME_RECORD_BASE_PLAYER_PREFS =
        "Minigame_Best_Time_Player_Name_Record";

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        foreach (var followingMinigame in allFollowingMinigames)
            followingMinigame.followingMinigame.OnGameOver += FollowingMinigame_OnGameOver;
    }

    private void FollowingMinigame_OnGameOver(object sender, MinigameBase.OnGameOverEventArgs e)
    {
        var endedGame = sender as MinigameBase;

        if (endedGame == null) return;

        var followingMinigameName = "";
        foreach (var followingMinigame in allFollowingMinigames)
        {
            if (followingMinigame.followingMinigame != endedGame) continue;

            followingMinigameName = followingMinigame.followingMinigameType.ToString();
        }

        if (followingMinigameName == "")
            endedGame.OnGameOver -= FollowingMinigame_OnGameOver;

        var currentMinigameRecordPlayerPrefsKey =
            GetPlayerPrefsCurrentWinStreakRecordKeyByMinigameName(followingMinigameName);

        var gameResult = e.gameOverReason;

        if (gameResult == MinigameBase.GameOverReason.Win)
        {
            PlayerPrefs.SetInt(currentMinigameRecordPlayerPrefsKey,
                PlayerPrefs.GetInt(currentMinigameRecordPlayerPrefsKey) + 1);

            TryStoreRecord(RecordType.BestTime, followingMinigameName, e.completionTime);
        }
        else
        {
            TryStoreRecord(RecordType.WinStreak, followingMinigameName,
                PlayerPrefs.GetInt(currentMinigameRecordPlayerPrefsKey));
            PlayerPrefs.SetInt(currentMinigameRecordPlayerPrefsKey, 0);
        }
    }

    private void TryStoreRecord(RecordType recordType, string minigameName, int newRecord)
    {
        if (newRecord <= 0) return;


        var tempScore = -1;
        var tempName = "";
        for (var i = 0; i < storedTopCount; i++)
        {
            if (tempScore == 0) break;

            var playerPrefsRecordKey = "";
            var playerPrefsRecordNameKey = "";
            if (recordType == RecordType.WinStreak)
                playerPrefsRecordKey = GetPlayerPrefsWinStreakRecordKeyByMinigameName(minigameName, i);
            else if (recordType == RecordType.BestTime)
                playerPrefsRecordKey = GetPlayerPrefsBestTimeRecordKeyByMinigameName(minigameName, i);

            if (tempScore > 0)
            {
                if (recordType == RecordType.WinStreak)
                    playerPrefsRecordNameKey =
                        GetPlayerPrefsWinStreakPlayerNameRecordKeyByMinigameName(minigameName, i);
                else if (recordType == RecordType.BestTime)
                    playerPrefsRecordNameKey = GetPlayerPrefsBestTimePlayerNameRecordKeyByMinigameName(minigameName, i);


                var veryTempScore = PlayerPrefs.GetInt(playerPrefsRecordKey);
                var veryTempName = PlayerPrefs.GetString(playerPrefsRecordNameKey);
                PlayerPrefs.SetInt(playerPrefsRecordKey, tempScore);
                PlayerPrefs.SetString(playerPrefsRecordNameKey, tempName);
                tempScore = veryTempScore;
                tempName = veryTempName;
                continue;
            }

            if (PlayerPrefs.GetInt(playerPrefsRecordKey) > newRecord) continue;

            if (recordType == RecordType.WinStreak)
                playerPrefsRecordNameKey = GetPlayerPrefsWinStreakPlayerNameRecordKeyByMinigameName(minigameName, i);
            else if (recordType == RecordType.BestTime)
                playerPrefsRecordNameKey = GetPlayerPrefsBestTimePlayerNameRecordKeyByMinigameName(minigameName, i);

            tempScore = PlayerPrefs.GetInt(playerPrefsRecordKey);
            tempName = PlayerPrefs.GetString(playerPrefsRecordNameKey);
            PlayerPrefs.SetInt(playerPrefsRecordKey, newRecord);
            PlayerPrefs.SetString(playerPrefsRecordNameKey, currentPlayerName.text);
        }
    }

    private string GetPlayerPrefsWinStreakRecordKeyByMinigameName(string minigameName, int place)
    {
        return $"{FOLLOWING_MINIGAME_WIN_STREAK_RECORD_BASE_PLAYER_PREFS}_{minigameName}_{place}";
    }

    private string GetPlayerPrefsCurrentWinStreakRecordKeyByMinigameName(string minigameName)
    {
        return $"{CURRENT_WIN_STREAK_RECORD_BASE_PLAYER_PREFS}_{minigameName}";
    }

    private string GetPlayerPrefsBestTimeRecordKeyByMinigameName(string minigameName, int place)
    {
        return $"{FOLLOWING_MINIGAME_BEST_TIME_RECORD_BASE_PLAYER_PREFS}_{minigameName}_{place}";
    }

    private string GetPlayerPrefsWinStreakPlayerNameRecordKeyByMinigameName(string minigameName, int place)
    {
        return $"{FOLLOWING_MINIGAME_WIN_STREAK_PLAYER_NAME_RECORD_BASE_PLAYER_PREFS}_{minigameName}_{place}";
    }

    private string GetPlayerPrefsBestTimePlayerNameRecordKeyByMinigameName(string minigameName, int place)
    {
        return $"{FOLLOWING_MINIGAME_BEST_TIME_PLAYER_NAME_RECORD_BASE_PLAYER_PREFS}_{minigameName}_{place}";
    }

    public List<PlayerRecord> GetPlayerRecords(MinigameTypes minigameType, RecordType recordType, int numberOfPlaces)
    {
        var recordList = new List<PlayerRecord>();

        var minigameName = minigameType.ToString();

        for (var i = 0; i < numberOfPlaces; i++)
        {
            var playerPrefsRecordKey = "";
            var playerPrefsRecordNameKey = "";
            if (recordType == RecordType.WinStreak)
            {
                playerPrefsRecordKey = GetPlayerPrefsWinStreakRecordKeyByMinigameName(minigameName, i);
                playerPrefsRecordNameKey = GetPlayerPrefsWinStreakPlayerNameRecordKeyByMinigameName(minigameName, i);
            }
            else if (recordType == RecordType.BestTime)
            {
                playerPrefsRecordKey = GetPlayerPrefsBestTimeRecordKeyByMinigameName(minigameName, i);
                playerPrefsRecordNameKey = GetPlayerPrefsBestTimePlayerNameRecordKeyByMinigameName(minigameName, i);
            }

            recordList.Add(new PlayerRecord
            {
                playerName = PlayerPrefs.GetString(playerPrefsRecordNameKey),
                playerRecord = PlayerPrefs.GetInt(playerPrefsRecordKey)
            });
        }

        Debug.Log(recordList[1].playerRecord);
        return recordList;
    }
}
