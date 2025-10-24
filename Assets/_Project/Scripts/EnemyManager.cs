using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemies")]
    public List<GameObject> enemyPrefabs = new List<GameObject>(); // Сюда перетащить префабы врагов в инспекторе

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
    private List<Vector3> clickedPositions = new List<Vector3>();

    void Update()
    {
        HandleEnemySelection();
        
        if (Input.GetMouseButtonDown(0))
        {
            TryRegisterClick();
        }
    }

    void HandleEnemySelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && enemyPrefabs.Count > 0)
        {
            selectedEnemyIndex = 0;
            Debug.Log($"[EnemyManager] Selected enemy: {enemyPrefabs[0].name}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && enemyPrefabs.Count > 1)
        {
            selectedEnemyIndex = 1;
            Debug.Log($"[EnemyManager] Selected enemy: {enemyPrefabs[1].name}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && enemyPrefabs.Count > 2)
        {
            selectedEnemyIndex = 2;
            Debug.Log($"[EnemyManager] Selected enemy: {enemyPrefabs[2].name}");
        }
    }

    void TryRegisterClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementMask))
        {
            Vector3 pos = hit.point;
            clickedPositions.Add(pos);

            // Спавним врага
            GameObject enemy = Instantiate(enemyPrefabs[selectedEnemyIndex], pos, Quaternion.identity);
            Debug.Log($"[EnemyManager] Spawned {enemy.name} at {pos}");

            // Опционально: создаём маркер (можно убрать, если не нужен)
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

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!drawGizmos) return;
        Gizmos.color = gizmoColor;
        foreach (var p in clickedPositions)
        {
            Gizmos.DrawSphere(p, gizmoRadius);
            Gizmos.DrawLine(p, p + Vector3.up * 0.4f);
        }
#endif
    }

    public void ClearClickedPositions()
    {
        clickedPositions.Clear();
    }
}