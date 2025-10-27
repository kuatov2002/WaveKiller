using System;
using IDosGames;
using UnityEngine;

public static class StaticEvents
{
    public static Action<bool> OnGameOver = delegate { };
    public static bool IsGameOver;

    public static float time;

    public static void StartTimer()
    {
        time = Time.time;
    }

    public static float EndTimer()
    {
        time = Time.time - time;
        return time;
    }

    public static void GiveReward()
    {
        float remaining = Mathf.Max(0f, 360f - time);
        int reward = Mathf.FloorToInt(remaining);

        ClaimRewardSystem.ClaimCoinReward(reward, 0);
        UserDataService.UpdateCustomUserData("lastRaceTime", time);
        UserDataService.UpdateCustomUserData("lastRaceCoins", reward);

        // Получаем текущий лучший результат
        float currentBest = float.MaxValue;
        string bestTimeStr = UserDataService.GetCachedCustomUserData("bestRaceTime");
        if (!string.IsNullOrEmpty(bestTimeStr) && float.TryParse(bestTimeStr, out float parsed))
            currentBest = parsed;

        float newBest = Mathf.Min(currentBest, time);
        UserDataService.UpdateCustomUserData("bestRaceTime", newBest);
    }
}