using System.Globalization;
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
        
        float seconds = StaticEvents.EndTimer();
        string formattedTime = FormatTime(seconds);
        
        float remaining = Mathf.Max(0f, 360f - StaticEvents.time);

        // Округляем в int — можно FloorToInt, RoundToInt или CeilToInt в зависимости от желаемого поведения.
        int reward = Mathf.FloorToInt(remaining);
        
        timeText.SetText("Your race time is " + formattedTime+"\n"+"You won " + reward + " coins");
        StaticEvents.GiveReward();
    }

    private string FormatTime(float totalSeconds)
    {
        int totalSecondsInt = Mathf.FloorToInt(totalSeconds);
        int minutes = totalSecondsInt / 60;
        int seconds = totalSecondsInt % 60;
        return $"{minutes}:{seconds:D2}";
    }
}