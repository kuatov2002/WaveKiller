using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private bool forWin;
    private CanvasGroup canvasGroup; // Используется для плавного появления (fade)

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is missing on " + gameObject.name);
            return;
        }

        // Скрываем UI при старте
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Подписываемся на событие окончания игры
        StaticEvents.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        // Отписываемся, чтобы избежать утечек памяти
        StaticEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(bool isWin)
    {
        // Показываем UI только если он соответствует результату (win/lose)
        if (isWin == forWin)
        {
            StartCoroutine(FadeIn());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float duration = 1f; // Длительность fadeIn в секундах
        float elapsed = 0f;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}