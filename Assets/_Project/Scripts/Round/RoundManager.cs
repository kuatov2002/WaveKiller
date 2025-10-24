using System;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("References")] 
    public TextMeshProUGUI timer;
    
    [Space(10)]
    public int money;
    public float timeToLose; // время в секундах до проигрыша

    private float currentTime;

    private void Start()
    {
        ResetTimer();
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SpendMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
    }

    public void ResetTimer()
    {
        currentTime = timeToLose;
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            currentTime = 0;
            timer.SetText("0:00");
            OnTimeRanOut();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timer.SetText($"{minutes}:{seconds:D2}");
    }

    private void OnTimeRanOut()
    {
        Debug.Log("Время вышло! Игра окончена.");
    }
}