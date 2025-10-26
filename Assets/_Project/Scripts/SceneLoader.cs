using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Перезагружаем текущую сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}