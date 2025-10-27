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
        // Вычисляем оставшееся время как float
        float remaining = Mathf.Max(0f, 360f - time);

        // Округляем в int — можно FloorToInt, RoundToInt или CeilToInt в зависимости от желаемого поведения.
        int reward = Mathf.FloorToInt(remaining); // например, округляем вниз

        ClaimRewardSystem.ClaimCoinReward(reward, 1);
    }
}