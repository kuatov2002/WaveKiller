using System.Globalization;
using IDosGames;
using TMPro;
using UnityEngine;

public class IdosCoinsFinish : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    
    private void Start()
    {
        StaticEvents.OnGameOver += Finish;
    }

    private void OnDestroy()
    {
        StaticEvents.OnGameOver -= Finish;
    }

    public void Finish(bool isWin)
    {
        if (!isWin) return;
        
        float currentTime = StaticEvents.EndTimer();
        string formattedCurrentTime = FormatTime(currentTime);

        // Получаем текущий рекорд
        string bestTimeStr = UserDataService.GetCachedCustomUserData("bestRaceTime");
        float bestTime = float.MaxValue;

        if (!string.IsNullOrEmpty(bestTimeStr) && float.TryParse(bestTimeStr, out float parsedBest))
        {
            bestTime = parsedBest;
        }

        // Проверяем, побит ли рекорд
        bool isNewRecord = currentTime < bestTime;

        // Обновляем рекорд через StaticEvents (он сам обновит данные)
        StaticEvents.GiveReward();

        // После обновления получаем актуальный рекорд (на случай, если он только что установлен)
        bestTimeStr = UserDataService.GetCachedCustomUserData("bestRaceTime");
        if (!string.IsNullOrEmpty(bestTimeStr) && float.TryParse(bestTimeStr, out float updatedBest))
        {
            bestTime = updatedBest;
        }

        string formattedBestTime = FormatTime(bestTime);

        float remaining = Mathf.Max(0f, 360f - currentTime);
        int reward = Mathf.FloorToInt(remaining);

        string recordMessage = isNewRecord ? "\n<b>New Record!</b>" : $"\nBest time: {formattedBestTime}";

        timeText.SetText(
            $"Your race time is {formattedCurrentTime}\n" +
            $"You won {reward} coins" +
            recordMessage
        );
    }

    private string FormatTime(float totalSeconds)
    {
        // Защита от некорректных значений (например, float.MaxValue)
        if (totalSeconds >= 3600f) // больше часа — не отображаем
            return "--:--";

        int totalSecondsInt = Mathf.FloorToInt(totalSeconds);
        int minutes = totalSecondsInt / 60;
        int seconds = totalSecondsInt % 60;
        return $"{minutes}:{seconds:D2}";
    }
}