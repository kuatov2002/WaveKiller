using System;

public static class StaticEvents
{
    public static Action<bool> OnGameOver;
    public static bool IsGameOver;
}