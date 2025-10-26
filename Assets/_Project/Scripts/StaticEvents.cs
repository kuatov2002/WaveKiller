using System;

public static class StaticEvents
{
    public static Action<bool> OnGameOver = delegate { };
    public static bool IsGameOver;
}