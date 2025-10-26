using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesListUI : MonoBehaviour
{
    [System.Serializable]
    public class EnemySlot
    {
        public TextMeshProUGUI costText;
        public Image background;
        public Image cooldownFill; // ← новое поле для отображения кулдауна
    }
    
    public List<EnemySlot> enemies;
}