using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public GameObject prefab;
    public int cost;
    public float spawnCooldown = 0.5f; // индивидуальный кулдаун для этого типа врага
}

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI moneyText;
    public EnemiesListUI enemiesListUI;
    
    [Header("Enemies")]
    public List<EnemyData> enemies = new();
    public int money = 100;

    [Header("Raycast / Placement")]
    public LayerMask placementMask = ~0;

    private int selectedEnemyIndex = 0;
    private Dictionary<GameObject, float> lastSpawnTimeByPrefab = new();

    private void Start()
    {
        moneyText.text = $"Money: {money}";
        for (int i = 0; i < enemies.Count; i++)
        {
            enemiesListUI.enemies[i].costText.SetText(enemies[i].cost.ToString());
        }

        // Инициализируем словарь с прошлым временем для всех префабов
        foreach (var enemy in enemies)
        {
            lastSpawnTimeByPrefab[enemy.prefab] = -999f;
        }

        UpdateVisuals();
    }

    void Update()
    {
        HandleEnemySelection();
        UpdateCooldownVisuals(); // ← добавь эту строку

        if (Input.GetMouseButtonDown(0))
        {
            TryRegisterClick();
        }
    }

    void UpdateCooldownVisuals()
    {
        for (int i = 0; i < enemies.Count && i < enemiesListUI.enemies.Count; i++)
        {
            EnemyData enemyData = enemies[i];
            EnemiesListUI.EnemySlot slot = enemiesListUI.enemies[i];
        
            float lastSpawn = lastSpawnTimeByPrefab[enemyData.prefab];
            float timeSinceSpawn = Time.time - lastSpawn;
            float cooldown = enemyData.spawnCooldown;

            // Если кулдаун активен — отображаем прогресс
            if (timeSinceSpawn < cooldown)
            {
                // fillAmount = сколько времени ОСТАЛОСЬ / полный кулдаун → но мы хотим "заполнять от 1 к 0"
                // Чтобы визуально было: полный круг = кулдаун активен, пустой = готов
                // Поэтому: fillAmount = (cooldown - timeSinceSpawn) / cooldown
                slot.cooldownFill.fillAmount = (cooldown - timeSinceSpawn) / cooldown;
                slot.cooldownFill.gameObject.SetActive(true);
            }
            else
            {
                slot.cooldownFill.fillAmount = 0f;
                slot.cooldownFill.gameObject.SetActive(false); // можно скрыть, если не нужен пустой индикатор
            }
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemiesListUI.enemies[i].background.color = i == selectedEnemyIndex ? Color.white : Color.black;
        }
    }

    void HandleEnemySelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && enemies.Count > 0)
        {
            selectedEnemyIndex = 0;
            Debug.Log($"[EnemyManager] Selected enemy: {enemies[0].prefab.name} (Cost: {enemies[0].cost})");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && enemies.Count > 1)
        {
            selectedEnemyIndex = 1;
            Debug.Log($"[EnemyManager] Selected enemy: {enemies[1].prefab.name} (Cost: {enemies[1].cost})");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && enemies.Count > 2)
        {
            selectedEnemyIndex = 2;
            Debug.Log($"[EnemyManager] Selected enemy: {enemies[2].prefab.name} (Cost: {enemies[2].cost})");
        }
        
        UpdateVisuals();
    }

    void TryRegisterClick()
    {
        EnemyData selectedEnemy = enemies[selectedEnemyIndex];
        GameObject prefab = selectedEnemy.prefab;

        // Проверка кулдауна для конкретного врага
        float lastTime = lastSpawnTimeByPrefab[prefab];
        if (Time.time - lastTime < selectedEnemy.spawnCooldown)
        {
            Debug.Log($"[EnemyManager] Spawn for {prefab.name} is on cooldown.");
            return;
        }

        if (money < selectedEnemy.cost)
        {
            Debug.Log($"[EnemyManager] Not enough money to spawn {prefab.name}. Need {selectedEnemy.cost}, have {money}.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementMask))
        {
            Vector3 pos = hit.point;

            // Спавним врага
            GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
            SpendMoney(selectedEnemy.cost);
            lastSpawnTimeByPrefab[prefab] = Time.time; // обновляем время последнего спавна именно для этого префаба
            Debug.Log($"[EnemyManager] Spawned {enemy.name} at {pos}. Money left: {money}");
        }
        else
        {
            Debug.Log("[EnemyManager] Click did not hit placement mask.");
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = $"Money: {money}";
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        moneyText.text = $"Money: {money}";
    }
}