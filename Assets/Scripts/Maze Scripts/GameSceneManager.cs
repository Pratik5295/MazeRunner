using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void GoToGameScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void GoToGameMenu()
    {
        SceneManager.LoadScene(0);
    }
}
