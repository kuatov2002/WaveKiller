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

        string bestTimeStr = UserDataService.GetCachedCustomUserData("bestRaceTime");
        float bestTime = float.MaxValue;

        // 🔹 Используем InvariantCulture для согласованности с записью
        if (!string.IsNullOrEmpty(bestTimeStr) && 
            float.TryParse(bestTimeStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedBest))
        {
            bestTime = parsedBest;
        }

        bool isNewRecord = currentTime < bestTime;
        float newBest = isNewRecord ? currentTime : bestTime;

        // Обновляем данные на сервере
        StaticEvents.GiveReward(); // ← он пересчитает то же самое (если логика совпадает)

        // Отображаем на основе наших локальных вычислений (согласованных с сохранением)
        string formattedBestTime = FormatTime(newBest);
        float remaining = Mathf.Max(0f, 360f - currentTime);
        int reward = Mathf.FloorToInt(remaining);

        string recordMessage = isNewRecord 
            ? "\n<b>New Record!</b>" 
            : $"\nBest time: {formattedBestTime}";

        timeText.SetText(
            $"Your race time is {formattedCurrentTime}\n" +
            $"You won {reward} coins" +
            recordMessage
        );
    }

    private string FormatTime(float totalSeconds)
    {
        if (totalSeconds <= 0f || totalSeconds >= 3600f)
            return "--:--";

        int totalSecondsInt = Mathf.FloorToInt(totalSeconds);
        int minutes = totalSecondsInt / 60;
        int seconds = totalSecondsInt % 60;
        return $"{minutes}:{seconds:D2}";
    }
}