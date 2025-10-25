using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public GameObject prefab;
    public int cost;
}

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI moneyText;
    public EnemiesListUI enemiesListUI;
    
    [Header("Enemies")]
    public List<EnemyData> enemies = new(); // Теперь каждая запись содержит префаб + стоимость
    public int money = 100; // начальные деньги (можно настроить)

    [Header("Raycast / Placement")]
    public LayerMask placementMask = ~0;

    [Header("Marker (runtime)")]
    public float markerLifetime = 3f;
    public float markerScale = 0.35f;
    public Color markerColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Gizmos (editor)")]
    public bool drawGizmos = true;
    public Color gizmoColor = new Color(1f, 0.6f, 0.0f, 0.8f);
    public float gizmoRadius = 0.4f;

    private int selectedEnemyIndex = 0;
    private List<Vector3> clickedPositions = new();

    private void Start()
    {
        moneyText.text = $"Money: {money}";
        for (int i = 0; i < enemies.Count; i++)
        {
            enemiesListUI.enemies[i].costText.SetText(enemies[i].cost.ToString());
        }

        UpdateVisuals();
    }

    void Update()
    {
        HandleEnemySelection();

        if (Input.GetMouseButtonDown(0))
        {
            TryRegisterClick();
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
        int currentCost = enemies[selectedEnemyIndex].cost;
        if (money < currentCost)
        {
            Debug.Log($"[EnemyManager] Not enough money to spawn {enemies[selectedEnemyIndex].prefab.name}. Need {currentCost}, have {money}.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementMask))
        {
            Vector3 pos = hit.point;
            clickedPositions.Add(pos);

            // Спавним врага
            GameObject enemy = Instantiate(enemies[selectedEnemyIndex].prefab, pos, Quaternion.identity);
            SpendMoney(currentCost);
            Debug.Log($"[EnemyManager] Spawned {enemy.name} at {pos}. Money left: {money}");

            CreateRuntimeMarker(pos);
        }
        else
        {
            Debug.Log("[EnemyManager] Click did not hit placement mask.");
        }
    }

    void CreateRuntimeMarker(Vector3 pos)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "EM_Marker";
        marker.transform.position = pos;
        marker.transform.localScale = Vector3.one * markerScale;

        var col = marker.GetComponent<Collider>();
        if (col != null) Destroy(col);

        var rend = marker.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = markerColor,
                enableInstancing = true
            };
            rend.material = mat;
        }

        Destroy(marker, markerLifetime);
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

    public void ClearClickedPositions()
    {
        clickedPositions.Clear();
    }
}