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
        string bestTimeStr = UserDataService.GetCachedCustomUserData("bestRaceTime");
        float bestTime = float.MaxValue;

        if (!string.IsNullOrEmpty(bestTimeStr) && float.TryParse(bestTimeStr, out float parsedBest))
        {
            bestTime = parsedBest;
        }

        // Новый лучший результат — минимальное из старого и текущего
        float newBest = Mathf.Min(bestTime, time);

        UserDataService.UpdateCustomUserData("bestRaceTime", newBest);
    }
}